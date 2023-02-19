using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R2S.Catalog.Infrastructure.IntegrationTests.Extensions;

public static class CatalogServicesTestsRegistrationExtension
{
    public static IServiceCollection AddTestCatalogServices(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        services.AddCatalogServices(configuration);
        services.AddLogging(logging => logging.AddConsole());

        return services;
    }
}
