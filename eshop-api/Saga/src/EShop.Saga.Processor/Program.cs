using EShop.Saga.Processor;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(log => log.AddConsole());
builder.Services.AddSagaServices(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

var host = builder.Build();

host.MapHealthChecks("/hc");

host.Run();
