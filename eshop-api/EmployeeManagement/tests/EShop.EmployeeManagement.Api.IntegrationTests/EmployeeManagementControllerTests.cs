using Microsoft.Net.Http.Headers;
using EShop.EmployeeManagement.Core.Enums;
using EShop.EmployeeManagement.Core.Read.Queries;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace EShop.EmployeeManagement.Api.IntegrationTests;

public class EmployeeManagementControllerTests : BaseControllerTests
{
    private const string API_BASE_URL = "api/EmployeeManagement";

    [SetUp]
    public override async Task Setup()
    {
        await base.Setup();
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();
    }

    [Test]
    [Category("Get Employee")]
    public async Task When_Employee_Is_Unauthenitcated_Then_Get_Employee_By_Id_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var employeeManagementClient = _webApplicationFactory.CreateClient();

        var response = await employeeManagementClient.GetAsync(Get.Employee(defaultemployeeId));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Get Employee")]
    public async Task When_Employee_Is_Sales_Manager_Then_It_Cant_Get_Employee_By_Id()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.SalesManager);
        var employeeManagementClient = _webApplicationFactory.CreateClient();

        var response = await employeeManagementClient.GetAsync(Get.Employee(defaultemployeeId));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Get Employee")]
    public async Task When_Employee_Is_Administrator_Then_It_Can_Get_Employee_By_Id()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        
        var response = await employeeManagementClient.GetAsync(Get.Employee(defaultemployeeId));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }


    [Test]
    [Category("Get Employee")]
    public async Task When_Getting_Non_Existing_Employee_Then_Not_Found_Should_Be_Returned()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
        var employeeManagementClient = _webApplicationFactory.CreateClient();

        var response = await employeeManagementClient.GetAsync(Get.Employee(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("List Employees")]
    public async Task When_Employee_Is_Unauthenitcated_Then_List_Employees_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var listEmployeeQuery = new ListEmployeeQuery()
        {
            OrderBy = ListEmployeeOrderBy.Email,
            OrderByDirection = OrderByDirections.ASC,
            PageIndex = 1,
            PageSize = 1
        };

        var response = await employeeManagementClient.GetAsync(Get.Employees(listEmployeeQuery));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("List Employees")]
    public async Task When_Employee_Is_Administrator_Then_It_Can_List_Employees()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var listEmployeeQuery = new ListEmployeeQuery()
        {
            OrderBy = ListEmployeeOrderBy.UserName,
            OrderByDirection = OrderByDirections.ASC,
            PageIndex = 1,
            PageSize = 1
        };

        var response = await employeeManagementClient.GetAsync(Get.Employees(listEmployeeQuery));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("List Employees")]
    public async Task When_Employee_Is_Sales_Manager_Then_It_Cant_List_Employees()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.SalesManager);
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var listEmployeeQuery = new ListEmployeeQuery()
        {
            OrderBy = ListEmployeeOrderBy.Email,
            OrderByDirection = OrderByDirections.ASC,
            PageIndex = 1,
            PageSize = 1
        };

        var response = await employeeManagementClient.GetAsync(Get.Employees(listEmployeeQuery));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Update Employee Roles")]
    public async Task When_Employee_Is_Unauthenitcated_Then_Save_Roles_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var roles = new Roles[] { Roles.Administrator, Roles.SalesManager };
        var content = new StringContent(JsonSerializer.Serialize(roles));
        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        var response = await employeeManagementClient.PatchAsync(Patch.Roles(defaultemployeeId), content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Update Employee Roles")]
    public async Task When_Employee_Is_Administrator_Then_It_Can_Save_Roles()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var roles = new Roles[] { Roles.Administrator, Roles.SalesManager };
        var content = new StringContent(JsonSerializer.Serialize(roles));
        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        var response = await employeeManagementClient.PatchAsync(Patch.Roles(defaultemployeeId), content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Update Employee Roles")]
    public async Task When_Employee_Is_Sales_Manager_Then_It_Cant_Save_Roles()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.SalesManager);
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var roles = new Roles[] { Roles.Administrator, Roles.SalesManager };
        var content = new StringContent(JsonSerializer.Serialize(roles));
        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        var response = await employeeManagementClient.PatchAsync(Patch.Roles(defaultemployeeId), content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Update Password")]
    public async Task When_Employee_Is_Unauthenitcated_Then_Update_Password_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize("passworD!c^12"));
        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        var response = await employeeManagementClient.PatchAsync(Patch.Password(defaultemployeeId), content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Update Password")]
    public async Task When_Employee_Is_Administrator_Then_It_Can_Change_Password()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize("passworD!c^12"));
        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        var response = await employeeManagementClient.PatchAsync(Patch.Password(defaultemployeeId), content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Update Password")]
    public async Task When_Employee_Is_Sales_Manager_Then_It_Cant_Change_Password()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.SalesManager);
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize("passworD!c^12"));
        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        var response = await employeeManagementClient.PatchAsync(Patch.Password(defaultemployeeId), content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    public static class Get
    {
        public static string Employee(Guid employeeId) => $"{API_BASE_URL}/{employeeId}";

        public static string Employees(ListEmployeeQuery listEmployeeQuery) => $"{API_BASE_URL}/list?{ConvertToQueryParams(listEmployeeQuery)}";

    }

    public static class Patch
    {
        public static string Roles(Guid employeeId) => $"{API_BASE_URL}/{employeeId}/roles";
        public static string Password(Guid employeeId) => $"{API_BASE_URL}/{employeeId}/password";
    }
}

