using EShop.Ordering.Infrastructure.IntegrationTests.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Ordering.Infrastructure.IntegrationTests;
public class BaseOrderingIntegationTests
{
    protected ServiceProvider serviceProvider;

    [SetUp]
    public virtual async Task SetupAsync()
    {
        var sc = new ServiceCollection();

        AddServices(sc);

        serviceProvider = sc.BuildServiceProvider();

        var _orderDbContext = serviceProvider.GetRequiredService<OrderingDbContext>();

        await _orderDbContext.ClearDb();
    }

    protected virtual void AddServices(ServiceCollection sc)
    {
        sc.AddTestOrderingServices();
    }

    [TearDown]
    public virtual async Task TearDownAsync()
    {
        await serviceProvider.DisposeAsync();
    }
    
}
