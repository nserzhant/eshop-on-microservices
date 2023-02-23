using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Core.Interfaces;

public interface ICatalogBrandRepository
{
    internal Task CreateCatalogBrandAsync(CatalogBrand catalogBrandToCreate);
    Task<CatalogBrand?> GetCatalogBrandAsync(Guid catalogBrandId);
    internal void UpdateCatalogBrand(CatalogBrand catalogBrandToUpdate);
    internal void DeleteCatalogBrand(CatalogBrand catalogBrand);
    Task<CatalogBrand?> GetCatalogBrandByNameAsync(string catalogBrandName);
    Task SaveChangesAsync();
}
