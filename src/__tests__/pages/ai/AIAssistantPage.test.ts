import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { ref } from 'vue'
import AIAssistantPage from '@/pages/ai/AIAssistantPage.vue'

// Mock vue-router
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
  useRoute: () => ({
    query: {},
  }),
}))

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock components
vi.mock('@/components/common/LoadingSpinner.vue', () => ({
  default: { template: '<div class="loading-spinner"></div>' },
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AIMessageContent: { template: '<div class="ai-message"></div>' },
  AIVoiceInput: { template: '<div class="ai-voice"></div>' },
  AIOperationProgress: { template: '<div class="ai-progress"></div>' },
  AIContentSuggestions: { template: '<div class="ai-suggestions"></div>' },
  AIWorkflowBuilder: { template: '<div class="ai-workflow"></div>' },
}))
vi.mock('@/components/ai/ComparisonPanel.vue', () => ({
  default: { template: '<div class="comparison-panel"></div>' },
}))
vi.mock('@/components/ai/ComparisonModal.vue', () => ({
  default: { template: '<div class="comparison-modal"></div>' },
}))

// Mock stores
vi.mock('@/stores/aiServices', () => ({
  useAIServicesStore: () => ({
    isLoading: false,
    error: null,
  }),
}))
vi.mock('@/stores/comparison', () => ({
  useComparisonStore: () => ({
    items: [],
    isOpen: false,
  }),
}))
vi.mock('@/stores/aiWorkflows', () => ({
  useAIWorkflowsStore: () => ({
    initialize: vi.fn(),
    workflows: [],
  }),
}))
vi.mock('@/stores/aiPreferences', () => ({
  useAIPreferencesStore: () => ({
    initialize: vi.fn(),
    preferences: {},
  }),
}))
vi.mock('@/stores/ui', () => ({
  useUIStore: () => ({
    sidebarCollapsed: false,
  }),
}))

// Mock composables
vi.mock('@/composables/useSlashCommands', () => ({
  useSlashCommands: () => ({
    parseInput: vi.fn(),
    filterCommands: vi.fn(() => []),
    SLASH_COMMANDS: [],
  }),
}))
vi.mock('@/composables/useVoiceInput', () => ({
  useVoiceInput: () => ({
    isListening: ref(false),
    isSupported: ref(true),
    transcript: ref(''),
    startListening: vi.fn(),
    stopListening: vi.fn(),
  }),
}))

function mountComponent() {
  return shallowMount(AIAssistantPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
      },
    },
  })
}

describe('AIAssistantPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render AI assistant page', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Chat State', () => {
    it('should have messages array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.messages).toBeDefined()
      expect(Array.isArray(vm.messages)).toBe(true)
    })

    it('should have chat history', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.chatHistory).toBeDefined()
      expect(Array.isArray(vm.chatHistory)).toBe(true)
    })

    it('should have input message empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.inputMessage).toBe('')
    })

    it('should have is typing state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isTyping).toBe('boolean')
    })
  })

  describe('Settings', () => {
    it('should have settings object', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.settings).toBeDefined()
    })

    it('should have style setting', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.settings.style).toBeDefined()
    })

    it('should have language setting', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.settings.language).toBeDefined()
    })

    it('should have show sources setting', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.settings.showSources).toBe('boolean')
    })

    it('should have save history setting', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.settings.saveHistory).toBe('boolean')
    })
  })

  describe('UI State', () => {
    it('should have show settings state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showSettings).toBe('boolean')
    })

    it('should have show tools palette state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showToolsPalette).toBe('boolean')
    })

    it('should have show workflow builder state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showWorkflowBuilder).toBe('boolean')
    })

    it('should have history sidebar collapsed state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isHistorySidebarCollapsed).toBe('boolean')
    })
  })

  describe('Suggested Prompts', () => {
    it('should have suggested prompts', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.suggestedPrompts).toBeDefined()
      expect(Array.isArray(vm.suggestedPrompts)).toBe(true)
    })

    it('should have prompt properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      if (vm.suggestedPrompts.length > 0) {
        const prompt = vm.suggestedPrompts[0]
        expect(prompt.title).toBeDefined()
        expect(prompt.text).toBeDefined()
        expect(prompt.icon).toBeDefined()
      }
    })
  })

  describe('Quick Actions', () => {
    it('should have quick actions', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.quickActions).toBeDefined()
      expect(Array.isArray(vm.quickActions)).toBe(true)
    })
  })

  describe('Functions', () => {
    it('should have sendMessage function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.sendMessage).toBe('function')
    })

    it('should have startNewChat function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.startNewChat).toBe('function')
    })
  })

  describe('Context Panel', () => {
    it('should have show context panel state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showContextPanel).toBe('boolean')
    })

    it('should have show content browser state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showContentBrowser).toBe('boolean')
    })
  })
})
