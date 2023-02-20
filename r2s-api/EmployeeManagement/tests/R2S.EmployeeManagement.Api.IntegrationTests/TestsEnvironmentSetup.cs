using Microsoft.Extensions.DependencyInjection;
using R2S.EmployeeManagement.Core;
using R2S.EmployeeManagement.Core.IntegrationTests;
using R2S.EmployeeManagement.Core.IntegrationTests.Infrastructure;

namespace R2S.EmployeeManagement.Api.IntegrationTests;

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
