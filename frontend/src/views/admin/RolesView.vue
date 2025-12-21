<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Tag from 'primevue/tag'
import Dialog from 'primevue/dialog'
import Checkbox from 'primevue/checkbox'
import Menu from 'primevue/menu'
import Skeleton from 'primevue/skeleton'

// Base Components
import ErrorState from '@/components/base/ErrorState.vue'
import { PageHeader, StatsBar, EmptyState } from '@/components/base'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import type { StatItem } from '@/components/base/StatsBar.vue'

const { locale } = useI18n()
const prefersReducedMotion = useReducedMotion()

// RTL support
const isRTL = computed(() => locale.value === 'ar')

// Animation control
const isContentVisible = ref(false)
const shouldAnimate = computed(() => !prefersReducedMotion.value)

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: isRTL.value ? 'إدارة الأدوار' : 'Role Management' }
])

// State
const isLoading = ref(true)
const error = ref<Error | null>(null)
const searchQuery = ref('')
const showRoleDialog = ref(false)
const showDeleteDialog = ref(false)
const showViewDialog = ref(false)
const viewMode = ref<'grid' | 'list'>('grid')
const selectedRoles = ref<string[]>([])

// Pagination
const currentPage = ref(1)
const itemsPerPage = ref(10)

// Filter type
const filterType = ref<'all' | 'system' | 'custom'>('all')

// Type toggle options
const typeOptions = computed(() => [
  { value: 'all', label: isRTL.value ? 'الكل' : 'All', icon: 'pi-list' },
  { value: 'system', label: isRTL.value ? 'نظام' : 'System', icon: 'pi-lock' },
  { value: 'custom', label: isRTL.value ? 'مخصص' : 'Custom', icon: 'pi-cog' }
])

// Per page options
const perPageOptions = [
  { value: 5, label: '5' },
  { value: 10, label: '10' },
  { value: 20, label: '20' },
  { value: 50, label: '50' }
]

// Types
interface Role {
  id: string
  name: string
  nameArabic: string
  description: string
  descriptionArabic: string
  usersCount: number
  permissions: string[]
  type: 'system' | 'custom'
  color: string
  icon: string
  createdAt: Date
}

// Roles data
const roles = ref<Role[]>([
  {
    id: '1',
    name: 'Administrator',
    nameArabic: 'مدير النظام',
    description: 'Full system access with all permissions',
    descriptionArabic: 'وصول كامل للنظام مع جميع الصلاحيات',
    usersCount: 5,
    permissions: ['read', 'write', 'delete', 'admin', 'manage_users', 'manage_roles'],
    type: 'system',
    color: '#ef4444',
    icon: 'pi-shield',
    createdAt: new Date(Date.now() - 365 * 24 * 60 * 60 * 1000)
  },
  {
    id: '2',
    name: 'Editor',
    nameArabic: 'محرر',
    description: 'Can create, edit and publish content',
    descriptionArabic: 'يمكنه إنشاء وتحرير ونشر المحتوى',
    usersCount: 15,
    permissions: ['read', 'write', 'publish'],
    type: 'system',
    color: '#3b82f6',
    icon: 'pi-pencil',
    createdAt: new Date(Date.now() - 300 * 24 * 60 * 60 * 1000)
  },
  {
    id: '3',
    name: 'Viewer',
    nameArabic: 'مشاهد',
    description: 'Read-only access to content',
    descriptionArabic: 'وصول للقراءة فقط',
    usersCount: 150,
    permissions: ['read'],
    type: 'system',
    color: '#64748b',
    icon: 'pi-eye',
    createdAt: new Date(Date.now() - 280 * 24 * 60 * 60 * 1000)
  },
  {
    id: '4',
    name: 'Moderator',
    nameArabic: 'مشرف',
    description: 'Can moderate discussions and user content',
    descriptionArabic: 'يمكنه الإشراف على المناقشات ومحتوى المستخدمين',
    usersCount: 8,
    permissions: ['read', 'write', 'moderate', 'delete_comments'],
    type: 'custom',
    color: '#8b5cf6',
    icon: 'pi-flag',
    createdAt: new Date(Date.now() - 200 * 24 * 60 * 60 * 1000)
  },
  {
    id: '5',
    name: 'Content Manager',
    nameArabic: 'مدير المحتوى',
    description: 'Manages all content and media assets',
    descriptionArabic: 'يدير جميع المحتوى والوسائط',
    usersCount: 12,
    permissions: ['read', 'write', 'delete', 'publish', 'manage_media'],
    type: 'custom',
    color: '#10b981',
    icon: 'pi-folder',
    createdAt: new Date(Date.now() - 150 * 24 * 60 * 60 * 1000)
  },
  {
    id: '6',
    name: 'Analyst',
    nameArabic: 'محلل',
    description: 'Access to reports and analytics',
    descriptionArabic: 'الوصول للتقارير والتحليلات',
    usersCount: 6,
    permissions: ['read', 'view_reports', 'export_data'],
    type: 'custom',
    color: '#f59e0b',
    icon: 'pi-chart-bar',
    createdAt: new Date(Date.now() - 100 * 24 * 60 * 60 * 1000)
  },
  {
    id: '7',
    name: 'Support Agent',
    nameArabic: 'وكيل دعم',
    description: 'Can handle support tickets and user queries',
    descriptionArabic: 'يمكنه التعامل مع تذاكر الدعم واستفسارات المستخدمين',
    usersCount: 20,
    permissions: ['read', 'write', 'manage_tickets'],
    type: 'custom',
    color: '#06b6d4',
    icon: 'pi-headphones',
    createdAt: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000)
  },
  {
    id: '8',
    name: 'Guest',
    nameArabic: 'زائر',
    description: 'Limited access for external users',
    descriptionArabic: 'وصول محدود للمستخدمين الخارجيين',
    usersCount: 45,
    permissions: ['read_public'],
    type: 'system',
    color: '#94a3b8',
    icon: 'pi-user',
    createdAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000)
  }
])

// Computed stats for StatsBar
const stats = computed<StatItem[]>(() => {
  const total = roles.value.length
  const systemRoles = roles.value.filter(r => r.type === 'system').length
  const customRoles = roles.value.filter(r => r.type === 'custom').length
  const totalUsers = roles.value.reduce((sum, r) => sum + r.usersCount, 0)

  return [
    {
      icon: 'pi pi-shield',
      value: total,
      label: 'Total Roles',
      labelArabic: 'إجمالي الأدوار',
      colorClass: 'primary'
    },
    {
      icon: 'pi pi-lock',
      value: systemRoles,
      label: 'System Roles',
      labelArabic: 'أدوار النظام',
      colorClass: 'info'
    },
    {
      icon: 'pi pi-cog',
      value: customRoles,
      label: 'Custom Roles',
      labelArabic: 'أدوار مخصصة',
      colorClass: 'success'
    },
    {
      icon: 'pi pi-users',
      value: totalUsers,
      label: 'Assigned Users',
      labelArabic: 'المستخدمين المعينين',
      colorClass: 'warning'
    }
  ]
})

// Active filters count
const activeFiltersCount = computed(() => {
  let count = 0
  if (filterType.value !== 'all') count++
  return count
})

// Clear all filters
const clearAllFilters = () => {
  filterType.value = 'all'
  searchQuery.value = ''
  currentPage.value = 1
}

// Reset pagination when filters change
const onFilterChange = () => {
  currentPage.value = 1
}

// Filtered roles
const filteredRoles = computed(() => {
  let result = [...roles.value]

  // Type filter
  if (filterType.value !== 'all') {
    result = result.filter(r => r.type === filterType.value)
  }

  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(r =>
      r.name.toLowerCase().includes(query) ||
      r.nameArabic.includes(query) ||
      r.description.toLowerCase().includes(query)
    )
  }

  return result
})

// Pagination computed properties
const totalPages = computed(() => Math.ceil(filteredRoles.value.length / itemsPerPage.value))

const paginatedRoles = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredRoles.value.slice(start, end)
})

const paginationInfo = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value + 1
  const end = Math.min(currentPage.value * itemsPerPage.value, filteredRoles.value.length)
  const total = filteredRoles.value.length
  return { start, end, total }
})

const visiblePages = computed(() => {
  const pages: (number | string)[] = []
  const total = totalPages.value
  const current = currentPage.value

  if (total <= 7) {
    for (let i = 1; i <= total; i++) pages.push(i)
  } else {
    pages.push(1)
    if (current > 3) pages.push('...')

    const start = Math.max(2, current - 1)
    const end = Math.min(total - 1, current + 1)

    for (let i = start; i <= end; i++) pages.push(i)

    if (current < total - 2) pages.push('...')
    pages.push(total)
  }

  return pages
})

const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
  }
}

// Helper functions
function getName(role: Role) {
  return isRTL.value ? role.nameArabic : role.name
}

function getDescription(role: Role) {
  return isRTL.value ? role.descriptionArabic : role.description
}

function getTypeLabel(type: string) {
  const labels: Record<string, { en: string; ar: string }> = {
    system: { en: 'System', ar: 'نظام' },
    custom: { en: 'Custom', ar: 'مخصص' }
  }
  return isRTL.value ? labels[type]?.ar : labels[type]?.en
}

function getTypeSeverity(type: string): "info" | "success" | "secondary" {
  return type === 'system' ? 'info' : 'success'
}

function getPermissionLabel(permission: string) {
  const labels: Record<string, { en: string; ar: string }> = {
    read: { en: 'Read', ar: 'قراءة' },
    write: { en: 'Write', ar: 'كتابة' },
    delete: { en: 'Delete', ar: 'حذف' },
    admin: { en: 'Admin', ar: 'إدارة' },
    publish: { en: 'Publish', ar: 'نشر' },
    moderate: { en: 'Moderate', ar: 'إشراف' },
    manage_users: { en: 'Manage Users', ar: 'إدارة المستخدمين' },
    manage_roles: { en: 'Manage Roles', ar: 'إدارة الأدوار' },
    manage_media: { en: 'Manage Media', ar: 'إدارة الوسائط' },
    manage_tickets: { en: 'Manage Tickets', ar: 'إدارة التذاكر' },
    view_reports: { en: 'View Reports', ar: 'عرض التقارير' },
    export_data: { en: 'Export Data', ar: 'تصدير البيانات' },
    delete_comments: { en: 'Delete Comments', ar: 'حذف التعليقات' },
    read_public: { en: 'Read Public', ar: 'قراءة عامة' }
  }
  return isRTL.value ? labels[permission]?.ar || permission : labels[permission]?.en || permission
}

// Selection
function toggleSelect(id: string) {
  const index = selectedRoles.value.indexOf(id)
  if (index === -1) {
    selectedRoles.value.push(id)
  } else {
    selectedRoles.value.splice(index, 1)
  }
}

function isSelected(id: string) {
  return selectedRoles.value.includes(id)
}

// Action menu
const actionMenu = ref()
const currentRole = ref<Role | null>(null)

const actionMenuItems = computed(() => [
  { label: isRTL.value ? 'عرض التفاصيل' : 'View Details', icon: 'pi pi-eye', command: () => viewRole(currentRole.value) },
  { label: isRTL.value ? 'تعديل' : 'Edit', icon: 'pi pi-pencil', command: () => editRole(currentRole.value) },
  { label: isRTL.value ? 'نسخ' : 'Duplicate', icon: 'pi pi-copy', command: () => duplicateRole(currentRole.value) },
  { separator: true },
  { label: isRTL.value ? 'حذف' : 'Delete', icon: 'pi pi-trash', class: 'text-red-500', command: () => deleteRole(currentRole.value) }
])

function toggleActionMenu(event: Event, role: Role) {
  currentRole.value = role
  actionMenu.value.toggle(event)
}

function viewRole(role: Role | null) {
  if (role) {
    currentRole.value = role
    showViewDialog.value = true
  }
}

function editRole(role: Role | null) {
  if (role) {
    currentRole.value = role
  }
  showRoleDialog.value = true
}

function duplicateRole(role: Role | null) {
  if (role) {
    const newRole: Role = {
      ...role,
      id: `role-${Date.now()}`,
      name: `${role.name} (Copy)`,
      nameArabic: `${role.nameArabic} (نسخة)`,
      type: 'custom',
      usersCount: 0
    }
    roles.value.push(newRole)
  }
}

function deleteRole(role: Role | null) {
  if (role) {
    currentRole.value = role
    showDeleteDialog.value = true
  }
}

function confirmDeleteRole() {
  if (currentRole.value) {
    roles.value = roles.value.filter(r => r.id !== currentRole.value?.id)
    showDeleteDialog.value = false
    currentRole.value = null
  }
}

function addRole() {
  currentRole.value = null
  showRoleDialog.value = true
}

// Data loading with error handling
async function loadRoles() {
  try {
    error.value = null
    await new Promise(resolve => setTimeout(resolve, 600))
    isLoading.value = false

    // Trigger entrance animations
    if (shouldAnimate.value) {
      requestAnimationFrame(() => {
        isContentVisible.value = true
      })
    } else {
      isContentVisible.value = true
    }
  } catch (e) {
    error.value = e as Error
    isLoading.value = false
  }
}

function handleRetry() {
  isLoading.value = true
  isContentVisible.value = false
  loadRoles()
}

// Lifecycle
onMounted(() => {
  loadRoles()
})
</script>

<template>
  <div class="roles-view" :class="{ 'rtl': isRTL }">
    <!-- Error State -->
    <ErrorState
      v-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل الأدوار' : 'Failed to load roles'"
      show-retry
      size="lg"
      @retry="handleRetry"
    />

    <template v-else>
    <!-- Page Header -->
    <PageHeader
      :title="isRTL ? 'إدارة الأدوار' : 'Role Management'"
      :description="isRTL ? 'إدارة الأدوار والصلاحيات في النظام' : 'Manage roles and permissions in the system'"
      :breadcrumbs="breadcrumbs"
      padding-bottom="xl"
      background-variant="gradient"
      :animated="shouldAnimate"
    >
      <template #actions>
        <Button
          :label="isRTL ? 'تصدير' : 'Export'"
          icon="pi pi-download"
          severity="secondary"
        />
        <Button
          :label="isRTL ? 'إضافة دور' : 'Add Role'"
          icon="pi pi-plus"
          @click="addRole"
        />
      </template>
    </PageHeader>

    <!-- Stats Bar -->
    <StatsBar
      :stats="stats"
      :loading="isLoading"
      overlap="lg"
      :animated="shouldAnimate"
      :animate-numbers="shouldAnimate"
      :counter-duration="1200"
    />

    <!-- Main Content -->
    <div v-if="isLoading" class="main-content">
      <div class="loading-state">
        <Skeleton height="60px" class="mb-4" borderRadius="16px" />
        <div class="roles-grid">
          <Skeleton v-for="i in 6" :key="i" height="220px" borderRadius="16px" />
        </div>
      </div>
    </div>

    <div v-else class="main-content">
      <!-- Toolbar -->
      <div class="toolbar">
        <div class="toolbar-row">
          <div class="toolbar-left">
            <!-- Search -->
            <div class="search-box">
              <i class="pi pi-search"></i>
              <InputText
                v-model="searchQuery"
                :placeholder="isRTL ? 'البحث عن أدوار...' : 'Search roles...'"
                class="search-input"
                @input="onFilterChange"
              />
            </div>

            <!-- Type Toggle -->
            <div class="type-toggle">
              <button
                v-for="option in typeOptions"
                :key="option.value"
                :class="{ active: filterType === option.value }"
                @click="filterType = option.value as 'all' | 'system' | 'custom'; onFilterChange()"
              >
                <i :class="['pi', option.icon]"></i>
                <span>{{ option.label }}</span>
              </button>
            </div>

            <!-- Bulk Actions -->
            <div class="bulk-actions" v-if="selectedRoles.length > 0">
              <span class="selection-count">
                {{ selectedRoles.length }} {{ isRTL ? 'محدد' : 'selected' }}
              </span>
              <Button icon="pi pi-trash" text size="small" severity="danger" v-tooltip.top="isRTL ? 'حذف' : 'Delete'" />
            </div>
          </div>

          <div class="toolbar-right">
            <!-- Items Per Page -->
            <div class="per-page-control">
              <span class="per-page-label">{{ isRTL ? 'عرض' : 'Show' }}</span>
              <select
                v-model="itemsPerPage"
                @change="onFilterChange"
                class="per-page-select"
              >
                <option v-for="option in perPageOptions" :key="option.value" :value="option.value">
                  {{ option.label }}
                </option>
              </select>
            </div>

            <!-- View Toggle -->
            <div class="view-toggle">
              <button
                :class="{ active: viewMode === 'list' }"
                @click="viewMode = 'list'"
                :title="isRTL ? 'عرض القائمة' : 'List View'"
              >
                <i class="pi pi-list"></i>
              </button>
              <button
                :class="{ active: viewMode === 'grid' }"
                @click="viewMode = 'grid'"
                :title="isRTL ? 'عرض الشبكة' : 'Grid View'"
              >
                <i class="pi pi-th-large"></i>
              </button>
            </div>
          </div>
        </div>

        <!-- Active Filters Tags -->
        <div v-if="activeFiltersCount > 0 || searchQuery" class="active-filters">
          <span class="active-filters-label">{{ isRTL ? 'التصفية النشطة:' : 'Active filters:' }}</span>

          <span v-if="searchQuery" class="filter-tag">
            <i class="pi pi-search"></i>
            "{{ searchQuery }}"
            <button @click="searchQuery = ''; onFilterChange()"><i class="pi pi-times"></i></button>
          </span>

          <span v-if="filterType !== 'all'" class="filter-tag">
            <i :class="['pi', typeOptions.find(o => o.value === filterType)?.icon]"></i>
            {{ typeOptions.find(o => o.value === filterType)?.label }}
            <button @click="filterType = 'all'; onFilterChange()"><i class="pi pi-times"></i></button>
          </span>

          <button class="clear-all-link" @click="clearAllFilters">
            {{ isRTL ? 'مسح الكل' : 'Clear all' }}
          </button>
        </div>
      </div>

      <!-- Roles List View -->
      <div
        v-if="viewMode === 'list'"
        class="roles-table"
        :class="{
          'roles-table--animated': shouldAnimate,
          'roles-table--visible': isContentVisible
        }"
      >
        <div class="table-header">
          <div class="table-cell checkbox-cell">
            <Checkbox :binary="true" />
          </div>
          <div class="table-cell role-cell">{{ isRTL ? 'الدور' : 'Role' }}</div>
          <div class="table-cell type-cell">{{ isRTL ? 'النوع' : 'Type' }}</div>
          <div class="table-cell users-cell">{{ isRTL ? 'المستخدمين' : 'Users' }}</div>
          <div class="table-cell permissions-cell">{{ isRTL ? 'الصلاحيات' : 'Permissions' }}</div>
          <div class="table-cell actions-cell">{{ isRTL ? 'إجراءات' : 'Actions' }}</div>
        </div>

        <div
          v-for="(role, index) in paginatedRoles"
          :key="role.id"
          class="table-row"
          :class="{ selected: isSelected(role.id) }"
          :style="shouldAnimate ? { '--row-index': index } : undefined"
        >
          <div class="table-cell checkbox-cell" @click.stop>
            <Checkbox
              :modelValue="isSelected(role.id)"
              :binary="true"
              @change="toggleSelect(role.id)"
            />
          </div>
          <div class="table-cell role-cell">
            <div class="role-info">
              <div class="role-icon" :style="{ background: role.color + '20', color: role.color }">
                <i class="pi pi-shield"></i>
              </div>
              <div class="role-details">
                <span class="role-name">{{ getName(role) }}</span>
                <span class="role-description">{{ getDescription(role) }}</span>
              </div>
            </div>
          </div>
          <div class="table-cell type-cell">
            <Tag :value="getTypeLabel(role.type)" :severity="getTypeSeverity(role.type)" />
          </div>
          <div class="table-cell users-cell">
            <span class="users-count">{{ role.usersCount }}</span>
          </div>
          <div class="table-cell permissions-cell">
            <div class="permissions-tags">
              <Tag
                v-for="perm in role.permissions.slice(0, 3)"
                :key="perm"
                :value="getPermissionLabel(perm)"
                severity="secondary"
                class="perm-tag"
              />
              <span v-if="role.permissions.length > 3" class="more-perms">
                +{{ role.permissions.length - 3 }}
              </span>
            </div>
          </div>
          <div class="table-cell actions-cell">
            <Button
              icon="pi pi-ellipsis-v"
              text
              rounded
              size="small"
              class="action-menu-btn"
              @click.stop="(e: Event) => toggleActionMenu(e, role)"
            />
          </div>
        </div>

        <Menu ref="actionMenu" :model="actionMenuItems" popup class="action-menu" />
      </div>

      <!-- Roles Grid View -->
      <div
        v-else
        class="roles-grid"
        :class="{
          'roles-grid--animated': shouldAnimate,
          'roles-grid--visible': isContentVisible
        }"
      >
        <div
          v-for="(role, index) in paginatedRoles"
          :key="role.id"
          class="role-card"
          :class="{ selected: isSelected(role.id) }"
          :style="shouldAnimate ? { '--card-index': index } : undefined"
        >
          <div class="card-select" @click.stop>
            <Checkbox
              :modelValue="isSelected(role.id)"
              :binary="true"
              @change="toggleSelect(role.id)"
            />
          </div>

          <!-- Card Header -->
          <div class="card-header">
            <div class="role-icon large" :style="{ background: role.color + '20', color: role.color }">
              <i class="pi pi-shield"></i>
            </div>
            <Tag :value="getTypeLabel(role.type)" :severity="getTypeSeverity(role.type)" class="type-tag" />
          </div>

          <!-- Card Info -->
          <div class="card-info">
            <h3 class="role-name">{{ getName(role) }}</h3>
            <p class="role-description">{{ getDescription(role) }}</p>
          </div>

          <!-- Permissions -->
          <div class="card-permissions">
            <span class="permissions-label">{{ isRTL ? 'الصلاحيات:' : 'Permissions:' }}</span>
            <div class="permissions-list">
              <Tag
                v-for="perm in role.permissions.slice(0, 4)"
                :key="perm"
                :value="getPermissionLabel(perm)"
                severity="secondary"
                class="perm-tag"
              />
              <span v-if="role.permissions.length > 4" class="more-perms">
                +{{ role.permissions.length - 4 }} {{ isRTL ? 'أخرى' : 'more' }}
              </span>
            </div>
          </div>

          <!-- Card Footer -->
          <div class="card-footer">
            <div class="users-info">
              <i class="pi pi-users"></i>
              <span>{{ role.usersCount }} {{ isRTL ? 'مستخدم' : 'users' }}</span>
            </div>
            <div class="card-actions">
              <Button
                icon="pi pi-pencil"
                text
                rounded
                size="small"
                @click.stop="editRole(role)"
                :title="isRTL ? 'تعديل' : 'Edit'"
              />
              <Button
                icon="pi pi-ellipsis-v"
                text
                rounded
                size="small"
                @click.stop="(e: Event) => toggleActionMenu(e, role)"
                :title="isRTL ? 'المزيد' : 'More'"
              />
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <EmptyState
        v-if="filteredRoles.length === 0"
        icon="pi-shield"
        title="No Roles Found"
        title-arabic="لم يتم العثور على أدوار"
        description="Try adjusting your search criteria"
        description-arabic="حاول تعديل معايير البحث"
        action-label="Clear Filters"
        action-label-arabic="مسح الفلاتر"
        action-icon="pi-times"
        variant="search"
        @action="clearAllFilters"
      />

      <!-- Pagination -->
      <div v-if="filteredRoles.length > 0 && totalPages > 1" class="pagination-wrapper">
        <div class="pagination-info">
          <span>
            {{ isRTL ? `عرض ${paginationInfo.start} إلى ${paginationInfo.end} من ${paginationInfo.total} دور` : `Showing ${paginationInfo.start} to ${paginationInfo.end} of ${paginationInfo.total} roles` }}
          </span>
        </div>

        <div class="pagination-controls">
          <button
            class="page-btn nav-btn"
            :disabled="currentPage === 1"
            @click="goToPage(currentPage - 1)"
          >
            <i :class="isRTL ? 'pi pi-angle-right' : 'pi pi-angle-left'"></i>
          </button>

          <template v-for="page in visiblePages" :key="page">
            <span v-if="page === '...'" class="page-ellipsis">...</span>
            <button
              v-else
              class="page-btn"
              :class="{ active: currentPage === page }"
              @click="goToPage(page as number)"
            >
              {{ page }}
            </button>
          </template>

          <button
            class="page-btn nav-btn"
            :disabled="currentPage === totalPages"
            @click="goToPage(currentPage + 1)"
          >
            <i :class="isRTL ? 'pi pi-angle-left' : 'pi pi-angle-right'"></i>
          </button>
        </div>
      </div>
    </div>

    <!-- Role Dialog -->
    <Dialog
      v-model:visible="showRoleDialog"
      :header="currentRole ? (isRTL ? 'تعديل الدور' : 'Edit Role') : (isRTL ? 'إضافة دور' : 'Add Role')"
      :style="{ width: '500px' }"
      :modal="true"
      :dismissableMask="true"
      class="role-dialog"
    >
      <div class="dialog-content">
        <p class="placeholder-text">{{ isRTL ? 'نموذج الدور سيظهر هنا' : 'Role form will appear here' }}</p>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showRoleDialog = false" />
        <Button :label="isRTL ? 'حفظ' : 'Save'" @click="showRoleDialog = false" />
      </template>
    </Dialog>

    <!-- View Role Dialog -->
    <Dialog
      v-model:visible="showViewDialog"
      :header="isRTL ? 'تفاصيل الدور' : 'Role Details'"
      :style="{ width: '500px' }"
      :modal="true"
      :dismissableMask="true"
      class="view-dialog"
    >
      <div class="dialog-content" v-if="currentRole">
        <div class="role-icon-large" :style="{ backgroundColor: currentRole.color + '20', color: currentRole.color }">
          <i :class="['pi', currentRole.icon]"></i>
        </div>
        <h3>{{ getName(currentRole) }}</h3>
        <Tag :value="currentRole.type === 'system' ? (isRTL ? 'نظام' : 'System') : (isRTL ? 'مخصص' : 'Custom')" :severity="currentRole.type === 'system' ? 'info' : 'success'" />
        <p class="description">{{ getDescription(currentRole) }}</p>
        <div class="permissions-section">
          <h4>{{ isRTL ? 'الصلاحيات' : 'Permissions' }}</h4>
          <div class="permissions-list">
            <Tag v-for="perm in currentRole.permissions" :key="perm" :value="perm" severity="secondary" />
          </div>
        </div>
        <div class="users-section">
          <span><i class="pi pi-users"></i> {{ currentRole.usersCount }} {{ isRTL ? 'مستخدم' : 'users' }}</span>
        </div>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إغلاق' : 'Close'" @click="showViewDialog = false" />
      </template>
    </Dialog>

    <!-- Delete Confirmation Dialog -->
    <Dialog
      v-model:visible="showDeleteDialog"
      :header="isRTL ? 'تأكيد الحذف' : 'Confirm Delete'"
      :style="{ width: '400px' }"
      :modal="true"
      :dismissableMask="true"
      class="confirm-dialog"
    >
      <div class="dialog-content">
        <i class="pi pi-exclamation-triangle" style="font-size: 2rem; color: var(--red-500);"></i>
        <p>{{ isRTL ? 'هل أنت متأكد من حذف هذا الدور؟' : 'Are you sure you want to delete this role?' }}</p>
        <p class="role-name" v-if="currentRole">{{ getName(currentRole) }}</p>
        <p class="warning-text" v-if="currentRole && currentRole.usersCount > 0">
          {{ isRTL ? `تحذير: هذا الدور مُسند إلى ${currentRole.usersCount} مستخدم.` : `Warning: This role is assigned to ${currentRole.usersCount} users.` }}
        </p>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showDeleteDialog = false" />
        <Button :label="isRTL ? 'حذف' : 'Delete'" severity="danger" @click="confirmDeleteRole" />
      </template>
    </Dialog>
    </template>
  </div>
</template>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

// ============================================
// PAGE CONTAINER
// ============================================

.roles-view {
  @include page-view;

  &.rtl {
    direction: rtl;

    .breadcrumb-separator {
      transform: rotate(180deg);
    }
  }
}

// ============================================
// MAIN CONTENT
// ============================================

.main-content {
  @include page-content;
}

.loading-state {
  .roles-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
    gap: $spacing-4;
  }
}

// ============================================
// TOOLBAR - Using global mixins
// ============================================

.toolbar {
  @include toolbar-container;
}

.toolbar-row {
  @include toolbar-row;
}

.toolbar-left {
  @include toolbar-section-left;
}

.toolbar-right {
  @include toolbar-section-right;
}

.search-box {
  @include toolbar-search(280px);
}

.search-input {
  @include toolbar-search-input;
}

.type-toggle {
  @include type-toggle;
}

.per-page-control {
  @include per-page-control;
}

.per-page-label {
  @include per-page-label;
}

.per-page-select {
  @include per-page-select;
}

.active-filters {
  @include active-filters-row;
}

.active-filters-label {
  @include active-filters-label;
}

.filter-tag {
  @include filter-tag;
}

.clear-all-link {
  @include clear-all-link;
}

.bulk-actions {
  @include bulk-actions;
}

.selection-count {
  @include selection-count;
}

.view-toggle {
  @include premium-view-toggle;
}

// ============================================
// ROLES TABLE
// ============================================

.roles-table {
  background: #fff;
  border-radius: $radius-xl;
  box-shadow: $shadow-card;
  overflow: hidden;
}

.table-header {
  display: flex;
  align-items: center;
  padding: $spacing-4;
  background: $slate-50;
  border-bottom: 1px solid $slate-100;
  font-size: $font-size-sm;
  font-weight: $font-weight-semibold;
  color: $slate-600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.table-row {
  display: flex;
  align-items: center;
  padding: $spacing-4;
  border-bottom: 1px solid $slate-100;
  transition: all $transition-fast;
  cursor: pointer;

  &:hover {
    background: $intalio-teal-50;
  }

  &.selected {
    background: rgba($intalio-teal-500, 0.08);
  }

  &:last-child {
    border-bottom: none;
  }
}

.table-cell {
  &.checkbox-cell {
    width: 48px;
    flex-shrink: 0;
  }

  &.role-cell {
    flex: 2;
    min-width: 280px;
  }

  &.type-cell {
    width: 100px;
    flex-shrink: 0;
  }

  &.users-cell {
    width: 100px;
    flex-shrink: 0;
    text-align: center;
  }

  &.permissions-cell {
    flex: 1;
    min-width: 200px;
  }

  &.actions-cell {
    width: 60px;
    flex-shrink: 0;
    text-align: center;
  }
}

.role-info {
  display: flex;
  align-items: center;
  gap: $spacing-3;
}

.role-icon {
  width: 40px;
  height: 40px;
  border-radius: $radius-lg;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;

  i {
    font-size: 1.1rem;
  }

  &.large {
    width: 56px;
    height: 56px;

    i {
      font-size: 1.5rem;
    }
  }
}

.role-details {
  display: flex;
  flex-direction: column;
  gap: $spacing-1;
  min-width: 0;
}

.role-name {
  font-weight: $font-weight-semibold;
  color: $slate-900;
}

.role-description {
  font-size: $font-size-sm;
  color: $slate-500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.users-count {
  font-weight: $font-weight-semibold;
  color: $slate-700;
}

.permissions-tags {
  display: flex;
  flex-wrap: wrap;
  gap: $spacing-1;
  align-items: center;
}

.perm-tag {
  font-size: $font-size-xs !important;
}

.more-perms {
  font-size: $font-size-xs;
  color: $slate-500;
  font-weight: $font-weight-medium;
}

.action-menu-btn {
  @include action-menu-btn;
}

.action-menu {
  @include action-menu-popup;
}

// ============================================
// ROLES GRID
// ============================================

.roles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: $spacing-4;
}

.role-card {
  position: relative;
  background: #fff;
  border-radius: $radius-xl;
  padding: $spacing-5;
  box-shadow: $shadow-card;
  transition: all $transition-base;
  border: 2px solid transparent;
  display: flex;
  flex-direction: column;

  &:hover {
    box-shadow: $shadow-card-hover;
    transform: translateY(-2px);
  }

  &.selected {
    border-color: $intalio-teal-400;
    background: $intalio-teal-50;
  }

  .card-select {
    position: absolute;
    top: $spacing-3;
    inset-inline-end: $spacing-3;
  }

  .card-header {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    margin-bottom: $spacing-4;

    .type-tag {
      font-size: $font-size-xs;
    }
  }

  .card-info {
    margin-bottom: $spacing-4;

    .role-name {
      font-size: $font-size-lg;
      margin-bottom: $spacing-2;
    }

    .role-description {
      white-space: normal;
      line-height: 1.5;
    }
  }

  .card-permissions {
    flex: 1;
    margin-bottom: $spacing-4;

    .permissions-label {
      font-size: $font-size-xs;
      font-weight: $font-weight-semibold;
      color: $slate-500;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      display: block;
      margin-bottom: $spacing-2;
    }

    .permissions-list {
      display: flex;
      flex-wrap: wrap;
      gap: $spacing-2;
      align-items: center;
    }
  }

  .card-footer {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding-top: $spacing-4;
    border-top: 1px solid $slate-100;

    .users-info {
      display: flex;
      align-items: center;
      gap: $spacing-2;
      font-size: $font-size-sm;
      color: $slate-500;

      i {
        font-size: 0.875rem;
      }
    }

    .card-actions {
      display: flex;
      gap: $spacing-1;
    }
  }
}

// ============================================
// PAGINATION - Using global mixins
// ============================================

.pagination-wrapper {
  @include pagination-wrapper;
}

.pagination-info {
  @include pagination-info;
}

.pagination-controls {
  @include pagination-controls;
}

.page-btn {
  @include pagination-btn;
}

.page-ellipsis {
  @include pagination-ellipsis;
}

// ============================================
// ROLE DIALOG
// ============================================

.role-dialog {
  .dialog-content {
    padding: $spacing-6;

    .placeholder-text {
      text-align: center;
      color: $slate-500;
    }
  }
}

// ============================================
// RESPONSIVE
// ============================================

@media (max-width: $breakpoint-lg) {
  .toolbar-row {
    flex-wrap: wrap;
  }

  .toolbar-left {
    order: 1;
    flex-basis: 100%;
  }

  .toolbar-right {
    order: 2;
    width: 100%;
    justify-content: space-between;
    margin-top: $spacing-2;
  }

  .table-cell {
    &.permissions-cell {
      display: none;
    }
  }
}

@include mobile {
  .toolbar-row {
    flex-direction: column;
    align-items: stretch;
    gap: $spacing-3;
  }

  .toolbar-left {
    flex-direction: column;
    gap: $spacing-3;
  }

  .toolbar-right {
    flex-direction: row;
    justify-content: space-between;
  }

  .search-box {
    width: 100%;
  }

  .type-toggle {
    width: 100%;
    justify-content: center;

    button {
      flex: 1;
      justify-content: center;
    }
  }

  .roles-grid {
    grid-template-columns: 1fr;
  }

  .table-cell {
    &.type-cell,
    &.users-cell {
      display: none;
    }

    &.role-cell {
      flex: 1;
    }
  }

  .pagination-wrapper {
    flex-direction: column;
    gap: $spacing-3;
    padding: $spacing-4;
  }

  .pagination-info {
    order: 2;
    text-align: center;
  }

  .pagination-controls {
    order: 1;
    width: 100%;
    justify-content: center;
  }
}

// ============================================
// ANIMATIONS
// ============================================

.role-card {
  @include card-item-animation;
}

@include staggered-animation-delays('.role-card', 12, 0.05s);

// ============================================
// ANIMATIONS
// ============================================

@keyframes tableRowFadeIn {
  from {
    opacity: 0;
    transform: translateX(-16px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

@keyframes cardFadeIn {
  from {
    opacity: 0;
    transform: translateY(20px) scale(0.96);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

// Table animation
.roles-table {
  &--animated {
    .table-row {
      opacity: 0;
      transform: translateX(-16px);
    }

    &.roles-table--visible {
      .table-row {
        animation: tableRowFadeIn 0.35s ease-out forwards;
        animation-delay: calc(var(--row-index, 0) * 50ms);
      }
    }
  }

  &:not(.roles-table--animated) {
    .table-row {
      opacity: 1;
      transform: none;
      animation: none;
    }
  }
}

// Grid animation
.roles-grid {
  &--animated {
    .role-card {
      opacity: 0;
      transform: translateY(20px) scale(0.96);
    }

    &.roles-grid--visible {
      .role-card {
        animation: cardFadeIn 0.4s ease-out forwards;
        animation-delay: calc(var(--card-index, 0) * 70ms);
      }
    }
  }

  &:not(.roles-grid--animated) {
    .role-card {
      opacity: 1;
      transform: none;
      animation: none;
    }
  }
}

// Enhanced table row hover
.table-row {
  &:hover {
    background: rgba($intalio-teal-500, 0.04);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    transform: translateX(4px);
  }
}

// Enhanced card hover
.role-card {
  &:hover {
    box-shadow:
      0 12px 28px rgba(0, 0, 0, 0.12),
      0 6px 12px rgba(0, 0, 0, 0.08);
    transform: translateY(-4px) scale(1.01);
  }
}

// RTL adjustments
.rtl {
  .roles-table--animated .table-row {
    transform: translateX(16px);
  }

  .table-row:hover {
    transform: translateX(-4px);
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================
@media (prefers-reduced-motion: reduce) {
  .roles-table {
    &--animated .table-row {
      opacity: 1;
      transform: none;
      animation: none !important;
    }
  }

  .roles-grid {
    &--animated .role-card {
      opacity: 1;
      transform: none;
      animation: none !important;
    }
  }

  .table-row {
    transition: background-color 0.15s;

    &:hover {
      transform: none;
    }
  }

  .role-card {
    transition: background-color 0.15s, box-shadow 0.15s;

    &:hover {
      transform: none;
    }
  }
}
</style>
