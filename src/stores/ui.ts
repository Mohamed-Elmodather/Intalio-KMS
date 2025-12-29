import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface Toast {
  id: string
  type: 'success' | 'error' | 'warning' | 'info'
  title: string
  message?: string
  duration?: number
}

export interface Modal {
  id: string
  component: string
  props?: Record<string, unknown>
}

export const useUIStore = defineStore('ui', () => {
  // Sidebar state
  const sidebarCollapsed = ref(localStorage.getItem('sidebar_collapsed') === 'true')
  const sidebarOpen = ref(false) // For mobile

  // Loading states
  const globalLoading = ref(false)
  const loadingMessages = ref<Map<string, string>>(new Map())

  // Toasts
  const toasts = ref<Toast[]>([])

  // Modals
  const modals = ref<Modal[]>([])

  // Theme
  const theme = ref<'light' | 'dark'>(
    (localStorage.getItem('theme') as 'light' | 'dark') || 'light'
  )

  // Search
  const searchQuery = ref('')
  const searchOpen = ref(false)

  // Computed
  const isLoading = computed(() => globalLoading.value || loadingMessages.value.size > 0)

  // Sidebar actions
  function toggleSidebar() {
    sidebarCollapsed.value = !sidebarCollapsed.value
    localStorage.setItem('sidebar_collapsed', String(sidebarCollapsed.value))
  }

  function openMobileSidebar() {
    sidebarOpen.value = true
  }

  function closeMobileSidebar() {
    sidebarOpen.value = false
  }

  // Loading actions
  function startLoading(key: string, message?: string) {
    loadingMessages.value.set(key, message || 'Loading...')
  }

  function stopLoading(key: string) {
    loadingMessages.value.delete(key)
  }

  function setGlobalLoading(loading: boolean) {
    globalLoading.value = loading
  }

  // Toast actions
  function showToast(toast: Omit<Toast, 'id'>) {
    const id = crypto.randomUUID()
    const newToast: Toast = { ...toast, id }
    toasts.value.push(newToast)

    // Auto-remove after duration
    const duration = toast.duration || 5000
    setTimeout(() => {
      removeToast(id)
    }, duration)
  }

  function removeToast(id: string) {
    const index = toasts.value.findIndex((t) => t.id === id)
    if (index !== -1) {
      toasts.value.splice(index, 1)
    }
  }

  function showSuccess(title: string, message?: string) {
    showToast({ type: 'success', title, message })
  }

  function showError(title: string, message?: string) {
    showToast({ type: 'error', title, message })
  }

  function showWarning(title: string, message?: string) {
    showToast({ type: 'warning', title, message })
  }

  function showInfo(title: string, message?: string) {
    showToast({ type: 'info', title, message })
  }

  // Modal actions
  function openModal(component: string, props?: Record<string, unknown>) {
    const id = crypto.randomUUID()
    modals.value.push({ id, component, props })
    return id
  }

  function closeModal(id?: string) {
    if (id) {
      const index = modals.value.findIndex((m) => m.id === id)
      if (index !== -1) {
        modals.value.splice(index, 1)
      }
    } else {
      modals.value.pop()
    }
  }

  function closeAllModals() {
    modals.value = []
  }

  // Theme actions
  function setTheme(newTheme: 'light' | 'dark') {
    theme.value = newTheme
    localStorage.setItem('theme', newTheme)
    document.documentElement.classList.toggle('dark', newTheme === 'dark')
  }

  function toggleTheme() {
    setTheme(theme.value === 'light' ? 'dark' : 'light')
  }

  // Search actions
  function openSearch() {
    searchOpen.value = true
  }

  function closeSearch() {
    searchOpen.value = false
    searchQuery.value = ''
  }

  return {
    // State
    sidebarCollapsed,
    sidebarOpen,
    globalLoading,
    loadingMessages,
    toasts,
    modals,
    theme,
    searchQuery,
    searchOpen,

    // Computed
    isLoading,

    // Actions
    toggleSidebar,
    openMobileSidebar,
    closeMobileSidebar,
    startLoading,
    stopLoading,
    setGlobalLoading,
    showToast,
    removeToast,
    showSuccess,
    showError,
    showWarning,
    showInfo,
    openModal,
    closeModal,
    closeAllModals,
    setTheme,
    toggleTheme,
    openSearch,
    closeSearch,
  }
})
