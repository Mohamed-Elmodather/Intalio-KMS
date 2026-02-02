<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
import { useI18n } from 'vue-i18n'
import UnifiedSearch from './UnifiedSearch.vue'
import LanguageSwitcher from '@/components/common/LanguageSwitcher.vue'
import ViewAllButton from '@/components/common/ViewAllButton.vue'

const { t } = useI18n()

// Types
export interface HeaderAction {
  id: string
  icon: string
  label: string
  badge?: number | string
  badgeClass?: string
  onClick?: () => void
}

export interface CreateMenuItem {
  id: string
  icon: string
  iconBgClass: string
  iconColorClass: string
  title: string
  description: string
  onClick?: () => void
}

export interface UserInfo {
  displayName: string
  email?: string
  role?: string
  avatar?: string
  initials?: string
  isOnline?: boolean
}

export interface NotificationItem {
  id: string
  icon?: string
  title: string
  message?: string
  type?: 'info' | 'success' | 'warning' | 'error'
  isRead?: boolean
}

// Props
interface Props {
  logoSrc?: string
  logoAlt?: string
  appName?: string
  showSearch?: boolean
  searchPlaceholder?: string
  user?: UserInfo | null
  actions?: HeaderAction[]
  createMenuItems?: CreateMenuItem[]
  notifications?: NotificationItem[]
  unreadCount?: number
  height?: string
}

const props = withDefaults(defineProps<Props>(), {
  logoSrc: '/Intalio.png',
  logoAlt: 'Logo',
  appName: 'Knowledge Hub',
  showSearch: true,
  searchPlaceholder: 'Search anything... articles, documents, people, events',
  user: null,
  actions: () => [],
  createMenuItems: () => [],
  notifications: () => [],
  unreadCount: 0,
  height: '64px'
})

// Emits
const emit = defineEmits<{
  'toggle-sidebar': []
  'search': [query: string]
  'ai-click': []
  'action-click': [actionId: string]
  'create-item-click': [itemId: string]
  'notification-click': [notificationId: string]
  'mark-all-read': []
  'view-all-notifications': []
  'profile-click': []
  'settings-click': []
  'logout': []
}>()

// State
const searchQuery = ref('')
const showCreateMenu = ref(false)
const showNotifications = ref(false)
const showUserMenu = ref(false)

// Computed
const userInitials = computed(() => {
  if (props.user?.initials) return props.user.initials
  if (!props.user?.displayName) return 'U'
  const names = props.user.displayName.split(' ')
  return names.map(n => n[0]).join('').toUpperCase().slice(0, 2)
})

// Methods
function toggleSidebar() {
  emit('toggle-sidebar')
}

function handleSearch(query: string) {
  emit('search', query)
}

function handleAiClick() {
  emit('ai-click')
}

function toggleCreate() {
  showCreateMenu.value = !showCreateMenu.value
  showNotifications.value = false
  showUserMenu.value = false
}

function toggleNotifications() {
  showNotifications.value = !showNotifications.value
  showCreateMenu.value = false
  showUserMenu.value = false
}

function toggleUserMenu() {
  showUserMenu.value = !showUserMenu.value
  showCreateMenu.value = false
  showNotifications.value = false
}

function closeDropdowns(e: MouseEvent) {
  if (!(e.target as Element).closest('.header-dropdown-trigger')) {
    showCreateMenu.value = false
    showNotifications.value = false
    showUserMenu.value = false
  }
}

function handleCreateItemClick(itemId: string) {
  emit('create-item-click', itemId)
  showCreateMenu.value = false
}

function handleNotificationClick(notificationId: string) {
  emit('notification-click', notificationId)
}

function handleMarkAllRead() {
  emit('mark-all-read')
}

function handleViewAllNotifications() {
  emit('view-all-notifications')
  showNotifications.value = false
}

function handleProfileClick() {
  emit('profile-click')
  showUserMenu.value = false
}

function handleSettingsClick() {
  emit('settings-click')
  showUserMenu.value = false
}

function handleLogout() {
  emit('logout')
  showUserMenu.value = false
}

onMounted(() => {
  document.addEventListener('click', closeDropdowns)
})

onBeforeUnmount(() => {
  document.removeEventListener('click', closeDropdowns)
})
</script>

<template>
  <header class="unified-header" :style="{ height }">
    <div class="unified-header__content">
      <!-- Left: Logo & Toggle -->
      <div class="unified-header__left">
        <button @click="toggleSidebar" class="unified-header__toggle" :title="$t('header.toggleSidebar')">
          <i class="fas fa-bars"></i>
        </button>
        <div class="unified-header__brand">
          <slot name="logo">
            <a href="/" class="unified-header__logo-link">
              <img :src="logoSrc" :alt="logoAlt" class="unified-header__logo">
            </a>
          </slot>
          <div v-if="appName" class="unified-header__app-name">
            <span>{{ appName }}</span>
          </div>
        </div>
      </div>

      <!-- Center: Search -->
      <div v-if="showSearch" class="unified-header__center">
        <UnifiedSearch
          v-model="searchQuery"
          :placeholder="searchPlaceholder"
          @search="handleSearch"
          @ai-click="handleAiClick"
        />
      </div>

      <!-- Right: Actions -->
      <div class="unified-header__right">
        <!-- Create Button -->
        <div v-if="createMenuItems.length > 0" class="header-dropdown-trigger relative">
          <button @click.stop="toggleCreate" class="unified-header__btn" :title="$t('header.createNew')">
            <i class="fas fa-plus"></i>
          </button>
          <Transition name="dropdown">
            <div v-if="showCreateMenu" class="unified-header__dropdown unified-header__dropdown--create">
              <div class="unified-header__dropdown-header">
                <p>{{ $t('header.createNew') }}</p>
              </div>
              <div class="unified-header__dropdown-content">
                <button
                  v-for="item in createMenuItems"
                  :key="item.id"
                  @click="handleCreateItemClick(item.id)"
                  class="unified-header__create-item"
                >
                  <div class="unified-header__create-icon" :class="item.iconBgClass">
                    <i :class="[item.icon, item.iconColorClass]"></i>
                  </div>
                  <div class="unified-header__create-text">
                    <p class="unified-header__create-title">{{ item.title }}</p>
                    <p class="unified-header__create-desc">{{ item.description }}</p>
                  </div>
                </button>
              </div>
            </div>
          </Transition>
        </div>

        <!-- Notifications -->
        <div class="header-dropdown-trigger relative">
          <button @click.stop="toggleNotifications" class="unified-header__btn" :title="$t('header.notifications')">
            <i class="fas fa-bell"></i>
            <span v-if="unreadCount > 0" class="unified-header__badge">
              {{ unreadCount > 9 ? '9+' : unreadCount }}
            </span>
          </button>
          <Transition name="dropdown">
            <div v-if="showNotifications" class="unified-header__dropdown unified-header__dropdown--notifications">
              <div class="unified-header__dropdown-header unified-header__dropdown-header--between">
                <p>{{ $t('header.notifications') }}</p>
                <button @click="handleMarkAllRead" class="unified-header__mark-read">{{ $t('header.markAllRead') }}</button>
              </div>
              <div class="unified-header__notifications-list">
                <div v-if="notifications.length === 0" class="unified-header__notifications-empty">
                  <i class="fas fa-bell-slash"></i>
                  <p>{{ $t('header.noNotifications') }}</p>
                </div>
                <a
                  v-for="notif in notifications"
                  :key="notif.id"
                  href="#"
                  @click.prevent="handleNotificationClick(notif.id)"
                  class="unified-header__notification-item"
                  :class="{ 'unified-header__notification-item--unread': !notif.isRead }"
                >
                  <div
                    class="unified-header__notification-icon"
                    :class="{
                      'bg-green-100 text-green-600': notif.type === 'success',
                      'bg-amber-100 text-amber-600': notif.type === 'warning',
                      'bg-red-100 text-red-600': notif.type === 'error',
                      'bg-blue-100 text-blue-600': !notif.type || notif.type === 'info'
                    }"
                  >
                    <i :class="notif.icon || 'fas fa-bell'"></i>
                  </div>
                  <div class="unified-header__notification-content">
                    <p class="unified-header__notification-title">{{ notif.title }}</p>
                    <p v-if="notif.message" class="unified-header__notification-message">{{ notif.message }}</p>
                  </div>
                  <div v-if="!notif.isRead" class="unified-header__notification-dot"></div>
                </a>
              </div>
              <div class="unified-header__dropdown-footer">
                <ViewAllButton variant="text" :label="$t('header.viewAllNotifications')" @click="handleViewAllNotifications" />
              </div>
            </div>
          </Transition>
        </div>

        <!-- Custom Actions -->
        <button
          v-for="action in actions"
          :key="action.id"
          @click="emit('action-click', action.id)"
          class="unified-header__btn"
          :title="action.label"
        >
          <i :class="action.icon"></i>
          <span v-if="action.badge" class="unified-header__badge" :class="action.badgeClass">
            {{ action.badge }}
          </span>
        </button>

        <!-- Slot for additional actions -->
        <slot name="actions"></slot>

        <!-- Language Switcher -->
        <LanguageSwitcher />

        <!-- Divider -->
        <div v-if="user" class="unified-header__divider"></div>

        <!-- User Menu -->
        <div v-if="user" class="header-dropdown-trigger relative">
          <button @click.stop="toggleUserMenu" class="unified-header__user-btn">
            <div class="unified-header__user-info">
              <p class="unified-header__user-name">{{ user.displayName }}</p>
              <p class="unified-header__user-status">
                <span v-if="user.isOnline !== false" class="unified-header__status-dot"></span>
                {{ user.isOnline !== false ? $t('user.online') : $t('user.offline') }}
              </p>
            </div>
            <div class="unified-header__avatar">
              <span v-if="!user.avatar">{{ userInitials }}</span>
              <img v-else :src="user.avatar" :alt="user.displayName" class="unified-header__avatar-img">
              <span v-if="user.isOnline !== false" class="unified-header__avatar-status"></span>
            </div>
            <i class="fas fa-chevron-down unified-header__chevron"></i>
          </button>
          <Transition name="dropdown">
            <div v-if="showUserMenu" class="unified-header__dropdown unified-header__dropdown--user">
              <div class="unified-header__user-header">
                <div class="unified-header__user-avatar-lg">
                  {{ userInitials }}
                </div>
                <div>
                  <p class="unified-header__user-name-lg">{{ user.displayName }}</p>
                  <p v-if="user.role" class="unified-header__user-role">{{ user.role }}</p>
                  <p v-if="user.email" class="unified-header__user-email">{{ user.email }}</p>
                </div>
              </div>
              <div class="unified-header__user-menu">
                <button @click="handleProfileClick" class="unified-header__user-menu-item">
                  <i class="fas fa-user"></i>
                  <span>{{ $t('user.myProfile') }}</span>
                </button>
                <button @click="handleSettingsClick" class="unified-header__user-menu-item">
                  <i class="fas fa-cog"></i>
                  <span>{{ $t('nav.settings') }}</span>
                </button>
              </div>
              <div class="unified-header__user-footer">
                <button @click="handleLogout" class="unified-header__logout-btn">
                  <i class="fas fa-sign-out-alt"></i>
                  <span>{{ $t('auth.signOut') }}</span>
                </button>
              </div>
            </div>
          </Transition>
        </div>
      </div>
    </div>
  </header>
</template>

<style scoped>
.unified-header {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 50;
  height: 56px !important;
  background: linear-gradient(180deg, rgba(255, 255, 255, 0.98) 0%, rgba(255, 255, 255, 0.95) 100%);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04), 0 4px 20px rgba(0, 0, 0, 0.02);
}

@media (min-width: 640px) {
  .unified-header {
    height: 64px !important;
  }
}

.unified-header__content {
  height: 100%;
  padding: 0 8px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

@media (min-width: 640px) {
  .unified-header__content {
    padding: 0 16px;
  }
}

.unified-header__left {
  display: flex;
  align-items: center;
  gap: 8px;
}

@media (min-width: 640px) {
  .unified-header__left {
    gap: 12px;
  }
}

.unified-header__toggle {
  width: 34px;
  height: 34px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: none;
  cursor: pointer;
  color: #6b7280;
  transition: all 0.2s ease;
}

@media (min-width: 640px) {
  .unified-header__toggle {
    width: 40px;
    height: 40px;
    border-radius: 12px;
  }
}

.unified-header__toggle:hover {
  background: linear-gradient(135deg, rgba(20, 184, 166, 0.08), rgba(13, 148, 136, 0.05));
  color: #14b8a6;
}

.unified-header__brand {
  display: flex;
  align-items: center;
  gap: 8px;
}

@media (min-width: 640px) {
  .unified-header__brand {
    gap: 12px;
  }
}

.unified-header__logo-link {
  display: flex;
}

.unified-header__logo {
  height: 24px;
  object-fit: contain;
}

@media (min-width: 640px) {
  .unified-header__logo {
    height: 32px;
  }
}

.unified-header__app-name {
  display: none;
  align-items: center;
  gap: 8px;
  padding-inline-start: 12px;
  border-inline-start: 1px solid #e5e7eb;
}

@media (min-width: 768px) {
  .unified-header__app-name {
    display: flex;
  }
}

.unified-header__app-name span {
  font-size: 14px;
  font-weight: 600;
  color: #374151;
}

.unified-header__center {
  flex: 1;
  max-width: 280px;
  margin: 0 8px;
}

@media (min-width: 640px) {
  .unified-header__center {
    max-width: 400px;
    margin: 0 16px;
  }
}

@media (min-width: 1024px) {
  .unified-header__center {
    max-width: 640px;
    margin: 0 24px;
  }
}

.unified-header__right {
  display: flex;
  align-items: center;
  gap: 2px;
}

@media (min-width: 640px) {
  .unified-header__right {
    gap: 4px;
  }
}

.unified-header__btn {
  position: relative;
  width: 34px;
  height: 34px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: none;
  cursor: pointer;
  color: #6b7280;
  transition: all 0.2s ease;
  font-size: 14px;
}

@media (min-width: 640px) {
  .unified-header__btn {
    width: 40px;
    height: 40px;
    border-radius: 12px;
    font-size: 16px;
  }
}

.unified-header__btn:hover {
  background: linear-gradient(135deg, rgba(20, 184, 166, 0.08), rgba(13, 148, 136, 0.05));
  color: #14b8a6;
}

.unified-header__badge {
  position: absolute;
  top: 4px;
  inset-inline-end: 4px;
  min-width: 14px;
  height: 14px;
  padding: 0 3px;
  background: linear-gradient(135deg, #ef4444, #dc2626);
  color: white;
  font-size: 9px;
  font-weight: 600;
  border-radius: 7px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1.5px solid white;
  box-shadow: 0 2px 4px rgba(239, 68, 68, 0.3);
}

@media (min-width: 640px) {
  .unified-header__badge {
    top: 6px;
    inset-inline-end: 6px;
    min-width: 16px;
    height: 16px;
    padding: 0 4px;
    font-size: 10px;
    border-radius: 8px;
    border: 2px solid white;
  }
}

.unified-header__divider {
  width: 1px;
  height: 24px;
  background: #e5e7eb;
  margin: 0 4px;
  display: none;
}

@media (min-width: 640px) {
  .unified-header__divider {
    display: block;
    height: 32px;
    margin: 0 8px;
  }
}

.unified-header__user-btn {
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 4px;
  border-radius: 10px;
  background: transparent;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
}

@media (min-width: 640px) {
  .unified-header__user-btn {
    gap: 12px;
    padding: 4px 8px;
    border-radius: 12px;
  }
}

.unified-header__user-btn:hover {
  background: #f9fafb;
}

.unified-header__user-info {
  text-align: end;
  display: none;
}

@media (min-width: 768px) {
  .unified-header__user-info {
    display: block;
  }
}

.unified-header__user-name {
  font-size: 14px;
  font-weight: 600;
  color: #1f2937;
  transition: color 0.2s ease;
}

.unified-header__user-btn:hover .unified-header__user-name {
  color: #0f766e;
}

.unified-header__user-status {
  font-size: 12px;
  color: #6b7280;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 4px;
}

[dir="rtl"] .unified-header__user-status {
  justify-content: flex-start;
}

.unified-header__status-dot {
  width: 6px;
  height: 6px;
  background: #10b981;
  border-radius: 50%;
}

.unified-header__avatar {
  width: 32px;
  height: 32px;
  border-radius: 10px;
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 700;
  font-size: 12px;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
  position: relative;
}

@media (min-width: 640px) {
  .unified-header__avatar {
    width: 40px;
    height: 40px;
    border-radius: 12px;
    font-size: 14px;
  }
}

.unified-header__avatar-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 10px;
}

@media (min-width: 640px) {
  .unified-header__avatar-img {
    border-radius: 12px;
  }
}

.unified-header__avatar-status {
  position: absolute;
  bottom: -2px;
  inset-inline-end: -2px;
  width: 10px;
  height: 10px;
  background: #10b981;
  border: 2px solid white;
  border-radius: 50%;
}

@media (min-width: 640px) {
  .unified-header__avatar-status {
    width: 12px;
    height: 12px;
  }
}

.unified-header__chevron {
  font-size: 10px;
  color: #9ca3af;
  transition: color 0.2s ease;
  display: none;
}

@media (min-width: 640px) {
  .unified-header__chevron {
    display: block;
  }
}

.unified-header__user-btn:hover .unified-header__chevron {
  color: #14b8a6;
}

/* Dropdowns */
.unified-header__dropdown {
  position: absolute;
  inset-inline-end: 0;
  top: calc(100% + 8px);
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.12), 0 0 0 1px rgba(0, 0, 0, 0.04);
  z-index: 100;
  overflow: hidden;
}

@media (min-width: 640px) {
  .unified-header__dropdown {
    border-radius: 16px;
  }
}

.unified-header__dropdown--create {
  width: 200px;
}

@media (min-width: 640px) {
  .unified-header__dropdown--create {
    width: 224px;
  }
}

.unified-header__dropdown--notifications {
  width: calc(100vw - 16px);
  max-width: 320px;
  inset-inline-end: -8px;
}

@media (min-width: 400px) {
  .unified-header__dropdown--notifications {
    width: 320px;
    inset-inline-end: 0;
  }
}

.unified-header__dropdown--user {
  width: 220px;
}

@media (min-width: 640px) {
  .unified-header__dropdown--user {
    width: 256px;
  }
}

.unified-header__dropdown-header {
  padding: 12px 16px;
  border-bottom: 1px solid #f3f4f6;
}

.unified-header__dropdown-header p {
  font-size: 11px;
  font-weight: 600;
  color: #9ca3af;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.unified-header__dropdown-header--between {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.unified-header__mark-read {
  font-size: 12px;
  color: #14b8a6;
  background: none;
  border: none;
  cursor: pointer;
  font-weight: 500;
}

.unified-header__mark-read:hover {
  color: #0f766e;
}

.unified-header__dropdown-content {
  padding: 4px 0;
}

.unified-header__create-item {
  display: flex;
  align-items: center;
  gap: 12px;
  width: 100%;
  padding: 8px 12px;
  background: none;
  border: none;
  cursor: pointer;
  transition: background 0.2s ease;
  text-align: start;
}

.unified-header__create-item:hover {
  background: #f9fafb;
}

.unified-header__create-icon {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.unified-header__create-icon i {
  font-size: 14px;
}

.unified-header__create-text {
  min-width: 0;
}

.unified-header__create-title {
  font-size: 14px;
  font-weight: 500;
  color: #374151;
}

.unified-header__create-desc {
  font-size: 12px;
  color: #9ca3af;
}

/* Notifications */
.unified-header__notifications-list {
  max-height: 320px;
  overflow-y: auto;
}

.unified-header__notifications-empty {
  padding: 32px 16px;
  text-align: center;
  color: #9ca3af;
}

.unified-header__notifications-empty i {
  font-size: 24px;
  margin-bottom: 8px;
}

.unified-header__notifications-empty p {
  font-size: 14px;
}

.unified-header__notification-item {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  padding: 10px 12px;
  transition: background 0.2s ease;
  text-decoration: none;
}

@media (min-width: 640px) {
  .unified-header__notification-item {
    gap: 12px;
    padding: 12px 16px;
  }
}

.unified-header__notification-item:hover {
  background: #f9fafb;
}

.unified-header__notification-item--unread {
  background: rgba(20, 184, 166, 0.05);
}

.unified-header__notification-icon {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.unified-header__notification-icon i {
  font-size: 14px;
}

.unified-header__notification-content {
  flex: 1;
  min-width: 0;
}

.unified-header__notification-title {
  font-size: 14px;
  color: #1f2937;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.unified-header__notification-message {
  font-size: 12px;
  color: #9ca3af;
  margin-top: 2px;
}

.unified-header__notification-dot {
  width: 8px;
  height: 8px;
  background: #14b8a6;
  border-radius: 50%;
  flex-shrink: 0;
  margin-top: 8px;
}

.unified-header__dropdown-footer {
  padding: 12px 16px;
  border-top: 1px solid #f3f4f6;
  background: #fafafa;
}

.unified-header__view-all {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  width: 100%;
  font-size: 14px;
  color: #14b8a6;
  font-weight: 500;
  background: none;
  border: none;
  cursor: pointer;
}

.unified-header__view-all:hover {
  color: #0f766e;
}

.unified-header__view-all i {
  font-size: 12px;
}

/* User Menu */
.unified-header__user-header {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px;
  border-bottom: 1px solid #f3f4f6;
  background: linear-gradient(135deg, #f0fdfa, white);
  border-radius: 12px 12px 0 0;
}

@media (min-width: 640px) {
  .unified-header__user-header {
    gap: 12px;
    padding: 16px;
    border-radius: 16px 16px 0 0;
  }
}

.unified-header__user-avatar-lg {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 700;
  font-size: 16px;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
  flex-shrink: 0;
}

@media (min-width: 640px) {
  .unified-header__user-avatar-lg {
    width: 48px;
    height: 48px;
    border-radius: 12px;
    font-size: 18px;
  }
}

.unified-header__user-name-lg {
  font-size: 14px;
  font-weight: 600;
  color: #1f2937;
}

.unified-header__user-role {
  font-size: 12px;
  color: #6b7280;
}

.unified-header__user-email {
  font-size: 12px;
  color: #14b8a6;
  margin-top: 2px;
}

.unified-header__user-menu {
  padding: 8px 0;
}

.unified-header__user-menu-item {
  display: flex;
  align-items: center;
  gap: 12px;
  width: 100%;
  padding: 8px 16px;
  background: none;
  border: none;
  cursor: pointer;
  transition: background 0.2s ease;
  text-align: start;
}

.unified-header__user-menu-item:hover {
  background: #f9fafb;
}

.unified-header__user-menu-item i {
  width: 20px;
  color: #9ca3af;
}

.unified-header__user-menu-item span {
  font-size: 14px;
  color: #374151;
}

.unified-header__user-footer {
  padding: 8px 0;
  border-top: 1px solid #f3f4f6;
}

.unified-header__logout-btn {
  display: flex;
  align-items: center;
  gap: 12px;
  width: 100%;
  padding: 8px 16px;
  background: none;
  border: none;
  cursor: pointer;
  transition: background 0.2s ease;
  color: #dc2626;
}

.unified-header__logout-btn:hover {
  background: #fef2f2;
}

.unified-header__logout-btn i {
  width: 20px;
}

.unified-header__logout-btn span {
  font-size: 14px;
  font-weight: 500;
}

/* Dropdown Transitions */
.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.2s ease;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}
</style>
