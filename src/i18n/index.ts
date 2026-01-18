import { createI18n } from 'vue-i18n'
import en from './locales/en'
import ar from './locales/ar'

export type MessageSchema = typeof en

const i18n = createI18n<[MessageSchema], 'en' | 'ar'>({
  legacy: false,
  locale: localStorage.getItem('locale') || 'en',
  fallbackLocale: 'en',
  messages: {
    en,
    ar,
  },
})

export default i18n
