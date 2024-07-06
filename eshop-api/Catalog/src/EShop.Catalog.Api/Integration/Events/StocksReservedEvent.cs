namespace EShop.Catalog.Integration.Events;

public record StocksReservedEvent(Guid CorrelationId, decimal TotalPrice);
