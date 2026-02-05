import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import SearchResultsPage from '@/pages/search/SearchResultsPage.vue'

// Mock vue-router
const mockReplace = vi.fn()
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
    replace: mockReplace,
  }),
  useRoute: () => ({
    query: { q: '' },
  }),
}))

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock stores
vi.mock('@/stores/ui', () => ({
  useUIStore: () => ({
    sidebarCollapsed: false,
  }),
}))

vi.mock('@/stores/aiServices', () => ({
  useAIServicesStore: () => ({
    isLoading: false,
    error: null,
  }),
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AIVoiceInput: { template: '<div class="ai-voice"></div>' },
}))

// Mock common components
vi.mock('@/components/common/EmptyState.vue', () => ({
  default: { template: '<div class="empty-state"></div>' },
}))

vi.mock('@/components/common/Pagination.vue', () => ({
  default: { template: '<div class="pagination"></div>' },
}))

// Mock clipboard API
const mockWriteText = vi.fn()
Object.defineProperty(navigator, 'clipboard', {
  value: {
    writeText: mockWriteText,
  },
  writable: true,
  configurable: true,
})

// Mock window.scrollTo
window.scrollTo = vi.fn()

function mountComponent() {
  return shallowMount(SearchResultsPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
        'router-link': true,
      },
    },
  })
}

describe('SearchResultsPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render search results page', () => {
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

    it('should have header text', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.pageTitle).toBe('Search')
      expect(vm.textConstants.searchPlaceholder).toBeDefined()
      expect(vm.textConstants.searchButton).toBeDefined()
    })

    it('should have quick filter text', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.quickFilterAll).toBeDefined()
      expect(vm.textConstants.quickFilterArticles).toBeDefined()
      expect(vm.textConstants.quickFilterDocuments).toBeDefined()
    })

    it('should have filter sidebar text', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.filters).toBeDefined()
      expect(vm.textConstants.clearAll).toBeDefined()
      expect(vm.textConstants.contentType).toBeDefined()
      expect(vm.textConstants.dateRange).toBeDefined()
    })

    it('should have AI feature text', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.didYouMean).toBeDefined()
      expect(vm.textConstants.aiDetectedEntities).toBeDefined()
      expect(vm.textConstants.aiSummary).toBeDefined()
      expect(vm.textConstants.aiSearchInsights).toBeDefined()
    })
  })

  describe('Search State', () => {
    it('should have is loading state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isLoading).toBe(false)
    })

    it('should have search query field', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.searchQuery).toBe('')
    })

    it('should have display query field', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.displayQuery).toBe('')
    })

    it('should have view mode state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.viewMode).toBe('list')
    })

    it('should have sort by state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sortBy).toBe('relevance')
    })

    it('should have selected date state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedDate).toBe('all')
    })

    it('should have author filter state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.authorFilter).toBe('')
    })

    it('should have current page state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.currentPage).toBe(1)
    })

    it('should have page size state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.pageSize).toBe(10)
    })
  })

  describe('Layout State', () => {
    it('should have is filter sidebar collapsed state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isFilterSidebarCollapsed).toBe(false)
    })

    it('should have show mobile filters state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showMobileFilters).toBe(false)
    })
  })

  describe('Filter Data', () => {
    it('should have quick filters array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.quickFilters).toBeDefined()
      expect(Array.isArray(vm.quickFilters)).toBe(true)
      expect(vm.quickFilters.length).toBe(5)
    })

    it('should have all quick filter active by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const allFilter = vm.quickFilters.find((f: any) => f.id === 'all')
      expect(allFilter.active).toBe(true)
    })

    it('should have content types array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.contentTypes).toBeDefined()
      expect(Array.isArray(vm.contentTypes)).toBe(true)
      expect(vm.contentTypes.length).toBe(6)
    })

    it('should have date filters computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.dateFilters).toBeDefined()
      expect(Array.isArray(vm.dateFilters)).toBe(true)
      expect(vm.dateFilters.length).toBe(5)
    })

    it('should have top authors array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.topAuthors).toBeDefined()
      expect(Array.isArray(vm.topAuthors)).toBe(true)
    })

    it('should have popular tags array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.popularTags).toBeDefined()
      expect(Array.isArray(vm.popularTags)).toBe(true)
    })
  })

  describe('Results Data', () => {
    it('should have results array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.results).toBeDefined()
      expect(Array.isArray(vm.results)).toBe(true)
    })

    it('should have total results count', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.totalResults).toBe(0)
    })

    it('should have search time', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.searchTime).toBe(0)
    })
  })

  describe('Computed Pagination', () => {
    it('should have total pages computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.totalPages).toBe(0)
    })

    it('should have start result computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.startResult).toBe(1)
    })

    it('should have end result computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.endResult).toBe(0)
    })

    it('should have pagination pages computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.paginationPages).toBeDefined()
      expect(Array.isArray(vm.paginationPages)).toBe(true)
    })

    it('should have active filters count computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // 3 content types are unchecked by default (courses, events, polls)
      expect(vm.activeFiltersCount).toBe(3)
    })
  })

  describe('AI Features State', () => {
    it('should have show AI panel state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showAIPanel).toBe(true)
    })

    it('should have is analyzing query state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isAnalyzingQuery).toBe(false)
    })

    it('should have show did you mean state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showDidYouMean).toBe(true)
    })

    it('should have query intent ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.queryIntent).toBeNull()
    })

    it('should have extracted entities array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.extractedEntities).toBeDefined()
      expect(Array.isArray(vm.extractedEntities)).toBe(true)
    })

    it('should have did you mean suggestions array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.didYouMeanSuggestions).toBeDefined()
      expect(Array.isArray(vm.didYouMeanSuggestions)).toBe(true)
    })

    it('should have AI search insights array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiSearchInsights).toBeDefined()
      expect(Array.isArray(vm.aiSearchInsights)).toBe(true)
    })

    it('should have related searches array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.relatedSearches).toBeDefined()
      expect(Array.isArray(vm.relatedSearches)).toBe(true)
    })

    it('should have AI summary ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiSummary).toBe('')
    })
  })

  describe('Functions', () => {
    it('should have toggle quick filter function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.toggleQuickFilter).toBe('function')
    })

    it('should have clear filters function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.clearFilters).toBe('function')
    })

    it('should have perform search function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.performSearch).toBe('function')
    })

    it('should have generate mock results function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.generateMockResults).toBe('function')
    })

    it('should have analyze search query function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.analyzeSearchQuery).toBe('function')
    })

    it('should have highlight text function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.highlightText).toBe('function')
    })

    it('should have toggle tag function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.toggleTag).toBe('function')
    })

    it('should have apply did you mean function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.applyDidYouMean).toBe('function')
    })

    it('should have apply entity filter function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.applyEntityFilter).toBe('function')
    })

    it('should have apply search insight function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.applySearchInsight).toBe('function')
    })

    it('should have go to page function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.goToPage).toBe('function')
    })

    it('should have handle voice transcript function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleVoiceTranscript).toBe('function')
    })

    it('should have copy AI summary function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.copyAISummary).toBe('function')
    })
  })

  describe('Toggle Quick Filter', () => {
    it('should activate all filter and deactivate others', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const allFilter = vm.quickFilters.find((f: any) => f.id === 'all')
      vm.toggleQuickFilter(allFilter)
      expect(allFilter.active).toBe(true)
      const othersActive = vm.quickFilters.filter((f: any) => f.id !== 'all' && f.active)
      expect(othersActive.length).toBe(0)
    })

    it('should toggle non-all filter', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const articlesFilter = vm.quickFilters.find((f: any) => f.id === 'articles')
      vm.toggleQuickFilter(articlesFilter)
      expect(articlesFilter.active).toBe(true)
      const allFilter = vm.quickFilters.find((f: any) => f.id === 'all')
      expect(allFilter.active).toBe(false)
    })
  })

  describe('Clear Filters', () => {
    it('should reset all filters', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.selectedDate = 'week'
      vm.clearFilters()
      expect(vm.selectedDate).toBe('all')
    })

    it('should activate all quick filter', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.clearFilters()
      const allFilter = vm.quickFilters.find((f: any) => f.id === 'all')
      expect(allFilter.active).toBe(true)
    })
  })

  describe('Highlight Text', () => {
    it('should return text unchanged when no query', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.displayQuery = ''
      const result = vm.highlightText('test text')
      expect(result).toBe('test text')
    })

    it('should highlight matching words', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.displayQuery = 'test query'
      const result = vm.highlightText('this is a test document')
      expect(result).toContain('<mark')
      expect(result).toContain('test')
    })
  })

  describe('Handle Voice Transcript', () => {
    it('should update search query with transcript', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleVoiceTranscript('voice search', false)
      expect(vm.searchQuery).toBe('voice search')
    })
  })

  describe('Copy AI Summary', () => {
    it('should copy AI summary to clipboard', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.aiSummary = 'Test summary'
      vm.copyAISummary()
      expect(mockWriteText).toHaveBeenCalledWith('Test summary')
    })
  })

  describe('Helper Functions', () => {
    it('should have get entity type color function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getEntityTypeColor).toBe('function')
    })

    it('should return correct entity colors', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getEntityTypeColor('person')).toContain('blue')
      expect(vm.getEntityTypeColor('organization')).toContain('purple')
      expect(vm.getEntityTypeColor('topic')).toContain('teal')
      expect(vm.getEntityTypeColor('date')).toContain('amber')
      expect(vm.getEntityTypeColor('location')).toContain('green')
      expect(vm.getEntityTypeColor('unknown')).toContain('gray')
    })

    it('should have get entity type icon function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getEntityTypeIcon).toBe('function')
    })

    it('should return correct entity icons', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getEntityTypeIcon('person')).toContain('fa-user')
      expect(vm.getEntityTypeIcon('organization')).toContain('fa-building')
      expect(vm.getEntityTypeIcon('topic')).toContain('fa-tag')
      expect(vm.getEntityTypeIcon('date')).toContain('fa-calendar')
      expect(vm.getEntityTypeIcon('location')).toContain('fa-map-marker')
      expect(vm.getEntityTypeIcon('unknown')).toContain('fa-circle')
    })

    it('should have get intent type color function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getIntentTypeColor).toBe('function')
    })

    it('should return correct intent colors', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getIntentTypeColor('informational')).toContain('blue')
      expect(vm.getIntentTypeColor('navigational')).toContain('purple')
      expect(vm.getIntentTypeColor('transactional')).toContain('green')
      expect(vm.getIntentTypeColor('unknown')).toContain('gray')
    })

    it('should have get intent label function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getIntentLabel).toBe('function')
    })

    it('should return correct intent labels', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getIntentLabel('informational')).toBe(vm.textConstants.lookingForInfo)
      expect(vm.getIntentLabel('navigational')).toBe(vm.textConstants.findingSpecific)
      expect(vm.getIntentLabel('transactional')).toBe(vm.textConstants.takingAction)
    })
  })

  describe('Go To Page', () => {
    it('should update current page for valid page number', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.totalResults = 50
      vm.pageSize = 10
      await wrapper.vm.$nextTick()
      vm.goToPage(2)
      expect(vm.currentPage).toBe(2)
    })

    it('should not update for invalid page number', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.currentPage = 1
      vm.goToPage(-1)
      expect(vm.currentPage).toBe(1)
    })

    it('should not update for ellipsis string', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.currentPage = 1
      vm.goToPage('...')
      expect(vm.currentPage).toBe(1)
    })

    it('should scroll to top on page change', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.totalResults = 50
      vm.pageSize = 10
      await wrapper.vm.$nextTick()
      vm.goToPage(2)
      expect(window.scrollTo).toHaveBeenCalled()
    })
  })

  describe('Toggle Tag', () => {
    it('should toggle tag active state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const tag = { id: 1, name: 'test', active: false }
      vm.toggleTag(tag)
      expect(tag.active).toBe(true)
    })
  })

  describe('Apply Did You Mean', () => {
    it('should update search query with suggestion', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.applyDidYouMean('suggested query')
      expect(vm.searchQuery).toBe('suggested query')
    })

    it('should hide did you mean after applying', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showDidYouMean = true
      vm.applyDidYouMean('suggested query')
      expect(vm.showDidYouMean).toBe(false)
    })
  })

  describe('Apply Entity Filter', () => {
    it('should append entity text to search query', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.searchQuery = 'initial query'
      vm.applyEntityFilter({ text: 'Entity', type: 'topic', confidence: 0.9 })
      expect(vm.searchQuery).toBe('initial query Entity')
    })
  })
})
