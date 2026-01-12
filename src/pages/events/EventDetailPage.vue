<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import { CommentsSection, SocialShareButtons, RelatedContentCarousel, BookmarkButton } from '@/components/common'
import type { EventAttendee, EventAgendaItem, EventSpeaker, Author, Attachment } from '@/types/detail-pages'

const route = useRoute()
const router = useRouter()

// State
const isLoading = ref(true)
const activeTab = ref('description')
const rsvpStatus = ref<'going' | 'maybe' | 'not_going' | null>(null)
const isRsvpLoading = ref(false)
const showShareModal = ref(false)
const showCalendarModal = ref(false)
const showAllAttendees = ref(false)

// Event Data
const event = ref({
  id: route.params.id as string,
  title: 'AFC Asian Cup 2027 Opening Ceremony Planning Meeting',
  description: `<p>Join us for an important planning session for the AFC Asian Cup 2027 Opening Ceremony. This meeting will bring together key stakeholders, event coordinators, and creative teams to finalize the ceremony's theme, logistics, and production timeline.</p>
  <p class="mt-4">We'll be discussing:</p>
  <ul class="list-disc ml-6 mt-2 space-y-1">
    <li>Ceremony theme and creative direction</li>
    <li>Artist and performer lineup</li>
    <li>Technical production requirements</li>
    <li>Safety and security protocols</li>
    <li>Media coverage and broadcast schedule</li>
  </ul>
  <p class="mt-4">Your input and expertise are crucial to making this a world-class event that showcases the best of Asian football culture.</p>`,
  coverImage: 'https://picsum.photos/seed/event-cover/1200/400',
  startDate: new Date('2026-02-15T09:00:00'),
  endDate: new Date('2026-02-15T17:00:00'),
  location: 'King Fahd International Stadium, Riyadh',
  virtualLink: 'https://teams.microsoft.com/meeting/123',
  isVirtual: true,
  isHybrid: true,
  category: 'Planning',
  status: 'upcoming',
  capacity: 150,
  organizer: {
    id: 'org-1',
    name: 'Sarah Al-Hassan',
    initials: 'SA',
    role: 'Event Director',
    department: 'Tournament Operations',
    email: 'sarah.alhassan@afc2027.com'
  } as Author,
  tags: ['Planning', 'Opening Ceremony', 'AFC 2027', 'Stadium']
})

// Attendees
const attendees = ref<EventAttendee[]>([
  { id: '1', name: 'Ahmed Imam', initials: 'AI', status: 'going', respondedAt: new Date() },
  { id: '2', name: 'Sarah Chen', initials: 'SC', status: 'going', respondedAt: new Date() },
  { id: '3', name: 'Mike Johnson', initials: 'MJ', status: 'going', respondedAt: new Date() },
  { id: '4', name: 'Emily Davis', initials: 'ED', status: 'maybe', respondedAt: new Date() },
  { id: '5', name: 'Alex Thompson', initials: 'AT', status: 'going', respondedAt: new Date() },
  { id: '6', name: 'Lisa Wang', initials: 'LW', status: 'going', respondedAt: new Date() },
  { id: '7', name: 'David Kim', initials: 'DK', status: 'maybe', respondedAt: new Date() },
  { id: '8', name: 'Maria Garcia', initials: 'MG', status: 'not_going', respondedAt: new Date() }
])

// Agenda
const agenda = ref<EventAgendaItem[]>([
  { id: '1', time: '09:00', title: 'Registration & Welcome Coffee', description: 'Check-in and networking', duration: 30, type: 'networking' },
  { id: '2', time: '09:30', title: 'Opening Remarks', description: 'Welcome address by the Event Director', speaker: event.value.organizer, duration: 15, type: 'keynote' },
  { id: '3', time: '09:45', title: 'Creative Vision Presentation', description: 'Overview of the ceremony theme and creative direction', duration: 45, type: 'session' },
  { id: '4', time: '10:30', title: 'Coffee Break', duration: 15, type: 'break' },
  { id: '5', time: '10:45', title: 'Technical Production Workshop', description: 'Deep dive into staging, lighting, and sound requirements', duration: 90, type: 'workshop' },
  { id: '6', time: '12:15', title: 'Lunch Break', duration: 60, type: 'break' },
  { id: '7', time: '13:15', title: 'Safety & Security Briefing', description: 'Review of protocols and emergency procedures', duration: 45, type: 'session' },
  { id: '8', time: '14:00', title: 'Media & Broadcast Planning', description: 'Coverage schedule and press coordination', duration: 60, type: 'session' },
  { id: '9', time: '15:00', title: 'Coffee Break', duration: 15, type: 'break' },
  { id: '10', time: '15:15', title: 'Interactive Q&A Session', description: 'Open discussion and feedback', duration: 60, type: 'session' },
  { id: '11', time: '16:15', title: 'Closing Remarks & Next Steps', description: 'Summary and action items', duration: 30, type: 'keynote' },
  { id: '12', time: '16:45', title: 'Networking Reception', duration: 75, type: 'networking' }
])

// Speakers
const speakers = ref<EventSpeaker[]>([
  {
    id: '1',
    name: 'Sarah Al-Hassan',
    avatar: '',
    title: 'Event Director',
    company: 'AFC 2027 Organizing Committee',
    bio: 'Sarah has over 15 years of experience in major sporting event management, including the 2022 World Cup.',
    sessions: ['Opening Remarks', 'Closing Remarks'],
    socialLinks: { linkedin: '#', twitter: '#' }
  },
  {
    id: '2',
    name: 'James Chen',
    avatar: '',
    title: 'Creative Director',
    company: 'Global Events Productions',
    bio: 'James has designed opening ceremonies for Olympic Games, Asian Games, and multiple World Cup events.',
    sessions: ['Creative Vision Presentation'],
    socialLinks: { linkedin: '#' }
  },
  {
    id: '3',
    name: 'Dr. Mohammed Al-Rashid',
    avatar: '',
    title: 'Head of Security',
    company: 'Saudi Sports Authority',
    bio: 'Dr. Al-Rashid leads security operations for major events in Saudi Arabia with 20+ years of expertise.',
    sessions: ['Safety & Security Briefing'],
    socialLinks: {}
  }
])

// Documents
const documents = ref<Attachment[]>([
  { id: '1', name: 'Event Brief - Opening Ceremony.pdf', type: 'pdf', size: 2400000, url: '#', uploadedBy: event.value.organizer, uploadedAt: new Date() },
  { id: '2', name: 'Ceremony Timeline Draft.xlsx', type: 'xlsx', size: 156000, url: '#', uploadedBy: event.value.organizer, uploadedAt: new Date() },
  { id: '3', name: 'Stadium Layout Plans.pdf', type: 'pdf', size: 8500000, url: '#', uploadedBy: event.value.organizer, uploadedAt: new Date() },
  { id: '4', name: 'Creative Mood Board.pptx', type: 'pptx', size: 15200000, url: '#', uploadedBy: event.value.organizer, uploadedAt: new Date() }
])

// Computed
const goingCount = computed(() => attendees.value.filter(a => a.status === 'going').length)
const maybeCount = computed(() => attendees.value.filter(a => a.status === 'maybe').length)
const notGoingCount = computed(() => attendees.value.filter(a => a.status === 'not_going').length)

const displayedAttendees = computed(() => {
  if (showAllAttendees.value) return attendees.value.filter(a => a.status === 'going')
  return attendees.value.filter(a => a.status === 'going').slice(0, 6)
})

const formattedDate = computed(() => {
  const options: Intl.DateTimeFormatOptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }
  return event.value.startDate.toLocaleDateString('en-US', options)
})

const formattedTime = computed(() => {
  const start = event.value.startDate.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })
  const end = event.value.endDate.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })
  return `${start} - ${end}`
})

const daysUntilEvent = computed(() => {
  const now = new Date()
  const diff = event.value.startDate.getTime() - now.getTime()
  return Math.ceil(diff / (1000 * 60 * 60 * 24))
})

// Methods
onMounted(async () => {
  // Simulate loading
  await new Promise(resolve => setTimeout(resolve, 800))
  isLoading.value = false
})

function goBack() {
  router.push({ name: 'Events' })
}

async function handleRsvp(status: 'going' | 'maybe' | 'not_going') {
  isRsvpLoading.value = true
  try {
    await new Promise(resolve => setTimeout(resolve, 500))
    rsvpStatus.value = status
  } finally {
    isRsvpLoading.value = false
  }
}

function addToCalendar(type: 'google' | 'outlook' | 'ical') {
  const title = encodeURIComponent(event.value.title)
  const start = event.value.startDate.toISOString().replace(/[-:]/g, '').split('.')[0] + 'Z'
  const end = event.value.endDate.toISOString().replace(/[-:]/g, '').split('.')[0] + 'Z'
  const location = encodeURIComponent(event.value.location)
  const description = encodeURIComponent('AFC Asian Cup 2027 Planning Meeting')

  let url = ''
  switch (type) {
    case 'google':
      url = `https://calendar.google.com/calendar/render?action=TEMPLATE&text=${title}&dates=${start}/${end}&location=${location}&details=${description}`
      break
    case 'outlook':
      url = `https://outlook.live.com/calendar/0/deeplink/compose?subject=${title}&startdt=${event.value.startDate.toISOString()}&enddt=${event.value.endDate.toISOString()}&location=${location}&body=${description}`
      break
    case 'ical':
      // Generate ICS file download
      const icsContent = `BEGIN:VCALENDAR
VERSION:2.0
BEGIN:VEVENT
DTSTART:${start}
DTEND:${end}
SUMMARY:${event.value.title}
LOCATION:${event.value.location}
END:VEVENT
END:VCALENDAR`
      const blob = new Blob([icsContent], { type: 'text/calendar' })
      const link = document.createElement('a')
      link.href = URL.createObjectURL(blob)
      link.download = `${event.value.title}.ics`
      link.click()
      showCalendarModal.value = false
      return
  }

  window.open(url, '_blank')
  showCalendarModal.value = false
}

function getAgendaTypeIcon(type: EventAgendaItem['type']): string {
  const icons: Record<string, string> = {
    session: 'fas fa-chalkboard-teacher',
    break: 'fas fa-coffee',
    networking: 'fas fa-users',
    keynote: 'fas fa-microphone',
    workshop: 'fas fa-tools'
  }
  return icons[type] || 'fas fa-calendar'
}

function getAgendaTypeColor(type: EventAgendaItem['type']): string {
  const colors: Record<string, string> = {
    session: 'bg-blue-100 text-blue-600',
    break: 'bg-orange-100 text-orange-600',
    networking: 'bg-purple-100 text-purple-600',
    keynote: 'bg-teal-100 text-teal-600',
    workshop: 'bg-green-100 text-green-600'
  }
  return colors[type] || 'bg-gray-100 text-gray-600'
}

function getFileIcon(type: string): string {
  const icons: Record<string, string> = {
    pdf: 'fas fa-file-pdf text-red-500',
    xlsx: 'fas fa-file-excel text-green-500',
    xls: 'fas fa-file-excel text-green-500',
    pptx: 'fas fa-file-powerpoint text-orange-500',
    ppt: 'fas fa-file-powerpoint text-orange-500',
    docx: 'fas fa-file-word text-blue-500',
    doc: 'fas fa-file-word text-blue-500'
  }
  return icons[type] || 'fas fa-file text-gray-500'
}

function formatFileSize(bytes: number): string {
  if (bytes < 1024) return bytes + ' B'
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB'
  return (bytes / (1024 * 1024)).toFixed(1) + ' MB'
}

function getInitialsColor(initials: string): string {
  const colors = [
    'from-teal-400 to-teal-600',
    'from-blue-400 to-blue-600',
    'from-purple-400 to-purple-600',
    'from-pink-400 to-pink-600',
    'from-orange-400 to-orange-600',
    'from-green-400 to-green-600'
  ]
  const index = initials.charCodeAt(0) % colors.length
  return colors[index]
}
</script>

<template>
  <div class="event-detail-page">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-[60vh]">
      <LoadingSpinner size="lg" text="Loading event details..." />
    </div>

    <template v-else>
      <!-- Hero Section -->
      <div class="relative h-64 md:h-80 rounded-2xl overflow-hidden mb-6">
        <img :src="event.coverImage" :alt="event.title" class="w-full h-full object-cover">
        <div class="absolute inset-0 bg-gradient-to-t from-black/80 via-black/40 to-transparent"></div>

        <!-- Back Button -->
        <button @click="goBack" class="absolute top-4 left-4 px-4 py-2 bg-white/20 backdrop-blur-sm text-white rounded-lg hover:bg-white/30 transition-colors">
          <i class="fas fa-arrow-left mr-2"></i>Back to Events
        </button>

        <!-- Date Badge -->
        <div class="absolute top-4 right-4 bg-white rounded-xl shadow-lg p-3 text-center min-w-[80px]">
          <p class="text-xs text-gray-500 uppercase font-medium">{{ event.startDate.toLocaleDateString('en-US', { month: 'short' }) }}</p>
          <p class="text-3xl font-bold text-teal-600">{{ event.startDate.getDate() }}</p>
          <p class="text-xs text-gray-500">{{ event.startDate.getFullYear() }}</p>
        </div>

        <!-- Event Info Overlay -->
        <div class="absolute bottom-0 left-0 right-0 p-6">
          <div class="flex items-center gap-2 mb-2">
            <span class="px-3 py-1 bg-teal-500 text-white text-sm font-medium rounded-full">{{ event.category }}</span>
            <span v-if="event.isHybrid" class="px-3 py-1 bg-purple-500 text-white text-sm font-medium rounded-full">Hybrid Event</span>
            <span v-else-if="event.isVirtual" class="px-3 py-1 bg-blue-500 text-white text-sm font-medium rounded-full">Virtual</span>
          </div>
          <h1 class="text-2xl md:text-3xl font-bold text-white mb-2">{{ event.title }}</h1>
          <div class="flex flex-wrap items-center gap-4 text-white/90 text-sm">
            <span><i class="far fa-calendar mr-2"></i>{{ formattedDate }}</span>
            <span><i class="far fa-clock mr-2"></i>{{ formattedTime }}</span>
            <span><i class="fas fa-map-marker-alt mr-2"></i>{{ event.location }}</span>
          </div>
        </div>
      </div>

      <!-- Action Bar -->
      <div class="flex flex-wrap items-center justify-between gap-4 mb-6 p-4 bg-white rounded-xl shadow-sm border border-gray-100">
        <div class="flex items-center gap-3">
          <!-- RSVP Buttons -->
          <div class="flex items-center gap-2 p-1 bg-gray-100 rounded-lg">
            <button
              @click="handleRsvp('going')"
              :disabled="isRsvpLoading"
              :class="[
                'px-4 py-2 rounded-lg font-medium text-sm transition-all',
                rsvpStatus === 'going' ? 'bg-green-500 text-white shadow-md' : 'text-gray-600 hover:bg-gray-200'
              ]"
            >
              <i class="fas fa-check mr-1"></i>Going
            </button>
            <button
              @click="handleRsvp('maybe')"
              :disabled="isRsvpLoading"
              :class="[
                'px-4 py-2 rounded-lg font-medium text-sm transition-all',
                rsvpStatus === 'maybe' ? 'bg-yellow-500 text-white shadow-md' : 'text-gray-600 hover:bg-gray-200'
              ]"
            >
              <i class="fas fa-question mr-1"></i>Maybe
            </button>
            <button
              @click="handleRsvp('not_going')"
              :disabled="isRsvpLoading"
              :class="[
                'px-4 py-2 rounded-lg font-medium text-sm transition-all',
                rsvpStatus === 'not_going' ? 'bg-red-500 text-white shadow-md' : 'text-gray-600 hover:bg-gray-200'
              ]"
            >
              <i class="fas fa-times mr-1"></i>Can't Go
            </button>
          </div>

          <!-- Days Until Event -->
          <div v-if="daysUntilEvent > 0" class="hidden md:flex items-center gap-2 text-sm text-gray-500">
            <i class="fas fa-hourglass-half text-teal-500"></i>
            <span>{{ daysUntilEvent }} days until event</span>
          </div>
        </div>

        <div class="flex items-center gap-3">
          <button
            @click="showCalendarModal = true"
            class="px-4 py-2 bg-teal-50 text-teal-600 rounded-lg font-medium text-sm hover:bg-teal-100 transition-colors"
          >
            <i class="fas fa-calendar-plus mr-2"></i>Add to Calendar
          </button>
          <BookmarkButton content-id="event-1" content-type="event" />
          <SocialShareButtons :title="event.title" layout="horizontal" size="sm" />
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Left Column - Main Content -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Tabs -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
            <div class="flex border-b border-gray-100">
              <button
                v-for="tab in [
                  { id: 'description', label: 'Description', icon: 'fas fa-info-circle' },
                  { id: 'agenda', label: 'Agenda', icon: 'fas fa-list-ol' },
                  { id: 'speakers', label: 'Speakers', icon: 'fas fa-users' },
                  { id: 'documents', label: 'Documents', icon: 'fas fa-folder' }
                ]"
                :key="tab.id"
                @click="activeTab = tab.id"
                :class="[
                  'flex-1 px-4 py-3 text-sm font-medium transition-colors flex items-center justify-center gap-2',
                  activeTab === tab.id
                    ? 'text-teal-600 border-b-2 border-teal-500 bg-teal-50/50'
                    : 'text-gray-500 hover:text-gray-700 hover:bg-gray-50'
                ]"
              >
                <i :class="tab.icon"></i>
                {{ tab.label }}
              </button>
            </div>

            <!-- Tab Content -->
            <div class="p-6">
              <!-- Description Tab -->
              <div v-if="activeTab === 'description'" v-html="event.description" class="prose prose-teal max-w-none"></div>

              <!-- Agenda Tab -->
              <div v-else-if="activeTab === 'agenda'" class="space-y-4">
                <div
                  v-for="item in agenda"
                  :key="item.id"
                  class="flex gap-4 p-4 rounded-xl hover:bg-gray-50 transition-colors"
                >
                  <div class="text-center min-w-[60px]">
                    <p class="text-lg font-bold text-gray-900">{{ item.time }}</p>
                    <p class="text-xs text-gray-500">{{ item.duration }} min</p>
                  </div>
                  <div class="flex-1">
                    <div class="flex items-center gap-2 mb-1">
                      <span :class="['px-2 py-0.5 rounded-full text-xs font-medium', getAgendaTypeColor(item.type)]">
                        <i :class="[getAgendaTypeIcon(item.type), 'mr-1']"></i>
                        {{ item.type }}
                      </span>
                    </div>
                    <h4 class="font-semibold text-gray-900">{{ item.title }}</h4>
                    <p v-if="item.description" class="text-sm text-gray-600 mt-1">{{ item.description }}</p>
                    <p v-if="item.speaker" class="text-sm text-teal-600 mt-1">
                      <i class="fas fa-user mr-1"></i>{{ item.speaker.name }}
                    </p>
                  </div>
                </div>
              </div>

              <!-- Speakers Tab -->
              <div v-else-if="activeTab === 'speakers'" class="grid gap-6">
                <div
                  v-for="speaker in speakers"
                  :key="speaker.id"
                  class="flex gap-4 p-4 bg-gray-50 rounded-xl"
                >
                  <div :class="['w-16 h-16 rounded-xl flex items-center justify-center text-white font-bold text-xl bg-gradient-to-br', getInitialsColor(speaker.name.split(' ').map(n => n[0]).join(''))]">
                    {{ speaker.name.split(' ').map(n => n[0]).join('') }}
                  </div>
                  <div class="flex-1">
                    <h4 class="font-semibold text-gray-900">{{ speaker.name }}</h4>
                    <p class="text-sm text-gray-600">{{ speaker.title }}</p>
                    <p class="text-sm text-teal-600">{{ speaker.company }}</p>
                    <p class="text-sm text-gray-500 mt-2">{{ speaker.bio }}</p>
                    <div class="flex items-center gap-2 mt-2">
                      <span class="text-xs text-gray-500">Sessions:</span>
                      <span v-for="(session, idx) in speaker.sessions" :key="idx" class="px-2 py-0.5 bg-teal-100 text-teal-700 text-xs rounded-full">
                        {{ session }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Documents Tab -->
              <div v-else-if="activeTab === 'documents'" class="space-y-3">
                <div
                  v-for="doc in documents"
                  :key="doc.id"
                  class="flex items-center justify-between p-4 bg-gray-50 rounded-xl hover:bg-gray-100 transition-colors"
                >
                  <div class="flex items-center gap-4">
                    <div class="w-12 h-12 rounded-lg bg-white border border-gray-200 flex items-center justify-center">
                      <i :class="getFileIcon(doc.type)" class="text-xl"></i>
                    </div>
                    <div>
                      <p class="font-medium text-gray-900">{{ doc.name }}</p>
                      <p class="text-sm text-gray-500">{{ formatFileSize(doc.size) }}</p>
                    </div>
                  </div>
                  <button class="px-4 py-2 text-teal-600 hover:bg-teal-50 rounded-lg transition-colors">
                    <i class="fas fa-download mr-1"></i>Download
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Comments Section -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
            <CommentsSection content-type="event" :content-id="event.id" />
          </div>
        </div>

        <!-- Right Column - Sidebar -->
        <div class="space-y-6">
          <!-- Event Info Card -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
            <h3 class="font-semibold text-gray-900 mb-4">Event Details</h3>
            <div class="space-y-4">
              <div class="flex items-start gap-3">
                <div class="w-10 h-10 rounded-lg bg-teal-100 flex items-center justify-center">
                  <i class="fas fa-calendar text-teal-600"></i>
                </div>
                <div>
                  <p class="text-sm text-gray-500">Date</p>
                  <p class="font-medium text-gray-900">{{ formattedDate }}</p>
                </div>
              </div>
              <div class="flex items-start gap-3">
                <div class="w-10 h-10 rounded-lg bg-blue-100 flex items-center justify-center">
                  <i class="fas fa-clock text-blue-600"></i>
                </div>
                <div>
                  <p class="text-sm text-gray-500">Time</p>
                  <p class="font-medium text-gray-900">{{ formattedTime }}</p>
                </div>
              </div>
              <div class="flex items-start gap-3">
                <div class="w-10 h-10 rounded-lg bg-purple-100 flex items-center justify-center">
                  <i class="fas fa-map-marker-alt text-purple-600"></i>
                </div>
                <div>
                  <p class="text-sm text-gray-500">Location</p>
                  <p class="font-medium text-gray-900">{{ event.location }}</p>
                </div>
              </div>
              <div v-if="event.virtualLink" class="flex items-start gap-3">
                <div class="w-10 h-10 rounded-lg bg-green-100 flex items-center justify-center">
                  <i class="fas fa-video text-green-600"></i>
                </div>
                <div>
                  <p class="text-sm text-gray-500">Virtual Link</p>
                  <a :href="event.virtualLink" target="_blank" class="font-medium text-teal-600 hover:underline">Join Online</a>
                </div>
              </div>
            </div>
          </div>

          <!-- Organizer Card -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
            <h3 class="font-semibold text-gray-900 mb-4">Organizer</h3>
            <div class="flex items-center gap-3">
              <div :class="['w-12 h-12 rounded-full flex items-center justify-center text-white font-medium bg-gradient-to-br', getInitialsColor(event.organizer.initials)]">
                {{ event.organizer.initials }}
              </div>
              <div>
                <p class="font-medium text-gray-900">{{ event.organizer.name }}</p>
                <p class="text-sm text-gray-500">{{ event.organizer.role }}</p>
              </div>
            </div>
            <a :href="'mailto:' + event.organizer.email" class="mt-4 block w-full py-2 text-center text-teal-600 bg-teal-50 rounded-lg hover:bg-teal-100 transition-colors">
              <i class="fas fa-envelope mr-2"></i>Contact Organizer
            </a>
          </div>

          <!-- Attendees Card -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
            <div class="flex items-center justify-between mb-4">
              <h3 class="font-semibold text-gray-900">Attendees</h3>
              <div class="flex gap-2 text-xs">
                <span class="px-2 py-1 bg-green-100 text-green-700 rounded-full">{{ goingCount }} going</span>
                <span class="px-2 py-1 bg-yellow-100 text-yellow-700 rounded-full">{{ maybeCount }} maybe</span>
              </div>
            </div>

            <div class="flex flex-wrap gap-2 mb-4">
              <div
                v-for="attendee in displayedAttendees"
                :key="attendee.id"
                :title="attendee.name"
                :class="['w-10 h-10 rounded-full flex items-center justify-center text-white text-sm font-medium cursor-pointer hover:scale-110 transition-transform bg-gradient-to-br', getInitialsColor(attendee.initials)]"
              >
                {{ attendee.initials }}
              </div>
              <button
                v-if="goingCount > 6 && !showAllAttendees"
                @click="showAllAttendees = true"
                class="w-10 h-10 rounded-full bg-gray-100 text-gray-600 text-sm font-medium hover:bg-gray-200 transition-colors"
              >
                +{{ goingCount - 6 }}
              </button>
            </div>

            <button class="w-full py-2 text-center text-gray-600 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors text-sm">
              <i class="fas fa-user-plus mr-2"></i>Invite Others
            </button>
          </div>

          <!-- Tags -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
            <h3 class="font-semibold text-gray-900 mb-3">Tags</h3>
            <div class="flex flex-wrap gap-2">
              <span
                v-for="tag in event.tags"
                :key="tag"
                class="px-3 py-1 bg-gray-100 text-gray-700 rounded-full text-sm hover:bg-gray-200 cursor-pointer transition-colors"
              >
                {{ tag }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Related Events -->
      <div class="mt-8">
        <RelatedContentCarousel
          content-type="event"
          :content-id="event.id"
          title="Related Events"
          :limit="4"
        />
      </div>
    </template>

    <!-- Add to Calendar Modal -->
    <Teleport to="body">
      <div v-if="showCalendarModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-sm overflow-hidden">
          <div class="p-6 border-b border-gray-100">
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-semibold text-gray-900">Add to Calendar</h3>
              <button @click="showCalendarModal = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                <i class="fas fa-times text-gray-400"></i>
              </button>
            </div>
          </div>
          <div class="p-6 space-y-3">
            <button
              @click="addToCalendar('google')"
              class="w-full flex items-center gap-3 p-4 rounded-xl border border-gray-200 hover:bg-gray-50 transition-colors"
            >
              <i class="fab fa-google text-2xl text-red-500"></i>
              <span class="font-medium text-gray-900">Google Calendar</span>
            </button>
            <button
              @click="addToCalendar('outlook')"
              class="w-full flex items-center gap-3 p-4 rounded-xl border border-gray-200 hover:bg-gray-50 transition-colors"
            >
              <i class="fab fa-microsoft text-2xl text-blue-500"></i>
              <span class="font-medium text-gray-900">Outlook Calendar</span>
            </button>
            <button
              @click="addToCalendar('ical')"
              class="w-full flex items-center gap-3 p-4 rounded-xl border border-gray-200 hover:bg-gray-50 transition-colors"
            >
              <i class="fas fa-calendar-alt text-2xl text-gray-500"></i>
              <span class="font-medium text-gray-900">Download .ics File</span>
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
.prose ul {
  list-style-type: disc;
  padding-left: 1.5rem;
}

.prose p {
  margin-bottom: 1rem;
}
</style>
