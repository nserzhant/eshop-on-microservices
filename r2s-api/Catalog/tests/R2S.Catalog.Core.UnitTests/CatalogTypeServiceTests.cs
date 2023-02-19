using NSubstitute;
using R2S.Catalog.Core.Exceptions;
using R2S.Catalog.Core.Interfaces;
using R2S.Catalog.Core.Models;
using R2S.Catalog.Core.Services;

namespace R2S.Catalog.Core.UnitTests;

[TestFixture]
[Category("Catalog Type")]
public class CatalogTypeServiceTests
{
    private CatalogTypeService _catalogTypeService;
    private ICatalogItemRepository _catalogItemRepository;

    [SetUp]
    public void Setup()
    {
        _catalogItemRepository = Substitute.For<ICatalogItemRepository>();
        _catalogTypeService = new CatalogTypeService(_catalogItemRepository,
            Substitute.For<ICatalogTypeRepository>());
    }

    [Test]
    public void When_Delete_Catalog_Type_And_Catalog_Item_Wit_Type_Exists_Then_Exception_Should_Be_Thrown()
    {
        var catalogType = new CatalogType("Test catalog type");
        _catalogItemRepository.DoesCatalogItemsWithTypeExistsAsync(catalogType.Id).Returns(Task.FromResult(true));

        Task act() => _catalogTypeService.DeleteCatalogTypeAsync(catalogType);

        Assert.That(act, Throws.TypeOf<CatalogItemsForTypeExistsException>());
    }
}
