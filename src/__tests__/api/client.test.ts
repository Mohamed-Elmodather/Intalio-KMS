import { describe, it, expect, vi, beforeEach } from 'vitest'
import { handleApiResponse } from '@/api/client'

describe('API Client', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    localStorage.clear()
  })

  describe('handleApiResponse', () => {
    it('should return data when response is successful', async () => {
      const mockData = { id: 1, name: 'Test' }
      const mockPromise = Promise.resolve({
        data: {
          success: true,
          data: mockData,
        },
      })

      const result = await handleApiResponse(mockPromise)

      expect(result).toEqual(mockData)
    })

    it('should throw error when response is not successful', async () => {
      const mockPromise = Promise.resolve({
        data: {
          success: false,
          message: 'Custom error message',
          data: null,
        },
      })

      await expect(handleApiResponse(mockPromise)).rejects.toThrow(
        'Custom error message'
      )
    })

    it('should throw generic error when no message provided', async () => {
      const mockPromise = Promise.resolve({
        data: {
          success: false,
          data: null,
        },
      })

      await expect(handleApiResponse(mockPromise)).rejects.toThrow(
        'API request failed'
      )
    })

    it('should handle array data', async () => {
      const mockData = [{ id: 1 }, { id: 2 }]
      const mockPromise = Promise.resolve({
        data: {
          success: true,
          data: mockData,
        },
      })

      const result = await handleApiResponse(mockPromise)

      expect(result).toEqual(mockData)
      expect(Array.isArray(result)).toBe(true)
    })

    it('should handle null data when success is true', async () => {
      const mockPromise = Promise.resolve({
        data: {
          success: true,
          data: null,
        },
      })

      const result = await handleApiResponse(mockPromise)

      expect(result).toBeNull()
    })

    it('should propagate promise rejection', async () => {
      const mockPromise = Promise.reject(new Error('Network error'))

      await expect(handleApiResponse(mockPromise)).rejects.toThrow('Network error')
    })
  })

  describe('Auth Token Handling', () => {
    it('should store auth token in localStorage', () => {
      localStorage.setItem('auth_token', 'test-token')

      expect(localStorage.getItem('auth_token')).toBe('test-token')
    })

    it('should remove auth token from localStorage', () => {
      localStorage.setItem('auth_token', 'test-token')
      localStorage.removeItem('auth_token')

      expect(localStorage.getItem('auth_token')).toBeNull()
    })

    it('should store refresh token in localStorage', () => {
      localStorage.setItem('refresh_token', 'test-refresh-token')

      expect(localStorage.getItem('refresh_token')).toBe('test-refresh-token')
    })
  })
})
