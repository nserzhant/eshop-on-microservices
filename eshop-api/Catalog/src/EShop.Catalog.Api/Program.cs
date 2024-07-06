using EShop.Catalog.Api.Constants;
using EShop.Catalog.Api.Data;
using EShop.Catalog.Api.Filters;
using EShop.Catalog.Api.Settings;
using EShop.Catalog.Infrastructure;
using EShop.Catalog.Infrastructure.ConsumeFilters;
using EShop.Catalog.Integration.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

// Get settings to configure JWT tokens

var employeeJwtSettings = builder.Configuration.GetSection(ConfigurationKeys.EMPLOYEE_JWT_CONFIG_NAME)
    .Get<JWTSettings>() ?? new JWTSettings();

var clientJwtSettings = builder.Configuration.GetSection(ConfigurationKeys.CLIENT_JWT_CONFIG_NAME)
    .Get<JWTSettings>() ?? new JWTSettings();

// Get settings to configure Message Broker connection

var messageBrokerSettings = builder.Configuration.GetSection(ConfigurationKeys.MESSAGE_BROKER_CONFIG_NAME)
    .Get<MessageBrockerSettings>() ?? new MessageBrockerSettings();

// Configure settings for Client app
var clientOrigins = builder.Configuration[ConfigurationKeys.SPA_CLIENT_ORIGINS_CONFIG_NAME];

// Setting to init Db with test data on startup

var initDbOnStartup = builder.Configuration.GetValue<bool>(ConfigurationKeys.INIT_DB_ON_STARTUP_CONFIG_NAME, false);
var generatedItemPictureUriHost = builder.Configuration.GetValue(ConfigurationKeys.GENERATED_ITEMS_PICTURE_URI_HOST_CONFIG_NAME, "");

// Add services to the container.
builder.Services.AddCatalogServices(builder.Configuration);

builder.Services.AddAuthentication()
    .AddJwtBearer(AuthenticationSchemeNames.Employee, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.FromSeconds(0),
            ValidateIssuer = true,
            ValidateAudience = !string.IsNullOrEmpty(employeeJwtSettings.Audience),
            ValidAudience = employeeJwtSettings.Audience,
            ValidIssuer = employeeJwtSettings.Issuer,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        options.RequireHttpsMetadata = false;
        options.MetadataAddress = employeeJwtSettings.MetadataAddress;
    })
    .AddJwtBearer(AuthenticationSchemeNames.Client, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.FromSeconds(0),
            ValidateIssuer = true,
            ValidateAudience = !string.IsNullOrEmpty(clientJwtSettings.Audience),
            ValidAudience = clientJwtSettings.Audience,
            ValidIssuer = clientJwtSettings.Issuer,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        options.RequireHttpsMetadata = false;
        options.MetadataAddress = employeeJwtSettings.MetadataAddress;
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(AuthenticationSchemeNames.Employee, AuthenticationSchemeNames.Client)
        .Build();
});

builder.Services.AddControllers(options => options.Filters.Add<CatalogDomainExceptionFilter>());
builder.Services.AddOpenApiDocument(document =>
{
    document.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        Description = "Type into the textbox: {your JWT token}."
    });
    document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
});
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    if (clientOrigins != null)
        builder.WithOrigins(clientOrigins.Split(',')).AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddHttpLogging(options => new HttpLoggingOptions());

builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

// The following line enables Application Insights telemetry collection.
builder.Services.AddApplicationInsightsTelemetry();

// Add MassTransit Consumers worker (Generally it should be implemented as a separate worker service)
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReserveStocksConsumer>();
    x.AddConsumer<ReleaseStocksConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(messageBrokerSettings.RabbitMQHost, messageBrokerSettings.RabbitMQPort, messageBrokerSettings.RabbitMQVirtualHost, h =>
        {
            h.Username(messageBrokerSettings.RabbitMQUsername);
            h.Password(messageBrokerSettings.RabbitMQPassword);
            
        });

        cfg.ReceiveEndpoint(messageBrokerSettings.ReserveStockQueueName, configureEndpoint =>
        {
            configureEndpoint.ConfigureConsumer<ReserveStocksConsumer>(context);
            configureEndpoint.UseConsumeFilter(typeof(IdempotentConsumingFilter<>), context);
        });

        cfg.ReceiveEndpoint(messageBrokerSettings.ReleaseStockQueueName, configureEndpoint =>
        {
            configureEndpoint.ConfigureConsumer<ReleaseStocksConsumer>(context);
            configureEndpoint.UseConsumeFilter(typeof(IdempotentConsumingFilter<>), context);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUI();
    app.UseCors("corsapp");
}

app.UseHttpLogging();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/hc");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Images")),
    RequestPath = "/images"
});

if (initDbOnStartup)
{
    using var scope = app.Services.CreateScope();
    var scopedProvider = scope.ServiceProvider;
    var dbContext = scopedProvider.GetRequiredService<CatalogDbContext>();
    var pictureUriPrefix = @$"{generatedItemPictureUriHost}/images";
    await DbInitializer.InitializeDbWIthTestData(dbContext, pictureUriPrefix);
}

app.Run();
public partial class Program { }