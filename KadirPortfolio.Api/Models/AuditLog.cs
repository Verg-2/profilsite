using System.ComponentModel.DataAnnotations;

namespace KadirPortfolio.Api.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        
        [Required]
        public string Action { get; set; }
        
        public string Details { get; set; }
        
        public string IpAddress { get; set; }
        
        public string UserAgent { get; set; }
        
        public string? AdminEmail { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
