using System;
using System.Collections.Generic;

namespace KadirPortfolio.Api.Models
{
    public class HomeSetting
    {
        public int Id { get; set; }
        public string HeroTitle { get; set; } = string.Empty;
        public string? HeroTitleEn { get; set; }
        public string HeroSubtitle { get; set; } = string.Empty;
        public string? HeroSubtitleEn { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
        public string PreTitle { get; set; } = string.Empty;
        public string? PreTitleEn { get; set; }
        public string ButtonText { get; set; } = string.Empty;
        public string? ButtonTextEn { get; set; }
        public string ButtonUrl { get; set; } = string.Empty;
        public string SecondaryButtonText { get; set; } = string.Empty;
        public string? SecondaryButtonTextEn { get; set; }
        public string SecondaryButtonUrl { get; set; } = string.Empty;
        public string LightCursor { get; set; } = string.Empty;
        public string DarkCursor { get; set; } = string.Empty;
    }
}
