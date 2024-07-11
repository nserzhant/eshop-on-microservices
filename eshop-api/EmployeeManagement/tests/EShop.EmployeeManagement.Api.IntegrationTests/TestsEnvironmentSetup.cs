using Microsoft.Extensions.DependencyInjection;
using EShop.EmployeeManagement.Infrastructure;
using EShop.EmployeeManagement.Infrastructure.IntegrationTests;
using EShop.EmployeeManagement.Infrastructure.IntegrationTests.Infrastructure;

namespace EShop.EmployeeManagement.Api.IntegrationTests;

[SetUpFixture]
public class TestsEnvironmentSetup
{
    [OneTimeSetUp]
    public void RunBeforeTestsExecution()
    {
        var sc = new ServiceCollection();
        sc.AddTestEmployeeServices();

        using var services = sc.BuildServiceProvider();
        var db = services.GetRequiredService<EmployeeDbContext>();

        //Recreate Db
        db.RecreateDb();
    }
}
