namespace EShop.Payment.Processor.Events;
public record PaymentFailedEvent(Guid CorrelationId);