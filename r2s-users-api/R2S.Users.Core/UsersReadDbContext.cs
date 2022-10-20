using Microsoft.EntityFrameworkCore;
using R2S.Users.Core.Read.ReadModels;

namespace R2S.Users.Core
{
    public class UsersReadDbContext : DbContext
    {
        public UsersReadDbContext(DbContextOptions<UsersReadDbContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        public IQueryable<UserReadModel> Users
        {
            get { return Set<UserReadModel>().AsNoTracking(); }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.HasDefaultSchema("users");

            builder.Entity<UserReadModel>().ToTable("AspNetUsers");
            builder.Entity<RoleReadModel>().ToTable("AspNetRoles");
            builder.Entity<UserReadModel>().HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<Dictionary<string, object>>("AspNetUserRoles",
                    j => j
                        .HasOne<RoleReadModel>()
                        .WithMany()
                        .HasForeignKey("RoleId"),
                    j => j
                        .HasOne<UserReadModel>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        );

            base.OnModelCreating(builder);
        }
    }
}
