<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'

const { locale } = useI18n()
const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

// RTL support
const isRTL = computed(() => locale.value === 'ar')

// Sidebar state
const sidebarOpen = ref(true)
const isMobile = ref(false)

// Check viewport on mount and resize
const checkViewport = () => {
  isMobile.value = window.innerWidth < 1024
  if (isMobile.value) {
    sidebarOpen.value = false
  }
}

// Toggle sidebar
const toggleSidebar = () => {
  sidebarOpen.value = !sidebarOpen.value
}

// Close sidebar on mobile when route changes
watch(() => route.path, () => {
  if (isMobile.value) {
    sidebarOpen.value = false
  }
})

// Initialize viewport check
if (typeof window !== 'undefined') {
  checkViewport()
  window.addEventListener('resize', checkViewport)
}

// Navigation sections
const navSections = computed(() => [
  {
    id: 'management',
    title: isRTL.value ? 'إدارة المستخدمين' : 'User Management',
    items: [
      {
        id: 'users',
        label: isRTL.value ? 'المستخدمين' : 'Users',
        icon: 'pi pi-users',
        route: '/admin/users',
        badge: 156
      },
      {
        id: 'roles',
        label: isRTL.value ? 'الأدوار' : 'Roles',
        icon: 'pi pi-shield',
        route: '/admin/roles',
        badge: 8
      },
      {
        id: 'groups',
        label: isRTL.value ? 'المجموعات' : 'Groups',
        icon: 'pi pi-sitemap',
        route: '/admin/groups',
        badge: 12
      }
    ]
  },
  {
    id: 'system',
    title: isRTL.value ? 'إعدادات النظام' : 'System',
    items: [
      {
        id: 'settings',
        label: isRTL.value ? 'الإعدادات' : 'Settings',
        icon: 'pi pi-cog',
        route: '/admin/settings'
      },
      {
        id: 'audit',
        label: isRTL.value ? 'سجل التدقيق' : 'Audit Log',
        icon: 'pi pi-history',
        route: '/admin/audit'
      },
      {
        id: 'integrations',
        label: isRTL.value ? 'التكاملات' : 'Integrations',
        icon: 'pi pi-link',
        route: '/admin/integrations'
      }
    ]
  },
  {
    id: 'security',
    title: isRTL.value ? 'الأمان' : 'Security',
    items: [
      {
        id: 'security-settings',
        label: isRTL.value ? 'إعدادات الأمان' : 'Security',
        icon: 'pi pi-lock',
        route: '/admin/security'
      },
      {
        id: 'sessions',
        label: isRTL.value ? 'الجلسات' : 'Sessions',
        icon: 'pi pi-desktop',
        route: '/admin/sessions',
        badge: 24
      }
    ]
  }
])

// Check if route is active
const isActive = (itemRoute: string) => route.path === itemRoute

// Get page title based on current route
const pageTitle = computed(() => {
  const routeToTitle: Record<string, { en: string; ar: string }> = {
    '/admin/users': { en: 'Users', ar: 'المستخدمين' },
    '/admin/roles': { en: 'Roles', ar: 'الأدوار' },
    '/admin/groups': { en: 'Groups', ar: 'المجموعات' },
    '/admin/settings': { en: 'Settings', ar: 'الإعدادات' },
    '/admin/audit': { en: 'Audit Log', ar: 'سجل التدقيق' },
    '/admin/integrations': { en: 'Integrations', ar: 'التكاملات' },
    '/admin/security': { en: 'Security', ar: 'الأمان' },
    '/admin/sessions': { en: 'Sessions', ar: 'الجلسات' }
  }
  const title = routeToTitle[route.path]
  return title ? (isRTL.value ? title.ar : title.en) : (isRTL.value ? 'الإدارة' : 'Admin')
})

// Current user
const currentUser = computed(() => ({
  name: isRTL.value
    ? (authStore.user?.displayNameArabic || authStore.user?.displayName || 'Admin User')
    : (authStore.user?.displayName || 'Admin User'),
  role: isRTL.value ? 'مدير النظام' : 'System Admin',
  avatar: authStore.user?.avatarUrl || 'https://api.dicebear.com/7.x/avataaars/svg?seed=Admin'
}))

// Navigate
const navigateTo = (path: string) => {
  router.push(path)
}
</script>

<template>
  <div class="admin-layout" :class="{ 'rtl': isRTL, 'sidebar-open': sidebarOpen }">
    <!-- Sidebar Overlay (Mobile) -->
    <div
      v-if="isMobile"
      class="sidebar-overlay"
      :class="{ visible: sidebarOpen }"
      @click="sidebarOpen = false"
    ></div>

    <!-- Sidebar -->
    <aside class="admin-sidebar" :class="{ open: sidebarOpen }">
      <!-- Sidebar Header -->
      <div class="sidebar-header">
        <div class="sidebar-logo">
          <i class="pi pi-shield"></i>
        </div>
        <div class="sidebar-brand">
          <h1 class="brand-title">{{ isRTL ? 'لوحة الإدارة' : 'Admin Panel' }}</h1>
          <p class="brand-subtitle">AFC 2027 KMS</p>
        </div>
      </div>

      <!-- Navigation -->
      <nav class="sidebar-nav">
        <div v-for="section in navSections" :key="section.id" class="nav-section">
          <h3 class="nav-section-title">{{ section.title }}</h3>
          <ul class="nav-list">
            <li v-for="item in section.items" :key="item.id">
              <button
                class="nav-item"
                :class="{ active: isActive(item.route) }"
                @click="navigateTo(item.route)"
              >
                <span class="nav-icon">
                  <i :class="item.icon"></i>
                </span>
                <span class="nav-text">{{ item.label }}</span>
                <span v-if="item.badge" class="nav-badge">{{ item.badge }}</span>
              </button>
            </li>
          </ul>
        </div>
      </nav>

      <!-- Sidebar Footer -->
      <div class="sidebar-footer">
        <button class="sidebar-user" @click="navigateTo('/profile')">
          <div class="user-avatar">
            <img :src="currentUser.avatar" :alt="currentUser.name" />
          </div>
          <div class="user-info">
            <p class="user-name">{{ currentUser.name }}</p>
            <p class="user-role">{{ currentUser.role }}</p>
          </div>
          <i class="pi pi-chevron-right user-menu-icon"></i>
        </button>
      </div>
    </aside>

    <!-- Main Content Area -->
    <main class="admin-main">
      <!-- Top Bar -->
      <header class="admin-topbar">
        <div class="topbar-left">
          <button class="menu-toggle" @click="toggleSidebar">
            <i class="pi pi-bars"></i>
          </button>

          <nav class="topbar-breadcrumb">
            <button @click="navigateTo('/')">
              <i class="pi pi-home"></i>
            </button>
            <span class="separator">/</span>
            <button @click="navigateTo('/admin/users')">
              {{ isRTL ? 'الإدارة' : 'Admin' }}
            </button>
            <span class="separator">/</span>
            <span class="current">{{ pageTitle }}</span>
          </nav>
        </div>

        <div class="topbar-right">
          <button class="topbar-action" :title="isRTL ? 'الإشعارات' : 'Notifications'">
            <i class="pi pi-bell"></i>
            <span class="badge">3</span>
          </button>
          <button class="topbar-action" :title="isRTL ? 'المساعدة' : 'Help'">
            <i class="pi pi-question-circle"></i>
          </button>
          <button class="topbar-action" @click="navigateTo('/')" :title="isRTL ? 'الخروج' : 'Exit Admin'">
            <i class="pi pi-sign-out"></i>
          </button>
        </div>
      </header>

      <!-- Page Content -->
      <div class="admin-content">
        <router-view />
      </div>
    </main>
  </div>
</template>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

// ============================================
// ADMIN LAYOUT - Using new admin mixins
// ============================================

.admin-layout {
  @include admin-layout;

  &.rtl {
    direction: rtl;

    .user-menu-icon {
      transform: rotate(180deg);
    }
  }
}

// ============================================
// SIDEBAR OVERLAY
// ============================================

.sidebar-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 99;
  opacity: 0;
  visibility: hidden;
  transition: all $transition-base;

  &.visible {
    opacity: 1;
    visibility: visible;
  }
}

// ============================================
// ADMIN SIDEBAR
// ============================================

.admin-sidebar {
  @include admin-sidebar;
}

.sidebar-header {
  @include admin-sidebar-header;
}

.sidebar-logo {
  @include admin-sidebar-logo;
}

.sidebar-brand {
  @include admin-sidebar-brand;
}

.sidebar-nav {
  @include admin-sidebar-nav;
}

.nav-section {
  @include admin-nav-section;
}

.nav-section-title {
  @include admin-nav-section-title;
}

.nav-list {
  list-style: none;
  margin: 0;
  padding: 0;
}

.nav-item {
  @include admin-nav-item;
}

.sidebar-footer {
  @include admin-sidebar-footer;
}

.sidebar-user {
  @include admin-sidebar-user;
}

// ============================================
// ADMIN MAIN
// ============================================

.admin-main {
  @include admin-main;
}

.admin-topbar {
  @include admin-topbar;
}

.topbar-left {
  @include admin-topbar-left;
}

.menu-toggle {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: none;
  border-radius: $radius-lg;
  color: $slate-500;
  cursor: pointer;
  transition: all $transition-fast;

  &:hover {
    background: $slate-100;
    color: $slate-700;
  }
}

.topbar-breadcrumb {
  @include admin-topbar-breadcrumb;
}

.topbar-right {
  @include admin-topbar-right;
}

.topbar-action {
  @include admin-topbar-action;
}

.admin-content {
  @include admin-content;
}

// ============================================
// RESPONSIVE
// ============================================

@media (max-width: $breakpoint-lg) {
  .admin-sidebar {
    transform: translateX(-100%);
    box-shadow: $shadow-2xl;

    [dir='rtl'] & {
      transform: translateX(100%);
    }

    &.open {
      transform: translateX(0);
    }
  }

  .admin-main {
    margin-inline-start: 0;
  }
}

@include mobile {
  .admin-topbar {
    padding: 0 $spacing-4;
  }

  .admin-content {
    padding: $spacing-4;
  }

  .topbar-breadcrumb {
    display: none;
  }
}

// ============================================
// ANIMATIONS
// ============================================

.nav-section {
  animation: fadeInUp 0.3s ease-out forwards;
  opacity: 0;

  @for $i from 1 through 4 {
    &:nth-child(#{$i}) {
      animation-delay: #{$i * 0.08}s;
    }
  }
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
