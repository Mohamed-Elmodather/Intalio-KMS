import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
export default defineConfig({
    plugins: [
        vue(),
    ],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        port: 3000,
        proxy: {
            '/api': {
                target: 'http://localhost:5001',
                changeOrigin: true
            }
        }
    },
    css: {
        preprocessorOptions: {
            scss: {
                // Legacy variables injected globally for backward compatibility
                // New components using design system should NOT import tokens with 'as *'
                // to avoid conflicts - use namespace instead: @use '...' as tokens;
                additionalData: "@use \"@/assets/styles/_variables.scss\" as *;"
            }
        }
    }
});
