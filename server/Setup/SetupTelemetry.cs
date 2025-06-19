using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

/// <summary>
/// Setup class to add OpenTelemetry tracing, metrics, and logging to the service
/// collection for observability of the call paths.
/// </summary>
public static class SetupTelemetryExtension
{
    public static void AddTelemetry(this IServiceCollection services)
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
                b.AddMeter("*").AddAspNetCoreInstrumentation().AddHttpClientInstrumentation()
            )
            .WithLogging()
            .UseOtlpExporter();
    }
}
