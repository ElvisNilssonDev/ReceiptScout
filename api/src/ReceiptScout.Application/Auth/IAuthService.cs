using ReceiptScout.Application.Auth.Dtos;

namespace ReceiptScout.Application.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterDto dto);
    Task<AuthResponse> LoginAsync(LoginDto dto);
}