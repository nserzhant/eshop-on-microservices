using EShop.Basket.Api.IntegrationTests.Infrastructure;
using EShop.Basket.Core.Interfaces;
using EShop.Basket.Core.Models;
using EShop.Basket.Infrastructure.IntegrationTests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace EShop.Basket.Api.IntegrationTests;

internal class BasketControllerTests : BaseBasketIntegrationTests
{
    private const string API_BASE_URL = "api/Basket";
    private readonly string CheckoutUrl = $"{API_BASE_URL}/checkout";

    private WebApplicationFactory<Program> webApplicationFactory;
    private TestAuthenticationContextBuilder _testAuthenticationContextBuilder;

    [SetUp]
    public override void Setup()
    {
        base.Setup();

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
            });
        });
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();

        webApplicationFactory.Dispose();
    }

    [Test]
    [Category("Save Basket")]
    public async Task When_Customer_Is_Unauthenticated_Then_Save_Basket_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var basketClient = webApplicationFactory.CreateClient();
        var content = createBasketContent(new CustomerBasket());

        var response = await basketClient.PostAsync(API_BASE_URL, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Save Basket")]
    public async Task When_Customer_Is_Authenticated_Then_Basket_Can_Be_Saved()
    {
        _testAuthenticationContextBuilder.SetAuthorizedAs(Guid.NewGuid());
        var basketClient = webApplicationFactory.CreateClient();
        var content = createBasketContent(new CustomerBasket());

        var response = await basketClient.PostAsync(API_BASE_URL, content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Checkout Basket")]
    public async Task When_Customer_Is_Unauthenticated_Then_Checkout_Basket_Returns_Unathorized()
    {
        _testAuthenticationContextBuilder.SetUnauthenticated();
        var basketClient = webApplicationFactory.CreateClient();

        var response = await basketClient.PostAsync(CheckoutUrl, null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Checkout Basket")]
    public async Task When_Customer_Is_Authenticated_Then_Basket_Can_Be_Checked_Out()
    {
        _testAuthenticationContextBuilder.SetAuthorizedAs(Guid.NewGuid());
        var basketClient = webApplicationFactory.CreateClient();

        var response = await basketClient.PostAsync(CheckoutUrl, null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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
        var storedBasket = createCustomerBasket(customerId);
        _testAuthenticationContextBuilder.SetAuthorizedAs(customerId);
        var basketClient = webApplicationFactory.CreateClient();

        var response = await basketClient.GetAsync(API_BASE_URL);
        var basket = await fromHttpResponseMessage<CustomerBasket>(response);

        Assert.That(basket, Is.Not.Null);
        Assert.That(basket.Items[0].CatalogItemId, Is.EqualTo(storedBasket.Items[0].CatalogItemId));
        Assert.That(basket.Items[0].Name, Is.EqualTo(storedBasket.Items[0].Name));
        Assert.That(basket.Items[0].BrandName, Is.EqualTo(storedBasket.Items[0].BrandName));
        Assert.That(basket.Items[0].Type, Is.EqualTo(storedBasket.Items[0].Type));
        Assert.That(basket.Items[0].Qty, Is.EqualTo(storedBasket.Items[0].Qty));
        Assert.That(basket.Items[0].PictureUri, Is.EqualTo(storedBasket.Items[0].PictureUri));
        Assert.That(basket.Items[0].Price, Is.EqualTo(storedBasket.Items[0].Price));
    }

    // Save Basket => Checkout Basket => Get Basket => Check Content

    private CustomerBasket createCustomerBasket(Guid customerId)
    {
        CustomerBasket basket = new CustomerBasket();

        basket.Items = 
        [
            new BasketItem
            { 
                CatalogItemId = Guid.NewGuid(), 
                Name = "Test Catalog Item",
                BrandName = "Test Brand", 
                Type = "Test Type", 
                Qty = 1,
                PictureUri = "/images/testItem.png",
                Price = 34m
            }
        ];

        var basketRepository = serviceProvider.GetRequiredService<IBasketRepository>();

        basketRepository.SaveBasketAsync(customerId, basket);

        return basket;
    }

    private static StringContent createBasketContent(CustomerBasket customerBasket)
    {
        var content = new StringContent(JsonSerializer.Serialize(customerBasket));

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

