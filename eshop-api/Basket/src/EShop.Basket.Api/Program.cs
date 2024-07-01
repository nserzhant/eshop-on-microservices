using EShop.Basket.Api;
using EShop.Basket.Api.Integration.Consumers;
using EShop.Basket.Api.Settings;
using EShop.Basket.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

// Get settings to configure JWT tokens

var jwtSettings = builder.Configuration.GetSection(Consts.JWT_CONFIG_NAME)
    .Get<JWTSettings>() ?? new JWTSettings();

// Get settings to configure Message Broker connection

var messageBrokerSettings = builder.Configuration.GetSection(Consts.MESSAGE_BROKER_CONFIG_NAME)
    .Get<MessageBrockerSettings>() ?? new MessageBrockerSettings();

// Configure settings for Client app
var clientOrigins = builder.Configuration[Consts.SPA_CLIENT_ORIGIN_CONFIG_NAME];

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddBasketSercices(builder.Configuration);
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ClockSkew = TimeSpan.FromSeconds(0),
        ValidateIssuer = true,
        ValidateAudience = !string.IsNullOrEmpty(jwtSettings.Audience),
        ValidAudience = jwtSettings.Audience,
        ValidIssuer = jwtSettings.Issuer,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };

    options.MetadataAddress = jwtSettings.MetadataAddress;
    options.RequireHttpsMetadata = false;
});
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
        builder.WithOrigins(clientOrigins).AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddHttpLogging(options => new HttpLoggingOptions());

// Add MassTransit Consumers worker (Generally it should be implemented as a separate worker service)
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ClearBasketConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(messageBrokerSettings.RabbitMQHost, messageBrokerSettings.RabbitMQPort, messageBrokerSettings.RabbitMQVirtualHost, h =>
        {
            h.Username(messageBrokerSettings.RabbitMQUsername);
            h.Password(messageBrokerSettings.RabbitMQPassword);
        });

        cfg.ReceiveEndpoint(messageBrokerSettings.QueueName, configureEndpoint =>
        {
            configureEndpoint.ConfigureConsumer<ClearBasketConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUI();
    app.UseCors("corsapp");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }