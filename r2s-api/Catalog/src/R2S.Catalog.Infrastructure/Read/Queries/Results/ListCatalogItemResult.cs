using R2S.Catalog.Infrastructure.Read.ReadModels;

namespace R2S.Catalog.Infrastructure.Read.Queries.Results;

public class ListCatalogItemResult
{
    public IList<CatalogItemReadModel> CatalogItems { get; set; }
    public int TotalCount { get; set; }
}
