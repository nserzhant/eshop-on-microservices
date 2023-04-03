using EShop.Catalog.Core.Exceptions;
using EShop.Catalog.Core.Models;

namespace EShop.Catalog.Core.UnitTests;

[TestFixture]
[Category("Catalog Brand")]
public class CatalogBrandTests
{
    [Test]
    public void When_Create_Catalog_Brand_Then_All_Fields_Should_Be_Set_Correctly()
    {
        string catalogBrandName = "test brand name";

        var catalogBrand = new CatalogBrand(catalogBrandName);

        Assert.That(catalogBrand.Brand, Is.EqualTo(catalogBrandName));
    }

    [Test]
    public void When_Creating_Catalog_Brand_With_Null_Or_Empty_Name_Then_Exception_Should_Be_Thrown()
    {
        Action actNull = () => new CatalogBrand(null);
        Action actEmpty = () => new CatalogBrand("");

        Assert.That(actNull, Throws.TypeOf<CatalogBrandIsNullOrEmptyException>());
        Assert.That(actEmpty, Throws.TypeOf<CatalogBrandIsNullOrEmptyException>());
    }

    [Test]
    public void When_Update_Catalog_Brand_Then_Fields_Should_Be_Updated()
    {
        var catalogBrand = new CatalogBrand("defaultBrandName");
        var newBrandName = "Updated brand";
        var newBrandTs = new byte[] { 1, 2 };

        catalogBrand.UpdateBrand(newBrandName);
        catalogBrand.UpdateTs(newBrandTs);

        Assert.That(catalogBrand.Brand, Is.EqualTo(newBrandName));
        Assert.That(catalogBrand.Ts, Is.EqualTo(newBrandTs));
    }

    [Test]
    public void When_Update_Catalog_Brand_With_Null_Or_Empty_Then_Exception_Should_Be_Thrown()
    {
        var catalogBrand = new CatalogBrand("defaultBrandName");

        Action actNull = () => catalogBrand.UpdateBrand(null);
        Action actEmpty = () => catalogBrand.UpdateBrand(string.Empty);

        Assert.That(actNull, Throws.TypeOf<CatalogBrandIsNullOrEmptyException>());
        Assert.That(actEmpty, Throws.TypeOf<CatalogBrandIsNullOrEmptyException>());
    }

    [Test]
    public void When_Update_TS_Brand_With_Null_Or_Empty_Then_Exception_Should_Be_Thrown()
    {
        var catalogBrand = new CatalogBrand("defaultBrandName");

        Action act = () => catalogBrand.UpdateTs(null);

        Assert.That(act, Throws.TypeOf<CatalogBrandTsIsNullException>());
    }
}
