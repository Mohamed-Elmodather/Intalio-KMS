import apiClient from './client'
import type { SearchResult, SearchFilters, PaginatedResponse } from '@/types'

const SEARCH_BASE = '/search'
const AI_BASE = '/ai'

export interface SearchParams extends SearchFilters {
  query: string
  page?: number
  pageSize?: number
}

export const searchApi = {
  // Full-text search
  async search(params: SearchParams): Promise<PaginatedResponse<SearchResult>> {
    const response = await apiClient.get<PaginatedResponse<SearchResult>>(`${SEARCH_BASE}/search`, {
      params,
    })
    return response.data
  },

  // Get search suggestions
  async getSuggestions(query: string): Promise<string[]> {
    const response = await apiClient.get<string[]>(`${SEARCH_BASE}/suggestions`, {
      params: { query },
    })
    return response.data
  },

  // Semantic/AI search
  async semanticSearch(query: string, filters?: SearchFilters): Promise<SearchResult[]> {
    const response = await apiClient.post<SearchResult[]>(`${AI_BASE}/search`, {
      query,
      ...filters,
    })
    return response.data
  },

  // Get popular searches
  async getPopularSearches(): Promise<string[]> {
    const response = await apiClient.get<string[]>(`${SEARCH_BASE}/popular`)
    return response.data
  },

  // Get recent searches for current user
  async getRecentSearches(): Promise<string[]> {
    const response = await apiClient.get<string[]>(`${SEARCH_BASE}/recent`)
    return response.data
  },
}

export default searchApi
