using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Infrastructure.EntityConfigurations;

internal class CatalogTypeConfiguration : IEntityTypeConfiguration<CatalogType>
{
    public void Configure(EntityTypeBuilder<CatalogType> builder)
    {
        builder.HasKey(ct => ct.Id);
        builder.Property(ct => ct.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ct => ct.Ts)
            .IsRowVersion();
    }
}
