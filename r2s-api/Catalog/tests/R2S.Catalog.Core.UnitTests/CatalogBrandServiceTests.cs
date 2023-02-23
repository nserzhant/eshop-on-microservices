using NSubstitute;
using R2S.Catalog.Core.Exceptions;
using R2S.Catalog.Core.Interfaces;
using R2S.Catalog.Core.Models;
using R2S.Catalog.Core.Services;

namespace R2S.Catalog.Core.UnitTests;

[TestFixture]
[Category("Catalog Brand")]
public class CatalogBrandServiceTests
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
    public void When_Delete_Catalog_Brand_And_Catalog_Item_Wit_Type_Exists_Then_Exception_Should_Be_Thrown()
    {
        var catalogBrand = new CatalogBrand("Test catalog brand");
        _catalogItemRepository.DoesCatalogItemsWithBrandExistsAsync(catalogBrand.Id).Returns(true);

        Task act() => _catalogBrandService.DeleteCatalogBrandAsync(catalogBrand);

        Assert.That(act, Throws.TypeOf<CatalogItemsForBrandExistsException>());
    }

    [Test]
    public void When_Create_Catalog_Brand_And_The_Same_Already_Exists_Then_Exception_Should_Be_Thrown()
    {
        var catalogBrandName = "Test Brand";
        var catalogBrandAlreadyExists = new CatalogBrand(catalogBrandName);
        var catalogBrand = new CatalogBrand(catalogBrandName);
        _catalogBrandRepository.GetCatalogBrandByNameAsync(catalogBrandName).Returns(catalogBrandAlreadyExists);

        Task act() => _catalogBrandService.CreateCatalogBrandAsync(catalogBrand);

        Assert.That(act, Throws.TypeOf<CatalogBrandAlreadyExistsException>());
    }

    [Test]
    public void When_Update_Catalog_Brand_With_The_Name_That_Already_Exists_Then_Exception_Should_Be_Thrown()
    {
        var catalogBrandName = "Test Brand";
        var catalogBrandAlreadyExists = new CatalogBrand(catalogBrandName);
        var catalogBrand = new CatalogBrand(catalogBrandName);
        _catalogBrandRepository.GetCatalogBrandByNameAsync(catalogBrandName).Returns(catalogBrandAlreadyExists);

        Task act() => _catalogBrandService.UpdateCatalogBrandAsync(catalogBrand);

        Assert.That(act, Throws.TypeOf<CatalogBrandAlreadyExistsException>());
    }

    [Test]
    public void When_Update_Catalog_Brand_That_Already_Exists_Then_Exception_Should_Not_Thrown()
    {
        var catalogBrandName = "Test Brand";
        var catalogBrand = new CatalogBrand(catalogBrandName);
        _catalogBrandRepository.GetCatalogBrandByNameAsync(catalogBrandName).Returns(catalogBrand);

        Task act() => _catalogBrandService.UpdateCatalogBrandAsync(catalogBrand);

        Assert.That(act, Throws.Nothing);
    }
}
