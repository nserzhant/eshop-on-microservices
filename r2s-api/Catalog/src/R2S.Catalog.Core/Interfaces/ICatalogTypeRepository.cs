using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Core.Interfaces;

public interface ICatalogTypeRepository
{
    Task CreateCatalogTypeAsync(CatalogType catalogTypeToCreate);
    Task<CatalogType?> GetCatalogTypeAsync(Guid catalogTypeId);
    void UpdateCatalogType(CatalogType catalogTypeToUpdate);
    internal void DeleteCatalogType(CatalogType catalogType);
    Task SaveChangesAsync();
}
