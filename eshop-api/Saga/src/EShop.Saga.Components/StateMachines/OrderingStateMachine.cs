using EShop.Basket.Integration.Commands;
using EShop.Basket.Integration.Events;
using EShop.Catalog.Integration.Commands;
using EShop.Catalog.Integration.Events;
using EShop.Ordering.Integration.Commands;
using EShop.Ordering.Integration.Events;
using EShop.Payment.Integration.Commands;
using EShop.Payment.Integration.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EShop.Saga.Components.StateMachines;

public class OrderingStateMachine : MassTransitStateMachine<OrderingStateMachineInstance>
{
    private readonly IOptions<OrderingStateMachineSettings> _orderingStateMachineSettings;
    private readonly ILogger<OrderingStateMachine> _logger;

    public State ReservingStocks { get; }
    public State StocksReserved { get; }
    public State OrderCreated { get; }
    public State PaymentProcessed { get; }
    public State PaymentFailed { get; }
    public State CheckoutFailed { get; }

    public Event<BasketCheckedOutEvent> BasketCheckedOutEvent { get; private set; }
    public Event<StocksReservedEvent> StocksReservedEvent { get; private set; }
    public Event<StocksReservationFailedEvent> StocksReservationFailedEvent { get; private set; }
    public Event<OrderCreatedEvent> OrderCreatedEvent { get; private set; }
    public Event<PaymentProcessedEvent> PaymentProcessedEvent { get; private set; }
    public Event<PaymentFailedEvent> PaymentFailedEvent { get; private set; }
    public Event<StocksReleasedEvent> StocksReleasedEvent { get; private set; }
    public Event<BasketClearedEvent> BasketClearedEvent { get; private set; }

    public OrderingStateMachine(IOptions<OrderingStateMachineSettings> orderingStateMachineSettings, ILogger<OrderingStateMachine> logger)
    {
        _orderingStateMachineSettings = orderingStateMachineSettings;
        _logger = logger;

        InstanceState(x => x.CurrentState);

        Event(() => this.BasketCheckedOutEvent, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => this.StocksReservedEvent, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => this.StocksReservationFailedEvent, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => this.OrderCreatedEvent, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => this.PaymentProcessedEvent, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => this.PaymentFailedEvent, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => this.StocksReleasedEvent, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => this.BasketClearedEvent, x => x.CorrelateById(c => c.Message.CorrelationId));

        Initially(
            When(BasketCheckedOutEvent)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                    context.Saga.CustomerId = context.Message.CustomerId;
                    context.Saga.CustomerEmail = context.Message.CustomerEmail;
                    context.Saga.ShippingAddress = context.Message.ShippingAddress;
                    context.Saga.Items = context.Message.Items;
                    context.Saga.BasketId = context.Message.BasketId;

                })
                .Then(context => _logger.LogInformation($"Processing BasketCheckedOutEvent, CorrelationId: {context.Message.CorrelationId}"))
                .Send(new Uri($"queue:{_orderingStateMachineSettings.Value.ReserveStocksQueueName}"), context =>
                    new ReserveStocksCommand
                    (
                        context.Message.CorrelationId,
                        context.Message.Items.Select(itm => new ReserveStockItem(itm.CatalogItemId, itm.Qty)).ToList()
                    ))
                .TransitionTo(ReservingStocks));

        During(ReservingStocks,
            When(StocksReservedEvent)
                .Then(context => _logger.LogInformation($"Processing StocksReservedEvent, CorrelationId: {context.Message.CorrelationId}"))
                .Send(new Uri($"queue:{_orderingStateMachineSettings.Value.ProcessPaymentQueueName}"), context =>
                    new ProcessPaymentCommand
                    (
                        context.Message.CorrelationId,
                        context.Saga.CustomerId,
                        context.Message.TotalPrice
                    ))
                .TransitionTo(StocksReserved),
            When(StocksReservationFailedEvent)
                .Then(context => _logger.LogInformation($"Processing StocksReservationFailedEvent, CorrelationId: {context.Message.CorrelationId}"))
                .TransitionTo(CheckoutFailed));

        During(StocksReserved,
            When(PaymentProcessedEvent)
                .Then(context => _logger.LogInformation($"Processing PaymentProcessedEvent, CorrelationId: {context.Message.CorrelationId}"))
                .Send(new Uri($"queue:{_orderingStateMachineSettings.Value.CreateOrderQueueName}"), context =>
                    new CreateOrderCommand
                    (
                        context.Message.CorrelationId,
                        context.Saga.CustomerId,
                        context.Saga.CustomerEmail,
                        context.Saga.ShippingAddress,
                        context.Saga.Items.Select(itm => new CreateOrdeItem
                        (
                            itm.CatalogItemId, 
                            itm.ItemName, 
                            itm.Qty,
                            itm.Description,
                            itm.Price,
                            itm.TypeName,
                            itm.BrandName,
                            itm.PictureUri
                        )).ToList()
                    ))
                .TransitionTo(PaymentProcessed),
            When(PaymentFailedEvent)
                .Then(context => _logger.LogInformation($"Processing PaymentFailedEvent, CorrelationId: {context.Message.CorrelationId}"))
                .Send(new Uri($"queue:{_orderingStateMachineSettings.Value.ReleaseStockQueueName}"), context =>
                    new ReleaseStocksCommand
                    (
                        context.Message.CorrelationId,
                        context.Saga.Items.Select(itm => new ReleaseStockItem(itm.CatalogItemId, itm.Qty)).ToList()
                    ))
                .TransitionTo(PaymentFailed));

        During(PaymentProcessed,
            When(OrderCreatedEvent)
                .Then(context => _logger.LogInformation($"Processing OrderCreatedEvent, CorrelationId: {context.Message.CorrelationId}"))
                .Send(new Uri($"queue:{_orderingStateMachineSettings.Value.ClearBasketQueueName}"), context =>
                    new ClearBasketCommand
                    (
                        context.Message.CorrelationId,
                        context.Saga.CustomerId,
                        context.Saga.BasketId
                    ))
                .TransitionTo(OrderCreated));

        During(OrderCreated,
            When(BasketClearedEvent)
                .Then(context => _logger.LogInformation($"Processing BasketClearedEvent, Finalizing, CorrelationId: {context.Message.CorrelationId}"))
                .Finalize());

        During(PaymentFailed,
            When(StocksReleasedEvent)
                .Then(context => _logger.LogInformation($"Processing StocksReleasedEvent, CorrelationId: {context.Message.CorrelationId}"))
                .TransitionTo(CheckoutFailed));
    }
}