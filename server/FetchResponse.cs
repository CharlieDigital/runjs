using System.Text;
using System.Text.Json;
using Jint;
using Jint.Native;
using Jint.Native.Json;
using Jint.Native.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RunJS;

/// <summary>
/// Represents the response to a request, mimicking the JavaScript Response object interface.
/// This is intended to be returned by a fetch-like function to a JavaScript engine.
/// </summary>
public class FetchResponse(HttpResponseMessage responseMessage, Engine engine)
{
    private byte[]? _bodyBytes; // Cache for the response body.

    /// <summary>
    /// A boolean indicating whether the response was successful (status in the range 200-299).
    /// </summary>
    public bool ok { get; } = responseMessage.IsSuccessStatusCode;

    /// <summary>
    /// The status code of the response.
    /// </summary>
    public int status { get; } = (int)responseMessage.StatusCode;

    /// <summary>
    /// The status message corresponding to the status code.
    /// </summary>
    public string statusText { get; } =
        responseMessage.ReasonPhrase ?? string.Empty;

    /// <summary>
    /// A ReadableStream of the body contents.
    /// </summary>
    public Stream? body { get; } = responseMessage.Content?.ReadAsStream();

    /// <summary>
    /// Takes the Response stream and reads it to completion. It returns a promise
    /// that resolves with a string.
    /// </summary>
    public async Task<string> text()
    {
        await ReadBodyBytesOnceAsync();
        return Encoding.UTF8.GetString(_bodyBytes ?? []);
    }

    /// <summary>
    /// Takes the Response stream and reads it to completion. It returns a promise
    /// that resolves with the result of parsing the body text as JSON.
    /// </summary>
    public async Task<JObject?> json()
    {
        var bodyAsText = await text();

        if (string.IsNullOrWhiteSpace(bodyAsText))
        {
            return null;
        }

        var result = JObject.Parse(bodyAsText);

        return result;
    }

    /// <summary>
    /// Takes the Response stream and reads it to completion. It returns a promise
    /// that resolves with a Blob (represented as a byte array in C#).
    /// </summary>
    public async Task<byte[]> blob()
    {
        await ReadBodyBytesOnceAsync();
        return _bodyBytes ?? [];
    }

    /// <summary>
    /// Reads the content from the HttpResponseMessage into a byte array, if not
    /// already read.  This ensures the stream is only read once.
    /// </summary>
    private async Task ReadBodyBytesOnceAsync()
    {
        if (_bodyBytes == null && responseMessage.Content != null)
        {
            _bodyBytes = await responseMessage.Content.ReadAsByteArrayAsync();
        }
    }
}
