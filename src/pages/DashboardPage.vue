<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

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
const activePolls = ref([
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
    alert(`You voted for: ${selectedOption}`)
  } else if (poll.hasVoted) {
    // View results - navigate to poll page
    router.push(`/polls/${poll.id}`)
  } else {
    alert('Please select an option first')
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
  router.push(`/users/${userId}`)
}

function viewActivityTarget(activity: any) {
  const routes: Record<string, string> = {
    article: '/articles',
    document: '/documents',
    course: '/learning',
    event: '/events'
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
  const routes: Record<string, string> = {
    'IT Support': '/self-services/it-support',
    'HR Portal': '/self-services/hr',
    'Facilities': '/self-services/facilities',
    'Finance': '/self-services/finance',
    'Travel': '/self-services/travel',
    'Benefits': '/self-services/benefits'
  }
  router.push(routes[service.label] || '/self-services')
}

// Self Services
const selfServices = ref([
  { id: 1, label: 'IT Support', icon: 'fas fa-headset', iconBg: 'bg-blue-100', iconColor: 'text-blue-600' },
  { id: 2, label: 'HR Requests', icon: 'fas fa-user-tie', iconBg: 'bg-violet-100', iconColor: 'text-violet-600' },
  { id: 3, label: 'Book Room', icon: 'fas fa-door-open', iconBg: 'bg-emerald-100', iconColor: 'text-emerald-600' },
  { id: 4, label: 'Expenses', icon: 'fas fa-receipt', iconBg: 'bg-amber-100', iconColor: 'text-amber-600' },
  { id: 5, label: 'Time Off', icon: 'fas fa-umbrella-beach', iconBg: 'bg-primary-100', iconColor: 'text-primary-600' },
  { id: 6, label: 'Feedback', icon: 'fas fa-comment-dots', iconBg: 'bg-rose-100', iconColor: 'text-rose-600' }
])

// Carousel State
const currentSlide = ref(0)
const isAutoPlaying = ref(true)
let carouselInterval: number | null = null

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
    author: { name: 'AFC Media', initials: 'AF', role: 'Official News', color: '#006847' },
    views: '245,847',
    comments: '4,521',
    link: '/events',
    actionText: 'View Match Details'
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
    author: { name: 'Sports Desk', initials: 'SD', role: 'Football Analysis', color: '#006847' },
    views: '189,562',
    comments: '2,856',
    link: '/articles',
    actionText: 'Read Full Story'
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
    author: { name: 'LOC Events', initials: 'LO', role: 'Local Organizing Committee', color: '#10b981' },
    views: '167,103',
    comments: '1,892',
    link: '/events',
    actionText: 'Event Details'
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
    author: { name: 'Venue Operations', initials: 'VO', role: 'Stadium Management', color: '#8b5cf6' },
    views: '98,543',
    comments: '734',
    link: '/events',
    actionText: 'Explore Venues'
  },
  {
    id: 5,
    type: 'stats',
    typeLabel: 'Statistics',
    typeIcon: 'fas fa-chart-bar',
    title: 'AFC Asian Cup History: Japan Leads with 4 Titles',
    description: 'Japan leads the Asian Cup honors with 4 titles, followed by Saudi Arabia and Iran with 3 each. Explore the complete history of champions, top scorers, and legendary moments from 1956 to present.',
    icon: 'fas fa-medal',
    image: 'https://images.unsplash.com/photo-1560272564-c83b66b1ad12?w=600&h=400&fit=crop',
    gradientClass: 'bg-gradient-to-br from-red-400 to-rose-500',
    date: 'Jan 2, 2027',
    author: { name: 'Stats Team', initials: 'ST', role: 'Data Analytics', color: '#ef4444' },
    views: '145,876',
    comments: '1,267',
    link: '/articles',
    actionText: 'View Statistics'
  }
])

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
  <div class="px-8 py-6">
    <!-- Welcome Section -->
    <div class="card-animated rounded-2xl p-8 mb-8 relative overflow-hidden stagger-1">
      <!-- Decorative elements -->
      <div class="welcome-decoration w-64 h-64 -top-32 -right-32" style="animation-delay: 0s;"></div>
      <div class="welcome-decoration w-48 h-48 -bottom-24 -left-24" style="animation-delay: 2s;"></div>
      <div class="welcome-decoration w-32 h-32 top-1/2 right-1/4" style="animation-delay: 4s;"></div>

      <div class="relative flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 mb-2 fade-in-up" style="animation-delay: 0.3s;">
            Good {{ timeOfDay }}, Ahmed <span class="text-xl inline-block icon-float">👋</span>
          </h1>
          <p class="text-gray-500 max-w-lg fade-in-up" style="animation-delay: 0.4s;">
            You have <span class="font-medium text-primary-600">3 pending approvals</span> and
            <span class="font-medium text-primary-600">2 new courses</span> waiting for you.
          </p>
          <div class="flex gap-3 mt-6 fade-in-up" style="animation-delay: 0.5s;">
            <router-link to="/learning" class="px-5 py-2.5 btn-vibrant text-white rounded-xl font-medium text-sm flex items-center gap-2 ripple">
              <i class="fas fa-play text-xs"></i>
              Continue Learning
            </router-link>
            <router-link to="/articles/new" class="px-5 py-2.5 bg-white border border-gray-200 text-gray-700 rounded-xl font-medium text-sm hover:bg-primary-50 hover:border-primary-200 hover:text-primary-700 transition-all flex items-center gap-2 ripple">
              <i class="fas fa-plus text-xs"></i>
              Create Content
            </router-link>
          </div>
        </div>
        <div class="hidden lg:flex items-center justify-center scale-in" style="animation-delay: 0.6s;">
          <div class="w-36 h-36 rounded-2xl bg-gradient-to-br from-primary-400 to-primary-600 flex items-center justify-center shadow-2xl shadow-primary-500/30 icon-float">
            <i class="fas fa-lightbulb text-white text-5xl"></i>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Updates Carousel - Featured Section -->
    <div class="card-animated rounded-3xl p-8 mb-10 stagger-1 bg-gradient-to-br from-white via-white to-teal-50/50 border-2 border-teal-100 shadow-xl shadow-teal-100/20" @mouseenter="pauseCarousel" @mouseleave="resumeCarousel">
      <div class="flex items-center justify-between mb-6">
        <h2 class="text-2xl font-bold text-gray-900 flex items-center gap-4">
          <div class="w-12 h-12 rounded-2xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
            <i class="fas fa-fire text-white text-lg"></i>
          </div>
          <div>
            <span class="block">Latest Updates</span>
            <span class="text-sm font-medium text-teal-600">AFC Asian Cup Saudi Arabia 2027</span>
          </div>
        </h2>
        <div class="flex items-center gap-3">
          <span class="px-3 py-1.5 rounded-full bg-teal-100 text-teal-700 text-sm font-semibold">{{ currentSlide + 1 }} / {{ recentUpdates.length }}</span>
          <button @click="toggleAutoPlay" class="p-2.5 rounded-xl bg-white border border-gray-200 hover:bg-teal-50 hover:border-teal-200 text-gray-500 hover:text-teal-600 transition-all shadow-sm">
            <i :class="isAutoPlaying ? 'fas fa-pause' : 'fas fa-play'" class="text-sm"></i>
          </button>
        </div>
      </div>

      <!-- Progress bar -->
      <div class="h-1.5 bg-gray-100 rounded-full mb-6 overflow-hidden">
        <div class="slide-progress rounded-full" :key="currentSlide" v-if="isAutoPlaying"></div>
      </div>

      <div class="updates-carousel">
        <!-- Navigation Buttons -->
        <button @click="prevSlide" class="carousel-nav-btn prev">
          <i class="fas fa-chevron-left"></i>
        </button>
        <button @click="nextSlide" class="carousel-nav-btn next">
          <i class="fas fa-chevron-right"></i>
        </button>

        <!-- Slides -->
        <div class="carousel-track" :style="{ transform: 'translateX(-' + (currentSlide * 100) + '%)' }">
          <div v-for="(update, index) in recentUpdates" :key="update.id"
               class="carousel-slide"
               :class="{ active: index === currentSlide }">
            <div class="slide-content">
              <!-- Image Side -->
              <div class="slide-image">
                <div class="relative h-64 lg:h-80 rounded-2xl overflow-hidden shadow-lg group">
                  <img v-if="update.image" :src="update.image" :alt="update.title" class="absolute inset-0 w-full h-full object-cover transition-transform duration-500 group-hover:scale-105">
                  <div v-else class="absolute inset-0" :class="update.gradientClass">
                    <div class="absolute inset-0 flex items-center justify-center">
                      <div class="w-20 h-20 rounded-2xl bg-white/20 backdrop-blur-sm flex items-center justify-center">
                        <i :class="[update.icon, 'text-4xl text-white']"></i>
                      </div>
                    </div>
                  </div>
                  <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-black/20 to-transparent"></div>
                  <div class="absolute top-4 left-4">
                    <span :class="['update-badge', update.type]" class="text-sm px-4 py-1.5 shadow-lg">
                      <i :class="update.typeIcon" class="mr-1"></i>
                      {{ update.typeLabel }}
                    </span>
                  </div>
                  <div class="absolute bottom-4 left-4 right-4 flex justify-between items-center">
                    <div class="flex items-center gap-2 text-white/90 text-sm">
                      <i class="fas fa-eye"></i>
                      <span>{{ update.views }} views</span>
                    </div>
                    <div class="px-3 py-1.5 rounded-lg bg-black/40 backdrop-blur-sm text-white text-sm font-medium">
                      <i class="fas fa-calendar-alt mr-1"></i>
                      {{ update.date }}
                    </div>
                  </div>
                </div>
              </div>

              <!-- Content Side -->
              <div class="slide-text">
                <h3 class="text-2xl lg:text-3xl font-bold text-gray-900 mb-4 leading-tight">{{ update.title }}</h3>
                <p class="text-gray-600 mb-6 text-base leading-relaxed line-clamp-3">{{ update.description }}</p>
                <div class="flex items-center gap-6 mb-6 pb-6 border-b border-gray-100">
                  <div class="flex items-center gap-3">
                    <div class="w-10 h-10 rounded-xl flex items-center justify-center text-white text-sm font-bold shadow-md"
                         :style="{ backgroundColor: update.author.color }">
                      {{ update.author.initials }}
                    </div>
                    <div>
                      <p class="text-sm font-semibold text-gray-900">{{ update.author.name }}</p>
                      <p class="text-xs text-gray-500">{{ update.author.role }}</p>
                    </div>
                  </div>
                  <div class="flex items-center gap-4 text-sm text-gray-500">
                    <span class="flex items-center gap-1.5"><i class="fas fa-comment text-teal-500"></i>{{ update.comments }} comments</span>
                  </div>
                </div>
                <div class="flex gap-4">
                  <router-link :to="update.link" class="px-6 py-3 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-xl font-semibold text-sm flex items-center gap-2 shadow-lg shadow-teal-200 hover:shadow-xl hover:shadow-teal-300 transition-all hover:-translate-y-0.5">
                    {{ update.actionText }}
                    <i class="fas fa-arrow-right text-xs"></i>
                  </router-link>
                  <button class="px-4 py-3 bg-white border-2 border-gray-200 text-gray-700 rounded-xl font-medium text-sm hover:bg-teal-50 hover:border-teal-200 hover:text-teal-700 transition-all flex items-center gap-2">
                    <i class="fas fa-bookmark"></i>
                    Save
                  </button>
                  <button class="px-4 py-3 bg-white border-2 border-gray-200 text-gray-700 rounded-xl font-medium text-sm hover:bg-teal-50 hover:border-teal-200 hover:text-teal-700 transition-all flex items-center gap-2">
                    <i class="fas fa-share-alt"></i>
                    Share
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Dots Navigation -->
      <div class="carousel-dots">
        <button v-for="(update, index) in recentUpdates" :key="'dot-' + index"
                @click="goToSlide(index)"
                class="carousel-dot"
                :class="{ active: index === currentSlide }">
        </button>
      </div>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5 mb-8">
      <div v-for="(stat, index) in stats" :key="stat.label"
           class="stat-card card-animated rounded-2xl p-5"
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
        <div class="stat-value text-3xl font-bold text-gray-900">{{ formatNumber(stat.displayValue) }}</div>
        <div class="text-sm text-gray-500 mt-1">{{ stat.label }}</div>
        <router-link :to="stat.link" class="text-xs text-primary-600 font-medium mt-3 flex items-center gap-1 hover:text-primary-700">
          {{ stat.linkText }} <i class="fas fa-arrow-right text-[10px]"></i>
        </router-link>
      </div>
    </div>

    <!-- Main Grid Row 1 - Articles & Documents -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
      <!-- Recent Articles -->
      <div class="card-animated rounded-2xl p-6 stagger-2 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
            <div class="w-11 h-11 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-newspaper text-white"></i>
            </div>
            <div>
              <span class="block">Recent Articles</span>
              <span class="text-xs font-medium text-teal-600">Latest news & updates</span>
            </div>
          </h2>
          <router-link to="/articles" class="px-3 py-1.5 text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1.5 transition-all">
            View All <i class="fas fa-arrow-right text-xs"></i>
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
              <div v-if="index === 0" class="absolute -top-1 -right-1 w-4 h-4 bg-red-500 rounded-full flex items-center justify-center">
                <span class="text-white text-[6px] font-bold">NEW</span>
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
                <span class="text-[10px] text-gray-400"><i class="fas fa-eye mr-0.5"></i>{{ article.views }}</span>
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
      <div class="card-animated rounded-2xl p-6 stagger-3 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
            <div class="w-11 h-11 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-folder-open text-white"></i>
            </div>
            <div>
              <span class="block">Recent Documents</span>
              <span class="text-xs font-medium text-teal-600">Files & resources</span>
            </div>
          </h2>
          <router-link to="/documents" class="px-3 py-1.5 text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1.5 transition-all">
            View All <i class="fas fa-arrow-right text-xs"></i>
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
              <div v-if="index === 0" class="absolute -top-1 -right-1 w-4 h-4 bg-teal-500 rounded-full flex items-center justify-center">
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
        <button class="w-full mt-4 py-3 text-sm font-semibold text-teal-600 bg-teal-50 hover:bg-teal-100 border-2 border-dashed border-teal-200 hover:border-teal-300 rounded-xl transition-all flex items-center justify-center gap-2">
          <i class="fas fa-cloud-upload-alt"></i>
          Upload New Document
        </button>
      </div>
    </div>

    <!-- Recent Media - Enhanced Section -->
    <div class="card-animated rounded-2xl p-5 mb-6 stagger-2 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
      <div class="flex items-center justify-between mb-4">
        <h2 class="text-base font-bold text-gray-900 flex items-center gap-3">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
            <i class="fas fa-photo-film text-white text-sm"></i>
          </div>
          <div>
            <span class="block">Recent Media</span>
            <span class="text-xs font-medium text-teal-600">Videos & podcasts</span>
          </div>
        </h2>
        <router-link to="/media" class="px-3 py-1.5 text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1.5 transition-all">
          View All <i class="fas fa-arrow-right text-xs"></i>
        </router-link>
      </div>
      <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-3">
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
                <i :class="media.type === 'audio' ? 'fas fa-headphones text-violet-600' : 'fas fa-play text-teal-600 ml-0.5'" class="text-sm"></i>
              </div>
            </div>

            <!-- Duration Badge -->
            <div class="absolute bottom-1.5 right-1.5 px-1.5 py-0.5 rounded bg-black/80 text-white text-[10px] font-semibold backdrop-blur-sm">
              {{ media.duration }}
            </div>

            <!-- Type Badge -->
            <div v-if="media.type === 'audio'" class="absolute top-1.5 left-1.5 px-1.5 py-0.5 rounded-full bg-violet-500 text-white text-[9px] font-semibold flex items-center gap-1">
              <i class="fas fa-podcast"></i> Audio
            </div>
            <div v-else-if="media.progress === 100" class="absolute top-1.5 left-1.5 px-1.5 py-0.5 rounded-full bg-green-500 text-white text-[9px] font-semibold flex items-center gap-1">
              <i class="fas fa-check"></i> Watched
            </div>

            <!-- Progress Bar -->
            <div v-if="media.progress > 0 && media.progress < 100" class="absolute bottom-0 left-0 right-0 h-1 bg-black/30">
              <div class="h-full bg-teal-500" :style="{ width: media.progress + '%' }"></div>
            </div>

            <!-- Save Button -->
            <button @click="saveMedia(media.id, $event)"
                    :class="['absolute top-1.5 right-1.5 w-6 h-6 rounded-full flex items-center justify-center transition-all backdrop-blur-sm', isMediaSaved(media.id) ? 'bg-teal-500 opacity-100' : 'bg-black/50 hover:bg-teal-500 opacity-0 group-hover:opacity-100']">
              <i :class="[isMediaSaved(media.id) ? 'fas' : 'far', 'fa-bookmark text-white text-[10px]']"></i>
            </button>
          </div>

          <!-- Content -->
          <div class="p-2.5">
            <div class="flex items-center gap-1.5 mb-1">
              <span class="text-[9px] font-semibold px-1.5 py-0.5 rounded-full bg-teal-100 text-teal-600">{{ media.category }}</span>
            </div>
            <h4 class="font-semibold text-gray-900 text-xs leading-tight line-clamp-2 group-hover:text-teal-600 transition-colors mb-1">{{ media.title }}</h4>
            <div class="flex items-center gap-2 text-[10px] text-gray-400">
              <span><i class="fas fa-eye mr-0.5"></i>{{ media.views }}</span>
              <span>•</span>
              <span>{{ media.date }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Grid Row 2 -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
      <!-- Events - Enhanced -->
      <div class="card-animated rounded-2xl p-6 stagger-3 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-calendar-alt text-white text-sm"></i>
            </div>
            <div>
              <span class="block">Upcoming Events</span>
              <span class="text-xs font-medium text-teal-600">AFC Asian Cup 2027</span>
            </div>
          </h2>
          <router-link to="/events" class="px-3 py-1.5 text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1.5 transition-all">
            View All <i class="fas fa-arrow-right text-xs"></i>
          </router-link>
        </div>

        <div class="space-y-3">
          <div v-for="event in upcomingEvents" :key="event.id"
               @click="viewEvent(event.id)"
               class="event-card relative rounded-xl cursor-pointer border border-gray-100 hover:border-teal-200 hover:shadow-lg transition-all group overflow-hidden"
               :class="event.isFeatured ? 'bg-gradient-to-r from-teal-50 to-emerald-50' : 'bg-white'">

            <!-- Featured Badge -->
            <div v-if="event.isFeatured" class="absolute top-0 right-0">
              <div class="bg-gradient-to-r from-amber-400 to-orange-500 text-white text-[9px] font-bold px-2 py-0.5 rounded-bl-lg">
                <i class="fas fa-star mr-0.5"></i> FEATURED
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
                           class="w-5 h-5 rounded-full border-2 border-white flex items-center justify-center text-[7px] font-bold text-white transition-transform hover:scale-110 hover:z-10"
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
      <div class="card-animated rounded-2xl p-6 stagger-4 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-chart-pie text-white text-sm"></i>
            </div>
            <div>
              <span class="block">Active Polls</span>
              <span class="text-xs font-medium text-teal-600">Cast your vote</span>
            </div>
          </h2>
          <router-link to="/polls" class="px-3 py-1.5 text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1.5 transition-all">
            View All <i class="fas fa-arrow-right text-xs"></i>
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
                  <span class="text-xs text-gray-500">{{ poll.totalVotes.toLocaleString() }} votes</span>
                  <span class="w-1 h-1 rounded-full bg-gray-300"></span>
                  <span class="text-xs text-amber-600 font-medium"><i class="fas fa-clock mr-1"></i>{{ poll.endsIn }}</span>
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
                    <span class="text-sm font-bold min-w-[40px] text-right" :style="{ color: option.color || '#14b8a6' }">{{ option.votes }}%</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Poll Footer -->
            <div class="flex items-center justify-between mt-4 pt-4 border-t border-gray-100">
              <div v-if="poll.hasVoted" class="flex items-center gap-1.5 text-xs text-green-600 font-medium">
                <i class="fas fa-check-circle"></i>
                <span>You voted</span>
              </div>
              <div v-else class="text-xs text-gray-500">
                <i class="fas fa-users mr-1"></i> {{ selectedPollOptions[poll.id] ? 'Ready to vote' : 'Select an option' }}
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
      <div class="card-animated rounded-2xl p-6 stagger-5 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-graduation-cap text-white text-sm"></i>
            </div>
            <div>
              <span class="block">My Learning</span>
              <span class="text-xs font-medium text-teal-600">Continue your journey</span>
            </div>
          </h2>
          <router-link to="/learning" class="px-3 py-1.5 text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1.5 transition-all">
            Browse <i class="fas fa-arrow-right text-xs"></i>
          </router-link>
        </div>

        <!-- Quick Stats -->
        <div class="grid grid-cols-4 gap-2 mb-4">
          <div class="text-center p-2 rounded-lg bg-white border border-gray-100">
            <p class="text-lg font-bold text-teal-600">{{ learningStats.inProgress }}</p>
            <p class="text-[9px] text-gray-500 font-medium">In Progress</p>
          </div>
          <div class="text-center p-2 rounded-lg bg-white border border-gray-100">
            <p class="text-lg font-bold text-green-600">{{ learningStats.completed }}</p>
            <p class="text-[9px] text-gray-500 font-medium">Completed</p>
          </div>
          <div class="text-center p-2 rounded-lg bg-white border border-gray-100">
            <p class="text-lg font-bold text-blue-600">{{ learningStats.totalHours }}</p>
            <p class="text-[9px] text-gray-500 font-medium">Hours</p>
          </div>
          <div class="text-center p-2 rounded-lg bg-white border border-gray-100">
            <p class="text-lg font-bold text-amber-600">{{ learningStats.certificates }}</p>
            <p class="text-[9px] text-gray-500 font-medium">Certificates</p>
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
                  <div class="absolute -bottom-1 -right-1 w-7 h-7 rounded-full bg-white shadow-md flex items-center justify-center">
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
                    <span><i class="fas fa-book-open mr-1 text-teal-400"></i>{{ course.lessonsCompleted }}/{{ course.totalLessons }} lessons</span>
                    <span><i class="fas fa-clock mr-1 text-teal-400"></i>{{ course.duration }}</span>
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
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Team Activity - Enhanced -->
      <div class="card-animated rounded-2xl p-6 stagger-5 bg-gradient-to-br from-white to-teal-50/30 border border-teal-100/50">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-users text-white text-sm"></i>
            </div>
            <div>
              <span class="block">Team Activity</span>
              <span class="text-xs font-medium text-teal-600">Recent updates</span>
            </div>
          </h2>
          <router-link to="/collaboration" class="px-3 py-1.5 text-sm text-teal-600 hover:text-white bg-teal-50 hover:bg-teal-500 rounded-lg font-medium flex items-center gap-1.5 transition-all">
            View All <i class="fas fa-arrow-right text-xs"></i>
          </router-link>
        </div>

        <div class="space-y-3 max-h-[400px] overflow-y-auto pr-1 custom-scrollbar">
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
                <div :class="['absolute -bottom-1 -right-1 w-5 h-5 rounded-full flex items-center justify-center border-2 border-white', activity.actionBg]">
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

      <!-- Quick Self Services -->
      <div class="card-animated rounded-2xl p-6 stagger-6">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-base font-semibold text-gray-900 flex items-center gap-3">
            <div class="icon-soft w-9 h-9 rounded-xl flex items-center justify-center">
              <i class="fas fa-grid-2 text-sm"></i>
            </div>
            Quick Services
          </h2>
          <router-link to="/self-services" class="text-sm text-primary-600 font-medium hover:text-primary-700">View All</router-link>
        </div>
        <div class="grid grid-cols-3 gap-3">
          <button v-for="service in selfServices" :key="service.id"
                  @click="openService(service)"
                  class="flex flex-col items-center gap-2 p-4 rounded-xl bg-gray-50 hover:bg-primary-50 hover:shadow-sm border border-transparent hover:border-teal-200 transition-all group">
            <div :class="['w-10 h-10 rounded-xl flex items-center justify-center transition-transform group-hover:scale-110', service.iconBg]">
              <i :class="[service.icon, service.iconColor]"></i>
            </div>
            <span class="text-xs font-medium text-gray-700 group-hover:text-teal-600 text-center transition-colors">{{ service.label }}</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Carousel Styles */
.updates-carousel {
  position: relative;
  overflow: hidden;
  width: 100%;
}

.carousel-track {
  display: flex;
  width: 100%;
  transition: transform 0.5s ease-in-out;
}

.carousel-slide {
  min-width: 100%;
  width: 100%;
  flex: 0 0 100%;
  display: flex;
}

.slide-content {
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 0.5rem;
  width: 100%;
}

@media (min-width: 1024px) {
  .slide-content {
    flex-direction: row;
  }
}

.slide-image {
  width: 100%;
}

@media (min-width: 1024px) {
  .slide-image {
    width: 40%;
    flex-shrink: 0;
  }
}

.slide-text {
  display: flex;
  flex-direction: column;
  justify-content: center;
  padding: 1rem 0;
  width: 100%;
}

@media (min-width: 1024px) {
  .slide-text {
    width: 60%;
  }
}

.carousel-nav-btn {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(20, 184, 166, 0.2);
  color: #0d9488;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  z-index: 10;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
}

.carousel-nav-btn:hover {
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
  transform: translateY(-50%) scale(1.1);
  box-shadow: 0 8px 25px rgba(20, 184, 166, 0.4);
}

.carousel-nav-btn.prev {
  left: 16px;
}

.carousel-nav-btn.next {
  right: 16px;
}

.carousel-dots {
  display: flex;
  justify-content: center;
  gap: 8px;
  margin-top: 16px;
}

.carousel-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #d1d5db;
  border: none;
  cursor: pointer;
  transition: all 0.3s ease;
  padding: 0;
}

.carousel-dot:hover {
  background: #99f6e4;
  transform: scale(1.2);
}

.carousel-dot.active {
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  width: 24px;
  border-radius: 12px;
}

.update-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.update-badge.announcement {
  background: linear-gradient(135deg, #fef3c7, #fde68a);
  color: #92400e;
}

.update-badge.article {
  background: linear-gradient(135deg, #dbeafe, #bfdbfe);
  color: #1e40af;
}

.update-badge.event {
  background: linear-gradient(135deg, #d1fae5, #a7f3d0);
  color: #065f46;
}

.update-badge.course {
  background: linear-gradient(135deg, #ede9fe, #ddd6fe);
  color: #5b21b6;
}

.update-badge.policy {
  background: linear-gradient(135deg, #fee2e2, #fecaca);
  color: #991b1b;
}

.update-badge.venue {
  background: linear-gradient(135deg, #ede9fe, #ddd6fe);
  color: #5b21b6;
}

.update-badge.stats {
  background: linear-gradient(135deg, #fee2e2, #fecaca);
  color: #991b1b;
}

.slide-progress {
  height: 100%;
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  animation: slideProgress 5s linear;
}

@keyframes slideProgress {
  from { width: 0%; }
  to { width: 100%; }
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
