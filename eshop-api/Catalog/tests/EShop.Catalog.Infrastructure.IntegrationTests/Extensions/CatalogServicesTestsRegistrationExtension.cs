using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EShop.Catalog.Infrastructure.IntegrationTests.Extensions;

public static class CatalogServicesTestsRegistrationExtension
{
    public static IServiceCollection AddTestCatalogServices(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        services.AddCatalogServices(configuration);
        services.AddLogging(logging => logging.AddConsole());

        return services;
    }
}
