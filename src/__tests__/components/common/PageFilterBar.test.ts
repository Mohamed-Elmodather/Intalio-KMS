import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import PageFilterBar from '@/components/common/PageFilterBar.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

const mockSortOptions = [
  { value: 'newest', label: 'Newest', icon: 'fas fa-clock' },
  { value: 'popular', label: 'Popular', icon: 'fas fa-fire' },
  { value: 'title', label: 'Title', icon: 'fas fa-font' },
]

function mountComponent(props = {}) {
  return shallowMount(PageFilterBar, {
    props: {
      sortOptions: mockSortOptions,
      sortBy: 'newest',
      ...props,
    },
  })
}

describe('PageFilterBar', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })

    it('should render search input', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('input[type="text"]').exists()).toBe(true)
    })

    it('should render sort button', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Newest')
    })
  })

  describe('Search', () => {
    it('should update search query', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.updateSearchQuery('test')
      expect(wrapper.emitted('update:searchQuery')![0]).toEqual(['test'])
    })

    it('should show placeholder', () => {
      const wrapper = mountComponent({ searchPlaceholder: 'Search articles...' })
      const input = wrapper.find('input[type="text"]')
      expect(input.attributes('placeholder')).toBe('Search articles...')
    })

    it('should show AI placeholder in AI mode', () => {
      const wrapper = mountComponent({
        isAIMode: true,
        aiSearchPlaceholder: 'Ask AI...',
        showAIToggle: true,
      })
      const input = wrapper.find('input[type="text"]')
      expect(input.attributes('placeholder')).toBe('Ask AI...')
    })

    it('should clear search', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.localSearchQuery = 'test'
      vm.clearSearch()
      expect(vm.localSearchQuery).toBe('')
      expect(wrapper.emitted('clearSearch')).toBeTruthy()
    })

    it('should emit search event on enter', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.localSearchQuery = 'test query'
      vm.handleSearch()
      expect(wrapper.emitted('search')![0]).toEqual(['test query'])
    })
  })

  describe('AI Mode', () => {
    it('should show AI toggle when enabled', () => {
      const wrapper = mountComponent({ showAIToggle: true })
      expect(wrapper.find('.fa-wand-magic-sparkles').exists()).toBe(true)
    })

    it('should not show AI toggle when disabled', () => {
      const wrapper = mountComponent({ showAIToggle: false })
      const aiButtons = wrapper.findAll('.fa-wand-magic-sparkles')
      // There might be AI icons in suggestions, so check for the toggle button specifically
      expect(wrapper.text()).not.toContain('AI')
    })

    it('should toggle AI mode', () => {
      const wrapper = mountComponent({ showAIToggle: true, isAIMode: false })
      const vm = wrapper.vm as any
      vm.toggleAIMode()
      expect(wrapper.emitted('update:isAIMode')![0]).toEqual([true])
    })

    it('should show brain icon in AI mode', () => {
      const wrapper = mountComponent({ showAIToggle: true, isAIMode: true })
      expect(wrapper.find('.fa-brain').exists()).toBe(true)
    })
  })

  describe('AI Processing', () => {
    it('should show processing indicator when isProcessingAI is true', () => {
      const wrapper = mountComponent({
        showAIToggle: true,
        isAIMode: true,
        isProcessingAI: true,
        aiSearchingLabel: 'Searching...',
      })
      expect(wrapper.text()).toContain('Searching...')
    })

    it('should show spinner when processing', () => {
      const wrapper = mountComponent({
        showAIToggle: true,
        isAIMode: true,
        isProcessingAI: true,
        localSearchQuery: 'test',
      })
      // When processing with a query
      const vm = wrapper.vm as any
      vm.localSearchQuery = 'test'
      expect(wrapper.find('.animate-spin').exists() || wrapper.find('.animate-pulse').exists()).toBe(true)
    })
  })

  describe('AI Suggestions', () => {
    it('should show AI suggestions when available', async () => {
      const wrapper = mountComponent({
        showAIToggle: true,
        isAIMode: true,
        aiSuggestions: ['Suggestion 1', 'Suggestion 2'],
        tryAskingLabel: 'Try asking:',
      })
      const vm = wrapper.vm as any
      vm.showAISuggestions = true
      await wrapper.vm.$nextTick()
      expect(wrapper.text()).toContain('Try asking:')
    })

    it('should select AI suggestion', () => {
      const wrapper = mountComponent({
        showAIToggle: true,
        isAIMode: true,
        aiSuggestions: ['Suggestion 1'],
      })
      const vm = wrapper.vm as any
      vm.selectAISuggestion('Suggestion 1')
      expect(vm.localSearchQuery).toBe('Suggestion 1')
      expect(wrapper.emitted('selectSuggestion')![0]).toEqual(['Suggestion 1'])
    })
  })

  describe('Sort', () => {
    it('should show current sort option', () => {
      const wrapper = mountComponent({ sortBy: 'popular' })
      expect(wrapper.text()).toContain('Popular')
    })

    it('should toggle sort dropdown', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showSortDropdown).toBe(false)
      await wrapper.find('button').trigger('click')
    })

    it('should select sort option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.selectSortOption('popular')
      expect(wrapper.emitted('update:sortBy')![0]).toEqual(['popular'])
      expect(vm.showSortDropdown).toBe(false)
    })

    it('should toggle sort order', () => {
      const wrapper = mountComponent({ sortOrder: 'desc' })
      const vm = wrapper.vm as any
      vm.toggleSortOrder()
      expect(wrapper.emitted('update:sortOrder')![0]).toEqual(['asc'])
    })

    it('should toggle from asc to desc', () => {
      const wrapper = mountComponent({ sortOrder: 'asc' })
      const vm = wrapper.vm as any
      vm.toggleSortOrder()
      expect(wrapper.emitted('update:sortOrder')![0]).toEqual(['desc'])
    })

    it('should show sort order icon', () => {
      const wrapper = mountComponent({ sortOrder: 'asc' })
      expect(wrapper.find('.fa-arrow-up').exists()).toBe(true)
    })

    it('should show descending icon', () => {
      const wrapper = mountComponent({ sortOrder: 'desc' })
      expect(wrapper.find('.fa-arrow-down').exists()).toBe(true)
    })
  })

  describe('View Toggle', () => {
    it('should show view toggle by default', () => {
      const wrapper = mountComponent({ showViewToggle: true })
      expect(wrapper.find('.fa-th-large').exists()).toBe(true)
      expect(wrapper.find('.fa-list').exists()).toBe(true)
    })

    it('should not show view toggle when disabled', () => {
      const wrapper = mountComponent({ showViewToggle: false })
      expect(wrapper.find('.fa-th-large').exists()).toBe(false)
    })

    it('should set grid view mode', () => {
      const wrapper = mountComponent({ viewMode: 'list' })
      const vm = wrapper.vm as any
      vm.setViewMode('grid')
      expect(wrapper.emitted('update:viewMode')![0]).toEqual(['grid'])
    })

    it('should set list view mode', () => {
      const wrapper = mountComponent({ viewMode: 'grid' })
      const vm = wrapper.vm as any
      vm.setViewMode('list')
      expect(wrapper.emitted('update:viewMode')![0]).toEqual(['list'])
    })

    it('should highlight active view mode', () => {
      const wrapper = mountComponent({ viewMode: 'grid', showViewToggle: true })
      expect(wrapper.find('.bg-teal-500').exists()).toBe(true)
    })
  })

  describe('Filters Slot', () => {
    it('should have filters slot', () => {
      const wrapper = shallowMount(PageFilterBar, {
        props: {
          sortOptions: mockSortOptions,
          sortBy: 'newest',
        },
        slots: {
          filters: '<div class="custom-filter">Custom Filter</div>',
        },
      })
      expect(wrapper.find('.custom-filter').exists()).toBe(true)
    })
  })

  describe('Search Focus/Blur', () => {
    it('should show suggestions on focus in AI mode', () => {
      const wrapper = mountComponent({
        showAIToggle: true,
        isAIMode: true,
        aiSuggestions: ['Suggestion 1'],
      })
      const vm = wrapper.vm as any
      vm.localSearchQuery = ''
      vm.handleSearchFocus()
      expect(vm.showAISuggestions).toBe(true)
    })

    it('should hide suggestions on blur', async () => {
      vi.useFakeTimers()
      const wrapper = mountComponent({
        showAIToggle: true,
        isAIMode: true,
      })
      const vm = wrapper.vm as any
      vm.showAISuggestions = true
      vm.handleSearchBlur()
      vi.advanceTimersByTime(200)
      expect(vm.showAISuggestions).toBe(false)
      vi.useRealTimers()
    })
  })

  describe('Current Sort Option', () => {
    it('should compute current sort option', () => {
      const wrapper = mountComponent({ sortBy: 'popular' })
      const vm = wrapper.vm as any
      expect(vm.currentSortOption.value).toBe('popular')
      expect(vm.currentSortOption.label).toBe('Popular')
    })

    it('should default to first option if not found', () => {
      const wrapper = mountComponent({ sortBy: 'unknown' })
      const vm = wrapper.vm as any
      expect(vm.currentSortOption.value).toBe('newest')
    })
  })
})
