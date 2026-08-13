using System;
using System.ComponentModel.DataAnnotations;

namespace KadirPortfolio.Api.Models
{
    public class TranslationMemory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(64)] // SHA-256 is 64 hex characters
        public string OriginalHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string TargetLanguage { get; set; } = string.Empty;

        [Required]
        public string TranslatedText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
