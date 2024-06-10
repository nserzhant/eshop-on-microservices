namespace EShop.Ordering.Infrastructure.Read.Queries;

public enum ListOrderOrderBy
{
    Id,
    OrderStatus,
    OrderDate,
    CustomerEmail
}

public class ListOrderQuery
{
    public ListOrderOrderBy OrderBy { get; set; } = ListOrderOrderBy.Id;
    public OrderByDirections OrderByDirection { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 1;
    public Guid? CustomerId { get; set; } = null;
} 