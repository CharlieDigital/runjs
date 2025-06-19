import { openai } from "@ai-sdk/openai";
import {
  generateText,
  experimental_createMCPClient as createMCPClient,
} from "ai";
import { program } from "commander";
import "dotenv/config";
import { traceProvider } from "./telemetry";

// 👉 Copy the .env.sample file to .env and set your OPENAI_API_KEY

program.argument("<prompt>", "The prompt to execute").action(async (prompt) => {
  console.log("MCP Endpoint:", process.env.MCP_ENDPOINT);
  console.log("Executing prompt:", prompt);

  const mcpClient = await createMCPClient({
    transport: {
      type: "sse",
      url: process.env.MCP_ENDPOINT || "INVALID_URL",
    },
  });

  const tools = await mcpClient.tools();

  try {
    const { text } = await generateText({
      model: openai("gpt-4.1-nano"),
      prompt,
      tools,
      experimental_telemetry: {
        isEnabled: true,
        tracer: traceProvider.getTracer("ai"),
      },
      maxSteps: 10, // 👈 Very, very important or you get no output!
    });

    console.log("  ⮑  Output:", text);
  } finally {
    await mcpClient.close();
  }
});

program.parse();
