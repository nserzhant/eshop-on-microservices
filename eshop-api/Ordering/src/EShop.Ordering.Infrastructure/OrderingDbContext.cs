using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using EShop.Ordering.Core.Models;
using EShop.Ordering.Infrastructure.ConsumeFilters;

namespace EShop.Ordering.Infrastructure;

public class OrderingDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ConsumedIntegrationCommand> ConsumedIntegrationCommands { get; set; }

    public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(DbConsts.ORDERING_DB_SCHEMA_NAME);

        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

public class OrderingDbContextFactory : IDesignTimeDbContextFactory<OrderingDbContext>
{
    public OrderingDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.migrations.json").Build();
        var connectionString = configuration.GetConnectionString(DbConsts.ORDERING_DB_CONNECTION_STRING_NAME);
        var optionsBuilder = new DbContextOptionsBuilder<OrderingDbContext>();

        optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsHistoryTable("_EFMigrationsHistory", DbConsts.ORDERING_DB_SCHEMA_NAME));

        return new OrderingDbContext(optionsBuilder.Options);
    }
}
