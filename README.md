# RunJS - A .NET MCP Server to Run JS Using Jint

This project contains an MCP server that can execute JavaScript in an isolated sandbox and return a result from the script.

This is extremely powerful as in many cases, you may want to run JavaScript, but it may be difficult to deploy it into live infrastructure (e.g. deploy a serverless function for each piece of code).

[Jint](https://github.com/sebastienros/jint) is a C# library that embeds a JavaScript runtime into .NET and allows controlling the execution sandbox by specifying:

- Memory limits
- Number of statements
- Runtime
- Depth of calls (recursion)

This makes it easy to generate and run JavaScript dynamically within your prompt as a tool

## Project Setup

The project is set up in the following structure:

```text
📁 app
  📁 src                      # The client application using Vercel AI SDK
  .env.sample                 # Sample .envfile; make a copy as .env
📁 server
  Program.cs                  # A .NET MCP server exposing the Jint tool
docker-compose.yaml           # Start the Aspire Dashboard container for OTEL
```

## Configuring Your Local Environment

You'll need to [Install the .NET SDK if you don't have it](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks).  These are available for Windows, Linux, and macOS.

Once installed, you can run the following command to start the server:

```shell
dotnet run --project server
```

This starts the MCP server on port 5000.  To use this with the LLM API call, you will need to expose it to OpenAI using a proxy like ngrok or the [VS Code Ports tool](https://code.visualstudio.com/docs/debugtest/port-forwarding).

> 👉 Be sure to set the port as public

Once you've done this, you will need to create a copy of the `.env.sample` file and as `.env` and set your OpenAI API key and the URL:

```text
OPENAI_API_KEY=sk-proj-kSZWV-M7.......K_MMv8JZRmIA
MCP_ENDPOINT=https://mhjt5hqd-5000.use.devtunnels.ms/sse
```

From the `/app` directory, run the following:

```shell
cd app
npm i
npm run app -- "Use your echo tool wih my name: <YOUR_NAME_HERE>; write out your response"
```

This should invoke the .NET MCP endpoint and output your name!

## Security

This code uses very simple API key based security.  If you need it to be more secure, fork it!

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
```

If you run the following:

```shell
docker compose up
```

You will also get the [Aspire Dashboard](https://aspiredashboard.com/) at [http://localhost:18888](http://localhost:18888) to trace the internal calls to the tools.
