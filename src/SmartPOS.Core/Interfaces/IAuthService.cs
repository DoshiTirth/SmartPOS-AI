using SmartPOS.Core.DTOs;

namespace SmartPOS.Core.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
        string GenerateJwtToken(int userId, string email, string role, string fullName);
    }
}