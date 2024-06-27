namespace EShop.Payment.Processor.Events;
public record PaymentProcessedEvent(Guid CorrelationId);
