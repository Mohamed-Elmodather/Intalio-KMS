<script setup lang="ts">
/**
 * MainLayout Component
 *
 * Main application layout with sidebar, header, and content area.
 * Enhanced with glassmorphism, micro-interactions, and smooth transitions.
 */
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useUIStore } from '@/stores/ui.store'
import { useAuthStore } from '@/stores/auth.store'
import { useConfirm } from 'primevue/useconfirm'
import { useReducedMotion } from '@/composables/useReducedMotion'
import { Avatar } from '@/components/base'
import Menu from 'primevue/menu'

const prefersReducedMotion = useReducedMotion()

const router = useRouter()
const route = useRoute()
const { t, locale: i18nLocale } = useI18n()
const uiStore = useUIStore()
const authStore = useAuthStore()
const confirm = useConfirm()

function toggleLanguage() {
  uiStore.toggleLocale()
  i18nLocale.value = uiStore.locale
}

const userMenu = ref()
const searchQuery = ref('')
const searchFocused = ref(false)
const layoutMounted = ref(false)

// Trigger entrance animations after mount
onMounted(() => {
  requestAnimationFrame(() => {
    layoutMounted.value = true
  })
})

const navigationItems = computed(() => [
  { label: t('nav.dashboard'), icon: 'pi pi-th-large', route: '/', exact: true },
  { label: t('nav.tasks'), icon: 'pi pi-check-square', route: '/workflow/tasks' },
  { label: t('nav.content'), icon: 'pi pi-file-edit', route: '/content' },
  { label: t('nav.documents'), icon: 'pi pi-folder-open', route: '/documents' },
  { label: t('nav.media'), icon: 'pi pi-images', route: '/media' },
  { label: t('nav.communities'), icon: 'pi pi-comments', route: '/communities' },
  { label: t('nav.lessonsLearned'), icon: 'pi pi-book', route: '/lessons-learned' },
  { label: t('nav.team'), icon: 'pi pi-users', route: '/team' },
  { label: t('nav.calendar'), icon: 'pi pi-calendar', route: '/calendar' },
  { label: t('nav.aiServices'), icon: 'pi pi-sparkles', route: '/ai-services' },
  { label: t('nav.search'), icon: 'pi pi-search', route: '/search' }
])

const userMenuItems = computed(() => [
  { label: t('nav.profile'), icon: 'pi pi-user', command: () => router.push('/profile') },
  { label: t('nav.settings'), icon: 'pi pi-cog', command: () => router.push('/settings') },
  { separator: true },
  { label: t('auth.logout'), icon: 'pi pi-sign-out', command: () => confirmLogout() }
])

function isActiveRoute(item: { route: string; exact?: boolean }) {
  if (item.exact) {
    return route.path === item.route
  }
  return route.path.startsWith(item.route)
}

function toggleUserMenu(event: Event) {
  userMenu.value.toggle(event)
}

function confirmLogout() {
  confirm.require({
    message: t('auth.logoutConfirm'),
    header: t('auth.logout'),
    icon: 'pi pi-sign-out',
    accept: async () => {
      await authStore.logout()
      router.push('/login')
    }
  })
}

function handleSearch() {
  if (searchQuery.value.trim()) {
    router.push({ name: 'search', query: { q: searchQuery.value } })
  }
}
</script>

<template>
  <div
    class="layout-wrapper"
    :class="{
      'sidebar-collapsed': uiStore.sidebarCollapsed,
      'layout-mounted': layoutMounted,
      'reduced-motion': prefersReducedMotion
    }"
  >
    <!-- Skip Link for Keyboard Navigation -->
    <a href="#main-content" class="skip-link">
      {{ t('accessibility.skipToContent') || 'Skip to main content' }}
    </a>

    <!-- Sidebar -->
    <aside class="sidebar" :class="{ collapsed: uiStore.sidebarCollapsed }">
      <!-- Logo Section -->
      <div class="sidebar-header">
        <router-link to="/" class="logo-link">
          <img
            src="@/assets/images/logo.png"
            alt="Intalio KMS"
            class="logo-full"
            v-show="!uiStore.sidebarCollapsed"
          />
          <div class="logo-compact" v-show="uiStore.sidebarCollapsed">
            <span>K</span>
          </div>
        </router-link>
      </div>

      <!-- Navigation -->
      <nav class="sidebar-nav">
        <div class="nav-group">
          <router-link
            v-for="item in navigationItems"
            :key="item.route"
            :to="item.route"
            class="nav-item"
            :class="{ active: isActiveRoute(item) }"
          >
            <span class="nav-icon">
              <i :class="item.icon"></i>
            </span>
            <span class="nav-label" v-show="!uiStore.sidebarCollapsed">{{ item.label }}</span>
            <span class="nav-indicator" v-if="isActiveRoute(item)"></span>
          </router-link>
        </div>

        <!-- Admin Section -->
        <div v-if="authStore.hasRole('Administrator')" class="nav-group admin-group">
          <div class="nav-section-header" v-show="!uiStore.sidebarCollapsed">
            <span>{{ t('nav.admin') }}</span>
          </div>
          <div class="nav-divider" v-show="uiStore.sidebarCollapsed"></div>

          <router-link to="/admin/users" class="nav-item" :class="{ active: route.path.startsWith('/admin/users') }">
            <span class="nav-icon">
              <i class="pi pi-users"></i>
            </span>
            <span class="nav-label" v-show="!uiStore.sidebarCollapsed">{{ t('admin.users') }}</span>
          </router-link>

          <router-link to="/admin/roles" class="nav-item" :class="{ active: route.path.startsWith('/admin/roles') }">
            <span class="nav-icon">
              <i class="pi pi-shield"></i>
            </span>
            <span class="nav-label" v-show="!uiStore.sidebarCollapsed">{{ t('admin.roles') }}</span>
          </router-link>

          <router-link to="/admin/groups" class="nav-item" :class="{ active: route.path.startsWith('/admin/groups') }">
            <span class="nav-icon">
              <i class="pi pi-sitemap"></i>
            </span>
            <span class="nav-label" v-show="!uiStore.sidebarCollapsed">{{ t('admin.groups') }}</span>
          </router-link>

          <router-link to="/integrations" class="nav-item" :class="{ active: route.path.startsWith('/integrations') }">
            <span class="nav-icon">
              <i class="pi pi-link"></i>
            </span>
            <span class="nav-label" v-show="!uiStore.sidebarCollapsed">{{ t('admin.integrations') }}</span>
          </router-link>
        </div>
      </nav>

      <!-- Sidebar Footer -->
      <div class="sidebar-footer">
        <!-- User Quick Info -->
        <div class="user-quick" v-show="!uiStore.sidebarCollapsed">
          <Avatar
            :image="authStore.userAvatar"
            :name="authStore.userDisplayName || 'User'"
            shape="circle"
            class="user-avatar"
          />
          <div class="user-info">
            <span class="user-name">{{ authStore.userDisplayName }}</span>
            <span class="user-role">{{ authStore.user?.jobTitle || 'User' }}</span>
          </div>
        </div>

        <!-- Collapse Toggle -->
        <button class="collapse-btn" @click="uiStore.toggleSidebar" :title="uiStore.sidebarCollapsed ? 'Expand' : 'Collapse'">
          <i :class="uiStore.sidebarCollapsed ? 'pi pi-angle-double-right' : 'pi pi-angle-double-left'"></i>
        </button>
      </div>
    </aside>

    <!-- Main Content Area -->
    <div class="main-wrapper">
      <!-- Header -->
      <header class="header">
        <div class="header-start">
          <!-- Mobile Menu Toggle -->
          <button class="mobile-menu-btn" @click="uiStore.toggleSidebar">
            <i class="pi pi-bars"></i>
          </button>

          <!-- Search Bar -->
          <div class="search-container" :class="{ focused: searchFocused }">
            <i class="pi pi-search search-icon"></i>
            <input
              v-model="searchQuery"
              type="text"
              :placeholder="t('common.search')"
              class="search-input"
              @focus="searchFocused = true"
              @blur="searchFocused = false"
              @keyup.enter="handleSearch"
            />
            <kbd class="search-shortcut" v-show="!searchFocused">⌘K</kbd>
          </div>
        </div>

        <div class="header-end">
          <!-- Language Toggle -->
          <button class="header-btn language-btn" @click="toggleLanguage">
            <i class="pi pi-globe"></i>
            <span>{{ uiStore.locale === 'en' ? 'عربي' : 'EN' }}</span>
          </button>

          <!-- Notifications -->
          <router-link to="/notifications" class="header-btn notification-btn">
            <i class="pi pi-bell"></i>
            <span class="notification-badge">3</span>
          </router-link>

          <!-- User Menu -->
          <button class="user-trigger" @click="toggleUserMenu">
            <Avatar
              :image="authStore.userAvatar"
              :name="authStore.userDisplayName || 'User'"
              shape="circle"
              class="trigger-avatar"
            />
            <div class="trigger-info">
              <span class="trigger-name">{{ authStore.userDisplayName }}</span>
              <span class="trigger-role">{{ authStore.user?.jobTitle || 'User' }}</span>
            </div>
            <i class="pi pi-chevron-down trigger-arrow"></i>
          </button>
          <Menu ref="userMenu" :model="userMenuItems" popup class="user-dropdown" />
        </div>
      </header>

      <!-- Page Content -->
      <main id="main-content" class="content" tabindex="-1">
        <router-view v-slot="{ Component, route: currentRoute }">
          <transition
            :name="prefersReducedMotion ? 'page-fade' : 'page-scale'"
            mode="out-in"
            appear
          >
            <component :is="Component" :key="currentRoute.path" />
          </transition>
        </router-view>
      </main>
    </div>
  </div>
</template>

<style lang="scss" scoped>
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

// ============================================
// LAYOUT WRAPPER
// ============================================

.layout-wrapper {
  display: flex;
  min-height: 100vh;
  background: $slate-50;
}

// ============================================
// SKIP LINK - ACCESSIBILITY
// ============================================

.skip-link {
  position: fixed;
  top: -100%;
  left: 50%;
  transform: translateX(-50%);
  z-index: 9999;
  padding: $spacing-3 $spacing-6;
  background: $intalio-teal-600;
  color: white;
  font-weight: $font-weight-semibold;
  font-size: $font-size-sm;
  border-radius: 0 0 $radius-lg $radius-lg;
  text-decoration: none;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.2);
  transition: top 0.2s ease;

  &:focus {
    top: 0;
    outline: none;
  }

  &:focus-visible {
    outline: 2px solid white;
    outline-offset: 2px;
  }
}

// ============================================
// SIDEBAR
// ============================================

.sidebar {
  width: $sidebar-width;
  height: 100vh;
  position: fixed;
  top: 0;
  inset-inline-start: 0;
  z-index: $z-fixed;
  display: flex;
  flex-direction: column;
  background: white;
  border-inline-end: 1px solid $slate-200;
  transition: width $duration-moderate $ease-out-expo;

  &.collapsed {
    width: $sidebar-collapsed-width;

    .nav-item {
      justify-content: center;
      padding: $spacing-3;

      .nav-label {
        display: none;
      }
    }

    .nav-section-header {
      display: none;
    }
  }
}

// ============================================
// SIDEBAR HEADER
// ============================================

.sidebar-header {
  height: $header-height;
  display: flex;
  align-items: center;
  padding: 0 $spacing-5;
  border-bottom: 1px solid $slate-100;
}

.logo-link {
  display: flex;
  align-items: center;
  text-decoration: none;
}

.logo-full {
  height: 36px;
  width: auto;
}

.logo-compact {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: $gradient-primary;
  border-radius: $radius-lg;
  color: white;
  font-weight: $font-weight-bold;
  font-size: $font-size-lg;
}

// ============================================
// SIDEBAR NAVIGATION
// ============================================

.sidebar-nav {
  flex: 1;
  padding: $spacing-4;
  overflow-y: auto;
  @include custom-scrollbar(4px, transparent, $slate-200);
}

.nav-group {
  display: flex;
  flex-direction: column;
  gap: $spacing-1;

  &.admin-group {
    margin-top: $spacing-6;
    padding-top: $spacing-4;
    border-top: 1px solid $slate-100;
  }
}

.nav-section-header {
  padding: $spacing-2 $spacing-4;
  margin-bottom: $spacing-2;

  span {
    font-size: $font-size-xs;
    font-weight: $font-weight-semibold;
    color: $slate-400;
    text-transform: uppercase;
    letter-spacing: 0.08em;
  }
}

.nav-divider {
  height: 1px;
  background: $slate-200;
  margin: $spacing-2 $spacing-3;
}

.nav-item {
  position: relative;
  display: flex;
  align-items: center;
  gap: $spacing-3;
  padding: $spacing-3 $spacing-4;
  border-radius: $radius-xl;
  color: $slate-600;
  text-decoration: none;
  font-size: $font-size-sm;
  font-weight: $font-weight-medium;
  transition:
    background $duration-fast $ease-default,
    color $duration-fast $ease-default,
    transform $duration-fast $ease-spring;

  &:hover {
    background: $slate-100;
    color: $slate-900;
    transform: translateX(4px);

    .nav-icon {
      color: $intalio-teal-600;
      transform: scale(1.1);
    }

    [dir='rtl'] & {
      transform: translateX(-4px);
    }
  }

  &:active {
    transform: translateX(2px) scale(0.98);

    [dir='rtl'] & {
      transform: translateX(-2px) scale(0.98);
    }
  }

  &.active {
    background: rgba($intalio-teal-500, 0.1);
    color: $intalio-teal-700;

    .nav-icon {
      color: $intalio-teal-600;
      background: rgba($intalio-teal-500, 0.15);
    }

    .nav-indicator {
      position: absolute;
      inset-inline-start: 0;
      top: 50%;
      transform: translateY(-50%);
      width: 3px;
      height: 24px;
      background: $gradient-primary;
      border-radius: 0 $radius-full $radius-full 0;
      box-shadow: 0 0 8px rgba($intalio-teal-500, 0.4);
      animation: indicatorPulse 2s ease-in-out infinite;
    }
  }
}

@keyframes indicatorPulse {
  0%, 100% {
    box-shadow: 0 0 8px rgba($intalio-teal-500, 0.4);
  }
  50% {
    box-shadow: 0 0 12px rgba($intalio-teal-500, 0.6);
  }
}

.nav-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: $radius-lg;
  color: $slate-500;
  transition:
    color $duration-fast $ease-default,
    background $duration-fast $ease-default,
    transform $duration-fast $ease-spring;

  i {
    font-size: 1.1rem;
  }
}

.nav-label {
  white-space: nowrap;
}

// ============================================
// SIDEBAR FOOTER
// ============================================

.sidebar-footer {
  padding: $spacing-4;
  border-top: 1px solid $slate-100;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: $spacing-3;
}

.user-quick {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  flex: 1;
  min-width: 0;
}

.user-avatar {
  flex-shrink: 0;
  width: 40px !important;
  height: 40px !important;
  background: $gradient-primary !important;
}

.user-info {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.user-name {
  font-size: $font-size-sm;
  font-weight: $font-weight-semibold;
  color: $slate-900;
  @include truncate;
}

.user-role {
  font-size: $font-size-xs;
  color: $slate-500;
  @include truncate;
}

.collapse-btn {
  @include flex-center;
  width: 32px;
  height: 32px;
  border-radius: $radius-lg;
  background: $slate-100;
  border: none;
  color: $slate-500;
  cursor: pointer;
  transition: all $transition-fast;
  flex-shrink: 0;

  &:hover {
    background: $slate-200;
    color: $slate-700;
  }

  i {
    font-size: 0.875rem;
  }
}

// ============================================
// MAIN WRAPPER
// ============================================

.main-wrapper {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  margin-inline-start: $sidebar-width;
  transition: margin $duration-moderate $ease-out-expo;

  .sidebar-collapsed & {
    margin-inline-start: $sidebar-collapsed-width;
  }
}

// ============================================
// HEADER - Enhanced with glassmorphism
// ============================================

.header {
  height: $header-height;
  background: linear-gradient(
    135deg,
    rgba(255, 255, 255, 0.95) 0%,
    rgba(255, 255, 255, 0.85) 100%
  );
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border-bottom: 1px solid rgba(255, 255, 255, 0.3);
  box-shadow:
    0 4px 30px rgba(0, 0, 0, 0.03),
    0 1px 3px rgba(0, 0, 0, 0.02);
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 $spacing-6;
  position: sticky;
  top: 0;
  z-index: $z-sticky;
  transition: box-shadow $duration-normal $ease-default;

  &:hover {
    box-shadow:
      0 4px 30px rgba(0, 0, 0, 0.05),
      0 1px 3px rgba(0, 0, 0, 0.03);
  }
}

.header-start {
  display: flex;
  align-items: center;
  gap: $spacing-4;
  flex: 1;
}

.mobile-menu-btn {
  display: none;
  @include flex-center;
  width: 40px;
  height: 40px;
  border-radius: $radius-lg;
  background: transparent;
  border: none;
  color: $slate-600;
  cursor: pointer;

  @media (max-width: $breakpoint-lg) {
    display: flex;
  }

  &:hover {
    background: $slate-100;
  }
}

// ============================================
// SEARCH BAR
// ============================================

.search-container {
  position: relative;
  max-width: 400px;
  width: 100%;

  &.focused {
    .search-input {
      background: white;
      border-color: $intalio-teal-500;
      box-shadow: $shadow-focus-ring;
    }

    .search-icon {
      color: $intalio-teal-600;
    }

    .search-shortcut {
      opacity: 0;
    }
  }
}

.search-icon {
  position: absolute;
  top: 50%;
  inset-inline-start: $spacing-4;
  transform: translateY(-50%);
  color: $slate-400;
  font-size: 1rem;
  pointer-events: none;
  transition: color $transition-fast;
}

.search-input {
  width: 100%;
  padding: $spacing-3 $spacing-4 $spacing-3 calc($spacing-4 + 1.5rem);
  font-size: $font-size-sm;
  color: $slate-900;
  background: $slate-100;
  border: 1.5px solid transparent;
  border-radius: $radius-xl;
  outline: none;
  transition: all $transition-fast;

  &::placeholder {
    color: $slate-400;
  }

  &:hover:not(:focus) {
    background: $slate-200;
  }
}

.search-shortcut {
  position: absolute;
  top: 50%;
  inset-inline-end: $spacing-3;
  transform: translateY(-50%);
  padding: $spacing-1 $spacing-2;
  font-size: $font-size-xs;
  font-family: inherit;
  color: $slate-400;
  background: white;
  border: 1px solid $slate-200;
  border-radius: $radius-sm;
  transition: opacity $transition-fast;
}

// ============================================
// HEADER END
// ============================================

.header-end {
  display: flex;
  align-items: center;
  gap: $spacing-2;
}

.header-btn {
  @include flex-center;
  gap: $spacing-2;
  padding: $spacing-2 $spacing-3;
  background: transparent;
  border: none;
  border-radius: $radius-lg;
  color: $slate-600;
  font-size: $font-size-sm;
  font-weight: $font-weight-medium;
  cursor: pointer;
  transition: all $transition-fast;

  i {
    font-size: 1.1rem;
  }

  &:hover {
    background: $slate-100;
    color: $slate-900;
  }

  &.language-btn {
    span {
      @media (max-width: $breakpoint-md) {
        display: none;
      }
    }
  }
}

.notification-btn {
  position: relative;
}

.notification-badge {
  position: absolute;
  top: 4px;
  inset-inline-end: 4px;
  min-width: 18px;
  height: 18px;
  padding: 0 $spacing-1;
  font-size: 10px;
  font-weight: $font-weight-bold;
  color: white;
  background: $error-500;
  border-radius: $radius-full;
  display: flex;
  align-items: center;
  justify-content: center;
}

// ============================================
// USER TRIGGER
// ============================================

.user-trigger {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  padding: $spacing-2 $spacing-3;
  background: transparent;
  border: 1px solid $slate-200;
  border-radius: $radius-xl;
  cursor: pointer;
  transition: all $transition-fast;

  &:hover {
    background: $slate-50;
    border-color: $slate-300;
  }
}

.trigger-avatar {
  width: 36px !important;
  height: 36px !important;
  background: $gradient-primary !important;
  flex-shrink: 0;
}

.trigger-info {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  min-width: 0;

  @media (max-width: $breakpoint-md) {
    display: none;
  }
}

.trigger-name {
  font-size: $font-size-sm;
  font-weight: $font-weight-semibold;
  color: $slate-900;
  line-height: 1.2;
}

.trigger-role {
  font-size: $font-size-xs;
  color: $slate-500;
}

.trigger-arrow {
  font-size: 0.75rem;
  color: $slate-400;

  @media (max-width: $breakpoint-md) {
    display: none;
  }
}

// ============================================
// CONTENT AREA
// ============================================

.content {
  flex: 1;
  padding: $spacing-6;
  background: linear-gradient(180deg, $slate-50 0%, $slate-100 100%);
  min-height: calc(100vh - $header-height);

  @media (max-width: $breakpoint-md) {
    padding: $spacing-4;
  }
}

// ============================================
// PAGE TRANSITIONS - Enhanced
// ============================================

// Scale transition (default)
.page-scale-enter-active {
  animation: pageEnterScale $duration-moderate $ease-out-quart;
}

.page-scale-leave-active {
  animation: pageLeaveScale $duration-fast ease-in;
}

@keyframes pageEnterScale {
  from {
    opacity: 0;
    transform: scale(0.96) translateY(10px);
  }
  to {
    opacity: 1;
    transform: scale(1) translateY(0);
  }
}

@keyframes pageLeaveScale {
  from {
    opacity: 1;
    transform: scale(1) translateY(0);
  }
  to {
    opacity: 0;
    transform: scale(0.98) translateY(-5px);
  }
}

// Fade transition (for reduced motion)
.page-fade-enter-active {
  animation: pageFadeIn $duration-fast $ease-out-quart;
}

.page-fade-leave-active {
  animation: pageFadeOut $duration-fast ease-in;
}

@keyframes pageFadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes pageFadeOut {
  from {
    opacity: 1;
  }
  to {
    opacity: 0;
  }
}

// Legacy page transition support
.page-enter-active {
  animation: pageFadeIn $duration-fast $ease-out-quart;
}

.page-leave-active {
  animation: pageFadeOut $duration-fast ease-in;
}

// ============================================
// RESPONSIVE
// ============================================

@media (max-width: $breakpoint-lg) {
  .sidebar {
    transform: translateX(-100%);
    box-shadow: $shadow-2xl;

    &:not(.collapsed) {
      transform: translateX(0);
    }
  }

  .main-wrapper {
    margin-inline-start: 0;
  }

  .sidebar-collapsed .main-wrapper {
    margin-inline-start: 0;
  }
}

// ============================================
// RTL SUPPORT
// ============================================

[dir="rtl"] {
  .nav-item.active .nav-indicator {
    inset-inline-start: auto;
    inset-inline-end: 0;
    border-radius: $radius-full 0 0 $radius-full;
  }

  @media (max-width: $breakpoint-lg) {
    .sidebar {
      transform: translateX(100%);

      &:not(.collapsed) {
        transform: translateX(0);
      }
    }
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================

.reduced-motion,
.layout-wrapper.reduced-motion {
  .nav-item {
    transition: background $duration-fast $ease-default, color $duration-fast $ease-default !important;

    &:hover,
    &:active {
      transform: none !important;
    }
  }

  .nav-icon {
    transition: color $duration-fast $ease-default, background $duration-fast $ease-default !important;

    &:hover {
      transform: none !important;
    }
  }

  .nav-indicator {
    animation: none !important;
  }

  .sidebar {
    transition: width $duration-fast $ease-default !important;
  }
}

@media (prefers-reduced-motion: reduce) {
  .layout-wrapper {
    .nav-item,
    .nav-icon,
    .sidebar,
    .main-wrapper {
      transition-duration: 0.01ms !important;
      animation-duration: 0.01ms !important;
    }

    .nav-indicator {
      animation: none !important;
    }
  }
}
</style>
