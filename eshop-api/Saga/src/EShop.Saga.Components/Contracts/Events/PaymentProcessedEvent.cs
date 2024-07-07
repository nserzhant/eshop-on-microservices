namespace EShop.Payment.Integration.Events;

public record PaymentProcessedEvent(Guid CorrelationId);
