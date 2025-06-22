/// <reference types="vite/client" />

interface ViteTypeOptions {
  strictImportMetaEnv: unknown;
}

interface ImportMetaEnv {
  readonly VITE_OPENAI_API_KEY: string;
  readonly VITE_ANTHROPIC_API_KEY: string;
  readonly VITE_MCP_ENDPOINT: string;
  readonly VITE_SECRETS_ENDPOINT: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
