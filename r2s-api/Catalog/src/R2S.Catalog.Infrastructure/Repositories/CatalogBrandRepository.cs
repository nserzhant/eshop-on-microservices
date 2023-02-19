using Microsoft.EntityFrameworkCore;
using R2S.Catalog.Core.Interfaces;
using R2S.Catalog.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public async Task SaveChangesAsync()
    {
        await _catalogDbContext.SaveChangesAsync();
    }
}
