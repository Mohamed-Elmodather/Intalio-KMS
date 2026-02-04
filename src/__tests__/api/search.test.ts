import { describe, it, expect, vi, beforeEach } from 'vitest'
import { searchApi } from '@/api/search'
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

describe('Search API', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('search', () => {
    it('should perform search with query only', async () => {
      const mockResponse = {
        items: [{ id: '1', title: 'Result 1' }],
        totalCount: 1,
        page: 1,
        pageSize: 10,
      }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockResponse })

      const result = await searchApi.search({ query: 'test' })

      expect(apiClient.get).toHaveBeenCalledWith('/search/search', {
        params: { query: 'test' },
      })
      expect(result).toEqual(mockResponse)
    })

    it('should perform search with pagination', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({
        data: { items: [], totalCount: 0 },
      })

      await searchApi.search({ query: 'test', page: 2, pageSize: 20 })

      expect(apiClient.get).toHaveBeenCalledWith('/search/search', {
        params: { query: 'test', page: 2, pageSize: 20 },
      })
    })

    it('should perform search with filters', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({
        data: { items: [], totalCount: 0 },
      })

      await searchApi.search({
        query: 'test',
        contentTypes: ['article', 'document'],
        dateFrom: '2024-01-01',
        dateTo: '2024-12-31',
      } as any)

      expect(apiClient.get).toHaveBeenCalledWith('/search/search', {
        params: expect.objectContaining({ query: 'test' }),
      })
    })
  })

  describe('getSuggestions', () => {
    it('should get search suggestions', async () => {
      const mockSuggestions = ['suggestion 1', 'suggestion 2', 'suggestion 3']
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockSuggestions })

      const result = await searchApi.getSuggestions('test')

      expect(apiClient.get).toHaveBeenCalledWith('/search/suggestions', {
        params: { query: 'test' },
      })
      expect(result).toEqual(mockSuggestions)
    })

    it('should return empty array for no suggestions', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: [] })

      const result = await searchApi.getSuggestions('xyz123')

      expect(result).toEqual([])
    })
  })

  describe('semanticSearch', () => {
    it('should perform semantic search', async () => {
      const mockResults = [
        { id: '1', title: 'Semantic Result', relevanceScore: 0.95 },
      ]
      vi.mocked(apiClient.post).mockResolvedValue({ data: mockResults })

      const result = await searchApi.semanticSearch('what is machine learning')

      expect(apiClient.post).toHaveBeenCalledWith('/ai/search', {
        query: 'what is machine learning',
      })
      expect(result).toEqual(mockResults)
    })

    it('should perform semantic search with filters', async () => {
      vi.mocked(apiClient.post).mockResolvedValue({ data: [] })

      const filters = { contentTypes: ['article'] }
      await searchApi.semanticSearch('test query', filters as any)

      expect(apiClient.post).toHaveBeenCalledWith('/ai/search', {
        query: 'test query',
        ...filters,
      })
    })
  })

  describe('getPopularSearches', () => {
    it('should get popular searches', async () => {
      const mockPopular = ['trending topic', 'popular search', 'hot topic']
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockPopular })

      const result = await searchApi.getPopularSearches()

      expect(apiClient.get).toHaveBeenCalledWith('/search/popular')
      expect(result).toEqual(mockPopular)
    })
  })

  describe('getRecentSearches', () => {
    it('should get recent searches for current user', async () => {
      const mockRecent = ['my search 1', 'my search 2']
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockRecent })

      const result = await searchApi.getRecentSearches()

      expect(apiClient.get).toHaveBeenCalledWith('/search/recent')
      expect(result).toEqual(mockRecent)
    })
  })
})
