<template>
  <QCardSection class="col-4 text-body2 text-grey-7 gt-sm bg-grey-1">
    <p>
      This is a demo app for RunJS: an MCP server that lets LLMs generate and execute
      JavaScript safely in a .NET runtime using the
      <strong>Jint</strong> C# JavaScript interpreter and a built-in secret manager.
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
      <li>Edit the {{ platform }} API key for the app in <code>app/.env</code></li>
      <li>
        To test with the storage of secret values like API keys, enter the value in the
        <strong>"Secret Value"</strong> field and click <strong>"Store Secret"</strong>.
      </li>
      <li>
        Select a sample API service for a quick test; view the documentation for endpoints
        on the sample API.
      </li>
    </ol>
    <p>
      Then describe the request that you want to make and the data you want to extract
      from the response in the <strong>"Prompt"</strong> field
    </p>
    <p>
      The app will use the {{ platform }} API to generate a JavaScript function that makes
      the API call and extracts the data you want. The JS is executed securely in the MCP
      RunJS server with secrets injection.
    </p>
    <p>
      The secret hides sensitive keys from the LLM and retrieves it when we make the API
      call in code. If you use the
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

defineProps<{
  platform: string;
}>();

function openAspireDashboard() {
  window.open("http://localhost:18888", "_blank");
}
</script>

<style scoped>
ol {
  margin-left: -12px;
}

li {
  margin-bottom: none;
}
</style>
