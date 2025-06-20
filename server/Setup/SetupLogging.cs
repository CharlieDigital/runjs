using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace RunJS;

/// <summary>
/// Logging setup for Serilog.
/// </summary>
public static class SetupLoggingExtension
{
    public static void AddCustomLogging(this WebApplicationBuilder builder)
    {
        // Set up logging.
        var logConfiguration = new LoggerConfiguration();

        if (builder.Environment.IsDevelopment())
        {
            logConfiguration
                .WriteTo.Console(outputTemplate: LoggingConstants.DevelopmentTemplate)
                .MinimumLevel.Debug();
        }
        else
        {
            logConfiguration
                .WriteTo.Console(outputTemplate: LoggingConstants.ProductionTemplate)
                .MinimumLevel.Debug();
        }

        logConfiguration
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);

        Log.Logger = logConfiguration.CreateLogger();

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog();
    }
}

/// <summary>
///     Static class for wrapping logging constants
/// </summary>
public static class LoggingConstants
{
    /// <summary>
    ///     The expression statement for development logging.
    /// </summary>
    /// <returns>An expression statement used for logging in development environments.</returns>
    public static readonly string DevelopmentTemplate =
        "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj} ({Here}){NewLine}{Exception}";

    /// <summary>
    ///     The expression statement for production logging.
    /// </summary>
    /// <returns>An expression statement used for logging in production environments.</returns>
    public static readonly string ProductionTemplate =
        "[{Level:u3}] {Message:lj} ({Here}){NewLine}{Exception}";

    /// <summary>
    ///     The expression statement for development logging.
    /// </summary>
    /// <returns>An expression statement used for logging in development environments.</returns>
    public static readonly string DevelopmentProcessorTemplate =
        "[{Timestamp:HH:mm:ss.fff} {Level:u3} {WorkspaceId}] {Message:lj} ({Here}){NewLine}{Exception}";

    /// <summary>
    ///     The expression statement for production logging.
    /// </summary>
    /// <returns>An expression statement used for logging in production environments.</returns>
    public static readonly string ProductionProcessorTemplate =
        "[{Level:u3} {WorkspaceId}] {Message:lj} ({Here}){NewLine}{Exception}";
}

public static class LoggerExtensions
{
    public static ILogger Here(
        this ILogger logger,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0
    )
    {
        var srcFile = Path.GetFileName(sourceFilePath);
        var here = $" {srcFile}:{memberName}@{sourceLineNumber}";

        return logger
            .ForContext("Here", here)
            .ForContext("MemberName", memberName)
            .ForContext("FilePath", sourceFilePath)
            .ForContext("LineNumber", sourceLineNumber);
    }
}
