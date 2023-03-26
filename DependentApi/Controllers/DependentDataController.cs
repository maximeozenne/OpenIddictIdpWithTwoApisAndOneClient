using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace StandaloneApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class DependentDataController : ControllerBase
{
    public DependentDataController() {}

    [HttpGet]
    public async Task<IEnumerable<string>> Get()
    {
        var reply = new List<string>()
        {
            "Dependent data"
        };

        var client = new HttpClient();
        //client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", Request.Headers[HeaderNames.Authorization].ToString());

        var accessToken = await HttpContext.GetTokenAsync("access_token");
        //var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5446/standalonedata");

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