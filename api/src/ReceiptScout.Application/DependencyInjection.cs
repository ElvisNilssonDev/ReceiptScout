using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ReceiptScout.Application.Receipts;

namespace ReceiptScout.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IReceiptService, ReceiptService>();
        // Fler services registreras här (CategoryService, ExpenseReportService, AuthService — Day 5)

        return services;
    }
}