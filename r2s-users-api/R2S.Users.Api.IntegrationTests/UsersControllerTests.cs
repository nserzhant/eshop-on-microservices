using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using NSubstitute;
using R2S.Users.Api.IntegrationTests.Infrastructure;
using R2S.Users.Core;
using R2S.Users.Core.Entities;
using R2S.Users.Core.Enums;
using R2S.Users.Core.IntegrationTests;
using R2S.Users.Core.IntegrationTests.Infrastructure;
using R2S.Users.Core.Read;
using R2S.Users.Core.Read.Queries;
using R2S.Users.Core.Services;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Web;

namespace R2S.Users.Api.IntegrationTests
{
    public class UsersControllerTests : BaseUsersIntegrationTests
    {
        private const string API_BASE_URL = "api/Users";

        private WebApplicationFactory<Program> _webApplicationFactory;
        private TestAuthenticationContextBuilder _testAuthenticationContextBuilder;
        private IUserService _userService;
        private IUserQueryService _userQueryService;
        private Guid _defaultUserId;

        [SetUp]
        public override async Task Setup()
        {
            await base.Setup();

            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");
            _testAuthenticationContextBuilder = new TestAuthenticationContextBuilder();
            _webApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    conf.AddJsonFile(configPath);
                });
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(s =>
                    {
                        s.DefaultAuthenticateScheme = "Test";
                        s.DefaultChallengeScheme = "Test";
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                    services.AddSingleton(_testAuthenticationContextBuilder);
                });
            });


            _userService = serviceProvier.GetRequiredService<IUserService>();
            _userQueryService = serviceProvier.GetRequiredService<IUserQueryService>();
            _defaultUserId = await createDefaultUserAsync("test@user.com");
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            _webApplicationFactory.Dispose();
        }

        [Test]
        [Category("Get User")]
        public async Task When_User_Is_Unauthenitcated_Then_Get_User_By_Id_Returns_Unathorized()
        {
            _testAuthenticationContextBuilder.SetUnauthenticated();
            var usersClient = _webApplicationFactory.CreateClient();

            var response = await usersClient.GetAsync(Get.User(_defaultUserId));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        [Category("Get User")]
        public async Task When_User_Is_Sales_Manager_Then_It_Cant_Get_User_By_Id()
        {
            _testAuthenticationContextBuilder.WithRole(Roles.SalesManager);
            var usersClient = _webApplicationFactory.CreateClient();

            var response = await usersClient.GetAsync(Get.User(_defaultUserId));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("Get User")]
        public async Task When_User_Is_Administrator_Then_It_Can_Get_User_By_Id()
        {
            _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
            var usersClient = _webApplicationFactory.CreateClient();
            
            var response = await usersClient.GetAsync(Get.User(_defaultUserId));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }


        [Test]
        [Category("Get User")]
        public async Task When_Getting_Non_Existing_User_Then_Not_Found_Should_Be_Returned()
        {
            _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
            var usersClient = _webApplicationFactory.CreateClient();

            var response = await usersClient.GetAsync(Get.User(Guid.NewGuid()));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("List Users")]
        public async Task When_User_Is_Unauthenitcated_Then_List_Users_Returns_Unathorized()
        {
            _testAuthenticationContextBuilder.SetUnauthenticated();
            var usersClient = _webApplicationFactory.CreateClient();
            var listUserQuery = new ListUserQuery()
            {
                OrderBy = ListUserOrderBy.Email,
                OrderByDirection = OrderByDirections.ASC,
                PageIndex = 1,
                PageSize = 1
            };

            var response = await usersClient.GetAsync(Get.Users(listUserQuery));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        [Category("List Users")]
        public async Task When_User_Is_Administrator_Then_It_Can_List_Users()
        {
            _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
            var usersClient = _webApplicationFactory.CreateClient();
            var listUserQuery = new ListUserQuery()
            {
                OrderBy = ListUserOrderBy.UserName,
                OrderByDirection = OrderByDirections.ASC,
                PageIndex = 1,
                PageSize = 1
            };

            var response = await usersClient.GetAsync(Get.Users(listUserQuery));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Category("List Users")]
        public async Task When_User_Is_Sales_Manager_Then_It_Cant_List_Users()
        {
            _testAuthenticationContextBuilder.WithRole(Roles.SalesManager);
            var usersClient = _webApplicationFactory.CreateClient();
            var listUserQuery = new ListUserQuery()
            {
                OrderBy = ListUserOrderBy.Email,
                OrderByDirection = OrderByDirections.ASC,
                PageIndex = 1,
                PageSize = 1
            };

            var response = await usersClient.GetAsync(Get.Users(listUserQuery));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("Update User Roles")]
        public async Task When_User_Is_Unauthenitcated_Then_Save_Roles_Returns_Unathorized()
        {
            _testAuthenticationContextBuilder.SetUnauthenticated();
            var usersClient = _webApplicationFactory.CreateClient();
            var roles = new Roles[] { Roles.Administrator, Roles.SalesManager };
            var content = new StringContent(JsonSerializer.Serialize(roles));
            content.Headers.Remove(HeaderNames.ContentType);
            content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

            var response = await usersClient.PatchAsync(Patch.Roles(_defaultUserId), content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        [Category("Update User Roles")]
        public async Task When_User_Is_Administrator_Then_It_Can_Save_Roles()
        {
            _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
            var usersClient = _webApplicationFactory.CreateClient();
            var roles = new Roles[] { Roles.Administrator, Roles.SalesManager };
            var content = new StringContent(JsonSerializer.Serialize(roles));
            content.Headers.Remove(HeaderNames.ContentType);
            content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

            var response = await usersClient.PatchAsync(Patch.Roles(_defaultUserId), content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Category("Update User Roles")]
        public async Task When_User_Is_Sales_Manager_Then_It_Cant_Save_Roles()
        {
            _testAuthenticationContextBuilder.WithRole(Roles.SalesManager);
            var usersClient = _webApplicationFactory.CreateClient();
            var roles = new Roles[] { Roles.Administrator, Roles.SalesManager };
            var content = new StringContent(JsonSerializer.Serialize(roles));
            content.Headers.Remove(HeaderNames.ContentType);
            content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

            var response = await usersClient.PatchAsync(Patch.Roles(_defaultUserId), content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }



        [Test]
        [Category("Update Password")]
        public async Task When_User_Is_Unauthenitcated_Then_Update_Password_Returns_Unathorized()
        {
            _testAuthenticationContextBuilder.SetUnauthenticated();
            var usersClient = _webApplicationFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize("passworD!c^12"));
            content.Headers.Remove(HeaderNames.ContentType);
            content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

            var response = await usersClient.PatchAsync(Patch.Password(_defaultUserId), content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        [Category("Update Password")]
        public async Task When_User_Is_Administrator_Then_It_Can_Change_Password()
        {
            _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
            var usersClient = _webApplicationFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize("passworD!c^12"));
            content.Headers.Remove(HeaderNames.ContentType);
            content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

            var response = await usersClient.PatchAsync(Patch.Password(_defaultUserId), content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Category("Update Password")]
        public async Task When_User_Is_Sales_Manager_Then_It_Cant_Change_Password()
        {
            _testAuthenticationContextBuilder.WithRole(Roles.SalesManager);
            var usersClient = _webApplicationFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize("passworD!c^12"));
            content.Headers.Remove(HeaderNames.ContentType);
            content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

            var response = await usersClient.PatchAsync(Patch.Password(_defaultUserId), content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        private async Task<Guid> createDefaultUserAsync(string userEmail)
        {
            var password = "3242f$fDc%dD";
            await _userService.Register(userEmail, password);
            var user = await _userQueryService.GetByEmail(userEmail);
            await _userService.SaveUserRoles(user.Id, Roles.SalesManager);

            return user.Id;
        }

        private static string ConvertToQueryParams<T>(T obj) where T : class
        {
            var stringBuilder = new StringBuilder();
            var t = obj.GetType();
            var properties = t.GetProperties();

            foreach (PropertyInfo p in properties)
            {
                var val = p.GetValue(obj);

                if (val != null)
                {

                    stringBuilder.Append(String.Format("{0}={1}&", p.Name, val.ToString()));
                }
            }

            return stringBuilder.ToString().TrimEnd('&');
        }

        public static class Get
        {
            public static string User(Guid userId) => $"{API_BASE_URL}/{userId}";

            public static string Users(ListUserQuery listUserQuery) => $"{API_BASE_URL}/list?{ConvertToQueryParams(listUserQuery)}";

        }

        public static class Patch
        {
            public static string Roles(Guid userId) => $"{API_BASE_URL}/{userId}/roles";
            public static string Password(Guid userId) => $"{API_BASE_URL}/{userId}/password";
        }
    }
}

