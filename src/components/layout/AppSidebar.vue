<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { useUIStore } from '@/stores/ui'

const route = useRoute()
const uiStore = useUIStore()

interface NavItem {
  id: string
  icon: string
  label: string
  route: string
  badge?: string
  badgeClass?: string
}

interface WorkspaceItem {
  id: string
  label: string
  initials: string
  color: string
}

const navigationItems: NavItem[] = [
  { id: 'dashboard', icon: 'fas fa-th-large', label: 'Dashboard', route: '/' },
  { id: 'articles', icon: 'fas fa-book-open', label: 'Articles', route: '/articles', badge: '24', badgeClass: 'bg-teal-200 text-teal-700' },
  { id: 'documents', icon: 'fas fa-folder', label: 'Documents', route: '/documents' },
  { id: 'media', icon: 'fas fa-photo-video', label: 'Media Center', route: '/media' },
  { id: 'learning', icon: 'fas fa-graduation-cap', label: 'Learning', route: '/learning', badge: '2', badgeClass: 'bg-blue-200 text-blue-700' },
  { id: 'events', icon: 'fas fa-calendar-alt', label: 'Events', route: '/events' },
  { id: 'collaboration', icon: 'fas fa-users', label: 'Collaboration', route: '/collaboration' },
  { id: 'polls', icon: 'fas fa-poll', label: 'Polls & Surveys', route: '/polls', badge: '3', badgeClass: 'bg-orange-200 text-orange-700' },
  { id: 'services', icon: 'fas fa-concierge-bell', label: 'Self-Services', route: '/self-services' },
]

const workspaceItems: WorkspaceItem[] = [
  { id: 'ws-hr', label: 'HR Portal', initials: 'HR', color: '#8B5CF6' },
  { id: 'ws-it', label: 'IT Knowledge', initials: 'IT', color: '#3B82F6' },
  { id: 'ws-sales', label: 'Sales Enablement', initials: 'SE', color: '#10B981' },
]

const isCollapsed = computed(() => uiStore.sidebarCollapsed)

function isActive(item: NavItem): boolean {
  if (item.route === '/') {
    return route.path === '/'
  }
  return route.path.startsWith(item.route)
}
</script>

<template>
  <div class="flex flex-col h-full">
    <!-- Main Navigation -->
    <nav class="flex-1 overflow-y-auto py-6 px-3">
      <div class="space-y-1">
        <router-link
          v-for="item in navigationItems"
          :key="item.id"
          :to="item.route"
          class="sidebar-item"
          :class="{ 'active': isActive(item) }"
          :title="isCollapsed ? item.label : ''"
        >
          <div class="sidebar-item-icon" :class="{ 'active': isActive(item) }">
            <i :class="item.icon"></i>
          </div>
          <span v-show="!isCollapsed" class="sidebar-item-label">{{ item.label }}</span>
          <span v-if="item.badge && !isCollapsed" class="sidebar-item-badge" :class="item.badgeClass">
            {{ item.badge }}
          </span>
          <span v-if="item.badge && isCollapsed" class="absolute top-1 right-1 w-2 h-2 bg-teal-400 rounded-full"></span>
        </router-link>
      </div>

      <!-- Workspaces -->
      <div class="mt-6 pt-6 border-t border-gray-100">
        <p v-show="!isCollapsed" class="px-3 mb-2 text-xs font-semibold text-gray-400 uppercase tracking-wider">Workspaces</p>
        <div class="space-y-1">
          <a
            v-for="workspace in workspaceItems"
            :key="workspace.id"
            href="#"
            class="sidebar-workspace-item group"
          >
            <div class="workspace-avatar shadow-sm transition-transform group-hover:scale-110" :style="{ backgroundColor: workspace.color }">
              {{ workspace.initials }}
            </div>
            <span v-show="!isCollapsed" class="sidebar-item-label">{{ workspace.label }}</span>
          </a>
        </div>
      </div>

      <!-- Bottom Actions -->
      <div class="mt-auto pt-6 border-t border-gray-100">
        <router-link to="/settings" class="sidebar-item group">
          <div class="sidebar-item-icon group-hover:bg-teal-100 group-hover:text-teal-600 group-hover:rotate-90 transition-all duration-500">
            <i class="fas fa-cog"></i>
          </div>
          <span v-show="!isCollapsed" class="sidebar-item-label">Settings</span>
        </router-link>
        <router-link to="/ai-assistant" class="sidebar-item group">
          <div class="sidebar-item-icon group-hover:bg-teal-100 group-hover:text-teal-600 group-hover:scale-110 transition-all">
            <i class="fas fa-wand-magic-sparkles"></i>
          </div>
          <span v-show="!isCollapsed" class="sidebar-item-label">AI Assistant</span>
        </router-link>
      </div>
    </nav>
  </div>
</template>

<style scoped>
.sidebar-item {
  @apply flex items-center gap-3 px-3 py-2.5 rounded-xl text-gray-600 hover:bg-gray-50 hover:text-teal-600 transition-all relative;
}

.sidebar-item.active {
  @apply bg-teal-50 text-teal-700;
}

.sidebar-item-icon {
  @apply w-9 h-9 rounded-lg flex items-center justify-center bg-gray-100 text-gray-500 flex-shrink-0 transition-all;
}

.sidebar-item-icon.active,
.sidebar-item.active .sidebar-item-icon {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  @apply text-white shadow-md;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.35);
}

.sidebar-item:hover .sidebar-item-icon:not(.active) {
  @apply bg-teal-100 text-teal-600;
}

.sidebar-item-label {
  @apply text-sm font-medium whitespace-nowrap;
}

.sidebar-item-badge {
  @apply ml-auto px-2 py-0.5 text-xs font-semibold rounded-md;
}

.sidebar-workspace-item {
  @apply flex items-center gap-3 px-3 py-2 rounded-xl text-gray-600 hover:bg-gray-50 hover:text-teal-600 transition-all;
}

.workspace-avatar {
  @apply w-8 h-8 rounded-lg flex items-center justify-center text-white text-xs font-semibold flex-shrink-0;
}
</style>
