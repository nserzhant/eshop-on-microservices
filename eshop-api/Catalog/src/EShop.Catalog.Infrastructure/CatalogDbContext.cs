using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using EShop.Catalog.Core.Models;
using System.Reflection;

namespace EShop.Catalog.Infrastructure;

public class CatalogDbContext : DbContext
{
    public DbSet<CatalogBrand> CatalogBrands { get; set; }
    public DbSet<CatalogType> CatalogTypes { get; set; }
    public DbSet<CatalogItem> CatalogItems { get; set; }

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(DbConsts.CATALOG_DB_SCHEMA_NAME);

        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

public class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var connectionString = configuration.GetConnectionString(DbConsts.CATALOG_DB_CONNECTION_STRING_NAME);
        var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();

        optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsHistoryTable("_EFMigrationsHistory", DbConsts.CATALOG_DB_SCHEMA_NAME));

        return new CatalogDbContext(optionsBuilder.Options);
    }
}
