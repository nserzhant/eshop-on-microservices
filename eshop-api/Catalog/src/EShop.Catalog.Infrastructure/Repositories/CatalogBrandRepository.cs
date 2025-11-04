using Microsoft.EntityFrameworkCore;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;

namespace EShop.Catalog.Infrastructure.Repositories;

public class CatalogBrandRepository : ICatalogBrandRepository
{
    private readonly CatalogDbContext _catalogDbContext;

    public CatalogBrandRepository(CatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }

    public async Task CreateCatalogBrandAsync(CatalogBrand catalogBrandToCreate)
    {
        await _catalogDbContext.AddAsync(catalogBrandToCreate);
    }

    public void UpdateCatalogBrand(CatalogBrand catalogBrandToUpdate)
    {
        _catalogDbContext.Update(catalogBrandToUpdate);
    }

    public void DeleteCatalogBrand(CatalogBrand catalogBrand)
    {
        _catalogDbContext.Remove(catalogBrand);
    }

    public async Task<CatalogBrand?> GetCatalogBrandAsync(Guid catalogBrandId)
    {
        var item = await _catalogDbContext.CatalogBrands
            .FirstOrDefaultAsync(cb => cb.Id == catalogBrandId);

        return item;
    }

    public async Task<CatalogBrand?> GetCatalogBrandByNameAsync(string catalogBrandName)
    {
        var item = await _catalogDbContext.CatalogBrands
            .AsNoTracking()
            .FirstOrDefaultAsync(cb => cb.Brand == catalogBrandName);

        return item;
    }

    public async Task SaveChangesAsync()
    {
        await _catalogDbContext.SaveChangesAsync();
    }
}
