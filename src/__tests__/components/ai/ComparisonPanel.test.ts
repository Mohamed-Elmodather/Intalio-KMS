import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import ComparisonPanel from '@/components/ai/ComparisonPanel.vue'
import { ref, computed } from 'vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string, params?: any) => params ? `${key} ${JSON.stringify(params)}` : key,
  }),
}))

const mockSelectedItems = [
  { id: '1', title: 'Document 1', type: 'document' },
  { id: '2', title: 'Article 1', type: 'article' },
  { id: '3', title: 'Media 1', type: 'media' },
]

const mockTogglePanelMinimized = vi.fn()
const mockRemoveItem = vi.fn()
const mockClearSelection = vi.fn()
const mockStartComparison = vi.fn()

vi.mock('@/stores/comparison', () => ({
  useComparisonStore: () => ({
    hasItems: true,
    isPanelMinimized: false,
    itemCount: 3,
    selectedItems: mockSelectedItems,
    canCompare: true,
    typeBreakdown: { document: 1, article: 1, media: 1 },
    togglePanelMinimized: mockTogglePanelMinimized,
    removeItem: mockRemoveItem,
    clearSelection: mockClearSelection,
    startComparison: mockStartComparison,
  }),
}))

function mountComponent() {
  return shallowMount(ComparisonPanel, {
    global: {
      mocks: {
        $t: (key: string) => key,
      },
      stubs: {
        Teleport: true,
        Transition: true,
      },
    },
  })
}

describe('ComparisonPanel', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render when hasItems is true', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })

    it('should render header', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comparison.compare')
    })

    it('should show item count', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comparison.itemsCount')
    })

    it('should render layer-group icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-layer-group').exists()).toBe(true)
    })
  })

  describe('Type Icons', () => {
    it('should have document icon mapping', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-file-alt').exists()).toBe(true)
    })

    it('should have article icon mapping', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-newspaper').exists()).toBe(true)
    })

    it('should have media icon mapping', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-photo-video').exists()).toBe(true)
    })
  })

  describe('Type Colors', () => {
    it('should apply document colors', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.bg-blue-100').exists()).toBe(true)
    })

    it('should apply article colors', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.bg-purple-100').exists()).toBe(true)
    })

    it('should apply media colors', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.bg-pink-100').exists()).toBe(true)
    })
  })

  describe('Item Display', () => {
    it('should display item titles', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Document 1')
      expect(wrapper.text()).toContain('Article 1')
      expect(wrapper.text()).toContain('Media 1')
    })

    it('should display item types', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('document')
      expect(wrapper.text()).toContain('article')
      expect(wrapper.text()).toContain('media')
    })
  })

  describe('Displayed Items Computed', () => {
    it('should limit displayed items to 5', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.displayedItems.length).toBeLessThanOrEqual(5)
    })

    it('should calculate extra count', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.extraCount).toBe(0) // 3 items, limit is 5
    })
  })

  describe('Actions', () => {
    it('should call togglePanelMinimized when minimize clicked', async () => {
      const wrapper = mountComponent()
      const minBtn = wrapper.find('.fa-minus').element.parentElement
      await minBtn?.click()
      expect(mockTogglePanelMinimized).toHaveBeenCalled()
    })

    it('should call clearSelection when clear button clicked', async () => {
      const wrapper = mountComponent()
      const buttons = wrapper.findAll('button')
      const clearBtn = buttons.find(b => b.text().includes('common.clearAll'))
      await clearBtn?.trigger('click')
      expect(mockClearSelection).toHaveBeenCalled()
    })

    it('should call startComparison when compare button clicked', async () => {
      const wrapper = mountComponent()
      const buttons = wrapper.findAll('button')
      const compareBtn = buttons.find(b => b.text().includes('comparison.compare') && b.find('.fa-chart-bar').exists())
      await compareBtn?.trigger('click')
      expect(mockStartComparison).toHaveBeenCalled()
    })
  })

  describe('Remove Item', () => {
    it('should call removeItem when remove button clicked', async () => {
      const wrapper = mountComponent()
      const removeBtn = wrapper.find('.fa-times.text-xs').element.parentElement
      await removeBtn?.click()
      expect(mockRemoveItem).toHaveBeenCalled()
    })
  })

  describe('Type Breakdown', () => {
    it('should display type breakdown badges', () => {
      const wrapper = mountComponent()
      // Check for type counts in the breakdown
      expect(wrapper.text()).toContain('document')
      expect(wrapper.text()).toContain('article')
      expect(wrapper.text()).toContain('media')
    })
  })

  describe('Compare Button State', () => {
    it('should apply enabled styling when canCompare is true', () => {
      const wrapper = mountComponent()
      const compareBtn = wrapper.findAll('button').find(b =>
        b.text().includes('comparison.compare') && b.find('.fa-chart-bar').exists()
      )
      expect(compareBtn?.classes()).toContain('bg-teal-500')
    })
  })

  describe('Panel Header', () => {
    it('should have gradient header', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.bg-gradient-to-r').exists()).toBe(true)
    })

    it('should show teal gradient colors', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.from-teal-500').exists()).toBe(true)
      expect(wrapper.find('.to-teal-600').exists()).toBe(true)
    })
  })
})
