using EShop.Catalog.Infrastructure.ConsumeFilters;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EShop.Catalog.Infrastructure.EntityConfigurations;
public class ConsumedIntegrationCommandConfiguration : IEntityTypeConfiguration<ConsumedIntegrationCommand>
{
    public void Configure(EntityTypeBuilder<ConsumedIntegrationCommand> builder)
    {
        builder.HasKey(consumedCommand => consumedCommand.MessageId);
    }
}