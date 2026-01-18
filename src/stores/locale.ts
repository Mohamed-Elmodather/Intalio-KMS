import { defineStore } from 'pinia'
import { ref, computed, watch } from 'vue'

export type Locale = 'en' | 'ar'

export const useLocaleStore = defineStore('locale', () => {
  // State
  const currentLocale = ref<Locale>(
    (localStorage.getItem('locale') as Locale) || 'en'
  )

  // Computed
  const isRTL = computed(() => currentLocale.value === 'ar')
  const direction = computed(() => isRTL.value ? 'rtl' : 'ltr')

  // Actions
  function setLocale(locale: Locale) {
    currentLocale.value = locale
    localStorage.setItem('locale', locale)
    applyDirection()
  }

  function toggleLocale() {
    setLocale(currentLocale.value === 'en' ? 'ar' : 'en')
  }

  function applyDirection() {
    const html = document.documentElement
    html.setAttribute('dir', direction.value)
    html.setAttribute('lang', currentLocale.value)
  }

  // Initialize direction on store creation
  function initLocale() {
    applyDirection()
  }

  // Watch for locale changes
  watch(currentLocale, () => {
    applyDirection()
  })

  return {
    // State
    currentLocale,

    // Computed
    isRTL,
    direction,

    // Actions
    setLocale,
    toggleLocale,
    initLocale,
  }
})
