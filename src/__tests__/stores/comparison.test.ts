import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useComparisonStore } from '@/stores/comparison'
import type { ComparisonItem } from '@/types'

// Mock crypto.randomUUID
vi.stubGlobal('crypto', {
  randomUUID: () => 'test-uuid-' + Math.random().toString(36).substr(2, 9),
})

const createMockItem = (id: string, type: string = 'article'): ComparisonItem => ({
  id,
  type: type as any,
  title: `Test Item ${id}`,
  description: `Description for ${id}`,
  thumbnail: '/test.jpg',
  createdAt: new Date().toISOString(),
})

describe('Comparison Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
    localStorage.clear()
  })

  describe('Initial State', () => {
    it('should have empty selected items', () => {
      const store = useComparisonStore()
      expect(store.selectedItems).toEqual([])
    })

    it('should not be comparing initially', () => {
      const store = useComparisonStore()
      expect(store.isComparing).toBe(false)
    })

    it('should not have panel minimized initially', () => {
      const store = useComparisonStore()
      expect(store.isPanelMinimized).toBe(false)
    })

    it('should have zero item count', () => {
      const store = useComparisonStore()
      expect(store.itemCount).toBe(0)
    })

    it('should not be able to compare with no items', () => {
      const store = useComparisonStore()
      expect(store.canCompare).toBe(false)
    })
  })

  describe('addItem', () => {
    it('should add item to selection', () => {
      const store = useComparisonStore()
      const item = createMockItem('1')

      store.addItem(item)

      expect(store.selectedItems).toHaveLength(1)
      expect(store.selectedItems[0].id).toBe('1')
    })

    it('should prevent duplicate items', () => {
      const store = useComparisonStore()
      const item = createMockItem('1')

      store.addItem(item)
      store.addItem(item)

      expect(store.selectedItems).toHaveLength(1)
    })

    it('should clear analysis when adding items', () => {
      const store = useComparisonStore()
      store.analysisResults = { id: 'test' } as any

      store.addItem(createMockItem('1'))

      expect(store.analysisResults).toBeNull()
    })
  })

  describe('removeItem', () => {
    it('should remove item from selection', () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1'))
      store.addItem(createMockItem('2'))

      store.removeItem('1')

      expect(store.selectedItems).toHaveLength(1)
      expect(store.selectedItems[0].id).toBe('2')
    })

    it('should do nothing if item not found', () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1'))

      store.removeItem('999')

      expect(store.selectedItems).toHaveLength(1)
    })

    it('should clear analysis when removing items', () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1'))
      store.analysisResults = { id: 'test' } as any

      store.removeItem('1')

      expect(store.analysisResults).toBeNull()
    })
  })

  describe('toggleItem', () => {
    it('should add item if not selected', () => {
      const store = useComparisonStore()
      const item = createMockItem('1')

      store.toggleItem(item)

      expect(store.selectedItems).toHaveLength(1)
    })

    it('should remove item if already selected', () => {
      const store = useComparisonStore()
      const item = createMockItem('1')

      store.addItem(item)
      store.toggleItem(item)

      expect(store.selectedItems).toHaveLength(0)
    })
  })

  describe('clearSelection', () => {
    it('should clear all items', () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1'))
      store.addItem(createMockItem('2'))

      store.clearSelection()

      expect(store.selectedItems).toHaveLength(0)
    })

    it('should reset comparing state', () => {
      const store = useComparisonStore()
      store.isComparing = true

      store.clearSelection()

      expect(store.isComparing).toBe(false)
    })

    it('should clear analysis results', () => {
      const store = useComparisonStore()
      store.analysisResults = { id: 'test' } as any

      store.clearSelection()

      expect(store.analysisResults).toBeNull()
    })
  })

  describe('startComparison', () => {
    it('should start comparison when 2+ items selected', () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1'))
      store.addItem(createMockItem('2'))

      store.startComparison()

      expect(store.isComparing).toBe(true)
    })

    it('should not start comparison with less than 2 items', () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1'))

      store.startComparison()

      expect(store.isComparing).toBe(false)
    })

    it('should set active tab to side-by-side', () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1'))
      store.addItem(createMockItem('2'))
      store.activeTab = 'insights'

      store.startComparison()

      expect(store.activeTab).toBe('side-by-side')
    })
  })

  describe('closeComparison', () => {
    it('should close comparison modal', () => {
      const store = useComparisonStore()
      store.isComparing = true

      store.closeComparison()

      expect(store.isComparing).toBe(false)
    })
  })

  describe('togglePanelMinimized', () => {
    it('should toggle panel minimized state', () => {
      const store = useComparisonStore()

      expect(store.isPanelMinimized).toBe(false)
      store.togglePanelMinimized()
      expect(store.isPanelMinimized).toBe(true)
      store.togglePanelMinimized()
      expect(store.isPanelMinimized).toBe(false)
    })
  })

  describe('setActiveTab', () => {
    it('should set active tab', () => {
      const store = useComparisonStore()

      store.setActiveTab('insights')
      expect(store.activeTab).toBe('insights')

      store.setActiveTab('entities')
      expect(store.activeTab).toBe('entities')
    })
  })

  describe('Computed Properties', () => {
    it('should correctly compute itemCount', () => {
      const store = useComparisonStore()
      expect(store.itemCount).toBe(0)

      store.addItem(createMockItem('1'))
      expect(store.itemCount).toBe(1)

      store.addItem(createMockItem('2'))
      expect(store.itemCount).toBe(2)
    })

    it('should correctly compute hasItems', () => {
      const store = useComparisonStore()
      expect(store.hasItems).toBe(false)

      store.addItem(createMockItem('1'))
      expect(store.hasItems).toBe(true)
    })

    it('should correctly compute canCompare', () => {
      const store = useComparisonStore()
      expect(store.canCompare).toBe(false)

      store.addItem(createMockItem('1'))
      expect(store.canCompare).toBe(false)

      store.addItem(createMockItem('2'))
      expect(store.canCompare).toBe(true)
    })

    it('should correctly compute itemsByType', () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1', 'article'))
      store.addItem(createMockItem('2', 'article'))
      store.addItem(createMockItem('3', 'document'))

      const byType = store.itemsByType
      expect(byType.article).toHaveLength(2)
      expect(byType.document).toHaveLength(1)
    })

    it('should correctly compute typeBreakdown', () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1', 'article'))
      store.addItem(createMockItem('2', 'article'))
      store.addItem(createMockItem('3', 'document'))

      const breakdown = store.typeBreakdown
      expect(breakdown.article).toBe(2)
      expect(breakdown.document).toBe(1)
    })

    it('should correctly check if item is selected', () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1'))

      expect(store.isItemSelected('1')).toBe(true)
      expect(store.isItemSelected('2')).toBe(false)
    })
  })

  describe('generateAnalysis', () => {
    it('should return null when cannot compare', async () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1'))

      const result = await store.generateAnalysis()

      expect(result).toBeNull()
    })

    it('should set isAnalyzing during analysis', async () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1'))
      store.addItem(createMockItem('2'))

      const promise = store.generateAnalysis()
      expect(store.isAnalyzing).toBe(true)

      await promise
      expect(store.isAnalyzing).toBe(false)
    })

    it('should generate analysis for valid selection', async () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1'))
      store.addItem(createMockItem('2'))

      const result = await store.generateAnalysis()

      expect(result).not.toBeNull()
      expect(result?.similarity).toBeGreaterThanOrEqual(60)
      expect(store.analysisResults).toEqual(result)
    })
  })

  describe('Persistence', () => {
    it('should save items to storage', () => {
      const store = useComparisonStore()
      store.addItem(createMockItem('1'))

      store.saveToStorage()

      const saved = localStorage.getItem('comparison_items')
      expect(saved).not.toBeNull()
      expect(JSON.parse(saved!)).toHaveLength(1)
    })

    it('should load items from storage', () => {
      const items = [createMockItem('1'), createMockItem('2')]
      localStorage.setItem('comparison_items', JSON.stringify(items))

      const store = useComparisonStore()
      store.loadFromStorage()

      expect(store.selectedItems).toHaveLength(2)
    })

    it('should initialize persistence on initPersistence', () => {
      const items = [createMockItem('1')]
      localStorage.setItem('comparison_items', JSON.stringify(items))

      const store = useComparisonStore()
      store.initPersistence()

      expect(store.selectedItems).toHaveLength(1)
    })
  })
})
