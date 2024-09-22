using EShop.Basket.Api.Integration.Consumers;
using EShop.Basket.Api.Settings;
using MassTransit;

namespace EShop.Basket.Api.RegistrationExtensions;

public static class MassTransitRegistrationExtension
{
    public static IServiceCollection AddMassTransitServices(this IServiceCollection services, IConfiguration configuration)
    {
        var messageBrokerSettings = configuration.GetSection(Consts.MESSAGE_BROKER_CONFIG_NAME).Get<MessageBrockerSettings>();

        if(messageBrokerSettings == null || 
            (string.IsNullOrEmpty(messageBrokerSettings.AzureServiceBusConnectionString) && 
            string.IsNullOrEmpty(messageBrokerSettings.RabbitMQHost)))
        {
            return services;
        }

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ClearBasketConsumer>();


            if (messageBrokerSettings.AzureServiceBusConnectionString != null)
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(messageBrokerSettings.AzureServiceBusConnectionString);

                    cfg.ReceiveEndpoint(messageBrokerSettings.QueueName, configureEndpoint =>
                    {
                        configureEndpoint.ConfigureConsumer<ClearBasketConsumer>(context);
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
                        configureEndpoint.ConfigureConsumer<ClearBasketConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            }
        });

        return services;
    }
}
