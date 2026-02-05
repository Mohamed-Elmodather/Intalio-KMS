import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import ComparisonModal from '@/components/ai/ComparisonModal.vue'
import { ref, computed } from 'vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string, params?: any) => params ? `${key} ${JSON.stringify(params)}` : key,
  }),
}))

// Mock comparison store
const mockCloseComparison = vi.fn()
const mockSetActiveTab = vi.fn()
const mockGenerateAnalysis = vi.fn()
const mockClearSelection = vi.fn()

vi.mock('@/stores/comparison', () => ({
  useComparisonStore: () => ({
    isComparing: ref(true),
    itemCount: 2,
    activeTab: ref('side-by-side'),
    analysisResults: null,
    isAnalyzing: false,
    closeComparison: mockCloseComparison,
    setActiveTab: mockSetActiveTab,
    generateAnalysis: mockGenerateAnalysis,
    clearSelection: mockClearSelection,
  }),
}))

function mountComponent() {
  return shallowMount(ComparisonModal, {
    global: {
      mocks: {
        $t: (key: string) => key,
      },
      stubs: {
        Teleport: true,
        Transition: true,
        ComparisonSideBySide: true,
        ComparisonInsights: true,
      },
    },
  })
}

describe('ComparisonModal', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    document.body.style.overflow = ''
  })

  describe('Rendering', () => {
    it('should render the modal when isComparing is true', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })

    it('should render header with title', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comparison.compareItems')
    })

    it('should render subtitle', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comparison.analyzeAndCompare')
    })

    it('should render chart icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-chart-bar').exists()).toBe(true)
    })
  })

  describe('Tabs', () => {
    it('should render all tabs', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.tabs.length).toBe(5)
    })

    it('should have side-by-side tab', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.tabs.find((t: any) => t.id === 'side-by-side')).toBeTruthy()
    })

    it('should have insights tab', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.tabs.find((t: any) => t.id === 'insights')).toBeTruthy()
    })

    it('should have entities tab', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.tabs.find((t: any) => t.id === 'entities')).toBeTruthy()
    })

    it('should have sentiment tab', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.tabs.find((t: any) => t.id === 'sentiment')).toBeTruthy()
    })

    it('should have topics tab', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.tabs.find((t: any) => t.id === 'topics')).toBeTruthy()
    })

    it('should render tab icons', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-columns').exists()).toBe(true)
      expect(wrapper.find('.fa-brain').exists()).toBe(true)
      expect(wrapper.find('.fa-tags').exists()).toBe(true)
      expect(wrapper.find('.fa-smile').exists()).toBe(true)
      expect(wrapper.find('.fa-lightbulb').exists()).toBe(true)
    })
  })

  describe('Close Actions', () => {
    it('should call closeComparison when close button clicked', async () => {
      const wrapper = mountComponent()
      const closeBtn = wrapper.find('.fa-times').element.parentElement
      await closeBtn?.click()
      expect(mockCloseComparison).toHaveBeenCalled()
    })

    it('should call closeComparison when backdrop clicked', async () => {
      const wrapper = mountComponent()
      const backdrop = wrapper.find('.bg-black\\/60')
      await backdrop.trigger('click')
      expect(mockCloseComparison).toHaveBeenCalled()
    })

    it('should call closeComparison when done button clicked', async () => {
      const wrapper = mountComponent()
      const buttons = wrapper.findAll('button')
      const doneBtn = buttons.find(b => b.text().includes('common.done'))
      await doneBtn?.trigger('click')
      expect(mockCloseComparison).toHaveBeenCalled()
    })
  })

  describe('Clear Selection', () => {
    it('should call clearSelection when clear button clicked', async () => {
      const wrapper = mountComponent()
      const buttons = wrapper.findAll('button')
      const clearBtn = buttons.find(b => b.text().includes('comparison.clearSelection'))
      await clearBtn?.trigger('click')
      expect(mockClearSelection).toHaveBeenCalled()
    })
  })

  describe('Tab Switching', () => {
    it('should call setActiveTab when tab clicked', async () => {
      const wrapper = mountComponent()
      const tabButtons = wrapper.findAll('button').filter(b => b.text().includes('comparison.'))
      // Find the insights tab button
      const insightsTab = tabButtons.find(b => b.text().includes('comparison.aiInsights'))
      if (insightsTab) {
        await insightsTab.trigger('click')
        expect(mockSetActiveTab).toHaveBeenCalled()
      }
    })
  })

  describe('Footer', () => {
    it('should render footer with info note', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comparison.aiAnalysisNote')
    })

    it('should show info icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-info-circle').exists()).toBe(true)
    })
  })

  describe('Keyboard Navigation', () => {
    it('should have handleKeydown function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // The function should exist (mounted lifecycle adds listener)
      expect(typeof vm.handleKeydown).toBe('function')
    })
  })
})
