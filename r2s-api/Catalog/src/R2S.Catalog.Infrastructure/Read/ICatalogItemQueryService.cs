using R2S.Catalog.Infrastructure.Read.Queries;
using R2S.Catalog.Infrastructure.Read.Queries.Results;
using R2S.Catalog.Infrastructure.Read.ReadModels;

namespace R2S.Catalog.Infrastructure.Read;

public interface ICatalogItemQueryService
{
    Task<CatalogItemReadModel?> GetById(Guid id);
    Task<ListCatalogItemResult> GetCatalogItems(ListCatalogItemQuery listCatalogItemQuery);
}
