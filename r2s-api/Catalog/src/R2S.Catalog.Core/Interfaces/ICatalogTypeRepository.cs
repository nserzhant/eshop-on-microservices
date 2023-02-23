using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Core.Interfaces;

public interface ICatalogTypeRepository
{
    internal Task CreateCatalogTypeAsync(CatalogType catalogTypeToCreate);
    internal void UpdateCatalogType(CatalogType catalogTypeToUpdate);
    Task<CatalogType?> GetCatalogTypeAsync(Guid catalogTypeId);
    internal void DeleteCatalogType(CatalogType catalogType);
    Task<CatalogType?> GetCatalogTypeByNameAsync(string catalogTypeName);
    Task SaveChangesAsync();
}
