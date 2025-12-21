<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import { PageHeader, StatsBar, EmptyState } from '@/components/base'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import type { StatItem } from '@/components/base/StatsBar.vue'
import ErrorState from '@/components/base/ErrorState.vue'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'

const { locale } = useI18n()
const prefersReducedMotion = useReducedMotion()

// RTL support
const isRTL = computed(() => locale.value === 'ar')

// Animation control
const isContentVisible = ref(false)
const shouldAnimate = computed(() => !prefersReducedMotion.value)
const LOADING_DELAY = 600

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: isRTL.value ? 'أعضاء الفريق' : 'Team Members' }
])

// Loading and error state
const loading = ref(true)
const error = ref<Error | null>(null)
const searchQuery = ref('')
const selectedDepartment = ref<string | null>(null)
const selectedRole = ref<string | null>(null)
const viewMode = ref<'grid' | 'list'>('grid')
const showMemberDialog = ref(false)
const selectedMember = ref<TeamMember | null>(null)
const showFilters = ref(false)

// Pagination
const currentPage = ref(1)
const itemsPerPage = ref(12)

// Filter type for type toggle
const filterType = ref<'all' | 'admin' | 'manager' | 'lead' | 'member'>('all')

// Type toggle options
const typeOptions = computed(() => [
  { value: 'all', label: isRTL.value ? 'الكل' : 'All', icon: 'pi-users' },
  { value: 'admin', label: isRTL.value ? 'مدراء' : 'Admins', icon: 'pi-shield' },
  { value: 'manager', label: isRTL.value ? 'مشرفين' : 'Managers', icon: 'pi-star' },
  { value: 'lead', label: isRTL.value ? 'قادة' : 'Leads', icon: 'pi-flag' },
  { value: 'member', label: isRTL.value ? 'أعضاء' : 'Members', icon: 'pi-user' }
])

// Status options for filter panel
const statusOptions = computed(() => [
  { value: 'all', label: isRTL.value ? 'جميع الحالات' : 'All Status' },
  { value: 'online', label: isRTL.value ? 'متصل' : 'Online' },
  { value: 'away', label: isRTL.value ? 'بعيد' : 'Away' },
  { value: 'busy', label: isRTL.value ? 'مشغول' : 'Busy' },
  { value: 'offline', label: isRTL.value ? 'غير متصل' : 'Offline' }
])

const filterStatus = ref<string>('all')

// Per page options
const perPageOptions = [
  { value: 8, label: '8' },
  { value: 12, label: '12' },
  { value: 24, label: '24' },
  { value: 48, label: '48' }
]

// Active filters count
const activeFiltersCount = computed(() => {
  let count = 0
  if (filterType.value !== 'all') count++
  if (selectedDepartment.value) count++
  if (filterStatus.value !== 'all') count++
  return count
})

// Clear all filters
const clearAllFilters = () => {
  filterType.value = 'all'
  selectedDepartment.value = null
  selectedRole.value = null
  filterStatus.value = 'all'
  searchQuery.value = ''
  currentPage.value = 1
}

// Reset pagination when filters change
const onFilterChange = () => {
  currentPage.value = 1
}

// Types
interface TeamMember {
  id: string
  name: string
  nameArabic: string
  email: string
  phone?: string
  avatarUrl?: string
  jobTitle: string
  jobTitleArabic: string
  department: string
  departmentArabic: string
  role: 'admin' | 'manager' | 'lead' | 'member'
  status: 'online' | 'away' | 'busy' | 'offline'
  projectsCount: number
  tasksCount: number
  joinedAt: Date
}

interface Department {
  id: string
  name: string
  nameArabic: string
  icon: string
  count: number
}

// Departments
const departments = ref<Department[]>([
  { id: 'operations', name: 'Operations', nameArabic: 'العمليات', icon: 'pi pi-cog', count: 45 },
  { id: 'media', name: 'Media & Communications', nameArabic: 'الإعلام والاتصالات', icon: 'pi pi-camera', count: 32 },
  { id: 'security', name: 'Security', nameArabic: 'الأمن', icon: 'pi pi-shield', count: 28 },
  { id: 'hospitality', name: 'Hospitality', nameArabic: 'الضيافة', icon: 'pi pi-heart', count: 38 },
  { id: 'technology', name: 'Technology', nameArabic: 'التكنولوجيا', icon: 'pi pi-desktop', count: 22 },
  { id: 'logistics', name: 'Logistics', nameArabic: 'اللوجستيات', icon: 'pi pi-truck', count: 35 }
])


// Team members
const members = ref<TeamMember[]>([
  {
    id: '1',
    name: 'Mohammed Al-Rashid',
    nameArabic: 'محمد الراشد',
    email: 'mohammed.rashid@afc2027.sa',
    phone: '+966 50 123 4567',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Mohammed',
    jobTitle: 'Operations Director',
    jobTitleArabic: 'مدير العمليات',
    department: 'operations',
    departmentArabic: 'العمليات',
    role: 'admin',
    status: 'online',
    projectsCount: 12,
    tasksCount: 24,
    joinedAt: new Date(Date.now() - 365 * 24 * 60 * 60 * 1000)
  },
  {
    id: '2',
    name: 'Sara Al-Ahmad',
    nameArabic: 'سارة الأحمد',
    email: 'sara.ahmad@afc2027.sa',
    phone: '+966 50 234 5678',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sara',
    jobTitle: 'Media Manager',
    jobTitleArabic: 'مديرة الإعلام',
    department: 'media',
    departmentArabic: 'الإعلام والاتصالات',
    role: 'manager',
    status: 'online',
    projectsCount: 8,
    tasksCount: 15,
    joinedAt: new Date(Date.now() - 280 * 24 * 60 * 60 * 1000)
  },
  {
    id: '3',
    name: 'Ahmed Hassan',
    nameArabic: 'أحمد حسن',
    email: 'ahmed.hassan@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Ahmed',
    jobTitle: 'Security Coordinator',
    jobTitleArabic: 'منسق الأمن',
    department: 'security',
    departmentArabic: 'الأمن',
    role: 'lead',
    status: 'away',
    projectsCount: 6,
    tasksCount: 18,
    joinedAt: new Date(Date.now() - 200 * 24 * 60 * 60 * 1000)
  },
  {
    id: '4',
    name: 'Fatima Khan',
    nameArabic: 'فاطمة خان',
    email: 'fatima.khan@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Fatima',
    jobTitle: 'Hospitality Lead',
    jobTitleArabic: 'قائدة الضيافة',
    department: 'hospitality',
    departmentArabic: 'الضيافة',
    role: 'lead',
    status: 'online',
    projectsCount: 5,
    tasksCount: 12,
    joinedAt: new Date(Date.now() - 150 * 24 * 60 * 60 * 1000)
  },
  {
    id: '5',
    name: 'Omar Khalid',
    nameArabic: 'عمر خالد',
    email: 'omar.khalid@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Omar',
    jobTitle: 'IT Specialist',
    jobTitleArabic: 'أخصائي تقنية المعلومات',
    department: 'technology',
    departmentArabic: 'التكنولوجيا',
    role: 'member',
    status: 'busy',
    projectsCount: 4,
    tasksCount: 9,
    joinedAt: new Date(Date.now() - 120 * 24 * 60 * 60 * 1000)
  },
  {
    id: '6',
    name: 'Layla Ahmad',
    nameArabic: 'ليلى أحمد',
    email: 'layla.ahmad@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Layla',
    jobTitle: 'Logistics Coordinator',
    jobTitleArabic: 'منسقة اللوجستيات',
    department: 'logistics',
    departmentArabic: 'اللوجستيات',
    role: 'member',
    status: 'online',
    projectsCount: 7,
    tasksCount: 14,
    joinedAt: new Date(Date.now() - 90 * 24 * 60 * 60 * 1000)
  },
  {
    id: '7',
    name: 'Yusuf Al-Farsi',
    nameArabic: 'يوسف الفارسي',
    email: 'yusuf.farsi@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Yusuf',
    jobTitle: 'Operations Analyst',
    jobTitleArabic: 'محلل عمليات',
    department: 'operations',
    departmentArabic: 'العمليات',
    role: 'member',
    status: 'offline',
    projectsCount: 3,
    tasksCount: 8,
    joinedAt: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000)
  },
  {
    id: '8',
    name: 'Nora Al-Qahtani',
    nameArabic: 'نورة القحطاني',
    email: 'nora.qahtani@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Nora',
    jobTitle: 'Content Specialist',
    jobTitleArabic: 'أخصائية محتوى',
    department: 'media',
    departmentArabic: 'الإعلام والاتصالات',
    role: 'member',
    status: 'online',
    projectsCount: 5,
    tasksCount: 11,
    joinedAt: new Date(Date.now() - 45 * 24 * 60 * 60 * 1000)
  },
  {
    id: '9',
    name: 'Khalid Al-Mutairi',
    nameArabic: 'خالد المطيري',
    email: 'khalid.mutairi@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Khalid',
    jobTitle: 'Security Officer',
    jobTitleArabic: 'ضابط أمن',
    department: 'security',
    departmentArabic: 'الأمن',
    role: 'member',
    status: 'online',
    projectsCount: 2,
    tasksCount: 6,
    joinedAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000)
  },
  {
    id: '10',
    name: 'Aisha Binti Omar',
    nameArabic: 'عائشة بنت عمر',
    email: 'aisha.omar@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Aisha',
    jobTitle: 'Guest Relations Manager',
    jobTitleArabic: 'مديرة علاقات الضيوف',
    department: 'hospitality',
    departmentArabic: 'الضيافة',
    role: 'manager',
    status: 'away',
    projectsCount: 9,
    tasksCount: 20,
    joinedAt: new Date(Date.now() - 180 * 24 * 60 * 60 * 1000)
  },
  {
    id: '11',
    name: 'Hassan Al-Dosari',
    nameArabic: 'حسن الدوسري',
    email: 'hassan.dosari@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Hassan',
    jobTitle: 'Systems Engineer',
    jobTitleArabic: 'مهندس أنظمة',
    department: 'technology',
    departmentArabic: 'التكنولوجيا',
    role: 'lead',
    status: 'online',
    projectsCount: 6,
    tasksCount: 13,
    joinedAt: new Date(Date.now() - 220 * 24 * 60 * 60 * 1000)
  },
  {
    id: '12',
    name: 'Maryam Al-Sulaiman',
    nameArabic: 'مريم السليمان',
    email: 'maryam.sulaiman@afc2027.sa',
    avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Maryam',
    jobTitle: 'Transport Coordinator',
    jobTitleArabic: 'منسقة النقل',
    department: 'logistics',
    departmentArabic: 'اللوجستيات',
    role: 'member',
    status: 'busy',
    projectsCount: 4,
    tasksCount: 10,
    joinedAt: new Date(Date.now() - 75 * 24 * 60 * 60 * 1000)
  }
])

// Computed stats
const statsBarItems = computed<StatItem[]>(() => {
  const total = members.value.length
  const online = members.value.filter(m => m.status === 'online').length
  const deptCount = departments.value.length
  const totalProjects = members.value.reduce((sum, m) => sum + m.projectsCount, 0)

  return [
    {
      icon: 'pi pi-users',
      value: total,
      label: 'Total Members',
      labelArabic: 'إجمالي الأعضاء',
      colorClass: 'primary'
    },
    {
      icon: 'pi pi-circle-fill',
      value: online,
      label: 'Online Now',
      labelArabic: 'متصل الآن',
      colorClass: 'success'
    },
    {
      icon: 'pi pi-sitemap',
      value: deptCount,
      label: 'Departments',
      labelArabic: 'الأقسام',
      colorClass: 'info'
    },
    {
      icon: 'pi pi-folder',
      value: totalProjects,
      label: 'Active Projects',
      labelArabic: 'المشاريع النشطة',
      colorClass: 'warning'
    }
  ]
})

// Filtered members
const filteredMembers = computed(() => {
  let result = [...members.value]

  // Type toggle filter (role-based)
  if (filterType.value !== 'all') {
    result = result.filter(m => m.role === filterType.value)
  }

  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(m =>
      m.name.toLowerCase().includes(query) ||
      m.nameArabic.includes(query) ||
      m.email.toLowerCase().includes(query) ||
      m.jobTitle.toLowerCase().includes(query)
    )
  }

  // Department filter
  if (selectedDepartment.value) {
    result = result.filter(m => m.department === selectedDepartment.value)
  }

  // Status filter
  if (filterStatus.value !== 'all') {
    result = result.filter(m => m.status === filterStatus.value)
  }

  // Role filter (from dropdown - keep for backward compatibility)
  if (selectedRole.value) {
    result = result.filter(m => m.role === selectedRole.value)
  }

  return result
})

// Pagination computed properties
const totalPages = computed(() => Math.ceil(filteredMembers.value.length / itemsPerPage.value))

const paginatedMembers = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredMembers.value.slice(start, end)
})

const paginationInfo = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value + 1
  const end = Math.min(currentPage.value * itemsPerPage.value, filteredMembers.value.length)
  const total = filteredMembers.value.length
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
function getName(member: TeamMember) {
  return isRTL.value ? member.nameArabic : member.name
}

function getJobTitle(member: TeamMember) {
  return isRTL.value ? member.jobTitleArabic : member.jobTitle
}

function getDepartmentName(deptId: string) {
  const dept = departments.value.find(d => d.id === deptId)
  if (!dept) return deptId
  return isRTL.value ? dept.nameArabic : dept.name
}

function getRoleBadgeClass(role: string) {
  return role
}

function getRoleLabel(role: string) {
  const labels: Record<string, { en: string; ar: string }> = {
    admin: { en: 'Admin', ar: 'مدير' },
    manager: { en: 'Manager', ar: 'مشرف' },
    lead: { en: 'Team Lead', ar: 'قائد فريق' },
    member: { en: 'Member', ar: 'عضو' }
  }
  return isRTL.value ? labels[role]?.ar : labels[role]?.en
}

function getInitials(name: string) {
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
}

function formatDate(date: Date) {
  return new Intl.DateTimeFormat(isRTL.value ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'short'
  }).format(date)
}

function openMemberProfile(member: TeamMember) {
  selectedMember.value = member
  showMemberDialog.value = true
}

function sendEmail(member: TeamMember) {
  window.location.href = `mailto:${member.email}`
}

// Lifecycle
async function loadTeamMembers() {
  try {
    error.value = null
    loading.value = true

    await new Promise(resolve => setTimeout(resolve, LOADING_DELAY))

    loading.value = false

    if (shouldAnimate.value) {
      requestAnimationFrame(() => {
        isContentVisible.value = true
      })
    } else {
      isContentVisible.value = true
    }
  } catch (e) {
    error.value = e as Error
    loading.value = false
  }
}

async function handleRetry() {
  await loadTeamMembers()
}

onMounted(() => {
  loadTeamMembers()
})
</script>

<template>
  <div class="team-members-view" :class="{ 'rtl': isRTL }">
    <!-- Page Header -->
    <PageHeader
      :title="isRTL ? 'أعضاء الفريق' : 'Team Members'"
      :description="isRTL ? 'تواصل مع زملائك في فريق كأس آسيا 2027' : 'Connect with your AFC Asian Cup 2027 team colleagues'"
      :breadcrumbs="breadcrumbs"
      padding-bottom="xl"
      background-variant="gradient"
      :animated="shouldAnimate"
    >
      <template #actions>
        <Button
          :label="isRTL ? 'تصدير' : 'Export'"
          icon="pi pi-download"
          class="header-btn-secondary"
        />
        <Button
          :label="isRTL ? 'دعوة عضو' : 'Invite Member'"
          icon="pi pi-user-plus"
          class="header-btn-primary"
        />
      </template>
    </PageHeader>

    <!-- Stats Bar -->
    <StatsBar
      :stats="statsBarItems"
      :loading="loading"
      overlap="lg"
      :animated="shouldAnimate"
      :animate-numbers="shouldAnimate"
      :counter-duration="1200"
    />

    <!-- Loading State -->
    <div v-if="loading" class="main-content">
      <div class="loading-state">
        <!-- Toolbar Skeleton -->
        <Skeleton height="60px" class="mb-4" borderRadius="16px" />

        <!-- Department Chips Skeleton -->
        <div class="department-chips-skeleton">
          <Skeleton v-for="i in 6" :key="i" width="120px" height="36px" borderRadius="20px" />
        </div>

        <!-- Cards Skeleton -->
        <div class="members-grid">
          <Skeleton v-for="i in 8" :key="i" height="280px" borderRadius="16px" />
        </div>
      </div>
    </div>

    <!-- Error State -->
    <ErrorState
      v-else-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل أعضاء الفريق' : 'Failed to load team members'"
      show-retry
      @retry="handleRetry"
    />

    <!-- Main Content -->
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
                :placeholder="isRTL ? 'البحث عن أعضاء...' : 'Search members...'"
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
                @click="filterType = option.value as 'all' | 'admin' | 'manager' | 'lead' | 'member'; onFilterChange()"
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
                :class="{ active: viewMode === 'grid' }"
                @click="viewMode = 'grid'"
                :title="isRTL ? 'عرض الشبكة' : 'Grid View'"
              >
                <i class="pi pi-th-large"></i>
              </button>
              <button
                :class="{ active: viewMode === 'list' }"
                @click="viewMode = 'list'"
                :title="isRTL ? 'عرض القائمة' : 'List View'"
              >
                <i class="pi pi-list"></i>
              </button>
            </div>
          </div>
        </div>

        <!-- Expanded Filters Panel -->
        <div v-if="showFilters" class="filters-panel">
          <div class="filter-group">
            <label class="filter-label">{{ isRTL ? 'القسم' : 'Department' }}</label>
            <div class="filter-options">
              <button
                :class="['filter-chip', { active: !selectedDepartment }]"
                @click="selectedDepartment = null; onFilterChange()"
              >
                {{ isRTL ? 'الكل' : 'All' }}
              </button>
              <button
                v-for="dept in departments"
                :key="dept.id"
                :class="['filter-chip', { active: selectedDepartment === dept.id }]"
                @click="selectedDepartment = dept.id; onFilterChange()"
              >
                <i :class="dept.icon"></i>
                {{ isRTL ? dept.nameArabic : dept.name }}
              </button>
            </div>
          </div>

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

          <span v-if="selectedDepartment" class="filter-tag">
            <i class="pi pi-building"></i>
            {{ isRTL ? departments.find(d => d.id === selectedDepartment)?.nameArabic : departments.find(d => d.id === selectedDepartment)?.name }}
            <button @click="selectedDepartment = null; onFilterChange()"><i class="pi pi-times"></i></button>
          </span>

          <span v-if="filterStatus !== 'all'" class="filter-tag">
            {{ statusOptions.find(o => o.value === filterStatus)?.label }}
            <button @click="filterStatus = 'all'; onFilterChange()"><i class="pi pi-times"></i></button>
          </span>

          <button class="clear-all-link" @click="clearAllFilters">
            {{ isRTL ? 'مسح الكل' : 'Clear all' }}
          </button>
        </div>
      </div>

      <!-- Members Grid -->
      <div v-if="viewMode === 'grid'" class="members-grid">
        <div
          v-for="member in paginatedMembers"
          :key="member.id"
          class="member-card"
          @click="openMemberProfile(member)"
        >
          <!-- Avatar -->
          <div class="member-avatar" :class="{ 'avatar-fallback': !member.avatarUrl }">
            <img v-if="member.avatarUrl" :src="member.avatarUrl" :alt="getName(member)" />
            <span v-else>{{ getInitials(member.name) }}</span>
            <span class="status-indicator" :class="member.status"></span>
          </div>

          <!-- Info -->
          <div class="member-info">
            <h3 class="member-name">{{ getName(member) }}</h3>
            <p class="member-role">{{ getJobTitle(member) }}</p>
            <span class="member-department">
              <i class="pi pi-building"></i>
              {{ getDepartmentName(member.department) }}
            </span>
            <span class="member-role-badge" :class="getRoleBadgeClass(member.role)">
              {{ getRoleLabel(member.role) }}
            </span>
          </div>

          <!-- Stats -->
          <div class="member-stats">
            <div class="member-stat">
              <span class="stat-value">{{ member.projectsCount }}</span>
              <span class="stat-label">{{ isRTL ? 'مشاريع' : 'Projects' }}</span>
            </div>
            <div class="member-stat">
              <span class="stat-value">{{ member.tasksCount }}</span>
              <span class="stat-label">{{ isRTL ? 'مهام' : 'Tasks' }}</span>
            </div>
          </div>

          <!-- Actions -->
          <div class="member-actions">
            <button class="member-action-btn" @click.stop="sendEmail(member)" :title="isRTL ? 'إرسال بريد' : 'Send Email'">
              <i class="pi pi-envelope"></i>
            </button>
            <button class="member-action-btn" @click.stop :title="isRTL ? 'رسالة' : 'Message'">
              <i class="pi pi-comments"></i>
            </button>
            <button class="member-action-btn primary" @click.stop="openMemberProfile(member)" :title="isRTL ? 'عرض الملف' : 'View Profile'">
              <i class="pi pi-user"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Members List -->
      <div v-else class="members-list">
        <div
          v-for="member in paginatedMembers"
          :key="member.id"
          class="member-list-item"
          @click="openMemberProfile(member)"
        >
          <!-- Avatar -->
          <div class="member-avatar small" :class="{ 'avatar-fallback': !member.avatarUrl }">
            <img v-if="member.avatarUrl" :src="member.avatarUrl" :alt="getName(member)" />
            <span v-else>{{ getInitials(member.name) }}</span>
            <span class="status-indicator" :class="member.status"></span>
          </div>

          <!-- Info -->
          <div class="member-info">
            <h3 class="member-name">{{ getName(member) }}</h3>
            <p class="member-role">{{ getJobTitle(member) }}</p>
          </div>

          <!-- Department -->
          <span class="member-department">
            <i class="pi pi-building"></i>
            {{ getDepartmentName(member.department) }}
          </span>

          <!-- Role Badge -->
          <span class="member-role-badge" :class="getRoleBadgeClass(member.role)">
            {{ getRoleLabel(member.role) }}
          </span>

          <!-- Stats -->
          <div class="list-stats">
            <span><i class="pi pi-folder"></i> {{ member.projectsCount }}</span>
            <span><i class="pi pi-check-square"></i> {{ member.tasksCount }}</span>
          </div>

          <!-- Actions -->
          <div class="member-actions">
            <button class="member-action-btn" @click.stop="sendEmail(member)">
              <i class="pi pi-envelope"></i>
            </button>
            <button class="member-action-btn primary" @click.stop="openMemberProfile(member)">
              <i class="pi pi-user"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <EmptyState
        v-if="filteredMembers.length === 0"
        icon="pi-users"
        title="No Members Found"
        title-arabic="لم يتم العثور على أعضاء"
        description="Try adjusting your search criteria"
        description-arabic="حاول تعديل معايير البحث"
        action-label="Clear Filters"
        action-label-arabic="مسح الفلاتر"
        action-icon="pi-times"
        variant="search"
        @action="clearAllFilters"
      />

      <!-- Pagination -->
      <div v-if="filteredMembers.length > 0 && totalPages > 1" class="pagination-wrapper">
        <div class="pagination-info">
          <span>
            {{ isRTL ? `عرض ${paginationInfo.start} إلى ${paginationInfo.end} من ${paginationInfo.total} عضو` : `Showing ${paginationInfo.start} to ${paginationInfo.end} of ${paginationInfo.total} members` }}
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

    <!-- Member Profile Dialog -->
    <Dialog
      v-model:visible="showMemberDialog"
      :header="selectedMember ? getName(selectedMember) : ''"
      :style="{ width: '500px' }"
      :modal="true"
      :dismissableMask="true"
      class="member-dialog"
    >
      <div v-if="selectedMember" class="member-profile">
        <div class="profile-header">
          <div class="profile-avatar" :class="{ 'avatar-fallback': !selectedMember.avatarUrl }">
            <img v-if="selectedMember.avatarUrl" :src="selectedMember.avatarUrl" :alt="getName(selectedMember)" />
            <span v-else>{{ getInitials(selectedMember.name) }}</span>
            <span class="status-indicator large" :class="selectedMember.status"></span>
          </div>
          <div class="profile-info">
            <h2>{{ getName(selectedMember) }}</h2>
            <p class="job-title">{{ getJobTitle(selectedMember) }}</p>
            <span class="member-role-badge" :class="getRoleBadgeClass(selectedMember.role)">
              {{ getRoleLabel(selectedMember.role) }}
            </span>
          </div>
        </div>

        <div class="profile-details">
          <div class="detail-item">
            <i class="pi pi-envelope"></i>
            <div>
              <label>{{ isRTL ? 'البريد الإلكتروني' : 'Email' }}</label>
              <span>{{ selectedMember.email }}</span>
            </div>
          </div>
          <div v-if="selectedMember.phone" class="detail-item">
            <i class="pi pi-phone"></i>
            <div>
              <label>{{ isRTL ? 'الهاتف' : 'Phone' }}</label>
              <span>{{ selectedMember.phone }}</span>
            </div>
          </div>
          <div class="detail-item">
            <i class="pi pi-building"></i>
            <div>
              <label>{{ isRTL ? 'القسم' : 'Department' }}</label>
              <span>{{ getDepartmentName(selectedMember.department) }}</span>
            </div>
          </div>
          <div class="detail-item">
            <i class="pi pi-calendar"></i>
            <div>
              <label>{{ isRTL ? 'تاريخ الانضمام' : 'Joined' }}</label>
              <span>{{ formatDate(selectedMember.joinedAt) }}</span>
            </div>
          </div>
        </div>

        <div class="profile-stats">
          <div class="profile-stat">
            <span class="value">{{ selectedMember.projectsCount }}</span>
            <span class="label">{{ isRTL ? 'مشاريع' : 'Projects' }}</span>
          </div>
          <div class="profile-stat">
            <span class="value">{{ selectedMember.tasksCount }}</span>
            <span class="label">{{ isRTL ? 'مهام' : 'Tasks' }}</span>
          </div>
        </div>
      </div>

      <template #footer>
        <Button
          :label="isRTL ? 'إرسال رسالة' : 'Send Message'"
          icon="pi pi-comments"
          severity="secondary"
        />
        <Button
          :label="isRTL ? 'إرسال بريد' : 'Send Email'"
          icon="pi pi-envelope"
          @click="selectedMember && sendEmail(selectedMember)"
        />
      </template>
    </Dialog>
  </div>
</template>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

// ============================================
// PAGE CONTAINER
// ============================================

.team-members-view {
  @include page-view;

  &.rtl {
    direction: rtl;

    .breadcrumb-separator {
      transform: rotate(180deg);
    }
  }
}

// Header button styles (used in PageHeader slot)
.header-btn-primary {
  @include header-btn-primary;
}

.header-btn-secondary {
  @include header-btn-secondary;
}

// ============================================
// MAIN CONTENT
// ============================================

.main-content {
  @include page-content;
}

.loading-state {
  .department-chips-skeleton {
    display: flex;
    gap: $spacing-3;
    margin-bottom: $spacing-6;
    flex-wrap: wrap;
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

.view-toggle {
  @include premium-view-toggle;
}

// ============================================
// MEMBERS GRID
// ============================================

.members-grid {
  @include members-grid;
  @include members-grid-animated;
}

.member-card {
  @include member-card;
}

.member-avatar {
  @include member-avatar(96px);

  &.small {
    @include member-avatar(56px);
    margin-bottom: 0;
  }
}

.member-info {
  @include member-info;
}

.member-name {
  @include member-name;
}

.member-role {
  @include member-role;
}

.member-department {
  @include member-department;
}

.member-role-badge {
  @include member-role-badge;
}

.member-stats {
  @include member-stats;
}

.member-stat {
  @include member-stat;
}

.member-actions {
  @include member-actions;
}

.member-action-btn {
  @include member-action-btn;
}

// ============================================
// MEMBERS LIST
// ============================================

.members-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;
}

.member-list-item {
  @include member-list-item;

  .member-info {
    align-items: flex-start;
    text-align: start;
    flex: 1;
  }

  .member-department {
    margin-top: 0;
  }

  .member-role-badge {
    margin-top: 0;
  }

  .list-stats {
    display: flex;
    gap: $spacing-4;
    color: $slate-500;
    font-size: $font-size-sm;

    span {
      display: flex;
      align-items: center;
      gap: $spacing-1;

      i {
        font-size: 0.875rem;
      }
    }
  }

  .member-actions {
    margin-top: 0;
    width: auto;
  }

  @include mobile {
    flex-wrap: wrap;

    .member-info {
      flex-basis: calc(100% - 72px);
    }

    .member-department,
    .member-role-badge,
    .list-stats {
      display: none;
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
// MEMBER DIALOG
// ============================================

.member-dialog {
  .member-profile {
    .profile-header {
      display: flex;
      gap: $spacing-5;
      padding-bottom: $spacing-5;
      border-bottom: 1px solid $slate-100;
      margin-bottom: $spacing-5;
    }

    .profile-avatar {
      @include member-avatar(80px);
      margin-bottom: 0;

      .status-indicator.large {
        width: 20px;
        height: 20px;
        bottom: 4px;
        right: 4px;
        border-width: 3px;
      }
    }

    .profile-info {
      flex: 1;

      h2 {
        margin: 0 0 $spacing-1 0;
        font-size: $font-size-xl;
        font-weight: $font-weight-bold;
        color: $slate-900;
      }

      .job-title {
        margin: 0 0 $spacing-2 0;
        color: $slate-500;
      }

      .member-role-badge {
        margin-top: 0;
      }
    }

    .profile-details {
      display: flex;
      flex-direction: column;
      gap: $spacing-4;
      margin-bottom: $spacing-5;
    }

    .detail-item {
      display: flex;
      align-items: flex-start;
      gap: $spacing-3;

      > i {
        width: 36px;
        height: 36px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: $slate-100;
        border-radius: $radius-lg;
        color: $intalio-teal-500;
        flex-shrink: 0;
      }

      > div {
        display: flex;
        flex-direction: column;
        gap: $spacing-1;

        label {
          font-size: $font-size-xs;
          color: $slate-500;
          text-transform: uppercase;
          letter-spacing: 0.5px;
        }

        span {
          color: $slate-900;
          font-weight: $font-weight-medium;
        }
      }
    }

    .profile-stats {
      display: flex;
      gap: $spacing-6;
      padding: $spacing-4;
      background: $slate-50;
      border-radius: $radius-xl;
    }

    .profile-stat {
      flex: 1;
      text-align: center;

      .value {
        display: block;
        font-size: $font-size-2xl;
        font-weight: $font-weight-bold;
        color: $intalio-teal-600;
      }

      .label {
        font-size: $font-size-sm;
        color: $slate-500;
      }
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
</style>
