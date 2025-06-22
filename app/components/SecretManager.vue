<template>
  <QCardSection>
    <QInput
      v-model="secretValue"
      label="Secret Value"
      color="deep-purple"
      hint="A secret value (like an API key) that is encrypted and stored securely.  When you click 'Store Secret', the value will be saved and a unique ID will be generated.  Use this ID in your prompt to refer to the secret."
      :type="showSecret ? 'text' : 'password'"
      outlined
    >
      <template #append>
        <QBtn
          color="deep-purple"
          :icon="showSecret ? tabOutlineEye : tabOutlineEyeOff"
          @click="showSecret = !showSecret"
          flat
          rounded
        />
        <QBtn
          label="Store Secret"
          color="deep-purple"
          :icon-right="tabOutlineKey"
          :loading="executing"
          :disable="secretValue.trim().length < 3"
          @click="emit('saveSecret')"
          no-caps
          rounded
          flat
        />
      </template>
    </QInput>
  </QCardSection>
  <QCardSection>
    <QChip
      style="font-family: 'Reddit Mono', monospace"
      :icon="copied ? tabOutlineClipboardCheck : tabOutlineClipboardCopy"
      :color="copied ? 'light-green-6' : 'deep-purple'"
      @click="copy(secretId)"
      outline
      clickable
      >{{ copied ? "Copied to clipboard" : secretId }}</QChip
    >
  </QCardSection>
</template>

<script setup lang="ts">
import {
  tabOutlineClipboardCheck,
  tabOutlineClipboardCopy,
  tabOutlineEye,
  tabOutlineEyeOff,
  tabOutlineKey,
} from "quasar-extras-svg-icons/tabler-icons-v3";

const secretId = defineModel<string>({ required: true });

const secretValue = defineModel<string>("secretValue", {
  required: true,
});

defineProps<{
  executing: boolean;
}>();

const emit = defineEmits<{
  saveSecret: [];
}>();

const showSecret = ref(true);

const { copied, copy } = useClipboard();
</script>

<style scoped></style>
