using System.Text;
using Serilog;

namespace RunJS;

/// <summary>
/// .NET `HttpClient` implementation that provides a JS `fetch`-like interface.
/// This will be handed to the Jint Engine to allow it to execute HTTP requests.
/// </summary>
public class FetchHttpClient
{
    private static readonly HttpClient _httpClient = new();
    private static readonly ILogger Log = Serilog.Log.ForContext<FetchHttpClient>();

    /// <summary>
    /// Performs an HTTP request. This method mimics the JavaScript `fetch` function.
    /// </summary>
    /// <param name="url">The URL for the request.</param>
    /// <param name="options">Request options object containing method, body, headers, etc. Can be null for GET requests.</param>
    /// <returns>A `Task` that resolves to a `FetchResponse` object.</returns>
    public async Task<FetchResponse> fetch(string url, dynamic? options = null)
    {
        // Extract method from options, default to GET
        var method = options?.method?.ToString() ?? "GET";
        var request = new HttpRequestMessage(new HttpMethod(method), url);

        Log.Here().Information("Making {Method} request to {Url}", method, url);

        // Handle request body if provided
        if (options?.body != null)
        {
            var body = options.body.ToString();
            var contentType = "application/json"; // Default content type

            // Check if headers contain Content-Type
            if (options?.headers != null)
            {
                try
                {
                    // Try to extract Content-Type from headers
                    var headers = options.headers;

                    if (headers["Content-Type"] != null)
                    {
                        contentType = headers["Content-Type"].ToString();
                    }
                    else if (headers["content-type"] != null)
                    {
                        contentType = headers["content-type"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    Log.Here()
                        .Warning(ex, "Failed to parse Content-Type from headers, using default");
                }
            }

            request.Content = new StringContent(body, Encoding.UTF8, contentType);
        }

        // Handle custom headers if provided
        if (options?.headers != null)
        {
            try
            {
                var headers = options.headers;
                // Iterate through headers dynamically
                foreach (var header in headers)
                {
                    var headerName = header.Name ?? header.Key;
                    var headerValue = header.Value?.ToString();

                    if (headerName != null && headerValue != null)
                    {
                        // Skip Content-Type as it's handled above with the content
                        if (
                            string.Equals(
                                headerName,
                                "Content-Type",
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        {
                            continue;
                        }

                        request.Headers.TryAddWithoutValidation(headerName, headerValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Here().Warning(ex, "Failed to parse custom headers");
            }
        }

        // The `HttpResponseMessage` is not disposed here because it is passed to the `FetchResponse`
        // which needs to be able to read the response content stream.
        var response = await _httpClient.SendAsync(request);

        // We don't call `EnsureSuccessStatusCode()` because the JS fetch API doesn't throw on HTTP error statuses.
        // Instead, it returns a response with `ok` set to `false`. The `FetchResponse` class handles this.
        Log.Here().Information("Request completed with status {StatusCode}", response.StatusCode);

        return new FetchResponse(response);
    }
}
