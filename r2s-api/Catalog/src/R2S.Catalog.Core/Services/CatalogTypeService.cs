using R2S.Catalog.Core.Exceptions;
using R2S.Catalog.Core.Interfaces;
using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Core.Services;

public interface ICatalogTypeService
{
    Task DeleteCatalogTypeAsync(CatalogType catalogType);
}

public class CatalogTypeService : ICatalogTypeService
{
    private readonly ICatalogItemRepository _catalogItemRepository;
    private readonly ICatalogTypeRepository _catalogTypeRepository;

    public CatalogTypeService(ICatalogItemRepository catalogItemRepository, ICatalogTypeRepository catalogTypeRepository)
    {
        _catalogItemRepository = catalogItemRepository;
        _catalogTypeRepository = catalogTypeRepository;
    }

    public async Task DeleteCatalogTypeAsync(CatalogType catalogType)
    {
        var doesCatalogItemsExists = await _catalogItemRepository.DoesCatalogItemsWithTypeExistsAsync(catalogType.Id);

        if (doesCatalogItemsExists)
        {
            throw new CatalogItemsForTypeExistsException();
        }

        _catalogTypeRepository.DeleteCatalogType(catalogType);

        await _catalogTypeRepository.SaveChangesAsync();
    }
}
