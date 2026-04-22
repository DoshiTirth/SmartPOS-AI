using Microsoft.EntityFrameworkCore;
using SmartPOS.Core.DTOs;
using SmartPOS.Core.Interfaces;
using SmartPOS.Core.Models;
using SmartPOS.Infrastructure.Data;

namespace SmartPOS.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SmartPOSDbContext _context;

        public CustomerRepository(SmartPOSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    FullName = c.FullName,
                    Email = c.Email,
                    Phone = c.Phone,
                    LoyaltyPoints = c.LoyaltyPoints,
                    Segment = c.Segment,
                    CreatedAt = c.CreatedAt
                })
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers
                .Where(c => c.CustomerId == customerId)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    FullName = c.FullName,
                    Email = c.Email,
                    Phone = c.Phone,
                    LoyaltyPoints = c.LoyaltyPoints,
                    Segment = c.Segment,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerDto?> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers
                .Where(c => c.Email == email)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    FullName = c.FullName,
                    Email = c.Email,
                    Phone = c.Phone,
                    LoyaltyPoints = c.LoyaltyPoints,
                    Segment = c.Segment,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Customer> CreateCustomerAsync(CreateCustomerDto dto)
        {
            var customer = new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> UpdateCustomerAsync(int customerId, CreateCustomerDto dto)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return false;

            customer.FullName = dto.FullName;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}