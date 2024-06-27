using EShop.Payment.Processor.Commands;
using EShop.Payment.Processor.Events;
using MassTransit;
using Microsoft.Extensions.Options;

namespace EShop.Payment.Processor.Consumers;

public class ProcessPaymentConsumer : IConsumer<ProcessPaymentCommand>
{
    private IOptions<PaymentProcessorSettings> _paymentProcessorSettings;
    private ILogger<ProcessPaymentConsumer> _logger;

    public ProcessPaymentConsumer(IOptions<PaymentProcessorSettings> paymentProcessorSettings, ILogger<ProcessPaymentConsumer> logger)
    {
        _paymentProcessorSettings = paymentProcessorSettings;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
    {
        var command = context.Message;

        _logger.LogInformation("Start Processing ProcessPaymentCommand, CorrelationId: {CorrelationId}, Command: {@command}", context.CorrelationId, command);

        // Result of the fake payment operation is configured in app.settings file

        var isPaymentSuccess = _paymentProcessorSettings.Value.ProcessPayment;

        if (isPaymentSuccess)
        {
            await context.Publish(new PaymentProcessedEvent(command.CorrelationId));
        }
        else
        {
            await context.Publish(new PaymentFailedEvent(command.CorrelationId));
        }

        _logger.LogInformation("End Processing ProcessPaymentCommand, CorrelationId: {CorrelationId}, Result: {@command}", context.CorrelationId, isPaymentSuccess);

    }
}