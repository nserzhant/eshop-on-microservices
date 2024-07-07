using EShop.Saga.Components.StateMachines;
using MassTransit;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MassTransit.EntityFrameworkCoreIntegration;

namespace EShop.Saga.Components.Infrastructure;

public class OrderingStateMap : SagaClassMap<OrderingStateMachineInstance>
{
    protected override void Configure(EntityTypeBuilder<OrderingStateMachineInstance> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.BasketId);
        entity.Property(x => x.CustomerId);
        entity.Property(x => x.CustomerEmail).HasMaxLength(100);
        entity.Property(x => x.ShippingAddress).HasMaxLength(255);
        entity.Property(x => x.Items).HasJsonConversion();
    }
}
