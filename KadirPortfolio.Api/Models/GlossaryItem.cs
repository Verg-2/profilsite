using System.ComponentModel.DataAnnotations;

namespace KadirPortfolio.Api.Models
{
    public class GlossaryItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string OriginalTerm { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string TargetTerm { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
