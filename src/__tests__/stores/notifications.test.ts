import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useNotificationsStore } from '@/stores/notifications'
import { notificationsApi } from '@/api'

// Mock the API module
vi.mock('@/api', () => ({
  notificationsApi: {
    getNotifications: vi.fn(),
    getUnreadCount: vi.fn(),
    markAsRead: vi.fn(),
    markAllAsRead: vi.fn(),
    deleteNotification: vi.fn(),
  },
}))

describe('Notifications Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Initial State', () => {
    it('should have empty notifications array', () => {
      const store = useNotificationsStore()
      expect(store.notifications).toEqual([])
    })

    it('should have zero unread count', () => {
      const store = useNotificationsStore()
      expect(store.unreadCount).toBe(0)
    })

    it('should not be loading initially', () => {
      const store = useNotificationsStore()
      expect(store.isLoading).toBe(false)
    })
  })

  describe('fetchNotifications', () => {
    it('should fetch and append notifications', async () => {
      const mockNotifications = [
        { id: '1', title: 'Test 1', message: 'Message 1', isRead: false },
        { id: '2', title: 'Test 2', message: 'Message 2', isRead: true },
      ]

      vi.mocked(notificationsApi.getNotifications).mockResolvedValue({
        items: mockNotifications,
        hasNext: true,
        totalCount: 10,
        page: 1,
        pageSize: 20,
        hasPrevious: false,
      })

      const store = useNotificationsStore()
      await store.fetchNotifications()

      expect(store.notifications).toHaveLength(2)
      expect(store.hasMore).toBe(true)
    })

    it('should reset notifications when reset=true', async () => {
      const store = useNotificationsStore()
      store.notifications = [{ id: 'old', title: 'Old', message: '', isRead: true }] as any

      vi.mocked(notificationsApi.getNotifications).mockResolvedValue({
        items: [{ id: 'new', title: 'New', message: '', isRead: false }],
        hasNext: false,
        totalCount: 1,
        page: 1,
        pageSize: 20,
        hasPrevious: false,
      })

      await store.fetchNotifications(true)

      expect(store.notifications).toHaveLength(1)
      expect(store.notifications[0].id).toBe('new')
    })

    it('should not fetch when already loading', async () => {
      const store = useNotificationsStore()
      store.isLoading = true

      await store.fetchNotifications()

      expect(notificationsApi.getNotifications).not.toHaveBeenCalled()
    })

    it('should not fetch when no more pages', async () => {
      const store = useNotificationsStore()
      store.hasMore = false

      await store.fetchNotifications()

      expect(notificationsApi.getNotifications).not.toHaveBeenCalled()
    })
  })

  describe('fetchUnreadCount', () => {
    it('should fetch and update unread count', async () => {
      vi.mocked(notificationsApi.getUnreadCount).mockResolvedValue(5)

      const store = useNotificationsStore()
      await store.fetchUnreadCount()

      expect(store.unreadCount).toBe(5)
    })

    it('should handle fetch error', async () => {
      vi.mocked(notificationsApi.getUnreadCount).mockRejectedValue(new Error('Network error'))

      const store = useNotificationsStore()
      await store.fetchUnreadCount()

      expect(store.unreadCount).toBe(0)
    })
  })

  describe('markAsRead', () => {
    it('should mark notification as read', async () => {
      vi.mocked(notificationsApi.markAsRead).mockResolvedValue(undefined)

      const store = useNotificationsStore()
      store.notifications = [
        { id: '1', title: 'Test', message: '', isRead: false },
      ] as any
      store.unreadCount = 1

      await store.markAsRead('1')

      expect(store.notifications[0].isRead).toBe(true)
      expect(store.unreadCount).toBe(0)
    })

    it('should not decrement unread count if already read', async () => {
      vi.mocked(notificationsApi.markAsRead).mockResolvedValue(undefined)

      const store = useNotificationsStore()
      store.notifications = [
        { id: '1', title: 'Test', message: '', isRead: true },
      ] as any
      store.unreadCount = 0

      await store.markAsRead('1')

      expect(store.unreadCount).toBe(0)
    })
  })

  describe('markAllAsRead', () => {
    it('should mark all notifications as read', async () => {
      vi.mocked(notificationsApi.markAllAsRead).mockResolvedValue(undefined)

      const store = useNotificationsStore()
      store.notifications = [
        { id: '1', title: 'Test 1', message: '', isRead: false },
        { id: '2', title: 'Test 2', message: '', isRead: false },
        { id: '3', title: 'Test 3', message: '', isRead: true },
      ] as any
      store.unreadCount = 2

      await store.markAllAsRead()

      expect(store.notifications.every((n) => n.isRead)).toBe(true)
      expect(store.unreadCount).toBe(0)
    })
  })

  describe('deleteNotification', () => {
    it('should delete notification and update unread count', async () => {
      vi.mocked(notificationsApi.deleteNotification).mockResolvedValue(undefined)

      const store = useNotificationsStore()
      store.notifications = [
        { id: '1', title: 'Test 1', message: '', isRead: false },
        { id: '2', title: 'Test 2', message: '', isRead: true },
      ] as any
      store.unreadCount = 1

      await store.deleteNotification('1')

      expect(store.notifications).toHaveLength(1)
      expect(store.notifications[0].id).toBe('2')
      expect(store.unreadCount).toBe(0)
    })

    it('should not change unread count when deleting read notification', async () => {
      vi.mocked(notificationsApi.deleteNotification).mockResolvedValue(undefined)

      const store = useNotificationsStore()
      store.notifications = [
        { id: '1', title: 'Test', message: '', isRead: true },
      ] as any
      store.unreadCount = 0

      await store.deleteNotification('1')

      expect(store.notifications).toHaveLength(0)
      expect(store.unreadCount).toBe(0)
    })
  })

  describe('addNotification', () => {
    it('should add notification to beginning of list', () => {
      const store = useNotificationsStore()
      store.notifications = [
        { id: '1', title: 'Old', message: '', isRead: true },
      ] as any

      store.addNotification({
        id: '2',
        title: 'New',
        message: '',
        isRead: false,
      } as any)

      expect(store.notifications).toHaveLength(2)
      expect(store.notifications[0].id).toBe('2')
    })

    it('should increment unread count for unread notifications', () => {
      const store = useNotificationsStore()
      store.unreadCount = 0

      store.addNotification({
        id: '1',
        title: 'Test',
        message: '',
        isRead: false,
      } as any)

      expect(store.unreadCount).toBe(1)
    })

    it('should not increment unread count for read notifications', () => {
      const store = useNotificationsStore()
      store.unreadCount = 0

      store.addNotification({
        id: '1',
        title: 'Test',
        message: '',
        isRead: true,
      } as any)

      expect(store.unreadCount).toBe(0)
    })
  })

  describe('Computed Properties', () => {
    it('should return unread notifications', () => {
      const store = useNotificationsStore()
      store.notifications = [
        { id: '1', title: 'Test 1', isRead: false },
        { id: '2', title: 'Test 2', isRead: true },
        { id: '3', title: 'Test 3', isRead: false },
      ] as any

      expect(store.unreadNotifications).toHaveLength(2)
      expect(store.unreadNotifications.map((n) => n.id)).toEqual(['1', '3'])
    })

    it('should return recent notifications (first 5)', () => {
      const store = useNotificationsStore()
      store.notifications = Array.from({ length: 10 }, (_, i) => ({
        id: String(i),
        title: `Test ${i}`,
        isRead: false,
      })) as any

      expect(store.recentNotifications).toHaveLength(5)
      expect(store.recentNotifications[0].id).toBe('0')
    })
  })
})
