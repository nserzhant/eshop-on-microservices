namespace EShop.Saga.Processor;

public class MessageBrockerSettings
{
    public string? AzureServiceBusConnectionString { get; set; } = null;
    public string RabbitMQHost { get; set; }
    public ushort RabbitMQPort { get; set; }
    public string RabbitMQVirtualHost { get; set; }
    public string RabbitMQUsername { get; set; }
    public string RabbitMQPassword { get; set; }
}
