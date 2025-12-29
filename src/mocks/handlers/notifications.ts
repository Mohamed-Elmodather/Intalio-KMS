import { http, HttpResponse, delay } from 'msw'
import type { Notification, PaginatedResponse } from '@/types'

const API_BASE = import.meta.env.VITE_API_BASE_URL || '/api'

// Mock notifications
const mockNotifications: Notification[] = [
  {
    id: '1',
    type: 'info',
    title: 'New comment on "Q4 Strategy Document"',
    message: 'Sarah Johnson commented on your document',
    link: '/documents/1',
    icon: 'fas fa-comment',
    isRead: false,
    createdAt: new Date(Date.now() - 5 * 60 * 1000).toISOString(),
  },
  {
    id: '2',
    type: 'success',
    title: 'Course completed',
    message: 'You completed "Advanced Excel Training"',
    link: '/learning/1',
    icon: 'fas fa-graduation-cap',
    isRead: false,
    createdAt: new Date(Date.now() - 60 * 60 * 1000).toISOString(),
  },
  {
    id: '3',
    type: 'warning',
    title: 'Meeting reminder',
    message: 'Team meeting starts in 30 minutes',
    link: '/events/1',
    icon: 'fas fa-calendar',
    isRead: false,
    createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString(),
  },
  {
    id: '4',
    type: 'info',
    title: 'New poll available',
    message: '"Office Location Preferences" is waiting for your vote',
    link: '/polls/1',
    icon: 'fas fa-poll',
    isRead: true,
    createdAt: new Date(Date.now() - 3 * 60 * 60 * 1000).toISOString(),
  },
  {
    id: '5',
    type: 'info',
    title: 'Document updated',
    message: '"HR Policy 2024" was updated by HR Team',
    link: '/documents/1',
    icon: 'fas fa-file-alt',
    isRead: true,
    createdAt: new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString(),
  },
]

export const notificationsHandlers = [
  // Get notifications
  http.get(`${API_BASE}/notifications`, async ({ request }) => {
    await delay(300)

    const url = new URL(request.url)
    const page = parseInt(url.searchParams.get('page') || '1')
    const pageSize = parseInt(url.searchParams.get('pageSize') || '20')

    const response: PaginatedResponse<Notification> = {
      items: mockNotifications,
      total: mockNotifications.length,
      page,
      pageSize,
      totalPages: 1,
      hasNext: false,
      hasPrevious: false,
    }

    return HttpResponse.json(response)
  }),

  // Get unread count
  http.get(`${API_BASE}/notifications/unread-count`, async () => {
    await delay(100)
    const count = mockNotifications.filter(n => !n.isRead).length
    return HttpResponse.json({ count })
  }),

  // Mark as read
  http.post(`${API_BASE}/notifications/:id/read`, async ({ params }) => {
    await delay(200)
    const notif = mockNotifications.find(n => n.id === params.id)
    if (notif) {
      notif.isRead = true
    }
    return HttpResponse.json({ success: true })
  }),

  // Mark all as read
  http.post(`${API_BASE}/notifications/read-all`, async () => {
    await delay(300)
    mockNotifications.forEach(n => n.isRead = true)
    return HttpResponse.json({ success: true })
  }),
]
