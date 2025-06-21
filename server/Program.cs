using Microsoft.EntityFrameworkCore;
using RunJS;

var builder = WebApplication.CreateBuilder(args);

// Load the configuration.
builder.Services.Configure<AppConfig>(
    builder.Configuration.GetSection(nameof(AppConfig))
);

var currentConfig = builder
    .Configuration.GetSection(nameof(AppConfig))
    .Get<AppConfig>();

if (currentConfig == null)
{
    Environment.Exit(1); // ! EXIT: Couldn't load the config.
}

// Setup the services
builder.AddCustomLogging();
builder.Services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly();
builder.Services.AddTelemetry(); // OpenTelemetry @ http://localhost:18888
builder.Services.AddControllers();
builder.Services.AddScoped<SecretsService>();
builder.Services.AddSingleton(currentConfig);
builder.Services.AddDbContext<SecretsDatabase>();
builder.Services.AddEncryption();

// Build and start app
var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetService<SecretsDatabase>()!;
db.Database.EnsureCreated();
db.Database.Migrate(); // 👈 Migrate the database if needed

app.MapControllers(); // 👈 Web API endpoints for secrets
app.MapMcp(); // 👈 MCP /sse endpointS
app.Run();
