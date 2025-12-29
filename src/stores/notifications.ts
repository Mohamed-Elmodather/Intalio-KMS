import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Notification } from '@/types'
import { notificationsApi } from '@/api'

export const useNotificationsStore = defineStore('notifications', () => {
  // State
  const notifications = ref<Notification[]>([])
  const unreadCount = ref(0)
  const isLoading = ref(false)
  const hasMore = ref(true)
  const page = ref(1)

  // Getters
  const unreadNotifications = computed(() =>
    notifications.value.filter((n) => !n.isRead)
  )

  const recentNotifications = computed(() =>
    notifications.value.slice(0, 5)
  )

  // Actions
  async function fetchNotifications(reset = false) {
    if (reset) {
      page.value = 1
      notifications.value = []
      hasMore.value = true
    }

    if (!hasMore.value || isLoading.value) return

    isLoading.value = true
    try {
      const response = await notificationsApi.getNotifications({
        page: page.value,
        pageSize: 20,
      })

      notifications.value.push(...response.items)
      hasMore.value = response.hasNext
      page.value++
    } catch (error) {
      console.error('Failed to fetch notifications:', error)
    } finally {
      isLoading.value = false
    }
  }

  async function fetchUnreadCount() {
    try {
      unreadCount.value = await notificationsApi.getUnreadCount()
    } catch (error) {
      console.error('Failed to fetch unread count:', error)
    }
  }

  async function markAsRead(id: string) {
    try {
      await notificationsApi.markAsRead(id)
      const notification = notifications.value.find((n) => n.id === id)
      if (notification && !notification.isRead) {
        notification.isRead = true
        unreadCount.value = Math.max(0, unreadCount.value - 1)
      }
    } catch (error) {
      console.error('Failed to mark notification as read:', error)
    }
  }

  async function markAllAsRead() {
    try {
      await notificationsApi.markAllAsRead()
      notifications.value.forEach((n) => (n.isRead = true))
      unreadCount.value = 0
    } catch (error) {
      console.error('Failed to mark all as read:', error)
    }
  }

  async function deleteNotification(id: string) {
    try {
      await notificationsApi.deleteNotification(id)
      const index = notifications.value.findIndex((n) => n.id === id)
      if (index !== -1) {
        const notification = notifications.value[index]
        if (notification && !notification.isRead) {
          unreadCount.value = Math.max(0, unreadCount.value - 1)
        }
        notifications.value.splice(index, 1)
      }
    } catch (error) {
      console.error('Failed to delete notification:', error)
    }
  }

  // Add notification from real-time source (SignalR)
  function addNotification(notification: Notification) {
    notifications.value.unshift(notification)
    if (!notification.isRead) {
      unreadCount.value++
    }
  }

  return {
    // State
    notifications,
    unreadCount,
    isLoading,
    hasMore,

    // Getters
    unreadNotifications,
    recentNotifications,

    // Actions
    fetchNotifications,
    fetchUnreadCount,
    markAsRead,
    markAllAsRead,
    deleteNotification,
    addNotification,
  }
})
