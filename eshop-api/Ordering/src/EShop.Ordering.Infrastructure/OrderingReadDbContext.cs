using EShop.Ordering.Core.Models;
using EShop.Ordering.Infrastructure.Read.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace EShop.Ordering.Infrastructure;
public class OrderingReadDbContext : DbContext
{
    public OrderingReadDbContext(DbContextOptions<OrderingReadDbContext> dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public IQueryable<OrderReadModel> Orders => Set<OrderReadModel>().AsNoTracking()
        .Include(o => o.OrderItems);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DbConsts.ORDERING_DB_SCHEMA_NAME);
        modelBuilder.Entity<OrderReadModel>().ToTable($"{nameof(Order)}s");
        modelBuilder.Entity<OrderItemReadModel>()
            .ToTable($"{nameof(OrderItem)}s")
            .HasOne<OrderReadModel>()
            .WithMany(nameof(OrderReadModel.OrderItems))
            .HasForeignKey("OrderId");

        base.OnModelCreating(modelBuilder);
    }
}