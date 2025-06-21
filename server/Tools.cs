using System.ComponentModel;
using Jint;
using ModelContextProtocol.Server;
using Serilog;

namespace RunJS;

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
        Description(
            "Runs JavaScript code and returns the result."
                + "Wrap the code statements in an IIFE like so and return a value "
                + "if needed: (async () => { /* YOUR CODE STATEMENTS HERE */ })(). "
                + "If you need to access a web API, you can access it via `fetch`. "
                + "Be sure to handle promises with async/await and call .json() or "
                + ".text() on the response to get the payload. Be sure to return "
                + "a value"
        )
    ]
    public static string RunJavaScriptCode(
        [Description(
            "The JavaScript code to execute; returns 'void' if there is no result."
        )]
            string code
    )
    {
        Log.Here().Information("Running JavaScript code: {Code}", code);

        var engine = new Engine(options =>
        {
            options.LimitMemory(4_000_000); // 4 MB
            options.TimeoutInterval(TimeSpan.FromSeconds(5));
            options.MaxStatements(500);
            options.ExperimentalFeatures = ExperimentalFeature.TaskInterop;
        });

        using var client = new FetchHttpClient(engine);

        engine.SetValue("fetch", client.Fetch);

        var result = engine.Evaluate(code).UnwrapIfPromise();

        Log.Here()
            .Information("  ⮑  Executed JavaScript code, result: {Result}", result);

        return result?.ToString() ?? "void";
    }
}
