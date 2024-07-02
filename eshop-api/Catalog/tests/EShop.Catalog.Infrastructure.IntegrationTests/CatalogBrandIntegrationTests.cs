using Microsoft.Extensions.DependencyInjection;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;
using EShop.Catalog.Core.Services;
using EShop.Catalog.Infrastructure.Read;
using EShop.Catalog.Infrastructure.Read.Queries;

namespace EShop.Catalog.Infrastructure.IntegrationTests;

[TestFixture]
[Category("Catalog Brand")]
public class CatalogBrandIntegrationTests : BaseCatalogIntegrationTests
{
    private ICatalogBrandRepository _catalogBrandRepository;
    private ICatalogBrandService _catalogBrandService;
    private ICatalogBrandQueryService _catalogBrandQueryService;

    [SetUp]
    public override async Task SetupAsync()
    {
        await base.SetupAsync();

        _catalogBrandRepository = serviceProvider.GetRequiredService<ICatalogBrandRepository>();
        _catalogBrandService = serviceProvider.GetRequiredService<ICatalogBrandService>();
        _catalogBrandQueryService = serviceProvider.GetRequiredService<ICatalogBrandQueryService>();
    }

    [Test]
    [Category("Catalog Brand Repository")]
    public async Task When_Save_Catalog_Brand_Than_Id_Should_Be_Generated()
    {
        var catalogBrandToCreate = new CatalogBrand("test brand");

        await _catalogBrandService.CreateCatalogBrandAsync(catalogBrandToCreate);

        Assert.That(catalogBrandToCreate.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    [Category("Catalog Brand Repository")]
    public async Task When_Save_Catalog_Brand_Exists_It_Could_Be_Retreived_By_Id()
    {
        var catalogBrandToCreate = new CatalogBrand("test brand");
        await _catalogBrandService.CreateCatalogBrandAsync(catalogBrandToCreate);

        var catalogBrandSaved = await _catalogBrandRepository.GetCatalogBrandAsync(catalogBrandToCreate.Id);

        Assert.That(catalogBrandSaved, Is.Not.Null);
        Assert.That(catalogBrandSaved.Id, Is.EqualTo(catalogBrandToCreate.Id));
        Assert.That(catalogBrandSaved.Brand, Is.EqualTo("test brand"));
    }

    [Test]
    [Category("Catalog Brand Repository")]
    public async Task When_Save_Catalog_Brand_Exists_It_Could_Be_Retreived_By_Name()
    {
        var catalogBrandName = "test brand";
        var catalogBrandToCreate = new CatalogBrand(catalogBrandName);
        await _catalogBrandService.CreateCatalogBrandAsync(catalogBrandToCreate);

        var catalogBrandSaved = await _catalogBrandRepository.GetCatalogBrandByNameAsync(catalogBrandName);

        Assert.That(catalogBrandSaved, Is.Not.Null);
        Assert.That(catalogBrandSaved.Id, Is.EqualTo(catalogBrandToCreate.Id));
        Assert.That(catalogBrandSaved.Brand, Is.EqualTo("test brand"));
    }

    [Test]
    [Category("Catalog Brand Repository")]
    [Category("Catalog Brand Query Service")]
    public async Task When_Update_Catalog_Brand_Then_It_Could_Be_Retreived_By_Id()
    {
        var catalogBrandName = "catalog brand";
        var catalogBrand = await createCatalogBrandAsync(catalogBrandName);
        catalogBrand.UpdateBrand("updated brand");

        await _catalogBrandService.UpdateCatalogBrandAsync(catalogBrand);

        var catalogBrandUpdated = await _catalogBrandQueryService.GetById(catalogBrand.Id);
        Assert.That(catalogBrandUpdated, Is.Not.Null);
        Assert.That(catalogBrandUpdated.Brand, Is.EqualTo(catalogBrandUpdated.Brand));
    }

    [Test]
    [Category("Catalog Brand Repository")]
    public async Task When_Delete_Catalog_Brand_Then_It_Could_Not_Be_Retreived_By_Id()
    {
        var catalogBrandName = "catalog brand";
        var catalogBrand = await createCatalogBrandAsync(catalogBrandName);
        await _catalogBrandRepository.SaveChangesAsync();
        

        await _catalogBrandService.DeleteCatalogBrandAsync(catalogBrand);

        var catalogBrandUpdated = await _catalogBrandQueryService.GetById(catalogBrand.Id);
        Assert.That(catalogBrandUpdated, Is.Null);
    }

    [Test]
    [Category("Catalog Brand Query Service")]
    public async Task When_Brands_Exists_Then_They_Could_Be_Requested_By_List_Query()
    {
        await createCatalogBrandAsync("A Brand");
        await createCatalogBrandAsync("B Brand");
        await createCatalogBrandAsync("C Brand");

        var listCatalogBrandQuery = new ListCatalogBrandQuery()
        {
            OrderByDirection = OrderByDirections.ASC,
            PageIndex = 0,
            PageSize = 2
        };

        var resultAsc = await _catalogBrandQueryService.GetCatalogBrands(listCatalogBrandQuery);
        listCatalogBrandQuery.OrderByDirection = OrderByDirections.DESC;
        var resultDesc = await _catalogBrandQueryService.GetCatalogBrands(listCatalogBrandQuery);

        Assert.That(resultAsc, Is.Not.Null);
        Assert.That(resultAsc.TotalCount, Is.EqualTo(3));
        Assert.That(resultAsc.CatalogBrands.Count, Is.EqualTo(2));
        Assert.That(resultAsc.CatalogBrands[0].Brand, Is.EqualTo("A Brand"));

        Assert.That(resultDesc, Is.Not.Null);
        Assert.That(resultDesc.TotalCount, Is.EqualTo(3));
        Assert.That(resultDesc.CatalogBrands.Count, Is.EqualTo(2));
        Assert.That(resultDesc.CatalogBrands[0].Brand, Is.EqualTo("C Brand"));
    }

    [Test]
    [Category("Catalog Brand Query Service")]
    public async Task When_Page_Size_Is_Zero_Then_All_Brands_Should_Be_ReturnedAsync()
    {
        await createCatalogBrandAsync("A Brand");
        await createCatalogBrandAsync("B Brand");
        await createCatalogBrandAsync("C Brand");
        var listCatalogBrandQuery = new ListCatalogBrandQuery()
        {
            OrderByDirection = OrderByDirections.ASC,
            PageIndex = 0,
            PageSize = 0
        };

        var result = await _catalogBrandQueryService.GetCatalogBrands(listCatalogBrandQuery);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.TotalCount, Is.EqualTo(3));
        Assert.That(result.CatalogBrands.Count, Is.EqualTo(3));
    }

    [Test]
    [Category("Catalog Brand Query Service")]
    public async Task When_Catalog_Brand_Exists_Then_It_Could_Be_Queried_By_Id()
    {
        var catalogBrandToCreate = new CatalogBrand("test brand");
        await _catalogBrandService.CreateCatalogBrandAsync(catalogBrandToCreate);

        var catalogBrandReadModel = await _catalogBrandQueryService.GetById(catalogBrandToCreate.Id);

        Assert.That(catalogBrandReadModel, Is.Not.Null);
        Assert.That(catalogBrandReadModel.Id, Is.EqualTo(catalogBrandToCreate.Id));
        Assert.That(catalogBrandReadModel.Brand, Is.EqualTo("test brand"));
        Assert.That(catalogBrandReadModel.Ts, Is.EqualTo(catalogBrandToCreate.Ts));
    }
}