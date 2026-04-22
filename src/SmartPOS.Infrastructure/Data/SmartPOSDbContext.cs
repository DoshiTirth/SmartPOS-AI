using Microsoft.EntityFrameworkCore;
using SmartPOS.Core.Models;

namespace SmartPOS.Infrastructure.Data
{
    public class SmartPOSDbContext : DbContext
    {
        public SmartPOSDbContext(DbContextOptions<SmartPOSDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionItem> TransactionItems { get; set; }
        public DbSet<AnomalyLog> AnomalyLogs { get; set; }
        public DbSet<SalesForecast> SalesForecasts { get; set; }
        public DbSet<AIAssistantLog> AIAssistantLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
            });

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(512);
                entity.HasOne(e => e.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(e => e.RoleId);
            });

            // Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
            });

            // Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.SKU).IsUnique();
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.CostPrice).HasColumnType("decimal(10,2)");
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryId);
            });

            // Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            });

            // Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);
                entity.Property(e => e.TransactionCode).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.TransactionCode).IsUnique();
                entity.Property(e => e.Subtotal).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TaxAmount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
                entity.HasOne(e => e.Cashier)
                      .WithMany()
                      .HasForeignKey(e => e.CashierId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Transactions)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // TransactionItem
            modelBuilder.Entity<TransactionItem>(entity =>
            {
                entity.HasKey(e => e.TransactionItemId);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Discount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.LineTotal).HasColumnType("decimal(10,2)");
                entity.HasOne(e => e.Transaction)
                      .WithMany(t => t.Items)
                      .HasForeignKey(e => e.TransactionId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.TransactionItems)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // AnomalyLog
            modelBuilder.Entity<AnomalyLog>(entity =>
            {
                entity.HasKey(e => e.AnomalyLogId);
                entity.HasOne(e => e.Transaction)
                      .WithMany()
                      .HasForeignKey(e => e.TransactionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // SalesForecast
            modelBuilder.Entity<SalesForecast>(entity =>
            {
                entity.HasKey(e => e.ForecastId);
                entity.Property(e => e.PredictedSales).HasColumnType("decimal(10,2)");
                entity.Property(e => e.ActualSales).HasColumnType("decimal(10,2)");
            });

            // AIAssistantLog
            modelBuilder.Entity<AIAssistantLog>(entity =>
            {
                entity.HasKey(e => e.LogId);
            });
        }
    }
}