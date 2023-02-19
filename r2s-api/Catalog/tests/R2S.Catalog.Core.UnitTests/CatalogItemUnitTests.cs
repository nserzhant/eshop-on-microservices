using R2S.Catalog.Core.Exceptions;
using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Core.UnitTests;

[Category("Catalog Item")]
[TestFixture]
public class CatalogItemUnitTests
{

    [Test]
    public void When_Create_Catalog_Item_Then_All_Fields_Should_Be_Set_Correctly()
    {
        string name = "name";
        string description = "description";
        decimal price = 12.4m;
        string pictureUri = @"http:\\test.com\image.png";
        Guid catalogTypeId = Guid.NewGuid();
        Guid catalogBrandId= Guid.NewGuid();

        var catalogItem = new CatalogItem(name, description, price, pictureUri,catalogTypeId, catalogBrandId);  

        Assert.That(catalogItem.Name, Is.EqualTo(name));
        Assert.That(catalogItem.Description, Is.EqualTo(description));
        Assert.That(catalogItem.Price, Is.EqualTo(price));
        Assert.That(catalogItem.PictureUri, Is.EqualTo(pictureUri));
        Assert.That(catalogItem.CatalogTypeId, Is.EqualTo(catalogTypeId));
        Assert.That(catalogItem.CatalogBrandId, Is.EqualTo(catalogBrandId));
    }

    [Test]
    public void When_Create_Catalog_Item_With_Null_Or_Empty_Name_Then_Exception_Should_Be_Thrown()
    {
        var (_, description, price, pictureUri, catalogTypeId, catalogBrandId) = createDefaultValues();

        Action actNull = () => new CatalogItem(null, description, price, pictureUri, catalogTypeId, catalogBrandId);
        Action actEmpty = () => new CatalogItem(string.Empty, description, price, pictureUri, catalogTypeId, catalogBrandId);

        Assert.That(actNull, Throws.TypeOf<CatalogItemNameIsNullOrEmptyException>());
        Assert.That(actEmpty, Throws.TypeOf<CatalogItemNameIsNullOrEmptyException>());
    }

    [Test]
    public void When_Create_Catalog_Item_With_Negative_Price_Then_Exception_Should_Be_Thrown()
    {
        var (name, description, _, pictureUri, catalogTypeId, catalogBrandId) = createDefaultValues();

        Action act = () => new CatalogItem(name, description, -22.6m, pictureUri, catalogTypeId, catalogBrandId);

        Assert.That(act, Throws.TypeOf<CatalogItemNegativePriceException>());
    }

    [Test]
    public void When_Create_Catalog_Item_With_Empty_Type_Reference_Then_Exception_Should_Be_Thrown()
    {
        var (name, description, price, pictureUri, _, catalogBrandId) = createDefaultValues();

        Action act = () => new CatalogItem(name, description, price, pictureUri, Guid.Empty, catalogBrandId);

        Assert.That(act, Throws.TypeOf<CatalogItemTypeIsEmptyException>());
    }

    [Test]
    public void When_Create_Catalog_Item_With_Empty_Brand_Reference_Then_Exception_Should_Be_Thrown()
    {
        var (name, description, price, pictureUri, catalogTypeId, _) = createDefaultValues();

        Action act = () => new CatalogItem(name, description, price, pictureUri, catalogTypeId, Guid.Empty);

        Assert.That(act, Throws.TypeOf<CatalogItemBrandIsEmptyException>());
    }

    [Test]
    public void When_Update_Catalog_Item_Then_Fields_Should_Be_Updated()
    {
        var catalogItem = createItemWithDefaulFields();
        var newItemName = "Updated Item";
        var newItemTs = new byte[] { 1, 2 };
        var newPrice = 212.4m;
        var newCatalogTypeId = Guid.NewGuid();
        var newCatalogBrandId = Guid.NewGuid();

        catalogItem.UpdateName(newItemName);
        catalogItem.UpdateTs(newItemTs);
        catalogItem.UpdatePrice(newPrice);
        catalogItem.UpdateBrand(newCatalogBrandId);
        catalogItem.UpdateType(newCatalogTypeId);

        Assert.That(catalogItem.Name, Is.EqualTo(newItemName));
        Assert.That(catalogItem.Ts, Is.EqualTo(newItemTs));
        Assert.That(catalogItem.Price, Is.EqualTo(newPrice));
        Assert.That(catalogItem.CatalogBrandId, Is.EqualTo(newCatalogBrandId));
        Assert.That(catalogItem.CatalogTypeId, Is.EqualTo(newCatalogTypeId));
    }

    [Test]
    public void When_Update_Catalog_Item_With_Null_Or_Empty_Name_Then_Exception_Should_Be_Thrown()
    {
        var catalogItem = createItemWithDefaulFields();

        Action actNull = () => catalogItem.UpdateName(null);
        Action actEmpty = () => catalogItem.UpdateName(string.Empty);

        Assert.That(actNull, Throws.TypeOf<CatalogItemNameIsNullOrEmptyException>());
        Assert.That(actEmpty, Throws.TypeOf<CatalogItemNameIsNullOrEmptyException>());
    }

    [Test]
    public void When_Update_Catalog_Item_With_Negative_Price_Then_Exception_Should_Be_Thrown()
    {
        var catalogItem = createItemWithDefaulFields();

        Action act = () => catalogItem.UpdatePrice(-112.4m);

        Assert.That(act, Throws.TypeOf<CatalogItemNegativePriceException>());
    }

    [Test]
    public void When_Update_TS_Item_With_Null_Or_Empty_Then_Exception_Should_Be_Thrown()
    {            
        var catalogItem = createItemWithDefaulFields();

        Action act = () => catalogItem.UpdateTs(null);

        Assert.That(act, Throws.TypeOf<CatalogItemTsIsNullException>());
    }

    [Test]
    public void When_Update_Item_Empty_Type_Then_Exception_Should_Be_Thrown()
    {
        var catalogItem = createItemWithDefaulFields();

        Action actEmpty = () => catalogItem.UpdateType(Guid.Empty);

        Assert.That(actEmpty, Throws.TypeOf<CatalogItemTypeIsEmptyException>());
    }

    [Test]
    public void When_Update_Item_With_Empty_Brand_Then_Exception_Should_Be_Thrown()
    {
        var catalogItem = createItemWithDefaulFields();

        Action actEmpty = () => catalogItem.UpdateBrand(Guid.Empty);

        Assert.That(actEmpty, Throws.TypeOf<CatalogItemBrandIsEmptyException>());
    }

    protected static CatalogItem createItemWithDefaulFields()
    {
        var (name, description, price, pictureUri, catalogTypeId, catalogBrandId) = createDefaultValues();

        return new CatalogItem(name, description, price, pictureUri, catalogTypeId, catalogBrandId);
    }

    private static (string,string,decimal, string, Guid, Guid) createDefaultValues()
    {
        return ("name", "description", 12.4m, @"http:\\test.com\image.png", Guid.NewGuid(), Guid.NewGuid());
    }
}