using Microsoft.EntityFrameworkCore;
using EShop.Catalog.Core.Models;
using EShop.Catalog.Infrastructure.Read.Queries;
using EShop.Catalog.Infrastructure.Read.Queries.Results;
using EShop.Catalog.Infrastructure.Read.ReadModels;
using System.Linq.Dynamic.Core;

namespace EShop.Catalog.Infrastructure.Read;

public class CatalogItemQueryService : ICatalogItemQueryService
{
    private readonly CatalogReadDbContext _catalogReadDbContext;

    public CatalogItemQueryService(CatalogReadDbContext catalogReadDbContext)
    {
        _catalogReadDbContext = catalogReadDbContext;
    }

    public async Task<CatalogItemReadModel?> GetById(Guid id)
    {
        var catalogItem = await _catalogReadDbContext
            .CatalogItems
            .FirstOrDefaultAsync(b => b.Id == id);

        return catalogItem;
    }

    public async Task<ListCatalogItemResult> GetCatalogItems(ListCatalogItemQuery listCatalogItemQuery)
    {
        var count = _catalogReadDbContext.CatalogItems
            .Where(ci => listCatalogItemQuery.NameFilter == null
                || ci.Name.Contains(listCatalogItemQuery.NameFilter))
            .Where(ci => listCatalogItemQuery.BrandFilter == null
                || ci.CatalogBrand.Brand.Contains(listCatalogItemQuery.BrandFilter))
            .Where(ci => listCatalogItemQuery.TypeFilter == null
                || ci.CatalogType.Type.Contains(listCatalogItemQuery.TypeFilter)).Count();

        var orderByTableNamePrefix = listCatalogItemQuery.OrderBy switch
        {
            ListCatalogItemOrderBy.Brand => $"{nameof(CatalogBrand)}.",
            ListCatalogItemOrderBy.Type => $"{nameof(CatalogType)}.",
            _ => ""
        };

        var orderByExpression = $"{orderByTableNamePrefix}{listCatalogItemQuery.OrderBy} {listCatalogItemQuery.OrderByDirection}";

        var catalogItems = await _catalogReadDbContext.CatalogItems
            .Where(ci => listCatalogItemQuery.NameFilter == null
                || ci.Name.Contains(listCatalogItemQuery.NameFilter))
            .Where(ci => listCatalogItemQuery.BrandFilter == null
                || ci.CatalogBrand.Brand.Contains(listCatalogItemQuery.BrandFilter))
            .Where(ci => listCatalogItemQuery.TypeFilter == null
                || ci.CatalogType.Type.Contains(listCatalogItemQuery.TypeFilter))
            .OrderBy(orderByExpression)
            .Skip(listCatalogItemQuery.PageIndex * listCatalogItemQuery.PageSize)
            .Take(listCatalogItemQuery.PageSize)
            .ToListAsync();

        var result = new ListCatalogItemResult()
        {
            TotalCount = count,
            CatalogItems = catalogItems
        };

        return result;
    }
}
