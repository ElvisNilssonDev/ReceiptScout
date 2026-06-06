using ReceiptScout.Application.Auth.Dtos;
using ReceiptScout.Application.Common.Exceptions;
using ReceiptScout.Application.Common.Interfaces;

namespace ReceiptScout.Application.Auth;

public class AuthService : IAuthService
{
    private readonly IIdentityService _identity;
    private readonly IJwtTokenService _jwt;

    public AuthService(IIdentityService identity, IJwtTokenService jwt)
    {
        _identity = identity;
        _jwt = jwt;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterDto dto)
    {
        var (success, userId, errors) = await _identity.CreateUserAsync(dto.Email, dto.Password);
        if (!success || userId is null)
            throw new InvalidOperationException(string.Join("; ", errors));

        var roles = await _identity.GetRolesAsync(userId);
        var token = _jwt.GenerateToken(userId, dto.Email, roles);
        return new AuthResponse(token, dto.Email, roles.AsReadOnly());
    }

    public async Task<AuthResponse> LoginAsync(LoginDto dto)
    {
        var ok = await _identity.CheckPasswordAsync(dto.Email, dto.Password);
        if (!ok) throw new ForbiddenException("Invalid email or password.");

        var userId = await _identity.GetUserIdByEmailAsync(dto.Email)
            ?? throw new ForbiddenException("Invalid email or password.");

        var roles = await _identity.GetRolesAsync(userId);
        var token = _jwt.GenerateToken(userId, dto.Email, roles);
        return new AuthResponse(token, dto.Email, roles.AsReadOnly());
    }
}