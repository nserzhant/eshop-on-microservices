using EShop.Catalog.Core.Models;

namespace EShop.Catalog.Core.Interfaces;

public interface ICatalogTypeRepository
{
    internal Task CreateCatalogTypeAsync(CatalogType catalogTypeToCreate);
    internal void UpdateCatalogType(CatalogType catalogTypeToUpdate);
    Task<CatalogType?> GetCatalogTypeAsync(Guid catalogTypeId);
    internal void DeleteCatalogType(CatalogType catalogType);
    Task<CatalogType?> GetCatalogTypeByNameAsync(string catalogTypeName);
    Task SaveChangesAsync();
}
