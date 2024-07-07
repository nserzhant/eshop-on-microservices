using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EShop.Saga.Components.Infrastructure;

public class EShopSagaDbContext : SagaDbContext
{
    public EShopSagaDbContext(DbContextOptions<EShopSagaDbContext> options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new OrderingStateMap(); }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DBConsts.SAGA_DB_SCHEMA_NAME);

        base.OnModelCreating(modelBuilder);
    }
}
public class EShopSagaDbContextFactory : IDesignTimeDbContextFactory<EShopSagaDbContext>
{
    public EShopSagaDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.migrations.json").Build();
        var connectionString = configuration.GetConnectionString(DBConsts.SAGA_DB_CONNECTION_STRING_NAME);
        var optionsBuilder = new DbContextOptionsBuilder<EShopSagaDbContext>();

        optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsHistoryTable("_EFMigrationsHistory", DBConsts.SAGA_DB_SCHEMA_NAME));

        return new EShopSagaDbContext(optionsBuilder.Options);
    }
}
