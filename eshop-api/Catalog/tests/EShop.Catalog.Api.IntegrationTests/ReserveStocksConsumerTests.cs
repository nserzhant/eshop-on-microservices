using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Infrastructure.IntegrationTests;
using EShop.Catalog.Integration.Commands;
using EShop.Catalog.Integration.Consumers;
using EShop.Catalog.Integration.Events;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Catalog.Api.IntegrationTests;

[TestFixture]
[Category("ReserveStocksConsumer")]
[Category("Saga")]
public class ReserveStocksConsumerTests : BaseCatalogIntegrationTests
{
    private ITestHarness _harness;
    private ICatalogItemRepository _catalogItemRepository;

    public override async Task SetupAsync()
    {
        await base.SetupAsync();

        _harness = serviceProvider.GetRequiredService<ITestHarness>();
        _catalogItemRepository = serviceProvider.GetRequiredService<ICatalogItemRepository>();

        await _harness.Start();
    }

    public override async Task TearDownAsync()
    {
        await _harness.Stop();
        await base.TearDownAsync();
    }

    protected override void AddServices(ServiceCollection sc)
    {
        base.AddServices(sc);

        sc.AddMassTransitTestHarness(cfg =>
        {
            cfg.AddConsumer<ReserveStocksConsumer>();
        });
    }

    [Test]
    public async Task When_Reserve_Stocks_Then_Available_Qty_Should_Be_Reduced()
    {
        var availableQty = 12;
        var orderedQty = 10;
        var catalogItem = await createCatalogItemAsync("Test Item", availableQty: availableQty);
        var command = new ReserveStocksCommand(Guid.NewGuid(), [new ReserveStockItem(catalogItem.Id, orderedQty)]);

        await _harness.Bus.Publish(command);

        Assert.That(await _harness.Consumed.Any<ReserveStocksCommand>());
        var catalogItemActual = await _catalogItemRepository.GetCatalogItemAsync(catalogItem.Id);
        Assert.That(catalogItemActual?.AvailableQty, Is.EqualTo(2));
    }

    [Test]
    public async Task When_Reserve_Stocks_Then_Stock_Reserved_Event_With_Correct_Total_Price_Should_Be_Published()
    {
        var correlationId = Guid.NewGuid();
        var availableQty = 12;
        var orderedQty = 10;
        var price = 17.6m;
        var catalogItem = await createCatalogItemAsync("Test Item", availableQty: availableQty, price: price);
        var command = new ReserveStocksCommand(correlationId, [new ReserveStockItem(catalogItem.Id, orderedQty)]);

        await _harness.Bus.Publish(command);

        Assert.That(await _harness.Consumed.Any<ReserveStocksCommand>());
        var consumerHarness = _harness.GetConsumerHarness<ReserveStocksConsumer>();
        Assert.That(await consumerHarness.Consumed.Any<ReserveStocksCommand>());
        Assert.That(await _harness.Published.Any<StocksReservedEvent>(publishedMessage =>
        {
            return  publishedMessage.Context.Message.CorrelationId == correlationId &&
                    publishedMessage.Context.Message.TotalPrice == 176;
        }));
    }


    [Test]
    public async Task When_Reserve_Not_Available_Stocks_Then_Stock_Reservation_Failed_Event_Should_Be_Published()
    {
        var correlationId = Guid.NewGuid();
        var availableQty = 12;
        var orderedQty = 14;
        var catalogItem = await createCatalogItemAsync("Test Item", availableQty: availableQty);
        var command = new ReserveStocksCommand(correlationId, [new ReserveStockItem(catalogItem.Id, orderedQty)]);

        await _harness.Bus.Publish(command);

        Assert.That(await _harness.Consumed.Any<ReserveStocksCommand>());
        var consumerHarness = _harness.GetConsumerHarness<ReserveStocksConsumer>();
        Assert.That(await consumerHarness.Consumed.Any<ReserveStocksCommand>());
        Assert.That(await _harness.Published.Any<StocksReservationFailedEvent>(publishedMessage =>
        {
            return publishedMessage.Context.Message.CorrelationId == correlationId;
        }));
    }
}
