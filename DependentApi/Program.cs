using OpenIddict.Client;
using OpenIddict.Server.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
});

builder.Services
    .AddOpenIddict()
    .AddValidation(options =>
    {
        // Note: the validation handler uses OpenID Connect discovery
        // to retrieve the address of the introspection endpoint.
        options.SetIssuer("https://localhost:5443/");
        options.AddAudiences("dependentapi");

        // Configure the validation handler to use introspection and register the client
        // credentials used when communicating with the remote introspection endpoint.
        options.UseIntrospection()
                .SetClientId("dependentapi")
                .SetClientSecret("dependentapi-secret");

        // disable access token encyption for this
        options.UseAspNetCore();

        // Register the System.Net.Http integration.
        options.UseSystemNetHttp();
    })
    // Register the OpenIddict client components.
    .AddClient(options =>
    {
        // Allow grant_type=client_credentials to be negotiated.
        options.AllowClientCredentialsFlow();

        // Disable token storage, which is not necessary for non-interactive flows like
        // grant_type=password, grant_type=client_credentials or grant_type=refresh_token.
        options.DisableTokenStorage();

        // Register the System.Net.Http integration and use the identity of the current
        // assembly as a more specific user agent, which can be useful when dealing with
        // providers that use the user agent as a way to throttle requests (e.g Reddit).
        options.UseSystemNetHttp()
               .SetProductInformation(typeof(Program).Assembly);

        // Add a client registration matching the client application definition in the server project.
        var registration = new OpenIddictClientRegistration
        {
            Issuer = new Uri("https://localhost:5443/", UriKind.Absolute),

            ClientId = "dependentapi",
            ClientSecret = "dependentapi-secret"
        };
        registration.Scopes.Add("dependentapi");
        options.AddRegistration(registration);
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
