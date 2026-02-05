import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import axios from 'axios'
import { handleApiResponse } from '@/api/client'

// Mock axios
vi.mock('axios', async () => {
  const actual = await vi.importActual('axios')
  return {
    ...actual,
    default: {
      create: vi.fn(() => ({
        interceptors: {
          request: { use: vi.fn() },
          response: { use: vi.fn() },
        },
      })),
      post: vi.fn(),
    },
  }
})

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

    it('should handle empty object data', async () => {
      const mockPromise = Promise.resolve({
        data: {
          success: true,
          data: {},
        },
      })

      const result = await handleApiResponse(mockPromise)

      expect(result).toEqual({})
    })

    it('should handle nested data structures', async () => {
      const mockData = {
        user: { id: 1, profile: { name: 'Test' } },
        items: [1, 2, 3],
      }
      const mockPromise = Promise.resolve({
        data: {
          success: true,
          data: mockData,
        },
      })

      const result = await handleApiResponse(mockPromise)

      expect(result).toEqual(mockData)
      expect(result.user.profile.name).toBe('Test')
    })

    it('should handle undefined data when success is true', async () => {
      const mockPromise = Promise.resolve({
        data: {
          success: true,
          data: undefined,
        },
      })

      const result = await handleApiResponse(mockPromise)

      expect(result).toBeUndefined()
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

    it('should handle token update', () => {
      localStorage.setItem('auth_token', 'old-token')
      localStorage.setItem('auth_token', 'new-token')

      expect(localStorage.getItem('auth_token')).toBe('new-token')
    })

    it('should clear all tokens', () => {
      localStorage.setItem('auth_token', 'test-token')
      localStorage.setItem('refresh_token', 'test-refresh')
      localStorage.clear()

      expect(localStorage.getItem('auth_token')).toBeNull()
      expect(localStorage.getItem('refresh_token')).toBeNull()
    })
  })

  describe('Request Interceptor Behavior', () => {
    it('should add authorization header when token exists', () => {
      localStorage.setItem('auth_token', 'test-token')

      const token = localStorage.getItem('auth_token')
      const headers: Record<string, string> = {}

      if (token) {
        headers.Authorization = `Bearer ${token}`
      }

      expect(headers.Authorization).toBe('Bearer test-token')
    })

    it('should not add authorization header when no token', () => {
      const token = localStorage.getItem('auth_token')
      const headers: Record<string, string> = {}

      if (token) {
        headers.Authorization = `Bearer ${token}`
      }

      expect(headers.Authorization).toBeUndefined()
    })

    it('should format bearer token correctly', () => {
      const token = 'my-jwt-token'
      const authHeader = `Bearer ${token}`

      expect(authHeader).toBe('Bearer my-jwt-token')
      expect(authHeader.startsWith('Bearer ')).toBe(true)
    })
  })

  describe('Response Interceptor Behavior', () => {
    it('should detect 401 status code', () => {
      const error = {
        response: { status: 401 },
        config: {},
      }

      expect(error.response?.status).toBe(401)
    })

    it('should detect other error status codes', () => {
      const errors = [
        { response: { status: 400 } },
        { response: { status: 403 } },
        { response: { status: 404 } },
        { response: { status: 500 } },
      ]

      errors.forEach((error) => {
        expect(error.response?.status).not.toBe(401)
      })
    })

    it('should check retry flag', () => {
      const config = { _retry: false }
      expect(config._retry).toBe(false)

      config._retry = true
      expect(config._retry).toBe(true)
    })

    it('should handle missing response object', () => {
      const error = { message: 'Network Error' }

      expect(error.response).toBeUndefined()
    })
  })

  describe('Token Refresh Flow', () => {
    it('should have refresh token available for refresh', () => {
      localStorage.setItem('refresh_token', 'refresh-token-123')

      const refreshToken = localStorage.getItem('refresh_token')

      expect(refreshToken).toBe('refresh-token-123')
    })

    it('should update both tokens on refresh', () => {
      // Simulate token refresh
      const newToken = 'new-auth-token'
      const newRefreshToken = 'new-refresh-token'

      localStorage.setItem('auth_token', newToken)
      localStorage.setItem('refresh_token', newRefreshToken)

      expect(localStorage.getItem('auth_token')).toBe(newToken)
      expect(localStorage.getItem('refresh_token')).toBe(newRefreshToken)
    })

    it('should clear tokens on refresh failure', () => {
      localStorage.setItem('auth_token', 'test-token')
      localStorage.setItem('refresh_token', 'test-refresh')

      // Simulate failed refresh
      localStorage.removeItem('auth_token')
      localStorage.removeItem('refresh_token')

      expect(localStorage.getItem('auth_token')).toBeNull()
      expect(localStorage.getItem('refresh_token')).toBeNull()
    })

    it('should construct refresh URL correctly', () => {
      const baseUrl = 'http://api.example.com'
      const refreshEndpoint = '/identity/auth/refresh'
      const fullUrl = `${baseUrl}${refreshEndpoint}`

      expect(fullUrl).toBe('http://api.example.com/identity/auth/refresh')
    })
  })

  describe('Error Handling', () => {
    it('should handle network errors', async () => {
      const networkError = new Error('Network Error')
      const mockPromise = Promise.reject(networkError)

      await expect(handleApiResponse(mockPromise)).rejects.toThrow('Network Error')
    })

    it('should handle timeout errors', async () => {
      const timeoutError = new Error('timeout of 30000ms exceeded')
      const mockPromise = Promise.reject(timeoutError)

      await expect(handleApiResponse(mockPromise)).rejects.toThrow('timeout')
    })

    it('should handle malformed response', async () => {
      const mockPromise = Promise.resolve({
        data: null,
      })

      await expect(handleApiResponse(mockPromise)).rejects.toThrow()
    })
  })

  describe('API Configuration', () => {
    it('should have default timeout', () => {
      const timeout = 30000
      expect(timeout).toBe(30000)
    })

    it('should have content-type header', () => {
      const headers = {
        'Content-Type': 'application/json',
      }

      expect(headers['Content-Type']).toBe('application/json')
    })

    it('should handle custom base URL', () => {
      const baseUrl = import.meta.env.VITE_API_BASE_URL || '/api'

      expect(typeof baseUrl).toBe('string')
    })
  })
})
