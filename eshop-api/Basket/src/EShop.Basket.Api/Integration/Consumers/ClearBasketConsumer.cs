using EShop.Basket.Core.Interfaces;
using EShop.Basket.Integration.Commands;
using EShop.Basket.Integration.Events;
using MassTransit;

namespace EShop.Basket.Api.Integration.Consumers;

public class ClearBasketConsumer : IConsumer<ClearBasketCommand>
{
    private readonly ILogger<ClearBasketConsumer> _logger;
    private readonly IBasketRepository _basketRepository;

    public ClearBasketConsumer(ILogger<ClearBasketConsumer> logger, IBasketRepository basketRepository)
    {
        _logger = logger;
        _basketRepository = basketRepository;
    }

    public async Task Consume(ConsumeContext<ClearBasketCommand> context)
    {
        var command = context.Message;

        _logger.LogInformation("Start Processing ClearBasketCommand, CorrelationId: {CorrelationId}, Command: {@command}", context.CorrelationId, command);

        var basket = await _basketRepository.GetBasketAsync(command.CustomerId);

        if (basket.Id == command.BasketId)
        {
            await _basketRepository.DeleteBasketAsync(command.CustomerId);
        }

        var basketCleared = new BasketClearedEvent(command.CorrelationId);

        await context.Publish(basketCleared);

        _logger.LogInformation($"End Processing ClearBasketCommand CorrelationId: {command.CorrelationId}");
    }
}
