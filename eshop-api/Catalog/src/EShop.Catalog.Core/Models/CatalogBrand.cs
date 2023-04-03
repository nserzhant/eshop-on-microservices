using EShop.Catalog.Core.Exceptions;

namespace EShop.Catalog.Core.Models;

public class CatalogBrand : BaseEntity
{
    public string Brand { get; private set; }

    public CatalogBrand(string? brand) => UpdateBrand(brand);

    public void UpdateBrand(string? brand)
    {
        if (string.IsNullOrEmpty(brand))
            throw new CatalogBrandIsNullOrEmptyException();

        Brand = brand;
    }

    public void UpdateTs(byte[]? ts)
    {
        if (ts == null)
            throw new CatalogBrandTsIsNullException();

        Ts = ts;
    }
}