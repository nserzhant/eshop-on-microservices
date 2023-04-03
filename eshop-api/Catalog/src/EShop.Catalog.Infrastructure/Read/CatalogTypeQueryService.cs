using Microsoft.EntityFrameworkCore;
using EShop.Catalog.Infrastructure.Read.Queries;
using EShop.Catalog.Infrastructure.Read.Queries.Results;
using EShop.Catalog.Infrastructure.Read.ReadModels;
using System.Linq.Dynamic.Core;

namespace EShop.Catalog.Infrastructure.Read;

public class CatalogTypeQueryService : ICatalogTypeQueryService
{
    private readonly CatalogReadDbContext _catalogReadDbContext;

    public CatalogTypeQueryService(CatalogReadDbContext catalogReadDbContext)
    {
        _catalogReadDbContext = catalogReadDbContext;
    }

    public async Task<CatalogTypeReadModel?> GetById(Guid id)
    {
        var catalogType = await _catalogReadDbContext.CatalogTypes
            .FirstOrDefaultAsync(ct => ct.Id == id);

        return catalogType;
    }

    public async Task<ListCatalogTypeResult> GetCatalogTypes(ListCatalogTypeQuery listCatalogTypeQuery)
    {
        var count = _catalogReadDbContext.CatalogTypes.Count();
        var orderByExpression = $"{nameof(CatalogTypeReadModel.Type)} {listCatalogTypeQuery.OrderByDirection}";
        IQueryable<CatalogTypeReadModel> catalogTypesQueryable = _catalogReadDbContext.CatalogTypes.OrderBy(orderByExpression);

        if (listCatalogTypeQuery.PageSize > 0)
        {
            catalogTypesQueryable = catalogTypesQueryable
                .Skip(listCatalogTypeQuery.PageIndex * listCatalogTypeQuery.PageSize)
                .Take(listCatalogTypeQuery.PageSize);
        }

        var catalogTypes = await catalogTypesQueryable
            .OrderBy(orderByExpression)
            .ToListAsync();

        var result = new ListCatalogTypeResult()
        {
            TotalCount = count,
            CatalogTypes = catalogTypes
        };

        return result;
    }
}
