using Microsoft.EntityFrameworkCore;

namespace RunJS;

[Index(nameof(Id))]
public class Secret
{
    public required string Id { get; set; }

    public required string EncryptedValue { get; set; }
}
