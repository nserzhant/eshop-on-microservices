namespace EShop.Catalog.Integration.Commands;

public record ReserveStockItem(Guid CatalogItemId, int Qty);

public record ReserveStocksCommand(Guid CorrelationId, List<ReserveStockItem> Items);

