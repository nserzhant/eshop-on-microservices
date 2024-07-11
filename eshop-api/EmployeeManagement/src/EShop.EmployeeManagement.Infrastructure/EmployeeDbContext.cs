using EShop.EmployeeManagement.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EShop.EmployeeManagement.Infrastructure;

public class EmployeeDbContext : IdentityDbContext<Employee, IdentityRole<Guid>, Guid>
{
    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(DbConsts.EMPLOYEE_DB_SCHEMA_NAME);

        base.OnModelCreating(builder);
    }
}

public class EmployeeDBContextFactory : IDesignTimeDbContextFactory<EmployeeDbContext>
{
    public EmployeeDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.migrations.json").Build();
        var connectionString = configuration.GetConnectionString(DbConsts.EMPLOYEE_DB_CONNECTION_STRING_NAME);
        var optionsBuilder = new DbContextOptionsBuilder<EmployeeDbContext>();

        optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsHistoryTable("_EFMigrationsHistory", DbConsts.EMPLOYEE_DB_SCHEMA_NAME));

        return new EmployeeDbContext(optionsBuilder.Options);
    }
}