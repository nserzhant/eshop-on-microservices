using Microsoft.Net.Http.Headers;
using EShop.Catalog.Api.Constants;
using EShop.Catalog.Api.Models;
using EShop.Catalog.Core.Models;
using EShop.Catalog.Infrastructure.Read.Queries;
using EShop.Catalog.Infrastructure.Read.Queries.Results;
using EShop.Catalog.Infrastructure.Read.ReadModels;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace EShop.Catalog.Api.IntegrationTests;

[TestFixture]
[Category("Catalog Item")]
public class CatalogItemControllerTests : BaseCatalogControllerTests
{
    private const string API_BASE_URL = "api/CatalogItem";
    private CatalogItem defaultCatalogItem;
    private CatalogType defaultCatalogType;
    private CatalogBrand defaultCatalogBrand;

    public override async Task SetupAsync()
    {
        await base.SetupAsync();

        defaultCatalogBrand = await createCatalogBrandAsync("default brand");
        defaultCatalogType = await createCatalogTypeAsync("default catalog type");
        defaultCatalogItem = await createCatalogItemAsync("default Item");
    }

    [Test]
    [Category("Create Item")]
    public async Task When_User_Is_Unauthenticated_Then_Create_Item_Returns_Unauthorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();
        var content = createItemContent(new CatalogItemDTO());

        var response = await catalogItemClient.PostAsync(Post.Item, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Create Item")]
    public async Task When_Employee_Is_Not_Sales_Manager_Then_Create_Item_Returns_Forbidden()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee);
        var catalogItemClient = webApplicationFactory.CreateClient();
        var content = createItemContent(new CatalogItemDTO());

        var response = await catalogItemClient.PostAsync(Post.Item, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Create Item")]
    public async Task When_Customer_Creates_Item_Then_Unauthorized_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Customer);
        var catalogItemClient = webApplicationFactory.CreateClient();
        var content = createItemContent(new CatalogItemDTO());

        var response = await catalogItemClient.PostAsync(Post.Item, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Create Item")]
    public async Task When_Employee_Is_Sales_Manager_Then_Item_Can_Be_Created()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee).AsSalesManager();
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
    public async Task When_User_Is_Unauthenticated_Then_Item_Can_Be_Retreived()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.GetAsync(Item(defaultCatalogItem.Id));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Get Item")]
    public async Task When_User_Requests_Non_Existing_Item_Then_Not_Found_Is_Returned()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.GetAsync(Item(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("Create Item")]
    public async Task When_Employee_Creates_Item_Then_Item_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
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
        Assert.That(createdCatalogItem.Name, Is.EqualTo(itemName));
        Assert.That(createdCatalogItem.Description, Is.EqualTo(description));
        Assert.That(createdCatalogItem.Price, Is.EqualTo(price));
        Assert.That(createdCatalogItem.PictureUri, Is.EqualTo(pictureURI));
        Assert.That(createdCatalogItem.CatalogBrandId, Is.EqualTo(defaultCatalogBrand.Id));
        Assert.That(createdCatalogItem.CatalogBrand.Id, Is.EqualTo(defaultCatalogBrand.Id));
        Assert.That(createdCatalogItem.CatalogType.Id, Is.EqualTo(defaultCatalogType.Id));
        Assert.That(createdCatalogItem.AvailableQty, Is.EqualTo(availableQty));
    }

    [Test]
    [Category("Get Item")]
    public async Task When_Employee_Creates_Item_Then_It_Can_Be_Retrieved_By_Id()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
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
        var createdCatalogItemId = createdCatalogItem.Id;

        var getResponse = await catalogItemClient.GetAsync(Item(createdCatalogItemId!));

        var getCatalogItem = await fromHttpResponseMessage<CatalogItemReadModel>(getResponse);
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
    public async Task When_User_Is_Unauthenticated_Then_Update_Item_Returns_Unauthorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();
        var updateContent = createItemContent(new CatalogItemDTO());

        var response = await catalogItemClient.PutAsync(Item(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Update Item")]
    public async Task When_Employee_Is_Not_Sales_Manager_Then_Update_Item_Returns_Forbidden()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee);
        var catalogItemClient = webApplicationFactory.CreateClient();
        var updateContent = createItemContent(new CatalogItemDTO());

        var response = await catalogItemClient.PutAsync(Item(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Update Item")]
    public async Task When_Customer_Updates_Item_Then_Unauthorized_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Customer);
        var catalogItemClient = webApplicationFactory.CreateClient();
        var updateContent = createItemContent(new CatalogItemDTO());

        var response = await catalogItemClient.PutAsync(Item(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Update Item")]
    public async Task When_Employee_Updates_Non_Existing_Item_Then_Not_Found_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
        var catalogItemClient = webApplicationFactory.CreateClient();
        var updateContent = createItemContent(new CatalogItemDTO() { Name = "Test" });

        var response = await catalogItemClient.PutAsync(Item(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("Update Item")]
    public async Task When_Employee_Updates_Item_Then_Updated_Version_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
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
    public async Task When_User_Is_Unauthenticated_Then_Delete_Item_Returns_Unauthorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.DeleteAsync(Item(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Delete Item")]
    public async Task When_Employee_Is_Not_Sales_Manager_Then_Delete_Item_Returns_Forbidden()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee);
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.DeleteAsync(Item(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Delete Item")]
    public async Task When_Employee_Deletes_Non_Existing_Item_Then_Not_Found_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee).AsSalesManager();
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.DeleteAsync(Item(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("Delete Item")]
    public async Task When_Customer_Deletes_Item_Then_Unauthorized_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Customer);
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.DeleteAsync(Item(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Delete Item")]
    public async Task When_Employee_Deletes_Item_Then_Item_Can_Not_Be_Retrieved_Anymore()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee).AsSalesManager();
        var catalogItemClient = webApplicationFactory.CreateClient();

        var response = await catalogItemClient.DeleteAsync(Item(defaultCatalogItem.Id));

        var getResponse = await catalogItemClient.GetAsync(Item(defaultCatalogItem.Id));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("List Catalog Items")]
    public async Task When_User_Is_Unauthenticated_Then_Items_Can_Be_Listed()
    {
        await createCatalogItemAsync("A Item");
        await createCatalogItemAsync("B Item");
        testAuthenticationContextBuilder.SetUnauthenticated();
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