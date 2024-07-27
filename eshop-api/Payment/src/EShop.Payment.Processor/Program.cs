using EShop.Payment.Processor;
using EShop.Payment.Processor.Integration.Consumers;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddLogging(log => log.AddConsole());

var messageBrokerSettings = builder.Configuration.GetSection(Consts.MESSAGE_BROKER_CONFIG_NAME).Get<MessageBrockerSettings>();

if (messageBrokerSettings == null)
{
    throw new ConfigurationException("Message broker configuration not found");
}

builder.Services.Configure<PaymentProcessorSettings>(builder.Configuration.GetSection(Consts.PAYMENT_SETTINGS_CONFIG_NAME));
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProcessPaymentConsumer>();

    if (messageBrokerSettings.AzureServiceBusConnectionString != null)
    {
        x.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host(messageBrokerSettings.AzureServiceBusConnectionString);

            cfg.ReceiveEndpoint(messageBrokerSettings.QueueName, configureEndpoint =>
            {
                configureEndpoint.ConfigureConsumer<ProcessPaymentConsumer>(context);
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
                configureEndpoint.ConfigureConsumer<ProcessPaymentConsumer>(context);
            });

            cfg.ConfigureEndpoints(context);
        });
    }

});

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

var host = builder.Build();


host.MapHealthChecks("/hc");

host.Run();