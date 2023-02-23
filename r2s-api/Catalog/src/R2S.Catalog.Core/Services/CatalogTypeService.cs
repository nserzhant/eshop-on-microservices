using R2S.Catalog.Core.Exceptions;
using R2S.Catalog.Core.Interfaces;
using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Core.Services;

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
        var doesCatalogItemsExists = await _catalogItemRepository.DoesCatalogItemsWithTypeExistsAsync(catalogType.Id);

        if (doesCatalogItemsExists)
        {
            throw new CatalogItemsForTypeExistsException();
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
