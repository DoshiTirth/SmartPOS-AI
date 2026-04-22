using SmartPOS.Core.DTOs;
using SmartPOS.Core.Models;

namespace SmartPOS.Core.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDto?> GetCustomerByIdAsync(int customerId);
        Task<CustomerDto?> GetCustomerByEmailAsync(string email);
        Task<Customer> CreateCustomerAsync(CreateCustomerDto dto);
        Task<bool> UpdateCustomerAsync(int customerId, CreateCustomerDto dto);
    }
}