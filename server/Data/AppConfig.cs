namespace RunJS;

public record AppConfig(DbConfig Db, JintConfig Jint);

public record DbConfig(string ConnectionString);

public record JintConfig(
    int LimitMemory = 5_000_000,
    int TimeoutIntervalSeconds = 10,
    int MaxStatements = 100
);
