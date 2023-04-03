using EShop.Catalog.Core.Models;

namespace EShop.Catalog.Core.Interfaces;

public interface ICatalogBrandRepository
{
    internal Task CreateCatalogBrandAsync(CatalogBrand catalogBrandToCreate);
    Task<CatalogBrand?> GetCatalogBrandAsync(Guid catalogBrandId);
    internal void UpdateCatalogBrand(CatalogBrand catalogBrandToUpdate);
    internal void DeleteCatalogBrand(CatalogBrand catalogBrand);
    Task<CatalogBrand?> GetCatalogBrandByNameAsync(string catalogBrandName);
    Task SaveChangesAsync();
}
