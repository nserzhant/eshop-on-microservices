using Microsoft.EntityFrameworkCore;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;

namespace EShop.Catalog.Infrastructure.Repositories;

public class CatalogItemRepository : ICatalogItemRepository
{
    private readonly CatalogDbContext _catalogDbContext;

    public CatalogItemRepository(CatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }

    public async Task CreateCatalogItemAsync(CatalogItem catalogItemToCreate)
    {
        await _catalogDbContext.AddAsync(catalogItemToCreate);
    }

    public void UpdateCatalogItem(CatalogItem catalogItemToUpdate)
    {
        _catalogDbContext.Update(catalogItemToUpdate);
    }

    public void DeleteCatalogItem(CatalogItem catalogItem)
    {
        _catalogDbContext.Remove(catalogItem);
    }

    public async Task<CatalogItem?> GetCatalogItemAsync(Guid catalogItemId)
    {
        var item = await _catalogDbContext.CatalogItems
            .AsNoTracking()
            .FirstOrDefaultAsync(ci => ci.Id == catalogItemId);

        return item;
    }

    public async Task<bool> DoesCatalogItemsWithTypeExistsAsync(Guid catalogTypeId)
    {
        var exists = await _catalogDbContext.CatalogItems
            .AsNoTracking()
            .AnyAsync(ci => ci.CatalogTypeId == catalogTypeId);

        return exists;
    }

    public async Task<bool> DoesCatalogItemsWithBrandExistsAsync(Guid catalogBrandId)
    {
        var exists = await _catalogDbContext.CatalogItems
            .AsNoTracking()
            .AnyAsync(ci => ci.CatalogBrandId == catalogBrandId);

        return exists;
    }

    public async Task<CatalogItem?> GetCatalogItemAsync(string catalogItemName, Guid catalogTypeId, Guid catalogBrandId)
    {
        var item = await _catalogDbContext.CatalogItems
            .AsNoTracking()
            .FirstOrDefaultAsync(ci => ci.Name == catalogItemName &&
            ci.CatalogTypeId == catalogTypeId &&
            ci.CatalogBrandId == catalogBrandId );

        return item;
    }

    public async Task SaveChangesAsync()
    {
        await _catalogDbContext.SaveChangesAsync();
    }
}
