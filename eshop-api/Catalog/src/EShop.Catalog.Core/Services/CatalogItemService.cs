using EShop.Catalog.Core.Exceptions;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;

namespace EShop.Catalog.Core.Services;

public interface ICatalogItemService
{
    Task CreateCatalogItemAsync(CatalogItem catalogItem);
    Task UpdateCatalogItemAsync(CatalogItem catalogItem);
}

public class CatalogItemService : ICatalogItemService
{
    private readonly ICatalogBrandRepository _catalogBrandRepository;
    private readonly ICatalogTypeRepository _catalogTypeRepository;
    private readonly ICatalogItemRepository _catalogItemRepository;

    public CatalogItemService(ICatalogBrandRepository catalogBrandRepository, ICatalogTypeRepository catalogTypeRepository, ICatalogItemRepository catalogItemRepository)
    {
        _catalogBrandRepository = catalogBrandRepository;
        _catalogTypeRepository = catalogTypeRepository;
        _catalogItemRepository = catalogItemRepository;
    }

    public async Task CreateCatalogItemAsync(CatalogItem catalogItem)
    {
        await validateCatalogItem(catalogItem);

        await _catalogItemRepository.CreateCatalogItemAsync(catalogItem);
        await _catalogItemRepository.SaveChangesAsync();
    }

    public async Task UpdateCatalogItemAsync(CatalogItem catalogItem)
    {
        await validateCatalogItem(catalogItem);

        _catalogItemRepository.UpdateCatalogItem(catalogItem);
        await _catalogItemRepository.SaveChangesAsync();
    }

    private async Task validateCatalogItem(CatalogItem catalogItem)
    {
        var catalogBrand = await _catalogBrandRepository.GetCatalogBrandAsync(catalogItem.CatalogBrandId);
        var catalogType = await _catalogTypeRepository.GetCatalogTypeAsync(catalogItem.CatalogTypeId);
        var catalogItemAlreadyExists = await _catalogItemRepository.GetCatalogItemAsync(catalogItem.Name,
            catalogItem.CatalogTypeId, catalogItem.CatalogBrandId);

        if (catalogBrand == null)
        {
            throw new CatalogBrandNotExistsException();
        }

        if (catalogType == null)
        {
            throw new CatalogTypeNotExistsException();
        }

        if(catalogItemAlreadyExists != null && catalogItemAlreadyExists.Id != catalogItem.Id)
        {
            throw new CatalogItemAlreadyExistsException();
        }
    }
}
