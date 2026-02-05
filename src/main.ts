import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import i18n from './i18n'
import App from './App.vue'
import { useLocaleStore } from './stores/locale'
import { installErrorHandler } from './utils/errorHandler'

// Styles
import '@fortawesome/fontawesome-free/css/all.min.css'
import './styles/main.css'
import './styles/original-styles.css'

async function enableMocking() {
  if (import.meta.env.VITE_ENABLE_MOCKS !== 'true') {
    return
  }

  try {
    const { worker } = await import('./mocks/browser')
    await worker.start({
      onUnhandledRequest: 'bypass',
    })
    console.log('[MSW] Mock Service Worker started')
  } catch (error) {
    console.warn('[MSW] Failed to start Mock Service Worker:', error)
  }
}

// Initialize app
async function init() {
  await enableMocking()

  const app = createApp(App)
  const pinia = createPinia()

  // Install plugins
  app.use(pinia)
  app.use(router)
  app.use(i18n)

  // Install global error handler
  installErrorHandler(app)

  // Initialize locale store
  const localeStore = useLocaleStore()
  localeStore.initLocale()

  // Mount app
  app.mount('#app')
}

init().catch(console.error)
