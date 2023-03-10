using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenIddict.Client;
using System.Net.Http.Headers;

namespace StandaloneApi.Controllers;

[ApiController]
[Route("[controller]")]
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
        var reply = new List<string>()
        {
            "Dependent data"
        };

        var client = new HttpClient();

        using var scope = _serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<OpenIddictClientService>();

        var (tokenResponse, _) = await service.AuthenticateWithClientCredentialsAsync(
            new Uri("https://localhost:5443/", UriKind.Absolute),
            new string[] { "standaloneapi" });
        var token = tokenResponse.AccessToken;

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
}