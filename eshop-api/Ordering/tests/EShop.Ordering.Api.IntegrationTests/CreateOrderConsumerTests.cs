using EShop.Ordering.Api.Integration.Consumers;
using EShop.Ordering.Infrastructure.IntegrationTests;
using EShop.Ordering.Infrastructure.Read;
using EShop.Ordering.Infrastructure.Read.Queries;
using EShop.Ordering.Infrastructure.Services;
using EShop.Ordering.Integration.Commands;
using EShop.Ordering.Integration.Events;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace EShop.Ordering.Api.IntegrationTests;

[TestFixture]
[Category("CreateOrderConsumer")]
[Category("Saga")]
public class CreateOrderConsumerTests : BaseOrderingIntegationTests
{
    private ITestHarness _harness;
    private IOrderQueryService _orderQueryService;
    private IDateTimeService _dateTimeService;
    public override async Task SetupAsync()
    {
        await base.SetupAsync();

        _harness = serviceProvider.GetRequiredService<ITestHarness>();
        _orderQueryService = serviceProvider.GetRequiredService<IOrderQueryService>();

        await _harness.Start();
    }

    public override async Task TearDownAsync()
    {
        await _harness.Stop();
        await base.TearDownAsync();
    }

    protected override void AddServices(ServiceCollection sc)
    {
        base.AddServices(sc);

        sc.AddMassTransitTestHarness(cfg =>
        {
            cfg.AddConsumer<CreateOrderConsumer>();
        });

        _dateTimeService = Substitute.For<IDateTimeService>();

        sc.AddTransient((sc) => _dateTimeService);
    }

    [Test]
    public async Task When_Publish_Create_Order_Command_Then_Order_Should_Be_Created()
    {
        var customerId = Guid.NewGuid();
        var customerEmail = "test@example.com";
        var shippingAddress = "Test Shipping Address";
        var itemId = Guid.NewGuid();
        var itemName = "Test item";
        var qty = 22;
        var price = 12m;
        var description = "Test Item Description";
        var typeName = "Test item Type";
        var brandName = "Test item Brand";
        var pictureUrl = "item.png";
        var orderDate = DateTime.UtcNow.AddDays(-2);
        _dateTimeService.GetCurrentDateTime().Returns(orderDate);
        var createOrderCommand = new CreateOrderCommand(Guid.NewGuid(), customerId, customerEmail, shippingAddress,
            new List<CreateOrdeItem>
            {
                new CreateOrdeItem(itemId,itemName,qty,description, price,typeName,brandName,pictureUrl)
            });

        await _harness.Bus.Publish(createOrderCommand);

        Assert.That(await _harness.Consumed.Any<CreateOrderCommand>());
        var order = (await _orderQueryService.ListOrdersAsync(new ListOrderQuery()
        {
            CustomerId = customerId,
            PageIndex = 0,
            PageSize = 1
        }))?.Orders[0];
        Assert.That(order, Is.Not.Null);
        Assert.That(order.CustomerEmail, Is.EqualTo(customerEmail));
        Assert.That(order.ShippingAddress, Is.EqualTo(shippingAddress));
        Assert.That(order.OrderDate, Is.EqualTo(orderDate));
        Assert.That(order.OrderItems.Count, Is.EqualTo(1));
        Assert.That(order.OrderItems[0].PictureUri, Is.EqualTo(pictureUrl));
        Assert.That(order.OrderItems[0].Name, Is.EqualTo(itemName));
        Assert.That(order.OrderItems[0].Qty, Is.EqualTo(qty));
        Assert.That(order.OrderItems[0].Price, Is.EqualTo(price));
        Assert.That(order.OrderItems[0].Description, Is.EqualTo(description));
        Assert.That(order.OrderItems[0].TypeName, Is.EqualTo(typeName));
        Assert.That(order.OrderItems[0].BrandName, Is.EqualTo(brandName));
    }


    [Test]
    public async Task When_Publish_Create_Order_Command_Then_Created_Order_Event_Should_Be_Published()
    {
        var correlationId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var createOrderCommand = new CreateOrderCommand(correlationId, customerId, "test@example.com", "Test Address",
            new List<CreateOrdeItem>
            {
                new CreateOrdeItem( Guid.NewGuid(),"Test item",23,"Test Description", 33m ,"Test Type","Test Brand ","item2.png")
            });

        await _harness.Bus.Publish(createOrderCommand);

        Assert.That(await _harness.Consumed.Any<CreateOrderCommand>());
        var consumerHarness = _harness.GetConsumerHarness<CreateOrderConsumer>();
        Assert.That(await consumerHarness.Consumed.Any<CreateOrderCommand>());
        Assert.That(await _harness.Published.Any<OrderCreatedEvent>(publishedMessage =>
        {
            return publishedMessage.Context.Message.CorrelationId == correlationId;
        }));
    }
}
