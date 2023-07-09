using EShop.Catalog.Core.Exceptions;

namespace EShop.Catalog.Core.Models;

public class CatalogItem : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; set; }
    public decimal? Price { get; private set; }
    public string? PictureUri { get; set; }
    public Guid CatalogTypeId { get; private set; }
    public Guid CatalogBrandId { get; private set; }
    public int AvailableQty { get; private set; } = 0;

    public CatalogItem(string? name, Guid catalogTypeId, Guid catalogBrandId)
    {
        UpdateName(name);
        UpdateBrand(catalogBrandId);
        UpdateType(catalogTypeId);
    }

    public CatalogItem(string? name, Guid catalogTypeId, Guid catalogBrandId, string description, decimal price, int availableQty, string pictureUri)
    {
        UpdateName(name);
        UpdateBrand(catalogBrandId);
        UpdateType(catalogTypeId);
        Description = description;
        UpdateAvailableQty(availableQty);
        UpdatePrice(price);
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

    public void UpdateAvailableQty(int availableQty)
    {
        if (availableQty < 0)
            throw new CatalogItemNegativeQtyException();

        AvailableQty = availableQty;
    }
}
