using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace StandaloneApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DependentDataController : ControllerBase
{
    public DependentDataController() { }

    [HttpGet]
    public async Task<IEnumerable<string>> Get()
    {
        var reply = new List<string>()
        {
            "Dependent data"
        };
        var _httpClient = new HttpClient();

        var response = await _httpClient.GetAsync("https://localhost:5446/standalonedata");

        if (response?.IsSuccessStatusCode == true)
        {
            var content =  await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<string>>(content);
            reply.AddRange(list);
        }

        return reply;
    }
}