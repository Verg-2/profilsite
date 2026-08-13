using KadirPortfolio.Api.Models;

namespace KadirPortfolio.Api.Services
{
    public interface IAuditLogger
    {
        Task LogAsync(string action, string details, string ipAddress, string userAgent, string? adminEmail = null);
    }
}
