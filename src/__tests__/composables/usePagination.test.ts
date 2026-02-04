import { describe, it, expect, beforeEach } from 'vitest'
import { ref, nextTick } from 'vue'
import { usePagination } from '@/composables/usePagination'

describe('usePagination', () => {
  describe('Initial State', () => {
    it('should have default page 1', () => {
      const items = ref([1, 2, 3, 4, 5])
      const { currentPage } = usePagination(items)
      expect(currentPage.value).toBe(1)
    })

    it('should have default items per page of 10', () => {
      const items = ref([1, 2, 3, 4, 5])
      const { itemsPerPage } = usePagination(items)
      expect(itemsPerPage.value).toBe(10)
    })

    it('should accept custom default page', () => {
      const items = ref(Array.from({ length: 50 }, (_, i) => i))
      const { currentPage } = usePagination(items, { defaultPage: 3 })
      expect(currentPage.value).toBe(3)
    })

    it('should accept custom items per page', () => {
      const items = ref([1, 2, 3, 4, 5])
      const { itemsPerPage } = usePagination(items, { defaultPerPage: 5 })
      expect(itemsPerPage.value).toBe(5)
    })

    it('should accept custom per page options', () => {
      const items = ref([1, 2, 3, 4, 5])
      const { itemsPerPageOptions } = usePagination(items, {
        perPageOptions: [5, 15, 25],
      })
      expect(itemsPerPageOptions).toEqual([5, 15, 25])
    })
  })

  describe('Computed Properties', () => {
    it('should compute total items', () => {
      const items = ref([1, 2, 3, 4, 5])
      const { totalItems } = usePagination(items)
      expect(totalItems.value).toBe(5)
    })

    it('should compute total pages', () => {
      const items = ref(Array.from({ length: 25 }, (_, i) => i))
      const { totalPages } = usePagination(items, { defaultPerPage: 10 })
      expect(totalPages.value).toBe(3)
    })

    it('should return at least 1 total page for empty items', () => {
      const items = ref<number[]>([])
      const { totalPages } = usePagination(items)
      expect(totalPages.value).toBe(1)
    })

    it('should compute paginated items correctly', () => {
      const items = ref([1, 2, 3, 4, 5, 6, 7, 8, 9, 10])
      const { paginatedItems } = usePagination(items, { defaultPerPage: 3 })
      expect(paginatedItems.value).toEqual([1, 2, 3])
    })

    it('should compute start index', () => {
      const items = ref(Array.from({ length: 25 }, (_, i) => i + 1))
      const { startIndex, goToPage } = usePagination(items, { defaultPerPage: 10 })

      expect(startIndex.value).toBe(1)

      goToPage(2)
      expect(startIndex.value).toBe(11)
    })

    it('should return 0 start index for empty items', () => {
      const items = ref<number[]>([])
      const { startIndex } = usePagination(items)
      expect(startIndex.value).toBe(0)
    })

    it('should compute end index', () => {
      const items = ref(Array.from({ length: 25 }, (_, i) => i + 1))
      const { endIndex, goToPage } = usePagination(items, { defaultPerPage: 10 })

      expect(endIndex.value).toBe(10)

      goToPage(3)
      expect(endIndex.value).toBe(25)
    })
  })

  describe('goToPage', () => {
    it('should navigate to a valid page', () => {
      const items = ref(Array.from({ length: 50 }, (_, i) => i))
      const { currentPage, goToPage } = usePagination(items, { defaultPerPage: 10 })

      goToPage(3)
      expect(currentPage.value).toBe(3)
    })

    it('should not navigate to page below 1', () => {
      const items = ref(Array.from({ length: 50 }, (_, i) => i))
      const { currentPage, goToPage } = usePagination(items, { defaultPerPage: 10 })

      goToPage(0)
      expect(currentPage.value).toBe(1)

      goToPage(-1)
      expect(currentPage.value).toBe(1)
    })

    it('should not navigate beyond total pages', () => {
      const items = ref(Array.from({ length: 25 }, (_, i) => i))
      const { currentPage, goToPage, totalPages } = usePagination(items, {
        defaultPerPage: 10,
      })

      goToPage(100)
      expect(currentPage.value).toBe(1) // Should stay at current page
    })
  })

  describe('nextPage', () => {
    it('should go to next page', () => {
      const items = ref(Array.from({ length: 50 }, (_, i) => i))
      const { currentPage, nextPage } = usePagination(items, { defaultPerPage: 10 })

      nextPage()
      expect(currentPage.value).toBe(2)
    })

    it('should not go beyond last page', () => {
      const items = ref(Array.from({ length: 25 }, (_, i) => i))
      const { currentPage, nextPage, goToPage } = usePagination(items, {
        defaultPerPage: 10,
      })

      goToPage(3) // Go to last page
      nextPage()
      expect(currentPage.value).toBe(3)
    })
  })

  describe('prevPage', () => {
    it('should go to previous page', () => {
      const items = ref(Array.from({ length: 50 }, (_, i) => i))
      const { currentPage, prevPage, goToPage } = usePagination(items, {
        defaultPerPage: 10,
      })

      goToPage(3)
      prevPage()
      expect(currentPage.value).toBe(2)
    })

    it('should not go below page 1', () => {
      const items = ref(Array.from({ length: 50 }, (_, i) => i))
      const { currentPage, prevPage } = usePagination(items, { defaultPerPage: 10 })

      prevPage()
      expect(currentPage.value).toBe(1)
    })
  })

  describe('resetPage', () => {
    it('should reset to page 1', () => {
      const items = ref(Array.from({ length: 50 }, (_, i) => i))
      const { currentPage, goToPage, resetPage } = usePagination(items, {
        defaultPerPage: 10,
      })

      goToPage(5)
      resetPage()
      expect(currentPage.value).toBe(1)
    })
  })

  describe('Watchers', () => {
    it('should reset page when items per page changes', async () => {
      const items = ref(Array.from({ length: 50 }, (_, i) => i))
      const { currentPage, itemsPerPage, goToPage } = usePagination(items, {
        defaultPerPage: 10,
      })

      goToPage(3)
      itemsPerPage.value = 20
      await nextTick()

      expect(currentPage.value).toBe(1)
    })

    it('should reset to page 1 when items shrink below current page', async () => {
      const items = ref(Array.from({ length: 50 }, (_, i) => i))
      const { currentPage, goToPage } = usePagination(items, { defaultPerPage: 10 })

      goToPage(5) // Page 5 of 5
      items.value = items.value.slice(0, 10) // Now only 1 page
      await nextTick()

      expect(currentPage.value).toBe(1)
    })
  })

  describe('Paginated Items', () => {
    it('should return correct items for page 1', () => {
      const items = ref(['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j'])
      const { paginatedItems } = usePagination(items, { defaultPerPage: 3 })

      expect(paginatedItems.value).toEqual(['a', 'b', 'c'])
    })

    it('should return correct items for middle page', () => {
      const items = ref(['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j'])
      const { paginatedItems, goToPage } = usePagination(items, { defaultPerPage: 3 })

      goToPage(2)
      expect(paginatedItems.value).toEqual(['d', 'e', 'f'])
    })

    it('should return correct items for last page (partial)', () => {
      const items = ref(['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j'])
      const { paginatedItems, goToPage } = usePagination(items, { defaultPerPage: 3 })

      goToPage(4)
      expect(paginatedItems.value).toEqual(['j'])
    })

    it('should return empty array for empty items', () => {
      const items = ref<string[]>([])
      const { paginatedItems } = usePagination(items)

      expect(paginatedItems.value).toEqual([])
    })
  })
})
