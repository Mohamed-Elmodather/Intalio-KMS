import { createApp } from 'vue'
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import PrimeVue from 'primevue/config'
import ToastService from 'primevue/toastservice'
import ConfirmationService from 'primevue/confirmationservice'
import DialogService from 'primevue/dialogservice'

import App from './App.vue'
import router from './router'
import i18n from './plugins/i18n'
import { IntalioTheme } from './assets/styles/themes/intalio-theme'

// Styles
import 'primeicons/primeicons.css'
import 'primeflex/primeflex.css'
import './assets/styles/main.scss'

const app = createApp(App)

// Pinia Store
const pinia = createPinia()
pinia.use(piniaPluginPersistedstate)
app.use(pinia)

// Vue Router
app.use(router)

// Vue I18n
app.use(i18n)

// PrimeVue with Intalio custom theme
app.use(PrimeVue, {
  theme: {
    preset: IntalioTheme,
    options: {
      darkModeSelector: '.dark-mode',
      cssLayer: {
        name: 'primevue',
        order: 'tailwind-base, primevue, tailwind-utilities'
      }
    }
  },
  ripple: true,
  inputStyle: 'outlined'
})

app.use(ToastService)
app.use(ConfirmationService)
app.use(DialogService)

app.mount('#app')
