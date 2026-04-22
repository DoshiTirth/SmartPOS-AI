using SmartPOS.Core.DTOs;
using SmartPOS.Core.Models;

namespace SmartPOS.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int productId);
        Task<ProductDto?> GetProductByBarcodeAsync(string barcode);
        Task<ProductDto?> GetProductBySKUAsync(string sku);
        Task<IEnumerable<LowStockProductDto>> GetLowStockProductsAsync();
        Task<Product> CreateProductAsync(CreateProductDto dto);
        Task<bool> UpdateProductAsync(int productId, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int productId);
        Task<bool> AdjustStockAsync(int productId, int quantity, int adjustedByUserId, string reason);
    }
}