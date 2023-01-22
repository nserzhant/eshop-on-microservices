using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using R2S.EmployeeManagement.Core.Entities;
using R2S.EmployeeManagement.Core.IntegrationTests.Infrastructure;

namespace R2S.EmployeeManagement.Core.IntegrationTests
{
    public class BaseUsersIntegrationTests
    {
        protected ServiceProvider serviceProvier;


        [SetUp]
        public virtual async Task Setup()
        {
            //Setup services
            ServiceCollection sc = new ServiceCollection();

            sc.AddTestUsersServices();

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
