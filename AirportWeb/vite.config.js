import { fileURLToPath } from "url"
import { defineConfig } from "vite"
import vue from "@vitejs/plugin-vue"

export default defineConfig({
    plugins: [
        vue()
    ],
    server: {
        port: 8080,
        open: true,
        host: true
    },
    resolve: {
        alias: {
            "@": fileURLToPath(new URL('./src', import.meta.url)),
            "@utils": fileURLToPath(new URL('./src/utils', import.meta.url)),
            "@components": fileURLToPath(new URL('./src/components', import.meta.url)),
            "@pages": fileURLToPath(new URL('./src/pages', import.meta.url))
        }
    }
})