import {
  generateText,
  experimental_createMCPClient as createMCPClient,
} from "ai";
import { createOpenAI } from "@ai-sdk/openai";

// Make an API call to open AI
export default defineEventHandler(async (event) => {
  const { prompt } = await readBody(event);

  console.log("Received prompt:", prompt);

  const openai = createOpenAI({
    apiKey: import.meta.env.VITE_OPENAI_API_KEY,
  });

  const mcpClient = await createMCPClient({
    transport: {
      type: "sse",
      url: process.env.VITE_MCP_ENDPOINT || "INVALID_URL",
    },
  });

  const tools = await mcpClient.tools();

  const result = await generateText({
    model: openai("gpt-4.1-mini"),
    prompt: prompt?.toString(),
    maxSteps: 10,
    tools,
    system: `
# Role
You are a backend JavaScript developer with deep knowledge of Javascript, REST APIs, and web services.
Your job is to help write the JavaScript statements necessary to help the user complete their task.
You may need to make an API call or write some JavaScript code to process data.

## Instructions
- The user will ask you to write some JavaScript code to process data or make an API call.
- You can write JavaScript and use a tool to run the JavaScript code.
- If the user asks you to make an API call, you can use the \`fetch\` function to make the call.
- The user may include instructions on how to make the API call like sample documentation or a snippet of OpenAPI specs.
- Use that information to construct the JavaScript code to make a \`fetch\` call.
- The user may include an API key that starts with \`runjs:secret:\`; this is the secretId for the tool.
- If the user provides a secretId, you need to supply it to the tool when making API calls.
- You are allowed to write async/await code as necessary to handle Promises.
- Just write the statements to execute.
- Your last statement should always \`return\` a value based on the user's request.
`,
  });

  return result.text;
});
