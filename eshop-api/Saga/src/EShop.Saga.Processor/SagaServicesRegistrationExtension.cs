using EShop.Saga.Components;
using EShop.Saga.Components.Infrastructure;
using EShop.Saga.Components.StateMachines;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EShop.Saga.Processor;

public static class SagaServicesRegistrationExtension
{
    public static IServiceCollection AddSagaServices(this IServiceCollection services, IConfiguration configuration)
    {
        var messageBrokerSettings = configuration.GetSection(Consts.MESSAGE_BROKER_CONFIG_NAME)
            .Get<MessageBrockerSettings>();

        if (messageBrokerSettings == null)
        {
            throw new ConfigurationException("Message broker configuration not found");
        }

        var connectionString = configuration.GetConnectionString(DBConsts.SAGA_DB_CONNECTION_STRING_NAME);

        services.Configure<OrderingStateMachineSettings>(configuration.GetSection(Consts.ORDERING_STATE_MACHINE_SETTINGS_CONFIG_NAME));

        services.AddMassTransit(x =>
        {
            x.AddSagaStateMachine<OrderingStateMachine, OrderingStateMachineInstance>()
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Pessimistic;

                    r.AddDbContext<DbContext, EShopSagaDbContext>((provider, builder) =>
                    {
                        builder.UseSqlServer(connectionString, m =>
                        {
                            m.MigrationsAssembly(typeof(EShopSagaDbContext).Assembly.GetName().Name);
                            m.MigrationsHistoryTable("_EFMigrationsHistory", DBConsts.SAGA_DB_SCHEMA_NAME);
                        });
                    });
                });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(messageBrokerSettings.RabbitMQHost, messageBrokerSettings.RabbitMQPort, messageBrokerSettings.RabbitMQVirtualHost, h =>
                {
                    h.Username(messageBrokerSettings.RabbitMQUsername);
                    h.Password(messageBrokerSettings.RabbitMQPassword);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
