using EShop.EmployeeManagement.Infrastructure;
using EShop.EmployeeManagement.Infrastructure.Entities;
using EShop.EmployeeManagement.Infrastructure.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EShop.EmployeeManagement.Api.Data;

public class DbInitializer
{
    const string ADMIN_USER_NAME = "admin@example.com";
    const string ADMIN_PASSWORD = "9>wmDeQLqaHdSK#";

    const string SALES_MANAGER_USER_NAME = "salesmanager@example.com";
    const string SALES_MANAGER_PASSWORD = "8^54khgfkDbm52*";

    const string NON_ASSIGNED_USER_NAME = "nonassigned@example.com";
    const string NON_ASSIGNED_USER_PASSWORD = "84^tDmviioD83@7";

    public static async Task InitializeDbWIthTestData(UserManager<Employee> userManager, EmployeeDbContext employeeDbContext)
    {
        await employeeDbContext.Database.MigrateAsync();

        if (!employeeDbContext.Users.Any())
        {
            await userManager.CreateAsync(new Employee()
            {
                Email = ADMIN_USER_NAME,
                UserName = ADMIN_USER_NAME
            }, ADMIN_PASSWORD);

            var admin = await userManager.FindByNameAsync(ADMIN_USER_NAME);

            await userManager.AddToRoleAsync(admin!, Roles.Administrator.ToString());

            await userManager.CreateAsync(new Employee()
            {
                Email = SALES_MANAGER_USER_NAME,
                UserName = SALES_MANAGER_USER_NAME
            }, SALES_MANAGER_PASSWORD);

            var salesManager = await userManager.FindByNameAsync(SALES_MANAGER_USER_NAME);

            await userManager.AddToRoleAsync(salesManager!, Roles.SalesManager.ToString());

            await userManager.CreateAsync(new Employee()
            {
                Email = NON_ASSIGNED_USER_NAME,
                UserName = NON_ASSIGNED_USER_NAME
            }, NON_ASSIGNED_USER_PASSWORD);

            await employeeDbContext.SaveChangesAsync();
        }
    }
}
