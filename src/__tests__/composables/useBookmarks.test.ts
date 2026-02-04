import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useBookmarks } from '@/composables/useBookmarks'

describe('useBookmarks', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Initial State', () => {
    it('should have empty bookmarks array', () => {
      const { bookmarks } = useBookmarks()
      expect(bookmarks.value).toEqual([])
    })

    it('should not be loading initially', () => {
      const { isLoading } = useBookmarks()
      expect(isLoading.value).toBe(false)
    })

    it('should have null error initially', () => {
      const { error } = useBookmarks()
      expect(error.value).toBeNull()
    })

    it('should have zero bookmark count', () => {
      const { bookmarkCount } = useBookmarks()
      expect(bookmarkCount.value).toBe(0)
    })
  })

  describe('isBookmarked', () => {
    it('should return false when not bookmarked', () => {
      const { isBookmarked } = useBookmarks()
      expect(isBookmarked('article-1', 'article')).toBe(false)
    })

    it('should return true when bookmarked', () => {
      const { bookmarks, isBookmarked } = useBookmarks()
      bookmarks.value = [
        {
          id: 'bm-1',
          contentId: 'article-1',
          contentType: 'article',
          createdAt: new Date(),
        },
      ]

      expect(isBookmarked('article-1', 'article')).toBe(true)
    })

    it('should differentiate by content type', () => {
      const { bookmarks, isBookmarked } = useBookmarks()
      bookmarks.value = [
        {
          id: 'bm-1',
          contentId: 'item-1',
          contentType: 'article',
          createdAt: new Date(),
        },
      ]

      expect(isBookmarked('item-1', 'article')).toBe(true)
      expect(isBookmarked('item-1', 'document')).toBe(false)
    })
  })

  describe('getBookmark', () => {
    it('should return undefined when not found', () => {
      const { getBookmark } = useBookmarks()
      expect(getBookmark('article-1', 'article')).toBeUndefined()
    })

    it('should return bookmark when found', () => {
      const { bookmarks, getBookmark } = useBookmarks()
      const bookmark = {
        id: 'bm-1',
        contentId: 'article-1',
        contentType: 'article' as const,
        createdAt: new Date(),
      }
      bookmarks.value = [bookmark]

      const result = getBookmark('article-1', 'article')
      expect(result).toBeDefined()
      expect(result?.id).toBe('bm-1')
    })
  })

  describe('loadBookmarks', () => {
    it('should set loading state while fetching', async () => {
      const { isLoading, loadBookmarks } = useBookmarks()

      const promise = loadBookmarks()
      expect(isLoading.value).toBe(true)

      await promise
      expect(isLoading.value).toBe(false)
    })

    it('should load mock bookmarks', async () => {
      const { bookmarks, loadBookmarks } = useBookmarks()

      await loadBookmarks()

      expect(bookmarks.value.length).toBeGreaterThan(0)
    })

    it('should clear error before loading', async () => {
      const { error, loadBookmarks } = useBookmarks()
      error.value = 'Previous error'

      await loadBookmarks()

      expect(error.value).toBeNull()
    })
  })

  describe('toggleBookmark', () => {
    it('should add bookmark when not exists', async () => {
      const { bookmarks, toggleBookmark } = useBookmarks()

      const result = await toggleBookmark('new-article', 'article')

      expect(result).toBe(true)
      expect(bookmarks.value.some((b) => b.contentId === 'new-article')).toBe(true)
    })

    it('should remove bookmark when exists', async () => {
      const { bookmarks, toggleBookmark } = useBookmarks()
      bookmarks.value = [
        {
          id: 'bm-1',
          contentId: 'article-1',
          contentType: 'article',
          createdAt: new Date(),
        },
      ]

      const result = await toggleBookmark('article-1', 'article')

      expect(result).toBe(false)
      expect(bookmarks.value.some((b) => b.contentId === 'article-1')).toBe(false)
    })

    it('should add bookmark with collection id', async () => {
      const { bookmarks, toggleBookmark } = useBookmarks()

      await toggleBookmark('article-1', 'article', 'collection-1')

      const bookmark = bookmarks.value.find((b) => b.contentId === 'article-1')
      expect(bookmark?.collectionId).toBe('collection-1')
    })
  })

  describe('addToCollection', () => {
    it('should update existing bookmark with collection', async () => {
      const { bookmarks, addToCollection } = useBookmarks()
      bookmarks.value = [
        {
          id: 'bm-1',
          contentId: 'article-1',
          contentType: 'article',
          createdAt: new Date(),
        },
      ]

      await addToCollection('article-1', 'article', 'collection-1')

      expect(bookmarks.value[0].collectionId).toBe('collection-1')
    })

    it('should create new bookmark with collection if not exists', async () => {
      const { bookmarks, addToCollection } = useBookmarks()

      await addToCollection('new-article', 'article', 'collection-1')

      const bookmark = bookmarks.value.find((b) => b.contentId === 'new-article')
      expect(bookmark).toBeDefined()
      expect(bookmark?.collectionId).toBe('collection-1')
    })
  })

  describe('bookmarkCount', () => {
    it('should update when bookmarks change', async () => {
      const { bookmarks, bookmarkCount, toggleBookmark } = useBookmarks()

      expect(bookmarkCount.value).toBe(0)

      await toggleBookmark('article-1', 'article')
      expect(bookmarkCount.value).toBe(1)

      await toggleBookmark('article-2', 'article')
      expect(bookmarkCount.value).toBe(2)

      await toggleBookmark('article-1', 'article')
      expect(bookmarkCount.value).toBe(1)
    })
  })
})
