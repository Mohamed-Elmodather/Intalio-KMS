import { ref, computed, watch, type Ref, type ComputedRef } from 'vue'

export interface UsePaginationOptions {
  defaultPage?: number
  defaultPerPage?: number
  perPageOptions?: number[]
}

export interface UsePaginationReturn<T> {
  currentPage: Ref<number>
  itemsPerPage: Ref<number>
  itemsPerPageOptions: number[]
  totalPages: ComputedRef<number>
  totalItems: ComputedRef<number>
  paginatedItems: ComputedRef<T[]>
  startIndex: ComputedRef<number>
  endIndex: ComputedRef<number>
  goToPage: (page: number) => void
  nextPage: () => void
  prevPage: () => void
  resetPage: () => void
}

export function usePagination<T>(
  items: Ref<T[]> | ComputedRef<T[]>,
  options: UsePaginationOptions = {}
): UsePaginationReturn<T> {
  const {
    defaultPage = 1,
    defaultPerPage = 10,
    perPageOptions = [10, 20, 50, 100]
  } = options

  // State
  const currentPage = ref(defaultPage)
  const itemsPerPage = ref(defaultPerPage)

  // Computed
  const totalItems = computed(() => items.value.length)

  const totalPages = computed(() =>
    Math.ceil(totalItems.value / itemsPerPage.value) || 1
  )

  const paginatedItems = computed(() => {
    const start = (currentPage.value - 1) * itemsPerPage.value
    return items.value.slice(start, start + itemsPerPage.value)
  })

  const startIndex = computed(() => {
    if (totalItems.value === 0) return 0
    return (currentPage.value - 1) * itemsPerPage.value + 1
  })

  const endIndex = computed(() =>
    Math.min(currentPage.value * itemsPerPage.value, totalItems.value)
  )

  // Methods
  function goToPage(page: number) {
    if (page >= 1 && page <= totalPages.value) {
      currentPage.value = page
    }
  }

  function nextPage() {
    if (currentPage.value < totalPages.value) {
      currentPage.value++
    }
  }

  function prevPage() {
    if (currentPage.value > 1) {
      currentPage.value--
    }
  }

  function resetPage() {
    currentPage.value = 1
  }

  // Watch for items changes - reset to page 1 when items change significantly
  watch(totalItems, (newTotal, oldTotal) => {
    // Reset to page 1 if current page exceeds total pages
    if (currentPage.value > Math.ceil(newTotal / itemsPerPage.value)) {
      currentPage.value = 1
    }
  })

  // Reset to page 1 when items per page changes
  watch(itemsPerPage, () => {
    currentPage.value = 1
  })

  return {
    currentPage,
    itemsPerPage,
    itemsPerPageOptions: perPageOptions,
    totalPages,
    totalItems,
    paginatedItems,
    startIndex,
    endIndex,
    goToPage,
    nextPage,
    prevPage,
    resetPage
  }
}
