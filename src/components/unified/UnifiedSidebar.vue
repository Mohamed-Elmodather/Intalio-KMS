<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

// Types
export interface NavItem {
  id: string
  icon: string
  label: string
  route: string
  badge?: string | number
  badgeClass?: string
}

export interface WorkspaceItem {
  id: string
  label: string
  initials: string
  color: string
  route?: string
}

export interface BottomAction {
  id: string
  icon: string
  label: string
  route?: string
  hoverEffect?: 'rotate' | 'scale' | 'none'
}

// Props
interface Props {
  navigationItems?: NavItem[]
  workspaceItems?: WorkspaceItem[]
  bottomActions?: BottomAction[]
  workspacesTitle?: string
  isCollapsed?: boolean
  showWorkspaces?: boolean
  showBottomActions?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  navigationItems: () => [],
  workspaceItems: () => [],
  bottomActions: () => [],
  workspacesTitle: 'Workspaces',
  isCollapsed: false,
  showWorkspaces: true,
  showBottomActions: true
})

// Emits
const emit = defineEmits<{
  'nav-click': [item: NavItem]
  'workspace-click': [item: WorkspaceItem]
  'action-click': [action: BottomAction]
}>()

// Router
const route = useRoute()

// Methods
function isActive(item: NavItem): boolean {
  if (item.route === '/') {
    return route.path === '/'
  }
  return route.path.startsWith(item.route)
}

function handleNavClick(item: NavItem) {
  emit('nav-click', item)
}

function handleWorkspaceClick(item: WorkspaceItem) {
  emit('workspace-click', item)
}

function handleActionClick(action: BottomAction) {
  emit('action-click', action)
}

function getHoverClass(effect?: string): string {
  switch (effect) {
    case 'rotate':
      return 'group-hover:rotate-90'
    case 'scale':
      return 'group-hover:scale-110'
    default:
      return ''
  }
}
</script>

<template>
  <div class="unified-sidebar">
    <!-- Main Navigation -->
    <nav class="unified-sidebar__nav">
      <div class="unified-sidebar__nav-items">
        <router-link
          v-for="item in navigationItems"
          :key="item.id"
          :to="item.route"
          class="unified-sidebar__item"
          :class="{ 'unified-sidebar__item--active': isActive(item) }"
          :title="isCollapsed ? item.label : ''"
          @click="handleNavClick(item)"
        >
          <div
            class="unified-sidebar__icon"
            :class="{ 'unified-sidebar__icon--active': isActive(item) }"
          >
            <i :class="item.icon"></i>
          </div>
          <span v-show="!isCollapsed" class="unified-sidebar__label">{{ item.label }}</span>
          <span
            v-if="item.badge && !isCollapsed"
            class="unified-sidebar__badge"
            :class="item.badgeClass"
          >
            {{ item.badge }}
          </span>
          <span
            v-if="item.badge && isCollapsed"
            class="unified-sidebar__badge-dot"
          ></span>
        </router-link>
      </div>

      <!-- Workspaces -->
      <div v-if="showWorkspaces && workspaceItems.length > 0" class="unified-sidebar__section">
        <p v-show="!isCollapsed" class="unified-sidebar__section-title">{{ workspacesTitle }}</p>
        <div class="unified-sidebar__workspace-items">
          <component
            :is="item.route ? 'router-link' : 'a'"
            v-for="item in workspaceItems"
            :key="item.id"
            :to="item.route"
            :href="item.route ? undefined : '#'"
            class="unified-sidebar__workspace group"
            @click="handleWorkspaceClick(item)"
          >
            <div
              class="unified-sidebar__workspace-avatar"
              :style="{ backgroundColor: item.color }"
            >
              {{ item.initials }}
            </div>
            <span v-show="!isCollapsed" class="unified-sidebar__label">{{ item.label }}</span>
          </component>
        </div>
      </div>

      <!-- Bottom Actions -->
      <div v-if="showBottomActions && bottomActions.length > 0" class="unified-sidebar__bottom">
        <component
          :is="action.route ? 'router-link' : 'button'"
          v-for="action in bottomActions"
          :key="action.id"
          :to="action.route"
          class="unified-sidebar__item group"
          @click="handleActionClick(action)"
        >
          <div
            class="unified-sidebar__icon transition-all duration-500"
            :class="[
              'group-hover:bg-teal-100 group-hover:text-teal-600',
              getHoverClass(action.hoverEffect)
            ]"
          >
            <i :class="action.icon"></i>
          </div>
          <span v-show="!isCollapsed" class="unified-sidebar__label">{{ action.label }}</span>
        </component>
      </div>
    </nav>

    <!-- Slot for custom content -->
    <slot></slot>
  </div>
</template>

<style scoped>
.unified-sidebar {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.unified-sidebar__nav {
  flex: 1;
  overflow-y: auto;
  padding: 24px 12px;
  display: flex;
  flex-direction: column;
}

.unified-sidebar__nav-items {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.unified-sidebar__item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px 12px;
  border-radius: 12px;
  color: #4b5563;
  text-decoration: none;
  transition: all 0.2s ease;
  position: relative;
  background: none;
  border: none;
  width: 100%;
  cursor: pointer;
  text-align: start;
}

.unified-sidebar__item:hover {
  background: #f9fafb;
  color: #14b8a6;
}

.unified-sidebar__item--active {
  background: #f0fdfa;
  color: #0f766e;
}

.unified-sidebar__icon {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f3f4f6;
  color: #6b7280;
  flex-shrink: 0;
  transition: all 0.2s ease;
}

.unified-sidebar__item:hover .unified-sidebar__icon {
  background: #ccfbf1;
  color: #14b8a6;
}

.unified-sidebar__icon--active {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.35);
}

.unified-sidebar__item--active .unified-sidebar__icon,
.unified-sidebar__item:hover .unified-sidebar__icon--active {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
}

.unified-sidebar__label {
  font-size: 14px;
  font-weight: 500;
  white-space: nowrap;
}

.unified-sidebar__badge {
  margin-inline-start: auto;
  padding: 2px 8px;
  font-size: 12px;
  font-weight: 600;
  border-radius: 6px;
}

.unified-sidebar__badge-dot {
  position: absolute;
  top: 4px;
  inset-inline-end: 4px;
  width: 8px;
  height: 8px;
  background: #14b8a6;
  border-radius: 50%;
}

.unified-sidebar__section {
  margin-top: 24px;
  padding-top: 24px;
  border-top: 1px solid #f3f4f6;
}

.unified-sidebar__section-title {
  padding: 0 12px;
  margin-bottom: 8px;
  font-size: 11px;
  font-weight: 600;
  color: #9ca3af;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.unified-sidebar__workspace-items {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.unified-sidebar__workspace {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 8px 12px;
  border-radius: 12px;
  color: #4b5563;
  text-decoration: none;
  transition: all 0.2s ease;
}

.unified-sidebar__workspace:hover {
  background: #f9fafb;
  color: #14b8a6;
}

.unified-sidebar__workspace-avatar {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 12px;
  font-weight: 600;
  flex-shrink: 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s ease;
}

.unified-sidebar__workspace:hover .unified-sidebar__workspace-avatar {
  transform: scale(1.1);
}

.unified-sidebar__bottom {
  margin-top: auto;
  padding-top: 24px;
  border-top: 1px solid #f3f4f6;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

/* Scrollbar styling */
.unified-sidebar__nav::-webkit-scrollbar {
  width: 4px;
}

.unified-sidebar__nav::-webkit-scrollbar-track {
  background: transparent;
}

.unified-sidebar__nav::-webkit-scrollbar-thumb {
  background: #e5e7eb;
  border-radius: 2px;
}

.unified-sidebar__nav::-webkit-scrollbar-thumb:hover {
  background: #d1d5db;
}
</style>
