using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace EShop.Basket.Infrastructure.IntegrationTests;
public class BaseBasketIntegrationTests
{
    protected ServiceProvider serviceProvider;

    [SetUp]
    public virtual void Setup()
    {
        var sc = new ServiceCollection();

        sc.AddTestBasketServices();

        serviceProvider = sc.BuildServiceProvider();

        IDatabase database = serviceProvider.GetRequiredService<IDatabase>();

        database.Execute("FLUSHDB");
    }

    [TearDown]
    public virtual void TearDown()
    {
        serviceProvider.Dispose();
    }
}
