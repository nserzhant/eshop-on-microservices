using EShop.Catalog.Core.Models;

namespace EShop.Catalog.Core.Interfaces;

public interface ICatalogItemRepository
{
    internal Task CreateCatalogItemAsync(CatalogItem catalogItemToCreate);
    Task<CatalogItem?> GetCatalogItemAsync(Guid catalogItemId);
    internal void UpdateCatalogItem(CatalogItem catalogItemToUpdate);
    void DeleteCatalogItem(CatalogItem catalogItem);
    Task<bool> CatalogItemsWithTypeExistAsync(Guid catalogTypeId);
    Task<bool> CatalogItemsWithBrandExistAsync(Guid catalogBrandId);
    Task<CatalogItem?> GetCatalogItemAsync(string catalogItemName, Guid catalogTypeId, Guid catalogBrandId);
    Task SaveChangesAsync();
}
