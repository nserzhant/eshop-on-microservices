using Microsoft.Extensions.DependencyInjection;
using R2S.Catalog.Infrastructure;
using R2S.Catalog.Infrastructure.IntegrationTests.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R2S.Catalog.Api.IntegrationTests;

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
