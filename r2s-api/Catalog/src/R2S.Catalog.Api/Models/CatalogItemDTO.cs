namespace R2S.Catalog.Api.Models;

public class CatalogItemDTO
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? PictureUri { get; set; }
    public Guid TypeId { get; set; }
    public Guid BrandId { get; set; }
    public byte[]? Ts { get; set; }
    public int AvailableQty { get; set; }
}
