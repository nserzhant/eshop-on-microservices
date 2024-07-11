using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EShop.EmployeeManagement.Infrastructure.IntegrationTests;

public static class TestsEmployeeServicesRegistrationExtension
{
    public static IServiceCollection AddTestEmployeeServices(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.tests.json")
            .AddEnvironmentVariables()
            .Build();

        services.AddEmployeeServices(configuration);
        services.AddLogging(logging => logging.AddConsole());

        return services;
    }
}
