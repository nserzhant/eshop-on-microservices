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
[Category("Catalog Type")]
public class CatalogTypeControllerTests : BaseCatalogControllerTests
{
    private const string API_BASE_URL = "api/CatalogType";
    private Guid defaultCatalogTypeId = Guid.Empty;
    private CatalogType? defaultType = null;
    
    public override async Task SetupAsync()
    {
        await base.SetupAsync();

        defaultType = await createCatalogTypeAsync("default type");
        defaultCatalogTypeId = defaultType.Id;
    }

    [Test]
    [Category("Create Catalog Type")]
    public async Task When_User_Is_Unauthenticated_Then_Create_Catalog_Type_Returns_Unauthorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogTypeClient = webApplicationFactory.CreateClient();
        var content = createCatalogTypeContent(new CatalogTypeDTO());

        var response = await catalogTypeClient.PostAsync(Post.CatalogType, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Create Catalog Type")]
    public async Task When_Employee_Is_Not_Sales_Manager_Then_Create_Catalog_Type_Returns_Forbidden()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee);
        var catalogTypeClient = webApplicationFactory.CreateClient();
        var content = createCatalogTypeContent(new CatalogTypeDTO());

        var response = await catalogTypeClient.PostAsync(Post.CatalogType, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Create Catalog Type")]
    public async Task When_Employee_Is_Sales_Manager_Then_Catalog_Type_Can_Be_Created()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
        var catalogTypeClient = webApplicationFactory.CreateClient();
        var content = createCatalogTypeContent(new CatalogTypeDTO() { Type = "Test type" });

        var response = await catalogTypeClient.PostAsync(Post.CatalogType, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    [Category("Create Catalog Type")]
    public async Task When_Customer_Creates_Catalog_Type_Then_Unauthorized_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Customer);
        var catalogTypeClient = webApplicationFactory.CreateClient();
        var content = createCatalogTypeContent(new CatalogTypeDTO() { Type = "Test type" });

        var response = await catalogTypeClient.PostAsync(Post.CatalogType, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Get Catalog Type")]
    public async Task When_User_Is_Unauthenticated_Then_Catalog_Type_Can_Be_Retrieved()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogTypeClient = webApplicationFactory.CreateClient();

        var response = await catalogTypeClient.GetAsync(CatalogType(defaultCatalogTypeId));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Get Catalog Type")]
    public async Task When_User_Requests_Non_Existing_Catalog_Type_Then_Not_Found_Is_Returned()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogTypeClient = webApplicationFactory.CreateClient();

        var response = await catalogTypeClient.GetAsync(CatalogType(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("Create Catalog Type")]
    public async Task When_Employee_Creates_Catalog_Type_Then_Catalog_Type_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
        var catalogTypeClient = webApplicationFactory.CreateClient();
        var catalogType = new CatalogTypeDTO { Type = "Test catalog type" };
        var content = createCatalogTypeContent(catalogType);

        var response = await catalogTypeClient.PostAsync(Post.CatalogType, content);

        var createdCatalogType = await fromHttpResponseMessage<CatalogTypeReadModel>(response);
        Assert.That(createdCatalogType, Is.Not.Null);
        Assert.That(createdCatalogType.Type, Is.EqualTo("Test catalog type"));
    }

    [Test]
    [Category("Create Catalog Type")]
    [Category("Get Catalog Type")]
    public async Task When_Employee_Creates_Catalog_Type_Then_It_Can_Be_Retrieved_By_Id ()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
        var catalogTypeClient = webApplicationFactory.CreateClient();
        var catalogType = new CatalogTypeDTO { Type = "Test catalog type" };
        var content = createCatalogTypeContent(catalogType);
        var response = await catalogTypeClient.PostAsync(Post.CatalogType, content);
        var createdCatalogType = await fromHttpResponseMessage<CatalogTypeReadModel>(response);

        var getResponse = await catalogTypeClient.GetAsync(CatalogType(createdCatalogType!.Id));

        var getCatalogType = await fromHttpResponseMessage<CatalogTypeReadModel>(getResponse);
        Assert.That(getCatalogType, Is.Not.Null);
        Assert.That(getCatalogType.Type, Is.EqualTo("Test catalog type"));
    }

    [Test]
    [Category("Update Catalog Type")]
    public async Task When_User_Is_Unauthenticated_Then_Update_Catalog_Type_Returns_Unauthorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogTypeClient = webApplicationFactory.CreateClient();
        var updateContent = createCatalogTypeContent(new CatalogTypeDTO());

        var response = await catalogTypeClient.PutAsync(CatalogType(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Update Catalog Type")]
    public async Task When_Employee_Is_Not_Sales_Manager_Then_Update_Catalog_Type_Returns_Forbidden()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee);
        var catalogTypeClient = webApplicationFactory.CreateClient();
        var updateContent = createCatalogTypeContent(new CatalogTypeDTO());

        var response = await catalogTypeClient.PutAsync(CatalogType(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Update Catalog Type")]
    public async Task When_Customer_Updates_Catalog_Type_Then_Unauthorized_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Customer);
        var catalogTypeClient = webApplicationFactory.CreateClient();
        var updateContent = createCatalogTypeContent(new CatalogTypeDTO());

        var response = await catalogTypeClient.PutAsync(CatalogType(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Update Catalog Type")]
    public async Task When_Employee_Updates_Non_Existing_Catalog_Type_Then_Not_Found_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
        var catalogTypeClient = webApplicationFactory.CreateClient();
        var updateContent = createCatalogTypeContent(new CatalogTypeDTO() { Type = "Test" });

        var response = await catalogTypeClient.PutAsync(CatalogType(Guid.NewGuid()), updateContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("Update Catalog Type")]
    public async Task When_Employee_Updates_Catalog_Type_Then_Updated_Version_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee).AsSalesManager();
        var catalogTypeClient = webApplicationFactory.CreateClient();
        var updateContent = createCatalogTypeContent(new CatalogTypeDTO()
        {
            Type = "Test updated",
            Ts = defaultType!.Ts
        });

        var response = await catalogTypeClient.PutAsync(CatalogType(defaultCatalogTypeId), updateContent);

        var updatedType = await fromHttpResponseMessage<CatalogTypeReadModel>(response);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(updatedType, Is.Not.Null);
        Assert.That(updatedType.Type, Is.EqualTo("Test updated"));
    }


    [Test]
    [Category("Delete Catalog Type")]
    public async Task When_User_Is_Unauthenticated_Then_Delete_Catalog_Type_Returns_Unauthorized()
    {
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogTypeClient = webApplicationFactory.CreateClient();

        var response = await catalogTypeClient.DeleteAsync(CatalogType(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Delete Catalog Type")]
    public async Task When_Employee_Is_Not_Sales_Manager_Then_Delete_Catalog_Type_Returns_Forbidden()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee);
        var catalogTypeClient = webApplicationFactory.CreateClient();

        var response = await catalogTypeClient.DeleteAsync(CatalogType(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Delete Catalog Type")]
    public async Task When_Employee_Deletes_Non_Existing_Catalog_Type_Then_Not_Found_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee)
            .AsSalesManager();
        var catalogTypeClient = webApplicationFactory.CreateClient();

        var response = await catalogTypeClient.DeleteAsync(CatalogType(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("Delete Catalog Type")]
    public async Task When_Customer_Deletes_Catalog_Type_Then_Unauthorized_Is_Returned()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Customer);
        var catalogTypeClient = webApplicationFactory.CreateClient();

        var response = await catalogTypeClient.DeleteAsync(CatalogType(defaultCatalogTypeId));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Delete Catalog Type")]
    public async Task When_Employee_Deletes_Catalog_Type_Then_Catalog_Type_Can_Not_Be_Retrieved_Anymore()
    {
        testAuthenticationContextBuilder.SetAuthenticated(AuthenticationSchemeNames.Employee).AsSalesManager();
        var catalogTypeClient = webApplicationFactory.CreateClient();

        var response = await catalogTypeClient.DeleteAsync(CatalogType(defaultCatalogTypeId));

        var getResponse = await catalogTypeClient.GetAsync(CatalogType(defaultCatalogTypeId));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("List Catalog Types")]
    public async Task When_User_Is_Unauthenticated_Then_Catalog_Types_Can_Be_Listed()
    {
        await createCatalogTypeAsync("A Type");
        await createCatalogTypeAsync("B Type");
        testAuthenticationContextBuilder.SetUnauthenticated();
        var catalogTypeClient = webApplicationFactory.CreateClient();
        var listCatalogTypeQuery = new ListCatalogTypeQuery()
        {
            OrderByDirection = OrderByDirections.DESC,
            PageIndex = 2,
            PageSize = 1
        };

        var response = await catalogTypeClient.GetAsync(CatalogTypes(listCatalogTypeQuery));

        var listReponse = await fromHttpResponseMessage<ListCatalogTypeResult>(response);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(listReponse, Is.Not.Null);
        Assert.That(listReponse.TotalCount, Is.EqualTo(3));
        Assert.That(listReponse.CatalogTypes[0].Type, Is.EqualTo("A Type"));
    }

    private static StringContent createCatalogTypeContent(CatalogTypeDTO TypeToCreate)
    {
        var content = new StringContent(JsonSerializer.Serialize(TypeToCreate));

        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        return content;
    }

    public static string CatalogType(Guid catalogItemId) => $"{API_BASE_URL}/{catalogItemId}";

    public static string CatalogTypes(ListCatalogTypeQuery listCatalogTypeQuery) =>
         $"{API_BASE_URL}/list?{ConvertToQueryParams(listCatalogTypeQuery)}";

    public static class Post
    {
        public static string CatalogType => $"{API_BASE_URL}";
    }
}
