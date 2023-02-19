using NSubstitute;
using NSubstitute.ReturnsExtensions;
using R2S.Catalog.Core.Exceptions;
using R2S.Catalog.Core.Interfaces;
using R2S.Catalog.Core.Models;
using R2S.Catalog.Core.Services;

namespace R2S.Catalog.Core.UnitTests;

[Category("Catalog Item")]
[TestFixture]
public class CatalogItemServiceUnitTests
{
    private CatalogItemService _catalogItemService;
    private ICatalogBrandRepository _catalogBrandRepository;
    private ICatalogTypeRepository _catalogTypeRepository;
    private CatalogItem _catalogItem;

    [SetUp]
    public void Setup()
    {
        _catalogBrandRepository = Substitute.For<ICatalogBrandRepository>();
        _catalogTypeRepository = Substitute.For<ICatalogTypeRepository>();
        _catalogItemService = new CatalogItemService(_catalogBrandRepository, _catalogTypeRepository,
            Substitute.For<ICatalogItemRepository>());
        _catalogItem = new CatalogItem("test", 
            "test", 
            null, 
            null, 
            Guid.NewGuid(), 
            Guid.NewGuid());
    }

    [Test]
    public void When_Saving_Catalog_Item_And_Brand_Not_Exists_Then_Exception_Should_Be_Thrown()
    {
        _catalogBrandRepository.GetCatalogBrandAsync(_catalogItem.CatalogBrandId).ReturnsNull();
        _catalogTypeRepository.GetCatalogTypeAsync(_catalogItem.CatalogTypeId).Returns(new CatalogType("test"));

        Task act() => _catalogItemService.UpdateCatalogItemAsync(_catalogItem);

        Assert.That(act, Throws.TypeOf<CatalogBrandNotExistsException>());
    }

    [Test]
    public void When_Saving_Catalog_Item_And_Type_Not_Exists_Then_Exception_Should_Be_Thrown()
    {
        _catalogBrandRepository.GetCatalogBrandAsync(_catalogItem.CatalogBrandId).Returns(new CatalogBrand("test"));
        _catalogTypeRepository.GetCatalogTypeAsync(_catalogItem.CatalogTypeId).ReturnsNull();

        Task act() => _catalogItemService.UpdateCatalogItemAsync(_catalogItem);

        Assert.That(act, Throws.TypeOf<CatalogTypeNotExistsException>());
    }

    [Test]
    public void When_Creating_Catalog_Item_And_Brand_Not_Exists_Then_Exception_Should_Be_Thrown()
    {
        _catalogBrandRepository.GetCatalogBrandAsync(_catalogItem.CatalogBrandId).ReturnsNull();
        _catalogTypeRepository.GetCatalogTypeAsync(_catalogItem.CatalogTypeId).Returns(new CatalogType("test"));

        Task act() => _catalogItemService.CreateCatalogItemAsync(_catalogItem);

        Assert.That(act, Throws.TypeOf<CatalogBrandNotExistsException>());
    }

    [Test]
    public void When_Creating_Catalog_Item_And_Type_Not_Exists_Then_Exception_Should_Be_Thrown()
    {
        _catalogBrandRepository.GetCatalogBrandAsync(_catalogItem.CatalogBrandId).Returns(Task.FromResult(new CatalogBrand("test")));
        _catalogTypeRepository.GetCatalogTypeAsync(_catalogItem.CatalogTypeId).ReturnsNull();

        Task act() => _catalogItemService.CreateCatalogItemAsync(_catalogItem);

        Assert.That(act, Throws.TypeOf<CatalogTypeNotExistsException>());
    }
}
