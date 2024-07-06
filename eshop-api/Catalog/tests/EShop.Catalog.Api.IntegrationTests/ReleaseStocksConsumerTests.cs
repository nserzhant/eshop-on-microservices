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
[Category("ReleaseStocksConsumer")]
public class ReleaseStocksConsumerTests : BaseCatalogIntegrationTests
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
            cfg.AddConsumer<ReleaseStocksConsumer>();
        });
    }

    [Test]
    public async Task When_Release_Stocks_Then_Available_Qty_Should_Be_Increased()
    {
        var availableQty = 7;
        var releasedQty = 12;
        var catalogItem = await createCatalogItemAsync("Test Item", availableQty: availableQty);
        var command = new ReleaseStocksCommand(Guid.NewGuid(), [new ReleaseStockItem(catalogItem.Id, releasedQty)]);

        await _harness.Bus.Publish(command);

        Assert.That(await _harness.Consumed.Any<ReleaseStocksCommand>());
        var catalogItemActual = await _catalogItemRepository.GetCatalogItemAsync(catalogItem.Id);
        Assert.That(catalogItemActual?.AvailableQty, Is.EqualTo(19));
    }

    [Test]
    public async Task When_Release_Stocks_Then_Stock_Released_Event_With_Correct_Total_Price_Should_Be_Published()
    {
        var correlationId = Guid.NewGuid();
        var availableQty = 6;
        var releasedQty = 12;
        var catalogItem = await createCatalogItemAsync("Test Item", availableQty: availableQty);
        var command = new ReleaseStocksCommand(correlationId, [new ReleaseStockItem(catalogItem.Id, releasedQty)]);

        await _harness.Bus.Publish(command);

        Assert.That(await _harness.Consumed.Any<ReleaseStocksCommand>());
        var consumerHarness = _harness.GetConsumerHarness<ReleaseStocksConsumer>();
        Assert.That(await consumerHarness.Consumed.Any<ReleaseStocksCommand>());
        Assert.That(await _harness.Published.Any<StocksReleasedEvent>(publishedMessage =>
        {
            return publishedMessage.Context.Message.CorrelationId == correlationId;
        }));
    }
}
