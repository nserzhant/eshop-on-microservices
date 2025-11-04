using EShop.Catalog.Core.Exceptions;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;

namespace EShop.Catalog.Core.Services;

public interface ICatalogTypeService
{
    Task CreateCatalogTypeAsync(CatalogType catalogType);
    Task UpdateCatalogTypeAsync(CatalogType catalogType);
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
        var catalogItemsExist = await _catalogItemRepository.CatalogItemsWithTypeExistAsync(catalogType.Id);

        if (catalogItemsExist)
        {
            throw new CatalogItemsForTypeExistException();
        }

        _catalogTypeRepository.DeleteCatalogType(catalogType);

        await _catalogTypeRepository.SaveChangesAsync();
    }

    public async Task CreateCatalogTypeAsync(CatalogType catalogType)
    {
        var catalogTypeExists = await _catalogTypeRepository.GetCatalogTypeByNameAsync(catalogType.Type);

        if (catalogTypeExists != null)
        {
            throw new CatalogTypeAlreadyExistsException();
        }

        await _catalogTypeRepository.CreateCatalogTypeAsync(catalogType);
        await _catalogTypeRepository.SaveChangesAsync();
    }

    public async Task UpdateCatalogTypeAsync(CatalogType catalogType)
    {
        var catalogTypeExists = await _catalogTypeRepository.GetCatalogTypeByNameAsync(catalogType.Type);

        if (catalogTypeExists != null && catalogTypeExists.Id != catalogType.Id)
        {
            throw new CatalogTypeAlreadyExistsException();
        }

        _catalogTypeRepository.UpdateCatalogType(catalogType);

        await _catalogTypeRepository.SaveChangesAsync();
    }
}
