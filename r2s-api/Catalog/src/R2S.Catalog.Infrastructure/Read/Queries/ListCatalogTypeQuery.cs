namespace R2S.Catalog.Infrastructure.Read.Queries;

public class ListCatalogTypeQuery
{
    public OrderByDirections OrderByDirection { get; set; }
    public int PageIndex { get; set; } = 0; 
    public int PageSize { get; set; } = 0;
}
