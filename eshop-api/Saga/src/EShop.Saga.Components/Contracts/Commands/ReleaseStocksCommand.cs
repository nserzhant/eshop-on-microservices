namespace EShop.Catalog.Integration.Commands;

public record ReleaseStockItem(Guid CatalogItemId, int Qty);

public record ReleaseStocksCommand(Guid CorrelationId, List<ReleaseStockItem> Items);

