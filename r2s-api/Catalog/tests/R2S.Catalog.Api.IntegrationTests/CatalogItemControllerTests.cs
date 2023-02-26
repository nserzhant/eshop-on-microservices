using Microsoft.Net.Http.Headers;
using R2S.Catalog.Api.Models;
using R2S.Catalog.Core.Models;
using R2S.Catalog.Infrastructure.Read.Queries;
using R2S.Catalog.Infrastructure.Read.Queries.Results;
using R2S.Catalog.Infrastructure.Read.ReadModels;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace R2S.Catalog.Api.IntegrationTests;

[TestFixture]
[Category("Catalog Item")]
public class CatalogItemControllerTests : BaseCatalogControllerTests
{
    private const string API_BASE_URL = "api/CatalogItem";
    private CatalogItem defaultCatalogItem;
    private CatalogType defaultCatalogType;
    private CatalogBrand defaultCatalogBrand;

    [SetUp]
    public async Task SetupAsync()
    {
        await base.SetupAsync();

        defaultCatalogBrand = await createCatalogBrandAsync("default brand");
        defaultCatalogType = await createCatalogTypeAsync("default catalog type");
        defaultCatalogItem = await createCatalogItemAsync("default Item");
    }

    [Test]
    [Category("Create Item")]
    public async Task When_Client_Is_Unauthenticated_Then_Create_Item_Returns_Unathorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();
        var content = createItemContent(new CatalogItemDTO());

        var response = await catalogItemClient.PostAsync(Post.Item, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Create Item")]
    public async Task When_Client_Is_Not_Sales_Manager_Then_It_Cant_Create_Item()
    {
        testAuthenticationContextBuilder.SetAuthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();
        var content = createItemContent(new CatalogItemDTO());

        var response = await catalogItemClient.PostAsync(Post.Item, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Create Item")]
    public async Task When_Client_Is_Sales_Manager_Then_It_Can_Create_Item()
    {
        testAuthenticationContextBuilder.SetAuthenticated().AsSalesManager();
        var catalogItemClient = webApplicationFactory.CreateClient();
        var content = createItemContent(new CatalogItemDTO() 
        {
            Name = "Test Item",
            BrandId = defaultCatalogBrand.Id,
            TypeId = defaultCatalogType.Id
        });

        var response = await catalogItemClient.PostAsync(Post.Item, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    [Category("Get Item")]
    public async Task When_Client_Is_Unauthenticated_Then_Get_Item_Returns_Unathorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.GetAsync(Item(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Get Item")]
    public async Task When_Client_Is_Authenticated_Then_It_Can_Get_Item()
    {
        testAuthenticationContextBuilder.SetAuthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.GetAsync(Item(defaultCatalogItem.Id));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Get Item")]
    public async Task When_Client_Is_Requesting_Non_Existing_Item_Then_Not_Found_Returns()
    {
        testAuthenticationContextBuilder.SetAuthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.GetAsync(Item(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("Create Item")]
    [Category("Get Item")]
    public async Task When_Client_Created_Item_Then_It_Can_Get_Created_Item()
    {
        testAuthenticationContextBuilder.SetAuthenticated().AsSalesManager();
        var catalogItemClient = webApplicationFactory.CreateClient();
        var itemName = "Test Item";
        var description = "Test Item Description";
        var price = 33m;
        var pictureURI = @"http:\\localhost\testpicture.png";
        var availableQty = 47;
        var catalogItem = new CatalogItemDTO
        {
            Name = itemName,
            Description = description,
            Price = price,
            PictureUri = pictureURI,
            BrandId = defaultCatalogBrand.Id,
            TypeId = defaultCatalogType.Id,
            AvailableQty = availableQty
        };
        var content = createItemContent(catalogItem);
        var response = await catalogItemClient.PostAsync(Post.Item, content);
        var createdCatalogItem = await fromHttpResponseMessage<CatalogItemReadModel>(response);
        Assert.That(createdCatalogItem, Is.Not.Null);
        var createdCatalogItemId = createdCatalogItem.Id;

        var getResponse = await catalogItemClient.GetAsync(Item(createdCatalogItemId));
        var getCatalogItem = await fromHttpResponseMessage<CatalogItemReadModel>(getResponse);

        Assert.That(createdCatalogItem.Name, Is.EqualTo(itemName));
        Assert.That(getCatalogItem, Is.Not.Null);
        Assert.That(getCatalogItem.Name, Is.EqualTo(itemName));
        Assert.That(getCatalogItem.Description, Is.EqualTo(description));
        Assert.That(getCatalogItem.Price, Is.EqualTo(price));
        Assert.That(getCatalogItem.PictureUri, Is.EqualTo(pictureURI));
        Assert.That(getCatalogItem.CatalogBrandId, Is.EqualTo(defaultCatalogBrand.Id));
        Assert.That(getCatalogItem.CatalogBrand.Id, Is.EqualTo(defaultCatalogBrand.Id));
        Assert.That(getCatalogItem.CatalogType.Id, Is.EqualTo(defaultCatalogType.Id));
        Assert.That(getCatalogItem.AvailableQty, Is.EqualTo(availableQty));
    }

    [Test]
    [Category("Update Item")]
    public async Task When_Client_Is_Unauthenticated_Then_Update_Item_Returns_Unathorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();
        var updateContent = createItemContent(new CatalogItemDTO());

        var response = await catalogItemClient.PutAsync(Item(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Update Item")]
    public async Task When_Client_Is_Not_Sales_Manager_Then_It_Cant_Update_Item()
    {
        testAuthenticationContextBuilder.SetAuthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();
        var updateContent = createItemContent(new CatalogItemDTO());

        var response = await catalogItemClient.PutAsync(Item(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Update Item")]
    public async Task When_Client_Is_Updating_Non_Existing_Item_Then_Not_Found_Returns()
    {
        testAuthenticationContextBuilder.SetAuthenticated().AsSalesManager();
        var catalogItemClient = webApplicationFactory.CreateClient();
        var updateContent = createItemContent(new CatalogItemDTO() { Name = "Test" });

        var response = await catalogItemClient.PutAsync(Item(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("Update Item")]
    public async Task When_Client_Is_Updating_Item_Then_Updated_Version_Could_Be_Requested()
    {
        testAuthenticationContextBuilder.SetAuthenticated().AsSalesManager();
        var catalogItemClient = webApplicationFactory.CreateClient();
        var itemNameUpdated = "Test Item Updated";
        var descriptionUpdated = "Test Item Description Updated";
        var itemPriceUpdated = 55m;
        var availableQty = 467;
        var pictureURIUpdated = @"http:\\localhost\testpictureUpdated.png";
        var catalogBrand = await createCatalogBrandAsync("new catalog Brand");
        var catalogType = await createCatalogTypeAsync("new catalog Type");
        var updateContent = createItemContent(new CatalogItemDTO()
        {
            Name = itemNameUpdated,
            Description = descriptionUpdated,
            Price = itemPriceUpdated,
            PictureUri = pictureURIUpdated,
            BrandId = catalogBrand.Id,
            TypeId = catalogType.Id,
            Ts = defaultCatalogItem!.Ts,
            AvailableQty = availableQty
        });

        var response = await catalogItemClient.PutAsync(Item(defaultCatalogItem.Id), updateContent);

        var updatedItem = await fromHttpResponseMessage<CatalogItemReadModel>(response);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(updatedItem, Is.Not.Null);
        Assert.That(updatedItem.Name, Is.EqualTo(itemNameUpdated));
        Assert.That(updatedItem.Description, Is.EqualTo(descriptionUpdated));
        Assert.That(updatedItem.Price, Is.EqualTo(itemPriceUpdated));
        Assert.That(updatedItem.CatalogTypeId, Is.EqualTo(catalogType.Id));
        Assert.That(updatedItem.CatalogType.Id, Is.EqualTo(catalogType.Id));
        Assert.That(updatedItem.CatalogBrand.Id, Is.EqualTo(catalogBrand.Id));
        Assert.That(updatedItem.AvailableQty, Is.EqualTo(availableQty));
    }


    [Test]
    [Category("Delete Item")]
    public async Task When_Client_Is_Unauthenticated_Then_Delete_Item_Returns_Unathorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.DeleteAsync(Item(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Delete Item")]
    public async Task When_Client_Is_Not_Sales_Manager_Then_It_Cant_Delete_Item()
    {
        testAuthenticationContextBuilder.SetAuthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.DeleteAsync(Item(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Delete Item")]
    public async Task When_Client_Is_Deleting_Non_Existing_Item_Then_Not_Found_Returns()
    {
        testAuthenticationContextBuilder.SetAuthenticated().AsSalesManager();
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.DeleteAsync(Item(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("Delete Item")]
    public async Task When_Client_Is_Deleting_Item_Then_Item_Could_Not_Be_Requested_Anymore()
    {
        testAuthenticationContextBuilder.SetAuthenticated().AsSalesManager();
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.DeleteAsync(Item(defaultCatalogItem.Id));

        var getResponse = await catalogItemClient.GetAsync(Item(defaultCatalogItem.Id));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("List Items")]
    public async Task When_Client_Is_Unauthenitcated_Then_List_Items_Returns_Unathorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();
        var listCatalogItemQuery = new ListCatalogItemQuery();

        var response = await catalogItemClient.GetAsync(Items(listCatalogItemQuery));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("List Catalog Items")]
    public async Task When_User_Is_Administrator_Then_It_Can_List_Item()
    {
        await createCatalogItemAsync("A Item");
        await createCatalogItemAsync("B Item");
        testAuthenticationContextBuilder.SetAuthenticated().AsSalesManager();
        var catalogItemClient = webApplicationFactory.CreateClient();
        var listCatalogItemQuery = new ListCatalogItemQuery()
        {
            OrderByDirection = OrderByDirections.DESC,
            PageIndex = 2,
            PageSize = 1
        };

        var response = await catalogItemClient.GetAsync(Items(listCatalogItemQuery));
        var listReponse = await fromHttpResponseMessage<ListCatalogItemResult>(response);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(listReponse, Is.Not.Null);
        Assert.That(listReponse.TotalCount, Is.EqualTo(3));
        Assert.That(listReponse.CatalogItems[0].Name, Is.EqualTo("A Item"));
    }

    private static StringContent createItemContent(CatalogItemDTO ItemToCreate)
    {
        var content = new StringContent(JsonSerializer.Serialize(ItemToCreate));

        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        return content;
    }

    public static string Item(Guid catalogItemId) => $"{API_BASE_URL}/{catalogItemId}";

    public static string Items(ListCatalogItemQuery listCatalogItemQuery) =>
         $"{API_BASE_URL}/list?{ConvertToQueryParams(listCatalogItemQuery)}";

    public static class Post
    {
        public static string Item => $"{API_BASE_URL}";
    }
}