namespace RunJS;

public record AppConfig(DbConfig Db, JintConfig Jint, SecretsConfig Secrets);

public record DbConfig(string ConnectionString);

public record SecretsConfig(bool UseDatabase = false);

public record JintConfig(
    int LimitMemory = 5_000_000,
    int TimeoutIntervalSeconds = 10,
    int MaxStatements = 1000
);
