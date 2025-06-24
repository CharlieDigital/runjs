using Polly;
using Polly.Retry;

namespace RunJS;

public static class SetupResilienceExtension
{
    public static IServiceCollection AddResilience(this IServiceCollection services)
    {
        var strategy = new RetryStrategyOptions<HttpResponseMessage>
        {
            // The number of retry attempts
            MaxRetryAttempts = 4,
            // The predicate that determines whether the result should be handled by the pipeline
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .HandleResult(r =>
                    r.StatusCode == System.Net.HttpStatusCode.TooManyRequests
                )
                .Handle<HttpRequestException>(),
            // A delay generation strategy that reads the Retry-After header.
            DelayGenerator = static args =>
            {
                if (
                    args.Outcome.Result is HttpResponseMessage responseMessage
                    && TryGetDelay(responseMessage, out TimeSpan delay)
                )
                {
                    return new ValueTask<TimeSpan?>(delay);
                }

                // Returning null means the retry strategy will use its internal delay for this attempt.
                return new ValueTask<TimeSpan?>((TimeSpan?)null);

                static bool TryGetDelay(
                    HttpResponseMessage response,
                    out TimeSpan delay
                )
                {
                    if (
                        response.Headers.TryGetValues("Retry-After", out var values)
                    )
                    {
                        delay = TimeSpan.FromSeconds(
                            int.Parse(values.FirstOrDefault() ?? "0")
                        );

                        return true;
                    }

                    delay = TimeSpan.FromSeconds(0);

                    return false;
                }
            }
        };

        var pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(strategy)
            .AddTimeout(TimeSpan.FromSeconds(60))
            .Build();

        services.AddSingleton(pipeline);

        return services;
    }
}
