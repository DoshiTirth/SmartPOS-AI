using System.Transactions;

namespace SmartPOS.Core.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int LoyaltyPoints { get; set; } = 0;
        public string? Segment { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}