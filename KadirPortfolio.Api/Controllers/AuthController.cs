using KadirPortfolio.Api.Models;
using KadirPortfolio.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace KadirPortfolio.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICaptchaService _captchaService;
        private readonly IAuditLogger _auditLogger;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, ICaptchaService captchaService, IAuditLogger auditLogger, IConfiguration config)
        {
            _authService = authService;
            _captchaService = captchaService;
            _auditLogger = auditLogger;
            _config = config;
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Honeypot Check (Bot Protection)
            if (!string.IsNullOrEmpty(request.UsernameHoneypot))
            {
                await _auditLogger.LogAsync("LOGIN_BOT_BLOCKED", "Bot detected via honeypot", HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"]);
                return BadRequest(new { success = false, message = "Geçersiz istek (Bot tespiti)." });
            }

            // 2. Captcha Verification
            var isCaptchaValid = await _captchaService.VerifyCaptchaAsync(request.CaptchaToken);
            if (!isCaptchaValid)
            {
                await _auditLogger.LogAsync("LOGIN_CAPTCHA_FAILED", "Invalid captcha token", HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"]);
                return BadRequest(new { success = false, message = "Captcha doğrulaması başarısız." });
            }

            // 3. User Credentials Verification
            var user = await _authService.AuthenticateAsync(request.Email, request.Password);
            if (user == null)
            {
                await _auditLogger.LogAsync("LOGIN_FAILED", $"Failed login attempt for {request.Email}", HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"]);
                return Unauthorized(new { success = false, message = "E-posta veya şifre hatalı." });
            }

            // 4. Anomaly Detection (Device/Location)
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = Request.Headers["User-Agent"].ToString() ?? "Unknown";
            var deviceHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(userAgent)));
            
            await _authService.TrackDeviceAsync(user, deviceHash, ipAddress);

            // 5. Bypass 2FA and directly login (Due to Google SMTP blocking Render IP)
            var jwtToken = await _authService.GenerateJwtTokenAsync(user, TimeSpan.FromMinutes(15));
            var refreshToken = await _authService.GenerateRefreshTokenAsync();
            var refreshTokenExpiry = request.RememberMe ? DateTime.UtcNow.AddDays(1) : DateTime.UtcNow.AddHours(2);

            await _authService.SaveRefreshTokenAsync(user, refreshToken, refreshTokenExpiry);
            SetRefreshTokenCookie(refreshToken, refreshTokenExpiry);

            await _auditLogger.LogAsync("LOGIN_SUCCESS", "Admin successfully logged in (2FA Bypassed)", ipAddress, userAgent, user.Email);

            return Ok(new { success = true, token = jwtToken });
        }

        [HttpPost("verify-2fa")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> Verify2Fa([FromBody] Verify2FaRequest request)
        {
            var isValid = await _authService.Verify2FaCodeAsync(request.Email, request.Code);
            if (!isValid)
            {
                return BadRequest(new { success = false, message = "Geçersiz veya süresi dolmuş kod." });
            }

            var user = await _authService.GetUserByEmailAsync(request.Email);
            if (user == null) return Unauthorized();

            var jwtToken = await _authService.GenerateJwtTokenAsync(user, TimeSpan.FromMinutes(15));
            var refreshToken = await _authService.GenerateRefreshTokenAsync();
            var refreshTokenExpiry = request.RememberMe ? DateTime.UtcNow.AddDays(1) : DateTime.UtcNow.AddHours(2);

            await _authService.SaveRefreshTokenAsync(user, refreshToken, refreshTokenExpiry);

            SetRefreshTokenCookie(refreshToken, refreshTokenExpiry);

            await _auditLogger.LogAsync("LOGIN_SUCCESS", "Admin successfully logged in", HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"], user.Email);

            return Ok(new { success = true, token = jwtToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken)) return Unauthorized(new { message = "Refresh token bulunamadı." });

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer ")) return Unauthorized();

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"] ?? "KadirPortfolio",
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"] ?? "KadirPortfolioUI",
                ValidateLifetime = false, // Accept expired token
                ValidAlgorithms = new[] { Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256 }
            };

            string userIdStr = null;
            try
            {
                var principal = handler.ValidateToken(token, validationParameters, out Microsoft.IdentityModel.Tokens.SecurityToken validatedToken);
                userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            }
            catch (Exception)
            {
                return Unauthorized(new { message = "Geçersiz erişim token'ı." });
            }

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var user = await _authService.ValidateRefreshTokenAsync(userId, refreshToken);
            if (user == null) return Unauthorized(new { message = "Geçersiz veya süresi dolmuş refresh token." });

            var newJwtToken = await _authService.GenerateJwtTokenAsync(user, TimeSpan.FromMinutes(15));
            var newRefreshToken = await _authService.GenerateRefreshTokenAsync();
            
            // Keep the same expiry duration
            var expiry = user.RefreshTokenExpiryTime ?? DateTime.UtcNow.AddDays(1);
            await _authService.SaveRefreshTokenAsync(user, newRefreshToken, expiry);

            SetRefreshTokenCookie(newRefreshToken, expiry);

            return Ok(new { success = true, token = newJwtToken });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            await _auditLogger.LogAsync("LOGOUT", "Admin logged out", HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"], email);

            return Ok(new { success = true });
        }

        private void SetRefreshTokenCookie(string token, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}
