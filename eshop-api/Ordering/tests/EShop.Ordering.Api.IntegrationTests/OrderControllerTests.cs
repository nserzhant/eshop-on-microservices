using EShop.Ordering.Api.IntegrationTests.Infrastructure;
using EShop.Ordering.Core.Interfaces;
using EShop.Ordering.Core.Models;
using EShop.Ordering.Infrastructure.IntegrationTests;
using EShop.Ordering.Infrastructure.Read.ReadModels;
using EShop.Ordering.Infrastructure.Read.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;

namespace EShop.Ordering.Api.IntegrationTests;

public class OrderControllerTests : BaseOrderingIntegationTests
{
    private const string API_BASE_URL = "api/order";

    private WebApplicationFactory<Program> webApplicationFactory;
    private TestAuthenticationContextBuilder _testAuthenticationContextBuilder;
    private IOrderRepository _orderRepository;

    [SetUp]
    public override async Task SetupAsync()
    {
        await base.SetupAsync();

        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.tests.json");
        _testAuthenticationContextBuilder = new TestAuthenticationContextBuilder();
        webApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.AddJsonFile(configPath);
                conf.AddEnvironmentVariables();
            });
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(s =>
                {
                    s.DefaultAuthenticateScheme = "Test";
                    s.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                services.AddSingleton(_testAuthenticationContextBuilder);
            });
        });

        _orderRepository = serviceProvider.GetRequiredService<IOrderRepository>();
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();

        webApplicationFactory.Dispose();
    }

    [Test]
    [Category("Get Order")]
    public async Task When_Customer_Order_Exists_Then_Customer_Could_Get_The_Order_By_Id()
    {
        var customerId = Guid.NewGuid();
        var orderingClient = webApplicationFactory.CreateClient();
        var order = await createCustomerOrder(customerId);
        _testAuthenticationContextBuilder.SetAuthorizedAs(customerId);

        var response = await orderingClient.GetAsync(Order(order.Id));

        var orderFromResponse = await fromHttpResponseMessage<OrderReadModel>(response);
        var orderItemFromReponse = orderFromResponse?.OrderItems?.FirstOrDefault();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(orderFromResponse, Is.Not.Null);
        Assert.That(orderFromResponse.Id, Is.EqualTo(order.Id));
        Assert.That(orderFromResponse.OrderDate, Is.EqualTo(order.OrderDate));
        Assert.That(orderFromResponse.CustomerId, Is.EqualTo(customerId));
        Assert.That(orderFromResponse.ShippingAddress, Is.EqualTo(order.ShippingAddress));
        Assert.That(orderItemFromReponse, Is.Not.Null);
        Assert.That(orderItemFromReponse.Name, Is.EqualTo(order.OrderItems.First().Name));
        Assert.That(orderItemFromReponse.Description, Is.EqualTo(order.OrderItems.First().Description));
        Assert.That(orderItemFromReponse.TypeName, Is.EqualTo(order.OrderItems.First().TypeName));
        Assert.That(orderItemFromReponse.BrandName, Is.EqualTo(order.OrderItems.First().BrandName));
        Assert.That(orderItemFromReponse.Qty, Is.EqualTo(order.OrderItems.First().Qty));
        Assert.That(orderItemFromReponse.PictureUri, Is.EqualTo(order.OrderItems.First().PictureUri));
    }

    [Test]
    [Category("Get Order")]
    public async Task When_Customer_Is_Unauthenticated_Then_Get_Order_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var orderingClient = webApplicationFactory.CreateClient();

        var response = await orderingClient.GetAsync(Order(1));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Get Order")]
    public async Task When_Customer_Is_Trying_To_Get_Anothers_Order_Then_Forbidden_Returns()
    {
        var customerId = Guid.NewGuid();
        var anotherCustomerId = Guid.NewGuid();
        var orderingClient = webApplicationFactory.CreateClient();
        var order = await createCustomerOrder(anotherCustomerId);
        _testAuthenticationContextBuilder.SetAuthorizedAs(customerId);

        var response = await orderingClient.GetAsync(Order(order.Id));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Get Order")]
    public async Task When_Customer_Is_Requesting_Non_Existing_Order_Then_Not_Found_Returns()
    {
        var customerId = Guid.NewGuid();
        var orderingClient = webApplicationFactory.CreateClient();
        _testAuthenticationContextBuilder.SetAuthorizedAs(customerId);

        var response = await orderingClient.GetAsync(Order(1));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Category("Get All Orders")]
    public async Task When_Customer_Is_Unauthenticated_Then_Get_All_Orders_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var orderingClient = webApplicationFactory.CreateClient();

        var response = await orderingClient.GetAsync(AllOrders);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Get All Orders")]
    public async Task When_Customer_Orders_Exists_Then_Get_All_Orders_Returns_Them()
    {
        var customerId = Guid.NewGuid();
        var orderingClient = webApplicationFactory.CreateClient();
        await createCustomerOrder(customerId);
        await createCustomerOrder(customerId);
        _testAuthenticationContextBuilder.SetAuthorizedAs(customerId);

        var response = await orderingClient.GetAsync(AllOrders);

        var ordersFromResponse = await fromHttpResponseMessage<ListOrderResult>(response);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(ordersFromResponse, Is.Not.Null);
        Assert.That(ordersFromResponse.TotalCount, Is.EqualTo(2));
        Assert.That(ordersFromResponse.Orders.Count, Is.EqualTo(2));
    }

    [Test]
    [Category("Get All Orders")]
    public async Task When_Another_Customer_Orders_Exists_Then_Get_All_Orders_Does_Not_Return_Them()
    {
        var customerId = Guid.NewGuid();
        var orderingClient = webApplicationFactory.CreateClient();
        var anotherCustomerId = Guid.NewGuid();
        await createCustomerOrder(anotherCustomerId);
        _testAuthenticationContextBuilder.SetAuthorizedAs(customerId);

        var response = await orderingClient.GetAsync(AllOrders);

        var ordersFromResponse = await fromHttpResponseMessage<ListOrderResult>(response);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(ordersFromResponse, Is.Not.Null);
        Assert.That(ordersFromResponse.TotalCount, Is.EqualTo(0));
        Assert.That(ordersFromResponse.Orders.Count, Is.EqualTo(0));
    }

    [Test]
    [Category("Return Order")]
    public async Task When_Customer_Is_Unauthenticated_Then_Return_Order_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var orderingClient = webApplicationFactory.CreateClient();

        var response = await orderingClient.PostAsync(Return(1), null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Return Order")]
    public async Task When_Customer_Is_Trying_To_Return_Anothers_Order_Then_Forbidden_Returns()
    {
        var customerId = Guid.NewGuid();
        var anotherCustomerId = Guid.NewGuid();
        var orderingClient = webApplicationFactory.CreateClient();
        var order = await createCustomerOrder(anotherCustomerId);
        _testAuthenticationContextBuilder.SetAuthorizedAs(customerId);

        var response = await orderingClient.PostAsync(Return(order.Id), null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Return Order")]
    public async Task When_Customer_Order_Exists_Then_Customer_Could_Return_The_Order_By_Id()
    {
        var customerId = Guid.NewGuid();
        var orderingClient = webApplicationFactory.CreateClient();
        var order = await createCustomerOrder(customerId, Core.Models.OrderStatus.Delivered);
        _testAuthenticationContextBuilder.SetAuthorizedAs(customerId);

        var response = await orderingClient.PostAsync(Return(order.Id), null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Return Order")]
    public async Task When_Customer_Is_Trying_To_Return_Non_Existing_Order_Then_Not_Found_Returns()
    {
        var customerId = Guid.NewGuid();
        var orderingClient = webApplicationFactory.CreateClient();
        _testAuthenticationContextBuilder.SetAuthorizedAs(customerId);

        var response = await orderingClient.PostAsync(Return(1), null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    private async Task<Order> createCustomerOrder(Guid customerId, Core.Models.OrderStatus orderStatus = Core.Models.OrderStatus.Created)
    {
        var orderDate = DateTime.UtcNow;
        var customerEmail = "testemail@example.com";
        var shippingAddress = "Test User Address";
        var catalogItemId = Guid.NewGuid(); ;
        var name = "Test Item";
        var description = "Test Item Description";
        var typeName = "Type Name";
        var brandName = "Brand Name";
        var qty = 25;
        var pictureUri = @"\picture.png";
        List<OrderItem> orderItems =
            [
                new OrderItem(catalogItemId, name, description, typeName, brandName, qty, pictureUri)
            ];
        var order = new Order(orderDate, customerId, customerEmail, shippingAddress, orderItems);

        switch (orderStatus)
        {
            case Core.Models.OrderStatus.Shipped: order.Ship(); break;
            case Core.Models.OrderStatus.Delivered:
                order.Ship();
                order.Deliver();
                break;
            default: break;
        }

        await _orderRepository.CreateOrder(order);
        await _orderRepository.SaveChangesAsync();

        return order;
    }

    protected async Task<T?> fromHttpResponseMessage<T>(HttpResponseMessage? response)
    {
        if (response == null)
        {
            return default;
        }

        var stringContent = await response.Content.ReadAsStringAsync();
        var jsonSerializationOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IgnoreReadOnlyProperties = false, IncludeFields = true };

        try
        {
            var result = JsonSerializer.Deserialize<T>(stringContent, jsonSerializationOptions);

            return result;
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(stringContent))
                throw new Exception(stringContent);

            throw ex;
        }
    }

    public static string Order(int orderId) => $"{API_BASE_URL}/{orderId}";
    public static string AllOrders => $"{API_BASE_URL}";
    public static string Return(int orderId) => $"{API_BASE_URL}/{orderId}/return";
}