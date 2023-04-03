using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using EShop.EmployeeManagement.Core.Entities;
using EShop.EmployeeManagement.Core.Enums;

namespace EShop.EmployeeManagement.Core;

public class EmployeeDbContext : IdentityDbContext<Employee, IdentityRole<Guid>, Guid>
{
    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("employee");

        base.OnModelCreating(builder);
    }
}

public class EmployeeDBContextFactory : IDesignTimeDbContextFactory<EmployeeDbContext>
{
    public EmployeeDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var connectionString = configuration.GetConnectionString("employeeDbConnectionString");
        var optionsBuilder = new DbContextOptionsBuilder<EmployeeDbContext>();

        optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsHistoryTable("_EFMigrationsHistory", "employee"));

        return new EmployeeDbContext(optionsBuilder.Options);
    }
}

public class EmployeeDbContextInitializer
{
    const string DEFAULT_ADMINISTRATOR_EMAIL = "administrator@yourdomain.com";
    private UserManager<Employee> _userManager;

    public EmployeeDbContextInitializer(UserManager<Employee> userManager)
    {
        _userManager = userManager;
    }

    public async Task InitializeDB(string administratorPassword)
    {
        var employeeDBContextFactory = new EmployeeDBContextFactory();
        var dbContext = employeeDBContextFactory.CreateDbContext(null);

        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();

        await createDefaultAdministrator(administratorPassword);
    }

    private async Task createDefaultAdministrator(string administratorPassword)
    {
        var administrator = new Employee
        {
            Email = DEFAULT_ADMINISTRATOR_EMAIL,
            UserName = DEFAULT_ADMINISTRATOR_EMAIL
        };

        var result = await _userManager.CreateAsync(administrator, administratorPassword);

        if (!result.Succeeded)
        {
            var errors = getErrorsMessage(result);
            throw new Exception($"Failed to create default admin account: {errors}");
        }

        result = await _userManager.AddToRolesAsync(administrator, new[] { Roles.Administrator.ToString() });

        if (!result.Succeeded)
        {
            var errors = getErrorsMessage(result);

            throw new Exception($"Failed to add admin role: {errors}");

        }
    }

    private static string getErrorsMessage(IdentityResult result)
    {
        return result.Errors.Select(err => err.Description)
            .Aggregate((a, b) => a + ", " + b);
    }
}