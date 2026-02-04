import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import PollCreatePage from '@/pages/polls/PollCreatePage.vue'

// Mock vue-router
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
    back: vi.fn(),
  }),
  useRoute: () => ({
    params: {},
    query: {},
  }),
}))

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock stores
vi.mock('@/stores/aiServices', () => ({
  useAIServicesStore: () => ({
    isLoading: false,
    error: null,
  }),
}))

vi.mock('@/stores/ui', () => ({
  useUIStore: () => ({
    showToast: vi.fn(),
  }),
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AISuggestionChip: { template: '<span class="ai-suggestion"></span>' },
}))

function mountComponent() {
  return shallowMount(PollCreatePage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
        'router-link': true,
      },
    },
  })
}

describe('PollCreatePage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render poll create page', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Text Constants', () => {
    it('should have text constants defined', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants).toBeDefined()
    })

    it('should have page title', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.pageTitle).toBeDefined()
    })

    it('should have step labels', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.step1).toBeDefined()
      expect(vm.textConstants.step2).toBeDefined()
      expect(vm.textConstants.step3).toBeDefined()
      expect(vm.textConstants.step4).toBeDefined()
    })

    it('should have action labels', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.createPoll).toBeDefined()
      expect(vm.textConstants.saveDraft).toBeDefined()
      expect(vm.textConstants.cancel).toBeDefined()
    })
  })

  describe('Form State', () => {
    it('should have current step', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.currentStep).toBe(1)
    })

    it('should have is creating state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isCreating).toBe(false)
    })
  })

  describe('Question Fields', () => {
    it('should have question field', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.question).toBe('')
    })

    it('should have description field', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.description).toBe('')
    })

    it('should have selected category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedCategory).toBe('')
    })
  })

  describe('Options', () => {
    it('should have options array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.options).toBeDefined()
      expect(Array.isArray(vm.options)).toBe(true)
    })

    it('should have initial empty options', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.options.length).toBeGreaterThanOrEqual(2)
    })
  })

  describe('Settings', () => {
    it('should have allow multiple setting', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.allowMultiple).toBe('boolean')
    })

    it('should have anonymous voting setting', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isAnonymous).toBe('boolean')
    })

    it('should have results visibility setting', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.resultsVisibility).toBeDefined()
    })

    it('should have target audience', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.targetAudience).toBeDefined()
    })

    it('should have require comment setting', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.requireComment).toBe('boolean')
    })
  })

  describe('Schedule Settings', () => {
    it('should have schedule start option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.scheduleStart).toBe('boolean')
    })

    it('should have start date field', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.startDate).toBeDefined()
    })

    it('should have selected duration', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedDuration).toBeDefined()
    })

    it('should have custom end date', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.customEndDate).toBeDefined()
    })
  })

  describe('AI Features', () => {
    it('should have show AI suggestions modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showAISuggestionsModal).toBe('boolean')
    })

    it('should have is generating suggestions state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isGeneratingSuggestions).toBe('boolean')
    })

    it('should have AI suggestions array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiSuggestions).toBeDefined()
      expect(Array.isArray(vm.aiSuggestions)).toBe(true)
    })

    it('should have AI selected category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiSelectedCategory).toBeDefined()
    })

    it('should have AI generation mode', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiGenerationMode).toBeDefined()
    })

    it('should have AI topic input', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiTopicInput).toBeDefined()
    })

    it('should have AI content input', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiContentInput).toBeDefined()
    })
  })

  describe('Preview', () => {
    it('should have show preview state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showPreview).toBe('boolean')
    })
  })

  describe('Categories', () => {
    it('should have categories defined', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // Categories are in textConstants
      expect(vm.textConstants.catGeneral).toBeDefined()
      expect(vm.textConstants.catFeedback).toBeDefined()
    })
  })
})
