namespace EShop.Ordering.Core.Models;
public class OrderItem : BaseEntity
{
    public Guid CatalogItemId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; set; }
    public decimal Price { get; private set; }
    public string TypeName { get; private set; }
    public string BrandName { get; private set; }
    public string PictureUri { get; private set; }
    public int Qty { get; private set; }
    private OrderItem() { }

    public OrderItem(Guid catalogItemId, string name, string? description, decimal price, string typeName, string brandName, int qty, string pictureUri)
    {
        CatalogItemId = catalogItemId;
        Name = name;
        Description = description;
        Price = price;
        TypeName = typeName;
        BrandName = brandName;
        PictureUri = pictureUri;
        Qty = qty;
    }
}