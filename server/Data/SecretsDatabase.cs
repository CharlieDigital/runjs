using Microsoft.EntityFrameworkCore;

namespace RunJS;

public partial class SecretsDatabase(AppConfig config) : DbContext
{
    /// <summary>
    /// Configure Postgres for sanity (e.g. use snake_case)
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        optionsBuilder
            .UseNpgsql(
                config.DbConfig.ConnectionString,
                o => o.UseAdminDatabase("postgres")
            )
            .UseSnakeCaseNamingConvention()
            .EnableDetailedErrors() // ⚠️ ONLY DEV CODE
            .EnableSensitiveDataLogging(); // ⚠️ ONLY DEV CODE
    }
}
