using System;
using System.Collections.Generic;

namespace KadirPortfolio.Api.Models
{
    public class AboutSetting
    {
        public int Id { get; set; }
        public string MainTitle { get; set; } = string.Empty;
        public string? MainTitleEn { get; set; }
        public string SubTitle { get; set; } = string.Empty;
        public string? SubTitleEn { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
        public string CardTitle { get; set; } = string.Empty;
        public string? CardTitleEn { get; set; }
        public string CardSubtitle { get; set; } = string.Empty;
        public string? CardSubtitleEn { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string? BioEn { get; set; }
        
        // SEO & Job Status
        public bool IsLookingForJob { get; set; } = false;

        public List<AboutCard> Cards { get; set; } = new();
    }

    public class AboutCard
    {
        public int Id { get; set; }
        public int AboutSettingId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public AboutSetting? AboutSetting { get; set; }
        public int CardType { get; set; } // 1: Normal, 2: List
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? TitleEn { get; set; }
        public string? Text { get; set; }
        public string? TextEn { get; set; }
        public List<string>? ListItems { get; set; } 
        public List<string>? ListItemsEn { get; set; }
        // Yeni: Soft Delete için
        public bool IsDeleted { get; set; } = false;
    }
}
