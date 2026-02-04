import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useLocaleStore } from '@/stores/locale'

describe('Locale Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
    localStorage.clear()
  })

  describe('Initial State', () => {
    it('should default to English locale', () => {
      const store = useLocaleStore()
      expect(store.currentLocale).toBe('en')
    })

    it('should load locale from localStorage if available', () => {
      localStorage.setItem('locale', 'ar')
      const store = useLocaleStore()
      expect(store.currentLocale).toBe('ar')
    })

    it('should have isRTL false for English', () => {
      const store = useLocaleStore()
      expect(store.isRTL).toBe(false)
    })

    it('should have direction ltr for English', () => {
      const store = useLocaleStore()
      expect(store.direction).toBe('ltr')
    })
  })

  describe('setLocale', () => {
    it('should update locale to Arabic', () => {
      const store = useLocaleStore()
      store.setLocale('ar')
      expect(store.currentLocale).toBe('ar')
    })

    it('should save locale to localStorage', () => {
      const store = useLocaleStore()
      store.setLocale('ar')
      expect(localStorage.getItem('locale')).toBe('ar')
    })

    it('should update isRTL when switching to Arabic', () => {
      const store = useLocaleStore()
      store.setLocale('ar')
      expect(store.isRTL).toBe(true)
    })

    it('should update direction when switching to Arabic', () => {
      const store = useLocaleStore()
      store.setLocale('ar')
      expect(store.direction).toBe('rtl')
    })

    it('should update isRTL when switching back to English', () => {
      const store = useLocaleStore()
      store.setLocale('ar')
      store.setLocale('en')
      expect(store.isRTL).toBe(false)
    })
  })

  describe('toggleLocale', () => {
    it('should toggle from English to Arabic', () => {
      const store = useLocaleStore()
      store.toggleLocale()
      expect(store.currentLocale).toBe('ar')
    })

    it('should toggle from Arabic to English', () => {
      const store = useLocaleStore()
      store.setLocale('ar')
      store.toggleLocale()
      expect(store.currentLocale).toBe('en')
    })

    it('should update RTL state when toggling', () => {
      const store = useLocaleStore()
      expect(store.isRTL).toBe(false)
      store.toggleLocale()
      expect(store.isRTL).toBe(true)
      store.toggleLocale()
      expect(store.isRTL).toBe(false)
    })
  })

  describe('initLocale', () => {
    it('should apply direction to document', () => {
      const store = useLocaleStore()
      store.initLocale()
      expect(document.documentElement.getAttribute('dir')).toBe('ltr')
      expect(document.documentElement.getAttribute('lang')).toBe('en')
    })

    it('should apply RTL direction for Arabic', () => {
      const store = useLocaleStore()
      store.setLocale('ar')
      store.initLocale()
      expect(document.documentElement.getAttribute('dir')).toBe('rtl')
      expect(document.documentElement.getAttribute('lang')).toBe('ar')
    })
  })
})
