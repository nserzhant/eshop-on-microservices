using Microsoft.Extensions.DependencyInjection;
using R2S.Catalog.Core.Interfaces;
using R2S.Catalog.Core.Models;
using R2S.Catalog.Core.Services;
using R2S.Catalog.Infrastructure.Read;
using R2S.Catalog.Infrastructure.Read.Queries;

namespace R2S.Catalog.Infrastructure.IntegrationTests;

[TestFixture]
[Category("Catalog Item")]
public class CatalogItemIntegrationTests : BaseCatalogIntegrationTests
{
    private ICatalogItemRepository _catalogItemRepository;
    private ICatalogItemService _catalogItemService;
    private ICatalogItemQueryService _catalogItemQueryService;

    [SetUp]
    public override async Task SetupAsync()
    {
        await base.SetupAsync();

        _catalogItemRepository = serviceProvider.GetRequiredService<ICatalogItemRepository>();
        _catalogItemService = serviceProvider.GetRequiredService<ICatalogItemService>();
        _catalogItemQueryService = serviceProvider.GetRequiredService<ICatalogItemQueryService>();
    }

    [Test]
    [Category("Catalog Item Repository")]
    public async Task When_Save_Catalog_Item_Than_Id_Should_Be_Generated()
    {
        var catalogBrand = await createCatalogBrandAsync("Brand");
        var catalogType = await createCatalogTypeAsync("Type");
        var catalogItemName = "test catalog item";
        var catalogItemToCreate = new CatalogItem(catalogItemName, null, 1, null, catalogType.Id, catalogBrand.Id);

        await _catalogItemService.CreateCatalogItemAsync(catalogItemToCreate);

        Assert.That(catalogItemToCreate.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    [Category("Catalog Item Repository")]
    public async Task When_Catalog_Item_Created_Then_It_Could_Be_Retreived_By_Id()
    {
        var catalogBrand = await createCatalogBrandAsync("Brand");
        var catalogType = await createCatalogTypeAsync("Type");
        var catalogItemName = "test catalog item";
        var catalogItemToCreate = new CatalogItem(catalogItemName, null, 1, null, catalogType.Id, catalogBrand.Id);
        await _catalogItemService.CreateCatalogItemAsync(catalogItemToCreate);

        var catalogItemSaved = await _catalogItemRepository.GetCatalogItemAsync(catalogItemToCreate.Id);

        Assert.That(catalogItemSaved, Is.Not.Null);
        Assert.That(catalogItemSaved.Id, Is.EqualTo(catalogItemToCreate.Id));
        Assert.That(catalogItemSaved.Name, Is.EqualTo("test catalog item"));
    }

    [Test]
    [Category("Catalog Item Repository")]
    public async Task When_Catalog_Item_Created_Then_It_Could_Be_Retreived_By_Name_Brand_Type()
    {
        var catalogBrand = await createCatalogBrandAsync("Brand");
        var catalogType = await createCatalogTypeAsync("Type");
        var catalogItemName = "test catalog item";
        var catalogItemToCreate = new CatalogItem(catalogItemName, null, 1, null, catalogType.Id, catalogBrand.Id);
        await _catalogItemService.CreateCatalogItemAsync(catalogItemToCreate);

        var catalogItemSaved = await _catalogItemRepository.GetCatalogItemAsync(catalogItemName, catalogType.Id, catalogBrand.Id);

        Assert.That(catalogItemSaved, Is.Not.Null);
        Assert.That(catalogItemSaved.Id, Is.EqualTo(catalogItemToCreate.Id));
        Assert.That(catalogItemSaved.Name, Is.EqualTo("test catalog item"));
    }

    [Test]
    [Category("Catalog Item Repository")]
    public async Task When_Catalog_Item_Created_Then_Catalog_Brand_Exists_Check_Working()
    {
        var catalogBrand = await createCatalogBrandAsync("Brand");
        var catalogType = await createCatalogTypeAsync("Type");
        var catalogItemName = "test catalog item";
        var catalogItemToCreate = new CatalogItem(catalogItemName, null, 1, null, catalogType.Id, catalogBrand.Id);
        await _catalogItemService.CreateCatalogItemAsync(catalogItemToCreate);
        var catalogBrandWithItemId = catalogBrand.Id;
        var catalogBrandWithoutItemId = Guid.NewGuid();

        var itemsWithBrandExists = await _catalogItemRepository.DoesCatalogItemsWithBrandExistsAsync(catalogBrandWithItemId);
        var itemsWithoutBrandExists = await _catalogItemRepository.DoesCatalogItemsWithBrandExistsAsync(catalogBrandWithoutItemId);

        Assert.That(itemsWithBrandExists, Is.True);
        Assert.That(itemsWithoutBrandExists, Is.False);
    }

    [Test]
    [Category("Catalog Item Repository")]
    public async Task When_Catalog_Item_Created_Then_Catalog_Type_Exists_Check_Working()
    {
        var catalogBrand = await createCatalogBrandAsync("Brand");
        var catalogType = await createCatalogTypeAsync("Type");
        var catalogItemName = "test catalog item";
        var catalogItemToCreate = new CatalogItem(catalogItemName, null, 1, null, catalogType.Id, catalogBrand.Id);
        await _catalogItemService.CreateCatalogItemAsync(catalogItemToCreate);
        var catalogTypeWithItemId = catalogType.Id;
        var catalogTypeWithoutItemId = Guid.NewGuid();

        var itemsWithTypeExists = await _catalogItemRepository.DoesCatalogItemsWithTypeExistsAsync(catalogTypeWithItemId);
        var itemsWithoutTypeExists = await _catalogItemRepository.DoesCatalogItemsWithTypeExistsAsync(catalogTypeWithoutItemId);

        Assert.That(itemsWithTypeExists, Is.True);
        Assert.That(itemsWithoutTypeExists, Is.False);
    }

    [Test]
    [Category("Catalog Item Repository")]
    [Category("Catalog Item Query Service")]
    public async Task When_Update_Catalog_Item_Name_Then_It_Could_Be_Retreived_By_Id()
    {
        string catalogItemName = "test catalog item";
        string updatedCatalogItemName = "updated catalog item";
        CatalogItem catalogItem = await createCatalogItemAsync(catalogItemName);
        await _catalogItemRepository.SaveChangesAsync();
        catalogItem.UpdateName(updatedCatalogItemName);

        await _catalogItemService.UpdateCatalogItemAsync(catalogItem);

        var catalogItemUpdated = await _catalogItemQueryService.GetById(catalogItem.Id);
        Assert.That(catalogItemUpdated, Is.Not.Null);
        Assert.That(catalogItemUpdated.Name, Is.EqualTo(updatedCatalogItemName));
    }

    [Test]
    [Category("Catalog Item Repository")]
    [Category("Catalog Item Query Service")]
    public async Task When_Update_Catalog_Item_Qty_Then_It_Could_Be_Retreived_By_Id()
    {
        string catalogItemName = "test catalog item";
        CatalogItem catalogItem = await createCatalogItemAsync(catalogItemName);
        await _catalogItemRepository.SaveChangesAsync();
        var updatedCatalogItemPrice = catalogItem.Price + 100;
        catalogItem.UpdatePrice(updatedCatalogItemPrice);

        await _catalogItemService.UpdateCatalogItemAsync(catalogItem);

        var catalogItemUpdated = await _catalogItemQueryService.GetById(catalogItem.Id);
        Assert.That(catalogItemUpdated, Is.Not.Null);
        Assert.That(catalogItemUpdated.Price, Is.EqualTo(updatedCatalogItemPrice));
    }

    [Test]
    [Category("Catalog Item Repository")]
    public async Task When_Delete_Catalog_Item_Then_It_Could_Not_Be_Retreived_By_Id()
    {
        string catalogItemName = "test catalog item";
        CatalogItem catalogItem = await createCatalogItemAsync(catalogItemName);
        await _catalogItemRepository.SaveChangesAsync();

        _catalogItemRepository.DeleteCatalogItem(catalogItem);
        await _catalogItemRepository.SaveChangesAsync();

        var catalogItemUpdated = await _catalogItemQueryService.GetById(catalogItem.Id);
        Assert.That(catalogItemUpdated, Is.Null);
    }

    [Test]
    [Category("Catalog Item Query Service")]
    public async Task When_Catalog_Items_Exists_Then_They_Could_Be_Requested_By_List_Query()
    {
        await createCatalogItemAsync("A Catalog Item");
        await createCatalogItemAsync("B Catalog Item");
        await createCatalogItemAsync("C Catalog Item");
        var listCatalogItemQuery = new ListCatalogItemQuery()
        {
            OrderByDirection = OrderByDirections.ASC,
            PageIndex = 0,
            PageSize = 2
        };

        var resultAsc = await _catalogItemQueryService.GetCatalogItems(listCatalogItemQuery);
        listCatalogItemQuery.OrderByDirection = OrderByDirections.DESC;
        var resultDesc = await _catalogItemQueryService.GetCatalogItems(listCatalogItemQuery);

        Assert.That(resultAsc, Is.Not.Null);
        Assert.That(resultAsc.TotalCount, Is.EqualTo(3));
        Assert.That(resultAsc.CatalogItems.Count, Is.EqualTo(2));
        Assert.That(resultAsc.CatalogItems[0].Name, Is.EqualTo("A Catalog Item"));

        Assert.That(resultDesc, Is.Not.Null);
        Assert.That(resultDesc.TotalCount, Is.EqualTo(3));
        Assert.That(resultDesc.CatalogItems.Count, Is.EqualTo(2));
        Assert.That(resultDesc.CatalogItems[0].Name, Is.EqualTo("C Catalog Item"));
    }

    [Test]
    [TestCase(ListCatalogItemOrderBy.Name, OrderByDirections.ASC, "FIRST ITEM")]
    [TestCase(ListCatalogItemOrderBy.Name, OrderByDirections.DESC, "FOURTH ITEM")]
    [TestCase(ListCatalogItemOrderBy.Brand, OrderByDirections.ASC, "FOURTH ITEM")]
    [TestCase(ListCatalogItemOrderBy.Brand, OrderByDirections.DESC, "THRIRD ITEM")]
    [TestCase(ListCatalogItemOrderBy.Type, OrderByDirections.ASC, "THRIRD ITEM")]
    [TestCase(ListCatalogItemOrderBy.Type, OrderByDirections.DESC, "SECOND ITEM")]
    [TestCase(ListCatalogItemOrderBy.Price, OrderByDirections.ASC, "SECOND ITEM")]
    [TestCase(ListCatalogItemOrderBy.Price, OrderByDirections.DESC, "FIRST ITEM")]
    [Category("Catalog Item Query Service")]
    public async Task When_Catalog_Items_Exists_Then_They_Could_Ordered_By_Name_Brand_Type_Price(ListCatalogItemOrderBy orderBy, OrderByDirections directions, string resultingItemDescription)
    {
        await createCatalogItemAsync("A Catalog Item", "B Catalog Brand", "C Catalog Type", 40m, "FIRST ITEM");
        await createCatalogItemAsync("B Catalog Item", "C Catalog Brand", "D Catalog Type", 10m, "SECOND ITEM");
        await createCatalogItemAsync("C Catalog Item", "D Catalog Brand", "A Catalog Type", 20m, "THRIRD ITEM");
        await createCatalogItemAsync("D Catalog Item", "A Catalog Brand", "B Catalog Type", 30m, "FOURTH ITEM");
        var listCatalogItemQuery = new ListCatalogItemQuery()
        {
            OrderBy = orderBy,
            OrderByDirection = directions,
            PageIndex = 0,
            PageSize = 3
        };

        var result = await _catalogItemQueryService.GetCatalogItems(listCatalogItemQuery);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.TotalCount, Is.EqualTo(4));
        Assert.That(result.CatalogItems.Count, Is.EqualTo(3));
        Assert.That(result.CatalogItems[0].Description, Is.EqualTo(resultingItemDescription));
    }


    [Test]
    [TestCase("A Catalog", null, null, "FIRST ITEM")]
    [TestCase("C Catalog", null, null, "THRIRD ITEM")]
    [TestCase(null, "A Catalog", null, "FOURTH ITEM")]
    [TestCase(null, "C Catalog", null, "SECOND ITEM")]
    [TestCase(null, null, "A Catalog", "THRIRD ITEM")]
    [TestCase(null, null, "C Catalog", "FIRST ITEM")]
    public async Task When_Catalog_Items_Exists_Then_They_Could_Be_Filtered_By_Name_Brand_Type(string? name, string? brand, string? type, string resultingItemDescription)
    {
        await createCatalogItemAsync("A Catalog Item", "B Catalog Brand", "C Catalog Type", 40m, "FIRST ITEM");
        await createCatalogItemAsync("B Catalog Item", "C Catalog Brand", "D Catalog Type", 10m, "SECOND ITEM");
        await createCatalogItemAsync("C Catalog Item", "D Catalog Brand", "A Catalog Type", 20m, "THRIRD ITEM");
        await createCatalogItemAsync("D Catalog Item", "A Catalog Brand", "B Catalog Type", 30m, "FOURTH ITEM");
        var listCatalogItemQuery = new ListCatalogItemQuery()
        {
            PageIndex = 0,
            PageSize = 4,
            NameFilter = name,
            BrandFilter = brand,
            TypeFilter = type
        };

        var result = await _catalogItemQueryService.GetCatalogItems(listCatalogItemQuery);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.CatalogItems.Count, Is.EqualTo(1));
        Assert.That(result.CatalogItems[0].Description, Is.EqualTo(resultingItemDescription));
    }

    [Test]
    [Category("Catalog Item Query Service")]
    public async Task When_Catalog_Item_Exists_Then_It_Could_Be_Queried_By_Id()
    {
        var catalogItemName = "test catalog Item";
        var catalogItemDescription = "test catalog description";
        var price = 12.43m;
        var pictureUri = @"http:\\localhost\item.png";
        var catalogTypeName = "test catalog type name";
        var catalogBrandName = "test catalog brand name";
        var catalogType = await createCatalogTypeAsync(catalogTypeName);
        var catalogBrand = await createCatalogBrandAsync(catalogBrandName);
        var catalogItemToCreate = new CatalogItem(catalogItemName, catalogItemDescription, price, pictureUri, catalogType.Id, catalogBrand.Id);
        await _catalogItemService.CreateCatalogItemAsync(catalogItemToCreate);

        var catalogItemReadModel = await _catalogItemQueryService.GetById(catalogItemToCreate.Id);

        Assert.That(catalogItemReadModel, Is.Not.Null);
        Assert.That(catalogItemReadModel.Id, Is.EqualTo(catalogItemToCreate.Id));
        Assert.That(catalogItemReadModel.Name, Is.EqualTo(catalogItemName));
        Assert.That(catalogItemReadModel.Description, Is.EqualTo(catalogItemDescription));
        Assert.That(catalogItemReadModel.Price, Is.EqualTo(price));
        Assert.That(catalogItemReadModel.PictureUri, Is.EqualTo(pictureUri));
        Assert.That(catalogItemReadModel.CatalogBrand, Is.Not.Null);
        Assert.That(catalogItemReadModel.CatalogBrand.Brand, Is.EqualTo(catalogBrandName));
        Assert.That(catalogItemReadModel.CatalogType.Type, Is.EqualTo(catalogTypeName));
        Assert.That(catalogItemReadModel.Ts, Is.EqualTo(catalogItemToCreate.Ts));
    }
}