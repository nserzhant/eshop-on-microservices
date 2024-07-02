namespace EShop.Basket.Api.Integration.Commands;

public record ClearBasketCommand(Guid CorrelationId, Guid UserId, Guid BasketId);