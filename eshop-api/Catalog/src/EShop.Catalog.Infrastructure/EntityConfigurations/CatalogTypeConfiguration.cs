using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShop.Catalog.Core.Models;

namespace EShop.Catalog.Infrastructure.EntityConfigurations;

internal class CatalogTypeConfiguration : IEntityTypeConfiguration<CatalogType>
{
    public void Configure(EntityTypeBuilder<CatalogType> builder)
    {
        builder.HasKey(ct => ct.Id);
        builder.Property(ct => ct.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(ct => ct.Type).IsUnique(true);

        builder.Property(ct => ct.Ts)
            .IsRowVersion();
    }
}
