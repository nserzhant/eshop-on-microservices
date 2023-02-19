namespace R2S.Catalog.Infrastructure.Read.ReadModels;

public class CatalogItemReadModel
{
    public byte[] Ts { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? PictureUri { get; set; }
    public CatalogTypeReadModel CatalogType { get; set; }
    public CatalogBrandReadModel CatalogBrand { get; set; }
}
