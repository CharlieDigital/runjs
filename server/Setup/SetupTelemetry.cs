using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace RunJS;

/// <summary>
/// Setup class to add OpenTelemetry tracing, metrics, and logging to the service
/// collection for observability of the call paths.
/// </summary>
public static class SetupTelemetryExtension
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services)
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("runjs:server"))
            .WithTracing(b =>
                b.AddSource("*")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .ConfigureResource(r => r.AddService("runjs:server:http"))
            )
            .WithMetrics(b =>
                b.AddMeter("*")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
            )
            .WithLogging()
            .UseOtlpExporter();

        return services;
    }
}
