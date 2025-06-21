using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RunJS;

var builder = WebApplication.CreateBuilder(args);

// Setup the services
builder.AddCustomLogging();
builder.Services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly();
builder.Services.AddTelemetry(); // OpenTelemetry @ http://localhost:18888

// Build and start app
var app = builder.Build();
app.MapMcp();
app.Run();
