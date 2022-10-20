using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace R2S.Users.Core.IntegrationTests
{
    public static class UsersServicesTestsRegistrationExtension
    {
        public static IServiceCollection AddTestUsersServices(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            services.AddUsersServices(configuration);
            services.AddLogging(logging => logging.AddConsole());

            return services;
        }
    }
}
