import { describe, it, expect, vi, beforeEach } from 'vitest'
import { authApi } from '@/api/auth'
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

describe('Auth API', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('login', () => {
    it('should send login request with credentials', async () => {
      const mockResponse = {
        data: {
          token: 'test-token',
          refreshToken: 'test-refresh',
          user: { id: '1', email: 'test@example.com' },
        },
      }
      vi.mocked(apiClient.post).mockResolvedValue(mockResponse)

      const result = await authApi.login({
        email: 'test@example.com',
        password: 'password123',
      })

      expect(apiClient.post).toHaveBeenCalledWith('/identity/auth/login', {
        email: 'test@example.com',
        password: 'password123',
      })
      expect(result).toEqual(mockResponse.data)
    })

    it('should propagate login error', async () => {
      vi.mocked(apiClient.post).mockRejectedValue(new Error('Invalid credentials'))

      await expect(
        authApi.login({ email: 'test@example.com', password: 'wrong' })
      ).rejects.toThrow('Invalid credentials')
    })
  })

  describe('logout', () => {
    it('should send logout request', async () => {
      vi.mocked(apiClient.post).mockResolvedValue({ data: {} })

      await authApi.logout()

      expect(apiClient.post).toHaveBeenCalledWith('/identity/auth/logout')
    })
  })

  describe('getCurrentUser', () => {
    it('should fetch current user', async () => {
      const mockUser = {
        id: '1',
        email: 'test@example.com',
        displayName: 'Test User',
      }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockUser })

      const result = await authApi.getCurrentUser()

      expect(apiClient.get).toHaveBeenCalledWith('/identity/users/me')
      expect(result).toEqual(mockUser)
    })
  })

  describe('updateProfile', () => {
    it('should update user profile', async () => {
      const updateData = { displayName: 'New Name' }
      const mockUser = { id: '1', displayName: 'New Name' }
      vi.mocked(apiClient.put).mockResolvedValue({ data: mockUser })

      const result = await authApi.updateProfile(updateData)

      expect(apiClient.put).toHaveBeenCalledWith('/identity/users/me', updateData)
      expect(result).toEqual(mockUser)
    })
  })

  describe('getSSOConfig', () => {
    it('should fetch SSO configuration', async () => {
      const mockConfig = {
        enabled: true,
        provider: 'azure',
        loginUrl: 'https://login.example.com',
      }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockConfig })

      const result = await authApi.getSSOConfig()

      expect(apiClient.get).toHaveBeenCalledWith('/identity/auth/sso-config')
      expect(result).toEqual(mockConfig)
    })
  })

  describe('handleSSOCallback', () => {
    it('should handle SSO callback with code', async () => {
      const mockResponse = {
        token: 'sso-token',
        refreshToken: 'sso-refresh',
        user: { id: '1' },
      }
      vi.mocked(apiClient.post).mockResolvedValue({ data: mockResponse })

      const result = await authApi.handleSSOCallback('auth-code-123')

      expect(apiClient.post).toHaveBeenCalledWith('/identity/auth/sso-callback', {
        code: 'auth-code-123',
      })
      expect(result).toEqual(mockResponse)
    })
  })

  describe('refreshToken', () => {
    it('should refresh authentication token', async () => {
      const mockResponse = {
        token: 'new-token',
        refreshToken: 'new-refresh-token',
      }
      vi.mocked(apiClient.post).mockResolvedValue({ data: mockResponse })

      const result = await authApi.refreshToken('old-refresh-token')

      expect(apiClient.post).toHaveBeenCalledWith('/identity/auth/refresh', {
        refreshToken: 'old-refresh-token',
      })
      expect(result).toEqual(mockResponse)
    })
  })

  describe('changePassword', () => {
    it('should change password', async () => {
      vi.mocked(apiClient.put).mockResolvedValue({ data: {} })

      await authApi.changePassword('oldPass123', 'newPass456')

      expect(apiClient.put).toHaveBeenCalledWith('/identity/auth/change-password', {
        currentPassword: 'oldPass123',
        newPassword: 'newPass456',
      })
    })
  })

  describe('requestPasswordReset', () => {
    it('should request password reset', async () => {
      vi.mocked(apiClient.post).mockResolvedValue({ data: {} })

      await authApi.requestPasswordReset('user@example.com')

      expect(apiClient.post).toHaveBeenCalledWith('/identity/auth/forgot-password', {
        email: 'user@example.com',
      })
    })
  })

  describe('resetPassword', () => {
    it('should reset password with token', async () => {
      vi.mocked(apiClient.post).mockResolvedValue({ data: {} })

      await authApi.resetPassword('reset-token', 'newPassword123')

      expect(apiClient.post).toHaveBeenCalledWith('/identity/auth/reset-password', {
        token: 'reset-token',
        newPassword: 'newPassword123',
      })
    })
  })
})
