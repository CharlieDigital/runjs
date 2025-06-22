using ILogger = Serilog.ILogger;

namespace RunJS;

public static class SetupSecretsExtensions
{
    private static readonly ILogger Log = Serilog.Log.ForContext(
        typeof(SetupSecretsExtensions)
    );

    /// <summary>
    /// Adds the necessary services for handling encrypted secrets.
    /// </summary>
    public static IServiceCollection AddSecretsServices(
        this IServiceCollection services,
        AppConfig config
    )
    {
        if (config.Secrets.UseDatabase)
        {
            Log.Here().Information("Using database for secrets storage");

            services.AddScoped<ISecretsService, DbSecretsService>();
            services.AddDbContext<SecretsDatabase>();
        }
        else
        {
            Log.Here().Information("Using memory for secrets storage");

            services.AddSingleton<ISecretsService, MemorySecretsService>();
        }

        return services;
    }
}
