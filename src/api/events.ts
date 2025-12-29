import apiClient from './client'
import type { CalendarEvent, PaginatedResponse, PaginationParams } from '@/types'

const CALENDAR_BASE = '/calendar'

export interface EventFilters extends PaginationParams {
  type?: string
  status?: string
  startDate?: string
  endDate?: string
}

export const eventsApi = {
  // Get all events
  async getEvents(filters?: EventFilters): Promise<PaginatedResponse<CalendarEvent>> {
    const response = await apiClient.get<PaginatedResponse<CalendarEvent>>(`${CALENDAR_BASE}/events`, {
      params: filters,
    })
    return response.data
  },

  // Get single event
  async getEvent(id: string): Promise<CalendarEvent> {
    const response = await apiClient.get<CalendarEvent>(`${CALENDAR_BASE}/events/${id}`)
    return response.data
  },

  // Create event
  async createEvent(data: Partial<CalendarEvent>): Promise<CalendarEvent> {
    const response = await apiClient.post<CalendarEvent>(`${CALENDAR_BASE}/events`, data)
    return response.data
  },

  // Update event
  async updateEvent(id: string, data: Partial<CalendarEvent>): Promise<CalendarEvent> {
    const response = await apiClient.put<CalendarEvent>(`${CALENDAR_BASE}/events/${id}`, data)
    return response.data
  },

  // Delete event
  async deleteEvent(id: string): Promise<void> {
    await apiClient.delete(`${CALENDAR_BASE}/events/${id}`)
  },

  // RSVP to event
  async rsvpEvent(id: string, status: 'accepted' | 'declined' | 'tentative'): Promise<void> {
    await apiClient.post(`${CALENDAR_BASE}/events/${id}/rsvp`, { status })
  },

  // Get upcoming events
  async getUpcomingEvents(limit?: number): Promise<CalendarEvent[]> {
    const response = await apiClient.get<CalendarEvent[]>(`${CALENDAR_BASE}/events/upcoming`, {
      params: { limit },
    })
    return response.data
  },

  // Create Teams meeting
  async createTeamsMeeting(eventId: string): Promise<{ teamsLink: string }> {
    const response = await apiClient.post<{ teamsLink: string }>(
      `${CALENDAR_BASE}/events/${eventId}/teams-meeting`
    )
    return response.data
  },
}

export default eventsApi
