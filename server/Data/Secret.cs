using Microsoft.EntityFrameworkCore;

namespace RunJS;

[Index(nameof(Id))]
public class Secret
{
    /// <summary>
    /// An ID of the secret which is a GUID prefixed with `runjs:secret:`.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The encrypted value of the secret, stored as a Base64 encoded string.
    /// </summary>
    public required string EncryptedValue { get; set; }

    /// <summary>
    /// When true, the secret is deleted after being read once via the
    /// <see cref="DbSecretsService"/>.
    /// </summary>
    public bool? ReadOnce { get; set; }
}
