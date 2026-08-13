using System.ComponentModel.DataAnnotations;

namespace KadirPortfolio.Api.Models
{
    public class ContactCard
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string? TitleEn { get; set; }

        public string? Subtitle { get; set; }
        public string? SubtitleEn { get; set; }

        public string? Url { get; set; }

        [Required]
        public string Icon { get; set; }

        public int OrderIndex { get; set; }
    }
}
