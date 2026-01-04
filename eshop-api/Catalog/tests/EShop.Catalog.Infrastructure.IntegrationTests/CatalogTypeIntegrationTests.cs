using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;
using EShop.Catalog.Core.Services;
using EShop.Catalog.Infrastructure.Read;
using EShop.Catalog.Infrastructure.Read.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Catalog.Infrastructure.IntegrationTests;

[TestFixture]
[Category("Catalog Type")]
public class CatalogTypeIntegrationTests : BaseCatalogIntegrationTests
{
    private ICatalogTypeRepository _catalogTypeRepository;
    private ICatalogTypeService _catalogTypeService;
    private ICatalogTypeQueryService _catalogTypeQueryService;

    public override async Task SetupAsync()
    {
        await base.SetupAsync();

        _catalogTypeRepository = serviceProvider.GetRequiredService<ICatalogTypeRepository>(); 
        _catalogTypeService = serviceProvider.GetRequiredService<ICatalogTypeService>();
        _catalogTypeQueryService = serviceProvider.GetRequiredService<ICatalogTypeQueryService>();
    }

    [Test]
    [Category("Catalog Type Repository")]
    public async Task When_Catalog_Type_Exists_Then_It_Can_Be_Retrieved_By_Id()
    {
        var catalogTypeToCreate = new CatalogType("test catalog type");
        await _catalogTypeService.CreateCatalogTypeAsync(catalogTypeToCreate);

        var catalogTypeSaved = await _catalogTypeRepository.GetCatalogTypeAsync(catalogTypeToCreate.Id);

        Assert.That(catalogTypeSaved, Is.Not.Null);
        Assert.That(catalogTypeSaved.Id, Is.EqualTo(catalogTypeToCreate.Id));
        Assert.That(catalogTypeSaved.Type, Is.EqualTo("test catalog type"));
    }

    [Test]
    [Category("Catalog Type Repository")]
    [Category("Catalog Type Query Service")]
    public async Task When_Catalog_Type_Is_Updated_Then_It_Can_Be_Retrieved_By_Id()
    {
        var catalogTypeName = "test catalog type";
        var updatedCatalogTypeName = "updated catalog type";
        var catalogType = await createCatalogTypeAsync(catalogTypeName);
        catalogType.UpdateType(updatedCatalogTypeName);

        await _catalogTypeService.UpdateCatalogTypeAsync(catalogType);

        var catalogTypeUpdated = await _catalogTypeQueryService.GetById(catalogType.Id);
        Assert.That(catalogTypeUpdated, Is.Not.Null);
        Assert.That(catalogTypeUpdated.Type, Is.EqualTo(updatedCatalogTypeName));
    }

    [Test]
    [Category("Optimistic Concurrency Check")]
    [Category("Catalog Type Repository")]
    public async Task When_Catalog_Type_Is_Updated_In_Parallel_Then_Update_Fails()
    {
        var catalogTypeName = "test catalog type";
        var updatedCatalogTypeName = "updated catalog type";
        var catalogType = await createCatalogTypeAsync(catalogTypeName);
        var initialVersion = catalogType.Ts;
        catalogType.UpdateType(updatedCatalogTypeName);
        await _catalogTypeService.UpdateCatalogTypeAsync(catalogType);
        // Set the original version to emulate parallel update
        catalogType.UpdateTs(initialVersion);

        var actUpdate = async () => await _catalogTypeService.UpdateCatalogTypeAsync(catalogType);

        Assert.That(actUpdate, Throws.TypeOf<DbUpdateConcurrencyException>());
    }

    [Test]
    [Category("Catalog Type Repository")]
    public async Task When_Catalog_Type_Is_Deleted_Then_It_Can_Not_Be_Retrieved()
    {
        var catalogTypeName = "test catalog type";
        var catalogType = await createCatalogTypeAsync(catalogTypeName);
        await _catalogTypeRepository.SaveChangesAsync();

        await _catalogTypeService.DeleteCatalogTypeAsync(catalogType);

        var catalogTypeUpdated = await _catalogTypeQueryService.GetById(catalogType.Id);
        Assert.That(catalogTypeUpdated, Is.Null);
    }

    [Test]
    [Category("Catalog Type Query Service")]
    public async Task When_Catalog_Types_Exist_Then_They_Can_Be_Retrieved_By_List_Query()
    {
        await createCatalogTypeAsync("A Catalog Type");
        await createCatalogTypeAsync("B Catalog Type");
        await createCatalogTypeAsync("C Catalog Type");

        var listCatalogTypeQuery = new ListCatalogTypeQuery()
        {
            OrderByDirection = OrderByDirections.ASC,
            PageIndex = 0,
            PageSize = 2
        };

        var resultAsc = await _catalogTypeQueryService.GetCatalogTypes(listCatalogTypeQuery);
        listCatalogTypeQuery.OrderByDirection = OrderByDirections.DESC;
        var resultDesc = await _catalogTypeQueryService.GetCatalogTypes(listCatalogTypeQuery);

        Assert.That(resultAsc, Is.Not.Null);
        Assert.That(resultAsc.TotalCount, Is.EqualTo(3));
        Assert.That(resultAsc.CatalogTypes.Count, Is.EqualTo(2));
        Assert.That(resultAsc.CatalogTypes[0].Type, Is.EqualTo("A Catalog Type"));
        Assert.That(resultDesc, Is.Not.Null);
        Assert.That(resultDesc.TotalCount, Is.EqualTo(3));
        Assert.That(resultDesc.CatalogTypes.Count, Is.EqualTo(2));
        Assert.That(resultDesc.CatalogTypes[0].Type, Is.EqualTo("C Catalog Type"));
    }

    [Test]
    [Category("Catalog Type Query Service")]
    public async Task When_Page_Size_Is_Zero_Then_All_Types_Should_Be_Returned()
    {
        await createCatalogTypeAsync("A Catalog Type");
        await createCatalogTypeAsync("B Catalog Type");
        await createCatalogTypeAsync("C Catalog Type");
        var listCatalogTypeQuery = new ListCatalogTypeQuery()
        {
            OrderByDirection = OrderByDirections.ASC,
            PageIndex = 0,
            PageSize = 0
        };

        var result = await _catalogTypeQueryService.GetCatalogTypes(listCatalogTypeQuery);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.TotalCount, Is.EqualTo(3));
        Assert.That(result.CatalogTypes.Count, Is.EqualTo(3));
    }

    [Test]
    [Category("Catalog Type Query Service")]
    public async Task When_Catalog_Type_Exists_Then_It_Can_Be_Queried_By_Id()
    {
        var catalogTypeName = "test catalog type";
        var catalogTypeToCreate = new CatalogType(catalogTypeName);
        await _catalogTypeService.CreateCatalogTypeAsync(catalogTypeToCreate);

        var catalogTypeReadModel = await _catalogTypeQueryService.GetById(catalogTypeToCreate.Id);

        Assert.That(catalogTypeReadModel, Is.Not.Null);
        Assert.That(catalogTypeReadModel.Id, Is.EqualTo(catalogTypeToCreate.Id));
        Assert.That(catalogTypeReadModel.Type, Is.EqualTo(catalogTypeName));
        Assert.That(catalogTypeReadModel.Ts, Is.EqualTo(catalogTypeToCreate.Ts));
    }
}
