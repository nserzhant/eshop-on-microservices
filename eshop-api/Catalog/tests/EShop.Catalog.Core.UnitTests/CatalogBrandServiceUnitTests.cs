using NSubstitute;
using EShop.Catalog.Core.Exceptions;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;
using EShop.Catalog.Core.Services;

namespace EShop.Catalog.Core.UnitTests;

[TestFixture]
[Category("Catalog Brand")]
public class CatalogBrandServiceUnitTests
{
    private CatalogBrandService _catalogBrandService;
    private ICatalogItemRepository _catalogItemRepository;
    private ICatalogBrandRepository _catalogBrandRepository;

    [SetUp]
    public void Setup()
    {
        _catalogItemRepository = Substitute.For<ICatalogItemRepository>();
        _catalogBrandRepository = Substitute.For<ICatalogBrandRepository>();
        _catalogBrandService = new CatalogBrandService(_catalogItemRepository, _catalogBrandRepository);
    }

    [Test]
    public void When_Delete_Catalog_Brand_And_Catalog_Item_With_Type_Exists_Then_Exception_Should_Be_Thrown()
    {
        var catalogBrand = new CatalogBrand("Test catalog brand");
        _catalogItemRepository.CatalogItemsWithBrandExistAsync(catalogBrand.Id).Returns(true);

        Task act() => _catalogBrandService.DeleteCatalogBrandAsync(catalogBrand);

        Assert.That(act, Throws.TypeOf<CatalogItemsForBrandExistException>());
    }

    [Test]
    public void When_Create_Catalog_Brand_With_Already_Existed_Name_Then_Exception_Should_Be_Thrown()
    {
        var catalogBrandName = "Test Brand";
        var catalogBrandAlreadyExists = new CatalogBrand(catalogBrandName);
        var catalogBrand = new CatalogBrand(catalogBrandName);
        _catalogBrandRepository.GetCatalogBrandByNameAsync(catalogBrandName).Returns(catalogBrandAlreadyExists);

        Task act() => _catalogBrandService.CreateCatalogBrandAsync(catalogBrand);

        Assert.That(act, Throws.TypeOf<CatalogBrandAlreadyExistsException>());
    }

    [Test]
    public void When_Update_Catalog_Brand_With_Already_Existed_Name_Then_Exception_Should_Be_Thrown()
    {
        var catalogBrandName = "Test Brand";
        var catalogBrandAlreadyExists = new CatalogBrand(catalogBrandName);
        var catalogBrand = new CatalogBrand(catalogBrandName);
        _catalogBrandRepository.GetCatalogBrandByNameAsync(catalogBrandName).Returns(catalogBrandAlreadyExists);

        Task act() => _catalogBrandService.UpdateCatalogBrandAsync(catalogBrand);

        Assert.That(act, Throws.TypeOf<CatalogBrandAlreadyExistsException>());
    }

    [Test]
    public async Task When_Create_Catalog_Brand_Then_It_Is_Saved()
    {
        var catalogBrand = new CatalogBrand("Test Brand");

        await _catalogBrandService.CreateCatalogBrandAsync(catalogBrand);

        await _catalogBrandRepository.Received().CreateCatalogBrandAsync(catalogBrand);
        await _catalogBrandRepository.Received().SaveChangesAsync();
    }

    [Test]
    public async Task When_Update_Catalog_Brand_Then_It_Is_Saved()
    {
        var catalogBrand = new CatalogBrand("Test Brand");

        await _catalogBrandService.UpdateCatalogBrandAsync(catalogBrand);

        _catalogBrandRepository.Received().UpdateCatalogBrand(catalogBrand);
        await _catalogBrandRepository.Received().SaveChangesAsync();
    }

    [Test]
    public async Task When_Delete_Catalog_Brand_Then_It_Is_Saved()
    {
        var catalogBrand = new CatalogBrand("Test Brand");

        await _catalogBrandService.DeleteCatalogBrandAsync(catalogBrand);

        _catalogBrandRepository.Received().DeleteCatalogBrand(catalogBrand);
        await _catalogBrandRepository.Received().SaveChangesAsync();
    }
}
