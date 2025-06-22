using System.ComponentModel;
using System.Text.RegularExpressions;
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
public static partial class JintTool
{
    private static readonly ILogger Log = Serilog.Log.ForContext(typeof(JintTool));

    [
        McpServerTool(Name = "runJavaScript"),
        Description(
            """
                Runs generated JavaScript code that returns a result.
                ALWAYS wrap the code statements in an IIFE like so: (async () => { /* YOUR CODE STATEMENTS HERE */ })().
                The last statement should be a return statement returning a value.
                If you need to access a web API, you can access it via `fetch`.
                Call .json() or .text() on the response to get the payload.
                Be sure to handle promises with async/await syntax!

                You may also use jsonpath-plus library to query to extract values using JSONPath.
                You should use the JSONPath like this: JSONPath.JSONPath({path: '<JSON_PATH_QUERY_HERE>', json: <JSON_OBJECT>});
                """
        )
    ]
    public static async Task<string> RunJavaScriptCode(
        [Description(
            "The JavaScript code to execute; returns 'void' if there is no result."
        )]
            string code,
        ISecretsService secretsService,
        AppConfig appConfig
    )
    {
        var secretIds = ExtractAllSecretIds(code).ToList();

        Log.Here()
            .Information(
                "Running JavaScript code: {Code}\n\n  ⮑  With secret IDs 🔑: {SecretIds}",
                code,
                secretIds
            );

        foreach (var secretId in secretIds)
        {
            // TODO: Optimize this if there are many secret IDs
            code = code.Replace(secretId, await secretsService.Retrieve(secretId));
        }

        var hasJsonPathQuery = code.Contains(
            "JSONPath",
            StringComparison.OrdinalIgnoreCase
        );

        var engine = new Engine(options =>
        {
            options.LimitMemory(appConfig.Jint.LimitMemory);
            options.TimeoutInterval(
                TimeSpan.FromSeconds(appConfig.Jint.TimeoutIntervalSeconds)
            );
            options.MaxStatements(appConfig.Jint.MaxStatements);
            options.ExperimentalFeatures = ExperimentalFeature.TaskInterop;

            if (hasJsonPathQuery)
            {
                options.EnableModules(
                    Path.Join(Directory.GetCurrentDirectory(), "Mcp/libs")
                );
            }
        });

        using var client = new FetchHttpClient();

        engine.SetValue("fetch", client.Fetch);

        if (hasJsonPathQuery)
        {
            // Register the JSONPath library if needed
            engine.Modules.Import("./jsonpath-plus.browser-esm.min.js");
        }

        var result = engine.Evaluate(code).UnwrapIfPromise();

        Log.Here()
            .Information("  ⮑  Executed JavaScript code, result: {Result}", result);

        return result?.ToString() ?? "void";
    }

    /// <summary>
    /// Regular expression to match secret IDs in the format `runjs:secret:<GUID_N_FORMAT>`
    /// where GUID is 32 alphanumeric characters without dashes
    /// </summary>
    [GeneratedRegex(@"runjs:secret:\w{32}", RegexOptions.Compiled)]
    private static partial Regex SecretIdRegex();

    /// <summary>
    /// Extracts all secret IDs from the provided code string
    /// </summary>
    /// <param name="code">The code string to search for secret IDs</param>
    /// <returns>Collection of all secret IDs found</returns>
    public static IEnumerable<string> ExtractAllSecretIds(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            Log.Here()
                .Debug("Code string is null or empty, no secret IDs to extract");
            return [];
        }

        var matches = SecretIdRegex().Matches(code);
        var secretIds = matches.Select(m => m.Value).ToList();

        Log.Here().Debug("Found {Count} secret IDs in code", secretIds.Count);

        return secretIds;
    }
}
