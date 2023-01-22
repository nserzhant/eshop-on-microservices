using Microsoft.Net.Http.Headers;
using R2S.EmployeeManagement.Api.Models;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace R2S.EmployeeManagement.Api.IntegrationTests
{
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
        [Category("Register User")]
        public async Task When_User_Is_Unauthenitcated_Then_User_Can_Register()
        {
            _testAuthenticationContextBuilder.SetUnauthenticated();
            var usersClient = _webApplicationFactory.CreateClient();
            var userDTO = new UserDTO { Email = "sample@email.com", Password = defaultUserPassword };
            var content = new StringContent(JsonSerializer.Serialize(userDTO));
            content.Headers.Remove(HeaderNames.ContentType);
            content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);


            var response = await usersClient.PostAsync(Post.Register, content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Category("Change Password")]
        public async Task When_User_Is_Authenticated_Then_Passwod_Could_Be_Changed()
        {
            _testAuthenticationContextBuilder.SetAuthorizedAs(defaultUserId);
            var usersClient = _webApplicationFactory.CreateClient();
            var changePasswodDTO = new ChangePasswordDTO { OldPassword = defaultUserPassword, NewPassword = $"{defaultUserPassword}_updated" };
            var content = new StringContent(JsonSerializer.Serialize(changePasswodDTO));
            content.Headers.Remove(HeaderNames.ContentType);
            content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

            var response = await usersClient.PatchAsync(Patch.ChangePassword, content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Category("Change Password")]
        public async Task When_User_Is_Unauthenitcated_Then_Change_Password_Returns_Unathorized()
        {
            _testAuthenticationContextBuilder.SetUnauthenticated();
            var usersClient = _webApplicationFactory.CreateClient();
            var changePasswodDTO = new ChangePasswordDTO { OldPassword = defaultUserPassword, NewPassword = $"{defaultUserPassword}_updated" };
            var content = new StringContent(JsonSerializer.Serialize(changePasswodDTO));
            content.Headers.Remove(HeaderNames.ContentType);
            content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

            var response = await usersClient.PatchAsync(Patch.ChangePassword, content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        [Category("Change Email")]
        public async Task When_User_Is_Authenticated_Then_Email_Could_Be_Changed()
        {
            _testAuthenticationContextBuilder.SetAuthorizedAs(defaultUserId);
            var usersClient = _webApplicationFactory.CreateClient();
            var userDTO = new UserDTO { Email = "newEmail@email.com", Password = defaultUserPassword };
            var content = new StringContent(JsonSerializer.Serialize(userDTO));
            content.Headers.Remove(HeaderNames.ContentType);
            content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

            var response = await usersClient.PatchAsync(Patch.ChangeEmail, content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Category("Change Email")]
        public async Task When_User_Is_Unauthenitcated_Then_Change_Email_Returns_Unathorized()
        {
            _testAuthenticationContextBuilder.SetUnauthenticated();
            var usersClient = _webApplicationFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize("newEmail@email.com"));
            content.Headers.Remove(HeaderNames.ContentType);
            content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

            var response = await usersClient.PatchAsync(Patch.ChangeEmail, content);

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
}
