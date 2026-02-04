import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import CollaborationPage from '@/pages/collaboration/CollaborationPage.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AISuggestionChip: { template: '<span class="ai-suggestion"></span>' },
  AISentimentBadge: { template: '<span class="ai-sentiment"></span>' },
}))

// Mock stores
vi.mock('@/stores/aiServices', () => ({
  useAIServicesStore: () => ({
    isLoading: false,
    error: null,
  }),
}))

function mountComponent() {
  return shallowMount(CollaborationPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
      },
    },
  })
}

describe('CollaborationPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render collaboration page', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Channel State', () => {
    it('should have selected channel id', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedChannelId).toBe('1')
    })

    it('should have selected DM id null', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedDMId).toBeNull()
    })
  })

  describe('Message State', () => {
    it('should have message input empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.messageInput).toBe('')
    })

    it('should have search query empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.searchQuery).toBe('')
    })
  })

  describe('UI State', () => {
    it('should have show right panel state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showRightPanel).toBe(true)
    })

    it('should have left panel collapsed state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.leftPanelCollapsed).toBe(false)
    })

    it('should have right panel collapsed state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.rightPanelCollapsed).toBe(false)
    })

    it('should have right panel tab', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.rightPanelTab).toBe('details')
    })
  })

  describe('AI Features', () => {
    it('should have AI features enabled', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showAIFeatures).toBe(true)
    })

    it('should have unified search query', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.unifiedSearchQuery).toBe('')
    })

    it('should have AI search mode state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isAISearchMode).toBe(false)
    })

    it('should have show AI suggestions state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showAISuggestions).toBe(false)
    })

    it('should have natural language search suggestions', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.nlSearchSuggestions).toBeDefined()
      expect(vm.nlSearchSuggestions.length).toBeGreaterThan(0)
    })
  })

  describe('Modal States', () => {
    it('should have create channel modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showCreateChannelModal).toBe(false)
    })

    it('should have content picker state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showContentPicker).toBe(false)
    })

    it('should have emoji picker state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showEmojiPicker).toBe(false)
    })

    it('should have mention autocomplete state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showMentionAutocomplete).toBe(false)
    })
  })

  describe('Thread State', () => {
    it('should have expanded thread id null', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.expandedThreadId).toBeNull()
    })

    it('should have thread reply input empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.threadReplyInput).toBe('')
    })
  })

  describe('Typing State', () => {
    it('should have is typing state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isTyping).toBe(false)
    })

    it('should have typing users array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.typingUsers).toBeDefined()
      expect(Array.isArray(vm.typingUsers)).toBe(true)
    })
  })

  describe('Team State', () => {
    it('should have expanded team ids set', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.expandedTeamIds).toBeDefined()
    })
  })

  describe('New Channel Form', () => {
    it('should have new channel form', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.newChannel).toBeDefined()
    })

    it('should have new channel name empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.newChannel.name).toBe('')
    })

    it('should have new channel description empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.newChannel.description).toBe('')
    })
  })

  describe('Mention State', () => {
    it('should have mention query empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.mentionQuery).toBe('')
    })
  })
})
