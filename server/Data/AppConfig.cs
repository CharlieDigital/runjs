namespace RunJS;

public record AppConfig(DbConfig DbConfig);

public record DbConfig(string ConnectionString);
