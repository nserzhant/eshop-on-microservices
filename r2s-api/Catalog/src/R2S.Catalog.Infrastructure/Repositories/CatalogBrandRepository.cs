using Microsoft.EntityFrameworkCore;
using R2S.Catalog.Core.Interfaces;
using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Infrastructure.Repositories;

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
            .FirstOrDefaultAsync(cb => cb.Brand == catalogBrandName);

        return item;
    }

    public async Task SaveChangesAsync()
    {
        await _catalogDbContext.SaveChangesAsync();
    }
}
