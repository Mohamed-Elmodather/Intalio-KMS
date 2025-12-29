import apiClient from './client'
import type { AuthResponse, LoginCredentials, SSOConfig, User } from '@/types'

const AUTH_BASE = '/identity/auth'
const USERS_BASE = '/identity/users'

export const authApi = {
  // Login with email/password
  async login(credentials: LoginCredentials): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>(`${AUTH_BASE}/login`, credentials)
    return response.data
  },

  // Get SSO configuration
  async getSSOConfig(): Promise<SSOConfig> {
    const response = await apiClient.get<SSOConfig>(`${AUTH_BASE}/sso-config`)
    return response.data
  },

  // Handle SSO callback
  async handleSSOCallback(code: string): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>(`${AUTH_BASE}/sso-callback`, { code })
    return response.data
  },

  // Refresh token
  async refreshToken(refreshToken: string): Promise<{ token: string; refreshToken: string }> {
    const response = await apiClient.post<{ token: string; refreshToken: string }>(
      `${AUTH_BASE}/refresh`,
      { refreshToken }
    )
    return response.data
  },

  // Logout
  async logout(): Promise<void> {
    await apiClient.post(`${AUTH_BASE}/logout`)
  },

  // Get current user
  async getCurrentUser(): Promise<User> {
    const response = await apiClient.get<User>(`${USERS_BASE}/me`)
    return response.data
  },

  // Update current user profile
  async updateProfile(data: Partial<User>): Promise<User> {
    const response = await apiClient.put<User>(`${USERS_BASE}/me`, data)
    return response.data
  },

  // Change password
  async changePassword(currentPassword: string, newPassword: string): Promise<void> {
    await apiClient.put(`${AUTH_BASE}/change-password`, { currentPassword, newPassword })
  },

  // Request password reset
  async requestPasswordReset(email: string): Promise<void> {
    await apiClient.post(`${AUTH_BASE}/forgot-password`, { email })
  },

  // Reset password with token
  async resetPassword(token: string, newPassword: string): Promise<void> {
    await apiClient.post(`${AUTH_BASE}/reset-password`, { token, newPassword })
  },
}

export default authApi
