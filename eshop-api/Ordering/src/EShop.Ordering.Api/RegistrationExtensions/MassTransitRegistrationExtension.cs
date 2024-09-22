using EShop.Ordering.Api.Integration.Consumers;
using EShop.Ordering.Api.Settings;
using EShop.Ordering.Infrastructure.ConsumeFilters;
using MassTransit;

namespace EShop.Ordering.Api.RegistrationExtensions;

public static class MassTransitRegistrationExtension
{
    public static IServiceCollection AddMassTransitServices(this IServiceCollection services, IConfiguration configuration)
    {
        var messageBrokerSettings = configuration.GetSection(Consts.MESSAGE_BROKER_CONFIG_NAME).Get<MessageBrockerSettings>();

        if (messageBrokerSettings == null ||
            (string.IsNullOrEmpty(messageBrokerSettings.AzureServiceBusConnectionString) &&
            string.IsNullOrEmpty(messageBrokerSettings.RabbitMQHost)))
        {
            return services;
        }

        services.AddMassTransit(x =>
        {
            x.AddConsumer<CreateOrderConsumer>();

            if (messageBrokerSettings.AzureServiceBusConnectionString != null)
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(messageBrokerSettings.AzureServiceBusConnectionString);

                    cfg.ReceiveEndpoint(messageBrokerSettings.QueueName, configureEndpoint =>
                    {
                        configureEndpoint.ConfigureConsumer<CreateOrderConsumer>(context);
                        configureEndpoint.UseConsumeFilter(typeof(IdempotentConsumingFilter<>), context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            }
            else
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(messageBrokerSettings.RabbitMQHost, messageBrokerSettings.RabbitMQPort, messageBrokerSettings.RabbitMQVirtualHost, h =>
                    {
                        h.Username(messageBrokerSettings.RabbitMQUsername);
                        h.Password(messageBrokerSettings.RabbitMQPassword);
                    });

                    cfg.ReceiveEndpoint(messageBrokerSettings.QueueName, configureEndpoint =>
                    {
                        configureEndpoint.ConfigureConsumer<CreateOrderConsumer>(context);
                        configureEndpoint.UseConsumeFilter(typeof(IdempotentConsumingFilter<>), context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            }
        });

        return services;
    }
}
