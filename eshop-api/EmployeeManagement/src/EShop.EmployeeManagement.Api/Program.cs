using EShop.EmployeeManagement.Api;
using EShop.EmployeeManagement.Api.Data;
using EShop.EmployeeManagement.Api.Filters;
using EShop.EmployeeManagement.Api.Settings;
using EShop.EmployeeManagement.Core;
using EShop.EmployeeManagement.Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

// Get settings to configure JWT tokens

var jwtSettingsSection = builder.Configuration.GetSection(Consts.JWT_CONFIG_NAME);
var jwtSettings = new JWTSettings();

jwtSettingsSection.Bind(jwtSettings);

// Configure settings for Client app
var clientOrigin = builder.Configuration[Consts.SPA_CLIENT_ORIGIN_CONFIG_NAME];

// Setting to init Db with test data on startup
var initDbOnStartup = builder.Configuration.GetValue<bool>(Consts.INIT_DB_ON_STARTUP_CONFIG_NAME, false);

// Add services to the container.

builder.Services.Configure<JWTSettings>(jwtSettingsSection);
builder.Services.AddControllers(options => options.Filters.Add<EmployeeDomainExceptionFilter>());
builder.Services.AddEmployeeServices(builder.Configuration);

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
    if (clientOrigin != null)
        builder.WithOrigins(clientOrigin).AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddHttpLogging(options => new HttpLoggingOptions());

var app = builder.Build();

app.UseHttpLogging();
//// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
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
    var dbContext = scopedProvider.GetRequiredService<EmployeeDbContext>();
    var userManager = scopedProvider.GetRequiredService<UserManager<Employee>>();
    await DbInitializer.InitializeDbWIthTestData(userManager, dbContext);
}

app.Run();
public partial class Program { }