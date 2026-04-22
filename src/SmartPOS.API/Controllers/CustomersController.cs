using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPOS.Core.DTOs;
using SmartPOS.Core.Interfaces;

namespace SmartPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerRepository.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound(new { message = "Customer not found." });
            return Ok(customer);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var customer = await _customerRepository.GetCustomerByEmailAsync(email);
            if (customer == null) return NotFound(new { message = "Customer not found." });
            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var customer = await _customerRepository.CreateCustomerAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = customer.CustomerId }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCustomerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _customerRepository.UpdateCustomerAsync(id, dto);
            if (!success) return NotFound(new { message = "Customer not found." });
            return NoContent();
        }
    }
}