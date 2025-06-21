using Microsoft.AspNetCore.Mvc;

namespace RunJS;

/// <summary>
/// Controller endpoints for managing secrets.  This allows passing secret handles
/// to the LLM which will get replaced with actual secret keys.
/// </summary>
[ApiController]
[Route("[controller]")]
public class SecretsController(
    SecretsDatabase database,
    SecretsService secretsService
) : ControllerBase
{
    /// <summary>
    /// Adds a secret to the backend secret store and returns the ID of the secret.
    /// </summary>
    /// <param name="value">The value to add to the secret store; will be encrypted.</param>
    /// <returns>The ID of the secret.</returns>
    [HttpPost("")]
    public async Task<string> AddSecret([FromBody] string value)
    {
        var encryptedValue = secretsService.Encrypt(value);

        var id = $"runjs:secret:{Guid.NewGuid():N}";

        await database.Secrets.AddAsync(
            new Secret { Id = id, EncryptedValue = encryptedValue }
        );

        await database.SaveChangesAsync();

        return id;
    }
}
