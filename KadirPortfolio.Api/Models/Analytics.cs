using System;

namespace KadirPortfolio.Api.Models
{
    public class AnalyticsData
    {
        public int Id { get; set; }
        public string VisitorIp { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime VisitDate { get; set; }
    }

    public class SystemHealthLog
    {
        public int Id { get; set; }
        public string ErrorType { get; set; } = string.Empty; // 404, API_Error
        public string Details { get; set; } = string.Empty;
        public DateTime LogDate { get; set; }
    }
}
