using Microsoft.AspNetCore.Mvc;

namespace StandaloneApi.Controllers;

[ApiController]
[Route("[controller]")]
public class StandaloneDataController : ControllerBase
{
    public StandaloneDataController() {}

    [HttpGet]
    public IEnumerable<string> Get() => new List<string>() { "Standalone data" };
}