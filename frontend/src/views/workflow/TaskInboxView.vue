<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useUIStore } from '@/stores/ui.store'
import { useReducedMotion } from '@/composables/useReducedMotion'
import ErrorState from '@/components/base/ErrorState.vue'
import { PageHeader, StatsBar, LoadingSpinner, Avatar } from '@/components/base'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import type { StatItem } from '@/components/base/StatsBar.vue'
import type { Task, TaskAction } from '@/types/task.types'
import {
  TASK_TYPE_CONFIG,
  TASK_PRIORITY_CONFIG,
  isTaskOverdue,
  isDueToday,
  getRelativeDueDate,
  sortTasksByUrgency,
  getDefaultActionsForTask
} from '@/types/task.types'
import TaskDetailPanel from '@/components/tasks/TaskDetailPanel.vue'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'

const router = useRouter()
const uiStore = useUIStore()
const prefersReducedMotion = useReducedMotion()

const isRTL = computed(() => uiStore.locale === 'ar')
const isLoading = ref(true)
const error = ref<Error | null>(null)
const isContentVisible = ref(false)

// Animation control
const shouldAnimate = computed(() => !prefersReducedMotion.value)

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: isRTL.value ? 'صندوق المهام' : 'Task Inbox' }
])

// Panel state
const showTaskPanel = ref(false)
const selectedTask = ref<Task | null>(null)

// Filters
const searchQuery = ref('')
const selectedStatus = ref<string>('all')
const selectedType = ref<string>('all')
const selectedPriority = ref<string>('all')
const viewMode = ref<'list' | 'grid'>('list')
const showFilters = ref(false)

// Type toggle options (main filter in toolbar)
const typeToggleOptions = computed(() => [
  { value: 'all', label: isRTL.value ? 'الكل' : 'All', icon: 'pi-th-large' },
  { value: 'approval', label: isRTL.value ? 'موافقة' : 'Approval', icon: 'pi-check-circle' },
  { value: 'review', label: isRTL.value ? 'مراجعة' : 'Review', icon: 'pi-eye' },
  { value: 'assignment', label: isRTL.value ? 'مهمة' : 'Assignment', icon: 'pi-bookmark' }
])

// Status options for filter panel
const statusOptions = computed(() => [
  { label: isRTL.value ? 'جميع الحالات' : 'All Status', value: 'all' },
  { label: isRTL.value ? 'قيد الانتظار' : 'Pending', value: 'pending' },
  { label: isRTL.value ? 'قيد التنفيذ' : 'In Progress', value: 'in_progress' },
  { label: isRTL.value ? 'متأخر' : 'Overdue', value: 'overdue' },
  { label: isRTL.value ? 'مكتمل' : 'Completed', value: 'completed' }
])

// Priority options for filter panel
const priorityOptions = computed(() => [
  { label: isRTL.value ? 'جميع الأولويات' : 'All Priorities', value: 'all' },
  { label: isRTL.value ? 'عاجل' : 'Urgent', value: 'urgent' },
  { label: isRTL.value ? 'مرتفع' : 'High', value: 'high' },
  { label: isRTL.value ? 'متوسط' : 'Medium', value: 'medium' },
  { label: isRTL.value ? 'منخفض' : 'Low', value: 'low' }
])

// Count active filters
const activeFiltersCount = computed(() => {
  let count = 0
  if (selectedType.value !== 'all') count++
  if (selectedStatus.value !== 'all') count++
  if (selectedPriority.value !== 'all') count++
  return count
})

// Sample tasks data (would come from API)
const tasks = ref<Task[]>([
  {
    id: 1,
    title: 'Approve Budget Proposal 2025',
    titleArabic: 'الموافقة على ميزانية 2025',
    description: 'Please review and approve the Q1 2025 budget proposal for the Marketing department. This includes allocation for digital campaigns, events, and content production.',
    type: 'approval',
    status: 'pending',
    priority: 'urgent',
    dueDate: new Date(Date.now() - 24 * 60 * 60 * 1000),
    createdAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000),
    assignee: { id: 1, name: 'Current User', initials: 'CU' },
    requester: { id: 2, name: 'Fatima Khalid', nameArabic: 'فاطمة خالد', initials: 'FK', department: 'Marketing' },
    relatedItem: {
      type: 'document',
      id: 101,
      title: 'Budget Proposal 2025',
      icon: 'pi pi-file',
      url: '/documents/101'
    },
    actions: [],
    commentCount: 3,
    comments: [
      { id: 1, user: { id: 2, name: 'Fatima Khalid', initials: 'FK' }, content: 'This is urgent, we need approval before EOD.', createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000) }
    ]
  },
  {
    id: 2,
    title: 'Review Training Materials',
    titleArabic: 'مراجعة مواد التدريب',
    description: 'Review the updated training materials for new employee onboarding program.',
    type: 'review',
    status: 'pending',
    priority: 'high',
    dueDate: new Date(),
    createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
    assignee: { id: 1, name: 'Current User', initials: 'CU' },
    requester: { id: 3, name: 'Sara Ali', nameArabic: 'سارة علي', initials: 'SA', department: 'HR' },
    relatedItem: {
      type: 'document',
      id: 102,
      title: 'Employee Onboarding Guide v2',
      icon: 'pi pi-book',
      url: '/documents/102'
    },
    actions: [],
    commentCount: 0
  },
  {
    id: 3,
    title: 'Complete Project Status Report',
    titleArabic: 'إكمال تقرير حالة المشروع',
    description: 'Compile and submit the weekly project status report for the AFC 2027 preparation activities.',
    type: 'assignment',
    status: 'in_progress',
    priority: 'medium',
    dueDate: new Date(Date.now() + 24 * 60 * 60 * 1000),
    createdAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
    assignee: { id: 1, name: 'Current User', initials: 'CU' },
    requester: { id: 4, name: 'Mohammed Omar', nameArabic: 'محمد عمر', initials: 'MO', department: 'PMO' },
    progress: 65,
    actions: [],
    commentCount: 1
  },
  {
    id: 4,
    title: 'You were mentioned in AFC 2027 Planning',
    titleArabic: 'تمت الإشارة إليك في تخطيط كأس آسيا 2027',
    description: '@CurrentUser what are your thoughts on the venue selection for the group stage matches?',
    type: 'mention',
    status: 'pending',
    priority: 'low',
    dueDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000),
    createdAt: new Date(Date.now() - 1 * 60 * 60 * 1000),
    assignee: { id: 1, name: 'Current User', initials: 'CU' },
    requester: { id: 5, name: 'Ahmed Hassan', nameArabic: 'أحمد حسن', initials: 'AH', department: 'Operations' },
    relatedItem: {
      type: 'community',
      id: 5,
      title: 'AFC 2027 Planning Committee',
      icon: 'pi pi-users',
      url: '/communities/5'
    },
    actions: [],
    commentCount: 5
  },
  {
    id: 5,
    title: 'Approve Vendor Contract',
    titleArabic: 'الموافقة على عقد المورد',
    description: 'Review and approve the contract for stadium equipment vendor.',
    type: 'approval',
    status: 'pending',
    priority: 'high',
    dueDate: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000),
    createdAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
    assignee: { id: 1, name: 'Current User', initials: 'CU' },
    requester: { id: 6, name: 'Omar Ahmed', nameArabic: 'عمر أحمد', initials: 'OA', department: 'Procurement' },
    relatedItem: {
      type: 'document',
      id: 103,
      title: 'Stadium Equipment Contract',
      icon: 'pi pi-file',
      url: '/documents/103'
    },
    actions: [],
    commentCount: 2
  },
  {
    id: 6,
    title: 'Weekly Meeting Reminder',
    titleArabic: 'تذكير بالاجتماع الأسبوعي',
    description: 'Weekly project sync meeting scheduled for tomorrow at 10:00 AM.',
    type: 'reminder',
    status: 'pending',
    priority: 'medium',
    dueDate: new Date(Date.now() + 1 * 24 * 60 * 60 * 1000),
    createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000),
    assignee: { id: 1, name: 'Current User', initials: 'CU' },
    requester: { id: 7, name: 'System', initials: 'SY' },
    relatedItem: {
      type: 'event',
      id: 201,
      title: 'Weekly Project Sync',
      icon: 'pi pi-calendar',
      url: '/calendar/events/201'
    },
    actions: [],
    commentCount: 0
  },
  {
    id: 7,
    title: 'Review Marketing Strategy Document',
    titleArabic: 'مراجعة وثيقة استراتيجية التسويق',
    description: 'Review the Q2 marketing strategy document and provide feedback.',
    type: 'review',
    status: 'completed',
    priority: 'medium',
    dueDate: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
    createdAt: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000),
    completedAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
    assignee: { id: 1, name: 'Current User', initials: 'CU' },
    requester: { id: 2, name: 'Fatima Khalid', nameArabic: 'فاطمة خالد', initials: 'FK', department: 'Marketing' },
    relatedItem: {
      type: 'document',
      id: 104,
      title: 'Q2 Marketing Strategy',
      icon: 'pi pi-file',
      url: '/documents/104'
    },
    actions: [],
    commentCount: 4
  }
])

// Load tasks data
const LOADING_DELAY = 600

async function loadTasks() {
  try {
    error.value = null
    isLoading.value = true

    await new Promise(resolve => setTimeout(resolve, LOADING_DELAY))

    tasks.value.forEach(task => {
      task.actions = getDefaultActionsForTask(task)
    })

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
    console.error('Failed to load tasks:', e)
  }
}

async function handleRetry() {
  await loadTasks()
}

onMounted(() => {
  loadTasks()
})

// Computed stats
const stats = computed(() => {
  const allTasks = tasks.value
  return {
    total: allTasks.length,
    pending: allTasks.filter(t => t.status === 'pending' || t.status === 'in_progress').length,
    overdue: allTasks.filter(t => isTaskOverdue(t)).length,
    dueToday: allTasks.filter(t => isDueToday(t)).length,
    completed: allTasks.filter(t => t.status === 'completed').length
  }
})

// StatsBar items
const statsBarItems = computed<StatItem[]>(() => [
  {
    icon: 'pi pi-inbox',
    value: stats.value.pending,
    label: 'Pending',
    labelArabic: 'قيد الانتظار',
    colorClass: 'primary'
  },
  {
    icon: 'pi pi-exclamation-triangle',
    value: stats.value.overdue,
    label: 'Overdue',
    labelArabic: 'متأخر',
    colorClass: 'error'
  },
  {
    icon: 'pi pi-calendar',
    value: stats.value.dueToday,
    label: 'Due Today',
    labelArabic: 'مستحق اليوم',
    colorClass: 'warning'
  },
  {
    icon: 'pi pi-check-circle',
    value: stats.value.completed,
    label: 'Completed',
    labelArabic: 'مكتمل',
    colorClass: 'success'
  }
])

// Filtered and sorted tasks
const filteredTasks = computed(() => {
  let result = [...tasks.value]

  // Filter by search query
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(t =>
      t.title.toLowerCase().includes(query) ||
      t.requester.name.toLowerCase().includes(query) ||
      t.description?.toLowerCase().includes(query)
    )
  }

  // Filter by status
  if (selectedStatus.value !== 'all') {
    if (selectedStatus.value === 'overdue') {
      result = result.filter(t => isTaskOverdue(t))
    } else {
      result = result.filter(t => t.status === selectedStatus.value)
    }
  }

  // Filter by type
  if (selectedType.value !== 'all') {
    result = result.filter(t => t.type === selectedType.value)
  }

  // Filter by priority
  if (selectedPriority.value !== 'all') {
    result = result.filter(t => t.priority === selectedPriority.value)
  }

  // Sort by urgency
  return sortTasksByUrgency(result)
})

// Task handlers
function openTaskPanel(task: Task) {
  selectedTask.value = task
  showTaskPanel.value = true
}

function closeTaskPanel() {
  showTaskPanel.value = false
  selectedTask.value = null
}

function handleQuickAction(action: TaskAction, task: Task, event: Event) {
  event.stopPropagation()
  handleTaskAction(action, task)
}

function handleTaskAction(action: TaskAction, task: Task) {
  switch (action.type) {
    case 'approve':
      task.status = 'completed'
      task.completedAt = new Date()
      break
    case 'reject':
      task.status = 'cancelled'
      break
    case 'complete':
      task.status = 'completed'
      task.completedAt = new Date()
      if (task.progress !== undefined) task.progress = 100
      break
    case 'defer':
      task.status = 'deferred'
      break
    case 'view':
      if (task.relatedItem) {
        router.push(task.relatedItem.url)
      }
      break
  }

  // Update actions after status change
  task.actions = getDefaultActionsForTask(task)
  closeTaskPanel()
}

function clearAllFilters() {
  searchQuery.value = ''
  selectedStatus.value = 'all'
  selectedType.value = 'all'
  selectedPriority.value = 'all'
}
</script>

<template>
  <div class="task-inbox-view" :class="{ rtl: isRTL }">
    <!-- Page Header -->
    <PageHeader
      :title="isRTL ? 'صندوق المهام' : 'Task Inbox'"
      :description="isRTL ? 'إدارة المهام المعلقة والموافقات والتكليفات' : 'Manage your pending tasks, approvals, and assignments'"
      :breadcrumbs="breadcrumbs"
      padding-bottom="xl"
      background-variant="gradient"
      :animated="shouldAnimate"
    />

    <!-- Stats Bar -->
    <StatsBar
      :stats="statsBarItems"
      :loading="isLoading"
      overlap="lg"
      :animated="shouldAnimate"
      :animate-numbers="shouldAnimate"
      :counter-duration="1200"
    />

    <!-- Premium Toolbar -->
    <div class="toolbar">
      <div class="toolbar-row">
        <div class="toolbar-left">
          <!-- Search -->
          <div class="search-box">
            <i class="pi pi-search"></i>
            <InputText
              v-model="searchQuery"
              :placeholder="isRTL ? 'البحث في المهام...' : 'Search tasks...'"
              class="search-input"
            />
          </div>

          <!-- Type Toggle -->
          <div class="type-toggle">
            <button
              v-for="option in typeToggleOptions"
              :key="option.value"
              :class="{ active: selectedType === option.value }"
              @click="selectedType = option.value"
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
          <!-- View Toggle -->
          <div class="view-toggle">
            <button
              :class="{ active: viewMode === 'list' }"
              @click="viewMode = 'list'"
              :title="isRTL ? 'عرض قائمة' : 'List view'"
            >
              <i class="pi pi-list"></i>
            </button>
            <button
              :class="{ active: viewMode === 'grid' }"
              @click="viewMode = 'grid'"
              :title="isRTL ? 'عرض شبكي' : 'Grid view'"
            >
              <i class="pi pi-th-large"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Expanded Filters Panel -->
      <Transition name="filter-panel">
        <div v-if="showFilters" class="filters-panel">
        <div class="filter-group">
          <label class="filter-label">{{ isRTL ? 'الحالة' : 'Status' }}</label>
          <div class="filter-options">
            <button
              v-for="option in statusOptions"
              :key="option.value"
              :class="['filter-chip', { active: selectedStatus === option.value }]"
              @click="selectedStatus = option.value"
            >
              {{ option.label }}
              <span v-if="option.value === 'overdue' && stats.overdue > 0" class="chip-badge">
                {{ stats.overdue }}
              </span>
            </button>
          </div>
        </div>

        <div class="filter-group">
          <label class="filter-label">{{ isRTL ? 'الأولوية' : 'Priority' }}</label>
          <div class="filter-options">
            <button
              v-for="option in priorityOptions"
              :key="option.value"
              :class="['filter-chip', { active: selectedPriority === option.value }]"
              @click="selectedPriority = option.value"
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
      </Transition>

      <!-- Active Filters Tags -->
      <div v-if="(activeFiltersCount > 0 || searchQuery) && !showFilters" class="active-filters">
        <span class="active-filters-label">{{ isRTL ? 'التصفية النشطة:' : 'Active filters:' }}</span>

        <span v-if="searchQuery" class="filter-tag">
          <i class="pi pi-search"></i>
          "{{ searchQuery }}"
          <button @click="searchQuery = ''"><i class="pi pi-times"></i></button>
        </span>

        <span v-if="selectedType !== 'all'" class="filter-tag">
          <i :class="['pi', typeToggleOptions.find(o => o.value === selectedType)?.icon]"></i>
          {{ typeToggleOptions.find(o => o.value === selectedType)?.label }}
          <button @click="selectedType = 'all'"><i class="pi pi-times"></i></button>
        </span>

        <span v-if="selectedStatus !== 'all'" class="filter-tag">
          {{ statusOptions.find(o => o.value === selectedStatus)?.label }}
          <button @click="selectedStatus = 'all'"><i class="pi pi-times"></i></button>
        </span>

        <span v-if="selectedPriority !== 'all'" class="filter-tag">
          {{ priorityOptions.find(o => o.value === selectedPriority)?.label }}
          <button @click="selectedPriority = 'all'"><i class="pi pi-times"></i></button>
        </span>

        <button class="clear-all-link" @click="clearAllFilters">
          {{ isRTL ? 'مسح الكل' : 'Clear all' }}
        </button>
      </div>
    </div>

    <!-- Task List -->
    <div class="content-area">
      <!-- Loading State -->
      <LoadingSpinner
        v-if="isLoading"
        size="lg"
        variant="spinner"
        label="Loading tasks..."
        label-arabic="جاري تحميل المهام..."
        :show-label="true"
        centered
        class="loading-state"
      />

      <!-- Error State -->
      <ErrorState
        v-else-if="error"
        :error="error"
        :title="isRTL ? 'فشل تحميل المهام' : 'Failed to load tasks'"
        show-retry
        @retry="handleRetry"
      />

      <!-- Empty State -->
      <div v-else-if="filteredTasks.length === 0" class="empty-state">
        <div class="empty-icon">
          <i class="pi pi-inbox"></i>
        </div>
        <h3>{{ isRTL ? 'لا توجد مهام' : 'No tasks found' }}</h3>
        <p>{{ activeFiltersCount > 0 || searchQuery ? (isRTL ? 'جرب تعديل الفلاتر' : 'Try adjusting your filters') : (isRTL ? 'لا توجد مهام معلقة!' : 'You\'re all caught up!') }}</p>
        <Button
          v-if="activeFiltersCount > 0 || searchQuery"
          :label="isRTL ? 'مسح الفلاتر' : 'Clear Filters'"
          icon="pi pi-times"
          outlined
          @click="clearAllFilters"
        />
      </div>

      <!-- Task List -->
      <div
        v-else
        class="task-list"
        :class="{
          'grid-view': viewMode === 'grid',
          'task-list--animated': shouldAnimate,
          'task-list--visible': isContentVisible
        }"
      >
        <div
          v-for="(task, index) in filteredTasks"
          :key="task.id"
          class="task-card"
          :class="{
            'is-overdue': isTaskOverdue(task),
            'is-due-today': isDueToday(task),
            'is-completed': task.status === 'completed' || task.status === 'cancelled',
            'priority-urgent': task.priority === 'urgent',
            'priority-high': task.priority === 'high'
          }"
          :style="shouldAnimate ? { '--task-index': index } : undefined"
          @click="openTaskPanel(task)"
        >
          <!-- Task Type Icon -->
          <div
            class="task-type-icon"
            :style="{
              background: TASK_TYPE_CONFIG[task.type].bgColor,
              color: TASK_TYPE_CONFIG[task.type].color
            }"
          >
            <i :class="TASK_TYPE_CONFIG[task.type].icon"></i>
          </div>

          <!-- Task Content -->
          <div class="task-content">
            <div class="task-header">
              <span class="task-title">{{ task.title }}</span>
              <div class="task-badges">
                <span
                  v-if="isTaskOverdue(task)"
                  class="badge badge-overdue"
                >
                  <i class="pi pi-exclamation-triangle"></i>
                  Overdue
                </span>
                <span
                  v-else-if="isDueToday(task)"
                  class="badge badge-today"
                >
                  Due Today
                </span>
                <span
                  v-if="task.status === 'completed'"
                  class="badge badge-completed"
                >
                  <i class="pi pi-check"></i>
                  Completed
                </span>
              </div>
            </div>

            <p v-if="task.description" class="task-description">{{ task.description }}</p>

            <div class="task-meta">
              <!-- Requester -->
              <div class="meta-item requester">
                <Avatar
                  :name="task.requester.name"
                  :image="task.requester.avatar"
                  shape="circle"
                  size="sm"
                  class="requester-avatar"
                />
                <span>{{ task.requester.name }}</span>
                <span v-if="task.requester.department" class="department">{{ task.requester.department }}</span>
              </div>

              <!-- Due Date -->
              <div class="meta-item due-date" :class="{ overdue: isTaskOverdue(task), 'due-today': isDueToday(task) }">
                <i class="pi pi-clock"></i>
                <span>{{ getRelativeDueDate(task) }}</span>
              </div>

              <!-- Priority -->
              <div
                class="meta-item priority"
                :style="{
                  background: TASK_PRIORITY_CONFIG[task.priority].bgColor,
                  color: TASK_PRIORITY_CONFIG[task.priority].color
                }"
              >
                {{ TASK_PRIORITY_CONFIG[task.priority].label }}
              </div>

              <!-- Type Badge -->
              <div
                class="meta-item type-badge"
                :style="{
                  background: TASK_TYPE_CONFIG[task.type].bgColor,
                  color: TASK_TYPE_CONFIG[task.type].color
                }"
              >
                {{ TASK_TYPE_CONFIG[task.type].label }}
              </div>

              <!-- Comments -->
              <div v-if="task.commentCount && task.commentCount > 0" class="meta-item comments">
                <i class="pi pi-comment"></i>
                <span>{{ task.commentCount }}</span>
              </div>
            </div>

            <!-- Progress Bar (for assignments) -->
            <div v-if="task.progress !== undefined && task.type === 'assignment'" class="task-progress">
              <div class="progress-bar">
                <div class="progress-fill" :style="{ width: `${task.progress}%` }"></div>
              </div>
              <span class="progress-text">{{ task.progress }}%</span>
            </div>

            <!-- Related Item -->
            <div v-if="task.relatedItem" class="related-item">
              <i :class="task.relatedItem.icon"></i>
              <span>{{ task.relatedItem.title }}</span>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="task-actions" @click.stop>
            <Button
              v-for="action in task.actions.filter(a => a.primary)"
              :key="action.type"
              :icon="action.icon"
              :severity="action.severity || 'secondary'"
              size="small"
              rounded
              outlined
              :title="action.label"
              @click="handleQuickAction(action, task, $event)"
            />
            <Button
              icon="pi pi-ellipsis-v"
              severity="secondary"
              size="small"
              rounded
              text
              title="More options"
              @click.stop="openTaskPanel(task)"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Task Detail Panel -->
    <TaskDetailPanel
      :task="selectedTask"
      :visible="showTaskPanel"
      @close="closeTaskPanel"
      @action="handleTaskAction"
    />
  </div>
</template>

<style lang="scss" scoped>
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

// ============================================
// TASK INBOX VIEW - MAIN CONTAINER
// Following Universal Spacing Guidelines from _mixins.scss
// ============================================

.task-inbox-view {
  @include page-view;

  &.rtl {
    direction: rtl;
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

// Type Toggle - matching Documents page
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

  .chip-badge {
    margin-inline-start: $spacing-1;
    padding: 2px 6px;
    font-size: 0.625rem;
    font-weight: $font-weight-semibold;
    background: $error-100;
    color: $error-700;
    border-radius: $radius-full;
  }
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

// View toggle using premium mixin
.view-toggle {
  @include premium-view-toggle;
}

// ============================================
// CONTENT AREA
// ============================================

.content-area {
  @include content-area;
}

.loading-state,
.empty-state {
  @include empty-state;
  padding: 4rem; // 64px
}

.loading-state {
  i {
    font-size: 2rem;
    color: $intalio-teal-500;
    margin-bottom: $spacing-4;
  }

  p {
    color: $slate-500;
  }
}

.empty-icon {
  @include empty-state-icon;
}

.empty-state {
  h3 {
    @include empty-state-title;
  }

  p {
    @include empty-state-text;
    margin-bottom: $spacing-4;
  }
}

// ============================================
// TASK LIST
// ============================================

.task-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;

  &.grid-view {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
    gap: $spacing-4;

    @media (max-width: $breakpoint-md) {
      grid-template-columns: 1fr;
    }
  }
}

.task-card {
  display: flex;
  align-items: flex-start;
  gap: $spacing-4;
  padding: $spacing-5;
  background: white;
  border: 1px solid $slate-200;
  border-radius: $radius-xl;
  transition: all $transition-fast;
  cursor: pointer;

  &:hover {
    border-color: $slate-300;
    box-shadow: $shadow-card-hover;
    transform: translateY(-1px);

    .task-actions {
      opacity: 1;
    }
  }

  // States
  &.is-overdue {
    border-color: rgba($error-500, 0.3);
    background: linear-gradient(135deg, $error-50 0%, white 100%);

    .task-type-icon {
      box-shadow: 0 0 0 2px $error-100;
    }
  }

  &.is-due-today {
    border-color: rgba($warning-500, 0.3);
    background: linear-gradient(135deg, $warning-50 0%, white 100%);
  }

  &.is-completed {
    opacity: 0.7;
    background: $slate-50;

    .task-title {
      text-decoration: line-through;
      color: $slate-500;
    }
  }
}

.task-type-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 48px;
  height: 48px;
  border-radius: $radius-xl;
  flex-shrink: 0;

  i {
    font-size: 1.25rem;
  }
}

.task-content {
  flex: 1;
  min-width: 0;
}

.task-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: $spacing-3;
  margin-bottom: $spacing-2;
}

.task-title {
  font-size: $text-base;
  font-weight: $font-weight-semibold;
  color: $slate-900;
  line-height: 1.4;
  transition: color $transition-fast;

  .task-card:hover & {
    color: $intalio-teal-600;
  }
}

.task-badges {
  display: flex;
  gap: $spacing-2;
  flex-shrink: 0;
}

.badge {
  display: inline-flex;
  align-items: center;
  gap: $spacing-1;
  padding: 2px $spacing-2;
  font-size: $text-xs;
  font-weight: $font-weight-semibold;
  border-radius: $radius-full;
  white-space: nowrap;

  i {
    font-size: 0.625rem;
  }

  &.badge-overdue {
    background: $error-100;
    color: $error-700;
  }

  &.badge-today {
    background: $warning-100;
    color: $warning-700;
  }

  &.badge-completed {
    background: $success-100;
    color: $success-700;
  }
}

.task-description {
  font-size: $text-sm;
  color: $slate-600;
  line-height: 1.5;
  margin: 0 0 $spacing-3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.task-meta {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: $spacing-3;
}

.meta-item {
  display: inline-flex;
  align-items: center;
  gap: $spacing-1;
  font-size: $text-xs;
  color: $slate-500;

  &.requester {
    .requester-avatar {
      width: 20px !important;
      height: 20px !important;
      font-size: 0.625rem !important;
      background: $gradient-primary !important;
    }

    span {
      font-weight: $font-weight-medium;
      color: $slate-700;
    }

    .department {
      color: $slate-400;
      font-weight: $font-weight-normal;

      &::before {
        content: '•';
        margin: 0 $spacing-1;
      }
    }
  }

  &.due-date {
    i {
      font-size: 0.75rem;
    }

    &.overdue {
      color: $error-600;
      font-weight: $font-weight-semibold;
    }

    &.due-today {
      color: $warning-600;
      font-weight: $font-weight-semibold;
    }
  }

  &.priority,
  &.type-badge {
    padding: 2px $spacing-2;
    border-radius: $radius-full;
    font-weight: $font-weight-semibold;
  }

  &.comments {
    i {
      font-size: 0.75rem;
    }
  }
}

.task-progress {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  margin-top: $spacing-3;
}

.progress-bar {
  flex: 1;
  height: 6px;
  background: $slate-200;
  border-radius: $radius-full;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: $gradient-primary;
  border-radius: $radius-full;
  transition: width 0.3s ease;
}

.progress-text {
  font-size: $text-xs;
  font-weight: $font-weight-semibold;
  color: $slate-600;
  min-width: 32px;
}

.related-item {
  display: inline-flex;
  align-items: center;
  gap: $spacing-2;
  margin-top: $spacing-3;
  padding: $spacing-2 $spacing-3;
  background: $slate-100;
  border-radius: $radius-lg;
  font-size: $text-xs;
  color: $slate-600;

  i {
    color: $intalio-teal-500;
    font-size: 0.875rem;
  }
}

.task-actions {
  display: flex;
  gap: $spacing-1;
  opacity: 0;
  transition: opacity $transition-fast;
  flex-shrink: 0;

  @media (max-width: $breakpoint-md) {
    opacity: 1;
  }

  :deep(.p-button) {
    width: 36px;
    height: 36px;

    &.p-button-success {
      &:hover {
        background: $success-500;
        border-color: $success-500;
        color: white;
      }
    }

    &.p-button-danger {
      &:hover {
        background: $error-500;
        border-color: $error-500;
        color: white;
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

  .type-toggle {
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;

    &::-webkit-scrollbar {
      display: none;
    }
  }
}

@media (max-width: $breakpoint-md) {
  .task-inbox-view {
    padding: $spacing-4;
  }

  .toolbar-row {
    flex-direction: column;
    align-items: stretch;
    gap: $spacing-3;
  }

  .toolbar-left {
    flex-direction: column;
    align-items: stretch;
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

  .filter-btn {
    width: 100%;
    justify-content: center;
  }

  .filters-panel {
    flex-direction: column;
  }

  .filter-group {
    width: 100%;
  }

  .task-list {
    &.grid-view {
      grid-template-columns: 1fr;
    }
  }

  .task-card {
    flex-direction: column;

    .task-actions {
      width: 100%;
      justify-content: flex-end;
      padding-top: $spacing-3;
      border-top: 1px solid $slate-100;
      margin-top: $spacing-3;
    }
  }
}

// ============================================
// FILTER PANEL TRANSITION
// ============================================
.filter-panel-enter-active {
  animation: filterPanelSlideDown 0.3s ease-out forwards;
}

.filter-panel-leave-active {
  animation: filterPanelSlideUp 0.25s ease-in forwards;
}

@keyframes filterPanelSlideDown {
  from {
    opacity: 0;
    transform: translateY(-12px);
    max-height: 0;
  }
  to {
    opacity: 1;
    transform: translateY(0);
    max-height: 300px;
  }
}

@keyframes filterPanelSlideUp {
  from {
    opacity: 1;
    transform: translateY(0);
    max-height: 300px;
  }
  to {
    opacity: 0;
    transform: translateY(-12px);
    max-height: 0;
  }
}

// ============================================
// TASK LIST STAGGER ANIMATION
// ============================================
@keyframes taskCardFadeIn {
  from {
    opacity: 0;
    transform: translateX(-20px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.task-list {
  // Animation state - cards hidden initially
  &--animated {
    .task-card {
      opacity: 0;
      transform: translateX(-20px);
    }

    // When visible, animate cards in with stagger
    &.task-list--visible {
      .task-card {
        animation: taskCardFadeIn 0.4s ease-out forwards;
        animation-delay: calc(var(--task-index, 0) * 60ms);
      }
    }
  }

  // Ensure cards without animation are visible
  &:not(.task-list--animated) {
    .task-card {
      opacity: 1;
      transform: none;
      animation: none;
    }
  }
}

// ============================================
// PRIORITY PULSE EFFECTS
// ============================================
@keyframes urgentPulse {
  0%, 100% {
    box-shadow: 0 0 0 0 rgba($error-500, 0.4);
  }
  50% {
    box-shadow: 0 0 0 6px rgba($error-500, 0);
  }
}

@keyframes highPriorityGlow {
  0%, 100% {
    border-color: rgba($warning-500, 0.3);
  }
  50% {
    border-color: rgba($warning-500, 0.6);
  }
}

.task-card {
  &.priority-urgent:not(.is-completed) {
    animation: urgentPulse 2.5s ease-in-out infinite;

    .task-type-icon {
      position: relative;

      &::after {
        content: '';
        position: absolute;
        inset: -3px;
        border-radius: inherit;
        border: 2px solid $error-500;
        animation: urgentPulse 2s ease-in-out infinite;
      }
    }
  }

  &.priority-high:not(.is-completed):not(.is-overdue) {
    animation: highPriorityGlow 3s ease-in-out infinite;
  }
}

// RTL animation adjustments
.rtl {
  @keyframes taskCardFadeIn {
    from {
      opacity: 0;
      transform: translateX(20px);
    }
    to {
      opacity: 1;
      transform: translateX(0);
    }
  }

  .task-list--animated .task-card {
    transform: translateX(20px);
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================
@media (prefers-reduced-motion: reduce) {
  .task-list {
    &--animated {
      .task-card {
        opacity: 1;
        transform: none;
        animation: none !important;
      }
    }
  }

  .filter-panel-enter-active,
  .filter-panel-leave-active {
    animation: none !important;
  }

  .filters-panel {
    transition: none !important;
  }

  .task-card {
    animation: none !important;
    transition: background-color $transition-fast, box-shadow $transition-fast, border-color $transition-fast;

    &:hover {
      transform: none;
    }

    &.priority-urgent,
    &.priority-high {
      animation: none !important;

      .task-type-icon::after {
        animation: none !important;
        display: none;
      }
    }
  }
}
</style>
