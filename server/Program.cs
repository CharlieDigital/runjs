using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RunJS;

var builder = WebApplication.CreateBuilder(args);

// Load the configuration.
builder.Services.Configure<DbConfig>(
    builder.Configuration.GetSection(nameof(DbConfig))
);

var currentConfig = builder
    .Configuration.GetSection(nameof(DbConfig))
    .Get<DbConfig>();

if (currentConfig == null)
{
    Environment.Exit(1); // ! EXIT: Couldn't load the config.
}

// Setup the services
builder.AddCustomLogging();
builder.Services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly();
builder.Services.AddTelemetry(); // OpenTelemetry @ http://localhost:18888
builder.Services.AddControllers();
builder.Services.AddSingleton(currentConfig);
builder.Services.AddDbContext<SecretsDatabase>();

// Build and start app
var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetService<SecretsDatabase>()!;
db.Database.EnsureCreated();
db.Database.Migrate(); // 👈 Migrate the database if needed

app.MapControllers(); // 👈 Web API endpoints for secrets
app.MapMcp(); // 👈 MCP /sse endpointS
app.Run();
