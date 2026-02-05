import { defineConfig, type UserConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { resolve } from 'path'

// https://vitejs.dev/config/
export default defineConfig(({ mode }): UserConfig => {
  const isProduction = mode === 'production'

  return {
    plugins: [vue()],

    resolve: {
      alias: {
        '@': resolve(__dirname, 'src'),
      },
    },

    // Development server configuration
    server: {
      port: 3000,
      open: true,
      proxy: {
        '/api': {
          target: 'http://localhost:5010',
          changeOrigin: true,
        },
        '/hubs': {
          target: 'http://localhost:5010',
          changeOrigin: true,
          ws: true,
        },
      },
    },

    // Preview server (for testing production builds locally)
    preview: {
      port: 4173,
    },

    // Build configuration
    build: {
      outDir: 'dist',
      // Generate sourcemaps for production (useful for error tracking)
      sourcemap: isProduction ? 'hidden' : true,
      // Minification
      minify: isProduction ? 'esbuild' : false,
      // Target modern browsers
      target: 'es2020',
      // Chunk size warning limit (500kb)
      chunkSizeWarningLimit: 500,
      // CSS code splitting
      cssCodeSplit: true,
      // Rollup options
      rollupOptions: {
        output: {
          // Manual chunk splitting for better caching
          manualChunks: {
            // Vue core
            'vue-vendor': ['vue', 'vue-router', 'pinia'],
            // i18n
            'i18n': ['vue-i18n'],
            // UI libraries
            'ui-vendor': ['@fortawesome/fontawesome-free'],
            // Utilities
            'utils': ['axios', 'dompurify'],
          },
          // Asset file naming
          assetFileNames: (assetInfo) => {
            const info = assetInfo.name?.split('.') ?? []
            const ext = info[info.length - 1]
            if (/png|jpe?g|svg|gif|tiff|bmp|ico/i.test(ext)) {
              return `assets/images/[name]-[hash][extname]`
            }
            if (/woff2?|eot|ttf|otf/i.test(ext)) {
              return `assets/fonts/[name]-[hash][extname]`
            }
            return `assets/[name]-[hash][extname]`
          },
          // Chunk file naming
          chunkFileNames: isProduction
            ? 'assets/js/[name]-[hash].js'
            : 'assets/js/[name].js',
          // Entry file naming
          entryFileNames: isProduction
            ? 'assets/js/[name]-[hash].js'
            : 'assets/js/[name].js',
        },
      },
      // Report compressed size
      reportCompressedSize: true,
    },

    // Optimization
    optimizeDeps: {
      include: ['vue', 'vue-router', 'pinia', 'vue-i18n', 'axios'],
    },

    // CSS configuration
    css: {
      devSourcemap: true,
    },

    // Define global constants
    define: {
      __VUE_OPTIONS_API__: false,
      __VUE_PROD_DEVTOOLS__: false,
      __VUE_PROD_HYDRATION_MISMATCH_DETAILS__: false,
    },

    // Environment variables prefix
    envPrefix: 'VITE_',

    // Logging
    logLevel: isProduction ? 'warn' : 'info',
  }
})
