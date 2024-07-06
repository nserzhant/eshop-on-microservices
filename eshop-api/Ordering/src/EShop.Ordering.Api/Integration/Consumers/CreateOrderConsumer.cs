using EShop.Ordering.Core.Interfaces;
using EShop.Ordering.Core.Models;
using EShop.Ordering.Infrastructure.Services;
using EShop.Ordering.Integration.Commands;
using EShop.Ordering.Integration.Events;
using MassTransit;

namespace EShop.Ordering.Api.Integration.Consumers;

public class CreateOrderConsumer : IConsumer<CreateOrderCommand>
{
    private readonly ILogger<CreateOrderConsumer> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IDateTimeService _dateTimeService;

    public CreateOrderConsumer(IOrderRepository orderRepository, ILogger<CreateOrderConsumer> logger, IDateTimeService dateTimeService)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _dateTimeService = dateTimeService;
    }

    public async Task Consume(ConsumeContext<CreateOrderCommand> context)
    {
        var command = context.Message;

        _logger.LogInformation("Start Processing CreateOrderCommand, CorrelationId: {CorrelationId}, Command: {@command}", context.CorrelationId, command);

        var orderItems = command.Items
            .Select(itm => new OrderItem(itm.CatalogItemId, itm.ItemName, itm.Description, itm.Price, itm.TypeName, itm.BrandName, itm.Qty, itm.PictureUri))
            .ToList();
        var order = new Order(_dateTimeService.GetCurrentDateTime(), command.CustomerId, command.CustomerEmail, command.ShippingAddress, orderItems);

        await _orderRepository.CreateOrder(order);
        await _orderRepository.SaveChangesAsync();

        var orderCreated = new OrderCreatedEvent(command.CorrelationId, order.Id);

        await context.Publish(orderCreated);

        _logger.LogInformation("End Processing CreateOrderCommand, CorrelationId: {CorrelationId}", context.CorrelationId);
    }
}