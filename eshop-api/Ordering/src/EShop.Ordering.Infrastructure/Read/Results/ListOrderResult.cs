using EShop.Ordering.Infrastructure.Read.ReadModels;

namespace EShop.Ordering.Infrastructure.Read.Results;
public class ListOrderResult
{
    public IList<OrderReadModel> Orders { get; set; }
    public int TotalCount { get; set; }
}
