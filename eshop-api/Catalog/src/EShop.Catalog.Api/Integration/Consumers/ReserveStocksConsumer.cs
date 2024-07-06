using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;
using EShop.Catalog.Core.Services;
using EShop.Catalog.Integration.Commands;
using EShop.Catalog.Integration.Events;
using MassTransit;

namespace EShop.Catalog.Integration.Consumers;

public class ReserveStocksConsumer : IConsumer<ReserveStocksCommand>
{
    private readonly ILogger<ReserveStocksConsumer> _logger;
    private readonly ICatalogItemService _catalogItemService;
    private readonly ICatalogItemRepository _catalogItemRepository;

    public ReserveStocksConsumer(ILogger<ReserveStocksConsumer> logger,
        ICatalogItemService catalogItemService,
        ICatalogItemRepository catalogItemRepository)
    {
        _logger = logger;
        _catalogItemService = catalogItemService;
        _catalogItemRepository = catalogItemRepository;
    }

    public async Task Consume(ConsumeContext<ReserveStocksCommand> context)
    {
        var command = context.Message;

        _logger.LogInformation("Start Processing ReserveStocksCommand, CorrelationId: {CorrelationId}, Command: {@command}", context.CorrelationId, command);

        bool isQtyAvailable = true;
        var itemsToUpdate = new Dictionary<Guid, CatalogItem>();
        decimal totalPrice = 0;

        foreach (var item in command.Items)
        {
            var catalogItem = await _catalogItemRepository.GetCatalogItemAsync(item.CatalogItemId);

            if (catalogItem == null || catalogItem.AvailableQty < item.Qty)
            {
                _logger.LogError("Item Reservation Failed, CorrelationId: {CorrelationId}, Item: {item}, Requested Qty: {@command}", context.CorrelationId, catalogItem, item.Qty);

                isQtyAvailable = false;
                break;
            }
            else
            {
                itemsToUpdate.Add(item.CatalogItemId, catalogItem);
                totalPrice += item.Qty * catalogItem.Price ?? 0;
            }
        }

        if (!isQtyAvailable)
        {
            var reservationFailedEvent = new StocksReservationFailedEvent(command.CorrelationId);
            await context.Publish(reservationFailedEvent);

            return;
        }

        foreach (var item in command.Items)
        {
            var itemToUpdate = itemsToUpdate[item.CatalogItemId];
            itemToUpdate.UpdateAvailableQty(itemToUpdate.AvailableQty - item.Qty);
            await _catalogItemService.UpdateCatalogItemAsync(itemToUpdate);
        }

        await _catalogItemRepository.SaveChangesAsync();

        var catalogUpdated = new StocksReservedEvent(command.CorrelationId, totalPrice);
        await context.Publish(catalogUpdated);

        _logger.LogInformation("End Processing ReserveStocksCommand, CorrelationId: {CorrelationId}", context.CorrelationId);

    }
}
