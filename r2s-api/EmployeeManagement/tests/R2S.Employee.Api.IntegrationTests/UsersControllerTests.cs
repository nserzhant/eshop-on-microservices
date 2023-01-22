using Microsoft.Net.Http.Headers;
using R2S.EmployeeManagement.Core.Enums;
using R2S.EmployeeManagement.Core.Read.Queries;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace R2S.EmployeeManagement.Api.IntegrationTests
{
    public class UsersControllerTests : BaseControllerTests
    {
        private const string API_BASE_URL = "api/Users";

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
        [Category("Get User")]
        public async Task When_User_Is_Unauthenitcated_Then_Get_User_By_Id_Returns_Unathorized()
        {
            _testAuthenticationContextBuilder.SetUnauthenticated();
            var usersClient = _webApplicationFactory.CreateClient();

            var response = await usersClient.GetAsync(Get.User(defaultUserId));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        [Category("Get User")]
        public async Task When_User_Is_Sales_Manager_Then_It_Cant_Get_User_By_Id()
        {
            _testAuthenticationContextBuilder.WithRole(Roles.SalesManager);
            var usersClient = _webApplicationFactory.CreateClient();

            var response = await usersClient.GetAsync(Get.User(defaultUserId));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("Get User")]
        public async Task When_User_Is_Administrator_Then_It_Can_Get_User_By_Id()
        {
            _testAuthenticationContextBuilder.WithRole(Roles.Administrator);
            var usersClient = _webApplicationFactory.CreateClient();
            
            var response = await usersClient.GetAsync(Get.User(defaultUserId));

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
            var listUserQuery = new ListEmployeeQuery()
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
            var listUserQuery = new ListEmployeeQuery()
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
            var listUserQuery = new ListEmployeeQuery()
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

            var response = await usersClient.PatchAsync(Patch.Roles(defaultUserId), content);

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

            var response = await usersClient.PatchAsync(Patch.Roles(defaultUserId), content);

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

            var response = await usersClient.PatchAsync(Patch.Roles(defaultUserId), content);

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

            var response = await usersClient.PatchAsync(Patch.Password(defaultUserId), content);

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

            var response = await usersClient.PatchAsync(Patch.Password(defaultUserId), content);

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

            var response = await usersClient.PatchAsync(Patch.Password(defaultUserId), content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        public static class Get
        {
            public static string User(Guid userId) => $"{API_BASE_URL}/{userId}";

            public static string Users(ListEmployeeQuery listUserQuery) => $"{API_BASE_URL}/list?{ConvertToQueryParams(listUserQuery)}";

        }

        public static class Patch
        {
            public static string Roles(Guid userId) => $"{API_BASE_URL}/{userId}/roles";
            public static string Password(Guid userId) => $"{API_BASE_URL}/{userId}/password";
        }
    }
}

