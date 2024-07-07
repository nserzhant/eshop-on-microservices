namespace EShop.Saga.Components.StateMachines;

public class OrderingStateMachineSettings
{
    public string ReserveStocksQueueName { get; set; } = "reserve-stocks";
    public string ProcessPaymentQueueName { get; set; } = "process-payment";
    public string CreateOrderQueueName { get; set; } = "create-order";
    public string ReleaseStockQueueName { get; set; } = "release-stocks";
    public string ClearBasketQueueName { get; set; } = "clear-basket";
}
