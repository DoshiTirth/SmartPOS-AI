using SmartPOS.Core.DTOs;

namespace SmartPOS.Core.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
        Task<IEnumerable<DailySalesDto>> GetSalesLast30DaysAsync();
        Task<IEnumerable<TopSellingProductDto>> GetTopSellingProductsAsync(int topN = 10, int days = 30);
    }
}