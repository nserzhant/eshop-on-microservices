using Microsoft.EntityFrameworkCore;
using EShop.EmployeeManagement.Infrastructure.Read.ReadModels;

namespace EShop.EmployeeManagement.Infrastructure;

public class EmployeeReadDbContext : DbContext
{
    public EmployeeReadDbContext(DbContextOptions<EmployeeReadDbContext> dbContextOptions)
        : base(dbContextOptions)
    {

    }

    public IQueryable<EmployeeReadModel> Users
    {
        get { return Set<EmployeeReadModel>().AsNoTracking(); }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        builder.HasDefaultSchema(DbConsts.EMPLOYEE_DB_SCHEMA_NAME);

        builder.Entity<EmployeeReadModel>().ToTable("AspNetUsers");
        builder.Entity<RoleReadModel>().ToTable("AspNetRoles");
        builder.Entity<EmployeeReadModel>().HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<Dictionary<string, object>>("AspNetUserRoles",
                j => j
                    .HasOne<RoleReadModel>()
                    .WithMany()
                    .HasForeignKey("RoleId"),
                j => j
                    .HasOne<EmployeeReadModel>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    );

        base.OnModelCreating(builder);
    }
}
