using EShop.Basket.Core.Models;

namespace EShop.Basket.Core.Interfaces;
public interface IBasketRepository
{
    Task SaveBasketAsync(Guid customerId, CustomerBasket basket);
    Task DeleteBasketAsync(Guid customerId);
    Task<CustomerBasket> GetBasketAsync(Guid customerId);
}
