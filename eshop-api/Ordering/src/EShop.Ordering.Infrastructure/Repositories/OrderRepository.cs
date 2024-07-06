using EShop.Ordering.Core.Interfaces;
using EShop.Ordering.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EShop.Ordering.Infrastructure.Repositories;
public class OrderRepository : IOrderRepository
{
    private readonly OrderingDbContext _orderingDbContext;

    public OrderRepository(OrderingDbContext orderingDbContext)
    {
        this._orderingDbContext = orderingDbContext;
    }

    public async Task CreateOrder(Order order)
    {
        await _orderingDbContext.AddAsync(order);
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        var order = await _orderingDbContext.Orders
            .AsNoTracking()
            .Include(o=> o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        return order;
    }

    public void UpdateOrder(Order order)
    {
        _orderingDbContext.Orders.Update(order);
    }

    public async Task SaveChangesAsync()
    {
        await _orderingDbContext.SaveChangesAsync();
    }
}
