<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useUIStore } from '@/stores/ui'
import { useNotificationsStore } from '@/stores/notifications'
import { UnifiedHeader, UnifiedSidebar } from '@/components/unified'
import type { NavItem, WorkspaceItem, BottomAction, CreateMenuItem, NotificationItem } from '@/components/unified'
import AppToast from '../common/AppToast.vue'

const router = useRouter()
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
const navigationItems: NavItem[] = [
  { id: 'dashboard', icon: 'fas fa-th-large', label: 'Dashboard', route: '/' },
  { id: 'articles', icon: 'fas fa-book-open', label: 'Articles', route: '/articles', badge: '24', badgeClass: 'bg-teal-200 text-teal-700' },
  { id: 'documents', icon: 'fas fa-folder', label: 'Documents', route: '/documents' },
  { id: 'media', icon: 'fas fa-photo-video', label: 'Media Center', route: '/media' },
  { id: 'collections', icon: 'fas fa-layer-group', label: 'Collections', route: '/collections' },
  { id: 'learning', icon: 'fas fa-graduation-cap', label: 'Learning', route: '/learning', badge: '2', badgeClass: 'bg-blue-200 text-blue-700' },
  { id: 'events', icon: 'fas fa-calendar-alt', label: 'Events', route: '/events' },
  { id: 'collaboration', icon: 'fas fa-users', label: 'Collaboration', route: '/collaboration' },
  { id: 'polls', icon: 'fas fa-poll', label: 'Polls & Surveys', route: '/polls', badge: '3', badgeClass: 'bg-orange-200 text-orange-700' },
  { id: 'services', icon: 'fas fa-concierge-bell', label: 'Self-Services', route: '/self-services' },
]

// Workspace items for sidebar
const workspaceItems: WorkspaceItem[] = [
  { id: 'ws-hr', label: 'HR Portal', initials: 'HR', color: '#8B5CF6' },
  { id: 'ws-it', label: 'IT Knowledge', initials: 'IT', color: '#3B82F6' },
  { id: 'ws-sales', label: 'Sales Enablement', initials: 'SE', color: '#10B981' },
]

// Bottom actions for sidebar
const bottomActions: BottomAction[] = [
  { id: 'settings', icon: 'fas fa-cog', label: 'Settings', route: '/settings', hoverEffect: 'rotate' },
  { id: 'ai-assistant', icon: 'fas fa-wand-magic-sparkles', label: 'AI Assistant', route: '/ai-assistant', hoverEffect: 'scale' },
]

// Create menu items for header
const createMenuItems: CreateMenuItem[] = [
  { id: 'article', icon: 'fas fa-file-alt', iconBgClass: 'bg-blue-50', iconColorClass: 'text-blue-500', title: 'Article', description: 'Write a new article' },
  { id: 'poll', icon: 'fas fa-poll', iconBgClass: 'bg-purple-50', iconColorClass: 'text-purple-500', title: 'Poll', description: 'Create a survey' },
  { id: 'event', icon: 'fas fa-calendar-plus', iconBgClass: 'bg-green-50', iconColorClass: 'text-green-500', title: 'Event', description: 'Schedule an event' },
  { id: 'document', icon: 'fas fa-folder-plus', iconBgClass: 'bg-amber-50', iconColorClass: 'text-amber-500', title: 'Document', description: 'Upload a file' },
]

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
const headerActions = [
  { id: 'messages', icon: 'fas fa-comment-dots', label: 'Messages', badge: 3 }
]

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
      app-name="Knowledge Hub"
      search-placeholder="Search anything... articles, documents, people, events"
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

    <!-- Fixed Sidebar - starts at 64px from top -->
    <aside
      class="fixed left-0 bg-white border-r z-40 overflow-hidden sidebar-bounce"
      style="top: 64px; height: calc(100vh - 64px);"
      :class="isCollapsed ? 'w-20' : 'w-64'"
    >
      <UnifiedSidebar
        :navigation-items="navigationItems"
        :workspace-items="workspaceItems"
        :bottom-actions="bottomActions"
        :is-collapsed="isCollapsed"
        workspaces-title="Workspaces"
      />
    </aside>

    <!-- Main Content - margin-top: 64px for header -->
    <main
      class="relative z-10 content-bounce"
      style="margin-top: 64px; min-height: calc(100vh - 64px);"
      :class="isCollapsed ? 'ml-20' : 'ml-64'"
    >
      <slot />
    </main>

    <AppToast />
  </div>
</template>
