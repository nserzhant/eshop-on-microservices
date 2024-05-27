using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EShop.EmployeeManagement.Api.IntegrationTests.Infrastructure;
using EShop.EmployeeManagement.Core.IntegrationTests;
using EShop.EmployeeManagement.Core.Read;
using EShop.EmployeeManagement.Core.Services;
using System.Reflection;
using System.Text;

namespace EShop.EmployeeManagement.Api.IntegrationTests;

public class BaseControllerTests : BaseEmployeeIntegrationTests
{
    protected WebApplicationFactory<Program> _webApplicationFactory;
    protected TestAuthenticationContextBuilder _testAuthenticationContextBuilder;
    protected Guid defaultemployeeId;
    protected string defaultEmail = "test@user.com";
    protected string defaultPassword = "3242f$fDc%dD";

    private IEmployeeService _employeeService;
    private IEmployeeQueryService _employeeQueryService;

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


        _employeeService = serviceProvier.GetRequiredService<IEmployeeService>();
        _employeeQueryService = serviceProvier.GetRequiredService<IEmployeeQueryService>();

        defaultemployeeId = await registerEmployeeAsync(defaultEmail, defaultPassword);
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();

        _webApplicationFactory.Dispose();
    }

    private async Task<Guid> registerEmployeeAsync(string email, string password)
    {
        await _employeeService.Register(email, password);
        var employee = await _employeeQueryService.GetByEmail(email);

        return employee.Id;
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
