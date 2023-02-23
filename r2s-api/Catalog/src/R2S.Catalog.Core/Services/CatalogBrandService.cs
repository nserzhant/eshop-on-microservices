using R2S.Catalog.Core.Exceptions;
using R2S.Catalog.Core.Interfaces;
using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Core.Services;

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
        var doesCatalogItemsExists = await _catalogItemRepository.DoesCatalogItemsWithBrandExistsAsync(catalogBrand.Id);

        if (doesCatalogItemsExists)
        {
            throw new CatalogItemsForBrandExistsException();
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
