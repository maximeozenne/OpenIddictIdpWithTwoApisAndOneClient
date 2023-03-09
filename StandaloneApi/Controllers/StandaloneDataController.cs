using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace StandaloneApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class StandaloneDataController : ControllerBase
{
    public StandaloneDataController() {}

    [HttpGet]
    public IEnumerable<string> Get() => new List<string>() { "Standalone data" };
}