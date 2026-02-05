import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AIVoiceInput from '@/components/ai/AIVoiceInput.vue'
import { ref } from 'vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock useVoiceInput composable
const mockStartListening = vi.fn()
const mockStopListening = vi.fn()
const mockToggleListening = vi.fn()
const mockSetLanguage = vi.fn()

vi.mock('@/composables/useVoiceInput', () => ({
  useVoiceInput: () => ({
    isListening: ref(false),
    isSupported: ref(true),
    transcript: ref(''),
    interimTranscript: ref(''),
    error: ref(null),
    confidence: ref(0),
    startListening: mockStartListening,
    stopListening: mockStopListening,
    toggleListening: mockToggleListening,
    setLanguage: mockSetLanguage,
    supportedLanguages: [
      { code: 'en-US', name: 'English', flag: '🇺🇸' },
      { code: 'ar-SA', name: 'Arabic', flag: '🇸🇦' },
    ],
  }),
}))

function mountComponent(props = {}) {
  return shallowMount(AIVoiceInput, {
    props,
    global: {
      mocks: {
        $t: (key: string) => key,
      },
    },
  })
}

describe('AIVoiceInput', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.voice-input-container').exists()).toBe(true)
    })

    it('should render voice button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.voice-button').exists()).toBe(true)
    })

    it('should show microphone icon when not listening', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-microphone').exists()).toBe(true)
    })
  })

  describe('Size Classes', () => {
    it('should apply small size classes', () => {
      const wrapper = mountComponent({ size: 'sm' })
      const button = wrapper.find('.voice-button')
      expect(button.classes()).toContain('w-8')
      expect(button.classes()).toContain('h-8')
    })

    it('should apply medium size classes by default', () => {
      const wrapper = mountComponent()
      const button = wrapper.find('.voice-button')
      expect(button.classes()).toContain('w-10')
      expect(button.classes()).toContain('h-10')
    })

    it('should apply large size classes', () => {
      const wrapper = mountComponent({ size: 'lg' })
      const button = wrapper.find('.voice-button')
      expect(button.classes()).toContain('w-12')
      expect(button.classes()).toContain('h-12')
    })
  })

  describe('Voice Button Click', () => {
    it('should call toggleListening on click', async () => {
      const wrapper = mountComponent()
      const button = wrapper.find('.voice-button')
      await button.trigger('click')
      expect(mockToggleListening).toHaveBeenCalled()
    })
  })

  describe('Language Selector', () => {
    it('should show language selector when showLanguageSelector is true', () => {
      const wrapper = mountComponent({ showLanguageSelector: true })
      expect(wrapper.find('.language-selector').exists()).toBe(true)
    })

    it('should hide language selector by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.language-selector').exists()).toBe(false)
    })

    it('should toggle language dropdown on click', async () => {
      const wrapper = mountComponent({ showLanguageSelector: true })
      const langButton = wrapper.find('.language-button')
      await langButton.trigger('click')
      const vm = wrapper.vm as any
      expect(vm.showLanguageDropdown).toBe(true)
    })
  })

  describe('Default Props', () => {
    it('should have default language', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedLanguage).toBe('en-US')
    })

    it('should have default size', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.size).toBe('md')
    })

    it('should have showLanguageSelector false by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showLanguageSelector).toBe(false)
    })
  })

  describe('Selected Language', () => {
    it('should use provided language prop', () => {
      const wrapper = mountComponent({ language: 'ar-SA' })
      const vm = wrapper.vm as any
      expect(vm.selectedLanguage).toBe('ar-SA')
    })

    it('should get selected language info', () => {
      const wrapper = mountComponent({ language: 'en-US', showLanguageSelector: true })
      const vm = wrapper.vm as any
      const langInfo = vm.getSelectedLanguageInfo()
      expect(langInfo.code).toBe('en-US')
      expect(langInfo.name).toBe('English')
    })
  })

  describe('Select Language', () => {
    it('should update selected language', async () => {
      const wrapper = mountComponent({ showLanguageSelector: true })
      const vm = wrapper.vm as any
      vm.selectLanguage('ar-SA')
      expect(vm.selectedLanguage).toBe('ar-SA')
      expect(vm.showLanguageDropdown).toBe(false)
    })
  })

  describe('Button Styling', () => {
    it('should apply idle styling when not listening', () => {
      const wrapper = mountComponent()
      const button = wrapper.find('.voice-button')
      expect(button.classes()).toContain('bg-gray-100')
      expect(button.classes()).toContain('text-gray-500')
    })
  })
})
