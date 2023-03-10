using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;
using System.Net.Http.Headers;
using OpenIddict.Client;

namespace Client.Controllers
{
    public class HomeController : Controller
    {

        private readonly IServiceProvider _serviceProvider;

        public HomeController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> StandaloneAsync()
        {
            var data = await GetData("https://localhost:5446/standalonedata") ?? new[] { "Error while gathering data" };
            return View(data);
        }

        public async Task<IActionResult> Dependent()
        {
            var data = await GetData("https://localhost:5445/dependentdata") ?? new[] { "Error while gathering data" };
            return View(data);
        }

        private async Task<IEnumerable<string>> GetData(string requestUri)
        {
            //using var client = provider.GetRequiredService<HttpClient>();
            var client = new HttpClient();

            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<OpenIddictClientService>();

            var (tokenResponse, _) = await service.AuthenticateWithClientCredentialsAsync(new Uri("https://localhost:5443/", UriKind.Absolute));
            var token = tokenResponse.AccessToken;

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await client.SendAsync(request);

            if (response?.IsSuccessStatusCode == true)
            {
                var content = await response.Content.ReadAsStringAsync();
                var reply = JsonConvert.DeserializeObject<string[]>(content);
                return reply;
            }

            return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}