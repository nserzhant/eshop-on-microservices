using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EShop.Ordering.Infrastructure.IntegrationTests.Extensions;
public static class TestOrderingServicesRegistrationExtension
{
    public static IServiceCollection AddTestOrderingServices(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.tests.json")
            .AddEnvironmentVariables()
            .Build();

        services.AddOrderingServices(configuration);
        services.AddLogging(logging => logging.AddConsole());

        return services;
    }
}
