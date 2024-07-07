namespace EShop.Payment.Integration.Events;

public record PaymentFailedEvent(Guid CorrelationId);