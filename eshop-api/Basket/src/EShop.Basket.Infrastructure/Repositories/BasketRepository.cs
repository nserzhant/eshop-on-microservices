using EShop.Basket.Core.Interfaces;
using EShop.Basket.Core.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace EShop.Basket.Infrastructure.Repositories;
public class BasketRepository : IBasketRepository
{
    private readonly IDatabase _database;

    public BasketRepository(IDatabase database)
    {
        _database = database;
    }

    public async Task<CustomerBasket> GetBasketAsync(Guid customerId)
    {
        var data = await _database.StringGetAsync(customerId.ToString());

        if (data.IsNullOrEmpty)
            return new CustomerBasket();

        var result = JsonSerializer.Deserialize<CustomerBasket>(data.ToString(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        return result;
    }

    public async Task SaveBasketAsync(Guid customerId, CustomerBasket basket)
    {
        var content = JsonSerializer.Serialize(basket);
        await _database.StringSetAsync(customerId.ToString(), content);
    }

    public async Task DeleteBasketAsync(Guid customerId)
    {
        await _database.KeyDeleteAsync(customerId.ToString());
    }
}
