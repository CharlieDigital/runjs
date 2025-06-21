using Microsoft.AspNetCore.DataProtection;
using ILogger = Serilog.ILogger;

namespace RunJS;

/// <summary>
/// Provides symmetric encryption for storing encrypted values as strings using .NET's built-in data protection.
/// Uses AES-256-CBC with HMAC-SHA256 for authenticated encryption.
/// </summary>
public class EncryptionService(IDataProtector dataProtector)
{
    private static readonly ILogger Log =
        Serilog.Log.ForContext<EncryptionService>();

    private readonly IDataProtector _protector = dataProtector;

    /// <summary>
    /// Encrypts a plaintext value and returns it as a Base64 encoded string.
    /// </summary>
    /// <param name="plaintext">The value to encrypt</param>
    /// <returns>Base64 encoded encrypted string</returns>
    /// <exception cref="ArgumentNullException">Thrown when plaintext is null</exception>
    public string Encrypt(string plaintext)
    {
        ArgumentNullException.ThrowIfNull(plaintext);

        try
        {
            var encrypted = _protector.Protect(plaintext);
            return encrypted;
        }
        catch (Exception ex)
        {
            Log.Here().Error(ex, "Failed to encrypt value");
            throw;
        }
    }

    /// <summary>
    /// Decrypts an encrypted string back to the original plaintext value.
    /// </summary>
    /// <param name="encryptedValue">The encrypted string to decrypt</param>
    /// <returns>The original plaintext value</returns>
    /// <exception cref="ArgumentNullException">Thrown when encryptedValue is null</exception>
    /// <exception cref="CryptographicException">Thrown when decryption fails</exception>
    public string Decrypt(string encryptedValue)
    {
        ArgumentNullException.ThrowIfNull(encryptedValue);

        try
        {
            var decrypted = _protector.Unprotect(encryptedValue);
            return decrypted;
        }
        catch (Exception ex)
        {
            Log.Here()
                .Error(
                    ex,
                    "Failed to decrypt value - data may be corrupted or key is incorrect"
                );
            throw;
        }
    }
}
