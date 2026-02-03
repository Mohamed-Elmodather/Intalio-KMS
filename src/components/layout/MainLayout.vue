<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/auth'
import { useUIStore } from '@/stores/ui'
import { useNotificationsStore } from '@/stores/notifications'
import { UnifiedHeader, UnifiedSidebar } from '@/components/unified'
import type { NavItem, WorkspaceItem, BottomAction, CreateMenuItem, NotificationItem } from '@/components/unified'
import AppToast from '../common/AppToast.vue'

const router = useRouter()
const { t } = useI18n()
const authStore = useAuthStore()
const uiStore = useUIStore()
const notificationsStore = useNotificationsStore()

const isCollapsed = computed(() => uiStore.sidebarCollapsed)

// User info for header
const user = computed(() => {
  if (!authStore.user) return null
  return {
    displayName: authStore.user.displayName || 'User',
    email: authStore.user.email,
    role: authStore.user.jobTitle || authStore.user.role,
    avatar: authStore.user.avatar,
    isOnline: true
  }
})

// Navigation items for sidebar
const navigationItems = computed<NavItem[]>(() => [
  { id: 'dashboard', icon: 'fas fa-th-large', label: t('nav.dashboard'), route: '/' },
  { id: 'articles', icon: 'fas fa-book-open', label: t('nav.articles'), route: '/articles', badge: '24', badgeClass: 'bg-teal-200 text-teal-700' },
  { id: 'documents', icon: 'fas fa-folder', label: t('nav.documents'), route: '/documents' },
  { id: 'media', icon: 'fas fa-photo-video', label: t('nav.media'), route: '/media' },
  { id: 'collections', icon: 'fas fa-layer-group', label: t('nav.collections'), route: '/collections' },
  { id: 'learning', icon: 'fas fa-graduation-cap', label: t('nav.learning'), route: '/learning', badge: '2', badgeClass: 'bg-blue-200 text-blue-700' },
  { id: 'events', icon: 'fas fa-calendar-alt', label: t('nav.events'), route: '/events' },
  { id: 'collaboration', icon: 'fas fa-users', label: t('nav.collaboration'), route: '/collaboration' },
  { id: 'polls', icon: 'fas fa-poll', label: t('nav.polls'), route: '/polls', badge: '3', badgeClass: 'bg-orange-200 text-orange-700' },
  { id: 'services', icon: 'fas fa-concierge-bell', label: t('nav.services'), route: '/self-services' },
  { id: 'analytics', icon: 'fas fa-chart-bar', label: t('nav.analytics'), route: '/analytics' },
])

// Workspace items for sidebar
const workspaceItems = computed<WorkspaceItem[]>(() => [
  { id: 'ws-hr', label: 'HR Portal', initials: 'HR', color: '#8B5CF6' },
  { id: 'ws-it', label: 'IT Knowledge', initials: 'IT', color: '#3B82F6' },
  { id: 'ws-sales', label: 'Sales Enablement', initials: 'SE', color: '#10B981' },
])

// Bottom actions for sidebar
const bottomActions = computed<BottomAction[]>(() => [
  { id: 'settings', icon: 'fas fa-cog', label: t('nav.settings'), route: '/settings', hoverEffect: 'rotate' },
  { id: 'ai-assistant', icon: 'fas fa-wand-magic-sparkles', label: t('nav.aiAssistant'), route: '/ai-assistant', hoverEffect: 'scale' },
])

// Create menu items for header
const createMenuItems = computed<CreateMenuItem[]>(() => [
  { id: 'article', icon: 'fas fa-file-alt', iconBgClass: 'bg-blue-50', iconColorClass: 'text-blue-500', title: t('create.article'), description: t('create.articleDesc') },
  { id: 'poll', icon: 'fas fa-poll', iconBgClass: 'bg-purple-50', iconColorClass: 'text-purple-500', title: t('create.poll'), description: t('create.pollDesc') },
  { id: 'event', icon: 'fas fa-calendar-plus', iconBgClass: 'bg-green-50', iconColorClass: 'text-green-500', title: t('create.event'), description: t('create.eventDesc') },
  { id: 'document', icon: 'fas fa-folder-plus', iconBgClass: 'bg-amber-50', iconColorClass: 'text-amber-500', title: t('create.document'), description: t('create.documentDesc') },
])

// Notifications
const notifications = computed<NotificationItem[]>(() =>
  notificationsStore.recentNotifications.map(n => ({
    id: n.id,
    icon: n.icon,
    title: n.title,
    message: n.message,
    type: n.type as 'info' | 'success' | 'warning' | 'error',
    isRead: n.isRead
  }))
)

const unreadCount = computed(() => notificationsStore.unreadCount)

// Header actions
const headerActions = computed(() => [
  { id: 'messages', icon: 'fas fa-comment-dots', label: t('common.messages'), badge: 3 }
])

// Event handlers
function handleToggleSidebar() {
  uiStore.toggleSidebar()
}

function handleSearch(query: string) {
  router.push({ name: 'SearchResults', query: { q: query } })
}

function handleAiClick() {
  router.push({ name: 'AIAssistant' })
}

function handleCreateItemClick(itemId: string) {
  const routes: Record<string, string> = {
    article: 'ArticleCreate',
    poll: 'PollCreate',
    event: 'Events',
    document: 'Documents'
  }
  if (routes[itemId]) {
    router.push({ name: routes[itemId] })
  }
}

function handleMarkAllRead() {
  notificationsStore.markAllAsRead()
}

function handleViewAllNotifications() {
  router.push('/notifications')
}

function handleProfileClick() {
  router.push({ name: 'Profile' })
}

function handleSettingsClick() {
  router.push({ name: 'Settings' })
}

async function handleLogout() {
  await authStore.logout()
  router.push({ name: 'Login' })
}

onMounted(async () => {
  await authStore.checkAuth()
  notificationsStore.fetchUnreadCount()
})
</script>

<template>
  <div class="elegant-bg min-h-screen">
    <!-- Floating Particles Background -->
    <div class="particles">
      <div class="particle"></div>
      <div class="particle"></div>
      <div class="particle"></div>
      <div class="particle"></div>
      <div class="particle"></div>
      <div class="particle"></div>
      <div class="particle"></div>
      <div class="particle"></div>
      <div class="particle"></div>
    </div>

    <!-- Unified Header -->
    <UnifiedHeader
      logo-src="/Intalio.png"
      logo-alt="Intalio"
      :app-name="$t('app.name')"
      :search-placeholder="$t('common.searchPlaceholder')"
      :user="user"
      :create-menu-items="createMenuItems"
      :notifications="notifications"
      :unread-count="unreadCount"
      :actions="headerActions"
      @toggle-sidebar="handleToggleSidebar"
      @search="handleSearch"
      @ai-click="handleAiClick"
      @create-item-click="handleCreateItemClick"
      @mark-all-read="handleMarkAllRead"
      @view-all-notifications="handleViewAllNotifications"
      @profile-click="handleProfileClick"
      @settings-click="handleSettingsClick"
      @logout="handleLogout"
    />

    <!-- Fixed Sidebar - starts below header -->
    <aside
      class="layout-sidebar fixed start-0 overflow-hidden sidebar-bounce"
      :class="isCollapsed ? 'w-20' : 'w-64'"
    >
      <UnifiedSidebar
        :navigation-items="navigationItems"
        :workspace-items="workspaceItems"
        :bottom-actions="bottomActions"
        :is-collapsed="isCollapsed"
        :workspaces-title="$t('nav.workspaces.title')"
      />
    </aside>

    <!-- Main Content - margin-top for header -->
    <main
      class="layout-main relative z-10 content-bounce"
      :class="['sm:ms-20', { 'sm:ms-64': !isCollapsed }]"
    >
      <slot />
    </main>

    <AppToast />
  </div>
</template>

<style scoped>
.layout-sidebar {
  display: none;
  top: 56px;
  height: calc(100vh - 56px);
  background-color: #ffffff;
  border-inline-end: 1px solid #e5e7eb;
  z-index: 40;
  box-shadow: 2px 0 8px rgba(0, 0, 0, 0.05);
}

@media (min-width: 640px) {
  .layout-sidebar {
    display: block;
    top: 64px;
    height: calc(100vh - 64px);
  }
}

.layout-main {
  margin-top: 56px;
  min-height: calc(100vh - 56px);
}

@media (min-width: 640px) {
  .layout-main {
    margin-top: 64px;
    min-height: calc(100vh - 64px);
  }
}
</style>
