using Microsoft.Extensions.DependencyInjection;
using R2S.Catalog.Core.Interfaces;
using R2S.Catalog.Core.Models;
using R2S.Catalog.Core.Services;
using R2S.Catalog.Infrastructure.Read;
using R2S.Catalog.Infrastructure.Read.Queries;

namespace R2S.Catalog.Infrastructure.IntegrationTests;

[TestFixture]
[Category("Catalog Type")]
public class CatalogTypeIntegrationTests : BaseCatalogIntegrationTests
{
    private ICatalogTypeRepository _catalogTypeRepository;
    private ICatalogTypeService _catalogTypeService;
    private ICatalogTypeQueryService _catalogTypeQueryService;

    [SetUp]
    public override async Task SetupAsync()
    {
        await base.SetupAsync();

        _catalogTypeRepository = serviceProvider.GetRequiredService<ICatalogTypeRepository>(); 
        _catalogTypeService = serviceProvider.GetRequiredService<ICatalogTypeService>();
        _catalogTypeQueryService = serviceProvider.GetRequiredService<ICatalogTypeQueryService>();
    }

    [Test]
    [Category("Catalog Type Repository")]
    public async Task When_Save_Catalog_Type_Than_Id_Should_Be_Generated()
    {
        var catalogTypeToCreate = new CatalogType("test catalog type");

        await _catalogTypeRepository.CreateCatalogTypeAsync(catalogTypeToCreate);
        await _catalogTypeRepository.SaveChangesAsync();

        Assert.That(catalogTypeToCreate.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    [Category("Catalog Type Repository")]
    public async Task When_Save_Catalog_Type_Then_It_Could_Be_Retreived_By_Id()
    {
        var catalogTypeToCreate = new CatalogType("test catalog type");
        await _catalogTypeRepository.CreateCatalogTypeAsync(catalogTypeToCreate);
        await _catalogTypeRepository.SaveChangesAsync();

        var catalogTypeSaved = await _catalogTypeRepository.GetCatalogTypeAsync(catalogTypeToCreate.Id);

        Assert.That(catalogTypeSaved, Is.Not.Null);
        Assert.That(catalogTypeSaved.Id, Is.EqualTo(catalogTypeToCreate.Id));
        Assert.That(catalogTypeSaved.Type, Is.EqualTo("test catalog type"));
    }

    [Test]
    [Category("Catalog Type Repository")]
    [Category("Catalog Type Query Service")]
    public async Task When_Update_Catalog_Type_Then_It_Could_Be_Retreived_By_Id()
    {
        string catalogTypeName = "test catalog type";
        string updatedCatalogTypeName = "updated catalog type";
        CatalogType catalogType = await createCatalogTypeAsync(catalogTypeName);
        await _catalogTypeRepository.SaveChangesAsync();
        catalogType.UpdateType(updatedCatalogTypeName);

        _catalogTypeRepository.UpdateCatalogType(catalogType);
        await _catalogTypeRepository.SaveChangesAsync();

        var catalogTypeUpdated = await _catalogTypeQueryService.GetById(catalogType.Id);
        Assert.That(catalogTypeUpdated, Is.Not.Null);
        Assert.That(catalogTypeUpdated.Type, Is.EqualTo(updatedCatalogTypeName));
    }

    [Test]
    [Category("Catalog Type Repository")]
    public async Task When_Delete_Catalog_Type_Then_It_Could_Not_Be_Retreived_By_Id()
    {
        string catalogTypeName = "test catalog type";
        CatalogType catalogType = await createCatalogTypeAsync(catalogTypeName);
        await _catalogTypeRepository.SaveChangesAsync();

        await _catalogTypeService.DeleteCatalogTypeAsync(catalogType);

        var catalogTypeUpdated = await _catalogTypeQueryService.GetById(catalogType.Id);
        Assert.That(catalogTypeUpdated, Is.Null);
    }

    [Test]
    [Category("Catalog Type Query Service")]
    public async Task When_Types_Exists_Then_They_Could_Be_Requested_By_List_Query()
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
    public async Task When_Catalog_Type_Exists_Then_It_Could_Be_Queried_By_Id()
    {
        var catalogTypeName = "test catalog type";
        var catalogTypeToCreate = new CatalogType(catalogTypeName);
        await _catalogTypeRepository.CreateCatalogTypeAsync(catalogTypeToCreate);
        await _catalogTypeRepository.SaveChangesAsync();

        var catalogTypeReadModel = await _catalogTypeQueryService.GetById(catalogTypeToCreate.Id);

        Assert.That(catalogTypeReadModel, Is.Not.Null);
        Assert.That(catalogTypeReadModel.Id, Is.EqualTo(catalogTypeToCreate.Id));
        Assert.That(catalogTypeReadModel.Type, Is.EqualTo(catalogTypeName));
        Assert.That(catalogTypeReadModel.Ts, Is.EqualTo(catalogTypeToCreate.Ts));
    }
}
