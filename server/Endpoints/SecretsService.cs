using ILogger = Serilog.ILogger;

namespace RunJS;

/// <summary>
/// Provides symmetric encryption for storing encrypted values as strings using .NET's built-in data protection.
/// Uses AES-256-CBC with HMAC-SHA256 for authenticated encryption.
/// </summary>
public class SecretsService(
    EncryptionService encryptionService,
    SecretsDatabase secretsDatabase
)
{
    private static readonly ILogger Log = Serilog.Log.ForContext<SecretsService>();

    /// <summary>
    /// Encrypts a value using the data protection service and stores it in the
    /// database.
    /// </summary>
    /// <param name="value">The value to encrypt.</param>
    /// <returns>The ID of the newly created secret prefixed with `runjs:secret:`</returns>
    public async Task<string> Store(string value)
    {
        var encryptedValue = encryptionService.Encrypt(value);

        var id = $"runjs:secret:{Guid.NewGuid():N}";

        await secretsDatabase.Secrets.AddAsync(
            new Secret { Id = id, EncryptedValue = encryptedValue }
        );

        await secretsDatabase.SaveChangesAsync();

        return id;
    }

    /// <summary>
    /// Retrieves a secret by its ID, decrypting the value before returning it.
    /// </summary>
    /// <param name="id">The ID of the secret prefixed with `runjs:secret:`</param>
    /// <returns>The decrypted, plaintext value of the secret.</returns>
    public async Task<string> Retrieve(string id)
    {
        var secret = await secretsDatabase.Secrets.FindAsync(id);

        if (secret == null)
        {
            Log.Here().Warning("Secret with ID {Id} not found", id);
            throw new KeyNotFoundException($"Secret with ID {id} not found");
        }

        return encryptionService.Decrypt(secret.EncryptedValue);
    }
}
