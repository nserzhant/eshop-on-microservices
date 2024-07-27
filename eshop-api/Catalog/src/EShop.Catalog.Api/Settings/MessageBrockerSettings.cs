namespace EShop.Catalog.Api.Settings;

public class MessageBrockerSettings
{
    public string? AzureServiceBusConnectionString { get; set; } = null;
    public string RabbitMQHost { get; set; } = "localhost";
    public ushort RabbitMQPort { get; set; } = 5672;
    public string RabbitMQVirtualHost { get; set; } = "/";
    public string RabbitMQUsername { get; set; } = "guest";
    public string RabbitMQPassword { get; set; } = "guest";
    public string ReserveStockQueueName { get; set; } = "reserve-stocks";
    public string ReleaseStockQueueName { get; set; } = "release-stocks";
}
