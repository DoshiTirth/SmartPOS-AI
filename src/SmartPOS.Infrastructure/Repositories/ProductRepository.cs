using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartPOS.Core.DTOs;
using SmartPOS.Core.Interfaces;
using SmartPOS.Core.Models;
using SmartPOS.Infrastructure.Data;

namespace SmartPOS.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SmartPOSDbContext _context;
        private readonly string _connectionString;

        public ProductRepository(SmartPOSDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("SmartPOSDB")!;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    SKU = p.SKU,
                    Barcode = p.Barcode,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category!.CategoryName,
                    UnitPrice = p.UnitPrice,
                    CostPrice = p.CostPrice,
                    StockQuantity = p.StockQuantity,
                    LowStockThreshold = p.LowStockThreshold,
                    IsActive = p.IsActive
                })
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<ProductDto?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductId == productId)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    SKU = p.SKU,
                    Barcode = p.Barcode,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category!.CategoryName,
                    UnitPrice = p.UnitPrice,
                    CostPrice = p.CostPrice,
                    StockQuantity = p.StockQuantity,
                    LowStockThreshold = p.LowStockThreshold,
                    IsActive = p.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ProductDto?> GetProductByBarcodeAsync(string barcode)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Barcode == barcode && p.IsActive)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    SKU = p.SKU,
                    Barcode = p.Barcode,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category!.CategoryName,
                    UnitPrice = p.UnitPrice,
                    CostPrice = p.CostPrice,
                    StockQuantity = p.StockQuantity,
                    LowStockThreshold = p.LowStockThreshold,
                    IsActive = p.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ProductDto?> GetProductBySKUAsync(string sku)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.SKU == sku && p.IsActive)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    SKU = p.SKU,
                    Barcode = p.Barcode,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category!.CategoryName,
                    UnitPrice = p.UnitPrice,
                    CostPrice = p.CostPrice,
                    StockQuantity = p.StockQuantity,
                    LowStockThreshold = p.LowStockThreshold,
                    IsActive = p.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<LowStockProductDto>> GetLowStockProductsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<LowStockProductDto>(
                "EXEC sp_GetLowStockProducts"
            );
        }

        public async Task<Product> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,
                SKU = dto.SKU,
                Barcode = dto.Barcode,
                CategoryId = dto.CategoryId,
                UnitPrice = dto.UnitPrice,
                CostPrice = dto.CostPrice,
                StockQuantity = dto.StockQuantity,
                LowStockThreshold = dto.LowStockThreshold,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> UpdateProductAsync(int productId, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            product.ProductName = dto.ProductName;
            product.UnitPrice = dto.UnitPrice;
            product.CostPrice = dto.CostPrice;
            product.LowStockThreshold = dto.LowStockThreshold;
            product.IsActive = dto.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdjustStockAsync(int productId, int quantity, int adjustedByUserId, string reason)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            var adjustment = new StockAdjustment
            {
                ProductId = productId,
                AdjustedByUserId = adjustedByUserId,
                QuantityBefore = product.StockQuantity,
                QuantityAfter = product.StockQuantity + quantity,
                Reason = reason,
                AdjustedAt = DateTime.UtcNow
            };

            product.StockQuantity += quantity;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}