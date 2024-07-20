using EShop.Saga.Processor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var initDbOnStartup = builder.Configuration.GetValue<bool>(Consts.INIT_DB_ON_STARTUP_CONFIG_NAME, false);

builder.Services.AddLogging(log => log.AddConsole());
builder.Services.AddSagaServices(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

var host = builder.Build();

host.MapHealthChecks("/hc");

if (initDbOnStartup)
{
    using var scope = host.Services.CreateScope();
    var scopedProvider = scope.ServiceProvider;
    var dbContext = scopedProvider.GetRequiredService<DbContext>();
    await dbContext.Database.MigrateAsync();
}

host.Run();
