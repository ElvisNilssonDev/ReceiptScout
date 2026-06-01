using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ReceiptScout.Application.Categories;
using ReceiptScout.Application.ExpenseReports;
using ReceiptScout.Application.Receipts;
using System.Reflection;

namespace ReceiptScout.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IReceiptService, ReceiptService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IExpenseReportService, ExpenseReportService>();

        return services;
    }
}