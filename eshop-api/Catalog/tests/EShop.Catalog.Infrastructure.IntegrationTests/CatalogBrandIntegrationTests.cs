using EShop.Catalog.Core.Exceptions;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;
using EShop.Catalog.Core.Services;
using EShop.Catalog.Infrastructure.Read;
using EShop.Catalog.Infrastructure.Read.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
    public async Task When_Catalog_Brand_Exists_Then_It_Can_Be_Retrieved_By_Id()
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
    public async Task When_Catalog_Brand_Exists_Then_It_Can_Be_Retrieved_By_Name()
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
    public async Task When_Catalog_Brand_Is_Updated_Then_It_Can_Be_Retrieved_By_Id()
    {
        var catalogBrandName = "catalog brand";
        var catalogBrand = await createCatalogBrandAsync(catalogBrandName);
        catalogBrand.UpdateBrand("updated brand");

        await _catalogBrandService.UpdateCatalogBrandAsync(catalogBrand);

        var catalogBrandUpdated = await _catalogBrandQueryService.GetById(catalogBrand.Id);
        Assert.That(catalogBrandUpdated, Is.Not.Null);
        Assert.That(catalogBrandUpdated.Brand, Is.EqualTo("updated brand"));
    }


    [Test]
    [Category("Optimistic Concurrency Check")]
    [Category("Catalog Brand Repository")]
    public async Task When_Catalog_Brand_Is_Updated_In_Parallel_Then_Update_Fails()
    {
        var catalogBrandName = "catalog brand";
        var catalogBrand = await createCatalogBrandAsync(catalogBrandName);
        var initialVersion = catalogBrand.Ts;
        catalogBrand.UpdateBrand("updated brand");
        await _catalogBrandService.UpdateCatalogBrandAsync(catalogBrand);
        // Set the original version to emulate parallel update
        catalogBrand.UpdateTs(initialVersion);

        var actUpdate = async () => await _catalogBrandService.UpdateCatalogBrandAsync(catalogBrand);

        Assert.That(actUpdate, Throws.TypeOf<DbUpdateConcurrencyException>());
    }

    [Test]
    [Category("Catalog Brand Repository")]
    public async Task When_Catalog_Brand_Is_Deleted_Then_It_Can_Not_Be_Retrieved()
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
    public async Task When_Catalog_Brands_Exist_Then_They_Can_Be_Retrieved_By_List_Query()
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
    public async Task When_Page_Size_Is_Zero_Then_All_Brands_Should_Be_Returned()
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
    public async Task When_Catalog_Brand_Exists_Then_It_Can_Be_Queried_By_Id()
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