namespace EShop.Catalog.Infrastructure.Read.Queries;

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
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 0;
    public string? NameFilter { get; set; } = null;
    public string? BrandFilter { get; set; } = null;
    public string? TypeFilter { get; set; } = null;
}
