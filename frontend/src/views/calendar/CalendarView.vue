<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import ErrorState from '@/components/base/ErrorState.vue'
import { PageHeader, StatsBar, Avatar } from '@/components/base'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import type { StatItem } from '@/components/base/StatsBar.vue'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Calendar from 'primevue/calendar'
import Dialog from 'primevue/dialog'
import Dropdown from 'primevue/dropdown'
import Checkbox from 'primevue/checkbox'
import Skeleton from 'primevue/skeleton'

const { locale } = useI18n()
const prefersReducedMotion = useReducedMotion()

// Animation control
const isContentVisible = ref(false)
const isViewTransitioning = ref(false)
const shouldAnimate = computed(() => !prefersReducedMotion.value)
const LOADING_DELAY = 600

// Loading and error state
const loading = ref(true)
const error = ref<Error | null>(null)
const selectedDate = ref(new Date())
const currentView = ref<'month' | 'week' | 'day' | 'agenda'>('month')
const showCreateEventDialog = ref(false)
const showEventDetailDialog = ref(false)
const selectedEvent = ref<CalendarEvent | null>(null)

// RTL support
const isRTL = computed(() => locale.value === 'ar')

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: isRTL.value ? 'التقويم' : 'Calendar' }
])

// Type configurations
const CALENDAR_COLORS: Record<string, { color: string; bgColor: string }> = {
  blue: { color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.15)' },
  green: { color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.15)' },
  purple: { color: '#8b5cf6', bgColor: 'rgba(139, 92, 246, 0.15)' },
  orange: { color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.15)' },
  red: { color: '#ef4444', bgColor: 'rgba(239, 68, 68, 0.15)' },
  gold: { color: '#d4af37', bgColor: 'rgba(212, 175, 55, 0.15)' }
}

const RSVP_CONFIG: Record<string, { color: string; bgColor: string; label: string; labelAr: string }> = {
  Accepted: { color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', label: 'Accepted', labelAr: 'مقبول' },
  Declined: { color: '#ef4444', bgColor: 'rgba(239, 68, 68, 0.1)', label: 'Declined', labelAr: 'مرفوض' },
  Tentative: { color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'Maybe', labelAr: 'ربما' },
  Pending: { color: '#6b7280', bgColor: 'rgba(107, 114, 128, 0.1)', label: 'Pending', labelAr: 'قيد الانتظار' }
}

interface CalendarItem {
  id: string
  name: string
  nameAr?: string
  color: string
  isDefault: boolean
}

interface CalendarEvent {
  id: string
  title: string
  titleAr?: string
  startDate: Date
  endDate: Date
  isAllDay: boolean
  location?: string
  isOnline: boolean
  onlineMeetingUrl?: string
  description?: string
  color: string
  calendarId: string
  attendeeCount: number
  myRsvpStatus?: string
  canEdit: boolean
  canDelete: boolean
  attendees?: { id: string; name: string; rsvpStatus: string }[]
}

interface DayCell {
  date: Date
  dayNumber: number
  isCurrentMonth: boolean
  isToday: boolean
  events: CalendarEvent[]
}

const calendars = ref<CalendarItem[]>([
  { id: '1', name: 'My Calendar', nameAr: 'تقويمي', color: 'blue', isDefault: true },
  { id: '2', name: 'Team Events', nameAr: 'فعاليات الفريق', color: 'green', isDefault: false },
  { id: '3', name: 'AFC Cup 2027', nameAr: 'كأس آسيا 2027', color: 'gold', isDefault: false }
])

const selectedCalendarIds = ref<string[]>(['1', '2', '3'])
const events = ref<CalendarEvent[]>([])

const newEvent = ref({
  title: '',
  startDate: new Date(),
  endDate: new Date(),
  isAllDay: false,
  location: '',
  isOnline: false,
  description: '',
  calendarId: '1'
})

// View options
const viewOptions = computed(() => [
  { label: isRTL.value ? 'شهر' : 'Month', value: 'month', icon: 'pi pi-calendar' },
  { label: isRTL.value ? 'أسبوع' : 'Week', value: 'week', icon: 'pi pi-th-large' },
  { label: isRTL.value ? 'يوم' : 'Day', value: 'day', icon: 'pi pi-stop' },
  { label: isRTL.value ? 'جدول' : 'Agenda', value: 'agenda', icon: 'pi pi-list' }
])

const hours = Array.from({ length: 24 }, (_, i) => i)

// Week days
const weekDays = computed(() => {
  return isRTL.value
    ? ['أحد', 'إثنين', 'ثلاثاء', 'أربعاء', 'خميس', 'جمعة', 'سبت']
    : ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']
})

// Current period title
const currentPeriodTitle = computed(() => {
  const date = selectedDate.value
  const loc = isRTL.value ? 'ar-SA' : 'en-US'

  if (currentView.value === 'week') {
    const weekStart = getWeekStart(date)
    const weekEnd = new Date(weekStart)
    weekEnd.setDate(weekEnd.getDate() + 6)
    return `${weekStart.toLocaleDateString(loc, { month: 'short', day: 'numeric' })} - ${weekEnd.toLocaleDateString(loc, { month: 'short', day: 'numeric', year: 'numeric' })}`
  }

  return date.toLocaleDateString(loc, { month: 'long', year: 'numeric' })
})

// Calendar weeks for month view
const calendarWeeks = computed((): DayCell[][] => {
  const weeks: DayCell[][] = []
  const date = new Date(selectedDate.value)
  date.setDate(1)

  const firstDay = date.getDay()
  date.setDate(date.getDate() - firstDay)

  for (let w = 0; w < 6; w++) {
    const week: DayCell[] = []
    for (let d = 0; d < 7; d++) {
      const currentDate = new Date(date)
      week.push({
        date: currentDate,
        dayNumber: currentDate.getDate(),
        isCurrentMonth: currentDate.getMonth() === selectedDate.value.getMonth(),
        isToday: isToday(currentDate),
        events: getEventsForDay(currentDate)
      })
      date.setDate(date.getDate() + 1)
    }
    weeks.push(week)
  }

  return weeks
})

// Current week days for week view
const currentWeekDays = computed(() => {
  const days = []
  const weekStart = getWeekStart(selectedDate.value)
  const loc = isRTL.value ? 'ar-SA' : 'en-US'

  for (let i = 0; i < 7; i++) {
    const date = new Date(weekStart)
    date.setDate(date.getDate() + i)
    days.push({
      date,
      dayNumber: date.getDate(),
      dayName: date.toLocaleDateString(loc, { weekday: 'short' }),
      isToday: isToday(date),
      isWeekend: date.getDay() === 5 || date.getDay() === 6
    })
  }

  return days
})

// Agenda items
const agendaItems = computed(() => {
  const items = []
  const startDate = new Date(selectedDate.value)

  for (let i = 0; i < 14; i++) {
    const date = new Date(startDate)
    date.setDate(date.getDate() + i)

    const dayEvents = getEventsForDay(date)
    if (dayEvents.length > 0 || isToday(date)) {
      items.push({
        date,
        isToday: isToday(date),
        events: dayEvents
      })
    }
  }

  return items
})

// Upcoming events
const upcomingEvents = computed(() => {
  const now = new Date()
  return events.value
    .filter(e => new Date(e.startDate) >= now)
    .sort((a, b) => new Date(a.startDate).getTime() - new Date(b.startDate).getTime())
    .slice(0, 5)
})

// Stats
const stats = computed(() => ({
  todayEvents: events.value.filter(e => isToday(new Date(e.startDate))).length,
  weekEvents: events.value.filter(e => {
    const eventDate = new Date(e.startDate)
    const weekStart = getWeekStart(new Date())
    const weekEnd = new Date(weekStart)
    weekEnd.setDate(weekEnd.getDate() + 7)
    return eventDate >= weekStart && eventDate < weekEnd
  }).length,
  pendingRsvp: events.value.filter(e => e.myRsvpStatus === 'Pending').length
}))

// StatsBar items
const statsBarItems = computed<StatItem[]>(() => [
  {
    icon: 'pi pi-sun',
    value: stats.value.todayEvents,
    label: "Today's Events",
    labelArabic: 'فعاليات اليوم',
    colorClass: 'primary'
  },
  {
    icon: 'pi pi-calendar',
    value: stats.value.weekEvents,
    label: 'This Week',
    labelArabic: 'هذا الأسبوع',
    colorClass: 'info'
  },
  {
    icon: 'pi pi-clock',
    value: stats.value.pendingRsvp,
    label: 'Pending RSVP',
    labelArabic: 'في انتظار الرد',
    colorClass: 'warning'
  }
])

// Helpers
function isToday(date: Date): boolean {
  const today = new Date()
  return date.toDateString() === today.toDateString()
}

function getWeekStart(date: Date): Date {
  const d = new Date(date)
  d.setDate(d.getDate() - d.getDay())
  return d
}

function getEventsForDay(date: Date): CalendarEvent[] {
  return events.value.filter(e => {
    const eventDate = new Date(e.startDate)
    return eventDate.toDateString() === date.toDateString()
  })
}

function getEventsForHour(date: Date, hour: number): CalendarEvent[] {
  return events.value.filter(e => {
    const eventDate = new Date(e.startDate)
    return eventDate.toDateString() === date.toDateString() && eventDate.getHours() === hour
  })
}

function formatTime(date: Date | string): string {
  const d = new Date(date)
  return d.toLocaleTimeString(isRTL.value ? 'ar-SA' : 'en-US', {
    hour: '2-digit',
    minute: '2-digit'
  })
}

function formatHour(hour: number): string {
  const date = new Date()
  date.setHours(hour, 0, 0)
  return date.toLocaleTimeString(isRTL.value ? 'ar-SA' : 'en-US', {
    hour: '2-digit',
    hour12: true
  })
}

function formatEventTime(event: CalendarEvent): string {
  if (event.isAllDay) return isRTL.value ? 'طوال اليوم' : 'All day'
  return `${formatTime(event.startDate)} - ${formatTime(event.endDate)}`
}

function formatDate(date: Date, format: string): string {
  const loc = isRTL.value ? 'ar-SA' : 'en-US'

  switch (format) {
    case 'full':
      return date.toLocaleDateString(loc, { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })
    case 'short':
      return date.toLocaleDateString(loc, { month: 'short', day: 'numeric' })
    case 'weekday':
      return date.toLocaleDateString(loc, { weekday: 'long' })
    default:
      return date.toLocaleDateString(loc)
  }
}

function getEventTitle(event: CalendarEvent): string {
  return isRTL.value && event.titleAr ? event.titleAr : event.title
}

function getCalendarName(cal: CalendarItem): string {
  return isRTL.value && cal.nameAr ? cal.nameAr : cal.name
}

function getCalendarColor(colorKey: string) {
  return CALENDAR_COLORS[colorKey] || CALENDAR_COLORS.blue
}

function getRsvpConfig(status: string) {
  return RSVP_CONFIG[status] || RSVP_CONFIG.Pending
}

// Navigation
function goToToday() {
  selectedDate.value = new Date()
}

function navigatePrev() {
  const date = new Date(selectedDate.value)
  if (currentView.value === 'month') {
    date.setMonth(date.getMonth() - 1)
  } else if (currentView.value === 'week') {
    date.setDate(date.getDate() - 7)
  } else {
    date.setDate(date.getDate() - 1)
  }
  selectedDate.value = date
}

function navigateNext() {
  const date = new Date(selectedDate.value)
  if (currentView.value === 'month') {
    date.setMonth(date.getMonth() + 1)
  } else if (currentView.value === 'week') {
    date.setDate(date.getDate() + 7)
  } else {
    date.setDate(date.getDate() + 1)
  }
  selectedDate.value = date
}

function onDayClick(day: DayCell) {
  selectedDate.value = day.date
  if (currentView.value === 'month') {
    currentView.value = 'day'
  }
}

function createEventAt(date: Date, hour: number) {
  const start = new Date(date)
  start.setHours(hour, 0, 0)
  const end = new Date(start)
  end.setHours(hour + 1)

  newEvent.value.startDate = start
  newEvent.value.endDate = end
  showCreateEventDialog.value = true
}

function viewEvent(event: CalendarEvent) {
  selectedEvent.value = event
  showEventDetailDialog.value = true
}

function createEvent() {
  const calendar = calendars.value.find(c => c.id === newEvent.value.calendarId)
  const event: CalendarEvent = {
    id: Date.now().toString(),
    title: newEvent.value.title,
    titleAr: newEvent.value.title,
    startDate: newEvent.value.startDate,
    endDate: newEvent.value.endDate,
    isAllDay: newEvent.value.isAllDay,
    location: newEvent.value.location,
    isOnline: newEvent.value.isOnline,
    description: newEvent.value.description,
    calendarId: newEvent.value.calendarId,
    color: calendar?.color || 'blue',
    attendeeCount: 0,
    canEdit: true,
    canDelete: true
  }
  events.value.push(event)
  showCreateEventDialog.value = false

  newEvent.value = {
    title: '',
    startDate: new Date(),
    endDate: new Date(),
    isAllDay: false,
    location: '',
    isOnline: false,
    description: '',
    calendarId: '1'
  }
}

function deleteEvent() {
  if (selectedEvent.value) {
    events.value = events.value.filter(e => e.id !== selectedEvent.value?.id)
    showEventDetailDialog.value = false
  }
}

function rsvp(status: string) {
  if (selectedEvent.value) {
    selectedEvent.value.myRsvpStatus = status
  }
}

// Watch for view changes to trigger transition animation
watch(currentView, () => {
  if (shouldAnimate.value) {
    isViewTransitioning.value = true
    requestAnimationFrame(() => {
      setTimeout(() => {
        isViewTransitioning.value = false
      }, 50)
    })
  }
})

async function loadCalendar() {
  try {
    error.value = null
    loading.value = true

    await new Promise(resolve => setTimeout(resolve, LOADING_DELAY))

    const today = new Date()

    events.value = [
      {
        id: '1',
        title: 'Team Standup',
        titleAr: 'اجتماع الفريق اليومي',
        startDate: new Date(today.getFullYear(), today.getMonth(), today.getDate(), 9, 0),
        endDate: new Date(today.getFullYear(), today.getMonth(), today.getDate(), 9, 30),
        isAllDay: false,
        location: 'Meeting Room A',
        isOnline: true,
        onlineMeetingUrl: 'https://meet.example.com/standup',
        color: 'blue',
        calendarId: '1',
        attendeeCount: 5,
        myRsvpStatus: 'Accepted',
        canEdit: true,
        canDelete: true,
        attendees: [
          { id: '1', name: 'Mohammed Al-Rashid', rsvpStatus: 'Accepted' },
          { id: '2', name: 'Sara Ali', rsvpStatus: 'Accepted' }
        ]
      },
      {
        id: '2',
        title: 'Project Review',
        titleAr: 'مراجعة المشروع',
        startDate: new Date(today.getFullYear(), today.getMonth(), today.getDate(), 14, 0),
        endDate: new Date(today.getFullYear(), today.getMonth(), today.getDate(), 15, 0),
        isAllDay: false,
        isOnline: true,
        color: 'green',
        calendarId: '2',
        attendeeCount: 8,
        myRsvpStatus: 'Pending',
        canEdit: false,
        canDelete: false
      },
      {
        id: '3',
        title: 'AFC Cup Planning Session',
        titleAr: 'جلسة تخطيط كأس آسيا',
        startDate: new Date(today.getFullYear(), today.getMonth(), today.getDate() + 1, 10, 0),
        endDate: new Date(today.getFullYear(), today.getMonth(), today.getDate() + 1, 12, 0),
        isAllDay: false,
        location: 'Conference Hall B',
        isOnline: false,
        color: 'gold',
        calendarId: '3',
        attendeeCount: 15,
        myRsvpStatus: 'Accepted',
        canEdit: true,
        canDelete: true
      },
      {
        id: '4',
        title: 'Stadium Inspection',
        titleAr: 'تفتيش الملعب',
        startDate: new Date(today.getFullYear(), today.getMonth(), today.getDate() + 2, 0, 0),
        endDate: new Date(today.getFullYear(), today.getMonth(), today.getDate() + 2, 23, 59),
        isAllDay: true,
        location: 'King Abdullah Sports City',
        isOnline: false,
        color: 'purple',
        calendarId: '2',
        attendeeCount: 20,
        myRsvpStatus: 'Accepted',
        canEdit: false,
        canDelete: false
      }
    ]

    loading.value = false

    // Trigger entrance animations
    if (shouldAnimate.value) {
      requestAnimationFrame(() => {
        isContentVisible.value = true
      })
    } else {
      isContentVisible.value = true
    }
  } catch (e) {
    error.value = e as Error
    loading.value = false
  }
}

async function handleRetry() {
  await loadCalendar()
}

onMounted(() => {
  loadCalendar()
})
</script>

<template>
  <div class="calendar-view" :class="{ 'rtl': isRTL }">
    <!-- Page Header -->
    <PageHeader
      :title="isRTL ? 'التقويم' : 'Calendar'"
      :description="isRTL ? 'إدارة الفعاليات والمواعيد والاجتماعات' : 'Manage events, appointments, and meetings'"
      :breadcrumbs="breadcrumbs"
      padding-bottom="lg"
      background-variant="gradient"
      :animated="shouldAnimate"
    >
      <template #actions>
        <Button
          :label="isRTL ? 'اليوم' : 'Today'"
          severity="secondary"
          @click="goToToday"
        />
        <Button
          :label="isRTL ? 'إنشاء فعالية' : 'Create Event'"
          icon="pi pi-plus"
          @click="showCreateEventDialog = true"
        />
      </template>
    </PageHeader>

    <!-- Stats Bar -->
    <StatsBar
      :stats="statsBarItems"
      :loading="loading"
      overlap="lg"
      :animated="shouldAnimate"
      :animate-numbers="shouldAnimate"
      :counter-duration="1200"
    />

    <!-- Loading State -->
    <div v-if="loading" class="loading-grid">
      <div class="loading-sidebar">
        <Skeleton height="300px" class="mb-4" borderRadius="16px" />
        <Skeleton height="200px" class="mb-4" borderRadius="16px" />
        <Skeleton height="250px" borderRadius="16px" />
      </div>
      <div class="loading-main">
        <Skeleton height="600px" borderRadius="16px" />
      </div>
    </div>

    <!-- Error State -->
    <ErrorState
      v-else-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل التقويم' : 'Failed to load calendar'"
      show-retry
      @retry="handleRetry"
    />

    <!-- Main Content -->
    <div v-else class="calendar-content">
      <!-- Sidebar -->
      <aside class="calendar-sidebar">
        <!-- Mini Calendar -->
        <div class="sidebar-card">
          <Calendar
            v-model="selectedDate"
            inline
            class="mini-calendar"
            @date-select="(date: Date) => selectedDate = date"
          />
        </div>

        <!-- My Calendars -->
        <div class="sidebar-card">
          <div class="card-header">
            <h4>{{ isRTL ? 'تقويماتي' : 'My Calendars' }}</h4>
            <button class="add-btn">
              <i class="pi pi-plus"></i>
            </button>
          </div>
          <div class="calendars-list">
            <label
              v-for="cal in calendars"
              :key="cal.id"
              class="calendar-item"
            >
              <Checkbox
                v-model="selectedCalendarIds"
                :value="cal.id"
                :inputId="`cal-${cal.id}`"
              />
              <span
                class="calendar-dot"
                :style="{ backgroundColor: getCalendarColor(cal.color).color }"
              ></span>
              <span class="calendar-name">{{ getCalendarName(cal) }}</span>
              <span v-if="cal.isDefault" class="default-badge">
                {{ isRTL ? 'افتراضي' : 'Default' }}
              </span>
            </label>
          </div>
        </div>

        <!-- Upcoming Events -->
        <div class="sidebar-card">
          <div class="card-header">
            <h4>{{ isRTL ? 'الفعاليات القادمة' : 'Upcoming Events' }}</h4>
          </div>
          <div v-if="upcomingEvents.length === 0" class="empty-state-mini">
            <i class="pi pi-calendar"></i>
            <p>{{ isRTL ? 'لا توجد فعاليات قادمة' : 'No upcoming events' }}</p>
          </div>
          <div v-else class="upcoming-list">
            <div
              v-for="event in upcomingEvents"
              :key="event.id"
              class="upcoming-item"
              @click="viewEvent(event)"
            >
              <div
                class="event-indicator"
                :style="{ backgroundColor: getCalendarColor(event.color).color }"
              ></div>
              <div class="event-info">
                <span class="event-title">{{ getEventTitle(event) }}</span>
                <span class="event-time">{{ formatEventTime(event) }}</span>
              </div>
            </div>
          </div>
        </div>
      </aside>

      <!-- Main Calendar -->
      <main class="calendar-main">
        <div class="main-card">
          <!-- Calendar Toolbar -->
          <div class="calendar-toolbar">
            <div class="toolbar-left">
              <button class="nav-btn" @click="navigatePrev">
                <i :class="isRTL ? 'pi pi-chevron-right' : 'pi pi-chevron-left'"></i>
              </button>
              <h2 class="period-title">{{ currentPeriodTitle }}</h2>
              <button class="nav-btn" @click="navigateNext">
                <i :class="isRTL ? 'pi pi-chevron-left' : 'pi pi-chevron-right'"></i>
              </button>
            </div>
            <div class="view-toggle">
              <button
                v-for="view in viewOptions"
                :key="view.value"
                :class="['view-btn', { active: currentView === view.value }]"
                @click="currentView = view.value as 'month' | 'week' | 'day' | 'agenda'"
              >
                <i :class="view.icon"></i>
                <span>{{ view.label }}</span>
              </button>
            </div>
          </div>

          <!-- Month View -->
          <div
            v-if="currentView === 'month'"
            class="month-view"
            :class="{
              'view--animated': shouldAnimate,
              'view--visible': isContentVisible && !isViewTransitioning
            }"
          >
            <div class="week-header">
              <div v-for="day in weekDays" :key="day" class="week-day-name">
                {{ day }}
              </div>
            </div>
            <div class="month-grid">
              <div v-for="(week, weekIndex) in calendarWeeks" :key="weekIndex" class="week-row">
                <div
                  v-for="day in week"
                  :key="day.date.toISOString()"
                  :class="['day-cell', {
                    'other-month': !day.isCurrentMonth,
                    'today': day.isToday
                  }]"
                  @click="onDayClick(day)"
                >
                  <div class="day-header">
                    <span class="day-number">{{ day.dayNumber }}</span>
                    <span v-if="day.events.length > 3" class="more-events">
                      +{{ day.events.length - 3 }}
                    </span>
                  </div>
                  <div class="day-events">
                    <div
                      v-for="event in day.events.slice(0, 3)"
                      :key="event.id"
                      class="event-chip"
                      :style="{
                        backgroundColor: getCalendarColor(event.color).bgColor,
                        borderColor: getCalendarColor(event.color).color,
                        color: getCalendarColor(event.color).color
                      }"
                      @click.stop="viewEvent(event)"
                    >
                      <span v-if="!event.isAllDay" class="event-time-mini">{{ formatTime(event.startDate) }}</span>
                      {{ getEventTitle(event) }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Week View -->
          <div
            v-else-if="currentView === 'week'"
            class="week-view"
            :class="{
              'view--animated': shouldAnimate,
              'view--visible': isContentVisible && !isViewTransitioning
            }"
          >
            <div class="week-header-row">
              <div class="time-column-header"></div>
              <div
                v-for="day in currentWeekDays"
                :key="day.date.toISOString()"
                :class="['week-day-header', { today: day.isToday }]"
              >
                <span class="day-name">{{ day.dayName }}</span>
                <span class="day-num">{{ day.dayNumber }}</span>
              </div>
            </div>
            <div class="week-body">
              <div v-for="hour in hours" :key="hour" class="time-row">
                <div class="time-label">{{ formatHour(hour) }}</div>
                <div
                  v-for="day in currentWeekDays"
                  :key="`${hour}-${day.date.toISOString()}`"
                  :class="['time-cell', { weekend: day.isWeekend }]"
                  @click="createEventAt(day.date, hour)"
                >
                  <div
                    v-for="event in getEventsForHour(day.date, hour)"
                    :key="event.id"
                    class="week-event"
                    :style="{ backgroundColor: getCalendarColor(event.color).color }"
                    @click.stop="viewEvent(event)"
                  >
                    {{ getEventTitle(event) }}
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Day View -->
          <div
            v-else-if="currentView === 'day'"
            class="day-view"
            :class="{
              'view--animated': shouldAnimate,
              'view--visible': isContentVisible && !isViewTransitioning
            }"
          >
            <div class="day-header-info">
              <h3>{{ formatDate(selectedDate, 'full') }}</h3>
            </div>
            <div class="day-body">
              <div v-for="hour in hours" :key="hour" class="time-row">
                <div class="time-label">{{ formatHour(hour) }}</div>
                <div
                  class="time-cell full-width"
                  @click="createEventAt(selectedDate, hour)"
                >
                  <div
                    v-for="event in getEventsForHour(selectedDate, hour)"
                    :key="event.id"
                    class="day-event"
                    :style="{ backgroundColor: getCalendarColor(event.color).color }"
                    @click.stop="viewEvent(event)"
                  >
                    <div class="event-title">{{ getEventTitle(event) }}</div>
                    <div class="event-time-detail">{{ formatEventTime(event) }}</div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Agenda View -->
          <div
            v-else-if="currentView === 'agenda'"
            class="agenda-view"
            :class="{
              'view--animated': shouldAnimate,
              'view--visible': isContentVisible && !isViewTransitioning
            }"
          >
            <div v-if="agendaItems.length === 0" class="empty-state">
              <div class="empty-icon">
                <i class="pi pi-calendar"></i>
              </div>
              <h4>{{ isRTL ? 'لا توجد فعاليات' : 'No events in this period' }}</h4>
              <p>{{ isRTL ? 'قم بإنشاء فعالية جديدة للبدء' : 'Create a new event to get started' }}</p>
            </div>
            <div v-else class="agenda-list">
              <div v-for="item in agendaItems" :key="item.date.toISOString()" class="agenda-day">
                <div :class="['agenda-day-header', { today: item.isToday }]">
                  <span class="day-name">{{ formatDate(item.date, 'weekday') }}</span>
                  <span class="day-date">{{ formatDate(item.date, 'short') }}</span>
                  <span v-if="item.isToday" class="today-badge">{{ isRTL ? 'اليوم' : 'Today' }}</span>
                </div>
                <div class="agenda-events">
                  <div
                    v-for="event in item.events"
                    :key="event.id"
                    class="agenda-event"
                    @click="viewEvent(event)"
                  >
                    <div
                      class="event-bar"
                      :style="{ backgroundColor: getCalendarColor(event.color).color }"
                    ></div>
                    <div class="event-content">
                      <div class="event-main">
                        <h5 class="event-title">{{ getEventTitle(event) }}</h5>
                        <div class="event-meta">
                          <span class="event-time-info">
                            <i :class="event.isAllDay ? 'pi pi-calendar' : 'pi pi-clock'"></i>
                            {{ formatEventTime(event) }}
                          </span>
                          <span v-if="event.location" class="event-location">
                            <i class="pi pi-map-marker"></i>
                            {{ event.location }}
                          </span>
                          <span v-if="event.isOnline" class="event-online">
                            <i class="pi pi-video"></i>
                            {{ isRTL ? 'عبر الإنترنت' : 'Online' }}
                          </span>
                        </div>
                      </div>
                      <div class="event-actions">
                        <div v-if="event.attendeeCount > 0" class="attendees-info">
                          <i class="pi pi-users"></i>
                          <span>{{ event.attendeeCount }}</span>
                        </div>
                        <span
                          v-if="event.myRsvpStatus"
                          class="rsvp-badge"
                          :style="{
                            color: getRsvpConfig(event.myRsvpStatus).color,
                            backgroundColor: getRsvpConfig(event.myRsvpStatus).bgColor
                          }"
                        >
                          {{ isRTL ? getRsvpConfig(event.myRsvpStatus).labelAr : getRsvpConfig(event.myRsvpStatus).label }}
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>

    <!-- Create Event Dialog -->
    <Dialog
      v-model:visible="showCreateEventDialog"
      :header="isRTL ? 'إنشاء فعالية جديدة' : 'Create New Event'"
      :style="{ width: '600px' }"
      modal
      class="premium-dialog"
    >
      <div class="dialog-form">
        <div class="form-group">
          <label class="form-label">{{ isRTL ? 'عنوان الفعالية' : 'Event Title' }} *</label>
          <InputText
            v-model="newEvent.title"
            :placeholder="isRTL ? 'أدخل عنوان الفعالية' : 'Enter event title'"
            class="form-input"
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">{{ isRTL ? 'تاريخ البداية' : 'Start Date' }} *</label>
            <Calendar
              v-model="newEvent.startDate"
              :showTime="!newEvent.isAllDay"
              showIcon
              class="form-input"
            />
          </div>
          <div class="form-group">
            <label class="form-label">{{ isRTL ? 'تاريخ النهاية' : 'End Date' }} *</label>
            <Calendar
              v-model="newEvent.endDate"
              :showTime="!newEvent.isAllDay"
              showIcon
              class="form-input"
            />
          </div>
        </div>

        <div class="form-group checkbox-group">
          <Checkbox v-model="newEvent.isAllDay" inputId="isAllDay" binary />
          <label for="isAllDay">{{ isRTL ? 'طوال اليوم' : 'All day event' }}</label>
        </div>

        <div class="form-group">
          <label class="form-label">{{ isRTL ? 'الموقع' : 'Location' }}</label>
          <InputText
            v-model="newEvent.location"
            :placeholder="isRTL ? 'أضف موقعًا' : 'Add location'"
            class="form-input"
          />
        </div>

        <div class="form-group checkbox-group">
          <Checkbox v-model="newEvent.isOnline" inputId="isOnline" binary />
          <label for="isOnline">{{ isRTL ? 'اجتماع عبر الإنترنت' : 'Online meeting' }}</label>
        </div>

        <div class="form-group">
          <label class="form-label">{{ isRTL ? 'الوصف' : 'Description' }}</label>
          <Textarea
            v-model="newEvent.description"
            :placeholder="isRTL ? 'أضف وصفًا' : 'Add description'"
            rows="3"
            class="form-input"
          />
        </div>

        <div class="form-group">
          <label class="form-label">{{ isRTL ? 'التقويم' : 'Calendar' }}</label>
          <Dropdown
            v-model="newEvent.calendarId"
            :options="calendars"
            :optionLabel="(opt: CalendarItem) => isRTL && opt.nameAr ? opt.nameAr : opt.name"
            optionValue="id"
            class="form-input"
          />
        </div>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <Button
            :label="isRTL ? 'إلغاء' : 'Cancel'"
            severity="secondary"
            outlined
            @click="showCreateEventDialog = false"
          />
          <Button
            :label="isRTL ? 'إنشاء' : 'Create'"
            icon="pi pi-check"
            @click="createEvent"
          />
        </div>
      </template>
    </Dialog>

    <!-- Event Detail Dialog -->
    <Dialog
      v-model:visible="showEventDetailDialog"
      :header="selectedEvent ? getEventTitle(selectedEvent) : ''"
      :style="{ width: '500px' }"
      modal
      class="premium-dialog"
    >
      <div v-if="selectedEvent" class="event-detail">
        <div class="detail-item">
          <i class="pi pi-clock"></i>
          <span>{{ formatEventTime(selectedEvent) }}</span>
        </div>

        <div v-if="selectedEvent.location" class="detail-item">
          <i class="pi pi-map-marker"></i>
          <span>{{ selectedEvent.location }}</span>
        </div>

        <div v-if="selectedEvent.isOnline" class="detail-item">
          <i class="pi pi-video"></i>
          <a :href="selectedEvent.onlineMeetingUrl" target="_blank" class="join-link">
            {{ isRTL ? 'انضمام إلى الاجتماع' : 'Join meeting' }}
          </a>
        </div>

        <div v-if="selectedEvent.description" class="detail-description">
          <p>{{ selectedEvent.description }}</p>
        </div>

        <div v-if="selectedEvent.attendees?.length" class="detail-attendees">
          <h5>{{ isRTL ? 'الحضور' : 'Attendees' }} ({{ selectedEvent.attendees.length }})</h5>
          <div class="attendees-list">
            <div
              v-for="attendee in selectedEvent.attendees.slice(0, 5)"
              :key="attendee.id"
              class="attendee-item"
            >
              <Avatar :name="attendee.name" shape="circle" size="sm" />
              <span class="attendee-name">{{ attendee.name }}</span>
              <span
                class="attendee-rsvp"
                :style="{
                  color: getRsvpConfig(attendee.rsvpStatus).color,
                  backgroundColor: getRsvpConfig(attendee.rsvpStatus).bgColor
                }"
              >
                {{ isRTL ? getRsvpConfig(attendee.rsvpStatus).labelAr : getRsvpConfig(attendee.rsvpStatus).label }}
              </span>
            </div>
          </div>
        </div>

        <div class="rsvp-section">
          <span class="rsvp-label">{{ isRTL ? 'ردك' : 'Your Response' }}</span>
          <div class="rsvp-buttons">
            <button
              :class="['rsvp-btn accept', { active: selectedEvent.myRsvpStatus === 'Accepted' }]"
              @click="rsvp('Accepted')"
            >
              {{ isRTL ? 'قبول' : 'Accept' }}
            </button>
            <button
              :class="['rsvp-btn maybe', { active: selectedEvent.myRsvpStatus === 'Tentative' }]"
              @click="rsvp('Tentative')"
            >
              {{ isRTL ? 'ربما' : 'Maybe' }}
            </button>
            <button
              :class="['rsvp-btn decline', { active: selectedEvent.myRsvpStatus === 'Declined' }]"
              @click="rsvp('Declined')"
            >
              {{ isRTL ? 'رفض' : 'Decline' }}
            </button>
          </div>
        </div>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <Button
            v-if="selectedEvent?.canDelete"
            :label="isRTL ? 'حذف' : 'Delete'"
            severity="danger"
            text
            @click="deleteEvent"
          />
          <Button
            v-if="selectedEvent?.canEdit"
            :label="isRTL ? 'تعديل' : 'Edit'"
            severity="secondary"
            outlined
          />
        </div>
      </template>
    </Dialog>
  </div>
</template>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

// ============================================
// CALENDAR VIEW - MAIN CONTAINER
// Following Universal Spacing Guidelines from _mixins.scss
// ============================================

.calendar-view {
  @include page-view;

  &.rtl {
    direction: rtl;

    .breadcrumb-separator {
      transform: rotate(180deg);
    }
  }
}

// Header button styles (used in PageHeader slot)
.header-btn-primary {
  @include header-btn-primary;
}

.header-btn-secondary {
  @include header-btn-secondary;
}

// ============================================
// LOADING STATE
// ============================================

.loading-grid {
  @include content-grid(320px);
  padding-top: 0; // Stats bar provides spacing
}

.loading-sidebar {
  display: flex;
  flex-direction: column;
}

// ============================================
// CALENDAR CONTENT LAYOUT - Using standardized content-grid mixin
// ============================================

.calendar-content {
  @include content-grid(320px);
}

// ============================================
// SIDEBAR
// ============================================

.calendar-sidebar {
  display: flex;
  flex-direction: column;
  gap: $spacing-4;
}

.sidebar-card {
  @include card-base;
  padding: $spacing-4;
  overflow: hidden;
}

.card-header {
  @include flex-between;
  margin-bottom: $spacing-4;

  h4 {
    margin: 0;
    font-size: $font-size-base;
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }
}

.add-btn {
  width: 28px;
  height: 28px;
  border-radius: $radius-md;
  border: none;
  background: $slate-100;
  color: $slate-600;
  cursor: pointer;
  @include flex-center;
  transition: all $transition-fast;

  &:hover {
    background: $intalio-teal-500;
    color: white;
  }
}

// Mini Calendar
.mini-calendar {
  width: 100%;

  :deep(.p-datepicker) {
    border: none;
    padding: 0;
    width: 100%;

    .p-datepicker-header {
      padding: 0 0 $spacing-3;
      border: none;
      background: transparent;
    }

    .p-datepicker-calendar {
      font-size: $font-size-sm;

      th {
        font-weight: $font-weight-medium;
        color: $slate-500;
        padding: $spacing-2;
      }

      td {
        padding: $spacing-1;

        > span {
          width: 32px;
          height: 32px;
          border-radius: $radius-md;

          &.p-highlight {
            background: $intalio-teal-500;
            color: white;
          }
        }
      }
    }
  }
}

// Calendars List
.calendars-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;
}

.calendar-item {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  padding: $spacing-2;
  border-radius: $radius-md;
  cursor: pointer;
  transition: background $transition-fast;

  &:hover {
    background: $slate-50;
  }
}

.calendar-dot {
  width: 10px;
  height: 10px;
  border-radius: $radius-full;
}

.calendar-name {
  flex: 1;
  font-size: $font-size-sm;
  color: $slate-700;
}

.default-badge {
  font-size: $font-size-xs;
  padding: 2px $spacing-2;
  background: $slate-100;
  color: $slate-500;
  border-radius: $radius-sm;
}

// Upcoming Events
.upcoming-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;
}

.upcoming-item {
  display: flex;
  gap: $spacing-3;
  padding: $spacing-3;
  border-radius: $radius-lg;
  cursor: pointer;
  transition: all $transition-fast;

  &:hover {
    background: $slate-50;
  }
}

.event-indicator {
  width: 4px;
  border-radius: $radius-full;
  flex-shrink: 0;
}

.event-info {
  flex: 1;
  min-width: 0;

  .event-title {
    display: block;
    font-size: $font-size-sm;
    font-weight: $font-weight-medium;
    color: $slate-900;
    @include truncate;
  }

  .event-time {
    font-size: $font-size-xs;
    color: $slate-500;
  }
}

.empty-state-mini {
  text-align: center;
  padding: $spacing-6;
  color: $slate-400;

  i {
    font-size: 2rem;
    margin-bottom: $spacing-2;
  }

  p {
    margin: 0;
    font-size: $font-size-sm;
  }
}

// ============================================
// MAIN CALENDAR
// ============================================

.calendar-main {
  min-width: 0;
}

.main-card {
  @include card-base;
  overflow: hidden;
}

// Calendar Toolbar
.calendar-toolbar {
  @include flex-between;
  padding: $spacing-4 $spacing-5;
  border-bottom: 1px solid $slate-100;
  gap: $spacing-4;
  flex-wrap: wrap;
}

.toolbar-left {
  display: flex;
  align-items: center;
  gap: $spacing-3;
}

.nav-btn {
  width: 36px;
  height: 36px;
  border-radius: $radius-lg;
  border: 1px solid $slate-200;
  background: white;
  color: $slate-600;
  cursor: pointer;
  @include flex-center;
  transition: all $transition-fast;

  &:hover {
    background: $slate-50;
    border-color: $slate-300;
  }
}

.period-title {
  margin: 0;
  font-size: $font-size-lg;
  font-weight: $font-weight-semibold;
  color: $slate-900;
  min-width: 200px;
  text-align: center;
}

.view-toggle {
  @include view-toggle;
}

.view-btn {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-2 $spacing-3;
  border: none;
  background: transparent;
  color: $slate-600;
  font-size: $font-size-sm;
  font-weight: $font-weight-medium;
  cursor: pointer;
  border-radius: $radius-md;
  transition: all $transition-fast;

  &:hover {
    background: $slate-100;
  }

  &.active {
    background: $intalio-teal-500;
    color: white;
  }

  span {
    @include mobile {
      display: none;
    }
  }
}

// ============================================
// MONTH VIEW
// ============================================

.month-view {
  padding: $spacing-4;
}

.week-header {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  margin-bottom: $spacing-2;
}

.week-day-name {
  text-align: center;
  font-size: $font-size-sm;
  font-weight: $font-weight-semibold;
  color: $slate-500;
  padding: $spacing-2;
}

.month-grid {
  border: 1px solid $slate-200;
  border-radius: $radius-lg;
  overflow: hidden;
}

.week-row {
  display: grid;
  grid-template-columns: repeat(7, 1fr);

  &:not(:last-child) {
    border-bottom: 1px solid $slate-100;
  }
}

.day-cell {
  min-height: 100px;
  padding: $spacing-2;
  cursor: pointer;
  transition: background $transition-fast;
  border-right: 1px solid $slate-100;

  &:last-child {
    border-right: none;
  }

  &:hover {
    background: $slate-50;
  }

  &.other-month {
    background: $slate-50;

    .day-number {
      color: $slate-400;
    }
  }

  &.today {
    background: rgba($intalio-teal-500, 0.05);

    .day-number {
      background: $intalio-teal-500;
      color: white;
      border-radius: $radius-full;
      width: 28px;
      height: 28px;
      @include flex-center;
    }
  }
}

.day-header {
  @include flex-between;
  margin-bottom: $spacing-1;
}

.day-number {
  font-size: $font-size-sm;
  font-weight: $font-weight-medium;
  color: $slate-700;
}

.more-events {
  font-size: $font-size-xs;
  color: $slate-500;
}

.day-events {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.event-chip {
  font-size: 11px;
  padding: 2px $spacing-2;
  border-radius: $radius-sm;
  border-left: 3px solid;
  @include truncate;
  cursor: pointer;
  transition: transform $transition-fast;

  &:hover {
    transform: scale(1.02);
  }
}

.event-time-mini {
  font-weight: $font-weight-medium;
  margin-right: $spacing-1;
}

// ============================================
// WEEK VIEW
// ============================================

.week-view {
  overflow-x: auto;
}

.week-header-row {
  display: flex;
  border-bottom: 1px solid $slate-200;
  position: sticky;
  top: 0;
  background: white;
  z-index: 10;
}

.time-column-header {
  width: 70px;
  flex-shrink: 0;
}

.week-day-header {
  flex: 1;
  text-align: center;
  padding: $spacing-3;
  border-left: 1px solid $slate-100;

  &.today {
    background: rgba($intalio-teal-500, 0.05);

    .day-num {
      background: $intalio-teal-500;
      color: white;
      border-radius: $radius-full;
      width: 32px;
      height: 32px;
      display: inline-flex;
      align-items: center;
      justify-content: center;
    }
  }
}

.day-name {
  display: block;
  font-size: $font-size-xs;
  color: $slate-500;
  margin-bottom: $spacing-1;
}

.day-num {
  font-size: $font-size-lg;
  font-weight: $font-weight-semibold;
  color: $slate-900;
}

.week-body {
  max-height: 600px;
  overflow-y: auto;
  @include custom-scrollbar;
}

.time-row {
  display: flex;
  min-height: 50px;
  border-bottom: 1px solid $slate-100;

  &:hover {
    .time-cell:not(.weekend) {
      background: rgba($intalio-teal-500, 0.02);
    }
  }
}

.time-label {
  width: 70px;
  flex-shrink: 0;
  padding: $spacing-2;
  font-size: $font-size-xs;
  color: $slate-500;
  text-align: right;
  padding-right: $spacing-3;
}

.time-cell {
  flex: 1;
  border-left: 1px solid $slate-100;
  padding: 2px;
  cursor: pointer;
  transition: background $transition-fast;

  &.weekend {
    background: $slate-50;
  }

  &:hover {
    background: rgba($intalio-teal-500, 0.05);
  }
}

.week-event {
  font-size: 11px;
  padding: $spacing-1 $spacing-2;
  border-radius: $radius-sm;
  color: white;
  @include truncate;
  cursor: pointer;
  transition: transform $transition-fast;

  &:hover {
    transform: scale(1.02);
  }
}

// ============================================
// DAY VIEW
// ============================================

.day-view {
  padding: $spacing-4;
}

.day-header-info {
  padding: $spacing-4;
  text-align: center;
  border-bottom: 1px solid $slate-100;
  margin-bottom: $spacing-4;

  h3 {
    margin: 0;
    font-size: $font-size-xl;
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }
}

.day-body {
  max-height: 600px;
  overflow-y: auto;
  @include custom-scrollbar;
}

.full-width {
  flex: 1 !important;
}

.day-event {
  padding: $spacing-3;
  border-radius: $radius-md;
  color: white;
  cursor: pointer;
  transition: transform $transition-fast;

  &:hover {
    transform: scale(1.01);
  }

  .event-title {
    font-weight: $font-weight-semibold;
    margin-bottom: $spacing-1;
  }

  .event-time-detail {
    font-size: $font-size-sm;
    opacity: 0.9;
  }
}

// ============================================
// AGENDA VIEW
// ============================================

.agenda-view {
  padding: $spacing-4;
}

.empty-state {
  @include empty-state;
  padding: $spacing-12;
}

.empty-icon {
  @include empty-state-icon;
}

.agenda-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-6;
}

.agenda-day {
  animation: fadeInUp 0.4s ease-out both;
}

.agenda-day-header {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  padding: $spacing-3;
  background: $slate-50;
  border-radius: $radius-lg;
  margin-bottom: $spacing-3;

  &.today {
    background: rgba($intalio-teal-500, 0.1);

    .day-name, .day-date {
      color: $intalio-teal-700;
    }
  }

  .day-name {
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }

  .day-date {
    color: $slate-500;
  }
}

.today-badge {
  margin-left: auto;
  font-size: $font-size-xs;
  font-weight: $font-weight-semibold;
  padding: $spacing-1 $spacing-2;
  background: $intalio-teal-500;
  color: white;
  border-radius: $radius-md;
}

.agenda-events {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;
  padding-left: $spacing-4;
}

.agenda-event {
  display: flex;
  gap: $spacing-4;
  padding: $spacing-4;
  background: white;
  border-radius: $radius-xl;
  box-shadow: $shadow-sm;
  cursor: pointer;
  transition: all $transition-base;

  &:hover {
    box-shadow: $shadow-md;
    transform: translateX(4px);
  }
}

.event-bar {
  width: 4px;
  border-radius: $radius-full;
  flex-shrink: 0;
}

.event-content {
  flex: 1;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: $spacing-4;
}

.event-main {
  flex: 1;
  min-width: 0;

  .event-title {
    margin: 0 0 $spacing-2;
    font-size: $font-size-base;
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }
}

.event-meta {
  display: flex;
  flex-wrap: wrap;
  gap: $spacing-3;
  font-size: $font-size-sm;
  color: $slate-500;

  span {
    display: flex;
    align-items: center;
    gap: $spacing-1;
  }
}

.event-actions {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  flex-shrink: 0;
}

.attendees-info {
  display: flex;
  align-items: center;
  gap: $spacing-1;
  font-size: $font-size-sm;
  color: $slate-500;
}

.rsvp-badge {
  font-size: $font-size-xs;
  font-weight: $font-weight-medium;
  padding: $spacing-1 $spacing-2;
  border-radius: $radius-md;
}

// ============================================
// DIALOGS
// ============================================

.premium-dialog {
  :deep(.p-dialog-header) {
    padding: $spacing-5;
    border-bottom: 1px solid $slate-100;
  }

  :deep(.p-dialog-content) {
    padding: $spacing-5;
  }

  :deep(.p-dialog-footer) {
    padding: $spacing-4 $spacing-5;
    border-top: 1px solid $slate-100;
  }
}

.dialog-form {
  display: flex;
  flex-direction: column;
  gap: $spacing-4;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: $spacing-4;

  @include mobile {
    grid-template-columns: 1fr;
  }
}

.form-label {
  font-size: $font-size-sm;
  font-weight: $font-weight-medium;
  color: $slate-700;
}

.form-input {
  width: 100%;
}

.checkbox-group {
  flex-direction: row;
  align-items: center;
  gap: $spacing-2;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: $spacing-3;
}

// Event Detail
.event-detail {
  display: flex;
  flex-direction: column;
  gap: $spacing-4;
}

.detail-item {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  padding: $spacing-3;
  background: $slate-50;
  border-radius: $radius-lg;

  i {
    color: $slate-500;
  }
}

.join-link {
  color: $intalio-teal-600;
  text-decoration: none;
  font-weight: $font-weight-medium;

  &:hover {
    text-decoration: underline;
  }
}

.detail-description {
  padding: $spacing-4;
  background: $slate-50;
  border-radius: $radius-lg;

  p {
    margin: 0;
    color: $slate-700;
    line-height: 1.6;
  }
}

.detail-attendees {
  h5 {
    margin: 0 0 $spacing-3;
    font-size: $font-size-sm;
    font-weight: $font-weight-semibold;
    color: $slate-700;
  }
}

.attendees-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;
}

.attendee-item {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  padding: $spacing-2;
  background: $slate-50;
  border-radius: $radius-md;
}

.attendee-name {
  flex: 1;
  font-size: $font-size-sm;
  color: $slate-700;
}

.attendee-rsvp {
  font-size: $font-size-xs;
  font-weight: $font-weight-medium;
  padding: 2px $spacing-2;
  border-radius: $radius-sm;
}

.rsvp-section {
  padding-top: $spacing-4;
  border-top: 1px solid $slate-100;
}

.rsvp-label {
  display: block;
  font-size: $font-size-sm;
  font-weight: $font-weight-medium;
  color: $slate-700;
  margin-bottom: $spacing-3;
}

.rsvp-buttons {
  display: flex;
  gap: $spacing-2;
}

.rsvp-btn {
  flex: 1;
  padding: $spacing-2 $spacing-3;
  border: 2px solid transparent;
  border-radius: $radius-lg;
  font-size: $font-size-sm;
  font-weight: $font-weight-medium;
  cursor: pointer;
  transition: all $transition-fast;

  &.accept {
    background: rgba($success-500, 0.1);
    color: $success-600;
    border-color: rgba($success-500, 0.2);

    &.active, &:hover {
      background: $success-500;
      color: white;
      border-color: $success-500;
    }
  }

  &.maybe {
    background: rgba($warning-500, 0.1);
    color: $warning-600;
    border-color: rgba($warning-500, 0.2);

    &.active, &:hover {
      background: $warning-500;
      color: white;
      border-color: $warning-500;
    }
  }

  &.decline {
    background: rgba($error-500, 0.1);
    color: $error-600;
    border-color: rgba($error-500, 0.2);

    &.active, &:hover {
      background: $error-500;
      color: white;
      border-color: $error-500;
    }
  }
}

// ============================================
// RTL SUPPORT
// ============================================

.rtl {
  .agenda-event:hover {
    transform: translateX(-4px);
  }

  .event-chip {
    border-left: none;
    border-right: 3px solid;
  }

  .time-label {
    text-align: left;
    padding-left: $spacing-3;
    padding-right: $spacing-2;
  }

  .agenda-events {
    padding-left: 0;
    padding-right: $spacing-4;
  }

  .today-badge {
    margin-left: 0;
    margin-right: auto;
  }
}

// ============================================
// ANIMATIONS
// ============================================

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes viewFadeIn {
  from {
    opacity: 0;
    transform: scale(0.98);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

@keyframes eventChipPop {
  0% {
    transform: scale(0.9);
    opacity: 0;
  }
  50% {
    transform: scale(1.02);
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}

@keyframes dayCellFadeIn {
  from {
    opacity: 0;
    transform: translateY(8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

// View transition animations
.month-view,
.week-view,
.day-view,
.agenda-view {
  &.view--animated {
    opacity: 0;
    transform: scale(0.98);
  }

  &.view--visible {
    animation: viewFadeIn 0.35s ease-out forwards;
  }

  &:not(.view--animated) {
    opacity: 1;
    transform: none;
    animation: none;
  }
}

// Month view day cell stagger animation
.month-view.view--animated.view--visible {
  .week-row {
    .day-cell {
      animation: dayCellFadeIn 0.3s ease-out both;
    }

    @for $w from 0 through 5 {
      &:nth-child(#{$w + 1}) {
        .day-cell {
          @for $d from 0 through 6 {
            &:nth-child(#{$d + 1}) {
              animation-delay: calc((#{$w * 7 + $d}) * 20ms);
            }
          }
        }
      }
    }
  }
}

// Event chip animation
.event-chip {
  transition: transform 0.2s ease, box-shadow 0.2s ease;

  &:hover {
    transform: scale(1.03) translateY(-1px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  }
}

// Week event animation
.week-event {
  transition: transform 0.2s ease, box-shadow 0.2s ease;

  &:hover {
    transform: scale(1.03);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
  }
}

// Day event animation
.day-event {
  transition: transform 0.2s ease, box-shadow 0.2s ease;

  &:hover {
    transform: scale(1.02) translateY(-2px);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
  }
}

// Agenda event hover enhancement
.agenda-event {
  transition: all 0.25s ease;

  &:hover {
    box-shadow:
      0 10px 25px rgba(0, 0, 0, 0.1),
      0 4px 10px rgba(0, 0, 0, 0.05);
    transform: translateX(6px);
  }
}

// Day cell hover effect
.day-cell {
  transition: background 0.2s ease, transform 0.2s ease;

  &:hover:not(.other-month) {
    background: rgba($intalio-teal-500, 0.08);
    transform: scale(1.02);
    z-index: 1;
  }
}

// View button transition
.view-btn {
  transition: all 0.2s ease;

  &:active {
    transform: scale(0.95);
  }
}

// Navigation button animation
.nav-btn {
  transition: all 0.2s ease;

  &:active {
    transform: scale(0.9);
  }
}

// Sidebar card entrance
.sidebar-card {
  transition: box-shadow 0.3s ease;

  &:hover {
    box-shadow: $shadow-md;
  }
}

// Upcoming item hover
.upcoming-item {
  transition: all 0.2s ease;

  &:hover {
    background: rgba($intalio-teal-500, 0.06);
    transform: translateX(4px);
  }
}

@include staggered-animation-delays('.agenda-day', 10, 0.05s);

// ============================================
// RTL ANIMATION ADJUSTMENTS
// ============================================

.rtl {
  .agenda-event:hover {
    transform: translateX(-6px);
  }

  .upcoming-item:hover {
    transform: translateX(-4px);
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================

@media (prefers-reduced-motion: reduce) {
  .month-view,
  .week-view,
  .day-view,
  .agenda-view {
    &.view--animated {
      opacity: 1;
      transform: none;
      animation: none !important;
    }

    .day-cell {
      animation: none !important;
    }
  }

  .event-chip,
  .week-event,
  .day-event,
  .agenda-event,
  .day-cell,
  .upcoming-item,
  .sidebar-card {
    transition: background-color 0.15s, box-shadow 0.15s;

    &:hover {
      transform: none;
    }
  }

  .view-btn,
  .nav-btn {
    transition: background-color 0.15s;

    &:active {
      transform: none;
    }
  }

  .agenda-day {
    animation: none !important;
  }
}
</style>
