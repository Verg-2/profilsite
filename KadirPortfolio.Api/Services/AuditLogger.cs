using KadirPortfolio.Api.Data;
using KadirPortfolio.Api.Models;

namespace KadirPortfolio.Api.Services
{
    public class AuditLogger : IAuditLogger
    {
        private readonly AppDbContext _context;

        public AuditLogger(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string action, string details, string ipAddress, string userAgent, string? adminEmail = null)
        {
            var log = new AuditLog
            {
                Action = action,
                Details = details,
                IpAddress = ipAddress ?? "Unknown",
                UserAgent = userAgent ?? "Unknown",
                AdminEmail = adminEmail
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
