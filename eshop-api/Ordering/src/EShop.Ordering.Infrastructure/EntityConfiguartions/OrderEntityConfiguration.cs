using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using EShop.Ordering.Core.Models;

namespace EShop.Ordering.Infrastructure.EntityConfiguartions;
public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Metadata.FindNavigation(nameof(Order.OrderItems))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
         
        builder.Property(o => o.ShippingAddress)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(o => o.CustomerEmail)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cb => cb.Ts)
            .IsRowVersion();
    }
}