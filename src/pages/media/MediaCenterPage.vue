<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import ContentActionsDropdown from '@/components/common/ContentActionsDropdown.vue'
import AddToCollectionModal from '@/components/common/AddToCollectionModal.vue'
import { useAIServicesStore } from '@/stores/aiServices'
import { AILoadingIndicator, AIConfidenceBar, AISuggestionChip } from '@/components/ai'
import type { AutoTagResult, OCRResult, ClassificationResult } from '@/types/ai'

const router = useRouter()
const aiStore = useAIServicesStore()

// State
const showUploadModal = ref(false)
const isDragging = ref(false)
const activeTab = ref('all')
const searchQuery = ref('')
const unifiedSearchQuery = ref('')
const isAISearchMode = ref(false)
const showAISuggestions = ref(false)
const categoryFilter = ref('')
const sortBy = ref('recent')
const sortOrder = ref<'asc' | 'desc'>('desc')
const currentPage = ref(1)
const itemsPerPage = ref(6)
const showFilters = ref(false)
const trendingScrollRef = ref<HTMLElement | null>(null)
const likedMedia = ref(new Set<number>())
const savedMedia = ref(new Set<number>())
const viewMode = ref('grid')

// Sidebar state
const isSidebarCollapsed = ref(false)
const expandedFolders = ref(new Set<string>(['root', 'tournament', 'media-assets']))
const selectedFolder = ref<string | null>(null)
const currentView = ref<'all' | 'recent' | 'popular' | 'saved' | 'trash'>('all')

// Selection mode
const isSelectionMode = ref(false)
const selectedMedia = ref(new Set<number>())

// Add to Collection modal
const showAddToCollectionModal = ref(false)
const selectedItemForCollection = ref<{
  id: number
  title: string
  thumbnail?: string
} | null>(null)

// Dropdown filter states
const showMediaTypeFilter = ref(false)
const showCategoryFilter = ref(false)
const showTagFilter = ref(false)
const showSortDropdown = ref(false)
const showStatusFilter = ref(false)
const selectedMediaTypes = ref<string[]>([])
const selectedCategories = ref<string[]>([])
const selectedTags = ref<string[]>([])
const selectedStatusFilters = ref<string[]>([])

// Status filter options
const statusFilterOptions = [
  { id: 'saved', label: 'My Saved', icon: 'fas fa-bookmark', color: 'text-amber-500' },
  { id: 'shared', label: 'Shared with me', icon: 'fas fa-share-alt', color: 'text-purple-500' }
]

// Sort options with icons
const sortOptionsList = ref([
  { value: 'recent', label: 'Date Modified', icon: 'fas fa-clock' },
  { value: 'name', label: 'Name', icon: 'fas fa-font' },
  { value: 'popular', label: 'Most Popular', icon: 'fas fa-fire' },
  { value: 'views', label: 'Most Viewed', icon: 'fas fa-eye' },
  { value: 'duration', label: 'Duration', icon: 'fas fa-hourglass-half' }
])

// View navigation items
const viewNavItems = ref([
  { id: 'all', name: 'All Media', icon: 'fas fa-photo-film', color: 'text-teal-500' },
  { id: 'recent', name: 'Recently Viewed', icon: 'fas fa-clock', color: 'text-blue-500' },
  { id: 'popular', name: 'Most Popular', icon: 'fas fa-fire', color: 'text-orange-500' },
  { id: 'saved', name: 'Saved', icon: 'fas fa-bookmark', color: 'text-amber-500' },
  { id: 'trash', name: 'Trash', icon: 'fas fa-trash-alt', color: 'text-red-500' }
])

// Folder tree structure
const folderTree = ref([
  {
    id: 'root',
    name: 'AFC Asian Cup 2027',
    icon: 'fas fa-trophy',
    children: [
      {
        id: 'tournament',
        name: 'Tournament Coverage',
        icon: 'fas fa-folder',
        children: [
          { id: 'highlights', name: 'Match Highlights', icon: 'fas fa-folder', children: [] },
          { id: 'ceremonies', name: 'Ceremonies', icon: 'fas fa-folder', children: [] },
          { id: 'interviews', name: 'Interviews', icon: 'fas fa-folder', children: [] }
        ]
      },
      {
        id: 'media-assets',
        name: 'Media Assets',
        icon: 'fas fa-folder',
        children: [
          { id: 'photos', name: 'Photos', icon: 'fas fa-folder', children: [] },
          { id: 'videos', name: 'Videos', icon: 'fas fa-folder', children: [] },
          { id: 'audio', name: 'Audio & Podcasts', icon: 'fas fa-folder', children: [] }
        ]
      },
      {
        id: 'teams',
        name: 'Teams',
        icon: 'fas fa-folder',
        children: [
          { id: 'team-profiles', name: 'Team Profiles', icon: 'fas fa-folder', children: [] },
          { id: 'player-content', name: 'Player Content', icon: 'fas fa-folder', children: [] }
        ]
      },
      {
        id: 'venues',
        name: 'Venues & Stadiums',
        icon: 'fas fa-folder',
        children: []
      },
      {
        id: 'fan-zone',
        name: 'Fan Zone',
        icon: 'fas fa-folder',
        children: []
      }
    ]
  }
])

// Media type filter options
const mediaTypeOptions = ref([
  { id: 'video', label: 'Videos', icon: 'fas fa-video', color: 'text-blue-500' },
  { id: 'audio', label: 'Audio', icon: 'fas fa-headphones', color: 'text-purple-500' },
  { id: 'image', label: 'Images', icon: 'fas fa-image', color: 'text-green-500' },
  { id: 'gallery', label: 'Galleries', icon: 'fas fa-images', color: 'text-amber-500' }
])

// Sort Options
const sortOptions = ref([
  { value: 'recent', label: 'Recent', icon: 'fas fa-clock' },
  { value: 'popular', label: 'Popular', icon: 'fas fa-fire' },
  { value: 'duration', label: 'Duration', icon: 'fas fa-hourglass-half' }
])

// Media Tabs
const mediaTabs = ref([
  { id: 'all', label: 'All', icon: 'fas fa-th-large' },
  { id: 'video', label: 'Videos', icon: 'fas fa-video' },
  { id: 'audio', label: 'Audio', icon: 'fas fa-headphones' },
  { id: 'image', label: 'Images', icon: 'fas fa-image' },
  { id: 'gallery', label: 'Galleries', icon: 'fas fa-images' },
])

// Categories
const categories = ref(['Highlights', 'Interviews', 'Behind the Scenes', 'Matches', 'Teams', 'Venues', 'Fans'])

// Date utility functions
function formatRelativeDate(dateInput: Date | string): string {
  const date = typeof dateInput === 'string' ? new Date(dateInput) : dateInput
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMs / 3600000)
  const diffDays = Math.floor(diffMs / 86400000)
  const diffWeeks = Math.floor(diffDays / 7)

  if (diffMins < 1) return 'Just now'
  if (diffMins < 60) return `${diffMins} min ago`
  if (diffHours < 24) return diffHours === 1 ? '1 hour ago' : `${diffHours} hours ago`
  if (diffDays < 7) return diffDays === 1 ? 'Yesterday' : `${diffDays} days ago`
  if (diffWeeks < 4) return diffWeeks === 1 ? '1 week ago' : `${diffWeeks} weeks ago`
  return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
}

function isNewContent(dateInput: Date | string, daysThreshold = 7): boolean {
  const date = typeof dateInput === 'string' ? new Date(dateInput) : dateInput
  const now = new Date()
  const diffDays = Math.floor((now.getTime() - date.getTime()) / 86400000)
  return diffDays <= daysThreshold
}

function generateDateDaysAgo(days: number): Date {
  const date = new Date()
  date.setDate(date.getDate() - days)
  return date
}

// Watch History (Continue Watching) - Using actual dates
const watchHistoryData = [
  {
    id: 1,
    title: 'AFC Asian Cup 2027: Tournament Preview',
    progress: 65,
    durationSeconds: 1935, // 32:15
    thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400&h=225&fit=crop',
    category: 'Tournament',
    type: 'video',
    author: 'AFC Media Team',
    lastWatchedDate: generateDateDaysAgo(0), // Today, 2 hours ago
    viewCount: 2400
  },
  {
    id: 2,
    title: 'Stadium Tour: King Fahd International',
    progress: 30,
    durationSeconds: 1122, // 18:42
    thumbnail: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=400&h=225&fit=crop',
    category: 'Venues',
    type: 'video',
    author: 'Sports Network',
    lastWatchedDate: generateDateDaysAgo(1), // Yesterday
    viewCount: 1200
  },
  {
    id: 3,
    title: 'Top 10 Goals from Qualifiers',
    progress: 80,
    durationSeconds: 920, // 15:20
    thumbnail: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=400&h=225&fit=crop',
    category: 'Highlights',
    type: 'video',
    author: 'AFC Highlights',
    lastWatchedDate: generateDateDaysAgo(3),
    viewCount: 890
  },
  {
    id: 4,
    title: 'Team Japan: Road to 2027',
    progress: 45,
    durationSeconds: 2700, // 45:00
    thumbnail: 'https://images.unsplash.com/photo-1489944440615-453fc2b6a9a9?w=400&h=225&fit=crop',
    category: 'Teams',
    type: 'video',
    author: 'Sports Documentary',
    lastWatchedDate: generateDateDaysAgo(7),
    viewCount: 1800
  },
  {
    id: 5,
    title: 'Opening Ceremony Preparations',
    progress: 12,
    durationSeconds: 2295, // 38:15
    thumbnail: 'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=400&h=225&fit=crop',
    category: 'Events',
    type: 'video',
    author: 'Event Coverage',
    lastWatchedDate: generateDateDaysAgo(14),
    viewCount: 956
  },
]

// Computed watch history with formatted values
const watchHistory = computed(() => {
  return watchHistoryData.map(item => {
    const totalSeconds = item.durationSeconds
    const watchedSeconds = Math.floor(totalSeconds * (item.progress / 100))
    const remainingSeconds = totalSeconds - watchedSeconds
    const remainingMinutes = Math.ceil(remainingSeconds / 60)

    return {
      ...item,
      duration: formatDuration(totalSeconds),
      timeRemaining: `${remainingMinutes} min left`,
      lastWatched: formatRelativeDate(item.lastWatchedDate),
      views: formatViewCount(item.viewCount)
    }
  })
})

function formatDuration(seconds: number): string {
  const mins = Math.floor(seconds / 60)
  const secs = seconds % 60
  return `${mins}:${secs.toString().padStart(2, '0')}`
}

function formatViewCount(count: number): string {
  if (count >= 1000) {
    return (count / 1000).toFixed(1).replace(/\.0$/, '') + 'K'
  }
  return count.toString()
}

// Category Cards - Base definitions (counts will be computed)
const categoryCardDefs = [
  { id: 'highlights', name: 'Highlights', icon: 'fas fa-star', gradient: 'cat-training', category: 'Highlights' },
  { id: 'interviews', name: 'Interviews', icon: 'fas fa-microphone', gradient: 'cat-product', category: 'Interviews' },
  { id: 'behind', name: 'Behind the Scenes', icon: 'fas fa-film', gradient: 'cat-leadership', category: 'Behind the Scenes' },
  { id: 'teams', name: 'Teams', icon: 'fas fa-users', gradient: 'cat-technical', category: 'Teams' },
  { id: 'venues', name: 'Venues', icon: 'fas fa-building', gradient: 'cat-culture', category: 'Venues' },
  { id: 'fans', name: 'Fans', icon: 'fas fa-heart', gradient: 'cat-news', category: 'Fans' },
]

// Featured Videos Carousel - Using actual dates
const featuredVideosData = [
  {
    id: 1,
    title: 'AFC Asian Cup 2027: Tournament Preview',
    description: 'Get an exclusive behind-the-scenes look at the preparations for the biggest football event in Asia.',
    durationSeconds: 1935,
    viewCount: 24500,
    thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=1200&h=675&fit=crop',
    category: 'Tournament',
    author: 'AFC Media Team',
    publishedDate: generateDateDaysAgo(2),
    likes: 1250,
    comments: 89
  },
  {
    id: 2,
    title: 'Opening Ceremony Preparations Unveiled',
    description: 'A sneak peek at the spectacular opening ceremony planned for the tournament.',
    durationSeconds: 1122,
    viewCount: 18200,
    thumbnail: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=1200&h=675&fit=crop',
    category: 'Events',
    author: 'Sports Network',
    publishedDate: generateDateDaysAgo(3),
    likes: 890,
    comments: 56
  },
  {
    id: 3,
    title: 'Top 10 Goals from Qualifying Rounds',
    description: 'Relive the most spectacular goals scored during the qualification matches.',
    durationSeconds: 750,
    viewCount: 45800,
    thumbnail: 'https://images.unsplash.com/photo-1551958219-acbc608c6377?w=1200&h=675&fit=crop',
    category: 'Highlights',
    author: 'AFC Media Team',
    publishedDate: generateDateDaysAgo(7),
    likes: 2340,
    comments: 178
  },
  {
    id: 4,
    title: 'Meet the Teams: Group Stage Draw Analysis',
    description: 'Expert analysis of the group stage draw and predictions for the tournament.',
    durationSeconds: 1695,
    viewCount: 12100,
    thumbnail: 'https://images.unsplash.com/photo-1489944440615-453fc2b6a9a9?w=1200&h=675&fit=crop',
    category: 'Analysis',
    author: 'Football Experts',
    publishedDate: generateDateDaysAgo(5),
    likes: 567,
    comments: 42
  }
]

// Computed featured videos with formatted values
const featuredVideos = computed(() => {
  return featuredVideosData.map(item => ({
    ...item,
    duration: formatDuration(item.durationSeconds),
    views: formatViewCount(item.viewCount),
    date: formatRelativeDate(item.publishedDate)
  }))
})

const currentFeaturedIndex = ref(0)
const featuredVideo = computed(() => featuredVideos.value[currentFeaturedIndex.value])
const carouselInterval = ref<number | null>(null)
const autoPlayDelay = 5000 // 5 seconds

// Carousel Navigation
function nextFeatured() {
  currentFeaturedIndex.value = (currentFeaturedIndex.value + 1) % featuredVideos.value.length
}

function prevFeatured() {
  currentFeaturedIndex.value = (currentFeaturedIndex.value - 1 + featuredVideos.value.length) % featuredVideos.value.length
}

function goToFeatured(index: number) {
  currentFeaturedIndex.value = index
}

// Auto-play functions
function startAutoPlay() {
  if (carouselInterval.value) return
  carouselInterval.value = window.setInterval(() => {
    nextFeatured()
  }, autoPlayDelay)
}

function stopAutoPlay() {
  if (carouselInterval.value) {
    clearInterval(carouselInterval.value)
    carouselInterval.value = null
  }
}

function pauseAutoPlay() {
  stopAutoPlay()
}

function resumeAutoPlay() {
  startAutoPlay()
}

// Up Next Videos - Computed from featured videos (excluding current)
const upNextVideos = computed(() => {
  return featuredVideos.value
    .filter((_, index) => index !== currentFeaturedIndex.value)
    .slice(0, 4)
})

// Media Items Raw Data - with actual dates (reactive for mutations)
const mediaItemsData = ref([
  { id: 5, title: 'Saudi Arabia vs Japan: Opening Match Preview', type: 'video', category: 'Highlights', folderId: 'highlights', duration: '12:45', viewCount: 45200, publishedDate: generateDateDaysAgo(2), thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400&h=225&fit=crop', hasTranscript: true, author: 'AFC Media', tags: ['Saudi Arabia', 'Japan', 'Group A', 'Preview'], sharedWithMe: true, sharedBy: 'John Smith' },
  { id: 6, title: 'King Fahd Stadium: Behind the Scenes Tour', type: 'video', category: 'Behind the Scenes', folderId: 'venues', duration: '18:30', viewCount: 32100, publishedDate: generateDateDaysAgo(3), thumbnail: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=400&h=225&fit=crop', hasTranscript: true, author: 'Venue Team', tags: ['Stadium', 'Riyadh', 'Exclusive'], sharedWithMe: false },
  { id: 13, title: 'AFC Asian Cup 2027 Official Draw Ceremony', type: 'gallery', category: 'Highlights', folderId: 'ceremonies', photoCount: 48, viewCount: 28500, publishedDate: generateDateDaysAgo(1), thumbnail: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=400&h=225&fit=crop', author: 'AFC Media', tags: ['Draw', 'Official', 'Ceremony'], sharedWithMe: true, sharedBy: 'Sarah Wilson' },
  { id: 18, title: 'AFC Asian Cup Official Podcast: Episode 1', type: 'audio', category: 'Interviews', folderId: 'audio', duration: '45:30', viewCount: 12300, publishedDate: generateDateDaysAgo(2), thumbnail: 'https://images.unsplash.com/photo-1478737270239-2f02b77fc618?w=400&h=225&fit=crop', hasTranscript: true, author: 'AFC Media', tags: ['Podcast', 'Official', 'Analysis'], sharedWithMe: false },
  { id: 19, title: 'King Fahd Stadium Aerial View', type: 'image', category: 'Venues', folderId: 'venues', viewCount: 18500, publishedDate: generateDateDaysAgo(1), thumbnail: 'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=400&h=225&fit=crop', author: 'AFC Media', tags: ['Stadium', 'Riyadh', 'Aerial'], sharedWithMe: false },
  { id: 7, title: 'Coach Interview: Saudi Arabia Tactics Analysis', type: 'video', category: 'Interviews', folderId: 'interviews', duration: '25:15', viewCount: 18700, publishedDate: generateDateDaysAgo(7), thumbnail: 'https://images.unsplash.com/photo-1551958219-acbc608c6377?w=400&h=225&fit=crop', hasTranscript: true, author: 'Sports Desk', tags: ['Saudi Arabia', 'Coach', 'Tactics'], sharedWithMe: true, sharedBy: 'Mike Johnson' },
  { id: 20, title: 'Match Day Commentary: Live Analysis', type: 'audio', category: 'Matches', folderId: 'audio', duration: '90:00', viewCount: 8700, publishedDate: generateDateDaysAgo(4), thumbnail: 'https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?w=400&h=225&fit=crop', author: 'AFC Radio', tags: ['Commentary', 'Live', 'Analysis'], sharedWithMe: false },
  { id: 21, title: 'Saudi Arabia Team Photo 2027', type: 'image', category: 'Teams', folderId: 'team-profiles', viewCount: 35200, publishedDate: generateDateDaysAgo(3), thumbnail: 'https://images.unsplash.com/photo-1517466787929-bc90951d0974?w=400&h=225&fit=crop', author: 'AFC Media', tags: ['Saudi Arabia', 'Team', 'Official'], sharedWithMe: false },
  { id: 14, title: 'Fan Zone Setup: Riyadh Boulevard', type: 'gallery', category: 'Fans', folderId: 'fan-zone', photoCount: 32, viewCount: 15300, publishedDate: generateDateDaysAgo(3), thumbnail: 'https://images.unsplash.com/photo-1560272564-c83b66b1ad12?w=400&h=225&fit=crop', author: 'Events Team', tags: ['Fans', 'Riyadh', 'Fan Zone'], sharedWithMe: false },
  { id: 8, title: 'Top 10 Goals: AFC Asian Cup History', type: 'video', category: 'Highlights', folderId: 'highlights', duration: '15:20', viewCount: 89400, publishedDate: generateDateDaysAgo(7), thumbnail: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=400&h=225&fit=crop', hasTranscript: true, author: 'AFC Media', tags: ['Goals', 'History', 'Top 10'], sharedWithMe: false },
  { id: 22, title: 'Trophy Unveiling Ceremony', type: 'image', category: 'Highlights', folderId: 'ceremonies', viewCount: 42100, publishedDate: generateDateDaysAgo(7), thumbnail: 'https://images.unsplash.com/photo-1579952363873-27f3bade9f55?w=400&h=225&fit=crop', author: 'AFC Media', tags: ['Trophy', 'Ceremony', 'Official'], sharedWithMe: true, sharedBy: 'Emily Davis' },
  { id: 9, title: 'Team Japan: Road to Saudi Arabia 2027', type: 'video', category: 'Teams', folderId: 'team-profiles', duration: '22:10', viewCount: 41200, publishedDate: generateDateDaysAgo(14), thumbnail: 'https://images.unsplash.com/photo-1606925797300-0b35e9d1794e?w=400&h=225&fit=crop', hasTranscript: true, author: 'AFC Media', tags: ['Japan', 'Team', 'Documentary'], sharedWithMe: false },
  { id: 23, title: 'Pre-Match Analysis Podcast: Group A', type: 'audio', category: 'Matches', folderId: 'audio', duration: '32:15', viewCount: 6400, publishedDate: generateDateDaysAgo(7), thumbnail: 'https://images.unsplash.com/photo-1508098682722-e99c43a406b2?w=400&h=225&fit=crop', author: 'AFC Analysts', tags: ['Podcast', 'Group A', 'Analysis'], sharedWithMe: false },
  { id: 15, title: 'Volunteer Training Program Launch', type: 'gallery', category: 'Behind the Scenes', folderId: 'photos', photoCount: 65, viewCount: 12800, publishedDate: generateDateDaysAgo(7), thumbnail: 'https://images.unsplash.com/photo-1517466787929-bc90951d0974?w=400&h=225&fit=crop', author: 'LOC Team', tags: ['Volunteers', 'Training', 'Behind the Scenes'], sharedWithMe: false },
  { id: 10, title: 'Salem Al-Dawsari: Exclusive Interview', type: 'video', category: 'Interviews', folderId: 'interviews', duration: '18:30', viewCount: 67300, publishedDate: generateDateDaysAgo(14), thumbnail: 'https://images.unsplash.com/photo-1579952363873-27f3bade9f55?w=400&h=225&fit=crop', hasTranscript: false, author: 'Sports Desk', tags: ['Saudi Arabia', 'Player', 'Exclusive'], sharedWithMe: false },
  { id: 24, title: 'Fans Celebrating at Boulevard', type: 'image', category: 'Fans', folderId: 'fan-zone', viewCount: 28900, publishedDate: generateDateDaysAgo(14), thumbnail: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=400&h=225&fit=crop', author: 'Events Team', tags: ['Fans', 'Celebration', 'Riyadh'], sharedWithMe: false },
  { id: 16, title: 'Stadium Infrastructure Progress Photos', type: 'gallery', category: 'Venues', folderId: 'venues', photoCount: 28, viewCount: 9500, publishedDate: generateDateDaysAgo(14), thumbnail: 'https://images.unsplash.com/photo-1489944440615-453fc2b6a9a9?w=400&h=225&fit=crop', author: 'Venue Operations', tags: ['Stadium', 'Infrastructure', 'Progress'], sharedWithMe: false },
  { id: 11, title: 'Group Stage Draw Analysis', type: 'video', category: 'Highlights', folderId: 'highlights', duration: '35:00', viewCount: 52100, publishedDate: generateDateDaysAgo(21), thumbnail: 'https://images.unsplash.com/photo-1518091043644-c1d4457512c6?w=400&h=225&fit=crop', hasTranscript: true, author: 'AFC Analysts', tags: ['Draw', 'Analysis', 'Groups'], sharedWithMe: false },
  { id: 12, title: 'AFC Podcast: Tournament Predictions', type: 'audio', category: 'Interviews', folderId: 'audio', duration: '48:20', viewCount: 8900, publishedDate: generateDateDaysAgo(21), thumbnail: 'https://images.unsplash.com/photo-1508098682722-e99c43a406b2?w=400&h=225&fit=crop', hasTranscript: false, author: 'AFC Media', tags: ['Podcast', 'Predictions', 'Analysis'], sharedWithMe: false },
  { id: 17, title: 'Media Accreditation Workshop', type: 'gallery', category: 'Behind the Scenes', folderId: 'photos', photoCount: 42, viewCount: 6200, publishedDate: generateDateDaysAgo(21), thumbnail: 'https://images.unsplash.com/photo-1504450758481-7338eba7524a?w=400&h=225&fit=crop', author: 'Media Team', tags: ['Media', 'Workshop', 'Behind the Scenes'], sharedWithMe: false },
  { id: 25, title: 'Player Training Session Highlights', type: 'video', category: 'Teams', folderId: 'player-content', duration: '8:45', viewCount: 22300, publishedDate: generateDateDaysAgo(5), thumbnail: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=400&h=225&fit=crop', hasTranscript: false, author: 'AFC Media', tags: ['Training', 'Players', 'Exclusive'], sharedWithMe: false },
  { id: 26, title: 'Official Tournament Videos Collection', type: 'video', category: 'Highlights', folderId: 'videos', duration: '25:00', viewCount: 15800, publishedDate: generateDateDaysAgo(7), thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400&h=225&fit=crop', hasTranscript: true, author: 'AFC Media', tags: ['Official', 'Tournament', 'Collection'], sharedWithMe: false },
])

// Computed media items with formatted values and computed isNew
const mediaItems = computed(() => {
  return mediaItemsData.value.map(item => ({
    ...item,
    views: formatViewCount(item.viewCount),
    date: formatRelativeDate(item.publishedDate),
    isNew: isNewContent(item.publishedDate)
  }))
})

// Computed Media Stats - derived from mediaItems
const mediaStats = computed(() => ({
  totalVideos: mediaItemsData.value.filter(m => m.type === 'video').length,
  galleries: mediaItemsData.value.filter(m => m.type === 'gallery').length,
  audio: mediaItemsData.value.filter(m => m.type === 'audio').length,
  images: mediaItemsData.value.filter(m => m.type === 'image').length
}))

// Computed Category Cards with dynamic counts
const categoryCards = computed(() => {
  return categoryCardDefs.map(card => ({
    ...card,
    count: mediaItemsData.value.filter(m => m.category === card.category).length
  }))
})

// Computed: All available tags from media items
const allTags = computed(() => {
  const tags = new Set<string>()
  mediaItems.value.forEach(item => {
    item.tags?.forEach(tag => tags.add(tag))
  })
  return Array.from(tags).sort()
})

/// Computed: Active Filters Count
const activeFiltersCount = computed(() => {
  let count = 0
  if (searchQuery.value) count++
  if (selectedFolder.value) count++
  count += selectedMediaTypes.value.length
  count += selectedCategories.value.length
  count += selectedTags.value.length
  count += selectedStatusFilters.value.length
  return count
})

// Computed: Filtered Media
const filteredMedia = computed(() => {
  let result = [...mediaItems.value]

  // Filter by active tab (quick filter)
  if (activeTab.value !== 'all') {
    result = result.filter(m => m.type === activeTab.value)
  }

  // Filter by dropdown media types (if any selected)
  if (selectedMediaTypes.value.length > 0) {
    result = result.filter(m => selectedMediaTypes.value.includes(m.type))
  }

  // Filter by search query
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    result = result.filter(m => m.title.toLowerCase().includes(q))
  }

  // Filter by single category (old filter)
  if (categoryFilter.value) {
    result = result.filter(m => m.category === categoryFilter.value)
  }

  // Filter by dropdown categories (if any selected)
  if (selectedCategories.value.length > 0) {
    result = result.filter(m => selectedCategories.value.includes(m.category))
  }

  // Filter by tags (if any selected)
  if (selectedTags.value.length > 0) {
    result = result.filter(m => m.tags?.some(tag => selectedTags.value.includes(tag)))
  }

  // Filter by status (saved/shared)
  if (selectedStatusFilters.value.length > 0) {
    result = result.filter(m => {
      if (selectedStatusFilters.value.includes('saved') && savedMedia.value.has(m.id)) return true
      if (selectedStatusFilters.value.includes('shared') && m.sharedWithMe) return true
      return false
    })
  }

  // Filter by selected folder (including children folders)
  if (selectedFolder.value) {
    const folderIds = getAllFolderIds(selectedFolder.value)
    result = result.filter(m => m.folderId && folderIds.includes(m.folderId))
  }

  return result
})

// Computed: Paginated Media
const paginatedMedia = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredMedia.value.slice(start, end)
})

// Computed: Trending Media (sorted by views)
const trendingMedia = computed(() => {
  return [...mediaItems.value]
    .sort((a, b) => {
      const viewsA = parseFloat(a.views.replace('K', '')) * (a.views.includes('K') ? 1000 : 1)
      const viewsB = parseFloat(b.views.replace('K', '')) * (b.views.includes('K') ? 1000 : 1)
      return viewsB - viewsA
    })
    .slice(0, 6)
})

// Computed: Total Pages
const totalPages = computed(() => {
  return Math.ceil(filteredMedia.value.length / itemsPerPage.value)
})

// Computed: Total Views
const totalViews = computed(() => {
  const total = mediaItems.value.reduce((sum, media) => {
    const views = parseFloat(media.views.replace('K', '')) * (media.views.includes('K') ? 1000 : 1)
    return sum + views
  }, 0)
  return total >= 1000 ? (total / 1000).toFixed(1) + 'K' : total.toString()
})

// Get count by media type
function getMediaTypeCount(type: string): number {
  return mediaItems.value.filter(m => m.type === type).length
}

// Methods
function playFeatured() {
  router.push({ name: 'MediaPlayer', params: { id: featuredVideo.value.id.toString() } })
}

function goToMedia(media: any) {
  router.push({ name: 'MediaPlayer', params: { id: media.id.toString() } })
}

function scrollTrending(direction: 'left' | 'right') {
  if (trendingScrollRef.value) {
    const scrollAmount = 280
    const currentScroll = trendingScrollRef.value.scrollLeft
    trendingScrollRef.value.scrollTo({
      left: direction === 'left' ? currentScroll - scrollAmount : currentScroll + scrollAmount,
      behavior: 'smooth'
    })
  }
}

function toggleLikeMedia(id: number) {
  if (likedMedia.value.has(id)) {
    likedMedia.value.delete(id)
  } else {
    likedMedia.value.add(id)
  }
}

function toggleSaveMedia(id: number) {
  if (savedMedia.value.has(id)) {
    savedMedia.value.delete(id)
  } else {
    savedMedia.value.add(id)
  }
}

function isMediaLiked(id: number): boolean {
  return likedMedia.value.has(id)
}

function isMediaSaved(id: number): boolean {
  return savedMedia.value.has(id)
}

function shareMedia(media: any) {
  if (navigator.share) {
    navigator.share({
      title: media.title,
      text: `Check out: ${media.title}`,
      url: window.location.origin + '/media/' + media.id
    })
  } else {
    navigator.clipboard.writeText(window.location.origin + '/media/' + media.id)
    alert('Link copied to clipboard!')
  }
}

function openAddToCollection(media: any) {
  selectedItemForCollection.value = {
    id: media.id,
    title: media.title,
    thumbnail: media.thumbnail
  }
  showAddToCollectionModal.value = true
}

function showDownloadAlert(title: string) {
  alert('Download: ' + title)
}

function copyMediaLink(mediaId: number) {
  if (typeof navigator !== 'undefined' && navigator.clipboard) {
    navigator.clipboard.writeText(window.location.origin + '/media/' + mediaId)
  }
}

function handleAddedToCollection(collectionIds: string[]) {
  if (collectionIds.length > 0) {
    alert(`Added to ${collectionIds.length} collection(s)!`)
  }
  showAddToCollectionModal.value = false
  selectedItemForCollection.value = null
}

function handleDrop(e: DragEvent) {
  isDragging.value = false
  const files = e.dataTransfer?.files
  if (files && files.length > 0) {
    showUploadModal.value = true
  }
}

function toggleCategoryFilter(cat: string) {
  if (categoryFilter.value === cat) {
    categoryFilter.value = ''
  } else {
    categoryFilter.value = cat
  }
}

function clearFilters() {
  categoryFilter.value = ''
  activeTab.value = 'all'
  searchQuery.value = ''
  selectedMediaTypes.value = []
  selectedCategories.value = []
  selectedFolder.value = null
}

// Sidebar functions
function toggleFolder(folderId: string) {
  if (expandedFolders.value.has(folderId)) {
    expandedFolders.value.delete(folderId)
  } else {
    expandedFolders.value.add(folderId)
  }
}

function isFolderExpanded(folderId: string): boolean {
  return expandedFolders.value.has(folderId)
}

function selectFolder(folderId: string) {
  selectedFolder.value = selectedFolder.value === folderId ? null : folderId
}

function getFolderName(folderId: string): string {
  // Search recursively through folder tree
  function findFolder(folders: any[]): string | null {
    for (const folder of folders) {
      if (folder.id === folderId) return folder.name
      if (folder.children?.length) {
        const found = findFolder(folder.children)
        if (found) return found
      }
    }
    return null
  }
  return findFolder(folderTree.value) || folderId
}

// Get all folder IDs including the selected folder and all its children (recursive)
function getAllFolderIds(folderId: string): string[] {
  const ids: string[] = [folderId]

  function findAndCollectChildren(folders: any[]): boolean {
    for (const folder of folders) {
      if (folder.id === folderId) {
        // Found the folder, collect all children recursively
        collectAllChildIds(folder.children || [])
        return true
      }
      if (folder.children?.length) {
        if (findAndCollectChildren(folder.children)) return true
      }
    }
    return false
  }

  function collectAllChildIds(children: any[]) {
    for (const child of children) {
      ids.push(child.id)
      if (child.children?.length) {
        collectAllChildIds(child.children)
      }
    }
  }

  findAndCollectChildren(folderTree.value)
  return ids
}

// Selection mode functions
function toggleSelectionMode() {
  isSelectionMode.value = !isSelectionMode.value
  if (!isSelectionMode.value) {
    selectedMedia.value.clear()
  }
}

function toggleMediaSelection(id: number) {
  if (selectedMedia.value.has(id)) {
    selectedMedia.value.delete(id)
  } else {
    selectedMedia.value.add(id)
  }
}

function isMediaSelected(id: number): boolean {
  return selectedMedia.value.has(id)
}

function selectAllMedia() {
  filteredMedia.value.forEach(media => {
    selectedMedia.value.add(media.id)
  })
}

function clearMediaSelection() {
  selectedMedia.value.clear()
}

const selectedCount = computed(() => selectedMedia.value.size)
const isAllSelected = computed(() => selectedMedia.value.size === filteredMedia.value.length && filteredMedia.value.length > 0)
const isSomeSelected = computed(() => selectedMedia.value.size > 0 && selectedMedia.value.size < filteredMedia.value.length)

// Bulk actions
function bulkDownload() {
  const selected = Array.from(selectedMedia.value)
  alert(`Downloading ${selected.length} items...`)
}

function bulkShare() {
  const selected = Array.from(selectedMedia.value)
  alert(`Sharing ${selected.length} items...`)
}

function bulkAddToPlaylist() {
  const selected = Array.from(selectedMedia.value)
  alert(`Adding ${selected.length} items to playlist...`)
}

function bulkMoveToTrash() {
  const selected = Array.from(selectedMedia.value)
  alert(`Moving ${selected.length} items to trash...`)
  selectedMedia.value.clear()
  isSelectionMode.value = false
}

function bulkRestore() {
  const selected = Array.from(selectedMedia.value)
  alert(`Restoring ${selected.length} items...`)
  selectedMedia.value.clear()
  isSelectionMode.value = false
}

function bulkPermanentDelete() {
  const selected = Array.from(selectedMedia.value)
  if (confirm(`Permanently delete ${selected.length} items? This cannot be undone.`)) {
    alert(`Deleted ${selected.length} items permanently`)
    selectedMedia.value.clear()
    isSelectionMode.value = false
  }
}

// Dropdown filter functions
function toggleMediaType(type: string) {
  const index = selectedMediaTypes.value.indexOf(type)
  if (index > -1) {
    selectedMediaTypes.value.splice(index, 1)
  } else {
    selectedMediaTypes.value.push(type)
  }
}

function isMediaTypeSelected(type: string): boolean {
  return selectedMediaTypes.value.includes(type)
}

function toggleCategorySelection(cat: string) {
  const index = selectedCategories.value.indexOf(cat)
  if (index > -1) {
    selectedCategories.value.splice(index, 1)
  } else {
    selectedCategories.value.push(cat)
  }
}

function isCategorySelected(cat: string): boolean {
  return selectedCategories.value.includes(cat)
}

function toggleTagFilter(tag: string) {
  const index = selectedTags.value.indexOf(tag)
  if (index > -1) {
    selectedTags.value.splice(index, 1)
  } else {
    selectedTags.value.push(tag)
  }
}

function isTagSelected(tag: string): boolean {
  return selectedTags.value.includes(tag)
}

function toggleStatusFilter(status: string) {
  const index = selectedStatusFilters.value.indexOf(status)
  if (index > -1) {
    selectedStatusFilters.value.splice(index, 1)
  } else {
    selectedStatusFilters.value.push(status)
  }
}

function isStatusSelected(status: string): boolean {
  return selectedStatusFilters.value.includes(status)
}

function toggleMediaTypeFilter(typeId: string) {
  const index = selectedMediaTypes.value.indexOf(typeId)
  if (index > -1) {
    selectedMediaTypes.value.splice(index, 1)
  } else {
    selectedMediaTypes.value.push(typeId)
  }
}

function clearAllFilters() {
  searchQuery.value = ''
  selectedFolder.value = null
  selectedMediaTypes.value = []
  selectedCategories.value = []
  selectedTags.value = []
  selectedStatusFilters.value = []
  categoryFilter.value = ''
  activeTab.value = 'all'
}

// Get current sort option
const currentSortOption = computed(() => {
  return sortOptionsList.value.find(opt => opt.value === sortBy.value) ?? sortOptionsList.value[0]!
})

function selectSortOption(value: string) {
  sortBy.value = value
  showSortDropdown.value = false
}

// View counts
const viewCounts = computed(() => ({
  all: mediaItems.value.length,
  recent: Math.floor(mediaItems.value.length * 0.4),
  popular: Math.floor(mediaItems.value.length * 0.3),
  saved: savedMedia.value.size,
  trash: 0
}))

function handlePageChange(page: number) {
  currentPage.value = page
}

function handlePerPageChange(perPage: number) {
  itemsPerPage.value = perPage
  currentPage.value = 1
}

// Get initials from author name
function getInitials(name: string | undefined): string {
  if (!name) return 'IN'
  return name.split(' ').map(n => n[0]).join('')
}

// Create particles effect
function createParticles() {
  const particlesContainer = document.getElementById('media-particles')
  if (!particlesContainer) return

  const particleCount = 50
  for (let i = 0; i < particleCount; i++) {
    const particle = document.createElement('div')
    particle.className = 'particle'
    particle.style.left = Math.random() * 100 + '%'
    particle.style.animationDuration = (Math.random() * 10 + 10) + 's'
    particle.style.animationDelay = Math.random() * 5 + 's'
    particle.style.width = (Math.random() * 3 + 2) + 'px'
    particle.style.height = particle.style.width
    particlesContainer.appendChild(particle)
  }
}

// ============================================================================
// AI Features State & Functions
// ============================================================================

const showAIFeatures = ref(true)
const showAIAnalysisModal = ref(false)
const selectedMediaForAI = ref<any>(null)
const isAnalyzingMedia = ref(false)
const showAISuggestionsPanel = ref(false)

// AI Analysis Results
interface AIMediaAnalysis {
  mediaId: number
  autoTags: string[]
  category: string
  categoryConfidence: number
  description: string
  ocrText?: string
  ocrConfidence?: number
  faces?: number
  objects?: string[]
  colors?: string[]
  processingTime: number
}

const aiAnalysisResults = ref<Map<number, AIMediaAnalysis>>(new Map())
const aiSuggestedTags = ref<string[]>([])
const aiSuggestedCategories = ref<{ name: string; confidence: number }[]>([])

// Mock AI Analysis Data
const mockAIAnalysis: Record<number, AIMediaAnalysis> = {
  5: {
    mediaId: 5,
    autoTags: ['Saudi Arabia', 'Japan', 'Football', 'Preview', 'Match', 'Tournament', 'Asian Cup'],
    category: 'Sports Preview',
    categoryConfidence: 0.94,
    description: 'Pre-match preview video featuring analysis of the upcoming Saudi Arabia vs Japan opening match, including team formations, key players to watch, and tactical predictions.',
    objects: ['Football field', 'Players', 'Stadium', 'Flags'],
    colors: ['Green', 'White', 'Blue'],
    processingTime: 1.2
  },
  6: {
    mediaId: 6,
    autoTags: ['Stadium', 'King Fahd', 'Tour', 'Behind the Scenes', 'Riyadh', 'Infrastructure'],
    category: 'Venue Tour',
    categoryConfidence: 0.96,
    description: 'Exclusive behind-the-scenes tour of King Fahd International Stadium showcasing state-of-the-art facilities, pitch quality, and spectator amenities.',
    objects: ['Stadium seats', 'Pitch', 'Scoreboard', 'VIP area'],
    colors: ['Green', 'Gray', 'Blue'],
    processingTime: 1.4
  },
  13: {
    mediaId: 13,
    autoTags: ['Draw Ceremony', 'Official', 'AFC', 'Teams', 'Groups', 'Event'],
    category: 'Official Event',
    categoryConfidence: 0.98,
    description: 'Photo gallery capturing key moments from the AFC Asian Cup 2027 Official Draw Ceremony, featuring team representatives and the group stage reveal.',
    faces: 24,
    objects: ['Stage', 'Trophy', 'Flags', 'Podium'],
    colors: ['Gold', 'White', 'Red'],
    processingTime: 2.1
  },
  19: {
    mediaId: 19,
    autoTags: ['Aerial', 'Stadium', 'King Fahd', 'Riyadh', 'Architecture', 'Drone'],
    category: 'Venue Photography',
    categoryConfidence: 0.95,
    description: 'Stunning aerial photograph of King Fahd International Stadium captured by drone, showcasing the stadium\'s impressive architecture and surrounding facilities.',
    ocrText: 'King Fahd International Stadium - Capacity: 68,000',
    ocrConfidence: 0.89,
    objects: ['Stadium', 'Parking lots', 'Roads', 'Landscape'],
    colors: ['Green', 'Gray', 'Brown'],
    processingTime: 1.8
  },
  21: {
    mediaId: 21,
    autoTags: ['Team Photo', 'Saudi Arabia', 'Official', 'Players', 'Squad', '2027'],
    category: 'Team Content',
    categoryConfidence: 0.97,
    description: 'Official team photograph of the Saudi Arabia national football team for the AFC Asian Cup 2027, featuring all squad members in their home kit.',
    faces: 26,
    objects: ['Players', 'Jerseys', 'Football', 'Backdrop'],
    colors: ['Green', 'White'],
    processingTime: 1.5
  },
  22: {
    mediaId: 22,
    autoTags: ['Trophy', 'Unveiling', 'Ceremony', 'AFC', 'Official', 'Event'],
    category: 'Official Event',
    categoryConfidence: 0.96,
    description: 'Image capturing the ceremonial unveiling of the AFC Asian Cup trophy, a key moment in the tournament preparations.',
    ocrText: 'AFC Asian Cup 2027',
    ocrConfidence: 0.92,
    objects: ['Trophy', 'Stage', 'Lights', 'Curtain'],
    colors: ['Gold', 'Blue', 'White'],
    processingTime: 1.3
  }
}

// ============================================================================
// Enhanced AI Features - Sidebar Panel, Recommendations, Insights
// ============================================================================

// AI Sidebar Panel State
const showAISidebar = ref(false)
const aiSidebarTab = ref<'summary' | 'transcript' | 'entities' | 'translate'>('summary')
const selectedMediaForSidebar = ref<any>(null)

// AI Summary State
const aiSummaryType = ref<'brief' | 'detailed' | 'bullets'>('brief')
const aiSummary = ref<{ text: string; keyPoints: string[]; confidence: number } | null>(null)
const isGeneratingSummary = ref(false)

// AI Transcript State
const aiTranscript = ref<{ text: string; timestamps: { time: string; text: string }[]; language: string; confidence: number } | null>(null)
const isGeneratingTranscript = ref(false)

// AI Entities State
const aiEntities = ref<{ type: string; name: string; count: number }[]>([])
const isExtractingEntities = ref(false)
const entityFilterType = ref<string | null>(null)

// AI Translation State
const aiTranslation = ref<{ language: string; text: string } | null>(null)
const isTranslating = ref(false)
const targetLanguage = ref('ar')
const availableLanguages = [
  { code: 'ar', name: 'Arabic', flag: '🇸🇦' },
  { code: 'ja', name: 'Japanese', flag: '🇯🇵' },
  { code: 'ko', name: 'Korean', flag: '🇰🇷' },
  { code: 'zh', name: 'Chinese', flag: '🇨🇳' },
  { code: 'fr', name: 'French', flag: '🇫🇷' },
  { code: 'es', name: 'Spanish', flag: '🇪🇸' }
]

// AI Recommendations State
const aiRecommendations = ref<any[]>([])
const isLoadingRecommendations = ref(false)
const recommendationType = ref<'forYou' | 'similar' | 'trending'>('forYou')

// AI Video Insights State
interface KeyMoment {
  timestamp: string
  title: string
  description: string
  type: 'goal' | 'highlight' | 'interview' | 'ceremony' | 'general'
}
interface Speaker {
  name: string
  role: string
  speakingTime: string
  segments: number
}
interface SentimentData {
  overall: 'positive' | 'neutral' | 'negative'
  score: number
  breakdown: { label: string; value: number }[]
}

const aiKeyMoments = ref<KeyMoment[]>([])
const aiSpeakers = ref<Speaker[]>([])
const aiSentiment = ref<SentimentData | null>(null)
const isAnalyzingInsights = ref(false)

// AI Accessibility State
const aiCaptions = ref<{ time: string; text: string }[]>([])
const aiAudioDescription = ref<string>('')
const isGeneratingCaptions = ref(false)
const isGeneratingAudioDesc = ref(false)

// AI Natural Language Search
const naturalLanguageQuery = ref('')
const aiSearchResults = ref<any[]>([])
const isProcessingNLSearch = ref(false)
const nlSearchSuggestions = [
  'Show me highlights from Japan matches',
  'Find interviews with Saudi players',
  'Stadium tours with behind the scenes',
  'Most viewed videos this week',
  'Content about fan celebrations'
]

// Mock AI Data for Summaries
const mockSummaries: Record<number, Record<string, { text: string; keyPoints: string[]; confidence: number }>> = {
  5: {
    brief: { text: 'Preview of the Saudi Arabia vs Japan opening match featuring tactical analysis and key player insights.', keyPoints: ['Opening match analysis', 'Key players highlighted'], confidence: 0.92 },
    detailed: { text: 'This comprehensive preview explores the highly anticipated opening match between Saudi Arabia and Japan. The analysis covers both teams\' recent form, tactical approaches, and key players to watch. Saudi Arabia enters as hosts with home advantage, while Japan brings their technical excellence and tournament experience.', keyPoints: ['Saudi Arabia home advantage', 'Japan technical strength', 'Tactical formations analysis', 'Key matchups identified'], confidence: 0.94 },
    bullets: { text: '• Opening match: Saudi Arabia vs Japan\n• Saudi Arabia: Home advantage, passionate crowd support\n• Japan: Technical excellence, tournament experience\n• Key players: Al-Dawsari (SAU), Mitoma (JPN)\n• Expected tactical battle between possession styles', keyPoints: [], confidence: 0.91 }
  },
  6: {
    brief: { text: 'Exclusive tour of King Fahd International Stadium showcasing world-class facilities prepared for AFC Asian Cup 2027.', keyPoints: ['Stadium facilities', 'Tournament preparations'], confidence: 0.95 },
    detailed: { text: 'Take an exclusive behind-the-scenes tour of King Fahd International Stadium, one of the premier venues for AFC Asian Cup 2027. This video showcases the stadium\'s state-of-the-art facilities including the hybrid grass pitch, modern VIP lounges, press facilities, and fan zones. The venue has undergone significant upgrades to meet FIFA standards.', keyPoints: ['Hybrid grass pitch technology', 'VIP and hospitality areas', 'Media facilities tour', 'Fan zone preparations', 'Accessibility features'], confidence: 0.96 },
    bullets: { text: '• King Fahd International Stadium tour\n• Capacity: 68,000 spectators\n• Hybrid grass pitch technology\n• Modern VIP and hospitality suites\n• State-of-the-art media center\n• Enhanced accessibility features', keyPoints: [], confidence: 0.93 }
  }
}

// Mock AI Data for Transcripts
const mockTranscripts: Record<number, { text: string; timestamps: { time: string; text: string }[]; language: string; confidence: number }> = {
  5: {
    text: 'Welcome to our preview of the opening match between Saudi Arabia and Japan. This highly anticipated clash will set the tone for the entire tournament...',
    timestamps: [
      { time: '00:00', text: 'Welcome to our preview of the opening match' },
      { time: '00:15', text: 'Saudi Arabia enters as hosts with tremendous support' },
      { time: '00:45', text: 'Japan brings technical excellence and experience' },
      { time: '01:30', text: 'Key player analysis: Salem Al-Dawsari' },
      { time: '02:15', text: 'Japanese tactics under manager discussion' },
      { time: '03:00', text: 'Predicted lineups and formations' },
      { time: '04:30', text: 'Historical head-to-head record' },
      { time: '05:45', text: 'Match prediction and final thoughts' }
    ],
    language: 'English',
    confidence: 0.94
  },
  6: {
    text: 'Join us for an exclusive behind-the-scenes tour of King Fahd International Stadium, the crown jewel of AFC Asian Cup 2027 venues...',
    timestamps: [
      { time: '00:00', text: 'Introduction to King Fahd Stadium' },
      { time: '00:30', text: 'Arriving at the main entrance' },
      { time: '01:15', text: 'Tour of the pitch and playing surface' },
      { time: '02:30', text: 'VIP lounges and hospitality areas' },
      { time: '04:00', text: 'Press facilities and media center' },
      { time: '05:30', text: 'Fan zones and concourse areas' },
      { time: '07:00', text: 'Behind the scenes: Operations center' },
      { time: '08:30', text: 'Final preparations for tournament' }
    ],
    language: 'English',
    confidence: 0.96
  }
}

// Mock AI Data for Entities
const mockEntities: Record<number, { type: string; name: string; count: number }[]> = {
  5: [
    { type: 'Team', name: 'Saudi Arabia', count: 12 },
    { type: 'Team', name: 'Japan', count: 10 },
    { type: 'Person', name: 'Salem Al-Dawsari', count: 5 },
    { type: 'Person', name: 'Kaoru Mitoma', count: 4 },
    { type: 'Location', name: 'King Fahd Stadium', count: 3 },
    { type: 'Event', name: 'AFC Asian Cup 2027', count: 8 },
    { type: 'Organization', name: 'AFC', count: 4 }
  ],
  6: [
    { type: 'Location', name: 'King Fahd International Stadium', count: 15 },
    { type: 'Location', name: 'Riyadh', count: 6 },
    { type: 'Organization', name: 'AFC', count: 4 },
    { type: 'Event', name: 'AFC Asian Cup 2027', count: 7 },
    { type: 'Facility', name: 'VIP Lounge', count: 3 },
    { type: 'Facility', name: 'Media Center', count: 4 }
  ]
}

// Mock AI Data for Key Moments
const mockKeyMoments: Record<number, KeyMoment[]> = {
  5: [
    { timestamp: '00:45', title: 'Team Analysis Begins', description: 'In-depth look at both teams\' strengths', type: 'general' },
    { timestamp: '02:15', title: 'Star Player Spotlight', description: 'Focus on Al-Dawsari\'s form', type: 'highlight' },
    { timestamp: '04:30', title: 'Tactical Breakdown', description: 'Formation analysis and strategies', type: 'general' },
    { timestamp: '06:00', title: 'Historical Rivalry', description: 'Past encounters between the teams', type: 'highlight' }
  ],
  8: [
    { timestamp: '00:30', title: 'Goal #10', description: 'Spectacular long-range strike', type: 'goal' },
    { timestamp: '01:45', title: 'Goal #9', description: 'Clinical header from corner', type: 'goal' },
    { timestamp: '03:00', title: 'Goal #8', description: 'Team play finishing move', type: 'goal' },
    { timestamp: '04:15', title: 'Goal #7', description: 'Solo dribble and finish', type: 'goal' },
    { timestamp: '05:30', title: 'Goal #6', description: 'Free kick masterclass', type: 'goal' }
  ]
}

// Mock AI Data for Speakers
const mockSpeakers: Record<number, Speaker[]> = {
  7: [
    { name: 'Roberto Mancini', role: 'Saudi Arabia Manager', speakingTime: '8:30', segments: 12 },
    { name: 'Interviewer', role: 'Sports Journalist', speakingTime: '4:15', segments: 15 },
    { name: 'Translator', role: 'Arabic-English', speakingTime: '2:30', segments: 8 }
  ],
  10: [
    { name: 'Salem Al-Dawsari', role: 'Saudi Arabia Captain', speakingTime: '10:45', segments: 18 },
    { name: 'Sports Desk Host', role: 'Interviewer', speakingTime: '5:30', segments: 20 }
  ]
}

// Mock AI Data for Sentiment
const mockSentiment: Record<number, SentimentData> = {
  5: { overall: 'positive', score: 0.78, breakdown: [{ label: 'Excitement', value: 85 }, { label: 'Anticipation', value: 90 }, { label: 'Confidence', value: 72 }] },
  7: { overall: 'positive', score: 0.82, breakdown: [{ label: 'Optimism', value: 88 }, { label: 'Determination', value: 85 }, { label: 'Respect', value: 79 }] },
  14: { overall: 'positive', score: 0.95, breakdown: [{ label: 'Joy', value: 98 }, { label: 'Excitement', value: 95 }, { label: 'Pride', value: 92 }] }
}

// AI Functions
async function openAISidebar(media: any) {
  selectedMediaForSidebar.value = media
  showAISidebar.value = true
  aiSidebarTab.value = 'summary'
  // Reset states
  aiSummary.value = null
  aiTranscript.value = null
  aiEntities.value = []
  aiTranslation.value = null
  aiKeyMoments.value = []
  aiSpeakers.value = []
  aiSentiment.value = null
}

function closeAISidebar() {
  showAISidebar.value = false
  selectedMediaForSidebar.value = null
}

async function generateAISummary() {
  if (!selectedMediaForSidebar.value || isGeneratingSummary.value) return

  isGeneratingSummary.value = true
  aiSummary.value = null

  try {
    await new Promise(resolve => setTimeout(resolve, 1200))
    const mediaId = selectedMediaForSidebar.value.id
    aiSummary.value = mockSummaries[mediaId]?.[aiSummaryType.value] || {
      text: `AI-generated ${aiSummaryType.value} summary of "${selectedMediaForSidebar.value.title}": This ${selectedMediaForSidebar.value.type} content provides valuable insights about ${selectedMediaForSidebar.value.category}.`,
      keyPoints: ['Key insight from content', 'Important highlight', 'Notable information'],
      confidence: 0.85
    }
  } finally {
    isGeneratingSummary.value = false
  }
}

async function generateAITranscript() {
  if (!selectedMediaForSidebar.value || isGeneratingTranscript.value) return
  if (!['video', 'audio'].includes(selectedMediaForSidebar.value.type)) return

  isGeneratingTranscript.value = true
  aiTranscript.value = null

  try {
    await new Promise(resolve => setTimeout(resolve, 1800))
    const mediaId = selectedMediaForSidebar.value.id
    aiTranscript.value = mockTranscripts[mediaId] || {
      text: `[AI-generated transcript for "${selectedMediaForSidebar.value.title}"]\n\nThis is an automatically generated transcript of the ${selectedMediaForSidebar.value.type} content. The speech-to-text AI has processed the audio track to provide this textual representation.`,
      timestamps: [
        { time: '00:00', text: 'Introduction and welcome' },
        { time: '01:00', text: 'Main content begins' },
        { time: '03:00', text: 'Key points discussed' },
        { time: '05:00', text: 'Conclusion and summary' }
      ],
      language: 'English',
      confidence: 0.88
    }
  } finally {
    isGeneratingTranscript.value = false
  }
}

async function extractAIEntities() {
  if (!selectedMediaForSidebar.value || isExtractingEntities.value) return

  isExtractingEntities.value = true
  aiEntities.value = []

  try {
    await new Promise(resolve => setTimeout(resolve, 1000))
    const mediaId = selectedMediaForSidebar.value.id
    aiEntities.value = mockEntities[mediaId] || [
      { type: 'Event', name: 'AFC Asian Cup 2027', count: 5 },
      { type: 'Organization', name: 'AFC', count: 3 },
      { type: 'Location', name: selectedMediaForSidebar.value.category, count: 2 }
    ]
  } finally {
    isExtractingEntities.value = false
  }
}

async function translateContent() {
  if (!selectedMediaForSidebar.value || isTranslating.value) return

  isTranslating.value = true
  aiTranslation.value = null

  try {
    await new Promise(resolve => setTimeout(resolve, 1500))
    const langName = availableLanguages.find(l => l.code === targetLanguage.value)?.name || 'Arabic'
    const content = aiTranscript.value?.text || selectedMediaForSidebar.value.title

    // Mock translations
    const translations: Record<string, string> = {
      ar: `[ترجمة عربية]\n${content}`,
      ja: `[日本語翻訳]\n${content}`,
      ko: `[한국어 번역]\n${content}`,
      zh: `[中文翻译]\n${content}`,
      fr: `[Traduction française]\n${content}`,
      es: `[Traducción al español]\n${content}`
    }

    aiTranslation.value = {
      language: langName,
      text: translations[targetLanguage.value] || `[Translated to ${langName}]\n${content}`
    }
  } finally {
    isTranslating.value = false
  }
}

async function loadAIRecommendations() {
  isLoadingRecommendations.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 800))

    // Generate recommendations based on type
    const allMedia = mediaItems.value

    if (recommendationType.value === 'forYou') {
      // Based on watch history categories
      const watchedCategories = watchHistory.value.map(w => w.category)
      aiRecommendations.value = allMedia
        .filter(m => watchedCategories.includes(m.category) || m.isNew)
        .slice(0, 6)
    } else if (recommendationType.value === 'similar') {
      // Based on selected media
      const selected = selectedMediaForSidebar.value
      if (selected) {
        aiRecommendations.value = allMedia
          .filter(m => m.id !== selected.id && (m.category === selected.category || m.tags?.some((t: string) => selected.tags?.includes(t))))
          .slice(0, 6)
      }
    } else {
      // Trending - sorted by views
      aiRecommendations.value = [...allMedia]
        .sort((a, b) => {
          const viewsA = parseFloat(a.views.replace('K', '')) * (a.views.includes('K') ? 1000 : 1)
          const viewsB = parseFloat(b.views.replace('K', '')) * (b.views.includes('K') ? 1000 : 1)
          return viewsB - viewsA
        })
        .slice(0, 6)
    }
  } finally {
    isLoadingRecommendations.value = false
  }
}

async function analyzeVideoInsights() {
  if (!selectedMediaForSidebar.value || isAnalyzingInsights.value) return

  isAnalyzingInsights.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 1500))
    const mediaId = selectedMediaForSidebar.value.id

    aiKeyMoments.value = mockKeyMoments[mediaId] || [
      { timestamp: '00:00', title: 'Introduction', description: 'Content begins', type: 'general' },
      { timestamp: '02:00', title: 'Main Segment', description: 'Core content', type: 'highlight' }
    ]

    aiSpeakers.value = mockSpeakers[mediaId] || []
    aiSentiment.value = mockSentiment[mediaId] || { overall: 'neutral', score: 0.5, breakdown: [{ label: 'Neutral', value: 50 }] }
  } finally {
    isAnalyzingInsights.value = false
  }
}

async function generateAICaptions() {
  if (!selectedMediaForSidebar.value || isGeneratingCaptions.value) return

  isGeneratingCaptions.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 2000))

    // Use transcript timestamps as captions if available
    if (aiTranscript.value?.timestamps) {
      aiCaptions.value = aiTranscript.value.timestamps
    } else {
      aiCaptions.value = [
        { time: '00:00', text: '[Opening scene]' },
        { time: '00:05', text: 'Welcome to AFC Asian Cup 2027 coverage' },
        { time: '00:15', text: '[Background music playing]' },
        { time: '00:30', text: 'Today we bring you exclusive content' }
      ]
    }
  } finally {
    isGeneratingCaptions.value = false
  }
}

async function generateAudioDescription() {
  if (!selectedMediaForSidebar.value || isGeneratingAudioDesc.value) return

  isGeneratingAudioDesc.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 1500))

    const media = selectedMediaForSidebar.value
    aiAudioDescription.value = `[Audio Description for "${media.title}"]\n\n` +
      `This ${media.type} content from the ${media.category} category shows ` +
      `scenes related to AFC Asian Cup 2027. ` +
      (media.type === 'gallery' ? `The gallery contains ${media.photoCount} photos. ` : '') +
      `Visual elements include sports imagery, tournament branding, and ` +
      `${media.category.toLowerCase()} related content. ` +
      `Created by ${media.author}.`
  } finally {
    isGeneratingAudioDesc.value = false
  }
}

async function processNaturalLanguageSearch() {
  if (!naturalLanguageQuery.value || isProcessingNLSearch.value) return

  isProcessingNLSearch.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 1000))

    const query = naturalLanguageQuery.value.toLowerCase()
    const allMedia = mediaItems.value

    // Simple NL processing
    let results = allMedia

    if (query.includes('highlight') || query.includes('goals')) {
      results = results.filter(m => m.category === 'Highlights' || m.tags?.some((t: string) => t.toLowerCase().includes('goal')))
    }
    if (query.includes('interview')) {
      results = results.filter(m => m.category === 'Interviews' || m.type === 'audio')
    }
    if (query.includes('stadium') || query.includes('tour') || query.includes('venue')) {
      results = results.filter(m => m.category === 'Venues' || m.tags?.some((t: string) => t.toLowerCase().includes('stadium')))
    }
    if (query.includes('japan')) {
      results = results.filter(m => m.tags?.some((t: string) => t.toLowerCase().includes('japan')))
    }
    if (query.includes('saudi')) {
      results = results.filter(m => m.tags?.some((t: string) => t.toLowerCase().includes('saudi')))
    }
    if (query.includes('most viewed') || query.includes('popular')) {
      results = [...results].sort((a, b) => {
        const viewsA = parseFloat(a.views.replace('K', '')) * (a.views.includes('K') ? 1000 : 1)
        const viewsB = parseFloat(b.views.replace('K', '')) * (b.views.includes('K') ? 1000 : 1)
        return viewsB - viewsA
      })
    }
    if (query.includes('fan') || query.includes('celebration')) {
      results = results.filter(m => m.category === 'Fans')
    }

    aiSearchResults.value = results.slice(0, 12)
  } finally {
    isProcessingNLSearch.value = false
  }
}

// Unified Search Handlers
function handleUnifiedSearch() {
  if (!unifiedSearchQuery.value) return

  if (isAISearchMode.value) {
    // Use AI-powered search
    naturalLanguageQuery.value = unifiedSearchQuery.value
    processNaturalLanguageSearch()
  } else {
    // Use normal text search
    searchQuery.value = unifiedSearchQuery.value
  }
  showAISuggestions.value = false
}

function handleSearchInput() {
  if (isAISearchMode.value) {
    // In AI mode, don't auto-search, wait for enter or button click
    showAISuggestions.value = !unifiedSearchQuery.value
  } else {
    // In normal mode, apply filter immediately
    searchQuery.value = unifiedSearchQuery.value
  }

  // Debounced AI suggestions fetch
  if (searchDebounceTimeout.value) {
    clearTimeout(searchDebounceTimeout.value)
  }
  searchDebounceTimeout.value = setTimeout(() => {
    fetchAISearchSuggestions(unifiedSearchQuery.value || searchQuery.value)
  }, 300)
}

function clearUnifiedSearch() {
  unifiedSearchQuery.value = ''
  searchQuery.value = ''
  naturalLanguageQuery.value = ''
  aiSearchResults.value = []
  showAISuggestions.value = false
}

// Watch for AI mode toggle to show suggestions
watch(isAISearchMode, (newValue) => {
  if (newValue && !unifiedSearchQuery.value) {
    showAISuggestions.value = true
  } else {
    showAISuggestions.value = false
  }
  // Sync the search query when switching modes
  if (!newValue) {
    searchQuery.value = unifiedSearchQuery.value
  }
})

function getEntityTypeColor(type: string): string {
  const colors: Record<string, string> = {
    'Team': 'bg-blue-100 text-blue-700',
    'Person': 'bg-purple-100 text-purple-700',
    'Location': 'bg-green-100 text-green-700',
    'Event': 'bg-amber-100 text-amber-700',
    'Organization': 'bg-teal-100 text-teal-700',
    'Facility': 'bg-rose-100 text-rose-700'
  }
  return colors[type] || 'bg-gray-100 text-gray-700'
}

function getMomentTypeIcon(type: string): string {
  const icons: Record<string, string> = {
    'goal': 'fas fa-futbol',
    'highlight': 'fas fa-star',
    'interview': 'fas fa-microphone',
    'ceremony': 'fas fa-award',
    'general': 'fas fa-bookmark'
  }
  return icons[type] || 'fas fa-bookmark'
}

function getSentimentColor(sentiment: string): string {
  const colors: Record<string, string> = {
    'positive': 'text-green-600',
    'neutral': 'text-gray-600',
    'negative': 'text-red-600'
  }
  return colors[sentiment] || 'text-gray-600'
}

function copyTranscript() {
  if (aiTranscript.value) {
    navigator.clipboard.writeText(aiTranscript.value.text)
    alert('Transcript copied to clipboard!')
  }
}

function copySummaryText() {
  if (aiSummary.value) {
    navigator.clipboard.writeText(aiSummary.value.text)
    alert('Summary copied to clipboard!')
  }
}

function copyTranslation() {
  if (aiTranslation.value) {
    navigator.clipboard.writeText(aiTranslation.value.text)
    alert('Translation copied to clipboard!')
  }
}

// AI Smart Search Suggestions
const aiSearchSuggestions = ref<string[]>([])
const isLoadingAISuggestions = ref(false)
const searchDebounceTimeout = ref<ReturnType<typeof setTimeout> | null>(null)

async function fetchAISearchSuggestions(query: string) {
  if (!query || query.length < 2) {
    aiSearchSuggestions.value = []
    return
  }

  isLoadingAISuggestions.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 400))

    // Generate AI-powered search suggestions from actual media content
    const q = query.toLowerCase()

    // Get unique suggestions from media titles, categories, tags, and authors
    const titleSuggestions = mediaItemsData.value
      .filter(m => m.title.toLowerCase().includes(q))
      .map(m => m.title)
      .slice(0, 3)

    const categorySuggestions = [...new Set(mediaItemsData.value.map(m => m.category))]
      .filter(c => c.toLowerCase().includes(q))
      .map(c => `${c} content`)

    const tagSuggestions = [...new Set(mediaItemsData.value.flatMap(m => m.tags || []))]
      .filter(t => t.toLowerCase().includes(q))
      .map(t => `${t} videos and media`)
      .slice(0, 2)

    // Combine and dedupe suggestions
    const allSuggestions = [...new Set([...titleSuggestions, ...categorySuggestions, ...tagSuggestions])]
    aiSearchSuggestions.value = allSuggestions.slice(0, 5)

    // Add AI-generated context suggestions
    if (q.includes('match') || q.includes('game')) {
      aiSearchSuggestions.value.unshift('AI suggests: Match highlights and replays')
    }
    if (q.includes('team') || q.includes('player')) {
      aiSearchSuggestions.value.unshift('AI suggests: Team profiles and interviews')
    }
    if (q.includes('stadium') || q.includes('venue')) {
      aiSearchSuggestions.value.unshift('AI suggests: Stadium tours and aerial views')
    }

    // Limit to 5 suggestions
    aiSearchSuggestions.value = aiSearchSuggestions.value.slice(0, 5)
  } finally {
    isLoadingAISuggestions.value = false
  }
}

function selectAISuggestion(suggestion: string) {
  // Remove "AI suggests: " prefix if present
  searchQuery.value = suggestion.replace('AI suggests: ', '')
  aiSearchSuggestions.value = []
}

// AI Analysis Functions
async function analyzeMedia(media: any) {
  if (isAnalyzingMedia.value) return

  selectedMediaForAI.value = media
  showAIAnalysisModal.value = true
  isAnalyzingMedia.value = true

  try {
    // Check if we already have analysis cached
    if (aiAnalysisResults.value.has(media.id)) {
      isAnalyzingMedia.value = false
      return
    }

    // Simulate AI analysis
    await new Promise(resolve => setTimeout(resolve, 1500))

    // Get mock analysis or generate default
    const analysis = mockAIAnalysis[media.id] || {
      mediaId: media.id,
      autoTags: media.tags || ['Media', 'Content', media.category],
      category: media.category,
      categoryConfidence: 0.85,
      description: `AI-generated description for "${media.title}": This ${media.type} content is categorized under ${media.category} and contains relevant information about AFC Asian Cup 2027.`,
      objects: ['Content'],
      colors: ['Various'],
      processingTime: 1.0
    }

    aiAnalysisResults.value.set(media.id, analysis)
  } finally {
    isAnalyzingMedia.value = false
  }
}

function getAIAnalysis(mediaId: number): AIMediaAnalysis | null {
  return aiAnalysisResults.value.get(mediaId) || null
}

function hasAIAnalysis(mediaId: number): boolean {
  return aiAnalysisResults.value.has(mediaId)
}

async function bulkAnalyzeMedia() {
  const selectedIds = Array.from(selectedMedia.value)
  if (selectedIds.length === 0) return

  isAnalyzingMedia.value = true

  try {
    for (const id of selectedIds) {
      const media = mediaItems.value.find(m => m.id === id)
      if (media && !aiAnalysisResults.value.has(id)) {
        await new Promise(resolve => setTimeout(resolve, 500))

        const analysis = mockAIAnalysis[id] || {
          mediaId: id,
          autoTags: media.tags || ['Media', media.category],
          category: media.category,
          categoryConfidence: 0.85,
          description: `AI analysis of ${media.title}`,
          processingTime: 0.5
        }

        aiAnalysisResults.value.set(id, analysis)
      }
    }
    alert(`AI analysis complete for ${selectedIds.length} items!`)
  } finally {
    isAnalyzingMedia.value = false
  }
}

function applyAITags(mediaId: number) {
  const analysis = aiAnalysisResults.value.get(mediaId)
  if (analysis) {
    // Find in raw data to allow mutation
    const media = mediaItemsData.value.find(m => m.id === mediaId)
    if (media) {
      media.tags = [...new Set([...(media.tags || []), ...analysis.autoTags])]
      alert(`Applied ${analysis.autoTags.length} AI-suggested tags!`)
    }
  }
}

function closeAIModal() {
  showAIAnalysisModal.value = false
  selectedMediaForAI.value = null
}

function toggleAIFeatures() {
  showAIFeatures.value = !showAIFeatures.value
}

// Get AI category badge color
function getAICategoryColor(category: string): string {
  const colors: Record<string, string> = {
    'Sports Preview': 'bg-blue-100 text-blue-700',
    'Venue Tour': 'bg-green-100 text-green-700',
    'Official Event': 'bg-purple-100 text-purple-700',
    'Venue Photography': 'bg-teal-100 text-teal-700',
    'Team Content': 'bg-amber-100 text-amber-700',
    'Interview': 'bg-rose-100 text-rose-700'
  }
  return colors[category] || 'bg-gray-100 text-gray-700'
}

onMounted(() => {
  createParticles()
  startAutoPlay()
})

onUnmounted(() => {
  stopAutoPlay()
})
</script>

<template>
  <div class="page-view">
    <!-- Particles Background -->
    <div class="particles" id="media-particles"></div>

    <!-- Hero Section (Matching Articles Page) -->
    <div class="hero-gradient relative overflow-hidden">
      <!-- Decorative elements with animations -->
      <div class="circle-drift-1 absolute top-0 right-0 w-96 h-96 bg-white/5 rounded-full"></div>
      <div class="circle-drift-2 absolute bottom-0 left-0 w-64 h-64 bg-white/5 rounded-full"></div>
      <div class="circle-drift-3 absolute top-1/2 right-1/4 w-32 h-32 bg-white/10 rounded-full"></div>

      <!-- Stats - Absolute Top Right -->
      <div class="stats-top-right">
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-video"></i>
          </div>
          <p class="stat-value-mini">{{ mediaStats.totalVideos }}</p>
          <p class="stat-label-mini">Videos</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-headphones"></i>
          </div>
          <p class="stat-value-mini">{{ mediaStats.audio }}</p>
          <p class="stat-label-mini">Audio</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-image"></i>
          </div>
          <p class="stat-value-mini">{{ mediaStats.images }}</p>
          <p class="stat-label-mini">Images</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-images"></i>
          </div>
          <p class="stat-value-mini">{{ mediaStats.galleries }}</p>
          <p class="stat-label-mini">Galleries</p>
        </div>
      </div>

      <div class="relative px-8 py-8">
        <div class="px-3 py-1 bg-white/20 backdrop-blur-sm rounded-full text-white text-xs font-semibold inline-flex items-center gap-2 mb-4">
          <i class="fas fa-photo-video"></i>
          AFC Asian Cup 2027
        </div>

        <h1 class="text-3xl font-bold text-white mb-2">Media Center</h1>
        <p class="text-teal-100 max-w-lg">Videos, audio, images, and photo galleries - all the multimedia content for AFC Asian Cup 2027.</p>

        <div class="flex flex-wrap gap-3 mt-6">
          <button @click="showUploadModal = true" class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
            <i class="fas fa-cloud-upload-alt"></i>
            Upload Media
          </button>
          <button @click="playFeatured" class="px-5 py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-xl font-semibold text-sm hover:bg-white/30 transition-all flex items-center gap-2">
            <i class="fas fa-play-circle"></i>
            Watch Featured
          </button>
        </div>
      </div>
    </div>

    <!-- Content Area with Padding -->
    <div class="px-6 py-6">

      <!-- Featured + Continue Watching Row -->
      <div class="featured-continue-row mb-10 fade-in-up">
        <!-- Featured Content (Left) -->
        <section class="featured-column">
          <div class="section-header-row">
            <h2 class="section-title-sm">
              <i class="fas fa-star text-amber-500"></i>
              Featured Content
            </h2>
            <router-link to="/media" class="view-all-link">View All <i class="fas fa-arrow-right"></i></router-link>
          </div>
          <!-- Featured Grid: Main + Up Next side by side -->
          <div class="featured-inner-grid">
            <!-- Main Featured Card with Carousel -->
            <div class="featured-carousel-wrapper" @mouseenter="pauseAutoPlay" @mouseleave="resumeAutoPlay">
              <div class="featured-main-card group" @click="playFeatured">
                <transition name="carousel-fade" mode="out-in">
                  <img :key="featuredVideo.id" :src="featuredVideo.thumbnail" :alt="featuredVideo.title" class="featured-main-img" />
                </transition>
                <div class="featured-main-overlay"></div>
                <!-- Badges -->
                <div class="featured-main-badges">
                  <span class="badge-featured"><i class="fas fa-star"></i> Featured</span>
                  <span class="badge-category">{{ featuredVideo.category }}</span>
                </div>
                <!-- Play Button -->
                <div class="featured-main-play">
                  <i class="fas fa-play"></i>
                </div>
                <!-- Content -->
                <div class="featured-main-content">
                  <div class="featured-main-meta">
                    <span><i class="fas fa-eye"></i> {{ featuredVideo.views }}</span>
                    <span><i class="fas fa-clock"></i> {{ featuredVideo.duration }}</span>
                    <span><i class="fas fa-heart"></i> {{ featuredVideo.likes }}</span>
                  </div>
                  <h3 class="featured-main-title">{{ featuredVideo.title }}</h3>
                  <p class="featured-main-desc">{{ featuredVideo.description }}</p>
                  <div class="featured-main-author">
                    <div class="author-avatar-sm">{{ getInitials(featuredVideo.author) }}</div>
                    <span>{{ featuredVideo.author }}</span>
                    <span class="dot">•</span>
                    <span>{{ featuredVideo.date }}</span>
                  </div>
                </div>
                <!-- Carousel Navigation Arrows -->
                <button class="carousel-arrow carousel-prev" @click.stop="prevFeatured">
                  <i class="fas fa-chevron-left"></i>
                </button>
                <button class="carousel-arrow carousel-next" @click.stop="nextFeatured">
                  <i class="fas fa-chevron-right"></i>
                </button>
                <!-- Carousel Dots -->
                <div class="carousel-dots">
                  <button
                    v-for="(video, index) in featuredVideos"
                    :key="video.id"
                    :class="['carousel-dot', { active: index === currentFeaturedIndex }]"
                    @click.stop="goToFeatured(index)"
                  ></button>
                </div>
              </div>
            </div>

            <!-- Up Next Videos (Vertical Stack) -->
            <div class="up-next-column">
              <h3 class="up-next-title"><i class="fas fa-list"></i> Up Next</h3>
              <div class="up-next-vertical">
                <div v-for="video in upNextVideos.slice(0, 3)" :key="video.id"
                     @click="goToMedia(video)"
                     class="up-next-card-compact group">
                  <div class="up-next-thumb-sm">
                    <img :src="video.thumbnail" :alt="video.title" class="up-next-img" />
                    <div class="up-next-play-sm"><i class="fas fa-play"></i></div>
                    <span class="up-next-duration">{{ video.duration }}</span>
                  </div>
                  <div class="up-next-info-sm">
                    <span class="up-next-category">{{ video.category }}</span>
                    <h4 class="up-next-card-title">{{ video.title }}</h4>
                    <span class="up-next-views"><i class="fas fa-eye"></i> {{ video.views }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>

        <!-- Continue Watching (Right) -->
        <section class="continue-column">
          <div class="section-header-row">
            <h2 class="section-title-sm">
              <i class="fas fa-history text-teal-500"></i>
              Continue Watching
            </h2>
            <router-link to="/media" class="view-all-link">View All ({{ watchHistory.length }}) <i class="fas fa-arrow-right"></i></router-link>
          </div>
          <div class="continue-vertical-scroll">
            <div v-for="item in watchHistory.slice(0, 3)" :key="item.id"
                 @click="goToMedia(item)"
                 class="continue-compact-card group">
              <div class="continue-compact-thumb">
                <img :src="item.thumbnail" :alt="item.title" class="continue-compact-img" />
                <div class="continue-compact-play"><i class="fas fa-play"></i></div>
                <span class="continue-compact-duration">{{ item.duration }}</span>
                <!-- Progress Bar -->
                <div class="continue-compact-progress">
                  <div class="continue-compact-progress-fill" :style="{ width: item.progress + '%' }"></div>
                </div>
              </div>
              <div class="continue-compact-info">
                <span class="continue-compact-category">{{ item.category }}</span>
                <h4 class="continue-compact-title">{{ item.title }}</h4>
                <div class="continue-compact-meta">
                  <span><i class="fas fa-redo"></i> {{ item.progress }}%</span>
                  <span><i class="fas fa-clock"></i> {{ item.timeRemaining }}</span>
                </div>
              </div>
            </div>
          </div>
        </section>
      </div>

      <!-- Trending -->
      <section class="trending-section mb-10 animate-in delay-2">
        <div class="trending-header">
          <h2 class="trending-title">
            <div class="trending-icon-wrapper">
              <i class="fas fa-fire"></i>
            </div>
            <div>
              <span class="block">Trending Now</span>
              <span class="trending-subtitle">Most watched this week</span>
            </div>
          </h2>
          <div class="trending-nav">
            <button class="trending-nav-btn" @click="scrollTrending('left')">
              <i class="fas fa-chevron-left"></i>
            </button>
            <button class="trending-nav-btn" @click="scrollTrending('right')">
              <i class="fas fa-chevron-right"></i>
            </button>
            <router-link to="/media" class="trending-view-all">
              View All <i class="fas fa-arrow-right"></i>
            </router-link>
          </div>
        </div>
        <div class="trending-scroll" ref="trendingScrollRef">
          <div v-for="(media, index) in trendingMedia" :key="media.id"
               class="trending-card group">
            <!-- Rank Number -->
            <div class="trending-rank">
              {{ index + 1 }}
            </div>
            <div class="trending-card-inner" @click="goToMedia(media)">
              <div class="trending-thumbnail">
                <img :src="media.thumbnail" :alt="media.title" class="trending-img" />
                <div class="trending-overlay"></div>
                <!-- Play Button -->
                <div class="trending-play-btn">
                  <i :class="media.type === 'video' ? 'fas fa-play' : media.type === 'audio' ? 'fas fa-headphones' : media.type === 'image' ? 'fas fa-expand' : 'fas fa-images'"></i>
                </div>
                <!-- Badges -->
                <div class="trending-badges">
                  <span v-if="media.isNew" class="trending-new-badge">
                    <i class="fas fa-sparkles"></i> New
                  </span>
                  <span v-if="index < 3" class="trending-hot-badge">
                    <i class="fas fa-fire-alt"></i> Hot
                  </span>
                </div>
                <!-- Duration -->
                <div class="trending-duration">
                  <i :class="media.type === 'video' ? 'fas fa-play-circle' : media.type === 'audio' ? 'fas fa-headphones' : media.type === 'image' ? 'fas fa-image' : 'fas fa-images'"></i>
                  {{ media.duration || (media.photoCount ? media.photoCount + ' photos' : 'View') }}
                </div>
                <!-- Hover Actions -->
                <div class="trending-actions">
                  <button
                    class="trending-action-btn"
                    :class="{ active: isMediaLiked(media.id) }"
                    @click.stop="toggleLikeMedia(media.id)"
                    title="Like">
                    <i :class="isMediaLiked(media.id) ? 'fas fa-heart' : 'far fa-heart'"></i>
                  </button>
                  <button
                    class="trending-action-btn"
                    :class="{ 'saved': isMediaSaved(media.id) }"
                    @click.stop="toggleSaveMedia(media.id)"
                    title="Save">
                    <i :class="isMediaSaved(media.id) ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                  </button>
                  <ContentActionsDropdown
                    :show-download="true"
                    @add-to-collection="openAddToCollection(media)"
                    @share="shareMedia(media)"
                    @download="() => showDownloadAlert(media.title)"
                    @copy-link="() => copyMediaLink(media.id)"
                  />
                </div>
              </div>
              <div class="trending-info">
                <div class="trending-info-header">
                  <span :class="['trending-type-badge', media.type]">
                    <i :class="media.type === 'video' ? 'fas fa-video' : media.type === 'audio' ? 'fas fa-headphones' : media.type === 'image' ? 'fas fa-image' : 'fas fa-images'"></i>
                    {{ media.type === 'video' ? 'Video' : media.type === 'audio' ? 'Audio' : media.type === 'image' ? 'Image' : 'Gallery' }}
                  </span>
                  <span :class="['category-tag', media.category.toLowerCase().split(' ')[0]]">{{ media.category }}</span>
                </div>
                <h4 class="trending-card-title">{{ media.title }}</h4>
                <div class="trending-author">
                  <div class="trending-author-avatar">
                    {{ media.author.charAt(0) }}
                  </div>
                  <span class="trending-author-name">{{ media.author }}</span>
                </div>
                <div class="trending-stats">
                  <span class="trending-stat">
                    <i class="fas fa-eye"></i> {{ media.views }}
                  </span>
                  <span class="trending-stat">
                    <i class="fas fa-heart"></i> {{ Math.floor(Math.random() * 500) + 100 }}
                  </span>
                  <span class="trending-stat">
                    <i class="fas fa-clock"></i> {{ media.date }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- AI Recommendations Section -->
      <section v-if="showAIFeatures" class="ai-recommendations-section mb-10 animate-in delay-2">
        <div class="bg-gradient-to-br from-teal-50 via-cyan-50 to-teal-50 rounded-2xl border border-teal-100 overflow-hidden">
          <!-- Section Header -->
          <div class="px-6 py-4 flex items-center justify-between border-b border-teal-100/50">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-cyan-500 flex items-center justify-center shadow-lg shadow-teal-200">
                <i class="fas fa-brain text-white text-sm"></i>
              </div>
              <div>
                <h2 class="text-lg font-bold text-gray-800">AI Recommendations</h2>
                <p class="text-xs text-gray-500">Personalized content suggestions powered by AI</p>
              </div>
            </div>
            <div class="flex items-center gap-2">
              <button
                v-for="type in [{ id: 'forYou', label: 'For You', icon: 'fas fa-user' }, { id: 'similar', label: 'Similar', icon: 'fas fa-clone' }, { id: 'trending', label: 'Trending', icon: 'fas fa-fire' }]"
                :key="type.id"
                @click="recommendationType = type.id as any; loadAIRecommendations()"
                :class="['px-3 py-1.5 rounded-lg text-xs font-medium transition-colors flex items-center gap-1.5', recommendationType === type.id ? 'bg-teal-500 text-white' : 'bg-white/80 text-gray-600 hover:bg-white']"
              >
                <i :class="type.icon"></i>
                {{ type.label }}
              </button>
            </div>
          </div>

          <!-- Recommendations Content -->
          <div class="p-4">
            <!-- Loading State -->
            <div v-if="isLoadingRecommendations" class="flex items-center justify-center py-12">
              <div class="w-10 h-10 border-3 border-teal-500 border-t-transparent rounded-full animate-spin"></div>
            </div>

            <!-- Recommendations Grid -->
            <div v-else-if="aiRecommendations.length > 0" class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
              <div
                v-for="media in aiRecommendations"
                :key="'rec-' + media.id"
                @click="goToMedia(media)"
                class="group cursor-pointer"
              >
                <div class="relative aspect-video rounded-xl overflow-hidden bg-gray-100 mb-2">
                  <img :src="media.thumbnail" :alt="media.title" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300" />
                  <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent opacity-0 group-hover:opacity-100 transition-opacity"></div>
                  <div class="absolute bottom-2 right-2 px-1.5 py-0.5 bg-black/70 text-white text-[10px] font-medium rounded">
                    {{ media.duration || (media.photoCount ? media.photoCount + ' photos' : 'View') }}
                  </div>
                  <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
                    <div class="w-10 h-10 rounded-full bg-white/90 flex items-center justify-center">
                      <i class="fas fa-play text-teal-500 ml-0.5"></i>
                    </div>
                  </div>
                  <!-- AI Match Badge -->
                  <div class="absolute top-2 left-2 px-1.5 py-0.5 bg-teal-500 text-white text-[9px] font-medium rounded flex items-center gap-1">
                    <i class="fas fa-sparkles text-[8px]"></i>
                    {{ Math.floor(Math.random() * 15) + 85 }}% match
                  </div>
                </div>
                <h4 class="text-xs font-medium text-gray-800 line-clamp-2 group-hover:text-teal-600 transition-colors">{{ media.title }}</h4>
                <p class="text-[10px] text-gray-400 mt-0.5">{{ media.views }} views</p>
              </div>
            </div>

            <!-- Empty State -->
            <div v-else class="text-center py-12 text-gray-400">
              <i class="fas fa-brain text-4xl mb-3 opacity-30"></i>
              <p class="text-sm">Select a recommendation type to get personalized suggestions</p>
              <button @click="loadAIRecommendations" class="mt-3 px-4 py-2 bg-teal-500 text-white text-sm rounded-lg hover:bg-teal-600 transition-colors">
                Get Recommendations
              </button>
            </div>
          </div>
        </div>
      </section>

      <!-- All Media Section -->
      <div class="all-media-wrapper bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden mb-10 animate-in delay-3">
        <!-- Section Header / Toolbar -->
        <div class="border-b border-gray-100">
          <!-- Top Row - Title and Primary Actions -->
          <div class="px-4 py-3 flex items-center justify-between">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
                <i class="fas fa-photo-film text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Media Library</span>
                <span class="text-xs font-medium text-gray-500">{{ filteredMedia.length }} items</span>
              </div>
            </h2>
            <div class="flex items-center gap-2">
              <!-- Primary Actions -->
              <button @click="showUploadModal = true" class="px-4 py-2 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-lg text-sm font-medium hover:from-teal-600 hover:to-teal-700 transition-all flex items-center gap-2 shadow-sm shadow-teal-200">
                <i class="fas fa-cloud-arrow-up"></i>
                Upload Media
              </button>
              <button class="px-3 py-2 bg-white border border-gray-200 text-gray-700 rounded-lg text-sm font-medium hover:bg-gray-50 hover:border-gray-300 transition-all flex items-center gap-2">
                <i class="fas fa-folder-plus text-amber-500"></i>
                New Folder
              </button>

              <!-- Divider -->
              <div class="w-px h-8 bg-gray-200 mx-1"></div>

              <!-- Refresh Button -->
              <button
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
                    'absolute left-3 top-1/2 -translate-y-1/2 text-sm transition-colors',
                    isAISearchMode ? 'fas fa-brain text-teal-500' : 'fas fa-search text-gray-400'
                  ]"></i>
                  <input
                    v-model="unifiedSearchQuery"
                    type="text"
                    :placeholder="isAISearchMode ? 'Ask AI: Show me highlights from Japan matches...' : 'Search media...'"
                    @keyup.enter="handleUnifiedSearch"
                    @input="handleSearchInput"
                    :class="[
                      'w-full pl-9 pr-20 py-2 text-sm focus:outline-none transition-all',
                      showAIFeatures ? 'rounded-r-lg' : 'rounded-lg',
                      isAISearchMode
                        ? 'bg-gradient-to-r from-teal-50 to-cyan-50 border border-teal-200 focus:ring-2 focus:ring-teal-400 focus:border-transparent placeholder:text-teal-400'
                        : 'bg-white border border-gray-200 focus:ring-2 focus:ring-teal-500 focus:border-transparent',
                      !showAIFeatures && 'rounded-l-lg'
                    ]"
                  >
                  <!-- Clear & Search Buttons -->
                  <div class="absolute right-2 top-1/2 -translate-y-1/2 flex items-center gap-1">
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
                class="absolute left-0 top-full mt-2 w-full bg-white rounded-xl shadow-lg border border-teal-100 py-2 z-50"
              >
                <div class="px-3 py-1.5 text-xs font-semibold text-teal-500 flex items-center gap-2">
                  <i class="fas fa-lightbulb"></i>
                  Try asking:
                </div>
                <button
                  v-for="suggestion in nlSearchSuggestions"
                  :key="suggestion"
                  @click="unifiedSearchQuery = suggestion; handleUnifiedSearch()"
                  class="w-full px-3 py-2 text-left text-sm text-gray-600 hover:bg-teal-50 hover:text-teal-700 transition-colors"
                >
                  <i class="fas fa-wand-magic-sparkles text-teal-400 mr-2 text-xs"></i>
                  {{ suggestion }}
                </button>
              </div>

              <!-- AI Processing Indicator -->
              <div
                v-if="isProcessingNLSearch"
                class="absolute left-0 top-full mt-2 w-full bg-white rounded-xl shadow-lg border border-teal-100 p-4 z-50"
              >
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 border-2 border-teal-500 border-t-transparent rounded-full animate-spin"></div>
                  <div>
                    <p class="text-sm font-medium text-gray-700">AI is analyzing your query...</p>
                    <p class="text-xs text-gray-400">Finding relevant media content</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Media Type Filter Dropdown -->
            <div class="relative">
              <button
                @click="showMediaTypeFilter = !showMediaTypeFilter"
                :class="[
                  'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                  selectedMediaTypes.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                ]"
              >
                <i class="fas fa-video text-sm"></i>
                <span>{{ selectedMediaTypes.length > 0 ? `${selectedMediaTypes.length} Types` : 'Media Type' }}</span>
                <i :class="showMediaTypeFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
              </button>

              <!-- Dropdown Menu -->
              <div
                v-if="showMediaTypeFilter"
                class="absolute left-0 top-full mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
              >
                <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Select Media Types</div>
                <div class="max-h-48 overflow-y-auto">
                  <button
                    v-for="type in mediaTypeOptions"
                    :key="type.id"
                    @click="toggleMediaType(type.id)"
                    :class="[
                      'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                      isMediaTypeSelected(type.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                    ]"
                  >
                    <div :class="[
                      'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                      isMediaTypeSelected(type.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                    ]">
                      <i v-if="isMediaTypeSelected(type.id)" class="fas fa-check text-white text-[8px]"></i>
                    </div>
                    <i :class="[type.icon, type.color, 'text-sm']"></i>
                    <span class="flex-1">{{ type.label }}</span>
                  </button>
                </div>
                <div class="my-2 border-t border-gray-100"></div>
                <div class="px-3 flex gap-2">
                  <button
                    @click="selectedMediaTypes = []; showMediaTypeFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                  >
                    Clear All
                  </button>
                  <button
                    @click="showMediaTypeFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                  >
                    Apply
                  </button>
                </div>
              </div>
              <div v-if="showMediaTypeFilter" @click="showMediaTypeFilter = false" class="fixed inset-0 z-40"></div>
            </div>

            <!-- Category Filter Dropdown -->
            <div class="relative">
              <button
                @click="showCategoryFilter = !showCategoryFilter"
                :class="[
                  'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                  selectedCategories.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                ]"
              >
                <i class="fas fa-layer-group text-sm"></i>
                <span>{{ selectedCategories.length > 0 ? `${selectedCategories.length} Categories` : 'Category' }}</span>
                <i :class="showCategoryFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
              </button>

              <!-- Dropdown Menu -->
              <div
                v-if="showCategoryFilter"
                class="absolute left-0 top-full mt-2 w-64 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
              >
                <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Select Categories</div>
                <div class="max-h-48 overflow-y-auto">
                  <button
                    v-for="cat in categories"
                    :key="cat"
                    @click="toggleCategorySelection(cat)"
                    :class="[
                      'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                      isCategorySelected(cat) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                    ]"
                  >
                    <div :class="[
                      'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                      isCategorySelected(cat) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                    ]">
                      <i v-if="isCategorySelected(cat)" class="fas fa-check text-white text-[8px]"></i>
                    </div>
                    <span class="flex-1">{{ cat }}</span>
                  </button>
                </div>
                <div class="my-2 border-t border-gray-100"></div>
                <div class="px-3 flex gap-2">
                  <button
                    @click="selectedCategories = []; showCategoryFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                  >
                    Clear All
                  </button>
                  <button
                    @click="showCategoryFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                  >
                    Apply
                  </button>
                </div>
              </div>
              <div v-if="showCategoryFilter" @click="showCategoryFilter = false" class="fixed inset-0 z-40"></div>
            </div>

            <!-- Tags Filter -->
            <div class="relative">
              <button
                @click="showTagFilter = !showTagFilter"
                :class="[
                  'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                  selectedTags.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                ]"
              >
                <i class="fas fa-tags text-sm"></i>
                <span>{{ selectedTags.length > 0 ? `${selectedTags.length} Tags` : 'Tags' }}</span>
                <i :class="showTagFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
              </button>

              <!-- Dropdown Menu -->
              <div
                v-if="showTagFilter"
                class="absolute left-0 top-full mt-2 w-64 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
              >
                <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Select Tags</div>
                <div class="max-h-48 overflow-y-auto">
                  <button
                    v-for="tag in allTags"
                    :key="tag"
                    @click="toggleTagFilter(tag)"
                    :class="[
                      'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                      isTagSelected(tag) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                    ]"
                  >
                    <div :class="[
                      'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                      isTagSelected(tag) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                    ]">
                      <i v-if="isTagSelected(tag)" class="fas fa-check text-white text-[8px]"></i>
                    </div>
                    <span class="flex-1">{{ tag }}</span>
                  </button>
                </div>

                <div class="my-2 border-t border-gray-100"></div>

                <div class="px-3 flex gap-2">
                  <button
                    @click="selectedTags = []; showTagFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                  >
                    Clear All
                  </button>
                  <button
                    @click="showTagFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                  >
                    Apply
                  </button>
                </div>
              </div>

              <!-- Click outside to close -->
              <div v-if="showTagFilter" @click="showTagFilter = false" class="fixed inset-0 z-40"></div>
            </div>

            <!-- Saved & Shared Filter Dropdown -->
            <div class="relative">
              <button
                @click="showStatusFilter = !showStatusFilter"
                :class="[
                  'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                  selectedStatusFilters.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                ]"
              >
                <i class="fas fa-bookmark text-sm"></i>
                <span>{{ selectedStatusFilters.length > 0 ? `${selectedStatusFilters.length} Saved & Shared` : 'Saved & Shared' }}</span>
                <i :class="showStatusFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
              </button>

              <!-- Dropdown Menu -->
              <div
                v-if="showStatusFilter"
                class="absolute left-0 top-full mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
              >
                <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Filter by Status</div>
                <div class="max-h-48 overflow-y-auto">
                  <button
                    v-for="option in statusFilterOptions"
                    :key="option.id"
                    @click="toggleStatusFilter(option.id)"
                    :class="[
                      'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                      isStatusSelected(option.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                    ]"
                  >
                    <div :class="[
                      'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                      isStatusSelected(option.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                    ]">
                      <i v-if="isStatusSelected(option.id)" class="fas fa-check text-white text-[8px]"></i>
                    </div>
                    <i :class="[option.icon, option.color]"></i>
                    <span class="flex-1">{{ option.label }}</span>
                  </button>
                </div>

                <div class="my-2 border-t border-gray-100"></div>

                <div class="px-3 flex gap-2">
                  <button
                    @click="selectedStatusFilters = []; showStatusFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                  >
                    Clear All
                  </button>
                  <button
                    @click="showStatusFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                  >
                    Apply
                  </button>
                </div>
              </div>

              <!-- Click outside to close -->
              <div v-if="showStatusFilter" @click="showStatusFilter = false" class="fixed inset-0 z-40"></div>
            </div>

              <!-- Sort Options with Order Toggle -->
              <div class="relative ml-auto flex items-center">
                <button
                  @click="showSortDropdown = !showSortDropdown"
                  class="flex items-center gap-2 px-3 py-1.5 rounded-l-lg text-xs font-medium transition-all border border-r-0 bg-white border-gray-200 text-gray-600 hover:bg-gray-50"
                >
                  <i :class="[currentSortOption.icon, 'text-sm text-teal-500']"></i>
                  <span>{{ currentSortOption.label }}</span>
                  <i :class="showSortDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>
                <button
                  @click="sortOrder = sortOrder === 'asc' ? 'desc' : 'asc'"
                  class="flex items-center justify-center w-8 h-8 rounded-r-lg text-xs font-medium transition-all border bg-white border-gray-200 text-gray-600 hover:bg-gray-50 hover:text-teal-600"
                  :title="sortOrder === 'asc' ? 'Ascending order - Click for descending' : 'Descending order - Click for ascending'"
                >
                  <i :class="sortOrder === 'asc' ? 'fas fa-arrow-up' : 'fas fa-arrow-down'" class="text-sm text-teal-500"></i>
                </button>

                <!-- Dropdown Menu -->
                <div
                  v-if="showSortDropdown"
                  class="absolute left-0 top-full mt-2 w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                >
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Sort By</div>
                  <div class="max-h-64 overflow-y-auto">
                    <button
                      v-for="option in sortOptionsList"
                      :key="option.value"
                      @click="selectSortOption(option.value)"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        sortBy === option.value ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <i :class="[option.icon, 'text-sm w-4', sortBy === option.value ? 'text-teal-500' : 'text-gray-400']"></i>
                      <span class="flex-1">{{ option.label }}</span>
                      <i v-if="sortBy === option.value" class="fas fa-check text-teal-500 text-xs"></i>
                    </button>
                  </div>
                </div>
                <div v-if="showSortDropdown" @click="showSortDropdown = false" class="fixed inset-0 z-40"></div>
              </div>

              <!-- View Toggle -->
              <div class="flex items-center gap-0.5 bg-white border border-gray-200 rounded-lg p-1">
                <button
                  @click="viewMode = 'grid'"
                  :class="['px-2.5 py-1 rounded-md transition-all', viewMode === 'grid' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
                  title="Grid view"
                >
                  <i class="fas fa-th-large text-xs"></i>
                </button>
                <button
                  @click="viewMode = 'list'"
                  :class="['px-2.5 py-1 rounded-md transition-all', viewMode === 'list' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
                  title="List view"
                >
                  <i class="fas fa-list text-xs"></i>
                </button>
              </div>
          </div>
        </div>

        <!-- Active Filters Bar -->
        <div v-if="activeFiltersCount > 0" class="flex items-center gap-3 mb-4 p-3 bg-white rounded-xl border border-gray-100 shadow-sm">
          <div class="flex items-center gap-2 px-2 py-1 bg-gray-100 rounded-lg">
            <i class="fas fa-filter text-gray-400 text-xs"></i>
            <span class="text-xs font-medium text-gray-600">Active Filters</span>
          </div>
          <div class="flex flex-wrap gap-2 flex-1">
            <!-- Search Filter -->
            <span v-if="unifiedSearchQuery || searchQuery" :class="[
              'px-2.5 py-1 rounded-lg text-xs font-medium flex items-center gap-1.5 border',
              isAISearchMode ? 'bg-teal-50 text-teal-700 border-teal-200' : 'bg-gray-100 text-gray-700 border-gray-200'
            ]">
              <i :class="['text-[10px]', isAISearchMode ? 'fas fa-wand-magic-sparkles text-teal-500' : 'fas fa-search']"></i>
              <span v-if="isAISearchMode" class="text-teal-500 font-semibold">AI:</span>
              "{{ unifiedSearchQuery || searchQuery }}"
              <button @click="clearUnifiedSearch" :class="['ml-1 rounded-full w-4 h-4 flex items-center justify-center', isAISearchMode ? 'hover:text-teal-900 hover:bg-teal-100' : 'hover:text-gray-900 hover:bg-gray-200']"><i class="fas fa-times text-[10px]"></i></button>
            </span>
            <!-- Folder Filter -->
            <span v-if="selectedFolder" class="px-2.5 py-1 bg-blue-50 text-blue-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-blue-100">
              <i class="fas fa-folder text-[10px]"></i>
              {{ getFolderName(selectedFolder) }}
              <button @click="selectedFolder = null" class="ml-1 hover:text-blue-900 hover:bg-blue-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
            </span>
            <!-- Media Type Filters -->
            <span
              v-for="typeId in selectedMediaTypes"
              :key="'type-' + typeId"
              class="px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-teal-100"
            >
              <i class="fas fa-photo-film text-[10px]"></i>
              {{ mediaTypeOptions.find(t => t.id === typeId)?.label || typeId }}
              <button @click="toggleMediaTypeFilter(typeId)" class="ml-1 hover:text-teal-900 hover:bg-teal-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
            </span>
            <!-- Category Filters -->
            <span
              v-for="cat in selectedCategories"
              :key="'cat-' + cat"
              class="px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-teal-100"
            >
              <i class="fas fa-layer-group text-[10px]"></i>
              {{ cat }}
              <button @click="toggleCategorySelection(cat)" class="ml-1 hover:text-teal-900 hover:bg-teal-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
            </span>
            <!-- Tag Filters -->
            <span
              v-for="tag in selectedTags"
              :key="'tag-' + tag"
              class="px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-teal-100"
            >
              <i class="fas fa-tag text-[10px]"></i>
              {{ tag }}
              <button @click="toggleTagFilter(tag)" class="ml-1 hover:text-teal-900 hover:bg-teal-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
            </span>
            <!-- Status Filters (Saved & Shared) -->
            <span
              v-for="status in selectedStatusFilters"
              :key="'status-' + status"
              class="px-2.5 py-1 bg-amber-50 text-amber-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-amber-100"
            >
              <i :class="[status === 'saved' ? 'fas fa-bookmark' : 'fas fa-share-alt', 'text-[10px]']"></i>
              {{ status === 'saved' ? 'My Saved' : 'Shared with me' }}
              <button @click="toggleStatusFilter(status)" class="ml-1 hover:text-amber-900 hover:bg-amber-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
            </span>
          </div>
          <button @click="clearAllFilters" class="px-3 py-1.5 text-xs font-medium text-red-600 hover:text-red-700 hover:bg-red-50 rounded-lg transition-colors flex items-center gap-1.5">
            <i class="fas fa-times-circle"></i>
            Clear all
          </button>
        </div>

        <div class="flex min-h-[500px]">
          <!-- Folder Tree Sidebar -->
          <div
            :class="[
              'border-r border-gray-100 bg-gray-50/50 transition-all duration-300 relative',
              isSidebarCollapsed ? 'w-12 min-w-[48px]' : 'w-64 min-w-[256px]'
            ]"
          >
            <!-- Collapse/Expand Button -->
            <button
              @click="isSidebarCollapsed = !isSidebarCollapsed"
              class="absolute -right-3 top-4 w-6 h-6 bg-white border border-gray-200 rounded-full shadow-sm flex items-center justify-center text-gray-500 hover:text-teal-600 hover:border-teal-300 transition-all z-10"
              :title="isSidebarCollapsed ? 'Expand sidebar' : 'Collapse sidebar'"
            >
              <i :class="isSidebarCollapsed ? 'fas fa-chevron-right' : 'fas fa-chevron-left'" class="text-[10px]"></i>
            </button>

            <!-- Collapsed State - Icons Only -->
            <div v-if="isSidebarCollapsed" class="p-2 pt-4 space-y-2">
              <button
                v-for="view in viewNavItems"
                :key="'collapsed-' + view.id"
                @click="currentView = view.id as any; isSidebarCollapsed = false"
                :class="[
                  'w-8 h-8 rounded-lg flex items-center justify-center transition-all',
                  currentView === view.id ? 'bg-teal-100 text-teal-600' : 'hover:bg-gray-100 ' + view.color
                ]"
                :title="view.name"
              >
                <i :class="[view.icon, 'text-sm']"></i>
              </button>
              <div class="border-t border-gray-200 my-2"></div>
              <button
                v-for="folder in folderTree[0]?.children || []"
                :key="folder.id"
                @click="selectFolder(folder.id); isSidebarCollapsed = false"
                :class="[
                  'w-8 h-8 rounded-lg flex items-center justify-center transition-all',
                  selectedFolder === folder.id ? 'bg-teal-100 text-teal-600' : 'hover:bg-gray-100 text-amber-500'
                ]"
                :title="folder.name"
              >
                <i class="fas fa-folder text-sm"></i>
              </button>
            </div>

            <!-- Expanded State - Full Tree -->
            <div v-else class="p-4">
              <!-- View Navigation -->
              <div class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-3">Views</div>
              <div class="space-y-1 mb-4">
                <button
                  v-for="view in viewNavItems"
                  :key="view.id"
                  @click="currentView = view.id as any"
                  :class="[
                    'w-full flex items-center gap-3 px-3 py-2 rounded-lg transition-all text-left',
                    currentView === view.id
                      ? 'bg-teal-100 text-teal-700'
                      : 'hover:bg-gray-100 text-gray-600'
                  ]"
                >
                  <i :class="[view.icon, 'text-sm w-4', currentView === view.id ? 'text-teal-600' : view.color]"></i>
                  <span class="text-sm font-medium flex-1">{{ view.name }}</span>
                  <span :class="[
                    'text-xs px-1.5 py-0.5 rounded-full min-w-[20px] text-center',
                    currentView === view.id ? 'bg-teal-200 text-teal-700' : 'bg-gray-100 text-gray-500'
                  ]">
                    {{ viewCounts[view.id as keyof typeof viewCounts] }}
                  </span>
                </button>
              </div>

              <div class="border-t border-gray-200 my-4"></div>

              <div class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-3">Folders</div>

              <!-- Folder Tree -->
              <div class="space-y-1">
                <template v-for="folder in folderTree" :key="folder.id">
                  <div>
                    <div
                      @click="selectFolder(folder.id)"
                      :class="[
                        'flex items-center gap-2 px-2 py-1.5 rounded-lg cursor-pointer transition-all group',
                        selectedFolder === folder.id ? 'bg-teal-100 text-teal-700' : 'hover:bg-gray-100 text-gray-700'
                      ]"
                    >
                      <button
                        v-if="folder.children && folder.children.length > 0"
                        @click.stop="toggleFolder(folder.id)"
                        class="w-4 h-4 flex items-center justify-center text-gray-400 hover:text-gray-600"
                      >
                        <i :class="isFolderExpanded(folder.id) ? 'fas fa-chevron-down' : 'fas fa-chevron-right'" class="text-[10px]"></i>
                      </button>
                      <span v-else class="w-4"></span>
                      <i :class="[folder.icon, selectedFolder === folder.id ? 'text-teal-600' : 'text-amber-500']" class="text-sm"></i>
                      <span class="text-sm font-medium truncate">{{ folder.name }}</span>
                    </div>

                    <!-- Level 1 Children -->
                    <div v-if="isFolderExpanded(folder.id) && folder.children" class="ml-4 mt-1 space-y-1">
                      <template v-for="child in folder.children" :key="child.id">
                        <div>
                          <div
                            @click="selectFolder(child.id)"
                            :class="[
                              'flex items-center gap-2 px-2 py-1.5 rounded-lg cursor-pointer transition-all',
                              selectedFolder === child.id ? 'bg-teal-100 text-teal-700' : 'hover:bg-gray-100 text-gray-700'
                            ]"
                          >
                            <button
                              v-if="child.children && child.children.length > 0"
                              @click.stop="toggleFolder(child.id)"
                              class="w-4 h-4 flex items-center justify-center text-gray-400 hover:text-gray-600"
                            >
                              <i :class="isFolderExpanded(child.id) ? 'fas fa-chevron-down' : 'fas fa-chevron-right'" class="text-[10px]"></i>
                            </button>
                            <span v-else class="w-4"></span>
                            <i :class="[child.icon, selectedFolder === child.id ? 'text-teal-600' : 'text-amber-500']" class="text-sm"></i>
                            <span class="text-sm truncate">{{ child.name }}</span>
                          </div>

                          <!-- Level 2 Children -->
                          <div v-if="isFolderExpanded(child.id) && child.children" class="ml-4 mt-1 space-y-1">
                            <div
                              v-for="grandchild in child.children"
                              :key="grandchild.id"
                              @click="selectFolder(grandchild.id)"
                              :class="[
                                'flex items-center gap-2 px-2 py-1.5 rounded-lg cursor-pointer transition-all',
                                selectedFolder === grandchild.id ? 'bg-teal-100 text-teal-700' : 'hover:bg-gray-100 text-gray-700'
                              ]"
                            >
                              <span class="w-4"></span>
                              <i class="fas fa-folder text-amber-500 text-sm"></i>
                              <span class="text-sm truncate">{{ grandchild.name }}</span>
                            </div>
                          </div>
                        </div>
                      </template>
                    </div>
                  </div>
                </template>
              </div>
            </div>
          </div>

          <!-- Main Content Area -->
          <div class="flex-1 p-4">
            <!-- Breadcrumb -->
            <div v-if="!isSelectionMode || selectedCount === 0" class="flex items-center justify-between mb-5">
              <div class="flex items-center gap-2 text-sm bg-white px-4 py-2 rounded-xl border border-gray-100 shadow-sm">
                <button @click="currentView = 'all'; selectedFolder = null" class="w-7 h-7 rounded-lg bg-gray-100 text-gray-500 hover:bg-teal-100 hover:text-teal-600 flex items-center justify-center transition-all">
                  <i class="fas fa-home text-xs"></i>
                </button>
                <i class="fas fa-chevron-right text-gray-300 text-xs"></i>
                <span class="text-gray-700 font-medium flex items-center gap-2 px-2 py-1 bg-gray-50 rounded-lg">
                  <i :class="[viewNavItems.find(v => v.id === currentView)?.icon, viewNavItems.find(v => v.id === currentView)?.color, 'text-sm']"></i>
                  {{ viewNavItems.find(v => v.id === currentView)?.name }}
                </span>
                <template v-if="selectedFolder">
                  <i class="fas fa-chevron-right text-gray-300 text-xs"></i>
                  <span class="text-gray-600 px-2 py-1 bg-gray-50 rounded-lg flex items-center gap-2">
                    <i class="fas fa-folder text-amber-500 text-xs"></i>
                    {{ getFolderName(selectedFolder) }}
                  </span>
                </template>
              </div>
              <div class="flex items-center gap-2 text-xs text-gray-500 bg-white px-3 py-2 rounded-lg border border-gray-100">
                <i class="fas fa-photo-film text-teal-500"></i>
                <span class="font-medium">{{ filteredMedia.length }}</span>
                <span>items</span>
              </div>
            </div>

            <!-- Selection Toolbar (Bulk Actions) -->
            <div v-if="isSelectionMode && selectedCount > 0" class="flex items-center justify-between mb-5 p-3 bg-gradient-to-r from-teal-500 to-teal-600 rounded-xl shadow-lg shadow-teal-200/50">
              <div class="flex items-center gap-4">
                <!-- Select All Checkbox -->
                <button
                  @click="isAllSelected ? clearMediaSelection() : selectAllMedia()"
                  class="flex items-center gap-2 text-white hover:bg-white/10 px-3 py-1.5 rounded-lg transition-all"
                >
                  <div :class="[
                    'w-5 h-5 rounded border-2 flex items-center justify-center transition-all',
                    isAllSelected ? 'bg-white border-white' : isSomeSelected ? 'bg-white/50 border-white' : 'border-white/70'
                  ]">
                    <i v-if="isAllSelected" class="fas fa-check text-teal-600 text-[10px]"></i>
                    <i v-else-if="isSomeSelected" class="fas fa-minus text-teal-600 text-[10px]"></i>
                  </div>
                  <span class="text-sm font-medium">{{ isAllSelected ? 'Deselect all' : 'Select all' }}</span>
                </button>

                <!-- Selected Count -->
                <div class="flex items-center gap-2 px-3 py-1.5 bg-white/20 rounded-lg">
                  <i class="fas fa-check-circle text-white/80"></i>
                  <span class="text-white text-sm font-semibold">{{ selectedCount }} selected</span>
                </div>
              </div>

              <!-- Bulk Actions -->
              <div class="flex items-center gap-2">
                <template v-if="currentView !== 'trash'">
                  <button
                    @click="bulkDownload"
                    class="flex items-center gap-2 px-3 py-2 bg-white/20 hover:bg-white/30 text-white rounded-lg transition-all text-sm font-medium"
                    title="Download selected"
                  >
                    <i class="fas fa-download"></i>
                    <span class="hidden sm:inline">Download</span>
                  </button>
                  <button
                    @click="bulkShare"
                    class="flex items-center gap-2 px-3 py-2 bg-white/20 hover:bg-white/30 text-white rounded-lg transition-all text-sm font-medium"
                    title="Share selected"
                  >
                    <i class="fas fa-share-alt"></i>
                    <span class="hidden sm:inline">Share</span>
                  </button>
                  <button
                    @click="bulkAddToPlaylist"
                    class="flex items-center gap-2 px-3 py-2 bg-white/20 hover:bg-white/30 text-white rounded-lg transition-all text-sm font-medium"
                    title="Add to playlist"
                  >
                    <i class="fas fa-list-ul"></i>
                    <span class="hidden sm:inline">Playlist</span>
                  </button>
                  <div class="w-px h-6 bg-white/30 mx-1"></div>
                  <button
                    @click="bulkMoveToTrash"
                    class="flex items-center gap-2 px-3 py-2 bg-red-500/80 hover:bg-red-500 text-white rounded-lg transition-all text-sm font-medium"
                    title="Move to trash"
                  >
                    <i class="fas fa-trash-alt"></i>
                    <span class="hidden sm:inline">Delete</span>
                  </button>
                </template>
                <template v-else>
                  <button
                    @click="bulkRestore"
                    class="flex items-center gap-2 px-3 py-2 bg-white/20 hover:bg-white/30 text-white rounded-lg transition-all text-sm font-medium"
                    title="Restore selected"
                  >
                    <i class="fas fa-undo"></i>
                    <span class="hidden sm:inline">Restore</span>
                  </button>
                  <button
                    @click="bulkPermanentDelete"
                    class="flex items-center gap-2 px-3 py-2 bg-red-500/80 hover:bg-red-500 text-white rounded-lg transition-all text-sm font-medium"
                    title="Delete permanently"
                  >
                    <i class="fas fa-trash"></i>
                    <span class="hidden sm:inline">Delete Forever</span>
                  </button>
                </template>

                <!-- Cancel -->
                <button
                  @click="toggleSelectionMode"
                  class="flex items-center gap-2 px-3 py-2 bg-white text-teal-600 hover:bg-gray-100 rounded-lg transition-all text-sm font-medium ml-2"
                >
                  <i class="fas fa-times"></i>
                  <span class="hidden sm:inline">Cancel</span>
                </button>
              </div>
            </div>

            <!-- Content Area with Media Grid/List -->
        <div class="content-area">
          <div class="content-area-inner">
            <!-- Grid View -->
            <div v-if="viewMode === 'grid'" class="media-grid">
              <div v-for="media in paginatedMedia" :key="media.id"
                   @click="isSelectionMode ? toggleMediaSelection(media.id) : goToMedia(media)"
                   class="media-card-enhanced group bg-white rounded-2xl overflow-hidden cursor-pointer transition-all duration-300 hover:-translate-y-1.5 border border-gray-100 shadow-sm hover:shadow-lg hover:border-teal-200">
                <!-- Card Media/Thumbnail -->
                <div class="card-thumbnail relative aspect-video">
                  <!-- Actual Thumbnail Image -->
                  <img :src="media.thumbnail" :alt="media.title" class="absolute inset-0 w-full h-full object-cover" />
                  <div class="absolute inset-0 bg-gradient-to-t from-black/50 via-black/10 to-transparent"></div>

                  <!-- Selection Checkbox -->
                  <div
                    @click.stop="toggleMediaSelection(media.id); if (!isSelectionMode) isSelectionMode = true"
                    :class="[
                      'absolute top-2 left-2 w-6 h-6 rounded-lg border-2 flex items-center justify-center cursor-pointer transition-all z-30',
                      isMediaSelected(media.id)
                        ? 'bg-teal-500 border-teal-500 text-white'
                        : isSelectionMode
                          ? 'bg-white/90 border-gray-300 hover:border-teal-400'
                          : 'bg-white/90 border-gray-300 hover:border-teal-400 opacity-0 group-hover:opacity-100'
                    ]"
                  >
                    <i v-if="isMediaSelected(media.id)" class="fas fa-check text-xs"></i>
                  </div>

                  <!-- Card Badges -->
                  <div class="absolute bottom-3 left-3 flex gap-1.5 z-20">
                    <span v-if="media.isNew" class="new-badge">
                      <i class="fas fa-sparkles"></i> New
                    </span>
                    <span v-if="media.hasTranscript" class="transcript-badge">
                      <i class="fas fa-closed-captioning"></i> CC
                    </span>
                  </div>

                  <!-- Duration/Photo Count Badge -->
                  <div class="absolute bottom-3 right-3 media-duration-badge z-20">
                    <template v-if="media.type === 'gallery'">
                      <i class="fas fa-images"></i> {{ media.photoCount }} photos
                    </template>
                    <template v-else-if="media.type === 'image'">
                      <i class="fas fa-image"></i> Image
                    </template>
                    <template v-else>
                      <i :class="media.type === 'video' ? 'fas fa-play-circle' : 'fas fa-headphones'"></i> {{ media.duration }}
                    </template>
                  </div>

                  <!-- Play Button Overlay -->
                  <div class="card-play-btn z-20">
                    <i :class="media.type === 'video' ? 'fas fa-play' : media.type === 'audio' ? 'fas fa-headphones' : media.type === 'image' ? 'fas fa-expand' : 'fas fa-images'"></i>
                  </div>

                  <!-- Hover Action Buttons -->
                  <div class="card-hover-actions z-20">
                    <button
                      class="card-action-btn"
                      :class="{ active: isMediaLiked(media.id) }"
                      @click.stop="toggleLikeMedia(media.id)"
                      title="Like">
                      <i :class="isMediaLiked(media.id) ? 'fas fa-heart' : 'far fa-heart'"></i>
                    </button>
                    <button
                      class="card-action-btn"
                      :class="{ saved: isMediaSaved(media.id) }"
                      @click.stop="toggleSaveMedia(media.id)"
                      title="Save">
                      <i :class="isMediaSaved(media.id) ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                    </button>
                    <button
                      class="card-action-btn"
                      @click.stop="shareMedia(media)"
                      title="Share">
                      <i class="fas fa-share-alt"></i>
                    </button>
                    <!-- AI Analyze Button -->
                    <button
                      v-if="showAIFeatures"
                      class="card-action-btn ai-action"
                      :class="{ analyzed: hasAIAnalysis(media.id) }"
                      @click.stop="analyzeMedia(media)"
                      title="AI Analyze">
                      <i class="fas fa-wand-magic-sparkles"></i>
                    </button>
                    <!-- AI Insights Button -->
                    <button
                      v-if="showAIFeatures && (media.type === 'video' || media.type === 'audio')"
                      class="card-action-btn ai-insights"
                      @click.stop="openAISidebar(media)"
                      title="AI Insights">
                      <i class="fas fa-brain"></i>
                    </button>
                  </div>

                  <!-- AI Analysis Badge -->
                  <div v-if="showAIFeatures && hasAIAnalysis(media.id)" class="absolute top-2 right-2 z-30">
                    <span class="px-2 py-1 bg-teal-500 text-white text-[10px] font-medium rounded-full flex items-center gap-1 shadow-lg">
                      <i class="fas fa-wand-magic-sparkles text-[8px]"></i>
                      AI Analyzed
                    </span>
                  </div>

                  <!-- Watch Progress (if any) -->
                  <div v-if="'progress' in media && media.progress" class="watch-progress z-20">
                    <div class="watch-progress-bar" :style="{ width: media.progress + '%' }"></div>
                  </div>
                </div>

                <!-- Card Body (Articles Style) -->
                <div class="p-4">
                  <!-- Meta Info -->
                  <div class="flex items-center gap-3 text-[11px] text-gray-400 mb-2">
                    <span class="flex items-center gap-1">
                      <i :class="media.type === 'video' ? 'fas fa-video' : media.type === 'audio' ? 'fas fa-headphones' : media.type === 'image' ? 'fas fa-image' : 'fas fa-images'" class="text-[9px]"></i>
                      {{ media.type === 'video' ? 'Video' : media.type === 'audio' ? 'Audio' : media.type === 'image' ? 'Image' : 'Gallery' }}
                    </span>
                    <span class="flex items-center gap-1">
                      <i class="fas fa-clock text-[9px]"></i>
                      {{ media.date }}
                    </span>
                  </div>

                  <!-- Title -->
                  <h3 class="text-sm font-semibold text-gray-800 mb-2 line-clamp-2 group-hover:text-teal-600 transition-colors leading-snug">
                    {{ media.title }}
                  </h3>

                  <!-- Category -->
                  <p class="text-xs text-gray-500 mb-3 flex items-center gap-2">
                    <span class="px-2 py-0.5 bg-teal-50 text-teal-600 text-[10px] font-medium rounded-full">
                      {{ media.category }}
                    </span>
                    <!-- AI Category Badge -->
                    <span v-if="showAIFeatures && hasAIAnalysis(media.id)" :class="['px-2 py-0.5 text-[10px] font-medium rounded-full flex items-center gap-1', getAICategoryColor(getAIAnalysis(media.id)?.category || '')]">
                      <i class="fas fa-wand-magic-sparkles text-[8px]"></i>
                      {{ getAIAnalysis(media.id)?.category }}
                    </span>
                  </p>

                  <!-- Tags -->
                  <div class="flex flex-wrap gap-1.5 mb-3">
                    <span
                      v-for="tag in media.tags?.slice(0, 3)"
                      :key="tag"
                      class="px-2 py-0.5 bg-gray-100 text-gray-600 text-[10px] font-medium rounded-full hover:bg-teal-50 hover:text-teal-600 transition-colors"
                    >
                      {{ tag }}
                    </span>
                    <!-- AI Suggested Tags indicator -->
                    <span v-if="showAIFeatures && hasAIAnalysis(media.id) && (getAIAnalysis(media.id)?.autoTags?.length || 0) > (media.tags?.length || 0)" class="px-2 py-0.5 bg-teal-50 text-teal-600 text-[10px] font-medium rounded-full flex items-center gap-1">
                      <i class="fas fa-plus text-[8px]"></i>
                      {{ (getAIAnalysis(media.id)?.autoTags?.length || 0) - (media.tags?.length || 0) }} AI tags
                    </span>
                  </div>

                  <!-- Footer -->
                  <div class="flex items-center justify-between pt-3 border-t border-gray-100">
                    <!-- Author -->
                    <div class="flex items-center gap-2">
                      <div class="w-7 h-7 rounded-full bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white text-[10px] font-semibold shadow-sm">
                        {{ getInitials(media.author) }}
                      </div>
                      <span class="text-xs text-gray-600 font-medium">{{ media.author || 'AFC Media' }}</span>
                    </div>

                    <!-- Stats -->
                    <div class="flex items-center gap-3 text-[11px] text-gray-400">
                      <span class="flex items-center gap-1 hover:text-teal-500 transition-colors">
                        <i class="fas fa-eye text-[9px]"></i>
                        {{ media.views }}
                      </span>
                      <span class="flex items-center gap-1 hover:text-red-400 transition-colors">
                        <i class="fas fa-heart text-[9px]"></i>
                        {{ likedMedia.has(media.id) ? '1' : '0' }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- List View -->
            <div v-else class="media-list">
              <div v-for="media in paginatedMedia" :key="media.id"
                   @click="isSelectionMode ? toggleMediaSelection(media.id) : goToMedia(media)"
                   class="media-list-item cursor-pointer group">
                <!-- Selection Checkbox -->
                <div
                  @click.stop="toggleMediaSelection(media.id); if (!isSelectionMode) isSelectionMode = true"
                  :class="[
                    'list-selection-checkbox w-6 h-6 rounded-lg border-2 flex items-center justify-center cursor-pointer transition-all flex-shrink-0',
                    isMediaSelected(media.id)
                      ? 'bg-teal-500 border-teal-500 text-white'
                      : isSelectionMode
                        ? 'bg-white border-gray-300 hover:border-teal-400'
                        : 'bg-white border-gray-300 hover:border-teal-400 opacity-0 group-hover:opacity-100'
                  ]"
                >
                  <i v-if="isMediaSelected(media.id)" class="fas fa-check text-xs"></i>
                </div>

                <!-- Thumbnail -->
                <div class="list-item-thumbnail">
                  <img :src="media.thumbnail" :alt="media.title" class="absolute inset-0 w-full h-full object-cover rounded-lg" />
                  <div class="absolute inset-0 bg-gradient-to-t from-black/40 to-transparent rounded-lg"></div>
                  <div class="list-play-overlay">
                    <i :class="media.type === 'video' ? 'fas fa-play' : media.type === 'audio' ? 'fas fa-headphones' : media.type === 'image' ? 'fas fa-expand' : 'fas fa-images'"></i>
                  </div>
                  <div class="list-duration-badge">
                    <template v-if="media.type === 'gallery'">
                      {{ media.photoCount }} photos
                    </template>
                    <template v-else-if="media.type === 'image'">
                      Image
                    </template>
                    <template v-else>
                      {{ media.duration }}
                    </template>
                  </div>
                </div>

                <!-- Content -->
                <div class="list-item-content">
                  <div class="list-item-header">
                    <div class="list-badges">
                      <span :class="['list-type-badge', media.type]">
                        <i :class="media.type === 'video' ? 'fas fa-video' : media.type === 'audio' ? 'fas fa-headphones' : media.type === 'image' ? 'fas fa-image' : 'fas fa-images'"></i>
                        {{ media.type === 'video' ? 'Video' : media.type === 'audio' ? 'Audio' : media.type === 'image' ? 'Image' : 'Gallery' }}
                      </span>
                      <span :class="['list-category-badge', media.category.toLowerCase().split(' ')[0]]">{{ media.category }}</span>
                      <span v-if="media.isNew" class="list-new-badge">
                        <i class="fas fa-sparkles"></i> New
                      </span>
                    </div>
                    <h3 class="list-item-title">{{ media.title }}</h3>
                  </div>
                  <div class="list-item-footer">
                    <div class="list-author">
                      <div class="list-author-avatar">
                        {{ getInitials(media.author) }}
                      </div>
                      <span>{{ media.author || 'AFC Media' }}</span>
                    </div>
                    <div class="list-stats">
                      <span><i class="fas fa-eye"></i> {{ media.views }}</span>
                      <span><i class="fas fa-clock"></i> {{ media.date }}</span>
                    </div>
                  </div>
                </div>

                <!-- Actions -->
                <div class="list-item-actions">
                  <button
                    class="list-action-btn"
                    :class="{ active: isMediaLiked(media.id) }"
                    @click.stop="toggleLikeMedia(media.id)"
                    title="Like">
                    <i :class="isMediaLiked(media.id) ? 'fas fa-heart' : 'far fa-heart'"></i>
                  </button>
                  <button
                    class="list-action-btn"
                    :class="{ saved: isMediaSaved(media.id) }"
                    @click.stop="toggleSaveMedia(media.id)"
                    title="Save">
                    <i :class="isMediaSaved(media.id) ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                  </button>
                  <button
                    class="list-action-btn"
                    @click.stop="shareMedia(media)"
                    title="Share">
                    <i class="fas fa-share-alt"></i>
                  </button>
                </div>
              </div>
            </div>

            <!-- Empty State -->
            <div v-if="filteredMedia.length === 0" class="empty-state">
              <div class="empty-state-icon">
                <i class="fas fa-video-slash"></i>
              </div>
              <h3 class="empty-state-title">No media found</h3>
              <p class="empty-state-text">Try adjusting your filters or search query</p>
              <button @click="clearFilters" class="btn-vibrant ripple">
                <i class="fas fa-undo mr-2"></i> Clear Filters
              </button>
            </div>
          </div>
        </div>

            <!-- Pagination -->
            <div v-if="filteredMedia.length > 0" class="mt-4 px-4 py-3 bg-white rounded-2xl border border-gray-100 shadow-sm">
              <div class="flex items-center justify-between flex-wrap gap-3">
                <!-- Left: Stats & Items Per Page -->
                <div class="flex items-center gap-4 flex-wrap">
                  <span class="text-xs text-gray-500">
                    Showing <span class="font-semibold text-gray-700">{{ Math.min((currentPage - 1) * itemsPerPage + 1, filteredMedia.length) }}</span>
                    to <span class="font-semibold text-gray-700">{{ Math.min(currentPage * itemsPerPage, filteredMedia.length) }}</span>
                    of <span class="font-semibold text-gray-700">{{ filteredMedia.length }}</span> items
                  </span>

                  <!-- Items Per Page Selector -->
                  <div class="flex items-center gap-2">
                    <span class="text-xs text-gray-500">Show:</span>
                    <select
                      v-model="itemsPerPage"
                      @change="handlePerPageChange(itemsPerPage)"
                      class="text-xs px-2 py-1.5 rounded-lg border border-gray-200 bg-white text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-teal-500 cursor-pointer"
                    >
                      <option :value="6">6</option>
                      <option :value="9">9</option>
                      <option :value="12">12</option>
                      <option :value="24">24</option>
                    </select>
                    <span class="text-xs text-gray-500">per page</span>
                  </div>
                </div>

                <!-- Right: Pagination Controls -->
                <div class="flex items-center gap-2">
                  <button
                    @click="handlePageChange(currentPage - 1)"
                    :disabled="currentPage === 1"
                    :class="[
                      'px-3 py-1.5 text-xs rounded-lg transition-colors flex items-center gap-1.5 border',
                      currentPage === 1
                        ? 'text-gray-300 border-gray-100 cursor-not-allowed'
                        : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100 border-gray-200'
                    ]"
                  >
                    <i class="fas fa-chevron-left text-[10px]"></i>
                    Previous
                  </button>

                  <!-- Page Numbers -->
                  <div class="flex items-center gap-1">
                    <template v-for="page in totalPages" :key="page">
                      <button
                        v-if="page === 1 || page === totalPages || (page >= currentPage - 1 && page <= currentPage + 1)"
                        @click="handlePageChange(page)"
                        :class="[
                          'w-8 h-8 text-xs rounded-lg transition-colors flex items-center justify-center',
                          page === currentPage
                            ? 'font-medium text-teal-600 bg-teal-50'
                            : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100'
                        ]"
                      >
                        {{ page }}
                      </button>
                      <span
                        v-else-if="page === currentPage - 2 || page === currentPage + 2"
                        class="text-xs text-gray-400 px-1"
                      >...</span>
                    </template>
                  </div>

                  <button
                    @click="handlePageChange(currentPage + 1)"
                    :disabled="currentPage === totalPages"
                    :class="[
                      'px-3 py-1.5 text-xs rounded-lg transition-colors flex items-center gap-1.5 border',
                      currentPage === totalPages
                        ? 'text-gray-300 border-gray-100 cursor-not-allowed'
                        : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100 border-gray-200'
                    ]"
                  >
                    Next
                    <i class="fas fa-chevron-right text-[10px]"></i>
                  </button>
                </div>
              </div>
            </div>
          </div><!-- End flex-1 main content -->
        </div><!-- End flex container -->
      </div><!-- End all-media-wrapper -->

    </div><!-- End Content Area -->

    <!-- Upload Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="showUploadModal" class="modal-overlay" @click.self="showUploadModal = false">
          <div class="modal-content modal-lg">
            <div class="modal-header">
              <h3 class="modal-title">Upload Media</h3>
              <button @click="showUploadModal = false" class="modal-close">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <div class="modal-body">
              <div class="space-y-6">
                <div class="border-2 border-dashed border-gray-200 rounded-2xl p-12 text-center hover:border-gray-400 transition-colors">
                  <div class="icon-soft mx-auto mb-4">
                    <i class="fas fa-cloud-upload-alt text-2xl"></i>
                  </div>
                  <p class="text-gray-700 font-medium mb-1">Drag and drop media files here</p>
                  <p class="text-sm text-gray-500 mb-4">Supports MP4, MOV, MP3, WAV (max 2GB)</p>
                  <button class="btn-vibrant ripple">Browse Files</button>
                </div>
                <div>
                  <label class="label">Title</label>
                  <input type="text" class="input" placeholder="Enter media title...">
                </div>
                <div>
                  <label class="label">Description</label>
                  <textarea class="input textarea" rows="3" placeholder="Enter description..."></textarea>
                </div>
                <div class="grid grid-cols-2 gap-4">
                  <div>
                    <label class="label">Category</label>
                    <select class="input select">
                      <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
                    </select>
                  </div>
                  <div>
                    <label class="label">Visibility</label>
                    <select class="input select">
                      <option>Public</option>
                      <option>Private</option>
                      <option>Unlisted</option>
                    </select>
                  </div>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button @click="showUploadModal = false" class="btn btn-secondary ripple">Cancel</button>
              <button class="btn-vibrant ripple"><i class="fas fa-upload mr-2"></i> Upload</button>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>

    <!-- Add to Collection Modal -->
    <AddToCollectionModal
      :show="showAddToCollectionModal"
      content-type="media"
      :content-id="selectedItemForCollection?.id ?? 0"
      :content-title="selectedItemForCollection?.title ?? ''"
      :content-thumbnail="selectedItemForCollection?.thumbnail"
      @close="showAddToCollectionModal = false; selectedItemForCollection = null"
      @added="handleAddedToCollection"
    />

    <!-- AI Analysis Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="showAIAnalysisModal" class="fixed inset-0 z-[9999] flex items-center justify-center p-4">
          <!-- Backdrop -->
          <div class="absolute inset-0 bg-black/60 backdrop-blur-sm" @click="closeAIModal"></div>

          <!-- Modal Content -->
          <div class="ai-analysis-modal relative bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-hidden">
            <!-- Modal Header -->
            <div class="px-6 py-4 bg-gradient-to-r from-teal-500 to-teal-600 text-white flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 bg-white/20 rounded-xl flex items-center justify-center">
                  <i class="fas fa-wand-magic-sparkles text-lg"></i>
                </div>
                <div>
                  <h3 class="font-semibold">AI Media Analysis</h3>
                  <p class="text-xs text-teal-100">Powered by Intalio AI Engine</p>
                </div>
              </div>
              <button @click="closeAIModal" class="w-8 h-8 rounded-lg bg-white/20 hover:bg-white/30 flex items-center justify-center transition-colors">
                <i class="fas fa-times"></i>
              </button>
            </div>

            <!-- Modal Body -->
            <div class="p-6 overflow-y-auto max-h-[calc(90vh-180px)]">
              <!-- Loading State -->
              <div v-if="isAnalyzingMedia" class="py-12 text-center">
                <div class="w-16 h-16 border-4 border-teal-500 border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
                <p class="text-gray-600 font-medium">Analyzing media...</p>
                <p class="text-sm text-gray-400 mt-1">AI is processing your content</p>
              </div>

              <!-- Analysis Results -->
              <div v-else-if="selectedMediaForAI && getAIAnalysis(selectedMediaForAI.id)" class="space-y-6">
                <!-- Media Preview -->
                <div class="flex gap-4 p-4 bg-gray-50 rounded-xl">
                  <img :src="selectedMediaForAI.thumbnail" :alt="selectedMediaForAI.title" class="w-32 h-20 object-cover rounded-lg" />
                  <div class="flex-1">
                    <h4 class="font-semibold text-gray-800">{{ selectedMediaForAI.title }}</h4>
                    <p class="text-sm text-gray-500">{{ selectedMediaForAI.type }} • {{ selectedMediaForAI.category }}</p>
                    <div class="flex items-center gap-2 mt-2">
                      <span class="px-2 py-0.5 bg-teal-100 text-teal-700 text-xs font-medium rounded-full">
                        {{ ((getAIAnalysis(selectedMediaForAI.id)?.categoryConfidence ?? 0) * 100).toFixed(0) }}% confidence
                      </span>
                      <span class="text-xs text-gray-400">
                        Processed in {{ getAIAnalysis(selectedMediaForAI.id)?.processingTime }}s
                      </span>
                    </div>
                  </div>
                </div>

                <!-- AI Description -->
                <div class="space-y-2">
                  <h5 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
                    <i class="fas fa-align-left text-teal-500"></i>
                    AI-Generated Description
                  </h5>
                  <p class="text-sm text-gray-600 bg-teal-50 p-4 rounded-lg border border-teal-100">
                    {{ getAIAnalysis(selectedMediaForAI.id)?.description }}
                  </p>
                </div>

                <!-- Auto Tags -->
                <div class="space-y-2">
                  <h5 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
                    <i class="fas fa-tags text-teal-500"></i>
                    AI-Suggested Tags
                  </h5>
                  <div class="flex flex-wrap gap-2">
                    <span
                      v-for="tag in getAIAnalysis(selectedMediaForAI.id)?.autoTags"
                      :key="tag"
                      class="px-3 py-1.5 bg-teal-100 text-teal-700 text-sm font-medium rounded-full"
                    >
                      {{ tag }}
                    </span>
                  </div>
                  <button
                    @click="applyAITags(selectedMediaForAI.id)"
                    class="mt-2 px-4 py-2 bg-teal-500 hover:bg-teal-600 text-white text-sm font-medium rounded-lg flex items-center gap-2 transition-colors"
                  >
                    <i class="fas fa-plus"></i>
                    Apply AI Tags
                  </button>
                </div>

                <!-- Detected Objects -->
                <div v-if="getAIAnalysis(selectedMediaForAI.id)?.objects?.length" class="space-y-2">
                  <h5 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
                    <i class="fas fa-shapes text-purple-500"></i>
                    Detected Objects
                  </h5>
                  <div class="flex flex-wrap gap-2">
                    <span
                      v-for="obj in getAIAnalysis(selectedMediaForAI.id)?.objects"
                      :key="obj"
                      class="px-3 py-1.5 bg-purple-100 text-purple-700 text-sm font-medium rounded-full"
                    >
                      {{ obj }}
                    </span>
                  </div>
                </div>

                <!-- Detected Colors -->
                <div v-if="getAIAnalysis(selectedMediaForAI.id)?.colors?.length" class="space-y-2">
                  <h5 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
                    <i class="fas fa-palette text-amber-500"></i>
                    Color Palette
                  </h5>
                  <div class="flex flex-wrap gap-2">
                    <span
                      v-for="color in getAIAnalysis(selectedMediaForAI.id)?.colors"
                      :key="color"
                      class="px-3 py-1.5 bg-amber-100 text-amber-700 text-sm font-medium rounded-full"
                    >
                      {{ color }}
                    </span>
                  </div>
                </div>

                <!-- Face Detection -->
                <div v-if="getAIAnalysis(selectedMediaForAI.id)?.faces" class="space-y-2">
                  <h5 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
                    <i class="fas fa-users text-blue-500"></i>
                    Face Detection
                  </h5>
                  <div class="p-3 bg-blue-50 rounded-lg border border-blue-100">
                    <p class="text-sm text-blue-700">
                      <span class="font-bold text-lg">{{ getAIAnalysis(selectedMediaForAI.id)?.faces }}</span>
                      faces detected in this media
                    </p>
                  </div>
                </div>

                <!-- OCR Results -->
                <div v-if="getAIAnalysis(selectedMediaForAI.id)?.ocrText" class="space-y-2">
                  <h5 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
                    <i class="fas fa-file-lines text-rose-500"></i>
                    OCR - Extracted Text
                    <span class="text-xs text-gray-400 font-normal">
                      ({{ ((getAIAnalysis(selectedMediaForAI.id)?.ocrConfidence ?? 0) * 100).toFixed(0) }}% confidence)
                    </span>
                  </h5>
                  <div class="p-3 bg-rose-50 rounded-lg border border-rose-100">
                    <p class="text-sm text-rose-700 font-mono">
                      {{ getAIAnalysis(selectedMediaForAI.id)?.ocrText }}
                    </p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Modal Footer -->
            <div class="px-6 py-4 bg-gray-50 border-t border-gray-100 flex items-center justify-between">
              <p class="text-xs text-gray-400">
                <i class="fas fa-info-circle mr-1"></i>
                AI analysis is for reference only
              </p>
              <div class="flex gap-2">
                <button @click="closeAIModal" class="px-4 py-2 bg-gray-200 hover:bg-gray-300 text-gray-700 rounded-lg text-sm font-medium transition-colors">
                  Close
                </button>
              </div>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>

    <!-- AI Sidebar Panel -->
    <Teleport to="body">
      <Transition name="slide-right">
        <div v-if="showAISidebar" class="fixed inset-0 z-[9999] flex justify-end">
          <!-- Backdrop -->
          <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="closeAISidebar"></div>

          <!-- Sidebar Panel -->
          <div class="relative w-full max-w-lg bg-white shadow-2xl h-full overflow-hidden flex flex-col">
            <!-- Header (Documents Page Style) -->
            <div class="px-6 py-4 bg-gradient-to-r from-teal-50 to-transparent border-b border-teal-100">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center">
                    <i class="fas fa-wand-magic-sparkles text-white"></i>
                  </div>
                  <div>
                    <h3 class="text-lg font-semibold text-gray-900">AI Analysis</h3>
                    <p class="text-sm text-gray-500">Powered by Intalio AI</p>
                  </div>
                </div>
                <button @click="closeAISidebar" class="w-8 h-8 rounded-lg hover:bg-gray-100 text-gray-400 hover:text-gray-600 flex items-center justify-center transition-all">
                  <i class="fas fa-times"></i>
                </button>
              </div>
            </div>

            <!-- Media Preview -->
            <div v-if="selectedMediaForSidebar" class="px-4 py-3 bg-gray-50 border-b border-gray-100">
              <div class="flex gap-3">
                <img :src="selectedMediaForSidebar.thumbnail" :alt="selectedMediaForSidebar.title" class="w-20 h-14 object-cover rounded-lg" />
                <div class="flex-1 min-w-0">
                  <h4 class="text-sm font-medium text-gray-800 truncate">{{ selectedMediaForSidebar.title }}</h4>
                  <p class="text-xs text-gray-500">{{ selectedMediaForSidebar.type }} • {{ selectedMediaForSidebar.category }}</p>
                </div>
              </div>
            </div>

            <!-- Content -->
            <div class="flex-1 overflow-y-auto p-4 space-y-4">
              <!-- AI Tab Navigation (Documents Page Style) -->
              <div class="grid grid-cols-4 gap-2">
                <button
                  v-for="tab in [
                    { id: 'summary', icon: 'fas fa-file-lines', label: 'Summary' },
                    { id: 'transcript', icon: 'fas fa-closed-captioning', label: 'Transcript' },
                    { id: 'entities', icon: 'fas fa-tags', label: 'Entities' },
                    { id: 'translate', icon: 'fas fa-language', label: 'Translate' }
                  ]"
                  :key="tab.id"
                  @click="aiSidebarTab = tab.id as any; tab.id === 'summary' ? generateAISummary() : tab.id === 'transcript' ? generateAITranscript() : tab.id === 'entities' ? extractAIEntities() : null"
                  :class="[
                    'px-3 py-2.5 rounded-xl text-xs font-medium transition-all flex flex-col items-center gap-1.5',
                    aiSidebarTab === tab.id
                      ? 'bg-teal-50 text-teal-700 border-2 border-teal-200'
                      : 'bg-gray-50 text-gray-600 border-2 border-transparent hover:bg-gray-100'
                  ]"
                >
                  <i :class="[tab.icon, 'text-base']"></i>
                  <span>{{ tab.label }}</span>
                </button>
              </div>

              <!-- Summary Tab -->
              <div v-if="aiSidebarTab === 'summary'" class="space-y-3">
                <!-- Summary Type Selection (Documents Page Style) -->
                <div class="flex gap-2">
                  <button
                    v-for="type in [
                      { id: 'brief', label: 'Brief', icon: 'fas fa-bolt' },
                      { id: 'detailed', label: 'Detailed', icon: 'fas fa-align-left' },
                      { id: 'bullets', label: 'Bullets', icon: 'fas fa-list-ul' }
                    ]"
                    :key="type.id"
                    @click="aiSummaryType = type.id as any; generateAISummary()"
                    :class="[
                      'flex-1 px-3 py-2 rounded-lg text-xs font-medium transition-all cursor-pointer flex items-center justify-center gap-1.5',
                      aiSummaryType === type.id
                        ? 'bg-teal-500 text-white shadow-sm'
                        : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                    ]"
                  >
                    <i :class="type.icon"></i>
                    {{ type.label }}
                  </button>
                </div>

                <!-- Generate Button -->
                <button
                  @click="generateAISummary"
                  :disabled="isGeneratingSummary"
                  class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors"
                >
                  <i :class="['fas', isGeneratingSummary ? 'fa-spinner fa-spin' : 'fa-wand-magic-sparkles']"></i>
                  {{ isGeneratingSummary ? 'Generating...' : 'Generate Summary' }}
                </button>

                <!-- Summary Result (Documents Page Style) -->
                <Transition name="slide-fade">
                  <div v-if="aiSummary" class="space-y-3 pt-3 border-t border-gray-100">
                    <div class="p-3 bg-teal-50 rounded-xl">
                      <div class="flex items-center justify-between mb-2">
                        <span class="text-sm font-medium text-teal-700">Summary</span>
                        <button @click="aiSummary = null" class="text-teal-400 hover:text-teal-600">
                          <i class="fas fa-times text-sm"></i>
                        </button>
                      </div>
                      <p class="text-sm text-teal-800 leading-relaxed whitespace-pre-line">{{ aiSummary.text }}</p>
                    </div>

                    <!-- Key Points -->
                    <div v-if="aiSummary.keyPoints?.length" class="p-3 bg-gray-50 rounded-xl">
                      <div class="flex items-center gap-2 mb-2">
                        <i class="fas fa-lightbulb text-amber-500"></i>
                        <span class="text-sm font-medium text-gray-700">Key Points</span>
                      </div>
                      <ul class="space-y-1.5">
                        <li v-for="point in aiSummary.keyPoints" :key="point" class="flex items-start gap-2 text-sm text-gray-600">
                          <i class="fas fa-check-circle text-teal-500 mt-0.5 flex-shrink-0"></i>
                          <span>{{ point }}</span>
                        </li>
                      </ul>
                    </div>

                    <!-- Confidence & Copy -->
                    <div class="flex items-center justify-between text-xs text-gray-400">
                      <span>{{ (aiSummary.confidence * 100).toFixed(0) }}% confidence</span>
                      <button @click="copySummaryText" class="flex items-center gap-1 text-teal-600 hover:text-teal-700">
                        <i class="fas fa-copy"></i>
                        Copy
                      </button>
                    </div>
                  </div>
                </Transition>
              </div>

              <!-- Transcript Tab -->
              <div v-if="aiSidebarTab === 'transcript'" class="space-y-3">
                <!-- Generate Button -->
                <button
                  @click="generateAITranscript"
                  :disabled="isGeneratingTranscript"
                  class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors"
                >
                  <i :class="['fas', isGeneratingTranscript ? 'fa-spinner fa-spin' : 'fa-closed-captioning']"></i>
                  {{ isGeneratingTranscript ? 'Generating...' : 'Generate Transcript' }}
                </button>

                <!-- Loading State -->
                <div v-if="isGeneratingTranscript" class="flex items-center justify-center py-6">
                  <div class="text-center">
                    <i class="fas fa-spinner fa-spin text-2xl text-teal-500 mb-2"></i>
                    <p class="text-sm text-gray-500">Processing audio...</p>
                  </div>
                </div>

                <!-- Transcript Result -->
                <Transition name="slide-fade">
                  <div v-if="aiTranscript && !isGeneratingTranscript" class="space-y-3 pt-3 border-t border-gray-100">
                    <!-- Language Badge -->
                    <div class="flex items-center justify-between">
                      <span class="px-2 py-1 bg-gray-100 text-gray-600 text-xs rounded-full flex items-center gap-1">
                        <i class="fas fa-language"></i>
                        {{ aiTranscript.language }}
                      </span>
                      <button @click="aiTranscript = null" class="text-gray-400 hover:text-gray-600">
                        <i class="fas fa-times text-sm"></i>
                      </button>
                    </div>

                    <!-- Timestamps List -->
                    <div class="space-y-1.5 max-h-56 overflow-y-auto">
                      <div
                        v-for="segment in aiTranscript.timestamps"
                        :key="segment.time"
                        class="flex items-start gap-2 p-2 rounded-lg hover:bg-teal-50 transition-all cursor-pointer group"
                      >
                        <span class="text-xs font-mono text-teal-600 bg-teal-100 px-2 py-0.5 rounded flex-shrink-0">{{ segment.time }}</span>
                        <p class="text-sm text-gray-700 flex-1">{{ segment.text }}</p>
                      </div>
                    </div>

                    <!-- Confidence & Copy -->
                    <div class="flex items-center justify-between text-xs text-gray-400 pt-2 border-t border-gray-100">
                      <span>{{ (aiTranscript.confidence * 100).toFixed(0) }}% accuracy</span>
                      <button @click="copyTranscript" class="flex items-center gap-1 text-teal-600 hover:text-teal-700">
                        <i class="fas fa-copy"></i>
                        Copy Full Transcript
                      </button>
                    </div>
                  </div>
                </Transition>
              </div>

              <!-- Entities Tab -->
              <div v-if="aiSidebarTab === 'entities'" class="space-y-3">
                <!-- Generate Button -->
                <button
                  @click="extractAIEntities"
                  :disabled="isExtractingEntities"
                  class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors"
                >
                  <i :class="['fas', isExtractingEntities ? 'fa-spinner fa-spin' : 'fa-magnifying-glass']"></i>
                  {{ isExtractingEntities ? 'Extracting...' : 'Extract Entities' }}
                </button>

                <!-- Entities Result -->
                <Transition name="slide-fade">
                  <div v-if="aiEntities.length > 0" class="space-y-3 pt-3 border-t border-gray-100">
                    <!-- Entity Filter -->
                    <div class="flex flex-wrap gap-1.5">
                      <button
                        @click="entityFilterType = null"
                        :class="[
                          'px-2 py-1 rounded-full text-xs font-medium transition-all',
                          entityFilterType === null
                            ? 'bg-teal-100 text-teal-700'
                            : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                        ]"
                      >
                        All ({{ aiEntities.length }})
                      </button>
                      <button
                        v-for="type in [...new Set(aiEntities.map(e => e.type))]"
                        :key="type"
                        @click="entityFilterType = type"
                        :class="[
                          'px-2 py-1 rounded-full text-xs font-medium transition-all capitalize',
                          entityFilterType === type
                            ? 'bg-teal-100 text-teal-700'
                            : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                        ]"
                      >
                        {{ type }} ({{ aiEntities.filter(e => e.type === type).length }})
                      </button>
                    </div>

                    <!-- Entity List -->
                    <div class="space-y-1.5 max-h-56 overflow-y-auto">
                      <div
                        v-for="entity in aiEntities.filter(e => !entityFilterType || e.type === entityFilterType)"
                        :key="entity.name"
                        :class="['flex items-center gap-2 p-2 rounded-lg transition-all', getEntityTypeColor(entity.type)]"
                      >
                        <i :class="[entity.type === 'Team' ? 'fas fa-users' : entity.type === 'Person' ? 'fas fa-user' : entity.type === 'Location' ? 'fas fa-map-marker-alt' : entity.type === 'Event' ? 'fas fa-calendar' : entity.type === 'Organization' ? 'fas fa-building' : 'fas fa-tag', 'text-sm']"></i>
                        <span class="flex-1 text-sm font-medium truncate">{{ entity.name }}</span>
                        <span class="text-xs opacity-70">{{ entity.count }}x</span>
                      </div>
                    </div>
                  </div>
                </Transition>
              </div>

              <!-- Translate Tab -->
              <div v-if="aiSidebarTab === 'translate'" class="space-y-3">
                <!-- Language Selection (Documents Page Style) -->
                <div class="grid grid-cols-3 gap-2">
                  <button
                    v-for="lang in [
                      { code: 'ar', label: 'Arabic', flag: '🇸🇦' },
                      { code: 'fr', label: 'French', flag: '🇫🇷' },
                      { code: 'es', label: 'Spanish', flag: '🇪🇸' },
                      { code: 'zh', label: 'Chinese', flag: '🇨🇳' },
                      { code: 'ja', label: 'Japanese', flag: '🇯🇵' },
                      { code: 'ko', label: 'Korean', flag: '🇰🇷' }
                    ]"
                    :key="lang.code"
                    @click="targetLanguage = lang.code"
                    :disabled="isTranslating"
                    :class="[
                      'px-2 py-2 rounded-lg text-xs font-medium transition-all flex flex-col items-center gap-1',
                      targetLanguage === lang.code && aiTranslation
                        ? 'bg-teal-100 text-teal-700 ring-2 ring-teal-300'
                        : 'bg-gray-50 text-gray-600 hover:bg-gray-100'
                    ]"
                  >
                    <span class="text-lg">{{ lang.flag }}</span>
                    <span>{{ lang.label }}</span>
                  </button>
                </div>

                <!-- Translate Button -->
                <button
                  @click="translateContent"
                  :disabled="isTranslating"
                  class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors"
                >
                  <i :class="['fas', isTranslating ? 'fa-spinner fa-spin' : 'fa-language']"></i>
                  {{ isTranslating ? 'Translating...' : 'Translate Content' }}
                </button>

                <!-- Loading State -->
                <div v-if="isTranslating" class="flex items-center justify-center py-6">
                  <div class="text-center">
                    <i class="fas fa-spinner fa-spin text-2xl text-teal-500 mb-2"></i>
                    <p class="text-sm text-gray-500">Translating content...</p>
                  </div>
                </div>

                <!-- Translation Result (Documents Page Style) -->
                <Transition name="slide-fade">
                  <div v-if="aiTranslation && !isTranslating" class="space-y-3 pt-3 border-t border-gray-100">
                    <div class="p-3 bg-blue-50 rounded-xl">
                      <div class="flex items-center justify-between mb-2">
                        <span class="text-sm font-medium text-blue-700">Translation</span>
                        <button @click="aiTranslation = null" class="text-blue-400 hover:text-blue-600">
                          <i class="fas fa-times text-sm"></i>
                        </button>
                      </div>
                      <p class="text-sm text-blue-800 leading-relaxed max-h-40 overflow-y-auto" :dir="targetLanguage === 'ar' ? 'rtl' : 'ltr'">
                        {{ aiTranslation.text }}
                      </p>
                    </div>
                    <div class="flex items-center justify-between text-xs text-gray-400">
                      <span>Translated to {{ aiTranslation.language }}</span>
                      <button @click="copyTranslation" class="flex items-center gap-1 text-teal-600 hover:text-teal-700">
                        <i class="fas fa-copy"></i>
                        Copy
                      </button>
                    </div>
                  </div>
                </Transition>
              </div>

              <!-- Accessibility Section (shown in all tabs) -->
              <div class="mt-6 pt-4 border-t border-gray-200">
                <h4 class="text-sm font-semibold text-gray-700 flex items-center gap-2 mb-3">
                  <i class="fas fa-universal-access text-blue-500"></i>
                  AI Accessibility Features
                </h4>

                <div class="grid grid-cols-2 gap-3">
                  <!-- Generate Captions -->
                  <button
                    @click="generateAICaptions"
                    :disabled="isGeneratingCaptions"
                    class="p-3 bg-blue-50 hover:bg-blue-100 border border-blue-200 rounded-xl text-left transition-colors disabled:opacity-50"
                  >
                    <div class="flex items-center gap-2 mb-1">
                      <i v-if="isGeneratingCaptions" class="fas fa-spinner animate-spin text-blue-500"></i>
                      <i v-else class="fas fa-closed-captioning text-blue-500"></i>
                      <span class="text-sm font-medium text-blue-700">Captions</span>
                    </div>
                    <p class="text-[10px] text-blue-600">
                      {{ aiCaptions.length > 0 ? `${aiCaptions.length} captions generated` : 'Generate AI captions' }}
                    </p>
                  </button>

                  <!-- Generate Audio Description -->
                  <button
                    @click="generateAudioDescription"
                    :disabled="isGeneratingAudioDesc"
                    class="p-3 bg-purple-50 hover:bg-purple-100 border border-purple-200 rounded-xl text-left transition-colors disabled:opacity-50"
                  >
                    <div class="flex items-center gap-2 mb-1">
                      <i v-if="isGeneratingAudioDesc" class="fas fa-spinner animate-spin text-purple-500"></i>
                      <i v-else class="fas fa-audio-description text-purple-500"></i>
                      <span class="text-sm font-medium text-purple-700">Audio Desc</span>
                    </div>
                    <p class="text-[10px] text-purple-600">
                      {{ aiAudioDescription ? 'Description ready' : 'Generate description' }}
                    </p>
                  </button>
                </div>

                <!-- Captions Preview -->
                <div v-if="aiCaptions.length > 0" class="mt-3 p-3 bg-blue-50 rounded-lg border border-blue-100">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-xs font-medium text-blue-700">Generated Captions</span>
                    <span class="text-[10px] text-blue-500">{{ aiCaptions.length }} segments</span>
                  </div>
                  <div class="max-h-24 overflow-y-auto space-y-1">
                    <div v-for="caption in aiCaptions.slice(0, 5)" :key="caption.time" class="flex gap-2 text-xs">
                      <span class="font-mono text-blue-600 shrink-0">{{ caption.time }}</span>
                      <span class="text-gray-600">{{ caption.text }}</span>
                    </div>
                    <p v-if="aiCaptions.length > 5" class="text-[10px] text-blue-400">+ {{ aiCaptions.length - 5 }} more...</p>
                  </div>
                </div>

                <!-- Audio Description Preview -->
                <div v-if="aiAudioDescription" class="mt-3 p-3 bg-purple-50 rounded-lg border border-purple-100">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-xs font-medium text-purple-700">Audio Description</span>
                    <button class="text-[10px] text-purple-500 hover:text-purple-700">
                      <i class="fas fa-volume-up mr-1"></i> Play
                    </button>
                  </div>
                  <p class="text-xs text-gray-600 leading-relaxed line-clamp-4">{{ aiAudioDescription }}</p>
                </div>
              </div>
            </div>

            <!-- Footer with Video Insights -->
            <div class="border-t border-gray-200 bg-gray-50 p-4">
              <button
                @click="analyzeVideoInsights"
                :disabled="isAnalyzingInsights"
                class="w-full px-4 py-2.5 bg-gradient-to-r from-purple-500 to-indigo-500 text-white rounded-lg font-medium flex items-center justify-center gap-2 hover:from-purple-600 hover:to-indigo-600 transition-colors disabled:opacity-50"
              >
                <i v-if="isAnalyzingInsights" class="fas fa-spinner animate-spin"></i>
                <i v-else class="fas fa-chart-line"></i>
                {{ isAnalyzingInsights ? 'Analyzing...' : 'Analyze Video Insights' }}
              </button>

              <!-- Insights Results -->
              <div v-if="aiKeyMoments.length > 0 || aiSpeakers.length > 0 || aiSentiment" class="mt-4 space-y-4">
                <!-- Key Moments -->
                <div v-if="aiKeyMoments.length > 0" class="space-y-2">
                  <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider">Key Moments</h4>
                  <div class="flex gap-2 overflow-x-auto pb-2">
                    <div
                      v-for="moment in aiKeyMoments.slice(0, 4)"
                      :key="moment.timestamp"
                      class="flex-shrink-0 p-2 bg-white rounded-lg border border-gray-200 text-center w-24"
                    >
                      <i :class="[getMomentTypeIcon(moment.type), 'text-teal-500 mb-1']"></i>
                      <p class="text-[10px] font-mono text-teal-600">{{ moment.timestamp }}</p>
                      <p class="text-[10px] text-gray-600 truncate">{{ moment.title }}</p>
                    </div>
                  </div>
                </div>

                <!-- Sentiment -->
                <div v-if="aiSentiment" class="flex items-center gap-3 p-3 bg-white rounded-lg border border-gray-200">
                  <div :class="['w-10 h-10 rounded-full flex items-center justify-center', aiSentiment.overall === 'positive' ? 'bg-green-100' : aiSentiment.overall === 'negative' ? 'bg-red-100' : 'bg-gray-100']">
                    <i :class="['fas', aiSentiment.overall === 'positive' ? 'fa-smile text-green-600' : aiSentiment.overall === 'negative' ? 'fa-frown text-red-600' : 'fa-meh text-gray-600']"></i>
                  </div>
                  <div class="flex-1">
                    <p class="text-sm font-medium text-gray-700 capitalize">{{ aiSentiment.overall }} Sentiment</p>
                    <div class="w-full h-1.5 bg-gray-200 rounded-full mt-1">
                      <div :class="['h-full rounded-full', aiSentiment.overall === 'positive' ? 'bg-green-500' : aiSentiment.overall === 'negative' ? 'bg-red-500' : 'bg-gray-500']" :style="{ width: (aiSentiment.score * 100) + '%' }"></div>
                    </div>
                  </div>
                  <span class="text-xs text-gray-400">{{ (aiSentiment.score * 100).toFixed(0) }}%</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<style scoped>
/* Page View Background */
.page-view {
  padding: 0;
  min-height: 100vh;
  background: linear-gradient(180deg, #f0fdfa 0%, #f8fafc 15%, #ffffff 100%);
  position: relative;
}

/* ============================================
   HERO SECTION (Matching Articles Page)
   ============================================ */
.hero-gradient {
  background: linear-gradient(135deg, #0d9488 0%, #14b8a6 50%, #10b981 100%);
}

.stats-top-right {
  position: absolute;
  top: 24px;
  right: 32px;
  display: flex;
  align-items: flex-start;
  gap: 12px;
  z-index: 10;
}

.stat-card-square {
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(8px);
  border-radius: 16px;
  width: 115px;
  height: 115px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 8px;
  text-align: center;
  transition: all 0.3s ease;
  cursor: pointer;
}

.stat-card-square:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: translateY(-4px) scale(1.02);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
}

.stat-card-square:hover .stat-icon-box {
  transform: scale(1.1);
}

.stat-icon-box {
  color: white;
  font-size: 20px;
  transition: transform 0.3s ease;
}

.stat-value-mini {
  font-size: 24px;
  font-weight: 700;
  color: white;
  line-height: 1;
  margin: 0;
}

.stat-label-mini {
  font-size: 11px;
  color: rgba(255, 255, 255, 0.7);
  line-height: 1;
  margin: 0;
}

/* Circle Drift Animations */
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
  0%, 100% { transform: translate(33%, -50%); }
  25% { transform: translate(28%, -45%); }
  50% { transform: translate(35%, -55%); }
  75% { transform: translate(30%, -48%); }
}

@keyframes drift-2 {
  0%, 100% { transform: translate(-33%, 50%); }
  33% { transform: translate(-28%, 45%); }
  66% { transform: translate(-38%, 55%); }
}

@keyframes drift-3 {
  0%, 100% { transform: translate(0, -50%); }
  50% { transform: translate(10%, -40%); }
}

/* Responsive Hero */
@media (max-width: 1024px) {
  .stats-top-right {
    position: relative;
    top: 0;
    right: 0;
    flex-wrap: wrap;
    justify-content: center;
    padding: 16px;
    gap: 8px;
  }

  .stat-card-square {
    width: 100px;
    height: 100px;
  }

  .stat-value-mini {
    font-size: 20px;
  }
}

@media (max-width: 640px) {
  .stats-top-right {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 8px;
  }

  .stat-card-square {
    width: 100%;
    height: 90px;
  }
}

/* Particles Effect */
.particles {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  overflow: hidden;
  z-index: 0;
  pointer-events: none;
}

.particle {
  position: absolute;
  width: 3px;
  height: 3px;
  background: rgba(20, 184, 166, 0.3);
  border-radius: 50%;
  animation: float linear infinite;
}

@keyframes float {
  0% {
    transform: translateY(100vh) translateX(0) scale(0);
    opacity: 0;
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 1;
  }
  100% {
    transform: translateY(-100vh) translateX(100px) scale(1);
    opacity: 0;
  }
}

/* Card Animated */
.card-animated {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.3);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
}

.card-animated::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
  transition: left 0.5s;
}

.card-animated:hover {
  transform: translateY(-8px);
  box-shadow: 0 16px 48px rgba(20, 184, 166, 0.4);
}

.card-animated:hover::before {
  left: 100%;
}

/* Button Vibrant */
.btn-vibrant {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 50%, #0f766e 100%);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 1rem;
  font-weight: 600;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(20, 184, 166, 0.4);
}

.btn-vibrant::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  width: 0;
  height: 0;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.3);
  transform: translate(-50%, -50%);
  transition: width 0.6s, height 0.6s;
}

.btn-vibrant:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(20, 184, 166, 0.6);
}

.btn-vibrant:hover::before {
  width: 300px;
  height: 300px;
}

.btn-vibrant:active {
  transform: translateY(0);
}

/* Media Card Hover Effects */
.media-card {
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
}

.media-card:hover {
  transform: scale(1.05);
  z-index: 10;
}

.play-btn {
  transition: all 0.3s ease;
}

.media-card:hover .play-btn {
  transform: scale(1.2);
  box-shadow: 0 8px 25px rgba(20, 184, 166, 0.5);
}

/* Icon Styles */
.icon-vibrant {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 0.75rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  transition: all 0.3s ease;
}

.icon-vibrant:hover {
  transform: rotate(10deg) scale(1.1);
  box-shadow: 0 4px 15px rgba(20, 184, 166, 0.5);
}

.icon-soft {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 3rem;
  height: 3rem;
  border-radius: 1rem;
  background: rgba(20, 184, 166, 0.1);
  color: #14b8a6;
  transition: all 0.3s ease;
}

.icon-soft:hover {
  background: rgba(20, 184, 166, 0.2);
  transform: scale(1.1);
}

/* Ripple Effect */
.ripple {
  position: relative;
  overflow: hidden;
}

.ripple::after {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  width: 0;
  height: 0;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.5);
  transform: translate(-50%, -50%);
  transition: width 0.6s, height 0.6s;
}

.ripple:active::after {
  width: 300px;
  height: 300px;
}

/* Fade-in Animations */
.fade-in-up {
  animation: fadeInUp 0.6s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.fade-in-up-delay-1 { animation-delay: 0.1s; animation-fill-mode: both; }
.fade-in-up-delay-2 { animation-delay: 0.2s; animation-fill-mode: both; }
.fade-in-up-delay-3 { animation-delay: 0.3s; animation-fill-mode: both; }
.fade-in-up-delay-4 { animation-delay: 0.4s; animation-fill-mode: both; }

/* ============================================
   CONTINUE WATCHING SECTION (Enhanced)
   ============================================ */
.continue-watching-row {
  display: flex;
  gap: 1.5rem;
  align-items: stretch;
}

.continue-watching-column {
  flex: 1;
  min-width: 0;
}

.section-header-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.section-title-sm {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 1.125rem;
  font-weight: 700;
  color: #1e293b;
}

.section-title-sm i {
  width: 2.5rem;
  height: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-radius: 0.75rem;
  font-size: 0.875rem;
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.2);
}

/* Continue Watching Scroll */
.continue-watching-scroll {
  display: flex;
  gap: 1rem;
  overflow-x: auto;
  scroll-snap-type: x mandatory;
  -webkit-overflow-scrolling: touch;
  padding-bottom: 0.5rem;
  padding-top: 0.25rem;
}

.continue-watching-scroll::-webkit-scrollbar {
  height: 4px;
}

.continue-watching-scroll::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 4px;
}

.continue-watching-scroll::-webkit-scrollbar-thumb {
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  border-radius: 4px;
}

/* Continue Watching Card */
.continue-watching-card {
  flex: 0 0 280px;
  scroll-snap-align: start;
  background: white;
  border-radius: 1rem;
  border: 1px solid #f1f5f9;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04);
}

.continue-watching-card:hover {
  transform: translateY(-6px);
  box-shadow: 0 12px 30px rgba(20, 184, 166, 0.15);
  border-color: #99f6e4;
}

/* Card Media */
.continue-card-media {
  position: relative;
  overflow: hidden;
}

.continue-card-thumbnail {
  position: relative;
  aspect-ratio: 16/9;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  background: #1e293b;
}

.continue-thumbnail-img {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.continue-watching-card:hover .continue-thumbnail-img {
  transform: scale(1.08);
}

.continue-thumbnail-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(to top, rgba(0, 0, 0, 0.5) 0%, rgba(0, 0, 0, 0.1) 50%, rgba(0, 0, 0, 0.2) 100%);
  pointer-events: none;
}

/* Category Badge */
.continue-category-badge {
  position: absolute;
  top: 0.625rem;
  left: 0.625rem;
  padding: 0.25rem 0.625rem;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(8px);
  border-radius: 100px;
  font-size: 0.625rem;
  font-weight: 600;
  color: #0f172a;
  text-transform: uppercase;
  letter-spacing: 0.02em;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

/* Duration Badge */
.continue-duration-badge {
  position: absolute;
  bottom: 0.625rem;
  right: 0.625rem;
  padding: 0.25rem 0.5rem;
  background: rgba(0, 0, 0, 0.75);
  backdrop-filter: blur(4px);
  border-radius: 0.375rem;
  font-size: 0.6875rem;
  font-weight: 500;
  color: white;
  display: flex;
  align-items: center;
}

/* Resume Badge */
.continue-resume-badge {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  padding: 0.5rem 1rem;
  background: white;
  border-radius: 100px;
  font-size: 0.75rem;
  font-weight: 600;
  color: #0f172a;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
  opacity: 0;
  transition: all 0.3s ease;
  z-index: 10;
}

.continue-watching-card:hover .continue-resume-badge {
  opacity: 1;
}

/* Progress Bar */
.continue-progress-bar {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: rgba(255, 255, 255, 0.3);
}

.continue-progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 0 2px 0 0;
  transition: width 0.3s ease;
  box-shadow: 0 0 8px rgba(20, 184, 166, 0.5);
}

/* Card Content */
.continue-card-content {
  padding: 0.875rem 1rem;
}

.continue-progress-text {
  font-size: 0.6875rem;
  color: #64748b;
  margin-bottom: 0.375rem;
}

.continue-card-title {
  font-size: 0.875rem;
  font-weight: 600;
  color: #0f172a;
  line-height: 1.4;
  margin: 0 0 0.625rem 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  transition: color 0.3s ease;
}

.continue-watching-card:hover .continue-card-title {
  color: #0d9488;
}

.continue-card-meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem;
}

.continue-author {
  display: flex;
  align-items: center;
  gap: 0.375rem;
}

.continue-author-avatar {
  width: 22px;
  height: 22px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 50%;
  color: white;
  font-size: 0.5rem;
  font-weight: 600;
}

.continue-author span {
  font-size: 0.6875rem;
  font-weight: 500;
  color: #475569;
}

.continue-last-watched {
  font-size: 0.625rem;
  color: #94a3b8;
  display: flex;
  align-items: center;
}

/* ============================================
   FEATURED CONTENT SECTION (Redesigned)
   ============================================ */
.featured-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1rem;
}

@media (min-width: 1024px) {
  .featured-grid {
    grid-template-columns: 2fr 1fr;
  }
}

/* Main Featured Card */
.featured-main-card {
  position: relative;
  border-radius: 1rem;
  overflow: hidden;
  cursor: pointer;
  background: #0f172a;
  aspect-ratio: 16/9;
}

@media (min-width: 1024px) {
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
  transition: transform 0.5s ease;
}

.featured-main-card:hover .featured-main-img {
  transform: scale(1.05);
}

.featured-main-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(to top, rgba(0,0,0,0.8) 0%, rgba(0,0,0,0.2) 50%, rgba(0,0,0,0.1) 100%);
}

.featured-main-badges {
  position: absolute;
  top: 0.75rem;
  left: 0.75rem;
  display: flex;
  gap: 0.5rem;
  z-index: 5;
}

.badge-featured {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.3rem 0.6rem;
  background: linear-gradient(135deg, #f59e0b, #d97706);
  color: white;
  font-size: 0.625rem;
  font-weight: 700;
  border-radius: 100px;
  text-transform: uppercase;
}

.badge-category {
  padding: 0.3rem 0.6rem;
  background: white;
  color: #0f172a;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 100px;
}

.featured-main-play {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 56px;
  height: 56px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
  border-radius: 50%;
  color: #0d9488;
  font-size: 1.25rem;
  box-shadow: 0 4px 20px rgba(0,0,0,0.3);
  transition: all 0.3s ease;
  z-index: 5;
}

.featured-main-card:hover .featured-main-play {
  transform: translate(-50%, -50%) scale(1.1);
  box-shadow: 0 6px 25px rgba(0,0,0,0.4);
}

.featured-main-play i {
  margin-left: 3px;
}

.featured-main-content {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 1rem;
  z-index: 5;
}

.featured-main-meta {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.5rem;
  font-size: 0.6875rem;
  color: rgba(255,255,255,0.8);
}

.featured-main-meta span {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.featured-main-title {
  font-size: 1.125rem;
  font-weight: 700;
  color: white;
  margin: 0 0 0.375rem 0;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.featured-main-desc {
  font-size: 0.75rem;
  color: rgba(255,255,255,0.7);
  margin: 0 0 0.5rem 0;
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.featured-main-author {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.6875rem;
  color: rgba(255,255,255,0.7);
}

.author-avatar-sm {
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  border-radius: 50%;
  color: white;
  font-size: 0.5rem;
  font-weight: 600;
}

.featured-main-author .dot {
  color: rgba(255,255,255,0.4);
}

/* Side Cards */
.featured-side-cards {
  display: flex;
  flex-direction: column;
  gap: 0.625rem;
}

.featured-side-card {
  display: flex;
  gap: 0.75rem;
  padding: 0.625rem;
  background: white;
  border: 1px solid #f1f5f9;
  border-radius: 0.75rem;
  cursor: pointer;
  transition: all 0.25s ease;
}

.featured-side-card:hover {
  border-color: #e2e8f0;
  box-shadow: 0 4px 12px rgba(0,0,0,0.06);
  transform: translateX(4px);
}

.side-card-thumb {
  position: relative;
  width: 100px;
  height: 60px;
  border-radius: 0.5rem;
  overflow: hidden;
  flex-shrink: 0;
  background: #1e293b;
}

.side-card-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.featured-side-card:hover .side-card-img {
  transform: scale(1.1);
}

.side-card-play {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0,0,0,0.3);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.featured-side-card:hover .side-card-play {
  opacity: 1;
}

.side-card-play i {
  color: white;
  font-size: 0.875rem;
}

.side-card-duration {
  position: absolute;
  bottom: 3px;
  right: 3px;
  padding: 0.125rem 0.375rem;
  background: rgba(0,0,0,0.8);
  color: white;
  font-size: 0.5625rem;
  font-weight: 500;
  border-radius: 0.25rem;
}

.side-card-info {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.side-card-category {
  font-size: 0.5625rem;
  font-weight: 600;
  color: #14b8a6;
  text-transform: uppercase;
  letter-spacing: 0.02em;
  margin-bottom: 0.25rem;
}

.side-card-title {
  font-size: 0.8125rem;
  font-weight: 600;
  color: #0f172a;
  margin: 0 0 0.25rem 0;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  transition: color 0.2s ease;
}

.featured-side-card:hover .side-card-title {
  color: #0d9488;
}

.side-card-meta {
  font-size: 0.625rem;
  color: #94a3b8;
}

.side-card-meta i {
  margin-right: 0.25rem;
}

/* Responsive */
@media (max-width: 1024px) {
  .featured-side-cards {
    flex-direction: row;
    overflow-x: auto;
    gap: 0.75rem;
    padding-bottom: 0.5rem;
  }

  .featured-side-card {
    flex: 0 0 260px;
  }
}

/* ============================================
   FEATURED + CONTINUE WATCHING ROW
   ============================================ */
.featured-continue-row {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1.5rem;
}

@media (min-width: 768px) {
  .featured-continue-row {
    grid-template-columns: 1.5fr 1fr;
  }
}

@media (min-width: 1024px) {
  .featured-continue-row {
    grid-template-columns: 2fr 1fr;
  }
}

.featured-column {
  min-width: 0;
  display: flex;
  flex-direction: column;
}

/* Featured Inner Grid: Main + Up Next */
.featured-inner-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1rem;
  flex: 1;
}

@media (min-width: 640px) {
  .featured-inner-grid {
    grid-template-columns: 1.8fr 1fr;
  }
}

.featured-carousel-wrapper {
  position: relative;
}

.featured-inner-grid .featured-main-card {
  aspect-ratio: 16/9;
  height: 100%;
}

/* Carousel Arrows */
.carousel-arrow {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.9);
  border: none;
  border-radius: 50%;
  color: #0f172a;
  font-size: 0.875rem;
  cursor: pointer;
  opacity: 0;
  transition: all 0.3s ease;
  z-index: 10;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
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
  left: 12px;
}

.carousel-next {
  right: 12px;
}

/* Carousel Dots */
.carousel-dots {
  position: absolute;
  bottom: 12px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: 8px;
  z-index: 10;
}

.carousel-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  border: none;
  background: rgba(255, 255, 255, 0.5);
  cursor: pointer;
  transition: all 0.3s ease;
  padding: 0;
}

.carousel-dot:hover {
  background: rgba(255, 255, 255, 0.8);
}

.carousel-dot.active {
  background: white;
  width: 24px;
  border-radius: 4px;
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

.continue-column {
  min-width: 0;
  display: flex;
  flex-direction: column;
}

.continue-column .continue-vertical-scroll {
  flex: 1;
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
  gap: 0.5rem;
  font-size: 0.8125rem;
  font-weight: 600;
  color: #334155;
  margin: 0 0 0.625rem 0;
}

.up-next-title i {
  color: #14b8a6;
}

.up-next-vertical {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  flex: 1;
}

/* Compact Up Next Card */
.up-next-card-compact {
  display: flex;
  gap: 0.625rem;
  padding: 0.5rem;
  background: white;
  border: 1px solid #f1f5f9;
  border-radius: 0.625rem;
  cursor: pointer;
  transition: all 0.25s ease;
}

.up-next-card-compact:hover {
  border-color: #e2e8f0;
  box-shadow: 0 4px 12px rgba(0,0,0,0.06);
  transform: translateX(4px);
}

.up-next-thumb-sm {
  position: relative;
  width: 90px;
  height: 54px;
  border-radius: 0.5rem;
  overflow: hidden;
  flex-shrink: 0;
  background: #1e293b;
}

.up-next-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.up-next-card-compact:hover .up-next-img {
  transform: scale(1.1);
}

.up-next-play-sm {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0,0,0,0.3);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.up-next-card-compact:hover .up-next-play-sm {
  opacity: 1;
}

.up-next-play-sm i {
  color: white;
  font-size: 0.75rem;
}

.up-next-duration {
  position: absolute;
  bottom: 3px;
  right: 3px;
  padding: 0.125rem 0.3rem;
  background: rgba(0,0,0,0.8);
  color: white;
  font-size: 0.5625rem;
  font-weight: 500;
  border-radius: 0.25rem;
}

.up-next-info-sm {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.up-next-category {
  font-size: 0.5625rem;
  font-weight: 600;
  color: #14b8a6;
  text-transform: uppercase;
  letter-spacing: 0.02em;
}

.up-next-card-title {
  font-size: 0.75rem;
  font-weight: 600;
  color: #0f172a;
  margin: 0.125rem 0;
  line-height: 1.25;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  transition: color 0.2s ease;
}

.up-next-card-compact:hover .up-next-card-title {
  color: #0d9488;
}

.up-next-views {
  font-size: 0.5625rem;
  color: #94a3b8;
}

.up-next-views i {
  margin-right: 0.25rem;
}

/* Continue Watching Vertical Scroll */
.continue-vertical-scroll {
  display: flex;
  flex-direction: column;
  gap: 0.625rem;
}

/* Continue Compact Card */
.continue-compact-card {
  display: flex;
  gap: 0.75rem;
  padding: 0.625rem;
  background: white;
  border: 1px solid #f1f5f9;
  border-radius: 0.75rem;
  cursor: pointer;
  transition: all 0.25s ease;
}

.continue-compact-card:hover {
  border-color: #e2e8f0;
  box-shadow: 0 4px 12px rgba(0,0,0,0.06);
  transform: translateX(4px);
}

.continue-compact-thumb {
  position: relative;
  width: 100px;
  height: 60px;
  border-radius: 0.5rem;
  overflow: hidden;
  flex-shrink: 0;
  background: #1e293b;
}

.continue-compact-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.continue-compact-card:hover .continue-compact-img {
  transform: scale(1.1);
}

.continue-compact-play {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0,0,0,0.3);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.continue-compact-card:hover .continue-compact-play {
  opacity: 1;
}

.continue-compact-play i {
  color: white;
  font-size: 0.875rem;
}

.continue-compact-duration {
  position: absolute;
  bottom: 3px;
  right: 3px;
  padding: 0.125rem 0.375rem;
  background: rgba(0,0,0,0.8);
  color: white;
  font-size: 0.5625rem;
  font-weight: 500;
  border-radius: 0.25rem;
}

.continue-compact-progress {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: rgba(255,255,255,0.3);
}

.continue-compact-progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  border-radius: 0 0 0.5rem 0.5rem;
}

.continue-compact-info {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.continue-compact-category {
  font-size: 0.5625rem;
  font-weight: 600;
  color: #14b8a6;
  text-transform: uppercase;
  letter-spacing: 0.02em;
  margin-bottom: 0.25rem;
}

.continue-compact-title {
  font-size: 0.8125rem;
  font-weight: 600;
  color: #0f172a;
  margin: 0 0 0.25rem 0;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  transition: color 0.2s ease;
}

.continue-compact-card:hover .continue-compact-title {
  color: #0d9488;
}

.continue-compact-meta {
  display: flex;
  gap: 0.75rem;
  font-size: 0.625rem;
  color: #94a3b8;
}

.continue-compact-meta i {
  margin-right: 0.25rem;
  color: #14b8a6;
}

/* Responsive - Stack on mobile */
@media (max-width: 1024px) {
  .continue-vertical-scroll {
    flex-direction: row;
    overflow-x: auto;
    gap: 0.75rem;
    padding-bottom: 0.5rem;
  }

  .continue-compact-card {
    flex: 0 0 260px;
  }
}


/* Trending Section */
.trending-section {
  background: linear-gradient(135deg, #fefefe 0%, #f8fafc 100%);
  border-radius: 1.5rem;
  padding: 1.5rem;
  border: 1px solid #e2e8f0;
}

.trending-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1.25rem;
}

.trending-title {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 1.125rem;
  font-weight: 700;
  color: #1e293b;
}

.trending-icon-wrapper {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 0.75rem;
  background: linear-gradient(135deg, #ef4444 0%, #f97316 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(239, 68, 68, 0.3);
}

.trending-icon-wrapper i {
  color: white;
  font-size: 0.875rem;
  animation: flicker 1.5s ease-in-out infinite;
}

@keyframes flicker {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}

.trending-subtitle {
  font-size: 0.75rem;
  font-weight: 500;
  color: #64748b;
}

.trending-nav {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.trending-nav-btn {
  width: 2rem;
  height: 2rem;
  border-radius: 0.5rem;
  background: white;
  border: 1px solid #e2e8f0;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s ease;
  color: #64748b;
}

.trending-nav-btn:hover {
  background: #0d9488;
  border-color: #0d9488;
  color: white;
}

.trending-view-all {
  padding: 0.5rem 1rem;
  font-size: 0.8125rem;
  font-weight: 600;
  color: #0d9488;
  background: #f0fdfa;
  border-radius: 0.5rem;
  text-decoration: none;
  display: flex;
  align-items: center;
  gap: 0.375rem;
  transition: all 0.2s ease;
}

.trending-view-all:hover {
  background: #0d9488;
  color: white;
}

.trending-scroll {
  display: flex;
  gap: 1rem;
  overflow-x: auto;
  overflow-y: visible;
  padding: 0.5rem 0.25rem 1rem;
  scroll-snap-type: x mandatory;
  scroll-behavior: smooth;
}

.trending-scroll::-webkit-scrollbar {
  height: 4px;
}

.trending-scroll::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 4px;
}

.trending-scroll::-webkit-scrollbar-thumb {
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  border-radius: 4px;
}

.trending-card {
  flex: 0 0 290px;
  scroll-snap-align: start;
  position: relative;
  display: flex;
  gap: 0.75rem;
}

.trending-rank {
  flex-shrink: 0;
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.125rem;
  font-weight: 800;
  color: white;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 0.5rem;
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.3);
}

.trending-card-inner {
  flex: 1;
  background: white;
  border-radius: 1rem;
  overflow: hidden;
  border: 1px solid #e2e8f0;
  cursor: pointer;
  transition: all 0.3s ease;
}

.trending-card-inner:hover {
  border-color: #14b8a6;
  box-shadow: 0 8px 25px rgba(20, 184, 166, 0.15);
  transform: translateY(-4px);
}

.trending-thumbnail {
  position: relative;
  aspect-ratio: 16/9;
  overflow: hidden;
}

.trending-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.trending-card-inner:hover .trending-img {
  transform: scale(1.08);
}

.trending-overlay {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.1);
  transition: background 0.3s ease;
}

.trending-card-inner:hover .trending-overlay {
  background: rgba(0, 0, 0, 0.4);
}

.trending-play-btn {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%) scale(0.8);
  width: 3rem;
  height: 3rem;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.95);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
}

.trending-play-btn i {
  color: #0d9488;
  font-size: 1rem;
  margin-left: 2px;
}

.trending-card-inner:hover .trending-play-btn {
  opacity: 1;
  transform: translate(-50%, -50%) scale(1);
}

.trending-badges {
  position: absolute;
  bottom: 0.5rem;
  left: 0.5rem;
  display: flex;
  gap: 0.375rem;
  z-index: 10;
}

.trending-new-badge {
  padding: 0.25rem 0.5rem;
  font-size: 0.625rem;
  font-weight: 600;
  background: linear-gradient(135deg, #8b5cf6 0%, #6366f1 100%);
  color: white;
  border-radius: 0.375rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.trending-hot-badge {
  padding: 0.25rem 0.5rem;
  font-size: 0.625rem;
  font-weight: 600;
  background: linear-gradient(135deg, #ef4444 0%, #f97316 100%);
  color: white;
  border-radius: 0.375rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.trending-duration {
  position: absolute;
  bottom: 0.5rem;
  right: 0.5rem;
  padding: 0.25rem 0.5rem;
  font-size: 0.6875rem;
  font-weight: 600;
  background: rgba(0, 0, 0, 0.75);
  color: white;
  border-radius: 0.375rem;
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.trending-actions {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
  opacity: 0;
  transform: translateX(8px);
  transition: all 0.3s ease;
  z-index: 10;
}

.trending-card-inner:hover .trending-actions {
  opacity: 1;
  transform: translateX(0);
}

.trending-action-btn {
  width: 1.75rem;
  height: 1.75rem;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.95);
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s ease;
  color: #64748b;
  font-size: 0.6875rem;
}

.trending-action-btn:hover {
  background: #0d9488;
  color: white;
  transform: scale(1.1);
}

.trending-action-btn.active {
  background: #fee2e2;
  color: #ef4444;
}

.trending-action-btn.active:hover {
  background: #ef4444;
  color: white;
}

.trending-action-btn.saved {
  background: #fef3c7;
  color: #f59e0b;
}

.trending-action-btn.saved:hover {
  background: #f59e0b;
  color: white;
}

.trending-info {
  padding: 0.875rem;
}

.trending-info-header {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  margin-bottom: 0.5rem;
}

.trending-info-header .category-tag {
  display: inline-flex;
  align-items: center;
  padding: 0.0625rem 0.25rem;
  font-size: 0.5rem;
  font-weight: 600;
  border-radius: 0.1875rem;
  text-transform: uppercase;
}

.trending-type-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.125rem;
  padding: 0.0625rem 0.25rem;
  font-size: 0.5rem;
  font-weight: 600;
  border-radius: 0.1875rem;
  text-transform: uppercase;
}

.trending-type-badge.video {
  background: #dbeafe;
  color: #2563eb;
}

.trending-type-badge.audio {
  background: #fce7f3;
  color: #db2777;
}

.trending-type-badge.image {
  background: #d1fae5;
  color: #059669;
}

.trending-type-badge.gallery {
  background: #ede9fe;
  color: #7c3aed;
}

.trending-card-title {
  font-size: 0.8125rem;
  font-weight: 600;
  color: #1e293b;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  margin-bottom: 0.5rem;
  transition: color 0.2s ease;
  min-height: 2.275rem; /* 2 lines: font-size * line-height * 2 */
}

.trending-card-inner:hover .trending-card-title {
  color: #0d9488;
}

.trending-author {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}

.trending-author-avatar {
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 50%;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.625rem;
  font-weight: 700;
  color: white;
}

.trending-author-name {
  font-size: 0.6875rem;
  font-weight: 500;
  color: #475569;
}

.trending-stats {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding-top: 0.5rem;
  border-top: 1px solid #f1f5f9;
}

.trending-stat {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.625rem;
  color: #64748b;
}

.trending-stat i {
  font-size: 0.5625rem;
  color: #94a3b8;
}

.trending-stat:first-child i {
  color: #ef4444;
}

.trending-stat:nth-child(2) i {
  color: #f472b6;
}

.trending-meta {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.6875rem;
  color: #64748b;
  margin-bottom: 0.5rem;
}

.trending-views {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.trending-views i {
  color: #ef4444;
}

.trending-category .category-tag {
  display: inline-flex;
  align-items: center;
  padding: 0.0625rem 0.25rem;
  font-size: 0.5rem;
  font-weight: 600;
  border-radius: 0.1875rem;
  text-transform: uppercase;
}

.category-tag.videos {
  background: #dbeafe;
  color: #2563eb;
}

.category-tag.highlights {
  background: #fef3c7;
  color: #d97706;
}

.category-tag.interviews {
  background: #d1fae5;
  color: #059669;
}

.category-tag.behind {
  background: #fce7f3;
  color: #db2777;
}

.category-tag.galleries {
  background: #ede9fe;
  color: #7c3aed;
}

.category-tag.teams {
  background: #dbeafe;
  color: #2563eb;
}

.category-tag.venues {
  background: #fef3c7;
  color: #d97706;
}

.category-tag.fans {
  background: #fce7f3;
  color: #db2777;
}

.category-tag.matches {
  background: #dcfce7;
  color: #16a34a;
}

.category-tag.behind {
  background: #f1f5f9;
  color: #475569;
}

/* Progress Bar on Cards */
.watch-progress {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: rgba(255,255,255,0.3);
}

.watch-progress-bar {
  height: 100%;
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  transition: width 0.3s ease;
}

/* Section Headers */
.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.section-title {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
}

.section-title i {
  width: 1.75rem;
  height: 1.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  transition: all 0.3s ease;
}

.section-title i.fa-history {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.25);
  color: #14b8a6;
}

.section-title i.fa-star {
  background: linear-gradient(135deg, #fefce8 0%, #fef9c3 100%);
  box-shadow: 0 2px 8px rgba(234, 179, 8, 0.25);
  color: #eab308;
}

.section-title i.fa-fire {
  background: linear-gradient(135deg, #fef2f2 0%, #fee2e2 100%);
  box-shadow: 0 2px 8px rgba(239, 68, 68, 0.25);
  color: #ef4444;
}

.section-title i.fa-th-large {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.25);
  color: #14b8a6;
}

.section-header:hover .section-title i {
  transform: scale(1.1) rotate(-5deg);
}

/* Enhanced Featured Section */
.featured-gradient {
  background: linear-gradient(135deg, #0f766e 0%, #115e59 50%, #134e4a 100%);
}

.featured-play-btn {
  animation: pulse-ring 2s ease-out infinite;
}

@keyframes pulse-ring {
  0% { box-shadow: 0 0 0 0 rgba(255, 255, 255, 0.4); }
  70% { box-shadow: 0 0 0 20px rgba(255, 255, 255, 0); }
  100% { box-shadow: 0 0 0 0 rgba(255, 255, 255, 0); }
}

/* Media Card Improvements */
.media-card-enhanced {
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  border: 1px solid #f1f5f9;
  position: relative;
}

.media-card-enhanced::before {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: 1rem;
  padding: 2px;
  background: linear-gradient(135deg, transparent 0%, transparent 100%);
  -webkit-mask: linear-gradient(#fff 0 0) content-box, linear-gradient(#fff 0 0);
  mask: linear-gradient(#fff 0 0) content-box, linear-gradient(#fff 0 0);
  -webkit-mask-composite: xor;
  mask-composite: exclude;
  opacity: 0;
  transition: opacity 0.4s ease;
  pointer-events: none;
}

.media-card-enhanced:hover::before {
  background: linear-gradient(135deg, #14b8a6 0%, #06b6d4 50%, #10b981 100%);
  opacity: 1;
}

.media-card-enhanced:hover {
  transform: translateY(-6px);
  box-shadow:
    0 20px 40px -15px rgba(20, 184, 166, 0.25),
    0 0 0 1px rgba(20, 184, 166, 0.1);
}

.media-card-enhanced .card-thumbnail {
  position: relative;
  overflow: hidden;
}

.media-card-enhanced:hover .card-thumbnail img,
.media-card-enhanced:hover .card-thumbnail .thumbnail-gradient {
  transform: scale(1.05);
}

.thumbnail-gradient {
  transition: transform 0.4s ease;
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
  height: 44px;
  padding-left: 2.75rem;
  padding-right: 2rem;
  border-radius: 0.75rem;
  border: 1.5px solid #e2e8f0;
  background: linear-gradient(135deg, #fafafa 0%, #f8fafc 100%);
  font-size: 0.875rem;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.search-input:focus {
  border-color: #14b8a6;
  box-shadow: 0 0 0 4px rgba(20, 184, 166, 0.1), 0 4px 12px rgba(20, 184, 166, 0.1);
  background: white;
  outline: none;
}

.search-input::placeholder {
  color: #94a3b8;
}

/* Filter Button */
.filter-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  background: white;
  border: 1.5px solid #e2e8f0;
  border-radius: 0.75rem;
  font-size: 0.8125rem;
  font-weight: 600;
  color: #475569;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.filter-btn:hover {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-color: #5eead4;
  color: #0d9488;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.15);
}

.filter-btn.active {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-color: #14b8a6;
  color: #0d9488;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.15);
}

.filter-btn i {
  transition: transform 0.3s ease;
}

.filter-btn:hover i {
  transform: rotate(90deg);
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
  box-shadow: 0 2px 6px rgba(20, 184, 166, 0.3);
}

/* Sort Select */
.sort-select {
  height: 36px;
  padding: 0 1.75rem 0 0.625rem;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  background: white url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='14' height='14' viewBox='0 0 24 24' fill='none' stroke='%2394a3b8' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpolyline points='6 9 12 15 18 9'%3E%3C/polyline%3E%3C/svg%3E") no-repeat right 0.5rem center;
  font-size: 0.8125rem;
  color: #334155;
  cursor: pointer;
  appearance: none;
}

.sort-select:focus {
  outline: none;
  border-color: #14b8a6;
}

/* View Toggle */
.view-toggle {
  display: flex;
  background: #f1f5f9;
  border-radius: 0.5rem;
  padding: 0.1875rem;
}

.view-toggle button {
  padding: 0.375rem 0.5rem;
  border: none;
  background: transparent;
  color: #64748b;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: all 0.15s ease;
  font-size: 0.8125rem;
}

.view-toggle button:hover {
  color: #334155;
}

.view-toggle button.active {
  background: white;
  color: #0d9488;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

/* Toolbar */
.toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1.25rem;
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

/* Filters Panel */
.filters-panel {
  margin-bottom: 1.25rem;
  padding: 1rem;
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  border-radius: 0.875rem;
  border: 1px solid #e2e8f0;
  animation: slideDown 0.25s ease-out;
}

@keyframes slideDown {
  from { opacity: 0; transform: translateY(-8px); }
  to { opacity: 1; transform: translateY(0); }
}

.filters-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.875rem;
}

.filters-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.8125rem;
  font-weight: 600;
  color: #334155;
}

.filters-title i {
  color: #14b8a6;
  font-size: 0.75rem;
}

.clear-filters-btn {
  padding: 0.25rem 0.5rem;
  background: transparent;
  border: none;
  font-size: 0.75rem;
  font-weight: 500;
  color: #ef4444;
  cursor: pointer;
}

.filter-groups {
  display: flex;
  flex-wrap: wrap;
  gap: 1.25rem;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
}

.filter-group-label {
  font-size: 0.625rem;
  font-weight: 600;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.filter-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
}

.filter-chip {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.4375rem 0.75rem;
  background: white;
  border: 1.5px solid #e2e8f0;
  border-radius: 100px;
  font-size: 0.75rem;
  font-weight: 500;
  color: #475569;
  cursor: pointer;
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.04);
}

.filter-chip:hover {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-color: #5eead4;
  color: #0d9488;
  transform: translateY(-1px);
  box-shadow: 0 3px 10px rgba(20, 184, 166, 0.15);
}

.filter-chip.active {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-color: transparent;
  color: white;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.35);
  transform: translateY(-1px);
}

.filter-chip.active:hover {
  box-shadow: 0 6px 20px rgba(20, 184, 166, 0.4);
}

/* All Media Section Header */
.all-media-section {
  background: white;
  border-radius: 1.25rem;
  border: 1px solid #e2e8f0;
  padding: 1.5rem;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.03);
}

.all-media-header {
  margin-bottom: 1.25rem;
}

.all-media-title-row {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 1.5rem;
  margin-bottom: 1.25rem;
  flex-wrap: wrap;
}

.all-media-title {
  display: flex;
  align-items: center;
  gap: 0.875rem;
  margin: 0;
}

.all-media-icon {
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 0.875rem;
  color: white;
  font-size: 1.25rem;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.all-media-heading {
  display: block;
  font-size: 1.25rem;
  font-weight: 700;
  color: #0f172a;
  line-height: 1.3;
}

.all-media-subtitle {
  display: block;
  font-size: 0.8125rem;
  font-weight: 400;
  color: #64748b;
  margin-top: 0.125rem;
}

.all-media-stats {
  display: flex;
  gap: 1rem;
}

.media-stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 0.75rem 1rem;
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  border-radius: 0.75rem;
  border: 1px solid #e2e8f0;
  min-width: 70px;
  transition: all 0.3s ease;
}

.media-stat-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  border-color: #14b8a6;
}

.media-stat-item i {
  font-size: 1rem;
  color: #14b8a6;
  margin-bottom: 0.25rem;
}

.media-stat-item .stat-count {
  font-size: 1.125rem;
  font-weight: 700;
  color: #0f172a;
  line-height: 1.2;
}

.media-stat-item .stat-label {
  font-size: 0.625rem;
  font-weight: 500;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.03em;
}

/* Media Type Tabs */
.media-type-tabs {
  display: flex;
  gap: 0.5rem;
  padding: 0.375rem;
  background: #f1f5f9;
  border-radius: 0.75rem;
  overflow-x: auto;
}

.media-type-tab {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  background: transparent;
  border: none;
  border-radius: 0.5rem;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #64748b;
  cursor: pointer;
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
  white-space: nowrap;
}

.media-type-tab:hover {
  color: #0f172a;
  background: rgba(255, 255, 255, 0.5);
}

.media-type-tab.active {
  background: white;
  color: #0d9488;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.media-type-tab i {
  font-size: 0.875rem;
}

.tab-count {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 20px;
  height: 20px;
  padding: 0 6px;
  background: #e2e8f0;
  border-radius: 100px;
  font-size: 0.6875rem;
  font-weight: 600;
  color: #64748b;
}

.media-type-tab.active .tab-count {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
}

/* Search Clear Button */
.search-clear {
  position: absolute;
  right: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #e2e8f0;
  border: none;
  border-radius: 50%;
  color: #64748b;
  font-size: 0.625rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.search-clear:hover {
  background: #ef4444;
  color: white;
}

/* Results Count */
.results-count {
  font-size: 0.8125rem;
  font-weight: 500;
  color: #64748b;
}

/* Sort Wrapper */
.sort-wrapper {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 0.75rem;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
}

.sort-wrapper i {
  font-size: 0.75rem;
  color: #64748b;
}

.sort-wrapper .sort-select {
  border: none;
  background: transparent;
  padding: 0;
  height: auto;
}

/* Card Play Button */
.card-play-btn {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%) scale(0.8);
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 50%;
  color: #0f172a;
  font-size: 1rem;
  opacity: 0;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.2);
}

.media-card-enhanced:hover .card-play-btn {
  opacity: 1;
  transform: translate(-50%, -50%) scale(1);
}

.card-play-btn i {
  margin-left: 2px;
}

/* Card Hover Actions */
.card-hover-actions {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
  opacity: 0;
  transform: translateX(10px);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.media-card-enhanced:hover .card-hover-actions {
  opacity: 1;
  transform: translateX(0);
}

.card-action-btn {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.95);
  border: none;
  border-radius: 50%;
  color: #475569;
  font-size: 0.75rem;
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.card-action-btn:hover {
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.card-action-btn.active {
  background: #fee2e2;
  color: #ef4444;
}

.card-action-btn.active:hover {
  background: #ef4444;
  color: white;
}

.card-action-btn.saved {
  background: #fef3c7;
  color: #f59e0b;
}

.card-action-btn.saved:hover {
  background: #f59e0b;
  color: white;
}

/* Media Type Badge */
.media-type-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.25rem;
  text-transform: uppercase;
  letter-spacing: 0.03em;
}

.media-type-badge.video {
  background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%);
  color: #1d4ed8;
}

.media-type-badge.audio {
  background: linear-gradient(135deg, #f3e8ff 0%, #e9d5ff 100%);
  color: #7c3aed;
}

.media-type-badge.image {
  background: linear-gradient(135deg, #dcfce7 0%, #bbf7d0 100%);
  color: #16a34a;
}

.media-type-badge.gallery {
  background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
  color: #d97706;
}

/* Media Category Tag */
.media-category-tag {
  display: inline-flex;
  align-items: center;
  padding: 0.25rem 0.5rem;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.25rem;
  text-transform: uppercase;
  letter-spacing: 0.03em;
}

.media-category-tag.highlights {
  background: #fef3c7;
  color: #d97706;
}

.media-category-tag.interviews {
  background: #dbeafe;
  color: #1d4ed8;
}

.media-category-tag.behind {
  background: #f3e8ff;
  color: #7c3aed;
}

.media-category-tag.matches {
  background: #dcfce7;
  color: #16a34a;
}

.media-category-tag.teams {
  background: #fee2e2;
  color: #dc2626;
}

.media-category-tag.venues {
  background: #ccfbf1;
  color: #0d9488;
}

.media-category-tag.fans {
  background: #fce7f3;
  color: #db2777;
}

/* Media Duration Badge */
.media-duration-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.625rem;
  background: rgba(0, 0, 0, 0.75);
  backdrop-filter: blur(4px);
  border-radius: 0.375rem;
  color: white;
  font-size: 0.6875rem;
  font-weight: 600;
}

.media-duration-badge i {
  font-size: 0.625rem;
  opacity: 0.8;
}

/* Transcript Badge */
.transcript-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(4px);
  border-radius: 0.25rem;
  color: #475569;
  font-size: 0.625rem;
  font-weight: 600;
}

/* List View Type Badge */
.list-type-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.25rem;
  text-transform: uppercase;
}

.list-type-badge.video {
  background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%);
  color: #1d4ed8;
}

.list-type-badge.audio {
  background: linear-gradient(135deg, #f3e8ff 0%, #e9d5ff 100%);
  color: #7c3aed;
}

.list-type-badge.image {
  background: linear-gradient(135deg, #dcfce7 0%, #bbf7d0 100%);
  color: #16a34a;
}

.list-type-badge.gallery {
  background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
  color: #d97706;
}

/* List Category Badge */
.list-category-badge {
  display: inline-flex;
  align-items: center;
  padding: 0.25rem 0.5rem;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.25rem;
  text-transform: uppercase;
}

.list-category-badge.highlights {
  background: #fef3c7;
  color: #d97706;
}

.list-category-badge.interviews {
  background: #dbeafe;
  color: #1d4ed8;
}

.list-category-badge.behind {
  background: #f3e8ff;
  color: #7c3aed;
}

.list-category-badge.matches {
  background: #dcfce7;
  color: #16a34a;
}

.list-category-badge.teams {
  background: #fee2e2;
  color: #dc2626;
}

.list-category-badge.venues {
  background: #ccfbf1;
  color: #0d9488;
}

.list-category-badge.fans {
  background: #fce7f3;
  color: #db2777;
}

/* List Action Button States */
.list-action-btn.active {
  background: #fee2e2;
  color: #ef4444;
}

.list-action-btn.active:hover {
  background: #ef4444;
  color: white;
}

.list-action-btn.saved {
  background: #fef3c7;
  color: #f59e0b;
}

.list-action-btn.saved:hover {
  background: #f59e0b;
  color: white;
}

/* Content Area Wrapper */
.content-area {
  background: white;
  border-radius: 1rem;
  border: 1px solid #f3f4f6;
  box-shadow: 0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1);
  overflow: hidden;
  animation: fadeInUp 0.5s ease-out;
}

.content-area-inner {
  padding: 1.25rem;
}

.all-media-section .content-area {
  margin-top: 0;
}

/* Media Grid */
.media-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 1.25rem;
}

/* Card Body Enhanced */
.card-body-enhanced {
  padding: 1rem;
  display: flex;
  flex-direction: column;
  gap: 0.625rem;
}

.card-meta-row {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  font-size: 0.6875rem;
  color: #94a3b8;
}

.card-title-enhanced {
  margin: 0;
  font-size: 0.9375rem;
  font-weight: 600;
  line-height: 1.4;
  color: #0f172a;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  transition: color 0.3s ease;
}

.media-card-enhanced:hover .card-title-enhanced {
  color: #0d9488;
}

.card-footer-enhanced {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: auto;
  padding-top: 0.75rem;
  border-top: 1px solid #f1f5f9;
}

.author-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.author-avatar {
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 50%;
  box-shadow: 0 2px 6px rgba(20, 184, 166, 0.3);
}

.author-avatar .avatar-text {
  color: white;
  font-size: 0.625rem;
  font-weight: 600;
}

.author-name {
  font-size: 0.75rem;
  font-weight: 500;
  color: #334155;
}

.card-stats {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.6875rem;
  color: #94a3b8;
}

.card-stats span {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

/* Category Badge Enhanced */
.category-badge {
  padding: 0.3125rem 0.625rem;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(8px);
  color: #475569;
  font-size: 0.6875rem;
  font-weight: 600;
  border-radius: 0.375rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

/* New Badge Animation */
.new-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.625rem;
  font-weight: 700;
  border-radius: 0.375rem;
  text-transform: uppercase;
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.4);
  animation: badgePulse 2s ease-in-out infinite;
}

@keyframes badgePulse {
  0%, 100% { box-shadow: 0 2px 8px rgba(20, 184, 166, 0.4); }
  50% { box-shadow: 0 4px 15px rgba(20, 184, 166, 0.6); }
}

/* Card Overlay on Hover */
.card-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(to top, rgba(13, 148, 136, 0.9) 0%, rgba(13, 148, 136, 0.4) 40%, transparent 70%);
  display: flex;
  align-items: flex-end;
  justify-content: center;
  padding: 1rem;
  opacity: 0;
  transition: opacity 0.4s ease;
}

.media-card-enhanced:hover .card-overlay {
  opacity: 1;
}

.overlay-btn {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 1rem;
  background: white;
  border: none;
  border-radius: 100px;
  font-size: 0.8125rem;
  font-weight: 600;
  color: #0d9488;
  cursor: pointer;
  transform: translateY(10px);
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
}

.media-card-enhanced:hover .overlay-btn {
  transform: translateY(0);
}

.overlay-btn:hover {
  background: #f0fdfa;
  transform: translateY(-2px) !important;
}

/* Gallery Stack Effect */
.gallery-stack {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
}

.gallery-stack-photo {
  position: absolute;
  width: 75%;
  height: 75%;
  border-radius: 0.75rem;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.15);
  transition: all 0.3s ease;
}

.media-card-enhanced:hover .gallery-stack-photo {
  transform: rotate(0deg) translateY(0) scale(1.02) !important;
}

.media-card-enhanced:hover .gallery-stack-photo:nth-child(1) {
  transform: translateX(-15%) rotate(-5deg) !important;
}

.media-card-enhanced:hover .gallery-stack-photo:nth-child(2) {
  transform: translateX(0) rotate(0deg) scale(1.05) !important;
  z-index: 5 !important;
}

.media-card-enhanced:hover .gallery-stack-photo:nth-child(3) {
  transform: translateX(15%) rotate(5deg) !important;
}

/* Empty State Enhanced */
.empty-state {
  text-align: center;
  padding: 3rem 1.5rem;
}

.empty-state-icon {
  width: 64px;
  height: 64px;
  margin: 0 auto 1rem;
  background: linear-gradient(135deg, #f1f5f9 0%, #e2e8f0 100%);
  border-radius: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.empty-state-icon i {
  font-size: 1.75rem;
  background: linear-gradient(135deg, #94a3b8 0%, #64748b 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.empty-state-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0 0 0.5rem;
}

.empty-state-text {
  font-size: 0.875rem;
  color: #64748b;
  margin: 0 0 1.25rem;
}

/* View All Link */
.view-all-link {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: #0d9488;
  text-decoration: none;
  transition: all 0.3s ease;
}

.view-all-link:hover {
  color: #0f766e;
  transform: translateX(2px);
}

.view-all-link i {
  transition: transform 0.3s ease;
}

.view-all-link:hover i {
  transform: translateX(3px);
}

/* Featured Section Card */
.featured-card {
  background: white;
  border-radius: 1.5rem;
  overflow: hidden;
  border: 1px solid #f1f5f9;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.featured-card:hover {
  box-shadow: 0 8px 30px rgba(20, 184, 166, 0.15);
}

/* Up Next Item */
.up-next-item {
  display: flex;
  gap: 0.75rem;
  padding: 0.625rem;
  border-radius: 0.75rem;
  cursor: pointer;
  transition: all 0.25s ease;
  border: 1px solid transparent;
}

.up-next-item:hover {
  background: white;
  border-color: #e2e8f0;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.up-next-item:hover .up-next-title {
  color: #0d9488;
}

.up-next-thumbnail {
  width: 96px;
  height: 60px;
  border-radius: 0.5rem;
  flex-shrink: 0;
  position: relative;
  overflow: hidden;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

.up-next-title {
  font-size: 0.8125rem;
  font-weight: 600;
  color: #0f172a;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  transition: color 0.25s ease;
}

.up-next-meta {
  font-size: 0.6875rem;
  color: #94a3b8;
  margin-top: 0.25rem;
}

/* Animation Classes */
.animate-in {
  animation: fadeInUp 0.5s cubic-bezier(0.4, 0, 0.2, 1) forwards;
}

.delay-1 { animation-delay: 0.05s; }
.delay-2 { animation-delay: 0.1s; }
.delay-3 { animation-delay: 0.15s; }
.delay-4 { animation-delay: 0.2s; }

/* Pagination (Documents Style) */
.pagination-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 1.5rem;
  background: white;
  border-radius: 0.75rem;
  border: 1px solid #f1f5f9;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04);
}

.pagination-left {
  flex: 1;
}

.pagination-center {
  display: flex;
  align-items: center;
  gap: 0.375rem;
}

.pagination-right {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 0.5rem;
}

.pagination-info {
  font-size: 0.8125rem;
  color: #64748b;
}

.per-page-label {
  font-size: 0.8125rem;
  color: #64748b;
}

.per-page-select-wrapper {
  position: relative;
}

.per-page-select {
  height: 34px;
  padding: 0 2rem 0 0.75rem;
  border: 1.5px solid #e2e8f0;
  border-radius: 0.5rem;
  background: white url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='%2394a3b8' stroke-width='2'%3E%3Cpath d='M6 9l6 6 6-6'/%3E%3C/svg%3E") no-repeat right 0.5rem center;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #334155;
  cursor: pointer;
  appearance: none;
  transition: all 0.2s ease;
}

.per-page-select:hover {
  border-color: #cbd5e1;
}

.per-page-select:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.pagination-btn {
  min-width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  background: white;
  color: #475569;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.pagination-btn:hover:not(:disabled) {
  background: #f0fdfa;
  border-color: #14b8a6;
  color: #0d9488;
}

.pagination-btn.active {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-color: transparent;
  color: white;
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
}

.modal-content {
  background: white;
  border-radius: 1.5rem;
  width: 100%;
  max-width: 500px;
  max-height: 90vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25);
}

.modal-lg {
  max-width: 700px;
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
  font-weight: 600;
  color: #0f172a;
  margin: 0;
}

.modal-close {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  background: #f1f5f9;
  border-radius: 0.5rem;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
}

.modal-close:hover {
  background: #fee2e2;
  color: #ef4444;
}

.modal-body {
  padding: 1.5rem;
  overflow-y: auto;
  flex: 1;
}

.modal-footer {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 1rem 1.5rem;
  border-top: 1px solid #f1f5f9;
  background: #f8fafc;
}

/* Form Styles */
.label {
  display: block;
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
  margin-bottom: 0.375rem;
}

.input {
  width: 100%;
  padding: 0.625rem 0.875rem;
  border: 1.5px solid #e2e8f0;
  border-radius: 0.75rem;
  font-size: 0.875rem;
  transition: all 0.2s ease;
}

.input:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.textarea {
  resize: vertical;
  min-height: 80px;
}

.select {
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%2394a3b8' stroke-width='2'%3E%3Cpath d='M6 9l6 6 6-6'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 0.75rem center;
  padding-right: 2.5rem;
}

.btn {
  padding: 0.625rem 1.25rem;
  border-radius: 0.75rem;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
  border: none;
}

.btn-secondary {
  background: #f1f5f9;
  color: #475569;
}

.btn-secondary:hover {
  background: #e2e8f0;
}

/* Modal Transition */
.modal-enter-active,
.modal-leave-active {
  transition: all 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .modal-content,
.modal-leave-to .modal-content {
  transform: scale(0.95) translateY(20px);
}

/* List View Styles */
.media-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.media-list-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.875rem;
  background: white;
  border: 1px solid #f1f5f9;
  border-radius: 0.875rem;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.media-list-item:hover {
  border-color: #e2e8f0;
  box-shadow: 0 4px 15px rgba(20, 184, 166, 0.08);
  transform: translateX(4px);
}

.list-item-thumbnail {
  position: relative;
  width: 160px;
  height: 90px;
  border-radius: 0.625rem;
  overflow: hidden;
  flex-shrink: 0;
}

.gallery-mini-stack {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.gallery-mini-photo {
  position: absolute;
  width: 80%;
  height: 80%;
  border-radius: 0.375rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.list-play-overlay {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.2);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.media-list-item:hover .list-play-overlay {
  opacity: 1;
}

.list-play-overlay i {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
  border-radius: 50%;
  color: #0d9488;
  font-size: 0.75rem;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
}

.list-duration-badge {
  position: absolute;
  bottom: 0.375rem;
  right: 0.375rem;
  padding: 0.1875rem 0.5rem;
  background: rgba(0, 0, 0, 0.75);
  backdrop-filter: blur(4px);
  color: white;
  font-size: 0.6875rem;
  font-weight: 500;
  border-radius: 0.25rem;
}

.list-item-content {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.list-item-header {
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
}

.list-badges {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.list-category-badge {
  padding: 0.25rem 0.625rem;
  font-size: 0.6875rem;
  font-weight: 600;
  border-radius: 100px;
}

.list-type-badge {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.6875rem;
  color: #64748b;
}

.list-type-badge i {
  font-size: 0.625rem;
}

.list-new-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.1875rem;
  padding: 0.1875rem 0.5rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.625rem;
  font-weight: 700;
  border-radius: 100px;
  text-transform: uppercase;
}

.list-item-title {
  margin: 0;
  font-size: 0.9375rem;
  font-weight: 600;
  color: #0f172a;
  line-height: 1.4;
  transition: color 0.3s ease;
}

.media-list-item:hover .list-item-title {
  color: #0d9488;
}

.list-item-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
}

.list-author {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.list-author-avatar {
  width: 26px;
  height: 26px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 50%;
  color: white;
  font-size: 0.5625rem;
  font-weight: 600;
}

.list-author span {
  font-size: 0.75rem;
  font-weight: 500;
  color: #475569;
}

.list-stats {
  display: flex;
  align-items: center;
  gap: 0.875rem;
  font-size: 0.6875rem;
  color: #94a3b8;
}

.list-stats span {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.list-stats i {
  font-size: 0.5625rem;
}

.list-item-actions {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.media-list-item:hover .list-item-actions {
  opacity: 1;
}

.list-action-btn {
  width: 34px;
  height: 34px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  color: #64748b;
  font-size: 0.8125rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.list-action-btn:hover {
  background: #f0fdfa;
  border-color: #99f6e4;
  color: #0d9488;
}

/* Responsive Adjustments */
@media (max-width: 1024px) {
  .search-box {
    width: 100%;
  }

  .media-grid {
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  }
}

@media (max-width: 768px) {
  .media-grid {
    grid-template-columns: 1fr;
  }

  .dropzone-card {
    width: 100%;
    flex-direction: row;
    gap: 1rem;
    padding: 1rem 1.5rem;
  }

  .dropzone-card-icon {
    margin-bottom: 0;
  }

  .dropzone-card p {
    margin: 0;
  }

  .toolbar {
    flex-direction: column;
    align-items: stretch;
  }

  .toolbar-left,
  .toolbar-right {
    width: 100%;
    justify-content: space-between;
  }

  .pagination-container {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }

  .pagination-left,
  .pagination-right {
    justify-content: center;
  }

  .media-list-item {
    flex-direction: column;
    align-items: stretch;
  }

  .list-item-thumbnail {
    width: 100%;
    height: 140px;
  }

  .list-item-actions {
    opacity: 1;
    justify-content: flex-end;
    margin-top: 0.5rem;
  }

  .all-media-title-row {
    flex-direction: column;
    gap: 1rem;
  }

  .all-media-stats {
    width: 100%;
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 0.5rem;
  }

  .media-type-tabs {
    gap: 0.25rem;
  }

  .media-type-tab {
    padding: 0.5rem 0.75rem;
    font-size: 0.75rem;
  }

  .media-type-tab span:not(.tab-count) {
    display: none;
  }

  .all-media-section {
    padding: 1rem;
  }

  .card-hover-actions {
    opacity: 1;
    transform: translateX(0);
  }
}

/* AI Feature Styles */
.card-action-btn.ai-action {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
}

.card-action-btn.ai-action:hover {
  background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
  transform: scale(1.1);
}

.card-action-btn.ai-action.analyzed {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
}

/* AI Insights Button */
.card-action-btn.ai-insights {
  background: linear-gradient(135deg, #8b5cf6 0%, #6366f1 100%);
  color: white;
}

.card-action-btn.ai-insights:hover {
  background: linear-gradient(135deg, #7c3aed 0%, #4f46e5 100%);
  transform: scale(1.1);
}

/* Slide Right Transition for AI Sidebar */
.slide-right-enter-active,
.slide-right-leave-active {
  transition: all 0.3s ease;
}

.slide-right-enter-active > div:last-child,
.slide-right-leave-active > div:last-child {
  transition: transform 0.3s ease;
}

.slide-right-enter-from,
.slide-right-leave-to {
  opacity: 0;
}

.slide-right-enter-from > div:last-child,
.slide-right-leave-to > div:last-child {
  transform: translateX(100%);
}

/* Slide Fade Transition (Documents Page Style) */
.slide-fade-enter-active {
  transition: all 0.3s ease-out;
}

.slide-fade-leave-active {
  transition: all 0.2s ease-in;
}

.slide-fade-enter-from {
  opacity: 0;
  transform: translateY(-10px);
}

.slide-fade-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

/* AI Analysis Modal */
.ai-analysis-modal {
  animation: modalSlideIn 0.3s ease-out;
}

@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: scale(0.95) translateY(20px);
  }
  to {
    opacity: 1;
    transform: scale(1) translateY(0);
  }
}

/* AI Badge Animation */
.ai-badge-pulse {
  animation: aiBadgePulse 2s infinite;
}

@keyframes aiBadgePulse {
  0%, 100% {
    box-shadow: 0 0 0 0 rgba(20, 184, 166, 0.4);
  }
  50% {
    box-shadow: 0 0 0 8px rgba(20, 184, 166, 0);
  }
}

/* AI Search Suggestions */
.ai-search-suggestion {
  transition: all 0.2s ease;
}

.ai-search-suggestion:hover {
  background: linear-gradient(90deg, #f0fdfa 0%, #ffffff 100%);
  transform: translateX(4px);
}

.ai-search-suggestion.ai-generated {
  background: linear-gradient(90deg, #f0fdfa 0%, #ccfbf1 100%);
  border-left: 3px solid #14b8a6;
}

/* AI Loading Shimmer */
.ai-shimmer {
  background: linear-gradient(
    90deg,
    transparent 0%,
    rgba(20, 184, 166, 0.1) 50%,
    transparent 100%
  );
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  0% {
    background-position: -200% 0;
  }
  100% {
    background-position: 200% 0;
  }
}
</style>
