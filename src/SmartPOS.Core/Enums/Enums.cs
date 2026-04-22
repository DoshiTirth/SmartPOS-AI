namespace SmartPOS.Core.Enums
{
    public enum UserRole
    {
        Admin = 1,
        Manager = 2,
        Cashier = 3
    }

    public enum TransactionStatus
    {
        Completed,
        Voided,
        Refunded
    }

    public enum PaymentMethod
    {
        Cash,
        Card,
        Other
    }

    public enum CustomerSegment
    {
        New,
        Occasional,
        Regular,
        HighValue,
        AtRisk
    }

    public enum StockStatus
    {
        InStock,
        LowStock,
        OutOfStock
    }
}