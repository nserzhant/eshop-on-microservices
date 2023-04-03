using EShop.Catalog.Infrastructure.Read.ReadModels;

namespace EShop.Catalog.Infrastructure.Read.Queries.Results;

public class ListCatalogBrandResult
{
    public IList<CatalogBrandReadModel> CatalogBrands { get; set; }
    public int TotalCount { get; set; }
}
