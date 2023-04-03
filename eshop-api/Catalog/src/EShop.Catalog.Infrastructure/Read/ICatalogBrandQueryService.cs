using EShop.Catalog.Infrastructure.Read.Queries;
using EShop.Catalog.Infrastructure.Read.Queries.Results;
using EShop.Catalog.Infrastructure.Read.ReadModels;

namespace EShop.Catalog.Infrastructure.Read;

public interface ICatalogBrandQueryService
{
    Task<CatalogBrandReadModel?> GetById(Guid id);
    Task<ListCatalogBrandResult> GetCatalogBrands(ListCatalogBrandQuery listCatalogBrandQuery);
}
