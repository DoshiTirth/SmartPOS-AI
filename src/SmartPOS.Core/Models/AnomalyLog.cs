namespace SmartPOS.Core.Models
{
    public class AnomalyLog
    {
        public int AnomalyLogId { get; set; }
        public int TransactionId { get; set; }
        public double AnomalyScore { get; set; }
        public DateTime DetectedAt { get; set; }
        public int? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public bool? IsConfirmed { get; set; }
        public string? Notes { get; set; }

        // Navigation
        public Transaction? Transaction { get; set; }
        public User? Reviewer { get; set; }
    }

    public class SalesForecast
    {
        public int ForecastId { get; set; }
        public int? ProductId { get; set; }
        public DateTime ForecastDate { get; set; }
        public decimal PredictedSales { get; set; }
        public decimal? ActualSales { get; set; }
        public string? ModelVersion { get; set; }
        public DateTime GeneratedAt { get; set; }

        // Navigation
        public Product? Product { get; set; }
    }

    public class AIAssistantLog
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}