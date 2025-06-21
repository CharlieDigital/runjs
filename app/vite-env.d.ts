/// <reference types="vite/client" />

interface ViteTypeOptions {
  strictImportMetaEnv: unknown;
}

interface ImportMetaEnv {
  readonly VITE_OPENAI_API_KEY: string;
  readonly VITE_MCP_ENDPOINT: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
