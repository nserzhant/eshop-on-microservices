using Microsoft.Extensions.DependencyInjection;
using EShop.EmployeeManagement.Infrastructure.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using EShop.EmployeeManagement.Infrastructure.Entities;
using EShop.EmployeeManagement.Infrastructure.Enums;

namespace EShop.EmployeeManagement.Infrastructure.IntegrationTests;

public class BaseEmployeeIntegrationTests
{
    protected ServiceProvider serviceProvier;
    protected UserManager<Employee> userManager;

    [SetUp]
    public virtual async Task Setup()
    {
        //Setup services
        ServiceCollection sc = new ServiceCollection();

        sc.AddTestEmployeeServices();

        serviceProvier = sc.BuildServiceProvider();
        var dbContext = serviceProvier.GetRequiredService<EmployeeDbContext>();

        await dbContext.ClearDb("AspNetRoles");

        userManager = serviceProvier.GetRequiredService<UserManager<Employee>>();
    }

    [TearDown]
    public virtual async Task TearDownAsync()
    {
        userManager.Dispose();

        await serviceProvier.DisposeAsync();
    }

    protected async Task<Guid> createEmployee(string email, params Roles[] roles)
    {
        string password = "abcdef123Av#";

        var employee = new Employee
        {
            Email = email,
            UserName = email
        };

        await userManager.CreateAsync(employee, password);
        await userManager.AddToRolesAsync(employee, roles.Select(r=>r.ToString()));

        return employee.Id;
    }
}
