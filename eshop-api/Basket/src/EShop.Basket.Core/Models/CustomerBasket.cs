namespace EShop.Basket.Core.Models;
public class CustomerBasket
{
    public Guid Id { get; init; }
    public List<BasketItem> Items { get; set; } = new List<BasketItem>();

    public CustomerBasket() 
    { 
        Id = Guid.NewGuid();
    }
}
