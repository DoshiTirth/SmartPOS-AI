namespace SmartPOS.Core.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public int CashierId { get; set; }
        public int? CustomerId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = "Completed";
        public bool IsAnomaly { get; set; } = false;
        public double? AnomalyScore { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User? Cashier { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<TransactionItem> Items { get; set; } = new List<TransactionItem>();
    }

    public class TransactionItem
    {
        public int TransactionItemId { get; set; }
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal LineTotal { get; set; }

        // Navigation
        public Transaction? Transaction { get; set; }
        public Product? Product { get; set; }
    }
}