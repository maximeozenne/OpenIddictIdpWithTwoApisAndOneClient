using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        public HomeController() {}

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
            var _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync(requestUri);

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