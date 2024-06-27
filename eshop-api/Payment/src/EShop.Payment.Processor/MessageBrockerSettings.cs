namespace EShop.Payment.Processor;
public class MessageBrockerSettings
{
    public string RabbitMQHost { get; set; }
    public ushort RabbitMQPort { get; set; }
    public string RabbitMQVirtualHost { get; set; }
    public string RabbitMQUsername { get; set; }
    public string RabbitMQPassword { get; set; }
    public string QueueName { get; set; }
}
