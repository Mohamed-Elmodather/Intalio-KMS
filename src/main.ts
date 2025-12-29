import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import App from './App.vue'

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

  // Install plugins
  app.use(createPinia())
  app.use(router)

  // Mount app
  app.mount('#app')
}

init().catch(console.error)
