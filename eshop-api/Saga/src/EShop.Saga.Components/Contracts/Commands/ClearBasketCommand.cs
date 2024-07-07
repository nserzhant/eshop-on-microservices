namespace EShop.Basket.Integration.Commands;

public record ClearBasketCommand(Guid CorrelationId, Guid CustomerId, Guid BasketId);