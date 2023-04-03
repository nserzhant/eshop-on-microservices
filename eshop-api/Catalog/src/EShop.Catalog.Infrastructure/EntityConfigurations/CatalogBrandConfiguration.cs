using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShop.Catalog.Core.Models;

namespace EShop.Catalog.Infrastructure.EntityConfigurations;

public class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>
{
    public void Configure(EntityTypeBuilder<CatalogBrand> builder)
    {
        builder.HasKey(cb => cb.Id);
        builder.Property(cb => cb.Brand)
            .IsRequired()            
            .HasMaxLength(100);

        builder.HasIndex(cb => cb.Brand).IsUnique(true);

        builder.Property(cb => cb.Ts)
            .IsRowVersion();
    }
}
