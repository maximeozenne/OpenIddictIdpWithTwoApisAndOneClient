using Client.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Client.Controllers
{
    public class HomeController : Controller
    {

        public HomeController() {}

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> StandaloneAsync()
        {
            var data = await GetData("https://localhost:5446/standalonedata");
            return View(data);
        }

        [Authorize]
        public async Task<IActionResult> DependentAsync()
        {
            var data = await GetData("https://localhost:5445/dependentdata");
            return View(data);
        }

        private async Task<IEnumerable<string>> GetData(string requestUri)
        {
            var client = new HttpClient();
            
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            using var response = await client.SendAsync(request);

            if (response?.IsSuccessStatusCode == true)
            {
                var content = await response.Content.ReadAsStringAsync();
                var reply = JsonConvert.DeserializeObject<string[]>(content);
                return reply;
            }

            return new[] { "Error while gathering data" };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}