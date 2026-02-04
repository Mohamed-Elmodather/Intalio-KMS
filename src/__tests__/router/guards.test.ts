import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { authGuard, titleGuard } from '@/router/guards'
import type { RouteLocationNormalized } from 'vue-router'

// Mock the auth store module
vi.mock('@/stores/auth', () => ({
  useAuthStore: vi.fn(() => ({
    isAuthenticated: false,
    token: null,
    userRole: 'viewer',
    checkAuth: vi.fn().mockResolvedValue(false),
  })),
}))

import { useAuthStore } from '@/stores/auth'

describe('Router Guards', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('authGuard', () => {
    const createMockRoute = (
      options: Partial<RouteLocationNormalized> = {}
    ): RouteLocationNormalized =>
      ({
        name: 'TestRoute',
        fullPath: '/test',
        path: '/test',
        meta: {},
        params: {},
        query: {},
        hash: '',
        matched: [],
        redirectedFrom: undefined,
        ...options,
      } as RouteLocationNormalized)

    it('should allow access to non-auth routes', async () => {
      const to = createMockRoute({ meta: { requiresAuth: false } })
      const from = createMockRoute()
      const next = vi.fn()

      await authGuard(to, from, next)

      expect(next).toHaveBeenCalledWith()
    })

    it('should redirect authenticated user from login to dashboard', async () => {
      vi.mocked(useAuthStore).mockReturnValue({
        isAuthenticated: true,
        token: 'valid-token',
        userRole: 'viewer',
        checkAuth: vi.fn().mockResolvedValue(true),
      } as any)

      const to = createMockRoute({
        name: 'Login',
        meta: { requiresAuth: false },
      })
      const from = createMockRoute()
      const next = vi.fn()

      await authGuard(to, from, next)

      expect(next).toHaveBeenCalledWith({ name: 'Dashboard' })
    })

    it('should redirect unauthenticated user to login', async () => {
      vi.mocked(useAuthStore).mockReturnValue({
        isAuthenticated: false,
        token: null,
        userRole: 'viewer',
        checkAuth: vi.fn().mockResolvedValue(false),
      } as any)

      const to = createMockRoute({
        meta: { requiresAuth: true },
        fullPath: '/protected',
      })
      const from = createMockRoute()
      const next = vi.fn()

      await authGuard(to, from, next)

      expect(next).toHaveBeenCalledWith({
        name: 'Login',
        query: { redirect: '/protected' },
      })
    })

    it('should validate token if token exists but not authenticated', async () => {
      const mockCheckAuth = vi.fn().mockResolvedValue(true)
      vi.mocked(useAuthStore).mockReturnValue({
        isAuthenticated: false,
        token: 'existing-token',
        userRole: 'viewer',
        checkAuth: mockCheckAuth,
      } as any)

      const to = createMockRoute({ meta: {} })
      const from = createMockRoute()
      const next = vi.fn()

      await authGuard(to, from, next)

      expect(mockCheckAuth).toHaveBeenCalled()
    })

    it('should redirect to login if token validation fails', async () => {
      const mockCheckAuth = vi.fn().mockResolvedValue(false)
      vi.mocked(useAuthStore).mockReturnValue({
        isAuthenticated: false,
        token: 'invalid-token',
        userRole: 'viewer',
        checkAuth: mockCheckAuth,
      } as any)

      const to = createMockRoute({
        meta: {},
        fullPath: '/protected',
      })
      const from = createMockRoute()
      const next = vi.fn()

      await authGuard(to, from, next)

      expect(next).toHaveBeenCalledWith({
        name: 'Login',
        query: { redirect: '/protected' },
      })
    })

    it('should check role-based access for authenticated user', async () => {
      vi.mocked(useAuthStore).mockReturnValue({
        isAuthenticated: true,
        token: 'valid-token',
        userRole: 'viewer',
        checkAuth: vi.fn().mockResolvedValue(true),
      } as any)

      const to = createMockRoute({
        meta: { roles: ['admin'] },
      })
      const from = createMockRoute()
      const next = vi.fn()

      await authGuard(to, from, next)

      // User is 'viewer' but route requires 'admin'
      expect(next).toHaveBeenCalledWith({ name: 'Dashboard' })
    })

    it('should allow access when user has required role', async () => {
      vi.mocked(useAuthStore).mockReturnValue({
        isAuthenticated: true,
        token: 'valid-token',
        userRole: 'admin',
        checkAuth: vi.fn().mockResolvedValue(true),
      } as any)

      const to = createMockRoute({
        meta: { roles: ['admin', 'editor'] },
      })
      const from = createMockRoute()
      const next = vi.fn()

      await authGuard(to, from, next)

      expect(next).toHaveBeenCalledWith()
    })

    it('should allow access when no roles specified', async () => {
      vi.mocked(useAuthStore).mockReturnValue({
        isAuthenticated: true,
        token: 'valid-token',
        userRole: 'viewer',
        checkAuth: vi.fn().mockResolvedValue(true),
      } as any)

      const to = createMockRoute({
        meta: {},
      })
      const from = createMockRoute()
      const next = vi.fn()

      await authGuard(to, from, next)

      expect(next).toHaveBeenCalledWith()
    })
  })

  describe('titleGuard', () => {
    it('should set page title from route meta', () => {
      const to = {
        meta: { title: 'Settings' },
      } as RouteLocationNormalized

      titleGuard(to)

      expect(document.title).toContain('Settings')
    })

    it('should set app title when no page title', () => {
      const to = {
        meta: {},
      } as RouteLocationNormalized

      titleGuard(to)

      expect(document.title).toBeTruthy()
    })

    it('should combine page title and app title', () => {
      const to = {
        meta: { title: 'My Page' },
      } as RouteLocationNormalized

      titleGuard(to)

      expect(document.title).toContain('My Page')
      expect(document.title).toContain('|')
    })
  })
})
