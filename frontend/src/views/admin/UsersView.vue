<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
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
const router = useRouter()
const prefersReducedMotion = useReducedMotion()

// RTL support
const isRTL = computed(() => locale.value === 'ar')

// Animation control
const isContentVisible = ref(false)
const shouldAnimate = computed(() => !prefersReducedMotion.value)

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: isRTL.value ? 'إدارة المستخدمين' : 'User Management' }
])

// State
const isLoading = ref(true)
const error = ref<Error | null>(null)
const searchQuery = ref('')
const showUserDialog = ref(false)
const showDeleteDialog = ref(false)
const showResetPasswordDialog = ref(false)
const showDisableDialog = ref(false)
const showFilters = ref(false)
const viewMode = ref<'grid' | 'list'>('list')
const selectedUsers = ref<string[]>([])

// Pagination
const currentPage = ref(1)
const itemsPerPage = ref(10)

// Filter type for role toggle
const filterType = ref<'all' | 'admin' | 'editor' | 'viewer'>('all')

// Type toggle options
const typeOptions = computed(() => [
  { value: 'all', label: isRTL.value ? 'الكل' : 'All', icon: 'pi-users' },
  { value: 'admin', label: isRTL.value ? 'مدراء' : 'Admins', icon: 'pi-shield' },
  { value: 'editor', label: isRTL.value ? 'محررين' : 'Editors', icon: 'pi-pencil' },
  { value: 'viewer', label: isRTL.value ? 'مشاهدين' : 'Viewers', icon: 'pi-eye' }
])

// Status filter options
const statusOptions = computed(() => [
  { value: 'all', label: isRTL.value ? 'جميع الحالات' : 'All Status' },
  { value: 'active', label: isRTL.value ? 'نشط' : 'Active' },
  { value: 'inactive', label: isRTL.value ? 'غير نشط' : 'Inactive' },
  { value: 'pending', label: isRTL.value ? 'معلق' : 'Pending' }
])

// Department filter options
const departmentOptions = computed(() => [
  { value: 'all', label: isRTL.value ? 'جميع الأقسام' : 'All Departments' },
  { value: 'operations', label: isRTL.value ? 'العمليات' : 'Operations' },
  { value: 'content', label: isRTL.value ? 'المحتوى' : 'Content' },
  { value: 'media', label: isRTL.value ? 'الإعلام' : 'Media' },
  { value: 'technology', label: isRTL.value ? 'التكنولوجيا' : 'Technology' },
  { value: 'security', label: isRTL.value ? 'الأمن' : 'Security' }
])

const filterStatus = ref<string>('all')
const filterDepartment = ref<string>('all')

// Per page options
const perPageOptions = [
  { value: 5, label: '5' },
  { value: 10, label: '10' },
  { value: 20, label: '20' },
  { value: 50, label: '50' }
]

// Types
interface User {
  id: string
  name: string
  nameArabic: string
  email: string
  phone?: string
  avatarUrl?: string
  role: 'admin' | 'editor' | 'viewer'
  department: string
  departmentArabic: string
  status: 'active' | 'inactive' | 'pending'
  lastLogin: Date
  createdAt: Date
}

// Users data
const users = ref<User[]>([
  {
    id: '1',
    name: 'Ahmed Hassan',
    nameArabic: 'أحمد حسن',
    email: 'ahmed.hassan@afc2027.sa',
    phone: '+966 50 123 4567',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Ahmed',
    role: 'admin',
    department: 'operations',
    departmentArabic: 'العمليات',
    status: 'active',
    lastLogin: new Date(),
    createdAt: new Date(Date.now() - 365 * 24 * 60 * 60 * 1000)
  },
  {
    id: '2',
    name: 'Sara Ali',
    nameArabic: 'سارة علي',
    email: 'sara.ali@afc2027.sa',
    phone: '+966 50 234 5678',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sara',
    role: 'editor',
    department: 'content',
    departmentArabic: 'المحتوى',
    status: 'active',
    lastLogin: new Date(Date.now() - 3600000),
    createdAt: new Date(Date.now() - 200 * 24 * 60 * 60 * 1000)
  },
  {
    id: '3',
    name: 'Mohammed Khan',
    nameArabic: 'محمد خان',
    email: 'mohammed.khan@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Mohammed',
    role: 'viewer',
    department: 'media',
    departmentArabic: 'الإعلام',
    status: 'inactive',
    lastLogin: new Date(Date.now() - 86400000 * 7),
    createdAt: new Date(Date.now() - 150 * 24 * 60 * 60 * 1000)
  },
  {
    id: '4',
    name: 'Fatima Al-Rashid',
    nameArabic: 'فاطمة الراشد',
    email: 'fatima.rashid@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Fatima',
    role: 'admin',
    department: 'technology',
    departmentArabic: 'التكنولوجيا',
    status: 'active',
    lastLogin: new Date(Date.now() - 7200000),
    createdAt: new Date(Date.now() - 300 * 24 * 60 * 60 * 1000)
  },
  {
    id: '5',
    name: 'Omar Khalid',
    nameArabic: 'عمر خالد',
    email: 'omar.khalid@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Omar',
    role: 'editor',
    department: 'media',
    departmentArabic: 'الإعلام',
    status: 'pending',
    lastLogin: new Date(Date.now() - 86400000 * 2),
    createdAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000)
  },
  {
    id: '6',
    name: 'Layla Ahmad',
    nameArabic: 'ليلى أحمد',
    email: 'layla.ahmad@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Layla',
    role: 'viewer',
    department: 'operations',
    departmentArabic: 'العمليات',
    status: 'active',
    lastLogin: new Date(Date.now() - 86400000),
    createdAt: new Date(Date.now() - 90 * 24 * 60 * 60 * 1000)
  },
  {
    id: '7',
    name: 'Yusuf Al-Farsi',
    nameArabic: 'يوسف الفارسي',
    email: 'yusuf.farsi@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Yusuf',
    role: 'editor',
    department: 'content',
    departmentArabic: 'المحتوى',
    status: 'active',
    lastLogin: new Date(Date.now() - 3600000 * 5),
    createdAt: new Date(Date.now() - 120 * 24 * 60 * 60 * 1000)
  },
  {
    id: '8',
    name: 'Nora Al-Qahtani',
    nameArabic: 'نورة القحطاني',
    email: 'nora.qahtani@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Nora',
    role: 'viewer',
    department: 'security',
    departmentArabic: 'الأمن',
    status: 'inactive',
    lastLogin: new Date(Date.now() - 86400000 * 14),
    createdAt: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000)
  },
  {
    id: '9',
    name: 'Khalid Al-Mutairi',
    nameArabic: 'خالد المطيري',
    email: 'khalid.mutairi@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Khalid',
    role: 'admin',
    department: 'security',
    departmentArabic: 'الأمن',
    status: 'active',
    lastLogin: new Date(Date.now() - 1800000),
    createdAt: new Date(Date.now() - 250 * 24 * 60 * 60 * 1000)
  },
  {
    id: '10',
    name: 'Aisha Binti Omar',
    nameArabic: 'عائشة بنت عمر',
    email: 'aisha.omar@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Aisha',
    role: 'editor',
    department: 'operations',
    departmentArabic: 'العمليات',
    status: 'active',
    lastLogin: new Date(Date.now() - 7200000 * 3),
    createdAt: new Date(Date.now() - 180 * 24 * 60 * 60 * 1000)
  },
  {
    id: '11',
    name: 'Hassan Al-Dosari',
    nameArabic: 'حسن الدوسري',
    email: 'hassan.dosari@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Hassan',
    role: 'viewer',
    department: 'technology',
    departmentArabic: 'التكنولوجيا',
    status: 'pending',
    lastLogin: new Date(Date.now() - 86400000 * 3),
    createdAt: new Date(Date.now() - 15 * 24 * 60 * 60 * 1000)
  },
  {
    id: '12',
    name: 'Maryam Al-Sulaiman',
    nameArabic: 'مريم السليمان',
    email: 'maryam.sulaiman@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Maryam',
    role: 'editor',
    department: 'content',
    departmentArabic: 'المحتوى',
    status: 'active',
    lastLogin: new Date(Date.now() - 3600000 * 2),
    createdAt: new Date(Date.now() - 75 * 24 * 60 * 60 * 1000)
  }
])

// Computed stats for StatsBar
const stats = computed<StatItem[]>(() => {
  const total = users.value.length
  const active = users.value.filter(u => u.status === 'active').length
  const admins = users.value.filter(u => u.role === 'admin').length
  const pending = users.value.filter(u => u.status === 'pending').length

  return [
    {
      icon: 'pi pi-users',
      value: total,
      label: 'Total Users',
      labelArabic: 'إجمالي المستخدمين',
      colorClass: 'primary'
    },
    {
      icon: 'pi pi-circle-fill',
      value: active,
      label: 'Active',
      labelArabic: 'نشط',
      colorClass: 'success'
    },
    {
      icon: 'pi pi-shield',
      value: admins,
      label: 'Admins',
      labelArabic: 'مدراء',
      colorClass: 'info'
    },
    {
      icon: 'pi pi-clock',
      value: pending,
      label: 'Pending',
      labelArabic: 'معلق',
      colorClass: 'warning'
    }
  ]
})

// Active filters count
const activeFiltersCount = computed(() => {
  let count = 0
  if (filterType.value !== 'all') count++
  if (filterStatus.value !== 'all') count++
  if (filterDepartment.value !== 'all') count++
  return count
})

// Clear all filters
const clearAllFilters = () => {
  filterType.value = 'all'
  filterStatus.value = 'all'
  filterDepartment.value = 'all'
  searchQuery.value = ''
  currentPage.value = 1
}

// Reset pagination when filters change
const onFilterChange = () => {
  currentPage.value = 1
}

// Filtered users
const filteredUsers = computed(() => {
  let result = [...users.value]

  // Type toggle filter (role-based)
  if (filterType.value !== 'all') {
    result = result.filter(u => u.role === filterType.value)
  }

  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(u =>
      u.name.toLowerCase().includes(query) ||
      u.nameArabic.includes(query) ||
      u.email.toLowerCase().includes(query)
    )
  }

  // Status filter
  if (filterStatus.value !== 'all') {
    result = result.filter(u => u.status === filterStatus.value)
  }

  // Department filter
  if (filterDepartment.value !== 'all') {
    result = result.filter(u => u.department === filterDepartment.value)
  }

  return result
})

// Pagination computed properties
const totalPages = computed(() => Math.ceil(filteredUsers.value.length / itemsPerPage.value))

const paginatedUsers = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredUsers.value.slice(start, end)
})

const paginationInfo = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value + 1
  const end = Math.min(currentPage.value * itemsPerPage.value, filteredUsers.value.length)
  const total = filteredUsers.value.length
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
function getName(user: User) {
  return isRTL.value ? user.nameArabic : user.name
}

function getDepartmentName(user: User) {
  return isRTL.value ? user.departmentArabic : user.department
}

function getInitials(name: string) {
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
}

function getRoleBadgeClass(role: string) {
  const classes: Record<string, string> = {
    admin: 'role-admin',
    editor: 'role-editor',
    viewer: 'role-viewer'
  }
  return classes[role] || ''
}

function getRoleLabel(role: string) {
  const labels: Record<string, { en: string; ar: string }> = {
    admin: { en: 'Admin', ar: 'مدير' },
    editor: { en: 'Editor', ar: 'محرر' },
    viewer: { en: 'Viewer', ar: 'مشاهد' }
  }
  return isRTL.value ? labels[role]?.ar : labels[role]?.en
}

function getStatusSeverity(status: string): "success" | "danger" | "warn" | "info" | "secondary" {
  const severities: Record<string, "success" | "danger" | "warn" | "info" | "secondary"> = {
    active: 'success',
    inactive: 'secondary',
    pending: 'warn'
  }
  return severities[status] || 'secondary'
}

function getStatusLabel(status: string) {
  const labels: Record<string, { en: string; ar: string }> = {
    active: { en: 'Active', ar: 'نشط' },
    inactive: { en: 'Inactive', ar: 'غير نشط' },
    pending: { en: 'Pending', ar: 'معلق' }
  }
  return isRTL.value ? labels[status]?.ar : labels[status]?.en
}

function formatDate(date: Date) {
  return new Intl.DateTimeFormat(isRTL.value ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  }).format(date)
}

function getRelativeTime(date: Date) {
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / (1000 * 60))
  const diffHours = Math.floor(diffMs / (1000 * 60 * 60))
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))

  if (diffMins < 60) {
    return isRTL.value ? `منذ ${diffMins} دقيقة` : `${diffMins}m ago`
  }
  if (diffHours < 24) {
    return isRTL.value ? `منذ ${diffHours} ساعة` : `${diffHours}h ago`
  }
  if (diffDays < 7) {
    return isRTL.value ? `منذ ${diffDays} أيام` : `${diffDays}d ago`
  }
  return formatDate(date)
}

// Selection
function toggleSelect(id: string) {
  const index = selectedUsers.value.indexOf(id)
  if (index === -1) {
    selectedUsers.value.push(id)
  } else {
    selectedUsers.value.splice(index, 1)
  }
}

function isSelected(id: string) {
  return selectedUsers.value.includes(id)
}

function selectAll() {
  if (selectedUsers.value.length === filteredUsers.value.length) {
    selectedUsers.value = []
  } else {
    selectedUsers.value = filteredUsers.value.map(u => u.id)
  }
}

// Action menu
const actionMenu = ref()
const currentUser = ref<User | null>(null)

const actionMenuItems = computed(() => [
  { label: isRTL.value ? 'عرض الملف' : 'View Profile', icon: 'pi pi-user', command: () => viewUser(currentUser.value) },
  { label: isRTL.value ? 'تعديل' : 'Edit', icon: 'pi pi-pencil', command: () => editUser(currentUser.value) },
  { label: isRTL.value ? 'إعادة تعيين كلمة المرور' : 'Reset Password', icon: 'pi pi-key', command: () => resetPassword(currentUser.value) },
  { separator: true },
  { label: isRTL.value ? 'تعطيل' : 'Disable', icon: 'pi pi-ban', command: () => disableUser(currentUser.value) },
  { label: isRTL.value ? 'حذف' : 'Delete', icon: 'pi pi-trash', class: 'text-red-500', command: () => deleteUser(currentUser.value) }
])

function toggleActionMenu(event: Event, user: User) {
  currentUser.value = user
  actionMenu.value.toggle(event)
}

function viewUser(user: User | null) {
  if (user) {
    currentUser.value = user
    router.push({ name: 'profile', params: { id: user.id } })
  }
}

function editUser(user: User | null) {
  if (user) {
    currentUser.value = user
  }
  showUserDialog.value = true
}

function resetPassword(user: User | null) {
  if (user) {
    currentUser.value = user
    showResetPasswordDialog.value = true
  }
}

function confirmResetPassword() {
  if (currentUser.value) {
    // In a real app, this would call an API
    console.log('Password reset for user:', currentUser.value.id)
    showResetPasswordDialog.value = false
  }
}

function disableUser(user: User | null) {
  if (user) {
    currentUser.value = user
    showDisableDialog.value = true
  }
}

function confirmDisableUser() {
  if (currentUser.value) {
    const user = users.value.find(u => u.id === currentUser.value?.id)
    if (user) {
      user.status = user.status === 'active' ? 'inactive' : 'active'
    }
    showDisableDialog.value = false
  }
}

function deleteUser(user: User | null) {
  if (user) {
    currentUser.value = user
    showDeleteDialog.value = true
  }
}

function confirmDeleteUser() {
  if (currentUser.value) {
    users.value = users.value.filter(u => u.id !== currentUser.value?.id)
    showDeleteDialog.value = false
    currentUser.value = null
  }
}

function addUser() {
  currentUser.value = null
  showUserDialog.value = true
}

// Data loading with error handling
async function loadUsers() {
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
  loadUsers()
}

// Lifecycle
onMounted(() => {
  loadUsers()
})
</script>

<template>
  <div class="users-view" :class="{ 'rtl': isRTL }">
    <!-- Error State -->
    <ErrorState
      v-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل المستخدمين' : 'Failed to load users'"
      show-retry
      size="lg"
      @retry="handleRetry"
    />

    <template v-else>
    <!-- Page Header -->
    <PageHeader
      :title="isRTL ? 'إدارة المستخدمين' : 'User Management'"
      :description="isRTL ? 'إدارة المستخدمين والأدوار والأذونات' : 'Manage users, roles, and permissions'"
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
          :label="isRTL ? 'إضافة مستخدم' : 'Add User'"
          icon="pi pi-user-plus"
          @click="addUser"
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
        <!-- Toolbar Skeleton -->
        <Skeleton height="60px" class="mb-4" borderRadius="16px" />

        <!-- Cards Skeleton -->
        <div class="users-grid">
          <Skeleton v-for="i in 8" :key="i" height="200px" borderRadius="16px" />
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
                :placeholder="isRTL ? 'البحث عن مستخدمين...' : 'Search users...'"
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
                @click="filterType = option.value as 'all' | 'admin' | 'editor' | 'viewer'; onFilterChange()"
              >
                <i :class="['pi', option.icon]"></i>
                <span>{{ option.label }}</span>
              </button>
            </div>

            <!-- Filter Button -->
            <button
              class="filter-btn"
              :class="{ active: showFilters, 'has-filters': activeFiltersCount > 0 }"
              @click="showFilters = !showFilters"
            >
              <i class="pi pi-filter"></i>
              <span>{{ isRTL ? 'تصفية' : 'Filters' }}</span>
              <span v-if="activeFiltersCount > 0" class="filter-badge">{{ activeFiltersCount }}</span>
            </button>

            <!-- Bulk Actions -->
            <div class="bulk-actions" v-if="selectedUsers.length > 0">
              <span class="selection-count">
                {{ selectedUsers.length }} {{ isRTL ? 'محدد' : 'selected' }}
              </span>
              <Button icon="pi pi-trash" text size="small" severity="danger" v-tooltip.top="isRTL ? 'حذف' : 'Delete'" />
              <Button icon="pi pi-ban" text size="small" v-tooltip.top="isRTL ? 'تعطيل' : 'Disable'" />
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
              <span class="per-page-label">{{ isRTL ? 'عنصر' : 'items' }}</span>
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

        <!-- Expanded Filters Panel -->
        <div v-if="showFilters" class="filters-panel">
          <div class="filter-group">
            <label class="filter-label">{{ isRTL ? 'الحالة' : 'Status' }}</label>
            <div class="filter-options">
              <button
                v-for="option in statusOptions"
                :key="option.value"
                :class="['filter-chip', { active: filterStatus === option.value }]"
                @click="filterStatus = option.value; onFilterChange()"
              >
                {{ option.label }}
              </button>
            </div>
          </div>

          <div class="filter-group">
            <label class="filter-label">{{ isRTL ? 'القسم' : 'Department' }}</label>
            <div class="filter-options">
              <button
                v-for="option in departmentOptions"
                :key="option.value"
                :class="['filter-chip', { active: filterDepartment === option.value }]"
                @click="filterDepartment = option.value; onFilterChange()"
              >
                {{ option.label }}
              </button>
            </div>
          </div>

          <div class="filter-actions" v-if="activeFiltersCount > 0 || searchQuery">
            <button class="clear-filters-btn" @click="clearAllFilters">
              <i class="pi pi-times"></i>
              {{ isRTL ? 'مسح الكل' : 'Clear All' }}
            </button>
          </div>
        </div>

        <!-- Active Filters Tags -->
        <div v-if="(activeFiltersCount > 0 || searchQuery) && !showFilters" class="active-filters">
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

          <span v-if="filterStatus !== 'all'" class="filter-tag">
            {{ statusOptions.find(o => o.value === filterStatus)?.label }}
            <button @click="filterStatus = 'all'; onFilterChange()"><i class="pi pi-times"></i></button>
          </span>

          <span v-if="filterDepartment !== 'all'" class="filter-tag">
            {{ departmentOptions.find(o => o.value === filterDepartment)?.label }}
            <button @click="filterDepartment = 'all'; onFilterChange()"><i class="pi pi-times"></i></button>
          </span>

          <button class="clear-all-link" @click="clearAllFilters">
            {{ isRTL ? 'مسح الكل' : 'Clear all' }}
          </button>
        </div>
      </div>

      <!-- Users List View -->
      <div
        v-if="viewMode === 'list'"
        class="users-table"
        :class="{
          'users-table--animated': shouldAnimate,
          'users-table--visible': isContentVisible
        }"
      >
        <div class="table-header">
          <div class="table-cell checkbox-cell">
            <Checkbox
              :modelValue="selectedUsers.length === filteredUsers.length && filteredUsers.length > 0"
              :binary="true"
              @change="selectAll"
            />
          </div>
          <div class="table-cell user-cell">{{ isRTL ? 'المستخدم' : 'User' }}</div>
          <div class="table-cell role-cell">{{ isRTL ? 'الدور' : 'Role' }}</div>
          <div class="table-cell department-cell">{{ isRTL ? 'القسم' : 'Department' }}</div>
          <div class="table-cell status-cell">{{ isRTL ? 'الحالة' : 'Status' }}</div>
          <div class="table-cell login-cell">{{ isRTL ? 'آخر دخول' : 'Last Login' }}</div>
          <div class="table-cell actions-cell">{{ isRTL ? 'إجراءات' : 'Actions' }}</div>
        </div>

        <div
          v-for="(user, index) in paginatedUsers"
          :key="user.id"
          class="table-row"
          :class="{ selected: isSelected(user.id) }"
          :style="shouldAnimate ? { '--row-index': index } : undefined"
        >
          <div class="table-cell checkbox-cell" @click.stop>
            <Checkbox
              :modelValue="isSelected(user.id)"
              :binary="true"
              @change="toggleSelect(user.id)"
            />
          </div>
          <div class="table-cell user-cell">
            <div class="user-info">
              <div class="user-avatar" :class="{ 'avatar-fallback': !user.avatarUrl }">
                <img v-if="user.avatarUrl" :src="user.avatarUrl" :alt="getName(user)" />
                <span v-else>{{ getInitials(user.name) }}</span>
              </div>
              <div class="user-details">
                <span class="user-name">{{ getName(user) }}</span>
                <span class="user-email">{{ user.email }}</span>
              </div>
            </div>
          </div>
          <div class="table-cell role-cell">
            <span class="role-badge" :class="getRoleBadgeClass(user.role)">
              {{ getRoleLabel(user.role) }}
            </span>
          </div>
          <div class="table-cell department-cell">
            <span class="department-text">{{ getDepartmentName(user) }}</span>
          </div>
          <div class="table-cell status-cell">
            <Tag :value="getStatusLabel(user.status)" :severity="getStatusSeverity(user.status)" class="status-tag" />
          </div>
          <div class="table-cell login-cell">
            <span class="login-time" :title="formatDate(user.lastLogin)">{{ getRelativeTime(user.lastLogin) }}</span>
          </div>
          <div class="table-cell actions-cell">
            <Button
              icon="pi pi-ellipsis-v"
              text
              rounded
              size="small"
              class="action-menu-btn"
              @click.stop="(e: Event) => toggleActionMenu(e, user)"
            />
          </div>
        </div>

        <Menu ref="actionMenu" :model="actionMenuItems" popup class="action-menu" />
      </div>

      <!-- Users Grid View -->
      <div
        v-else
        class="users-grid"
        :class="{
          'users-grid--animated': shouldAnimate,
          'users-grid--visible': isContentVisible
        }"
      >
        <div
          v-for="(user, index) in paginatedUsers"
          :key="user.id"
          class="user-card"
          :class="{ selected: isSelected(user.id) }"
          :style="shouldAnimate ? { '--card-index': index } : undefined"
        >
          <div class="card-select" @click.stop>
            <Checkbox
              :modelValue="isSelected(user.id)"
              :binary="true"
              @change="toggleSelect(user.id)"
            />
          </div>

          <!-- Avatar -->
          <div class="user-avatar large" :class="{ 'avatar-fallback': !user.avatarUrl }">
            <img v-if="user.avatarUrl" :src="user.avatarUrl" :alt="getName(user)" />
            <span v-else>{{ getInitials(user.name) }}</span>
            <span class="status-dot" :class="user.status"></span>
          </div>

          <!-- Info -->
          <div class="card-info">
            <h3 class="user-name">{{ getName(user) }}</h3>
            <p class="user-email">{{ user.email }}</p>
            <span class="role-badge" :class="getRoleBadgeClass(user.role)">
              {{ getRoleLabel(user.role) }}
            </span>
          </div>

          <!-- Meta -->
          <div class="card-meta">
            <div class="meta-item">
              <i class="pi pi-building"></i>
              <span>{{ getDepartmentName(user) }}</span>
            </div>
            <div class="meta-item">
              <i class="pi pi-clock"></i>
              <span>{{ getRelativeTime(user.lastLogin) }}</span>
            </div>
          </div>

          <!-- Actions -->
          <div class="card-actions">
            <Button
              icon="pi pi-pencil"
              text
              rounded
              size="small"
              @click.stop="editUser(user)"
              :title="isRTL ? 'تعديل' : 'Edit'"
            />
            <Button
              icon="pi pi-ellipsis-v"
              text
              rounded
              size="small"
              @click.stop="(e: Event) => toggleActionMenu(e, user)"
              :title="isRTL ? 'المزيد' : 'More'"
            />
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <EmptyState
        v-if="filteredUsers.length === 0"
        icon="pi-users"
        title="No Users Found"
        title-arabic="لم يتم العثور على مستخدمين"
        description="Try adjusting your search criteria"
        description-arabic="حاول تعديل معايير البحث"
        action-label="Clear Filters"
        action-label-arabic="مسح الفلاتر"
        action-icon="pi-times"
        variant="search"
        @action="clearAllFilters"
      />

      <!-- Pagination -->
      <div v-if="filteredUsers.length > 0 && totalPages > 1" class="pagination-wrapper">
        <div class="pagination-info">
          <span>
            {{ isRTL ? `عرض ${paginationInfo.start} إلى ${paginationInfo.end} من ${paginationInfo.total} مستخدم` : `Showing ${paginationInfo.start} to ${paginationInfo.end} of ${paginationInfo.total} users` }}
          </span>
        </div>

        <div class="pagination-controls">
          <button
            class="page-btn nav-btn"
            :disabled="currentPage === 1"
            @click="goToPage(currentPage - 1)"
            :aria-label="isRTL ? 'الصفحة السابقة' : 'Previous page'"
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
            :aria-label="isRTL ? 'الصفحة التالية' : 'Next page'"
          >
            <i :class="isRTL ? 'pi pi-angle-left' : 'pi pi-angle-right'"></i>
          </button>
        </div>
      </div>
    </div>

    <!-- User Dialog -->
    <Dialog
      v-model:visible="showUserDialog"
      :header="currentUser ? (isRTL ? 'تعديل المستخدم' : 'Edit User') : (isRTL ? 'إضافة مستخدم' : 'Add User')"
      :style="{ width: '500px' }"
      :modal="true"
      :dismissableMask="true"
      class="user-dialog"
    >
      <div class="dialog-content">
        <p class="placeholder-text">{{ isRTL ? 'نموذج المستخدم سيظهر هنا' : 'User form will appear here' }}</p>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showUserDialog = false" />
        <Button :label="isRTL ? 'حفظ' : 'Save'" @click="showUserDialog = false" />
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
        <p>{{ isRTL ? 'هل أنت متأكد من حذف هذا المستخدم؟' : 'Are you sure you want to delete this user?' }}</p>
        <p class="user-name" v-if="currentUser">{{ currentUser.name }}</p>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showDeleteDialog = false" />
        <Button :label="isRTL ? 'حذف' : 'Delete'" severity="danger" @click="confirmDeleteUser" />
      </template>
    </Dialog>

    <!-- Reset Password Dialog -->
    <Dialog
      v-model:visible="showResetPasswordDialog"
      :header="isRTL ? 'إعادة تعيين كلمة المرور' : 'Reset Password'"
      :style="{ width: '400px' }"
      :modal="true"
      :dismissableMask="true"
      class="confirm-dialog"
    >
      <div class="dialog-content">
        <i class="pi pi-key" style="font-size: 2rem; color: var(--primary-500);"></i>
        <p>{{ isRTL ? 'سيتم إرسال رابط إعادة تعيين كلمة المرور إلى:' : 'A password reset link will be sent to:' }}</p>
        <p class="user-email" v-if="currentUser">{{ currentUser.email }}</p>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showResetPasswordDialog = false" />
        <Button :label="isRTL ? 'إرسال' : 'Send'" @click="confirmResetPassword" />
      </template>
    </Dialog>

    <!-- Disable/Enable User Dialog -->
    <Dialog
      v-model:visible="showDisableDialog"
      :header="currentUser?.status === 'active' ? (isRTL ? 'تعطيل المستخدم' : 'Disable User') : (isRTL ? 'تفعيل المستخدم' : 'Enable User')"
      :style="{ width: '400px' }"
      :modal="true"
      :dismissableMask="true"
      class="confirm-dialog"
    >
      <div class="dialog-content">
        <i class="pi pi-ban" style="font-size: 2rem; color: var(--orange-500);"></i>
        <p v-if="currentUser?.status === 'active'">
          {{ isRTL ? 'هل أنت متأكد من تعطيل هذا المستخدم؟ لن يتمكن من تسجيل الدخول.' : 'Are you sure you want to disable this user? They will not be able to log in.' }}
        </p>
        <p v-else>
          {{ isRTL ? 'هل أنت متأكد من تفعيل هذا المستخدم؟' : 'Are you sure you want to enable this user?' }}
        </p>
        <p class="user-name" v-if="currentUser">{{ currentUser.name }}</p>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showDisableDialog = false" />
        <Button
          :label="currentUser?.status === 'active' ? (isRTL ? 'تعطيل' : 'Disable') : (isRTL ? 'تفعيل' : 'Enable')"
          :severity="currentUser?.status === 'active' ? 'warning' : 'success'"
          @click="confirmDisableUser"
        />
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

.users-view {
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
  .users-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
    gap: $spacing-4;
  }
}

// ============================================
// PREMIUM TOOLBAR - Using global mixins
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

// Type Toggle
.type-toggle {
  @include type-toggle;
}

// Filter Button
.filter-btn {
  @include filter-btn;

  .filter-badge {
    @include filter-badge;
  }
}

// Per Page Control
.per-page-control {
  @include per-page-control;
}

.per-page-label {
  @include per-page-label;
}

.per-page-select {
  @include per-page-select;
}

// Filters Panel
.filters-panel {
  @include filters-panel;
}

.filter-group {
  @include filters-panel-group;
}

.filter-label {
  @include filters-panel-label;
}

.filter-options {
  @include filters-panel-options;
}

.filter-chip {
  @include filter-chip;
}

.filter-actions {
  display: flex;
  align-items: flex-end;
  margin-inline-start: auto;
}

.clear-filters-btn {
  @include clear-filters-btn;
}

// Active Filters Tags
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

// Bulk Actions
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
// USERS TABLE
// ============================================

.users-table {
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

  &.user-cell {
    flex: 2;
    min-width: 250px;
  }

  &.role-cell {
    width: 100px;
    flex-shrink: 0;
  }

  &.department-cell {
    width: 140px;
    flex-shrink: 0;
  }

  &.status-cell {
    width: 100px;
    flex-shrink: 0;
  }

  &.login-cell {
    width: 120px;
    flex-shrink: 0;
  }

  &.actions-cell {
    width: 60px;
    flex-shrink: 0;
    text-align: center;
  }
}

.user-info {
  display: flex;
  align-items: center;
  gap: $spacing-3;
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: $radius-full;
  overflow: hidden;
  flex-shrink: 0;
  background: $intalio-teal-100;
  display: flex;
  align-items: center;
  justify-content: center;

  img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  &.avatar-fallback {
    color: $intalio-teal-600;
    font-weight: $font-weight-semibold;
    font-size: $font-size-sm;
  }

  &.large {
    width: 80px;
    height: 80px;
    font-size: $font-size-xl;
    position: relative;

    .status-dot {
      position: absolute;
      bottom: 4px;
      right: 4px;
      width: 16px;
      height: 16px;
      border-radius: $radius-full;
      border: 3px solid #fff;

      &.active {
        background: $success-500;
      }

      &.inactive {
        background: $slate-400;
      }

      &.pending {
        background: $warning-500;
      }
    }
  }
}

.user-details {
  display: flex;
  flex-direction: column;
  gap: $spacing-1;
  min-width: 0;
}

.user-name {
  font-weight: $font-weight-semibold;
  color: $slate-900;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.user-email {
  font-size: $font-size-sm;
  color: $slate-500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.role-badge {
  display: inline-flex;
  align-items: center;
  padding: $spacing-1 $spacing-3;
  border-radius: $radius-full;
  font-size: $font-size-xs;
  font-weight: $font-weight-semibold;
  text-transform: uppercase;
  letter-spacing: 0.5px;

  &.role-admin {
    background: rgba($error-500, 0.1);
    color: $error-600;
  }

  &.role-editor {
    background: rgba($info-500, 0.1);
    color: $info-600;
  }

  &.role-viewer {
    background: rgba($slate-500, 0.1);
    color: $slate-600;
  }
}

.department-text {
  font-size: $font-size-sm;
  color: $slate-600;
}

.status-tag {
  font-size: $font-size-xs;
}

.login-time {
  font-size: $font-size-sm;
  color: $slate-500;
}

.action-menu-btn {
  @include action-menu-btn;
}

.action-menu {
  @include action-menu-popup;
}

// ============================================
// USERS GRID
// ============================================

.users-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: $spacing-4;
}

.user-card {
  position: relative;
  background: #fff;
  border-radius: $radius-xl;
  padding: $spacing-6;
  box-shadow: $shadow-card;
  transition: all $transition-base;
  border: 2px solid transparent;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;

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
    inset-inline-start: $spacing-3;
  }

  .card-info {
    margin-top: $spacing-4;
    margin-bottom: $spacing-4;

    .user-name {
      font-size: $font-size-lg;
      margin-bottom: $spacing-1;
    }

    .user-email {
      margin-bottom: $spacing-3;
    }
  }

  .card-meta {
    width: 100%;
    display: flex;
    justify-content: center;
    gap: $spacing-6;
    padding-top: $spacing-4;
    border-top: 1px solid $slate-100;

    .meta-item {
      display: flex;
      align-items: center;
      gap: $spacing-2;
      font-size: $font-size-sm;
      color: $slate-500;

      i {
        font-size: 0.875rem;
      }
    }
  }

  .card-actions {
    @include card-action-btn;
    position: absolute;
    top: $spacing-3;
    inset-inline-end: $spacing-3;
  }

  &:hover .card-actions {
    opacity: 1;
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
// USER DIALOG
// ============================================

.user-dialog {
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
    &.department-cell,
    &.login-cell {
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
    overflow-x: auto;

    button {
      flex: 1;
      justify-content: center;
    }
  }

  .filter-btn {
    width: 100%;
    justify-content: center;
  }

  .per-page-control {
    flex: 1;
  }

  .filters-panel {
    flex-direction: column;
  }

  .filter-group {
    width: 100%;
  }

  .users-grid {
    grid-template-columns: 1fr;
  }

  .table-cell {
    &.role-cell,
    &.status-cell {
      display: none;
    }

    &.user-cell {
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

  .page-btn {
    min-width: 36px;
    height: 36px;
  }
}

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
.users-table {
  &--animated {
    .table-row {
      opacity: 0;
      transform: translateX(-16px);
    }

    &.users-table--visible {
      .table-row {
        animation: tableRowFadeIn 0.35s ease-out forwards;
        animation-delay: calc(var(--row-index, 0) * 50ms);
      }
    }
  }

  &:not(.users-table--animated) {
    .table-row {
      opacity: 1;
      transform: none;
      animation: none;
    }
  }
}

// Grid animation
.users-grid {
  &--animated {
    .user-card {
      opacity: 0;
      transform: translateY(20px) scale(0.96);
    }

    &.users-grid--visible {
      .user-card {
        animation: cardFadeIn 0.4s ease-out forwards;
        animation-delay: calc(var(--card-index, 0) * 60ms);
      }
    }
  }

  &:not(.users-grid--animated) {
    .user-card {
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
.user-card {
  &:hover {
    box-shadow:
      0 12px 28px rgba(0, 0, 0, 0.12),
      0 6px 12px rgba(0, 0, 0, 0.08);
    transform: translateY(-4px) scale(1.01);
    border-color: $intalio-teal-300;

    .user-avatar img {
      transform: scale(1.05);
    }
  }
}

// RTL adjustments
.rtl {
  .users-table--animated .table-row {
    transform: translateX(16px);
  }

  @keyframes tableRowFadeIn {
    from {
      opacity: 0;
      transform: translateX(16px);
    }
    to {
      opacity: 1;
      transform: translateX(0);
    }
  }

  .table-row:hover {
    transform: translateX(-4px);
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================
@media (prefers-reduced-motion: reduce) {
  .users-table {
    &--animated {
      .table-row {
        opacity: 1;
        transform: none;
        animation: none !important;
      }
    }
  }

  .users-grid {
    &--animated {
      .user-card {
        opacity: 1;
        transform: none;
        animation: none !important;
      }
    }
  }

  .table-row {
    transition: background-color 0.15s;

    &:hover {
      transform: none;
    }
  }

  .user-card {
    transition: background-color 0.15s, box-shadow 0.15s;

    &:hover {
      transform: none;

      .user-avatar img {
        transform: none;
      }
    }
  }
}
</style>
