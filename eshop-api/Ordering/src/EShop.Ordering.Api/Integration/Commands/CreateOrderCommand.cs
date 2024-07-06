namespace EShop.Ordering.Integration.Commands;

public record CreateOrderCommand(Guid CorrelationId, Guid CustomerId, string CustomerEmail, string ShippingAddress, List<CreateOrdeItem> Items);

public record CreateOrdeItem(Guid CatalogItemId, string ItemName, int Qty, string Description, decimal Price, string TypeName, string BrandName, string PictureUri);