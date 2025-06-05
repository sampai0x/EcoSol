import { defineConfig } from "vite"
import vue from "@vitejs/plugin-vue"
import path from "path"

export default defineConfig({
  plugins: [vue()],
  server: {
    port: 5173,
    proxy: {
      "/api": {
        target: "https://localhost:7272", // Mudança para HTTP
        changeOrigin: true,
        secure: false,
        configure: (proxy, _options) => {
          proxy.on("error", (err, _req, _res) => {
            console.log("proxy error", err)
          })
          proxy.on("proxyReq", (proxyReq, req, _res) => {
            console.log("Sending Request to the Target:", req.method, req.url)
          })
          proxy.on("proxyRes", (proxyRes, req, _res) => {
            console.log("Received Response from the Target:", proxyRes.statusCode, req.url)
          })
        },
      },
    },
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
})
