using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using R2S.Catalog.Core.Models;

namespace R2S.Catalog.Infrastructure.EntityConfigurations;

public class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        // Primary key
        builder.HasKey(ci => ci.Id);

        // Columns
        builder.Property(ci => ci.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(ci => ci.Description)
            .HasMaxLength(200);
        builder.Property(ci => ci.Price)
            .HasPrecision(10, 2);
        builder.Property(ci => ci.PictureUri)
            .HasMaxLength(300);
        builder.Property(ci => ci.CatalogBrandId)
            .IsRequired();
        builder.Property(ci => ci.CatalogTypeId)
            .IsRequired();

        // Foreign keys
        builder.HasOne<CatalogBrand>()
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CatalogType>()
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(ci => new { ci.Name, ci.CatalogBrandId, ci.CatalogTypeId }).IsUnique(true);

        // Timestamp column
        builder.Property(ci => ci.Ts)
            .IsRowVersion();
    }
}
