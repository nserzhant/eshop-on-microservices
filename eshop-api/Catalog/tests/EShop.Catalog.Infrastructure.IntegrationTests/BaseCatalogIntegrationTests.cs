using Microsoft.Extensions.DependencyInjection;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;
using EShop.Catalog.Core.Services;
using EShop.Catalog.Infrastructure.IntegrationTests.Extensions;

namespace EShop.Catalog.Infrastructure.IntegrationTests;

public class BaseCatalogIntegrationTests
{
    protected ServiceProvider serviceProvider;

    [SetUp]
    public virtual async Task SetupAsync()
    {
        //Setup services
        ServiceCollection sc = new ServiceCollection();

        sc.AddTestCatalogServices();

        serviceProvider = sc.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CatalogDbContext>();

        await dbContext.ClearDb();
    }

    [TearDown]
    public virtual void TearDown()
    {
        serviceProvider.Dispose();
    }

    protected async Task<CatalogType> createCatalogTypeAsync(string catalogTypeName)
    {
        // Create scoped context to prevent any side effects

        using var scopedProvider = serviceProvider.CreateScope();
        var catalogTypeRepository = scopedProvider.ServiceProvider.GetRequiredService<ICatalogTypeRepository>();
        var catalogTypeService = scopedProvider.ServiceProvider.GetRequiredService<ICatalogTypeService>();
        var catalogTypeToCreate = new CatalogType(catalogTypeName);
        
        await catalogTypeService.CreateCatalogTypeAsync(catalogTypeToCreate);

        var result = await catalogTypeRepository.GetCatalogTypeAsync(catalogTypeToCreate.Id);

        return result!;
    }

    protected async Task<CatalogBrand> createCatalogBrandAsync(string catalogBrandName)
    {
        using var scopedProvider = serviceProvider.CreateScope();
        var catalogBrandRepository = scopedProvider.ServiceProvider.GetRequiredService<ICatalogBrandRepository>();
        var catalogBrandService = scopedProvider.ServiceProvider.GetRequiredService<ICatalogBrandService>();
        var catalogBrandToCreate = new CatalogBrand(catalogBrandName);

        await catalogBrandService.CreateCatalogBrandAsync(catalogBrandToCreate);

        var result = await catalogBrandRepository.GetCatalogBrandAsync(catalogBrandToCreate.Id);

        return result!;
    }

    protected async Task<CatalogItem> createCatalogItemAsync(string catalogItemName, string? catalogBrandName = null, string? catalogTypeName = null, decimal? price = null, string? description = null)
    {
        using var scopedProvider = serviceProvider.CreateScope();
        var catalogItemService = scopedProvider.ServiceProvider.GetRequiredService<ICatalogItemService>();
        var catalogItemRepository = scopedProvider.ServiceProvider.GetRequiredService<ICatalogItemRepository>();
        var catalogBrandNameToCreate = catalogBrandName ?? Guid.NewGuid().ToString();
        var catalogTypeNameToCreate = catalogTypeName ?? Guid.NewGuid().ToString();
        var catalogBrandId = (await createCatalogBrandAsync(catalogBrandNameToCreate)).Id;
        var catalogTypeId =  (await createCatalogTypeAsync(catalogTypeNameToCreate)).Id;
        var itemPrice = price ?? 1m;
        var catalogItemToCreate = new CatalogItem(catalogItemName, catalogTypeId, catalogBrandId);

        catalogItemToCreate.UpdatePrice(itemPrice);
        catalogItemToCreate.Description = description;
        
        await catalogItemService.CreateCatalogItemAsync(catalogItemToCreate);

        var result = await catalogItemRepository.GetCatalogItemAsync(catalogItemToCreate.Id);

        return result!;
    }
}
