namespace EShop.Basket.Integration.Events;

public record BasketCheckoutItem(Guid CatalogItemId, string ItemName, int Qty, string Description, decimal Price, string TypeName, string BrandName, string PictureUri);

public record BasketCheckedOutEvent(Guid CorrelationId, Guid CustomerId, string CustomerEmail, string ShippingAddress, Guid BasketId, List<BasketCheckoutItem> Items);