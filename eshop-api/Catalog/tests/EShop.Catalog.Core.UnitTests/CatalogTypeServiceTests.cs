using NSubstitute;
using EShop.Catalog.Core.Exceptions;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;
using EShop.Catalog.Core.Services;

namespace EShop.Catalog.Core.UnitTests;

[TestFixture]
[Category("Catalog Type")]
public class CatalogTypeServiceTests
{
    private CatalogTypeService _catalogTypeService;
    private ICatalogTypeRepository _catalogTypeRepository;
    private ICatalogItemRepository _catalogItemRepository;

    [SetUp]
    public void Setup()
    {
        _catalogItemRepository = Substitute.For<ICatalogItemRepository>();
        _catalogTypeRepository = Substitute.For<ICatalogTypeRepository>();
        _catalogTypeService = new CatalogTypeService(_catalogItemRepository, _catalogTypeRepository);
    }

    [Test]
    public void When_Delete_Catalog_Type_And_Catalog_Item_Wit_Type_Exists_Then_Exception_Should_Be_Thrown()
    {
        var catalogType = new CatalogType("Test catalog type");
        _catalogItemRepository.DoesCatalogItemsWithTypeExistsAsync(catalogType.Id).Returns(true);

        Task act() => _catalogTypeService.DeleteCatalogTypeAsync(catalogType);

        Assert.That(act, Throws.TypeOf<CatalogItemsForTypeExistsException>());
    }

    [Test]
    public void When_Create_Catalog_Type_And_The_Same_Already_Exists_Then_Exception_Should_Be_Thrown()
    {
        var catalogTypeName = "Test Catalog Type";
        var catalogTypeAlreadyExists = new CatalogType(catalogTypeName);
        var catalogType = new CatalogType(catalogTypeName);
        _catalogTypeRepository.GetCatalogTypeByNameAsync(catalogTypeName).Returns(catalogTypeAlreadyExists);

        Task act() => _catalogTypeService.CreateCatalogTypeAsync(catalogType);

        Assert.That(act, Throws.TypeOf<CatalogTypeAlreadyExistsException>());
    }

    [Test]
    public void When_Update_Catalog_Type_With_The_Name_That_Already_Exists_Then_Exception_Should_Be_Thrown()
    {
        var catalogTypeName = "Test Catalog Type";
        var catalogTypeAlreadyExists = new CatalogType(catalogTypeName);
        var catalogType = new CatalogType(catalogTypeName);
        _catalogTypeRepository.GetCatalogTypeByNameAsync(catalogTypeName).Returns(catalogTypeAlreadyExists);

        Task act() => _catalogTypeService.UpdateCatalogTypeAsync(catalogType);

        Assert.That(act, Throws.TypeOf<CatalogTypeAlreadyExistsException>());
    }

    [Test]
    public void When_Update_Catalog_Type_That_Already_Exists_Then_Exception_Should_Not_Thrown()
    {
        var catalogTypeName = "Test Catalog Type";
        var catalogType = new CatalogType(catalogTypeName);
        _catalogTypeRepository.GetCatalogTypeByNameAsync(catalogTypeName).Returns(catalogType);

        Task act() => _catalogTypeService.UpdateCatalogTypeAsync(catalogType);

        Assert.That(act, Throws.Nothing);
    }
}
