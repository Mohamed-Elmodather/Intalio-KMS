import { describe, it, expect, vi, beforeEach } from 'vitest'
import { notificationsApi } from '@/api/notifications'
import apiClient from '@/api/client'

// Mock the API client
vi.mock('@/api/client', () => ({
  default: {
    post: vi.fn(),
    get: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}))

describe('Notifications API', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('getNotifications', () => {
    it('should fetch notifications without params', async () => {
      const mockResponse = {
        items: [
          { id: '1', message: 'Notification 1', read: false },
          { id: '2', message: 'Notification 2', read: true },
        ],
        totalCount: 2,
        page: 1,
        pageSize: 10,
      }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockResponse })

      const result = await notificationsApi.getNotifications()

      expect(apiClient.get).toHaveBeenCalledWith('/notifications', {
        params: undefined,
      })
      expect(result).toEqual(mockResponse)
    })

    it('should fetch notifications with pagination params', async () => {
      const mockResponse = { items: [], totalCount: 0, page: 2, pageSize: 20 }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockResponse })

      const params = { page: 2, pageSize: 20 }
      await notificationsApi.getNotifications(params)

      expect(apiClient.get).toHaveBeenCalledWith('/notifications', {
        params,
      })
    })
  })

  describe('getUnreadCount', () => {
    it('should return unread notification count', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: { count: 5 } })

      const result = await notificationsApi.getUnreadCount()

      expect(apiClient.get).toHaveBeenCalledWith('/notifications/unread-count')
      expect(result).toBe(5)
    })

    it('should return zero when no unread notifications', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: { count: 0 } })

      const result = await notificationsApi.getUnreadCount()

      expect(result).toBe(0)
    })
  })

  describe('markAsRead', () => {
    it('should mark notification as read', async () => {
      vi.mocked(apiClient.post).mockResolvedValue({ data: {} })

      await notificationsApi.markAsRead('notification-123')

      expect(apiClient.post).toHaveBeenCalledWith('/notifications/notification-123/read')
    })
  })

  describe('markAllAsRead', () => {
    it('should mark all notifications as read', async () => {
      vi.mocked(apiClient.post).mockResolvedValue({ data: {} })

      await notificationsApi.markAllAsRead()

      expect(apiClient.post).toHaveBeenCalledWith('/notifications/read-all')
    })
  })

  describe('deleteNotification', () => {
    it('should delete notification', async () => {
      vi.mocked(apiClient.delete).mockResolvedValue({ data: {} })

      await notificationsApi.deleteNotification('notification-456')

      expect(apiClient.delete).toHaveBeenCalledWith('/notifications/notification-456')
    })
  })

  describe('getPreferences', () => {
    it('should fetch notification preferences', async () => {
      const mockPreferences = {
        email: true,
        push: false,
        sms: false,
        inApp: true,
      }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockPreferences })

      const result = await notificationsApi.getPreferences()

      expect(apiClient.get).toHaveBeenCalledWith('/notifications/preferences')
      expect(result).toEqual(mockPreferences)
    })
  })

  describe('updatePreferences', () => {
    it('should update notification preferences', async () => {
      const newPreferences = {
        email: true,
        push: true,
        sms: false,
        inApp: true,
      }
      vi.mocked(apiClient.put).mockResolvedValue({ data: {} })

      await notificationsApi.updatePreferences(newPreferences)

      expect(apiClient.put).toHaveBeenCalledWith('/notifications/preferences', newPreferences)
    })

    it('should update single preference', async () => {
      vi.mocked(apiClient.put).mockResolvedValue({ data: {} })

      await notificationsApi.updatePreferences({ email: false })

      expect(apiClient.put).toHaveBeenCalledWith('/notifications/preferences', { email: false })
    })
  })
})
