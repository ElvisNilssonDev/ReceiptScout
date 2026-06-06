using Microsoft.AspNetCore.Identity;
using ReceiptScout.Application.Common.Interfaces;
using ReceiptScout.Domain.Identity;

namespace ReceiptScout.Infrastructure.Auth;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Success, string? UserId, string[] Errors)> CreateUserAsync(
        string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return (false, null, result.Errors.Select(e => e.Description).ToArray());

        var isFirstUser = _userManager.Users.Count() == 1;
        await _userManager.AddToRoleAsync(user, isFirstUser ? "Admin" : "User");

        return (true, user.Id, Array.Empty<string>());
    }

    public async Task<bool> CheckPasswordAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return false;
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<string?> GetUserIdByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user?.Id;
    }

    public async Task<IList<string>> GetRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null ? Array.Empty<string>() : await _userManager.GetRolesAsync(user);
    }
}