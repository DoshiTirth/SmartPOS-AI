using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPOS.Core.Interfaces;

namespace SmartPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _dashboardRepository.GetDashboardSummaryAsync();
            return Ok(summary);
        }

        [HttpGet("sales/last30days")]
        public async Task<IActionResult> GetSalesLast30Days()
        {
            var sales = await _dashboardRepository.GetSalesLast30DaysAsync();
            return Ok(sales);
        }

        [HttpGet("products/topselling")]
        public async Task<IActionResult> GetTopSelling([FromQuery] int topN = 10, [FromQuery] int days = 30)
        {
            var products = await _dashboardRepository.GetTopSellingProductsAsync(topN, days);
            return Ok(products);
        }
    }
}