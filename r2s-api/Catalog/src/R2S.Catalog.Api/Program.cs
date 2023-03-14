using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;
using R2S.Catalog.Api.Constants;
using R2S.Catalog.Api.Filters;
using R2S.Catalog.Api.Settings;
using R2S.Catalog.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Get settings to configure JWT tokens

var employeeJwtSettingsSection = builder.Configuration.GetSection(ConfigurationKeys.EMPLOYEE_JWT_CONFIG_NAME);
var employeeJwtSettings = new JWTSettings();
employeeJwtSettingsSection.Bind(employeeJwtSettings);

var clientJwtSettingsSection = builder.Configuration.GetSection(ConfigurationKeys.CLIENT_JWT_CONFIG_NAME);
var clientJwtSettings = new JWTSettings();
clientJwtSettingsSection.Bind(clientJwtSettings);

// Configure settings for Client app
var allowedClients = builder.Configuration[ConfigurationKeys.SPA_CLIENT_IP_CONFIG_NAME];

// Add services to the container.
builder.Services.AddCatalogServices(builder.Configuration);

builder.Services.AddAuthentication()
    .AddJwtBearer(AuthenticationSchemeNames.Employee, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.FromSeconds(0),
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidAudience = employeeJwtSettings.Audience,
            ValidIssuer = employeeJwtSettings.Issuer,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(employeeJwtSettings.JWTSecretKey)),
            ValidateIssuerSigningKey = true
        };
    })
    .AddJwtBearer(AuthenticationSchemeNames.Client, options =>
     {
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ClockSkew = TimeSpan.FromSeconds(0),
             ValidateIssuer = true,
             ValidateAudience = false,
             ValidAudience = clientJwtSettings.Audience,
             ValidIssuer = clientJwtSettings.Issuer,
             RequireExpirationTime = true,
             ValidateLifetime = true,
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clientJwtSettings.JWTSecretKey)),
             ValidateIssuerSigningKey = true
         };
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
    if (allowedClients != null)
        builder.WithOrigins(allowedClients).AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
    app.UseCors("corsapp");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program { }