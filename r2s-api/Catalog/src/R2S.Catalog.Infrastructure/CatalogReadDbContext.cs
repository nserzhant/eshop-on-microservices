using Microsoft.EntityFrameworkCore;
using R2S.Catalog.Core.Models;
using R2S.Catalog.Infrastructure.Read.ReadModels;

namespace R2S.Catalog.Infrastructure;

public class CatalogReadDbContext : DbContext
{
    public CatalogReadDbContext(DbContextOptions<CatalogReadDbContext> dbContextOptions)
        : base(dbContextOptions)
    {

    }

    public IQueryable<CatalogBrandReadModel> CatalogBrands => Set<CatalogBrandReadModel>().AsNoTracking();
    public IQueryable<CatalogTypeReadModel> CatalogTypes => Set<CatalogTypeReadModel>().AsNoTracking();
    public IQueryable<CatalogItemReadModel> CatalogItems => Set<CatalogItemReadModel>().AsNoTracking()
            .Include(ci => ci.CatalogBrand)
            .Include(ci => ci.CatalogType);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DbConsts.CATALOG_DB_SCHEMA_NAME);
        modelBuilder.Entity<CatalogBrandReadModel>().ToTable($"{nameof(CatalogBrand)}s");
        modelBuilder.Entity<CatalogTypeReadModel>().ToTable($"{nameof(CatalogType)}s");

        modelBuilder.Entity<CatalogItemReadModel>().ToTable($"{nameof(CatalogItem)}s");
        modelBuilder.Entity<CatalogItemReadModel>().HasOne(itm => itm.CatalogType);
        modelBuilder.Entity<CatalogItemReadModel>().HasOne(itm => itm.CatalogBrand);

        base.OnModelCreating(modelBuilder);
    }
}
