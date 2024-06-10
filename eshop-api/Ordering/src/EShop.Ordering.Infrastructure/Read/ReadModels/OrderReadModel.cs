namespace EShop.Ordering.Infrastructure.Read.ReadModels;
public record OrderReadModel
{
    public int Id { get; init; }
    public byte[] Ts { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public DateTime OrderDate { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerEmail { get; init; }
    public string ShippingAddress { get; init; }
    public List<OrderItemReadModel> OrderItems { get; init; }
}
