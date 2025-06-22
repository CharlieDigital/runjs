namespace RunJS;

public interface ISecretsService
{
    /// <summary>
    /// Encrypts a value and stores it in the database.
    /// </summary>
    /// <param name="value">The value to encrypt.</param>
    /// <param name="readOnce">When true, the secret will be deleted after being read.</param>
    /// <returns>The ID of the newly created secret prefixed with `runjs:secret:`</returns>
    Task<string> Store(string value, bool? readOnce = null);

    /// <summary>
    /// Retrieves a secret by its ID, decrypting the value before returning it.
    /// </summary>
    /// <param name="id">The ID of the secret prefixed with `runjs:secret:`</param>
    /// <returns>The decrypted, plaintext value of the secret.</returns>
    Task<string> Retrieve(string id);
}
