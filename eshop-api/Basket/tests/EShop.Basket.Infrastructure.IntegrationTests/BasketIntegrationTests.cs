using EShop.Basket.Core.Interfaces;
using EShop.Basket.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Basket.Infrastructure.IntegrationTests;

[TestFixture]
public class BasketIntegrationTests : BaseBasketIntegrationTests
{
    private IBasketRepository _repository;

    public override async Task SetupAsync()
    {
        await base.SetupAsync();

        _repository = serviceProvider.GetRequiredService<IBasketRepository>();
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
                    ItemName = "Test Name",
                    TypeName = "Test Basket Item Type",
                    BrandName = "Test Brand",
                    Description = "Test Description",
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
        Assert.That(basketSaved.Items[0].ItemName, Is.EqualTo(basket.Items[0].ItemName));
        Assert.That(basketSaved.Items[0].BrandName, Is.EqualTo(basket.Items[0].BrandName));
        Assert.That(basketSaved.Items[0].TypeName, Is.EqualTo(basket.Items[0].TypeName));
        Assert.That(basketSaved.Items[0].Qty, Is.EqualTo(basket.Items[0].Qty));
        Assert.That(basketSaved.Items[0].Price, Is.EqualTo(basket.Items[0].Price));
        Assert.That(basketSaved.Items[0].PictureUri, Is.EqualTo(basket.Items[0].PictureUri));
        Assert.That(basketSaved.Items[0].Description, Is.EqualTo(basket.Items[0].Description));
    }
}