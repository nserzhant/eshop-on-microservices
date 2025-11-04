using NSubstitute;
using EShop.Catalog.Core.Exceptions;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;
using EShop.Catalog.Core.Services;

namespace EShop.Catalog.Core.UnitTests;

[TestFixture]
[Category("Catalog Type")]
public class CatalogTypeServiceUnitTests
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
    public void When_Catalog_Type_Is_Used_By_Item_Then_Delete_Throws_Exception()
    {
        var catalogType = new CatalogType("Test catalog type");
        _catalogItemRepository.CatalogItemsWithTypeExistAsync(catalogType.Id).Returns(true);

        Task act() => _catalogTypeService.DeleteCatalogTypeAsync(catalogType);

        Assert.That(act, Throws.TypeOf<CatalogItemsForTypeExistException>());
    }

    [Test]
    public void When_Create_Catalog_Type_With_Already_Existed_Name_Then_Exception_Should_Be_Thrown()
    {
        var catalogTypeName = "Test Catalog Type";
        var catalogTypeAlreadyExists = new CatalogType(catalogTypeName);
        var catalogType = new CatalogType(catalogTypeName);
        _catalogTypeRepository.GetCatalogTypeByNameAsync(catalogTypeName).Returns(catalogTypeAlreadyExists);

        Task act() => _catalogTypeService.CreateCatalogTypeAsync(catalogType);

        Assert.That(act, Throws.TypeOf<CatalogTypeAlreadyExistsException>());
    }

    [Test]
    public void When_Update_Catalog_Type_With_Already_Existed_Name_Then_Exception_Should_Be_Thrown()
    {
        var catalogTypeName = "Test Catalog Type";
        var catalogTypeAlreadyExists = new CatalogType(catalogTypeName);
        var catalogType = new CatalogType(catalogTypeName);
        _catalogTypeRepository.GetCatalogTypeByNameAsync(catalogTypeName).Returns(catalogTypeAlreadyExists);

        Task act() => _catalogTypeService.UpdateCatalogTypeAsync(catalogType);

        Assert.That(act, Throws.TypeOf<CatalogTypeAlreadyExistsException>());
    }

    [Test]
    public async Task When_Create_Catalog_Type_Then_It_Is_Saved()
    {
        var catalogType = new CatalogType("Test Catalog Type");
        
        await _catalogTypeService.CreateCatalogTypeAsync(catalogType);

        await _catalogTypeRepository.Received().CreateCatalogTypeAsync(catalogType);
        await _catalogTypeRepository.Received().SaveChangesAsync();
    }

    [Test]
    public async Task When_Update_Catalog_Type_Then_It_Is_Saved()
    {
        var catalogType = new CatalogType("Test Catalog Type");

        await _catalogTypeService.UpdateCatalogTypeAsync(catalogType);

        _catalogTypeRepository.Received().UpdateCatalogType(catalogType);
        await _catalogTypeRepository.Received().SaveChangesAsync();
    }

    [Test]
    public async Task When_Delete_Catalog_Type_Then_It_Is_Saved()
    {
        var catalogType = new CatalogType("Test Catalog Type");

        await _catalogTypeService.DeleteCatalogTypeAsync(catalogType);

        _catalogTypeRepository.Received().DeleteCatalogType(catalogType);
        await _catalogTypeRepository.Received().SaveChangesAsync();
    }
}
