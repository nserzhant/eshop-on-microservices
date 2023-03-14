using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R2S.Catalog.Api.IntegrationTests.Infrastructure;
using R2S.Catalog.Infrastructure.IntegrationTests;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace R2S.Catalog.Api.IntegrationTests;

public class BaseCatalogControllerTests : BaseCatalogIntegrationTests
{
    protected WebApplicationFactory<Program> webApplicationFactory;
    protected TestAuthenticationContextBuilder testAuthenticationContextBuilder;

    [SetUp]
    public async Task SetupAsync()
    {
        await base.SetupAsync();

        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.json");
        testAuthenticationContextBuilder = new TestAuthenticationContextBuilder();
        webApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.AddJsonFile(configPath);
            });
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(testAuthenticationContextBuilder);
                services.AddTransient<IAuthenticationSchemeProvider, TestAuthenticationSchemeProvider>();
            });
        });

    }


    protected async Task<T?> fromHttpResponseMessage<T>(HttpResponseMessage? response)
    {
        if (response == null)
        {
            return default;
        }

        var stringContent = await response.Content.ReadAsStringAsync();
        var jsonSerializationOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IgnoreReadOnlyProperties = false, IncludeFields = true };

        try
        {
            var result = JsonSerializer.Deserialize<T>(stringContent, jsonSerializationOptions);

            return result;
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(stringContent))
                throw new Exception(stringContent);

            throw ex;
        }
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
