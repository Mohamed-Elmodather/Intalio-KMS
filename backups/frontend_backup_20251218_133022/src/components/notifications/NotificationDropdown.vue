<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { Avatar } from '@/components/base'
import Button from 'primevue/button'
import Badge from 'primevue/badge'
import OverlayPanel from 'primevue/overlaypanel'
import TabView from 'primevue/tabview'
import TabPanel from 'primevue/tabpanel'
import Skeleton from 'primevue/skeleton'

const { t, locale } = useI18n()
const router = useRouter()

// Refs
const op = ref()
const loading = ref(false)
const notifications = ref<any[]>([])
const unreadCount = ref(8)
const activeTab = ref(0)

// Mock data
const mockNotifications = [
  {
    id: '1',
    type: 'TaskAssigned',
    category: 'Workflow',
    priority: 'High',
    title: 'New Task Assigned',
    titleAr: 'مهمة جديدة مسندة',
    message: 'You have been assigned a new approval task',
    messageAr: 'تم تعيين مهمة موافقة جديدة لك',
    icon: 'pi-check-square',
    iconColor: '#3B82F6',
    actionUrl: '/workflow/tasks/123',
    actorName: 'Ahmed Hassan',
    isRead: false,
    createdAt: new Date(Date.now() - 30 * 60 * 1000),
    timeAgo: '30m'
  },
  {
    id: '2',
    type: 'ContentCommented',
    category: 'Content',
    priority: 'Normal',
    title: 'New Comment',
    titleAr: 'تعليق جديد',
    message: 'Sara commented on your article',
    messageAr: 'علقت سارة على مقالتك',
    icon: 'pi-comment',
    iconColor: '#10B981',
    actionUrl: '/content/456',
    actorName: 'Sara Ali',
    isRead: false,
    createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000),
    timeAgo: '2h'
  },
  {
    id: '3',
    type: 'EventInvitation',
    category: 'Calendar',
    priority: 'Normal',
    title: 'Event Invitation',
    titleAr: 'دعوة لحدث',
    message: 'Invited to Steering Committee Meeting',
    messageAr: 'دعوة لاجتماع اللجنة التوجيهية',
    icon: 'pi-calendar',
    iconColor: '#8B5CF6',
    actionUrl: '/calendar/events/789',
    actorName: 'Mohammed Al-Rashid',
    isRead: true,
    createdAt: new Date(Date.now() - 3 * 60 * 60 * 1000),
    timeAgo: '3h'
  },
  {
    id: '4',
    type: 'MentionNotification',
    category: 'Collaboration',
    priority: 'Normal',
    title: 'You were mentioned',
    titleAr: 'تم ذكرك',
    message: 'Fatima mentioned you in a discussion',
    messageAr: 'ذكرتك فاطمة في نقاش',
    icon: 'pi-at',
    iconColor: '#F59E0B',
    actionUrl: '/collaboration/discussions/101',
    actorName: 'Fatima Al-Saud',
    isRead: true,
    createdAt: new Date(Date.now() - 24 * 60 * 60 * 1000),
    timeAgo: '1d'
  },
  {
    id: '5',
    type: 'TaskOverdue',
    category: 'Workflow',
    priority: 'Urgent',
    title: 'Task Overdue',
    titleAr: 'مهمة متأخرة',
    message: 'Submit Budget Report is overdue',
    messageAr: 'تقديم تقرير الميزانية متأخر',
    icon: 'pi-exclamation-triangle',
    iconColor: '#EF4444',
    actionUrl: '/workflow/tasks/102',
    actorName: null,
    isRead: false,
    createdAt: new Date(Date.now() - 4 * 60 * 60 * 1000),
    timeAgo: '4h'
  }
]

// Computed
const isRtl = computed(() => locale.value === 'ar')
const unreadNotifications = computed(() => notifications.value.filter(n => !n.isRead))

// Methods
const toggle = (event: Event) => {
  op.value.toggle(event)
  if (!notifications.value.length) {
    loadNotifications()
  }
}

const loadNotifications = async () => {
  loading.value = true
  try {
    await new Promise(resolve => setTimeout(resolve, 300))
    notifications.value = mockNotifications
    unreadCount.value = mockNotifications.filter(n => !n.isRead).length
  } finally {
    loading.value = false
  }
}

const getTitle = (notification: any) => {
  return isRtl.value ? notification.titleAr : notification.title
}

const getMessage = (notification: any) => {
  return isRtl.value ? notification.messageAr : notification.message
}

const markAsRead = async (notification: any, event: Event) => {
  event.stopPropagation()
  notification.isRead = true
  unreadCount.value = Math.max(0, unreadCount.value - 1)
}

const markAllAsRead = async () => {
  notifications.value.forEach(n => n.isRead = true)
  unreadCount.value = 0
}

const navigateTo = (notification: any) => {
  if (!notification.isRead) {
    notification.isRead = true
    unreadCount.value = Math.max(0, unreadCount.value - 1)
  }
  op.value.hide()
  if (notification.actionUrl) {
    router.push(notification.actionUrl)
  }
}

const viewAll = () => {
  op.value.hide()
  router.push('/notifications')
}

const goToPreferences = () => {
  op.value.hide()
  router.push('/notifications/preferences')
}

// Lifecycle
onMounted(() => {
  // Could set up WebSocket/SignalR connection here
})

// Expose for parent
defineExpose({ toggle })
</script>

<template>
  <div class="notification-dropdown">
    <Button
      icon="pi pi-bell"
      class="p-button-rounded p-button-text notification-trigger"
      :badge="unreadCount > 0 ? String(unreadCount) : undefined"
      badge-class="p-badge-danger"
      @click="toggle"
      v-tooltip.bottom="t('notifications.title')"
    />

    <OverlayPanel
      ref="op"
      :style="{ width: '380px' }"
      class="notification-panel"
    >
      <div class="panel-header">
        <h3>{{ t('notifications.title') }}</h3>
        <div class="header-actions">
          <Button
            v-if="unreadCount > 0"
            :label="t('notifications.markAllRead')"
            class="p-button-link p-button-sm"
            @click="markAllAsRead"
          />
          <Button
            icon="pi pi-cog"
            class="p-button-rounded p-button-text p-button-sm"
            v-tooltip.top="t('notifications.preferences')"
            @click="goToPreferences"
          />
        </div>
      </div>

      <TabView v-model:activeIndex="activeTab" class="notification-tabs">
        <TabPanel value="0">
          <template #header>
            <span>{{ t('notifications.tabs.all') }}</span>
            <Badge
              v-if="notifications.length > 0"
              :value="notifications.length"
              class="ms-2"
            />
          </template>

          <div class="notification-list">
            <template v-if="loading">
              <div v-for="i in 3" :key="i" class="notification-skeleton">
                <Skeleton shape="circle" size="2.5rem" />
                <div class="flex-1">
                  <Skeleton width="70%" height="0.875rem" />
                  <Skeleton width="90%" height="0.75rem" class="mt-2" />
                </div>
              </div>
            </template>

            <template v-else-if="notifications.length === 0">
              <div class="empty-state">
                <i class="pi pi-bell-slash"></i>
                <p>{{ t('notifications.empty') }}</p>
              </div>
            </template>

            <template v-else>
              <div
                v-for="notification in notifications"
                :key="notification.id"
                class="notification-item"
                :class="{ unread: !notification.isRead }"
                @click="navigateTo(notification)"
              >
                <div
                  class="notification-icon"
                  :style="{ backgroundColor: notification.iconColor + '20', color: notification.iconColor }"
                >
                  <Avatar
                    v-if="notification.actorName"
                    :name="notification.actorName"
                    shape="circle"
                    :bg-color="notification.iconColor"
                    text-color="white"
                    size="sm"
                  />
                  <i v-else :class="'pi ' + notification.icon"></i>
                </div>

                <div class="notification-content">
                  <div class="notification-title">{{ getTitle(notification) }}</div>
                  <div class="notification-message">{{ getMessage(notification) }}</div>
                  <div class="notification-time">{{ notification.timeAgo }}</div>
                </div>

                <Button
                  v-if="!notification.isRead"
                  icon="pi pi-circle-fill"
                  class="p-button-rounded p-button-text p-button-sm unread-indicator"
                  @click="markAsRead(notification, $event)"
                  v-tooltip.top="t('notifications.markAsRead')"
                />
              </div>
            </template>
          </div>
        </TabPanel>

        <TabPanel value="1">
          <template #header>
            <span>{{ t('notifications.tabs.unread') }}</span>
            <Badge
              v-if="unreadCount > 0"
              :value="unreadCount"
              severity="danger"
              class="ms-2"
            />
          </template>

          <div class="notification-list">
            <template v-if="unreadNotifications.length === 0">
              <div class="empty-state">
                <i class="pi pi-check-circle"></i>
                <p>{{ t('notifications.allRead') }}</p>
              </div>
            </template>

            <template v-else>
              <div
                v-for="notification in unreadNotifications"
                :key="notification.id"
                class="notification-item unread"
                @click="navigateTo(notification)"
              >
                <div
                  class="notification-icon"
                  :style="{ backgroundColor: notification.iconColor + '20', color: notification.iconColor }"
                >
                  <Avatar
                    v-if="notification.actorName"
                    :name="notification.actorName"
                    shape="circle"
                    :bg-color="notification.iconColor"
                    text-color="white"
                    size="sm"
                  />
                  <i v-else :class="'pi ' + notification.icon"></i>
                </div>

                <div class="notification-content">
                  <div class="notification-title">{{ getTitle(notification) }}</div>
                  <div class="notification-message">{{ getMessage(notification) }}</div>
                  <div class="notification-time">{{ notification.timeAgo }}</div>
                </div>

                <Button
                  icon="pi pi-circle-fill"
                  class="p-button-rounded p-button-text p-button-sm unread-indicator"
                  @click="markAsRead(notification, $event)"
                  v-tooltip.top="t('notifications.markAsRead')"
                />
              </div>
            </template>
          </div>
        </TabPanel>
      </TabView>

      <div class="panel-footer">
        <Button
          :label="t('notifications.viewAll')"
          class="p-button-text w-full"
          @click="viewAll"
        />
      </div>
    </OverlayPanel>
  </div>
</template>

<style scoped lang="scss">
.notification-dropdown {
  position: relative;
}

.notification-trigger {
  position: relative;
}

:deep(.notification-panel) {
  .p-overlaypanel-content {
    padding: 0;
  }
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border-bottom: 1px solid var(--surface-border);

  h3 {
    margin: 0;
    font-size: 1rem;
    font-weight: 600;
  }

  .header-actions {
    display: flex;
    align-items: center;
    gap: 0.25rem;
  }
}

.notification-tabs {
  :deep(.p-tabview-nav) {
    padding: 0 0.5rem;
    border-bottom: 1px solid var(--surface-border);
  }

  :deep(.p-tabview-panels) {
    padding: 0;
  }
}

.notification-list {
  max-height: 350px;
  overflow-y: auto;
}

.notification-skeleton {
  display: flex;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  border-bottom: 1px solid var(--surface-border);
}

.notification-item {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  cursor: pointer;
  transition: background-color 0.2s;
  border-bottom: 1px solid var(--surface-border);

  &:last-child {
    border-bottom: none;
  }

  &:hover {
    background: var(--surface-hover);
  }

  &.unread {
    background: rgba(var(--primary-color-rgb), 0.05);

    .notification-title {
      font-weight: 600;
    }
  }
}

.notification-icon {
  width: 2.5rem;
  height: 2.5rem;
  min-width: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  font-size: 1rem;
}

.notification-content {
  flex: 1;
  min-width: 0;

  .notification-title {
    font-size: 0.875rem;
    margin-bottom: 0.125rem;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .notification-message {
    font-size: 0.75rem;
    color: var(--text-color-secondary);
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .notification-time {
    font-size: 0.7rem;
    color: var(--text-color-secondary);
    margin-top: 0.25rem;
  }
}

.unread-indicator {
  color: var(--primary-color);
  font-size: 0.5rem;

  &:hover {
    background: transparent !important;
  }
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  color: var(--text-color-secondary);

  i {
    font-size: 2rem;
    margin-bottom: 0.5rem;
  }

  p {
    margin: 0;
    font-size: 0.875rem;
  }
}

.panel-footer {
  padding: 0.5rem;
  border-top: 1px solid var(--surface-border);
}
</style>
