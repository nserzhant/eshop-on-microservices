using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using R2S.Users.Core.Entities;
using R2S.Users.Core.Enums;
using System.Linq;

namespace R2S.Users.Core
{
    public class UsersDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("users");

            base.OnModelCreating(builder);
        }
    }

    public class UsersDBContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
    {
        public UsersDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var connectionString = configuration.GetConnectionString("usersDbConnectionString");
            var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();

            optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsHistoryTable("_EFMigrationsHistory", "users"));

            return new UsersDbContext(optionsBuilder.Options);
        }
    }

    public class UsersDbContextInitializer
    {
        const string DEFAULT_ADMINISTRATOR_EMAIL = "administrator@yourdomain.com";
        private UserManager<User> _userManager;

        public UsersDbContextInitializer(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task InitializeDB(string adminUserPassword)
        {
            var usersDbContextFactory = new UsersDBContextFactory();
            var dbContext = usersDbContextFactory.CreateDbContext(null);

            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();

            await createDefaultAdminUser(adminUserPassword);
        }

        private async Task createDefaultAdminUser(string adminUserPassword)
        {

            var user = new User
            {
                Email = DEFAULT_ADMINISTRATOR_EMAIL,
                UserName = DEFAULT_ADMINISTRATOR_EMAIL
            };

            var result = await _userManager.CreateAsync(user, adminUserPassword);

            if (!result.Succeeded)
            {
                var errors = getErrorsMessage(result);
                throw new Exception($"Failed to create default admin account: {errors}");
            }

            result = await _userManager.AddToRolesAsync(user, new[] { Roles.Administrator.ToString() });

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
}