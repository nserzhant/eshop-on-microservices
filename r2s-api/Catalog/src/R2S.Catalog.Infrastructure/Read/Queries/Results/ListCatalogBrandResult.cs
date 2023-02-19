using R2S.Catalog.Infrastructure.Read.ReadModels;

namespace R2S.Catalog.Infrastructure.Read.Queries.Results;

public class ListCatalogBrandResult
{
    public IList<CatalogBrandReadModel> CatalogBrands { get; set; }
    public int TotalCount { get; set; }
}
