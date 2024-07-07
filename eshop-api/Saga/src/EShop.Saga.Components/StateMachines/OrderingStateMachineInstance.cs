using EShop.Basket.Integration.Events;
using MassTransit;

namespace EShop.Saga.Components.StateMachines;

public class OrderingStateMachineInstance : SagaStateMachineInstance
{
    public string CurrentState { get; set; }
    public Guid CorrelationId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerEmail { get; set; }
    public string ShippingAddress { get; set; }
    public Guid BasketId { get; set; }
    public List<BasketCheckoutItem> Items { get; set; }
}

