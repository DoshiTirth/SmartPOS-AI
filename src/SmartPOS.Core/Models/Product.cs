namespace SmartPOS.Core.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public int CategoryId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public Category? Category { get; set; }
        public ICollection<TransactionItem> TransactionItems { get; set; } = new List<TransactionItem>();
    }

    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    public class StockAdjustment
    {
        public int AdjustmentId { get; set; }
        public int ProductId { get; set; }
        public int AdjustedByUserId { get; set; }
        public int QuantityBefore { get; set; }
        public int QuantityAfter { get; set; }
        public string? Reason { get; set; }
        public DateTime AdjustedAt { get; set; }

        // Navigation
        public Product? Product { get; set; }
        public User? AdjustedBy { get; set; }
    }
}