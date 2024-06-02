using EShop.Basket.Core.Interfaces;
using EShop.Basket.Core.Services;
using EShop.Basket.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace EShop.Basket.Infrastructure;

public static class BasketServiceRegistrationExtension
{
    const string REDIS_CONNECTION_STRING_CONFIG_NAME = "redisConnectionString";

    public static IServiceCollection AddBasketSercices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IBasketRepository, BasketRepository>();
        services.AddTransient<IBasketService, BasketService>();
        services.AddScoped(services =>
        {
            var connectionString = configuration.GetConnectionString(REDIS_CONNECTION_STRING_CONFIG_NAME);

            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
            var database = connectionMultiplexer.GetDatabase();
            return database;
        });

        return services;
    }
}