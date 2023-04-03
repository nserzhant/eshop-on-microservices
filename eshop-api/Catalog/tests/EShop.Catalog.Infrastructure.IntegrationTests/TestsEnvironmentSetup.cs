using Microsoft.Extensions.DependencyInjection;
using EShop.Catalog.Infrastructure.IntegrationTests.Extensions;

namespace EShop.Catalog.Infrastructure.IntegrationTests;

[SetUpFixture]
public class TestsEnvironmentSetup
{
    [OneTimeSetUp]
    public void RunBeforeTestsExecution()
    {
        var sc = new ServiceCollection();

        sc.AddTestCatalogServices();

        using var services = sc.BuildServiceProvider();
        var db = services.GetRequiredService<CatalogDbContext>();

        //Recreate Db
        db.RecreateDb();
    }
}
