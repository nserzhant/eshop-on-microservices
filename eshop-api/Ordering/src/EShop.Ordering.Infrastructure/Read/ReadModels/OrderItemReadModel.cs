namespace EShop.Ordering.Infrastructure.Read.ReadModels;
public enum OrderStatus { Created, Shipped, Delivered, Returned }

public class OrderItemReadModel
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string? Description { get; set; }
    public decimal Price { get; init; }
    public string TypeName { get; init; }
    public string BrandName { get; init; }
    public string PictureUri { get; init; }
    public int Qty { get; init; }
}