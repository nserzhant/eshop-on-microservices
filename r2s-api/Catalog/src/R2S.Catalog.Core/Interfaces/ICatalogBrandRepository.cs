using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Core.Interfaces;

public interface ICatalogBrandRepository
{
    Task CreateCatalogBrandAsync(CatalogBrand catalogBrandToCreate);
    Task<CatalogBrand?> GetCatalogBrandAsync(Guid catalogBrandId);
    void UpdateCatalogBrand(CatalogBrand catalogBrandToUpdate);
    internal void DeleteCatalogBrand(CatalogBrand catalogBrand);
    Task SaveChangesAsync();
}
