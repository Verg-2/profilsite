using System;
using System.Collections.Generic;

namespace KadirPortfolio.Api.Models
{
    public class ProjectCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NameEn { get; set; }
        public string? Icon { get; set; }
        public List<Project> Projects { get; set; } = new();
    }

    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? TitleEn { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string? SummaryEn { get; set; }
        public int ProjectCategoryId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public ProjectCategory? Category { get; set; }
        public string Aim { get; set; } = string.Empty;
        public string? AimEn { get; set; }
        public string ChallengesAndSolutions { get; set; } = string.Empty;
        public string? ChallengesAndSolutionsEn { get; set; }
        public List<string>? TechTags { get; set; } 
        public List<string>? ImageUrls { get; set; }
        public string? VideoUrl { get; set; }
        
        // Theme-specific Media
        public List<string>? LightImageUrls { get; set; }
        public List<string>? DarkImageUrls { get; set; }
        public string? LightVideoUrl { get; set; }
        public string? DarkVideoUrl { get; set; }

        // SEO and Accessibility
        public string? ImageAltText { get; set; }
        public string? VideoAriaLabel { get; set; }

        // Advanced Media
        public string? Model3DUrl { get; set; }
        // Yeni: Soft Delete için
        public bool IsDeleted { get; set; } = false;
    }
}
