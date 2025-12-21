import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { User, AuthTokens } from '@/types'
import { authService } from '@/services/auth.service'

export const useAuthStore = defineStore('auth', () => {
  // State
  const user = ref<User | null>(null)
  const tokens = ref<AuthTokens | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!tokens.value?.accessToken)
  const accessToken = computed(() => tokens.value?.accessToken)
  const userDisplayName = computed(() => user.value?.displayName ?? '')
  const userEmail = computed(() => user.value?.email ?? '')
  const userAvatar = computed(() => user.value?.avatarUrl ?? '')
  const userRoles = computed(() => user.value?.roles ?? [])
  const userPermissions = computed(() => user.value?.permissions ?? [])

  // Actions
  async function login(email: string, password: string): Promise<boolean> {
    isLoading.value = true
    error.value = null

    try {
      const response = await authService.login(email, password)
      tokens.value = response.tokens
      user.value = response.user
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Login failed'
      return false
    } finally {
      isLoading.value = false
    }
  }

  async function loginWithSSO(): Promise<boolean> {
    isLoading.value = true
    error.value = null

    try {
      // Redirect to Azure AD login
      await authService.redirectToSSO()
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'SSO login failed'
      return false
    } finally {
      isLoading.value = false
    }
  }

  async function handleSSOCallback(code: string): Promise<boolean> {
    isLoading.value = true
    error.value = null

    try {
      const response = await authService.handleSSOCallback(code)
      tokens.value = response.tokens
      user.value = response.user
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'SSO callback failed'
      return false
    } finally {
      isLoading.value = false
    }
  }

  async function logout(): Promise<void> {
    try {
      await authService.logout()
    } finally {
      user.value = null
      tokens.value = null
    }
  }

  async function refreshToken(): Promise<boolean> {
    if (!tokens.value?.refreshToken) return false

    try {
      const response = await authService.refreshToken(tokens.value.refreshToken)
      tokens.value = response.tokens
      return true
    } catch {
      await logout()
      return false
    }
  }

  function hasRole(role: string): boolean {
    return userRoles.value.includes(role)
  }

  function hasPermission(permission: string): boolean {
    return userPermissions.value.includes(permission)
  }

  function hasAnyRole(roles: string[]): boolean {
    return roles.some(role => hasRole(role))
  }

  function hasAnyPermission(permissions: string[]): boolean {
    return permissions.some(permission => hasPermission(permission))
  }

  return {
    // State
    user,
    tokens,
    isLoading,
    error,
    // Getters
    isAuthenticated,
    accessToken,
    userDisplayName,
    userEmail,
    userAvatar,
    userRoles,
    userPermissions,
    // Actions
    login,
    loginWithSSO,
    handleSSOCallback,
    logout,
    refreshToken,
    hasRole,
    hasPermission,
    hasAnyRole,
    hasAnyPermission
  }
}, {
  persist: {
    key: 'intalio-auth',
    pick: ['tokens', 'user'],
    storage: sessionStorage
  }
})
