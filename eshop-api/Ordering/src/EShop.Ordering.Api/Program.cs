using EShop.Ordering.Api;
using EShop.Ordering.Api.Data;
using EShop.Ordering.Api.Filters;
using EShop.Ordering.Api.Settings;
using EShop.Ordering.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

// Get settings to configure JWT tokens

var jwtSettingsSection = builder.Configuration.GetSection(Consts.JWT_CONFIG_NAME);
var jwtSettings = new JWTSettings();

jwtSettingsSection.Bind(jwtSettings);

// Setting to init Db on startup

var initDbOnStartup = builder.Configuration.GetValue<bool>(Consts.INIT_DB_ON_STARTUP_CONFIG_NAME, false);

// Configure settings for Client app
var clientOrigins = builder.Configuration[Consts.SPA_CUSTOMER_ORIGINS_CONFIG_NAME];

// Add services to the container.

builder.Services.AddControllers(options => options.Filters.Add<OrderDomainExceptionFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOrderingServices(builder.Configuration);
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

// The following line enables Application Insights telemetry collection.
builder.Services.AddApplicationInsightsTelemetry();

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


if (initDbOnStartup)
{
    using var scope = app.Services.CreateScope();
    var scopedProvider = scope.ServiceProvider;
    var dbContext = scopedProvider.GetRequiredService<OrderingDbContext>();
    await DbInitializer.InitializeDb(dbContext);
}

app.Run();

public partial class Program { }