namespace R2S.Catalog.Infrastructure.Read.Queries;

public class ListCatalogBrandQuery
{
    public OrderByDirections OrderByDirection { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}
