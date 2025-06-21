using Microsoft.EntityFrameworkCore;

namespace RunJS;

public partial class Database
{
    public DbSet<Secret> Secrets { get; set; } = null!;
}
