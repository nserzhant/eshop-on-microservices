using EShop.Catalog.Infrastructure.Read.ReadModels;

namespace EShop.Catalog.Infrastructure.Read.Queries.Results;

public class ListCatalogTypeResult
{
    public IList<CatalogTypeReadModel> CatalogTypes { get; set; }
    public int TotalCount { get; set; }
}
