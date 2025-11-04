using MassTransit.Testing;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using EShop.Payment.Processor.Integration.Consumers;
using EShop.Payment.Integration.Commands;
using EShop.Payment.Integration.Events;

namespace EShop.Payment.Processor.Tests;

[TestFixture]
[Category("Payment")]
public class ProcessPaymentConsumerTests
{
    private ServiceProvider _serviceProvider;
    private ITestHarness _harness;
    private bool _processPayment = true;

    [SetUp]
    public async Task SetupAsync()
    {
        var serviceCollection = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<ProcessPaymentConsumer>();
            });

        serviceCollection.AddOptions<PaymentProcessorSettings>()
            .Configure(ps => ps.ProcessPayment = _processPayment);

        _serviceProvider = serviceCollection.BuildServiceProvider(true);
        _harness = _serviceProvider.GetRequiredService<ITestHarness>();

        await _harness.Start();
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await _harness.Stop();
        await _serviceProvider.DisposeAsync();
    }

    [Test]
    public async Task When_Payment_Is_Processed_Then_Payment_Processed_Event_Should_Be_Published()
    {
        _processPayment = true;
        var correlationId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var amount = 12.4m;

        await _harness.Bus.Publish(new ProcessPaymentCommand(correlationId, customerId, amount));

        var consumerHarness = _harness.GetConsumerHarness<ProcessPaymentConsumer>();
        Assert.That(await consumerHarness.Consumed.Any<ProcessPaymentCommand>());
        Assert.That(await _harness.Published.Any<PaymentProcessedEvent>( publishedMessage =>
        {
            return publishedMessage.Context.Message.CorrelationId == correlationId;
        }));
    }

    [Test]
    public async Task When_Payment_Is_Not_Processed_Then_Payment_Failed_Event_Should_Be_Published()
    {
        _processPayment = false;
        var correlationId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var amount = 12.4m;

        await _harness.Bus.Publish(new ProcessPaymentCommand(correlationId, customerId, amount));

        var consumerHarness = _harness.GetConsumerHarness<ProcessPaymentConsumer>();
        Assert.That(await consumerHarness.Consumed.Any<ProcessPaymentCommand>());
        Assert.That(await _harness.Published.Any<PaymentFailedEvent>(publishedMessage =>
        {
            return publishedMessage.Context.Message.CorrelationId == correlationId;
        }));
    }
}