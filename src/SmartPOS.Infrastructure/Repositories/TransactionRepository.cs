using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartPOS.Core.DTOs;
using SmartPOS.Core.Interfaces;
using SmartPOS.Infrastructure.Data;
using System.Xml.Linq;

namespace SmartPOS.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly SmartPOSDbContext _context;
        private readonly string _connectionString;

        public TransactionRepository(SmartPOSDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("SmartPOSDB")!;
        }

        public async Task<TransactionResultDto> ProcessTransactionAsync(CreateTransactionDto dto)
        {
            // Build XML for stored procedure
            var xml = new XElement("items",
                dto.Items.Select(i =>
                    new XElement("item",
                        new XAttribute("productId", i.ProductId),
                        new XAttribute("quantity", i.Quantity)
                    )
                )
            );

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync<TransactionResultDto>(
                "EXEC sp_ProcessTransaction @CashierId, @CustomerId, @PaymentMethod, @TaxRate, @DiscountAmount, @ItemsXml",
                new
                {
                    dto.CashierId,
                    dto.CustomerId,
                    dto.PaymentMethod,
                    TaxRate = 0.13m,
                    dto.DiscountAmount,
                    ItemsXml = xml.ToString()
                }
            );

            return result ?? throw new Exception("Transaction processing failed.");
        }

        public async Task<TransactionDetailDto?> GetTransactionDetailAsync(int transactionId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync(
                "EXEC sp_GetTransactionDetail @TransactionId",
                new { TransactionId = transactionId }
            );

            var header = await multi.ReadFirstOrDefaultAsync<TransactionDetailDto>();
            if (header == null) return null;

            var items = await multi.ReadAsync<TransactionLineItemDto>();
            header.Items = items.ToList();

            return header;
        }

        public async Task<IEnumerable<TransactionResultDto>> GetRecentTransactionsAsync(int count = 20)
        {
            return await _context.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .Select(t => new TransactionResultDto
                {
                    TransactionId = t.TransactionId,
                    TransactionCode = t.TransactionCode,
                    Subtotal = t.Subtotal,
                    TaxAmount = t.TaxAmount,
                    DiscountAmount = t.DiscountAmount,
                    TotalAmount = t.TotalAmount,
                    PaymentMethod = t.PaymentMethod,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> VoidTransactionAsync(int transactionId, int userId)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction == null || transaction.Status != "Completed") return false;

            // Restore stock for each item
            var items = await _context.TransactionItems
                .Where(ti => ti.TransactionId == transactionId)
                .ToListAsync();

            foreach (var item in items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;
                }
            }

            transaction.Status = "Voided";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}