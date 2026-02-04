import { describe, it, expect, vi, beforeEach } from 'vitest'
import { articlesApi } from '@/api/articles'
import apiClient from '@/api/client'

// Mock the API client
vi.mock('@/api/client', () => ({
  default: {
    post: vi.fn(),
    get: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}))

describe('Articles API', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('getArticles', () => {
    it('should fetch articles without filters', async () => {
      const mockResponse = {
        items: [{ id: '1', title: 'Article 1' }],
        totalCount: 1,
        page: 1,
        pageSize: 10,
      }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockResponse })

      const result = await articlesApi.getArticles()

      expect(apiClient.get).toHaveBeenCalledWith('/content/articles', {
        params: undefined,
      })
      expect(result).toEqual(mockResponse)
    })

    it('should fetch articles with filters', async () => {
      const mockResponse = { items: [], totalCount: 0 }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockResponse })

      const filters = {
        categoryId: 'cat-1',
        tagId: 'tag-1',
        page: 2,
        pageSize: 20,
      }
      await articlesApi.getArticles(filters)

      expect(apiClient.get).toHaveBeenCalledWith('/content/articles', {
        params: filters,
      })
    })

    it('should filter by author', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: { items: [] } })

      await articlesApi.getArticles({ authorId: 'author-123' })

      expect(apiClient.get).toHaveBeenCalledWith('/content/articles', {
        params: { authorId: 'author-123' },
      })
    })

    it('should filter by status', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: { items: [] } })

      await articlesApi.getArticles({ status: 'published' })

      expect(apiClient.get).toHaveBeenCalledWith('/content/articles', {
        params: { status: 'published' },
      })
    })
  })

  describe('getArticle', () => {
    it('should fetch single article by ID', async () => {
      const mockArticle = { id: '123', title: 'Test Article' }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockArticle })

      const result = await articlesApi.getArticle('123')

      expect(apiClient.get).toHaveBeenCalledWith('/content/articles/123')
      expect(result).toEqual(mockArticle)
    })

    it('should fetch single article by slug', async () => {
      const mockArticle = { id: '1', slug: 'test-article', title: 'Test Article' }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockArticle })

      const result = await articlesApi.getArticle('test-article')

      expect(apiClient.get).toHaveBeenCalledWith('/content/articles/test-article')
      expect(result).toEqual(mockArticle)
    })
  })

  describe('createArticle', () => {
    it('should create new article', async () => {
      const articleData = { title: 'New Article', content: 'Content here' }
      const mockResponse = { id: '1', ...articleData }
      vi.mocked(apiClient.post).mockResolvedValue({ data: mockResponse })

      const result = await articlesApi.createArticle(articleData)

      expect(apiClient.post).toHaveBeenCalledWith('/content/articles', articleData)
      expect(result).toEqual(mockResponse)
    })
  })

  describe('updateArticle', () => {
    it('should update existing article', async () => {
      const updateData = { title: 'Updated Title' }
      const mockResponse = { id: '123', title: 'Updated Title' }
      vi.mocked(apiClient.put).mockResolvedValue({ data: mockResponse })

      const result = await articlesApi.updateArticle('123', updateData)

      expect(apiClient.put).toHaveBeenCalledWith('/content/articles/123', updateData)
      expect(result).toEqual(mockResponse)
    })
  })

  describe('deleteArticle', () => {
    it('should delete article', async () => {
      vi.mocked(apiClient.delete).mockResolvedValue({ data: {} })

      await articlesApi.deleteArticle('123')

      expect(apiClient.delete).toHaveBeenCalledWith('/content/articles/123')
    })
  })

  describe('publishArticle', () => {
    it('should publish article', async () => {
      const mockArticle = { id: '123', status: 'published' }
      vi.mocked(apiClient.post).mockResolvedValue({ data: mockArticle })

      const result = await articlesApi.publishArticle('123')

      expect(apiClient.post).toHaveBeenCalledWith('/content/articles/123/publish')
      expect(result.status).toBe('published')
    })
  })

  describe('toggleLike', () => {
    it('should toggle article like', async () => {
      const mockResponse = { liked: true, likeCount: 10 }
      vi.mocked(apiClient.post).mockResolvedValue({ data: mockResponse })

      const result = await articlesApi.toggleLike('123')

      expect(apiClient.post).toHaveBeenCalledWith('/content/articles/123/like')
      expect(result).toEqual(mockResponse)
    })
  })

  describe('toggleBookmark', () => {
    it('should toggle article bookmark', async () => {
      const mockResponse = { bookmarked: true }
      vi.mocked(apiClient.post).mockResolvedValue({ data: mockResponse })

      const result = await articlesApi.toggleBookmark('123')

      expect(apiClient.post).toHaveBeenCalledWith('/content/articles/123/bookmark')
      expect(result).toEqual(mockResponse)
    })
  })

  describe('getCategories', () => {
    it('should fetch all categories', async () => {
      const mockCategories = [
        { id: '1', name: 'News' },
        { id: '2', name: 'Sports' },
      ]
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockCategories })

      const result = await articlesApi.getCategories()

      expect(apiClient.get).toHaveBeenCalledWith('/content/categories')
      expect(result).toEqual(mockCategories)
    })
  })

  describe('getTags', () => {
    it('should fetch all tags', async () => {
      const mockTags = [
        { id: '1', name: 'JavaScript' },
        { id: '2', name: 'Vue' },
      ]
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockTags })

      const result = await articlesApi.getTags()

      expect(apiClient.get).toHaveBeenCalledWith('/content/tags')
      expect(result).toEqual(mockTags)
    })
  })

  describe('getFeaturedArticles', () => {
    it('should fetch featured articles without limit', async () => {
      const mockArticles = [{ id: '1', featured: true }]
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockArticles })

      const result = await articlesApi.getFeaturedArticles()

      expect(apiClient.get).toHaveBeenCalledWith('/content/articles/featured', {
        params: { limit: undefined },
      })
      expect(result).toEqual(mockArticles)
    })

    it('should fetch featured articles with limit', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: [] })

      await articlesApi.getFeaturedArticles(5)

      expect(apiClient.get).toHaveBeenCalledWith('/content/articles/featured', {
        params: { limit: 5 },
      })
    })
  })

  describe('getRecentArticles', () => {
    it('should fetch recent articles without limit', async () => {
      const mockArticles = [{ id: '1', createdAt: new Date().toISOString() }]
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockArticles })

      const result = await articlesApi.getRecentArticles()

      expect(apiClient.get).toHaveBeenCalledWith('/content/articles/recent', {
        params: { limit: undefined },
      })
      expect(result).toEqual(mockArticles)
    })

    it('should fetch recent articles with limit', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: [] })

      await articlesApi.getRecentArticles(10)

      expect(apiClient.get).toHaveBeenCalledWith('/content/articles/recent', {
        params: { limit: 10 },
      })
    })
  })
})
