using System.ComponentModel;
using Jint;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Setup the services
builder.AddCustomLogging();
builder.Services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly();
builder.Services.AddTelemetry(); // OpenTelemetry @ http://localhost:18888

// Build and start app
var app = builder.Build();
app.MapMcp();
app.Run();

// Example MCP tool implementations
[McpServerToolType]
public static class EchoTool
{
    private static readonly ILogger Log = Serilog.Log.ForContext(typeof(EchoTool));

    [
        McpServerTool(Name = "echo"),
        Description(
            "Echoes the message back to the client; only use this if the user explicitly asks for an echo."
        )
    ]
    public static string Echo(string message)
    {
        Log.Here().Information($"Echoing message: {message}");
        return $"Hello to you! {message}";
    }
}

[McpServerToolType]
public static class JintTool
{
    private static readonly ILogger Log = Serilog.Log.ForContext(typeof(JintTool));

    [
        McpServerTool(Name = "runJavaScript"),
        Description("Runs JavaScript code and returns the result.")
    ]
    public static string RunJavaScriptCode(
        [Description("The JavaScript code to execute; returns 'void' if there is no result")]
            string code
    )
    {
        Log.Here().Information("Running JavaScript code: {Code}", code);

        var wrappedCode =
            $@"
            function main() {{
                {code}
            }}";

        var engine = new Engine(options =>
        {
            options.LimitMemory(1_000_000); // 1 MB
            options.TimeoutInterval(TimeSpan.FromSeconds(5));
            options.MaxStatements(500);
        });

        var result = engine.Execute(wrappedCode).Invoke("main");

        Log.Here().Information("  ⮑  Executed JavaScript code, result: {Result}", result);

        return result?.ToString() ?? "void";
    }
}
