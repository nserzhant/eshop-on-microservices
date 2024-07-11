using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EShop.EmployeeManagement.Infrastructure.Entities;
using EShop.EmployeeManagement.Infrastructure.Read;

namespace EShop.EmployeeManagement.Infrastructure;

public static class EmployeeServicesRegistrationExtension
{
    public static IServiceCollection AddEmployeeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEmployeeQueryService, EmployeeQueryService>();
        services.AddDbContext<EmployeeDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString(DbConsts.EMPLOYEE_DB_CONNECTION_STRING_NAME)
                , x => x.MigrationsHistoryTable("_EFMigrationsHistory", DbConsts.EMPLOYEE_DB_SCHEMA_NAME));
        });
        services.AddDbContext<EmployeeReadDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString(DbConsts.EMPLOYEE_DB_CONNECTION_STRING_NAME));
        });

        services.AddIdentity<Employee, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = 5;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireLowercase = true;
            options.User.RequireUniqueEmail = false;
        })
        .AddEntityFrameworkStores<EmployeeDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}
