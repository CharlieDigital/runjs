using System.ComponentModel;
using Jint;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly();

// OpenTelemetry configuration for visibility (http://localhost:18888)
builder
    .Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("runjs:server"))
    .WithTracing(b =>
        b.AddSource("*")
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .ConfigureResource(r => r.AddService("runjs:server:http"))
    )
    .WithMetrics(b => b.AddMeter("*").AddAspNetCoreInstrumentation().AddHttpClientInstrumentation())
    .WithLogging()
    .UseOtlpExporter();

var app = builder.Build();

app.MapMcp();

app.Run();

[McpServerToolType]
public static class EchoTool
{
    [
        McpServerTool(Name = "echo"),
        Description(
            "Echoes the message back to the client; only use this if the user explicitly asks for an echo."
        )
    ]
    public static string Echo(string message)
    {
        Console.WriteLine($"Echoing message: {message}");
        return $"Hello to you! {message}";
    }
}

[McpServerToolType]
public static class JintTool
{
    [
        McpServerTool(Name = "runJavaScript"),
        Description("Runs JavaScript code and returns the result.")
    ]
    public static string RunJavaScriptCode(
        [Description("The JavaScript code to execute; returns 'void' if there is no result")]
            string code
    )
    {
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
