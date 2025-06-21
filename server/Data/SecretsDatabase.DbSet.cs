using Microsoft.EntityFrameworkCore;

namespace RunJS;

public partial class SecretsDatabase
{
    public DbSet<Secret> Secrets { get; set; } = null!;
}
