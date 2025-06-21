<template>
  <div class="row full-height flex-center q-pa-sm">
    <QCard class="col-lg-7 col-md-9 col-sm-10 col-xs-12">
      <QCardSection horizontal>
        <QCardSection class="col-md-8 col-sm-12 col-xs-12">
          <QItem dense>
            <QItemSection>
              <QItemLabel class="text-h6">RunJS Demo App</QItemLabel>
            </QItemSection>
          </QItem>

          <QCardSection>
            <QInput
              v-model="secretValue"
              label="Secret Value"
              type="password"
              color="pink"
              hint="A secret value like an API key that is required to make the API call.  This value is encrypted and stored securely."
              outlined
            >
              <template #append>
                <QBtn
                  color="pink"
                  :icon-right="tabOutlineKey"
                  :loading="executing"
                  :disable="secretValue.trim().length < 3"
                  @click="saveSecret"
                  no-caps
                  rounded
                  flat
                  >Store Secret</QBtn
                >
              </template>
            </QInput>
          </QCardSection>
          <QCardSection>
            <code>{{ secretId }}</code>
          </QCardSection>
          <QSeparator spaced="md" />
          <QCardSection>
            <QSelect
              v-model="service"
              :options="services"
              label="Service"
              color="pink"
              hint="Select the service to call.  This will determine the API endpoint and parameters."
              outlined
            />
          </QCardSection>

          <QItem>
            <QItemSection>
              <QItemLabel class="text-body2 text-bold">HTTP Test Service</QItemLabel>
              <QItemLabel class="q-py-xs"
                ><a :href="serviceUrl" target="_blank" class="text-body2">{{
                  serviceUrl
                }}</a></QItemLabel
              >
              <QItemLabel caption>Click the link to see docs on this endpoint</QItemLabel>
            </QItemSection>
          </QItem>
          <QSeparator spaced="md" />
          <QCardSection>
            <QInput
              v-model="prompt"
              label="Prompt"
              type="textarea"
              color="pink"
              hint="The prompt to send to the LLM.  Describe your API call that you want to make."
              borderless
            />
          </QCardSection>
          <QCardActions align="right" class="q-pt-none">
            <QBtn
              color="pink"
              class="q-mt-md"
              :icon-right="tabOutlinePlayerPlay"
              :loading="executing"
              @click="executePrompt"
              borderless
              flat
              no-caps
              rounded
              >Run Prompt</QBtn
            >
          </QCardActions>
          <QSeparator spaced="md" />
          <QCardSection>
            <QInput
              v-model="result"
              input-style="font-family: monospace; font-size: 0.8rem; min-height: 200px"
              type="textarea"
              color="pink"
              hint="The result from the API call"
              readonly
            />
          </QCardSection>
        </QCardSection>

        <QSeparator vertical class="gt-sm" />
        <QCardSection class="col-4 text-caption text-grey-8 gt-sm">
          <p v-html="intro" />
        </QCardSection>
      </QCardSection>
    </QCard>
  </div>
</template>

<script setup lang="ts">
import { Notify } from "quasar";
import {
  tabOutlineKey,
  tabOutlinePlayerPlay,
} from "quasar-extras-svg-icons/tabler-icons-v3";

const secretId = ref("none");
const secretValue = ref("");
const prompt = ref("");
const result = ref("");
const services = ref(["jsonplaceholder", "httpbin"]);
const service = ref("httpbin");
const executing = ref(false);

const serviceUrl = computed(() => {
  switch (service.value) {
    case "jsonplaceholder":
      return "https://jsonplaceholder.typicode.com/";
    case "httpbin":
      return "https://httpbin.org/";
    default:
      return "Select a service";
  }
});

watch(
  secretId,
  (id) => {
    if (id === "none") {
      return;
    }

    prompt.value = prompt.value.replace(/runjs:secret:SECRET_GUID_HERE/g, id);
  },
  { immediate: true }
);

watch(
  service,
  (svc) => {
    if (svc === "jsonplaceholder") {
      prompt.value = `Generate some JavaScript that will POST to https://jsonplaceholder.typicode.com/posts/
Create a new post: { "title": "Hello", "body": "runjs:secret:SECRET_GUID_HERE", "userId": 1 }.
Include the Authorization header with the secret key runjs:secret:SECRET_GUID_HERE.
Return whether the JSON contains the phrase "abracadabra" anywhere in the response.`;
    } else {
      prompt.value = `Generate some JavaScript that will POST to https://httpbin.org/post
Use the the authorization header with value: runjs:secret:SECRET_GUID_HERE
Just return the raw response; do not modify it; do not add your commentary.;
DO NOT MODIFY THE RESPONSE AND REPLACE THE SECRET.
Just give me the response from the tool directly as text.`;
    }
  },
  { immediate: true }
);

async function saveSecret() {
  if (!secretValue.value) return;

  try {
    executing.value = true;

    const response = await fetch("/api/store-secret", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ secret: secretValue.value }),
    });

    if (response.ok) {
      secretId.value = await response.text();

      console.log("Secret saved:", secretId.value);
    } else {
      Notify.create({
        type: "negative",
        message: "Failed to store secret value.",
      });
    }
  } finally {
    executing.value = false;
  }
}

async function executePrompt() {
  if (!prompt.value) return;

  try {
    executing.value = true;
    result.value = ""; // Clear previous result

    const response = await fetch("/api/run-prompt", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ prompt: prompt.value }),
    });

    if (response.ok) {
      result.value = await response.text();
    } else {
      Notify.create({
        type: "negative",
        message: "Failed to execute prompt.",
      });
    }
  } finally {
    executing.value = false;
  }
}

const intro = `<p>This is a demo app for RunJS, a tool that allows you to run JavaScript code in the browser and make API calls to various services.</p>
<p>To get started:</p>
<ol>
<li>Run the database containers by executing <code>docker compose up</code> in the root directory.</li>
<li>Run the MCP server by navigating to the <code>/server</code> directory and executing <code>dotnet run</code>.</li>
<li>Edit the <code>.env</code> OpenAI API key for the backend</li>
<li>To test with the storage of secret values like API keys, enter the value in the <strong>"Secret Value"</strong> field and click <strong>"Store Secret"</strong>.</li>
<li>Select an API service from the dropdown menu to test against and check the documentation to see valid requests.</li>
</ol>
<p>Then describe the request that you want to make and the data you want to extract from the response in the "Prompt" field</p>
<p>The app will use the OpenAI API to generate a JavaScript function that makes the API call and extracts the data you want.</p>
<p>The purpose the secret is to demonstrate that we can hide sensitive keys from the LLM and retrieve it when we make the API call in code.  If you use the <code>httpbin</code> service, you should see it in the output.</p>
`;
</script>

<style>
#container {
  height: 100%;
}

code {
  background-color: #f5f5f5;
  color: #454545;
  padding: 0.05rem 0.4rem;
  border-radius: 4px;
  white-space: nowrap;
  border: 1px solid #ddd;
}

ol {
  margin: none;
}

li {
  margin-bottom: none;
}
</style>
