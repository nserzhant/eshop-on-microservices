using EShop.Ordering.Core.Exceptions;
using EShop.Ordering.Core.Models;

namespace EShop.Ordering.Core.UnitTests;

[TestFixture]
[Category("Order")]
public class OrderTests
{
    [Test]
    public void When_Create_Order_Then_Status_Should_Be_New()
    {
        var catalogItemId = Guid.NewGuid();
        var name = "Test Item";
        var description = "Test Item Description";
        var typeName = "Type Name";
        var brandName = "Brand Name";
        var qty = 25;
        var price = 12.3m;
        var pictureUri = @"\picture.url";
        List<OrderItem> orderItems =
            [
                new OrderItem(catalogItemId, name, description, price, typeName, brandName, qty, pictureUri)
            ];
        var orderDate = DateTime.UtcNow;
        var customerId = Guid.NewGuid();
        var customerEmail = "sample@example.com";
        var shippingAddress = "test address";

        var order = new Order(orderDate, customerId, customerEmail, shippingAddress, orderItems);

        var orderItem = order.OrderItems.First();
        Assert.That(order.OrderStatus, Is.EqualTo(OrderStatus.Created));
        Assert.That(order.OrderDate, Is.EqualTo(orderDate));
        Assert.That(order.CustomerId, Is.EqualTo(customerId));
        Assert.That(order.CustomerEmail, Is.EqualTo(customerEmail));
        Assert.That(order.ShippingAddress, Is.EqualTo(shippingAddress));
        Assert.That(orderItem.CatalogItemId, Is.EqualTo(catalogItemId));
        Assert.That(orderItem.Name, Is.EqualTo(name));
        Assert.That(orderItem.Description, Is.EqualTo(description));
        Assert.That(orderItem.Price, Is.EqualTo(price));
        Assert.That(orderItem.TypeName, Is.EqualTo(typeName));
        Assert.That(orderItem.BrandName, Is.EqualTo(brandName));
        Assert.That(orderItem.Qty, Is.EqualTo(qty));
        Assert.That(orderItem.PictureUri, Is.EqualTo(pictureUri));
    }


    [Test]
    public void When_Create_Order_Then_Items_Should_Be_Present()
    {
        var emptyOrderItemsList = new List<OrderItem>();

        Action act = () => new Order(DateTime.Now, Guid.NewGuid(), "test", "test", emptyOrderItemsList);

        Assert.That(act, Throws.TypeOf<NoOrderItemsExceptions>());
    }

    [Test]
    public void When_Ship_Order_Then_Order_Status_Should_Be_Shipped()
    {
        Order order = createNewOrder();

        order.Ship();

        Assert.That(order.OrderStatus, Is.EqualTo(OrderStatus.Shipped));
    }

    [Test]
    public void When_Ship_Order_With_Invalid_Status_Then_Exception_Should_Be_Thrown()
    {
        var shippedOrder = createShippedOrder();
        var deliveredOrder = createDeliveredOrder();
        var returnedOrder = createReturnedOrder();

        Action act1 = () => shippedOrder.Ship();
        Action act2 = () => deliveredOrder.Ship();
        Action act3 = () => returnedOrder.Ship();

        Assert.That(act1, Throws.TypeOf<InvalidOrderStatusException>());
        Assert.That(act2, Throws.TypeOf<InvalidOrderStatusException>());
        Assert.That(act3, Throws.TypeOf<InvalidOrderStatusException>());
    }

    [Test]
    public void When_Deliver_Order_Then_Order_Status_Should_Be_Delivered()
    {
        var order = createShippedOrder();

        order.Deliver();

        Assert.That(order.OrderStatus, Is.EqualTo(OrderStatus.Delivered));
    }

    [Test]
    public void When_Deliver_Order_With_Invalid_Status_Then_Exception_Should_Be_Thrown()
    {
        var newOrder = createNewOrder();
        var deliveredOrder = createDeliveredOrder();
        var returnedOrder = createReturnedOrder();

        Action act2 = () => newOrder.Deliver();
        Action act1 = () => deliveredOrder.Deliver();
        Action act3 = () => returnedOrder.Deliver();

        Assert.That(act1, Throws.TypeOf<InvalidOrderStatusException>());
        Assert.That(act2, Throws.TypeOf<InvalidOrderStatusException>());
        Assert.That(act3, Throws.TypeOf<InvalidOrderStatusException>());
    }

    [Test]
    public void When_Return_Order_Then_Order_Status_Should_Be_Returned()
    {
        var order = createDeliveredOrder();

        order.Return();

        Assert.That(order.OrderStatus, Is.EqualTo(OrderStatus.Returned));
    }

    [Test]
    public void When_Return_Order_With_Invalid_Status_Then_Exception_Should_Be_Thrown()
    {
        var newOrder = createNewOrder();
        var shippedOrder = createShippedOrder();
        var returnedOrder = createReturnedOrder();

        Action act2 = () => newOrder.Return();
        Action act1 = () => shippedOrder.Return();
        Action act3 = () => returnedOrder.Return();

        Assert.That(act1, Throws.TypeOf<InvalidOrderStatusException>());
        Assert.That(act2, Throws.TypeOf<InvalidOrderStatusException>());
        Assert.That(act3, Throws.TypeOf<InvalidOrderStatusException>());
    }

    private Order createReturnedOrder()
    {
        var order = createDeliveredOrder();

        order.Return();

        return order;
    }

    private Order createDeliveredOrder()
    {
        var order = createShippedOrder();

        order.Deliver();

        return order;
    }

    private Order createShippedOrder()
    {
        var order = createNewOrder();

        order.Ship();

        return order;
    }

    private Order createNewOrder()
    {
        var orderItems = new List<OrderItem>() { new OrderItem(Guid.NewGuid(), "Test Item", "Test Description", 12m, "Tset type", "Test Brand", 11, @"\picture.png") };
        var order = new Order(DateTime.UtcNow, Guid.NewGuid(), "sample@example.com", "test address", orderItems);

        return order;
    }
}