namespace EShop.Ordering.Integration.Events;

public record OrderCreatedEvent(Guid CorrelationId, int OrderId);
