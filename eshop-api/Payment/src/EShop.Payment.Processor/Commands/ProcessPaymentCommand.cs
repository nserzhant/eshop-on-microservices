namespace EShop.Payment.Processor.Commands;
public record ProcessPaymentCommand(Guid CorrelationId, Guid CustomerId, decimal Amount);