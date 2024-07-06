using EShop.Ordering.Core.Interfaces;
using EShop.Ordering.Core.Models;
using EShop.Ordering.Infrastructure.Read;
using EShop.Ordering.Infrastructure.Read.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Ordering.Infrastructure.IntegrationTests;

[TestFixture]
public class OrderingIntegrationTests : BaseOrderingIntegationTests
{
    private IOrderRepository _orderRepository;
    private IOrderQueryService _orderQueryService;

    [SetUp]
    public override async Task SetupAsync()
    {
        await base.SetupAsync();

        _orderRepository = serviceProvider.GetRequiredService<IOrderRepository>();
        _orderQueryService = serviceProvider.GetRequiredService<IOrderQueryService>();
    }

    [Test]
    [Category("Order Repository")]
    [Category("Order Query Service")]
    public async Task When_Save_Order_Then_It_Could_Be_Retreived_By_Id()
    {
        var orderDate = DateTime.UtcNow;
        var customerId = Guid.NewGuid();
        var customerEmail = "testemail@example.com";
        var shippingAddress = "Test User Address";
        var catalogItemId = Guid.NewGuid(); ;
        var name = "Test Item";
        var description = "Test Item Description";
        var typeName = "Type Name";
        var brandName = "Brand Name";
        var qty = 25;
        var price = 45;
        var pictureUri = @"\picture.png";
        List<OrderItem> orderItems =
            [
                new OrderItem(catalogItemId, name, description, price, typeName, brandName, qty, pictureUri)
            ];
        var order = new Order(orderDate, customerId, customerEmail, shippingAddress, orderItems);

        await _orderRepository.CreateOrder(order);
        await _orderRepository.SaveChangesAsync();

        var savedOrder = await _orderRepository.GetOrderByIdAsync(order.Id);
        var orderReadModel = await _orderQueryService.GetByIdAsync(order.Id);
        var savedOrderItem = savedOrder?.OrderItems?.FirstOrDefault();
        var orderItemReadModel = orderReadModel?.OrderItems?.FirstOrDefault();
        Assert.That(savedOrder, Is.Not.Null);
        Assert.That(savedOrder.Id, Is.EqualTo(order.Id));
        Assert.That(savedOrder.OrderDate, Is.EqualTo(orderDate));
        Assert.That(savedOrder.CustomerId, Is.EqualTo(customerId));
        Assert.That(savedOrder.CustomerEmail, Is.EqualTo(customerEmail));
        Assert.That(savedOrder.ShippingAddress, Is.EqualTo(shippingAddress));
        Assert.That(savedOrderItem, Is.Not.Null);
        Assert.That(savedOrderItem.CatalogItemId, Is.EqualTo(catalogItemId));
        Assert.That(savedOrderItem.Name, Is.EqualTo(name));
        Assert.That(savedOrderItem.Description, Is.EqualTo(description));
        Assert.That(savedOrderItem.TypeName, Is.EqualTo(typeName));
        Assert.That(savedOrderItem.BrandName, Is.EqualTo(brandName));
        Assert.That(savedOrderItem.Qty, Is.EqualTo(qty));
        Assert.That(savedOrderItem.PictureUri, Is.EqualTo(pictureUri));
        Assert.That(orderReadModel, Is.Not.Null);
        Assert.That(orderReadModel.Id, Is.EqualTo(order.Id));
        Assert.That(orderReadModel.OrderDate, Is.EqualTo(orderDate));
        Assert.That(orderReadModel.CustomerId, Is.EqualTo(customerId));
        Assert.That(orderReadModel.ShippingAddress, Is.EqualTo(shippingAddress));
        Assert.That(orderItemReadModel, Is.Not.Null);
        Assert.That(orderItemReadModel.Name, Is.EqualTo(name));
        Assert.That(orderItemReadModel.Description, Is.EqualTo(description));
        Assert.That(orderItemReadModel.TypeName, Is.EqualTo(typeName));
        Assert.That(orderItemReadModel.BrandName, Is.EqualTo(brandName));
        Assert.That(orderItemReadModel.Qty, Is.EqualTo(qty));
        Assert.That(orderItemReadModel.Price, Is.EqualTo(price));
        Assert.That(orderItemReadModel.PictureUri, Is.EqualTo(pictureUri));
    }

    [Test]
    [Category("Order Query Service")]
    public async Task When_Orders_Exists_Then_They_Could_Be_Queried_By_Customer_Id()
    {
        var customerId = Guid.NewGuid();
        var shippingAddress = "Address 1";
        await createOrderAsync(customerId, shippingAddress);
        await createOrderAsync(Guid.NewGuid(), Guid.NewGuid().ToString());

        var ordersFound = await _orderQueryService.ListOrdersAsync(new ListOrderQuery
        {
            CustomerId = customerId,
            PageSize = 1
        });

        var order = ordersFound?.Orders?.FirstOrDefault();
        Assert.That(order, Is.Not.Null);
        Assert.That(order.ShippingAddress, Is.EqualTo(shippingAddress));
    }

    [Test]
    [Category("Order Query Service")]
    [TestCase(ListOrderOrderBy.Id, OrderByDirections.ASC, "FIRST ITEM")]
    [TestCase(ListOrderOrderBy.Id, OrderByDirections.DESC, "FOURTH ITEM")]
    [TestCase(ListOrderOrderBy.OrderDate, OrderByDirections.ASC, "SECOND ITEM")]
    [TestCase(ListOrderOrderBy.OrderDate, OrderByDirections.DESC, "THIRD ITEM")]
    [TestCase(ListOrderOrderBy.CustomerEmail, OrderByDirections.ASC, "THIRD ITEM")]
    [TestCase(ListOrderOrderBy.CustomerEmail, OrderByDirections.DESC, "FIRST ITEM")]
    [TestCase(ListOrderOrderBy.OrderStatus, OrderByDirections.ASC, "FOURTH ITEM")]
    [TestCase(ListOrderOrderBy.OrderStatus, OrderByDirections.DESC, "SECOND ITEM")]
    public async Task When_Orders_Exists_Then_They_Could_Be_Ordered_By_Date_Status_Email_Id(ListOrderOrderBy orderBy, OrderByDirections orderByDirections, string expectedOrderShippingAddress)
    {
        await createOrderAsync(DateTime.Now.AddDays(2), new Guid("0098DF40-EFA8-44BC-AB74-36CA0CD98398"), "test email 4", Read.ReadModels.OrderStatus.Shipped, "FIRST ITEM");
        await createOrderAsync(DateTime.Now.AddDays(1), new Guid("1098DF40-EFA8-44BC-AB74-36CA0CD98398"), "test email 2", Read.ReadModels.OrderStatus.Returned, "SECOND ITEM");
        await createOrderAsync(DateTime.Now.AddDays(4), new Guid("2098DF40-EFA8-44BC-AB74-36CA0CD98398"), "test email 1", Read.ReadModels.OrderStatus.Delivered, "THIRD ITEM");
        await createOrderAsync(DateTime.Now.AddDays(3), new Guid("3098DF40-EFA8-44BC-AB74-36CA0CD98398"), "test email 3", Read.ReadModels.OrderStatus.Created, "FOURTH ITEM");

        var result = await _orderQueryService.ListOrdersAsync(new ListOrderQuery
        {
            OrderBy = orderBy,
            OrderByDirection = orderByDirections,
            PageIndex = 0,
            PageSize = 3
        });

        var firstOrder = result?.Orders?.FirstOrDefault();
        Assert.That(result, Is.Not.Null);
        Assert.That(firstOrder, Is.Not.Null);
        Assert.That(result.TotalCount, Is.EqualTo(4));
        Assert.That(result.Orders.Count, Is.EqualTo(3));
        Assert.That(firstOrder.ShippingAddress, Is.EqualTo(expectedOrderShippingAddress));
    }

    private async Task createOrderAsync(DateTime orderDate, Guid customerId, string customerEmail, Read.ReadModels.OrderStatus orderStatus, string shippingAddress)
    {
        var catalogItemId = Guid.NewGuid();
        var name = $"Test Item {customerId}";
        var description = $"Test Item Description {customerId}";
        var typeName = "Type Name 1";
        var brandName = "Brand Name 2";
        var qty = 125;
        var price = 98m;
        var pictureUri = @"\image.png";
        List<OrderItem> orderItems =
            [
                new OrderItem(catalogItemId, name, description, price, typeName, brandName, qty, pictureUri)
            ];
        var order = new Order(orderDate, customerId, customerEmail, shippingAddress, orderItems);

        switch (orderStatus)
        {
            case Read.ReadModels.OrderStatus.Shipped:
                order.Ship();
                break;
            case Read.ReadModels.OrderStatus.Delivered:
                order.Ship();
                order.Deliver();
                break;
            case Read.ReadModels.OrderStatus.Returned:
                order.Ship();
                order.Deliver();
                order.Return();
                break;
            default: break;
        }

        await _orderRepository.CreateOrder(order);
        await _orderRepository.SaveChangesAsync();
    }

    private async Task createOrderAsync(Guid customerId, string shippingAddress)
    {
        await createOrderAsync(DateTime.UtcNow, customerId, $"testemail{customerId}@example.com", Read.ReadModels.OrderStatus.Created, shippingAddress);
    }
}