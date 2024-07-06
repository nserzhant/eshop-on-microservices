using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Services;
using EShop.Catalog.Integration.Commands;
using EShop.Catalog.Integration.Events;
using MassTransit;

namespace EShop.Catalog.Integration.Consumers;

public class ReleaseStocksConsumer : IConsumer<ReleaseStocksCommand>
{
    private readonly ILogger<ReleaseStocksConsumer> _logger;
    private readonly ICatalogItemService _catalogItemService;
    private readonly ICatalogItemRepository _catalogItemRepository;

    public ReleaseStocksConsumer(ILogger<ReleaseStocksConsumer> logger,
        ICatalogItemService catalogItemService,
        ICatalogItemRepository catalogItemRepository)
    {
        _logger = logger;
        _catalogItemService = catalogItemService;
        _catalogItemRepository = catalogItemRepository;
    }

    public async Task Consume(ConsumeContext<ReleaseStocksCommand> context)
    {
        var command = context.Message;

        _logger.LogInformation("Start Processing ReleaseStocksCommand, CorrelationId: {CorrelationId}, Command: {@command}", context.CorrelationId, command);

        foreach (var item in command.Items)
        {
            var catalogItem = await _catalogItemRepository.GetCatalogItemAsync(item.CatalogItemId);

            if (catalogItem != null)
            {
                catalogItem.UpdateAvailableQty(catalogItem.AvailableQty + item.Qty);
                await _catalogItemService.UpdateCatalogItemAsync(catalogItem);
            }
            else
            {
                _logger.LogWarning("Item Not Found For Releasing Stock, CorrelationId: {CorrelationId}, itemId: {@itemId}", context.CorrelationId, item.CatalogItemId);
            }
        }

        await _catalogItemRepository.SaveChangesAsync();

        var stocksReleased = new StocksReleasedEvent(command.CorrelationId);

        await context.Publish(stocksReleased);

        _logger.LogInformation("End Processing ReserveStocksCommand, CorrelationId: {CorrelationId}", context.CorrelationId);

    }
}