using SmartPOS.Core.DTOs;

namespace SmartPOS.Core.Interfaces
{
    public interface ITransactionRepository
    {
        Task<TransactionResultDto> ProcessTransactionAsync(CreateTransactionDto dto);
        Task<TransactionDetailDto?> GetTransactionDetailAsync(int transactionId);
        Task<IEnumerable<TransactionResultDto>> GetRecentTransactionsAsync(int count = 20);
        Task<bool> VoidTransactionAsync(int transactionId, int userId);
    }
}