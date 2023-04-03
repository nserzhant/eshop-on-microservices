using EShop.Catalog.Infrastructure.Read.ReadModels;

namespace EShop.Catalog.Infrastructure.Read.Queries.Results;

public class ListCatalogItemResult
{
    public IList<CatalogItemReadModel> CatalogItems { get; set; }
    public int TotalCount { get; set; }
}
