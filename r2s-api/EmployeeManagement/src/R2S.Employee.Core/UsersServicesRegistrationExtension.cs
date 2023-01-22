using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R2S.EmployeeManagement.Core.Entities;
using R2S.EmployeeManagement.Core.Read;
using R2S.EmployeeManagement.Core.Services;

namespace R2S.EmployeeManagement.Core
{
    public static class UsersServicesRegistrationExtension
    {
        public static IServiceCollection AddUsersServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IEmployeeQueryService, EmployeeQueryService>();
            services.AddDbContext<EmployeeDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("usersDbConnectionString")
                    , x => x.MigrationsHistoryTable("_EFMigrationsHistory", "users"));
            });
            services.AddDbContext<EmployeeReadDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("usersDbConnectionString"));
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
}
