using EShop.Basket.Integration.Events;
using EShop.Catalog.Integration.Events;
using EShop.Ordering.Integration.Events;
using EShop.Payment.Integration.Events;
using EShop.Saga.Components.StateMachines;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EShop.Saga.Components.Tests;

[TestFixture]
[Category("OrderingStateMachine")]
public class OrderingStateMachineTests
{
    ServiceProvider _serviceProvider;
    InMemoryTestHarness _harness;
    ISagaStateMachineTestHarness<OrderingStateMachine, OrderingStateMachineInstance> _sagaHarness;

    [SetUp]
    public async Task SetUpAsync()
    {
        _serviceProvider = new ServiceCollection()
            .AddLogging(log => log.AddConsole())
            .AddTransient<OrderingStateMachine>()
            .BuildServiceProvider(true);

        var orderingStateMachine = _serviceProvider.GetRequiredService<OrderingStateMachine>();

        _harness = new InMemoryTestHarness();
        _sagaHarness = _harness.StateMachineSaga<OrderingStateMachineInstance, OrderingStateMachine>(orderingStateMachine);

        await _harness.Start();
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        _harness.Dispose();
        await _harness.Stop();
        await _serviceProvider.DisposeAsync();
    }

    [Test]
    public async Task When_Basket_Check_Out_Then_State_Should_Be_Reserving_Stocks()
    {
        var correlationId = Guid.NewGuid();
        await _harness.Bus.Publish(new BasketCheckedOutEvent(correlationId, Guid.NewGuid(), string.Empty, string.Empty, Guid.NewGuid(), new List<BasketCheckoutItem>()));

        var orderCreatingSagaId = await _sagaHarness.Exists(correlationId, x => x.ReservingStocks, TimeSpan.FromSeconds(10));

        Assert.That(orderCreatingSagaId.HasValue, Is.True);
        Assert.That(orderCreatingSagaId.Value, Is.EqualTo(correlationId));
    }

    [Test]
    public async Task When_Stocks_Reserved_Then_State_Should_Be_Stocks_Reserved()
    {
        var correlationId = Guid.NewGuid();
        await _harness.Bus.Publish(new BasketCheckedOutEvent(correlationId, Guid.NewGuid(), string.Empty, string.Empty, Guid.NewGuid(), new List<BasketCheckoutItem>()));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new StocksReservedEvent(correlationId, 10));

        var catalogUpdatedSagaId = await _sagaHarness.Exists(correlationId, x => x.StocksReserved, TimeSpan.FromSeconds(10));

        Assert.That(catalogUpdatedSagaId.HasValue, Is.True);
        Assert.That(catalogUpdatedSagaId.Value, Is.EqualTo(correlationId));
    }

    [Test]
    public async Task When_Stocks_Reservation_Failed_Then_State_Should_Be_Checkout_Failed()
    {
        var correlationId = Guid.NewGuid();
        await _harness.Bus.Publish(new BasketCheckedOutEvent(correlationId, Guid.NewGuid(), string.Empty, string.Empty, Guid.NewGuid(), new List<BasketCheckoutItem>()));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new StocksReservationFailedEvent(correlationId));

        var catalogUpdatedSagaId = await _sagaHarness.Exists(correlationId, x => x.CheckoutFailed, TimeSpan.FromSeconds(10));

        Assert.That(catalogUpdatedSagaId.HasValue, Is.True);
        Assert.That(catalogUpdatedSagaId.Value, Is.EqualTo(correlationId));
    }

    [Test]
    public async Task When_Payment_Successfully_Processed_Then_State_Should_Be_Payment_Processed()
    {
        var correlationId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        await _harness.Bus.Publish(new BasketCheckedOutEvent(correlationId, Guid.NewGuid(), string.Empty, string.Empty, Guid.NewGuid(), new List<BasketCheckoutItem>()));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new StocksReservedEvent(correlationId, 10));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new PaymentProcessedEvent(correlationId));

        var orderCreatedSagaId = await _sagaHarness.Exists(correlationId, x => x.PaymentProcessed, TimeSpan.FromSeconds(10));

        Assert.That(orderCreatedSagaId.HasValue, Is.True);
        Assert.That(orderCreatedSagaId.Value, Is.EqualTo(correlationId));
    }


    [Test]
    public async Task When_Payment_Failed_Then_State_Should_Be_Payment_Failed()
    {
        var correlationId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        await _harness.Bus.Publish(new BasketCheckedOutEvent(correlationId, Guid.NewGuid(), string.Empty, string.Empty, Guid.NewGuid(), new List<BasketCheckoutItem>()));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new StocksReservedEvent(correlationId, 10));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new PaymentFailedEvent(correlationId));

        var orderCreatedSagaId = await _sagaHarness.Exists(correlationId, x => x.PaymentFailed, TimeSpan.FromSeconds(10));

        Assert.That(orderCreatedSagaId.HasValue, Is.True);
        Assert.That(orderCreatedSagaId.Value, Is.EqualTo(correlationId));
    }

    [Test]
    public async Task When_Order_Created_Then_State_Should_Be_Order_Created()
    {
        var correlationId = Guid.NewGuid();
        var orderId = 3;
        await _harness.Bus.Publish(new BasketCheckedOutEvent(correlationId, Guid.NewGuid(), string.Empty, string.Empty, Guid.NewGuid(), new List<BasketCheckoutItem>()));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new StocksReservedEvent(correlationId, 10));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new PaymentProcessedEvent(correlationId));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new OrderCreatedEvent(correlationId, orderId));

        var orderCreatedSagaId = await _sagaHarness.Exists(correlationId, x => x.OrderCreated, TimeSpan.FromSeconds(10));

        Assert.That(orderCreatedSagaId.HasValue, Is.True);
        Assert.That(orderCreatedSagaId.Value, Is.EqualTo(correlationId));
    }


    [Test]
    public async Task When_Stocks_Released_Then_State_Should_Be_Checkout_Failed()
    {
        var correlationId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        await _harness.Bus.Publish(new BasketCheckedOutEvent(correlationId, Guid.NewGuid(), string.Empty, string.Empty, Guid.NewGuid(), new List<BasketCheckoutItem>()));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new StocksReservedEvent(correlationId, 10));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new PaymentFailedEvent(correlationId));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new StocksReleasedEvent(correlationId));

        var orderCreatedSagaId = await _sagaHarness.Exists(correlationId, x => x.CheckoutFailed, TimeSpan.FromSeconds(10));

        Assert.That(orderCreatedSagaId.HasValue, Is.True);
        Assert.That(orderCreatedSagaId.Value, Is.EqualTo(correlationId));
    }

    [Test]
    public async Task When_Basket_Cleared_Then_State_Should_Be_Finalized()
    {
        var correlationId = Guid.NewGuid();
        await _harness.Bus.Publish(new BasketCheckedOutEvent(correlationId, Guid.NewGuid(), string.Empty, string.Empty, Guid.NewGuid(), new List<BasketCheckoutItem>()));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new StocksReservedEvent(correlationId, 10));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new PaymentProcessedEvent(correlationId));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new OrderCreatedEvent(correlationId, 6));
        await _sagaHarness.Consumed.Any();
        await _harness.Bus.Publish(new BasketClearedEvent(correlationId));

        var finalizedSagaId = await _sagaHarness.Exists(correlationId, x => x.Final, TimeSpan.FromSeconds(10));

        Assert.That(finalizedSagaId.HasValue, Is.True);
        Assert.That(finalizedSagaId.Value, Is.EqualTo(correlationId));
    }
}