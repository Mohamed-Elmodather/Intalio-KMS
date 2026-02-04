import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { ref, computed } from 'vue'
import { createPinia, setActivePinia } from 'pinia'
import LanguageSwitcher from '@/components/common/LanguageSwitcher.vue'

// Mock vue-i18n
const mockLocale = ref('en')
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    locale: mockLocale,
  }),
}))

// Mock the locale store
const mockCurrentLocale = ref('en')
const mockSetLocale = vi.fn()

vi.mock('@/stores/locale', () => ({
  useLocaleStore: () => ({
    currentLocale: mockCurrentLocale.value,
    setLocale: mockSetLocale,
  }),
}))

describe('LanguageSwitcher', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockCurrentLocale.value = 'en'
    mockLocale.value = 'en'
    setActivePinia(createPinia())
  })

  describe('Rendering', () => {
    it('should render button element', () => {
      const wrapper = mount(LanguageSwitcher)
      expect(wrapper.find('button').exists()).toBe(true)
    })

    it('should render globe icon', () => {
      const wrapper = mount(LanguageSwitcher)
      expect(wrapper.find('.fa-globe').exists()).toBe(true)
    })

    it('should show "عربي" when locale is English', () => {
      mockCurrentLocale.value = 'en'
      const wrapper = mount(LanguageSwitcher)
      expect(wrapper.text()).toContain('عربي')
    })

    it('should show "EN" when locale is Arabic', () => {
      mockCurrentLocale.value = 'ar'
      const wrapper = mount(LanguageSwitcher)
      expect(wrapper.text()).toContain('EN')
    })
  })

  describe('Title Attribute', () => {
    it('should have Arabic switch title when in English', () => {
      mockCurrentLocale.value = 'en'
      const wrapper = mount(LanguageSwitcher)
      expect(wrapper.find('button').attributes('title')).toBe('التبديل للعربية')
    })

    it('should have English switch title when in Arabic', () => {
      mockCurrentLocale.value = 'ar'
      const wrapper = mount(LanguageSwitcher)
      expect(wrapper.find('button').attributes('title')).toBe('Switch to English')
    })
  })

  describe('Interactions', () => {
    it('should call setLocale with "ar" when switching from English', async () => {
      mockCurrentLocale.value = 'en'
      const wrapper = mount(LanguageSwitcher)
      await wrapper.find('button').trigger('click')

      expect(mockSetLocale).toHaveBeenCalledWith('ar')
    })

    it('should call setLocale with "en" when switching from Arabic', async () => {
      mockCurrentLocale.value = 'ar'
      const wrapper = mount(LanguageSwitcher)
      await wrapper.find('button').trigger('click')

      expect(mockSetLocale).toHaveBeenCalledWith('en')
    })
  })

  describe('Styling', () => {
    it('should have rounded-lg class', () => {
      const wrapper = mount(LanguageSwitcher)
      expect(wrapper.find('button').classes()).toContain('rounded-lg')
    })

    it('should have flex and items-center classes', () => {
      const wrapper = mount(LanguageSwitcher)
      const button = wrapper.find('button')
      expect(button.classes()).toContain('flex')
      expect(button.classes()).toContain('items-center')
    })

    it('should have hover state class', () => {
      const wrapper = mount(LanguageSwitcher)
      expect(wrapper.find('button').classes()).toContain('hover:bg-gray-100')
    })
  })
})
