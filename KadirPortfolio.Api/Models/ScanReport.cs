namespace KadirPortfolio.Api.Models
{
    public class ScanReport
    {
        public int SecurityScore { get; set; }
        public int SeoScore { get; set; }
        public int HealthScore { get; set; }
        public int PerformanceScore { get; set; }
        public List<ScanIssue> Issues { get; set; } = new List<ScanIssue>();
        public DateTime ScanDate { get; set; } = DateTime.UtcNow;
    }

    public class ScanIssue
    {
        public string Category { get; set; } = ""; // Security, SEO, Health, Performance
        public string Severity { get; set; } = ""; // Critical, Warning, Success
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
