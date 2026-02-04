import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { ref } from 'vue'
import MediaCenterPage from '@/pages/media/MediaCenterPage.vue'

// Mock vue-router
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
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
vi.mock('@/components/common/ContentActionsDropdown.vue', () => ({
  default: { template: '<div class="content-actions"><slot /></div>' },
}))
vi.mock('@/components/common/FilterDropdown.vue', () => ({
  default: { template: '<div class="filter-dropdown"><slot /></div>' },
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
vi.mock('@/components/common/AddToCollectionModal.vue', () => ({
  default: { template: '<div class="add-to-collection-modal"><slot /></div>' },
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AIConfidenceBar: { template: '<div class="ai-confidence"></div>' },
  AISuggestionChip: { template: '<span class="ai-suggestion"></span>' },
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

function mountComponent() {
  return shallowMount(MediaCenterPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
      },
    },
  })
}

describe('MediaCenterPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render media center page', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Core State', () => {
    it('should have active tab as all', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.activeTab).toBe('all')
    })

    it('should have search query empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.searchQuery).toBe('')
    })

    it('should have sort by recent', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sortBy).toBe('recent')
    })

    it('should have sort order desc', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sortOrder).toBe('desc')
    })

    it('should have view mode as grid', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.viewMode).toBe('grid')
    })
  })

  describe('Sidebar State', () => {
    it('should have sidebar collapsed state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isSidebarCollapsed).toBe(false)
    })

    it('should have expanded folders set', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.expandedFolders).toBeDefined()
    })

    it('should have selected folder null', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedFolder).toBeNull()
    })

    it('should have current view as all', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.currentView).toBe('all')
    })
  })

  describe('Selection Mode', () => {
    it('should have selection mode off', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isSelectionMode).toBe(false)
    })

    it('should have selected media set', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedMedia).toBeDefined()
    })
  })

  describe('Modal States', () => {
    it('should have upload modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showUploadModal).toBe(false)
    })

    it('should have add to collection modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showAddToCollectionModal).toBe(false)
    })

    it('should have share modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showShareModal).toBe(false)
    })

    it('should have filters state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showFilters).toBe(false)
    })

    it('should have sort dropdown state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showSortDropdown).toBe(false)
    })
  })

  describe('AI Features', () => {
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
  })

  describe('Filter State', () => {
    it('should have selected media types empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedMediaTypes).toEqual([])
    })

    it('should have selected categories empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedCategories).toEqual([])
    })

    it('should have selected tags empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedTags).toEqual([])
    })

    it('should have selected status filters empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedStatusFilters).toEqual([])
    })
  })

  describe('Status Filter Options', () => {
    it('should have status filter options', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.statusFilterOptions).toBeDefined()
      expect(vm.statusFilterOptions.length).toBe(2)
    })
  })

  describe('Sort Options', () => {
    it('should have sort options list', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sortOptionsList).toBeDefined()
      expect(vm.sortOptionsList.length).toBe(5)
    })

    it('should have recent sort option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const option = vm.sortOptionsList.find((o: any) => o.value === 'recent')
      expect(option).toBeDefined()
    })

    it('should have name sort option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const option = vm.sortOptionsList.find((o: any) => o.value === 'name')
      expect(option).toBeDefined()
    })

    it('should have popular sort option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const option = vm.sortOptionsList.find((o: any) => o.value === 'popular')
      expect(option).toBeDefined()
    })
  })

  describe('View Navigation', () => {
    it('should have view nav items', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.viewNavItems).toBeDefined()
      expect(vm.viewNavItems.length).toBe(5)
    })

    it('should have all media view', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const view = vm.viewNavItems.find((v: any) => v.id === 'all')
      expect(view).toBeDefined()
    })

    it('should have saved view', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const view = vm.viewNavItems.find((v: any) => v.id === 'saved')
      expect(view).toBeDefined()
    })

    it('should have trash view', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const view = vm.viewNavItems.find((v: any) => v.id === 'trash')
      expect(view).toBeDefined()
    })
  })

  describe('Folder Tree', () => {
    it('should have folder tree', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.folderTree).toBeDefined()
      expect(vm.folderTree.length).toBeGreaterThan(0)
    })

    it('should have root folder', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const root = vm.folderTree[0]
      expect(root.id).toBe('root')
    })
  })

  describe('Drag State', () => {
    it('should have is dragging state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isDragging).toBe(false)
    })
  })

  describe('Liked and Saved', () => {
    it('should have liked media set', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.likedMedia).toBeDefined()
    })

    it('should have saved media set', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.savedMedia).toBeDefined()
    })
  })
})
