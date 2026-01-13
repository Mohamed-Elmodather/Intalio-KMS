import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { ComparisonItem, ComparisonAnalysis } from '@/types'

export const useComparisonStore = defineStore('comparison', () => {
  // ============================================================================
  // State
  // ============================================================================

  // Selected items for comparison
  const selectedItems = ref<ComparisonItem[]>([])

  // UI state
  const isComparing = ref(false)
  const isPanelMinimized = ref(false)
  const isAnalyzing = ref(false)

  // Analysis results
  const analysisResults = ref<ComparisonAnalysis | null>(null)
  const analysisError = ref<string | null>(null)

  // Active tab in comparison modal
  const activeTab = ref<'side-by-side' | 'insights' | 'entities' | 'sentiment' | 'topics'>('side-by-side')

  // ============================================================================
  // Computed
  // ============================================================================

  const itemCount = computed(() => selectedItems.value.length)

  const hasItems = computed(() => selectedItems.value.length > 0)

  const canCompare = computed(() => selectedItems.value.length >= 2)

  const itemsByType = computed(() => {
    const grouped: Record<string, ComparisonItem[]> = {}
    for (const item of selectedItems.value) {
      if (!grouped[item.type]) {
        grouped[item.type] = []
      }
      grouped[item.type].push(item)
    }
    return grouped
  })

  const typeBreakdown = computed(() => {
    const counts: Record<string, number> = {}
    for (const item of selectedItems.value) {
      counts[item.type] = (counts[item.type] || 0) + 1
    }
    return counts
  })

  const isItemSelected = computed(() => {
    const idSet = new Set(selectedItems.value.map(item => item.id))
    return (id: string) => idSet.has(id)
  })

  // ============================================================================
  // Actions
  // ============================================================================

  /**
   * Add an item to the comparison selection
   */
  function addItem(item: ComparisonItem) {
    // Prevent duplicates
    if (selectedItems.value.some(i => i.id === item.id)) {
      return
    }
    selectedItems.value.push(item)
    // Clear previous analysis when selection changes
    analysisResults.value = null
    analysisError.value = null
  }

  /**
   * Remove an item from comparison
   */
  function removeItem(id: string) {
    const index = selectedItems.value.findIndex(item => item.id === id)
    if (index !== -1) {
      selectedItems.value.splice(index, 1)
    }
    // Clear analysis when selection changes
    analysisResults.value = null
    analysisError.value = null
  }

  /**
   * Toggle item selection
   */
  function toggleItem(item: ComparisonItem) {
    if (isItemSelected.value(item.id)) {
      removeItem(item.id)
    } else {
      addItem(item)
    }
  }

  /**
   * Clear all selected items
   */
  function clearSelection() {
    selectedItems.value = []
    analysisResults.value = null
    analysisError.value = null
    isComparing.value = false
  }

  /**
   * Start comparison (open modal)
   */
  function startComparison() {
    if (canCompare.value) {
      isComparing.value = true
      activeTab.value = 'side-by-side'
    }
  }

  /**
   * Close comparison modal
   */
  function closeComparison() {
    isComparing.value = false
  }

  /**
   * Toggle panel minimized state
   */
  function togglePanelMinimized() {
    isPanelMinimized.value = !isPanelMinimized.value
  }

  /**
   * Set active tab in comparison view
   */
  function setActiveTab(tab: typeof activeTab.value) {
    activeTab.value = tab
  }

  /**
   * Generate AI analysis for selected items
   */
  async function generateAnalysis(): Promise<ComparisonAnalysis | null> {
    if (!canCompare.value) return null

    isAnalyzing.value = true
    analysisError.value = null

    try {
      // Simulate API call with mock data
      await new Promise(resolve => setTimeout(resolve, 1500))

      const mockAnalysis: ComparisonAnalysis = {
        id: crypto.randomUUID(),
        similarity: Math.round(60 + Math.random() * 30), // 60-90%
        summary: generateMockSummary(),
        commonEntities: generateMockEntities(),
        sentimentComparison: generateMockSentiment(),
        commonTopics: generateMockTopics(),
        differences: generateMockDifferences(),
        keyPoints: generateMockKeyPoints(),
        classifications: generateMockClassifications(),
        createdAt: new Date().toISOString(),
      }

      analysisResults.value = mockAnalysis
      return mockAnalysis
    } catch (error) {
      analysisError.value = error instanceof Error ? error.message : 'Analysis failed'
      return null
    } finally {
      isAnalyzing.value = false
    }
  }

  // ============================================================================
  // Mock Data Generators
  // ============================================================================

  function generateMockSummary(): string {
    const items = selectedItems.value
    const typeNames = [...new Set(items.map(i => i.type))].join(', ')
    return `These ${items.length} items (${typeNames}) share common themes related to AFC Asian Cup 2027 preparations. ` +
      `They collectively cover tournament logistics, team preparations, venue information, and event scheduling. ` +
      `The content provides a comprehensive view of the upcoming tournament from multiple perspectives.`
  }

  function generateMockEntities() {
    const items = selectedItems.value
    const entities = [
      { name: 'AFC Asian Cup 2027', type: 'topic' as const, occurrences: items.length, items: items.map(i => i.id) },
      { name: 'Saudi Arabia', type: 'location' as const, occurrences: Math.ceil(items.length * 0.8), items: items.slice(0, Math.ceil(items.length * 0.8)).map(i => i.id) },
      { name: 'Riyadh', type: 'location' as const, occurrences: Math.ceil(items.length * 0.6), items: items.slice(0, Math.ceil(items.length * 0.6)).map(i => i.id) },
      { name: 'AFC', type: 'organization' as const, occurrences: Math.ceil(items.length * 0.7), items: items.slice(0, Math.ceil(items.length * 0.7)).map(i => i.id) },
      { name: '2027', type: 'date' as const, occurrences: items.length, items: items.map(i => i.id) },
    ]
    return entities
  }

  function generateMockSentiment() {
    return selectedItems.value.map(item => ({
      itemId: item.id,
      itemTitle: item.title,
      score: Math.random() * 0.6 + 0.2, // 0.2 to 0.8 (mostly positive)
      label: 'positive' as const,
      confidence: Math.random() * 0.3 + 0.7, // 0.7 to 1.0
    }))
  }

  function generateMockTopics() {
    const items = selectedItems.value
    return [
      { topic: 'Tournament Preparation', relevance: 0.95, items: items.map(i => i.id) },
      { topic: 'Venue Infrastructure', relevance: 0.82, items: items.slice(0, Math.ceil(items.length * 0.7)).map(i => i.id) },
      { topic: 'Team Logistics', relevance: 0.75, items: items.slice(0, Math.ceil(items.length * 0.6)).map(i => i.id) },
      { topic: 'Event Scheduling', relevance: 0.68, items: items.slice(0, Math.ceil(items.length * 0.5)).map(i => i.id) },
    ]
  }

  function generateMockDifferences(): string[] {
    const items = selectedItems.value
    const types = [...new Set(items.map(i => i.type))]

    const differences: string[] = []
    if (types.includes('document')) {
      differences.push('Documents focus on official guidelines and technical specifications')
    }
    if (types.includes('article')) {
      differences.push('Articles provide editorial perspective and analysis')
    }
    if (types.includes('media')) {
      differences.push('Media content emphasizes visual presentation and engagement')
    }
    if (types.includes('event')) {
      differences.push('Events contain scheduling and attendance information')
    }
    differences.push('Content varies in depth and target audience')

    return differences
  }

  function generateMockKeyPoints() {
    return selectedItems.value.map(item => ({
      itemId: item.id,
      points: [
        `Key information about ${item.title.split(' ').slice(0, 3).join(' ')}`,
        'Important dates and deadlines mentioned',
        'Stakeholder responsibilities outlined',
      ],
    }))
  }

  function generateMockClassifications() {
    return selectedItems.value.map(item => ({
      itemId: item.id,
      category: item.type === 'document' ? 'Official Document' :
                item.type === 'article' ? 'News/Editorial' :
                item.type === 'media' ? 'Multimedia Content' : 'Event Information',
      confidence: Math.random() * 0.2 + 0.8, // 0.8 to 1.0
    }))
  }

  // ============================================================================
  // Persistence
  // ============================================================================

  function saveToStorage() {
    try {
      localStorage.setItem('comparison_items', JSON.stringify(selectedItems.value))
    } catch (error) {
      console.error('Failed to save comparison items:', error)
    }
  }

  function loadFromStorage() {
    try {
      const stored = localStorage.getItem('comparison_items')
      if (stored) {
        selectedItems.value = JSON.parse(stored)
      }
    } catch (error) {
      console.error('Failed to load comparison items:', error)
    }
  }

  // Auto-save on changes
  function initPersistence() {
    loadFromStorage()
  }

  // ============================================================================
  // Return
  // ============================================================================

  return {
    // State
    selectedItems,
    isComparing,
    isPanelMinimized,
    isAnalyzing,
    analysisResults,
    analysisError,
    activeTab,

    // Computed
    itemCount,
    hasItems,
    canCompare,
    itemsByType,
    typeBreakdown,
    isItemSelected,

    // Actions
    addItem,
    removeItem,
    toggleItem,
    clearSelection,
    startComparison,
    closeComparison,
    togglePanelMinimized,
    setActiveTab,
    generateAnalysis,

    // Persistence
    saveToStorage,
    loadFromStorage,
    initPersistence,
  }
})
