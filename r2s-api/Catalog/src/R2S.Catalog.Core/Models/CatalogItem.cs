using R2S.Catalog.Core.Exceptions;

namespace R2S.Catalog.Core.Models;

public class CatalogItem : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; set; }
    public decimal? Price { get; private set; }
    public string? PictureUri { get; set; }
    public Guid CatalogTypeId { get; set; }
    public Guid CatalogBrandId { get; set; }

    public CatalogItem(string? name, string? description, decimal? price, string? pictureUri, Guid catalogTypeId, Guid catalogBrandId)
    {
        UpdateName(name);
        UpdatePrice(price);
        UpdateBrand(catalogBrandId);
        UpdateType(catalogTypeId);

        Description = description;
        PictureUri = pictureUri;
    }

    public void UpdateName(string? name)
    {
        if (string.IsNullOrEmpty(name))
            throw new CatalogItemNameIsNullOrEmptyException();

        Name = name;
    }

    public void UpdateTs(byte[]? ts)
    {
        if (ts == null)
            throw new CatalogItemTsIsNullException();

        Ts = ts;
    }

    public void UpdatePrice(decimal? price)
    {
        if (price < 0)
            throw new CatalogItemNegativePriceException();

        Price = price;
    }

    public void UpdateType(Guid catalogTypeId)
    {
        if (catalogTypeId == Guid.Empty)
            throw new CatalogItemTypeIsEmptyException();

        CatalogTypeId = catalogTypeId;
    }

    public void UpdateBrand(Guid catalogBrandId)
    {
        if(catalogBrandId == Guid.Empty)
            throw new CatalogItemBrandIsEmptyException();

        CatalogBrandId = catalogBrandId;
    }
}
