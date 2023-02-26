using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using R2S.Catalog.Api.Constants;
using R2S.Catalog.Api.Models;
using R2S.Catalog.Core.Interfaces;
using R2S.Catalog.Core.Models;
using R2S.Catalog.Core.Services;
using R2S.Catalog.Infrastructure.Read;
using R2S.Catalog.Infrastructure.Read.Queries;
using R2S.Catalog.Infrastructure.Read.Queries.Results;
using R2S.Catalog.Infrastructure.Read.ReadModels;

namespace R2S.Catalog.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogItemController : ControllerBase
{
    private readonly ICatalogItemRepository _catalogItemRepository;
    private readonly ICatalogItemService _catalogItemService;
    private readonly ICatalogItemQueryService _catalogItemQueryService;

    public CatalogItemController(ICatalogItemRepository catalogItemRepository, 
        ICatalogItemQueryService catalogItemQueryService,
        ICatalogItemService catalogItemService)
    {
        _catalogItemRepository = catalogItemRepository;
        _catalogItemQueryService = catalogItemQueryService;
        _catalogItemService = catalogItemService;
    }

    [ProducesResponseType(typeof(CatalogItemReadModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{catalogItemId:Guid}")]
    [Authorize]
    public async Task<IActionResult> GetCatalogItem(Guid catalogItemId)
    {
        var catalogItem = await _catalogItemQueryService.GetById(catalogItemId);

        if (catalogItem == null)
        {
            return NotFound();
        }

        return Ok(catalogItem);
    }

    [ProducesResponseType(typeof(CatalogItemReadModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CatalogDomainErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpPost()]
    [Authorize(Roles = Roles.SALES_MANAGER_ROLE_NAME)]
    public async Task<IActionResult> CreateCatalogItemAsync(CatalogItemDTO catalogItem)
    {
        var catalogItemToCreate = new CatalogItem(catalogItem.Name, catalogItem.TypeId, catalogItem.BrandId);

        catalogItemToCreate.Description = catalogItem.Description;
        catalogItemToCreate.PictureUri = catalogItem.PictureUri;
        catalogItemToCreate.UpdatePrice(catalogItem.Price);
        catalogItemToCreate.UpdateAvailableQty(catalogItem.AvailableQty);

        await _catalogItemService.CreateCatalogItemAsync(catalogItemToCreate);

        var catalogItemCreated = await _catalogItemQueryService.GetById(catalogItemToCreate.Id);

        return CreatedAtAction(nameof(GetCatalogItem), new { catalogItemId = catalogItemToCreate.Id }, catalogItemCreated);
    }

    [ProducesResponseType(typeof(CatalogItemReadModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CatalogDomainErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpPut("{catalogItemId:Guid}")]
    [Authorize(Roles = Roles.SALES_MANAGER_ROLE_NAME)]
    public async Task<IActionResult> UpdateCatalogItemAsync(Guid catalogItemId, CatalogItemDTO catalogItem)
    {
        var catalogItemToUpdate = await _catalogItemRepository.GetCatalogItemAsync(catalogItemId);

        if (catalogItemToUpdate == null)
        {
            return NotFound();
        }

        catalogItemToUpdate.UpdateName(catalogItem.Name);
        catalogItemToUpdate.UpdateType(catalogItem.TypeId);
        catalogItemToUpdate.UpdateBrand(catalogItem.BrandId);
        catalogItemToUpdate.UpdatePrice(catalogItem.Price);
        catalogItemToUpdate.UpdateTs(catalogItem.Ts);
        catalogItemToUpdate.UpdateAvailableQty(catalogItem.AvailableQty);
        catalogItemToUpdate.PictureUri = catalogItem.PictureUri;
        catalogItemToUpdate.Description = catalogItem.Description;

        await _catalogItemService.UpdateCatalogItemAsync(catalogItemToUpdate);
        await _catalogItemRepository.SaveChangesAsync();

        var catalogItemUpdated = await _catalogItemQueryService.GetById(catalogItemId);

        return CreatedAtAction(nameof(GetCatalogItem), new { catalogItemId = catalogItemId }, catalogItemUpdated);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CatalogDomainErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpDelete("{catalogItemId:Guid}")]
    [Authorize(Roles = Roles.SALES_MANAGER_ROLE_NAME)]
    public async Task<IActionResult> DeleteCatalogItemAsync(Guid catalogItemId)
    {
        var catalogItem = await _catalogItemRepository.GetCatalogItemAsync(catalogItemId);

        if (catalogItem == null)
        {
            return NotFound();
        }

        _catalogItemRepository.DeleteCatalogItem(catalogItem);
        await _catalogItemRepository.SaveChangesAsync();

        return NoContent();
    }

    [ProducesResponseType(typeof(ListCatalogItemResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("list")]
    [Authorize]
    public async Task<IActionResult> GetCatalogItemsAsync([FromQuery] ListCatalogItemQuery listCatalogItemQuery)
    {
        var result = await _catalogItemQueryService.GetCatalogItems(listCatalogItemQuery);

        return Ok(result);
    }
}
