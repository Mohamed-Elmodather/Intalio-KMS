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
  { label: isRTL.value ? 'إدارة المجموعات' : 'Group Management' }
])

// State
const isLoading = ref(true)
const error = ref<Error | null>(null)
const searchQuery = ref('')
const showGroupDialog = ref(false)
const showDeleteDialog = ref(false)
const showMembersDialog = ref(false)
const showAddMembersDialog = ref(false)
const viewMode = ref<'grid' | 'list'>('grid')
const selectedGroups = ref<string[]>([])

// Pagination
const currentPage = ref(1)
const itemsPerPage = ref(10)

// Filter type
const filterType = ref<'all' | 'department' | 'committee' | 'project'>('all')

// Type toggle options
const typeOptions = computed(() => [
  { value: 'all', label: isRTL.value ? 'الكل' : 'All', icon: 'pi-list' },
  { value: 'department', label: isRTL.value ? 'قسم' : 'Department', icon: 'pi-building' },
  { value: 'committee', label: isRTL.value ? 'لجنة' : 'Committee', icon: 'pi-users' },
  { value: 'project', label: isRTL.value ? 'مشروع' : 'Project', icon: 'pi-briefcase' }
])

// Per page options
const perPageOptions = [
  { value: 5, label: '5' },
  { value: 10, label: '10' },
  { value: 20, label: '20' },
  { value: 50, label: '50' }
]

// Types
interface Group {
  id: string
  name: string
  nameArabic: string
  description: string
  descriptionArabic: string
  membersCount: number
  type: 'department' | 'committee' | 'project'
  lead: string
  leadArabic: string
  leadAvatar: string
  status: 'active' | 'inactive'
  createdAt: Date
}

// Groups data
const groups = ref<Group[]>([
  {
    id: '1',
    name: 'Operations Team',
    nameArabic: 'فريق العمليات',
    description: 'Stadium operations and logistics management',
    descriptionArabic: 'إدارة عمليات الملاعب والخدمات اللوجستية',
    membersCount: 45,
    type: 'department',
    lead: 'Ahmed Hassan',
    leadArabic: 'أحمد حسن',
    leadAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Ahmed',
    status: 'active',
    createdAt: new Date(Date.now() - 365 * 24 * 60 * 60 * 1000)
  },
  {
    id: '2',
    name: 'Media Team',
    nameArabic: 'فريق الإعلام',
    description: 'Content creation, broadcasting and media relations',
    descriptionArabic: 'إنشاء المحتوى والبث والعلاقات الإعلامية',
    membersCount: 22,
    type: 'department',
    lead: 'Sara Ali',
    leadArabic: 'سارة علي',
    leadAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sara',
    status: 'active',
    createdAt: new Date(Date.now() - 300 * 24 * 60 * 60 * 1000)
  },
  {
    id: '3',
    name: 'Security Committee',
    nameArabic: 'لجنة الأمن',
    description: 'Security planning, crowd management and safety protocols',
    descriptionArabic: 'تخطيط الأمن وإدارة الحشود وبروتوكولات السلامة',
    membersCount: 18,
    type: 'committee',
    lead: 'Khalid Al-Mutairi',
    leadArabic: 'خالد المطيري',
    leadAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Khalid',
    status: 'active',
    createdAt: new Date(Date.now() - 280 * 24 * 60 * 60 * 1000)
  },
  {
    id: '4',
    name: 'Volunteers Coordination',
    nameArabic: 'تنسيق المتطوعين',
    description: 'Volunteer recruitment, training and deployment',
    descriptionArabic: 'توظيف وتدريب ونشر المتطوعين',
    membersCount: 120,
    type: 'project',
    lead: 'Fatima Al-Rashid',
    leadArabic: 'فاطمة الراشد',
    leadAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Fatima',
    status: 'active',
    createdAt: new Date(Date.now() - 200 * 24 * 60 * 60 * 1000)
  },
  {
    id: '5',
    name: 'Technology & IT',
    nameArabic: 'التكنولوجيا وتقنية المعلومات',
    description: 'IT infrastructure, systems and digital solutions',
    descriptionArabic: 'البنية التحتية لتقنية المعلومات والأنظمة والحلول الرقمية',
    membersCount: 35,
    type: 'department',
    lead: 'Omar Khalid',
    leadArabic: 'عمر خالد',
    leadAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Omar',
    status: 'active',
    createdAt: new Date(Date.now() - 180 * 24 * 60 * 60 * 1000)
  },
  {
    id: '6',
    name: 'Hospitality Committee',
    nameArabic: 'لجنة الضيافة',
    description: 'VIP services, hospitality and guest experience',
    descriptionArabic: 'خدمات كبار الشخصيات والضيافة وتجربة الضيوف',
    membersCount: 28,
    type: 'committee',
    lead: 'Layla Ahmad',
    leadArabic: 'ليلى أحمد',
    leadAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Layla',
    status: 'active',
    createdAt: new Date(Date.now() - 150 * 24 * 60 * 60 * 1000)
  },
  {
    id: '7',
    name: 'Transportation Project',
    nameArabic: 'مشروع النقل',
    description: 'Fan transportation and mobility solutions',
    descriptionArabic: 'حلول نقل المشجعين والتنقل',
    membersCount: 42,
    type: 'project',
    lead: 'Yusuf Al-Farsi',
    leadArabic: 'يوسف الفارسي',
    leadAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Yusuf',
    status: 'active',
    createdAt: new Date(Date.now() - 120 * 24 * 60 * 60 * 1000)
  },
  {
    id: '8',
    name: 'Marketing & Communications',
    nameArabic: 'التسويق والاتصالات',
    description: 'Brand management, marketing campaigns and PR',
    descriptionArabic: 'إدارة العلامة التجارية وحملات التسويق والعلاقات العامة',
    membersCount: 30,
    type: 'department',
    lead: 'Nora Al-Qahtani',
    leadArabic: 'نورة القحطاني',
    leadAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Nora',
    status: 'active',
    createdAt: new Date(Date.now() - 90 * 24 * 60 * 60 * 1000)
  },
  {
    id: '9',
    name: 'Sustainability Initiative',
    nameArabic: 'مبادرة الاستدامة',
    description: 'Environmental sustainability and green initiatives',
    descriptionArabic: 'الاستدامة البيئية والمبادرات الخضراء',
    membersCount: 15,
    type: 'project',
    lead: 'Maryam Al-Sulaiman',
    leadArabic: 'مريم السليمان',
    leadAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Maryam',
    status: 'inactive',
    createdAt: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000)
  },
  {
    id: '10',
    name: 'Legal & Compliance',
    nameArabic: 'الشؤون القانونية والامتثال',
    description: 'Legal affairs, contracts and regulatory compliance',
    descriptionArabic: 'الشؤون القانونية والعقود والامتثال التنظيمي',
    membersCount: 12,
    type: 'department',
    lead: 'Hassan Al-Dosari',
    leadArabic: 'حسن الدوسري',
    leadAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Hassan',
    status: 'active',
    createdAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000)
  }
])

// Computed stats for StatsBar
const stats = computed<StatItem[]>(() => {
  const total = groups.value.length
  const departments = groups.value.filter(g => g.type === 'department').length
  const committees = groups.value.filter(g => g.type === 'committee').length
  const totalMembers = groups.value.reduce((sum, g) => sum + g.membersCount, 0)

  return [
    {
      icon: 'pi pi-sitemap',
      value: total,
      label: 'Total Groups',
      labelArabic: 'إجمالي المجموعات',
      colorClass: 'primary'
    },
    {
      icon: 'pi pi-building',
      value: departments,
      label: 'Departments',
      labelArabic: 'الأقسام',
      colorClass: 'info'
    },
    {
      icon: 'pi pi-users',
      value: committees,
      label: 'Committees',
      labelArabic: 'اللجان',
      colorClass: 'success'
    },
    {
      icon: 'pi pi-user',
      value: totalMembers,
      label: 'Total Members',
      labelArabic: 'إجمالي الأعضاء',
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

// Filtered groups
const filteredGroups = computed(() => {
  let result = [...groups.value]

  // Type filter
  if (filterType.value !== 'all') {
    result = result.filter(g => g.type === filterType.value)
  }

  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(g =>
      g.name.toLowerCase().includes(query) ||
      g.nameArabic.includes(query) ||
      g.description.toLowerCase().includes(query)
    )
  }

  return result
})

// Pagination computed properties
const totalPages = computed(() => Math.ceil(filteredGroups.value.length / itemsPerPage.value))

const paginatedGroups = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredGroups.value.slice(start, end)
})

const paginationInfo = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value + 1
  const end = Math.min(currentPage.value * itemsPerPage.value, filteredGroups.value.length)
  const total = filteredGroups.value.length
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
function getName(group: Group) {
  return isRTL.value ? group.nameArabic : group.name
}

function getDescription(group: Group) {
  return isRTL.value ? group.descriptionArabic : group.description
}

function getLeadName(group: Group) {
  return isRTL.value ? group.leadArabic : group.lead
}

function getTypeLabel(type: string) {
  const labels: Record<string, { en: string; ar: string }> = {
    department: { en: 'Department', ar: 'قسم' },
    committee: { en: 'Committee', ar: 'لجنة' },
    project: { en: 'Project', ar: 'مشروع' }
  }
  return isRTL.value ? labels[type]?.ar : labels[type]?.en
}

function getTypeSeverity(type: string): "info" | "success" | "warn" | "secondary" {
  const severities: Record<string, "info" | "success" | "warn" | "secondary"> = {
    department: 'info',
    committee: 'success',
    project: 'warn'
  }
  return severities[type] || 'secondary'
}

function getTypeIcon(type: string) {
  const icons: Record<string, string> = {
    department: 'pi-building',
    committee: 'pi-users',
    project: 'pi-briefcase'
  }
  return icons[type] || 'pi-sitemap'
}

function getTypeColor(type: string) {
  const colors: Record<string, string> = {
    department: '#3b82f6',
    committee: '#10b981',
    project: '#f59e0b'
  }
  return colors[type] || '#64748b'
}

function getStatusLabel(status: string) {
  const labels: Record<string, { en: string; ar: string }> = {
    active: { en: 'Active', ar: 'نشط' },
    inactive: { en: 'Inactive', ar: 'غير نشط' }
  }
  return isRTL.value ? labels[status]?.ar : labels[status]?.en
}

function getStatusSeverity(status: string): "success" | "secondary" {
  return status === 'active' ? 'success' : 'secondary'
}

// Selection
function toggleSelect(id: string) {
  const index = selectedGroups.value.indexOf(id)
  if (index === -1) {
    selectedGroups.value.push(id)
  } else {
    selectedGroups.value.splice(index, 1)
  }
}

function isSelected(id: string) {
  return selectedGroups.value.includes(id)
}

// Action menu
const actionMenu = ref()
const currentGroup = ref<Group | null>(null)

const actionMenuItems = computed(() => [
  { label: isRTL.value ? 'عرض الأعضاء' : 'View Members', icon: 'pi pi-users', command: () => viewMembers(currentGroup.value) },
  { label: isRTL.value ? 'تعديل' : 'Edit', icon: 'pi pi-pencil', command: () => editGroup(currentGroup.value) },
  { label: isRTL.value ? 'إضافة أعضاء' : 'Add Members', icon: 'pi pi-user-plus', command: () => addMembers(currentGroup.value) },
  { separator: true },
  { label: isRTL.value ? 'حذف' : 'Delete', icon: 'pi pi-trash', class: 'text-red-500', command: () => deleteGroup(currentGroup.value) }
])

function toggleActionMenu(event: Event, group: Group) {
  currentGroup.value = group
  actionMenu.value.toggle(event)
}

function viewMembers(group: Group | null) {
  if (group) {
    currentGroup.value = group
    showMembersDialog.value = true
  }
}

function editGroup(group: Group | null) {
  if (group) {
    currentGroup.value = group
  }
  showGroupDialog.value = true
}

function addMembers(group: Group | null) {
  if (group) {
    currentGroup.value = group
    showAddMembersDialog.value = true
  }
}

function confirmAddMembers() {
  // In a real app, this would call an API to add selected members
  if (currentGroup.value) {
    currentGroup.value.membersCount += 1 // Simulate adding a member
    showAddMembersDialog.value = false
  }
}

function deleteGroup(group: Group | null) {
  if (group) {
    currentGroup.value = group
    showDeleteDialog.value = true
  }
}

function confirmDeleteGroup() {
  if (currentGroup.value) {
    groups.value = groups.value.filter(g => g.id !== currentGroup.value?.id)
    showDeleteDialog.value = false
    currentGroup.value = null
  }
}

function addGroup() {
  currentGroup.value = null
  showGroupDialog.value = true
}

// Data loading with error handling
async function loadGroups() {
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
  loadGroups()
}

// Lifecycle
onMounted(() => {
  loadGroups()
})
</script>

<template>
  <div class="groups-view" :class="{ 'rtl': isRTL }">
    <!-- Error State -->
    <ErrorState
      v-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل المجموعات' : 'Failed to load groups'"
      show-retry
      size="lg"
      @retry="handleRetry"
    />

    <template v-else>
    <!-- Page Header -->
    <PageHeader
      :title="isRTL ? 'إدارة المجموعات' : 'Group Management'"
      :description="isRTL ? 'إدارة الأقسام واللجان والمشاريع' : 'Manage departments, committees and projects'"
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
          :label="isRTL ? 'إضافة مجموعة' : 'Add Group'"
          icon="pi pi-plus"
          @click="addGroup"
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
        <div class="groups-grid">
          <Skeleton v-for="i in 6" :key="i" height="240px" borderRadius="16px" />
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
                :placeholder="isRTL ? 'البحث عن مجموعات...' : 'Search groups...'"
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
                @click="filterType = option.value as 'all' | 'department' | 'committee' | 'project'; onFilterChange()"
              >
                <i :class="['pi', option.icon]"></i>
                <span>{{ option.label }}</span>
              </button>
            </div>

            <!-- Bulk Actions -->
            <div class="bulk-actions" v-if="selectedGroups.length > 0">
              <span class="selection-count">
                {{ selectedGroups.length }} {{ isRTL ? 'محدد' : 'selected' }}
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

      <!-- Groups List View -->
      <div
        v-if="viewMode === 'list'"
        class="groups-table"
        :class="{
          'groups-table--animated': shouldAnimate,
          'groups-table--visible': isContentVisible
        }"
      >
        <div class="table-header">
          <div class="table-cell checkbox-cell">
            <Checkbox :binary="true" />
          </div>
          <div class="table-cell group-cell">{{ isRTL ? 'المجموعة' : 'Group' }}</div>
          <div class="table-cell type-cell">{{ isRTL ? 'النوع' : 'Type' }}</div>
          <div class="table-cell lead-cell">{{ isRTL ? 'المسؤول' : 'Lead' }}</div>
          <div class="table-cell members-cell">{{ isRTL ? 'الأعضاء' : 'Members' }}</div>
          <div class="table-cell status-cell">{{ isRTL ? 'الحالة' : 'Status' }}</div>
          <div class="table-cell actions-cell">{{ isRTL ? 'إجراءات' : 'Actions' }}</div>
        </div>

        <div
          v-for="(group, index) in paginatedGroups"
          :key="group.id"
          class="table-row"
          :class="{ selected: isSelected(group.id) }"
          :style="shouldAnimate ? { '--row-index': index } : undefined"
        >
          <div class="table-cell checkbox-cell" @click.stop>
            <Checkbox
              :modelValue="isSelected(group.id)"
              :binary="true"
              @change="toggleSelect(group.id)"
            />
          </div>
          <div class="table-cell group-cell">
            <div class="group-info">
              <div class="group-icon" :style="{ background: getTypeColor(group.type) + '20', color: getTypeColor(group.type) }">
                <i class="pi pi-sitemap"></i>
              </div>
              <div class="group-details">
                <span class="group-name">{{ getName(group) }}</span>
                <span class="group-description">{{ getDescription(group) }}</span>
              </div>
            </div>
          </div>
          <div class="table-cell type-cell">
            <Tag :value="getTypeLabel(group.type)" :severity="getTypeSeverity(group.type)" />
          </div>
          <div class="table-cell lead-cell">
            <div class="lead-info">
              <img :src="group.leadAvatar" :alt="getLeadName(group)" class="lead-avatar" />
              <span class="lead-name">{{ getLeadName(group) }}</span>
            </div>
          </div>
          <div class="table-cell members-cell">
            <span class="members-count">{{ group.membersCount }}</span>
          </div>
          <div class="table-cell status-cell">
            <Tag :value="getStatusLabel(group.status)" :severity="getStatusSeverity(group.status)" />
          </div>
          <div class="table-cell actions-cell">
            <Button
              icon="pi pi-ellipsis-v"
              text
              rounded
              size="small"
              class="action-menu-btn"
              @click.stop="(e: Event) => toggleActionMenu(e, group)"
            />
          </div>
        </div>

        <Menu ref="actionMenu" :model="actionMenuItems" popup class="action-menu" />
      </div>

      <!-- Groups Grid View -->
      <div
        v-else
        class="groups-grid"
        :class="{
          'groups-grid--animated': shouldAnimate,
          'groups-grid--visible': isContentVisible
        }"
      >
        <div
          v-for="(group, index) in paginatedGroups"
          :key="group.id"
          class="group-card"
          :class="{ selected: isSelected(group.id) }"
          :style="shouldAnimate ? { '--card-index': index } : undefined"
        >
          <div class="card-select" @click.stop>
            <Checkbox
              :modelValue="isSelected(group.id)"
              :binary="true"
              @change="toggleSelect(group.id)"
            />
          </div>

          <!-- Card Header -->
          <div class="card-header">
            <div class="group-icon large" :style="{ background: getTypeColor(group.type) + '20', color: getTypeColor(group.type) }">
              <i class="pi pi-sitemap"></i>
            </div>
            <div class="card-badges">
              <Tag :value="getTypeLabel(group.type)" :severity="getTypeSeverity(group.type)" class="type-tag" />
              <Tag :value="getStatusLabel(group.status)" :severity="getStatusSeverity(group.status)" class="status-tag" />
            </div>
          </div>

          <!-- Card Info -->
          <div class="card-info">
            <h3 class="group-name">{{ getName(group) }}</h3>
            <p class="group-description">{{ getDescription(group) }}</p>
          </div>

          <!-- Lead Info -->
          <div class="card-lead">
            <span class="lead-label">{{ isRTL ? 'المسؤول:' : 'Lead:' }}</span>
            <div class="lead-info">
              <img :src="group.leadAvatar" :alt="getLeadName(group)" class="lead-avatar" />
              <span class="lead-name">{{ getLeadName(group) }}</span>
            </div>
          </div>

          <!-- Card Footer -->
          <div class="card-footer">
            <div class="members-info">
              <i class="pi pi-users"></i>
              <span>{{ group.membersCount }} {{ isRTL ? 'عضو' : 'members' }}</span>
            </div>
            <div class="card-actions">
              <Button
                icon="pi pi-users"
                text
                rounded
                size="small"
                @click.stop="viewMembers(group)"
                :title="isRTL ? 'عرض الأعضاء' : 'View Members'"
              />
              <Button
                icon="pi pi-pencil"
                text
                rounded
                size="small"
                @click.stop="editGroup(group)"
                :title="isRTL ? 'تعديل' : 'Edit'"
              />
              <Button
                icon="pi pi-ellipsis-v"
                text
                rounded
                size="small"
                @click.stop="(e: Event) => toggleActionMenu(e, group)"
                :title="isRTL ? 'المزيد' : 'More'"
              />
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <EmptyState
        v-if="filteredGroups.length === 0"
        icon="pi-sitemap"
        title="No Groups Found"
        title-arabic="لم يتم العثور على مجموعات"
        description="Try adjusting your search criteria"
        description-arabic="حاول تعديل معايير البحث"
        action-label="Clear Filters"
        action-label-arabic="مسح الفلاتر"
        action-icon="pi-times"
        variant="search"
        @action="clearAllFilters"
      />

      <!-- Pagination -->
      <div v-if="filteredGroups.length > 0 && totalPages > 1" class="pagination-wrapper">
        <div class="pagination-info">
          <span>
            {{ isRTL ? `عرض ${paginationInfo.start} إلى ${paginationInfo.end} من ${paginationInfo.total} مجموعة` : `Showing ${paginationInfo.start} to ${paginationInfo.end} of ${paginationInfo.total} groups` }}
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

    <!-- Group Dialog -->
    <Dialog
      v-model:visible="showGroupDialog"
      :header="currentGroup ? (isRTL ? 'تعديل المجموعة' : 'Edit Group') : (isRTL ? 'إضافة مجموعة' : 'Add Group')"
      :style="{ width: '500px' }"
      :modal="true"
      :dismissableMask="true"
      class="group-dialog"
    >
      <div class="dialog-content">
        <p class="placeholder-text">{{ isRTL ? 'نموذج المجموعة سيظهر هنا' : 'Group form will appear here' }}</p>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showGroupDialog = false" />
        <Button :label="isRTL ? 'حفظ' : 'Save'" @click="showGroupDialog = false" />
      </template>
    </Dialog>

    <!-- View Members Dialog -->
    <Dialog
      v-model:visible="showMembersDialog"
      :header="isRTL ? 'أعضاء المجموعة' : 'Group Members'"
      :style="{ width: '500px' }"
      :modal="true"
      :dismissableMask="true"
      class="members-dialog"
    >
      <div class="dialog-content" v-if="currentGroup">
        <div class="group-header">
          <div class="group-icon" :style="{ backgroundColor: getTypeColor(currentGroup.type) + '20', color: getTypeColor(currentGroup.type) }">
            <i :class="['pi', getTypeIcon(currentGroup.type)]"></i>
          </div>
          <div>
            <h3>{{ getName(currentGroup) }}</h3>
            <Tag :value="getTypeLabel(currentGroup.type)" :severity="getTypeSeverity(currentGroup.type)" />
          </div>
        </div>
        <div class="members-section">
          <h4>{{ isRTL ? 'قائد المجموعة' : 'Group Lead' }}</h4>
          <div class="member-item lead">
            <div class="member-avatar">{{ currentGroup.leadAvatar }}</div>
            <span>{{ getLeadName(currentGroup) }}</span>
          </div>
        </div>
        <div class="members-section">
          <h4>{{ isRTL ? 'الأعضاء' : 'Members' }} ({{ currentGroup.membersCount }})</h4>
          <p class="placeholder-text">{{ isRTL ? 'قائمة الأعضاء ستظهر هنا' : 'Member list will appear here' }}</p>
        </div>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إضافة أعضاء' : 'Add Members'" icon="pi pi-user-plus" @click="showMembersDialog = false; addMembers(currentGroup)" />
        <Button :label="isRTL ? 'إغلاق' : 'Close'" severity="secondary" @click="showMembersDialog = false" />
      </template>
    </Dialog>

    <!-- Add Members Dialog -->
    <Dialog
      v-model:visible="showAddMembersDialog"
      :header="isRTL ? 'إضافة أعضاء' : 'Add Members'"
      :style="{ width: '500px' }"
      :modal="true"
      :dismissableMask="true"
      class="add-members-dialog"
    >
      <div class="dialog-content" v-if="currentGroup">
        <p>{{ isRTL ? `إضافة أعضاء إلى "${getName(currentGroup)}"` : `Add members to "${getName(currentGroup)}"` }}</p>
        <div class="search-members">
          <InputText :placeholder="isRTL ? 'البحث عن مستخدمين...' : 'Search users...'" class="w-full" />
        </div>
        <p class="placeholder-text">{{ isRTL ? 'قائمة المستخدمين المتاحين ستظهر هنا' : 'Available users list will appear here' }}</p>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showAddMembersDialog = false" />
        <Button :label="isRTL ? 'إضافة' : 'Add'" @click="confirmAddMembers" />
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
        <p>{{ isRTL ? 'هل أنت متأكد من حذف هذه المجموعة؟' : 'Are you sure you want to delete this group?' }}</p>
        <p class="group-name" v-if="currentGroup">{{ getName(currentGroup) }}</p>
        <p class="warning-text" v-if="currentGroup && currentGroup.membersCount > 0">
          {{ isRTL ? `تحذير: هذه المجموعة تحتوي على ${currentGroup.membersCount} عضو.` : `Warning: This group has ${currentGroup.membersCount} members.` }}
        </p>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showDeleteDialog = false" />
        <Button :label="isRTL ? 'حذف' : 'Delete'" severity="danger" @click="confirmDeleteGroup" />
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

.groups-view {
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
  .groups-grid {
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
// GROUPS TABLE
// ============================================

.groups-table {
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

  &.group-cell {
    flex: 2;
    min-width: 280px;
  }

  &.type-cell {
    width: 110px;
    flex-shrink: 0;
  }

  &.lead-cell {
    width: 160px;
    flex-shrink: 0;
  }

  &.members-cell {
    width: 90px;
    flex-shrink: 0;
    text-align: center;
  }

  &.status-cell {
    width: 90px;
    flex-shrink: 0;
  }

  &.actions-cell {
    width: 60px;
    flex-shrink: 0;
    text-align: center;
  }
}

.group-info {
  display: flex;
  align-items: center;
  gap: $spacing-3;
}

.group-icon {
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

.group-details {
  display: flex;
  flex-direction: column;
  gap: $spacing-1;
  min-width: 0;
}

.group-name {
  font-weight: $font-weight-semibold;
  color: $slate-900;
}

.group-description {
  font-size: $font-size-sm;
  color: $slate-500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.lead-info {
  display: flex;
  align-items: center;
  gap: $spacing-2;
}

.lead-avatar {
  width: 28px;
  height: 28px;
  border-radius: $radius-full;
  object-fit: cover;
}

.lead-name {
  font-size: $font-size-sm;
  color: $slate-700;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.members-count {
  font-weight: $font-weight-semibold;
  color: $slate-700;
}

.action-menu-btn {
  @include action-menu-btn;
}

.action-menu {
  @include action-menu-popup;
}

// ============================================
// GROUPS GRID
// ============================================

.groups-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: $spacing-4;
}

.group-card {
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

    .card-badges {
      display: flex;
      flex-direction: column;
      gap: $spacing-1;
      align-items: flex-end;

      .type-tag,
      .status-tag {
        font-size: $font-size-xs;
      }
    }
  }

  .card-info {
    margin-bottom: $spacing-4;

    .group-name {
      font-size: $font-size-lg;
      margin-bottom: $spacing-2;
    }

    .group-description {
      white-space: normal;
      line-height: 1.5;
    }
  }

  .card-lead {
    margin-bottom: $spacing-4;
    padding: $spacing-3;
    background: $slate-50;
    border-radius: $radius-lg;

    .lead-label {
      font-size: $font-size-xs;
      font-weight: $font-weight-semibold;
      color: $slate-500;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      display: block;
      margin-bottom: $spacing-2;
    }

    .lead-info {
      .lead-avatar {
        width: 32px;
        height: 32px;
      }

      .lead-name {
        font-weight: $font-weight-medium;
      }
    }
  }

  .card-footer {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding-top: $spacing-4;
    border-top: 1px solid $slate-100;
    margin-top: auto;

    .members-info {
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
// GROUP DIALOG
// ============================================

.group-dialog {
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
    &.lead-cell,
    &.status-cell {
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
      min-width: 0;

      span {
        display: none;
      }
    }
  }

  .groups-grid {
    grid-template-columns: 1fr;
  }

  .table-cell {
    &.type-cell,
    &.members-cell {
      display: none;
    }

    &.group-cell {
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
.groups-table {
  &--animated {
    .table-row {
      opacity: 0;
      transform: translateX(-16px);
    }

    &.groups-table--visible {
      .table-row {
        animation: tableRowFadeIn 0.35s ease-out forwards;
        animation-delay: calc(var(--row-index, 0) * 50ms);
      }
    }
  }

  &:not(.groups-table--animated) {
    .table-row {
      opacity: 1;
      transform: none;
      animation: none;
    }
  }
}

// Grid animation
.groups-grid {
  &--animated {
    .group-card {
      opacity: 0;
      transform: translateY(20px) scale(0.96);
    }

    &.groups-grid--visible {
      .group-card {
        animation: cardFadeIn 0.4s ease-out forwards;
        animation-delay: calc(var(--card-index, 0) * 70ms);
      }
    }
  }

  &:not(.groups-grid--animated) {
    .group-card {
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
.group-card {
  &:hover {
    box-shadow:
      0 12px 28px rgba(0, 0, 0, 0.12),
      0 6px 12px rgba(0, 0, 0, 0.08);
    transform: translateY(-4px) scale(1.01);
  }
}

// RTL adjustments
.rtl {
  .groups-table--animated .table-row {
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
  .groups-table {
    &--animated .table-row {
      opacity: 1;
      transform: none;
      animation: none !important;
    }
  }

  .groups-grid {
    &--animated .group-card {
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

  .group-card {
    transition: background-color 0.15s, box-shadow 0.15s;

    &:hover {
      transform: none;
    }
  }
}
</style>
