import { ref, computed, watch, type Ref, type ComputedRef } from 'vue'

/**
 * Filter configuration for a specific filter type
 */
export interface FilterConfig<T = string> {
  /** Unique key for this filter */
  key: string
  /** Display label */
  label?: string
  /** Initial selected values */
  initialValue?: T[]
  /** Function to extract the filterable value from an item */
  accessor?: (item: any) => T | T[] | undefined
  /** Whether to match any (OR) or all (AND) values */
  matchMode?: 'any' | 'all'
}

/**
 * Sort configuration
 */
export interface SortConfig {
  /** Available sort options */
  options: Array<{
    value: string
    label: string
    icon?: string
    sortFn?: (a: any, b: any) => number
  }>
  /** Default sort value */
  defaultValue?: string
  /** Default sort order */
  defaultOrder?: 'asc' | 'desc'
}

/**
 * Options for useFilters composable
 */
export interface UseFiltersOptions<T> {
  /** Source items to filter */
  items: Ref<T[]> | ComputedRef<T[]>
  /** Filter configurations */
  filters?: FilterConfig[]
  /** Sort configuration */
  sort?: SortConfig
  /** Search configuration */
  search?: {
    /** Fields to search in */
    fields: string[]
    /** Custom search function */
    searchFn?: (item: T, query: string) => boolean
  }
  /** Callback when filters change */
  onFiltersChange?: () => void
}

/**
 * Composable for unified filtering, sorting, and searching
 */
export function useFilters<T extends Record<string, any>>(options: UseFiltersOptions<T>) {
  const { items, filters = [], sort, search, onFiltersChange } = options

  // ============================================
  // FILTER STATE
  // ============================================

  // Create reactive refs for each filter
  const filterRefs = new Map<string, Ref<string[]>>()
  filters.forEach(config => {
    filterRefs.set(config.key, ref(config.initialValue || []))
  })

  // Search query
  const searchQuery = ref('')

  // Sorting
  const sortBy = ref(sort?.defaultValue || '')
  const sortOrder = ref<'asc' | 'desc'>(sort?.defaultOrder || 'desc')

  // ============================================
  // COMPUTED PROPERTIES
  // ============================================

  /**
   * Get a filter ref by key
   */
  function getFilter(key: string): Ref<string[]> {
    const filterRef = filterRefs.get(key)
    if (!filterRef) {
      // Create on-demand if not configured
      const newRef = ref<string[]>([])
      filterRefs.set(key, newRef)
      return newRef
    }
    return filterRef
  }

  /**
   * Count of active filters (excluding search)
   */
  const activeFiltersCount = computed(() => {
    let count = 0
    filterRefs.forEach(filterRef => {
      count += filterRef.value.length
    })
    if (searchQuery.value) count++
    return count
  })

  /**
   * Check if any filters are active
   */
  const hasActiveFilters = computed(() => activeFiltersCount.value > 0)

  /**
   * Get all active filter values as an object
   */
  const activeFilters = computed(() => {
    const result: Record<string, string[]> = {}
    filterRefs.forEach((filterRef, key) => {
      if (filterRef.value.length > 0) {
        result[key] = filterRef.value
      }
    })
    return result
  })

  /**
   * Filtered items based on all active filters
   */
  const filteredItems = computed(() => {
    let result = [...items.value]

    // Apply search
    if (searchQuery.value && search) {
      const query = searchQuery.value.toLowerCase()
      result = result.filter(item => {
        if (search.searchFn) {
          return search.searchFn(item, query)
        }
        // Default: search in specified fields
        return search.fields.some(field => {
          const value = getNestedValue(item, field)
          if (Array.isArray(value)) {
            return value.some(v => String(v).toLowerCase().includes(query))
          }
          return value && String(value).toLowerCase().includes(query)
        })
      })
    }

    // Apply each filter
    filters.forEach(config => {
      const filterRef = filterRefs.get(config.key)
      if (!filterRef || filterRef.value.length === 0) return

      const selectedValues = filterRef.value
      const accessor = config.accessor
      const matchMode = config.matchMode || 'any'

      result = result.filter(item => {
        const itemValue = accessor ? accessor(item) : item[config.key]

        if (itemValue === undefined || itemValue === null) {
          return false
        }

        const itemValues = Array.isArray(itemValue) ? itemValue : [itemValue]

        if (matchMode === 'all') {
          return selectedValues.every(v => itemValues.includes(v))
        } else {
          return selectedValues.some(v => itemValues.includes(v))
        }
      })
    })

    // Apply sorting
    if (sortBy.value && sort?.options) {
      const sortOption = sort.options.find(opt => opt.value === sortBy.value)
      if (sortOption?.sortFn) {
        result.sort((a, b) => {
          const comparison = sortOption.sortFn!(a, b)
          return sortOrder.value === 'desc' ? -comparison : comparison
        })
      }
    }

    return result
  })

  // ============================================
  // METHODS
  // ============================================

  /**
   * Clear all filters
   */
  function clearFilters() {
    filterRefs.forEach(filterRef => {
      filterRef.value = []
    })
    searchQuery.value = ''
  }

  /**
   * Clear a specific filter
   */
  function clearFilter(key: string) {
    const filterRef = filterRefs.get(key)
    if (filterRef) {
      filterRef.value = []
    }
  }

  /**
   * Toggle a value in a filter
   */
  function toggleFilter(key: string, value: string) {
    const filterRef = filterRefs.get(key)
    if (!filterRef) return

    const index = filterRef.value.indexOf(value)
    if (index > -1) {
      filterRef.value.splice(index, 1)
    } else {
      filterRef.value.push(value)
    }
  }

  /**
   * Set filter values
   */
  function setFilter(key: string, values: string[]) {
    const filterRef = filterRefs.get(key)
    if (filterRef) {
      filterRef.value = values
    }
  }

  /**
   * Remove a specific value from a filter
   */
  function removeFilterValue(key: string, value: string) {
    const filterRef = filterRefs.get(key)
    if (!filterRef) return

    const index = filterRef.value.indexOf(value)
    if (index > -1) {
      filterRef.value.splice(index, 1)
    }
  }

  /**
   * Reset sorting to defaults
   */
  function resetSort() {
    sortBy.value = sort?.defaultValue || ''
    sortOrder.value = sort?.defaultOrder || 'desc'
  }

  /**
   * Toggle sort order
   */
  function toggleSortOrder() {
    sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc'
  }

  /**
   * Set sort option
   */
  function setSort(value: string, order?: 'asc' | 'desc') {
    sortBy.value = value
    if (order) {
      sortOrder.value = order
    }
  }

  // ============================================
  // WATCHERS
  // ============================================

  // Watch all filter changes
  if (onFiltersChange) {
    const watchSources = [searchQuery, sortBy, sortOrder]
    filterRefs.forEach(filterRef => {
      watchSources.push(filterRef)
    })
    watch(watchSources, onFiltersChange, { deep: true })
  }

  // ============================================
  // HELPERS
  // ============================================

  /**
   * Get nested value from object using dot notation
   */
  function getNestedValue(obj: any, path: string): any {
    return path.split('.').reduce((current, key) => {
      return current && current[key] !== undefined ? current[key] : undefined
    }, obj)
  }

  // ============================================
  // RETURN
  // ============================================

  return {
    // State
    searchQuery,
    sortBy,
    sortOrder,

    // Computed
    filteredItems,
    activeFiltersCount,
    hasActiveFilters,
    activeFilters,

    // Methods
    getFilter,
    setFilter,
    toggleFilter,
    clearFilter,
    removeFilterValue,
    clearFilters,
    setSort,
    resetSort,
    toggleSortOrder
  }
}

/**
 * Preset filter configurations for common content types
 */
export const filterPresets = {
  /**
   * Standard content filters (categories, types, tags, status)
   */
  content: (options?: {
    categoryAccessor?: (item: any) => string | undefined
    typeAccessor?: (item: any) => string | undefined
    tagsAccessor?: (item: any) => string[] | undefined
  }): FilterConfig[] => [
    {
      key: 'categories',
      label: 'Categories',
      accessor: options?.categoryAccessor || ((item) => item.categoryId || item.category)
    },
    {
      key: 'types',
      label: 'Types',
      accessor: options?.typeAccessor || ((item) => item.type)
    },
    {
      key: 'tags',
      label: 'Tags',
      accessor: options?.tagsAccessor || ((item) => item.tags),
      matchMode: 'any'
    }
  ],

  /**
   * Status filters for saved/shared items
   */
  status: (options: {
    bookmarksRef?: Ref<number[]>
    savedRef?: Ref<Set<number | string>>
    sharedField?: string
  }): FilterConfig[] => [
    {
      key: 'status',
      label: 'Status',
      accessor: (item) => {
        const statuses: string[] = []

        // Check if bookmarked/saved
        if (options.bookmarksRef && options.bookmarksRef.value.includes(item.id)) {
          statuses.push('saved')
        }
        if (options.savedRef && options.savedRef.value.has(item.id)) {
          statuses.push('saved')
        }

        // Check if shared
        const sharedField = options.sharedField || 'sharedWithMe'
        if (item[sharedField]) {
          statuses.push('shared')
        }

        return statuses.length > 0 ? statuses : undefined
      },
      matchMode: 'any'
    }
  ]
}

/**
 * Common sort presets
 */
export const sortPresets = {
  /**
   * Standard content sorting (recent, popular, alphabetical)
   */
  content: (): SortConfig => ({
    options: [
      {
        value: 'recent',
        label: 'Most Recent',
        icon: 'fas fa-clock',
        sortFn: (a, b) => {
          const dateA = new Date(a.createdAt || a.date || 0).getTime()
          const dateB = new Date(b.createdAt || b.date || 0).getTime()
          return dateB - dateA
        }
      },
      {
        value: 'popular',
        label: 'Most Popular',
        icon: 'fas fa-fire',
        sortFn: (a, b) => (b.views || 0) - (a.views || 0)
      },
      {
        value: 'title',
        label: 'Alphabetical',
        icon: 'fas fa-sort-alpha-down',
        sortFn: (a, b) => (a.title || '').localeCompare(b.title || '')
      }
    ],
    defaultValue: 'recent',
    defaultOrder: 'desc'
  }),

  /**
   * Document sorting (date, name, size)
   */
  documents: (): SortConfig => ({
    options: [
      {
        value: 'date',
        label: 'Date Modified',
        icon: 'fas fa-clock',
        sortFn: (a, b) => {
          const dateA = new Date(a.modifiedAt || a.date || 0).getTime()
          const dateB = new Date(b.modifiedAt || b.date || 0).getTime()
          return dateB - dateA
        }
      },
      {
        value: 'name',
        label: 'Name',
        icon: 'fas fa-font',
        sortFn: (a, b) => (a.name || a.title || '').localeCompare(b.name || b.title || '')
      },
      {
        value: 'size',
        label: 'Size',
        icon: 'fas fa-weight-hanging',
        sortFn: (a, b) => (b.sizeBytes || 0) - (a.sizeBytes || 0)
      }
    ],
    defaultValue: 'date',
    defaultOrder: 'desc'
  }),

  /**
   * Event sorting (date, popularity)
   */
  events: (): SortConfig => ({
    options: [
      {
        value: 'date',
        label: 'Event Date',
        icon: 'fas fa-calendar',
        sortFn: (a, b) => {
          const dateA = new Date(a.eventDate || a.date || 0).getTime()
          const dateB = new Date(b.eventDate || b.date || 0).getTime()
          return dateA - dateB // Upcoming first
        }
      },
      {
        value: 'popular',
        label: 'Most Popular',
        icon: 'fas fa-users',
        sortFn: (a, b) => (b.attendeesCount || b.attendees?.length || 0) - (a.attendeesCount || a.attendees?.length || 0)
      },
      {
        value: 'title',
        label: 'Alphabetical',
        icon: 'fas fa-sort-alpha-down',
        sortFn: (a, b) => (a.title || '').localeCompare(b.title || '')
      }
    ],
    defaultValue: 'date',
    defaultOrder: 'asc'
  })
}
