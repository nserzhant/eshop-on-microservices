using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R2S.Users.Api.IntegrationTests.Infrastructure;
using R2S.Users.Core.IntegrationTests;
using R2S.Users.Core.Read;
using R2S.Users.Core.Services;
using System.Reflection;
using System.Text;

namespace R2S.Users.Api.IntegrationTests
{
    public class BaseControllerTests : BaseUsersIntegrationTests
    {
        protected WebApplicationFactory<Program> _webApplicationFactory;
        protected TestAuthenticationContextBuilder _testAuthenticationContextBuilder;
        protected Guid defaultUserId;
        protected string defaultUserEmail = "test@user.com";
        protected string defaultUserPassword = "3242f$fDc%dD";

        private IUserService _userService;
        private IUserQueryService _userQueryService;

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

            defaultUserId = await createUserAsync(defaultUserEmail, defaultUserPassword);
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            _webApplicationFactory.Dispose();
        }

        private async Task<Guid> createUserAsync(string userEmail, string password)
        {
            await _userService.Register(userEmail, password);
            var user = await _userQueryService.GetByEmail(userEmail);

            return user.Id;
        }

        protected static string ConvertToQueryParams<T>(T obj) where T : class
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
    }
}
