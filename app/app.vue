<template>
  <div class="row full-height flex-center q-pa-sm">
    <QCard class="col-lg-7 col-md-9 col-sm-10 col-xs-12">
      <QCardSection horizontal>
        <QCardSection class="col-md-8 col-sm-12 col-xs-12">
          <QItem dense>
            <QItemSection>
              <QItemLabel class="text-h6 text-deep-purple text-bold"
                >RunJS MCP Server Workbench App</QItemLabel
              >
            </QItemSection>
          </QItem>

          <QSeparator spaced="md" />

          <SecretManager
            v-model="secretId"
            v-model:secret-value="secretValue"
            :executing
            @save-secret="saveSecret"
          />

          <QSeparator spaced="md" />

          <QCardSection>
            <QInput
              v-model="prompt"
              label="Prompt"
              type="textarea"
              color="deep-purple"
              hint="Describe an API call or JavaScript code to generate and execute; pass in any data you want to use, extract, or transform. Include secret IDs if the API needs a secret key."
              input-style="max-height: 180px;"
              borderless
              autogrow
            />
          </QCardSection>
          <QCardActions align="right" class="q-pt-none">
            <HttpServiceSelector v-model="service" />
            <QSpace />
            <QSelect
              v-model="platform"
              class="q-mr-sm q-mt-md"
              color="deep-purple"
              :options="platforms"
              :hide-dropdown-icon="true"
              dense
              outlined
              rounded
            >
              <template #prepend>
                <span class="text-caption gt-sm">Platform</span>
              </template>
            </QSelect>
            <QBtn
              label="Run"
              color="deep-purple"
              class="q-mt-md"
              :icon-right="tabOutlinePlayerPlay"
              :loading="executing"
              @click="executePrompt"
              borderless
              flat
              no-caps
              rounded
            />
          </QCardActions>
          <QSeparator spaced="md" />
          <QCardSection>
            <QInput
              v-model="result"
              input-style="font-family: monospace; font-size: 0.8rem; min-height: 200px"
              type="textarea"
              color="deep-purple"
              hint="The result from the API call"
              readonly
            />
          </QCardSection>
        </QCardSection>

        <QSeparator vertical class="gt-sm" />

        <RightPanel />
      </QCardSection>
    </QCard>
  </div>
</template>

<script setup lang="ts">
import { tabOutlinePlayerPlay } from "quasar-extras-svg-icons/tabler-icons-v3";

const platforms = ref(["OpenAI", "Anthropic"]);

const {
  saveSecret,
  executePrompt,
  secretId,
  secretValue,
  prompt,
  result,
  executing,
  platform,
  service,
} = useBackend();
</script>

<style>
#container {
  height: 100%;
}

code {
  font-family: "Reddit Mono", monospace;
  background-color: #f5f5f5;
  color: #454545;
  padding: 0.05rem 0.4rem;
  border-radius: 4px;
  white-space: nowrap;
  border: 1px solid #ddd;
}

textarea {
  font-family: "Reddit Mono", monospace;
  font-size: 0.9rem;
}

ol {
  margin: none;
}

li {
  margin-bottom: none;
}
</style>
