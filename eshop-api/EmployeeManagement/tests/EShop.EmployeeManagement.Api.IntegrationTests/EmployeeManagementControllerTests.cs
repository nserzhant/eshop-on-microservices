using EShop.EmployeeManagement.Infrastructure.Entities;
using EShop.EmployeeManagement.Infrastructure.Enums;
using EShop.EmployeeManagement.Infrastructure.Read.Queries;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace EShop.EmployeeManagement.Api.IntegrationTests;

public class EmployeeManagementControllerTests : BaseControllerTests
{
    private const string API_BASE_URL = "api/EmployeeManagement";

    [Test]
    [Category("Get Employee")]
    public async Task When_User_Is_Unauthenticated_Then_Get_Employee_Returns_Unauthorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var employeeManagementClient = _webApplicationFactory.CreateClient();

        var response = await employeeManagementClient.GetAsync(Get.Employee(defaultemployeeId));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Get Employee")]
    public async Task When_Sales_Manager_Requests_Employee_Then_Forbidden_Is_Returned()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.SalesManager);
        var employeeManagementClient = _webApplicationFactory.CreateClient();

        var response = await employeeManagementClient.GetAsync(Get.Employee(defaultemployeeId));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Get Employee")]
    public async Task When_Administrator_Requests_Employee_Then_Ok_Is_Returned()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
        var employeeManagementClient = _webApplicationFactory.CreateClient();

        var response = await employeeManagementClient.GetAsync(Get.Employee(defaultemployeeId));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }


    [Test]
    [Category("Get Employee")]
    public async Task When_Administrator_Requests_Non_Existing_Employee_Then_Not_Found_Is_Returned()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
        var employeeManagementClient = _webApplicationFactory.CreateClient();

        var response = await employeeManagementClient.GetAsync(Get.Employee(Guid.NewGuid()));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("List Employees")]
    public async Task When_User_Is_Unauthenticated_Then_List_Employees_Returns_Unauthorized()
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
    public async Task When_Administrator_Lists_Employees_Then_List_Of_Employees_Should_Be_Returned()
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
    public async Task When_Sales_Manager_Lists_Employees_Then_Forbidden_Is_Returned()
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
    public async Task When_User_Is_Unauthenticated_Then_Change_Roles_Returns_Unauthorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var roles = new Roles[] { Roles.Administrator, Roles.SalesManager };
        var content = createStringContent(roles);

        var response = await employeeManagementClient.PatchAsync(Patch.Roles(defaultemployeeId), content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Update Employee Roles")]
    public async Task When_Administrator_Changes_Roles_Then_Roles_Should_Be_Successfully_Changed()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var roles = new Roles[] { Roles.Administrator, Roles.SalesManager };
        var content = createStringContent(roles);

        var response = await employeeManagementClient.PatchAsync(Patch.Roles(defaultemployeeId), content);

        var actualRoles = await userManager.GetRolesAsync(new Employee { Id = defaultemployeeId });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(actualRoles, Is.Not.Null);
        Assert.That(actualRoles.Count, Is.EqualTo(2));
        Assert.That(actualRoles, Does.Contain(Roles.Administrator.ToString()));
        Assert.That(actualRoles, Does.Contain(Roles.SalesManager.ToString()));
    }

    [Test]
    [Category("Update Employee Roles")]
    public async Task When_Sales_Manager_Changes_Roles_Then_Forbidden_Is_Returned()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.SalesManager);
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var roles = new Roles[] { Roles.Administrator, Roles.SalesManager };
        var content = createStringContent(roles);

        var response = await employeeManagementClient.PatchAsync(Patch.Roles(defaultemployeeId), content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Update Password")]
    public async Task When_User_Is_Unauthenticated_Then_Change_Password_Returns_Unauthorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var newPasswordContent = createStringContent("passworD!c^12");

        var response = await employeeManagementClient.PatchAsync(Patch.Password(defaultemployeeId), newPasswordContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Update Password")]
    public async Task When_Administrator_Changes_Password_Then_Password_Should_Be_Successfully_Changed()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var newPassword = "passworD!c^12";
        var newPasswordContent = createStringContent(newPassword);

        var response = await employeeManagementClient.PatchAsync(Patch.Password(defaultemployeeId), newPasswordContent);

        async Task assert() => await userManager.CheckPasswordAsync(new Employee { Email = defaultEmail }, newPassword);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(assert, Throws.Nothing);
    }

    [Test]
    [Category("Update Password")]
    public async Task When_Sales_Manager_Changes_Password_Then_Forbidden_Is_Returned()
    {
        _testAuthenticationContextBuilder.WithRole(Roles.SalesManager);
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var newPasswordContent = createStringContent("passworD!c^12");

        var response = await employeeManagementClient.PatchAsync(Patch.Password(defaultemployeeId), newPasswordContent);

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

    private static StringContent createStringContent<T>(T contentSource)
    {
        var content = new StringContent(JsonSerializer.Serialize(contentSource));

        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        return content;
    }
}

