using EShop.Ordering.Core.Interfaces;
using EShop.Ordering.Infrastructure.Read;
using EShop.Ordering.Infrastructure.Repositories;
using EShop.Ordering.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Ordering.Infrastructure;
public static class OrderingServicesRegistrationExtension
{
    public static IServiceCollection AddOrderingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IOrderRepository, OrderRepository>();
        services.AddTransient<IOrderQueryService, OrderQueryService>();
        services.AddTransient<IDateTimeService, DateTimeService>();

        services.AddDbContext<OrderingDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString(DbConsts.ORDERING_DB_CONNECTION_STRING_NAME)
                , x => x.MigrationsHistoryTable("_EFMigrationsHistory", DbConsts.ORDERING_DB_SCHEMA_NAME));
        });

        services.AddDbContext<OrderingReadDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString(DbConsts.ORDERING_DB_CONNECTION_STRING_NAME));
        });

        return services;
    }
} 