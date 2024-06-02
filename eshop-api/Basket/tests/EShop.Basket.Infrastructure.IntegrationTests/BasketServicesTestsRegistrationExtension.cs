using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EShop.Basket.Infrastructure.IntegrationTests;
public static class BasketServicesTestsRegistrationExtension
{
    public static IServiceCollection AddTestBasketServices(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()            
            .AddJsonFile("appsettings.tests.json")
            .AddEnvironmentVariables()
            .Build();

        services.AddBasketSercices(configuration);
        services.AddLogging(logging => logging.AddConsole());

        return services;
    }
}
