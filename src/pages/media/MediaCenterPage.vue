<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// State
const showUploadModal = ref(false)
const isDragging = ref(false)
const activeTab = ref('all')
const searchQuery = ref('')
const categoryFilter = ref('')
const sortBy = ref('recent')
const currentPage = ref(1)
const itemsPerPage = ref(6)
const showFilters = ref(false)
const trendingScrollRef = ref<HTMLElement | null>(null)
const likedMedia = ref(new Set<number>())
const savedMedia = ref(new Set<number>())
const viewMode = ref('grid')

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

// Media Stats
const mediaStats = ref({
  totalVideos: 156,
  galleries: 42,
  audio: 38,
  images: 245
})

// Watch History (Continue Watching)
const watchHistory = ref([
  {
    id: 1,
    title: 'CEO Vision 2025: Building the Future',
    progress: 65,
    duration: '32:15',
    timeRemaining: '11 min left',
    thumbnail: 'https://images.unsplash.com/photo-1560439514-4e9645039924?w=400&h=225&fit=crop',
    category: 'Leadership',
    type: 'video',
    author: 'John Smith',
    lastWatched: '2 hours ago',
    views: '2.4K'
  },
  {
    id: 2,
    title: 'Q4 Product Roadmap Overview',
    progress: 30,
    duration: '18:42',
    timeRemaining: '13 min left',
    thumbnail: 'https://images.unsplash.com/photo-1551434678-e076c223a692?w=400&h=225&fit=crop',
    category: 'Product Updates',
    type: 'video',
    author: 'Sarah Chen',
    lastWatched: 'Yesterday',
    views: '1.2K'
  },
  {
    id: 3,
    title: 'Security Best Practices 2024',
    progress: 80,
    duration: '15:20',
    timeRemaining: '3 min left',
    thumbnail: 'https://images.unsplash.com/photo-1563986768609-322da13575f3?w=400&h=225&fit=crop',
    category: 'Technical',
    type: 'video',
    author: 'David Kim',
    lastWatched: '3 days ago',
    views: '890'
  },
  {
    id: 4,
    title: 'Leadership Workshop: Building High-Performing Teams',
    progress: 45,
    duration: '45:00',
    timeRemaining: '25 min left',
    thumbnail: 'https://images.unsplash.com/photo-1552664730-d307ca884978?w=400&h=225&fit=crop',
    category: 'Training',
    type: 'video',
    author: 'Emily Davis',
    lastWatched: '1 week ago',
    views: '1.8K'
  },
  {
    id: 5,
    title: 'Tech Talk: Microservices Architecture Deep Dive',
    progress: 12,
    duration: '38:15',
    timeRemaining: '34 min left',
    thumbnail: 'https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=400&h=225&fit=crop',
    category: 'Technical',
    type: 'video',
    author: 'Alex Thompson',
    lastWatched: '2 weeks ago',
    views: '956'
  },
])

// Category Cards
const categoryCards = ref([
  { id: 'training', name: 'Training', icon: 'fas fa-chalkboard-teacher', count: 24, gradient: 'cat-training' },
  { id: 'product', name: 'Product Updates', icon: 'fas fa-box', count: 18, gradient: 'cat-product' },
  { id: 'leadership', name: 'Leadership', icon: 'fas fa-users-cog', count: 12, gradient: 'cat-leadership' },
  { id: 'technical', name: 'Technical', icon: 'fas fa-code', count: 32, gradient: 'cat-technical' },
  { id: 'culture', name: 'Culture', icon: 'fas fa-heart', count: 8, gradient: 'cat-culture' },
  { id: 'news', name: 'Company News', icon: 'fas fa-newspaper', count: 15, gradient: 'cat-news' },
])

// Featured Video
// Featured Videos Carousel
const featuredVideos = ref([
  {
    id: 1,
    title: 'AFC Asian Cup 2027: Tournament Preview',
    description: 'Get an exclusive behind-the-scenes look at the preparations for the biggest football event in Asia.',
    duration: '32:15',
    views: '24.5K',
    thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=1200&h=675&fit=crop',
    category: 'Tournament',
    author: 'AFC Media Team',
    date: '2 days ago',
    likes: 1250,
    comments: 89
  },
  {
    id: 2,
    title: 'Opening Ceremony Preparations Unveiled',
    description: 'A sneak peek at the spectacular opening ceremony planned for the tournament.',
    duration: '18:42',
    views: '18.2K',
    thumbnail: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=1200&h=675&fit=crop',
    category: 'Events',
    author: 'Sports Network',
    date: '3 days ago',
    likes: 890,
    comments: 56
  },
  {
    id: 3,
    title: 'Top 10 Goals from Qualifying Rounds',
    description: 'Relive the most spectacular goals scored during the qualification matches.',
    duration: '12:30',
    views: '45.8K',
    thumbnail: 'https://images.unsplash.com/photo-1551958219-acbc608c6377?w=1200&h=675&fit=crop',
    category: 'Highlights',
    author: 'AFC Media Team',
    date: '1 week ago',
    likes: 2340,
    comments: 178
  },
  {
    id: 4,
    title: 'Meet the Teams: Group Stage Draw Analysis',
    description: 'Expert analysis of the group stage draw and predictions for the tournament.',
    duration: '28:15',
    views: '12.1K',
    thumbnail: 'https://images.unsplash.com/photo-1489944440615-453fc2b6a9a9?w=1200&h=675&fit=crop',
    category: 'Analysis',
    author: 'Football Experts',
    date: '5 days ago',
    likes: 567,
    comments: 42
  }
])

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

// Up Next Videos
const upNextVideos = ref([
  {
    id: 2,
    title: 'Stadium Tour: King Fahd International',
    duration: '18:42',
    views: '12.3K',
    thumbnail: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=400&h=225&fit=crop',
    category: 'Venues',
    author: 'Sports Network'
  },
  {
    id: 3,
    title: 'Top 10 Goals from Qualifiers',
    duration: '24:30',
    views: '35.8K',
    thumbnail: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=400&h=225&fit=crop',
    category: 'Highlights',
    author: 'AFC Highlights'
  },
  {
    id: 4,
    title: 'Team Japan: Road to 2027',
    duration: '15:20',
    views: '8.9K',
    thumbnail: 'https://images.unsplash.com/photo-1489944440615-453fc2b6a9a9?w=400&h=225&fit=crop',
    category: 'Teams',
    author: 'Sports Documentary'
  },
  {
    id: 5,
    title: 'Opening Ceremony Preparations',
    duration: '12:45',
    views: '6.2K',
    thumbnail: 'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=400&h=225&fit=crop',
    category: 'Events',
    author: 'Event Coverage'
  },
])

// Media Items
const mediaItems = ref([
  { id: 5, title: 'Saudi Arabia vs Japan: Opening Match Preview', type: 'video', category: 'Highlights', duration: '12:45', views: '45.2K', date: '2 days ago', thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400&h=225&fit=crop', isNew: true, hasTranscript: true, author: 'AFC Media' },
  { id: 6, title: 'King Fahd Stadium: Behind the Scenes Tour', type: 'video', category: 'Behind the Scenes', duration: '18:30', views: '32.1K', date: '3 days ago', thumbnail: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=400&h=225&fit=crop', isNew: true, hasTranscript: true, author: 'Venue Team' },
  { id: 13, title: 'AFC Asian Cup 2027 Official Draw Ceremony', type: 'gallery', category: 'Highlights', photoCount: 48, views: '28.5K', date: '1 day ago', thumbnail: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=400&h=225&fit=crop', isNew: true, author: 'AFC Media' },
  { id: 18, title: 'AFC Asian Cup Official Podcast: Episode 1', type: 'audio', category: 'Interviews', duration: '45:30', views: '12.3K', date: '2 days ago', thumbnail: 'https://images.unsplash.com/photo-1478737270239-2f02b77fc618?w=400&h=225&fit=crop', isNew: true, hasTranscript: true, author: 'AFC Media' },
  { id: 19, title: 'King Fahd Stadium Aerial View', type: 'image', category: 'Venues', views: '18.5K', date: '1 day ago', thumbnail: 'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=400&h=225&fit=crop', isNew: true, author: 'AFC Media' },
  { id: 7, title: 'Coach Interview: Saudi Arabia Tactics Analysis', type: 'video', category: 'Interviews', duration: '25:15', views: '18.7K', date: '1 week ago', thumbnail: 'https://images.unsplash.com/photo-1551958219-acbc608c6377?w=400&h=225&fit=crop', isNew: false, hasTranscript: true, author: 'Sports Desk' },
  { id: 20, title: 'Match Day Commentary: Live Analysis', type: 'audio', category: 'Matches', duration: '90:00', views: '8.7K', date: '4 days ago', thumbnail: 'https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?w=400&h=225&fit=crop', isNew: true, author: 'AFC Radio' },
  { id: 21, title: 'Saudi Arabia Team Photo 2027', type: 'image', category: 'Teams', views: '35.2K', date: '3 days ago', thumbnail: 'https://images.unsplash.com/photo-1517466787929-bc90951d0974?w=400&h=225&fit=crop', isNew: true, author: 'AFC Media' },
  { id: 14, title: 'Fan Zone Setup: Riyadh Boulevard', type: 'gallery', category: 'Fans', photoCount: 32, views: '15.3K', date: '3 days ago', thumbnail: 'https://images.unsplash.com/photo-1560272564-c83b66b1ad12?w=400&h=225&fit=crop', isNew: true, author: 'Events Team' },
  { id: 8, title: 'Top 10 Goals: AFC Asian Cup History', type: 'video', category: 'Highlights', duration: '15:20', views: '89.4K', date: '1 week ago', thumbnail: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=400&h=225&fit=crop', isNew: false, hasTranscript: true, author: 'AFC Media' },
  { id: 22, title: 'Trophy Unveiling Ceremony', type: 'image', category: 'Highlights', views: '42.1K', date: '1 week ago', thumbnail: 'https://images.unsplash.com/photo-1579952363873-27f3bade9f55?w=400&h=225&fit=crop', isNew: false, author: 'AFC Media' },
  { id: 9, title: 'Team Japan: Road to Saudi Arabia 2027', type: 'video', category: 'Teams', duration: '22:10', views: '41.2K', date: '2 weeks ago', thumbnail: 'https://images.unsplash.com/photo-1606925797300-0b35e9d1794e?w=400&h=225&fit=crop', isNew: false, hasTranscript: true, author: 'AFC Media' },
  { id: 23, title: 'Pre-Match Analysis Podcast: Group A', type: 'audio', category: 'Matches', duration: '32:15', views: '6.4K', date: '1 week ago', thumbnail: 'https://images.unsplash.com/photo-1508098682722-e99c43a406b2?w=400&h=225&fit=crop', isNew: false, author: 'AFC Analysts' },
  { id: 15, title: 'Volunteer Training Program Launch', type: 'gallery', category: 'Behind the Scenes', photoCount: 65, views: '12.8K', date: '1 week ago', thumbnail: 'https://images.unsplash.com/photo-1517466787929-bc90951d0974?w=400&h=225&fit=crop', isNew: false, author: 'LOC Team' },
  { id: 10, title: 'Salem Al-Dawsari: Exclusive Interview', type: 'video', category: 'Interviews', duration: '18:30', views: '67.3K', date: '2 weeks ago', thumbnail: 'https://images.unsplash.com/photo-1579952363873-27f3bade9f55?w=400&h=225&fit=crop', isNew: false, hasTranscript: false, author: 'Sports Desk' },
  { id: 24, title: 'Fans Celebrating at Boulevard', type: 'image', category: 'Fans', views: '28.9K', date: '2 weeks ago', thumbnail: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=400&h=225&fit=crop', isNew: false, author: 'Events Team' },
  { id: 16, title: 'Stadium Infrastructure Progress Photos', type: 'gallery', category: 'Venues', photoCount: 28, views: '9.5K', date: '2 weeks ago', thumbnail: 'https://images.unsplash.com/photo-1489944440615-453fc2b6a9a9?w=400&h=225&fit=crop', isNew: false, author: 'Venue Operations' },
  { id: 11, title: 'Group Stage Draw Analysis', type: 'video', category: 'Highlights', duration: '35:00', views: '52.1K', date: '3 weeks ago', thumbnail: 'https://images.unsplash.com/photo-1518091043644-c1d4457512c6?w=400&h=225&fit=crop', isNew: false, hasTranscript: true, author: 'AFC Analysts' },
  { id: 12, title: 'AFC Podcast: Tournament Predictions', type: 'audio', category: 'Interviews', duration: '48:20', views: '8.9K', date: '3 weeks ago', thumbnail: 'https://images.unsplash.com/photo-1508098682722-e99c43a406b2?w=400&h=225&fit=crop', isNew: false, hasTranscript: false, author: 'AFC Media' },
  { id: 17, title: 'Media Accreditation Workshop', type: 'gallery', category: 'Behind the Scenes', photoCount: 42, views: '6.2K', date: '3 weeks ago', thumbnail: 'https://images.unsplash.com/photo-1504450758481-7338eba7524a?w=400&h=225&fit=crop', isNew: false, author: 'Media Team' },
])

// Computed: Active Filters Count
const activeFiltersCount = computed(() => {
  let count = 0
  if (categoryFilter.value) count++
  if (activeTab.value !== 'all') count++
  if (searchQuery.value) count++
  return count
})

// Computed: Filtered Media
const filteredMedia = computed(() => {
  let result = [...mediaItems.value]
  if (activeTab.value !== 'all') {
    result = result.filter(m => m.type === activeTab.value)
  }
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    result = result.filter(m => m.title.toLowerCase().includes(q))
  }
  if (categoryFilter.value) {
    result = result.filter(m => m.category === categoryFilter.value)
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
}

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
                  <button class="trending-action-btn" @click.stop="shareMedia(media)" title="Share">
                    <i class="fas fa-share-alt"></i>
                  </button>
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

      <!-- All Media Section -->
      <section class="all-media-section mb-10 animate-in delay-3">
        <!-- Section Header with Stats -->
        <div class="all-media-header">
          <div class="all-media-title-row">
            <h2 class="all-media-title">
              <div class="all-media-icon">
                <i class="fas fa-photo-film"></i>
              </div>
              <div>
                <span class="all-media-heading">Media Library</span>
                <span class="all-media-subtitle">Browse and discover all content</span>
              </div>
            </h2>
            <div class="all-media-stats">
              <div class="media-stat-item">
                <i class="fas fa-video"></i>
                <span class="stat-count">{{ mediaStats.totalVideos }}</span>
                <span class="stat-label">Videos</span>
              </div>
              <div class="media-stat-item">
                <i class="fas fa-headphones"></i>
                <span class="stat-count">{{ mediaStats.audio }}</span>
                <span class="stat-label">Audio</span>
              </div>
              <div class="media-stat-item">
                <i class="fas fa-image"></i>
                <span class="stat-count">{{ mediaStats.images }}</span>
                <span class="stat-label">Images</span>
              </div>
              <div class="media-stat-item">
                <i class="fas fa-images"></i>
                <span class="stat-count">{{ mediaStats.galleries }}</span>
                <span class="stat-label">Galleries</span>
              </div>
            </div>
          </div>

          <!-- Media Type Tabs -->
          <div class="media-type-tabs">
            <button
              v-for="tab in mediaTabs"
              :key="tab.id"
              @click="activeTab = tab.id"
              :class="['media-type-tab', { active: activeTab === tab.id }]">
              <i :class="tab.icon"></i>
              <span>{{ tab.label }}</span>
              <span v-if="tab.id !== 'all'" class="tab-count">{{ getMediaTypeCount(tab.id) }}</span>
            </button>
          </div>
        </div>

        <!-- Toolbar -->
        <div class="toolbar">
          <div class="toolbar-left">
            <!-- Search Box -->
            <div class="search-box">
              <i class="fas fa-search"></i>
              <input type="text" v-model="searchQuery" placeholder="Search media..." class="search-input">
              <button v-if="searchQuery" @click="searchQuery = ''" class="search-clear">
                <i class="fas fa-times"></i>
              </button>
            </div>

            <!-- Filter Button -->
            <button
              @click="showFilters = !showFilters"
              :class="['filter-btn', { active: showFilters || activeFiltersCount > 0 }]">
              <i class="fas fa-sliders-h"></i>
              <span>Filters</span>
              <span v-if="activeFiltersCount > 0" class="filter-badge">{{ activeFiltersCount }}</span>
            </button>
          </div>

          <div class="toolbar-right">
            <!-- Results Count -->
            <span class="results-count">{{ filteredMedia.length }} results</span>

            <!-- Sort -->
            <div class="sort-wrapper">
              <i class="fas fa-sort-amount-down"></i>
              <select v-model="sortBy" class="sort-select">
                <option v-for="option in sortOptions" :key="option.value" :value="option.value">
                  {{ option.label }}
                </option>
              </select>
            </div>

            <!-- View Toggle -->
            <div class="view-toggle">
              <button @click="viewMode = 'grid'" :class="{ active: viewMode === 'grid' }" title="Grid view">
                <i class="fas fa-th-large"></i>
              </button>
              <button @click="viewMode = 'list'" :class="{ active: viewMode === 'list' }" title="List view">
                <i class="fas fa-list"></i>
              </button>
            </div>
          </div>
        </div>

        <!-- Filters Panel -->
        <div v-if="showFilters" class="filters-panel">
          <div class="filters-header">
            <div class="filters-title">
              <i class="fas fa-sliders"></i>
              <span>Filters</span>
            </div>
            <button v-if="activeFiltersCount > 0" @click="clearFilters" class="clear-filters-btn">
              Clear All
            </button>
          </div>

          <div class="filter-groups">
            <div class="filter-group">
              <div class="filter-group-label">Category</div>
              <div class="filter-chips">
                <button
                  v-for="cat in categories"
                  :key="cat"
                  @click="toggleCategoryFilter(cat)"
                  :class="['filter-chip', { active: categoryFilter === cat }]">
                  {{ cat }}
                </button>
              </div>
            </div>

            <div class="filter-group">
              <div class="filter-group-label">Type</div>
              <div class="filter-chips">
                <button
                  v-for="tab in mediaTabs"
                  :key="tab.id"
                  @click="activeTab = tab.id"
                  :class="['filter-chip', { active: activeTab === tab.id }]">
                  <i :class="tab.icon"></i>
                  {{ tab.label }}
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Content Area with Media Grid/List -->
        <div class="content-area">
          <div class="content-area-inner">
            <!-- Grid View -->
            <div v-if="viewMode === 'grid'" class="media-grid">
              <div v-for="media in paginatedMedia" :key="media.id"
                   @click="goToMedia(media)"
                   class="media-card-enhanced rounded-2xl overflow-hidden cursor-pointer group bg-white">
                <!-- Card Media/Thumbnail -->
                <div class="card-thumbnail relative aspect-video">
                  <!-- Actual Thumbnail Image -->
                  <img :src="media.thumbnail" :alt="media.title" class="absolute inset-0 w-full h-full object-cover" />
                  <div class="absolute inset-0 bg-gradient-to-t from-black/50 via-black/10 to-transparent"></div>

                  <!-- Card Badges -->
                  <div class="absolute top-3 left-3 flex gap-1.5 z-20">
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
                  </div>

                  <!-- Watch Progress (if any) -->
                  <div v-if="media.progress" class="watch-progress z-20">
                    <div class="watch-progress-bar" :style="{ width: media.progress + '%' }"></div>
                  </div>
                </div>

                <!-- Card Body -->
                <div class="card-body-enhanced">
                  <div class="card-meta-row">
                    <span :class="['media-type-badge', media.type]">
                      <i :class="media.type === 'video' ? 'fas fa-video' : media.type === 'audio' ? 'fas fa-headphones' : media.type === 'image' ? 'fas fa-image' : 'fas fa-images'"></i>
                      {{ media.type === 'video' ? 'Video' : media.type === 'audio' ? 'Audio' : media.type === 'image' ? 'Image' : 'Gallery' }}
                    </span>
                    <span :class="['media-category-tag', media.category.toLowerCase().split(' ')[0]]">{{ media.category }}</span>
                  </div>

                  <h3 class="card-title-enhanced">{{ media.title }}</h3>

                  <div class="card-footer-enhanced">
                    <div class="author-info">
                      <div class="author-avatar">
                        <span class="avatar-text">{{ getInitials(media.author) }}</span>
                      </div>
                      <span class="author-name">{{ media.author || 'AFC Media' }}</span>
                    </div>
                    <div class="card-stats">
                      <span><i class="fas fa-eye"></i> {{ media.views }}</span>
                      <span><i class="fas fa-clock"></i> {{ media.date }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- List View -->
            <div v-else class="media-list">
              <div v-for="media in paginatedMedia" :key="media.id"
                   @click="goToMedia(media)"
                   class="media-list-item cursor-pointer group">
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

        <!-- Pagination (Documents Style) -->
        <div v-if="filteredMedia.length > 0" class="mt-8">
          <div class="pagination-container">
            <div class="pagination-left">
              <div class="pagination-info">
                Showing {{ (currentPage - 1) * itemsPerPage + 1 }} - {{ Math.min(currentPage * itemsPerPage, filteredMedia.length) }} of {{ filteredMedia.length }} media
              </div>
            </div>
            <div class="pagination-center">
              <button
                @click="handlePageChange(currentPage - 1)"
                :disabled="currentPage === 1"
                class="pagination-btn">
                <i class="fas fa-chevron-left"></i>
              </button>
              <template v-for="page in totalPages" :key="page">
                <button
                  @click="handlePageChange(page)"
                  :class="['pagination-btn', { active: currentPage === page }]">
                  {{ page }}
                </button>
              </template>
              <button
                @click="handlePageChange(currentPage + 1)"
                :disabled="currentPage === totalPages"
                class="pagination-btn">
                <i class="fas fa-chevron-right"></i>
              </button>
            </div>
            <div class="pagination-right">
              <span class="per-page-label">Items per page:</span>
              <div class="per-page-select-wrapper">
                <select v-model="itemsPerPage" @change="handlePerPageChange(itemsPerPage)" class="per-page-select">
                  <option :value="6">6</option>
                  <option :value="9">9</option>
                  <option :value="12">12</option>
                  <option :value="24">24</option>
                </select>
              </div>
            </div>
          </div>
        </div>
      </section>

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
  gap: 0.5rem;
  font-size: 1rem;
  font-weight: 600;
  color: #1e293b;
}

.section-title-sm i {
  width: 1.75rem;
  height: 1.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-radius: 0.5rem;
  font-size: 0.75rem;
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
  width: 2.75rem;
  height: 2.75rem;
  border-radius: 0.75rem;
  background: linear-gradient(135deg, #ef4444 0%, #f97316 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(239, 68, 68, 0.3);
}

.trending-icon-wrapper i {
  color: white;
  font-size: 1rem;
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
  top: 0.5rem;
  left: 0.5rem;
  display: flex;
  gap: 0.375rem;
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
  bottom: 0.5rem;
  left: 0.5rem;
  display: flex;
  gap: 0.375rem;
  opacity: 0;
  transform: translateY(8px);
  transition: all 0.3s ease;
}

.trending-card-inner:hover .trending-actions {
  opacity: 1;
  transform: translateY(0);
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
  background: #f8fafc;
  border-radius: 1rem;
  border: 1px solid #e2e8f0;
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
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
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
  font-size: 0.75rem;
  color: #94a3b8;
}

.list-stats span {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.list-stats i {
  font-size: 0.625rem;
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
    grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
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
</style>
