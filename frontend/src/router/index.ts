import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('@/views/layouts/MainLayout.vue'),
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'dashboard',
        component: () => import('@/views/dashboard/DashboardView.vue'),
        meta: { title: 'Dashboard' }
      },
      {
        path: 'content',
        name: 'content',
        component: () => import('@/views/content/ContentListView.vue'),
        meta: { title: 'Content' }
      },
      {
        path: 'content/create',
        name: 'content-create',
        component: () => import('@/views/content/ContentEditorView.vue'),
        meta: { title: 'Create Content', permission: 'content:create' }
      },
      {
        path: 'content/:id',
        name: 'content-view',
        component: () => import('@/views/content/ContentDetailView.vue'),
        meta: { title: 'View Content' }
      },
      {
        path: 'documents',
        name: 'documents',
        component: () => import('@/views/documents/DocumentLibraryView.vue'),
        meta: { title: 'Documents' }
      },
      {
        path: 'search',
        name: 'search',
        component: () => import('@/views/search/SearchResultsView.vue'),
        meta: { title: 'Search' }
      },
      {
        path: 'communities',
        name: 'communities',
        component: () => import('@/views/collaboration/CommunitiesView.vue'),
        meta: { title: 'Communities' }
      },
      {
        path: 'communities/:id',
        name: 'community-detail',
        component: () => import('@/views/collaboration/CommunityDetailView.vue'),
        meta: { title: 'Community' }
      },
      {
        path: 'discussions/:id',
        name: 'discussion-detail',
        component: () => import('@/views/collaboration/DiscussionView.vue'),
        meta: { title: 'Discussion' }
      },
      {
        path: 'lessons-learned',
        name: 'lessons-learned',
        component: () => import('@/views/collaboration/LessonsLearnedView.vue'),
        meta: { title: 'Lessons Learned' }
      },
      {
        path: 'lessons-learned/:id',
        name: 'lesson-detail',
        component: () => import('@/views/collaboration/LessonDetailView.vue'),
        meta: { title: 'Lesson Learned' }
      },
      {
        path: 'media',
        name: 'media-gallery',
        component: () => import('@/views/media/MediaGalleryView.vue'),
        meta: { title: 'Media Gallery' }
      },
      {
        path: 'media/gallery/:id',
        name: 'media-gallery-detail',
        component: () => import('@/views/media/MediaGalleryDetailView.vue'),
        meta: { title: 'Gallery' }
      },
      {
        path: 'media/video-editor',
        name: 'video-editor',
        component: () => import('@/views/media/VideoEditorView.vue'),
        meta: { title: 'Video Editor', permission: 'media:edit' }
      },
      {
        path: 'calendar',
        name: 'calendar',
        component: () => import('@/views/calendar/CalendarView.vue'),
        meta: { title: 'Calendar' }
      },
      {
        path: 'workflow/tasks',
        name: 'task-inbox',
        component: () => import('@/views/workflow/TaskInboxView.vue'),
        meta: { title: 'Task Inbox' }
      },
      {
        path: 'workflow/services',
        name: 'service-catalog',
        component: () => import('@/views/workflow/ServiceCatalogView.vue'),
        meta: { title: 'Service Catalog' }
      },
      {
        path: 'profile',
        name: 'profile',
        component: () => import('@/views/profile/ProfileView.vue'),
        meta: { title: 'Profile' }
      },
      {
        path: 'team',
        name: 'team-members',
        component: () => import('@/views/team/TeamMembersView.vue'),
        meta: { title: 'Team Members' }
      },
      {
        path: 'admin/users',
        name: 'admin-users',
        component: () => import('@/views/admin/UsersView.vue'),
        meta: { title: 'User Management', roles: ['Administrator'] }
      },
      {
        path: 'admin/roles',
        name: 'admin-roles',
        component: () => import('@/views/admin/RolesView.vue'),
        meta: { title: 'Role Management', roles: ['Administrator'] }
      },
      {
        path: 'admin/groups',
        name: 'admin-groups',
        component: () => import('@/views/admin/GroupsView.vue'),
        meta: { title: 'Group Management', roles: ['Administrator'] }
      },
      // AI Services
      {
        path: 'ai-services',
        name: 'ai-services',
        component: () => import('@/views/ai/AIServicesView.vue'),
        meta: { title: 'AI Services' }
      },
      {
        path: 'ai-services/semantic-search',
        name: 'semantic-search',
        component: () => import('@/views/ai/SemanticSearchView.vue'),
        meta: { title: 'Semantic Search' }
      },
      // Notifications
      {
        path: 'notifications',
        name: 'notifications',
        component: () => import('@/views/notifications/NotificationsView.vue'),
        meta: { title: 'Notifications' }
      },
      {
        path: 'notifications/preferences',
        name: 'notification-preferences',
        component: () => import('@/views/notifications/NotificationPreferencesView.vue'),
        meta: { title: 'Notification Preferences' }
      },
      // Integration
      {
        path: 'integrations',
        name: 'integrations',
        component: () => import('@/views/integration/IntegrationDashboardView.vue'),
        meta: { title: 'Integrations', roles: ['Administrator'] }
      },
      // Settings placeholder (redirects to profile for now)
      {
        path: 'settings',
        name: 'settings',
        component: () => import('@/views/profile/ProfileView.vue'),
        meta: { title: 'Settings' }
      }
    ]
  },
  {
    path: '/login',
    name: 'login',
    component: () => import('@/views/auth/LoginView.vue'),
    meta: { title: 'Login', requiresGuest: true }
  },
  {
    path: '/sso-callback',
    name: 'sso-callback',
    component: () => import('@/views/auth/SSOCallbackView.vue'),
    meta: { title: 'Authenticating...' }
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: () => import('@/views/errors/NotFoundView.vue'),
    meta: { title: '404 Not Found' }
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior(_to, _from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    } else {
      return { top: 0 }
    }
  }
})

// Navigation guards
router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore()

  // Update document title
  document.title = to.meta.title
    ? `${to.meta.title} | Intalio KMS`
    : 'Intalio KMS'

  // Check if route requires authentication
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({ name: 'login', query: { redirect: to.fullPath } })
    return
  }

  // Check if route requires guest (unauthenticated)
  if (to.meta.requiresGuest && authStore.isAuthenticated) {
    next({ name: 'dashboard' })
    return
  }

  // Check role requirements
  if (to.meta.roles && Array.isArray(to.meta.roles)) {
    const hasRequiredRole = authStore.hasAnyRole(to.meta.roles as string[])
    if (!hasRequiredRole) {
      next({ name: 'dashboard' })
      return
    }
  }

  // Check permission requirements
  if (to.meta.permission && typeof to.meta.permission === 'string') {
    if (!authStore.hasPermission(to.meta.permission)) {
      next({ name: 'dashboard' })
      return
    }
  }

  next()
})

export default router

// Type augmentation for route meta
declare module 'vue-router' {
  interface RouteMeta {
    title?: string
    requiresAuth?: boolean
    requiresGuest?: boolean
    roles?: string[]
    permission?: string
  }
}
