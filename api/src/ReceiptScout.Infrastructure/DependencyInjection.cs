using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReceiptScout.Application.Common.Interfaces;
using ReceiptScout.Domain.Identity;
using ReceiptScout.Infrastructure.Auth;
using ReceiptScout.Infrastructure.Persistence;
using ReceiptScout.Infrastructure.Persistence.Repositories;

namespace ReceiptScout.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IReceiptRepository, ReceiptRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IExpenseReportRepository, ExpenseReportRepository>();

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 12;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.User.RequireUniqueEmail = true;
        })
           .AddEntityFrameworkStores<AppDbContext>()
           .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Current user
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}