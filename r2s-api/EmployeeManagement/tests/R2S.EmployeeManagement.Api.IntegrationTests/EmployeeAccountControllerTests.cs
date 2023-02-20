using Microsoft.Net.Http.Headers;
using R2S.EmployeeManagement.Api.Models;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace R2S.EmployeeManagement.Api.IntegrationTests;

public class EmployeeAccountControllerTests : BaseControllerTests
{
    private const string API_BASE_URL = "api/EmployeeAccount";

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
    [Category("Register Employee")]
    public async Task When_Employee_Is_Unauthenitcated_Then_Employee_Can_Register()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var employeeDTO = new EmployeeDTO { Email = "sample@email.com", Password = defaultPassword };
        var content = new StringContent(JsonSerializer.Serialize(employeeDTO));
        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        var response = await employeeManagementClient.PostAsync(Post.Register, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Change Password")]
    public async Task When_Employee_Is_Authenticated_Then_Passwod_Could_Be_Changed()
    {
        _testAuthenticationContextBuilder.SetAuthorizedAs(defaultemployeeId);
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var changePasswodDTO = new ChangePasswordDTO { OldPassword = defaultPassword, NewPassword = $"{defaultPassword}_updated" };
        var content = new StringContent(JsonSerializer.Serialize(changePasswodDTO));
        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        var response = await employeeManagementClient.PatchAsync(Patch.ChangePassword, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Change Password")]
    public async Task When_Employee_Is_Unauthenitcated_Then_Change_Password_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var changePasswodDTO = new ChangePasswordDTO { OldPassword = defaultPassword, NewPassword = $"{defaultPassword}_updated" };
        var content = new StringContent(JsonSerializer.Serialize(changePasswodDTO));
        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        var response = await employeeManagementClient.PatchAsync(Patch.ChangePassword, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Change Email")]
    public async Task When_Employee_Is_Authenticated_Then_Email_Could_Be_Changed()
    {
        _testAuthenticationContextBuilder.SetAuthorizedAs(defaultemployeeId);
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var employeeDTO = new EmployeeDTO { Email = "newEmail@email.com", Password = defaultPassword };
        var content = new StringContent(JsonSerializer.Serialize(employeeDTO));
        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        var response = await employeeManagementClient.PatchAsync(Patch.ChangeEmail, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Change Email")]
    public async Task When_Employee_Is_Unauthenitcated_Then_Change_Email_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var employeeManagementClient = _webApplicationFactory.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize("newEmail@email.com"));
        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        var response = await employeeManagementClient.PatchAsync(Patch.ChangeEmail, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    public static class Post
    {
        public static string Register => $"{API_BASE_URL}/register";
    }

    public static class Patch
    {
        public static string ChangePassword => $"{API_BASE_URL}/changepassword";
        public static string ChangeEmail => $"{API_BASE_URL}/changeemail";
    }
}
