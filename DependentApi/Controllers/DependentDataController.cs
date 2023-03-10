using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using OpenIddict.Client;
using OpenIddict.Validation.AspNetCore;
using System.Net.Http.Headers;

namespace StandaloneApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class DependentDataController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public DependentDataController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [HttpGet]
    public async Task<IEnumerable<string>> Get()
    {
        var token = GetToken();

        var reply = new List<string>()
        {
            "Dependent data"
        };

        var client = new HttpClient();

        if (token is null)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<OpenIddictClientService>();

            var (tokenResponse, _) = await service.AuthenticateWithClientCredentialsAsync(
                new Uri("https://localhost:5443/", UriKind.Absolute),
                new string[] { "standaloneapi" });
            token = tokenResponse.AccessToken;

        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5446/standalonedata");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request);

        if (response?.IsSuccessStatusCode == true)
        {
            var content = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<string>>(content);
            reply.AddRange(list);
        }
        else
        {
            reply.Add("Error while gathering standalone data");
        }

        return reply;
    }

    private string GetToken()
    {
        var tokenStringValues = Request.Headers[HeaderNames.Authorization];
        if (StringValues.IsNullOrEmpty(tokenStringValues)) return null;

        var token = tokenStringValues.ToString();
        token = token.Replace("Bearer ", "");

        return token;
    }
}