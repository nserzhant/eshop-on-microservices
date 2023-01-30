using Microsoft.Extensions.DependencyInjection;
using R2S.EmployeeManagement.Core.IntegrationTests.Infrastructure;

namespace R2S.EmployeeManagement.Core.IntegrationTests
{
    public class BaseEmployeeIntegrationTests
    {
        protected ServiceProvider serviceProvier;

        [SetUp]
        public virtual async Task Setup()
        {
            //Setup services
            ServiceCollection sc = new ServiceCollection();

            sc.AddTestEmployeeServices();

            serviceProvier = sc.BuildServiceProvider();
            var dbContext = serviceProvier.GetRequiredService<EmployeeDbContext>();

            await dbContext.ClearDb("AspNetRoles");
        }

        [TearDown]
        public virtual void TearDown()
        {
            serviceProvier.Dispose();
        }
    }
}
