<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { useReducedMotion } from '@/composables/useReducedMotion'
import { PageHeader, StatsBar, EmptyState, Avatar } from '@/components/base'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import type { StatItem } from '@/components/base/StatsBar.vue'
import ErrorState from '@/components/base/ErrorState.vue'
import Button from 'primevue/button'
import Dropdown from 'primevue/dropdown'
import Checkbox from 'primevue/checkbox'
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'

const { locale } = useI18n()
const router = useRouter()
const prefersReducedMotion = useReducedMotion()

// RTL support
const isRTL = computed(() => locale.value === 'ar')

// Animation control
const shouldAnimate = computed(() => !prefersReducedMotion.value)

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: isRTL.value ? 'الإشعارات' : 'Notifications' }
])

// State
const loading = ref(true)
const error = ref<Error | null>(null)
const isContentVisible = ref(false)
const notifications = ref<Notification[]>([])
const selectedNotifications = ref<string[]>([])
const activeTab = ref<'all' | 'unread' | 'archived'>('all')
const selectedCategory = ref<string | null>(null)
const showDeleteDialog = ref(false)
const deleteTarget = ref<'single' | 'selected' | 'allRead'>('single')
const notificationToDelete = ref<Notification | null>(null)

// Type configurations
const CATEGORY_CONFIG: Record<string, { icon: string; color: string; bgColor: string; label: string; labelAr: string }> = {
  Workflow: { icon: 'pi pi-check-square', color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.1)', label: 'Workflow', labelAr: 'سير العمل' },
  Content: { icon: 'pi pi-file-edit', color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', label: 'Content', labelAr: 'المحتوى' },
  Collaboration: { icon: 'pi pi-users', color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'Collaboration', labelAr: 'التعاون' },
  Calendar: { icon: 'pi pi-calendar', color: '#8b5cf6', bgColor: 'rgba(139, 92, 246, 0.1)', label: 'Calendar', labelAr: 'التقويم' },
  System: { icon: 'pi pi-cog', color: '#6b7280', bgColor: 'rgba(107, 114, 128, 0.1)', label: 'System', labelAr: 'النظام' }
}

const PRIORITY_CONFIG: Record<string, { color: string; bgColor: string; label: string; labelAr: string }> = {
  Urgent: { color: '#ef4444', bgColor: 'rgba(239, 68, 68, 0.1)', label: 'Urgent', labelAr: 'عاجل' },
  High: { color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'High', labelAr: 'عالي' },
  Normal: { color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.1)', label: 'Normal', labelAr: 'عادي' },
  Low: { color: '#6b7280', bgColor: 'rgba(107, 114, 128, 0.1)', label: 'Low', labelAr: 'منخفض' }
}

// Interface
interface Notification {
  id: string
  type: string
  category: string
  priority: string
  title: string
  titleAr: string
  message: string
  messageAr: string
  icon: string
  iconColor: string
  actionUrl?: string
  actorName?: string
  actorAvatar?: string
  isRead: boolean
  isArchived: boolean
  createdAt: Date
}

// Stats
const stats = computed(() => {
  const total = notifications.value.filter(n => !n.isArchived).length
  const unread = notifications.value.filter(n => !n.isRead && !n.isArchived).length
  const today = notifications.value.filter(n => {
    const notifDate = new Date(n.createdAt)
    const todayDate = new Date()
    return notifDate.toDateString() === todayDate.toDateString() && !n.isArchived
  }).length

  return { total, unread, today }
})

const statsBarItems = computed<StatItem[]>(() => [
  {
    icon: 'pi pi-bell',
    value: stats.value.total,
    label: 'Total',
    labelArabic: 'إجمالي',
    colorClass: 'primary'
  },
  {
    icon: 'pi pi-envelope',
    value: stats.value.unread,
    label: 'Unread',
    labelArabic: 'غير مقروء',
    colorClass: 'error'
  },
  {
    icon: 'pi pi-calendar',
    value: stats.value.today,
    label: 'Today',
    labelArabic: 'اليوم',
    colorClass: 'success'
  }
])

// Empty state content based on active tab
const emptyStateDescription = computed(() => {
  if (activeTab.value === 'unread') {
    return 'You have read all your notifications'
  } else if (activeTab.value === 'archived') {
    return 'No archived notifications'
  }
  return "You haven't received any notifications yet"
})

const emptyStateDescriptionArabic = computed(() => {
  if (activeTab.value === 'unread') {
    return 'لقد قرأت جميع إشعاراتك'
  } else if (activeTab.value === 'archived') {
    return 'لا توجد إشعارات مؤرشفة'
  }
  return 'لم تتلق أي إشعارات بعد'
})

// Category options
const categoryOptions = computed(() => [
  { label: isRTL.value ? 'الكل' : 'All Categories', value: null },
  ...Object.entries(CATEGORY_CONFIG).map(([key, config]) => ({
    label: isRTL.value ? config.labelAr : config.label,
    value: key
  }))
])

// Tabs
const tabs = computed(() => [
  { key: 'all', label: isRTL.value ? 'الكل' : 'All', icon: 'pi pi-inbox', count: notifications.value.filter(n => !n.isArchived).length },
  { key: 'unread', label: isRTL.value ? 'غير مقروء' : 'Unread', icon: 'pi pi-envelope', count: notifications.value.filter(n => !n.isRead && !n.isArchived).length },
  { key: 'archived', label: isRTL.value ? 'مؤرشف' : 'Archived', icon: 'pi pi-inbox', count: notifications.value.filter(n => n.isArchived).length }
])

// Filtered notifications
const filteredNotifications = computed(() => {
  let result = [...notifications.value]

  // Tab filter
  if (activeTab.value === 'unread') {
    result = result.filter(n => !n.isRead && !n.isArchived)
  } else if (activeTab.value === 'archived') {
    result = result.filter(n => n.isArchived)
  } else {
    result = result.filter(n => !n.isArchived)
  }

  // Category filter
  if (selectedCategory.value) {
    result = result.filter(n => n.category === selectedCategory.value)
  }

  // Sort by date (newest first)
  result.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())

  return result
})

// Selection computed
const allSelected = computed(() =>
  selectedNotifications.value.length === filteredNotifications.value.length &&
  filteredNotifications.value.length > 0
)

const someSelected = computed(() =>
  selectedNotifications.value.length > 0 &&
  selectedNotifications.value.length < filteredNotifications.value.length
)

// Helper functions
const getCategoryConfig = (category: string) => CATEGORY_CONFIG[category] || CATEGORY_CONFIG.System
const getPriorityConfig = (priority: string) => PRIORITY_CONFIG[priority] || PRIORITY_CONFIG.Normal

const getTitle = (notification: Notification) => isRTL.value ? notification.titleAr : notification.title
const getMessage = (notification: Notification) => isRTL.value ? notification.messageAr : notification.message

const formatRelativeTime = (date: Date) => {
  const now = new Date()
  const diffMs = now.getTime() - new Date(date).getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMins / 60)
  const diffDays = Math.floor(diffHours / 24)

  if (diffMins < 1) {
    return isRTL.value ? 'الآن' : 'Just now'
  } else if (diffMins < 60) {
    return isRTL.value ? `منذ ${diffMins} دقيقة` : `${diffMins}m ago`
  } else if (diffHours < 24) {
    return isRTL.value ? `منذ ${diffHours} ساعة` : `${diffHours}h ago`
  } else if (diffDays < 7) {
    return isRTL.value ? `منذ ${diffDays} يوم` : `${diffDays}d ago`
  } else {
    return new Date(date).toLocaleDateString(isRTL.value ? 'ar-SA' : 'en-US', {
      month: 'short',
      day: 'numeric'
    })
  }
}

// Actions
const toggleSelectAll = () => {
  if (allSelected.value) {
    selectedNotifications.value = []
  } else {
    selectedNotifications.value = filteredNotifications.value.map(n => n.id)
  }
}

const toggleSelect = (id: string) => {
  const index = selectedNotifications.value.indexOf(id)
  if (index > -1) {
    selectedNotifications.value.splice(index, 1)
  } else {
    selectedNotifications.value.push(id)
  }
}

const markAsRead = (notification: Notification) => {
  notification.isRead = true
}

const markAsUnread = (notification: Notification) => {
  notification.isRead = false
}

const markSelectedAsRead = () => {
  notifications.value.forEach(n => {
    if (selectedNotifications.value.includes(n.id)) {
      n.isRead = true
    }
  })
  selectedNotifications.value = []
}

const markAllAsRead = () => {
  notifications.value.forEach(n => {
    if (!n.isArchived) {
      n.isRead = true
    }
  })
}

const archiveNotification = (notification: Notification) => {
  notification.isArchived = true
  notification.isRead = true
}

const unarchiveNotification = (notification: Notification) => {
  notification.isArchived = false
}

const confirmDeleteSingle = (notification: Notification) => {
  deleteTarget.value = 'single'
  notificationToDelete.value = notification
  showDeleteDialog.value = true
}

const confirmDeleteSelected = () => {
  deleteTarget.value = 'selected'
  showDeleteDialog.value = true
}

const confirmDeleteAllRead = () => {
  deleteTarget.value = 'allRead'
  showDeleteDialog.value = true
}

const executeDelete = () => {
  if (deleteTarget.value === 'single' && notificationToDelete.value) {
    notifications.value = notifications.value.filter(n => n.id !== notificationToDelete.value!.id)
  } else if (deleteTarget.value === 'selected') {
    notifications.value = notifications.value.filter(n => !selectedNotifications.value.includes(n.id))
    selectedNotifications.value = []
  } else if (deleteTarget.value === 'allRead') {
    notifications.value = notifications.value.filter(n => !n.isRead || n.isArchived)
  }
  showDeleteDialog.value = false
  notificationToDelete.value = null
}

const navigateToNotification = (notification: Notification) => {
  if (!notification.isRead) {
    markAsRead(notification)
  }
  if (notification.actionUrl) {
    router.push(notification.actionUrl)
  }
}

const goToPreferences = () => {
  router.push('/notifications/preferences')
}

// Load notifications data
const LOADING_DELAY = 600

async function loadNotifications() {
  try {
    error.value = null
    loading.value = true

    await new Promise(resolve => setTimeout(resolve, LOADING_DELAY))

    notifications.value = [
    {
      id: '1',
      type: 'TaskAssigned',
      category: 'Workflow',
      priority: 'High',
      title: 'New Task Assigned',
      titleAr: 'مهمة جديدة مسندة',
      message: 'You have been assigned a new approval task for Document Review - Security Protocol Manual v2.0',
      messageAr: 'تم تعيين مهمة موافقة جديدة لك لمراجعة المستند - دليل بروتوكول الأمن الإصدار 2.0',
      icon: 'pi pi-check-square',
      iconColor: '#3b82f6',
      actionUrl: '/workflow/tasks/123',
      actorName: 'Ahmed Hassan',
      actorAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Ahmed',
      isRead: false,
      isArchived: false,
      createdAt: new Date(Date.now() - 30 * 60 * 1000)
    },
    {
      id: '2',
      type: 'ContentCommented',
      category: 'Content',
      priority: 'Normal',
      title: 'New Comment',
      titleAr: 'تعليق جديد',
      message: 'Sara Ali commented on your article "Tournament Preparations Guide"',
      messageAr: 'علقت سارة علي على مقالتك "دليل استعدادات البطولة"',
      icon: 'pi pi-comment',
      iconColor: '#10b981',
      actionUrl: '/content/456',
      actorName: 'Sara Ali',
      actorAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sara',
      isRead: false,
      isArchived: false,
      createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000)
    },
    {
      id: '3',
      type: 'EventInvitation',
      category: 'Calendar',
      priority: 'Normal',
      title: 'Event Invitation',
      titleAr: 'دعوة لحدث',
      message: 'You have been invited to "Steering Committee Meeting" on December 15, 2027',
      messageAr: 'تمت دعوتك إلى "اجتماع اللجنة التوجيهية" في 15 ديسمبر 2027',
      icon: 'pi pi-calendar',
      iconColor: '#8b5cf6',
      actionUrl: '/calendar/events/789',
      actorName: 'Mohammed Al-Rashid',
      actorAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Mohammed',
      isRead: true,
      isArchived: false,
      createdAt: new Date(Date.now() - 3 * 60 * 60 * 1000)
    },
    {
      id: '4',
      type: 'MentionNotification',
      category: 'Collaboration',
      priority: 'Normal',
      title: 'You were mentioned',
      titleAr: 'تم ذكرك',
      message: 'Fatima mentioned you in a discussion "Venue Logistics Planning"',
      messageAr: 'ذكرتك فاطمة في نقاش "تخطيط لوجستيات الملعب"',
      icon: 'pi pi-at',
      iconColor: '#f59e0b',
      actionUrl: '/collaboration/discussions/101',
      actorName: 'Fatima Al-Saud',
      actorAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Fatima',
      isRead: true,
      isArchived: false,
      createdAt: new Date(Date.now() - 24 * 60 * 60 * 1000)
    },
    {
      id: '5',
      type: 'TaskOverdue',
      category: 'Workflow',
      priority: 'Urgent',
      title: 'Task Overdue',
      titleAr: 'مهمة متأخرة',
      message: 'Your task "Submit Budget Report Q4" is overdue by 2 days',
      messageAr: 'مهمتك "تقديم تقرير الميزانية للربع الرابع" متأخرة بيومين',
      icon: 'pi pi-exclamation-triangle',
      iconColor: '#ef4444',
      actionUrl: '/workflow/tasks/102',
      isRead: false,
      isArchived: false,
      createdAt: new Date(Date.now() - 4 * 60 * 60 * 1000)
    },
    {
      id: '6',
      type: 'ContentPublished',
      category: 'Content',
      priority: 'Normal',
      title: 'Content Published',
      titleAr: 'تم نشر المحتوى',
      message: 'Your article "Media Guidelines for AFC Asian Cup 2027" has been published',
      messageAr: 'تم نشر مقالتك "إرشادات الإعلام لكأس آسيا 2027"',
      icon: 'pi pi-check-circle',
      iconColor: '#10b981',
      actionUrl: '/content/789',
      isRead: true,
      isArchived: false,
      createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000)
    },
    {
      id: '7',
      type: 'SystemUpdate',
      category: 'System',
      priority: 'Low',
      title: 'System Maintenance',
      titleAr: 'صيانة النظام',
      message: 'Scheduled maintenance will occur on December 20, 2027 from 2:00 AM to 4:00 AM',
      messageAr: 'ستتم الصيانة المجدولة في 20 ديسمبر 2027 من الساعة 2:00 صباحاً إلى 4:00 صباحاً',
      icon: 'pi pi-cog',
      iconColor: '#6b7280',
      isRead: true,
      isArchived: false,
      createdAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000)
    },
    {
      id: '8',
      type: 'CommunityJoined',
      category: 'Collaboration',
      priority: 'Normal',
      title: 'New Member',
      titleAr: 'عضو جديد',
      message: 'Omar Ibrahim joined your community "Stadium Operations"',
      messageAr: 'انضم عمر إبراهيم إلى مجتمعك "عمليات الملعب"',
      icon: 'pi pi-user-plus',
      iconColor: '#f59e0b',
      actionUrl: '/collaboration/communities/stadium-ops',
      actorName: 'Omar Ibrahim',
      actorAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Omar',
      isRead: true,
      isArchived: true,
      createdAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000)
    }
  ]

    loading.value = false

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
    loading.value = false
    console.error('Failed to load notifications:', e)
  }
}

async function handleRetry() {
  await loadNotifications()
}

onMounted(() => {
  loadNotifications()
})
</script>

<template>
  <div class="notifications-view" :class="{ rtl: isRTL }">
    <!-- Page Header -->
    <PageHeader
      :title="isRTL ? 'الإشعارات' : 'Notifications'"
      :description="isRTL ? 'ابق على اطلاع بجميع التحديثات والأنشطة' : 'Stay updated with all activities and updates'"
      :breadcrumbs="breadcrumbs"
      padding-bottom="xl"
      background-variant="gradient"
      :animated="shouldAnimate"
    >
      <template #actions>
        <Button
          :label="isRTL ? 'التفضيلات' : 'Preferences'"
          icon="pi pi-cog"
          class="btn-preferences"
          @click="goToPreferences"
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

    <!-- Main Content -->
    <div class="main-content">
      <!-- Toolbar -->
      <div class="notifications-toolbar">
        <div class="toolbar-left">
          <!-- Tabs -->
          <div class="tabs-nav">
            <button
              v-for="tab in tabs"
              :key="tab.key"
              class="tab-btn"
              :class="{ active: activeTab === tab.key }"
              @click="activeTab = tab.key as 'all' | 'unread' | 'archived'"
            >
              <i :class="tab.icon"></i>
              <span>{{ tab.label }}</span>
              <span v-if="tab.count > 0" class="tab-count">{{ tab.count }}</span>
            </button>
          </div>
        </div>

        <div class="toolbar-right">
          <Dropdown
            v-model="selectedCategory"
            :options="categoryOptions"
            optionLabel="label"
            optionValue="value"
            :placeholder="isRTL ? 'الفئة' : 'Category'"
            class="category-dropdown"
          />
        </div>
      </div>

      <!-- Action Bar -->
      <div v-if="filteredNotifications.length > 0" class="action-bar">
        <div class="action-left">
          <div class="select-all">
            <Checkbox
              :modelValue="allSelected"
              :indeterminate="someSelected"
              :binary="true"
              @change="toggleSelectAll"
            />
            <span>{{ isRTL ? 'تحديد الكل' : 'Select all' }}</span>
          </div>
        </div>

        <div class="action-right">
          <template v-if="selectedNotifications.length > 0">
            <span class="selected-count">
              {{ selectedNotifications.length }} {{ isRTL ? 'محدد' : 'selected' }}
            </span>
            <Button
              icon="pi pi-check"
              :label="isRTL ? 'تعليم كمقروء' : 'Mark as read'"
              text
              size="small"
              @click="markSelectedAsRead"
            />
            <Button
              icon="pi pi-trash"
              :label="isRTL ? 'حذف' : 'Delete'"
              text
              size="small"
              severity="danger"
              @click="confirmDeleteSelected"
            />
          </template>
          <template v-else>
            <Button
              icon="pi pi-check-circle"
              :label="isRTL ? 'تعليم الكل كمقروء' : 'Mark all as read'"
              text
              size="small"
              :disabled="stats.unread === 0"
              @click="markAllAsRead"
            />
            <Button
              icon="pi pi-trash"
              :label="isRTL ? 'حذف المقروءة' : 'Delete read'"
              text
              size="small"
              severity="danger"
              :disabled="notifications.filter(n => n.isRead && !n.isArchived).length === 0"
              @click="confirmDeleteAllRead"
            />
          </template>
        </div>
      </div>

      <!-- Notifications List -->
      <div class="notifications-list">
        <!-- Loading -->
        <template v-if="loading">
          <div v-for="i in 5" :key="i" class="notification-skeleton">
            <Skeleton width="48px" height="48px" borderRadius="50%" />
            <div class="skeleton-content">
              <Skeleton width="200px" height="18px" class="mb-2" />
              <Skeleton width="100%" height="40px" class="mb-2" />
              <Skeleton width="150px" height="14px" />
            </div>
          </div>
        </template>

        <!-- Error State -->
        <ErrorState
          v-else-if="error"
          :error="error"
          :title="isRTL ? 'فشل تحميل الإشعارات' : 'Failed to load notifications'"
          show-retry
          @retry="handleRetry"
        />

        <!-- Empty State -->
        <EmptyState
          v-else-if="filteredNotifications.length === 0"
          icon="pi-bell-slash"
          title="No notifications"
          title-arabic="لا توجد إشعارات"
          :description="emptyStateDescription"
          :description-arabic="emptyStateDescriptionArabic"
          variant="default"
          size="lg"
        />

        <!-- Notifications -->
        <template v-else>
          <div
            v-for="(notification, index) in filteredNotifications"
            :key="notification.id"
            class="notification-card"
            :class="{
              unread: !notification.isRead,
              selected: selectedNotifications.includes(notification.id)
            }"
            :style="{ '--animation-order': index }"
          >
            <div class="notification-select">
              <Checkbox
                :modelValue="selectedNotifications.includes(notification.id)"
                :binary="true"
                @change="toggleSelect(notification.id)"
              />
            </div>

            <div class="notification-avatar">
              <Avatar
                :image="notification.actorAvatar"
                :name="notification.actorName"
                :icon="!notification.actorName ? notification.icon : undefined"
                size="lg"
                shape="circle"
              />
            </div>

            <div class="notification-body" @click="navigateToNotification(notification)">
              <div class="notification-header">
                <span class="notification-title">{{ getTitle(notification) }}</span>
                <span
                  v-if="notification.priority !== 'Normal'"
                  class="priority-badge"
                  :style="{ background: getPriorityConfig(notification.priority).bgColor, color: getPriorityConfig(notification.priority).color }"
                >
                  {{ isRTL ? getPriorityConfig(notification.priority).labelAr : getPriorityConfig(notification.priority).label }}
                </span>
              </div>

              <p class="notification-message">{{ getMessage(notification) }}</p>

              <div class="notification-footer">
                <span class="notification-time">
                  <i class="pi pi-clock"></i>
                  {{ formatRelativeTime(notification.createdAt) }}
                </span>
                <span class="notification-category" :style="{ color: getCategoryConfig(notification.category).color }">
                  <i :class="getCategoryConfig(notification.category).icon"></i>
                  {{ isRTL ? getCategoryConfig(notification.category).labelAr : getCategoryConfig(notification.category).label }}
                </span>
                <span v-if="notification.actorName" class="notification-actor">
                  <i class="pi pi-user"></i>
                  {{ notification.actorName }}
                </span>
              </div>
            </div>

            <div class="notification-actions">
              <Button
                v-if="!notification.isRead"
                icon="pi pi-check"
                text
                rounded
                size="small"
                v-tooltip.top="isRTL ? 'تعليم كمقروء' : 'Mark as read'"
                @click="markAsRead(notification)"
              />
              <Button
                v-else
                icon="pi pi-envelope"
                text
                rounded
                size="small"
                v-tooltip.top="isRTL ? 'تعليم كغير مقروء' : 'Mark as unread'"
                @click="markAsUnread(notification)"
              />
              <Button
                v-if="!notification.isArchived"
                icon="pi pi-inbox"
                text
                rounded
                size="small"
                v-tooltip.top="isRTL ? 'أرشفة' : 'Archive'"
                @click="archiveNotification(notification)"
              />
              <Button
                v-else
                icon="pi pi-reply"
                text
                rounded
                size="small"
                v-tooltip.top="isRTL ? 'إلغاء الأرشفة' : 'Unarchive'"
                @click="unarchiveNotification(notification)"
              />
              <Button
                icon="pi pi-trash"
                text
                rounded
                size="small"
                severity="danger"
                v-tooltip.top="isRTL ? 'حذف' : 'Delete'"
                @click="confirmDeleteSingle(notification)"
              />
            </div>
          </div>
        </template>
      </div>
    </div>

    <!-- Delete Confirmation Dialog -->
    <Dialog
      v-model:visible="showDeleteDialog"
      :header="isRTL ? 'تأكيد الحذف' : 'Confirm Delete'"
      :modal="true"
      :style="{ width: '400px' }"
      class="delete-dialog"
    >
      <div class="delete-dialog-content">
        <i class="pi pi-exclamation-triangle warning-icon"></i>
        <p>
          {{
            deleteTarget === 'single'
              ? (isRTL ? 'هل أنت متأكد من حذف هذا الإشعار؟' : 'Are you sure you want to delete this notification?')
              : deleteTarget === 'selected'
                ? (isRTL ? `هل أنت متأكد من حذف ${selectedNotifications.length} إشعار؟` : `Are you sure you want to delete ${selectedNotifications.length} notifications?`)
                : (isRTL ? 'هل أنت متأكد من حذف جميع الإشعارات المقروءة؟' : 'Are you sure you want to delete all read notifications?')
          }}
        </p>
      </div>
      <template #footer>
        <Button
          :label="isRTL ? 'إلغاء' : 'Cancel'"
          text
          @click="showDeleteDialog = false"
        />
        <Button
          :label="isRTL ? 'حذف' : 'Delete'"
          severity="danger"
          @click="executeDelete"
        />
      </template>
    </Dialog>
  </div>
</template>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

// ============================================
// NOTIFICATIONS VIEW - MAIN CONTAINER
// Following Universal Spacing Guidelines from _mixins.scss
// ============================================

.notifications-view {
  @include page-view;
}

// Header button styles (used in PageHeader slot)
.btn-preferences {
  background: rgba(#fff, 0.2);
  border: none;
  color: #fff;

  &:hover {
    background: rgba(#fff, 0.3);
  }
}

// ============================================
// MAIN CONTENT
// ============================================

.main-content {
  max-width: 900px;
  margin: 0 auto;
  padding: $spacing-6;
}

// Toolbar
.notifications-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: $spacing-4;
  margin-bottom: $spacing-4;
}

.tabs-nav {
  display: flex;
  gap: $spacing-1;
  background: #fff;
  padding: $spacing-1;
  border-radius: $radius-xl;
  box-shadow: $shadow-sm;
}

.tab-btn {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-2 $spacing-4;
  background: none;
  border: none;
  border-radius: $radius-lg;
  font-size: $text-sm;
  font-weight: $font-weight-medium;
  color: $slate-600;
  cursor: pointer;
  transition: all $transition-fast;

  &:hover {
    background: $slate-100;
    color: $slate-900;
  }

  &.active {
    background: $intalio-teal-500;
    color: #fff;

    .tab-count {
      background: rgba(#fff, 0.2);
      color: #fff;
    }
  }

  .tab-count {
    padding: 2px 6px;
    background: $slate-100;
    border-radius: $radius-full;
    font-size: $text-xs;
  }
}

.category-dropdown {
  min-width: 180px;
}

// Action Bar
.action-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: $spacing-3;
  padding: $spacing-3 $spacing-4;
  background: #fff;
  border-radius: $radius-xl;
  box-shadow: $shadow-sm;
  margin-bottom: $spacing-4;
}

.action-left {
  .select-all {
    display: flex;
    align-items: center;
    gap: $spacing-2;
    font-size: $text-sm;
    color: $slate-600;
  }
}

.action-right {
  display: flex;
  align-items: center;
  gap: $spacing-2;

  .selected-count {
    font-size: $text-sm;
    font-weight: $font-weight-medium;
    color: $slate-600;
    padding-right: $spacing-2;
    border-right: 1px solid $slate-200;

    .rtl & {
      padding-right: 0;
      padding-left: $spacing-2;
      border-right: none;
      border-left: 1px solid $slate-200;
    }
  }
}

// Notifications List
.notifications-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;
}

// Loading skeleton
.notification-skeleton {
  display: flex;
  gap: $spacing-4;
  padding: $spacing-5;
  background: #fff;
  border-radius: $radius-xl;

  .skeleton-content {
    flex: 1;
  }
}

// Notification Card
.notification-card {
  display: flex;
  align-items: flex-start;
  gap: $spacing-4;
  padding: $spacing-4;
  background: #fff;
  border-radius: $radius-xl;
  box-shadow: $shadow-sm;
  transition: all $transition-base;
  border: 2px solid transparent;

  @include card-item-animation;

  &:hover {
    box-shadow: $shadow-md;

    .notification-actions {
      opacity: 1;
    }
  }

  &.unread {
    background: rgba($intalio-teal-50, 0.5);
    border-color: $intalio-teal-100;

    .notification-title {
      font-weight: $font-weight-semibold;
    }

    &::before {
      content: '';
      position: absolute;
      left: 0;
      top: 50%;
      transform: translateY(-50%);
      width: 4px;
      height: 40%;
      background: $intalio-teal-500;
      border-radius: 0 $radius-full $radius-full 0;
    }
  }

  &.selected {
    background: $intalio-teal-50;
    border-color: $intalio-teal-300;
  }

  position: relative;
}

.notification-select {
  padding-top: $spacing-1;
}

.notification-avatar {
  :deep(.p-avatar) {
    width: 48px;
    height: 48px;
  }

  .avatar-placeholder {
    width: 48px;
    height: 48px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: $text-base;
    font-weight: $font-weight-semibold;

    i {
      font-size: $text-xl;
    }
  }
}

.notification-body {
  flex: 1;
  min-width: 0;
  cursor: pointer;
}

.notification-header {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  margin-bottom: $spacing-1;
}

.notification-title {
  font-size: $text-base;
  color: $slate-900;
}

.priority-badge {
  padding: 2px $spacing-2;
  border-radius: $radius-md;
  font-size: $text-xs;
  font-weight: $font-weight-semibold;
}

.notification-message {
  margin: 0 0 $spacing-2;
  font-size: $text-sm;
  color: $slate-600;
  line-height: 1.5;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.notification-footer {
  display: flex;
  flex-wrap: wrap;
  gap: $spacing-4;
  font-size: $text-xs;
  color: $slate-500;

  span {
    display: flex;
    align-items: center;
    gap: $spacing-1;
  }
}

.notification-actions {
  display: flex;
  gap: $spacing-1;
  opacity: 0;
  transition: opacity $transition-fast;

  @media (max-width: 768px) {
    opacity: 1;
  }
}

// ============================================
// DELETE DIALOG
// ============================================

.delete-dialog {
  :deep(.p-dialog-content) {
    padding: $spacing-6;
  }
}

.delete-dialog-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;

  .warning-icon {
    font-size: 48px;
    color: #f59e0b;
    margin-bottom: $spacing-4;
  }

  p {
    margin: 0;
    color: $slate-700;
    font-size: $text-base;
  }
}

// ============================================
// ANIMATIONS
// ============================================

@include staggered-animation-delays('.notification-card', 20, 0.03s);

// ============================================
// RTL SUPPORT
// ============================================

.rtl {
  direction: rtl;

  .breadcrumb-separator {
    transform: rotate(180deg);
  }

  .notification-card.unread::before {
    left: auto;
    right: 0;
    border-radius: $radius-full 0 0 $radius-full;
  }
}
</style>
