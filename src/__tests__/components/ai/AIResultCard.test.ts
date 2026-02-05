import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AIResultCard from '@/components/ai/AIResultCard.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock clipboard API
const mockWriteText = vi.fn().mockResolvedValue(undefined)
Object.defineProperty(navigator, 'clipboard', {
  value: {
    writeText: mockWriteText,
  },
  writable: true,
  configurable: true,
})

function mountComponent(props = {}) {
  return shallowMount(AIResultCard, {
    props: { content: 'Test content', ...props },
    global: {
      mocks: {
        $t: (key: string) => key,
      },
      stubs: {
        AIConfidenceBar: true,
      },
    },
  })
}

describe('AIResultCard', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the card', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.ai-result-card').exists()).toBe(true)
    })

    it('should render content', () => {
      const wrapper = mountComponent({ content: 'Test Result Content' })
      expect(wrapper.text()).toContain('Test Result Content')
    })

    it('should render title when provided', () => {
      const wrapper = mountComponent({ title: 'Custom Title' })
      expect(wrapper.text()).toContain('Custom Title')
    })
  })

  describe('Type Config', () => {
    it('should show summary type config', () => {
      const wrapper = mountComponent({ type: 'summary' })
      const vm = wrapper.vm as any
      expect(vm.typeConfig.icon).toBe('fas fa-compress-alt')
      expect(vm.typeConfig.label).toBe('ai.aiSummary')
    })

    it('should show translation type config', () => {
      const wrapper = mountComponent({ type: 'translation' })
      const vm = wrapper.vm as any
      expect(vm.typeConfig.icon).toBe('fas fa-language')
      expect(vm.typeConfig.label).toBe('ai.aiTranslation')
    })

    it('should show classification type config', () => {
      const wrapper = mountComponent({ type: 'classification' })
      const vm = wrapper.vm as any
      expect(vm.typeConfig.icon).toBe('fas fa-tags')
      expect(vm.typeConfig.label).toBe('ai.aiClassification')
    })

    it('should show general type config by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.typeConfig.icon).toBe('fas fa-wand-magic-sparkles')
      expect(vm.typeConfig.label).toBe('ai.aiResult')
    })
  })

  describe('Loading State', () => {
    it('should show loading skeleton when loading', () => {
      const wrapper = mountComponent({ loading: true })
      expect(wrapper.find('.bg-gray-200.rounded').exists()).toBe(true)
    })

    it('should apply pulse animation when loading', () => {
      const wrapper = mountComponent({ loading: true })
      expect(wrapper.find('.animate-pulse').exists()).toBe(true)
    })

    it('should hide content when loading', () => {
      const wrapper = mountComponent({ loading: true, content: 'Test' })
      expect(wrapper.find('.whitespace-pre-wrap').exists()).toBe(false)
    })
  })

  describe('Error State', () => {
    it('should show error message when error', () => {
      const wrapper = mountComponent({ error: 'Something went wrong' })
      expect(wrapper.text()).toContain('Something went wrong')
    })

    it('should show error icon', () => {
      const wrapper = mountComponent({ error: 'Error' })
      expect(wrapper.find('.text-red-600 .fa-exclamation-circle').exists()).toBe(true)
    })
  })

  describe('Confidence Bar', () => {
    it('should show confidence bar when confidence provided', () => {
      const wrapper = mountComponent({ confidence: 0.85 })
      expect(wrapper.findComponent({ name: 'AIConfidenceBar' }).exists()).toBe(true)
    })

    it('should not show confidence bar when undefined', () => {
      const wrapper = mountComponent()
      expect(wrapper.findComponent({ name: 'AIConfidenceBar' }).exists()).toBe(false)
    })
  })

  describe('Action Buttons', () => {
    it('should show copy button by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('common.copy')
    })

    it('should hide copy button when showCopy is false', () => {
      const wrapper = mountComponent({ showCopy: false })
      expect(wrapper.text()).not.toContain('common.copy')
    })

    it('should show regenerate button by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('ai.regenerate')
    })

    it('should hide regenerate button when showRegenerate is false', () => {
      const wrapper = mountComponent({ showRegenerate: false })
      expect(wrapper.text()).not.toContain('ai.regenerate')
    })

    it('should show dismiss button by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-times').exists()).toBe(true)
    })

    it('should hide dismiss button when showDismiss is false', () => {
      const wrapper = mountComponent({ showDismiss: false })
      const header = wrapper.find('.px-4.py-3.bg-gradient-to-r')
      expect(header.find('.fa-times').exists()).toBe(false)
    })
  })

  describe('Events', () => {
    it('should emit dismiss when dismiss clicked', async () => {
      const wrapper = mountComponent()
      const dismissBtn = wrapper.find('.fa-times').element.parentElement
      await dismissBtn?.click()
      expect(wrapper.emitted('dismiss')).toBeTruthy()
    })

    it('should emit regenerate when regenerate clicked', async () => {
      const wrapper = mountComponent()
      const buttons = wrapper.findAll('button')
      const regenerateBtn = buttons.find(b => b.text().includes('ai.regenerate'))
      await regenerateBtn?.trigger('click')
      expect(wrapper.emitted('regenerate')).toBeTruthy()
    })

    it('should copy content to clipboard and emit copy', async () => {
      const wrapper = mountComponent({ content: 'Copy this' })
      const buttons = wrapper.findAll('button')
      const copyBtn = buttons.find(b => b.text().includes('common.copy'))
      await copyBtn?.trigger('click')
      expect(mockWriteText).toHaveBeenCalledWith('Copy this')
      expect(wrapper.emitted('copy')).toBeTruthy()
    })
  })

  describe('Hide Actions When Loading/Error', () => {
    it('should hide actions when loading', () => {
      const wrapper = mountComponent({ loading: true })
      expect(wrapper.find('.px-4.py-3.bg-gray-50').exists()).toBe(false)
    })

    it('should hide actions when error', () => {
      const wrapper = mountComponent({ error: 'Error' })
      expect(wrapper.find('.px-4.py-3.bg-gray-50').exists()).toBe(false)
    })
  })
})
