using EShop.Ordering.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EShop.Ordering.Api.Data;

public class DbInitializer
{
    internal static async Task InitializeDb(OrderingDbContext orderingDbContext)
    {
        await orderingDbContext.Database.MigrateAsync();
    }
}
