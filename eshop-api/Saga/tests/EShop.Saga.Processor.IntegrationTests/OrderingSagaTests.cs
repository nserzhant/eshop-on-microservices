using EShop.Basket.Core.Interfaces;
using EShop.Basket.Core.Models;
using EShop.Basket.Infrastructure;
using EShop.Basket.Integration.Events;
using EShop.Catalog.Core.Interfaces;
using EShop.Catalog.Core.Models;
using EShop.Catalog.Core.Services;
using EShop.Catalog.Infrastructure;
using EShop.Ordering.Core.Interfaces;
using EShop.Ordering.Infrastructure;
using EShop.Saga.Components.StateMachines;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EShop.Saga.Processor.IntegrationTests;

[TestFixture]
[Category("OrderingSaga")]
[Category("MicroservicesIntegration")]
public class OrderingSagaTests
{
    private ServiceProvider _serviceProvider = null;
    private IBasketRepository _basketRepository = null;
    private ICatalogItemRepository _catalogItemRepository = null;
    private IOrderRepository _orderRepository = null;
    private ITestHarness _harness;
    ISagaStateMachineTestHarness<OrderingStateMachine, OrderingStateMachineInstance> _sagaHarness;

    [SetUp]
    public async Task SetupAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.tests.json")
            .AddEnvironmentVariables()
            .Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLogging(logging => logging.AddConsole());
        serviceCollection
            .AddOrderingServices(configuration)
            .AddCatalogServices(configuration)
            .AddTestSagaServices(configuration);

        await serviceCollection.AddBasketSercicesAsync(configuration);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        clearDbs();

        _basketRepository = _serviceProvider.GetRequiredService<IBasketRepository>();
        _catalogItemRepository = _serviceProvider.GetRequiredService<ICatalogItemRepository>();
        _orderRepository = _serviceProvider.GetRequiredService<IOrderRepository>();
        _harness = _serviceProvider.GetTestHarness();
        _sagaHarness = _harness.GetSagaStateMachineHarness<OrderingStateMachine, OrderingStateMachineInstance>();

        await _harness.Start();
    }


    [TearDown]
    public async Task TearDownAsync()
    {
        await _harness.Stop();
        await _serviceProvider.DisposeAsync();
    }

    [Test]
    public async Task When_Ordering_Process_Completed_Then_Data_Should_Be_Consistent_Across_All_Services()
    {
        var customerId = Guid.NewGuid();
        var customerEmail = "test@example.com";
        var shippingAddress = "Test Shipping Address";
        var checkoutQty = 8;
        var itemName = "Test item";
        var qty = 22;
        var price = 12m;
        var description = "Test Item Description";
        var typeName = "Test item Type";
        var brandName = "Test item Brand";
        var pictureUrl = "item.png";
        var correlationId = Guid.NewGuid();
        // Setup test environment
        var catalogItem = await createCatalogItemAsync(itemName, brandName, typeName, price, description, qty, pictureUrl);
        var catalogItemId = catalogItem.Id;
        var basket = await createBasketAsync(customerId, catalogItemId, itemName, brandName, typeName, price, description, qty, pictureUrl);
        var checkoutItem = new BasketCheckoutItem(catalogItemId, itemName, checkoutQty, description, price, typeName, brandName, pictureUrl);
        var basketCheckoutEvent = new BasketCheckedOutEvent(correlationId, customerId, customerEmail, shippingAddress, basket.Id, [checkoutItem]);

        await _harness.Bus.Publish(basketCheckoutEvent);

        var finalizedSagaCorrelationId = await _sagaHarness.Exists(correlationId, x => x.Final, TimeSpan.FromSeconds(10));
        Assert.That(finalizedSagaCorrelationId.HasValue, Is.True);
        Assert.That(finalizedSagaCorrelationId.Value, Is.EqualTo(correlationId));
        var orderId = 1; // Order is an identity field.
        var basketActual = await _basketRepository.GetBasketAsync(customerId);
        var catalogActual = await _catalogItemRepository.GetCatalogItemAsync(catalogItem.Id);
        var orderActual = await _orderRepository.GetOrderByIdAsync(orderId);
        var orderItem = orderActual?.OrderItems?.FirstOrDefault();
        Assert.That(basketActual.Items.Count, Is.EqualTo(0));
        Assert.That(orderActual, Is.Not.Null);
        Assert.That(orderActual.CustomerId, Is.EqualTo(customerId));
        Assert.That(orderActual.CustomerEmail, Is.EqualTo(customerEmail));
        Assert.That(orderActual.ShippingAddress, Is.EqualTo(shippingAddress));
        Assert.That(orderItem, Is.Not.Null);
        Assert.That(orderItem.CatalogItemId, Is.EqualTo(catalogItemId));
        Assert.That(orderItem.Name, Is.EqualTo(itemName));
        Assert.That(orderItem.BrandName, Is.EqualTo(brandName));
        Assert.That(orderItem.TypeName, Is.EqualTo(typeName));
        Assert.That(orderItem.Description, Is.EqualTo(description));
        Assert.That(orderItem.Price, Is.EqualTo(price));
        Assert.That(orderItem.PictureUri, Is.EqualTo(pictureUrl));
        Assert.That(catalogActual, Is.Not.Null);
        Assert.That(catalogActual.AvailableQty, Is.EqualTo(14));
    }

    private async Task<CustomerBasket> createBasketAsync(Guid customerId, Guid catalogItemId, string itemName, string brandName, string typeName, decimal price, string description, int qty, string pictureUrl)
    {
        var basket = new CustomerBasket();

        basket.Items =
        [
            new BasketItem
            {
                CatalogItemId = catalogItemId,
                ItemName = itemName,
                BrandName = brandName,
                TypeName = typeName,
                Description = description,
                Qty = qty,
                PictureUri = pictureUrl,
                Price = price
            }
        ];

        var basketRepository = _serviceProvider.GetRequiredService<IBasketRepository>();

        await basketRepository.SaveBasketAsync(customerId, basket);

        return basket;
    }

    protected async Task<CatalogType> createCatalogTypeAsync(string catalogTypeName)
    {
        // Create scoped context to prevent any side effects

        using var scopedProvider = _serviceProvider.CreateScope();
        var catalogTypeRepository = scopedProvider.ServiceProvider.GetRequiredService<ICatalogTypeRepository>();
        var catalogTypeService = scopedProvider.ServiceProvider.GetRequiredService<ICatalogTypeService>();
        var catalogTypeToCreate = new CatalogType(catalogTypeName);

        await catalogTypeService.CreateCatalogTypeAsync(catalogTypeToCreate);

        var result = await catalogTypeRepository.GetCatalogTypeAsync(catalogTypeToCreate.Id);

        return result!;
    }

    protected async Task<CatalogBrand> createCatalogBrandAsync(string catalogBrandName)
    {
        using var scopedProvider = _serviceProvider.CreateScope();
        var catalogBrandRepository = scopedProvider.ServiceProvider.GetRequiredService<ICatalogBrandRepository>();
        var catalogBrandService = scopedProvider.ServiceProvider.GetRequiredService<ICatalogBrandService>();
        var catalogBrandToCreate = new CatalogBrand(catalogBrandName);

        await catalogBrandService.CreateCatalogBrandAsync(catalogBrandToCreate);

        var result = await catalogBrandRepository.GetCatalogBrandAsync(catalogBrandToCreate.Id);

        return result!;
    }

    protected async Task<CatalogItem> createCatalogItemAsync(string catalogItemName, string catalogBrandName, string catalogTypeName, decimal price, string description, int availableQty, string pictureUrl)
    {
        using var scopedProvider = _serviceProvider.CreateScope();
        var catalogItemService = scopedProvider.ServiceProvider.GetRequiredService<ICatalogItemService>();
        var catalogItemRepository = scopedProvider.ServiceProvider.GetRequiredService<ICatalogItemRepository>();
        var catalogBrandId = (await createCatalogBrandAsync(catalogBrandName)).Id;
        var catalogTypeId = (await createCatalogTypeAsync(catalogTypeName)).Id;
        var catalogItemToCreate = new CatalogItem(catalogItemName, catalogTypeId, catalogBrandId, description, price, availableQty, pictureUrl);

        await catalogItemService.CreateCatalogItemAsync(catalogItemToCreate);

        var result = await catalogItemRepository.GetCatalogItemAsync(catalogItemToCreate.Id);

        return result!;
    }

    private void clearDbs()
    {
        var orderDbContext = _serviceProvider.GetRequiredService<OrderingDbContext>();
        var catalogDbContext = _serviceProvider.GetRequiredService<CatalogDbContext>();
        var eShopSagaDbContext = _serviceProvider.GetRequiredService<DbContext>();

        orderDbContext.Database.EnsureDeleted();
        orderDbContext.Database.EnsureCreated();
        catalogDbContext.Database.EnsureDeleted();
        catalogDbContext.Database.EnsureCreated();
        eShopSagaDbContext.Database.EnsureDeleted();
        eShopSagaDbContext.Database.EnsureCreated();
    }
}