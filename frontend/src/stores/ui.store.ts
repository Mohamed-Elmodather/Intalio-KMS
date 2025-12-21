import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export type Locale = 'en' | 'ar'
export type Theme = 'light' | 'dark'

export const useUIStore = defineStore('ui', () => {
  // State
  const locale = ref<Locale>('en')
  const theme = ref<Theme>('light')
  const sidebarCollapsed = ref(false)
  const sidebarMobileOpen = ref(false)

  // Getters
  const direction = computed(() => locale.value === 'ar' ? 'rtl' : 'ltr')
  const isRTL = computed(() => direction.value === 'rtl')
  const isDarkMode = computed(() => theme.value === 'dark')

  // Actions
  function setLocale(newLocale: Locale) {
    locale.value = newLocale
    document.documentElement.setAttribute('dir', direction.value)
    document.documentElement.setAttribute('lang', newLocale)
  }

  function toggleLocale() {
    setLocale(locale.value === 'en' ? 'ar' : 'en')
  }

  function setTheme(newTheme: Theme) {
    theme.value = newTheme
    if (newTheme === 'dark') {
      document.documentElement.classList.add('dark-mode')
    } else {
      document.documentElement.classList.remove('dark-mode')
    }
  }

  function toggleTheme() {
    setTheme(theme.value === 'light' ? 'dark' : 'light')
  }

  function toggleSidebar() {
    sidebarCollapsed.value = !sidebarCollapsed.value
  }

  function toggleMobileSidebar() {
    sidebarMobileOpen.value = !sidebarMobileOpen.value
  }

  function closeMobileSidebar() {
    sidebarMobileOpen.value = false
  }

  return {
    // State
    locale,
    theme,
    sidebarCollapsed,
    sidebarMobileOpen,
    // Getters
    direction,
    isRTL,
    isDarkMode,
    // Actions
    setLocale,
    toggleLocale,
    setTheme,
    toggleTheme,
    toggleSidebar,
    toggleMobileSidebar,
    closeMobileSidebar
  }
}, {
  persist: {
    key: 'intalio-ui',
    pick: ['locale', 'theme', 'sidebarCollapsed']
  }
})
