using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EShop.Basket.Infrastructure.IntegrationTests;
public static class TestBasketServicesRegistrationExtension
{
    public static async Task<IServiceCollection> AddTestBasketServicesAsync(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()            
            .AddJsonFile("appsettings.tests.json")
            .AddEnvironmentVariables()
            .Build();

        await services.AddBasketSercicesAsync(configuration);
        services.AddLogging(logging => logging.AddConsole());

        return services;
    }
}
