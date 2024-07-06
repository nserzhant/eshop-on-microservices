using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace EShop.Ordering.Infrastructure.ConsumeFilters;
public class IdempotentConsumingFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly ILogger<IdempotentConsumingFilter<T>> _logger;
    private readonly OrderingDbContext  _orderingDbContext;

    public IdempotentConsumingFilter(ILogger<IdempotentConsumingFilter<T>> logger,
        OrderingDbContext orderingDbContext)
    {
        _logger = logger;
        _orderingDbContext = orderingDbContext;
    }


    public void Probe(ProbeContext context) { }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {

        using var transaction = await _orderingDbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            var messageId = context.MessageId;
            var correlationId = context.CorrelationId;
            var commandType = typeof(T).ToString();

            if (!messageId.HasValue)
            {
                throw new NotSupportedException($"Command {commandType} with CorrelationId: {correlationId} cannot be processed without MessageId");
            }

            var consumedCommand = new ConsumedIntegrationCommand(messageId.Value);
            var exists = await _orderingDbContext.ConsumedIntegrationCommands.ContainsAsync(consumedCommand);

            if (exists)
            {
                _logger.LogWarning("Command was previously processed - MessageId: {messageId}, CorrelationId: {correlationId}  : {@consumedMessage}", messageId, correlationId, context.Message);

                transaction.Rollback();

                return;
            }

            _orderingDbContext.ConsumedIntegrationCommands.Add(consumedCommand);

            await next.Send(context);

            await _orderingDbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(ex, "error");

            throw ex;
        }
    }
}
