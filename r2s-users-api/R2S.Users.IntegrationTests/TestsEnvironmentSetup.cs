using Microsoft.Extensions.DependencyInjection;
using R2S.Users.Core.IntegrationTests.Infrastructure;

namespace R2S.Users.Core.IntegrationTests
{
    [SetUpFixture]
    public class TestsEnvironmentSetup
    {
        [OneTimeSetUp]
        public void RunBeforeTestsExecution()
        {
            var sc = new ServiceCollection();
            sc.AddTestUsersServices();

            using var services = sc.BuildServiceProvider();
            var db = services.GetRequiredService<UsersDbContext>();

            //Recreate Db
            db.RecreateDb();
        }
    }
}
