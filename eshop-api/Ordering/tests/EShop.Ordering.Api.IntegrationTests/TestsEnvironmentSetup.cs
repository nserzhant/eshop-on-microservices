using EShop.Ordering.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using EShop.Ordering.Infrastructure.IntegrationTests.Extensions;

namespace EShop.Ordering.Api.IntegrationTests;

[SetUpFixture]
public class TestsEnvironmentSetup
{
    [OneTimeSetUp]
    public void RunBeforeTestsExecution()
    {
        var sc = new ServiceCollection();

        sc.AddTestOrderingServices();

        using var services = sc.BuildServiceProvider();
        var db = services.GetRequiredService<OrderingDbContext>();

        //Recreate Db
        db.RecreateDb();
    }
}