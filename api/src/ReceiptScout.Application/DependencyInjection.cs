using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ReceiptScout.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Application services registreras här när vi bygger dem (Day 4–5)

        return services;
    }
}