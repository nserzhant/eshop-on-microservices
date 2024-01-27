using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using EShop.EmployeeManagement.AuthorizationServer;
using EShop.EmployeeManagement.AuthorizationServer.Data;
using EShop.EmployeeManagement.Core;
using System.Text;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.AspNetCore.HttpOverrides;
using EShop.Client.AuthorizationServer.Helpers;

var builder = WebApplication.CreateBuilder(args);
//Get settings to configure JWT tokens

var configuration = builder.Configuration;
var jwtSecretKey = configuration[Consts.JWT_SECRET_KEY_CONFIG_NAME] ?? throw new ArgumentNullException(Consts.JWT_SECRET_KEY_CONFIG_NAME);
builder.Services.AddEmployeeServices(builder.Configuration);
var accessTokenLifetime = configuration.GetValue<int>(Consts.ACCESS_TOKEN_LIFETIME_CONFIG_NAME);

//Configure OpenIdDict
var signingCertificateThumbprint = configuration[Consts.SIGNING_CERTIFICATE_THUMBPRINT_CONFIG_NAME];
var encryptionCertificateThumbprint = configuration[Consts.ENCRYPTION_CERTIFICATE_THUMBPRINT_CONFIG_NAME];
var useEphemeralKeys = configuration.GetValue(Consts.USE_EPHEMERAL_KEYS, false);

//Configure settings for Client apps
var clientConfigurations = builder.Configuration.GetSection(Consts.CLIENT_CONFIGURATION_CONFIG_NAME);
builder.Services.Configure<List<ClientConfiguration>>(clientConfigurations);
var clients = clientConfigurations.Get<List<ClientConfiguration>>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<OpenIDDictDbContext>(options =>
{
    // Configure the context to use an in-memory store.
    options.UseInMemoryDatabase(nameof(OpenIDDictDbContext));

    // Register the entity sets needed by OpenIddict.
    options.UseOpenIddict();
});

// Register the worker responsible for creating and seeding the SQL database.
// Note: in a real world application, this step should be part of a setup script.
builder.Services.AddHostedService<Worker>();


// OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
// (like pruning orphaned authorizations/tokens from the database) at regular intervals.
builder.Services.AddQuartz(options =>
{
    options.UseMicrosoftDependencyInjectionJobFactory();
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});


// Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddOpenIddict()

        // Register the OpenIddict core components.
        .AddCore(options =>
        {
            // Configure OpenIddict to use the EF Core stores/models.
            options.UseEntityFrameworkCore()
                .UseDbContext<OpenIDDictDbContext>();

            // Enable Quartz.NET integration.
            options.UseQuartz();
        })

        // Register the OpenIddict server components.
        .AddServer(options =>
        {
            options
                .AllowAuthorizationCodeFlow()
                .RequireProofKeyForCodeExchange()
                .AllowRefreshTokenFlow();

            options
                .SetAuthorizationEndpointUris("connect/authorize")
                .SetTokenEndpointUris("connect/token")
                .SetLogoutEndpointUris("connect/logout");

            options
                .AddSigningKey(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSecretKey)))
                .DisableAccessTokenEncryption();

            if (signingCertificateThumbprint != null)
            {
                options.AddSigningCertificate(CertificatesHelper.FindCertificate(signingCertificateThumbprint));
            }
            else if (builder.Environment.IsDevelopment())
            {
                if (useEphemeralKeys)
                {
                    options.AddEphemeralSigningKey();
                }
                else { options.AddDevelopmentSigningCertificate(); }
            }

            if (encryptionCertificateThumbprint != null)
            {
                options.AddEncryptionCertificate(CertificatesHelper.FindCertificate(encryptionCertificateThumbprint));
            }
            else if (builder.Environment.IsDevelopment())
            {
                if (useEphemeralKeys)
                {
                    options.AddEphemeralEncryptionKey();
                }
                else { options.AddDevelopmentEncryptionCertificate(); }
            }

            // Register scopes (permissions)
            options.RegisterScopes("api", Scopes.Roles);

            // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
            options
                .UseAspNetCore()
                .EnableTokenEndpointPassthrough()
                .EnableAuthorizationEndpointPassthrough()
                .EnableLogoutEndpointPassthrough()
                .DisableTransportSecurityRequirement();

            options.SetAccessTokenLifetime(TimeSpan.FromSeconds(accessTokenLifetime));

        });

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    var clientOrigins = clients?.Select(client => client.clientOrigin)?.ToArray();

    if (clientOrigins != null)
        builder.WithOrigins(clientOrigins).AllowAnyMethod().AllowAnyHeader();
}));

// Configure services for HTTP ingress
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


var applicationPathBase = Environment.GetEnvironmentVariable(Consts.APP_BASE_PATH_ENVIRONMENT_NAME);

// Use base path when HTTP Ingress is used to have correct openid-configuration links
// generated by the OpenIddict library.
app.UsePathBase(new PathString($"/{applicationPathBase}"));
app.UseHttpLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("corsapp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
