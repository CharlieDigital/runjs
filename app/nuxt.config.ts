// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: "2025-05-15",
  devtools: { enabled: true },
  modules: ["@nuxt/eslint", "nuxt-quasar-ui", "@vueuse/nuxt"],
  css: ["~/assets/css/global.css"],
  quasar: {
    sassVariables: "~/assets/css/quasar-variables.sass",
    plugins: ["Notify"],
  },
});