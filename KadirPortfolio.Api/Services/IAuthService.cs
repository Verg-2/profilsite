using KadirPortfolio.Api.Models;

namespace KadirPortfolio.Api.Services
{
    public interface IAuthService
    {
        Task<AdminUser?> AuthenticateAsync(string email, string password);
        Task<string> GenerateJwtTokenAsync(AdminUser user, TimeSpan expiration);
        Task<string> GenerateRefreshTokenAsync();
        Task SaveRefreshTokenAsync(AdminUser user, string refreshToken, DateTime expiryTime);
        Task<AdminUser?> GetUserByEmailAsync(string email);
        Task<AdminUser?> ValidateRefreshTokenAsync(int userId, string refreshToken);
        Task Send2FaCodeAsync(AdminUser user);
        Task<bool> Verify2FaCodeAsync(string email, string code);
        Task TrackDeviceAsync(AdminUser user, string deviceHash, string ipAddress);
    }
}
