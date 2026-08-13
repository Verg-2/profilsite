using System;
using System.Collections.Generic;

namespace KadirPortfolio.Api.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? TitleEn { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string? SummaryEn { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ContentEn { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Icon { get; set; }
        public DateTime PublishDate { get; set; }
        public List<string>? Tags { get; set; }
        public List<string>? TechIcons { get; set; } // e.g. ["fab fa-js", "fab fa-vuejs"]
        public string? ProTip { get; set; }
        public string? ProTipEn { get; set; }

        // Tip: "article" (kısa yazı) veya "book" (kitap modu)
        public string PostType { get; set; } = "article";

        // Kitap modu kapak rengi (CSS renk kodu, örn: "#1a1a2e")
        public string? BookColor { get; set; }

        public int? BlogCategoryId { get; set; }
        public BlogCategory? Category { get; set; }

        // Yeni: Soft Delete için
        public bool IsDeleted { get; set; } = false;
    }
}
