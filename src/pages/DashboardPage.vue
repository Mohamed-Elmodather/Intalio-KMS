<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAIServicesStore } from '@/stores/aiServices'
import { AILoadingIndicator, AISuggestionChip, AIConfidenceBar } from '@/components/ai'

const router = useRouter()
const { t } = useI18n()
const aiStore = useAIServicesStore()

// Current user data (would come from auth store in production)
const currentUser = ref({
  id: 0,
  name: 'Ahmed',
  fullName: 'Ahmed Imam',
  role: 'Media Director',
  avatar: '',
  initials: 'AI'
})

// Dynamic counts (would come from API in production)
const pendingTasksCount = ref(3)
const newUpdatesCount = ref(5)

// Time of day greeting
const timeOfDay = computed(() => {
  const hour = new Date().getHours()
  if (hour < 12) return 'morning'
  if (hour < 17) return 'afternoon'
  return 'evening'
})

// Stats data
const stats = ref([
  { label: 'Total Articles', value: '2,847', numValue: 2847, displayValue: 0, icon: 'fas fa-newspaper', iconBg: 'bg-primary-100', iconColor: 'text-primary-600', trend: '12%', trendUp: true, link: '/articles', linkText: 'View Articles' },
  { label: 'Active Users', value: '1,234', numValue: 1234, displayValue: 0, icon: 'fas fa-users', iconBg: 'bg-blue-100', iconColor: 'text-blue-600', trend: '8%', trendUp: true, link: '/collaboration', linkText: 'View Users' },
  { label: 'Documents', value: '8,492', numValue: 8492, displayValue: 0, icon: 'fas fa-file-alt', iconBg: 'bg-violet-100', iconColor: 'text-violet-600', trend: '23%', trendUp: true, link: '/documents', linkText: 'View Documents' },
  { label: 'Courses Completed', value: '456', numValue: 456, displayValue: 0, icon: 'fas fa-graduation-cap', iconBg: 'bg-amber-100', iconColor: 'text-amber-600', trend: '5%', trendUp: false, link: '/learning', linkText: 'View Courses' },
])

// Format number with commas
const formatNumber = (num: number) => {
  return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}

// Animated counter function
const animateCounter = (stat: any, duration = 2000) => {
  const target = stat.numValue
  const startTime = performance.now()
  const startValue = 0

  const easeOutQuart = (t: number) => 1 - Math.pow(1 - t, 4)

  const updateCounter = (currentTime: number) => {
    const elapsed = currentTime - startTime
    const progress = Math.min(elapsed / duration, 1)
    const easedProgress = easeOutQuart(progress)

    stat.displayValue = Math.floor(startValue + (target - startValue) * easedProgress)

    if (progress < 1) {
      requestAnimationFrame(updateCounter)
    } else {
      stat.displayValue = target
    }
  }

  requestAnimationFrame(updateCounter)
}

// Recent Articles
const recentArticles = ref([
  {
    id: 1,
    title: 'Saudi Arabia Squad Announcement for AFC Asian Cup 2027',
    excerpt: 'Head coach announces the final 26-man squad for the home tournament.',
    category: 'Team News',
    categoryClass: 'bg-green-100 text-green-700',
    readTime: '5 min read',
    icon: 'fas fa-users',
    iconBg: 'bg-gradient-to-br from-green-100 to-green-200',
    iconColor: 'text-green-600',
    image: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=100&h=100&fit=crop',
    author: { name: 'AFC Media', initials: 'AF', color: '#006847' },
    date: '2 hours ago',
    views: '12.5K'
  },
  {
    id: 2,
    title: 'King Fahd Stadium: A Complete Guide for Fans',
    excerpt: 'Everything you need to know about the iconic venue hosting the final.',
    category: 'Venues',
    categoryClass: 'bg-blue-100 text-blue-700',
    readTime: '8 min read',
    icon: 'fas fa-stadium',
    iconBg: 'bg-gradient-to-br from-blue-100 to-blue-200',
    iconColor: 'text-blue-600',
    image: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=100&h=100&fit=crop',
    author: { name: 'Venue Team', initials: 'VT', color: '#2563eb' },
    date: '5 hours ago',
    views: '8.2K'
  },
  {
    id: 3,
    title: 'AFC Asian Cup 2027: Complete Match Schedule Released',
    excerpt: 'Full fixture list with dates, times, and venues for all 51 matches.',
    category: 'Schedule',
    categoryClass: 'bg-amber-100 text-amber-700',
    readTime: '4 min read',
    icon: 'fas fa-calendar-days',
    iconBg: 'bg-gradient-to-br from-amber-100 to-amber-200',
    iconColor: 'text-amber-600',
    image: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=100&h=100&fit=crop',
    author: { name: 'LOC', initials: 'LO', color: '#d97706' },
    date: 'Yesterday',
    views: '15.8K'
  },
  {
    id: 4,
    title: 'Top 10 Players to Watch at AFC Asian Cup 2027',
    excerpt: 'Star players from across Asia ready to shine on the biggest stage.',
    category: 'Analysis',
    categoryClass: 'bg-purple-100 text-purple-700',
    readTime: '10 min read',
    icon: 'fas fa-star',
    iconBg: 'bg-gradient-to-br from-purple-100 to-purple-200',
    iconColor: 'text-purple-600',
    image: 'https://images.unsplash.com/photo-1551958219-acbc608c6377?w=100&h=100&fit=crop',
    author: { name: 'Sports Desk', initials: 'SD', color: '#7c3aed' },
    date: '2 days ago',
    views: '22.1K'
  }
])

// Upcoming Events
const upcomingEvents = ref([
  {
    id: 1,
    title: 'Saudi Arabia vs Japan - Opening Match',
    month: 'Jan',
    day: '10',
    year: '2027',
    time: '8:00 PM',
    location: 'King Fahd Stadium, Riyadh',
    category: 'Match',
    categoryIcon: 'fas fa-futbol',
    categoryColor: 'bg-green-100 text-green-700',
    gradientClass: 'from-green-500 to-emerald-600',
    isRegistered: true,
    isFeatured: true,
    attendees: [
      { name: 'Ahmed', initials: 'AH', color: '#14B8A6' },
      { name: 'Mohammed', initials: 'MO', color: '#3B82F6' },
      { name: 'Sara', initials: 'SA', color: '#8B5CF6' },
      { name: 'Khalid', initials: 'KH', color: '#F59E0B' },
      { name: 'Fatima', initials: 'FA', color: '#EF4444' }
    ],
    totalAttendees: 68000
  },
  {
    id: 2,
    title: 'Opening Ceremony Rehearsal',
    month: 'Jan',
    day: '09',
    year: '2027',
    time: '6:00 PM - 10:00 PM',
    location: 'King Fahd Stadium, Riyadh',
    category: 'Ceremony',
    categoryIcon: 'fas fa-star',
    categoryColor: 'bg-amber-100 text-amber-700',
    gradientClass: 'from-amber-500 to-orange-600',
    isRegistered: false,
    isFeatured: false,
    attendees: [
      { name: 'Event Team', initials: 'ET', color: '#10B981' },
      { name: 'LOC', initials: 'LO', color: '#6366F1' },
      { name: 'Media', initials: 'ME', color: '#EC4899' }
    ],
    totalAttendees: 5000
  },
  {
    id: 3,
    title: 'Team Captains Press Conference',
    month: 'Jan',
    day: '08',
    year: '2027',
    time: '2:00 PM - 4:00 PM',
    location: 'Media Center, Riyadh',
    category: 'Media',
    categoryIcon: 'fas fa-microphone',
    categoryColor: 'bg-blue-100 text-blue-700',
    gradientClass: 'from-blue-500 to-indigo-600',
    isRegistered: true,
    isFeatured: false,
    attendees: [
      { name: 'Press', initials: 'PR', color: '#F59E0B' },
      { name: 'Media', initials: 'MD', color: '#14B8A6' }
    ],
    totalAttendees: 250
  },
  {
    id: 4,
    title: 'Fan Zone Opening Event',
    month: 'Jan',
    day: '10',
    year: '2027',
    time: '12:00 PM',
    location: 'Boulevard Riyadh City',
    category: 'Fan Event',
    categoryIcon: 'fas fa-users',
    categoryColor: 'bg-purple-100 text-purple-700',
    gradientClass: 'from-purple-500 to-violet-600',
    isRegistered: false,
    isFeatured: false,
    attendees: [
      { name: 'Fans', initials: 'FN', color: '#EC4899' },
      { name: 'Staff', initials: 'ST', color: '#8B5CF6' }
    ],
    totalAttendees: 15000
  }
])

// Event actions
const registeredEvents = ref<Set<number>>(new Set([1, 3]))

function toggleEventRegistration(eventId: number, event: Event) {
  event.stopPropagation()
  if (registeredEvents.value.has(eventId)) {
    registeredEvents.value.delete(eventId)
  } else {
    registeredEvents.value.add(eventId)
  }
}

function isEventRegistered(eventId: number): boolean {
  return registeredEvents.value.has(eventId)
}

function setEventReminder(eventId: number, event: Event) {
  event.stopPropagation()
  alert(`Reminder set for event #${eventId}`)
}

function shareEvent(eventId: number, event: Event) {
  event.stopPropagation()
  alert(`Share event #${eventId}`)
}

// Active Polls
interface PollOption {
  label: string
  votes: number
  color: string
  flag?: string
}

interface Poll {
  id: number
  question: string
  icon: string
  iconBg: string
  options: PollOption[]
  totalVotes: number
  endsIn: string
  hasVoted: boolean
}

const activePolls = ref<Poll[]>([
  {
    id: 1,
    question: 'Who will win AFC Asian Cup 2027?',
    icon: 'fas fa-trophy',
    iconBg: 'bg-gradient-to-br from-yellow-400 to-amber-500',
    options: [
      { label: 'Saudi Arabia', votes: 38, color: '#006847', flag: '🇸🇦' },
      { label: 'Japan', votes: 28, color: '#BC002D', flag: '🇯🇵' },
      { label: 'South Korea', votes: 18, color: '#003478', flag: '🇰🇷' },
      { label: 'Iran', votes: 16, color: '#239F40', flag: '🇮🇷' }
    ],
    totalVotes: 12450,
    endsIn: 'Ends when tournament starts',
    hasVoted: false
  },
  {
    id: 2,
    question: 'Best stadium for the Final?',
    icon: 'fas fa-futbol',
    iconBg: 'bg-gradient-to-br from-teal-400 to-teal-600',
    options: [
      { label: 'King Fahd Stadium', votes: 45, color: '#14b8a6' },
      { label: 'King Abdullah Sports City', votes: 32, color: '#0d9488' },
      { label: 'Prince Abdullah Stadium', votes: 23, color: '#0f766e' }
    ],
    totalVotes: 8234,
    endsIn: 'Ends in 5 days',
    hasVoted: true
  }
])

// Poll actions
const selectedPollOptions = ref<Record<number, string>>({})

function selectPollOption(pollId: number, optionLabel: string) {
  selectedPollOptions.value[pollId] = optionLabel
}

function isOptionSelected(pollId: number, optionLabel: string): boolean {
  return selectedPollOptions.value[pollId] === optionLabel
}

function votePoll(poll: any) {
  const selectedOption = selectedPollOptions.value[poll.id]
  if (!poll.hasVoted && selectedOption) {
    // Submit vote
    poll.hasVoted = true
    alert(t('dashboard.youVotedFor', { option: selectedOption }))
  } else if (poll.hasVoted) {
    // View results - navigate to poll page
    router.push(`/polls/${poll.id}`)
  } else {
    alert(t('dashboard.selectOptionFirst'))
  }
}

function viewPoll(pollId: number) {
  router.push(`/polls/${pollId}`)
}

// Learning Courses
const learningCourses = ref([
  {
    id: 1,
    title: 'Stadium Operations Management',
    instructor: 'Dr. Ahmed Al-Rashid',
    instructorAvatar: 'AR',
    instructorColor: '#14b8a6',
    progress: 75,
    lessonsCompleted: 12,
    totalLessons: 16,
    duration: '8h 30m',
    category: 'Operations',
    difficulty: 'Advanced',
    icon: 'fas fa-stadium',
    gradientClass: 'from-teal-500 to-emerald-600',
    image: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=100&h=60&fit=crop',
    isBookmarked: true,
    lastAccessed: '2 hours ago',
    certificateAvailable: true
  },
  {
    id: 2,
    title: 'Event Security Protocols',
    instructor: 'Col. Khalid Hassan',
    instructorAvatar: 'KH',
    instructorColor: '#3b82f6',
    progress: 45,
    lessonsCompleted: 5,
    totalLessons: 11,
    duration: '6h 15m',
    category: 'Security',
    difficulty: 'Intermediate',
    icon: 'fas fa-shield-alt',
    gradientClass: 'from-blue-500 to-indigo-600',
    image: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=100&h=60&fit=crop',
    isBookmarked: false,
    lastAccessed: 'Yesterday',
    certificateAvailable: true
  },
  {
    id: 3,
    title: 'Media & Broadcasting Excellence',
    instructor: 'Sarah Mitchell',
    instructorAvatar: 'SM',
    instructorColor: '#8b5cf6',
    progress: 90,
    lessonsCompleted: 9,
    totalLessons: 10,
    duration: '5h 45m',
    category: 'Media',
    difficulty: 'Advanced',
    icon: 'fas fa-broadcast-tower',
    gradientClass: 'from-violet-500 to-purple-600',
    image: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=100&h=60&fit=crop',
    isBookmarked: true,
    lastAccessed: '3 days ago',
    certificateAvailable: false
  },
  {
    id: 4,
    title: 'Fan Experience Design',
    instructor: 'Omar Abdullah',
    instructorAvatar: 'OA',
    instructorColor: '#f59e0b',
    progress: 30,
    lessonsCompleted: 3,
    totalLessons: 10,
    duration: '4h 20m',
    category: 'Experience',
    difficulty: 'Beginner',
    icon: 'fas fa-users',
    gradientClass: 'from-amber-500 to-orange-600',
    image: 'https://images.unsplash.com/photo-1489944440615-453fc2b6a9a9?w=100&h=60&fit=crop',
    isBookmarked: false,
    lastAccessed: '1 week ago',
    certificateAvailable: true
  }
])

// Learning stats
const learningStats = computed(() => ({
  inProgress: learningCourses.value.filter(c => c.progress > 0 && c.progress < 100).length,
  completed: learningCourses.value.filter(c => c.progress === 100).length,
  totalHours: learningCourses.value.reduce((acc, c) => acc + parseFloat(c.duration), 0).toFixed(0),
  certificates: learningCourses.value.filter(c => c.progress === 100 && c.certificateAvailable).length
}))

// Learning actions
const bookmarkedCourses = ref<Set<number>>(new Set([1, 3]))

function toggleCourseBookmark(courseId: number, event: Event) {
  event.stopPropagation()
  if (bookmarkedCourses.value.has(courseId)) {
    bookmarkedCourses.value.delete(courseId)
  } else {
    bookmarkedCourses.value.add(courseId)
  }
}

function isCourseBookmarked(courseId: number): boolean {
  return bookmarkedCourses.value.has(courseId)
}

function continueCourse(courseId: number, event: Event) {
  event.stopPropagation()
  router.push(`/learning/${courseId}`)
}

function shareCourse(courseId: number, event: Event) {
  event.stopPropagation()
  alert(`Share course #${courseId}`)
}

// Media Items
const mediaItems = ref([
  {
    id: 1,
    title: 'Saudi Arabia vs Japan Preview',
    type: 'video',
    category: 'Match Preview',
    duration: '15:42',
    views: '245K',
    date: '2 hours ago',
    progress: 65,
    image: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400&h=225&fit=crop',
    gradientClass: 'bg-gradient-to-br from-green-600 to-green-800'
  },
  {
    id: 2,
    title: 'Opening Ceremony Highlights',
    type: 'video',
    category: 'Highlights',
    duration: '28:15',
    views: '189K',
    date: '5 hours ago',
    progress: 0,
    image: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=400&h=225&fit=crop',
    gradientClass: 'bg-gradient-to-br from-teal-400 to-teal-600'
  },
  {
    id: 3,
    title: 'AFC Asian Cup Podcast Ep. 12',
    type: 'audio',
    category: 'Podcast',
    duration: '45:00',
    views: '56K',
    date: 'Yesterday',
    progress: 30,
    gradientClass: 'bg-gradient-to-br from-violet-400 to-violet-600'
  },
  {
    id: 4,
    title: 'Stadium Tour: King Fahd',
    type: 'video',
    category: 'Behind the Scenes',
    duration: '12:30',
    views: '98K',
    date: '2 days ago',
    progress: 100,
    image: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=400&h=225&fit=crop',
    gradientClass: 'bg-gradient-to-br from-amber-400 to-amber-600'
  },
  {
    id: 5,
    title: 'Top Goals - Group Stage',
    type: 'video',
    category: 'Highlights',
    duration: '8:45',
    views: '312K',
    date: '3 days ago',
    progress: 0,
    image: 'https://images.unsplash.com/photo-1551958219-acbc608c6377?w=400&h=225&fit=crop',
    gradientClass: 'bg-gradient-to-br from-rose-500 to-pink-600'
  },
  {
    id: 6,
    title: 'Fan Zone Experience',
    type: 'video',
    category: 'Fan Content',
    duration: '6:20',
    views: '45K',
    date: '4 days ago',
    progress: 0,
    image: 'https://images.unsplash.com/photo-1489944440615-453fc2b6a9a9?w=400&h=225&fit=crop',
    gradientClass: 'bg-gradient-to-br from-blue-500 to-indigo-600'
  }
])

// Media actions
const savedMedia = ref<Set<number>>(new Set())

function playMedia(mediaId: number) {
  router.push(`/media/${mediaId}`)
}

function saveMedia(mediaId: number, event: Event) {
  event.stopPropagation()
  if (savedMedia.value.has(mediaId)) {
    savedMedia.value.delete(mediaId)
  } else {
    savedMedia.value.add(mediaId)
  }
}

function isMediaSaved(mediaId: number): boolean {
  return savedMedia.value.has(mediaId)
}

// Recent Documents
const recentDocuments = ref([
  {
    id: 1,
    name: 'AFC Asian Cup 2027 Schedule.pdf',
    size: '4.2 MB',
    modified: '2 hours ago',
    icon: 'fas fa-file-pdf',
    iconBg: 'bg-rose-100',
    iconColor: 'text-rose-600',
    type: 'PDF'
  },
  {
    id: 2,
    name: 'Stadium Operations Manual.docx',
    size: '12.5 MB',
    modified: '5 hours ago',
    icon: 'fas fa-file-word',
    iconBg: 'bg-blue-100',
    iconColor: 'text-blue-600',
    type: 'Word'
  },
  {
    id: 3,
    name: 'Team Statistics 2027.xlsx',
    size: '3.8 MB',
    modified: 'Yesterday',
    icon: 'fas fa-file-excel',
    iconBg: 'bg-emerald-100',
    iconColor: 'text-emerald-600',
    type: 'Excel'
  },
  {
    id: 4,
    name: 'Opening Ceremony Plan.pptx',
    size: '28.4 MB',
    modified: '2 days ago',
    icon: 'fas fa-file-powerpoint',
    iconBg: 'bg-orange-100',
    iconColor: 'text-orange-600',
    type: 'PowerPoint'
  }
])

// Bookmarked articles state
const bookmarkedArticles = ref<Set<number>>(new Set())

// Article actions
function goToArticle(articleId: number) {
  router.push(`/articles/${articleId}`)
}

function bookmarkArticle(articleId: number, event: Event) {
  event.stopPropagation()
  if (bookmarkedArticles.value.has(articleId)) {
    bookmarkedArticles.value.delete(articleId)
  } else {
    bookmarkedArticles.value.add(articleId)
  }
}

function isBookmarked(articleId: number): boolean {
  return bookmarkedArticles.value.has(articleId)
}

// Document modal state
const showUploadModal = ref(false)

function openUploadDocument() {
  // Navigate to documents page with upload modal open
  router.push({ path: '/documents', query: { action: 'upload' } })
}

// Document actions
function previewDocument(doc: any, event: Event) {
  event.stopPropagation()
  // Navigate to document view page with preview mode
  router.push({ path: `/documents/${doc.id}`, query: { preview: 'true' } })
}

function downloadDocument(doc: any, event: Event) {
  event.stopPropagation()
  // Simulate file download
  const mimeTypes: Record<string, string> = {
    'PDF': 'application/pdf',
    'Word': 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    'Excel': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    'PowerPoint': 'application/vnd.openxmlformats-officedocument.presentationml.presentation'
  }

  // Create a placeholder blob for demo purposes
  const content = `This is a placeholder for: ${doc.name}\n\nIn production, this would download the actual file from the server.`
  const blob = new Blob([content], { type: mimeTypes[doc.type] || 'application/octet-stream' })
  const url = URL.createObjectURL(blob)

  // Create download link and trigger click
  const link = document.createElement('a')
  link.href = url
  link.download = doc.name
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  URL.revokeObjectURL(url)
}

function openDocument(docId: number) {
  router.push(`/documents/${docId}`)
}

// Team Activities
const teamActivities = ref([
  {
    id: 1,
    user: { id: 101, name: 'Ahmed Al-Farsi', initials: 'AF', color: '#14b8a6', role: 'Media Director' },
    action: 'published article',
    target: 'Opening Match Preview: Saudi Arabia vs Japan',
    targetId: 1,
    targetType: 'article',
    time: '5 minutes ago',
    actionIcon: 'fas fa-newspaper',
    actionBg: 'bg-teal-100',
    actionColor: 'text-teal-600',
    likes: 24,
    comments: 8,
    isLiked: false
  },
  {
    id: 2,
    user: { id: 102, name: 'Fatima Hassan', initials: 'FH', color: '#3b82f6', role: 'Operations Lead' },
    action: 'uploaded document',
    target: 'Stadium Security Protocol v2.0',
    targetId: 2,
    targetType: 'document',
    time: '15 minutes ago',
    actionIcon: 'fas fa-file-alt',
    actionBg: 'bg-blue-100',
    actionColor: 'text-blue-600',
    likes: 12,
    comments: 3,
    isLiked: true
  },
  {
    id: 3,
    user: { id: 103, name: 'Mohammed Khalid', initials: 'MK', color: '#10b981', role: 'Training Manager' },
    action: 'completed course',
    target: 'Stadium Operations Management',
    targetId: 1,
    targetType: 'course',
    time: '1 hour ago',
    actionIcon: 'fas fa-graduation-cap',
    actionBg: 'bg-emerald-100',
    actionColor: 'text-emerald-600',
    likes: 18,
    comments: 5,
    isLiked: false
  },
  {
    id: 4,
    user: { id: 104, name: 'Sara Al-Rashid', initials: 'SR', color: '#f59e0b', role: 'Event Coordinator' },
    action: 'scheduled event',
    target: 'Opening Ceremony Rehearsal',
    targetId: 1,
    targetType: 'event',
    time: '2 hours ago',
    actionIcon: 'fas fa-calendar-check',
    actionBg: 'bg-amber-100',
    actionColor: 'text-amber-600',
    likes: 32,
    comments: 12,
    isLiked: true
  },
  {
    id: 5,
    user: { id: 105, name: 'Khalid Omar', initials: 'KO', color: '#8b5cf6', role: 'Volunteer Lead' },
    action: 'added media',
    target: 'Fan Zone Setup Photos',
    targetId: 3,
    targetType: 'media',
    time: '3 hours ago',
    actionIcon: 'fas fa-images',
    actionBg: 'bg-violet-100',
    actionColor: 'text-violet-600',
    likes: 45,
    comments: 15,
    isLiked: false
  },
  {
    id: 6,
    user: { id: 106, name: 'Layla Ahmed', initials: 'LA', color: '#ec4899', role: 'Communications' },
    action: 'created poll',
    target: 'Best Stadium for the Final?',
    targetId: 2,
    targetType: 'poll',
    time: '4 hours ago',
    actionIcon: 'fas fa-chart-pie',
    actionBg: 'bg-pink-100',
    actionColor: 'text-pink-600',
    likes: 67,
    comments: 28,
    isLiked: true
  }
])

// Activity actions
const likedActivities = ref<Set<number>>(new Set([2, 4, 6]))

function toggleActivityLike(activityId: number, event: Event) {
  event.stopPropagation()
  const activity = teamActivities.value.find(a => a.id === activityId)
  if (activity) {
    if (likedActivities.value.has(activityId)) {
      likedActivities.value.delete(activityId)
      activity.likes--
    } else {
      likedActivities.value.add(activityId)
      activity.likes++
    }
  }
}

function isActivityLiked(activityId: number): boolean {
  return likedActivities.value.has(activityId)
}

function commentOnActivity(activityId: number, event: Event) {
  event.stopPropagation()
  alert(`Comment on activity #${activityId}`)
}

// Team Activity actions
function viewUserProfile(userId: number) {
  router.push({ name: 'Profile', query: { userId: userId.toString() } })
}

function viewActivityTarget(activity: any) {
  const routes: Record<string, string> = {
    article: '/articles',
    document: '/documents',
    course: '/learning',
    event: '/events',
    poll: '/polls',
    media: '/media'
  }
  const basePath = routes[activity.targetType] || '/'
  router.push(`${basePath}/${activity.targetId}`)
}

// Event actions
function viewEvent(eventId: number) {
  router.push(`/events/${eventId}`)
}

// Learning actions
function viewCourse(courseId: number) {
  router.push(`/learning/${courseId}`)
}

// Self Service actions
function openService(service: any) {
  router.push(service.route || '/self-services')
}

// Self Services
const selfServices = ref([
  {
    id: 1,
    label: 'Ticket Request',
    description: 'Match tickets & hospitality',
    icon: 'fas fa-ticket-alt',
    gradientClass: 'from-teal-500 to-emerald-600',
    route: '/self-services/tickets',
    usageCount: 1247,
    isPopular: true,
    isNew: false
  },
  {
    id: 2,
    label: 'Accreditation',
    description: 'Staff & media access',
    icon: 'fas fa-id-badge',
    gradientClass: 'from-blue-500 to-indigo-600',
    route: '/self-services/accreditation',
    usageCount: 856,
    isPopular: true,
    isNew: false
  },
  {
    id: 3,
    label: 'Transportation',
    description: 'Shuttle & vehicle booking',
    icon: 'fas fa-bus',
    gradientClass: 'from-violet-500 to-purple-600',
    route: '/self-services/transport',
    usageCount: 623,
    isPopular: false,
    isNew: false
  },
  {
    id: 4,
    label: 'Accommodation',
    description: 'Hotel reservations',
    icon: 'fas fa-hotel',
    gradientClass: 'from-amber-500 to-orange-600',
    route: '/self-services/accommodation',
    usageCount: 412,
    isPopular: false,
    isNew: false
  },
  {
    id: 5,
    label: 'IT Support',
    description: 'Technical assistance',
    icon: 'fas fa-headset',
    gradientClass: 'from-rose-500 to-pink-600',
    route: '/self-services/it-support',
    usageCount: 934,
    isPopular: false,
    isNew: false
  },
  {
    id: 6,
    label: 'Volunteer Hub',
    description: 'Volunteer registration',
    icon: 'fas fa-hands-helping',
    gradientClass: 'from-green-500 to-teal-600',
    route: '/self-services/volunteers',
    usageCount: 2103,
    isPopular: true,
    isNew: true
  }
])

// Carousel State
const currentSlide = ref(0)
const isAutoPlaying = ref(true)
let carouselInterval: number | null = null

// Computed property for current featured update
const featuredUpdate = computed(() => recentUpdates.value[currentSlide.value])

// Computed property for up next items (shows all items)
const upNextUpdates = computed(() => recentUpdates.value)

// Check if an item is currently featured
const isCurrentlyFeatured = (id: number) => featuredUpdate.value?.id === id

const recentUpdates = ref([
  {
    id: 1,
    type: 'announcement',
    typeLabel: 'Breaking News',
    typeIcon: 'fas fa-bullhorn',
    title: 'Saudi Arabia vs Japan: Opening Match Set for January 10 at King Fahd Stadium',
    description: 'The host nation Saudi Arabia will kick off AFC Asian Cup 2027 against Japan in a blockbuster opening match. Over 68,000 fans expected at King Fahd International Stadium in Riyadh for this historic encounter.',
    icon: 'fas fa-futbol',
    image: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=600&h=400&fit=crop',
    gradientClass: 'bg-gradient-to-br from-amber-400 to-orange-500',
    date: 'Jan 5, 2027',
    timeAgo: '2 hours ago',
    readTime: '3 min read',
    author: { name: 'AFC Media', initials: 'AF', role: 'Official News', color: '#006847' },
    views: '245,847',
    likes: '12,543',
    comments: '4,521',
    shares: '3,201',
    link: '/articles/saudi-arabia-vs-japan-opening-match',
    actionText: 'View Match Details',
    tags: ['Opening Match', 'Saudi Arabia', 'Japan'],
    isFeatured: true,
    isTrending: true
  },
  {
    id: 2,
    type: 'article',
    typeLabel: 'Player Focus',
    typeIcon: 'fas fa-newspaper',
    title: 'Salem Al-Dawsari: Leading the Green Falcons to Glory on Home Soil',
    description: 'The Saudi captain and Al-Hilal star is ready to lead his nation in the biggest tournament on home ground. With 20 international goals, Al-Dawsari aims to inspire the next generation of Saudi football.',
    icon: 'fas fa-user-star',
    image: 'https://images.unsplash.com/photo-1551958219-acbc608c6377?w=600&h=400&fit=crop',
    gradientClass: 'bg-gradient-to-br from-blue-400 to-indigo-500',
    date: 'Jan 4, 2027',
    timeAgo: '1 day ago',
    readTime: '5 min read',
    author: { name: 'Sports Desk', initials: 'SD', role: 'Football Analysis', color: '#006847' },
    views: '189,562',
    likes: '8,721',
    comments: '2,856',
    shares: '1,892',
    link: '/articles/salem-al-dawsari-profile',
    actionText: 'Read Full Story',
    tags: ['Player Profile', 'Al-Dawsari', 'Captain'],
    isFeatured: false,
    isTrending: true
  },
  {
    id: 3,
    type: 'event',
    typeLabel: 'Tournament Update',
    typeIcon: 'fas fa-trophy',
    title: 'AFC Asian Cup 2027 Opening Ceremony - A Saudi Vision Unveiled',
    description: 'Experience the grandeur of Saudi hospitality at the opening ceremony featuring world-class performances, stunning visuals celebrating Saudi culture, and a spectacular drone show over Riyadh skyline.',
    icon: 'fas fa-star',
    image: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=600&h=400&fit=crop',
    gradientClass: 'bg-gradient-to-br from-emerald-400 to-teal-500',
    date: 'Jan 9, 2027',
    timeAgo: '3 days ago',
    readTime: '4 min read',
    author: { name: 'LOC Events', initials: 'LO', role: 'Local Organizing Committee', color: '#10b981' },
    views: '167,103',
    likes: '9,432',
    comments: '1,892',
    shares: '2,156',
    link: '/events/1',
    actionText: 'Event Details',
    tags: ['Opening Ceremony', 'Entertainment', 'Culture'],
    isFeatured: true,
    isTrending: false
  },
  {
    id: 4,
    type: 'venue',
    typeLabel: 'Venues',
    typeIcon: 'fas fa-stadium',
    title: 'World-Class Stadiums Ready: From Riyadh to Jeddah',
    description: 'Saudi Arabia\'s state-of-the-art stadiums are ready to host Asia\'s finest. King Fahd Stadium, King Abdullah Sports City, and Prince Abdullah Al-Faisal Stadium offer world-class facilities for 24 competing nations.',
    icon: 'fas fa-building',
    image: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=600&h=400&fit=crop',
    gradientClass: 'bg-gradient-to-br from-purple-400 to-violet-500',
    date: 'Jan 3, 2027',
    timeAgo: '4 days ago',
    readTime: '6 min read',
    author: { name: 'Venue Operations', initials: 'VO', role: 'Stadium Management', color: '#8b5cf6' },
    views: '98,543',
    likes: '5,123',
    comments: '734',
    shares: '892',
    link: '/articles/world-class-stadiums',
    actionText: 'Explore Venues',
    tags: ['Stadiums', 'Infrastructure', 'Facilities'],
    isFeatured: false,
    isTrending: false
  }
])

// Update interactions
const savedUpdates = ref(new Set<number>())
const likedUpdates = ref(new Set<number>())

function toggleSaveUpdate(id: number) {
  if (savedUpdates.value.has(id)) {
    savedUpdates.value.delete(id)
  } else {
    savedUpdates.value.add(id)
  }
}

function toggleLikeUpdate(id: number) {
  if (likedUpdates.value.has(id)) {
    likedUpdates.value.delete(id)
  } else {
    likedUpdates.value.add(id)
  }
}

function isUpdateSaved(id: number): boolean {
  return savedUpdates.value.has(id)
}

function isUpdateLiked(id: number): boolean {
  return likedUpdates.value.has(id)
}

function shareUpdate(update: any) {
  console.log('Sharing update:', update.title)
}

// Carousel Methods
const nextSlide = () => {
  currentSlide.value = (currentSlide.value + 1) % recentUpdates.value.length
}

const prevSlide = () => {
  currentSlide.value = currentSlide.value === 0
    ? recentUpdates.value.length - 1
    : currentSlide.value - 1
}

const goToSlide = (index: number) => {
  currentSlide.value = index
}

const goToSlideById = (id: number) => {
  const index = recentUpdates.value.findIndex(item => item.id === id)
  if (index !== -1) {
    currentSlide.value = index
  }
}

const startAutoPlay = () => {
  if (carouselInterval) clearInterval(carouselInterval)
  carouselInterval = window.setInterval(() => {
    if (isAutoPlaying.value) {
      nextSlide()
    }
  }, 5000)
}

const pauseCarousel = () => {
  isAutoPlaying.value = false
}

const resumeCarousel = () => {
  isAutoPlaying.value = true
}

const toggleAutoPlay = () => {
  isAutoPlaying.value = !isAutoPlaying.value
}

// ============================================================================
// AI Features - State & Functions
// ============================================================================

// AI State
const showAIInsightsPanel = ref(false)
const showRecommendationsModal = ref(false)
const showAnomalyAlert = ref(true)
const isLoadingInsights = ref(false)
const isLoadingRecommendations = ref(false)

// AI Interfaces
interface AIInsight {
  id: string
  type: 'trend' | 'alert' | 'opportunity' | 'prediction'
  title: string
  description: string
  metric?: string
  change?: number
  confidence: number
  action?: string
  actionLabel?: string
}

interface ContentRecommendation {
  id: string
  type: 'article' | 'document' | 'course' | 'event'
  title: string
  reason: string
  relevanceScore: number
  icon: string
  iconBg: string
}

interface ActivityAnomaly {
  id: string
  type: 'spike' | 'drop' | 'unusual'
  metric: string
  description: string
  severity: 'low' | 'medium' | 'high'
  detectedAt: string
  value: string
  expectedValue: string
}

// AI Data
const aiInsights = ref<AIInsight[]>([
  {
    id: '1',
    type: 'trend',
    title: 'Article Engagement Surge',
    description: 'Article views increased by 34% this week, driven by tournament schedule content.',
    metric: 'Article Views',
    change: 34,
    confidence: 0.92,
    action: '/articles',
    actionLabel: 'View Articles'
  },
  {
    id: '2',
    type: 'prediction',
    title: 'Expected Traffic Spike',
    description: 'AI predicts 45% increase in platform traffic during the opening match week.',
    metric: 'User Sessions',
    change: 45,
    confidence: 0.87,
    action: '/analytics',
    actionLabel: 'View Analytics'
  },
  {
    id: '3',
    type: 'opportunity',
    title: 'Content Gap Detected',
    description: 'Users are searching for "ticket booking guide" but no dedicated content exists.',
    confidence: 0.89,
    action: '/articles/new',
    actionLabel: 'Create Article'
  },
  {
    id: '4',
    type: 'alert',
    title: 'Document Downloads Declining',
    description: 'Policy document downloads dropped 18% - consider updating or promoting.',
    metric: 'Downloads',
    change: -18,
    confidence: 0.85,
    action: '/documents',
    actionLabel: 'Review Documents'
  }
])

const contentRecommendations = ref<ContentRecommendation[]>([
  {
    id: '1',
    type: 'article',
    title: 'Team Registration Deadline Reminder',
    reason: 'High relevance based on your recent activity in team management',
    relevanceScore: 0.95,
    icon: 'fas fa-file-alt',
    iconBg: 'bg-blue-100'
  },
  {
    id: '2',
    type: 'course',
    title: 'Tournament Operations Masterclass',
    reason: 'Recommended based on your role and upcoming responsibilities',
    relevanceScore: 0.91,
    icon: 'fas fa-graduation-cap',
    iconBg: 'bg-green-100'
  },
  {
    id: '3',
    type: 'document',
    title: 'Venue Safety Protocol 2027',
    reason: 'Recently updated - important for stadium operations team',
    relevanceScore: 0.88,
    icon: 'fas fa-file-pdf',
    iconBg: 'bg-red-100'
  },
  {
    id: '4',
    type: 'event',
    title: 'Media Briefing - Pre-Tournament',
    reason: 'Based on your media department affiliation',
    relevanceScore: 0.84,
    icon: 'fas fa-calendar',
    iconBg: 'bg-amber-100'
  },
  {
    id: '5',
    type: 'article',
    title: 'Broadcasting Rights Guidelines',
    reason: 'Frequently accessed by users with similar roles',
    relevanceScore: 0.82,
    icon: 'fas fa-file-alt',
    iconBg: 'bg-blue-100'
  }
])

const activityAnomalies = ref<ActivityAnomaly[]>([
  {
    id: '1',
    type: 'spike',
    metric: 'Document Downloads',
    description: 'Unusual spike in Venue Guidelines downloads detected',
    severity: 'medium',
    detectedAt: '2 hours ago',
    value: '847',
    expectedValue: '~250'
  },
  {
    id: '2',
    type: 'drop',
    metric: 'Course Completions',
    description: 'Course completion rate dropped below expected threshold',
    severity: 'low',
    detectedAt: '1 day ago',
    value: '12%',
    expectedValue: '~25%'
  }
])

// Quick AI insights for hero section
const quickAIInsight = computed(() => {
  return {
    text: 'Based on current trends, platform engagement is 23% above average this week.',
    trend: 'positive'
  }
})

// AI Functions
async function loadAIInsights() {
  isLoadingInsights.value = true
  showAIInsightsPanel.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 800))
    // Insights are already loaded from mock data
  } finally {
    isLoadingInsights.value = false
  }
}

async function loadRecommendations() {
  isLoadingRecommendations.value = true
  showRecommendationsModal.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 600))
    // Recommendations are already loaded from mock data
  } finally {
    isLoadingRecommendations.value = false
  }
}

function dismissAnomaly(id: string) {
  activityAnomalies.value = activityAnomalies.value.filter(a => a.id !== id)
  if (activityAnomalies.value.length === 0) {
    showAnomalyAlert.value = false
  }
}

function getInsightTypeIcon(type: string): string {
  switch (type) {
    case 'trend': return 'fas fa-chart-line'
    case 'prediction': return 'fas fa-crystal-ball'
    case 'opportunity': return 'fas fa-lightbulb'
    case 'alert': return 'fas fa-exclamation-triangle'
    default: return 'fas fa-info-circle'
  }
}

function getInsightTypeColor(type: string): string {
  switch (type) {
    case 'trend': return 'bg-blue-100 text-blue-600'
    case 'prediction': return 'bg-purple-100 text-purple-600'
    case 'opportunity': return 'bg-green-100 text-green-600'
    case 'alert': return 'bg-amber-100 text-amber-600'
    default: return 'bg-gray-100 text-gray-600'
  }
}

function getAnomalySeverityColor(severity: string): string {
  switch (severity) {
    case 'high': return 'bg-red-100 text-red-700 border-red-200'
    case 'medium': return 'bg-amber-100 text-amber-700 border-amber-200'
    case 'low': return 'bg-blue-100 text-blue-700 border-blue-200'
    default: return 'bg-gray-100 text-gray-700 border-gray-200'
  }
}

function navigateToInsight(insight: AIInsight) {
  if (insight.action) {
    router.push(insight.action)
  }
}

function navigateToRecommendation(rec: ContentRecommendation) {
  const routes: Record<string, string> = {
    article: '/articles',
    document: '/documents',
    course: '/learning',
    event: '/events'
  }
  router.push(routes[rec.type] || '/')
}

// Lifecycle
onMounted(() => {
  // Start counter animation
  setTimeout(() => {
    stats.value.forEach((stat, index) => {
      setTimeout(() => {
        animateCounter(stat, 1500)
      }, index * 200)
    })
  }, 500)

  // Start carousel
  startAutoPlay()
})

onUnmounted(() => {
  if (carouselInterval) clearInterval(carouselInterval)
})
</script>

<template>
  <div class="px-4 sm:px-6 lg:px-8 xl:px-10 py-4 sm:py-6">
    <!-- Welcome Section - Enhanced -->
    <div class="welcome-hero rounded-xl lg:rounded-2xl mb-5 lg:mb-8 relative overflow-hidden stagger-1">
      <!-- Decorative elements with drift animations -->
      <div class="absolute top-0 end-0 w-96 h-96 bg-white/5 rounded-full circle-drift-1"></div>
      <div class="absolute bottom-0 start-0 w-64 h-64 bg-white/5 rounded-full circle-drift-2"></div>
      <div class="absolute top-1/2 end-1/4 w-32 h-32 bg-white/10 rounded-full circle-drift-3"></div>

      <!-- Trophy Icon - Top Right with glow animation -->
      <div class="absolute top-4 end-4 lg:top-6 lg:end-6 w-14 h-14 sm:w-16 sm:h-16 lg:w-20 lg:h-20 rounded-xl lg:rounded-2xl bg-white/20 backdrop-blur-sm flex items-center justify-center trophy-glow">
        <i class="fas fa-trophy text-white text-xl sm:text-2xl lg:text-3xl"></i>
      </div>

      <div class="relative p-4 sm:p-6 lg:p-8">
        <div class="flex items-center gap-3 mb-4">
          <div class="px-3 py-1 bg-white/20 backdrop-blur-sm rounded-full text-white text-xs font-semibold flex items-center gap-2">
            <span class="w-2 h-2 bg-green-400 rounded-full animate-pulse"></span>
            {{ $t('dashboard.afcAsianCup') }}
          </div>
        </div>

        <h1 class="text-xl sm:text-2xl lg:text-3xl font-bold text-white mb-2 lg:mb-3 fade-in-up" style="animation-delay: 0.3s;">
          {{ $t(`dashboard.greeting.${timeOfDay}`, { name: currentUser.name }) }} <span class="text-lg sm:text-xl lg:text-2xl inline-block icon-float">👋</span>
        </h1>
        <p class="text-teal-100 max-w-lg fade-in-up text-sm lg:text-base" style="animation-delay: 0.4s;">
          {{ $t('dashboard.welcomeMessage') }}
          <span class="font-semibold text-white">{{ $t('dashboard.pendingTasks', { count: pendingTasksCount }) }}</span> {{ $t('common.and') }}
          <span class="font-semibold text-white">{{ $t('dashboard.newUpdates', { count: newUpdatesCount }) }}</span>.
        </p>

        <!-- Quick Action Buttons -->
        <div class="flex flex-wrap gap-2 lg:gap-3 mt-4 lg:mt-6 fade-in-up" style="animation-delay: 0.5s;">
          <router-link to="/learning" class="px-3 py-2 lg:px-5 lg:py-2.5 bg-white text-teal-600 rounded-lg lg:rounded-xl font-semibold text-xs lg:text-sm flex items-center gap-1.5 lg:gap-2 hover:bg-teal-50 transition-all shadow-lg shadow-black/10">
            <i class="fas fa-play text-xs"></i>
            {{ $t('learning.continueLearning') }}
          </router-link>
          <router-link to="/articles/new" class="px-3 py-2 lg:px-5 lg:py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-lg lg:rounded-xl font-semibold text-xs lg:text-sm hover:bg-white/30 transition-all flex items-center gap-1.5 lg:gap-2">
            <i class="fas fa-plus text-xs"></i>
            {{ $t('dashboard.createContent') }}
          </router-link>
          <router-link to="/events" class="px-3 py-2 lg:px-5 lg:py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-lg lg:rounded-xl font-semibold text-xs lg:text-sm hover:bg-white/30 transition-all flex items-center gap-1.5 lg:gap-2">
            <i class="fas fa-calendar text-xs"></i>
            {{ $t('dashboard.viewEvents') }}
          </router-link>
          <!-- AI Buttons -->
          <button @click="loadAIInsights" class="px-3 py-2 lg:px-5 lg:py-2.5 bg-gradient-to-r from-emerald-400 to-teal-400 text-white rounded-lg lg:rounded-xl font-semibold text-xs lg:text-sm hover:from-emerald-500 hover:to-teal-500 transition-all flex items-center gap-1.5 lg:gap-2 shadow-lg shadow-teal-500/20">
            <i class="fas fa-wand-magic-sparkles text-xs"></i>
            {{ $t('learning.aiInsights') }}
          </button>
          <button @click="loadRecommendations" class="px-3 py-2 lg:px-5 lg:py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-lg lg:rounded-xl font-semibold text-xs lg:text-sm hover:bg-white/30 transition-all flex items-center gap-1.5 lg:gap-2">
            <i class="fas fa-compass text-xs"></i>
            {{ $t('dashboard.forYou') }}
          </button>
        </div>

        <!-- AI Quick Insight Badge -->
        <div class="mt-3 sm:mt-4 fade-in-up" style="animation-delay: 0.6s;">
          <div class="inline-flex items-center gap-1.5 sm:gap-2 px-3 sm:px-4 py-1.5 sm:py-2 bg-white/10 backdrop-blur-sm rounded-lg sm:rounded-xl border border-white/20 max-w-full">
            <i class="fas fa-robot text-emerald-300 text-sm sm:text-base flex-shrink-0"></i>
            <span class="text-xs sm:text-sm text-white/90 truncate">{{ quickAIInsight.text }}</span>
            <span class="hidden sm:flex items-center gap-1 text-xs text-emerald-300 font-semibold flex-shrink-0">
              <i class="fas fa-arrow-up"></i> {{ $t('dashboard.trending') }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- AI Anomaly Alerts -->
    <div v-if="showAnomalyAlert && activityAnomalies.length > 0" class="mb-6">
      <div class="bg-gradient-to-r from-amber-50 to-orange-50 border border-amber-200 rounded-2xl p-4">
        <div class="flex items-center justify-between mb-3">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-400 to-orange-500 flex items-center justify-center">
              <i class="fas fa-exclamation-triangle text-white"></i>
            </div>
            <div>
              <h3 class="font-semibold text-gray-900">{{ $t('dashboard.aiDetectedAnomalies') }}</h3>
              <p class="text-xs text-gray-500">{{ $t('dashboard.anomalyPatternsDetected', { count: activityAnomalies.length }) }}</p>
            </div>
          </div>
          <button @click="showAnomalyAlert = false" class="p-2 hover:bg-amber-100 rounded-lg transition-colors">
            <i class="fas fa-times text-gray-400"></i>
          </button>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-3">
          <div v-for="anomaly in activityAnomalies" :key="anomaly.id"
               :class="['p-3 rounded-xl border flex items-start gap-3', getAnomalySeverityColor(anomaly.severity)]">
            <div class="w-8 h-8 rounded-lg bg-white/50 flex items-center justify-center flex-shrink-0">
              <i :class="[anomaly.type === 'spike' ? 'fas fa-arrow-trend-up' : anomaly.type === 'drop' ? 'fas fa-arrow-trend-down' : 'fas fa-question']"></i>
            </div>
            <div class="flex-1 min-w-0">
              <p class="font-medium text-sm">{{ anomaly.metric }}</p>
              <p class="text-xs opacity-80">{{ anomaly.description }}</p>
              <div class="flex items-center gap-3 mt-2 text-xs">
                <span>{{ $t('dashboard.actual') }}: <strong>{{ anomaly.value }}</strong></span>
                <span>{{ $t('dashboard.expected') }}: {{ anomaly.expectedValue }}</span>
                <span class="opacity-60">{{ anomaly.detectedAt }}</span>
              </div>
            </div>
            <button @click="dismissAnomaly(anomaly.id)" class="p-1 hover:bg-white/50 rounded transition-colors">
              <i class="fas fa-times text-xs"></i>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Latest Updates Section -->
    <section class="updates-section mb-5 lg:mb-8">
      <div class="updates-header">
        <h2 class="text-base sm:text-lg font-bold text-gray-900 flex items-center gap-2 sm:gap-3">
          <div class="w-9 h-9 sm:w-11 sm:h-11 rounded-lg sm:rounded-xl bg-gradient-to-br from-orange-500 to-orange-600 flex items-center justify-center shadow-lg shadow-orange-200">
            <i class="fas fa-fire text-white text-sm sm:text-base"></i>
          </div>
          <div>
            <span class="block">{{ $t('dashboard.latestUpdates') }}</span>
            <span class="text-[10px] sm:text-xs font-medium text-orange-600">{{ $t('dashboard.afcAsianCupShort') }}</span>
          </div>
        </h2>
        <router-link to="/articles" class="px-2 py-1 sm:px-3 sm:py-1.5 text-xs sm:text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1 sm:gap-1.5 transition-all">
          {{ $t('common.viewAll') }} <i class="fas fa-arrow-right text-[10px] sm:text-xs"></i>
        </router-link>
      </div>

      <!-- Updates Grid: Featured + Up Next -->
      <div v-if="featuredUpdate" class="updates-grid">
        <!-- Featured Article Card with Carousel -->
        <div class="updates-featured" @mouseenter="pauseCarousel" @mouseleave="resumeCarousel">
          <router-link :to="featuredUpdate.link" class="featured-main-card group">
            <transition name="carousel-fade" mode="out-in">
              <img :key="featuredUpdate.id" :src="featuredUpdate.image" :alt="featuredUpdate.title" class="featured-main-img" />
            </transition>
            <div class="featured-main-overlay"></div>
            <!-- Badges -->
            <div class="featured-main-badges">
              <span class="badge-featured"><i class="fas fa-star"></i> {{ $t('dashboard.featured') }}</span>
              <span class="badge-category">{{ featuredUpdate.typeLabel }}</span>
            </div>
            <!-- Hover Actions -->
            <div class="featured-hover-actions">
              <router-link :to="featuredUpdate.link" class="featured-hover-btn primary">
                <i class="fas fa-book-open"></i>
                <span>{{ $t('dashboard.readArticle') }}</span>
              </router-link>
              <div class="featured-hover-icons">
                <button class="hover-icon-btn" @click.stop="toggleLikeUpdate(featuredUpdate.id)" :class="{ active: isUpdateLiked(featuredUpdate.id) }">
                  <i :class="isUpdateLiked(featuredUpdate.id) ? 'fas fa-heart' : 'far fa-heart'"></i>
                </button>
                <button class="hover-icon-btn" @click.stop="toggleSaveUpdate(featuredUpdate.id)" :class="{ active: isUpdateSaved(featuredUpdate.id) }">
                  <i :class="isUpdateSaved(featuredUpdate.id) ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                </button>
                <button class="hover-icon-btn" @click.stop="shareUpdate(featuredUpdate)">
                  <i class="fas fa-share-alt"></i>
                </button>
              </div>
            </div>
            <!-- Content -->
            <div class="featured-main-content">
              <div class="featured-main-meta">
                <span><i class="fas fa-eye"></i> {{ featuredUpdate.views }}</span>
                <span><i class="fas fa-clock"></i> {{ featuredUpdate.readTime }}</span>
                <span><i class="fas fa-heart"></i> {{ featuredUpdate.likes }}</span>
              </div>
              <h3 class="featured-main-title">{{ featuredUpdate.title }}</h3>
              <p class="featured-main-desc">{{ featuredUpdate.description }}</p>
              <div class="featured-main-author">
                <div class="author-avatar-sm" :style="{ backgroundColor: featuredUpdate.author.color }">{{ featuredUpdate.author.initials }}</div>
                <span>{{ featuredUpdate.author.name }}</span>
                <span class="dot">•</span>
                <span>{{ featuredUpdate.timeAgo }}</span>
              </div>
            </div>
            <!-- Carousel Navigation Arrows -->
            <button class="carousel-arrow carousel-prev" @click.prevent="prevSlide">
              <i class="fas fa-chevron-left"></i>
            </button>
            <button class="carousel-arrow carousel-next" @click.prevent="nextSlide">
              <i class="fas fa-chevron-right"></i>
            </button>
            <!-- Carousel Dots -->
            <div class="carousel-dots">
              <button
                v-for="(update, index) in recentUpdates.slice(0, 5)"
                :key="update.id"
                :class="['carousel-dot', { active: index === currentSlide }]"
                @click.prevent="goToSlide(index)"
              ></button>
            </div>
          </router-link>
        </div>

        <!-- Up Next Column -->
        <div class="up-next-column">
          <h3 class="up-next-title"><i class="fas fa-list"></i> {{ $t('dashboard.upNext') }}</h3>
          <div class="up-next-list">
            <div
              v-for="update in upNextUpdates"
              :key="update.id"
              :class="['secondary-card group', { 'is-active': isCurrentlyFeatured(update.id) }]"
              @click="goToSlideById(update.id)"
            >
              <div class="secondary-link">
                <div class="secondary-image">
                  <img :src="update.image" :alt="update.title">
                  <div class="secondary-overlay"></div>
                  <span :class="['secondary-badge', update.type]">
                    <i :class="update.typeIcon"></i>
                    {{ update.typeLabel }}
                  </span>
                  <span v-if="update.isTrending" class="secondary-trending">
                    <i class="fas fa-fire-alt"></i>
                  </span>
                  <!-- Hover Actions -->
                  <div class="secondary-hover-actions">
                    <router-link :to="update.link" class="secondary-action-btn view" @click.stop>
                      <i class="fas fa-external-link-alt"></i>
                    </router-link>
                    <button class="secondary-action-btn" @click.stop="toggleLikeUpdate(update.id)" :class="{ active: isUpdateLiked(update.id) }">
                      <i :class="isUpdateLiked(update.id) ? 'fas fa-heart' : 'far fa-heart'"></i>
                    </button>
                    <button class="secondary-action-btn" @click.stop="toggleSaveUpdate(update.id)" :class="{ active: isUpdateSaved(update.id) }">
                      <i :class="isUpdateSaved(update.id) ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                    </button>
                  </div>
                </div>
                <div class="secondary-content">
                  <div class="secondary-meta">
                    <span class="secondary-time">{{ update.timeAgo }}</span>
                    <span class="secondary-read">{{ update.readTime }}</span>
                  </div>
                  <h4 class="secondary-title">{{ update.title }}</h4>
                  <p class="secondary-description">{{ update.description }}</p>
                  <div class="secondary-footer">
                    <div class="secondary-author">
                      <div class="secondary-avatar" :style="{ backgroundColor: update.author.color }">
                        {{ update.author.initials }}
                      </div>
                      <span>{{ update.author.name }}</span>
                    </div>
                    <div class="secondary-stats">
                      <span><i class="fas fa-eye"></i> {{ update.views }}</span>
                      <span><i class="fas fa-heart"></i> {{ update.likes }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Stats Grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-4 lg:gap-5 mb-6 lg:mb-8">
      <div v-for="(stat, index) in stats" :key="stat.label"
           class="stat-card card-animated rounded-xl sm:rounded-2xl p-4 sm:p-5"
           :style="{ animationDelay: index * 100 + 'ms' }">
        <div class="flex items-center justify-between mb-4">
          <div :class="['w-11 h-11 rounded-xl flex items-center justify-center', stat.iconBg]">
            <i :class="[stat.icon, stat.iconColor]"></i>
          </div>
          <div class="flex items-center gap-1 text-xs" :class="stat.trendUp ? 'text-emerald-600' : 'text-rose-600'">
            <i :class="stat.trendUp ? 'fas fa-arrow-up' : 'fas fa-arrow-down'"></i>
            {{ stat.trend }}
          </div>
        </div>
        <div class="stat-value text-2xl sm:text-3xl font-bold text-gray-900">{{ formatNumber(stat.displayValue) }}</div>
        <div class="text-xs sm:text-sm text-gray-500 mt-1">{{ stat.label }}</div>
        <router-link :to="stat.link" class="text-xs text-primary-600 font-medium mt-3 flex items-center gap-1 hover:text-primary-700">
          {{ stat.linkText }} <i class="fas fa-arrow-right text-[10px]"></i>
        </router-link>
      </div>
    </div>

    <!-- Main Grid Row 1 - Articles & Documents -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 lg:gap-6 mb-6 lg:mb-8">
      <!-- Recent Articles -->
      <div class="card-animated rounded-xl lg:rounded-2xl p-4 lg:p-6 stagger-2 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
        <div class="flex items-center justify-between mb-4 sm:mb-5">
          <h2 class="text-base sm:text-lg font-bold text-gray-900 flex items-center gap-2 sm:gap-3">
            <div class="w-9 h-9 sm:w-11 sm:h-11 rounded-lg sm:rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-newspaper text-white text-sm sm:text-base"></i>
            </div>
            <div>
              <span class="block">{{ $t('dashboard.recentArticles') }}</span>
              <span class="text-[10px] sm:text-xs font-medium text-teal-600">{{ $t('dashboard.latestNewsUpdates') }}</span>
            </div>
          </h2>
          <router-link to="/articles" class="px-2 py-1 sm:px-3 sm:py-1.5 text-xs sm:text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1 sm:gap-1.5 transition-all">
            {{ $t('common.viewAll') }} <i class="fas fa-arrow-right text-[10px] sm:text-xs"></i>
          </router-link>
        </div>
        <div class="space-y-2.5">
          <div v-for="(article, index) in recentArticles" :key="article.id"
               @click="goToArticle(article.id)"
               class="dashboard-article-row flex flex-row items-center gap-3 p-3 rounded-xl bg-white/80 hover:bg-white border border-gray-100 hover:border-teal-200 hover:shadow-lg cursor-pointer transition-all group">
            <!-- Article Image -->
            <div class="relative flex-shrink-0">
              <div class="w-12 h-12 rounded-xl overflow-hidden transition-all group-hover:scale-110 group-hover:shadow-md">
                <img v-if="article.image" :src="article.image" :alt="article.title" class="w-full h-full object-cover">
                <div v-else :class="[article.iconBg, 'w-full h-full flex items-center justify-center']">
                  <i :class="[article.icon, 'text-base', article.iconColor]"></i>
                </div>
              </div>
              <div v-if="index === 0" class="absolute -top-1 -end-1 w-4 h-4 bg-red-500 rounded-full flex items-center justify-center">
                <span class="text-white text-[8px] font-bold">{{ $t('dashboard.new') }}</span>
              </div>
            </div>
            <!-- Article Content -->
            <div class="flex-1 min-w-0">
              <h4 class="font-semibold text-gray-900 text-sm truncate group-hover:text-teal-600 transition-colors">
                {{ article.title }}
              </h4>
              <div class="flex items-center gap-2 mt-1">
                <span :class="['text-[10px] font-semibold px-1.5 py-0.5 rounded-full', article.categoryClass]">
                  {{ article.category }}
                </span>
                <span class="text-[10px] text-gray-400">{{ article.readTime }}</span>
                <span class="text-[10px] text-gray-400"><i class="fas fa-eye me-0.5"></i>{{ article.views }}</span>
              </div>
            </div>
            <!-- Actions -->
            <div class="flex items-center gap-1.5 opacity-0 group-hover:opacity-100 transition-opacity">
              <button @click.stop="goToArticle(article.id)" class="w-8 h-8 rounded-lg bg-teal-50 hover:bg-teal-100 flex items-center justify-center text-teal-500 hover:text-teal-600 transition-all" title="Read">
                <i class="fas fa-book-open text-xs"></i>
              </button>
              <button @click="bookmarkArticle(article.id, $event)" :class="['w-8 h-8 rounded-lg flex items-center justify-center transition-all', isBookmarked(article.id) ? 'bg-teal-500 text-white hover:bg-teal-600' : 'bg-teal-50 hover:bg-teal-100 text-teal-500 hover:text-teal-600']" title="Bookmark">
                <i :class="[isBookmarked(article.id) ? 'fas' : 'far', 'fa-bookmark text-xs']"></i>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Documents -->
      <div class="card-animated rounded-xl lg:rounded-2xl p-4 lg:p-6 stagger-3 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
        <div class="flex items-center justify-between mb-4 sm:mb-5">
          <h2 class="text-base sm:text-lg font-bold text-gray-900 flex items-center gap-2 sm:gap-3">
            <div class="w-9 h-9 sm:w-11 sm:h-11 rounded-lg sm:rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-folder-open text-white text-sm sm:text-base"></i>
            </div>
            <div>
              <span class="block">{{ $t('dashboard.recentDocuments') }}</span>
              <span class="text-[10px] sm:text-xs font-medium text-teal-600">{{ $t('dashboard.filesResources') }}</span>
            </div>
          </h2>
          <router-link to="/documents" class="px-2 py-1 sm:px-3 sm:py-1.5 text-xs sm:text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1 sm:gap-1.5 transition-all">
            {{ $t('common.viewAll') }} <i class="fas fa-arrow-right text-[10px] sm:text-xs"></i>
          </router-link>
        </div>
        <div class="space-y-2.5">
          <div v-for="(doc, index) in recentDocuments.slice(0, 4)" :key="doc.id"
               @click="openDocument(doc.id)"
               class="document-card flex items-center gap-3 p-3 rounded-xl bg-white/80 hover:bg-white border border-gray-100 hover:border-teal-200 hover:shadow-lg cursor-pointer transition-all group">
            <!-- Document Icon -->
            <div class="relative">
              <div :class="['w-12 h-12 rounded-xl flex items-center justify-center transition-all group-hover:scale-110 group-hover:shadow-md', doc.iconBg]">
                <i :class="[doc.icon, doc.iconColor, 'text-xl']"></i>
              </div>
              <div v-if="index === 0" class="absolute -top-1 -end-1 w-4 h-4 bg-teal-500 rounded-full flex items-center justify-center">
                <i class="fas fa-arrow-up text-white text-[8px]"></i>
              </div>
            </div>
            <!-- Document Info -->
            <div class="flex-1 min-w-0">
              <h4 class="font-semibold text-gray-900 text-sm truncate group-hover:text-teal-600 transition-colors">{{ doc.name }}</h4>
              <div class="flex items-center gap-2 mt-1">
                <span class="text-[10px] px-1.5 py-0.5 rounded bg-gray-100 text-gray-600 font-medium">{{ doc.type }}</span>
                <span class="text-[10px] text-gray-400">{{ doc.size }}</span>
                <span class="text-[10px] text-gray-300">•</span>
                <span class="text-[10px] text-gray-400">{{ doc.modified }}</span>
              </div>
            </div>
            <!-- Actions -->
            <div class="flex items-center gap-1.5 opacity-0 group-hover:opacity-100 transition-opacity">
              <button @click.stop="previewDocument(doc, $event)" class="w-8 h-8 rounded-lg bg-teal-50 hover:bg-teal-100 flex items-center justify-center text-teal-500 hover:text-teal-600 transition-all" title="Preview">
                <i class="fas fa-eye text-xs"></i>
              </button>
              <button @click.stop="downloadDocument(doc, $event)" class="w-8 h-8 rounded-lg bg-teal-50 hover:bg-teal-100 flex items-center justify-center text-teal-500 hover:text-teal-600 transition-all" title="Download">
                <i class="fas fa-download text-xs"></i>
              </button>
            </div>
          </div>
        </div>
        <!-- Upload Button -->
        <button @click="openUploadDocument" class="w-full mt-4 py-3 text-sm font-semibold text-teal-600 bg-teal-50 hover:bg-teal-100 border-2 border-dashed border-teal-200 hover:border-teal-300 rounded-xl transition-all flex items-center justify-center gap-2">
          <i class="fas fa-cloud-upload-alt"></i>
          {{ $t('dashboard.uploadNewDocument') }}
        </button>
      </div>
    </div>

    <!-- Recent Media - Enhanced Section -->
    <div class="card-animated rounded-xl lg:rounded-2xl p-4 lg:p-5 mb-5 lg:mb-6 stagger-2 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
      <div class="flex items-center justify-between mb-3 sm:mb-4">
        <h2 class="text-base sm:text-lg font-bold text-gray-900 flex items-center gap-2 sm:gap-3">
          <div class="w-9 h-9 sm:w-10 sm:h-10 rounded-lg sm:rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
            <i class="fas fa-photo-film text-white text-xs sm:text-sm"></i>
          </div>
          <div>
            <span class="block">{{ $t('dashboard.recentMedia') }}</span>
            <span class="text-[10px] sm:text-xs font-medium text-teal-600">{{ $t('dashboard.videosPodcasts') }}</span>
          </div>
        </h2>
        <router-link to="/media" class="px-2 py-1 sm:px-3 sm:py-1.5 text-xs sm:text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1 sm:gap-1.5 transition-all">
          {{ $t('common.viewAll') }} <i class="fas fa-arrow-right text-[10px] sm:text-xs"></i>
        </router-link>
      </div>
      <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-6 gap-2 sm:gap-3">
        <div v-for="media in mediaItems" :key="media.id"
             @click="playMedia(media.id)"
             class="media-card cursor-pointer group rounded-xl bg-white border border-gray-100 hover:border-teal-200 hover:shadow-lg transition-all overflow-hidden">
          <!-- Thumbnail -->
          <div class="relative aspect-video">
            <img v-if="media.image" :src="media.image" :alt="media.title" class="absolute inset-0 w-full h-full object-cover transition-transform duration-300 group-hover:scale-105">
            <div v-else class="absolute inset-0" :class="media.gradientClass"></div>
            <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent"></div>

            <!-- Play Button -->
            <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
              <div class="w-10 h-10 rounded-full bg-white/95 flex items-center justify-center shadow-lg transform scale-75 group-hover:scale-100 transition-transform">
                <i :class="media.type === 'audio' ? 'fas fa-headphones text-violet-600' : 'fas fa-play text-teal-600 ms-0.5'" class="text-sm"></i>
              </div>
            </div>

            <!-- Duration Badge -->
            <div class="absolute bottom-1.5 end-1.5 px-1.5 py-0.5 rounded bg-black/80 text-white text-[10px] font-semibold backdrop-blur-sm">
              {{ media.duration }}
            </div>

            <!-- Type Badge -->
            <div v-if="media.type === 'audio'" class="absolute top-1.5 start-1.5 px-1.5 py-0.5 rounded-full bg-violet-500 text-white text-[9px] font-semibold flex items-center gap-1">
              <i class="fas fa-podcast"></i> {{ $t('dashboard.audio') }}
            </div>
            <div v-else-if="media.progress === 100" class="absolute top-1.5 start-1.5 px-1.5 py-0.5 rounded-full bg-green-500 text-white text-[9px] font-semibold flex items-center gap-1">
              <i class="fas fa-check"></i> {{ $t('dashboard.watched') }}
            </div>

            <!-- Progress Bar -->
            <div v-if="media.progress > 0 && media.progress < 100" class="absolute bottom-0 start-0 end-0 h-1 bg-black/30">
              <div class="h-full bg-teal-500" :style="{ width: media.progress + '%' }"></div>
            </div>

            <!-- Save Button -->
            <button @click="saveMedia(media.id, $event)"
                    :class="['absolute top-1.5 end-1.5 w-6 h-6 rounded-full flex items-center justify-center transition-all backdrop-blur-sm', isMediaSaved(media.id) ? 'bg-teal-500 opacity-100' : 'bg-black/50 hover:bg-teal-500 opacity-0 group-hover:opacity-100']">
              <i :class="[isMediaSaved(media.id) ? 'fas' : 'far', 'fa-bookmark text-white text-[10px]']"></i>
            </button>
          </div>

          <!-- Content -->
          <div class="p-2 sm:p-2.5">
            <div class="flex items-center gap-1.5 mb-0.5 sm:mb-1">
              <span class="text-[8px] sm:text-[9px] font-semibold px-1 sm:px-1.5 py-0.5 rounded-full bg-teal-100 text-teal-600">{{ media.category }}</span>
            </div>
            <h4 class="font-semibold text-gray-900 text-[10px] sm:text-xs leading-tight line-clamp-2 group-hover:text-teal-600 transition-colors mb-0.5 sm:mb-1">{{ media.title }}</h4>
            <div class="flex items-center gap-1 sm:gap-2 text-[9px] sm:text-[10px] text-gray-400">
              <span><i class="fas fa-eye me-0.5"></i>{{ media.views }}</span>
              <span class="hidden sm:inline">•</span>
              <span class="hidden sm:inline">{{ media.date }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Grid Row 2 -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 lg:gap-6 mb-6 lg:mb-8">
      <!-- Events - Enhanced -->
      <div class="card-animated rounded-xl lg:rounded-2xl p-4 lg:p-6 stagger-3 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
        <div class="flex items-center justify-between mb-4 sm:mb-5">
          <h2 class="text-base sm:text-lg font-bold text-gray-900 flex items-center gap-2 sm:gap-3">
            <div class="w-9 h-9 sm:w-10 sm:h-10 rounded-lg sm:rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-calendar-alt text-white text-xs sm:text-sm"></i>
            </div>
            <div>
              <span class="block">{{ $t('dashboard.upcomingEvents') }}</span>
              <span class="text-[10px] sm:text-xs font-medium text-teal-600">{{ $t('dashboard.afcAsianCupShort') }}</span>
            </div>
          </h2>
          <router-link to="/events" class="px-2 py-1 sm:px-3 sm:py-1.5 text-xs sm:text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1 sm:gap-1.5 transition-all">
            {{ $t('common.viewAll') }} <i class="fas fa-arrow-right text-[10px] sm:text-xs"></i>
          </router-link>
        </div>

        <div class="space-y-2 sm:space-y-3">
          <div v-for="event in upcomingEvents" :key="event.id"
               @click="viewEvent(event.id)"
               class="event-card relative rounded-xl cursor-pointer border border-gray-100 hover:border-teal-200 hover:shadow-lg transition-all group overflow-hidden"
               :class="event.isFeatured ? 'bg-gradient-to-r from-teal-50 to-emerald-50' : 'bg-white'">

            <!-- Featured Badge -->
            <div v-if="event.isFeatured" class="absolute top-0 end-0">
              <div class="bg-gradient-to-r from-amber-400 to-orange-500 text-white text-[9px] font-bold px-2 py-0.5 rounded-es-lg rtl:rounded-es-none rtl:rounded-ee-lg">
                <i class="fas fa-star me-0.5"></i> {{ $t('dashboard.featured').toUpperCase() }}
              </div>
            </div>

            <div class="p-3 flex gap-3 h-full">
              <!-- Date Box -->
              <div :class="['w-14 h-14 rounded-xl bg-gradient-to-br flex flex-col items-center justify-center flex-shrink-0 shadow-md transition-transform group-hover:scale-110', event.gradientClass]">
                <span class="text-[10px] font-semibold text-white/90 uppercase">{{ event.month }}</span>
                <span class="text-xl font-bold text-white -mt-0.5">{{ event.day }}</span>
              </div>

              <!-- Event Info -->
              <div class="flex-1 min-w-0 flex flex-col">
                <div class="flex items-start justify-between gap-2">
                  <div class="min-w-0">
                    <h4 class="font-semibold text-gray-900 text-sm truncate group-hover:text-teal-600 transition-colors">{{ event.title }}</h4>
                    <div class="flex items-center gap-2 mt-1">
                      <span :class="['text-[10px] font-semibold px-1.5 py-0.5 rounded-full flex items-center gap-1', event.categoryColor]">
                        <i :class="event.categoryIcon" class="text-[8px]"></i>
                        {{ event.category }}
                      </span>
                      <span v-if="isEventRegistered(event.id)" class="text-[10px] font-semibold px-1.5 py-0.5 rounded-full bg-green-100 text-green-700 flex items-center gap-1">
                        <i class="fas fa-check-circle text-[8px]"></i>
                        Registered
                      </span>
                    </div>
                  </div>

                  <!-- Actions moved to top right -->
                  <div class="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity flex-shrink-0">
                    <button @click="toggleEventRegistration(event.id, $event)"
                            :class="['w-7 h-7 rounded-lg flex items-center justify-center transition-all', isEventRegistered(event.id) ? 'bg-green-500 text-white hover:bg-green-600' : 'bg-teal-50 hover:bg-teal-100 text-teal-500 hover:text-teal-600']"
                            :title="isEventRegistered(event.id) ? 'Cancel Registration' : 'Register'">
                      <i :class="isEventRegistered(event.id) ? 'fas fa-check' : 'fas fa-plus'" class="text-[10px]"></i>
                    </button>
                    <button @click="setEventReminder(event.id, $event)"
                            class="w-7 h-7 rounded-lg bg-teal-50 hover:bg-teal-100 flex items-center justify-center text-teal-500 hover:text-teal-600 transition-all"
                            title="Set Reminder">
                      <i class="fas fa-bell text-[10px]"></i>
                    </button>
                    <button @click="shareEvent(event.id, $event)"
                            class="w-7 h-7 rounded-lg bg-teal-50 hover:bg-teal-100 flex items-center justify-center text-teal-500 hover:text-teal-600 transition-all"
                            title="Share">
                      <i class="fas fa-share-alt text-[10px]"></i>
                    </button>
                  </div>
                </div>

                <!-- Time & Location -->
                <div class="flex flex-col gap-0.5 mt-2 text-[11px] text-gray-500">
                  <div class="flex items-center gap-1.5">
                    <i class="fas fa-clock text-teal-500 w-3"></i>
                    <span>{{ event.time }}</span>
                  </div>
                  <div class="flex items-center gap-1.5">
                    <i class="fas fa-map-marker-alt text-teal-500 w-3"></i>
                    <span class="truncate">{{ event.location }}</span>
                  </div>
                </div>

                <!-- Footer with Attendees -->
                <div class="flex items-center mt-auto pt-2 border-t border-gray-100">
                  <div class="flex items-center gap-2">
                    <div class="flex -space-x-1.5">
                      <div v-for="(attendee, idx) in event.attendees.slice(0, 3)" :key="idx"
                           class="w-5 h-5 rounded-full border-2 border-white flex items-center justify-center text-[8px] font-bold text-white transition-transform hover:scale-110 hover:z-10"
                           :style="{ backgroundColor: attendee.color }"
                           :title="attendee.name">
                        {{ attendee.initials }}
                      </div>
                    </div>
                    <span class="text-[10px] text-gray-400">{{ event.totalAttendees >= 1000 ? (event.totalAttendees / 1000).toFixed(0) + 'K' : event.totalAttendees }} attending</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

      </div>

      <!-- Polls -->
      <div class="card-animated rounded-xl lg:rounded-2xl p-4 lg:p-6 stagger-4 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
        <div class="flex items-center justify-between mb-4 sm:mb-5">
          <h2 class="text-base sm:text-lg font-bold text-gray-900 flex items-center gap-2 sm:gap-3">
            <div class="w-9 h-9 sm:w-10 sm:h-10 rounded-lg sm:rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-chart-pie text-white text-xs sm:text-sm"></i>
            </div>
            <div>
              <span class="block">{{ $t('dashboard.activePolls') }}</span>
              <span class="text-[10px] sm:text-xs font-medium text-teal-600">{{ $t('dashboard.castYourVote') }}</span>
            </div>
          </h2>
          <router-link to="/polls" class="px-2 py-1 sm:px-3 sm:py-1.5 text-xs sm:text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1 sm:gap-1.5 transition-all">
            {{ $t('common.viewAll') }} <i class="fas fa-arrow-right text-[10px] sm:text-xs"></i>
          </router-link>
        </div>
        <div class="space-y-5">
          <div v-for="poll in activePolls" :key="poll.id" class="poll-card p-5 rounded-2xl bg-white border border-gray-100 shadow-sm hover:shadow-md transition-all">
            <!-- Poll Header -->
            <div @click="viewPoll(poll.id)" class="flex items-start gap-3 mb-4 cursor-pointer group">
              <div :class="[poll.iconBg, 'w-10 h-10 rounded-xl flex items-center justify-center shadow-md flex-shrink-0 transition-transform group-hover:scale-110']">
                <i :class="[poll.icon, 'text-white text-sm']"></i>
              </div>
              <div class="flex-1 min-w-0">
                <h4 class="font-semibold text-gray-900 text-sm leading-tight group-hover:text-teal-600 transition-colors">{{ poll.question }}</h4>
                <div class="flex items-center gap-2 mt-1">
                  <span class="text-xs text-gray-500">{{ poll.totalVotes.toLocaleString() }} {{ $t('dashboard.votes') }}</span>
                  <span class="w-1 h-1 rounded-full bg-gray-300"></span>
                  <span class="text-xs text-amber-600 font-medium"><i class="fas fa-clock me-1"></i>{{ poll.endsIn }}</span>
                </div>
              </div>
            </div>

            <!-- Poll Options -->
            <div class="space-y-3">
              <div v-for="(option, index) in poll.options" :key="option.label"
                   @click="!poll.hasVoted && selectPollOption(poll.id, option.label)"
                   :class="[
                     'poll-option group rounded-xl p-3 transition-all relative overflow-hidden',
                     poll.hasVoted ? 'cursor-default' : 'cursor-pointer hover:bg-gray-100',
                     isOptionSelected(poll.id, option.label) && !poll.hasVoted ? 'ring-2 ring-teal-500 bg-teal-50' : 'bg-gray-50'
                   ]">
                <!-- Progress Background -->
                <div class="absolute inset-0 opacity-20 transition-all"
                     :style="{ width: option.votes + '%', backgroundColor: option.color || '#14b8a6' }"></div>
                <div class="relative flex items-center justify-between">
                  <div class="flex items-center gap-2">
                    <!-- Selection indicator for unvoted polls -->
                    <div v-if="!poll.hasVoted" class="w-5 h-5 rounded-full border-2 flex items-center justify-center flex-shrink-0 transition-all"
                         :class="isOptionSelected(poll.id, option.label) ? 'border-teal-500 bg-teal-500' : 'border-gray-300'">
                      <i v-if="isOptionSelected(poll.id, option.label)" class="fas fa-check text-white text-[8px]"></i>
                    </div>
                    <span v-if="option.flag" class="text-lg">{{ option.flag }}</span>
                    <span v-else-if="poll.hasVoted" class="w-5 h-5 rounded-full flex items-center justify-center text-xs font-bold text-white"
                          :style="{ backgroundColor: option.color || '#14b8a6' }">{{ index + 1 }}</span>
                    <span class="text-sm font-medium text-gray-700 group-hover:text-gray-900">{{ option.label }}</span>
                  </div>
                  <div class="flex items-center gap-2">
                    <div class="w-16 h-1.5 bg-gray-200 rounded-full overflow-hidden">
                      <div class="h-full rounded-full transition-all duration-500"
                           :style="{ width: option.votes + '%', backgroundColor: option.color || '#14b8a6' }"></div>
                    </div>
                    <span class="text-sm font-bold min-w-[40px] text-end" :style="{ color: option.color || '#14b8a6' }">{{ option.votes }}%</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Poll Footer -->
            <div class="flex items-center justify-between mt-4 pt-4 border-t border-gray-100">
              <div v-if="poll.hasVoted" class="flex items-center gap-1.5 text-xs text-green-600 font-medium">
                <i class="fas fa-check-circle"></i>
                <span>{{ $t('dashboard.youVoted') }}</span>
              </div>
              <div v-else class="text-xs text-gray-500">
                <i class="fas fa-users me-1"></i> {{ selectedPollOptions[poll.id] ? $t('dashboard.readyToVote') : $t('dashboard.selectAnOption') }}
              </div>
              <button @click="votePoll(poll)"
                      class="px-4 py-2 text-xs font-semibold rounded-lg transition-all"
                      :class="poll.hasVoted
                        ? 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                        : selectedPollOptions[poll.id]
                          ? 'bg-gradient-to-r from-teal-500 to-teal-600 text-white shadow-md shadow-teal-200 hover:shadow-lg'
                          : 'bg-gray-200 text-gray-400 cursor-not-allowed'"
                      :disabled="!poll.hasVoted && !selectedPollOptions[poll.id]">
                {{ poll.hasVoted ? 'View Results' : 'Vote Now' }}
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Learning Progress - Enhanced -->
      <div class="card-animated rounded-xl lg:rounded-2xl p-4 lg:p-6 stagger-5 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-graduation-cap text-white text-sm"></i>
            </div>
            <div>
              <span class="block">{{ $t('dashboard.myLearning') }}</span>
              <span class="text-xs font-medium text-teal-600">{{ $t('dashboard.continueYourJourney') }}</span>
            </div>
          </h2>
          <router-link to="/learning" class="px-3 py-1.5 text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1.5 transition-all">
            {{ $t('common.explore') }} <i class="fas fa-arrow-right text-xs"></i>
          </router-link>
        </div>

        <!-- Quick Stats -->
        <div class="grid grid-cols-4 gap-2 mb-4">
          <div class="text-center p-2 rounded-lg bg-white border border-gray-100">
            <p class="text-lg font-bold text-teal-600">{{ learningStats.inProgress }}</p>
            <p class="text-[9px] text-gray-500 font-medium">{{ $t('dashboard.inProgress') }}</p>
          </div>
          <div class="text-center p-2 rounded-lg bg-white border border-gray-100">
            <p class="text-lg font-bold text-green-600">{{ learningStats.completed }}</p>
            <p class="text-[9px] text-gray-500 font-medium">{{ $t('dashboard.completed') }}</p>
          </div>
          <div class="text-center p-2 rounded-lg bg-white border border-gray-100">
            <p class="text-lg font-bold text-blue-600">{{ learningStats.totalHours }}</p>
            <p class="text-[9px] text-gray-500 font-medium">{{ $t('dashboard.hours') }}</p>
          </div>
          <div class="text-center p-2 rounded-lg bg-white border border-gray-100">
            <p class="text-lg font-bold text-amber-600">{{ learningStats.certificates }}</p>
            <p class="text-[9px] text-gray-500 font-medium">{{ $t('dashboard.certificates') }}</p>
          </div>
        </div>

        <!-- Course List -->
        <div class="space-y-3">
          <div v-for="course in learningCourses" :key="course.id"
               @click="viewCourse(course.id)"
               class="course-card relative rounded-xl cursor-pointer border border-gray-100 hover:border-teal-200 hover:shadow-lg transition-all group overflow-hidden bg-white">

            <div class="p-3 h-full flex flex-col">
              <div class="flex gap-3">
                <!-- Course Thumbnail -->
                <div class="relative flex-shrink-0">
                  <div class="w-16 h-16 rounded-xl overflow-hidden shadow-md transition-transform group-hover:scale-105">
                    <img v-if="course.image" :src="course.image" :alt="course.title" class="w-full h-full object-cover">
                    <div v-else :class="['w-full h-full bg-gradient-to-br flex items-center justify-center', course.gradientClass]">
                      <i :class="[course.icon, 'text-white text-xl']"></i>
                    </div>
                  </div>
                  <!-- Progress Ring -->
                  <div class="absolute -bottom-1 -end-1 w-7 h-7 rounded-full bg-white shadow-md flex items-center justify-center">
                    <svg class="w-5 h-5 -rotate-90">
                      <circle cx="10" cy="10" r="8" fill="none" stroke="#e5e7eb" stroke-width="2"/>
                      <circle cx="10" cy="10" r="8" fill="none" stroke="#14b8a6" stroke-width="2"
                              :stroke-dasharray="50.3"
                              :stroke-dashoffset="50.3 - (50.3 * course.progress / 100)"
                              stroke-linecap="round"/>
                    </svg>
                    <span class="absolute text-[8px] font-bold text-teal-600">{{ course.progress }}%</span>
                  </div>
                </div>

                <!-- Course Info -->
                <div class="flex-1 min-w-0 flex flex-col">
                  <div class="flex items-start justify-between gap-2">
                    <h4 class="font-semibold text-gray-900 text-sm leading-tight group-hover:text-teal-600 transition-colors line-clamp-1">{{ course.title }}</h4>

                    <!-- Actions moved to top right -->
                    <div class="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity flex-shrink-0">
                      <button @click="toggleCourseBookmark(course.id, $event)"
                              :class="['w-7 h-7 rounded-lg flex items-center justify-center transition-all', isCourseBookmarked(course.id) ? 'bg-teal-500 text-white' : 'bg-teal-50 hover:bg-teal-100 text-teal-500 hover:text-teal-600']"
                              title="Bookmark">
                        <i :class="[isCourseBookmarked(course.id) ? 'fas' : 'far', 'fa-bookmark text-[10px]']"></i>
                      </button>
                      <button @click="shareCourse(course.id, $event)"
                              class="w-7 h-7 rounded-lg bg-teal-50 hover:bg-teal-100 flex items-center justify-center text-teal-500 hover:text-teal-600 transition-all"
                              title="Share">
                        <i class="fas fa-share-alt text-[10px]"></i>
                      </button>
                      <button @click="continueCourse(course.id, $event)"
                              class="w-7 h-7 rounded-lg bg-teal-500 hover:bg-teal-600 flex items-center justify-center text-white transition-all"
                              title="Continue">
                        <i class="fas fa-play text-[10px]"></i>
                      </button>
                    </div>
                  </div>

                  <!-- Category & Difficulty -->
                  <div class="flex items-center gap-2 mt-1">
                    <span class="text-[9px] font-semibold px-1.5 py-0.5 rounded-full bg-teal-100 text-teal-700">{{ course.category }}</span>
                    <span :class="['text-[9px] font-semibold px-1.5 py-0.5 rounded-full',
                      course.difficulty === 'Beginner' ? 'bg-green-100 text-green-700' :
                      course.difficulty === 'Intermediate' ? 'bg-amber-100 text-amber-700' :
                      'bg-red-100 text-red-700']">
                      {{ course.difficulty }}
                    </span>
                  </div>

                  <!-- Instructor -->
                  <div class="flex items-center gap-2 mt-2">
                    <div class="w-5 h-5 rounded-full flex items-center justify-center text-[8px] font-bold text-white"
                         :style="{ backgroundColor: course.instructorColor }">
                      {{ course.instructorAvatar }}
                    </div>
                    <span class="text-[10px] text-gray-500">{{ course.instructor }}</span>
                  </div>
                </div>
              </div>

              <!-- Progress Bar & Stats - pushed to bottom -->
              <div class="mt-auto pt-3 border-t border-gray-100">
                <div class="flex items-center justify-between mb-1.5">
                  <div class="flex items-center gap-3 text-[10px] text-gray-500">
                    <span><i class="fas fa-book-open me-1 text-teal-400"></i>{{ course.lessonsCompleted }}/{{ course.totalLessons }} {{ $t('dashboard.lessons') }}</span>
                    <span><i class="fas fa-clock me-1 text-teal-400"></i>{{ course.duration }}</span>
                  </div>
                  <span class="text-[10px] text-gray-400">{{ course.lastAccessed }}</span>
                </div>
                <div class="h-1.5 bg-gray-100 rounded-full overflow-hidden">
                  <div class="h-full bg-gradient-to-r from-teal-500 to-teal-600 rounded-full transition-all duration-500" :style="{ width: course.progress + '%' }"></div>
                </div>
              </div>
            </div>
          </div>
        </div>

      </div>
    </div>

    <!-- Main Grid Row 3 -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 lg:gap-6">
      <!-- Team Activity - Enhanced -->
      <div class="card-animated rounded-xl lg:rounded-2xl p-4 lg:p-6 stagger-5 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-users text-white text-sm"></i>
            </div>
            <div>
              <span class="block">{{ $t('dashboard.teamActivity') }}</span>
              <span class="text-xs font-medium text-teal-600">{{ $t('dashboard.recentUpdates') }}</span>
            </div>
          </h2>
          <router-link to="/collaboration" class="px-3 py-1.5 text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1.5 transition-all">
            {{ $t('common.viewAll') }} <i class="fas fa-arrow-right text-xs"></i>
          </router-link>
        </div>

        <div class="space-y-3 max-h-[400px] overflow-y-auto pe-1 custom-scrollbar">
          <div v-for="activity in teamActivities" :key="activity.id"
               @click="viewActivityTarget(activity)"
               class="activity-card p-3 rounded-xl bg-white border border-gray-100 hover:border-teal-200 hover:shadow-lg cursor-pointer transition-all group">

            <div class="flex gap-3">
              <!-- User Avatar -->
              <div class="relative flex-shrink-0">
                <div @click.stop="viewUserProfile(activity.user.id)"
                     class="w-10 h-10 rounded-xl flex items-center justify-center text-white text-xs font-bold cursor-pointer transition-transform group-hover:scale-110 shadow-md"
                     :style="{ backgroundColor: activity.user.color }"
                     :title="'View ' + activity.user.name + '\'s profile'">
                  {{ activity.user.initials }}
                </div>
                <!-- Action Icon Badge -->
                <div :class="['absolute -bottom-1 -end-1 w-5 h-5 rounded-full flex items-center justify-center border-2 border-white', activity.actionBg]">
                  <i :class="[activity.actionIcon, 'text-[8px]', activity.actionColor]"></i>
                </div>
              </div>

              <!-- Activity Content -->
              <div class="flex-1 min-w-0">
                <div class="flex items-start justify-between gap-2">
                  <div class="min-w-0">
                    <p class="text-sm text-gray-900 leading-snug">
                      <span @click.stop="viewUserProfile(activity.user.id)" class="font-semibold hover:text-teal-600 cursor-pointer transition-colors">{{ activity.user.name }}</span>
                      <span class="text-gray-500"> {{ activity.action }}</span>
                    </p>
                    <p class="text-sm font-medium text-teal-600 hover:text-teal-700 truncate mt-0.5 cursor-pointer transition-colors">{{ activity.target }}</p>
                  </div>

                  <!-- Time -->
                  <span class="text-[10px] text-gray-400 whitespace-nowrap flex-shrink-0">{{ activity.time }}</span>
                </div>

                <!-- User Role & Stats -->
                <div class="flex items-center justify-between mt-2 pt-2 border-t border-gray-50">
                  <span class="text-[10px] text-gray-400">{{ activity.user.role }}</span>

                  <!-- Engagement Stats & Actions -->
                  <div class="flex items-center gap-3">
                    <button @click="toggleActivityLike(activity.id, $event)"
                            :class="['flex items-center gap-1 text-[10px] transition-colors', isActivityLiked(activity.id) ? 'text-rose-500' : 'text-gray-400 hover:text-rose-500']">
                      <i :class="[isActivityLiked(activity.id) ? 'fas' : 'far', 'fa-heart']"></i>
                      <span>{{ activity.likes }}</span>
                    </button>
                    <button @click="commentOnActivity(activity.id, $event)"
                            class="flex items-center gap-1 text-[10px] text-gray-400 hover:text-teal-500 transition-colors">
                      <i class="far fa-comment"></i>
                      <span>{{ activity.comments }}</span>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Self Services - Enhanced -->
      <div class="card-animated rounded-2xl p-6 stagger-6 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-th-large text-white text-sm"></i>
            </div>
            <div>
              <span class="block">{{ $t('dashboard.quickServices') }}</span>
              <span class="text-xs font-medium text-teal-600">{{ $t('dashboard.afcAsianCupShort') }}</span>
            </div>
          </h2>
          <router-link to="/self-services" class="px-3 py-1.5 text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1.5 transition-all">
            {{ $t('common.viewAll') }} <i class="fas fa-arrow-right text-xs"></i>
          </router-link>
        </div>

        <div class="grid grid-cols-2 sm:grid-cols-3 gap-3">
          <button v-for="service in selfServices" :key="service.id"
                  @click="openService(service)"
                  class="service-card relative flex flex-col items-center gap-3 p-4 rounded-xl bg-white border border-gray-100 hover:border-teal-200 hover:shadow-lg transition-all group overflow-hidden">

            <!-- Popular/New Badge -->
            <div v-if="service.isNew" class="absolute top-0 end-0">
              <div class="bg-gradient-to-r from-green-400 to-emerald-500 text-white text-[8px] font-bold px-2 py-0.5 rounded-es-lg rtl:rounded-es-none rtl:rounded-ee-lg">
                {{ $t('dashboard.new') }}
              </div>
            </div>
            <div v-else-if="service.isPopular" class="absolute top-0 end-0">
              <div class="bg-gradient-to-r from-amber-400 to-orange-500 text-white text-[8px] font-bold px-2 py-0.5 rounded-es-lg rtl:rounded-es-none rtl:rounded-ee-lg">
                <i class="fas fa-fire me-0.5"></i>{{ $t('dashboard.popular') }}
              </div>
            </div>

            <!-- Icon -->
            <div :class="['w-12 h-12 rounded-xl bg-gradient-to-br flex items-center justify-center shadow-md transition-transform group-hover:scale-110 group-hover:shadow-lg', service.gradientClass]">
              <i :class="[service.icon, 'text-white text-lg']"></i>
            </div>

            <!-- Content -->
            <div class="text-center">
              <h4 class="text-sm font-semibold text-gray-900 group-hover:text-teal-600 transition-colors">{{ service.label }}</h4>
              <p class="text-[10px] text-gray-400 mt-0.5">{{ service.description }}</p>
            </div>

            <!-- Usage Count -->
            <div class="flex items-center gap-1 text-[9px] text-gray-400">
              <i class="fas fa-users"></i>
              <span>{{ service.usageCount.toLocaleString() }} uses</span>
            </div>
          </button>
        </div>
      </div>
    </div>

    <!-- AI Insights Panel Modal -->
    <Teleport to="body">
      <div v-if="showAIInsightsPanel" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[80vh] overflow-hidden">
          <div class="p-5 border-b border-gray-100 bg-gradient-to-r from-teal-500 to-emerald-500">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-white/20 flex items-center justify-center">
                  <i class="fas fa-wand-magic-sparkles text-white"></i>
                </div>
                <div>
                  <h3 class="font-semibold text-white">AI-Powered Insights</h3>
                  <p class="text-xs text-white/80">{{ $t('dashboard.intelligentAnalysis') }}</p>
                </div>
              </div>
              <button @click="showAIInsightsPanel = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <i class="fas fa-times text-white"></i>
              </button>
            </div>
          </div>

          <div class="p-5 overflow-y-auto max-h-[60vh]">
            <div v-if="isLoadingInsights" class="text-center py-12">
              <i class="fas fa-circle-notch fa-spin text-teal-500 text-3xl mb-3"></i>
              <p class="text-gray-500">{{ $t('dashboard.analyzingPlatformData') }}</p>
            </div>

            <div v-else class="space-y-4">
              <div v-for="insight in aiInsights" :key="insight.id"
                   @click="navigateToInsight(insight)"
                   class="p-4 rounded-xl border border-gray-200 hover:border-teal-300 hover:shadow-md transition-all cursor-pointer group">
                <div class="flex items-start gap-4">
                  <div :class="['w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0', getInsightTypeColor(insight.type)]">
                    <i :class="[getInsightTypeIcon(insight.type), 'text-lg']"></i>
                  </div>
                  <div class="flex-1 min-w-0">
                    <div class="flex items-center gap-2 mb-1">
                      <h4 class="font-semibold text-gray-900 group-hover:text-teal-700 transition-colors">{{ insight.title }}</h4>
                      <span v-if="insight.change" :class="['text-xs font-bold', insight.change > 0 ? 'text-green-600' : 'text-red-600']">
                        {{ insight.change > 0 ? '+' : '' }}{{ insight.change }}%
                      </span>
                    </div>
                    <p class="text-sm text-gray-600 mb-2">{{ insight.description }}</p>
                    <div class="flex items-center gap-4">
                      <div class="flex items-center gap-1.5">
                        <span class="text-xs text-gray-400">{{ $t('dashboard.confidence') }}:</span>
                        <div class="w-20 h-1.5 bg-gray-200 rounded-full overflow-hidden">
                          <div class="h-full bg-teal-500 rounded-full" :style="{ width: `${insight.confidence * 100}%` }"></div>
                        </div>
                        <span class="text-xs font-medium text-teal-600">{{ Math.round(insight.confidence * 100) }}%</span>
                      </div>
                      <span v-if="insight.actionLabel" class="text-xs text-teal-600 font-medium flex items-center gap-1">
                        {{ insight.actionLabel }} <i class="fas fa-arrow-right"></i>
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="p-4 border-t border-gray-100 bg-gray-50 flex justify-between items-center">
            <p class="text-xs text-gray-500">
              <i class="fas fa-info-circle me-1"></i>
              {{ $t('dashboard.insightsGenerated') }}
            </p>
            <button @click="showAIInsightsPanel = false"
                    class="px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors text-sm font-medium">
              Close
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- AI Recommendations Modal -->
    <Teleport to="body">
      <div v-if="showRecommendationsModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-lg max-h-[80vh] overflow-hidden">
          <div class="p-5 border-b border-gray-100 bg-gradient-to-r from-purple-500 to-indigo-500">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-white/20 flex items-center justify-center">
                  <i class="fas fa-compass text-white"></i>
                </div>
                <div>
                  <h3 class="font-semibold text-white">{{ $t('dashboard.recommendedForYou') }}</h3>
                  <p class="text-xs text-white/80">{{ $t('dashboard.personalizedSuggestions') }}</p>
                </div>
              </div>
              <button @click="showRecommendationsModal = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <i class="fas fa-times text-white"></i>
              </button>
            </div>
          </div>

          <div class="p-4 overflow-y-auto max-h-[55vh]">
            <div v-if="isLoadingRecommendations" class="text-center py-12">
              <i class="fas fa-circle-notch fa-spin text-purple-500 text-3xl mb-3"></i>
              <p class="text-gray-500">{{ $t('dashboard.findingContentForYou') }}</p>
            </div>

            <div v-else class="space-y-3">
              <div v-for="rec in contentRecommendations" :key="rec.id"
                   @click="navigateToRecommendation(rec)"
                   class="p-4 rounded-xl border border-gray-200 hover:border-purple-300 hover:bg-purple-50/30 transition-all cursor-pointer group">
                <div class="flex items-start gap-3">
                  <div :class="['w-10 h-10 rounded-lg flex items-center justify-center flex-shrink-0', rec.iconBg]">
                    <i :class="[rec.icon, 'text-gray-600']"></i>
                  </div>
                  <div class="flex-1 min-w-0">
                    <div class="flex items-center justify-between mb-1">
                      <h4 class="font-medium text-gray-900 group-hover:text-purple-700 transition-colors">{{ rec.title }}</h4>
                      <span class="text-xs font-bold text-purple-600">{{ Math.round(rec.relevanceScore * 100) }}% match</span>
                    </div>
                    <p class="text-xs text-gray-500">{{ rec.reason }}</p>
                    <span class="inline-block mt-2 text-xs text-gray-400 capitalize">
                      <i :class="rec.icon" class="me-1"></i>{{ rec.type }}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="p-4 border-t border-gray-100 bg-gray-50 flex justify-between items-center">
            <p class="text-xs text-gray-500">
              <i class="fas fa-robot me-1"></i>
              {{ $t('dashboard.basedOnActivity') }}
            </p>
            <button @click="showRecommendationsModal = false"
                    class="px-4 py-2 bg-purple-500 text-white rounded-lg hover:bg-purple-600 transition-colors text-sm font-medium">
              Close
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
/* Welcome Hero Section */
.welcome-hero {
  background: linear-gradient(135deg, #0d9488 0%, #14b8a6 50%, #10b981 100%);
  box-shadow: 0 10px 40px rgba(13, 148, 136, 0.3);
}

/* Trophy Animation */
.trophy-glow {
  animation: trophy-pulse 3s ease-in-out infinite, trophy-float 4s ease-in-out infinite;
}

@keyframes trophy-pulse {
  0%, 100% {
    box-shadow: 0 0 20px rgba(255, 255, 255, 0.2), 0 0 40px rgba(255, 255, 255, 0.1);
  }
  50% {
    box-shadow: 0 0 30px rgba(255, 255, 255, 0.4), 0 0 60px rgba(255, 255, 255, 0.2);
  }
}

@keyframes trophy-float {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-8px);
  }
}

/* Decorative Circle Animations */
.circle-drift-1 {
  animation: drift-1 20s ease-in-out infinite;
}

.circle-drift-2 {
  animation: drift-2 25s ease-in-out infinite;
}

.circle-drift-3 {
  animation: drift-3 18s ease-in-out infinite;
}

@keyframes drift-1 {
  0%, 100% {
    transform: translate(33%, -50%);
  }
  25% {
    transform: translate(28%, -45%);
  }
  50% {
    transform: translate(35%, -55%);
  }
  75% {
    transform: translate(30%, -48%);
  }
}

@keyframes drift-2 {
  0%, 100% {
    transform: translate(-33%, 50%);
  }
  33% {
    transform: translate(-28%, 45%);
  }
  66% {
    transform: translate(-38%, 55%);
  }
}

@keyframes drift-3 {
  0%, 100% {
    transform: translate(0, 0) scale(1);
  }
  50% {
    transform: translate(10px, -10px) scale(1.1);
  }
}

/* Latest Updates Section - Media Center Featured Style */
.updates-section {
  min-width: 0;
}

.updates-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

@media (min-width: 640px) {
  .updates-header {
    margin-bottom: 0.75rem;
  }
}

.updates-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9375rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0;
}

.updates-view-all {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  font-weight: 600;
  color: #0d9488;
  transition: all 0.2s ease;
}

.updates-view-all:hover {
  color: #0f766e;
  gap: 0.5rem;
}

/* Updates Grid */
.updates-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 0.75rem;
}

@media (min-width: 640px) {
  .updates-grid {
    gap: 1rem;
  }
}

@media (min-width: 768px) {
  .updates-grid {
    grid-template-columns: 1.5fr 1fr;
  }
}

@media (min-width: 1024px) {
  .updates-grid {
    grid-template-columns: 1.6fr 1fr;
    gap: 1rem;
  }
}

@media (min-width: 1280px) {
  .updates-grid {
    grid-template-columns: 1.8fr 1fr;
    gap: 1.25rem;
  }
}

/* Featured Main Card - Media Center Style */
.updates-featured {
  position: relative;
}

.featured-main-card {
  position: relative;
  display: block;
  border-radius: 0.75rem;
  overflow: hidden;
  cursor: pointer;
  background: #0f172a;
  aspect-ratio: 16/9;
}

@media (min-width: 640px) {
  .featured-main-card {
    border-radius: 1rem;
  }
}

@media (min-width: 1024px) {
  .featured-main-card {
    aspect-ratio: 16/9;
  }
}

@media (min-width: 1280px) {
  .featured-main-card {
    aspect-ratio: 2/1;
  }
}

.featured-main-img {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.featured-main-card:hover .featured-main-img {
  transform: scale(1.05);
}

.featured-main-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(to top, rgba(0,0,0,0.8) 0%, rgba(0,0,0,0.2) 50%, rgba(0,0,0,0.1) 100%);
  transition: background 0.3s ease;
}

.featured-main-card:hover .featured-main-overlay {
  background: linear-gradient(to top, rgba(0,0,0,0.85) 0%, rgba(0,0,0,0.5) 50%, rgba(0,0,0,0.4) 100%);
}

.featured-main-badges {
  position: absolute;
  top: 0.75rem;
  left: 0.75rem;
  display: flex;
  gap: 0.5rem;
  z-index: 2;
}

.badge-featured {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.375rem 0.75rem;
  background: linear-gradient(135deg, #f59e0b, #d97706);
  color: white;
  border-radius: 0.375rem;
  font-size: 0.6875rem;
  font-weight: 700;
  text-transform: uppercase;
}

.badge-category {
  display: inline-flex;
  align-items: center;
  padding: 0.375rem 0.75rem;
  background: rgba(20, 184, 166, 0.9);
  color: white;
  border-radius: 0.375rem;
  font-size: 0.6875rem;
  font-weight: 600;
  text-transform: uppercase;
}

.featured-hover-actions {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  opacity: 0;
  visibility: hidden;
  transition: all 0.3s ease;
  z-index: 10;
}

.featured-main-card:hover .featured-hover-actions {
  opacity: 1;
  visibility: visible;
}

.featured-hover-btn {
  padding: 0.875rem 1.75rem;
  border-radius: 0.75rem;
  background: rgba(255, 255, 255, 0.95);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  font-size: 0.9375rem;
  font-weight: 600;
  color: #0d9488;
  transition: all 0.3s ease;
  cursor: pointer;
  text-decoration: none;
  box-shadow: 0 4px 15px rgba(0,0,0,0.2);
}

.featured-hover-btn:hover {
  background: #0d9488;
  color: white;
  transform: scale(1.05);
  box-shadow: 0 6px 25px rgba(0,0,0,0.3);
}

.featured-hover-icons {
  display: flex;
  gap: 0.5rem;
}

.hover-icon-btn {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.9);
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  color: #64748b;
  font-size: 0.875rem;
  box-shadow: 0 2px 10px rgba(0,0,0,0.15);
}

.hover-icon-btn:hover {
  background: white;
  transform: scale(1.1);
  box-shadow: 0 4px 15px rgba(0,0,0,0.2);
}

.hover-icon-btn.active {
  color: #ef4444;
}

.hover-icon-btn:nth-child(2).active {
  color: #f59e0b;
}

.featured-main-content {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 1rem;
  z-index: 2;
}

@media (min-width: 640px) {
  .featured-main-content {
    padding: 1.25rem;
  }
}

@media (min-width: 1024px) {
  .featured-main-content {
    padding: 1.25rem;
  }
}

@media (min-width: 1280px) {
  .featured-main-content {
    padding: 1.5rem;
  }
}

.featured-main-meta {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
  color: rgba(255,255,255,0.9);
  font-size: 0.75rem;
}

@media (min-width: 640px) {
  .featured-main-meta {
    gap: 0.75rem;
    margin-bottom: 0.625rem;
    font-size: 0.8125rem;
  }
}

@media (min-width: 1280px) {
  .featured-main-meta {
    gap: 1rem;
    margin-bottom: 0.75rem;
    font-size: 0.875rem;
  }
}

.featured-main-meta span {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.featured-main-meta i {
  color: white;
}

.featured-main-title {
  font-size: 1.125rem;
  font-weight: 700;
  color: white;
  line-height: 1.3;
  margin: 0 0 0.375rem 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  transition: color 0.3s ease;
}

@media (min-width: 640px) {
  .featured-main-title {
    font-size: 1.25rem;
    margin: 0 0 0.5rem 0;
  }
}

@media (min-width: 1024px) {
  .featured-main-title {
    font-size: 1.25rem;
  }
}

@media (min-width: 1280px) {
  .featured-main-title {
    font-size: 1.5rem;
  }
}

.featured-main-card:hover .featured-main-title {
  color: #99f6e4;
}

.featured-main-desc {
  font-size: 0.8125rem;
  color: rgba(255,255,255,0.85);
  line-height: 1.5;
  margin: 0 0 0.5rem 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

@media (min-width: 640px) {
  .featured-main-desc {
    font-size: 0.875rem;
    margin: 0 0 0.625rem 0;
  }
}

@media (min-width: 1024px) {
  .featured-main-desc {
    font-size: 0.875rem;
    -webkit-line-clamp: 2;
  }
}

@media (min-width: 1280px) {
  .featured-main-desc {
    font-size: 1rem;
    margin: 0 0 0.75rem 0;
  }
}

.featured-main-author {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  color: rgba(255,255,255,0.9);
}

@media (min-width: 640px) {
  .featured-main-author {
    gap: 0.5rem;
    font-size: 0.875rem;
  }
}

.author-avatar-sm {
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 0.25rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 0.5rem;
  font-weight: 700;
}

@media (min-width: 640px) {
  .author-avatar-sm {
    width: 1.75rem;
    height: 1.75rem;
    border-radius: 0.375rem;
    font-size: 0.5625rem;
  }
}

.dot {
  opacity: 0.5;
}

/* Up Next Column */
.up-next-column {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.up-next-title {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  font-weight: 600;
  color: #334155;
  margin: 0 0 0.5rem 0;
}

@media (min-width: 640px) {
  .up-next-title {
    gap: 0.5rem;
    font-size: 0.8125rem;
    margin: 0 0 0.625rem 0;
  }
}

.up-next-title i {
  color: #64748b;
  font-size: 0.625rem;
}

@media (min-width: 640px) {
  .up-next-title i {
    font-size: 0.6875rem;
  }
}

.up-next-list {
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
  max-height: 400px;
  overflow-y: auto;
}

@media (min-width: 640px) {
  .up-next-list {
    gap: 0.5rem;
    max-height: 450px;
  }
}

@media (min-width: 1024px) {
  .up-next-list {
    max-height: 380px;
  }
}

@media (min-width: 1280px) {
  .up-next-list {
    max-height: 420px;
  }
}

/* Carousel Navigation */
.carousel-arrow {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.9);
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  opacity: 0.7;
  z-index: 10;
  color: #334155;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  font-size: 0.75rem;
}

@media (min-width: 640px) {
  .carousel-arrow {
    width: 36px;
    height: 36px;
    opacity: 0;
    font-size: 1rem;
  }
}

.featured-main-card:hover .carousel-arrow {
  opacity: 1;
}

.carousel-arrow:hover {
  background: white;
  transform: translateY(-50%) scale(1.1);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.carousel-prev {
  left: 0.5rem;
}

.carousel-next {
  right: 0.5rem;
}

@media (min-width: 640px) {
  .carousel-prev {
    left: 0.75rem;
  }

  .carousel-next {
    right: 0.75rem;
  }
}

/* Carousel Dots */
.carousel-dots {
  position: absolute;
  bottom: 0.5rem;
  right: 0.5rem;
  display: flex;
  gap: 0.25rem;
  z-index: 10;
}

@media (min-width: 640px) {
  .carousel-dots {
    bottom: 0.75rem;
    right: 0.75rem;
    gap: 0.375rem;
  }
}

.carousel-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.5);
  border: none;
  cursor: pointer;
  transition: all 0.3s ease;
  padding: 0;
}

@media (min-width: 640px) {
  .carousel-dot {
    width: 8px;
    height: 8px;
  }
}

.carousel-dot:hover {
  background: rgba(255, 255, 255, 0.8);
}

.carousel-dot.active {
  background: white;
  width: 16px;
  border-radius: 3px;
}

@media (min-width: 640px) {
  .carousel-dot.active {
    width: 20px;
    border-radius: 4px;
  }
}

/* Carousel Fade Transition */
.carousel-fade-enter-active,
.carousel-fade-leave-active {
  transition: opacity 0.4s ease;
}

.carousel-fade-enter-from,
.carousel-fade-leave-to {
  opacity: 0;
}

.secondary-card {
  position: relative;
  background: white;
  border-radius: 1rem;
  overflow: hidden;
  border: 1px solid #e2e8f0;
  transition: all 0.3s ease;
  cursor: pointer;
}

.secondary-card:hover {
  border-color: #14b8a6;
  box-shadow: 0 8px 25px rgba(20, 184, 166, 0.15);
  transform: translateY(-4px);
}

.secondary-card.is-active {
  border-color: #14b8a6;
  border-left: 4px solid #14b8a6;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  box-shadow: 0 4px 15px rgba(20, 184, 166, 0.2);
}

.secondary-link {
  display: flex;
  flex-direction: column;
  height: 100%;
}

@media (min-width: 768px) {
  .secondary-link {
    flex-direction: row;
  }
}

.secondary-image {
  position: relative;
  height: 120px;
  flex-shrink: 0;
  overflow: hidden;
}

@media (min-width: 640px) {
  .secondary-image {
    height: 140px;
  }
}

@media (min-width: 768px) {
  .secondary-image {
    width: 120px;
    height: auto;
    min-height: 100px;
  }
}

@media (min-width: 1024px) {
  .secondary-image {
    width: 120px;
    min-height: 100px;
  }
}

@media (min-width: 1280px) {
  .secondary-image {
    width: 140px;
    min-height: 120px;
  }
}

.secondary-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.secondary-card:hover .secondary-image img {
  transform: scale(1.1);
}

.secondary-hover-actions {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  gap: 0.375rem;
  opacity: 0;
  visibility: hidden;
  transition: all 0.3s ease;
  z-index: 5;
}

.secondary-card:hover .secondary-hover-actions {
  opacity: 1;
  visibility: visible;
}

.secondary-action-btn {
  width: 2rem;
  height: 2rem;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.95);
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  color: #64748b;
  font-size: 0.75rem;
  text-decoration: none;
  box-shadow: 0 2px 8px rgba(0,0,0,0.15);
}

.secondary-action-btn:hover {
  transform: scale(1.15);
  box-shadow: 0 4px 12px rgba(0,0,0,0.2);
}

.secondary-action-btn.view {
  background: #0d9488;
  color: white;
}

.secondary-action-btn.view:hover {
  background: #0f766e;
}

.secondary-action-btn.active {
  color: #ef4444;
}

.secondary-action-btn:nth-child(3).active {
  color: #f59e0b;
}

.secondary-overlay {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.secondary-card:hover .secondary-overlay {
  opacity: 1;
}

.secondary-badge {
  position: absolute;
  top: 0.625rem;
  left: 0.625rem;
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.625rem;
  border-radius: 1rem;
  font-size: 0.625rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.03em;
}

.secondary-badge.announcement {
  background: linear-gradient(135deg, #fef3c7, #fde68a);
  color: #92400e;
}

.secondary-badge.article {
  background: linear-gradient(135deg, #dbeafe, #bfdbfe);
  color: #1e40af;
}

.secondary-badge.event {
  background: linear-gradient(135deg, #d1fae5, #a7f3d0);
  color: #065f46;
}

.secondary-badge.venue {
  background: linear-gradient(135deg, #ede9fe, #ddd6fe);
  color: #5b21b6;
}

.secondary-badge.stats {
  background: linear-gradient(135deg, #fee2e2, #fecaca);
  color: #991b1b;
}

.secondary-trending {
  position: absolute;
  top: 0.625rem;
  right: 0.625rem;
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 50%;
  background: linear-gradient(135deg, #ef4444, #dc2626);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.625rem;
}

.secondary-content {
  flex: 1;
  padding: 0.75rem;
  display: flex;
  flex-direction: column;
}

@media (min-width: 640px) {
  .secondary-content {
    padding: 0.875rem;
  }
}

@media (min-width: 1280px) {
  .secondary-content {
    padding: 1rem;
  }
}

.secondary-meta {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.5rem;
  font-size: 0.6875rem;
  color: #64748b;
}

.secondary-time {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.secondary-read {
  color: #14b8a6;
  font-weight: 600;
}

.secondary-title {
  font-size: 0.8125rem;
  font-weight: 700;
  color: #0f172a;
  line-height: 1.4;
  margin-bottom: 0.375rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  transition: color 0.3s ease;
}

@media (min-width: 640px) {
  .secondary-title {
    font-size: 0.875rem;
    margin-bottom: 0.5rem;
  }
}

@media (min-width: 1280px) {
  .secondary-title {
    font-size: 0.9375rem;
  }
}

.secondary-card:hover .secondary-title {
  color: #0d9488;
}

.secondary-description {
  font-size: 0.75rem;
  color: #64748b;
  line-height: 1.5;
  display: none;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  overflow: hidden;
  margin-bottom: 0.5rem;
  flex: 1;
}

@media (min-width: 640px) {
  .secondary-description {
    display: -webkit-box;
    -webkit-line-clamp: 1;
  }
}

@media (min-width: 1024px) {
  .secondary-description {
    -webkit-line-clamp: 1;
    font-size: 0.75rem;
  }
}

@media (min-width: 1280px) {
  .secondary-description {
    -webkit-line-clamp: 2;
    font-size: 0.8125rem;
    margin-bottom: 0.75rem;
  }
}

.secondary-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 0.75rem;
  border-top: 1px solid #f1f5f9;
}

.secondary-author {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.75rem;
  color: #475569;
  font-weight: 500;
}

.secondary-avatar {
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 0.375rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 0.5625rem;
  font-weight: 700;
}

.secondary-stats {
  display: flex;
  gap: 0.75rem;
  font-size: 0.6875rem;
  color: #94a3b8;
}

.secondary-stats span {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.secondary-stats i {
  color: #14b8a6;
  font-size: 0.625rem;
}

/* Progress bar animation */
.progress-animated {
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  transition: width 1s ease-out;
}

/* Icon soft style */
.icon-soft {
  background: linear-gradient(135deg, #f0fdfa, #ccfbf1);
  color: #0d9488;
}

/* Media card hover */
.media-card:hover .rounded-xl {
  transform: scale(1.02);
  transition: transform 0.3s ease;
}

/* Custom scrollbar for activity feed */
.custom-scrollbar::-webkit-scrollbar {
  width: 4px;
}

.custom-scrollbar::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 4px;
}

.custom-scrollbar::-webkit-scrollbar-thumb {
  background: linear-gradient(180deg, #14b8a6, #0d9488);
  border-radius: 4px;
}

.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(180deg, #0d9488, #0f766e);
}
</style>
