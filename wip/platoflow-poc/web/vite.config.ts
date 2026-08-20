import { defineConfig } from "vite";
import { resolve } from "path";

// Gratify is imported from the submodule source (same pattern as labs/platoflow → labs/kea).
const gratify = resolve(__dirname, "../../../../submodules/gratify/src/gratify");

export default defineConfig({
  resolve: { alias: { gratify } },
  server: {
    port: 5215,
    strictPort: true,
    fs: { allow: [resolve(__dirname, "../../../..")] },
    proxy: {
      "/api": "http://127.0.0.1:5214",
      "/mcp": "http://127.0.0.1:5214",
    },
  },
  build: {
    rollupOptions: {
      input: {
        main: resolve(__dirname, "index.html"),
        editor: resolve(__dirname, "editor.html"),
        viewer: resolve(__dirname, "viewer.html"),
      },
    },
  },
});
