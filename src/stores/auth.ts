import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { User, AuthResponse, LoginCredentials, SSOConfig } from '@/types'
import { authApi } from '@/api'

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref<string | null>(localStorage.getItem('auth_token'))
  const refreshToken = ref<string | null>(localStorage.getItem('refresh_token'))
  const user = ref<User | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const ssoConfig = ref<SSOConfig | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!token.value && !!user.value)
  const userDisplayName = computed(() => user.value?.displayName || user.value?.email || 'User')
  const userRole = computed(() => user.value?.role || 'viewer')
  const isAdmin = computed(() => user.value?.role === 'admin')

  // Actions
  function setTokens(authToken: string, authRefreshToken: string) {
    token.value = authToken
    refreshToken.value = authRefreshToken
    localStorage.setItem('auth_token', authToken)
    localStorage.setItem('refresh_token', authRefreshToken)
  }

  function clearAuth() {
    token.value = null
    refreshToken.value = null
    user.value = null
    localStorage.removeItem('auth_token')
    localStorage.removeItem('refresh_token')
  }

  async function loginWithCredentials(credentials: LoginCredentials): Promise<boolean> {
    isLoading.value = true
    error.value = null

    try {
      const response: AuthResponse = await authApi.login(credentials)
      setTokens(response.token, response.refreshToken)
      user.value = response.user

      if (credentials.rememberMe) {
        localStorage.setItem('remember_me', 'true')
      }

      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Login failed'
      return false
    } finally {
      isLoading.value = false
    }
  }

  async function getSSOConfig(): Promise<SSOConfig | null> {
    try {
      ssoConfig.value = await authApi.getSSOConfig()
      return ssoConfig.value
    } catch {
      return null
    }
  }

  function initiateSSO() {
    if (ssoConfig.value) {
      const { authUrl, clientId, redirectUri, scopes } = ssoConfig.value
      const params = new URLSearchParams({
        client_id: clientId,
        redirect_uri: redirectUri,
        response_type: 'code',
        scope: scopes.join(' '),
      })
      window.location.href = `${authUrl}?${params.toString()}`
    }
  }

  async function handleSSOCallback(code: string): Promise<boolean> {
    isLoading.value = true
    error.value = null

    try {
      const response = await authApi.handleSSOCallback(code)
      setTokens(response.token, response.refreshToken)
      user.value = response.user
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'SSO login failed'
      return false
    } finally {
      isLoading.value = false
    }
  }

  async function logout() {
    try {
      await authApi.logout()
    } catch {
      // Ignore logout errors
    } finally {
      clearAuth()
    }
  }

  async function checkAuth(): Promise<boolean> {
    if (!token.value) {
      return false
    }

    isLoading.value = true
    try {
      user.value = await authApi.getCurrentUser()
      return true
    } catch {
      clearAuth()
      return false
    } finally {
      isLoading.value = false
    }
  }

  async function updateProfile(data: Partial<User>): Promise<boolean> {
    try {
      user.value = await authApi.updateProfile(data)
      return true
    } catch {
      return false
    }
  }

  return {
    // State
    token,
    refreshToken,
    user,
    isLoading,
    error,
    ssoConfig,

    // Getters
    isAuthenticated,
    userDisplayName,
    userRole,
    isAdmin,

    // Actions
    loginWithCredentials,
    getSSOConfig,
    initiateSSO,
    handleSSOCallback,
    logout,
    checkAuth,
    updateProfile,
    clearAuth,
  }
})
