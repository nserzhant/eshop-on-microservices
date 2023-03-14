using Microsoft.Net.Http.Headers;
using R2S.Catalog.Api.Constants;
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
[Category("Catalog Brand")]
public class CatalogBrandControllerTests : BaseCatalogControllerTests
{
    private const string API_BASE_URL = "api/CatalogBrand";
    private Guid defaultCatalogBrandId = Guid.Empty;
    private CatalogBrand? defaultBrand = null;

    [SetUp]
    public async Task SetupAsync()
    {
        await base.SetupAsync();
        
        defaultBrand = await createCatalogBrandAsync("default brand");
        defaultCatalogBrandId = defaultBrand.Id;
    }

    [Test]
    [Category("Create Brand")]
    public async Task When_User_Is_Unauthenticated_Then_Create_Brand_Returns_Unathorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogBrandClient = webApplicationFactory.CreateClient();
        var content = createBrandContent(new CatalogBrandDTO());

        var response = await catalogBrandClient.PostAsync(Post.Brand, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Create Brand")]
    public async Task When_Employee_Is_Not_Sales_Manager_Then_Create_Brand_Returns_Forbidden()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee);
        var catalogBrandClient = webApplicationFactory.CreateClient();
        var content = createBrandContent(new CatalogBrandDTO());

        var response = await catalogBrandClient.PostAsync(Post.Brand, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Create Brand")]
    public async Task When_Employee_Is_Sales_Manager_Then_Brand_Can_Be_Created()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee).AsSalesManager();
        var catalogBrandClient = webApplicationFactory.CreateClient();
        var content = createBrandContent(new CatalogBrandDTO() { Brand = "Test Brand" });

        var response = await catalogBrandClient.PostAsync(Post.Brand, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    [Category("Create Brand")]
    public async Task When_User_Is_Client_Then_Create_Brand_Returns_Unathorized()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Client);
        var catalogBrandClient = webApplicationFactory.CreateClient();
        var content = createBrandContent(new CatalogBrandDTO());

        var response = await catalogBrandClient.PostAsync(Post.Brand, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Get Brand")]
    public async Task When_User_Is_Unauthenticated_Then_Brand_Can_Be_Gotten()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogBrandClient = webApplicationFactory.CreateClient();

        var response = await catalogBrandClient.GetAsync(Brand(defaultCatalogBrandId));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Get Brand")]
    public async Task When_User_Is_Requesting_Non_Existing_Brand_Then_Not_Found_Returns()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogBrandClient = webApplicationFactory.CreateClient();

        var response = await catalogBrandClient.GetAsync(Brand(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("Create Brand")]
    [Category("Get Brand")]
    public async Task When_Employee_Created_The_Brand_Then_It_Can_Be_Requested_By_Id()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
        var catalogBrandClient = webApplicationFactory.CreateClient();
        var brand = new CatalogBrandDTO { Brand = "Test Brand" };
        var content = createBrandContent(brand);
        var response = await catalogBrandClient.PostAsync(Post.Brand, content);
        var createdCatalogBrand = await fromHttpResponseMessage<CatalogBrandReadModel>(response);

        Assert.That(createdCatalogBrand, Is.Not.Null);
        var createdCatalogBrandId = createdCatalogBrand.Id;

        var getResponse = await catalogBrandClient.GetAsync(Brand(createdCatalogBrandId));
        var getCatalogBrand = await fromHttpResponseMessage<CatalogBrandReadModel>(getResponse);

        Assert.That(createdCatalogBrand.Brand, Is.EqualTo("Test Brand"));
        Assert.That(getCatalogBrand, Is.Not.Null);
        Assert.That(getCatalogBrand.Brand, Is.EqualTo("Test Brand"));
    }


    [Test]
    [Category("Update Brand")]
    public async Task When_User_Is_Unauthenticated_Then_Update_Brand_Returns_Unathorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogBrandClient = webApplicationFactory.CreateClient();
        var updateContent = createBrandContent(new CatalogBrandDTO());

        var response = await catalogBrandClient.PutAsync(Brand(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Update Brand")]
    public async Task When_User_Is_Client_Then_Update_Brand_Returns_Unathorized()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Client);
        var catalogBrandClient = webApplicationFactory.CreateClient();
        var updateContent = createBrandContent(new CatalogBrandDTO());

        var response = await catalogBrandClient.PutAsync(Brand(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Update Brand")]
    public async Task When_Employee_Is_Not_Sales_Manager_Then_Update_Brand_Returns_Forbidden()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee);
        var catalogBrandClient = webApplicationFactory.CreateClient();
        var updateContent = createBrandContent(new CatalogBrandDTO());

        var response = await catalogBrandClient.PutAsync(Brand(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Update Brand")]
    public async Task When_Employee_Is_Updating_Non_Existing_Brand_Then_Not_Found_Returns()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
        var catalogBrandClient = webApplicationFactory.CreateClient();
        var updateContent = createBrandContent(new CatalogBrandDTO() { Brand = "Test" });

        var response = await catalogBrandClient.PutAsync(Brand(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("Update Brand")]
    public async Task When_Employee_Is_Updated_Brand_Then_Updated_Version_Could_Be_Gotten()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
        var catalogBrandClient = webApplicationFactory.CreateClient();            
        var updateContent = createBrandContent(new CatalogBrandDTO() 
        { 
            Brand = "Test updated",
            Ts = defaultBrand!.Ts
        });

        var response = await catalogBrandClient.PutAsync(Brand(defaultCatalogBrandId), updateContent);

        var updatedBrand = await fromHttpResponseMessage<CatalogBrandReadModel>(response);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(updatedBrand, Is.Not.Null);
        Assert.That(updatedBrand.Brand, Is.EqualTo("Test updated"));
    }


    [Test]
    [Category("Delete Brand")]
    public async Task When_User_Is_Unauthenticated_Then_Delete_Brand_Returns_Unathorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogBrandClient = webApplicationFactory.CreateClient();

        var response = await catalogBrandClient.DeleteAsync(Brand(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Delete Brand")]
    public async Task When_Employee_Is_Not_Sales_Manager_Then_Delete_Brand_Returns_Forbidden()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee);
        var catalogBrandClient = webApplicationFactory.CreateClient();

        var response = await catalogBrandClient.DeleteAsync(Brand(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Delete Brand")]
    public async Task When_Employee_Is_Deleting_Non_Existing_Brand_Then_Not_Found_Returns()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
        var catalogBrandClient = webApplicationFactory.CreateClient();

        var response = await catalogBrandClient.DeleteAsync(Brand(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("Delete Brand")]
    public async Task When_User_Is_Client_Then_Delete_Brand_Returns_Unathorized()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Client);
        var catalogBrandClient = webApplicationFactory.CreateClient();

        var response = await catalogBrandClient.DeleteAsync(Brand(defaultCatalogBrandId));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Delete Brand")]
    public async Task When_Employee_Is_Deleting_Brand_Then_Brand_Could_Not_Be_Requested_Anymore()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
        var catalogBrandClient = webApplicationFactory.CreateClient();

        var response = await catalogBrandClient.DeleteAsync(Brand(defaultCatalogBrandId));

        var getResponse =await catalogBrandClient.GetAsync(Brand(defaultCatalogBrandId));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("List Catalog Brands")]
    public async Task When_User_Is_Unauthenitcated_Then_Brands_Can_Be_Listed()
    {
        await createCatalogBrandAsync("A Brand");
        await createCatalogBrandAsync("B brand");
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogBrandClient = webApplicationFactory.CreateClient();
        var listCatalogBrandQuery = new ListCatalogBrandQuery()
        {
            OrderByDirection = OrderByDirections.DESC,
            PageIndex = 2,
            PageSize = 1
        };

        var response = await catalogBrandClient.GetAsync(Brands(listCatalogBrandQuery));
        var listReponse = await fromHttpResponseMessage<ListCatalogBrandResult>(response);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(listReponse, Is.Not.Null);
        Assert.That(listReponse.TotalCount, Is.EqualTo(3));
        Assert.That(listReponse.CatalogBrands[0].Brand, Is.EqualTo("A Brand"));
    }

    private static StringContent createBrandContent(CatalogBrandDTO brandToCreate)
    {
        var content = new StringContent(JsonSerializer.Serialize(brandToCreate));

        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        return content;
    }

    public static string Brand(Guid catalogItemId) => $"{API_BASE_URL}/{catalogItemId}";

    public static string Brands(ListCatalogBrandQuery listCatalogBrandQuery) =>
         $"{API_BASE_URL}/list?{ConvertToQueryParams(listCatalogBrandQuery)}";

    public static class Post
    {
        public static string Brand => $"{API_BASE_URL}";
    }
}