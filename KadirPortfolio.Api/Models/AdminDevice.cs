using System.ComponentModel.DataAnnotations;

namespace KadirPortfolio.Api.Models
{
    public class AdminDevice
    {
        public int Id { get; set; }
        
        public int AdminUserId { get; set; }
        public AdminUser AdminUser { get; set; }
        
        [Required]
        public string DeviceHash { get; set; }
        
        public string LastIpAddress { get; set; }
        
        public DateTime LastLoginDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsTrusted { get; set; } = true;
    }
}
