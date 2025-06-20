using Microsoft.EntityFrameworkCore;

namespace RunJS;

// This is our database.  The key is to inherit from DbContext
public partial class SecretsDatabase(AppConfig config) : DbContext
{
    // This method gets called on startup and we'll configure our database
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
