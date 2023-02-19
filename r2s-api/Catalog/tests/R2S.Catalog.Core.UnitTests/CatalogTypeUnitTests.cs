using R2S.Catalog.Core.Exceptions;
using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Core.UnitTests;

[TestFixture]
[Category("Catalog Type")]
public class CatalogTypeUnitTests
{
    [Test]
    public void When_Create_Catalog_Type_Then_All_Fields_Should_Be_Set_Correctly()
    {
        string catalogTypeName = "catalog Type test";

        var catalogType = new CatalogType(catalogTypeName);

        Assert.That(catalogType.Type, Is.EqualTo(catalogTypeName));
    }

    [Test]
    public void When_Creating_Catalog_Type_With_Null_Or_Empty_Name_Then_Exception_Should_Be_Thrown()
    {
        Action actNull = () => new CatalogType(null);
        Action actEmpty = () => new CatalogType("");

        Assert.That(actNull, Throws.TypeOf<CatalogTypeIsNullOrEmptyException>());
        Assert.That(actEmpty, Throws.TypeOf<CatalogTypeIsNullOrEmptyException>());
    }

    [Test]
    public void When_Update_Catalog_Type_Then_Fields_Should_Be_Updated()
    {
        var catalogType = new CatalogType("default Catalog Type Name");
        var newTypeName = "Updated Type";
        var newTypeTs = new byte[] { 3, 4 };

        catalogType.UpdateType(newTypeName);
        catalogType.UpdateTs(newTypeTs);

        Assert.That(catalogType.Type, Is.EqualTo(newTypeName));
        Assert.That(catalogType.Ts, Is.EqualTo(newTypeTs));
    }

    [Test]
    public void When_Update_Catalog_Type_With_Null_Or_Empty_Then_Exception_Should_Be_Thrown()
    {
        var catalogType = new CatalogType("default Catalog Type Name");

        Action actNull = () => catalogType.UpdateType(null);
        Action actEmpty = () => catalogType.UpdateType(string.Empty);

        Assert.That(actNull, Throws.TypeOf<CatalogTypeIsNullOrEmptyException>());
        Assert.That(actEmpty, Throws.TypeOf<CatalogTypeIsNullOrEmptyException>());
    }

    [Test]
    public void When_Update_TS_Type_With_Null_Or_Empty_Then_Exception_Should_Be_Thrown()
    {
        var catalogType = new CatalogType("default Catalog Type Name");

        Action act = () => catalogType.UpdateTs(null);

        Assert.That(act, Throws.TypeOf<CatalogTypeTsIsNullException>());
    }
}
