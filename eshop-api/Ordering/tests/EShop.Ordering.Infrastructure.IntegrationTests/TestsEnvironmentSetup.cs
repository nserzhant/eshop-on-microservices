using Microsoft.Extensions.DependencyInjection;

namespace EShop.Ordering.Infrastructure.IntegrationTests;

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
