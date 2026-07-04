using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.Services;

namespace ProductCatalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IProductRepository, DummyJsonProductRepository>(client =>
            client.BaseAddress = new Uri(configuration["DummyJson:BaseUrl"]!));

        services.AddHttpClient<IAuthRepository, DummyJsonAuthRepository>(client =>
            client.BaseAddress = new Uri(configuration["DummyJson:BaseUrl"]!));

        services.AddScoped<IJwtService, JwtService>();
        services.AddHttpContextAccessor();

        return services;
    }
}