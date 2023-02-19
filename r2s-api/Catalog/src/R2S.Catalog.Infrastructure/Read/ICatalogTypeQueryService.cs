using R2S.Catalog.Infrastructure.Read.Queries.Results;
using R2S.Catalog.Infrastructure.Read.Queries;
using R2S.Catalog.Infrastructure.Read.ReadModels;

namespace R2S.Catalog.Infrastructure.Read;

public interface ICatalogTypeQueryService
{
    Task<CatalogTypeReadModel?> GetById(Guid id);
    Task<ListCatalogTypeResult> GetCatalogTypes(ListCatalogTypeQuery listCatalogTypeQuery);
}
