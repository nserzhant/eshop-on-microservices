using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R2S.Users.Core.Entities;
using R2S.Users.Core.Read;
using R2S.Users.Core.Services;

namespace R2S.Users.Core
{
    public static class UsersServicesRegistrationExtension
    {
        public static IServiceCollection AddUsersServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserQueryService, UserQueryService>();
            services.AddDbContext<UsersDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("usersDbConnectionString")
                    , x => x.MigrationsHistoryTable("_EFMigrationsHistory", "users"));
            });
            services.AddDbContext<UsersReadDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("usersDbConnectionString"));
            });

            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
