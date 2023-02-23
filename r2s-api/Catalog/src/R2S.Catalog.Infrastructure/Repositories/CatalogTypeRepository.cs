using Microsoft.EntityFrameworkCore;
using R2S.Catalog.Core.Interfaces;
using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Infrastructure.Repositories;

public class CatalogTypeRepository : ICatalogTypeRepository
{
    private readonly CatalogDbContext _catalogDbContext;

    public CatalogTypeRepository(CatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }

    public async Task CreateCatalogTypeAsync(CatalogType catalogTypeToCreate)
    {
        await _catalogDbContext.AddAsync(catalogTypeToCreate);
    }

    public void UpdateCatalogType(CatalogType catalogTypeToUpdate)
    {
        _catalogDbContext.Update(catalogTypeToUpdate);
    }

    public void DeleteCatalogType(CatalogType catalogType)
    {
        _catalogDbContext.Remove(catalogType);
    }

    public async Task<CatalogType?> GetCatalogTypeAsync(Guid catalogTypeId)
    {
        var catalogType = await _catalogDbContext.CatalogTypes
            .FirstOrDefaultAsync(ct => ct.Id == catalogTypeId);

        return catalogType;
    }

    public async Task<CatalogType?> GetCatalogTypeByNameAsync(string catalogTypeName)
    {
        var catalogType = await _catalogDbContext.CatalogTypes
            .FirstOrDefaultAsync(ct => ct.Type == catalogTypeName);

        return catalogType;
    }

    public async Task SaveChangesAsync()
    {
        await _catalogDbContext.SaveChangesAsync();
    }
}
