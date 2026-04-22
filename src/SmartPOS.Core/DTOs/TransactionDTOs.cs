namespace SmartPOS.Core.DTOs
{
    public class CreateTransactionDto
    {
        public int CashierId { get; set; }
        public int? CustomerId { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public decimal DiscountAmount { get; set; } = 0;
        public List<TransactionItemDto> Items { get; set; } = new();
    }

    public class TransactionItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class TransactionResultDto
    {
        public int TransactionId { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class TransactionDetailDto
    {
        public int TransactionId { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsAnomaly { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CashierName { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public int? LoyaltyPoints { get; set; }
        public List<TransactionLineItemDto> Items { get; set; } = new();
    }

    public class TransactionLineItemDto
    {
        public int TransactionItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal LineTotal { get; set; }
    }
}