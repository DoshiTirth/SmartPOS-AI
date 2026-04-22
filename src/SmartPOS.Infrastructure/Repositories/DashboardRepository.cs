using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartPOS.Core.DTOs;
using SmartPOS.Core.Interfaces;

namespace SmartPOS.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SmartPOSDB")!;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync("EXEC sp_GetDashboardSummary");

            var today = await multi.ReadFirstOrDefaultAsync<dynamic>();
            var month = await multi.ReadFirstOrDefaultAsync<dynamic>();
            var stock = await multi.ReadFirstOrDefaultAsync<dynamic>();
            var anomaly = await multi.ReadFirstOrDefaultAsync<dynamic>();

            return new DashboardSummaryDto
            {
                TotalTransactionsToday = (int)(today?.TotalTransactionsToday ?? 0),
                TotalRevenueToday = (decimal)(today?.TotalRevenueToday ?? 0),
                AverageTransactionValue = (decimal)(today?.AverageTransactionValue ?? 0),
                TotalTransactionsMonth = (int)(month?.TotalTransactionsMonth ?? 0),
                TotalRevenueMonth = (decimal)(month?.TotalRevenueMonth ?? 0),
                LowStockProductCount = (int)(stock?.LowStockProductCount ?? 0),
                UnreviewedAnomalies = (int)(anomaly?.UnreviewedAnomalies ?? 0)
            };
        }

        public async Task<IEnumerable<DailySalesDto>> GetSalesLast30DaysAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<DailySalesDto>(
                "EXEC sp_GetSalesLast30Days"
            );
        }

        public async Task<IEnumerable<TopSellingProductDto>> GetTopSellingProductsAsync(int topN = 10, int days = 30)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<TopSellingProductDto>(
                "EXEC sp_GetTopSellingProducts @TopN, @Days",
                new { TopN = topN, Days = days }
            );
        }
    }
}