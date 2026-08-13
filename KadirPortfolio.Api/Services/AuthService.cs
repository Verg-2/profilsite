using KadirPortfolio.Api.Data;
using KadirPortfolio.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace KadirPortfolio.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;

        public AuthService(AppDbContext context, IConfiguration config, IMemoryCache cache, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _cache = cache;
            _emailService = emailService;
        }

        public async Task<AdminUser?> AuthenticateAsync(string email, string password)
        {
            var user = await _context.AdminUsers.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            var hasher = new PasswordHasher<AdminUser>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            
            if (result == PasswordVerificationResult.Success)
            {
                return user;
            }
            return null;
        }

        public async Task<string> GenerateJwtTokenAsync(AdminUser user, TimeSpan expiration)
        {
            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("Kritik Hata: JWT Secret Key (.env / Config) tanımlanmamış!");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"] ?? "KadirPortfolio",
                audience: _config["Jwt:Audience"] ?? "KadirPortfolioUI",
                claims: claims,
                expires: DateTime.UtcNow.Add(expiration),
                signingCredentials: credentials);

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<string> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return await Task.FromResult(Convert.ToBase64String(randomNumber));
        }

        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        public async Task SaveRefreshTokenAsync(AdminUser user, string refreshToken, DateTime expiryTime)
        {
            user.RefreshToken = HashToken(refreshToken);
            user.RefreshTokenExpiryTime = expiryTime;
            _context.AdminUsers.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<AdminUser?> GetUserByEmailAsync(string email)
        {
            return await _context.AdminUsers.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<AdminUser?> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            string hashedInputToken = HashToken(refreshToken);
            return await _context.AdminUsers.FirstOrDefaultAsync(u => u.Id == userId && u.RefreshToken == hashedInputToken && u.RefreshTokenExpiryTime > DateTime.UtcNow);
        }

        public async Task GenerateAndSend2FaCodeAsync(AdminUser user)
        {
            var code = System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            
            _cache.Set($"2FA_{user.Email}", code, TimeSpan.FromMinutes(3));

            await _emailService.SendEmailAsync(user.Email, "Yönetim Paneli Giriş Kodu (2FA)", $"Giriş kodunuz: {code}. Bu kod 3 dakika geçerlidir.");
        }

        private static readonly object _2faLock = new object();

        public async Task<bool> Verify2FaCodeAsync(string email, string code)
        {
            string attemptsKey = $"2FA_Attempts_{email}";
            string codeKey = $"2FA_{email}";

            lock (_2faLock)
            {
                int attempts = _cache.TryGetValue(attemptsKey, out int currentAttempts) ? currentAttempts : 0;

                if (attempts >= 3)
                {
                    _cache.Remove(codeKey);
                    _cache.Remove(attemptsKey);
                    return false; // Doğrudan kilitle
                }

                if (_cache.TryGetValue(codeKey, out string cachedCode))
                {
                    if (cachedCode == code)
                    {
                        _cache.Remove(codeKey);
                        _cache.Remove(attemptsKey);
                        return true;
                    }

                    // Hatalı deneme sayısını kilitleyerek ve güvenli şekilde artırarak yaz
                    _cache.Set(attemptsKey, attempts + 1, TimeSpan.FromMinutes(3));
                }
            }
            return await Task.FromResult(false);
        }

        public async Task TrackDeviceAsync(AdminUser user, string deviceHash, string ipAddress)
        {
            var existingDevice = await _context.AdminDevices.FirstOrDefaultAsync(d => d.AdminUserId == user.Id && d.DeviceHash == deviceHash);
            
            if (existingDevice == null)
            {
                var newDevice = new AdminDevice
                {
                    AdminUserId = user.Id,
                    DeviceHash = deviceHash,
                    LastIpAddress = ipAddress,
                    LastLoginDate = DateTime.UtcNow
                };
                _context.AdminDevices.Add(newDevice);
                await _context.SaveChangesAsync();

                await _emailService.SendEmailAsync(user.Email, "Yeni Cihaz Tespiti!", $"Hesabınıza yeni bir cihazdan (Tarayıcı İzi: {deviceHash}) veya konumdan (IP: {ipAddress}) giriş yapıldı. Eğer bu işlemi siz yapmadıysanız hemen şifrenizi değiştirin.");
            }
            else
            {
                existingDevice.LastIpAddress = ipAddress;
                existingDevice.LastLoginDate = DateTime.UtcNow;
                _context.AdminDevices.Update(existingDevice);
                await _context.SaveChangesAsync();
            }
        }
    }
}
