using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using EShop.Client.AuthorizationServer.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EShop.Client.AuthorizationServer;

public class Worker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<List<ClientConfiguration>> _clientConfigurations;

    public Worker(IServiceProvider serviceProvider, IOptions<List<ClientConfiguration>> clientConfigurations)
    {
        _serviceProvider = serviceProvider;
        _clientConfigurations = clientConfigurations;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<OpenIDDictDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        _clientConfigurations.Value.ForEach(async clientConfiguration =>
        {
            if (await manager.FindByClientIdAsync(clientConfiguration.clientId, cancellationToken) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = clientConfiguration.clientId,
                    Type = ClientTypes.Public,
                    DisplayName = clientConfiguration.displayName,
                    RedirectUris = { new Uri($"{clientConfiguration.clientOrigin}/login-callback") },
                    PostLogoutRedirectUris = { new Uri($"{clientConfiguration.clientOrigin}/") },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.Logout,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.Prefixes.Scope + "api",
                        Permissions.Prefixes.Scope + Scopes.Roles,
                        Permissions.ResponseTypes.Code
                    }
                }, cancellationToken);

            }
        });
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}