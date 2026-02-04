import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { ref } from 'vue'
import ArticlesPage from '@/pages/articles/ArticlesPage.vue'

// Mock vue-router
const mockPush = vi.fn()
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: mockPush,
  }),
}))

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock components
vi.mock('@/components/common/PageHeroHeader.vue', () => ({
  default: { template: '<div class="page-hero-header"><slot /></div>' },
}))
vi.mock('@/components/common/FilterDropdown.vue', () => ({
  default: { template: '<div class="filter-dropdown"><slot /></div>' },
}))
vi.mock('@/components/common/ContentActionsDropdown.vue', () => ({
  default: { template: '<div class="content-actions"><slot /></div>' },
}))
vi.mock('@/components/common/AddToCollectionModal.vue', () => ({
  default: { template: '<div class="add-to-collection-modal"><slot /></div>' },
}))
vi.mock('@/components/common/ViewAllButton.vue', () => ({
  default: { template: '<button class="view-all-btn"><slot /></button>' },
}))
vi.mock('@/components/common/EmptyState.vue', () => ({
  default: { template: '<div class="empty-state"><slot /></div>' },
}))
vi.mock('@/components/common/Pagination.vue', () => ({
  default: { template: '<div class="pagination"><slot /></div>' },
}))
vi.mock('@/components/common/ComparisonButton.vue', () => ({
  default: { template: '<button class="comparison-btn"><slot /></button>' },
}))
vi.mock('@/components/common/CategoryBadge.vue', () => ({
  default: { template: '<span class="category-badge"><slot /></span>' },
}))
vi.mock('@/components/common/StatusBadge.vue', () => ({
  default: { template: '<span class="status-badge"><slot /></span>' },
}))
vi.mock('@/components/common/TagBadge.vue', () => ({
  default: { template: '<span class="tag-badge"><slot /></span>' },
}))
vi.mock('@/components/common/ShareContentModal.vue', () => ({
  default: { template: '<div class="share-modal"><slot /></div>' },
}))
vi.mock('@/components/common/SkeletonLoader.vue', () => ({
  default: { template: '<div class="skeleton-loader"><slot /></div>' },
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AISuggestionChip: { template: '<span class="ai-suggestion"></span>' },
  AISentimentBadge: { template: '<span class="ai-sentiment"></span>' },
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
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

// Mock composables with proper Vue refs
vi.mock('@/composables/usePagination', () => ({
  usePagination: () => ({
    currentPage: ref(1),
    totalPages: ref(1),
    itemsPerPage: ref(10),
    totalItems: ref(0),
    goToPage: vi.fn(),
    nextPage: vi.fn(),
    prevPage: vi.fn(),
    resetPage: vi.fn(),
    setTotalItems: vi.fn(),
    paginatedItems: ref([]),
    itemsPerPageOptions: [5, 10, 20, 50, 100],
  }),
}))

// Helper to mount with shallow rendering
function mountWithShallow() {
  return shallowMount(ArticlesPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
      },
    },
  })
}

describe('ArticlesPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render articles page', () => {
      const wrapper = mountWithShallow()
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Core State', () => {
    it('should have initial search query empty', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.searchQuery).toBe('')
    })

    it('should have view mode as grid by default', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.viewMode).toBe('grid')
    })

    it('should have sort by recent', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.sortBy).toBe('recent')
    })

    it('should have sort order desc', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.sortOrder).toBe('desc')
    })

    it('should have empty selected categories', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.selectedCategories).toEqual([])
    })

    it('should have empty selected types', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.selectedTypes).toEqual([])
    })

    it('should have empty selected tags', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.selectedTags).toEqual([])
    })
  })

  describe('Categories', () => {
    it('should have categories defined', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.categories).toBeDefined()
      expect(Array.isArray(vm.categories)).toBe(true)
    })

    it('should have featured category', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      const featured = vm.categories.find((c: any) => c.id === 'featured')
      expect(featured).toBeDefined()
      expect(featured.isSpecial).toBe(true)
    })

    it('should have getting-started category', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      const category = vm.categories.find((c: any) => c.id === 'getting-started')
      expect(category).toBeDefined()
    })

    it('should have tutorials category', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      const category = vm.categories.find((c: any) => c.id === 'tutorials')
      expect(category).toBeDefined()
    })

    it('should have security category', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      const category = vm.categories.find((c: any) => c.id === 'security')
      expect(category).toBeDefined()
    })
  })

  describe('Article Types', () => {
    it('should have article types defined', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.articleTypes).toBeDefined()
      expect(vm.articleTypes.length).toBeGreaterThan(0)
    })

    it('should have guide type', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      const type = vm.articleTypes.find((t: any) => t.id === 'guide')
      expect(type).toBeDefined()
    })

    it('should have tutorial type', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      const type = vm.articleTypes.find((t: any) => t.id === 'tutorial')
      expect(type).toBeDefined()
    })

    it('should have news type', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      const type = vm.articleTypes.find((t: any) => t.id === 'news')
      expect(type).toBeDefined()
    })
  })

  describe('Articles Data', () => {
    it('should have articles array', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.articles).toBeDefined()
      expect(Array.isArray(vm.articles)).toBe(true)
    })

    it('should have article properties', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      const article = vm.articles[0]
      expect(article.id).toBeDefined()
      expect(article.title).toBeDefined()
      expect(article.excerpt).toBeDefined()
      expect(article.category).toBeDefined()
      expect(article.author).toBeDefined()
    })

    it('should have featured articles', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      const featuredArticles = vm.articles.filter((a: any) => a.featured)
      expect(featuredArticles.length).toBeGreaterThan(0)
    })
  })

  describe('Sort Options', () => {
    it('should have sort options', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.sortOptions).toBeDefined()
      expect(vm.sortOptions.length).toBe(3)
    })

    it('should have recent sort option', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      const option = vm.sortOptions.find((o: any) => o.value === 'recent')
      expect(option).toBeDefined()
    })

    it('should have popular sort option', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      const option = vm.sortOptions.find((o: any) => o.value === 'popular')
      expect(option).toBeDefined()
    })

    it('should compute current sort option', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.currentSortOption).toBeDefined()
      expect(vm.currentSortOption.value).toBe('recent')
    })

    it('should select sort option', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any

      vm.selectSortOption('popular')

      expect(vm.sortBy).toBe('popular')
      expect(vm.showSortDropdown).toBe(false)
    })
  })

  describe('AI Features', () => {
    it('should have AI features enabled by default', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.showAIFeatures).toBe(true)
    })

    it('should have AI recommendations enabled', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.showAIRecommendations).toBe(true)
    })

    it('should have AI recommendations data', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.aiRecommendations).toBeDefined()
      expect(Array.isArray(vm.aiRecommendations)).toBe(true)
    })

    it('should have mock AI recommendations', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.mockAIRecommendations).toBeDefined()
      expect(vm.mockAIRecommendations.length).toBe(3)
    })

    it('should have search suggestions state', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.showSearchSuggestions).toBe(false)
      expect(vm.searchSuggestions).toEqual([])
    })

    it('should have article sentiments map', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.articleSentiments).toBeDefined()
    })

    it('should have mock sentiments for articles', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.mockSentiments).toBeDefined()
      expect(vm.mockSentiments[1]).toBeDefined()
      expect(vm.mockSentiments[1].overall).toBe('positive')
    })
  })

  describe('Unified Search', () => {
    it('should have unified search query', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.unifiedSearchQuery).toBe('')
    })

    it('should have AI search mode state', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.isAISearchMode).toBe(false)
    })

    it('should have natural language search suggestions', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.nlSearchSuggestions).toBeDefined()
      expect(vm.nlSearchSuggestions.length).toBeGreaterThan(0)
    })
  })

  describe('Current User', () => {
    it('should have current user defined', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.currentUser).toBeDefined()
      expect(vm.currentUser.name).toBe('Ahmed Imam')
      expect(vm.currentUser.role).toBe('Product Director')
    })
  })

  describe('Tags', () => {
    it('should compute all tags from articles', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.allTags).toBeDefined()
      expect(Array.isArray(vm.allTags)).toBe(true)
    })
  })

  describe('Filter Options', () => {
    it('should compute type filter options', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.typeFilterOptions).toBeDefined()
      expect(vm.typeFilterOptions.length).toBeGreaterThan(0)
    })

    it('should compute category filter options', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.categoryFilterOptions).toBeDefined()
      expect(vm.categoryFilterOptions.length).toBeGreaterThan(0)
    })

    it('should compute tag filter options', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.tagFilterOptions).toBeDefined()
    })

    it('should compute status filter options', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.statusFilterOptionsComputed).toBeDefined()
    })
  })

  describe('Modal States', () => {
    it('should have add to collection modal state', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.showAddToCollectionModal).toBe(false)
    })

    it('should have filters visible state', () => {
      const wrapper = mountWithShallow()
      const vm = wrapper.vm as any
      expect(vm.showFilters).toBe(false)
    })
  })
})
