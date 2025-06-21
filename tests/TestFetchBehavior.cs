using Jint;
using Newtonsoft.Json.Linq;
using RunJS;

namespace tests;

public class TestFetchBehavior
{
    [Fact]
    public async Task Can_Get_With_Client()
    {
        var engine = new Engine();

        using var client = new FetchHttpClient(engine);
        var url = "https://jsonplaceholder.typicode.com/posts/1";

        var response = await client.Fetch(url);

        Assert.NotNull(response);
        Assert.True(response.ok);
        Assert.Equal(200, response.status);
        Assert.Equal("OK", response.statusText);

        var bodyText = await response.text();
        Assert.NotEmpty(bodyText);

        var json = await response.json();
        Assert.NotNull(json);
    }

    [Fact]
    public async Task Can_Post_With_Client()
    {
        var engine = new Engine();

        using var client = new FetchHttpClient(engine);
        var url = "https://jsonplaceholder.typicode.com/posts";

        var payload = JObject.FromObject(
            new
            {
                method = "POST",
                body = JObject
                    .FromObject(
                        new
                        {
                            title = "foo",
                            body = "bar",
                            userId = 1
                        }
                    )
                    .ToString(),
                headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json"
                }
            }
        );

        var response = await client.Fetch(url, payload);

        Assert.NotNull(response);
        Assert.True(response.ok);
        Assert.Equal(201, response.status);
        Assert.Equal("Created", response.statusText);

        var bodyText = await response.text();
        Assert.NotEmpty(bodyText);

        var json = await response.json();
        Assert.NotNull(json);
    }

    [Fact]
    public async Task Can_Fetch_With_Jint()
    {
        var engine = new Engine();

        // Register the FetchHttpClient in Jint
        using var client = new FetchHttpClient(engine);
        engine.SetValue("fetch", client.Fetch);

        var script = """
            (async () => {
                return await fetch('https://jsonplaceholder.typicode.com/posts/1')
            })()
            """;

        var result =
            engine.Evaluate(script).UnwrapIfPromise().ToObject() as FetchResponse;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ok);
        var text = await result.text();
        Assert.Contains("facere", text);
    }

    [Fact]
    public void Can_Fetch_In_Jint_Resolve_Async()
    {
        var engine = new Engine(options =>
        {
            options.ExperimentalFeatures = ExperimentalFeature.TaskInterop;
        });

        // Register the FetchHttpClient in Jint
        using var client = new FetchHttpClient(engine);
        engine.SetValue("fetch", client.Fetch);

        var messages = new List<string>();
        engine.SetValue(
            "log",
            new Action<object>(msg =>
            {
                messages.Add(msg?.ToString() ?? string.Empty);
            })
        );

        var script = """
            (async () => {
                const response = await fetch('https://jsonplaceholder.typicode.com/posts/1');
                log(await response.text());
                const data = await response.json();
                log(data);
                return data.title;
            })()
            """;

        var result = engine.Evaluate(script).UnwrapIfPromise();

        Assert.NotNull(result);
        Assert.Equal(
            "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
            result
        );
    }

    [Fact]
    public void Can_Post_In_Jint_Resolve_Async()
    {
        var engine = new Engine(options =>
        {
            options.ExperimentalFeatures = ExperimentalFeature.TaskInterop;
        });

        // Register the FetchHttpClient in Jint
        using var client = new FetchHttpClient(engine);
        engine.SetValue("fetch", client.Fetch);

        var script = """
            (async () => {
                const response = await fetch('https://jsonplaceholder.typicode.com/posts', {
                    method: 'POST',
                    body: JSON.stringify({
                        title: 'foo',
                        body: 'bar',
                        userId: 1
                    }),
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
                const data = await response.json();
                return data.title;
            })()
            """;

        var result = engine.Evaluate(script).UnwrapIfPromise();

        Assert.NotNull(result);
        Assert.Equal("foo", result);
    }
}
