using EShop.Catalog.Infrastructure.Read.Queries;
using EShop.Catalog.Infrastructure.Read.Queries.Results;
using EShop.Catalog.Infrastructure.Read.ReadModels;

namespace EShop.Catalog.Infrastructure.Read;

public interface ICatalogItemQueryService
{
    Task<CatalogItemReadModel?> GetById(Guid id);
    Task<ListCatalogItemResult> GetCatalogItems(ListCatalogItemQuery listCatalogItemQuery);
}
