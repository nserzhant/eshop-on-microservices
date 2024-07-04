
namespace EShop.Basket.Core.Models;

public class BasketItem
{
    public Guid CatalogItemId { get; set; }
    public required string ItemName { get; set; }
    public required string BrandName { get; set; }
    public required string TypeName { get; set; }
    public int Qty { get; set; }
    public decimal Price { get; set; }
    public string? PictureUri { get; set; }
    public string? Description { get; set; }
}