using EShop.Catalog.Api.Constants;
using EShop.Catalog.Api.Settings;
using EShop.Catalog.Infrastructure.ConsumeFilters;
using EShop.Catalog.Integration.Consumers;
using MassTransit;

namespace EShop.Catalog.Api.RegistrationExtensions;

public static class MassTransitRegistrationExtension
{
    public static IServiceCollection AddMassTransitServices(this IServiceCollection services, IConfiguration configuration)
    {
        var messageBrokerSettings = configuration.GetSection(ConfigurationKeys.MESSAGE_BROKER_CONFIG_NAME)
            .Get<MessageBrockerSettings>();

        if (messageBrokerSettings == null ||
            (string.IsNullOrEmpty(messageBrokerSettings.AzureServiceBusConnectionString) &&
            string.IsNullOrEmpty(messageBrokerSettings.RabbitMQHost)))
        {
            return services;
        }

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ReserveStocksConsumer>();
            x.AddConsumer<ReleaseStocksConsumer>();

            if (messageBrokerSettings.AzureServiceBusConnectionString != null)
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(messageBrokerSettings.AzureServiceBusConnectionString);
                    cfg.UseInMemoryOutbox(context);
                    cfg.UseConsumeFilter(typeof(IdempotentConsumingFilter<>), context);

                    cfg.ReceiveEndpoint(messageBrokerSettings.ReserveStockQueueName, configureEndpoint =>
                    {
                        configureEndpoint.ConfigureConsumer<ReserveStocksConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(messageBrokerSettings.ReleaseStockQueueName, configureEndpoint =>
                    {
                        configureEndpoint.ConfigureConsumer<ReleaseStocksConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            }
            else
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseInMemoryOutbox(context);
                    cfg.UseConsumeFilter(typeof(IdempotentConsumingFilter<>), context);

                    cfg.Host(messageBrokerSettings.RabbitMQHost, messageBrokerSettings.RabbitMQPort, messageBrokerSettings.RabbitMQVirtualHost, h =>
                    {
                        h.Username(messageBrokerSettings.RabbitMQUsername);
                        h.Password(messageBrokerSettings.RabbitMQPassword);
                    });

                    cfg.ReceiveEndpoint(messageBrokerSettings.ReserveStockQueueName, configureEndpoint =>
                    {
                        configureEndpoint.ConfigureConsumer<ReserveStocksConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(messageBrokerSettings.ReleaseStockQueueName, configureEndpoint =>
                    {
                        configureEndpoint.ConfigureConsumer<ReleaseStocksConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            }
        });

        return services;
    }
}
