using EShop.Basket.Api.Integration.Commands;
using EShop.Basket.Api.Integration.Consumers;
using EShop.Basket.Api.Integration.Events;
using EShop.Basket.Core.Interfaces;
using EShop.Basket.Core.Models;
using EShop.Basket.Infrastructure.IntegrationTests;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Basket.Api.IntegrationTests;
public class ClearBasketConsumerTests : BaseBasketIntegrationTests
{
    private ITestHarness _harness;
    private IBasketRepository _basketRepository;

    public override async Task SetupAsync()
    {
        await base.SetupAsync();

        _harness = serviceProvider.GetRequiredService<ITestHarness>();
        _basketRepository = serviceProvider.GetRequiredService<IBasketRepository>();

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
                cfg.AddConsumer<ClearBasketConsumer>();
            });
    }

    [Test]
    public async Task When_Clear_Basket_Command_Consumed_Then_Basket_Should_Be_ClearedAsync()
    {
        var customerId = Guid.NewGuid();
        var basket = new CustomerBasket()
        {
            Items = new List<BasketItem>()
            {
                new BasketItem()
                {
                    CatalogItemId = Guid.NewGuid(),
                    Name = "Sample Name",
                    Type = "Sample Basket Item Type",
                    BrandName = "Sample Brand",
                    PictureUri = "/image-test.png",
                    Price = 55m,
                    Qty = 7
                }
            }
        };
        await _basketRepository.SaveBasketAsync(customerId, basket);

        await _harness.Bus.Publish(new ClearBasketCommand(Guid.NewGuid(), customerId, basket.Id));

        Assert.That(await _harness.Consumed.Any<ClearBasketCommand>());
        var existingBasket = await _basketRepository.GetBasketAsync(customerId);
        Assert.That(existingBasket.Items.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task When_Clear_Basket_Command_Consumed_Then_Basket_Cleared_Event_Should_Be_Published()
    {
        var correlationId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var basket = new CustomerBasket() { Items = new List<BasketItem>() };
        await _basketRepository.SaveBasketAsync(customerId, basket);

        await _harness.Bus.Publish(new ClearBasketCommand(correlationId, customerId, basket.Id));

        var consumerHarness = _harness.GetConsumerHarness<ClearBasketConsumer>();
        Assert.That(await consumerHarness.Consumed.Any<ClearBasketCommand>());
        Assert.That(await _harness.Published.Any<BasketClearedEvent>(publishedMessage =>
        {
            return publishedMessage.Context.Message.CorrelationId == correlationId;
        }));
    }
}
