using Microsoft.AspNetCore.Mvc;

namespace RunJS;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Returns a simple health check response.
    /// </summary>
    /// <returns>A string indicating the service is healthy.</returns>
    [HttpGet("")]
    public IActionResult GetHealth()
    {
        return Ok(DateTime.UtcNow.ToString("o"));
    }
}
