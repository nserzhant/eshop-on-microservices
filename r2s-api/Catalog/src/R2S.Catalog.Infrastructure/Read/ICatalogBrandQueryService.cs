using R2S.Catalog.Infrastructure.Read.Queries;
using R2S.Catalog.Infrastructure.Read.Queries.Results;
using R2S.Catalog.Infrastructure.Read.ReadModels;

namespace R2S.Catalog.Infrastructure.Read;

public interface ICatalogBrandQueryService
{
    Task<CatalogBrandReadModel?> GetById(Guid id);
    Task<ListCatalogBrandResult> GetCatalogBrands(ListCatalogBrandQuery listCatalogBrandQuery);
}
