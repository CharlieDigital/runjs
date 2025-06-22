<template>
  <QCardSection class="col-4 text-body2 text-grey-8 gt-sm">
    <p>
      This is a demo app for RunJS, an MCP server that lets LLMs generate and execute
      JavaScript securely in a .NET runtime using the
      <strong>Jint</strong> C# JavaScript interpreter.
    </p>
    <p>To get started:</p>
    <ol>
      <li>
        Run the database containers by executing <code>docker compose up</code> in the
        root directory.
      </li>
      <li>
        Run the MCP server by navigating to the <code>/server</code> directory and
        executing <code>dotnet run</code>.
      </li>
      <li>Edit the <code>.env</code> OpenAI API key for the backend</li>
      <li>
        To test with the storage of secret values like API keys, enter the value in the
        <strong>"Secret Value"</strong> field and click <strong>"Store Secret"</strong>.
      </li>
      <li>
        Select an API service from the dropdown menu to test against and check the
        documentation to see valid requests.
      </li>
    </ol>
    <p>
      Then describe the request that you want to make and the data you want to extract
      from the response in the <strong>"Prompt"</strong> field
    </p>
    <p>
      The app will use the OpenAI API to generate a JavaScript function that makes the API
      call and extracts the data you want. That JavaScript is passed to the RunJS MCP
      server to execute.
    </p>
    <p>
      The purpose the secret is to demonstrate that we can hide sensitive keys from the
      LLM and retrieve it when we make the API call in code. If you use the
      <code>httpbin</code> service, you should see it in the output.
    </p>
    <QSeparator spaced="md" />
    <QChip
      color="deep-purple"
      :icon="tabOutlineEyeCode"
      @click="openAspireDashboard"
      clickable
      outline
      >View OpenTelemetry Logs</QChip
    >
  </QCardSection>
</template>

<script setup lang="ts">
import { tabOutlineEyeCode } from "quasar-extras-svg-icons/tabler-icons-v3";

function openAspireDashboard() {
  window.open("http://localhost:18888", "_blank");
}
</script>

<style scoped></style>
