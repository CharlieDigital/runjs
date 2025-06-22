namespace RunJS;

/// <summary>
/// A `Dictionary`-based implementation of the `ISecretsService` interface.
/// </summary>
public class MemorySecretsService(EncryptionService encryptionService)
    : ISecretsService
{
    private readonly Dictionary<string, EncryptedSecret> _secrets = [];

    /// <summary>
    /// Store the secret in memory after encrypting it with the
    /// <see cref="EncryptionService"/>.
    /// </summary>
    /// <param name="value">The value to encrypt.</param>
    /// <param name="readOnce">True if this value should only be read once.</param>
    /// <returns>The ID of the secret.</returns>
    public Task<string> Store(string value, bool? readOnce = null)
    {
        var encryptedValue = encryptionService.Encrypt(value);

        var id = $"runjs:secret:{Guid.NewGuid():N}";

        _secrets[id] = new EncryptedSecret(encryptedValue, readOnce == true);

        return Task.FromResult(id);
    }

    /// <summary>
    /// Retrieves a decrypted secret by its ID and removes it if it is marked as
    /// `readOnce`.
    /// </summary>
    /// <param name="id">The ID of the secret to retrieve.</param>
    /// <returns>The decrypted value of the secret.</returns>
    public Task<string> Retrieve(string id)
    {
        if (_secrets.TryGetValue(id, out var secret))
        {
            var decryptedValue = encryptionService.Decrypt(secret.EncryptedValue);

            if (secret.ReadOnce)
            {
                _secrets.Remove(id);
            }

            return Task.FromResult(decryptedValue);
        }

        throw new KeyNotFoundException($"Secret with ID '{id}' not found.");
    }
}

record EncryptedSecret(string EncryptedValue, bool ReadOnce);
