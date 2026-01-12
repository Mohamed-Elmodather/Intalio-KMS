import { ref, computed } from 'vue'
import type { Bookmark } from '@/types/detail-pages'

export function useBookmarks() {
  const bookmarks = ref<Bookmark[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Check if a specific content is bookmarked
  function isBookmarked(contentId: string, contentType: Bookmark['contentType']): boolean {
    return bookmarks.value.some(
      b => b.contentId === contentId && b.contentType === contentType
    )
  }

  // Get bookmark for specific content
  function getBookmark(contentId: string, contentType: Bookmark['contentType']): Bookmark | undefined {
    return bookmarks.value.find(
      b => b.contentId === contentId && b.contentType === contentType
    )
  }

  async function loadBookmarks() {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 500))
      // Mock bookmarks - in real app, fetch from API
      bookmarks.value = [
        {
          id: 'bm-1',
          contentId: 'article-1',
          contentType: 'article',
          createdAt: new Date(Date.now() - 86400000)
        },
        {
          id: 'bm-2',
          contentId: 'doc-5',
          contentType: 'document',
          createdAt: new Date(Date.now() - 172800000)
        }
      ]
    } catch (e) {
      error.value = 'Failed to load bookmarks'
      console.error(e)
    } finally {
      isLoading.value = false
    }
  }

  async function toggleBookmark(
    contentId: string,
    contentType: Bookmark['contentType'],
    collectionId?: string
  ): Promise<boolean> {
    const existing = getBookmark(contentId, contentType)

    try {
      await new Promise(resolve => setTimeout(resolve, 300))

      if (existing) {
        // Remove bookmark
        const index = bookmarks.value.findIndex(b => b.id === existing.id)
        if (index !== -1) {
          bookmarks.value.splice(index, 1)
        }
        return false
      } else {
        // Add bookmark
        const newBookmark: Bookmark = {
          id: `bm-${Date.now()}`,
          contentId,
          contentType,
          createdAt: new Date(),
          collectionId
        }
        bookmarks.value.push(newBookmark)
        return true
      }
    } catch (e) {
      error.value = 'Failed to update bookmark'
      throw e
    }
  }

  async function addToCollection(
    contentId: string,
    contentType: Bookmark['contentType'],
    collectionId: string
  ) {
    const existing = getBookmark(contentId, contentType)

    try {
      await new Promise(resolve => setTimeout(resolve, 300))

      if (existing) {
        existing.collectionId = collectionId
      } else {
        const newBookmark: Bookmark = {
          id: `bm-${Date.now()}`,
          contentId,
          contentType,
          createdAt: new Date(),
          collectionId
        }
        bookmarks.value.push(newBookmark)
      }
    } catch (e) {
      error.value = 'Failed to add to collection'
      throw e
    }
  }

  const bookmarkCount = computed(() => bookmarks.value.length)

  return {
    bookmarks,
    isLoading,
    error,
    bookmarkCount,
    isBookmarked,
    getBookmark,
    loadBookmarks,
    toggleBookmark,
    addToCollection
  }
}
