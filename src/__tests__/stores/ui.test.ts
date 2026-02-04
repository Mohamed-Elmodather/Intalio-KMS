import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useUIStore } from '@/stores/ui'

// Mock crypto.randomUUID
vi.stubGlobal('crypto', {
  randomUUID: () => 'test-uuid-' + Math.random().toString(36).substr(2, 9),
})

describe('UI Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
    localStorage.clear()
  })

  describe('Sidebar', () => {
    it('should have sidebar expanded by default', () => {
      const store = useUIStore()
      expect(store.sidebarCollapsed).toBe(false)
    })

    it('should toggle sidebar state', () => {
      const store = useUIStore()

      store.toggleSidebar()
      expect(store.sidebarCollapsed).toBe(true)
      expect(localStorage.getItem('sidebar_collapsed')).toBe('true')

      store.toggleSidebar()
      expect(store.sidebarCollapsed).toBe(false)
      expect(localStorage.getItem('sidebar_collapsed')).toBe('false')
    })

    it('should open and close mobile sidebar', () => {
      const store = useUIStore()

      expect(store.sidebarOpen).toBe(false)

      store.openMobileSidebar()
      expect(store.sidebarOpen).toBe(true)

      store.closeMobileSidebar()
      expect(store.sidebarOpen).toBe(false)
    })
  })

  describe('Loading State', () => {
    it('should track loading states with keys', () => {
      const store = useUIStore()

      store.startLoading('api-call', 'Fetching data...')
      expect(store.isLoading).toBe(true)
      expect(store.loadingMessages.get('api-call')).toBe('Fetching data...')

      store.stopLoading('api-call')
      expect(store.isLoading).toBe(false)
    })

    it('should handle multiple loading states', () => {
      const store = useUIStore()

      store.startLoading('api-1')
      store.startLoading('api-2')
      expect(store.isLoading).toBe(true)

      store.stopLoading('api-1')
      expect(store.isLoading).toBe(true) // Still loading api-2

      store.stopLoading('api-2')
      expect(store.isLoading).toBe(false)
    })

    it('should set global loading state', () => {
      const store = useUIStore()

      store.setGlobalLoading(true)
      expect(store.globalLoading).toBe(true)
      expect(store.isLoading).toBe(true)

      store.setGlobalLoading(false)
      expect(store.globalLoading).toBe(false)
    })
  })

  describe('Toasts', () => {
    it('should show toast notification', () => {
      const store = useUIStore()

      store.showToast({
        type: 'success',
        title: 'Success!',
        message: 'Operation completed',
      })

      expect(store.toasts).toHaveLength(1)
      expect(store.toasts[0].type).toBe('success')
      expect(store.toasts[0].title).toBe('Success!')
    })

    it('should show success toast', () => {
      const store = useUIStore()

      store.showSuccess('Done!', 'Task completed')

      expect(store.toasts).toHaveLength(1)
      expect(store.toasts[0].type).toBe('success')
    })

    it('should show error toast', () => {
      const store = useUIStore()

      store.showError('Error!', 'Something went wrong')

      expect(store.toasts).toHaveLength(1)
      expect(store.toasts[0].type).toBe('error')
    })

    it('should show warning toast', () => {
      const store = useUIStore()

      store.showWarning('Warning!', 'Please be careful')

      expect(store.toasts).toHaveLength(1)
      expect(store.toasts[0].type).toBe('warning')
    })

    it('should show info toast', () => {
      const store = useUIStore()

      store.showInfo('Info', 'Just so you know')

      expect(store.toasts).toHaveLength(1)
      expect(store.toasts[0].type).toBe('info')
    })

    it('should remove toast by id', () => {
      const store = useUIStore()

      store.showToast({ type: 'info', title: 'Test' })
      const toastId = store.toasts[0].id

      store.removeToast(toastId)

      expect(store.toasts).toHaveLength(0)
    })
  })

  describe('Modals', () => {
    it('should open modal and return id', () => {
      const store = useUIStore()

      const modalId = store.openModal('ConfirmDialog', { message: 'Are you sure?' })

      expect(modalId).toBeDefined()
      expect(store.modals).toHaveLength(1)
      expect(store.modals[0].component).toBe('ConfirmDialog')
      expect(store.modals[0].props?.message).toBe('Are you sure?')
    })

    it('should close modal by id', () => {
      const store = useUIStore()

      const modalId = store.openModal('TestModal')
      store.closeModal(modalId)

      expect(store.modals).toHaveLength(0)
    })

    it('should close last modal when no id provided', () => {
      const store = useUIStore()

      store.openModal('Modal1')
      store.openModal('Modal2')

      store.closeModal()

      expect(store.modals).toHaveLength(1)
      expect(store.modals[0].component).toBe('Modal1')
    })

    it('should close all modals', () => {
      const store = useUIStore()

      store.openModal('Modal1')
      store.openModal('Modal2')
      store.openModal('Modal3')

      store.closeAllModals()

      expect(store.modals).toHaveLength(0)
    })
  })

  describe('Theme', () => {
    it('should have light theme by default', () => {
      const store = useUIStore()
      expect(store.theme).toBe('light')
    })

    it('should set theme and save to localStorage', () => {
      const store = useUIStore()

      store.setTheme('dark')

      expect(store.theme).toBe('dark')
      expect(localStorage.getItem('theme')).toBe('dark')
    })

    it('should toggle theme', () => {
      const store = useUIStore()

      store.toggleTheme()
      expect(store.theme).toBe('dark')

      store.toggleTheme()
      expect(store.theme).toBe('light')
    })
  })

  describe('Search', () => {
    it('should open and close search', () => {
      const store = useUIStore()

      expect(store.searchOpen).toBe(false)

      store.openSearch()
      expect(store.searchOpen).toBe(true)

      store.closeSearch()
      expect(store.searchOpen).toBe(false)
    })

    it('should clear search query when closing', () => {
      const store = useUIStore()

      store.searchQuery = 'test query'
      store.searchOpen = true

      store.closeSearch()

      expect(store.searchQuery).toBe('')
      expect(store.searchOpen).toBe(false)
    })
  })
})
