<script setup lang="ts">
/**
 * DashboardView - Redesigned with Design System
 *
 * Main dashboard page using the Design System components, tokens, and mixins.
 * Features: Welcome section, stats, content overview, activity feed, tasks.
 * Enhanced with entrance animations, staggered reveals, and micro-interactions.
 */
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/auth.store'
import { useUIStore } from '@/stores/ui.store'
import { useReducedMotion } from '@/composables/useReducedMotion'
import ErrorState from '@/components/base/ErrorState.vue'

// PrimeVue Components
import Chart from 'primevue/chart'
import Menu from 'primevue/menu'
import Skeleton from 'primevue/skeleton'

// Base Components from Design System
import { PageHeader, StatsBar, Widget, Avatar } from '@/components/base'
import type { StatItem, WidgetTab } from '@/components/base'

// Dashboard Components
import { PrimaryButton, SecondaryButton, KnowledgeCard } from '@/components/dashboard'
import type { KnowledgeCardMeta } from '@/components/dashboard'

// Task Components
import TaskDetailPanel from '@/components/tasks/TaskDetailPanel.vue'
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

const router = useRouter()
const { locale } = useI18n()
const authStore = useAuthStore()
const uiStore = useUIStore()
const prefersReducedMotion = useReducedMotion()

const isRTL = computed(() => locale.value === 'ar')
const isLoading = ref(true)
const error = ref<Error | null>(null)
const isContentVisible = ref(false)

// Animation control
const shouldAnimate = computed(() => !prefersReducedMotion.value)

// ============================================
// CHART CONFIGURATION
// ============================================
const chartPeriod = ref<'6months' | 'year'>('6months')
const chartTabs: WidgetTab[] = [
  { label: '6 Months', value: '6months' },
  { label: 'Year', value: 'year' }
]

// ============================================
// TASK PANEL STATE
// ============================================
const showTaskPanel = ref(false)
const selectedTask = ref<Task | null>(null)
const eventMenu = ref()
const selectedEventId = ref<number | null>(null)

// ============================================
// GREETING & DATE
// ============================================
const greeting = computed(() => {
  const hour = new Date().getHours()
  if (hour < 12) return isRTL.value ? 'صباح الخير' : 'Good morning'
  if (hour < 17) return isRTL.value ? 'مساء الخير' : 'Good afternoon'
  return isRTL.value ? 'مساء الخير' : 'Good evening'
})

const formattedDate = computed(() => {
  return new Date().toLocaleDateString(uiStore.locale === 'ar' ? 'ar-SA' : 'en-US', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
})

// ============================================
// STATS DATA
// ============================================
const stats = computed<StatItem[]>(() => [
  {
    icon: 'pi pi-folder-open',
    value: 1234,
    label: 'Total Documents',
    labelArabic: 'إجمالي المستندات',
    colorClass: 'primary',
    trend: { value: '+12%', direction: 'up' },
    onClick: () => router.push('/documents')
  },
  {
    icon: 'pi pi-file-edit',
    value: 567,
    label: 'Published Articles',
    labelArabic: 'المقالات المنشورة',
    colorClass: 'info',
    trend: { value: '+8%', direction: 'up' },
    onClick: () => router.push('/content')
  },
  {
    icon: 'pi pi-comments',
    value: 45,
    label: 'Active Communities',
    labelArabic: 'المجتمعات النشطة',
    colorClass: 'success',
    trend: { value: '+3%', direction: 'up' },
    onClick: () => router.push('/communities')
  },
  {
    icon: 'pi pi-users',
    value: 892,
    label: 'Team Members',
    labelArabic: 'أعضاء الفريق',
    colorClass: 'warning',
    trend: { value: '+15%', direction: 'up' },
    onClick: () => router.push('/team')
  }
])

// ============================================
// QUICK LINKS
// ============================================
interface QuickLink {
  label: string
  labelArabic: string
  icon: string
  route: string
  variant: 'primary' | 'secondary'
}

const quickLinks = ref<QuickLink[]>([
  { label: 'New Document', labelArabic: 'مستند جديد', icon: 'pi pi-plus', route: '/documents', variant: 'primary' },
  { label: 'Write Article', labelArabic: 'كتابة مقال', icon: 'pi pi-pencil', route: '/content', variant: 'secondary' },
  { label: 'Create Event', labelArabic: 'إنشاء حدث', icon: 'pi pi-calendar-plus', route: '/calendar', variant: 'secondary' },
  { label: 'Start Discussion', labelArabic: 'بدء نقاش', icon: 'pi pi-comments', route: '/communities', variant: 'secondary' }
])

// ============================================
// RECENT CONTENT (for KnowledgeCards)
// ============================================
interface RecentContent {
  id: number
  title: string
  titleArabic?: string
  description: string
  type: 'document' | 'article' | 'note' | 'event'
  meta: KnowledgeCardMeta
  status?: 'draft' | 'published'
}

const recentContent = ref<RecentContent[]>([
  {
    id: 1,
    title: 'AFC 2027 Venue Selection Report',
    description: 'Comprehensive analysis of potential stadium venues for the upcoming tournament.',
    type: 'document',
    meta: { author: 'Ahmed Hassan', date: '2 hours ago', views: 234 },
    status: 'published'
  },
  {
    id: 2,
    title: 'Marketing Strategy Q1 2025',
    description: 'Strategic marketing plan focusing on digital engagement and fan experience.',
    type: 'article',
    meta: { author: 'Sara Ali', date: '5 hours ago', comments: 12 },
    status: 'published'
  },
  {
    id: 3,
    title: 'Team Meeting Notes - December',
    description: 'Summary of key decisions and action items from the monthly team meeting.',
    type: 'note',
    meta: { author: 'Mohammed Omar', date: '1 day ago' },
    status: 'draft'
  }
])

// ============================================
// RECENT ACTIVITY
// ============================================
interface Activity {
  id: number
  user: string
  action: string
  item: string
  time: string
  type: 'document' | 'comment' | 'event' | 'share' | 'publish'
}

const recentActivity = ref<Activity[]>([
  { id: 1, user: 'Ahmed Hassan', action: 'uploaded', item: 'Project Report Q4', time: '2 hours ago', type: 'document' },
  { id: 2, user: 'Sara Ali', action: 'commented on', item: 'Team Meeting Notes', time: '3 hours ago', type: 'comment' },
  { id: 3, user: 'Mohammed Omar', action: 'created', item: 'Training Session Event', time: '5 hours ago', type: 'event' },
  { id: 4, user: 'Fatima Khalid', action: 'shared', item: 'Budget Proposal 2025', time: '1 day ago', type: 'share' },
  { id: 5, user: 'Omar Ahmed', action: 'published', item: 'Company Newsletter', time: '1 day ago', type: 'publish' }
])

function getActivityIcon(type: string): string {
  const icons: Record<string, string> = {
    document: 'pi pi-file',
    comment: 'pi pi-comment',
    event: 'pi pi-calendar',
    share: 'pi pi-share-alt',
    publish: 'pi pi-send'
  }
  return icons[type] || 'pi pi-circle'
}

// ============================================
// TASKS DATA
// ============================================
const myTasks = ref<Task[]>([
  {
    id: 1,
    title: 'Approve Budget Proposal 2025',
    titleArabic: 'الموافقة على ميزانية 2025',
    description: 'Please review and approve the Q1 2025 budget proposal for the Marketing department.',
    type: 'approval',
    status: 'pending',
    priority: 'urgent',
    dueDate: new Date(Date.now() - 24 * 60 * 60 * 1000),
    createdAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000),
    assignee: { id: 1, name: 'Current User', initials: 'CU' },
    requester: { id: 2, name: 'Fatima Khalid', nameArabic: 'فاطمة خالد', initials: 'FK', department: 'Marketing' },
    relatedItem: { type: 'document', id: 101, title: 'Budget Proposal 2025', icon: 'pi pi-file', url: '/documents/101' },
    actions: [],
    commentCount: 3
  },
  {
    id: 2,
    title: 'Review Training Materials',
    titleArabic: 'مراجعة مواد التدريب',
    description: 'Review the updated training materials for new employee onboarding.',
    type: 'review',
    status: 'pending',
    priority: 'high',
    dueDate: new Date(),
    createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
    assignee: { id: 1, name: 'Current User', initials: 'CU' },
    requester: { id: 3, name: 'Sara Ali', nameArabic: 'سارة علي', initials: 'SA', department: 'HR' },
    relatedItem: { type: 'document', id: 102, title: 'Employee Onboarding Guide v2', icon: 'pi pi-book', url: '/documents/102' },
    actions: [],
    commentCount: 0
  },
  {
    id: 3,
    title: 'Complete Project Status Report',
    titleArabic: 'إكمال تقرير حالة المشروع',
    description: 'Compile and submit the weekly project status report.',
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
  }
])

// Initialize task actions
myTasks.value.forEach(task => {
  task.actions = getDefaultActionsForTask(task)
})

const sortedTasks = computed(() => sortTasksByUrgency(myTasks.value).slice(0, 4))
const pendingTasksCount = computed(() => myTasks.value.filter(t => t.status !== 'completed' && t.status !== 'cancelled').length)
const overdueTasksCount = computed(() => myTasks.value.filter(t => isTaskOverdue(t)).length)

// ============================================
// EVENTS
// ============================================
interface CalendarEvent {
  id: number
  title: string
  date: string
  time: string
  color: string
}

const upcomingEvents = ref<CalendarEvent[]>([
  { id: 1, title: 'Team Standup', date: '2024-12-10', time: '09:00 AM', color: '#00ae8d' },
  { id: 2, title: 'Project Review', date: '2024-12-11', time: '02:00 PM', color: '#3b82f6' },
  { id: 3, title: 'Training Workshop', date: '2024-12-12', time: '10:00 AM', color: '#8b5cf6' }
])

const eventMenuItems = ref([
  { label: 'View Details', icon: 'pi pi-eye', command: () => router.push(`/calendar/event/${selectedEventId.value}`) },
  { label: 'Edit Event', icon: 'pi pi-pencil', command: () => router.push(`/calendar/event/${selectedEventId.value}/edit`) },
  { separator: true },
  { label: 'Delete', icon: 'pi pi-trash', class: 'text-red-500', command: () => console.log('Delete:', selectedEventId.value) }
])

// ============================================
// CHART DATA
// ============================================
const chartData = computed(() => {
  const months = chartPeriod.value === '6months'
    ? ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun']
    : ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']

  const docsData = chartPeriod.value === '6months'
    ? [65, 59, 80, 81, 56, 72]
    : [45, 52, 65, 59, 80, 81, 56, 72, 68, 75, 82, 90]

  const articlesData = chartPeriod.value === '6months'
    ? [28, 48, 40, 35, 56, 45]
    : [20, 25, 28, 48, 40, 35, 56, 45, 50, 55, 60, 65]

  return {
    labels: months,
    datasets: [
      {
        label: 'Documents',
        data: docsData,
        borderColor: '#00ae8d',
        backgroundColor: 'rgba(0, 174, 141, 0.1)',
        fill: true,
        tension: 0.4,
        pointBackgroundColor: '#00ae8d',
        pointBorderColor: '#fff',
        pointBorderWidth: 2,
        pointRadius: 4
      },
      {
        label: 'Articles',
        data: articlesData,
        borderColor: '#3b82f6',
        backgroundColor: 'rgba(59, 130, 246, 0.1)',
        fill: true,
        tension: 0.4,
        pointBackgroundColor: '#3b82f6',
        pointBorderColor: '#fff',
        pointBorderWidth: 2,
        pointRadius: 4
      }
    ]
  }
})

const chartOptions = ref({
  responsive: true,
  maintainAspectRatio: false,
  interaction: { intersect: false, mode: 'index' },
  plugins: {
    legend: {
      display: true,
      position: 'bottom',
      labels: { usePointStyle: true, padding: 20, font: { size: 12, family: 'Inter' } }
    },
    tooltip: {
      backgroundColor: '#1e293b',
      titleFont: { size: 13, family: 'Inter' },
      bodyFont: { size: 12, family: 'Inter' },
      padding: 12,
      cornerRadius: 8
    }
  },
  scales: {
    x: { grid: { display: false }, ticks: { font: { size: 11, family: 'Inter' }, color: '#64748b' } },
    y: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { font: { size: 11, family: 'Inter' }, color: '#64748b' } }
  }
})

// ============================================
// ACTIONS
// ============================================
function navigateTo(route: string) {
  router.push(route)
}

function viewAllActivity() {
  router.push('/notifications')
}

function viewAllTasks() {
  router.push('/workflow/tasks')
}

function showEventMenu(event: Event, eventId: number) {
  selectedEventId.value = eventId
  eventMenu.value.toggle(event)
}

function openTaskPanel(task: Task) {
  selectedTask.value = task
  showTaskPanel.value = true
}

function closeTaskPanel() {
  showTaskPanel.value = false
  selectedTask.value = null
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
      task.progress = 100
      break
    case 'view':
      if (task.relatedItem) router.push(task.relatedItem.url)
      break
  }
  closeTaskPanel()
}

function handleContentClick(content: RecentContent) {
  router.push(`/documents/${content.id}`)
}

// ============================================
// ERROR HANDLING
// ============================================
async function handleRetry() {
  error.value = null
  isLoading.value = true
  await new Promise(resolve => setTimeout(resolve, 600))
  isLoading.value = false
  if (shouldAnimate.value) {
    requestAnimationFrame(() => {
      isContentVisible.value = true
    })
  } else {
    isContentVisible.value = true
  }
}

// ============================================
// LIFECYCLE
// ============================================
onMounted(async () => {
  await new Promise(resolve => setTimeout(resolve, 600))
  isLoading.value = false

  // Trigger entrance animations after content loads
  if (shouldAnimate.value) {
    requestAnimationFrame(() => {
      isContentVisible.value = true
    })
  } else {
    isContentVisible.value = true
  }
})
</script>

<template>
  <div class="dashboard" :class="{ 'rtl': isRTL }">
    <!-- ============================================
         PAGE HEADER with Welcome
         ============================================ -->
    <PageHeader
      :title="`${greeting}, ${authStore.userDisplayName}`"
      :description="formattedDate"
      padding-bottom="xl"
      background-variant="vivid"
      :animated="shouldAnimate"
    >
      <template #actions>
        <SecondaryButton
          v-for="link in quickLinks.slice(1)"
          :key="link.label"
          :icon="link.icon"
          @click="navigateTo(link.route)"
        >
          {{ isRTL ? link.labelArabic : link.label }}
        </SecondaryButton>
        <PrimaryButton
          :icon="quickLinks[0].icon"
          @click="navigateTo(quickLinks[0].route)"
        >
          {{ isRTL ? quickLinks[0].labelArabic : quickLinks[0].label }}
        </PrimaryButton>
      </template>
    </PageHeader>

    <!-- ============================================
         STATS BAR
         ============================================ -->
    <StatsBar
      :stats="stats"
      :loading="isLoading"
      overlap="lg"
      :animated="shouldAnimate"
      :animate-numbers="shouldAnimate"
      :counter-duration="1200"
    />

    <!-- ============================================
         MAIN CONTENT
         ============================================ -->
    <div class="dashboard__content">
      <!-- Loading State -->
      <div v-if="isLoading" class="dashboard__loading">
        <Skeleton height="300px" borderRadius="16px" />
        <Skeleton height="300px" borderRadius="16px" />
        <Skeleton height="200px" borderRadius="16px" />
        <Skeleton height="200px" borderRadius="16px" />
      </div>

      <!-- Error State -->
      <ErrorState
        v-else-if="error"
        :error="error"
        :title="isRTL ? 'فشل تحميل لوحة المعلومات' : 'Failed to load dashboard'"
        show-retry
        @retry="handleRetry"
      />

      <!-- Main Grid -->
      <div
        v-else
        class="dashboard__grid"
        :class="{
          'dashboard__grid--animated': shouldAnimate,
          'dashboard__grid--visible': isContentVisible
        }"
      >
        <!-- Chart Widget -->
        <Widget
          :title="isRTL ? 'نظرة عامة على المحتوى' : 'Content Overview'"
          icon="pi pi-chart-line"
          :tabs="chartTabs"
          v-model:active-tab="chartPeriod"
          :span="8"
        >
          <div class="chart-wrapper">
            <Chart type="line" :data="chartData" :options="chartOptions" />
          </div>
        </Widget>

        <!-- Recent Content Widget -->
        <Widget
          :title="isRTL ? 'المحتوى الأخير' : 'Recent Content'"
          icon="pi pi-folder"
          :span="4"
        >
          <template #action>
            <SecondaryButton size="sm" ghost @click="navigateTo('/documents')">
              {{ isRTL ? 'عرض الكل' : 'View All' }}
            </SecondaryButton>
          </template>

          <div class="content-list" :class="{ 'content-list--animated': shouldAnimate && isContentVisible }">
            <KnowledgeCard
              v-for="(content, index) in recentContent"
              :key="content.id"
              :style="shouldAnimate ? { '--content-index': index } : undefined"
              :title="content.title"
              :description="content.description"
              :type="content.type"
              :meta="content.meta"
              :status="content.status"
              compact
              @click="handleContentClick(content)"
            />
          </div>
        </Widget>

        <!-- Tasks Widget -->
        <Widget
          :title="isRTL ? 'مهامي' : 'My Tasks'"
          icon="pi pi-check-square"
          :span="6"
        >
          <template #title-extra>
            <span v-if="overdueTasksCount > 0" class="overdue-badge">
              {{ overdueTasksCount }} {{ isRTL ? 'متأخرة' : 'overdue' }}
            </span>
          </template>

          <template #action>
            <span class="pending-count">{{ pendingTasksCount }} {{ isRTL ? 'معلقة' : 'pending' }}</span>
            <SecondaryButton size="sm" ghost @click="viewAllTasks">
              {{ isRTL ? 'عرض الكل' : 'View All' }}
            </SecondaryButton>
          </template>

          <!-- Empty State -->
          <div v-if="sortedTasks.length === 0" class="tasks-empty">
            <i class="pi pi-check-circle"></i>
            <p>{{ isRTL ? 'لا توجد مهام معلقة!' : 'All caught up! No pending tasks.' }}</p>
          </div>

          <!-- Task List -->
          <div v-else class="tasks-list" :class="{ 'tasks-list--animated': shouldAnimate && isContentVisible }">
            <div
              v-for="(task, index) in sortedTasks"
              :key="task.id"
              class="task-item"
              :class="{
                'is-overdue': isTaskOverdue(task),
                'is-due-today': isDueToday(task)
              }"
              :style="shouldAnimate ? { '--task-index': index } : undefined"
              @click="openTaskPanel(task)"
            >
              <div
                class="task-icon"
                :style="{
                  background: TASK_TYPE_CONFIG[task.type].bgColor,
                  color: TASK_TYPE_CONFIG[task.type].color
                }"
              >
                <i :class="TASK_TYPE_CONFIG[task.type].icon"></i>
              </div>

              <div class="task-content">
                <div class="task-header">
                  <span class="task-title">{{ task.title }}</span>
                  <span v-if="isTaskOverdue(task)" class="badge badge--error">
                    <i class="pi pi-exclamation-triangle"></i>
                    {{ isRTL ? 'متأخرة' : 'Overdue' }}
                  </span>
                  <span v-else-if="isDueToday(task)" class="badge badge--warning">
                    {{ isRTL ? 'اليوم' : 'Due Today' }}
                  </span>
                </div>

                <div class="task-meta">
                  <Avatar
                    :name="task.requester.name"
                    shape="circle"
                    size="sm"
                    class="requester-avatar"
                  />
                  <span class="requester-name">{{ task.requester.name }}</span>
                  <span class="task-due">
                    <i class="pi pi-clock"></i>
                    {{ getRelativeDueDate(task) }}
                  </span>
                  <span
                    class="priority-badge"
                    :style="{
                      background: TASK_PRIORITY_CONFIG[task.priority].bgColor,
                      color: TASK_PRIORITY_CONFIG[task.priority].color
                    }"
                  >
                    {{ TASK_PRIORITY_CONFIG[task.priority].label }}
                  </span>
                </div>

                <div v-if="task.progress !== undefined" class="task-progress">
                  <div class="progress-bar">
                    <div class="progress-fill" :style="{ width: `${task.progress}%` }"></div>
                  </div>
                  <span class="progress-text">{{ task.progress }}%</span>
                </div>
              </div>

              <i class="pi pi-chevron-right task-chevron"></i>
            </div>
          </div>
        </Widget>

        <!-- Activity Widget -->
        <Widget
          :title="isRTL ? 'النشاط الأخير' : 'Recent Activity'"
          icon="pi pi-history"
          :span="3"
        >
          <template #action>
            <SecondaryButton size="sm" ghost @click="viewAllActivity">
              {{ isRTL ? 'عرض الكل' : 'View All' }}
            </SecondaryButton>
          </template>

          <div class="activity-list" :class="{ 'activity-list--animated': shouldAnimate && isContentVisible }">
            <div
              v-for="(activity, index) in recentActivity"
              :key="activity.id"
              class="activity-item"
              :style="shouldAnimate ? { '--item-index': index } : undefined"
            >
              <div class="activity-icon">
                <i :class="getActivityIcon(activity.type)"></i>
              </div>
              <div class="activity-content">
                <p>
                  <strong>{{ activity.user }}</strong>
                  {{ activity.action }}
                  <a href="#" @click.prevent>{{ activity.item }}</a>
                </p>
                <span class="activity-time">{{ activity.time }}</span>
              </div>
            </div>
          </div>
        </Widget>

        <!-- Events Widget -->
        <Widget
          :title="isRTL ? 'الأحداث القادمة' : 'Upcoming Events'"
          icon="pi pi-calendar"
          :span="3"
        >
          <template #action>
            <PrimaryButton size="sm" icon="pi pi-plus" @click="navigateTo('/calendar')">
              {{ isRTL ? 'جديد' : 'New' }}
            </PrimaryButton>
          </template>

          <Menu ref="eventMenu" :model="eventMenuItems" :popup="true" />

          <div class="events-list" :class="{ 'events-list--animated': shouldAnimate && isContentVisible }">
            <div
              v-for="(event, index) in upcomingEvents"
              :key="event.id"
              class="event-item"
              :style="shouldAnimate ? { '--event-index': index } : undefined"
            >
              <div class="event-indicator" :style="{ backgroundColor: event.color }"></div>
              <div class="event-date">
                <span class="day">{{ new Date(event.date).getDate() }}</span>
                <span class="month">{{ new Date(event.date).toLocaleDateString('en', { month: 'short' }) }}</span>
              </div>
              <div class="event-details">
                <span class="event-title">{{ event.title }}</span>
                <span class="event-time">
                  <i class="pi pi-clock"></i>
                  {{ event.time }}
                </span>
              </div>
              <button class="event-menu-btn" @click="showEventMenu($event, event.id)">
                <i class="pi pi-ellipsis-v"></i>
              </button>
            </div>
          </div>
        </Widget>

        <!-- Team Widget -->
        <Widget
          :title="isRTL ? 'الفريق متصل' : 'Team Online'"
          icon="pi pi-users"
          :span="2"
        >
          <template #action>
            <SecondaryButton size="sm" ghost @click="navigateTo('/team')">
              {{ isRTL ? 'عرض الكل' : 'View All' }}
            </SecondaryButton>
          </template>

          <div class="team-grid">
            <div
              v-for="i in 8"
              :key="i"
              class="team-member"
              @click="navigateTo('/team')"
            >
              <Avatar
                :name="['Ahmed', 'Sara', 'Mohammed', 'Fatima', 'Omar', 'Khalid', 'Layla', 'Noor'][i - 1]"
                shape="circle"
                size="sm"
                class="member-avatar"
              />
              <span class="online-dot"></span>
            </div>
            <div class="team-member team-member--more" @click="navigateTo('/team')">
              <span>+24</span>
            </div>
          </div>
        </Widget>
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

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

// ============================================
// DASHBOARD CONTAINER
// ============================================
.dashboard {
  min-height: 100vh;
  background: $color-bg-secondary;

  &.rtl {
    direction: rtl;
  }

  // Content area
  &__content {
    padding: 0 $spacing-6 $spacing-6;

    @media (max-width: $breakpoint-md) {
      padding: 0 $spacing-4 $spacing-4;
    }
  }

  // Loading state
  &__loading {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: $spacing-6;

    @media (max-width: $breakpoint-md) {
      grid-template-columns: 1fr;
    }
  }

  // Main grid
  &__grid {
    display: grid;
    grid-template-columns: repeat(12, 1fr);
    gap: $spacing-6;

    @media (max-width: $breakpoint-lg) {
      grid-template-columns: repeat(6, 1fr);
    }

    @media (max-width: $breakpoint-md) {
      grid-template-columns: 1fr;
    }

    // Animated state - widgets hidden initially
    &--animated {
      > :deep(.widget) {
        opacity: 0;
        transform: translateY(20px);
      }
    }

    // When visible, animate widgets in with stagger
    &--animated.dashboard__grid--visible {
      > :deep(.widget) {
        animation: widgetFadeInUp 0.5s ease-out forwards;

        @for $i from 1 through 6 {
          &:nth-child(#{$i}) {
            animation-delay: #{($i - 1) * 100 + 100}ms;
          }
        }
      }
    }
  }
}

// ============================================
// CHART WRAPPER
// ============================================
.chart-wrapper {
  height: 280px;
}

// ============================================
// CONTENT LIST
// ============================================
.content-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;

  // Animated state
  &--animated {
    :deep(.knowledge-card) {
      animation: listItemFadeIn 0.4s ease-out forwards;
      animation-delay: calc(var(--content-index, 0) * 75ms + 300ms);
      opacity: 0;
      transform: translateX(-10px);
    }
  }
}

// ============================================
// TASKS STYLES
// ============================================
.overdue-badge {
  display: inline-flex;
  align-items: center;
  padding: $spacing-1 $spacing-2;
  background: $color-error-light;
  color: $color-error-dark;
  font-size: $font-size-xs;
  font-weight: $font-weight-semibold;
  border-radius: $radius-full;
}

.pending-count {
  font-size: $font-size-xs;
  color: $color-text-muted;
  font-weight: $font-weight-medium;
  margin-inline-end: $spacing-2;
}

.tasks-empty {
  @include flex-column-center;
  padding: $spacing-8;
  text-align: center;

  i {
    font-size: 2.5rem;
    color: $color-success;
    margin-bottom: $spacing-3;
  }

  p {
    color: $color-text-muted;
    font-size: $font-size-sm;
    margin: 0;
  }
}

.tasks-list {
  @include stack($spacing-3);

  // Animated state
  &--animated {
    .task-item {
      animation: taskItemFadeIn 0.4s ease-out forwards;
      animation-delay: calc(var(--task-index, 0) * 100ms + 400ms);
      opacity: 0;
      transform: translateY(10px);
    }
  }
}

.task-item {
  display: flex;
  align-items: flex-start;
  gap: $spacing-3;
  padding: $spacing-4;
  background: $color-bg-tertiary;
  border-radius: $radius-xl;
  border: 1px solid transparent;
  cursor: pointer;
  transition:
    background-color $duration-fast $ease-default,
    border-color $duration-fast $ease-default,
    transform $duration-fast $ease-default;

  &:hover {
    background: $color-border-light;
    border-color: $color-border-default;
    transform: translateY(-1px);

    .task-title {
      color: $color-brand-primary;
    }

    .task-chevron {
      opacity: 1;
      transform: translateX(0);
    }
  }

  &.is-overdue {
    background: $color-error-light;
    border-color: rgba($color-error, 0.3);
  }

  &.is-due-today {
    background: $color-warning-light;
    border-color: rgba($color-warning, 0.3);
  }
}

.task-icon {
  @include flex-center;
  width: 40px;
  height: 40px;
  border-radius: $radius-lg;
  flex-shrink: 0;

  i {
    font-size: 1rem;
  }
}

.task-content {
  flex: 1;
  min-width: 0;
}

.task-header {
  display: flex;
  align-items: flex-start;
  gap: $spacing-2;
  margin-bottom: $spacing-2;
}

.task-title {
  flex: 1;
  font-size: $font-size-sm;
  font-weight: $font-weight-semibold;
  color: $color-text-primary;
  line-height: 1.4;
  transition: color $duration-fast $ease-default;
}

.badge {
  display: inline-flex;
  align-items: center;
  gap: $spacing-1;
  padding: 2px $spacing-2;
  font-size: $font-size-xs;
  font-weight: $font-weight-semibold;
  border-radius: $radius-full;
  white-space: nowrap;

  i {
    font-size: 0.625rem;
  }

  &--error {
    background: rgba($color-error, 0.15);
    color: $color-error-dark;
  }

  &--warning {
    background: rgba($color-warning, 0.15);
    color: $color-warning-dark;
  }
}

.task-meta {
  @include cluster($spacing-3);
}

.requester-avatar {
  width: 20px !important;
  height: 20px !important;
  font-size: $font-size-xs !important;
  background: $gradient-brand-primary !important;
}

.requester-name {
  font-size: $font-size-xs;
  color: $color-text-secondary;
  font-weight: $font-weight-medium;
}

.task-due {
  @include flex-center;
  gap: $spacing-1;
  font-size: $font-size-xs;
  color: $color-text-muted;

  i {
    font-size: 0.75rem;
  }

  .is-overdue & {
    color: $color-error-dark;
    font-weight: $font-weight-medium;
  }

  .is-due-today & {
    color: $color-warning-dark;
    font-weight: $font-weight-medium;
  }
}

.priority-badge {
  display: inline-flex;
  padding: 2px $spacing-2;
  font-size: $font-size-xs;
  font-weight: $font-weight-semibold;
  border-radius: $radius-full;
  text-transform: capitalize;
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
  background: $color-border-default;
  border-radius: $radius-full;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: $gradient-brand-primary;
  border-radius: $radius-full;
  transition: width $duration-slow $ease-out-expo;
}

.progress-text {
  font-size: $font-size-xs;
  font-weight: $font-weight-semibold;
  color: $color-text-secondary;
  min-width: 32px;
}

.task-chevron {
  color: $color-text-muted;
  font-size: 0.75rem;
  opacity: 0;
  transform: translateX(-4px);
  transition:
    opacity $duration-fast $ease-default,
    transform $duration-fast $ease-default;
  flex-shrink: 0;
  margin-top: $spacing-3;
}

// ============================================
// ACTIVITY STYLES
// ============================================
.activity-list {
  @include stack($spacing-4);

  // Animated state
  &--animated {
    .activity-item {
      animation: activityItemFadeIn 0.35s ease-out forwards;
      animation-delay: calc(var(--item-index, 0) * 75ms + 500ms);
      opacity: 0;
      transform: translateX(-8px);
    }
  }
}

.activity-item {
  display: flex;
  gap: $spacing-3;
}

.activity-icon {
  @include flex-center;
  width: 36px;
  height: 36px;
  background: $color-bg-tertiary;
  border-radius: $radius-lg;
  color: $color-text-muted;
  flex-shrink: 0;

  i {
    font-size: 0.875rem;
  }
}

.activity-content {
  flex: 1;
  min-width: 0;

  p {
    font-size: $font-size-sm;
    color: $color-text-secondary;
    margin: 0;
    line-height: 1.5;

    strong {
      color: $color-text-primary;
    }

    a {
      color: $color-brand-primary;
      text-decoration: none;

      &:hover {
        text-decoration: underline;
      }
    }
  }
}

.activity-time {
  font-size: $font-size-xs;
  color: $color-text-muted;
}

// ============================================
// EVENTS STYLES
// ============================================
.events-list {
  @include stack($spacing-3);

  // Animated state
  &--animated {
    .event-item {
      animation: eventItemFadeIn 0.4s ease-out forwards;
      animation-delay: calc(var(--event-index, 0) * 100ms + 500ms);
      opacity: 0;
      transform: scale(0.95);
    }
  }
}

.event-item {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  padding: $spacing-3;
  background: $color-bg-tertiary;
  border-radius: $radius-xl;
  transition: background-color $duration-fast $ease-default;

  &:hover {
    background: $color-border-light;
  }
}

.event-indicator {
  width: 4px;
  height: 40px;
  border-radius: $radius-full;
}

.event-date {
  @include flex-column-center;
  min-width: 44px;

  .day {
    font-size: $font-size-lg;
    font-weight: $font-weight-bold;
    color: $color-text-primary;
    line-height: 1;
  }

  .month {
    font-size: $font-size-xs;
    color: $color-text-muted;
    text-transform: uppercase;
  }
}

.event-details {
  flex: 1;
  min-width: 0;
}

.event-title {
  display: block;
  font-size: $font-size-sm;
  font-weight: $font-weight-medium;
  color: $color-text-primary;
  @include truncate;
}

.event-time {
  @include flex-center;
  gap: $spacing-1;
  font-size: $font-size-xs;
  color: $color-text-muted;

  i {
    font-size: 0.75rem;
  }
}

.event-menu-btn {
  @include flex-center;
  width: 32px;
  height: 32px;
  background: transparent;
  border: none;
  border-radius: $radius-lg;
  color: $color-text-muted;
  cursor: pointer;
  transition:
    background-color $duration-fast $ease-default,
    color $duration-fast $ease-default;

  &:hover {
    background: $color-border-default;
    color: $color-text-secondary;
  }
}

// ============================================
// TEAM STYLES
// ============================================
.team-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: $spacing-3;
}

.team-member {
  position: relative;
  @include flex-center;
  cursor: pointer;
  transition: transform $duration-fast $ease-default;

  &:hover {
    transform: scale(1.1);
  }

  &--more {
    width: 44px;
    height: 44px;
    background: $color-bg-tertiary;
    border-radius: 50%;
    font-size: $font-size-sm;
    font-weight: $font-weight-semibold;
    color: $color-text-secondary;

    &:hover {
      background: $color-bg-brand-subtle;
      color: $color-brand-primary;
    }
  }
}

.member-avatar {
  width: 44px !important;
  height: 44px !important;
  background: $gradient-brand-primary !important;
}

.online-dot {
  position: absolute;
  bottom: 2px;
  inset-inline-end: 2px;
  width: 10px;
  height: 10px;
  background: $color-success;
  border: 2px solid $color-bg-primary;
  border-radius: 50%;
}

// ============================================
// KEYFRAME ANIMATIONS
// ============================================

@keyframes widgetFadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes listItemFadeIn {
  from {
    opacity: 0;
    transform: translateX(-10px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

@keyframes taskItemFadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes activityItemFadeIn {
  from {
    opacity: 0;
    transform: translateX(-8px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

@keyframes eventItemFadeIn {
  from {
    opacity: 0;
    transform: scale(0.95);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

// ============================================
// RTL ANIMATION SUPPORT
// ============================================

.rtl {
  // Reverse X-axis translations for entrance animations
  .content-list--animated :deep(.knowledge-card) {
    transform: translateX(10px);
  }

  .activity-list--animated .activity-item {
    transform: translateX(8px);
  }

  .task-chevron {
    transform: translateX(4px);
  }

  .task-item:hover .task-chevron {
    transform: translateX(0);
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================

@media (prefers-reduced-motion: reduce) {
  .dashboard__grid--animated {
    > :deep(.widget) {
      opacity: 1;
      transform: none;
      animation: none !important;
    }
  }

  .content-list--animated :deep(.knowledge-card),
  .tasks-list--animated .task-item,
  .activity-list--animated .activity-item,
  .events-list--animated .event-item {
    opacity: 1;
    transform: none;
    animation: none !important;
  }

  .task-item,
  .event-item,
  .activity-item {
    transition: background-color $duration-fast $ease-default;
  }
}
</style>
