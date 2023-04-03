using Microsoft.EntityFrameworkCore;
using EShop.Catalog.Infrastructure.Read.Queries;
using EShop.Catalog.Infrastructure.Read.Queries.Results;
using EShop.Catalog.Infrastructure.Read.ReadModels;
using System.Linq.Dynamic.Core;

namespace EShop.Catalog.Infrastructure.Read;

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
        IQueryable<CatalogBrandReadModel> catalogBrandsQueryable = _catalogReadDbContext.CatalogBrands.OrderBy(orderByExpression);

        if(listCatalogBrandQuery.PageSize > 0)
        {
            catalogBrandsQueryable = catalogBrandsQueryable
                .Skip(listCatalogBrandQuery.PageIndex * listCatalogBrandQuery.PageSize)
                .Take(listCatalogBrandQuery.PageSize);
        }

        var catalogBrands = await catalogBrandsQueryable
            .OrderBy(orderByExpression)
            .ToListAsync();
       
        var result = new ListCatalogBrandResult()
        {
            TotalCount = count,
            CatalogBrands = catalogBrands
        };

        return result;
    }
}
