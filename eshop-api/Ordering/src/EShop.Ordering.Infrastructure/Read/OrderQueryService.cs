using EShop.Ordering.Infrastructure.Read.Queries;
using EShop.Ordering.Infrastructure.Read.ReadModels;
using EShop.Ordering.Infrastructure.Read.Results;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace EShop.Ordering.Infrastructure.Read;
public interface IOrderQueryService
{
    Task<OrderReadModel?> GetByIdAsync(int id);
    Task<ListOrderResult> ListOrdersAsync(ListOrderQuery query);
}

public class OrderQueryService : IOrderQueryService
{
    private readonly OrderingReadDbContext _dbContext;

    public OrderQueryService(OrderingReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderReadModel?> GetByIdAsync(int id)
    {
        var result = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == id);

        return result;
    }

    public async Task<ListOrderResult> ListOrdersAsync(ListOrderQuery query)
    {
        var count = _dbContext.Orders
            .Where(order => query.CustomerId == null || order.CustomerId == query.CustomerId)
            .Count();

        var orderByExpression = $"{query.OrderBy} {query.OrderByDirection}";

        var orders = await _dbContext.Orders
            .Where(order => query.CustomerId == null || order.CustomerId == query.CustomerId)
            .OrderBy(orderByExpression)
            .Skip(query.PageIndex * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var result = new ListOrderResult()
        {
            Orders = orders,
            TotalCount = count
        };

        return result;
    }
}