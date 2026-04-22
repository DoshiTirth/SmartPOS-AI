using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPOS.Core.DTOs;
using SmartPOS.Core.Interfaces;

namespace SmartPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionsController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessTransaction([FromBody] CreateTransactionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _transactionRepository.ProcessTransactionAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var detail = await _transactionRepository.GetTransactionDetailAsync(id);
            if (detail == null) return NotFound(new { message = "Transaction not found." });
            return Ok(detail);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent([FromQuery] int count = 20)
        {
            var transactions = await _transactionRepository.GetRecentTransactionsAsync(count);
            return Ok(transactions);
        }

        [HttpPost("{id}/void")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> VoidTransaction(int id, [FromQuery] int userId)
        {
            var success = await _transactionRepository.VoidTransactionAsync(id, userId);
            if (!success) return BadRequest(new { message = "Transaction cannot be voided." });
            return NoContent();
        }
    }
}