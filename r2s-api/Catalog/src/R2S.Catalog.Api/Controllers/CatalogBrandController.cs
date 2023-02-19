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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{catalogBrandId:Guid}")]
    [Authorize]
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
    [Authorize(Roles = Roles.SALES_MANAGER_ROLE_NAME)]
    public async Task<IActionResult> CreateCatalogBrandAsync(CatalogBrandDTO catalogBrand)
    {
        var catalogBrandToCreate = new CatalogBrand(catalogBrand.Brand);

        await _catalogBrandRepository.CreateCatalogBrandAsync(catalogBrandToCreate);
        await _catalogBrandRepository.SaveChangesAsync();

        var catalogBrandCreated = await _catalogBrandQueryService.GetById(catalogBrandToCreate.Id);

        return CreatedAtAction(nameof(GetCatalogBrand), new { catalogBrandId = catalogBrandToCreate.Id }, catalogBrandCreated);
    }

    [ProducesResponseType(typeof(CatalogBrandReadModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{catalogBrandId:Guid}")]
    [Authorize(Roles = Roles.SALES_MANAGER_ROLE_NAME)]
    public async Task<IActionResult> UpdateCatalogBrandAsync(Guid catalogBrandId, CatalogBrandDTO catalogBrand)
    {
        var catalogBrandToUpdate = await _catalogBrandRepository.GetCatalogBrandAsync(catalogBrandId);

        if (catalogBrandToUpdate == null)
        {
            return NotFound();
        }

        catalogBrandToUpdate.UpdateBrand(catalogBrand.Brand);
        catalogBrandToUpdate.UpdateTs(catalogBrand.Ts);

        _catalogBrandRepository.UpdateCatalogBrand(catalogBrandToUpdate);
        await _catalogBrandRepository.SaveChangesAsync();

        var catalogBrandUpdated = await _catalogBrandQueryService.GetById(catalogBrandId);

        return CreatedAtAction(nameof(GetCatalogBrand), new { catalogBrandId = catalogBrandId }, catalogBrandUpdated);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{catalogBrandId:Guid}")]
    [Authorize(Roles = Roles.SALES_MANAGER_ROLE_NAME)]
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
    [Authorize]
    public async Task<IActionResult> GetCatalogBrandsAsync([FromQuery] ListCatalogBrandQuery listCatalogBrandQuery)
    {
        var result = await _catalogBrandQueryService.GetCatalogBrands(listCatalogBrandQuery);

        return Ok(result);
    }
}
