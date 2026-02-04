import { describe, it, expect, vi, beforeEach } from 'vitest'
import { ref, nextTick } from 'vue'
import { useFilters, filterPresets, sortPresets } from '@/composables/useFilters'

interface TestItem {
  id: number
  title: string
  category: string
  tags: string[]
  views: number
  createdAt: string
}

const createTestItems = (): TestItem[] => [
  {
    id: 1,
    title: 'Alpha Article',
    category: 'news',
    tags: ['sports', 'football'],
    views: 100,
    createdAt: '2024-01-15',
  },
  {
    id: 2,
    title: 'Beta Article',
    category: 'sports',
    tags: ['football', 'tournament'],
    views: 250,
    createdAt: '2024-01-10',
  },
  {
    id: 3,
    title: 'Gamma Article',
    category: 'news',
    tags: ['politics'],
    views: 50,
    createdAt: '2024-01-20',
  },
  {
    id: 4,
    title: 'Delta Article',
    category: 'tech',
    tags: ['ai', 'innovation'],
    views: 500,
    createdAt: '2024-01-05',
  },
]

describe('useFilters', () => {
  describe('Initial State', () => {
    it('should have empty search query', () => {
      const items = ref(createTestItems())
      const { searchQuery } = useFilters({ items })
      expect(searchQuery.value).toBe('')
    })

    it('should have no active filters', () => {
      const items = ref(createTestItems())
      const { activeFiltersCount, hasActiveFilters } = useFilters({ items })
      expect(activeFiltersCount.value).toBe(0)
      expect(hasActiveFilters.value).toBe(false)
    })

    it('should return all items when no filters applied', () => {
      const items = ref(createTestItems())
      const { filteredItems } = useFilters({ items })
      expect(filteredItems.value).toHaveLength(4)
    })
  })

  describe('Search Filtering', () => {
    it('should filter items by search query', () => {
      const items = ref(createTestItems())
      const { searchQuery, filteredItems } = useFilters({
        items,
        search: { fields: ['title'] },
      })

      searchQuery.value = 'alpha'
      expect(filteredItems.value).toHaveLength(1)
      expect(filteredItems.value[0].title).toBe('Alpha Article')
    })

    it('should search case-insensitively', () => {
      const items = ref(createTestItems())
      const { searchQuery, filteredItems } = useFilters({
        items,
        search: { fields: ['title'] },
      })

      searchQuery.value = 'BETA'
      expect(filteredItems.value).toHaveLength(1)
      expect(filteredItems.value[0].title).toBe('Beta Article')
    })

    it('should search in multiple fields', () => {
      const items = ref(createTestItems())
      const { searchQuery, filteredItems } = useFilters({
        items,
        search: { fields: ['title', 'category'] },
      })

      searchQuery.value = 'tech'
      expect(filteredItems.value).toHaveLength(1)
      expect(filteredItems.value[0].category).toBe('tech')
    })

    it('should search in array fields', () => {
      const items = ref(createTestItems())
      const { searchQuery, filteredItems } = useFilters({
        items,
        search: { fields: ['tags'] },
      })

      searchQuery.value = 'football'
      expect(filteredItems.value).toHaveLength(2)
    })

    it('should use custom search function', () => {
      const items = ref(createTestItems())
      const { searchQuery, filteredItems } = useFilters({
        items,
        search: {
          fields: [],
          searchFn: (item, query) => item.views > parseInt(query) || false,
        },
      })

      searchQuery.value = '200'
      expect(filteredItems.value).toHaveLength(2) // Views > 200: Beta (250), Delta (500)
    })
  })

  describe('Filter Configuration', () => {
    it('should filter by configured filter', () => {
      const items = ref(createTestItems())
      const { getFilter, filteredItems } = useFilters({
        items,
        filters: [{ key: 'category' }],
      })

      const categoryFilter = getFilter('category')
      categoryFilter.value = ['news']

      expect(filteredItems.value).toHaveLength(2)
      expect(filteredItems.value.every((i) => i.category === 'news')).toBe(true)
    })

    it('should filter by multiple values (OR mode)', () => {
      const items = ref(createTestItems())
      const { getFilter, filteredItems } = useFilters({
        items,
        filters: [{ key: 'category', matchMode: 'any' }],
      })

      const categoryFilter = getFilter('category')
      categoryFilter.value = ['news', 'tech']

      expect(filteredItems.value).toHaveLength(3)
    })

    it('should filter with custom accessor', () => {
      const items = ref(createTestItems())
      const { getFilter, filteredItems } = useFilters({
        items,
        filters: [
          {
            key: 'tags',
            accessor: (item) => item.tags,
            matchMode: 'any',
          },
        ],
      })

      const tagsFilter = getFilter('tags')
      tagsFilter.value = ['football']

      expect(filteredItems.value).toHaveLength(2)
    })

    it('should use initial filter values', () => {
      const items = ref(createTestItems())
      const { filteredItems } = useFilters({
        items,
        filters: [{ key: 'category', initialValue: ['news'] }],
      })

      expect(filteredItems.value).toHaveLength(2)
    })
  })

  describe('Filter Actions', () => {
    it('should toggle filter value', () => {
      const items = ref(createTestItems())
      const { getFilter, toggleFilter } = useFilters({
        items,
        filters: [{ key: 'category' }],
      })

      toggleFilter('category', 'news')
      expect(getFilter('category').value).toContain('news')

      toggleFilter('category', 'news')
      expect(getFilter('category').value).not.toContain('news')
    })

    it('should set filter values', () => {
      const items = ref(createTestItems())
      const { getFilter, setFilter } = useFilters({
        items,
        filters: [{ key: 'category' }],
      })

      setFilter('category', ['news', 'sports'])
      expect(getFilter('category').value).toEqual(['news', 'sports'])
    })

    it('should remove specific filter value', () => {
      const items = ref(createTestItems())
      const { getFilter, setFilter, removeFilterValue } = useFilters({
        items,
        filters: [{ key: 'category' }],
      })

      setFilter('category', ['news', 'sports', 'tech'])
      removeFilterValue('category', 'sports')

      expect(getFilter('category').value).toEqual(['news', 'tech'])
    })

    it('should clear specific filter', () => {
      const items = ref(createTestItems())
      const { getFilter, setFilter, clearFilter } = useFilters({
        items,
        filters: [{ key: 'category' }],
      })

      setFilter('category', ['news', 'sports'])
      clearFilter('category')

      expect(getFilter('category').value).toEqual([])
    })

    it('should clear all filters', () => {
      const items = ref(createTestItems())
      const { searchQuery, getFilter, setFilter, clearFilters } = useFilters({
        items,
        filters: [{ key: 'category' }, { key: 'tags' }],
        search: { fields: ['title'] },
      })

      setFilter('category', ['news'])
      setFilter('tags', ['football'])
      searchQuery.value = 'test'

      clearFilters()

      expect(getFilter('category').value).toEqual([])
      expect(getFilter('tags').value).toEqual([])
      expect(searchQuery.value).toBe('')
    })
  })

  describe('Sorting', () => {
    it('should sort items by configured sort function', () => {
      const items = ref(createTestItems())
      const { filteredItems, setSort } = useFilters({
        items,
        sort: {
          options: [
            {
              value: 'views',
              label: 'Views',
              // sortFn returns positive when a > b (natural ascending)
              sortFn: (a, b) => a.views - b.views,
            },
          ],
          defaultOrder: 'desc',
        },
      })

      setSort('views', 'desc')
      expect(filteredItems.value[0].views).toBe(500) // Delta (highest views first in desc)
      expect(filteredItems.value[3].views).toBe(50) // Gamma (lowest views last)
    })

    it('should respect sort order', () => {
      const items = ref(createTestItems())
      const { filteredItems, setSort } = useFilters({
        items,
        sort: {
          options: [
            {
              value: 'views',
              label: 'Views',
              sortFn: (a, b) => a.views - b.views,
            },
          ],
          defaultOrder: 'asc',
        },
      })

      setSort('views', 'asc')
      expect(filteredItems.value[0].views).toBe(50) // Gamma first (ascending - lowest first)
    })

    it('should toggle sort order', () => {
      const items = ref(createTestItems())
      const { sortOrder, toggleSortOrder } = useFilters({
        items,
        sort: { options: [], defaultOrder: 'desc' },
      })

      expect(sortOrder.value).toBe('desc')
      toggleSortOrder()
      expect(sortOrder.value).toBe('asc')
      toggleSortOrder()
      expect(sortOrder.value).toBe('desc')
    })

    it('should reset sort to defaults', () => {
      const items = ref(createTestItems())
      const { sortBy, sortOrder, setSort, resetSort } = useFilters({
        items,
        sort: {
          options: [{ value: 'views', label: 'Views' }],
          defaultValue: 'recent',
          defaultOrder: 'desc',
        },
      })

      setSort('views', 'asc')
      resetSort()

      expect(sortBy.value).toBe('recent')
      expect(sortOrder.value).toBe('desc')
    })
  })

  describe('Computed Properties', () => {
    it('should compute active filters count', () => {
      const items = ref(createTestItems())
      const { activeFiltersCount, searchQuery, setFilter } = useFilters({
        items,
        filters: [{ key: 'category' }],
        search: { fields: ['title'] },
      })

      expect(activeFiltersCount.value).toBe(0)

      setFilter('category', ['news', 'sports'])
      expect(activeFiltersCount.value).toBe(2)

      searchQuery.value = 'test'
      expect(activeFiltersCount.value).toBe(3) // 2 category + 1 search
    })

    it('should compute hasActiveFilters', () => {
      const items = ref(createTestItems())
      const { hasActiveFilters, setFilter } = useFilters({
        items,
        filters: [{ key: 'category' }],
      })

      expect(hasActiveFilters.value).toBe(false)

      setFilter('category', ['news'])
      expect(hasActiveFilters.value).toBe(true)
    })

    it('should compute activeFilters object', () => {
      const items = ref(createTestItems())
      const { activeFilters, setFilter } = useFilters({
        items,
        filters: [{ key: 'category' }, { key: 'tags' }],
      })

      setFilter('category', ['news'])
      setFilter('tags', ['football', 'sports'])

      expect(activeFilters.value).toEqual({
        category: ['news'],
        tags: ['football', 'sports'],
      })
    })
  })

  describe('onFiltersChange Callback', () => {
    it('should call callback when filter changes', async () => {
      const items = ref(createTestItems())
      const callback = vi.fn()

      const { setFilter } = useFilters({
        items,
        filters: [{ key: 'category' }],
        onFiltersChange: callback,
      })

      setFilter('category', ['news'])
      await nextTick()

      expect(callback).toHaveBeenCalled()
    })

    it('should call callback when search changes', async () => {
      const items = ref(createTestItems())
      const callback = vi.fn()

      const { searchQuery } = useFilters({
        items,
        search: { fields: ['title'] },
        onFiltersChange: callback,
      })

      searchQuery.value = 'test'
      await nextTick()

      expect(callback).toHaveBeenCalled()
    })
  })

  describe('Filter Presets', () => {
    it('should create content filter preset', () => {
      const preset = filterPresets.content()

      expect(preset).toHaveLength(3)
      expect(preset.map((f) => f.key)).toEqual(['categories', 'types', 'tags'])
    })

    it('should create content filter with custom accessors', () => {
      const preset = filterPresets.content({
        categoryAccessor: (item) => item.customCategory,
      })

      expect(preset[0].accessor).toBeDefined()
    })
  })

  describe('Sort Presets', () => {
    it('should create content sort preset', () => {
      const preset = sortPresets.content()

      expect(preset.options).toHaveLength(3)
      expect(preset.options.map((o) => o.value)).toEqual([
        'recent',
        'popular',
        'title',
      ])
      expect(preset.defaultValue).toBe('recent')
    })

    it('should create documents sort preset', () => {
      const preset = sortPresets.documents()

      expect(preset.options).toHaveLength(3)
      expect(preset.options.map((o) => o.value)).toEqual(['date', 'name', 'size'])
    })

    it('should create events sort preset', () => {
      const preset = sortPresets.events()

      expect(preset.options).toHaveLength(3)
      expect(preset.defaultOrder).toBe('asc')
    })
  })
})
