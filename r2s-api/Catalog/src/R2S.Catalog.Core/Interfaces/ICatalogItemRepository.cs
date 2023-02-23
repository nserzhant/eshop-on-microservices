using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Core.Interfaces;

public interface ICatalogItemRepository
{
    internal Task CreateCatalogItemAsync(CatalogItem catalogItemToCreate);
    Task<CatalogItem?> GetCatalogItemAsync(Guid catalogItemId);
    internal void UpdateCatalogItem(CatalogItem catalogItemToUpdate);
    void DeleteCatalogItem(CatalogItem catalogItem);
    Task<bool> DoesCatalogItemsWithTypeExistsAsync(Guid catalogTypeId);
    Task<bool> DoesCatalogItemsWithBrandExistsAsync(Guid catalogBrandId);
    Task<CatalogItem?> GetCatalogItemAsync(string catalogItemName, Guid catalogTypeId, Guid catalogBrandId);
    Task SaveChangesAsync();
}
