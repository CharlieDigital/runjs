# RunJS - A .NET MCP Server to Run JS Using Jint

This project contains an MCP server that can execute JavaScript in an isolated sandbox and return a result from the script.  It is equipped with a `fetch` analogue implemented using `System.Net.HttpClient` that allows your generated JavaScript to make web requests 😎.

This is extremely powerful as in many cases, you may want to run JavaScript, but doing it *safely* is challenging because of the nature of JavaScript and generated code.

The RunJS MCP server uses [Jint](https://github.com/sebastienros/jint) -- a C# library that embeds a JavaScript runtime into .NET and allows controlling the execution sandbox by specifying:

- Memory limits
- Number of statements
- Runtime
- Depth of calls (recursion)

This makes it easy to generate and run JavaScript dynamically within your prompt as a tool without risk.

This can unlock a lot of use cases where JavaScript is needed to process some JSON, for example, and return text or run some transformation logic on incoming data.

Here's an example call using the Vercel AI SDK:

```typescript
const mcpClient = await createMCPClient({
  transport: {
    type: "sse",
    url: "http://localhost:5000/sse",
  },
});

const tools = await mcpClient.tools();

const prompt = `
  Generate and execute JavaScript that can parse the following JSON
  The JavaScript should 'return' the value
  Return only the value of the name property:
  { "id": 12345, "name": "Charles Chen", "handle": "chrlschn" }`

try {
  const { text } = await generateText({
    model: openai("gpt-4.1-nano"),
    prompt,
    tools,
    maxSteps: 10, // 👈 Very, very important or you get no output!
  });

  console.log("Output:", text);
} finally {
  await mcpClient.close();
}
```

The LLM will generate the following JavaScript:

```javascript
const jsonString = '{ "id": 12345, "name": "Charles Chen", "handle": "chrlschn" }';
const obj = JSON.parse(jsonString);
return obj.name;
```

And use the RunJS MCP server to execute it 🚀

## Project Setup

The project is set up in the following structure:

```text
📁 app
  📁 src                      # A sample client application using Vercel AI SDK
  .env                        # 👈 Make your own from the .env.sample
  .env.sample                 # Sample .envfile; make a copy as .env
📁 server
  Program.cs                  # A .NET MCP server exposing the Jint tool
builder-server.sh             # Simple script (command) to build the container
docker-compose.yaml           # Start the Aspire Dashboard container for OTEL
Dockerfile                    # Dockerfile for the .NET server app
```

## Configuring Your Local Environment

You'll need to [install the .NET SDK if you don't have it](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks); it is available for Windows, Linux, and macOS.

Once installed, you can run the following command to start the server:

```shell
dotnet run --project server
```

This starts the MCP server on port 5000.  For local use or use in a private network, you do not need to do anything special.  To expose your local MCP to an external client (e.g. local MCP and deployed application), you will need to map a proxy.

To use this with the LLM API call from a remote origin, you will need to expose it to OpenAI using a proxy like ngrok or the [VS Code Ports tool](https://code.visualstudio.com/docs/debugtest/port-forwarding).

> 👉 Be sure to set the port as public

Once you've done this, you will need to create a copy of the `.env.sample` file and as `.env` and set your OpenAI API key and the URL:

```text
OPENAI_API_KEY=sk-proj-kSZWV-M7.......K_MMv8JZRmIA
MCP_ENDPOINT=https://mhjt5hqd-5000.use.devtunnels.ms/sse
```

If you are only using a local call (like the `/app` directory):

```text
OPENAI_API_KEY=sk-proj-kSZWV-M7.......K_MMv8JZRmIA
MCP_ENDPOINT=http://localhost:5000/sse
```

From the `/app` directory, run the following:

```shell
cd app
npm i
npm run app -- "Use your echo tool with my name: <YOUR_NAME_HERE>; write out your response"
```

This should invoke the .NET MCP endpoint and output your name!

## Security

🚨 **THERE IS CURRENTLY NO AUTH** 🚨

[See the workstream here](https://github.com/modelcontextprotocol/csharp-sdk/pull/377)

**This is only suitable for running in a private network at the moment.**

Why might you use this? If your runtime application is Python, JavaScript, or some other language and you need a fast, easy, secure, controlled context to run generated code.

## Running the Server

```shell
# To start the server
dotnet run --project server

# To start the server with hot reload
dotnet watch run --project server --non-interactive
```

## Running the Client

```shell
cd app
npm run app -- "My prompt goes here"
```

To test this, you can run two types of prompts:

```shell
cd app

# Just test the echo
npm run app -- "Echo my name back to me: Charles"

# Generate and execute JavaScript
npm run app -- "Generate some JavaScript that will lowercase and return the string 'Hello, World' and execute it.  Give me the results; ONLY THE RESULTS"

# Something more complex"
npm run app -- 'Generate and execute JavaScript that can parse the following JSON and return the value of the name property: { "id": 12345, "name": "Charles Chen", "handle": "chrlschn" }'
```

## Testing Fetch

To test the `fetch` analogue using endpoints from [https://jsonplaceholder.typicode.com/](https://jsonplaceholder.typicode.com/) try the following prompts:

```shell
cd app

# Test a GET
npm run app -- "Generate some JavaScript that will GET a post from https://jsonplaceholder.typicode.com/posts/1 and retrieve the name property"

# Test a POST
npm run app -- 'Generate some JavaScript that will POST to https://jsonplaceholder.typicode.com/posts/ and create a new post: { "title": "Hello", "body": "World!", "userId": 1 }.  Return the id of the post from the result'
```

## Observability

If you run the following:

```shell
docker compose up
```

You will also get the [Aspire Dashboard](https://aspiredashboard.com/) at [http://localhost:18888](http://localhost:18888) to trace the internal calls to the tools.
