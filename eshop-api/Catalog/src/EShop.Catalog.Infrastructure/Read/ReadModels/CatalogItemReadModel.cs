namespace EShop.Catalog.Infrastructure.Read.ReadModels;

public class CatalogItemReadModel
{
    public byte[] Ts { get; init; }
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string? Description { get; init; }
    public decimal? Price { get; init; }
    public string? PictureUri { get; init; }
    public Guid CatalogTypeId { get; init; }
    public Guid CatalogBrandId { get; init; }
    public CatalogTypeReadModel CatalogType { get; init; }
    public CatalogBrandReadModel CatalogBrand { get; init; }
    public int AvailableQty { get; init; }
}
