import { describe, it, expect, vi, beforeEach } from 'vitest'
import { eventsApi } from '@/api/events'
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

describe('Events API', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('getEvents', () => {
    it('should fetch events without filters', async () => {
      const mockResponse = {
        items: [
          { id: '1', title: 'Event 1', start: '2027-01-01' },
          { id: '2', title: 'Event 2', start: '2027-01-15' },
        ],
        totalCount: 2,
        page: 1,
        pageSize: 10,
      }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockResponse })

      const result = await eventsApi.getEvents()

      expect(apiClient.get).toHaveBeenCalledWith('/calendar/events', {
        params: undefined,
      })
      expect(result).toEqual(mockResponse)
    })

    it('should fetch events with type filter', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: { items: [], totalCount: 0 } })

      await eventsApi.getEvents({ type: 'meeting' })

      expect(apiClient.get).toHaveBeenCalledWith('/calendar/events', {
        params: { type: 'meeting' },
      })
    })

    it('should fetch events with status filter', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: { items: [], totalCount: 0 } })

      await eventsApi.getEvents({ status: 'confirmed' })

      expect(apiClient.get).toHaveBeenCalledWith('/calendar/events', {
        params: { status: 'confirmed' },
      })
    })

    it('should fetch events with date range', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: { items: [], totalCount: 0 } })

      await eventsApi.getEvents({
        startDate: '2027-01-01',
        endDate: '2027-01-31',
      })

      expect(apiClient.get).toHaveBeenCalledWith('/calendar/events', {
        params: { startDate: '2027-01-01', endDate: '2027-01-31' },
      })
    })

    it('should fetch events with pagination', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: { items: [], totalCount: 0 } })

      await eventsApi.getEvents({ page: 2, pageSize: 20 })

      expect(apiClient.get).toHaveBeenCalledWith('/calendar/events', {
        params: { page: 2, pageSize: 20 },
      })
    })
  })

  describe('getEvent', () => {
    it('should fetch single event by ID', async () => {
      const mockEvent = {
        id: 'event-123',
        title: 'Test Event',
        start: '2027-01-01T10:00:00Z',
        end: '2027-01-01T12:00:00Z',
      }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockEvent })

      const result = await eventsApi.getEvent('event-123')

      expect(apiClient.get).toHaveBeenCalledWith('/calendar/events/event-123')
      expect(result).toEqual(mockEvent)
    })
  })

  describe('createEvent', () => {
    it('should create new event', async () => {
      const eventData = {
        title: 'New Event',
        start: '2027-02-01T09:00:00Z',
        end: '2027-02-01T10:00:00Z',
        description: 'Event description',
      }
      const mockResponse = { id: 'new-event', ...eventData }
      vi.mocked(apiClient.post).mockResolvedValue({ data: mockResponse })

      const result = await eventsApi.createEvent(eventData)

      expect(apiClient.post).toHaveBeenCalledWith('/calendar/events', eventData)
      expect(result).toEqual(mockResponse)
    })

    it('should create event with minimal data', async () => {
      const eventData = { title: 'Quick Meeting' }
      vi.mocked(apiClient.post).mockResolvedValue({ data: { id: '1', ...eventData } })

      await eventsApi.createEvent(eventData)

      expect(apiClient.post).toHaveBeenCalledWith('/calendar/events', eventData)
    })
  })

  describe('updateEvent', () => {
    it('should update existing event', async () => {
      const updateData = { title: 'Updated Event Title' }
      const mockResponse = { id: 'event-123', title: 'Updated Event Title' }
      vi.mocked(apiClient.put).mockResolvedValue({ data: mockResponse })

      const result = await eventsApi.updateEvent('event-123', updateData)

      expect(apiClient.put).toHaveBeenCalledWith('/calendar/events/event-123', updateData)
      expect(result).toEqual(mockResponse)
    })

    it('should update event time', async () => {
      const updateData = {
        start: '2027-02-15T14:00:00Z',
        end: '2027-02-15T16:00:00Z',
      }
      vi.mocked(apiClient.put).mockResolvedValue({ data: { id: 'event-123', ...updateData } })

      await eventsApi.updateEvent('event-123', updateData)

      expect(apiClient.put).toHaveBeenCalledWith('/calendar/events/event-123', updateData)
    })
  })

  describe('deleteEvent', () => {
    it('should delete event', async () => {
      vi.mocked(apiClient.delete).mockResolvedValue({ data: {} })

      await eventsApi.deleteEvent('event-123')

      expect(apiClient.delete).toHaveBeenCalledWith('/calendar/events/event-123')
    })
  })

  describe('rsvpEvent', () => {
    it('should RSVP accepted to event', async () => {
      vi.mocked(apiClient.post).mockResolvedValue({ data: {} })

      await eventsApi.rsvpEvent('event-123', 'accepted')

      expect(apiClient.post).toHaveBeenCalledWith('/calendar/events/event-123/rsvp', {
        status: 'accepted',
      })
    })

    it('should RSVP declined to event', async () => {
      vi.mocked(apiClient.post).mockResolvedValue({ data: {} })

      await eventsApi.rsvpEvent('event-123', 'declined')

      expect(apiClient.post).toHaveBeenCalledWith('/calendar/events/event-123/rsvp', {
        status: 'declined',
      })
    })

    it('should RSVP tentative to event', async () => {
      vi.mocked(apiClient.post).mockResolvedValue({ data: {} })

      await eventsApi.rsvpEvent('event-123', 'tentative')

      expect(apiClient.post).toHaveBeenCalledWith('/calendar/events/event-123/rsvp', {
        status: 'tentative',
      })
    })
  })

  describe('getUpcomingEvents', () => {
    it('should fetch upcoming events without limit', async () => {
      const mockEvents = [
        { id: '1', title: 'Upcoming 1' },
        { id: '2', title: 'Upcoming 2' },
      ]
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockEvents })

      const result = await eventsApi.getUpcomingEvents()

      expect(apiClient.get).toHaveBeenCalledWith('/calendar/events/upcoming', {
        params: { limit: undefined },
      })
      expect(result).toEqual(mockEvents)
    })

    it('should fetch upcoming events with limit', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: [] })

      await eventsApi.getUpcomingEvents(5)

      expect(apiClient.get).toHaveBeenCalledWith('/calendar/events/upcoming', {
        params: { limit: 5 },
      })
    })
  })

  describe('createTeamsMeeting', () => {
    it('should create Teams meeting for event', async () => {
      const mockResponse = { teamsLink: 'https://teams.microsoft.com/l/meetup-join/...' }
      vi.mocked(apiClient.post).mockResolvedValue({ data: mockResponse })

      const result = await eventsApi.createTeamsMeeting('event-123')

      expect(apiClient.post).toHaveBeenCalledWith('/calendar/events/event-123/teams-meeting')
      expect(result).toEqual(mockResponse)
    })
  })
})
