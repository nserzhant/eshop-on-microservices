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

    [SetUp]
    public void Setup()
    {
        _catalogItemRepository = Substitute.For<ICatalogItemRepository>();
        _catalogBrandService = new CatalogBrandService(_catalogItemRepository, 
            Substitute.For<ICatalogBrandRepository>());
    }

    [Test]
    public void When_Delete_Catalog_Brand_And_Catalog_Item_Wit_Type_Exists_Then_Exception_Should_Be_Thrown()
    {
        var catalogBrand = new CatalogBrand("Test catalog brand");
        _catalogItemRepository.DoesCatalogItemsWithBrandExistsAsync(catalogBrand.Id).Returns(Task.FromResult(true));

        Task act() => _catalogBrandService.DeleteCatalogBrandAsync(catalogBrand);

        Assert.That(act, Throws.TypeOf<CatalogItemsForBrandExistsException>());
    }
}
