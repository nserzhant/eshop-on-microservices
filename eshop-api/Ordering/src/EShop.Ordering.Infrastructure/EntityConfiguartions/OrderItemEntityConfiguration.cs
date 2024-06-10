using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using EShop.Ordering.Core.Models;

namespace EShop.Ordering.Infrastructure.EntityConfiguartions;
public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // Columns
        builder.Property(oi => oi.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(oi => oi.Description)
            .HasMaxLength(200);
        builder.Property(oi => oi.Price)
            .HasPrecision(10, 2);
        builder.Property(oi => oi.PictureUri)
            .HasMaxLength(300);
        builder.Property(oi => oi.BrandName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(oi => oi.Qty)
            .IsRequired();
        builder.Property(oi => oi.TypeName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(oi => oi.CatalogItemId)
            .IsRequired();

        // Timestamp column
        builder.Property(ci => ci.Ts)
            .IsRowVersion();
    }
}