using Microsoft.AspNetCore.Builder;
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
builder.Services.AddSingleton(new DbConfig(""));
builder.Services.AddDbContext<Database>();

// Build and start app
var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetService<Database>()!;
db.Database.EnsureCreated();

app.MapControllers(); // 👈 Web API endpoints for secrets
app.MapMcp(); // 👈 MCP /sse endpointS
app.Run();
