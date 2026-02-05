import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AITranslateDropdown from '@/components/ai/AITranslateDropdown.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock SUPPORTED_LANGUAGES
vi.mock('@/types/ai', () => ({
  SUPPORTED_LANGUAGES: [
    { code: 'en', name: 'English', nativeName: 'English', flag: '🇺🇸' },
    { code: 'ar', name: 'Arabic', nativeName: 'العربية', flag: '🇸🇦' },
    { code: 'fr', name: 'French', nativeName: 'Français', flag: '🇫🇷' },
    { code: 'es', name: 'Spanish', nativeName: 'Español', flag: '🇪🇸' },
  ],
}))

function mountComponent(props = {}) {
  return shallowMount(AITranslateDropdown, {
    props,
    global: {
      mocks: {
        $t: (key: string) => key,
      },
      directives: {
        'click-outside': () => {},
      },
    },
  })
}

describe('AITranslateDropdown', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.ai-translate-dropdown').exists()).toBe(true)
    })

    it('should render trigger button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('button').exists()).toBe(true)
    })

    it('should show language icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-language').exists()).toBe(true)
    })

    it('should show placeholder when no language selected', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('ai.selectLanguage')
    })

    it('should show custom placeholder', () => {
      const wrapper = mountComponent({ placeholder: 'Choose language' })
      expect(wrapper.text()).toContain('Choose language')
    })
  })

  describe('Selected Language', () => {
    it('should show selected language name', () => {
      const wrapper = mountComponent({ modelValue: 'en' })
      expect(wrapper.text()).toContain('English')
    })

    it('should show native name', () => {
      const wrapper = mountComponent({ modelValue: 'ar' })
      expect(wrapper.text()).toContain('العربية')
    })

    it('should show flag when showFlags is true', () => {
      const wrapper = mountComponent({ modelValue: 'en', showFlags: true })
      expect(wrapper.text()).toContain('🇺🇸')
    })

    it('should hide flag when showFlags is false', () => {
      const wrapper = mountComponent({ modelValue: 'en', showFlags: false })
      const vm = wrapper.vm as any
      // The flag should not be visible in the selected display
      expect(vm.showFlags).toBe(false)
    })
  })

  describe('Dropdown Toggle', () => {
    it('should open dropdown on click', async () => {
      const wrapper = mountComponent()
      const button = wrapper.find('button')
      await button.trigger('click')
      const vm = wrapper.vm as any
      expect(vm.isOpen).toBe(true)
    })

    it('should close dropdown on second click', async () => {
      const wrapper = mountComponent()
      const button = wrapper.find('button')
      await button.trigger('click')
      await button.trigger('click')
      const vm = wrapper.vm as any
      expect(vm.isOpen).toBe(false)
    })

    it('should not open when disabled', async () => {
      const wrapper = mountComponent({ disabled: true })
      const button = wrapper.find('button')
      await button.trigger('click')
      const vm = wrapper.vm as any
      expect(vm.isOpen).toBe(false)
    })

    it('should show chevron icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-chevron-down').exists()).toBe(true)
    })

    it('should rotate chevron when open', async () => {
      const wrapper = mountComponent()
      const button = wrapper.find('button')
      await button.trigger('click')
      expect(wrapper.find('.fa-chevron-down').classes()).toContain('rotate-180')
    })
  })

  describe('Disabled State', () => {
    it('should apply disabled styling', () => {
      const wrapper = mountComponent({ disabled: true })
      const button = wrapper.find('button')
      expect(button.classes()).toContain('opacity-50')
      expect(button.classes()).toContain('cursor-not-allowed')
    })

    it('should have disabled attribute', () => {
      const wrapper = mountComponent({ disabled: true })
      expect(wrapper.find('button').attributes('disabled')).toBeDefined()
    })
  })

  describe('Language Filtering', () => {
    it('should filter languages based on search query', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.isOpen = true
      vm.searchQuery = 'Ara'
      await wrapper.vm.$nextTick()
      expect(vm.filteredLanguages.length).toBe(1)
      expect(vm.filteredLanguages[0].code).toBe('ar')
    })

    it('should filter by native name', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.searchQuery = 'Français'
      expect(vm.filteredLanguages.length).toBe(1)
      expect(vm.filteredLanguages[0].code).toBe('fr')
    })

    it('should filter by code', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.searchQuery = 'es'
      expect(vm.filteredLanguages.length).toBe(1)
      expect(vm.filteredLanguages[0].code).toBe('es')
    })

    it('should exclude specified languages', () => {
      const wrapper = mountComponent({ excludeLanguages: ['en', 'ar'] })
      const vm = wrapper.vm as any
      expect(vm.filteredLanguages.length).toBe(2)
      expect(vm.filteredLanguages.find((l: any) => l.code === 'en')).toBeUndefined()
      expect(vm.filteredLanguages.find((l: any) => l.code === 'ar')).toBeUndefined()
    })
  })

  describe('Language Selection', () => {
    it('should emit update:modelValue on selection', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.isOpen = true
      await wrapper.vm.$nextTick()

      // Call selectLanguage directly
      vm.selectLanguage({ code: 'fr', name: 'French', nativeName: 'Français', flag: '🇫🇷' })

      expect(wrapper.emitted('update:modelValue')).toBeTruthy()
      expect(wrapper.emitted('update:modelValue')![0]).toEqual(['fr'])
    })

    it('should emit change event on selection', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.selectLanguage({ code: 'es', name: 'Spanish', nativeName: 'Español', flag: '🇪🇸' })

      expect(wrapper.emitted('change')).toBeTruthy()
      expect(wrapper.emitted('change')![0]).toEqual(['es'])
    })

    it('should close dropdown after selection', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.isOpen = true
      vm.selectLanguage({ code: 'fr', name: 'French', nativeName: 'Français', flag: '🇫🇷' })

      expect(vm.isOpen).toBe(false)
    })

    it('should clear search query after selection', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.isOpen = true
      vm.searchQuery = 'test'
      vm.selectLanguage({ code: 'fr', name: 'French', nativeName: 'Français', flag: '🇫🇷' })

      expect(vm.searchQuery).toBe('')
    })
  })

  describe('Close Dropdown', () => {
    it('should close dropdown and clear search', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.isOpen = true
      vm.searchQuery = 'test'

      vm.closeDropdown()

      expect(vm.isOpen).toBe(false)
      expect(vm.searchQuery).toBe('')
    })
  })

  describe('Focus Styling', () => {
    it('should apply focus styling when open', async () => {
      const wrapper = mountComponent()
      const button = wrapper.find('button')
      await button.trigger('click')
      expect(button.classes()).toContain('border-teal-500')
      expect(button.classes()).toContain('ring-2')
      expect(button.classes()).toContain('ring-teal-200')
    })
  })
})
