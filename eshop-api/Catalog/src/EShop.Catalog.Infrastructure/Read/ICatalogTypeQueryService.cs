using EShop.Catalog.Infrastructure.Read.Queries.Results;
using EShop.Catalog.Infrastructure.Read.Queries;
using EShop.Catalog.Infrastructure.Read.ReadModels;

namespace EShop.Catalog.Infrastructure.Read;

public interface ICatalogTypeQueryService
{
    Task<CatalogTypeReadModel?> GetById(Guid id);
    Task<ListCatalogTypeResult> GetCatalogTypes(ListCatalogTypeQuery listCatalogTypeQuery);
}
