import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useRelatedContent } from '@/composables/useRelatedContent'

describe('useRelatedContent', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Initial State', () => {
    it('should have empty related items array', () => {
      const { relatedItems } = useRelatedContent('article', 'test-id')
      expect(relatedItems.value).toEqual([])
    })

    it('should not be loading initially', () => {
      const { isLoading } = useRelatedContent('article', 'test-id')
      expect(isLoading.value).toBe(false)
    })

    it('should have null error initially', () => {
      const { error } = useRelatedContent('article', 'test-id')
      expect(error.value).toBeNull()
    })
  })

  describe('loadRelatedContent', () => {
    it('should set loading state while fetching', async () => {
      const { isLoading, loadRelatedContent } = useRelatedContent('article', 'test-id')

      const promise = loadRelatedContent()
      expect(isLoading.value).toBe(true)

      await promise
      expect(isLoading.value).toBe(false)
    })

    it('should load related content for articles', async () => {
      const { relatedItems, loadRelatedContent } = useRelatedContent('article', 'test-id')

      await loadRelatedContent()

      expect(relatedItems.value.length).toBeGreaterThan(0)
    })

    it('should load related content for documents', async () => {
      const { relatedItems, loadRelatedContent } = useRelatedContent('document', 'test-id')

      await loadRelatedContent()

      expect(relatedItems.value.length).toBeGreaterThan(0)
    })

    it('should load related content for media', async () => {
      const { relatedItems, loadRelatedContent } = useRelatedContent('media', 'test-id')

      await loadRelatedContent()

      expect(relatedItems.value.length).toBeGreaterThan(0)
    })

    it('should load related content for courses', async () => {
      const { relatedItems, loadRelatedContent } = useRelatedContent('course', 'test-id')

      await loadRelatedContent()

      expect(relatedItems.value.length).toBeGreaterThan(0)
    })

    it('should load related content for events', async () => {
      const { relatedItems, loadRelatedContent } = useRelatedContent('event', 'test-id')

      await loadRelatedContent()

      expect(relatedItems.value.length).toBeGreaterThan(0)
    })

    it('should load related content for polls', async () => {
      const { relatedItems, loadRelatedContent } = useRelatedContent('poll', 'test-id')

      await loadRelatedContent()

      expect(relatedItems.value.length).toBeGreaterThan(0)
    })

    it('should respect limit parameter', async () => {
      const { relatedItems, loadRelatedContent } = useRelatedContent('article', 'test-id')

      await loadRelatedContent(2)

      expect(relatedItems.value.length).toBeLessThanOrEqual(2)
    })

    it('should clear error before loading', async () => {
      const { error, loadRelatedContent } = useRelatedContent('article', 'test-id')
      error.value = 'Previous error'

      await loadRelatedContent()

      expect(error.value).toBeNull()
    })

    it('should return empty array for unknown content type', async () => {
      const { relatedItems, loadRelatedContent } = useRelatedContent('unknown', 'test-id')

      await loadRelatedContent()

      expect(relatedItems.value).toEqual([])
    })
  })

  describe('loadMixedContent', () => {
    it('should set loading state while fetching', async () => {
      const { isLoading, loadMixedContent } = useRelatedContent('article', 'test-id')

      const promise = loadMixedContent()
      expect(isLoading.value).toBe(true)

      await promise
      expect(isLoading.value).toBe(false)
    })

    it('should load mixed content from multiple types', async () => {
      const { relatedItems, loadMixedContent } = useRelatedContent('article', 'test-id')

      await loadMixedContent()

      expect(relatedItems.value.length).toBeGreaterThan(0)
      // Check that there are items of different types
      const types = new Set(relatedItems.value.map((item) => item.type))
      expect(types.size).toBeGreaterThan(1)
    })

    it('should respect limit parameter', async () => {
      const { relatedItems, loadMixedContent } = useRelatedContent('article', 'test-id')

      await loadMixedContent(3)

      expect(relatedItems.value.length).toBeLessThanOrEqual(3)
    })
  })

  describe('getContentTypeIcon', () => {
    it('should return correct icon for article', () => {
      const { getContentTypeIcon } = useRelatedContent('article', 'test-id')
      expect(getContentTypeIcon('article')).toBe('fas fa-file-alt')
    })

    it('should return correct icon for document', () => {
      const { getContentTypeIcon } = useRelatedContent('article', 'test-id')
      expect(getContentTypeIcon('document')).toBe('fas fa-file-pdf')
    })

    it('should return correct icon for media', () => {
      const { getContentTypeIcon } = useRelatedContent('article', 'test-id')
      expect(getContentTypeIcon('media')).toBe('fas fa-play-circle')
    })

    it('should return correct icon for course', () => {
      const { getContentTypeIcon } = useRelatedContent('article', 'test-id')
      expect(getContentTypeIcon('course')).toBe('fas fa-graduation-cap')
    })

    it('should return correct icon for event', () => {
      const { getContentTypeIcon } = useRelatedContent('article', 'test-id')
      expect(getContentTypeIcon('event')).toBe('fas fa-calendar-alt')
    })

    it('should return correct icon for poll', () => {
      const { getContentTypeIcon } = useRelatedContent('article', 'test-id')
      expect(getContentTypeIcon('poll')).toBe('fas fa-poll')
    })

    it('should return default icon for unknown type', () => {
      const { getContentTypeIcon } = useRelatedContent('article', 'test-id')
      expect(getContentTypeIcon('unknown' as any)).toBe('fas fa-file')
    })
  })

  describe('getContentTypeColor', () => {
    it('should return correct color for article', () => {
      const { getContentTypeColor } = useRelatedContent('article', 'test-id')
      expect(getContentTypeColor('article')).toContain('blue')
    })

    it('should return correct color for document', () => {
      const { getContentTypeColor } = useRelatedContent('article', 'test-id')
      expect(getContentTypeColor('document')).toContain('red')
    })

    it('should return correct color for media', () => {
      const { getContentTypeColor } = useRelatedContent('article', 'test-id')
      expect(getContentTypeColor('media')).toContain('purple')
    })

    it('should return correct color for course', () => {
      const { getContentTypeColor } = useRelatedContent('article', 'test-id')
      expect(getContentTypeColor('course')).toContain('green')
    })

    it('should return correct color for event', () => {
      const { getContentTypeColor } = useRelatedContent('article', 'test-id')
      expect(getContentTypeColor('event')).toContain('orange')
    })

    it('should return correct color for poll', () => {
      const { getContentTypeColor } = useRelatedContent('article', 'test-id')
      expect(getContentTypeColor('poll')).toContain('teal')
    })

    it('should return default color for unknown type', () => {
      const { getContentTypeColor } = useRelatedContent('article', 'test-id')
      expect(getContentTypeColor('unknown' as any)).toContain('gray')
    })
  })

  describe('getContentTypeLabel', () => {
    it('should return correct label for article', () => {
      const { getContentTypeLabel } = useRelatedContent('article', 'test-id')
      expect(getContentTypeLabel('article')).toBe('Article')
    })

    it('should return correct label for document', () => {
      const { getContentTypeLabel } = useRelatedContent('article', 'test-id')
      expect(getContentTypeLabel('document')).toBe('Document')
    })

    it('should return correct label for media', () => {
      const { getContentTypeLabel } = useRelatedContent('article', 'test-id')
      expect(getContentTypeLabel('media')).toBe('Media')
    })

    it('should return correct label for course', () => {
      const { getContentTypeLabel } = useRelatedContent('article', 'test-id')
      expect(getContentTypeLabel('course')).toBe('Course')
    })

    it('should return correct label for event', () => {
      const { getContentTypeLabel } = useRelatedContent('article', 'test-id')
      expect(getContentTypeLabel('event')).toBe('Event')
    })

    it('should return correct label for poll', () => {
      const { getContentTypeLabel } = useRelatedContent('article', 'test-id')
      expect(getContentTypeLabel('poll')).toBe('Poll')
    })

    it('should return type itself for unknown type', () => {
      const { getContentTypeLabel } = useRelatedContent('article', 'test-id')
      expect(getContentTypeLabel('custom' as any)).toBe('custom')
    })
  })

  describe('Related Items Structure', () => {
    it('should have valid structure for loaded items', async () => {
      const { relatedItems, loadRelatedContent } = useRelatedContent('article', 'test-id')

      await loadRelatedContent()

      relatedItems.value.forEach((item) => {
        expect(item.id).toBeDefined()
        expect(item.type).toBeDefined()
        expect(item.title).toBeDefined()
      })
    })

    it('should include metadata in items', async () => {
      const { relatedItems, loadRelatedContent } = useRelatedContent('article', 'test-id')

      await loadRelatedContent()

      const itemWithMetadata = relatedItems.value.find((item) => item.metadata)
      expect(itemWithMetadata).toBeDefined()
    })

    it('should include date in items', async () => {
      const { relatedItems, loadRelatedContent } = useRelatedContent('article', 'test-id')

      await loadRelatedContent()

      const itemWithDate = relatedItems.value.find((item) => item.date)
      expect(itemWithDate).toBeDefined()
    })
  })
})
