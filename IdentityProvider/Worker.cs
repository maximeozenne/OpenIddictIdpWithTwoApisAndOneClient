using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityProvider;

public class Worker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public Worker(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Register Standalone Api
        if (await manager.FindByClientIdAsync("standaloneapi") is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "standaloneapi",
                ClientSecret = "standaloneapi-secret",
                DisplayName = "Standalone Api Ressource Server",
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Revocation,
                    Permissions.Endpoints.Introspection,

                    Permissions.GrantTypes.ClientCredentials,

                    Permissions.Prefixes.Scope + "standaloneapi"
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            });
        }

        // Register Dependent Api
        if (await manager.FindByClientIdAsync("dependentapi") is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "dependentapi",
                ClientSecret = "dependentapi-secret",
                DisplayName = "Dependent Api Ressource Server",
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Revocation,
                    Permissions.Endpoints.Introspection,

                    Permissions.GrantTypes.ClientCredentials,

                    Permissions.Prefixes.Scope + "dependentapi",
                    Permissions.Prefixes.Scope + "standaloneapi"
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            });
        }

        // Register Client
        if (await manager.FindByClientIdAsync("client") is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "client",
                ClientSecret = "client-secret",
                DisplayName = "Client Application",
                PostLogoutRedirectUris =
                {
                    new Uri("https://localhost:5443/authentication/logout-callback")
                },
                RedirectUris =
                {
                    new Uri("https://localhost:5443/authentication/login-callback")
                },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Logout,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Revocation,
                    Permissions.Endpoints.Introspection,

                    Permissions.GrantTypes.ClientCredentials,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,

                    Permissions.ResponseTypes.Code,

                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "standaloneapi",
                    Permissions.Prefixes.Scope + "dependentapi"
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            });
        }

        // Register Postman
        if (await manager.FindByClientIdAsync("postman") is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "postman",
                ClientSecret = "postman-secret",
                DisplayName = "Postman",
                RedirectUris =
                {
                    new Uri("https://oauth.pstmn.io/v1/callback")
                },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Introspection,

                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.ClientCredentials,
                    Permissions.GrantTypes.RefreshToken,

                    Permissions.ResponseTypes.Code,

                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "standaloneapi",
                    Permissions.Prefixes.Scope + "dependentapi"
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            });
        }

        // Register Standalone Scope
        if (await scopeManager.FindByNameAsync("standaloneapi") is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "standaloneapi",
                DisplayName = "Standalone API access",
                Resources =
                {
                    "standaloneapi"
                },
                
            });
        }

        // Register Dependent Scope
        if (await scopeManager.FindByNameAsync("dependentapi") is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "dependentapi",
                DisplayName = "Dependent API access",
                Resources =
                {
                    "dependentapi"
                }
            });
        }

        // Register Test User
        if (await userManager.FindByNameAsync("testuser") is null)
        {
            var testuser = new IdentityUser
            {
                UserName = "testuser"
            };

            var result = await userManager.CreateAsync(testuser, "testpassword");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
