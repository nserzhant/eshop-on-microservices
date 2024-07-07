namespace EShop.Payment.Integration.Commands;
public record ProcessPaymentCommand(Guid CorrelationId, Guid CustomerId, decimal Amount);