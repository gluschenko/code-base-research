import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import legacy from '@vitejs/plugin-legacy'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        react({
            include: '**/*.{jsx,tsx}',
        }),
        legacy({
            targets: ["IE >= 11"],
            additionalLegacyPolyfills: ["whatwg-fetch"],
        })
    ],
    build: {
        outDir: "build",
        emptyOutDir: true,
    },
    server: {
        port: 5081,
    },
    preview: {
        port: 5081,
    }
})
