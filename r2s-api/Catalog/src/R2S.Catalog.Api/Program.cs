using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;
using R2S.Catalog.Api.Constants;
using R2S.Catalog.Api.Filters;
using R2S.Catalog.Api.Settings;
using R2S.Catalog.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Get settings to configure JWT tokens

var jwtSettingsSection = builder.Configuration.GetSection(ConfigurationKeys.JWT_CONFIG_NAME);
var jwtSettings = new JWTSettings();

jwtSettingsSection.Bind(jwtSettings);

// Configure settings for Client app
var allowedClients = builder.Configuration[ConfigurationKeys.SPA_CLIENT_IP_CONFIG_NAME];

// Add services to the container.
builder.Services.AddCatalogServices(builder.Configuration);
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
        ValidateAudience = false,
        ValidAudience = jwtSettings.Audience,
        ValidIssuer = jwtSettings.Issuer,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.JWTSecretKey)),
        ValidateIssuerSigningKey = true
    };
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