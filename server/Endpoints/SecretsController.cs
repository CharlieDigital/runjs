using Microsoft.AspNetCore.Mvc;

namespace RunJS;

/// <summary>
/// Controller endpoints for managing secrets.  This allows passing secret handles
/// to the LLM which will get replaced with actual secret keys.
/// </summary>
[ApiController]
[Route("[controller]")]
public class SecretsController : ControllerBase
{
    [HttpGet("{secretId}")]
    public string GetSecret(string secretId) =>
        $"It's supposed to be a secret! {secretId}";
}
