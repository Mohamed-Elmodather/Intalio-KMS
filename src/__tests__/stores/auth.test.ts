import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '@/stores/auth'
import { authApi } from '@/api'

// Mock the API module
vi.mock('@/api', () => ({
  authApi: {
    login: vi.fn(),
    logout: vi.fn(),
    getCurrentUser: vi.fn(),
    updateProfile: vi.fn(),
    getSSOConfig: vi.fn(),
    handleSSOCallback: vi.fn(),
  },
}))

describe('Auth Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
    localStorage.clear()
  })

  describe('Initial State', () => {
    it('should have null token when localStorage is empty', () => {
      const store = useAuthStore()
      expect(store.token).toBeNull()
    })

    it('should have null user initially', () => {
      const store = useAuthStore()
      expect(store.user).toBeNull()
    })

    it('should not be authenticated initially', () => {
      const store = useAuthStore()
      expect(store.isAuthenticated).toBe(false)
    })

    it('should have default role as viewer', () => {
      const store = useAuthStore()
      expect(store.userRole).toBe('viewer')
    })
  })

  describe('loginWithCredentials', () => {
    it('should login successfully with valid credentials', async () => {
      const mockResponse = {
        token: 'test-token',
        refreshToken: 'test-refresh-token',
        user: {
          id: '1',
          email: 'test@example.com',
          displayName: 'Test User',
          role: 'admin',
        },
      }

      vi.mocked(authApi.login).mockResolvedValue(mockResponse)

      const store = useAuthStore()
      const result = await store.loginWithCredentials({
        email: 'test@example.com',
        password: 'password123',
      })

      expect(result).toBe(true)
      expect(store.token).toBe('test-token')
      expect(store.user?.email).toBe('test@example.com')
      expect(store.isAuthenticated).toBe(true)
      expect(localStorage.getItem('auth_token')).toBe('test-token')
    })

    it('should handle login failure', async () => {
      vi.mocked(authApi.login).mockRejectedValue(new Error('Invalid credentials'))

      const store = useAuthStore()
      const result = await store.loginWithCredentials({
        email: 'test@example.com',
        password: 'wrong-password',
      })

      expect(result).toBe(false)
      expect(store.error).toBe('Invalid credentials')
      expect(store.isAuthenticated).toBe(false)
    })

    it('should set loading state during login', async () => {
      vi.mocked(authApi.login).mockImplementation(
        () => new Promise((resolve) => setTimeout(resolve, 100))
      )

      const store = useAuthStore()
      const loginPromise = store.loginWithCredentials({
        email: 'test@example.com',
        password: 'password123',
      })

      expect(store.isLoading).toBe(true)
      await loginPromise
      expect(store.isLoading).toBe(false)
    })
  })

  describe('logout', () => {
    it('should clear auth state on logout', async () => {
      const store = useAuthStore()

      // Set up authenticated state
      store.token = 'test-token'
      store.user = { id: '1', email: 'test@example.com' } as any
      localStorage.setItem('auth_token', 'test-token')

      vi.mocked(authApi.logout).mockResolvedValue(undefined)

      await store.logout()

      expect(store.token).toBeNull()
      expect(store.user).toBeNull()
      expect(store.isAuthenticated).toBe(false)
      expect(localStorage.getItem('auth_token')).toBeNull()
    })

    it('should clear auth even if API call fails', async () => {
      const store = useAuthStore()
      store.token = 'test-token'
      localStorage.setItem('auth_token', 'test-token')

      vi.mocked(authApi.logout).mockRejectedValue(new Error('Network error'))

      await store.logout()

      expect(store.token).toBeNull()
      expect(localStorage.getItem('auth_token')).toBeNull()
    })
  })

  describe('checkAuth', () => {
    it('should return false if no token exists', async () => {
      const store = useAuthStore()
      const result = await store.checkAuth()
      expect(result).toBe(false)
    })

    it('should validate token and fetch user', async () => {
      const mockUser = {
        id: '1',
        email: 'test@example.com',
        displayName: 'Test User',
        role: 'admin',
      }

      vi.mocked(authApi.getCurrentUser).mockResolvedValue(mockUser)

      const store = useAuthStore()
      store.token = 'valid-token'

      const result = await store.checkAuth()

      expect(result).toBe(true)
      expect(store.user).toEqual(mockUser)
    })

    it('should clear auth if token is invalid', async () => {
      vi.mocked(authApi.getCurrentUser).mockRejectedValue(new Error('Unauthorized'))

      const store = useAuthStore()
      store.token = 'invalid-token'
      localStorage.setItem('auth_token', 'invalid-token')

      const result = await store.checkAuth()

      expect(result).toBe(false)
      expect(store.token).toBeNull()
      expect(localStorage.getItem('auth_token')).toBeNull()
    })
  })

  describe('Computed Properties', () => {
    it('should return correct userDisplayName', () => {
      const store = useAuthStore()

      // No user
      expect(store.userDisplayName).toBe('User')

      // User with displayName
      store.user = { displayName: 'John Doe', email: 'john@example.com' } as any
      expect(store.userDisplayName).toBe('John Doe')

      // User without displayName
      store.user = { email: 'jane@example.com' } as any
      expect(store.userDisplayName).toBe('jane@example.com')
    })

    it('should correctly identify admin users', () => {
      const store = useAuthStore()

      expect(store.isAdmin).toBe(false)

      store.user = { role: 'viewer' } as any
      expect(store.isAdmin).toBe(false)

      store.user = { role: 'admin' } as any
      expect(store.isAdmin).toBe(true)
    })
  })

  describe('updateProfile', () => {
    it('should update user profile successfully', async () => {
      const updatedUser = {
        id: '1',
        email: 'test@example.com',
        displayName: 'Updated Name',
      }

      vi.mocked(authApi.updateProfile).mockResolvedValue(updatedUser)

      const store = useAuthStore()
      store.user = { id: '1', email: 'test@example.com', displayName: 'Old Name' } as any

      const result = await store.updateProfile({ displayName: 'Updated Name' })

      expect(result).toBe(true)
      expect(store.user?.displayName).toBe('Updated Name')
    })

    it('should return false on update failure', async () => {
      vi.mocked(authApi.updateProfile).mockRejectedValue(new Error('Update failed'))

      const store = useAuthStore()
      const result = await store.updateProfile({ displayName: 'New Name' })

      expect(result).toBe(false)
    })
  })
})
