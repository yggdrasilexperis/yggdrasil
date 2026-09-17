/// <reference types="vite/client" />

// Typed so the API base URL is not `any` at the call site.
interface ImportMetaEnv {
  readonly VITE_API_BASE_URL: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
