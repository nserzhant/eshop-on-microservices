namespace EShop.Catalog.Infrastructure.Read.Queries;

public class ListCatalogBrandQuery
{
    public OrderByDirections OrderByDirection { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 0;
}
