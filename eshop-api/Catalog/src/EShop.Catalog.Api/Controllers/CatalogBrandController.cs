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
public class CatalogBrandController : ControllerBase
{
    private readonly ICatalogBrandRepository _catalogBrandRepository;
    private readonly ICatalogBrandService _catalogBrandService;
    private readonly ICatalogBrandQueryService _catalogBrandQueryService;

    public CatalogBrandController(ICatalogBrandRepository catalogBrandRepository, 
        ICatalogBrandQueryService catalogBrandQueryService,
        ICatalogBrandService catalogBrandService)
    {
        _catalogBrandRepository = catalogBrandRepository;
        _catalogBrandQueryService = catalogBrandQueryService;
        _catalogBrandService = catalogBrandService;
    }

    [ProducesResponseType(typeof(CatalogBrandReadModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CatalogDomainErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpGet("{catalogBrandId:Guid}")]
    public async Task<IActionResult> GetCatalogBrand(Guid catalogBrandId)
    {
        var catalogBrand = await _catalogBrandQueryService.GetById(catalogBrandId);

        if (catalogBrand == null)
        {
            return NotFound();
        }

        return Ok(catalogBrand);
    }

    [ProducesResponseType(typeof(CatalogBrandReadModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost()]
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Employee, Roles = Roles.SALES_MANAGER_ROLE_NAME)]
    public async Task<IActionResult> CreateCatalogBrandAsync(CatalogBrandDTO catalogBrand)
    {
        var catalogBrandToCreate = new CatalogBrand(catalogBrand.Brand);

        await _catalogBrandService.CreateCatalogBrandAsync(catalogBrandToCreate);

        var catalogBrandCreated = await _catalogBrandQueryService.GetById(catalogBrandToCreate.Id);

        return CreatedAtAction(nameof(GetCatalogBrand), new { catalogBrandId = catalogBrandToCreate.Id }, catalogBrandCreated);
    }

    [ProducesResponseType(typeof(CatalogBrandReadModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CatalogDomainErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpPut("{catalogBrandId:Guid}")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Employee, Roles = Roles.SALES_MANAGER_ROLE_NAME)]
    public async Task<IActionResult> UpdateCatalogBrandAsync(Guid catalogBrandId, CatalogBrandDTO catalogBrand)
    {
        var catalogBrandToUpdate = await _catalogBrandRepository.GetCatalogBrandAsync(catalogBrandId);

        if (catalogBrandToUpdate == null)
        {
            return NotFound();
        }

        catalogBrandToUpdate.UpdateBrand(catalogBrand.Brand);
        catalogBrandToUpdate.UpdateTs(catalogBrand.Ts);

        await _catalogBrandService.UpdateCatalogBrandAsync(catalogBrandToUpdate);

        var catalogBrandUpdated = await _catalogBrandQueryService.GetById(catalogBrandId);

        return CreatedAtAction(nameof(GetCatalogBrand), new { catalogBrandId = catalogBrandId }, catalogBrandUpdated);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CatalogDomainErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpDelete("{catalogBrandId:Guid}")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Employee, Roles = Roles.SALES_MANAGER_ROLE_NAME)]
    public async Task<IActionResult> DeleteCatalogBrandAsync(Guid catalogBrandId)
    {
        var catalogBrand = await _catalogBrandRepository.GetCatalogBrandAsync(catalogBrandId);

        if (catalogBrand == null)
        {
            return NotFound();
        }

        await _catalogBrandService.DeleteCatalogBrandAsync(catalogBrand);

        return NoContent();
    }

    [ProducesResponseType(typeof(ListCatalogBrandResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("list")]
    public async Task<IActionResult> GetCatalogBrandsAsync([FromQuery] ListCatalogBrandQuery listCatalogBrandQuery)
    {
        var result = await _catalogBrandQueryService.GetCatalogBrands(listCatalogBrandQuery);

        return Ok(result);
    }
}
