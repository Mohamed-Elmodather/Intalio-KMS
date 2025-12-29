import apiClient from './client'
import type { Article, Category, Tag, PaginatedResponse, PaginationParams } from '@/types'

const CONTENT_BASE = '/content'

export interface ArticleFilters extends PaginationParams {
  categoryId?: string
  tagId?: string
  authorId?: string
  status?: string
}

export const articlesApi = {
  // Get all articles with pagination and filters
  async getArticles(filters?: ArticleFilters): Promise<PaginatedResponse<Article>> {
    const response = await apiClient.get<PaginatedResponse<Article>>(`${CONTENT_BASE}/articles`, {
      params: filters,
    })
    return response.data
  },

  // Get single article by ID or slug
  async getArticle(idOrSlug: string): Promise<Article> {
    const response = await apiClient.get<Article>(`${CONTENT_BASE}/articles/${idOrSlug}`)
    return response.data
  },

  // Create new article
  async createArticle(data: Partial<Article>): Promise<Article> {
    const response = await apiClient.post<Article>(`${CONTENT_BASE}/articles`, data)
    return response.data
  },

  // Update article
  async updateArticle(id: string, data: Partial<Article>): Promise<Article> {
    const response = await apiClient.put<Article>(`${CONTENT_BASE}/articles/${id}`, data)
    return response.data
  },

  // Delete article
  async deleteArticle(id: string): Promise<void> {
    await apiClient.delete(`${CONTENT_BASE}/articles/${id}`)
  },

  // Publish article
  async publishArticle(id: string): Promise<Article> {
    const response = await apiClient.post<Article>(`${CONTENT_BASE}/articles/${id}/publish`)
    return response.data
  },

  // Like/unlike article
  async toggleLike(id: string): Promise<{ liked: boolean; likeCount: number }> {
    const response = await apiClient.post<{ liked: boolean; likeCount: number }>(
      `${CONTENT_BASE}/articles/${id}/like`
    )
    return response.data
  },

  // Bookmark/unbookmark article
  async toggleBookmark(id: string): Promise<{ bookmarked: boolean }> {
    const response = await apiClient.post<{ bookmarked: boolean }>(
      `${CONTENT_BASE}/articles/${id}/bookmark`
    )
    return response.data
  },

  // Get categories
  async getCategories(): Promise<Category[]> {
    const response = await apiClient.get<Category[]>(`${CONTENT_BASE}/categories`)
    return response.data
  },

  // Get tags
  async getTags(): Promise<Tag[]> {
    const response = await apiClient.get<Tag[]>(`${CONTENT_BASE}/tags`)
    return response.data
  },

  // Get featured articles
  async getFeaturedArticles(limit?: number): Promise<Article[]> {
    const response = await apiClient.get<Article[]>(`${CONTENT_BASE}/articles/featured`, {
      params: { limit },
    })
    return response.data
  },

  // Get recent articles
  async getRecentArticles(limit?: number): Promise<Article[]> {
    const response = await apiClient.get<Article[]>(`${CONTENT_BASE}/articles/recent`, {
      params: { limit },
    })
    return response.data
  },
}

export default articlesApi
