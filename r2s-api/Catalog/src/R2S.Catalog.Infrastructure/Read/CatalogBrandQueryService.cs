using Microsoft.EntityFrameworkCore;
using R2S.Catalog.Infrastructure.Read.Queries;
using R2S.Catalog.Infrastructure.Read.Queries.Results;
using R2S.Catalog.Infrastructure.Read.ReadModels;
using System.Linq.Dynamic.Core;

namespace R2S.Catalog.Infrastructure.Read;

public class CatalogBrandQueryService : ICatalogBrandQueryService
{
    private readonly CatalogReadDbContext _catalogReadDbContext;

    public CatalogBrandQueryService(CatalogReadDbContext catalogReadDbContext)
    {
        _catalogReadDbContext = catalogReadDbContext;
    }

    public async Task<CatalogBrandReadModel?> GetById(Guid id)
    {
        var catalogBrand = await _catalogReadDbContext.CatalogBrands
            .FirstOrDefaultAsync(b => b.Id == id);

        return catalogBrand;
    }

    public async Task<ListCatalogBrandResult> GetCatalogBrands(ListCatalogBrandQuery listCatalogBrandQuery)
    {
        var count = _catalogReadDbContext.CatalogBrands.Count();
        var orderByExpression = $"{nameof(CatalogBrandReadModel.Brand)} {listCatalogBrandQuery.OrderByDirection}";
        
        var catalogBrands = await _catalogReadDbContext.CatalogBrands
            .OrderBy(orderByExpression)
            .Skip(listCatalogBrandQuery.PageIndex * listCatalogBrandQuery.PageSize)
            .Take(listCatalogBrandQuery.PageSize)
            .ToListAsync();
       
        var result = new ListCatalogBrandResult()
        {
            TotalCount = count,
            CatalogBrands = catalogBrands
        };

        return result;
    }
}
