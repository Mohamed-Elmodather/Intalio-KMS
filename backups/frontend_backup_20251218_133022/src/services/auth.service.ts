import { apiService } from './api.service'
import { mockUsers } from './mock.service'
import type { AuthResponse, AuthTokens } from '@/types'

// Check if we're in development mode
const isDev = import.meta.env.DEV || import.meta.env.VITE_USE_MOCK_DATA === 'true'

class AuthService {
  /**
   * Development login - bypasses real authentication
   * Uses mock user data for testing
   */
  async devLogin(email?: string): Promise<AuthResponse> {
    // Find mock user by email or use first user
    const mockUser = email
      ? mockUsers.find(u => u.email === email) || mockUsers[0]
      : mockUsers[0]

    // Generate fake tokens
    const fakeTokens: AuthTokens = {
      accessToken: 'dev-access-token-' + Date.now(),
      refreshToken: 'dev-refresh-token-' + Date.now(),
      expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString() // 24 hours
    }

    return {
      user: mockUser,
      tokens: fakeTokens
    }
  }

  async login(email: string, password: string): Promise<AuthResponse> {
    // In development, use dev login
    if (isDev) {
      console.log('[Auth] Using development login')
      return this.devLogin(email)
    }

    const response = await apiService.post<AuthResponse>('/identity/auth/login', {
      email,
      password
    })

    if (!response.success || !response.data) {
      throw new Error(response.message || 'Login failed')
    }

    return response.data
  }

  async redirectToSSO(): Promise<void> {
    // Get SSO configuration
    const response = await apiService.get<{ authUrl: string }>('/identity/auth/sso-config')

    if (!response.success || !response.data) {
      throw new Error('Failed to get SSO configuration')
    }

    // Redirect to Azure AD
    window.location.href = response.data.authUrl
  }

  async handleSSOCallback(code: string): Promise<AuthResponse> {
    const response = await apiService.post<AuthResponse>('/identity/auth/sso-callback', {
      code
    })

    if (!response.success || !response.data) {
      throw new Error(response.message || 'SSO authentication failed')
    }

    return response.data
  }

  async refreshToken(refreshToken: string): Promise<{ tokens: AuthTokens }> {
    const response = await apiService.post<{ tokens: AuthTokens }>('/identity/auth/refresh', {
      refreshToken
    })

    if (!response.success || !response.data) {
      throw new Error('Token refresh failed')
    }

    return response.data
  }

  async logout(): Promise<void> {
    try {
      await apiService.post('/identity/auth/logout')
    } catch {
      // Ignore errors on logout
    }
  }

  async getCurrentUser(): Promise<AuthResponse['user']> {
    const response = await apiService.get<AuthResponse['user']>('/identity/users/me')

    if (!response.success || !response.data) {
      throw new Error('Failed to get current user')
    }

    return response.data
  }

  async updateProfile(data: {
    displayName?: string
    displayNameArabic?: string
    phoneNumber?: string
    preferredLanguage?: 'en' | 'ar'
  }): Promise<void> {
    const response = await apiService.put('/identity/users/me/profile', data)

    if (!response.success) {
      throw new Error(response.message || 'Failed to update profile')
    }
  }

  async changePassword(currentPassword: string, newPassword: string): Promise<void> {
    const response = await apiService.post('/identity/auth/change-password', {
      currentPassword,
      newPassword
    })

    if (!response.success) {
      throw new Error(response.message || 'Failed to change password')
    }
  }
}

export const authService = new AuthService()
