using EShop.EmployeeManagement.Api.IntegrationTests.Infrastructure;
using EShop.EmployeeManagement.Infrastructure.IntegrationTests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text;

namespace EShop.EmployeeManagement.Api.IntegrationTests;

public class BaseControllerTests : BaseEmployeeIntegrationTests
{
    protected WebApplicationFactory<Program> _webApplicationFactory;
    protected TestAuthenticationContextBuilder _testAuthenticationContextBuilder;
    protected Guid defaultemployeeId;
    protected string defaultEmail = "test@user.com";

    [SetUp]
    public override async Task Setup()
    {
        await base.Setup();

        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.tests.json");
        _testAuthenticationContextBuilder = new TestAuthenticationContextBuilder();
        _webApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.AddJsonFile(configPath);
                conf.AddEnvironmentVariables();
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

        defaultemployeeId = await createEmployee(defaultEmail);
    }

    [TearDown]
    public override async Task TearDownAsync()
    {
        _webApplicationFactory.Dispose();

        await base.TearDownAsync();
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
