using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Services;
using EShop.Catalog.Infrastructure.Read;
using EShop.Catalog.Infrastructure.Repositories;

namespace EShop.Catalog.Infrastructure;

public static class CatalogServicesRegistrationExtention
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICatalogBrandRepository, CatalogBrandRepository>();
        services.AddTransient<ICatalogBrandService, CatalogBrandService>();
        services.AddTransient<ICatalogBrandQueryService, CatalogBrandQueryService>();

        services.AddTransient<ICatalogTypeRepository, CatalogTypeRepository>();
        services.AddTransient<ICatalogTypeService, CatalogTypeService>();
        services.AddTransient<ICatalogTypeQueryService, CatalogTypeQueryService>();

        services.AddTransient<ICatalogItemRepository, CatalogItemRepository>();
        services.AddTransient<ICatalogItemService, CatalogItemService>();
        services.AddTransient<ICatalogItemQueryService, CatalogItemQueryService>();

        services.AddDbContext<CatalogDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString(DbConsts.CATALOG_DB_CONNECTION_STRING_NAME)
                , x => x.MigrationsHistoryTable("_EFMigrationsHistory", DbConsts.CATALOG_DB_SCHEMA_NAME));
        });

        services.AddDbContext<CatalogReadDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString(DbConsts.CATALOG_DB_CONNECTION_STRING_NAME));
        });

        return services;
    }
}
