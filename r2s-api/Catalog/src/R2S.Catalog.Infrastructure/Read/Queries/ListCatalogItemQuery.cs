namespace R2S.Catalog.Infrastructure.Read.Queries;

public enum ListCatalogItemOrderBy
{
    Name,
    Brand,
    Type,
    Price
}

public class ListCatalogItemQuery
{
    public ListCatalogItemOrderBy OrderBy { get; set; } = ListCatalogItemOrderBy.Name;
    public OrderByDirections OrderByDirection { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string? NameFilter { get; set; }
    public string? BrandFilter { get; set; }
    public string? TypeFilter { get; set; }
}
