using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShop.Catalog.Api.Constants;
using EShop.Catalog.Api.Models;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;
using EShop.Catalog.Core.Services;
using EShop.Catalog.Infrastructure.Read;
using EShop.Catalog.Infrastructure.Read.Queries;
using EShop.Catalog.Infrastructure.Read.Queries.Results;
using EShop.Catalog.Infrastructure.Read.ReadModels;

namespace EShop.Catalog.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogTypeController : ControllerBase
{
    private readonly ICatalogTypeRepository _catalogTypeRepository;
    private readonly ICatalogTypeService _catalogTypeService;
    private readonly ICatalogTypeQueryService _catalogTypeQueryService;

    public CatalogTypeController(ICatalogTypeRepository catalogTypeRepository,
        ICatalogTypeQueryService catalogTypeQueryService,
        ICatalogTypeService catalogTypeService)
    {
        _catalogTypeRepository = catalogTypeRepository;
        _catalogTypeQueryService = catalogTypeQueryService;
        _catalogTypeService = catalogTypeService;
    }

    [ProducesResponseType(typeof(CatalogTypeReadModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{catalogTypeId:Guid}")]
    public async Task<IActionResult> GetCatalogType(Guid catalogTypeId)
    {
        var catalogType = await _catalogTypeQueryService.GetById(catalogTypeId);

        if (catalogType == null)
        {
            return NotFound();
        }

        return Ok(catalogType);
    }

    [ProducesResponseType(typeof(CatalogTypeReadModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CatalogDomainErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpPost()]
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Employee, Roles = Roles.SALES_MANAGER_ROLE_NAME)]
    public async Task<IActionResult> CreateCatalogTypeAsync(CatalogTypeDTO catalogType)
    {
        var catalogTypeToCreate = new CatalogType(catalogType.Type);

        await _catalogTypeService.CreateCatalogTypeAsync(catalogTypeToCreate);

        var catalogTypeCreated = await _catalogTypeQueryService.GetById(catalogTypeToCreate.Id);

        return CreatedAtAction(nameof(GetCatalogType), new { catalogTypeId = catalogTypeToCreate.Id }, catalogTypeCreated);
    }

    [ProducesResponseType(typeof(CatalogTypeReadModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CatalogDomainErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpPut("{catalogTypeId:Guid}")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Employee, Roles = Roles.SALES_MANAGER_ROLE_NAME)]
    public async Task<IActionResult> UpdateCatalogTypeAsync(Guid catalogTypeId, CatalogTypeDTO catalogType)
    {
        var catalogTypeToUpdate = await _catalogTypeRepository.GetCatalogTypeAsync(catalogTypeId);

        if (catalogTypeToUpdate == null)
        {
            return NotFound();
        }

        catalogTypeToUpdate.UpdateType(catalogType.Type);
        catalogTypeToUpdate.UpdateTs(catalogType.Ts);

        await _catalogTypeService.UpdateCatalogTypeAsync(catalogTypeToUpdate);

        var catalogTypeUpdated = await _catalogTypeQueryService.GetById(catalogTypeId);

        return CreatedAtAction(nameof(GetCatalogType), new { catalogTypeId = catalogTypeId }, catalogTypeUpdated);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CatalogDomainErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpDelete("{catalogTypeId:Guid}")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Employee, Roles = Roles.SALES_MANAGER_ROLE_NAME)]
    public async Task<IActionResult> DeleteCatalogTypeAsync(Guid catalogTypeId)
    {
        var catalogType = await _catalogTypeRepository.GetCatalogTypeAsync(catalogTypeId);

        if (catalogType == null)
        {
            return NotFound();
        }

        await _catalogTypeService.DeleteCatalogTypeAsync(catalogType);

        return NoContent();
    }

    [ProducesResponseType(typeof(ListCatalogTypeResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("list")]
    public async Task<IActionResult> GetCatalogTypesAsync([FromQuery] ListCatalogTypeQuery listCatalogTypeQuery)
    {
        var result = await _catalogTypeQueryService.GetCatalogTypes(listCatalogTypeQuery);

        return Ok(result);
    }
}
