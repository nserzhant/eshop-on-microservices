using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace EShop.Catalog.Infrastructure.ConsumeFilters;

public class IdempotentConsumingFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly ILogger<IdempotentConsumingFilter<T>> _logger;
    private readonly CatalogDbContext _catalogDbContext;

    public IdempotentConsumingFilter(ILogger<IdempotentConsumingFilter<T>> logger,
        CatalogDbContext catalogDbContext)
    {
        _logger = logger;
        _catalogDbContext = catalogDbContext;
    }


    public void Probe(ProbeContext context) { }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {

        using var transaction = await _catalogDbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

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
            var exists = await _catalogDbContext.ConsumedIntegrationCommands.ContainsAsync(consumedCommand);

            if (exists)
            {
                _logger.LogWarning("Command was previously processed - MessageId: {messageId}, CorrelationId: {correlationId}  : {@consumedMessage}", messageId, correlationId, context.Message);

                transaction.Rollback();

                return;
            }

            _catalogDbContext.ConsumedIntegrationCommands.Add(consumedCommand);

            await next.Send(context);

            await _catalogDbContext.SaveChangesAsync();
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
