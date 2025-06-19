using System.ComponentModel;
using Jint;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomLogging();
builder.Services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly();

// OpenTelemetry configuration for visibility (http://localhost:18888)
builder.Services.AddTelemetry();

var app = builder.Build();

app.MapMcp();

app.Run();

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

        var engine = new Engine(options =>
        {
            options.LimitMemory(1_000_000); // 1 MB
            options.TimeoutInterval(TimeSpan.FromSeconds(5));
            options.MaxStatements(500);
        });

        var result = engine.Execute(code);

        return result?.ToString() ?? "void";
    }
}
