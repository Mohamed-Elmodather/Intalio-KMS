import type { NavigationGuardNext, RouteLocationNormalized } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

export async function authGuard(
  to: RouteLocationNormalized,
  _from: RouteLocationNormalized,
  next: NavigationGuardNext
) {
  const authStore = useAuthStore()

  // Check if route requires authentication
  const requiresAuth = to.meta.requiresAuth !== false

  // If route doesn't require auth, proceed
  if (!requiresAuth) {
    // If user is already authenticated and tries to access login, redirect to dashboard
    if (to.name === 'Login' && authStore.isAuthenticated) {
      return next({ name: 'Dashboard' })
    }
    return next()
  }

  // Check authentication status
  if (!authStore.isAuthenticated) {
    // Try to verify existing token
    if (authStore.token) {
      const isValid = await authStore.checkAuth()
      if (isValid) {
        return checkRoleAccess(to, next, authStore)
      }
    }

    // Not authenticated, redirect to login
    return next({
      name: 'Login',
      query: { redirect: to.fullPath },
    })
  }

  // Check role-based access
  return checkRoleAccess(to, next, authStore)
}

function checkRoleAccess(
  to: RouteLocationNormalized,
  next: NavigationGuardNext,
  authStore: ReturnType<typeof useAuthStore>
) {
  const requiredRoles = to.meta.roles as string[] | undefined

  if (requiredRoles && requiredRoles.length > 0) {
    const userRole = authStore.userRole
    if (!requiredRoles.includes(userRole)) {
      // User doesn't have required role
      return next({ name: 'Dashboard' })
    }
  }

  return next()
}

export function titleGuard(to: RouteLocationNormalized) {
  const appTitle = import.meta.env.VITE_APP_TITLE || 'Intalio Knowledge Hub'
  const pageTitle = to.meta.title as string | undefined

  document.title = pageTitle ? `${pageTitle} | ${appTitle}` : appTitle
}
