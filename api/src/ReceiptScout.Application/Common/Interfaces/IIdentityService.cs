namespace ReceiptScout.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Success, string? UserId, string[] Errors)> CreateUserAsync(string email, string password);
    Task<bool> CheckPasswordAsync(string email, string password);
    Task<string?> GetUserIdByEmailAsync(string email);
    Task<IList<string>> GetRolesAsync(string userId);
}