using EShop.Basket.Core.Interfaces;
using EShop.Basket.Core.Models;
using EShop.Basket.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Basket.Infrastructure.IntegrationTests;

public class BasketIntegrationTests : BaseBasketIntegrationTests
{
    private IBasketRepository _repository;
    private IBasketService _basketService;

    [SetUp]
    public override void Setup()
    {
        base.Setup();

        _repository = serviceProvider.GetRequiredService<IBasketRepository>();
        _basketService = serviceProvider.GetRequiredService<IBasketService>();
    }

    [Category("Get Basket")]
    [Test]
    public async Task When_No_Basket_Created_Then_Empty_Should_Be_Returned()
    {
        var customerId = Guid.NewGuid();

        var basketSaved = await _repository.GetBasketAsync(customerId);


        Assert.That(basketSaved, Is.Not.Null);
        Assert.That(basketSaved.Items.Count, Is.EqualTo(0));
    }

    [Category("Get Basket")]
    [Test]
    public async Task When_Save_Basket_Then_It_Could_Be_Retreived_By_Customer_Id()
    {
        var customerId = Guid.NewGuid();
        var basket = new CustomerBasket()
        {  
            Items = [      
                new BasketItem () {
                    CatalogItemId = Guid.NewGuid(), 
                    Name = "Test Name",
                    Type = "Test Basket Item Type",
                    BrandName = "Sample Brand",
                    PictureUri = "/image.png",
                    Price = 123.3m,
                    Qty = 19
                }
             ]
        };
        await _repository.SaveBasketAsync(customerId, basket);

        var basketSaved = await _repository.GetBasketAsync(customerId);

        Assert.That(basketSaved, Is.Not.Null);
        Assert.That(basketSaved.Items[0].CatalogItemId, Is.EqualTo(basket.Items[0].CatalogItemId));
        Assert.That(basketSaved.Items[0].Name, Is.EqualTo(basket.Items[0].Name));
        Assert.That(basketSaved.Items[0].BrandName, Is.EqualTo(basket.Items[0].BrandName));
        Assert.That(basketSaved.Items[0].Type, Is.EqualTo(basket.Items[0].Type));
        Assert.That(basketSaved.Items[0].Qty, Is.EqualTo(basket.Items[0].Qty));
        Assert.That(basketSaved.Items[0].Price, Is.EqualTo(basket.Items[0].Price));
        Assert.That(basketSaved.Items[0].PictureUri, Is.EqualTo(basket.Items[0].PictureUri));
    }

    [Category("CheckOut")]
    [Test]
    public async Task When_Checkout_Then_Basket_Should_Be_Cleared()
    {
        var customerId = Guid.NewGuid();
        var basket = new CustomerBasket()
        {
            Items = [
                new BasketItem () {
                    CatalogItemId = Guid.NewGuid(),
                    Name = "Test Name",
                    Type = "Test Basket Item Type",
                    BrandName = "Sample Brand",

                    PictureUri = "/image.png",
                    Price = 123.3m,
                    Qty = 19
                }
             ]
        };
        await _repository.SaveBasketAsync(customerId, basket);

        await _basketService.CheckOutAsync(customerId);

        var currentBasket = await _repository.GetBasketAsync(customerId);
        Assert.That(currentBasket.Items.Count, Is.EqualTo(0));
    }
}