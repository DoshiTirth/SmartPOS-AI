using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPOS.Core.DTOs;
using SmartPOS.Core.Interfaces;

namespace SmartPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return NotFound(new { message = "Product not found." });
            return Ok(product);
        }

        [HttpGet("barcode/{barcode}")]
        public async Task<IActionResult> GetByBarcode(string barcode)
        {
            var product = await _productRepository.GetProductByBarcodeAsync(barcode);
            if (product == null) return NotFound(new { message = "Product not found." });
            return Ok(product);
        }

        [HttpGet("sku/{sku}")]
        public async Task<IActionResult> GetBySKU(string sku)
        {
            var product = await _productRepository.GetProductBySKUAsync(sku);
            if (product == null) return NotFound(new { message = "Product not found." });
            return Ok(product);
        }

        [HttpGet("lowstock")]
        public async Task<IActionResult> GetLowStock()
        {
            var products = await _productRepository.GetLowStockProductsAsync();
            return Ok(products);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var product = await _productRepository.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _productRepository.UpdateProductAsync(id, dto);
            if (!success) return NotFound(new { message = "Product not found." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productRepository.DeleteProductAsync(id);
            if (!success) return NotFound(new { message = "Product not found." });
            return NoContent();
        }

        [HttpPost("{id}/adjuststock")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AdjustStock(int id, [FromBody] AdjustStockDto dto)
        {
            var success = await _productRepository.AdjustStockAsync(id, dto.Quantity, dto.AdjustedByUserId, dto.Reason);
            if (!success) return NotFound(new { message = "Product not found." });
            return NoContent();
        }
    }
}