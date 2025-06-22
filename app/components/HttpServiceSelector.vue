<template>
  <div>
    <QSelect
      v-model="service"
      :options="services"
      :hide-dropdown-icon="true"
      class="q-mr-sm q-mt-md"
      color="deep-purple"
      outlined
      rounded
      dense
    >
      <template #prepend>
        <span class="text-caption gt-sm">Example</span>
      </template>
      <template #append>
        <QBtn
          color="deep-purple"
          target="_blank"
          :icon-right="tabOutlineExternalLink"
          :href="serviceUrl"
          @click.capture.stop
          flat
          rounded
          no-caps
          dense
        />
      </template>
    </QSelect>
  </div>
</template>

<script setup lang="ts">
import { tabOutlineExternalLink } from "quasar-extras-svg-icons/tabler-icons-v3";

const service = defineModel<string>({ required: true });

const services = ref(["jsonplaceholder", "httpbin"]);

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
</script>

<style scoped></style>
