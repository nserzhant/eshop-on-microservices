using Azure.Identity;
using EShop.Basket.Core.Interfaces;
using EShop.Basket.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace EShop.Basket.Infrastructure;

public static class BasketServiceRegistrationExtension
{
    const string REDIS_CONNECTION_STRING_CONFIG_NAME = "redisConnectionString";
    const string ENTRA_ID_REDIS_CONNECTION_STRING_NAME = "entraIdRedisConnectionString";

    public static async Task<IServiceCollection> AddBasketSercicesAsync(this IServiceCollection services, IConfiguration configuration)
    {
        var entraIdConnectionString = configuration.GetConnectionString(ENTRA_ID_REDIS_CONNECTION_STRING_NAME);
        var connectionString = configuration.GetConnectionString(REDIS_CONNECTION_STRING_CONFIG_NAME);

        ConfigurationOptions configurationOptions = null;

        if (!string.IsNullOrEmpty(entraIdConnectionString))
        {
            configurationOptions = await ConfigurationOptions.Parse(entraIdConnectionString)
                .ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential());
        }
        else if (!string.IsNullOrEmpty(connectionString))
        {
            configurationOptions = ConfigurationOptions.Parse(connectionString!);
        }

        services.AddTransient<IBasketRepository, BasketRepository>();
        services.AddScoped(services =>
        {
            if (configurationOptions == null)
                throw new ArgumentNullException(nameof(configurationOptions));

            var connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
            var database = connectionMultiplexer.GetDatabase();

            return database;
        });

        return services;
    }
}