using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReceiptScout.Domain.Entities;
using ReceiptScout.Domain.Identity;

namespace ReceiptScout.Infrastructure.Persistence.Seeding;

/// <summary>
/// Idempotent startup seeder. Safe to run on every boot in every environment:
/// applies pending migrations, ensures the Admin/User roles exist, bootstraps a
/// single admin account, and inserts the standard Swedish BAS expense categories.
/// Replaces the manual SSMS "promote first user to Admin" step.
/// </summary>
public class ApplicationDbSeeder
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApplicationDbSeeder> _logger;

    public ApplicationDbSeeder(
        AppDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILogger<ApplicationDbSeeder> logger)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await _db.Database.MigrateAsync();

        await EnsureRolesAsync();
        await EnsureAdminUserAsync();
        await EnsureBasCategoriesAsync();
    }

    private async Task EnsureRolesAsync()
    {
        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
                _logger.LogInformation("Seeded role {Role}.", role);
            }
        }
    }

    private async Task EnsureAdminUserAsync()
    {
        var email = _configuration["Seed:AdminEmail"] ?? "admin@receiptscout.local";
        var password = _configuration["Seed:AdminPassword"] ?? "Admin123!";

        if (await _userManager.FindByEmailAsync(email) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
        {
            _logger.LogError(
                "Failed to seed admin user: {Errors}",
                string.Join("; ", result.Errors.Select(e => e.Description)));
            return;
        }

        await _userManager.AddToRoleAsync(admin, "Admin");
        _logger.LogInformation("Seeded admin user {Email}.", email);
    }

    private async Task EnsureBasCategoriesAsync()
    {
        // Standard Swedish BAS-kontoplan expense accounts, with representative
        // VAT rates (25 % / 12 % / 6 %) so the AI categorization slice has real
        // targets to map receipts onto.
        var seed = new[]
        {
            new BasSeed("Inköp varor och material", "4010", 0.25m),
            new BasSeed("Lokalhyra",                 "5010", 0.25m),
            new BasSeed("Förbrukningsinventarier",   "5410", 0.25m),
            new BasSeed("Drivmedel",                 "5611", 0.25m),
            new BasSeed("Biljetter och resor",       "5810", 0.06m),
            new BasSeed("Kost och logi",             "5831", 0.12m),
            new BasSeed("Representation, avdragsgill","6071", 0.12m),
            new BasSeed("Kontorsmateriel",           "6110", 0.25m),
            new BasSeed("Mobiltelefon",              "6212", 0.25m),
            new BasSeed("Datakommunikation",         "6230", 0.25m),
            new BasSeed("IT-tjänster",               "6540", 0.25m),
            new BasSeed("Övriga avdragsgilla kostnader", "6991", 0.25m),
        };

        // Per-name check rather than all-or-nothing, so expanding this list later
        // adds the new accounts on next boot without violating the unique indexes.
        var existingNames = await _db.Categories
            .Select(c => c.Name)
            .ToListAsync();

        var toAdd = seed
            .Where(s => !existingNames.Contains(s.Name))
            .Select(s => new Category(s.Name, s.BasAccount, s.VatRate))
            .ToList();

        if (toAdd.Count == 0)
            return;

        _db.Categories.AddRange(toAdd);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} BAS categories.", toAdd.Count);
    }

    private readonly record struct BasSeed(string Name, string BasAccount, decimal VatRate);
}