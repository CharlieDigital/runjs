using ILogger = Serilog.ILogger;

namespace RunJS;

/// <summary>
/// Service which provides the operations for storing and retrieving encrypted
/// secrets using the <see cref="EncryptionService"/>
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
    /// <param name="readOnce">When true, it should be deleted after being read.</param>
    /// <returns>The ID of the newly created secret prefixed with `runjs:secret:`</returns>
    public async Task<string> Store(string value, bool? readOnce = null)
    {
        var encryptedValue = encryptionService.Encrypt(value);

        var id = $"runjs:secret:{Guid.NewGuid():N}";

        await secretsDatabase.Secrets.AddAsync(
            new Secret
            {
                Id = id,
                EncryptedValue = encryptedValue,
                ReadOnce = readOnce
            }
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

        if (secret.ReadOnce == true)
        {
            // Delete the secret after reading it once
            secretsDatabase.Secrets.Remove(secret);
            await secretsDatabase.SaveChangesAsync();
        }

        return encryptionService.Decrypt(secret.EncryptedValue);
    }
}
