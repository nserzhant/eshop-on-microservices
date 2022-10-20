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
    public class EmployeeAccountControllerTests : BaseUsersIntegrationTests
    {
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

        private async Task<Guid> createDefaultUserAsync(string userEmail)
        {
            var password = "3242f$fDc%dD";
            await _userService.Register(userEmail, password);
            var user = await _userQueryService.GetByEmail(userEmail);

            return user.Id;
        }
    }
}
