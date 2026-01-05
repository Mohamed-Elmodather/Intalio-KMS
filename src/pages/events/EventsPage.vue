<script setup lang="ts">
import { ref, computed } from 'vue'

// State
const showCreateModal = ref(false)
const showFilters = ref(false)
const calendarView = ref<'calendar' | 'grid' | 'list'>('calendar')
const currentDate = ref(new Date())
const searchQuery = ref('')
const selectedTypes = ref<string[]>([])
const selectedFormats = ref<string[]>([])

const weekDays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

const eventTypes = ref([
  { id: 'meeting', name: 'Meetings', icon: 'fas fa-users', color: '#3b82f6' },
  { id: 'training', name: 'Training', icon: 'fas fa-chalkboard-teacher', color: '#10b981' },
  { id: 'social', name: 'Social', icon: 'fas fa-glass-cheers', color: '#ec4899' },
  { id: 'review', name: 'Reviews', icon: 'fas fa-clipboard-check', color: '#f59e0b' },
  { id: 'webinar', name: 'Webinars', icon: 'fas fa-video', color: '#6366f1' },
])

interface Attendee {
  color: string
  initials: string
}

interface Event {
  id: number
  title: string
  date: string
  time: string
  location: string
  category: string
  categoryLabel: string
  virtual: boolean
  isGoing: boolean
  description: string
  attendees: Attendee[]
}

const events = ref<Event[]>([
  { id: 1, title: 'All-Hands Meeting Q4', date: '2025-12-24', time: '10:00 AM - 11:30 AM', location: 'Main Conference Room', category: 'meeting', categoryLabel: 'Meeting', virtual: true, isGoing: true, description: 'Quarterly all-hands meeting to discuss company progress and goals.', attendees: [{ color: '#14b8a6', initials: 'AI' }, { color: '#3b82f6', initials: 'SC' }, { color: '#ec4899', initials: 'LW' }] },
  { id: 2, title: 'Product Demo - New Features', date: '2025-12-26', time: '2:00 PM - 3:00 PM', location: 'Virtual - Zoom', category: 'webinar', categoryLabel: 'Webinar', virtual: true, isGoing: false, description: 'Demo of the latest product features to the team.', attendees: [{ color: '#6366f1', initials: 'DK' }, { color: '#0d9488', initials: 'MJ' }] },
  { id: 3, title: 'Year-End Performance Review', date: '2025-12-30', time: '9:00 AM - 12:00 PM', location: 'Board Room', category: 'review', categoryLabel: 'Review', virtual: false, isGoing: true, description: 'Annual performance review meeting.', attendees: [{ color: '#f59e0b', initials: 'HR' }] },
  { id: 4, title: 'Holiday Team Lunch', date: '2025-12-24', time: '12:30 PM - 2:00 PM', location: 'Cafeteria', category: 'social', categoryLabel: 'Social', virtual: false, isGoing: true, description: 'Holiday celebration lunch with the team.', attendees: [{ color: '#ec4899', initials: 'TM' }, { color: '#14b8a6', initials: 'AI' }, { color: '#3b82f6', initials: 'SC' }, { color: '#10b981', initials: 'ED' }] },
  { id: 5, title: 'Advanced Excel Training', date: '2025-12-27', time: '3:00 PM - 5:00 PM', location: 'Training Room A', category: 'training', categoryLabel: 'Training', virtual: false, isGoing: false, description: 'Advanced Excel techniques for productivity.', attendees: [{ color: '#10b981', initials: 'RG' }, { color: '#0d9488', initials: 'AT' }] },
  { id: 6, title: 'Sprint Planning Meeting', date: '2025-12-25', time: '10:00 AM - 11:00 AM', location: 'Scrum Room', category: 'meeting', categoryLabel: 'Meeting', virtual: false, isGoing: true, description: 'Planning for the upcoming sprint.', attendees: [{ color: '#3b82f6', initials: 'PM' }, { color: '#14b8a6', initials: 'AI' }, { color: '#6366f1', initials: 'DK' }] },
  { id: 7, title: 'Customer Success Webinar', date: '2025-12-28', time: '1:00 PM - 2:30 PM', location: 'Virtual - Teams', category: 'webinar', categoryLabel: 'Webinar', virtual: true, isGoing: false, description: 'Webinar on customer success strategies.', attendees: [{ color: '#6366f1', initials: 'CS' }, { color: '#ec4899', initials: 'LW' }] },
  { id: 8, title: 'New Year Planning Session', date: '2025-12-31', time: '2:00 PM - 4:00 PM', location: 'Executive Suite', category: 'meeting', categoryLabel: 'Meeting', virtual: false, isGoing: true, description: 'Strategic planning for the new year.', attendees: [{ color: '#14b8a6', initials: 'AI' }, { color: '#0d9488', initials: 'CEO' }] },
  { id: 9, title: 'Team Building Workshop', date: '2025-12-29', time: '9:00 AM - 12:00 PM', location: 'Innovation Lab', category: 'social', categoryLabel: 'Social', virtual: false, isGoing: false, description: 'Interactive team building exercises.', attendees: [{ color: '#ec4899', initials: 'HR' }, { color: '#10b981', initials: 'ED' }, { color: '#3b82f6', initials: 'SC' }] },
  { id: 10, title: 'Security Awareness Training', date: '2025-12-26', time: '10:00 AM - 11:00 AM', location: 'Virtual - Webex', category: 'training', categoryLabel: 'Training', virtual: true, isGoing: true, description: 'Mandatory security training for all employees.', attendees: [{ color: '#10b981', initials: 'IT' }, { color: '#f59e0b', initials: 'SEC' }] },
])

const newEvent = ref({
  title: '',
  startDate: '',
  endDate: '',
  description: '',
  location: '',
  category: 'meeting',
  virtual: false,
  allDay: false
})

// Computed properties
const eventStats = computed(() => ({
  total: events.value.length,
  thisWeek: events.value.filter(e => {
    const eventDate = new Date(e.date)
    const today = new Date()
    const weekFromNow = new Date(today.getTime() + 7 * 24 * 60 * 60 * 1000)
    return eventDate >= today && eventDate <= weekFromNow
  }).length,
  yourRsvps: events.value.filter(e => e.isGoing).length
}))

const todayEventsCount = computed(() => {
  const today = new Date().toISOString().split('T')[0]
  return events.value.filter(e => e.date === today).length
})

const featuredEvent = computed(() => {
  const upcoming = events.value
    .filter(e => new Date(e.date) >= new Date())
    .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())[0]
  if (upcoming) {
    const d = new Date(upcoming.date)
    return {
      ...upcoming,
      month: d.toLocaleString('default', { month: 'short' }).toUpperCase(),
      day: d.getDate(),
      dateFormatted: d.toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric' })
    }
  }
  return null
})

const currentMonthName = computed(() => currentDate.value.toLocaleString('default', { month: 'long' }))
const currentYear = computed(() => currentDate.value.getFullYear())

const activeFiltersCount = computed(() => selectedTypes.value.length + selectedFormats.value.length)

const filteredEvents = computed(() => {
  let result = events.value.map(e => {
    const d = new Date(e.date)
    return {
      ...e,
      month: d.toLocaleString('default', { month: 'short' }).toUpperCase(),
      day: d.getDate()
    }
  })

  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    result = result.filter(e => e.title.toLowerCase().includes(q) || e.location.toLowerCase().includes(q))
  }

  if (selectedTypes.value.length > 0) {
    result = result.filter(e => selectedTypes.value.includes(e.category))
  }

  if (selectedFormats.value.length > 0) {
    result = result.filter(e => {
      if (selectedFormats.value.includes('virtual') && e.virtual) return true
      if (selectedFormats.value.includes('in-person') && !e.virtual) return true
      return false
    })
  }

  return result.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
})

interface CalendarDay {
  date: number
  isCurrentMonth: boolean
  isToday: boolean
  events: Event[]
}

const calendarDays = computed<CalendarDay[]>(() => {
  const days: CalendarDay[] = []
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()
  const firstDay = new Date(year, month, 1)
  const lastDay = new Date(year, month + 1, 0)
  const startPadding = firstDay.getDay()
  const today = new Date()

  const prevMonthLastDay = new Date(year, month, 0).getDate()
  for (let i = startPadding - 1; i >= 0; i--) {
    days.push({ date: prevMonthLastDay - i, isCurrentMonth: false, isToday: false, events: [] })
  }

  for (let d = 1; d <= lastDay.getDate(); d++) {
    const dateStr = `${year}-${String(month + 1).padStart(2, '0')}-${String(d).padStart(2, '0')}`
    const dayEvents = events.value.filter(e => e.date === dateStr)
    days.push({
      date: d,
      isCurrentMonth: true,
      isToday: today.getDate() === d && today.getMonth() === month && today.getFullYear() === year,
      events: dayEvents
    })
  }

  const remaining = 42 - days.length
  for (let d = 1; d <= remaining; d++) {
    days.push({ date: d, isCurrentMonth: false, isToday: false, events: [] })
  }

  return days
})

const miniCalendarDays = computed(() => calendarDays.value.map(d => ({ ...d, hasEvents: d.events.length > 0 })))

const upcomingEvents = computed(() => {
  const today = new Date()
  return events.value
    .filter(e => new Date(e.date) >= today)
    .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
    .slice(0, 5)
    .map(e => {
      const d = new Date(e.date)
      return { ...e, month: d.toLocaleString('default', { month: 'short' }).toUpperCase(), day: d.getDate() }
    })
})

// Methods
function previousMonth() {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() - 1, 1)
}

function nextMonth() {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() + 1, 1)
}

function goToToday() {
  currentDate.value = new Date()
}

function selectDate(day: CalendarDay) {
  console.log('Selected date:', day)
}

function selectMiniCalendarDay(day: CalendarDay & { hasEvents: boolean }) {
  console.log('Mini calendar day:', day)
}

function openEvent(event: Event & { month?: string; day?: number }) {
  // Navigate to event detail page using router
  console.log('Opening event:', event.id)
}

function createEvent() {
  console.log('Creating event:', newEvent.value)
  showCreateModal.value = false
}

function toggleTypeFilter(typeId: string) {
  const idx = selectedTypes.value.indexOf(typeId)
  if (idx > -1) selectedTypes.value.splice(idx, 1)
  else selectedTypes.value.push(typeId)
}

function toggleFormatFilter(format: string) {
  const idx = selectedFormats.value.indexOf(format)
  if (idx > -1) selectedFormats.value.splice(idx, 1)
  else selectedFormats.value.push(format)
}

function clearFilters() {
  selectedTypes.value = []
  selectedFormats.value = []
  searchQuery.value = ''
}

function filterToday() {
  calendarView.value = 'list'
}

function filterMyEvents() {
  calendarView.value = 'list'
}

function getEventTypeIcon(category: string) {
  const type = eventTypes.value.find(t => t.id === category)
  return type ? type.icon : 'fas fa-calendar'
}

function getCategoryCount(categoryId: string) {
  return events.value.filter(e => e.category === categoryId).length
}
</script>

<template>
  <div class="events-page">
    <!-- Page Hero Section -->
    <div class="hero-section fade-in-up">
      <div class="hero-content">
        <div class="hero-left">
          <div class="hero-header">
            <div class="hero-icon">
              <i class="fas fa-calendar-alt"></i>
            </div>
            <div>
              <h1 class="hero-title"><span class="hero-title-highlight">Events</span> & Calendar</h1>
              <p class="hero-subtitle">Stay connected with company events, meetings, and activities</p>
            </div>
          </div>
        </div>
        <div class="hero-stats">
          <div class="stat-card-hero">
            <div class="stat-card-hero-icon teal">
              <i class="fas fa-calendar-alt"></i>
            </div>
            <div class="stat-card-hero-content">
              <p class="stat-card-hero-value">{{ eventStats.total }}</p>
              <p class="stat-card-hero-label">Total Events</p>
            </div>
          </div>
          <div class="stat-card-hero">
            <div class="stat-card-hero-icon blue">
              <i class="fas fa-calendar-week"></i>
            </div>
            <div class="stat-card-hero-content">
              <p class="stat-card-hero-value">{{ eventStats.thisWeek }}</p>
              <p class="stat-card-hero-label">This Week</p>
            </div>
          </div>
          <div class="stat-card-hero">
            <div class="stat-card-hero-icon teal">
              <i class="fas fa-check-circle"></i>
            </div>
            <div class="stat-card-hero-content">
              <p class="stat-card-hero-value">{{ eventStats.yourRsvps }}</p>
              <p class="stat-card-hero-label">Your RSVPs</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content Area -->
    <div class="main-content">

      <!-- Quick Actions -->
      <div class="quick-actions">
        <div class="quick-action-card" @click="filterToday">
          <div class="quick-action-icon today">
            <i class="fas fa-sun"></i>
          </div>
          <div class="quick-action-text">
            <h4>Today's Events</h4>
            <p>{{ todayEventsCount }} events today</p>
          </div>
        </div>
        <div class="quick-action-card" @click="calendarView = 'list'">
          <div class="quick-action-icon upcoming">
            <i class="fas fa-clock"></i>
          </div>
          <div class="quick-action-text">
            <h4>Upcoming</h4>
            <p>Next 7 days</p>
          </div>
        </div>
        <div class="quick-action-card" @click="filterMyEvents">
          <div class="quick-action-icon my-events">
            <i class="fas fa-user-check"></i>
          </div>
          <div class="quick-action-text">
            <h4>My Events</h4>
            <p>RSVP'd events</p>
          </div>
        </div>
        <div class="quick-action-card" @click="showCreateModal = true">
          <div class="quick-action-icon create">
            <i class="fas fa-plus"></i>
          </div>
          <div class="quick-action-text">
            <h4>Create Event</h4>
            <p>Schedule new</p>
          </div>
        </div>
      </div>

      <!-- Featured Event -->
      <div v-if="featuredEvent" class="featured-event">
        <div class="featured-badge">
          <i class="fas fa-star"></i>
          Featured Event
        </div>
        <div class="flex items-start gap-6">
          <div class="flex-1">
            <h2 class="text-xl font-bold text-gray-900 mb-2">{{ featuredEvent.title }}</h2>
            <p class="text-gray-600 mb-4">{{ featuredEvent.description }}</p>
            <div class="flex flex-wrap gap-4 text-sm text-gray-600">
              <span class="flex items-center gap-2">
                <i class="fas fa-calendar text-teal-500"></i>
                {{ featuredEvent.dateFormatted }}
              </span>
              <span class="flex items-center gap-2">
                <i class="fas fa-clock text-teal-500"></i>
                {{ featuredEvent.time }}
              </span>
              <span class="flex items-center gap-2">
                <i class="fas fa-map-marker-alt text-teal-500"></i>
                {{ featuredEvent.location }}
              </span>
              <span v-if="featuredEvent.virtual" class="flex items-center gap-2">
                <i class="fas fa-video text-teal-500"></i>
                Virtual
              </span>
            </div>
          </div>
          <div class="flex flex-col items-end gap-3">
            <div class="event-date-badge">
              <span class="event-date-month">{{ featuredEvent.month }}</span>
              <span class="event-date-day">{{ featuredEvent.day }}</span>
            </div>
            <button class="rsvp-btn pending">
              <i class="fas fa-check mr-1"></i> RSVP Now
            </button>
          </div>
        </div>
      </div>

      <!-- Toolbar -->
      <div class="toolbar">
        <div class="toolbar-left">
          <div class="search-box">
            <i class="fas fa-search"></i>
            <input type="text" v-model="searchQuery" placeholder="Search events..." class="search-input">
          </div>
          <button @click="showFilters = !showFilters"
                  :class="['filter-btn', { active: showFilters || activeFiltersCount > 0 }]">
            <i class="fas fa-filter"></i>
            <span>Filters</span>
            <span v-if="activeFiltersCount > 0" class="filter-badge">{{ activeFiltersCount }}</span>
          </button>
        </div>

        <div class="toolbar-right">
          <div class="view-toggle">
            <button @click="calendarView = 'calendar'" :class="{ active: calendarView === 'calendar' }">
              <i class="fas fa-calendar"></i> Calendar
            </button>
            <button @click="calendarView = 'grid'" :class="{ active: calendarView === 'grid' }">
              <i class="fas fa-th-large"></i> Grid
            </button>
            <button @click="calendarView = 'list'" :class="{ active: calendarView === 'list' }">
              <i class="fas fa-list"></i> List
            </button>
          </div>
        </div>
      </div>

      <!-- Filters Panel -->
      <div v-if="showFilters" class="filters-panel">
        <div class="filter-groups">
          <div class="filter-group">
            <div class="filter-group-label">Event Type</div>
            <div class="filter-chips">
              <button v-for="type in eventTypes" :key="type.id"
                      @click="toggleTypeFilter(type.id)"
                      :class="['filter-chip', { active: selectedTypes.includes(type.id) }]">
                <i :class="type.icon"></i>
                {{ type.name }}
              </button>
            </div>
          </div>
          <div class="filter-group">
            <div class="filter-group-label">Format</div>
            <div class="filter-chips">
              <button @click="toggleFormatFilter('virtual')"
                      :class="['filter-chip', { active: selectedFormats.includes('virtual') }]">
                <i class="fas fa-video"></i> Virtual
              </button>
              <button @click="toggleFormatFilter('in-person')"
                      :class="['filter-chip', { active: selectedFormats.includes('in-person') }]">
                <i class="fas fa-building"></i> In-Person
              </button>
              <button @click="toggleFormatFilter('hybrid')"
                      :class="['filter-chip', { active: selectedFormats.includes('hybrid') }]">
                <i class="fas fa-laptop-house"></i> Hybrid
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Content with Sidebar -->
      <div class="flex gap-6 content-with-sidebar">
        <!-- Main Content -->
        <div class="flex-1 min-w-0">

          <!-- Calendar View -->
          <div v-if="calendarView === 'calendar'" class="calendar-container">
            <div class="calendar-header">
              <div class="calendar-nav">
                <button @click="previousMonth" class="calendar-nav-btn">
                  <i class="fas fa-chevron-left"></i>
                </button>
                <h2 class="calendar-title">{{ currentMonthName }} {{ currentYear }}</h2>
                <button @click="nextMonth" class="calendar-nav-btn">
                  <i class="fas fa-chevron-right"></i>
                </button>
              </div>
              <button @click="goToToday" class="calendar-today-btn">Today</button>
            </div>

            <div class="calendar-weekdays">
              <div v-for="day in weekDays" :key="day" class="calendar-weekday">{{ day }}</div>
            </div>

            <div class="calendar-grid">
              <div v-for="(day, index) in calendarDays" :key="index"
                   :class="['calendar-day',
                            day.isToday ? 'today' : '',
                            !day.isCurrentMonth ? 'other-month' : '']"
                   @click="selectDate(day)">
                <span class="day-number">{{ day.date }}</span>
                <div class="day-events">
                  <div v-for="event in day.events.slice(0, 3)" :key="event.id"
                       @click.stop="openEvent(event)"
                       :class="['day-event', event.category]">
                    {{ event.title }}
                  </div>
                  <div v-if="day.events.length > 3" class="day-more">
                    +{{ day.events.length - 3 }} more
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Grid View -->
          <div v-if="calendarView === 'grid'" class="events-grid">
            <div v-for="event in filteredEvents" :key="event.id"
                 @click="openEvent(event)"
                 class="event-card">
              <div class="event-card-header">
                <span :class="['event-type-badge', event.category]">
                  <i :class="getEventTypeIcon(event.category)"></i>
                  {{ event.categoryLabel }}
                </span>
                <div class="event-date-badge">
                  <span class="event-date-month">{{ event.month }}</span>
                  <span class="event-date-day">{{ event.day }}</span>
                </div>
              </div>
              <div class="event-card-body">
                <h3 class="event-card-title">{{ event.title }}</h3>
                <div class="event-card-meta">
                  <div class="event-meta-item">
                    <i class="fas fa-clock"></i>
                    <span>{{ event.time }}</span>
                  </div>
                  <div class="event-meta-item">
                    <i :class="event.virtual ? 'fas fa-video' : 'fas fa-map-marker-alt'"></i>
                    <span>{{ event.location }}</span>
                  </div>
                </div>
                <div class="event-card-footer">
                  <div class="event-attendees">
                    <div class="attendee-avatars">
                      <div v-for="(attendee, i) in event.attendees.slice(0, 3)" :key="i"
                           class="attendee-avatar"
                           :style="{ backgroundColor: attendee.color }">
                        {{ attendee.initials }}
                      </div>
                    </div>
                    <span class="attendee-count">{{ event.attendees.length }} going</span>
                  </div>
                  <button :class="['rsvp-btn', event.isGoing ? 'going' : 'pending']">
                    {{ event.isGoing ? 'Going' : 'RSVP' }}
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- List View -->
          <div v-if="calendarView === 'list'" class="events-list">
            <div v-for="event in filteredEvents" :key="event.id"
                 @click="openEvent(event)"
                 class="event-list-item">
              <div class="event-list-date">
                <span class="text-[10px] font-bold text-teal-600 uppercase">{{ event.month }}</span>
                <span class="text-xl font-extrabold text-gray-900">{{ event.day }}</span>
              </div>
              <div class="event-list-info">
                <h4 class="event-list-title">{{ event.title }}</h4>
                <div class="event-list-details">
                  <span :class="['event-type-badge', event.category]">
                    {{ event.categoryLabel }}
                  </span>
                  <span class="flex items-center gap-1">
                    <i class="fas fa-clock text-teal-500"></i>
                    {{ event.time }}
                  </span>
                  <span class="flex items-center gap-1">
                    <i :class="[event.virtual ? 'fas fa-video' : 'fas fa-map-marker-alt', 'text-teal-500']"></i>
                    {{ event.location }}
                  </span>
                </div>
              </div>
              <div class="event-list-actions">
                <div class="attendee-avatars mr-3">
                  <div v-for="(attendee, i) in event.attendees.slice(0, 3)" :key="i"
                       class="attendee-avatar"
                       :style="{ backgroundColor: attendee.color }">
                    {{ attendee.initials }}
                  </div>
                </div>
                <button :class="['rsvp-btn', event.isGoing ? 'going' : 'pending']">
                  {{ event.isGoing ? 'Going' : 'RSVP' }}
                </button>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <div v-if="filteredEvents.length === 0 && calendarView !== 'calendar'" class="text-center py-12">
            <div class="w-16 h-16 mx-auto mb-4 bg-gray-100 rounded-2xl flex items-center justify-center">
              <i class="fas fa-calendar-xmark text-2xl text-gray-400"></i>
            </div>
            <h3 class="text-lg font-semibold text-gray-900 mb-2">No events found</h3>
            <p class="text-gray-600 mb-4">Try adjusting your filters or search query</p>
            <button @click="clearFilters" class="text-teal-600 font-medium hover:text-teal-700">
              Clear all filters
            </button>
          </div>
        </div>

        <!-- Sidebar -->
        <aside class="w-80 flex-shrink-0 content-sidebar hidden xl:block">
          <!-- Mini Calendar -->
          <div class="sidebar-section">
            <div class="sidebar-title">
              <i class="fas fa-calendar-alt"></i>
              {{ currentMonthName }} {{ currentYear }}
            </div>
            <div class="mini-calendar">
              <div v-for="d in ['S','M','T','W','T','F','S']" :key="d" class="mini-calendar-day header">{{ d }}</div>
              <div v-for="(day, i) in miniCalendarDays" :key="i"
                   :class="['mini-calendar-day',
                            day.isToday ? 'today' : '',
                            day.hasEvents ? 'has-event' : '',
                            !day.isCurrentMonth ? 'other-month' : '']"
                   @click="selectMiniCalendarDay(day)">
                {{ day.date }}
              </div>
            </div>
          </div>

          <!-- Upcoming Events -->
          <div class="sidebar-section">
            <div class="sidebar-title">
              <i class="fas fa-clock"></i>
              Upcoming Events
            </div>
            <div class="space-y-1">
              <div v-for="event in upcomingEvents" :key="event.id"
                   @click="openEvent(event)"
                   class="upcoming-event-item">
                <div class="upcoming-event-date">
                  <span>{{ event.month }}</span>
                  <span>{{ event.day }}</span>
                </div>
                <div class="upcoming-event-info">
                  <h4>{{ event.title }}</h4>
                  <p>{{ event.time }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Categories -->
          <div class="sidebar-section">
            <div class="sidebar-title">
              <i class="fas fa-tags"></i>
              Categories
            </div>
            <div class="space-y-1">
              <div v-for="cat in eventTypes" :key="cat.id"
                   @click="toggleTypeFilter(cat.id)"
                   :class="['category-item', { 'bg-teal-50': selectedTypes.includes(cat.id) }]">
                <span class="category-dot" :style="{ backgroundColor: cat.color }"></span>
                <span class="category-name">{{ cat.name }}</span>
                <span class="category-count">{{ getCategoryCount(cat.id) }}</span>
              </div>
            </div>
          </div>
        </aside>
      </div>

    </div>

    <!-- Create Event Modal -->
    <Teleport to="body">
      <div v-if="showCreateModal" class="modal-overlay" @click.self="showCreateModal = false">
        <div class="modal-content">
          <div class="modal-header">
            <h3 class="modal-title">Create Event</h3>
            <button @click="showCreateModal = false" class="modal-close">
              <i class="fas fa-times"></i>
            </button>
          </div>
          <div class="modal-body">
            <div class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Event Title</label>
                <input type="text" v-model="newEvent.title" placeholder="Enter event title..."
                       class="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-500 focus:border-teal-500">
              </div>
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">Start Date & Time</label>
                  <input type="datetime-local" v-model="newEvent.startDate"
                         class="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-500 focus:border-teal-500">
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">End Date & Time</label>
                  <input type="datetime-local" v-model="newEvent.endDate"
                         class="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-500 focus:border-teal-500">
                </div>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Description</label>
                <textarea v-model="newEvent.description" placeholder="Event description..." rows="3"
                          class="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-500 focus:border-teal-500"></textarea>
              </div>
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">Location</label>
                  <input type="text" v-model="newEvent.location" placeholder="Enter location..."
                         class="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-500 focus:border-teal-500">
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">Category</label>
                  <select v-model="newEvent.category"
                          class="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-500 focus:border-teal-500">
                    <option v-for="cat in eventTypes" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
                  </select>
                </div>
              </div>
              <div class="flex items-center gap-6">
                <label class="flex items-center gap-2 cursor-pointer">
                  <input type="checkbox" v-model="newEvent.virtual" class="w-4 h-4 text-teal-600 rounded focus:ring-teal-500">
                  <span class="text-sm text-gray-700">Virtual Event</span>
                </label>
                <label class="flex items-center gap-2 cursor-pointer">
                  <input type="checkbox" v-model="newEvent.allDay" class="w-4 h-4 text-teal-600 rounded focus:ring-teal-500">
                  <span class="text-sm text-gray-700">All Day</span>
                </label>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button @click="showCreateModal = false"
                    class="px-4 py-2 text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors">
              Cancel
            </button>
            <button @click="createEvent"
                    class="px-4 py-2 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-lg hover:from-teal-600 hover:to-teal-700 transition-all">
              <i class="fas fa-plus mr-2"></i> Create Event
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
/* Main Content */
.main-content {
  padding: 0;
}

/* Toolbar */
.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.25rem;
  gap: 1rem;
  flex-wrap: wrap;
}

.toolbar-left {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.toolbar-right {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

/* Search Box */
.search-box {
  position: relative;
  width: 300px;
}

.search-box i {
  position: absolute;
  left: 1rem;
  top: 50%;
  transform: translateY(-50%);
  color: #94a3b8;
  font-size: 0.875rem;
  transition: all 0.3s ease;
  z-index: 1;
}

.search-box:focus-within i {
  color: #14b8a6;
  transform: translateY(-50%) scale(1.1);
}

.search-input {
  width: 100%;
  height: 40px;
  padding-left: 2.75rem;
  padding-right: 1rem;
  border-radius: 0.75rem;
  border: 1.5px solid #e2e8f0;
  background: linear-gradient(135deg, #fafafa 0%, #f8fafc 100%);
  font-size: 0.875rem;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.search-input:focus {
  border-color: #14b8a6;
  box-shadow: 0 0 0 4px rgba(20, 184, 166, 0.1);
  background: white;
  outline: none;
}

/* Filter Button */
.filter-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: white;
  border: 1.5px solid #e2e8f0;
  border-radius: 0.75rem;
  font-size: 0.8125rem;
  font-weight: 600;
  color: #475569;
  cursor: pointer;
  transition: all 0.3s ease;
  height: 40px;
}

.filter-btn:hover,
.filter-btn.active {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-color: #14b8a6;
  color: #0d9488;
}

.filter-badge {
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 20px;
  height: 20px;
  padding: 0 6px;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.6875rem;
  font-weight: 700;
  border-radius: 9999px;
}

/* View Toggle */
.view-toggle {
  display: flex;
  background: #f1f5f9;
  border-radius: 0.5rem;
  padding: 0.1875rem;
}

.view-toggle button {
  padding: 0.5rem 0.75rem;
  border: none;
  background: transparent;
  color: #64748b;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: all 0.15s ease;
  font-size: 0.75rem;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 0.375rem;
}

.view-toggle button.active {
  background: white;
  color: #0d9488;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

/* Filters Panel */
.filters-panel {
  margin-bottom: 1.25rem;
  padding: 1rem;
  background: white;
  border-radius: 0.875rem;
  border: 1px solid #f1f5f9;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04);
  animation: slideDown 0.25s ease-out;
}

@keyframes slideDown {
  from { opacity: 0; transform: translateY(-8px); }
  to { opacity: 1; transform: translateY(0); }
}

.filter-groups {
  display: flex;
  flex-wrap: wrap;
  gap: 1.25rem;
}

.filter-group-label {
  font-size: 0.6875rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: #64748b;
  margin-bottom: 0.375rem;
}

.filter-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
}

.filter-chip {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 500;
  color: #475569;
  cursor: pointer;
  transition: all 0.2s ease;
}

.filter-chip:hover {
  background: #f0fdfa;
  border-color: #99f6e4;
  color: #0d9488;
}

.filter-chip.active {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-color: #14b8a6;
  color: #0d9488;
}

/* Featured Event Card */
.featured-event {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 50%, #e0f2fe 100%);
  border-radius: 1.25rem;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  position: relative;
  overflow: hidden;
  border: 1px solid rgba(20, 184, 166, 0.2);
}

.featured-event::before {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  width: 200px;
  height: 200px;
  background: radial-gradient(circle, rgba(20, 184, 166, 0.15) 0%, transparent 70%);
  pointer-events: none;
}

.featured-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
  font-size: 0.6875rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  border-radius: 9999px;
  margin-bottom: 0.75rem;
}

/* Calendar Styles */
.calendar-container {
  background: white;
  border-radius: 1rem;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(20, 184, 166, 0.08);
}

.calendar-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.25rem;
  background: linear-gradient(135deg, #f8fafc 0%, #f0fdfa 100%);
  border-bottom: 1px solid #f1f5f9;
}

.calendar-nav {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.calendar-nav-btn {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
}

.calendar-nav-btn:hover {
  background: #f0fdfa;
  border-color: #14b8a6;
  color: #0d9488;
}

.calendar-title {
  font-size: 1.125rem;
  font-weight: 700;
  color: #0f172a;
  min-width: 180px;
  text-align: center;
}

.calendar-today-btn {
  padding: 0.5rem 1rem;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  font-size: 0.8125rem;
  font-weight: 600;
  color: #475569;
  cursor: pointer;
  transition: all 0.2s ease;
}

.calendar-today-btn:hover {
  background: #f0fdfa;
  border-color: #14b8a6;
  color: #0d9488;
}

.calendar-weekdays {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  background: #f8fafc;
  border-bottom: 1px solid #f1f5f9;
}

.calendar-weekday {
  padding: 0.75rem;
  text-align: center;
  font-size: 0.75rem;
  font-weight: 600;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.calendar-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
}

.calendar-day {
  min-height: 100px;
  padding: 0.5rem;
  border-right: 1px solid #f1f5f9;
  border-bottom: 1px solid #f1f5f9;
  transition: all 0.2s ease;
  cursor: pointer;
}

.calendar-day:nth-child(7n) {
  border-right: none;
}

.calendar-day:hover {
  background: rgba(20, 184, 166, 0.05);
}

.calendar-day.today {
  background: linear-gradient(135deg, rgba(20, 184, 166, 0.08) 0%, rgba(20, 184, 166, 0.04) 100%);
}

.calendar-day.other-month {
  background: #fafafa;
}

.calendar-day.other-month .day-number {
  color: #cbd5e1;
}

.day-number {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  font-size: 0.8125rem;
  font-weight: 600;
  color: #334155;
  border-radius: 0.5rem;
  margin-bottom: 0.375rem;
}

.calendar-day.today .day-number {
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
}

.day-events {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.day-event {
  padding: 0.25rem 0.5rem;
  border-radius: 0.375rem;
  font-size: 0.6875rem;
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  cursor: pointer;
  transition: all 0.15s ease;
}

.day-event:hover {
  transform: scale(1.02);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.day-event.meeting { background: #dbeafe; color: #1e40af; }
.day-event.training { background: #d1fae5; color: #065f46; }
.day-event.social { background: #fce7f3; color: #9d174d; }
.day-event.review { background: #fef3c7; color: #92400e; }
.day-event.webinar { background: #e0e7ff; color: #3730a3; }

.day-more {
  font-size: 0.6875rem;
  color: #64748b;
  padding: 0.125rem 0.5rem;
  cursor: pointer;
}

.day-more:hover {
  color: #0d9488;
}

/* Event Cards Grid */
.events-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 1rem;
}

.event-card {
  background: white;
  border-radius: 1rem;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(20, 184, 166, 0.08);
  transition: all 0.3s ease;
  cursor: pointer;
}

.event-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.1);
  border-color: rgba(20, 184, 166, 0.2);
}

.event-card-header {
  position: relative;
  padding: 1rem;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-bottom: 1px solid rgba(20, 184, 166, 0.1);
}

.event-date-badge {
  position: absolute;
  top: 1rem;
  right: 1rem;
  width: 56px;
  height: 56px;
  background: white;
  border-radius: 0.75rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.event-date-month {
  font-size: 0.625rem;
  font-weight: 700;
  text-transform: uppercase;
  color: #0d9488;
  letter-spacing: 0.05em;
}

.event-date-day {
  font-size: 1.25rem;
  font-weight: 800;
  color: #0f172a;
  line-height: 1;
}

.event-type-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.625rem;
  border-radius: 9999px;
  font-size: 0.6875rem;
  font-weight: 600;
}

.event-type-badge.meeting { background: #dbeafe; color: #1e40af; }
.event-type-badge.training { background: #d1fae5; color: #065f46; }
.event-type-badge.social { background: #fce7f3; color: #9d174d; }
.event-type-badge.review { background: #fef3c7; color: #92400e; }
.event-type-badge.webinar { background: #e0e7ff; color: #3730a3; }

.event-card-body {
  padding: 1rem;
}

.event-card-title {
  font-size: 1rem;
  font-weight: 700;
  color: #0f172a;
  margin-bottom: 0.5rem;
  line-height: 1.4;
}

.event-card-meta {
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
  margin-bottom: 0.75rem;
}

.event-meta-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.8125rem;
  color: #64748b;
}

.event-meta-item i {
  width: 16px;
  color: #14b8a6;
}

.event-card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 0.75rem;
  border-top: 1px solid #f1f5f9;
}

.event-attendees {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.attendee-avatars {
  display: flex;
  margin-left: -8px;
}

.attendee-avatar {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  border: 2px solid white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.625rem;
  font-weight: 600;
  color: white;
  margin-left: -8px;
}

.attendee-count {
  font-size: 0.75rem;
  color: #64748b;
}

.rsvp-btn {
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  font-size: 0.75rem;
  font-weight: 600;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
}

.rsvp-btn.going {
  background: linear-gradient(135deg, #d1fae5, #a7f3d0);
  color: #065f46;
}

.rsvp-btn.pending {
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
}

.rsvp-btn:hover {
  transform: scale(1.05);
}

/* Event List View */
.events-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.event-list-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: white;
  border-radius: 0.875rem;
  border: 1px solid #f1f5f9;
  transition: all 0.2s ease;
  cursor: pointer;
}

.event-list-item:hover {
  border-color: rgba(20, 184, 166, 0.3);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  transform: translateX(4px);
}

.event-list-date {
  width: 60px;
  height: 60px;
  background: linear-gradient(135deg, #f0fdfa, #ccfbf1);
  border-radius: 0.75rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.event-list-info {
  flex: 1;
  min-width: 0;
}

.event-list-title {
  font-size: 0.9375rem;
  font-weight: 600;
  color: #0f172a;
  margin-bottom: 0.25rem;
}

.event-list-details {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  font-size: 0.8125rem;
  color: #64748b;
}

.event-list-actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

/* Quick Actions */
.quick-actions {
  display: flex;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
}

.quick-action-card {
  flex: 1;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  background: white;
  border-radius: 0.875rem;
  border: 1px solid #f1f5f9;
  cursor: pointer;
  transition: all 0.2s ease;
}

.quick-action-card:hover {
  border-color: #14b8a6;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.1);
  transform: translateY(-2px);
}

.quick-action-icon {
  width: 44px;
  height: 44px;
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.125rem;
}

.quick-action-icon.today { background: linear-gradient(135deg, #dbeafe, #bfdbfe); color: #1e40af; }
.quick-action-icon.upcoming { background: linear-gradient(135deg, #d1fae5, #a7f3d0); color: #065f46; }
.quick-action-icon.my-events { background: linear-gradient(135deg, #fce7f3, #fbcfe8); color: #9d174d; }
.quick-action-icon.create { background: linear-gradient(135deg, #ccfbf1, #99f6e4); color: #0d9488; }

.quick-action-text h4 {
  font-size: 0.875rem;
  font-weight: 600;
  color: #0f172a;
  margin: 0 0 0.125rem;
}

.quick-action-text p {
  font-size: 0.75rem;
  color: #64748b;
  margin: 0;
}

/* Sidebar */
.sidebar-section {
  background: white;
  border-radius: 1rem;
  padding: 1rem;
  margin-bottom: 1rem;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(20, 184, 166, 0.08);
}

.sidebar-title {
  font-size: 0.875rem;
  font-weight: 700;
  color: #0f172a;
  margin-bottom: 0.875rem;
  display: flex;
  align-items: center;
  gap: 0.625rem;
}

.sidebar-title i {
  width: 1.75rem;
  height: 1.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0.5rem;
  font-size: 0.75rem;
  transition: all 0.3s ease;
}

.sidebar-title i.fa-calendar-alt {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.25);
  color: #14b8a6;
}

.sidebar-title i.fa-clock {
  background: linear-gradient(135deg, #eef2ff 0%, #e0e7ff 100%);
  box-shadow: 0 2px 8px rgba(99, 102, 241, 0.25);
  color: #6366f1;
}

.sidebar-title i.fa-tags {
  background: linear-gradient(135deg, #faf5ff 0%, #f3e8ff 100%);
  box-shadow: 0 2px 8px rgba(168, 85, 247, 0.25);
  color: #a855f7;
}

.sidebar-section:hover .sidebar-title i {
  transform: scale(1.1) rotate(-5deg);
}

/* Mini Calendar */
.mini-calendar {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 0.125rem;
  text-align: center;
}

.mini-calendar-day {
  padding: 0.375rem;
  font-size: 0.75rem;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: all 0.15s ease;
}

.mini-calendar-day:hover {
  background: #f0fdfa;
}

.mini-calendar-day.header {
  font-weight: 600;
  color: #64748b;
  cursor: default;
}

.mini-calendar-day.header:hover {
  background: transparent;
}

.mini-calendar-day.today {
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
  font-weight: 600;
}

.mini-calendar-day.has-event {
  font-weight: 600;
  color: #0d9488;
}

.mini-calendar-day.other-month {
  color: #cbd5e1;
}

/* Upcoming Events List */
.upcoming-event-item {
  display: flex;
  gap: 0.75rem;
  padding: 0.75rem;
  border-radius: 0.625rem;
  transition: all 0.2s ease;
  cursor: pointer;
}

.upcoming-event-item:hover {
  background: #f0fdfa;
}

.upcoming-event-date {
  width: 44px;
  height: 44px;
  background: linear-gradient(135deg, #f0fdfa, #ccfbf1);
  border-radius: 0.625rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.upcoming-event-date span:first-child {
  font-size: 0.5625rem;
  font-weight: 700;
  text-transform: uppercase;
  color: #0d9488;
}

.upcoming-event-date span:last-child {
  font-size: 1rem;
  font-weight: 800;
  color: #0f172a;
  line-height: 1;
}

.upcoming-event-info h4 {
  font-size: 0.8125rem;
  font-weight: 600;
  color: #0f172a;
  margin: 0 0 0.125rem;
  line-height: 1.3;
}

.upcoming-event-info p {
  font-size: 0.75rem;
  color: #64748b;
  margin: 0;
}

/* Categories */
.category-item {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  padding: 0.5rem;
  border-radius: 0.5rem;
  cursor: pointer;
  transition: all 0.15s ease;
}

.category-item:hover {
  background: #f0fdfa;
}

.category-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
}

.category-name {
  flex: 1;
  font-size: 0.8125rem;
  color: #334155;
}

.category-count {
  font-size: 0.75rem;
  color: #94a3b8;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
  padding: 1rem;
}

.modal-content {
  background: white;
  border-radius: 1rem;
  width: 100%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
}

.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid #f1f5f9;
}

.modal-title {
  font-size: 1.125rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0;
}

.modal-close {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f1f5f9;
  border: none;
  border-radius: 0.5rem;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
}

.modal-close:hover {
  background: #e2e8f0;
  color: #0f172a;
}

.modal-body {
  padding: 1.5rem;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 1.25rem 1.5rem;
  border-top: 1px solid #f1f5f9;
}

/* Responsive */
.content-with-sidebar {
  display: flex;
  gap: 1.5rem;
}

@media (max-width: 1280px) {
  .content-with-sidebar {
    flex-direction: column;
  }
  .content-sidebar {
    width: 100%;
  }
}

@media (max-width: 768px) {
  .search-box {
    width: 100%;
  }
  .quick-actions {
    flex-direction: column;
  }
  .hero-stats {
    flex-direction: column;
  }
  .toolbar {
    flex-direction: column;
    align-items: stretch;
  }
  .toolbar-left,
  .toolbar-right {
    width: 100%;
  }
}
</style>
