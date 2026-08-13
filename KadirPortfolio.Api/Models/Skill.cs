using System;
using System.Collections.Generic;

namespace KadirPortfolio.Api.Models
{
    public class SkillCategory
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? TitleEn { get; set; }
        public string Icon { get; set; } = string.Empty;
        
        public List<SkillItem> Skills { get; set; } = new();
    }

    public class SkillItem
    {
        public int Id { get; set; }
        public int SkillCategoryId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public SkillCategory? Category { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NameEn { get; set; }
        public int Percentage { get; set; }
        public string Color { get; set; } = string.Empty; // Hex color
        // Yeni: Soft Delete için
        public bool IsDeleted { get; set; } = false;
    }
}
