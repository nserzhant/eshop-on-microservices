using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EShop.EmployeeManagement.Core.IntegrationTests;

public static class EmployeeServicesTestsRegistrationExtension
{
    public static IServiceCollection AddTestEmployeeServices(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        services.AddEmployeeServices(configuration);
        services.AddLogging(logging => logging.AddConsole());

        return services;
    }
}
