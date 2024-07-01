namespace EShop.Basket.Api.Integration.Events;

public record BasketCheckoutItem(Guid CatalogItemId, decimal Qty);
public record BasketCheckedOutEvent(Guid CorrelationId, Guid CustomerId, Guid BasketId, List<BasketCheckoutItem> Items);