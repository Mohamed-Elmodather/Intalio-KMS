<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import PageHeroHeader from '@/components/common/PageHeroHeader.vue'
import FilterDropdown from '@/components/common/FilterDropdown.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import Pagination from '@/components/common/Pagination.vue'
import ComparisonButton from '@/components/common/ComparisonButton.vue'
import CategoryBadge from '@/components/common/CategoryBadge.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import ShareContentModal from '@/components/common/ShareContentModal.vue'
import SkeletonLoader from '@/components/common/SkeletonLoader.vue'
import { useAIServicesStore } from '@/stores/aiServices'
import { usePagination } from '@/composables/usePagination'
import { AILoadingIndicator, AISuggestionChip, AIConfidenceBar } from '@/components/ai'
import ComparisonPanel from '@/components/ai/ComparisonPanel.vue'
import ComparisonModal from '@/components/ai/ComparisonModal.vue'
import type { SummarizationResult, ClassificationResult } from '@/types/ai'

const router = useRouter()
const { t } = useI18n()

// Initialize AI store
const aiStore = useAIServicesStore()

// State
const showCreateModal = ref(false)
const showFilters = ref(false)
const calendarView = ref<'calendar' | 'grid' | 'list'>('grid')
const currentDate = ref(new Date())
const searchQuery = ref('')
const selectedTypes = ref<string[]>([])
const selectedFormats = ref<string[]>([])
const quickFilter = ref<'all' | 'today' | 'week' | 'month' | 'custom' | 'myevents' | 'virtual' | 'inperson'>('all')
const showShareModal = ref(false)
const selectedEventForShare = ref<Event | null>(null)
const shareEventUrl = computed(() => {
  if (!selectedEventForShare.value) return ''
  return `${window.location.origin}/events/${selectedEventForShare.value.id}`
})

// Filter dropdown states
const showDateFilter = ref(false)
const showSortDropdown = ref(false)
const sortBy = ref('date')
const sortOrder = ref<'asc' | 'desc'>('asc')

// Sort options
const sortOptions = computed(() => [
  { value: 'date', label: t('common.date'), icon: 'fas fa-calendar' },
  { value: 'title', label: t('common.title'), icon: 'fas fa-font' },
  { value: 'attendees', label: t('events.attendees'), icon: 'fas fa-users' }
])

// Format filter options
const formatOptions = computed(() => [
  { id: 'virtual', label: t('events.virtual'), icon: 'fas fa-video', color: 'text-blue-500' },
  { id: 'in-person', label: t('events.inPerson'), icon: 'fas fa-building', color: 'text-amber-500' }
])

// Date range options
const dateRangeOptions = computed(() => [
  { id: 'all', label: t('events.allDates'), icon: 'fas fa-calendar' },
  { id: 'today', label: t('common.today'), icon: 'fas fa-sun' },
  { id: 'week', label: t('common.thisWeek'), icon: 'fas fa-calendar-week' },
  { id: 'month', label: t('common.thisMonth'), icon: 'fas fa-calendar-alt' },
  { id: 'custom', label: t('events.customRange'), icon: 'fas fa-calendar-days' }
])

// My Events filter (separate from date range)
const showMyEventsOnly = ref(false)
const showFeaturedOnly = ref(false)
const showInterestedOnly = ref(false)

// Unified Search State
const showAIFeatures = ref(true)
const unifiedSearchQuery = ref('')
const isAISearchMode = ref(false)
const showAISuggestions = ref(false)
const naturalLanguageQuery = ref('')
const aiSearchResults = ref<any[]>([])
const isProcessingNLSearch = ref(false)
const nlSearchSuggestions = computed(() => [
  t('events.aiSuggestion1'),
  t('events.aiSuggestion2'),
  t('events.aiSuggestion3'),
  t('events.aiSuggestion4'),
  t('events.aiSuggestion5')
])

// Custom date range state
const customDateStart = ref('')
const customDateEnd = ref('')
const showCustomDatePicker = ref(false)

// Calendar enhancement state
const selectedCalendarDay = ref<CalendarDay | null>(null)
const showDayDetailPopup = ref(false)
const calendarMode = ref<'month' | 'week'>('month')

interface CalendarDay {
  date: number
  fullDate?: string
  isCurrentMonth: boolean
  isToday: boolean
  events: Event[]
}

const weekDays = computed(() => [
  t('events.weekDays.sun'),
  t('events.weekDays.mon'),
  t('events.weekDays.tue'),
  t('events.weekDays.wed'),
  t('events.weekDays.thu'),
  t('events.weekDays.fri'),
  t('events.weekDays.sat')
])

const eventTypes = computed(() => [
  { id: 'meeting', name: t('events.eventTypeNames.meetings'), icon: 'fas fa-users', color: '#3b82f6' },
  { id: 'training', name: t('events.eventTypeNames.training'), icon: 'fas fa-chalkboard-teacher', color: '#10b981' },
  { id: 'social', name: t('events.eventTypeNames.social'), icon: 'fas fa-glass-cheers', color: '#ec4899' },
  { id: 'review', name: t('events.eventTypeNames.reviews'), icon: 'fas fa-clipboard-check', color: '#f59e0b' },
  { id: 'webinar', name: t('events.eventTypeNames.webinars'), icon: 'fas fa-video', color: '#6366f1' },
])

// Filter options for FilterDropdown
const eventTypeFilterOptions = computed(() =>
  eventTypes.value.map(type => ({
    id: type.id,
    label: type.name,
    icon: type.icon,
    color: type.color
  }))
)

const formatFilterOptions = computed(() =>
  formatOptions.map(format => ({
    id: format.id,
    label: format.label,
    icon: format.icon,
    color: format.color
  }))
)

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
  interested: boolean
  featured: boolean
  description: string
  attendees: Attendee[]
}

const events = ref<Event[]>([
  { id: 1, title: 'All-Hands Meeting Q4', date: '2025-12-24', time: '10:00 AM - 11:30 AM', location: 'Main Conference Room', category: 'meeting', categoryLabel: 'Meeting', virtual: true, isGoing: true, interested: false, featured: true, description: 'Quarterly all-hands meeting to discuss company progress and goals.', attendees: [{ color: '#14b8a6', initials: 'AI' }, { color: '#3b82f6', initials: 'SC' }, { color: '#ec4899', initials: 'LW' }] },
  { id: 2, title: 'Product Demo - New Features', date: '2025-12-26', time: '2:00 PM - 3:00 PM', location: 'Virtual - Zoom', category: 'webinar', categoryLabel: 'Webinar', virtual: true, isGoing: false, interested: true, featured: false, description: 'Demo of the latest product features to the team.', attendees: [{ color: '#6366f1', initials: 'DK' }, { color: '#0d9488', initials: 'MJ' }] },
  { id: 3, title: 'Year-End Performance Review', date: '2025-12-30', time: '9:00 AM - 12:00 PM', location: 'Board Room', category: 'review', categoryLabel: 'Review', virtual: false, isGoing: true, interested: false, featured: false, description: 'Annual performance review meeting.', attendees: [{ color: '#f59e0b', initials: 'HR' }] },
  { id: 4, title: 'Holiday Team Lunch', date: '2025-12-24', time: '12:30 PM - 2:00 PM', location: 'Cafeteria', category: 'social', categoryLabel: 'Social', virtual: false, isGoing: true, interested: false, featured: true, description: 'Holiday celebration lunch with the team.', attendees: [{ color: '#ec4899', initials: 'TM' }, { color: '#14b8a6', initials: 'AI' }, { color: '#3b82f6', initials: 'SC' }, { color: '#10b981', initials: 'ED' }] },
  { id: 5, title: 'Advanced Excel Training', date: '2025-12-27', time: '3:00 PM - 5:00 PM', location: 'Training Room A', category: 'training', categoryLabel: 'Training', virtual: false, isGoing: false, interested: true, featured: false, description: 'Advanced Excel techniques for productivity.', attendees: [{ color: '#10b981', initials: 'RG' }, { color: '#0d9488', initials: 'AT' }] },
  { id: 6, title: 'Sprint Planning Meeting', date: '2025-12-25', time: '10:00 AM - 11:00 AM', location: 'Scrum Room', category: 'meeting', categoryLabel: 'Meeting', virtual: false, isGoing: true, interested: false, featured: false, description: 'Planning for the upcoming sprint.', attendees: [{ color: '#3b82f6', initials: 'PM' }, { color: '#14b8a6', initials: 'AI' }, { color: '#6366f1', initials: 'DK' }] },
  { id: 7, title: 'Customer Success Webinar', date: '2025-12-28', time: '1:00 PM - 2:30 PM', location: 'Virtual - Teams', category: 'webinar', categoryLabel: 'Webinar', virtual: true, isGoing: false, interested: false, featured: true, description: 'Webinar on customer success strategies.', attendees: [{ color: '#6366f1', initials: 'CS' }, { color: '#ec4899', initials: 'LW' }] },
  { id: 8, title: 'New Year Planning Session', date: '2025-12-31', time: '2:00 PM - 4:00 PM', location: 'Executive Suite', category: 'meeting', categoryLabel: 'Meeting', virtual: false, isGoing: true, interested: false, featured: true, description: 'Strategic planning for the new year.', attendees: [{ color: '#14b8a6', initials: 'AI' }, { color: '#0d9488', initials: 'CEO' }] },
  { id: 9, title: 'Team Building Workshop', date: '2025-12-29', time: '9:00 AM - 12:00 PM', location: 'Innovation Lab', category: 'social', categoryLabel: 'Social', virtual: false, isGoing: false, interested: true, featured: false, description: 'Interactive team building exercises.', attendees: [{ color: '#ec4899', initials: 'HR' }, { color: '#10b981', initials: 'ED' }, { color: '#3b82f6', initials: 'SC' }] },
  { id: 10, title: 'Security Awareness Training', date: '2025-12-26', time: '10:00 AM - 11:00 AM', location: 'Virtual - Webex', category: 'training', categoryLabel: 'Training', virtual: true, isGoing: true, interested: false, featured: false, description: 'Mandatory security training for all employees.', attendees: [{ color: '#10b981', initials: 'IT' }, { color: '#f59e0b', initials: 'SEC' }] },
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

// Hero stats for PageHeroHeader component
const heroStats = computed(() => [
  { icon: 'fas fa-calendar-alt', value: eventStats.value.total, label: t('events.totalEvents') },
  { icon: 'fas fa-calendar-week', value: eventStats.value.thisWeek, label: t('events.thisWeek') },
  { icon: 'fas fa-check-circle', value: eventStats.value.yourRsvps, label: t('events.yourRsvps') },
  { icon: 'fas fa-sun', value: todayEventsCount.value, label: t('common.today') }
])

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

// Quick filter counts
const quickFilterCounts = computed(() => {
  const today = new Date().toISOString().split('T')[0]
  const weekFromNow = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]
  return {
    all: events.value.length,
    today: events.value.filter(e => e.date === today).length,
    week: events.value.filter(e => e.date >= today && e.date <= weekFromNow).length,
    myevents: events.value.filter(e => e.isGoing).length,
    virtual: events.value.filter(e => e.virtual).length,
    inperson: events.value.filter(e => !e.virtual).length
  }
})

// My upcoming events (RSVP'd)
const myUpcomingEvents = computed(() => {
  const today = new Date()
  return events.value
    .filter(e => e.isGoing && new Date(e.date) >= today)
    .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
    .slice(0, 3)
    .map(e => {
      const d = new Date(e.date)
      return {
        ...e,
        month: d.toLocaleString('default', { month: 'short' }).toUpperCase(),
        day: d.getDate(),
        dayName: d.toLocaleString('default', { weekday: 'short' })
      }
    })
})

// Countdown to featured event
const featuredEventCountdown = computed(() => {
  if (!featuredEvent.value) return null
  const eventDate = new Date(featuredEvent.value.date)
  const now = new Date()
  const diff = eventDate.getTime() - now.getTime()
  if (diff <= 0) return { days: 0, hours: 0, label: 'Today!' }
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
  if (days === 0) return { days: 0, hours, label: `${hours}h left` }
  if (days === 1) return { days: 1, hours, label: 'Tomorrow' }
  return { days, hours, label: `${days} days` }
})

// Filtered events for calendar view (applies all filters EXCEPT date range)
const calendarFilteredEvents = computed(() => {
  let result = [...events.value]

  // Apply My Events filter
  if (showMyEventsOnly.value) {
    result = result.filter(e => e.isGoing)
  }

  // Apply Featured filter
  if (showFeaturedOnly.value) {
    result = result.filter(e => e.featured)
  }

  // Apply Interested filter
  if (showInterestedOnly.value) {
    result = result.filter(e => e.interested)
  }

  // Apply search filter
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    result = result.filter(e => e.title.toLowerCase().includes(q) || e.location.toLowerCase().includes(q))
  }

  // Apply type filter
  if (selectedTypes.value.length > 0) {
    result = result.filter(e => selectedTypes.value.includes(e.category))
  }

  // Apply format filter
  if (selectedFormats.value.length > 0) {
    result = result.filter(e => {
      if (selectedFormats.value.includes('virtual') && e.virtual) return true
      if (selectedFormats.value.includes('in-person') && !e.virtual) return true
      return false
    })
  }

  return result
})

const filteredEvents = computed(() => {
  // If AI search mode is active and has results, use those
  if (isAISearchMode.value && aiSearchResults.value.length > 0) {
    return aiSearchResults.value.map(e => {
      const d = new Date(e.date)
      return {
        ...e,
        month: d.toLocaleString('default', { month: 'short' }).toUpperCase(),
        day: d.getDate()
      }
    })
  }

  const today = new Date().toISOString().split('T')[0]
  const weekFromNow = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]

  let result = events.value.map(e => {
    const d = new Date(e.date)
    return {
      ...e,
      month: d.toLocaleString('default', { month: 'short' }).toUpperCase(),
      day: d.getDate()
    }
  })

  // Apply date range filter
  if (quickFilter.value === 'today') {
    result = result.filter(e => e.date === today)
  } else if (quickFilter.value === 'week') {
    result = result.filter(e => e.date >= today && e.date <= weekFromNow)
  } else if (quickFilter.value === 'month') {
    const monthFromNow = new Date()
    monthFromNow.setMonth(monthFromNow.getMonth() + 1)
    const monthEnd = monthFromNow.toISOString().split('T')[0]
    result = result.filter(e => e.date >= today && e.date <= monthEnd)
  } else if (quickFilter.value === 'custom' && customDateStart.value && customDateEnd.value) {
    result = result.filter(e => e.date >= customDateStart.value && e.date <= customDateEnd.value)
  }

  // Apply My Events filter (separate from date range)
  if (showMyEventsOnly.value) {
    result = result.filter(e => e.isGoing)
  }

  // Apply Featured filter
  if (showFeaturedOnly.value) {
    result = result.filter(e => e.featured)
  }

  // Apply Interested filter
  if (showInterestedOnly.value) {
    result = result.filter(e => e.interested)
  }

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

  // Apply sorting
  result.sort((a, b) => {
    let comparison = 0
    if (sortBy.value === 'date') {
      comparison = new Date(a.date).getTime() - new Date(b.date).getTime()
    } else if (sortBy.value === 'title') {
      comparison = a.title.localeCompare(b.title)
    } else if (sortBy.value === 'attendees') {
      comparison = b.attendees.length - a.attendees.length
    }
    return sortOrder.value === 'asc' ? comparison : -comparison
  })

  return result
})

// Pagination
const {
  currentPage,
  itemsPerPage,
  itemsPerPageOptions,
  paginatedItems: paginatedEvents,
  totalItems,
  resetPage
} = usePagination(filteredEvents, {
  defaultPerPage: 6,
  perPageOptions: [6, 9, 12, 24]
})

// Reset to first page when filters change
watch(
  [searchQuery, quickFilter, customDateStart, customDateEnd, showMyEventsOnly, showFeaturedOnly, showInterestedOnly, selectedTypes, selectedFormats, sortBy, sortOrder, isAISearchMode, aiSearchResults],
  () => {
    resetPage()
  }
)

const calendarDays = computed<CalendarDay[]>(() => {
  const days: CalendarDay[] = []
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()
  const firstDay = new Date(year, month, 1)
  const lastDay = new Date(year, month + 1, 0)
  const startPadding = firstDay.getDay()
  const today = new Date()
  const filtered = calendarFilteredEvents.value

  const prevMonthLastDay = new Date(year, month, 0).getDate()
  for (let i = startPadding - 1; i >= 0; i--) {
    const prevMonth = month === 0 ? 11 : month - 1
    const prevYear = month === 0 ? year - 1 : year
    const dateStr = `${prevYear}-${String(prevMonth + 1).padStart(2, '0')}-${String(prevMonthLastDay - i).padStart(2, '0')}`
    const dayEvents = filtered.filter(e => e.date === dateStr)
    days.push({ date: prevMonthLastDay - i, fullDate: dateStr, isCurrentMonth: false, isToday: false, events: dayEvents })
  }

  for (let d = 1; d <= lastDay.getDate(); d++) {
    const dateStr = `${year}-${String(month + 1).padStart(2, '0')}-${String(d).padStart(2, '0')}`
    const dayEvents = filtered.filter(e => e.date === dateStr)
    days.push({
      date: d,
      fullDate: dateStr,
      isCurrentMonth: true,
      isToday: today.getDate() === d && today.getMonth() === month && today.getFullYear() === year,
      events: dayEvents
    })
  }

  const remaining = 42 - days.length
  const nextMonth = month === 11 ? 0 : month + 1
  const nextYear = month === 11 ? year + 1 : year
  for (let d = 1; d <= remaining; d++) {
    const dateStr = `${nextYear}-${String(nextMonth + 1).padStart(2, '0')}-${String(d).padStart(2, '0')}`
    const dayEvents = filtered.filter(e => e.date === dateStr)
    days.push({ date: d, fullDate: dateStr, isCurrentMonth: false, isToday: false, events: dayEvents })
  }

  return days
})

const miniCalendarDays = computed(() => calendarDays.value.map(d => ({ ...d, hasEvents: d.events.length > 0 })))

// Week view days (current week based on currentDate)
const weekViewDays = computed<CalendarDay[]>(() => {
  const days: CalendarDay[] = []
  const today = new Date()
  const curr = new Date(currentDate.value)
  const dayOfWeek = curr.getDay()
  const startOfWeek = new Date(curr)
  startOfWeek.setDate(curr.getDate() - dayOfWeek)
  const filtered = calendarFilteredEvents.value

  for (let i = 0; i < 7; i++) {
    const day = new Date(startOfWeek)
    day.setDate(startOfWeek.getDate() + i)
    const dateStr = `${day.getFullYear()}-${String(day.getMonth() + 1).padStart(2, '0')}-${String(day.getDate()).padStart(2, '0')}`
    const dayEvents = filtered.filter(e => e.date === dateStr)
    days.push({
      date: day.getDate(),
      fullDate: dateStr,
      isCurrentMonth: day.getMonth() === currentDate.value.getMonth(),
      isToday: day.toDateString() === today.toDateString(),
      events: dayEvents
    })
  }
  return days
})

// Calendar month stats (uses filtered events)
const calendarMonthStats = computed(() => {
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()
  const filtered = calendarFilteredEvents.value
  const monthEvents = filtered.filter(e => {
    const d = new Date(e.date)
    return d.getFullYear() === year && d.getMonth() === month
  })
  return {
    total: monthEvents.length,
    featured: monthEvents.filter(e => e.featured).length,
    going: monthEvents.filter(e => e.isGoing).length,
    virtual: monthEvents.filter(e => e.virtual).length
  }
})

// Selected day formatted date
const selectedDayFormatted = computed(() => {
  if (!selectedCalendarDay.value?.fullDate) return ''
  const d = new Date(selectedCalendarDay.value.fullDate)
  return d.toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric', year: 'numeric' })
})

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
  quickFilter.value = 'today'
  currentPage.value = 1
  // Switch to list/grid view to see filtered results
  if (calendarView.value === 'calendar') {
    calendarView.value = 'grid'
  }
}

function selectDate(day: CalendarDay) {
  selectedCalendarDay.value = day
  showDayDetailPopup.value = true
}

function closeDayDetailPopup() {
  showDayDetailPopup.value = false
  selectedCalendarDay.value = null
}

function selectMiniCalendarDay(day: CalendarDay & { hasEvents: boolean }) {
  selectedCalendarDay.value = day
  showDayDetailPopup.value = true
}

// Week navigation
function previousWeek() {
  const curr = new Date(currentDate.value)
  curr.setDate(curr.getDate() - 7)
  currentDate.value = curr
}

function nextWeek() {
  const curr = new Date(currentDate.value)
  curr.setDate(curr.getDate() + 7)
  currentDate.value = curr
}

// Get short time from event time
function getShortTime(time: string) {
  return time.split(' - ')[0]
}

function openEvent(event: Event & { month?: string; day?: number }) {
  router.push({ name: 'EventDetail', params: { id: event.id } })
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
  unifiedSearchQuery.value = ''
  quickFilter.value = 'all'
  customDateStart.value = ''
  customDateEnd.value = ''
  showCustomDatePicker.value = false
  showMyEventsOnly.value = false
  showFeaturedOnly.value = false
  showInterestedOnly.value = false
  currentPage.value = 1
}

// Unified Search Handlers
function handleSearchInput() {
  if (isAISearchMode.value) {
    showAISuggestions.value = !unifiedSearchQuery.value
  } else {
    searchQuery.value = unifiedSearchQuery.value
  }
}

async function processNaturalLanguageSearch() {
  if (!naturalLanguageQuery.value || isProcessingNLSearch.value) return

  isProcessingNLSearch.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 1000))

    const query = naturalLanguageQuery.value.toLowerCase()
    const allEvents = events.value

    let results = allEvents

    if (query.includes('virtual') || query.includes('online')) {
      results = results.filter(e => e.virtual)
    }
    if (query.includes('in-person') || query.includes('in person')) {
      results = results.filter(e => !e.virtual)
    }
    if (query.includes('meeting')) {
      results = results.filter(e => e.category === 'meeting')
    }
    if (query.includes('training')) {
      results = results.filter(e => e.category === 'training')
    }
    if (query.includes('webinar')) {
      results = results.filter(e => e.category === 'webinar')
    }
    if (query.includes('social')) {
      results = results.filter(e => e.category === 'social')
    }
    if (query.includes('registered') || query.includes('my events')) {
      results = results.filter(e => e.isGoing)
    }
    if (query.includes('this week') || query.includes('upcoming')) {
      const now = new Date()
      const weekFromNow = new Date(now.getTime() + 7 * 24 * 60 * 60 * 1000)
      results = results.filter(e => {
        const eventDate = new Date(e.date)
        return eventDate >= now && eventDate <= weekFromNow
      })
    }
    if (query.includes('this month')) {
      const now = new Date()
      const monthFromNow = new Date(now.getFullYear(), now.getMonth() + 1, now.getDate())
      results = results.filter(e => {
        const eventDate = new Date(e.date)
        return eventDate >= now && eventDate <= monthFromNow
      })
    }

    aiSearchResults.value = results.slice(0, 12)
  } finally {
    isProcessingNLSearch.value = false
  }
}

function handleUnifiedSearch() {
  if (!unifiedSearchQuery.value) return

  if (isAISearchMode.value) {
    naturalLanguageQuery.value = unifiedSearchQuery.value
    processNaturalLanguageSearch()
  } else {
    searchQuery.value = unifiedSearchQuery.value
  }
  showAISuggestions.value = false
}

function clearUnifiedSearch() {
  unifiedSearchQuery.value = ''
  searchQuery.value = ''
  naturalLanguageQuery.value = ''
  aiSearchResults.value = []
  showAISuggestions.value = false
}

watch(isAISearchMode, (newValue) => {
  if (newValue && !unifiedSearchQuery.value) {
    showAISuggestions.value = true
  } else {
    showAISuggestions.value = false
  }
  if (!newValue) {
    searchQuery.value = unifiedSearchQuery.value
  }
})

function setSort(value: string) {
  if (sortBy.value === value) {
    sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortBy.value = value
    sortOrder.value = 'asc'
  }
  showSortDropdown.value = false
}

function setDateRange(rangeId: string) {
  if (rangeId === 'custom') {
    showCustomDatePicker.value = true
  } else {
    quickFilter.value = rangeId as typeof quickFilter.value
    currentPage.value = 1
    showDateFilter.value = false
    showCustomDatePicker.value = false
  }
}

function applyCustomDateRange() {
  if (customDateStart.value && customDateEnd.value) {
    quickFilter.value = 'custom'
    currentPage.value = 1
    showDateFilter.value = false
    showCustomDatePicker.value = false
  }
}

function cancelCustomDateRange() {
  showCustomDatePicker.value = false
  if (quickFilter.value !== 'custom') {
    customDateStart.value = ''
    customDateEnd.value = ''
  }
}

// Computed for custom date range label
const customDateRangeLabel = computed(() => {
  if (quickFilter.value === 'custom' && customDateStart.value && customDateEnd.value) {
    const start = new Date(customDateStart.value).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
    const end = new Date(customDateEnd.value).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
    return `${start} - ${end}`
  }
  return null
})

function getEventTypeIcon(category: string) {
  const type = eventTypes.value.find(t => t.id === category)
  return type ? type.icon : 'fas fa-calendar'
}

function getCategoryCount(categoryId: string) {
  return events.value.filter(e => e.category === categoryId).length
}

function toggleInterested(eventId: number) {
  const event = events.value.find(e => e.id === eventId)
  if (event) {
    event.interested = !event.interested
    // If marking as interested and already going, keep going status
  }
}

function toggleReserve(eventId: number) {
  const event = events.value.find(e => e.id === eventId)
  if (event) {
    event.isGoing = !event.isGoing
    // If reserving, remove interested status (upgraded to going)
    if (event.isGoing) {
      event.interested = false
    }
  }
}

function shareEvent(event: Event) {
  selectedEventForShare.value = event
  showShareModal.value = true
}

// ============================================================================
// Comparison Functions
// ============================================================================

// ============================================================================
// AI Features State & Functions
// ============================================================================

// AI State
const showAIPanel = ref(true)
const isGeneratingDescription = ref(false)
const isAnalyzingSchedule = ref(false)
const isFetchingRelated = ref(false)
const showAIDescriptionModal = ref(false)
const showSmartScheduleModal = ref(false)
const showRelatedEventsPanel = ref(false)

// AI Interfaces
interface AIEventDescription {
  title: string
  description: string
  highlights: string[]
  targetAudience: string
  tags: string[]
}

interface ScheduleSuggestion {
  id: string
  timeSlot: string
  date: string
  score: number
  reason: string
  conflicts: string[]
  recommendation: 'optimal' | 'good' | 'available'
}

interface RelatedEvent {
  id: number
  title: string
  date: string
  category: string
  similarity: number
  reason: string
}

// AI Data
const generatedDescription = ref<AIEventDescription | null>(null)
const scheduleSuggestions = ref<ScheduleSuggestion[]>([])
const relatedEvents = ref<RelatedEvent[]>([])
const selectedEventForAI = ref<Event | null>(null)

// Mock AI Data
const mockGeneratedDescription: AIEventDescription = {
  title: 'Quarterly Strategy & Innovation Summit',
  description: 'Join us for an engaging session where leadership will share key updates on organizational progress, strategic initiatives, and upcoming opportunities. This event brings together teams across departments to align on goals and celebrate achievements.',
  highlights: [
    'Q4 Performance Review & Key Metrics',
    'Strategic Initiatives for 2026',
    'Team Recognition & Awards',
    'Interactive Q&A with Leadership',
    'Networking Opportunities'
  ],
  targetAudience: 'All employees, particularly those interested in company direction and cross-team collaboration',
  tags: ['Strategy', 'Leadership', 'Team Building', 'Quarterly Update', 'All-Hands']
}

const mockScheduleSuggestions: ScheduleSuggestion[] = [
  {
    id: '1',
    timeSlot: '10:00 AM - 11:30 AM',
    date: '2025-12-27',
    score: 0.95,
    reason: 'Most team members are available, no conflicting meetings, optimal focus time',
    conflicts: [],
    recommendation: 'optimal'
  },
  {
    id: '2',
    timeSlot: '2:00 PM - 3:30 PM',
    date: '2025-12-27',
    score: 0.82,
    reason: 'Good availability after lunch, allows preparation time',
    conflicts: ['2 optional meetings'],
    recommendation: 'good'
  },
  {
    id: '3',
    timeSlot: '9:00 AM - 10:30 AM',
    date: '2025-12-28',
    score: 0.75,
    reason: 'Morning slot available, some team members on PTO',
    conflicts: ['3 team members unavailable'],
    recommendation: 'available'
  },
  {
    id: '4',
    timeSlot: '3:00 PM - 4:30 PM',
    date: '2025-12-29',
    score: 0.68,
    reason: 'End of week slot, lower energy but available',
    conflicts: ['Weekend proximity', '1 conflicting review'],
    recommendation: 'available'
  }
]

const mockRelatedEvents: RelatedEvent[] = [
  {
    id: 101,
    title: 'Leadership Town Hall - January',
    date: '2026-01-15',
    category: 'meeting',
    similarity: 0.92,
    reason: 'Similar format and audience - leadership communication event'
  },
  {
    id: 102,
    title: 'Team Sync: Cross-Department Collaboration',
    date: '2026-01-08',
    category: 'meeting',
    similarity: 0.85,
    reason: 'Cross-team alignment focus, similar objectives'
  },
  {
    id: 103,
    title: 'Annual Planning Workshop',
    date: '2026-01-22',
    category: 'training',
    similarity: 0.78,
    reason: 'Strategic planning context, complementary to quarterly review'
  },
  {
    id: 104,
    title: 'Innovation Showcase',
    date: '2026-02-05',
    category: 'webinar',
    similarity: 0.71,
    reason: 'Shares innovation and progress theme'
  }
]

// AI Functions
async function generateEventDescription(event?: Event) {
  isGeneratingDescription.value = true
  showAIDescriptionModal.value = true
  selectedEventForAI.value = event || null

  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1200))
    generatedDescription.value = mockGeneratedDescription
  } catch (error) {
    console.error('Failed to generate description:', error)
  } finally {
    isGeneratingDescription.value = false
  }
}

async function analyzeSmartSchedule() {
  isAnalyzingSchedule.value = true
  showSmartScheduleModal.value = true

  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1500))
    scheduleSuggestions.value = mockScheduleSuggestions
  } catch (error) {
    console.error('Failed to analyze schedule:', error)
  } finally {
    isAnalyzingSchedule.value = false
  }
}

async function fetchRelatedEvents(event?: Event) {
  isFetchingRelated.value = true
  showRelatedEventsPanel.value = true
  selectedEventForAI.value = event || events.value[0] || null

  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    relatedEvents.value = mockRelatedEvents
  } catch (error) {
    console.error('Failed to fetch related events:', error)
  } finally {
    isFetchingRelated.value = false
  }
}

function applyGeneratedDescription() {
  if (generatedDescription.value) {
    newEvent.value.title = generatedDescription.value.title
    newEvent.value.description = generatedDescription.value.description
    showAIDescriptionModal.value = false
  }
}

function applyScheduleSuggestion(suggestion: ScheduleSuggestion) {
  newEvent.value.startDate = suggestion.date
  // Parse time slot
  const times = suggestion.timeSlot.split(' - ')
  if (times.length > 0) {
    // Could parse and apply time if needed
  }
  showSmartScheduleModal.value = false
}

function getRecommendationColor(rec: string) {
  switch (rec) {
    case 'optimal': return 'text-green-600 bg-green-100'
    case 'good': return 'text-blue-600 bg-blue-100'
    case 'available': return 'text-gray-600 bg-gray-100'
    default: return 'text-gray-600 bg-gray-100'
  }
}

function getCategoryIcon(category: string) {
  const type = eventTypes.value.find(t => t.id === category)
  return type ? type.icon : 'fas fa-calendar'
}

function getCategoryColor(category: string) {
  const type = eventTypes.value.find(t => t.id === category)
  return type ? type.color : '#6b7280'
}
</script>

<template>
  <div class="events-page">
    <!-- Hero Section -->
    <PageHeroHeader
      :stats="heroStats"
      badge-icon="fas fa-calendar-alt"
      :title="$t('events.title')"
      :subtitle="$t('events.subtitle')"
    >
      <template #actions>
        <button @click="showCreateModal = true" class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
          <i class="fas fa-plus"></i>
          {{ $t('events.createEvent') }}
        </button>
        <!-- AI Action Buttons -->
        <button @click="analyzeSmartSchedule"
                class="px-5 py-2.5 bg-gradient-to-r from-teal-500/20 to-emerald-500/20 backdrop-blur-sm text-white rounded-xl font-semibold text-sm flex items-center gap-2 hover:from-teal-500/30 hover:to-emerald-500/30 transition-all border border-white/20">
          <i class="fas fa-wand-magic-sparkles"></i>
          {{ $t('events.smartScheduling') }}
        </button>
        <button @click="fetchRelatedEvents()"
                class="px-5 py-2.5 bg-white/10 backdrop-blur-sm text-white rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-white/20 transition-all border border-white/20">
          <i class="fas fa-robot"></i>
          {{ $t('ai.aiSuggestions') }}
        </button>
      </template>
    </PageHeroHeader>

    <!-- Main Content Area -->
    <div class="events-content">

      <!-- Premium Featured Event -->
      <div v-if="featuredEvent" class="featured-event-premium">
        <div class="featured-event-bg">
          <div class="featured-orb featured-orb-1"></div>
          <div class="featured-orb featured-orb-2"></div>
        </div>
        <div class="featured-event-header">
          <div class="featured-badge-premium">
            <i class="fas fa-star"></i>
            <span>{{ $t('events.nextUp') }}</span>
          </div>
          <!-- Countdown Badge -->
          <div v-if="featuredEventCountdown" class="featured-countdown">
            <div class="countdown-icon">
              <i class="fas fa-hourglass-half"></i>
            </div>
            <span class="countdown-label">{{ featuredEventCountdown.label }}</span>
          </div>
          <div class="featured-date-badge">
            <span class="featured-date-month">{{ featuredEvent.month }}</span>
            <span class="featured-date-day">{{ featuredEvent.day }}</span>
          </div>
        </div>
        <div class="featured-event-body">
          <h2 class="featured-event-title">{{ featuredEvent.title }}</h2>
          <p class="featured-event-description">{{ featuredEvent.description }}</p>
          <div class="featured-event-meta">
            <div class="featured-meta-item">
              <div class="featured-meta-icon">
                <i class="fas fa-calendar"></i>
              </div>
              <span>{{ featuredEvent.dateFormatted }}</span>
            </div>
            <div class="featured-meta-item">
              <div class="featured-meta-icon">
                <i class="fas fa-clock"></i>
              </div>
              <span>{{ featuredEvent.time }}</span>
            </div>
            <div class="featured-meta-item">
              <div class="featured-meta-icon">
                <i class="fas fa-map-marker-alt"></i>
              </div>
              <span>{{ featuredEvent.location }}</span>
            </div>
            <div v-if="featuredEvent.virtual" class="featured-meta-item virtual">
              <div class="featured-meta-icon">
                <i class="fas fa-video"></i>
              </div>
              <span>{{ $t('events.virtualEvent') }}</span>
            </div>
          </div>
        </div>
        <div class="featured-event-footer">
          <div class="featured-attendees">
            <div class="featured-avatar-stack">
              <div class="featured-avatar" style="background: #14b8a6;">AI</div>
              <div class="featured-avatar" style="background: #3b82f6;">SC</div>
              <div class="featured-avatar" style="background: #ec4899;">LW</div>
            </div>
            <span class="featured-attendee-text">+12 attending</span>
          </div>
          <div class="featured-actions">
            <button class="featured-action-btn secondary" title="Set Reminder">
              <i class="fas fa-bell"></i>
            </button>
            <button class="featured-action-btn secondary" title="Share Event" @click="shareEvent(featuredEvent!)">
              <i class="fas fa-share-alt"></i>
            </button>
            <button class="featured-action-btn secondary" title="Add to Calendar">
              <i class="fas fa-calendar-plus"></i>
            </button>
            <button class="featured-rsvp-btn">
              <i class="fas fa-check"></i>
              <span>{{ $t('events.rsvpNow') }}</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Events Section Wrapper -->
      <div class="events-wrapper bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden mb-4">
        <!-- Section Header / Toolbar -->
        <div class="border-b border-gray-100">
          <!-- Top Row - Title and Primary Actions -->
          <div class="px-4 py-3 flex items-center justify-between">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
                <i class="fas fa-calendar-alt text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Events</span>
                <span class="text-xs font-medium text-gray-500">{{ filteredEvents.length }} events</span>
              </div>
            </h2>
            <div class="flex items-center gap-2">
              <!-- Primary Actions -->
              <button @click="showCreateModal = true" class="px-4 py-2 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-lg text-sm font-medium hover:from-teal-600 hover:to-teal-700 transition-all flex items-center gap-2 shadow-sm shadow-teal-200">
                <i class="fas fa-plus"></i>
                Create Event
              </button>

              <!-- Refresh Button -->
              <button
                @click="currentPage = 1"
                class="w-9 h-9 rounded-lg bg-gray-100 text-gray-600 hover:bg-gray-200 hover:text-gray-800 flex items-center justify-center transition-all"
                title="Refresh"
              >
                <i class="fas fa-sync-alt text-sm"></i>
              </button>
            </div>
          </div>

          <!-- Bottom Row - Search, Filters, View Options -->
          <div class="px-4 py-2 bg-gray-50/50 flex flex-wrap items-center gap-3">
            <!-- Unified Search with AI Integration -->
            <div class="relative flex-1 max-w-xl">
              <div class="flex items-stretch">
                <!-- AI Mode Toggle -->
                <button
                  v-if="showAIFeatures"
                  @click="isAISearchMode = !isAISearchMode"
                  :class="[
                    'px-3 rounded-l-lg border border-r-0 flex items-center gap-1.5 text-xs font-medium transition-all',
                    isAISearchMode
                      ? 'bg-gradient-to-r from-teal-500 to-cyan-500 border-teal-500 text-white'
                      : 'bg-gray-100 border-gray-200 text-gray-500 hover:bg-gray-200'
                  ]"
                  title="Toggle AI Search"
                >
                  <i class="fas fa-wand-magic-sparkles"></i>
                  <span class="hidden sm:inline">AI</span>
                </button>

                <!-- Search Input -->
                <div class="relative flex-1">
                  <i :class="[
                    'absolute start-3 top-1/2 -translate-y-1/2 text-sm transition-colors',
                    isAISearchMode ? 'fas fa-brain text-teal-500' : 'fas fa-search text-gray-400'
                  ]"></i>
                  <input
                    v-model="unifiedSearchQuery"
                    type="text"
                    :placeholder="isAISearchMode ? $t('events.aiSearchPlaceholder') : $t('events.searchEvents')"
                    @keyup.enter="handleUnifiedSearch"
                    @input="handleSearchInput"
                    :class="[
                      'w-full ps-9 pe-20 py-2 text-sm focus:outline-none transition-all',
                      showAIFeatures ? 'rounded-r-lg' : 'rounded-lg',
                      isAISearchMode
                        ? 'bg-gradient-to-r from-teal-50 to-cyan-50 border border-teal-200 focus:ring-2 focus:ring-teal-400 focus:border-transparent placeholder:text-teal-400'
                        : 'bg-white border border-gray-200 focus:ring-2 focus:ring-teal-500 focus:border-transparent',
                      !showAIFeatures && 'rounded-l-lg'
                    ]"
                  >
                  <!-- Clear & Search Buttons -->
                  <div class="absolute end-2 top-1/2 -translate-y-1/2 flex items-center gap-1">
                    <button
                      v-if="unifiedSearchQuery"
                      @click="clearUnifiedSearch"
                      :class="['p-1 rounded transition-colors', isAISearchMode ? 'text-teal-400 hover:text-teal-600' : 'text-gray-400 hover:text-gray-600']"
                    >
                      <i class="fas fa-times text-xs"></i>
                    </button>
                    <button
                      v-if="isAISearchMode && unifiedSearchQuery"
                      @click="handleUnifiedSearch"
                      :disabled="isProcessingNLSearch"
                      class="p-1 rounded text-teal-500 hover:text-teal-600 disabled:opacity-50"
                    >
                      <i :class="isProcessingNLSearch ? 'fas fa-spinner animate-spin' : 'fas fa-arrow-right'" class="text-sm"></i>
                    </button>
                  </div>
                </div>
              </div>

              <!-- AI Search Suggestions Dropdown -->
              <div
                v-if="showAIFeatures && isAISearchMode && showAISuggestions && !unifiedSearchQuery"
                class="absolute start-0 top-full mt-2 w-full bg-white rounded-xl shadow-lg border border-teal-100 py-2 z-50"
              >
                <div class="px-3 py-1.5 text-xs font-semibold text-teal-500 flex items-center gap-2">
                  <i class="fas fa-lightbulb"></i>
                  {{ $t('events.tryAsking') }}
                </div>
                <button
                  v-for="suggestion in nlSearchSuggestions"
                  :key="suggestion"
                  @click="unifiedSearchQuery = suggestion; handleUnifiedSearch()"
                  class="w-full px-3 py-2 text-start text-sm text-gray-700 hover:bg-teal-50 flex items-center gap-2"
                >
                  <i class="fas fa-search text-teal-400 text-xs"></i>
                  {{ suggestion }}
                </button>
              </div>

              <!-- AI Processing Indicator -->
              <div
                v-if="isProcessingNLSearch"
                class="absolute start-0 top-full mt-2 w-full bg-gradient-to-r from-teal-50 to-cyan-50 rounded-xl shadow-lg border border-teal-100 p-4 z-50"
              >
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-teal-500 to-cyan-500 flex items-center justify-center">
                    <i class="fas fa-brain text-white text-sm animate-pulse"></i>
                  </div>
                  <div>
                    <div class="text-sm font-medium text-teal-700">{{ $t('events.aiSearching') }}</div>
                    <div class="text-xs text-teal-500">{{ $t('events.analyzingQuery') }}</div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Date Range Filter Dropdown -->
            <div class="relative">
              <button
                @click="showDateFilter = !showDateFilter"
                :class="[
                  'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                  quickFilter !== 'all' ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                ]"
              >
                <i class="fas fa-calendar text-sm"></i>
                <span>{{ customDateRangeLabel || dateRangeOptions.find(d => d.id === quickFilter)?.label || $t('events.allDates') }}</span>
                <i :class="showDateFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ms-1"></i>
              </button>

              <!-- Dropdown Menu -->
              <div
                v-if="showDateFilter"
                class="absolute start-0 top-full mt-2 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                :class="showCustomDatePicker ? 'w-72' : 'w-48'"
              >
                <!-- Preset Options -->
                <div v-if="!showCustomDatePicker">
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">{{ $t('events.dateRange') }}</div>
                  <button
                    v-for="option in dateRangeOptions"
                    :key="option.id"
                    @click="setDateRange(option.id)"
                    :class="[
                      'w-full px-3 py-2 text-start text-sm flex items-center gap-3 transition-colors',
                      quickFilter === option.id ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                    ]"
                  >
                    <i :class="[option.icon, 'text-sm', quickFilter === option.id ? 'text-teal-500' : 'text-gray-400']"></i>
                    <span class="flex-1">{{ option.label }}</span>
                    <i v-if="quickFilter === option.id && option.id !== 'custom'" class="fas fa-check text-teal-500 text-xs"></i>
                    <i v-if="option.id === 'custom'" class="fas fa-chevron-right text-gray-400 text-xs"></i>
                  </button>
                </div>

                <!-- Custom Date Picker -->
                <div v-else class="px-3 py-2">
                  <div class="flex items-center gap-2 mb-3">
                    <button @click="showCustomDatePicker = false" class="text-gray-400 hover:text-gray-600">
                      <i class="fas fa-arrow-left text-sm"></i>
                    </button>
                    <span class="text-sm font-semibold text-gray-700">Custom Date Range</span>
                  </div>

                  <div class="space-y-3">
                    <div>
                      <label class="block text-xs font-medium text-gray-500 mb-1">Start Date</label>
                      <input
                        type="date"
                        v-model="customDateStart"
                        class="w-full px-3 py-2 text-sm border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent"
                      >
                    </div>
                    <div>
                      <label class="block text-xs font-medium text-gray-500 mb-1">End Date</label>
                      <input
                        type="date"
                        v-model="customDateEnd"
                        :min="customDateStart"
                        class="w-full px-3 py-2 text-sm border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent"
                      >
                    </div>
                  </div>

                  <div class="flex gap-2 mt-4">
                    <button
                      @click="cancelCustomDateRange"
                      class="flex-1 px-3 py-2 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                    >
                      Cancel
                    </button>
                    <button
                      @click="applyCustomDateRange"
                      :disabled="!customDateStart || !customDateEnd"
                      :class="[
                        'flex-1 px-3 py-2 text-xs font-medium rounded-lg transition-colors',
                        customDateStart && customDateEnd
                          ? 'text-white bg-teal-500 hover:bg-teal-600'
                          : 'text-gray-400 bg-gray-100 cursor-not-allowed'
                      ]"
                    >
                      Apply
                    </button>
                  </div>
                </div>
              </div>
              <div v-if="showDateFilter && !showCustomDatePicker" @click="showDateFilter = false" class="fixed inset-0 z-40"></div>
            </div>

            <!-- Event Type Filter Dropdown -->
            <FilterDropdown
              v-model="selectedTypes"
              icon="fas fa-calendar-check"
              :label="$t('events.eventType')"
              :selected-label="$t('events.types')"
              :header-label="$t('events.selectEventTypes')"
              :options="eventTypeFilterOptions"
              :clear-all-label="$t('common.clear')"
              :apply-label="$t('common.apply')"
            />

            <!-- Format Filter Dropdown -->
            <FilterDropdown
              v-model="selectedFormats"
              icon="fas fa-desktop"
              :label="$t('events.format')"
              :selected-label="$t('events.formats')"
              :header-label="$t('events.selectFormat')"
              :options="formatFilterOptions"
              :clear-all-label="$t('common.clear')"
              :apply-label="$t('common.apply')"
              dropdown-width="w-48"
            />

            <!-- Featured Toggle -->
            <button
              @click="showFeaturedOnly = !showFeaturedOnly; currentPage = 1"
              :class="[
                'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                showFeaturedOnly ? 'bg-amber-50 border-amber-200 text-amber-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
              ]"
            >
              <i class="fas fa-star text-sm"></i>
              <span>Featured</span>
              <span v-if="showFeaturedOnly" class="w-4 h-4 rounded-full bg-amber-500 text-white text-[10px] flex items-center justify-center">
                <i class="fas fa-check"></i>
              </span>
            </button>

            <!-- Interested Toggle -->
            <button
              @click="showInterestedOnly = !showInterestedOnly; currentPage = 1"
              :class="[
                'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                showInterestedOnly ? 'bg-red-50 border-red-200 text-red-600' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
              ]"
            >
              <i :class="showInterestedOnly ? 'fas fa-heart' : 'far fa-heart'" class="text-sm"></i>
              <span>Interested</span>
              <span v-if="showInterestedOnly" class="w-4 h-4 rounded-full bg-red-500 text-white text-[10px] flex items-center justify-center">
                <i class="fas fa-check"></i>
              </span>
            </button>

            <!-- My Events Toggle -->
            <button
              @click="showMyEventsOnly = !showMyEventsOnly; currentPage = 1"
              :class="[
                'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                showMyEventsOnly ? 'bg-purple-50 border-purple-200 text-purple-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
              ]"
            >
              <i class="fas fa-user-check text-sm"></i>
              <span>My Events</span>
              <span v-if="showMyEventsOnly" class="w-4 h-4 rounded-full bg-purple-500 text-white text-[10px] flex items-center justify-center">
                <i class="fas fa-check"></i>
              </span>
            </button>

            <!-- Clear Filters -->
            <button
              v-if="selectedTypes.length > 0 || selectedFormats.length > 0 || searchQuery || quickFilter !== 'all' || showMyEventsOnly || showFeaturedOnly || showInterestedOnly"
              @click="clearFilters"
              class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-medium text-red-600 bg-red-50 border border-red-200 hover:bg-red-100 transition-colors"
            >
              <i class="fas fa-times text-xs"></i>
              <span>Clear</span>
            </button>

            <!-- Spacer -->
            <div class="flex-1"></div>

            <!-- Sort Dropdown -->
            <div class="relative">
              <button
                @click="showSortDropdown = !showSortDropdown"
                class="flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border bg-white border-gray-200 text-gray-600 hover:bg-gray-50"
              >
                <i class="fas fa-sort text-sm"></i>
                <span>{{ sortOptions.find(s => s.value === sortBy)?.label || $t('common.sort') }}</span>
                <i :class="sortOrder === 'asc' ? 'fas fa-arrow-up' : 'fas fa-arrow-down'" class="text-[10px] ms-1"></i>
              </button>
              <!-- Dropdown Menu -->
              <div v-if="showSortDropdown" class="absolute end-0 top-full mt-2 w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50">
                <div class="px-3 py-1.5 text-[10px] font-semibold text-gray-400 uppercase tracking-wider">{{ $t('common.sortBy') }}</div>
                <button
                  v-for="option in sortOptions"
                  :key="option.value"
                  @click="sortBy = option.value; showSortDropdown = false"
                  :class="['w-full px-3 py-2 text-start text-xs flex items-center gap-2 hover:bg-gray-50', sortBy === option.value ? 'text-teal-600 bg-teal-50' : 'text-gray-600']"
                >
                  <i :class="option.icon" class="w-4"></i>
                  {{ option.label }}
                  <i v-if="sortBy === option.value" class="fas fa-check ms-auto text-teal-500"></i>
                </button>
                <div class="border-t border-gray-100 mt-2 pt-2">
                  <div class="px-3 py-1.5 text-[10px] font-semibold text-gray-400 uppercase tracking-wider">{{ $t('events.order') }}</div>
                  <button
                    @click="sortOrder = 'asc'; showSortDropdown = false"
                    :class="['w-full px-3 py-2 text-start text-xs flex items-center gap-2 hover:bg-gray-50', sortOrder === 'asc' ? 'text-teal-600 bg-teal-50' : 'text-gray-600']"
                  >
                    <i class="fas fa-arrow-up w-4"></i>
                    {{ $t('common.ascending') }}
                    <i v-if="sortOrder === 'asc'" class="fas fa-check ms-auto text-teal-500"></i>
                  </button>
                  <button
                    @click="sortOrder = 'desc'; showSortDropdown = false"
                    :class="['w-full px-3 py-2 text-start text-xs flex items-center gap-2 hover:bg-gray-50', sortOrder === 'desc' ? 'text-teal-600 bg-teal-50' : 'text-gray-600']"
                  >
                    <i class="fas fa-arrow-down w-4"></i>
                    {{ $t('common.descending') }}
                    <i v-if="sortOrder === 'desc'" class="fas fa-check ms-auto text-teal-500"></i>
                  </button>
                </div>
              </div>
              <div v-if="showSortDropdown" @click="showSortDropdown = false" class="fixed inset-0 z-40"></div>
            </div>

            <!-- View Toggle -->
            <div class="view-toggle">
              <button
                @click="calendarView = 'calendar'"
                :class="['px-2.5 py-1 rounded-md transition-all', calendarView === 'calendar' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
                title="Calendar view"
              >
                <i class="fas fa-calendar text-xs"></i>
              </button>
              <button
                @click="calendarView = 'grid'"
                :class="['px-2.5 py-1 rounded-md transition-all', calendarView === 'grid' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
                title="Grid view"
              >
                <i class="fas fa-th-large text-xs"></i>
              </button>
              <button
                @click="calendarView = 'list'"
                :class="['px-2.5 py-1 rounded-md transition-all', calendarView === 'list' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
                title="List view"
              >
                <i class="fas fa-list text-xs"></i>
              </button>
            </div>
          </div>
        </div>

        <!-- Content Area Inside Wrapper -->
        <div class="p-4">

      <!-- Content with Sidebar -->
      <div class="flex gap-6 content-with-sidebar">
        <!-- Main Content -->
        <div class="flex-1 min-w-0">

          <!-- Premium Calendar View -->
          <div v-if="calendarView === 'calendar'" class="calendar-premium">
            <!-- Enhanced Calendar Header -->
            <div class="calendar-header-premium">
              <div class="calendar-header-left">
                <div class="calendar-nav-premium">
                  <button @click="calendarMode === 'month' ? previousMonth() : previousWeek()" class="calendar-nav-btn-premium">
                    <i class="fas fa-chevron-left"></i>
                  </button>
                  <div class="calendar-title-wrapper">
                    <h2 class="calendar-title-premium">{{ currentMonthName }}</h2>
                    <span class="calendar-year-premium">{{ currentYear }}</span>
                  </div>
                  <button @click="calendarMode === 'month' ? nextMonth() : nextWeek()" class="calendar-nav-btn-premium">
                    <i class="fas fa-chevron-right"></i>
                  </button>
                </div>
                <!-- Month/Week/Today Toggle -->
                <div class="calendar-mode-toggle">
                  <button
                    @click="calendarMode = 'month'"
                    :class="['mode-btn', calendarMode === 'month' ? 'active' : '']"
                  >
                    <i class="fas fa-calendar-alt"></i>
                    Month
                  </button>
                  <button
                    @click="calendarMode = 'week'"
                    :class="['mode-btn', calendarMode === 'week' ? 'active' : '']"
                  >
                    <i class="fas fa-calendar-week"></i>
                    Week
                  </button>
                  <button
                    @click="goToToday"
                    class="mode-btn today-btn"
                  >
                    <i class="fas fa-crosshairs"></i>
                    Today
                  </button>
                </div>
              </div>
              <div class="calendar-header-right">
                <!-- Mini Stats -->
                <div class="calendar-stats">
                  <div class="stat-badge">
                    <i class="fas fa-calendar-check"></i>
                    <span>{{ calendarMonthStats.total }} events</span>
                  </div>
                  <div v-if="calendarMonthStats.featured > 0" class="stat-badge featured">
                    <i class="fas fa-star"></i>
                    <span>{{ calendarMonthStats.featured }} featured</span>
                  </div>
                  <div v-if="calendarMonthStats.going > 0" class="stat-badge going">
                    <i class="fas fa-check-circle"></i>
                    <span>{{ calendarMonthStats.going }} RSVP'd</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Category Legend -->
            <div class="calendar-legend">
              <div v-for="type in eventTypes" :key="type.id" class="legend-item">
                <span class="legend-dot" :style="{ background: type.color }"></span>
                <span class="legend-label">{{ type.name }}</span>
              </div>
            </div>

            <!-- Month View -->
            <template v-if="calendarMode === 'month'">
              <div class="calendar-weekdays-premium">
                <div v-for="day in weekDays" :key="day" class="calendar-weekday-premium">{{ day }}</div>
              </div>

              <div class="calendar-grid-premium">
                <div v-for="(day, index) in calendarDays" :key="index"
                     :class="['calendar-day-premium',
                              day.isToday ? 'is-today' : '',
                              !day.isCurrentMonth ? 'other-month' : '',
                              day.events.length > 0 ? 'has-events' : '']"
                     @click="selectDate(day)">
                  <span :class="['day-number-premium', day.isToday ? 'today-badge' : '']">{{ day.date }}</span>
                  <div class="day-events-premium">
                    <div v-for="event in day.events.slice(0, 3)" :key="event.id"
                         @click.stop="openEvent(event)"
                         :class="['day-event-premium', event.category]">
                      <div class="event-indicators">
                        <i v-if="event.featured" class="fas fa-star featured-indicator" title="Featured"></i>
                        <i v-if="event.isGoing" class="fas fa-check-circle going-indicator" title="RSVP'd"></i>
                      </div>
                      <span class="event-time-mini">{{ getShortTime(event.time) }}</span>
                      <span class="event-text">{{ event.title }}</span>
                    </div>
                    <div v-if="day.events.length > 3" class="day-more-premium">
                      +{{ day.events.length - 3 }} more
                    </div>
                  </div>
                </div>
              </div>
            </template>

            <!-- Week View -->
            <template v-else>
              <div class="calendar-weekdays-premium week-view">
                <div v-for="(day, idx) in weekViewDays" :key="idx" class="calendar-weekday-premium week-day-header">
                  <span class="weekday-name">{{ weekDays[idx] }}</span>
                  <span :class="['weekday-date', day.isToday ? 'today-date' : '']">{{ day.date }}</span>
                </div>
              </div>

              <div class="calendar-week-grid">
                <div v-for="(day, index) in weekViewDays" :key="index"
                     :class="['calendar-week-day',
                              day.isToday ? 'is-today' : '',
                              day.events.length > 0 ? 'has-events' : '']"
                     @click="selectDate(day)">
                  <div class="week-day-events">
                    <div v-for="event in day.events" :key="event.id"
                         @click.stop="openEvent(event)"
                         :class="['week-event-card', event.category]">
                      <div class="week-event-header">
                        <span class="week-event-time">{{ event.time }}</span>
                        <div class="week-event-badges">
                          <i v-if="event.featured" class="fas fa-star featured-indicator" title="Featured"></i>
                          <i v-if="event.isGoing" class="fas fa-check-circle going-indicator" title="RSVP'd"></i>
                          <i v-if="event.virtual" class="fas fa-video virtual-indicator" title="Virtual"></i>
                        </div>
                      </div>
                      <h4 class="week-event-title">{{ event.title }}</h4>
                      <p class="week-event-location">
                        <i :class="event.virtual ? 'fas fa-video' : 'fas fa-map-marker-alt'"></i>
                        {{ event.location }}
                      </p>
                      <div class="week-event-footer">
                        <div class="week-event-attendees">
                          <div v-for="(attendee, i) in event.attendees.slice(0, 3)" :key="i"
                               class="mini-avatar"
                               :style="{ backgroundColor: attendee.color }">
                            {{ attendee.initials }}
                          </div>
                          <span v-if="event.attendees.length > 3" class="more-attendees">+{{ event.attendees.length - 3 }}</span>
                        </div>
                        <button
                          @click.stop="toggleReserve(event.id)"
                          :class="['week-rsvp-btn', event.isGoing ? 'going' : '']"
                        >
                          <i :class="event.isGoing ? 'fas fa-check' : 'fas fa-plus'"></i>
                        </button>
                      </div>
                    </div>
                    <div v-if="day.events.length === 0" class="week-no-events">
                      <i class="far fa-calendar"></i>
                      <span>No events</span>
                    </div>
                  </div>
                </div>
              </div>
            </template>
          </div>

          <!-- Premium Grid View -->
          <div v-if="calendarView === 'grid'" class="events-grid-premium">
            <div v-for="event in paginatedEvents" :key="event.id"
                 @click="openEvent(event)"
                 :class="['event-card-premium', event.category]">
              <div class="event-card-glow"></div>
              <div class="event-card-header-premium">
                <CategoryBadge
                  :category="event.categoryLabel"
                  :category-id="event.category"
                  :icon="getEventTypeIcon(event.category)"
                  size="sm"
                  variant="gradient"
                />
                <div class="event-date-badge-premium">
                  <span class="event-date-month-premium">{{ event.month }}</span>
                  <span class="event-date-day-premium">{{ event.day }}</span>
                </div>
              </div>
              <div class="event-card-body-premium">
                <h3 class="event-card-title-premium">{{ event.title }}</h3>
                <div class="event-card-meta-premium">
                  <div class="event-meta-item-premium">
                    <div class="meta-icon-wrapper">
                      <i class="fas fa-clock"></i>
                    </div>
                    <span>{{ event.time }}</span>
                  </div>
                  <div class="event-meta-item-premium">
                    <div class="meta-icon-wrapper">
                      <i :class="event.virtual ? 'fas fa-video' : 'fas fa-map-marker-alt'"></i>
                    </div>
                    <span>{{ event.location }}</span>
                  </div>
                </div>
              </div>
              <div class="event-card-footer-premium">
                <div class="event-attendees-premium">
                  <div class="attendee-avatars-premium">
                    <div v-for="(attendee, i) in event.attendees.slice(0, 3)" :key="i"
                         class="attendee-avatar-premium"
                         :style="{ backgroundColor: attendee.color }">
                      {{ attendee.initials }}
                    </div>
                  </div>
                  <span class="attendee-count-premium">{{ event.attendees.length }} going</span>
                </div>
                <div class="card-action-buttons">
                  <button
                    @click.stop="toggleInterested(event.id)"
                    :class="['card-action-btn', event.interested ? 'interested-active' : '']"
                    :title="event.interested ? 'Remove Interest' : 'Mark as Interested'"
                  >
                    <i :class="event.interested ? 'fas fa-heart' : 'far fa-heart'"></i>
                  </button>
                  <button class="card-action-btn" @click.stop="shareEvent(event)" title="Share">
                    <i class="fas fa-share-alt"></i>
                  </button>
                  <ComparisonButton
                    :item="event"
                    type="event"
                    size="md"
                    variant="solid"
                    class="card-action-btn"
                  />
                  <button
                    @click.stop="toggleReserve(event.id)"
                    :class="['rsvp-btn-premium', event.isGoing ? 'going' : 'pending']"
                  >
                    <i :class="event.isGoing ? 'fas fa-check-circle' : 'fas fa-plus-circle'"></i>
                    {{ event.isGoing ? 'Going' : 'RSVP' }}
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Premium List View -->
          <div v-if="calendarView === 'list'" class="events-list-premium">
            <div v-for="event in paginatedEvents" :key="event.id"
                 @click="openEvent(event)"
                 :class="['event-list-item-premium', event.category]">
              <div class="event-list-date-premium">
                <span class="list-date-month">{{ event.month }}</span>
                <span class="list-date-day">{{ event.day }}</span>
              </div>
              <div class="event-list-content-premium">
                <h4 class="event-list-title-premium">{{ event.title }}</h4>
                <div class="event-list-details-premium">
                  <span :class="['event-list-badge', event.category]">
                    <i :class="getEventTypeIcon(event.category)"></i>
                    {{ event.categoryLabel }}
                  </span>
                  <span class="event-list-meta-item">
                    <i class="fas fa-clock"></i>
                    {{ event.time }}
                  </span>
                  <span class="event-list-meta-item">
                    <i :class="event.virtual ? 'fas fa-video' : 'fas fa-map-marker-alt'"></i>
                    {{ event.location }}
                  </span>
                </div>
              </div>
              <div class="event-list-actions-premium">
                <div class="list-attendee-avatars">
                  <div v-for="(attendee, i) in event.attendees.slice(0, 3)" :key="i"
                       class="list-attendee-avatar"
                       :style="{ backgroundColor: attendee.color }">
                    {{ attendee.initials }}
                  </div>
                </div>
                <div class="list-action-buttons">
                  <button
                    @click.stop="toggleInterested(event.id)"
                    :class="['list-action-btn', event.interested ? 'interested-active' : '']"
                    :title="event.interested ? 'Remove Interest' : 'Mark as Interested'"
                  >
                    <i :class="event.interested ? 'fas fa-heart' : 'far fa-heart'"></i>
                  </button>
                  <button class="list-action-btn" @click.stop="shareEvent(event)" title="Share">
                    <i class="fas fa-share-alt"></i>
                  </button>
                  <button class="list-action-btn" @click.stop title="Add to Calendar">
                    <i class="fas fa-calendar-plus"></i>
                  </button>
                  <button
                    @click.stop="toggleReserve(event.id)"
                    :class="['list-rsvp-btn', event.isGoing ? 'going' : 'pending']"
                  >
                    <i :class="event.isGoing ? 'fas fa-check' : 'fas fa-plus'"></i>
                    {{ event.isGoing ? 'Reserved' : 'Reserve' }}
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <EmptyState
            v-if="filteredEvents.length === 0 && calendarView !== 'calendar'"
            icon="fas fa-calendar-xmark"
            title="No events found"
            description="Try adjusting your filters or search query"
            action-label="Clear all filters"
            action-icon="fas fa-refresh"
            size="lg"
            @action="clearFilters"
          />

          <!-- Pagination -->
          <Pagination
            v-if="filteredEvents.length > 0 && calendarView !== 'calendar'"
            v-model:current-page="currentPage"
            v-model:items-per-page="itemsPerPage"
            :total-items="totalItems"
            :items-per-page-options="[6, 9, 12, 24]"
            class="mt-4"
          />
        </div>

        <!-- Sidebar -->
        <aside class="w-80 flex-shrink-0 content-sidebar hidden xl:block">
          <!-- My Upcoming Events -->
          <div v-if="myUpcomingEvents.length > 0" class="sidebar-section my-events-section">
            <div class="sidebar-title">
              <i class="fas fa-user-check"></i>
              My Upcoming Events
            </div>
            <div class="my-events-list">
              <div v-for="event in myUpcomingEvents" :key="event.id"
                   @click="openEvent(event)"
                   class="my-event-item">
                <div class="my-event-date">
                  <span class="my-event-day">{{ event.day }}</span>
                  <span class="my-event-month">{{ event.month }}</span>
                </div>
                <div class="my-event-info">
                  <h4 class="my-event-title">{{ event.title }}</h4>
                  <div class="my-event-meta">
                    <span><i class="fas fa-clock"></i> {{ event.time }}</span>
                  </div>
                </div>
                <div class="my-event-actions">
                  <button class="my-event-action-btn" title="View Details">
                    <i class="fas fa-arrow-right"></i>
                  </button>
                </div>
              </div>
            </div>
            <button class="view-all-my-events" @click="quickFilter = 'myevents'">
              <span>View all my events</span>
              <i class="fas fa-chevron-right"></i>
            </button>
          </div>

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
                <div class="flex items-center justify-between mb-1">
                  <label class="block text-sm font-medium text-gray-700">Description</label>
                  <button @click="generateEventDescription()"
                          type="button"
                          class="px-3 py-1 text-xs font-medium text-teal-600 bg-teal-50 rounded-lg hover:bg-teal-100 transition-colors flex items-center gap-1">
                    <i class="fas fa-wand-magic-sparkles"></i>
                    AI Generate
                  </button>
                </div>
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
              {{ $t('common.cancel') }}
            </button>
            <button @click="createEvent"
                    class="px-4 py-2 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-lg hover:from-teal-600 hover:to-teal-700 transition-all">
              <i class="fas fa-plus me-2"></i> {{ $t('events.createEvent') }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- Day Detail Popup -->
    <Teleport to="body">
      <Transition name="popup-fade">
        <div v-if="showDayDetailPopup" class="day-popup-overlay" @click.self="closeDayDetailPopup">
          <div class="day-popup-content">
            <!-- Popup Header -->
            <div class="day-popup-header">
              <div class="day-popup-date">
                <div class="popup-date-badge" :class="{ 'is-today': selectedCalendarDay?.isToday }">
                  <span class="popup-date-day">{{ selectedCalendarDay?.date }}</span>
                  <span class="popup-date-month">{{ currentMonthName.slice(0, 3) }}</span>
                </div>
                <div class="popup-date-info">
                  <h3 class="popup-title">{{ selectedDayFormatted }}</h3>
                  <p class="popup-subtitle">
                    {{ selectedCalendarDay?.events.length || 0 }} event{{ (selectedCalendarDay?.events.length || 0) !== 1 ? 's' : '' }}
                  </p>
                </div>
              </div>
              <button @click="closeDayDetailPopup" class="popup-close-btn">
                <i class="fas fa-times"></i>
              </button>
            </div>

            <!-- Popup Body -->
            <div class="day-popup-body">
              <div v-if="selectedCalendarDay?.events.length === 0" class="popup-empty">
                <div class="empty-icon">
                  <i class="far fa-calendar-times"></i>
                </div>
                <p class="empty-text">{{ $t('events.noEventsScheduled') }}</p>
                <button @click="showCreateModal = true; closeDayDetailPopup()" class="popup-add-btn">
                  <i class="fas fa-plus"></i>
                  {{ $t('events.addEvent') }}
                </button>
              </div>

              <div v-else class="popup-events-list">
                <div v-for="event in selectedCalendarDay?.events" :key="event.id"
                     :class="['popup-event-card', event.category]"
                     @click="openEvent(event); closeDayDetailPopup()">
                  <div class="popup-event-time">
                    <i class="fas fa-clock"></i>
                    {{ event.time }}
                  </div>
                  <div class="popup-event-main">
                    <div class="popup-event-badges">
                      <CategoryBadge :category="event.categoryLabel" :category-id="event.category" size="xs" />
                      <StatusBadge v-if="event.featured" status="featured" size="xs" variant="gradient" />
                      <StatusBadge v-if="event.virtual" status="virtual" size="xs" variant="solid" />
                    </div>
                    <h4 class="popup-event-title">{{ event.title }}</h4>
                    <p class="popup-event-location">
                      <i :class="event.virtual ? 'fas fa-video' : 'fas fa-map-marker-alt'"></i>
                      {{ event.location }}
                    </p>
                    <p class="popup-event-desc">{{ event.description }}</p>
                  </div>
                  <div class="popup-event-footer">
                    <div class="popup-attendees">
                      <div v-for="(attendee, i) in event.attendees.slice(0, 4)" :key="i"
                           class="popup-attendee"
                           :style="{ backgroundColor: attendee.color }">
                        {{ attendee.initials }}
                      </div>
                      <span v-if="event.attendees.length > 4" class="popup-more-attendees">
                        +{{ event.attendees.length - 4 }}
                      </span>
                      <span class="popup-attendee-count">{{ event.attendees.length }} {{ $t('events.going') }}</span>
                    </div>
                    <div class="popup-actions">
                      <button
                        @click.stop="toggleInterested(event.id)"
                        :class="['popup-action-btn', event.interested ? 'interested' : '']"
                        :title="event.interested ? 'Remove Interest' : 'Mark as Interested'"
                      >
                        <i :class="event.interested ? 'fas fa-heart' : 'far fa-heart'"></i>
                      </button>
                      <button
                        @click.stop="toggleReserve(event.id)"
                        :class="['popup-rsvp-btn', event.isGoing ? 'going' : '']"
                      >
                        <i :class="event.isGoing ? 'fas fa-check-circle' : 'fas fa-plus-circle'"></i>
                        {{ event.isGoing ? $t('events.going') : $t('events.rsvp') }}
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>

    <!-- AI Smart Scheduling Modal -->
    <Teleport to="body">
      <div v-if="showSmartScheduleModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[80vh] overflow-hidden">
          <div class="p-6 border-b border-gray-100">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 bg-gradient-to-br from-teal-500 to-emerald-500 rounded-xl flex items-center justify-center">
                  <i class="fas fa-wand-magic-sparkles text-white"></i>
                </div>
                <div>
                  <h3 class="text-lg font-semibold text-gray-900">{{ $t('events.aiSmartScheduling') }}</h3>
                  <p class="text-sm text-gray-500">{{ $t('events.findOptimalTime') }}</p>
                </div>
              </div>
              <button @click="showSmartScheduleModal = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                <i class="fas fa-times text-gray-400"></i>
              </button>
            </div>
          </div>

          <div class="p-6 overflow-y-auto max-h-[60vh]">
            <AILoadingIndicator v-if="isAnalyzingSchedule" message="Analyzing calendars and finding optimal times..." />

            <div v-else-if="scheduleSuggestions.length > 0" class="space-y-4">
              <div class="bg-gradient-to-r from-teal-50 to-emerald-50 rounded-xl p-4 mb-4">
                <div class="flex items-center gap-2 text-teal-700 font-medium mb-1">
                  <i class="fas fa-lightbulb"></i>
                  {{ $t('events.aiAnalysisComplete') }}
                </div>
                <p class="text-sm text-gray-600">{{ $t('events.aiAnalysisDescription') }}</p>
              </div>

              <div v-for="suggestion in scheduleSuggestions" :key="suggestion.id"
                   class="border border-gray-200 rounded-xl p-4 hover:border-teal-300 hover:bg-teal-50/30 transition-all cursor-pointer"
                   @click="applyScheduleSuggestion(suggestion)">
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <div class="flex items-center gap-3 mb-2">
                      <span :class="['px-2 py-1 rounded-full text-xs font-semibold capitalize', getRecommendationColor(suggestion.recommendation)]">
                        {{ suggestion.recommendation }}
                      </span>
                      <span class="text-lg font-semibold text-gray-900">{{ suggestion.timeSlot }}</span>
                    </div>
                    <div class="text-sm text-gray-600 mb-2">
                      <i class="fas fa-calendar me-1"></i>
                      {{ new Date(suggestion.date).toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric' }) }}
                    </div>
                    <p class="text-sm text-gray-600">{{ suggestion.reason }}</p>
                    <div v-if="suggestion.conflicts.length > 0" class="mt-2 flex flex-wrap gap-2">
                      <span v-for="conflict in suggestion.conflicts" :key="conflict"
                            class="px-2 py-1 bg-amber-100 text-amber-700 rounded text-xs">
                        <i class="fas fa-exclamation-triangle me-1"></i>{{ conflict }}
                      </span>
                    </div>
                  </div>
                  <div class="text-end">
                    <div class="text-2xl font-bold text-teal-600">{{ Math.round(suggestion.score * 100) }}%</div>
                    <div class="text-xs text-gray-500">{{ $t('events.matchScore') }}</div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="p-4 border-t border-gray-100 flex justify-end gap-3">
            <button @click="analyzeSmartSchedule"
                    class="px-4 py-2 text-teal-600 hover:bg-teal-50 rounded-lg transition-colors flex items-center gap-2">
              <i class="fas fa-rotate"></i> {{ $t('common.refresh') }}
            </button>
            <button @click="showSmartScheduleModal = false"
                    class="px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors">
              {{ $t('common.close') }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- AI Event Description Generator Modal -->
    <Teleport to="body">
      <div v-if="showAIDescriptionModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[80vh] overflow-hidden">
          <div class="p-6 border-b border-gray-100">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 bg-gradient-to-br from-purple-500 to-pink-500 rounded-xl flex items-center justify-center">
                  <i class="fas fa-pen-fancy text-white"></i>
                </div>
                <div>
                  <h3 class="text-lg font-semibold text-gray-900">AI Event Description</h3>
                  <p class="text-sm text-gray-500">Generate compelling event content</p>
                </div>
              </div>
              <button @click="showAIDescriptionModal = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                <i class="fas fa-times text-gray-400"></i>
              </button>
            </div>
          </div>

          <div class="p-6 overflow-y-auto max-h-[60vh]">
            <AILoadingIndicator v-if="isGeneratingDescription" message="Generating event description..." />

            <div v-else-if="generatedDescription" class="space-y-6">
              <!-- Generated Title -->
              <div>
                <h5 class="font-medium text-gray-900 mb-2 flex items-center gap-2">
                  <i class="fas fa-heading text-purple-500"></i> Suggested Title
                </h5>
                <div class="bg-purple-50 rounded-lg p-3">
                  <p class="text-gray-900 font-medium">{{ generatedDescription.title }}</p>
                </div>
              </div>

              <!-- Generated Description -->
              <div>
                <h5 class="font-medium text-gray-900 mb-2 flex items-center gap-2">
                  <i class="fas fa-align-left text-purple-500"></i> Description
                </h5>
                <div class="bg-gray-50 rounded-lg p-4">
                  <p class="text-gray-700 whitespace-pre-line">{{ generatedDescription.description }}</p>
                </div>
              </div>

              <!-- Highlights -->
              <div>
                <h5 class="font-medium text-gray-900 mb-2 flex items-center gap-2">
                  <i class="fas fa-list-check text-purple-500"></i> Key Highlights
                </h5>
                <ul class="space-y-2">
                  <li v-for="(highlight, idx) in generatedDescription.highlights" :key="idx"
                      class="flex items-start gap-2 text-gray-700">
                    <i class="fas fa-check-circle text-purple-500 mt-1 text-sm"></i>
                    <span>{{ highlight }}</span>
                  </li>
                </ul>
              </div>

              <!-- Target Audience -->
              <div>
                <h5 class="font-medium text-gray-900 mb-2 flex items-center gap-2">
                  <i class="fas fa-users text-purple-500"></i> Target Audience
                </h5>
                <p class="text-gray-700 bg-gray-50 rounded-lg p-3">{{ generatedDescription.targetAudience }}</p>
              </div>

              <!-- Suggested Tags -->
              <div>
                <h5 class="font-medium text-gray-900 mb-2 flex items-center gap-2">
                  <i class="fas fa-tags text-purple-500"></i> Suggested Tags
                </h5>
                <div class="flex flex-wrap gap-2">
                  <span v-for="tag in generatedDescription.tags" :key="tag"
                        class="px-3 py-1 bg-purple-100 text-purple-700 rounded-full text-sm">
                    {{ tag }}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <div class="p-4 border-t border-gray-100 flex justify-end gap-3">
            <button @click="generateEventDescription()"
                    class="px-4 py-2 text-purple-600 hover:bg-purple-50 rounded-lg transition-colors flex items-center gap-2">
              <i class="fas fa-rotate"></i> Regenerate
            </button>
            <button @click="applyGeneratedDescription"
                    class="px-4 py-2 bg-purple-500 text-white rounded-lg hover:bg-purple-600 transition-colors flex items-center gap-2">
              <i class="fas fa-check"></i> Apply
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- AI Related Events Panel -->
    <Teleport to="body">
      <div v-if="showRelatedEventsPanel" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-lg max-h-[80vh] overflow-hidden">
          <div class="p-6 border-b border-gray-100">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 bg-gradient-to-br from-blue-500 to-indigo-500 rounded-xl flex items-center justify-center">
                  <i class="fas fa-robot text-white"></i>
                </div>
                <div>
                  <h3 class="text-lg font-semibold text-gray-900">AI Suggestions</h3>
                  <p class="text-sm text-gray-500">Related events you might like</p>
                </div>
              </div>
              <button @click="showRelatedEventsPanel = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                <i class="fas fa-times text-gray-400"></i>
              </button>
            </div>
          </div>

          <div class="p-6 overflow-y-auto max-h-[60vh]">
            <AILoadingIndicator v-if="isFetchingRelated" message="Finding related events..." />

            <div v-else-if="relatedEvents.length > 0" class="space-y-4">
              <div class="bg-gradient-to-r from-blue-50 to-indigo-50 rounded-xl p-4 mb-4">
                <div class="flex items-center gap-2 text-blue-700 font-medium mb-1">
                  <i class="fas fa-brain"></i>
                  Personalized Recommendations
                </div>
                <p class="text-sm text-gray-600">Based on your interests and activity patterns</p>
              </div>

              <div v-for="event in relatedEvents" :key="event.id"
                   class="border border-gray-200 rounded-xl p-4 hover:border-blue-300 hover:bg-blue-50/30 transition-all cursor-pointer">
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <div class="flex items-center gap-2 mb-2">
                      <div class="w-8 h-8 rounded-lg flex items-center justify-center"
                           :style="{ backgroundColor: getCategoryColor(event.category) + '20' }">
                        <i :class="getCategoryIcon(event.category)"
                           :style="{ color: getCategoryColor(event.category) }"></i>
                      </div>
                      <h4 class="font-semibold text-gray-900">{{ event.title }}</h4>
                    </div>
                    <div class="text-sm text-gray-600 mb-2">
                      <i class="fas fa-calendar me-1"></i>
                      {{ new Date(event.date).toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' }) }}
                    </div>
                    <p class="text-sm text-gray-500">{{ event.reason }}</p>
                  </div>
                  <div class="text-end ms-4">
                    <div class="text-xl font-bold text-blue-600">{{ Math.round(event.similarity * 100) }}%</div>
                    <div class="text-xs text-gray-500">Match</div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="p-4 border-t border-gray-100 flex justify-end gap-3">
            <button @click="fetchRelatedEvents()"
                    class="px-4 py-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors flex items-center gap-2">
              <i class="fas fa-rotate"></i> Refresh
            </button>
            <button @click="showRelatedEventsPanel = false"
                    class="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-colors">
              Close
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- Share Modal -->
    <ShareContentModal
      v-model="showShareModal"
      :title="selectedEventForShare?.title || ''"
      :description="selectedEventForShare?.description"
      :image="selectedEventForShare?.image"
      :url="shareEventUrl"
      content-type="Event"
    />

    <!-- Comparison Components -->
    <ComparisonPanel />
    <ComparisonModal />
  </div>
</template>

<style scoped>
/* Events Page Container */
.events-page {
  min-height: 100vh;
  background: #f9fafb;
}

/* Events Content Area */
.events-content {
  padding: 1.5rem 2rem;
  width: 100%;
}

/* Toolbar */
.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.25rem;
  gap: 1rem;
  flex-wrap: wrap;
  width: 100%;
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
  padding-inline-start: 2.75rem;
  padding-inline-end: 1rem;
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
  width: 100%;
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
  margin-inline-start: -8px;
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
  margin-inline-start: -8px;
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

/* ===============================================
   PREMIUM EVENTS PAGE STYLES
   =============================================== */

/* Premium Featured Event */
.featured-event-premium {
  position: relative;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95) 0%, rgba(240, 253, 250, 0.95) 100%);
  border-radius: 1.25rem;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  border: 1px solid rgba(20, 184, 166, 0.15);
  overflow: hidden;
  backdrop-filter: blur(10px);
  width: 100%;
}

.featured-event-bg {
  position: absolute;
  inset: 0;
  pointer-events: none;
}

.featured-orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(50px);
  opacity: 0.3;
}

.featured-orb-1 {
  width: 200px;
  height: 200px;
  background: radial-gradient(circle, #14b8a6 0%, transparent 70%);
  top: -80px;
  right: -50px;
}

.featured-orb-2 {
  width: 150px;
  height: 150px;
  background: radial-gradient(circle, #6366f1 0%, transparent 70%);
  bottom: -60px;
  left: 20%;
}

.featured-event-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
  position: relative;
  z-index: 1;
}

.featured-badge-premium {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  border-radius: 9999px;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.featured-badge-premium i {
  font-size: 0.6875rem;
  animation: twinkle 1.5s ease-in-out infinite;
}

@keyframes twinkle {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.6; transform: scale(0.8); }
}

.featured-date-badge {
  width: 64px;
  height: 64px;
  background: white;
  border-radius: 1rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1);
  border: 2px solid rgba(20, 184, 166, 0.2);
}

.featured-date-month {
  font-size: 0.6875rem;
  font-weight: 700;
  text-transform: uppercase;
  color: #14b8a6;
  letter-spacing: 0.05em;
}

.featured-date-day {
  font-size: 1.5rem;
  font-weight: 900;
  color: #0f172a;
  line-height: 1;
}

.featured-event-body {
  position: relative;
  z-index: 1;
  margin-bottom: 1.25rem;
}

.featured-event-title {
  font-size: 1.375rem;
  font-weight: 800;
  color: #0f172a;
  margin: 0 0 0.5rem;
  line-height: 1.3;
}

.featured-event-description {
  font-size: 0.9375rem;
  color: #64748b;
  margin: 0 0 1rem;
  line-height: 1.5;
}

.featured-event-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
}

.featured-meta-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  color: #475569;
}

.featured-meta-item.virtual {
  color: #6366f1;
}

.featured-meta-icon {
  width: 32px;
  height: 32px;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #14b8a6;
  font-size: 0.75rem;
}

.featured-meta-item.virtual .featured-meta-icon {
  background: linear-gradient(135deg, #eef2ff 0%, #e0e7ff 100%);
  color: #6366f1;
}

.featured-event-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 1rem;
  border-top: 1px solid rgba(20, 184, 166, 0.1);
  position: relative;
  z-index: 1;
}

.featured-attendees {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.featured-avatar-stack {
  display: flex;
  margin-inline-start: -8px;
}

.featured-avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border: 3px solid white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.6875rem;
  font-weight: 700;
  color: white;
  margin-inline-start: -8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.featured-attendee-text {
  font-size: 0.8125rem;
  color: #64748b;
  font-weight: 500;
}

.featured-rsvp-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 0.75rem;
  font-size: 0.875rem;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.featured-rsvp-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(20, 184, 166, 0.4);
}

/* Premium Event Cards Grid */
.events-grid-premium {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 1.25rem;
  width: 100%;
}

.event-card-premium {
  position: relative;
  background: white;
  border-radius: 1.25rem;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06);
  border: 1px solid #f1f5f9;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  cursor: pointer;
}

.event-card-premium:hover {
  transform: translateY(-6px);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.12);
}

.event-card-glow {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.event-card-premium:hover .event-card-glow {
  opacity: 1;
}

.event-card-premium.meeting .event-card-glow { background: linear-gradient(90deg, #3b82f6, #60a5fa); }
.event-card-premium.training .event-card-glow { background: linear-gradient(90deg, #10b981, #34d399); }
.event-card-premium.social .event-card-glow { background: linear-gradient(90deg, #ec4899, #f472b6); }
.event-card-premium.review .event-card-glow { background: linear-gradient(90deg, #f59e0b, #fbbf24); }
.event-card-premium.webinar .event-card-glow { background: linear-gradient(90deg, #6366f1, #818cf8); }

.event-card-premium.meeting:hover { border-color: rgba(59, 130, 246, 0.3); }
.event-card-premium.training:hover { border-color: rgba(16, 185, 129, 0.3); }
.event-card-premium.social:hover { border-color: rgba(236, 72, 153, 0.3); }
.event-card-premium.review:hover { border-color: rgba(245, 158, 11, 0.3); }
.event-card-premium.webinar:hover { border-color: rgba(99, 102, 241, 0.3); }

.event-card-header-premium {
  position: relative;
  padding: 1.25rem;
  background: linear-gradient(135deg, #f8fafc 0%, #f0fdfa 100%);
  border-bottom: 1px solid rgba(20, 184, 166, 0.08);
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.event-type-badge-premium {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.6875rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.03em;
}

.event-type-badge-premium.meeting { background: linear-gradient(135deg, #dbeafe, #bfdbfe); color: #1e40af; }
.event-type-badge-premium.training { background: linear-gradient(135deg, #d1fae5, #a7f3d0); color: #065f46; }
.event-type-badge-premium.social { background: linear-gradient(135deg, #fce7f3, #fbcfe8); color: #9d174d; }
.event-type-badge-premium.review { background: linear-gradient(135deg, #fef3c7, #fde68a); color: #92400e; }
.event-type-badge-premium.webinar { background: linear-gradient(135deg, #e0e7ff, #c7d2fe); color: #3730a3; }

.event-date-badge-premium {
  width: 56px;
  height: 56px;
  background: white;
  border-radius: 0.875rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
  border: 1px solid rgba(20, 184, 166, 0.1);
}

.event-date-month-premium {
  font-size: 0.5625rem;
  font-weight: 800;
  text-transform: uppercase;
  color: #14b8a6;
  letter-spacing: 0.05em;
}

.event-date-day-premium {
  font-size: 1.375rem;
  font-weight: 900;
  color: #0f172a;
  line-height: 1;
}

.event-card-body-premium {
  padding: 1.25rem;
}

.event-card-title-premium {
  font-size: 1rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0 0 0.75rem;
  line-height: 1.4;
  transition: color 0.3s ease;
}

.event-card-premium:hover .event-card-title-premium {
  color: #0d9488;
}

.event-card-meta-premium {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.event-meta-item-premium {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  font-size: 0.8125rem;
  color: #64748b;
}

.meta-icon-wrapper {
  width: 28px;
  height: 28px;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #14b8a6;
  font-size: 0.6875rem;
  flex-shrink: 0;
}

.event-card-footer-premium {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.25rem;
  background: #fafafa;
  border-top: 1px solid #f1f5f9;
}

.event-attendees-premium {
  display: flex;
  align-items: center;
  gap: 0.625rem;
}

.attendee-avatars-premium {
  display: flex;
  margin-inline-start: -6px;
}

.attendee-avatar-premium {
  width: 30px;
  height: 30px;
  border-radius: 50%;
  border: 2px solid white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.625rem;
  font-weight: 700;
  color: white;
  margin-inline-start: -6px;
  transition: transform 0.3s ease;
}

.event-card-premium:hover .attendee-avatar-premium {
  transform: translateY(-2px);
}

.attendee-count-premium {
  font-size: 0.75rem;
  color: #64748b;
  font-weight: 500;
}

.rsvp-btn-premium {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 1rem;
  border-radius: 0.625rem;
  font-size: 0.75rem;
  font-weight: 700;
  border: none;
  cursor: pointer;
  transition: all 0.3s ease;
}

.rsvp-btn-premium.going {
  background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
  color: #065f46;
}

.rsvp-btn-premium.pending {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.25);
}

.rsvp-btn-premium:hover {
  transform: scale(1.05);
}

/* Premium List View */
.events-list-premium {
  display: flex;
  flex-direction: column;
  gap: 0.875rem;
  width: 100%;
}

.event-list-item-premium {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  padding: 1.25rem;
  background: white;
  border-radius: 1rem;
  border: 1px solid #f1f5f9;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  cursor: pointer;
  position: relative;
  overflow: hidden;
}

.event-list-item-premium::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 4px;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.event-list-item-premium:hover::before {
  opacity: 1;
}

.event-list-item-premium.meeting::before { background: #3b82f6; }
.event-list-item-premium.training::before { background: #10b981; }
.event-list-item-premium.social::before { background: #ec4899; }
.event-list-item-premium.review::before { background: #f59e0b; }
.event-list-item-premium.webinar::before { background: #6366f1; }

.event-list-item-premium:hover {
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
  transform: translateX(4px);
}

.event-list-date-premium {
  width: 60px;
  height: 60px;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-radius: 0.875rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  border: 1px solid rgba(20, 184, 166, 0.1);
}

.list-date-month {
  font-size: 0.625rem;
  font-weight: 800;
  text-transform: uppercase;
  color: #14b8a6;
  letter-spacing: 0.05em;
}

.list-date-day {
  font-size: 1.375rem;
  font-weight: 900;
  color: #0f172a;
  line-height: 1;
}

.event-list-content-premium {
  flex: 1;
  min-width: 0;
}

.event-list-title-premium {
  font-size: 1rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0 0 0.375rem;
  transition: color 0.3s ease;
}

.event-list-item-premium:hover .event-list-title-premium {
  color: #0d9488;
}

.event-list-details-premium {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  align-items: center;
}

.event-list-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.625rem;
  border-radius: 9999px;
  font-size: 0.6875rem;
  font-weight: 600;
}

.event-list-badge.meeting { background: #dbeafe; color: #1e40af; }
.event-list-badge.training { background: #d1fae5; color: #065f46; }
.event-list-badge.social { background: #fce7f3; color: #9d174d; }
.event-list-badge.review { background: #fef3c7; color: #92400e; }
.event-list-badge.webinar { background: #e0e7ff; color: #3730a3; }

.event-list-meta-item {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.8125rem;
  color: #64748b;
}

.event-list-meta-item i {
  color: #14b8a6;
  font-size: 0.75rem;
}

.event-list-actions-premium {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.list-attendee-avatars {
  display: flex;
  margin-inline-start: -6px;
}

.list-attendee-avatar {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  border: 2px solid white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.5625rem;
  font-weight: 700;
  color: white;
  margin-inline-start: -6px;
}

.list-rsvp-btn {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  font-size: 0.75rem;
  font-weight: 700;
  border: none;
  cursor: pointer;
  transition: all 0.3s ease;
}

.list-rsvp-btn.going {
  background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
  color: #065f46;
}

.list-rsvp-btn.pending {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
}

.list-rsvp-btn:hover {
  transform: scale(1.05);
}

/* Premium Empty State */
.empty-state-premium {
  text-align: center;
  padding: 4rem 2rem;
}

.empty-state-icon {
  position: relative;
  width: 80px;
  height: 80px;
  margin: 0 auto 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.empty-state-icon-bg {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-radius: 1.25rem;
  animation: pulse 2s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { transform: scale(1); opacity: 1; }
  50% { transform: scale(1.05); opacity: 0.8; }
}

.empty-state-icon i {
  position: relative;
  z-index: 1;
  font-size: 2rem;
  color: #14b8a6;
}

.empty-state-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0 0 0.5rem;
}

.empty-state-text {
  font-size: 0.9375rem;
  color: #64748b;
  margin: 0 0 1.5rem;
}

.empty-state-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 0.75rem;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.empty-state-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(20, 184, 166, 0.4);
}

/* Responsive Updates for Premium Styles */
@media (max-width: 768px) {
  .featured-event-premium {
    padding: 1.25rem;
  }

  .featured-event-header {
    flex-direction: column;
    gap: 1rem;
  }

  .featured-date-badge {
    align-self: flex-start;
  }

  .featured-event-meta {
    flex-direction: column;
    gap: 0.75rem;
  }

  .featured-event-footer {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }

  .events-grid-premium {
    grid-template-columns: 1fr;
  }

  .event-list-item-premium {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .event-list-actions-premium {
    width: 100%;
    justify-content: space-between;
  }
}

/* ===============================================
   PREMIUM CALENDAR VIEW STYLES
   =============================================== */

.calendar-premium {
  background: white;
  border-radius: 1.25rem;
  overflow: hidden;
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.06);
  border: 1px solid rgba(20, 184, 166, 0.1);
  width: 100%;
}

.calendar-header-premium {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.25rem 1.5rem;
  background: linear-gradient(135deg, #f8fafc 0%, #f0fdfa 100%);
  border-bottom: 1px solid rgba(20, 184, 166, 0.08);
}

.calendar-nav-premium {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.calendar-nav-btn-premium {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  color: #64748b;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  font-size: 0.875rem;
}

.calendar-nav-btn-premium:hover {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-color: #14b8a6;
  color: #0d9488;
  transform: scale(1.05);
}

.calendar-title-wrapper {
  display: flex;
  align-items: baseline;
  gap: 0.5rem;
  min-width: 200px;
  justify-content: center;
}

.calendar-title-premium {
  font-size: 1.375rem;
  font-weight: 800;
  color: #0f172a;
  margin: 0;
}

.calendar-year-premium {
  font-size: 1rem;
  font-weight: 600;
  color: #14b8a6;
}

.calendar-today-btn-premium {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1.25rem;
  background: white;
  border: 1.5px solid #e2e8f0;
  border-radius: 0.75rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: #475569;
  cursor: pointer;
  transition: all 0.3s ease;
}

.calendar-today-btn-premium:hover {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-color: transparent;
  color: white;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.calendar-today-btn-premium i {
  font-size: 0.75rem;
}

.calendar-weekdays-premium {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  border-bottom: 1px solid #f1f5f9;
}

.calendar-weekday-premium {
  padding: 1rem;
  text-align: center;
  font-size: 0.75rem;
  font-weight: 700;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.08em;
}

.calendar-grid-premium {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
}

.calendar-day-premium {
  min-height: 110px;
  padding: 0.625rem;
  border-right: 1px solid #f1f5f9;
  border-bottom: 1px solid #f1f5f9;
  transition: all 0.3s ease;
  cursor: pointer;
  position: relative;
}

.calendar-day-premium:nth-child(7n) {
  border-right: none;
}

.calendar-day-premium:hover {
  background: linear-gradient(135deg, rgba(20, 184, 166, 0.03) 0%, rgba(20, 184, 166, 0.06) 100%);
}

.calendar-day-premium.is-today {
  background: linear-gradient(135deg, rgba(20, 184, 166, 0.08) 0%, rgba(20, 184, 166, 0.04) 100%);
}

.calendar-day-premium.other-month {
  background: #fafafa;
}

.calendar-day-premium.other-month .day-number-premium {
  color: #cbd5e1;
}

.calendar-day-premium.has-events::after {
  content: '';
  position: absolute;
  bottom: 4px;
  left: 50%;
  transform: translateX(-50%);
  width: 4px;
  height: 4px;
  background: #14b8a6;
  border-radius: 50%;
  opacity: 0.5;
}

.day-number-premium {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  font-size: 0.875rem;
  font-weight: 600;
  color: #334155;
  border-radius: 0.625rem;
  margin-bottom: 0.5rem;
  transition: all 0.3s ease;
}

.day-number-premium.today-badge {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  box-shadow: 0 4px 10px rgba(20, 184, 166, 0.35);
  font-weight: 700;
}

.day-events-premium {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.day-event-premium {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.25rem 0.5rem;
  border-radius: 0.375rem;
  font-size: 0.6875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  overflow: hidden;
}

.day-event-premium:hover {
  transform: translateX(2px);
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

.event-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  flex-shrink: 0;
}

.event-text {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.day-event-premium.meeting {
  background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%);
  color: #1e40af;
}
.day-event-premium.meeting .event-dot { background: #3b82f6; }

.day-event-premium.training {
  background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
  color: #065f46;
}
.day-event-premium.training .event-dot { background: #10b981; }

.day-event-premium.social {
  background: linear-gradient(135deg, #fce7f3 0%, #fbcfe8 100%);
  color: #9d174d;
}
.day-event-premium.social .event-dot { background: #ec4899; }

.day-event-premium.review {
  background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
  color: #92400e;
}
.day-event-premium.review .event-dot { background: #f59e0b; }

.day-event-premium.webinar {
  background: linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%);
  color: #3730a3;
}
.day-event-premium.webinar .event-dot { background: #6366f1; }

.day-more-premium {
  font-size: 0.625rem;
  font-weight: 600;
  color: #14b8a6;
  padding: 0.25rem 0.5rem;
  cursor: pointer;
  transition: all 0.2s ease;
  border-radius: 0.25rem;
}

.day-more-premium:hover {
  background: rgba(20, 184, 166, 0.1);
  color: #0d9488;
}

/* Calendar Responsive */
@media (max-width: 1024px) {
  .calendar-day-premium {
    min-height: 90px;
    padding: 0.5rem;
  }

  .day-number-premium {
    width: 28px;
    height: 28px;
    font-size: 0.8125rem;
  }

  .day-event-premium {
    font-size: 0.625rem;
    padding: 0.1875rem 0.375rem;
  }

  .event-dot {
    width: 5px;
    height: 5px;
  }
}

@media (max-width: 768px) {
  .calendar-header-premium {
    flex-direction: column;
    gap: 1rem;
    padding: 1rem;
  }

  .calendar-nav-premium {
    width: 100%;
    justify-content: space-between;
  }

  .calendar-title-wrapper {
    min-width: auto;
  }

  .calendar-today-btn-premium {
    width: 100%;
    justify-content: center;
  }

  .calendar-weekday-premium {
    padding: 0.75rem 0.25rem;
    font-size: 0.6875rem;
  }

  .calendar-day-premium {
    min-height: 70px;
    padding: 0.25rem;
  }

  .day-number-premium {
    width: 24px;
    height: 24px;
    font-size: 0.75rem;
    margin-bottom: 0.25rem;
  }

  .day-events-premium {
    display: none;
  }

  .calendar-day-premium.has-events::after {
    opacity: 1;
    width: 6px;
    height: 6px;
  }
}

/* ===============================================
   ENHANCED CALENDAR HEADER
   =============================================== */

.calendar-header-premium {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.25rem 1.5rem;
  background: linear-gradient(135deg, #f8fafc 0%, #f0fdfa 100%);
  border-bottom: 1px solid rgba(20, 184, 166, 0.08);
  flex-wrap: wrap;
  gap: 1rem;
}

.calendar-header-left {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  flex-wrap: wrap;
}

.calendar-header-right {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
}

/* Month/Week Toggle */
.calendar-mode-toggle {
  display: flex;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 0.625rem;
  overflow: hidden;
}

.mode-btn {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 0.875rem;
  border: none;
  background: transparent;
  font-size: 0.75rem;
  font-weight: 600;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
}

.mode-btn:hover {
  background: #f8fafc;
  color: #475569;
}

.mode-btn.active {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
}

.mode-btn i {
  font-size: 0.6875rem;
}

.mode-btn.today-btn {
  border-left: 1px solid #e2e8f0;
}

.mode-btn.today-btn:hover {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
}

/* Calendar Stats */
.calendar-stats {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.stat-badge {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 999px;
  font-size: 0.6875rem;
  font-weight: 600;
  color: #64748b;
}

.stat-badge i {
  font-size: 0.625rem;
}

.stat-badge.featured {
  background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
  border-color: #fcd34d;
  color: #92400e;
}

.stat-badge.going {
  background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
  border-color: #6ee7b7;
  color: #065f46;
}

/* Calendar Legend */
.calendar-legend {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1.5rem;
  padding: 0.75rem 1.5rem;
  background: #fafafa;
  border-bottom: 1px solid #f1f5f9;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.375rem;
}

.legend-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.legend-label {
  font-size: 0.6875rem;
  font-weight: 500;
  color: #64748b;
}

/* Event Indicators in Calendar */
.event-indicators {
  display: flex;
  align-items: center;
  gap: 0.125rem;
  flex-shrink: 0;
}

.featured-indicator {
  color: #f59e0b;
  font-size: 0.5rem;
}

.going-indicator {
  color: #10b981;
  font-size: 0.5rem;
}

.virtual-indicator {
  color: #6366f1;
  font-size: 0.5rem;
}

.event-time-mini {
  font-size: 0.5625rem;
  font-weight: 600;
  color: inherit;
  opacity: 0.8;
  flex-shrink: 0;
}

/* ===============================================
   WEEK VIEW STYLES
   =============================================== */

.calendar-weekdays-premium.week-view {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
}

.week-day-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.25rem;
  padding: 0.75rem 0.5rem;
}

.weekday-name {
  font-size: 0.6875rem;
  font-weight: 600;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.weekday-date {
  font-size: 1.125rem;
  font-weight: 700;
  color: #334155;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0.5rem;
}

.weekday-date.today-date {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  box-shadow: 0 4px 10px rgba(20, 184, 166, 0.35);
}

.calendar-week-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  min-height: 400px;
}

.calendar-week-day {
  border-right: 1px solid #f1f5f9;
  padding: 0.75rem 0.5rem;
  min-height: 350px;
  background: white;
  transition: background 0.2s ease;
}

.calendar-week-day:last-child {
  border-right: none;
}

.calendar-week-day:hover {
  background: linear-gradient(135deg, rgba(20, 184, 166, 0.02) 0%, rgba(20, 184, 166, 0.04) 100%);
}

.calendar-week-day.is-today {
  background: linear-gradient(135deg, rgba(20, 184, 166, 0.05) 0%, rgba(20, 184, 166, 0.02) 100%);
}

.week-day-events {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.week-event-card {
  padding: 0.625rem;
  border-radius: 0.625rem;
  cursor: pointer;
  transition: all 0.2s ease;
  border-inline-start: 3px solid;
}

.week-event-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.week-event-card.meeting {
  background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%);
  border-inline-start-color: #3b82f6;
}

.week-event-card.training {
  background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
  border-inline-start-color: #10b981;
}

.week-event-card.social {
  background: linear-gradient(135deg, #fce7f3 0%, #fbcfe8 100%);
  border-inline-start-color: #ec4899;
}

.week-event-card.review {
  background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
  border-inline-start-color: #f59e0b;
}

.week-event-card.webinar {
  background: linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%);
  border-inline-start-color: #6366f1;
}

.week-event-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.375rem;
}

.week-event-time {
  font-size: 0.625rem;
  font-weight: 600;
  color: #475569;
}

.week-event-badges {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.week-event-title {
  font-size: 0.75rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0 0 0.25rem 0;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.week-event-location {
  font-size: 0.625rem;
  color: #64748b;
  margin: 0 0 0.5rem 0;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.week-event-location i {
  font-size: 0.5rem;
}

.week-event-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.week-event-attendees {
  display: flex;
  align-items: center;
  gap: -0.25rem;
}

.mini-avatar {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.5rem;
  font-weight: 700;
  color: white;
  border: 1.5px solid white;
  margin-inline-end: -6px;
}

.more-attendees {
  font-size: 0.5625rem;
  font-weight: 600;
  color: #64748b;
  margin-inline-start: 0.375rem;
}

.week-rsvp-btn {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  border: none;
  background: white;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.625rem;
}

.week-rsvp-btn:hover {
  background: #14b8a6;
  color: white;
}

.week-rsvp-btn.going {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
}

.week-no-events {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem 1rem;
  color: #cbd5e1;
  text-align: center;
}

.week-no-events i {
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
}

.week-no-events span {
  font-size: 0.6875rem;
  font-weight: 500;
}

/* ===============================================
   DAY DETAIL POPUP
   =============================================== */

.day-popup-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.6);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
}

.day-popup-content {
  background: white;
  border-radius: 1.25rem;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  width: 100%;
  max-width: 540px;
  max-height: 85vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.day-popup-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 1.25rem 1.5rem;
  background: linear-gradient(135deg, #f8fafc 0%, #f0fdfa 100%);
  border-bottom: 1px solid rgba(20, 184, 166, 0.1);
}

.day-popup-date {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.popup-date-badge {
  width: 60px;
  height: 60px;
  background: white;
  border-radius: 0.875rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  border: 1px solid #e2e8f0;
}

.popup-date-badge.is-today {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-color: transparent;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.35);
}

.popup-date-badge.is-today .popup-date-day,
.popup-date-badge.is-today .popup-date-month {
  color: white;
}

.popup-date-day {
  font-size: 1.5rem;
  font-weight: 800;
  color: #0f172a;
  line-height: 1;
}

.popup-date-month {
  font-size: 0.6875rem;
  font-weight: 700;
  color: #14b8a6;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.popup-date-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.popup-title {
  font-size: 1rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0;
}

.popup-subtitle {
  font-size: 0.8125rem;
  color: #64748b;
  margin: 0;
}

.popup-close-btn {
  width: 36px;
  height: 36px;
  border-radius: 0.625rem;
  border: none;
  background: white;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.popup-close-btn:hover {
  background: #fee2e2;
  color: #ef4444;
}

.day-popup-body {
  flex: 1;
  overflow-y: auto;
  padding: 1.25rem 1.5rem;
}

.popup-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 1rem;
  text-align: center;
}

.empty-icon {
  width: 64px;
  height: 64px;
  background: linear-gradient(135deg, #f1f5f9 0%, #e2e8f0 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 1rem;
}

.empty-icon i {
  font-size: 1.5rem;
  color: #94a3b8;
}

.empty-text {
  font-size: 0.9375rem;
  color: #64748b;
  margin: 0 0 1.25rem 0;
}

.popup-add-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border: none;
  border-radius: 0.625rem;
  color: white;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
}

.popup-add-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.35);
}

.popup-events-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.popup-event-card {
  padding: 1rem;
  border-radius: 0.875rem;
  cursor: pointer;
  transition: all 0.2s ease;
  border-left: 4px solid;
}

.popup-event-card:hover {
  transform: translateX(4px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}

.popup-event-card.meeting {
  background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%);
  border-inline-start-color: #3b82f6;
}

.popup-event-card.training {
  background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
  border-inline-start-color: #10b981;
}

.popup-event-card.social {
  background: linear-gradient(135deg, #fce7f3 0%, #fbcfe8 100%);
  border-inline-start-color: #ec4899;
}

.popup-event-card.review {
  background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
  border-inline-start-color: #f59e0b;
}

.popup-event-card.webinar {
  background: linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%);
  border-inline-start-color: #6366f1;
}

.popup-event-time {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  font-weight: 600;
  color: #475569;
  margin-bottom: 0.625rem;
}

.popup-event-time i {
  font-size: 0.625rem;
}

.popup-event-main {
  margin-bottom: 0.875rem;
}

.popup-event-badges {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
  flex-wrap: wrap;
}

.popup-type-badge {
  padding: 0.25rem 0.625rem;
  border-radius: 999px;
  font-size: 0.625rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.03em;
}

.popup-type-badge.meeting {
  background: #3b82f6;
  color: white;
}

.popup-type-badge.training {
  background: #10b981;
  color: white;
}

.popup-type-badge.social {
  background: #ec4899;
  color: white;
}

.popup-type-badge.review {
  background: #f59e0b;
  color: white;
}

.popup-type-badge.webinar {
  background: #6366f1;
  color: white;
}

.popup-featured-badge {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.625rem;
  background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
  border-radius: 999px;
  font-size: 0.625rem;
  font-weight: 700;
  color: #92400e;
}

.popup-virtual-badge {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.625rem;
  background: linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%);
  border-radius: 999px;
  font-size: 0.625rem;
  font-weight: 700;
  color: #3730a3;
}

.popup-event-title {
  font-size: 1rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0 0 0.375rem 0;
  line-height: 1.3;
}

.popup-event-location {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.8125rem;
  color: #475569;
  margin: 0 0 0.5rem 0;
}

.popup-event-location i {
  font-size: 0.6875rem;
  color: #64748b;
}

.popup-event-desc {
  font-size: 0.8125rem;
  color: #64748b;
  margin: 0;
  line-height: 1.5;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.popup-event-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 0.75rem;
  border-top: 1px solid rgba(0, 0, 0, 0.06);
}

.popup-attendees {
  display: flex;
  align-items: center;
}

.popup-attendee {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.625rem;
  font-weight: 700;
  color: white;
  border: 2px solid white;
  margin-inline-end: -8px;
}

.popup-more-attendees {
  font-size: 0.6875rem;
  font-weight: 600;
  color: #64748b;
  margin-inline-start: 0.5rem;
  margin-inline-end: 0.5rem;
}

.popup-attendee-count {
  font-size: 0.75rem;
  color: #64748b;
}

.popup-actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.popup-action-btn {
  width: 36px;
  height: 36px;
  border-radius: 0.625rem;
  border: 1px solid #e2e8f0;
  background: white;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.popup-action-btn:hover {
  border-color: #fecaca;
  color: #ef4444;
  background: #fef2f2;
}

.popup-action-btn.interested {
  background: #fef2f2;
  border-color: #fecaca;
  color: #ef4444;
}

.popup-rsvp-btn {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 1rem;
  border-radius: 0.625rem;
  border: none;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.75rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
}

.popup-rsvp-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.35);
}

.popup-rsvp-btn.going {
  background: linear-gradient(135deg, #22c55e 0%, #16a34a 100%);
}

/* Popup Transition */
.popup-fade-enter-active,
.popup-fade-leave-active {
  transition: all 0.3s ease;
}

.popup-fade-enter-from,
.popup-fade-leave-to {
  opacity: 0;
}

.popup-fade-enter-from .day-popup-content,
.popup-fade-leave-to .day-popup-content {
  transform: scale(0.95) translateY(20px);
}

/* ===============================================
   FEATURED EVENT COUNTDOWN
   =============================================== */
.featured-countdown {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: rgba(239, 68, 68, 0.15);
  border: 1px solid rgba(239, 68, 68, 0.3);
  border-radius: 12px;
  color: #dc2626;
}

.countdown-icon {
  font-size: 0.875rem;
  animation: pulse-countdown 2s ease-in-out infinite;
}

@keyframes pulse-countdown {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.7; transform: scale(1.1); }
}

.countdown-label {
  font-size: 0.875rem;
  font-weight: 600;
}

/* ===============================================
   FEATURED EVENT ACTIONS
   =============================================== */
.featured-actions {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.featured-action-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 12px;
  color: white;
  cursor: pointer;
  transition: all 0.2s ease;
}

.featured-action-btn:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: translateY(-2px);
}

.featured-action-btn.secondary {
  background: rgba(255, 255, 255, 0.1);
}

/* ===============================================
   MY UPCOMING EVENTS SIDEBAR
   =============================================== */
.my-events-section {
  background: linear-gradient(135deg, rgba(139, 92, 246, 0.05) 0%, rgba(168, 139, 250, 0.05) 100%);
  border: 1px solid rgba(139, 92, 246, 0.15);
}

.my-events-section .sidebar-title {
  color: #7c3aed;
}

.my-events-section .sidebar-title i {
  color: #8b5cf6;
}

.my-events-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.my-event-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  background: white;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s ease;
  border: 1px solid rgba(139, 92, 246, 0.1);
}

.my-event-item:hover {
  transform: translateX(4px);
  box-shadow: 0 4px 12px rgba(139, 92, 246, 0.15);
  border-color: rgba(139, 92, 246, 0.3);
}

.my-event-date {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-width: 48px;
  padding: 0.5rem;
  background: linear-gradient(135deg, #8b5cf6 0%, #a78bfa 100%);
  border-radius: 10px;
  color: white;
}

.my-event-day {
  font-size: 1.125rem;
  font-weight: 700;
  line-height: 1;
}

.my-event-month {
  font-size: 0.625rem;
  font-weight: 600;
  text-transform: uppercase;
  opacity: 0.9;
}

.my-event-info {
  flex: 1;
  min-width: 0;
}

.my-event-title {
  font-size: 0.875rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 0.25rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.my-event-meta {
  font-size: 0.75rem;
  color: #6b7280;
}

.my-event-meta i {
  margin-inline-end: 0.25rem;
  color: #8b5cf6;
}

.my-event-actions {
  display: flex;
  align-items: center;
}

.my-event-action-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  background: rgba(139, 92, 246, 0.1);
  border: none;
  border-radius: 8px;
  color: #8b5cf6;
  cursor: pointer;
  transition: all 0.2s ease;
}

.my-event-action-btn:hover {
  background: #8b5cf6;
  color: white;
}

.view-all-my-events {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  width: 100%;
  padding: 0.75rem;
  margin-top: 0.75rem;
  background: transparent;
  border: 1px dashed rgba(139, 92, 246, 0.3);
  border-radius: 10px;
  color: #8b5cf6;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.view-all-my-events:hover {
  background: rgba(139, 92, 246, 0.1);
  border-style: solid;
}

/* ===============================================
   CARD ACTION BUTTONS (Grid View)
   =============================================== */
.card-action-buttons {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.card-action-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  background: #f3f4f6;
  border: none;
  border-radius: 10px;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s ease;
}

.card-action-btn:hover {
  background: #0d9488;
  color: white;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(13, 148, 136, 0.25);
}

.card-action-btn.interested-active {
  background: #fef2f2;
  color: #ef4444;
  border: 1px solid #fecaca;
}

.card-action-btn.interested-active:hover {
  background: #ef4444;
  color: white;
  box-shadow: 0 4px 8px rgba(239, 68, 68, 0.25);
}

/* ===============================================
   LIST ACTION BUTTONS
   =============================================== */
.list-action-buttons {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.list-action-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  background: #f3f4f6;
  border: none;
  border-radius: 8px;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s ease;
}

.list-action-btn:hover {
  background: #0d9488;
  color: white;
}

.list-action-btn.interested-active {
  background: #fef2f2;
  color: #ef4444;
  border: 1px solid #fecaca;
}

.list-action-btn.interested-active:hover {
  background: #ef4444;
  color: white;
}

@media (max-width: 768px) {
  .featured-actions {
    flex-wrap: wrap;
    gap: 0.5rem;
  }

  .featured-action-btn {
    width: 36px;
    height: 36px;
  }

  .card-action-buttons {
    flex-wrap: wrap;
  }

  .list-action-buttons {
    flex-wrap: wrap;
  }
}

/* ===============================================
   AI FEATURE STYLES
   =============================================== */

.ai-action-btn {
  transition: all 0.2s ease;
}

.ai-action-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.2);
}

.ai-modal-overlay {
  animation: modalFadeIn 0.3s ease-out;
}

@keyframes modalFadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.ai-modal-content {
  animation: modalSlideIn 0.3s ease-out;
}

@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: scale(0.95) translateY(10px);
  }
  to {
    opacity: 1;
    transform: scale(1) translateY(0);
  }
}

.ai-suggestion-card {
  transition: all 0.2s ease;
}

.ai-suggestion-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}

.ai-pulse {
  animation: aiPulse 2s ease-in-out infinite;
}

@keyframes aiPulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}

.ai-gradient-text {
  background: linear-gradient(135deg, #14b8a6 0%, #10b981 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.ai-score-bar {
  transition: width 0.5s ease-out;
}

.recommendation-optimal {
  background: linear-gradient(135deg, #dcfce7 0%, #bbf7d0 100%);
  border-color: #86efac;
}

.recommendation-good {
  background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%);
  border-color: #93c5fd;
}

.recommendation-available {
  background: linear-gradient(135deg, #f3f4f6 0%, #e5e7eb 100%);
  border-color: #d1d5db;
}
</style>
