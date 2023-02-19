using Microsoft.Extensions.DependencyInjection;
using R2S.Catalog.Infrastructure.IntegrationTests.Extensions;

namespace R2S.Catalog.Infrastructure.IntegrationTests;

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
