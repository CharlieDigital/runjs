using Jint;
using Newtonsoft.Json.Linq;

namespace tests;

public class TestJintBehavior
{
    [Fact]
    public void Can_Handle_Async_Json_Parse()
    {
        var engine = new Engine();

        engine.SetValue(
            "test",
            new Func<Task<string>>(async () =>
            {
                return await Task.FromResult("""{ "text": "Hello, World!" }""");
            })
        );

        var script = """
            (async () => {
                const json = JSON.parse(await test());
                return json.text;
            })()
            """;

        var result = engine.Evaluate(script).UnwrapIfPromise();

        Assert.NotNull(result);
        Assert.Equal("Hello, World!", result.ToString());
    }

    [Fact]
    public void Can_Handle_Async_Json_Parse_As_JS()
    {
        var engine = new Engine();

        engine.SetValue(
            "test",
            new Func<Task<JObject>>(async () =>
            {
                return await Task.FromResult(
                    JObject.Parse("""{ "text": "Hello, World!" }""")
                );
            })
        );

        var script = """
            (async () => {
                const json = await test();
                return json.text;
            })()
            """;

        var result = engine.Evaluate(script).UnwrapIfPromise();

        Assert.NotNull(result);
        Assert.Equal("Hello, World!", result.ToString());
    }

    [Fact]
    public void Can_Handle_Async_Json_Parse_As_JS_From_Object()
    {
        var engine = new Engine(options =>
            options.ExperimentalFeatures = ExperimentalFeature.TaskInterop
        );

        var fetcher = new Fetcher();
        engine.SetValue("fetch", fetcher.FetchAsync);

        var script = """
            (async () => {
                const response = await fetch();
                const json = await response.json();
                return await json.text;
            })()
            """;

        var result = engine.Evaluate(script).UnwrapIfPromise();

        Assert.NotNull(result);
        Assert.Equal("Hello, World!", result);
    }

    [Fact]
    public void Can_Handle_Async_Json_Parse_As_JS_From_Object_With_JsonPath()
    {
        var path = Path.Join(Directory.GetCurrentDirectory(), "Mcp/libs");

        var engine = new Engine(options => options.EnableModules(path));

        engine.Modules.Import("./jsonpath-plus.browser-esm.min.js");

        var script = """
            (async () => {
                const records = [
                    { id: 1, text: "Hello, Earth!" },
                    { id: 2, text: "Hello, Mars!" }
                ]

                const result = JSONPath.JSONPath({path: '$..text', json: records});

                return JSON.stringify(result);
            })()
            """;

        var result = engine.Evaluate(script).UnwrapIfPromise();

        Assert.NotNull(result);
        Assert.Contains("Hello, Earth!", result.ToString());
    }

    class Fetcher
    {
        public async Task<Response> FetchAsync()
        {
            return await Task.FromResult(new Response());
        }
    }

    class Response
    {
        public async Task<JObject> json()
        {
            return (
                await Task.FromResult(
                    JObject.Parse("""{ "text": "Hello, World!" }""")
                )
            );
        }
    }
}
