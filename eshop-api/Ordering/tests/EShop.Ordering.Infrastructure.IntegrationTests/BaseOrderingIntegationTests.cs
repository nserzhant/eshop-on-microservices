using Microsoft.Extensions.DependencyInjection;

namespace EShop.Ordering.Infrastructure.IntegrationTests;
public class BaseOrderingIntegationTests
{
    protected ServiceProvider serviceProvider;

    [SetUp]
    public virtual async Task SetupAsync()
    {
        var sc = new ServiceCollection();

        sc.AddTestOrderingServices();

        serviceProvider = sc.BuildServiceProvider();

        var _orderDbContext = serviceProvider.GetRequiredService<OrderingDbContext>();

        await _orderDbContext.ClearDb();
    }

    [TearDown]
    public virtual void TearDown()
    {
        serviceProvider.Dispose();
    }
}
