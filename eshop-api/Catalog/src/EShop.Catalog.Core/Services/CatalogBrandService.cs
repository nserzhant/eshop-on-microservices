using EShop.Catalog.Core.Exceptions;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;

namespace EShop.Catalog.Core.Services;

public interface ICatalogBrandService
{
    Task DeleteCatalogBrandAsync(CatalogBrand catalogBrand);
    Task CreateCatalogBrandAsync(CatalogBrand catalogBrand);
    Task UpdateCatalogBrandAsync(CatalogBrand catalogBrand);
}

public class CatalogBrandService : ICatalogBrandService
{
    private readonly ICatalogItemRepository _catalogItemRepository;
    private readonly ICatalogBrandRepository _catalogBrandRepository;

    public CatalogBrandService(ICatalogItemRepository catalogItemRepository, ICatalogBrandRepository catalogBrandRepository)
    {
        _catalogItemRepository = catalogItemRepository;
        _catalogBrandRepository = catalogBrandRepository;
    }

    public async Task DeleteCatalogBrandAsync(CatalogBrand catalogBrand)
    {
        var catalogItemsExist = await _catalogItemRepository.CatalogItemsWithBrandExistAsync(catalogBrand.Id);

        if (catalogItemsExist)
        {
            throw new CatalogItemsForBrandExistException();
        }

        _catalogBrandRepository.DeleteCatalogBrand(catalogBrand);

        await _catalogBrandRepository.SaveChangesAsync();
    }

    public async Task CreateCatalogBrandAsync(CatalogBrand catalogBrand)
    {
        var catalogBrandExists = await _catalogBrandRepository.GetCatalogBrandByNameAsync(catalogBrand.Brand);

        if (catalogBrandExists != null)
        {
            throw new CatalogBrandAlreadyExistsException();
        }

        await _catalogBrandRepository.CreateCatalogBrandAsync(catalogBrand);
        await _catalogBrandRepository.SaveChangesAsync();
    }

    public async Task UpdateCatalogBrandAsync(CatalogBrand catalogBrand)
    {
        var catalogBrandExists = await _catalogBrandRepository.GetCatalogBrandByNameAsync(catalogBrand.Brand);

        if (catalogBrandExists != null && catalogBrandExists.Id != catalogBrand.Id)
        {
            throw new CatalogBrandAlreadyExistsException();
        }

        _catalogBrandRepository.UpdateCatalogBrand(catalogBrand);

        await _catalogBrandRepository.SaveChangesAsync();
    }
}
