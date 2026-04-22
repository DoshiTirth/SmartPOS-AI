namespace SmartPOS.Core.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalTransactionsToday { get; set; }
        public decimal TotalRevenueToday { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public int TotalTransactionsMonth { get; set; }
        public decimal TotalRevenueMonth { get; set; }
        public int LowStockProductCount { get; set; }
        public int UnreviewedAnomalies { get; set; }
    }

    public class DailySalesDto
    {
        public DateTime SaleDate { get; set; }
        public int TransactionCount { get; set; }
        public decimal DailyRevenue { get; set; }
    }

    public class TopSellingProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int TotalUnitsSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}