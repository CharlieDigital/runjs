using RunJS;

public class DatabaseFixture
{
    private static readonly string _connectionString =
        "server=127.0.0.1;port=6543;database=runjs_test;user id=postgres;password=postgres;include error detail=true;";
    private static readonly object _sync = new();
    private static bool _initialized;

    /// <summary>
    ///     Constructor which initializes the database for testing each run.
    /// </summary>
    public DatabaseFixture()
    {
        // Use simple double lock-check mechanism.
        if (_initialized)
        {
            return;
        }

        lock (_sync)
        {
            if (_initialized)
            {
                return;
            }

            using var db = CreateDbContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            _initialized = true;
        }
    }

    public static SecretsDatabase CreateDbContext()
    {
        var appConfig = new AppConfig(new DbConfig(_connectionString));

        return new SecretsDatabase(appConfig);
    }
}
