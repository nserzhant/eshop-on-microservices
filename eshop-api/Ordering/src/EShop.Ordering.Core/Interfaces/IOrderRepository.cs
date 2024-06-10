using EShop.Ordering.Core.Models;

namespace EShop.Ordering.Core.Interfaces;
public interface IOrderRepository
{
    Task CreateOrder(Order order);
    void UpdateOrder(Order order);
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task SaveChangesAsync();
}
