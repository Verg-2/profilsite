using System;
using System.ComponentModel.DataAnnotations;

namespace KadirPortfolio.Api.Models
{
    public class ApiKeyConfig
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Alias { get; set; } = string.Empty;

        [Required]
        public string KeyValue { get; set; } = string.Empty; // Encrypted API Key

        [Required]
        public string IV { get; set; } = string.Empty; // Initialization Vector for AES

        [MaxLength(50)]
        public string AssignedTask { get; set; } = "Genel"; // 'Genel', 'Home', 'Blog', vs.

        [Required]
        [MaxLength(50)]
        public string Provider { get; set; } = "Google"; // "Google" veya "OpenAI"

        [MaxLength(255)]
        public string? BaseUrl { get; set; } // Özel endpointler için (Örn: DeepSeek)

        [MaxLength(100)]
        public string? ModelName { get; set; } // Hangi model kullanılacak (Örn: gpt-3.5-turbo, deepseek-chat)

        public bool IsActive { get; set; } = true;

        public int RequestCount { get; set; } = 0;
        
        public int TotalTokensUsed { get; set; } = 0;

        public DateTime? LastUsedDate { get; set; }
        
        [MaxLength(500)]
        public string? LastError { get; set; }
        
        public DateTime? LastErrorDate { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
