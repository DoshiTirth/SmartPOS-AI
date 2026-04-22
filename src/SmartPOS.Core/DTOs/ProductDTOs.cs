namespace SmartPOS.Core.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public bool IsLowStock => StockQuantity <= LowStockThreshold;
        public bool IsActive { get; set; }
    }

    public class CreateProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public int CategoryId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; } = 10;
    }

    public class UpdateProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int LowStockThreshold { get; set; }
        public bool IsActive { get; set; }
    }

    public class LowStockProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class AdjustStockDto
    {
        public int Quantity { get; set; }
        public int AdjustedByUserId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}