using Microsoft.EntityFrameworkCore;
using RunJS;

var builder = WebApplication.CreateBuilder(args);

// Load the configuration.
builder.Services.Configure<AppConfig>(
    builder.Configuration.GetSection("RunJSConfig")
);

var currentConfig = builder
    .Configuration.GetSection("RunJSConfig")
    .Get<AppConfig>();

if (currentConfig == null)
{
    Environment.Exit(1); // ! EXIT: Couldn't load the config.
}

// Setup the services
builder.AddCustomLogging();
builder.Services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly();
builder.Services.AddControllers();
builder
    .Services.AddTelemetry() // OpenTelemetry @ http://localhost:18888
    .AddSingleton(currentConfig)
    .AddEncryption()
    .AddSecretsServices(currentConfig)
    .AddResilience();

// Build and start app
var app = builder.Build();

if (currentConfig.Secrets.UseDatabase)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetService<SecretsDatabase>()!;
    db.Database.EnsureCreated();
    db.Database.Migrate(); // 👈 Migrate the database if needed
}

app.MapControllers(); // 👈 Web API endpoints for secrets
app.MapMcp(); // 👈 MCP /sse endpointS
app.Run();
