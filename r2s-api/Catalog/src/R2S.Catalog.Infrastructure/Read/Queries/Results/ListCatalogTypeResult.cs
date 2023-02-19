using R2S.Catalog.Infrastructure.Read.ReadModels;

namespace R2S.Catalog.Infrastructure.Read.Queries.Results;

public class ListCatalogTypeResult
{
    public IList<CatalogTypeReadModel> CatalogTypes { get; set; }
    public int TotalCount { get; set; }
}
