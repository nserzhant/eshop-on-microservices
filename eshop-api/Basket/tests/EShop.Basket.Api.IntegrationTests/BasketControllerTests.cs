using EShop.Basket.Api.IntegrationTests.Infrastructure;
using EShop.Basket.Api.Models;
using EShop.Basket.Core.Interfaces;
using EShop.Basket.Core.Models;
using EShop.Basket.Infrastructure.IntegrationTests;
using EShop.Basket.Integration.Events;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace EShop.Basket.Api.IntegrationTests;

[TestFixture]
public class BasketControllerTests : BaseBasketIntegrationTests
{
    private const string API_BASE_URL = "api/Basket";
    private readonly string CheckoutUrl = $"{API_BASE_URL}/checkout";

    private WebApplicationFactory<Program> webApplicationFactory;
    private TestAuthenticationContextBuilder _testAuthenticationContextBuilder;
    private ITestHarness _harness;

    public override async Task SetupAsync()
    {
        await base.SetupAsync();

        _testAuthenticationContextBuilder = new TestAuthenticationContextBuilder();
        webApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(s =>
                {
                    s.DefaultAuthenticateScheme = "Test";
                    s.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                services.AddSingleton(_testAuthenticationContextBuilder);
                services.AddMassTransitTestHarness();
            });
        });

        _harness = webApplicationFactory.Services.GetTestHarness();
        await _harness.Start();
    }

    public override async Task TearDownAsync()
    {
        await _harness.Stop();
        await base.TearDownAsync();

        await webApplicationFactory.DisposeAsync();
    }

    [Test]
    [Category("Save Basket")]
    public async Task When_Customer_Is_Unauthenticated_Then_Save_Basket_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var basketClient = webApplicationFactory.CreateClient();
        var content = createStringContent(new CustomerBasket());

        var response = await basketClient.PostAsync(API_BASE_URL, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Save Basket")]
    public async Task When_Customer_Is_Authenticated_Then_Basket_Can_Be_Saved()
    {
        _testAuthenticationContextBuilder.SetAuthorizedAs(Guid.NewGuid());
        var basketClient = webApplicationFactory.CreateClient();
        var content = createStringContent(new CustomerBasket());

        var response = await basketClient.PostAsync(API_BASE_URL, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Checkout Basket")]
    public async Task When_Customer_Is_Unauthenticated_Then_Checkout_Basket_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var basketClient = webApplicationFactory.CreateClient();
        var checkoutDTO = new CheckoutDTO() { ShippingAddress = "Shipping Address" };
        var content = createStringContent(checkoutDTO);

        var response = await basketClient.PostAsync(CheckoutUrl, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Checkout Basket")]
    public async Task When_Customer_Does_Not_Have_Email_Then_Checkout_Basket_Returns_Forbidden()
    {
        _testAuthenticationContextBuilder.SetAuthorizedAs(Guid.NewGuid(), string.Empty);
        var basketClient = webApplicationFactory.CreateClient();
        var checkoutDTO = new CheckoutDTO() { ShippingAddress = "Shipping Address" };
        var content = createStringContent(checkoutDTO);

        var response = await basketClient.PostAsync(CheckoutUrl, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("Checkout Basket")]
    public async Task When_Customer_Is_Authenticated_Then_Basket_Can_Be_Checked_Out()
    {
        _testAuthenticationContextBuilder.SetAuthorizedAs(Guid.NewGuid());
        var basketClient = webApplicationFactory.CreateClient();
        var checkoutDTO = new CheckoutDTO() { ShippingAddress = "Shipping Address" };
        var content = createStringContent(checkoutDTO);

        var response = await basketClient.PostAsync(CheckoutUrl, content);

        Assert.That(await _harness.Published.Any());
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Checkout Basket")]
    [Category("Saga")]
    public async Task When_Checkout_Basket_Then_Integration_Event_Should_Be_Published()
    {
        var customerId = Guid.NewGuid();
        var shippingAddress = "Test Address";
        var email = "testEmail@example.com";
        var basket = await createCustomerBasketAsync(customerId);
        _testAuthenticationContextBuilder.SetAuthorizedAs(customerId, email);
        var basketClient = webApplicationFactory.CreateClient();
        var checkoutDTO = new CheckoutDTO() { ShippingAddress = shippingAddress };
        var content = createStringContent(checkoutDTO);

        await basketClient.PostAsync(CheckoutUrl, content);

        var itemPublished = await _harness.Published.SelectAsync<BasketCheckedOutEvent>().FirstOrDefault();
        var basketCheckedOutEvent = itemPublished?.Context?.Message;
        Assert.That(basketCheckedOutEvent, Is.Not.Null);
        Assert.That(basketCheckedOutEvent.BasketId, Is.EqualTo(basket.Id));
        Assert.That(basketCheckedOutEvent.CustomerId, Is.EqualTo(customerId));
        Assert.That(basketCheckedOutEvent.ShippingAddress, Is.EqualTo(shippingAddress));
        Assert.That(basketCheckedOutEvent.CustomerEmail, Is.EqualTo(email));
        Assert.That(basketCheckedOutEvent.Items[0].CatalogItemId, Is.EqualTo(basket.Items[0].CatalogItemId));
        Assert.That(basketCheckedOutEvent.Items[0].Qty, Is.EqualTo(basket.Items[0].Qty));
        Assert.That(basketCheckedOutEvent.Items[0].ItemName, Is.EqualTo(basket.Items[0].ItemName));
        Assert.That(basketCheckedOutEvent.Items[0].BrandName, Is.EqualTo(basket.Items[0].BrandName));
        Assert.That(basketCheckedOutEvent.Items[0].Description, Is.EqualTo(basket.Items[0].Description));
        Assert.That(basketCheckedOutEvent.Items[0].PictureUri, Is.EqualTo(basket.Items[0].PictureUri));
        Assert.That(basketCheckedOutEvent.Items[0].TypeName, Is.EqualTo(basket.Items[0].TypeName));
        Assert.That(basketCheckedOutEvent.Items[0].Price, Is.EqualTo(basket.Items[0].Price));
    }
       
    [Test]
    [Category("Get Basket")]
    public async Task When_Customer_Is_Unauthenticated_Then_Get_Basket_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var basketClient = webApplicationFactory.CreateClient();

        var response = await basketClient.GetAsync(API_BASE_URL);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Get Basket")]
    public async Task When_Customer_Is_Authenticated_Then_Basket_Can_Get()
    {
        _testAuthenticationContextBuilder.SetAuthorizedAs(Guid.NewGuid());
        var basketClient = webApplicationFactory.CreateClient();

        var response = await basketClient.GetAsync(API_BASE_URL);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Get Basket")]
    public async Task When_Customer_Saved_Basket_Then_Basket_Can_Get()
    {
        var customerId = Guid.NewGuid();
        var basket = await createCustomerBasketAsync(customerId);
        _testAuthenticationContextBuilder.SetAuthorizedAs(customerId);
        var basketClient = webApplicationFactory.CreateClient();

        var response = await basketClient.GetAsync(API_BASE_URL);

        var basketFromReponse = await fromHttpResponseMessage<CustomerBasket>(response);
        Assert.That(basketFromReponse, Is.Not.Null);
        Assert.That(basketFromReponse.Items[0].CatalogItemId, Is.EqualTo(basket.Items[0].CatalogItemId));
        Assert.That(basketFromReponse.Items[0].ItemName, Is.EqualTo(basket.Items[0].ItemName));
        Assert.That(basketFromReponse.Items[0].Description, Is.EqualTo(basket.Items[0].Description));
        Assert.That(basketFromReponse.Items[0].BrandName, Is.EqualTo(basket.Items[0].BrandName));
        Assert.That(basketFromReponse.Items[0].TypeName, Is.EqualTo(basket.Items[0].TypeName));
        Assert.That(basketFromReponse.Items[0].Qty, Is.EqualTo(basket.Items[0].Qty));
        Assert.That(basketFromReponse.Items[0].PictureUri, Is.EqualTo(basket.Items[0].PictureUri));
        Assert.That(basketFromReponse.Items[0].Price, Is.EqualTo(basket.Items[0].Price));
    }

    private async Task<CustomerBasket> createCustomerBasketAsync(Guid customerId)
    {
        CustomerBasket basket = new CustomerBasket();

        basket.Items =
        [
            new BasketItem
            {
                CatalogItemId = Guid.NewGuid(),
                ItemName = "Test Catalog Item",
                Description = "Test Catalog Item Description",
                BrandName = "Test Brand",
                TypeName = "Test Type",
                Qty = 1,
                PictureUri = "/images/testItem.png",
                Price = 34m
            }
        ];

        var basketRepository = serviceProvider.GetRequiredService<IBasketRepository>();

        await basketRepository.SaveBasketAsync(customerId, basket);

        return basket;
    }

    private static StringContent createStringContent<T>(T contentSource)
    {
        var content = new StringContent(JsonSerializer.Serialize(contentSource));

        content.Headers.Remove(HeaderNames.ContentType);
        content.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Application.Json);

        return content;
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
}

