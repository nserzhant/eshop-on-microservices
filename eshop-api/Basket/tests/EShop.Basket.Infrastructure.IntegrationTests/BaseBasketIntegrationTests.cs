using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace EShop.Basket.Infrastructure.IntegrationTests;
public class BaseBasketIntegrationTests
{
    protected ServiceProvider serviceProvider;

    [SetUp]
    public virtual async Task SetupAsync()
    {
        var sc = new ServiceCollection();

        await AddServicesAsync(sc);

        serviceProvider = sc.BuildServiceProvider();

        IDatabase database = serviceProvider.GetRequiredService<IDatabase>();

        database.Execute("FLUSHDB");
    }

    protected virtual async Task AddServicesAsync(ServiceCollection sc)
    {
        await sc.AddTestBasketServicesAsync();
    }

    [TearDown]
    public virtual async Task TearDownAsync()
    {
        await serviceProvider.DisposeAsync();
    }
}
