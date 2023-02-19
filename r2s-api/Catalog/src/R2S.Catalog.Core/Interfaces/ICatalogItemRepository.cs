using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Core.Interfaces;

public interface ICatalogItemRepository
{
    internal Task CreateCatalogItemAsync(CatalogItem catalogItemToCreate);
    Task<CatalogItem?> GetCatalogItemAsync(Guid catalogItemId);
    internal void UpdateCatalogItem(CatalogItem catalogItemToUpdate);
    void DeleteCatalogItem(CatalogItem catalogItem);
    internal Task<bool> DoesCatalogItemsWithTypeExistsAsync(Guid catalogTypeId);
    internal Task<bool> DoesCatalogItemsWithBrandExistsAsync(Guid catalogBrandId);
    Task SaveChangesAsync();
}
