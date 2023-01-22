using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using R2S.EmployeeManagement.Core.Entities;
using R2S.EmployeeManagement.Core.Enums;
using System.Linq;

namespace R2S.EmployeeManagement.Core
{
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

    public class UsersDBContextFactory : IDesignTimeDbContextFactory<EmployeeDbContext>
    {
        public EmployeeDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var connectionString = configuration.GetConnectionString("usersDbConnectionString");
            var optionsBuilder = new DbContextOptionsBuilder<EmployeeDbContext>();

            optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsHistoryTable("_EFMigrationsHistory", "employee"));

            return new EmployeeDbContext(optionsBuilder.Options);
        }
    }

    public class UsersDbContextInitializer
    {
        const string DEFAULT_ADMINISTRATOR_EMAIL = "administrator@yourdomain.com";
        private UserManager<Entities.Employee> _userManager;

        public UsersDbContextInitializer(UserManager<Entities.Employee> userManager)
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

            var user = new Entities.Employee
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