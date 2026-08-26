using System.ComponentModel.DataAnnotations;

namespace KadirPortfolio.Api.Models
{
    public class SeoSetting
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Route { get; set; } = ""; // e.g. "/", "/hakkinda"
        
        public string SeoTitle { get; set; } = "";
        public string? SeoTitleEn { get; set; }
        
        public string SeoDescription { get; set; } = "";
        public string? SeoDescriptionEn { get; set; }
        
        public string GeoTitle { get; set; } = "";
        public string? GeoTitleEn { get; set; }
        
        public string GeoDescription { get; set; } = "";
        public string? GeoDescriptionEn { get; set; }
        
        public string Lang { get; set; } = "tr";
        
        public bool IsVisible { get; set; } = true;
    }
}
