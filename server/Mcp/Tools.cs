using System.ComponentModel;
using System.Threading.Tasks;
using Jint;
using ModelContextProtocol.Server;
using ILogger = Serilog.ILogger;

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
            """
                Runs JavaScript code and returns the result.
                Wrap the code statements in an IIFE like so:
                (async () => { /* YOUR CODE STATEMENTS HERE */ })().
                The last statement should return a value.
                If you need to access a web API, you can access it via `fetch`.
                Be sure to handle promises with async/await
                Call .json() or .text() on the response to get the payload.
                If the API call requires a secret header, use a placeholder for the value.
                The secret placeholder will be in the form `runjs:secret:<GUID>`.
                Be sure to return a value at the end.

                The user may provide a secret ID for using an API.
                If present, it will have the format `runjs:secret:<GUID>`.
                If it is not present, ignore it and the secretId is an empty string.
                """
        )
    ]
    public static async Task<string> RunJavaScriptCode(
        [Description(
            "The JavaScript code to execute; returns 'void' if there is no result."
        )]
            string code,
        [Description(
            "A secret ID to use for fetching API keys if provided by the user."
        )]
            string secretId,
        SecretsService secretsService
    )
    {
        Log.Here().Information("Running JavaScript code: {Code}", code);

        if (!string.IsNullOrWhiteSpace(secretId))
        {
            code = code.Replace(secretId, await secretsService.Retrieve(secretId));
        }

        var engine = new Engine(options =>
        {
            options.LimitMemory(5_000_000); // 5 MB
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
