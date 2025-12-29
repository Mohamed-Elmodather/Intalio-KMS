import apiClient from './client'
import type { Notification, PaginatedResponse, PaginationParams } from '@/types'

const NOTIFICATIONS_BASE = '/notifications'

export const notificationsApi = {
  // Get notifications
  async getNotifications(params?: PaginationParams): Promise<PaginatedResponse<Notification>> {
    const response = await apiClient.get<PaginatedResponse<Notification>>(NOTIFICATIONS_BASE, {
      params,
    })
    return response.data
  },

  // Get unread count
  async getUnreadCount(): Promise<number> {
    const response = await apiClient.get<{ count: number }>(`${NOTIFICATIONS_BASE}/unread-count`)
    return response.data.count
  },

  // Mark as read
  async markAsRead(id: string): Promise<void> {
    await apiClient.post(`${NOTIFICATIONS_BASE}/${id}/read`)
  },

  // Mark all as read
  async markAllAsRead(): Promise<void> {
    await apiClient.post(`${NOTIFICATIONS_BASE}/read-all`)
  },

  // Delete notification
  async deleteNotification(id: string): Promise<void> {
    await apiClient.delete(`${NOTIFICATIONS_BASE}/${id}`)
  },

  // Get notification preferences
  async getPreferences(): Promise<Record<string, boolean>> {
    const response = await apiClient.get<Record<string, boolean>>(`${NOTIFICATIONS_BASE}/preferences`)
    return response.data
  },

  // Update notification preferences
  async updatePreferences(preferences: Record<string, boolean>): Promise<void> {
    await apiClient.put(`${NOTIFICATIONS_BASE}/preferences`, preferences)
  },
}

export default notificationsApi
