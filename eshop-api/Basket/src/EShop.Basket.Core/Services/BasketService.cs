using EShop.Basket.Core.Interfaces;

namespace EShop.Basket.Core.Services;

public interface IBasketService
{
    Task CheckOutAsync(Guid customerId);
}

public class BasketService : IBasketService
{

    private readonly IBasketRepository _repository;

    public BasketService(IBasketRepository repository)
    {
        _repository = repository;
    }

    public async Task CheckOutAsync(Guid customerId)
    {
        await _repository.DeleteBasketAsync(customerId);
    }
}