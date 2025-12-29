<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// ============================================
// CORE STATE
// ============================================
const searchQuery = ref('')
const selectedCategory = ref('')
const sortBy = ref('recent')
const viewMode = ref<'grid' | 'list'>('grid')
const showFilters = ref(false)
const showFeaturedOnly = ref(false)
const currentPage = ref(1)
const itemsPerPage = ref(12)
const toasts = ref<Array<{ id: number; type: string; message: string }>>([])

const sortOptions = ref([
  { value: 'recent', label: 'Recent', icon: 'fas fa-clock' },
  { value: 'popular', label: 'Popular', icon: 'fas fa-fire' },
  { value: 'title', label: 'A-Z', icon: 'fas fa-sort-alpha-down' }
])

const currentUser = ref({
  name: 'Ahmed Imam',
  role: 'Product Director',
  initials: 'AI',
  email: 'ahmed.imam@intalio.com'
})

// ============================================
// DATA
// ============================================
const categories = ref([
  { id: 'getting-started', name: 'Getting Started', count: 15, icon: 'fas fa-rocket' },
  { id: 'tutorials', name: 'Tutorials', count: 28, icon: 'fas fa-graduation-cap' },
  { id: 'best-practices', name: 'Best Practices', count: 22, icon: 'fas fa-lightbulb' },
  { id: 'policies', name: 'Policies', count: 18, icon: 'fas fa-file-contract' },
  { id: 'tech', name: 'Technology', count: 34, icon: 'fas fa-microchip' },
  { id: 'hr', name: 'HR & Benefits', count: 12, icon: 'fas fa-heart' },
  { id: 'security', name: 'Security', count: 9, icon: 'fas fa-shield-halved' },
])

const articles = ref([
  {
    id: 1,
    title: 'Getting Started with the Knowledge Hub',
    excerpt: 'Learn how to navigate and make the most of our knowledge management platform.',
    category: 'Getting Started',
    readTime: '5 min',
    icon: 'fas fa-rocket',
    author: { name: 'Sarah Chen', initials: 'SC' },
    date: '2h ago',
    views: 1250,
    likes: 48,
    featured: true,
    categoryId: 'getting-started',
    tags: ['Onboarding', 'Guide', 'Basics']
  },
  {
    id: 2,
    title: 'Best Practices for Remote Team Collaboration',
    excerpt: 'Discover proven strategies to enhance productivity and communication.',
    category: 'Best Practices',
    readTime: '8 min',
    icon: 'fas fa-users',
    author: { name: 'Mike Johnson', initials: 'MJ' },
    date: '5h ago',
    views: 890,
    likes: 32,
    featured: true,
    categoryId: 'best-practices',
    tags: ['Remote', 'Collaboration']
  },
  {
    id: 3,
    title: 'Q4 Security Compliance Guidelines',
    excerpt: 'Updated security protocols and compliance requirements.',
    category: 'Security',
    readTime: '12 min',
    icon: 'fas fa-shield-halved',
    author: { name: 'Emily Davis', initials: 'ED' },
    date: 'Yesterday',
    views: 2100,
    likes: 67,
    featured: true,
    categoryId: 'security',
    tags: ['Security', 'Compliance']
  },
  {
    id: 4,
    title: 'Understanding Our Benefits Package',
    excerpt: 'A guide to employee benefits, insurance, and wellness programs.',
    category: 'HR & Benefits',
    readTime: '10 min',
    icon: 'fas fa-heart',
    author: { name: 'Lisa Wong', initials: 'LW' },
    date: '2 days ago',
    views: 1567,
    likes: 45,
    featured: false,
    categoryId: 'hr',
    tags: ['Benefits', 'Insurance']
  },
  {
    id: 5,
    title: 'API Integration Tutorial: Part 1',
    excerpt: 'Step-by-step guide to integrating with our REST API.',
    category: 'Tutorials',
    readTime: '15 min',
    icon: 'fas fa-code',
    author: { name: 'Alex Thompson', initials: 'AT' },
    date: '3 days ago',
    views: 3200,
    likes: 89,
    featured: false,
    categoryId: 'tutorials',
    tags: ['API', 'Development']
  },
  {
    id: 6,
    title: 'Cloud Infrastructure Best Practices',
    excerpt: 'Optimize your cloud deployments with proven patterns.',
    category: 'Technology',
    readTime: '20 min',
    icon: 'fas fa-cloud',
    author: { name: 'David Kim', initials: 'DK' },
    date: '4 days ago',
    views: 1890,
    likes: 56,
    featured: false,
    categoryId: 'tech',
    tags: ['Cloud', 'AWS']
  },
  {
    id: 7,
    title: 'New Employee Onboarding Checklist',
    excerpt: 'Everything new team members need to know.',
    category: 'Getting Started',
    readTime: '7 min',
    icon: 'fas fa-clipboard-check',
    author: { name: 'Rachel Green', initials: 'RG' },
    date: '5 days ago',
    views: 4500,
    likes: 112,
    featured: false,
    categoryId: 'getting-started',
    tags: ['Onboarding', 'Checklist']
  },
  {
    id: 8,
    title: 'Data Privacy and GDPR Compliance',
    excerpt: 'Understanding data protection regulations.',
    category: 'Policies',
    readTime: '18 min',
    icon: 'fas fa-user-shield',
    author: { name: 'James Wilson', initials: 'JW' },
    date: '1 week ago',
    views: 2340,
    likes: 78,
    featured: false,
    categoryId: 'policies',
    tags: ['GDPR', 'Privacy']
  },
  {
    id: 9,
    title: 'Effective Meeting Management',
    excerpt: 'Tips for running productive meetings.',
    category: 'Best Practices',
    readTime: '6 min',
    icon: 'fas fa-video',
    author: { name: 'Nancy Drew', initials: 'ND' },
    date: '1 week ago',
    views: 1678,
    likes: 41,
    featured: false,
    categoryId: 'best-practices',
    tags: ['Meetings', 'Tips']
  },
  {
    id: 10,
    title: 'Building Microservices with Docker',
    excerpt: 'A hands-on tutorial for containerizing applications.',
    category: 'Tutorials',
    readTime: '25 min',
    icon: 'fas fa-cube',
    author: { name: 'Chris Evans', initials: 'CE' },
    date: '2 weeks ago',
    views: 5600,
    likes: 156,
    featured: false,
    categoryId: 'tutorials',
    tags: ['Docker', 'Microservices']
  },
  {
    id: 11,
    title: 'Annual Leave Policy Updates',
    excerpt: 'Recent changes to vacation and time-off policies.',
    category: 'HR & Benefits',
    readTime: '4 min',
    icon: 'fas fa-umbrella-beach',
    author: { name: 'Lisa Wong', initials: 'LW' },
    date: '2 weeks ago',
    views: 3400,
    likes: 95,
    featured: false,
    categoryId: 'hr',
    tags: ['Leave', 'Policy']
  },
  {
    id: 12,
    title: 'Machine Learning Fundamentals',
    excerpt: 'Introduction to ML concepts and applications.',
    category: 'Technology',
    readTime: '30 min',
    icon: 'fas fa-brain',
    author: { name: 'Dr. Sarah Mitchell', initials: 'SM' },
    date: '2 weeks ago',
    views: 4200,
    likes: 128,
    featured: false,
    categoryId: 'tech',
    tags: ['ML', 'AI']
  },
])

const totalViews = computed(() => {
  const sum = articles.value.reduce((acc, a) => acc + a.views, 0)
  return formatNumber(sum)
})

const contributors = ref(24)

// ============================================
// PERSONALIZATION DATA
// ============================================

// Continue Reading - Articles with reading progress
const continueReading = ref([
  { articleId: 1, progress: 65, lastReadAt: '2 hours ago' },
  { articleId: 5, progress: 30, lastReadAt: '1 day ago' },
  { articleId: 10, progress: 80, lastReadAt: '3 days ago' },
])

// Recently Viewed - Last articles visited
const recentlyViewed = ref([5, 3, 8, 1, 10, 7])

// Bookmarks - Saved article IDs
const bookmarks = ref([1, 3, 5, 7])

// User Feedback - Ratings and helpfulness votes
const userFeedback = ref<Record<number, { helpful?: boolean; rating?: number }>>({
  1: { helpful: true, rating: 5 },
  3: { helpful: true, rating: 4 },
  5: { helpful: false, rating: 3 },
})

// ============================================
// ENGAGEMENT DATA
// ============================================

const articleEngagement = ref<Record<number, { helpfulUp: number; helpfulDown: number; rating: number; ratingCount: number; commentCount: number; lastUpdated: string; freshness: string }>>({
  1: { helpfulUp: 45, helpfulDown: 3, rating: 4.8, ratingCount: 24, commentCount: 12, lastUpdated: '2 days ago', freshness: 'fresh' },
  2: { helpfulUp: 28, helpfulDown: 5, rating: 4.2, ratingCount: 18, commentCount: 8, lastUpdated: '5 days ago', freshness: 'fresh' },
  3: { helpfulUp: 62, helpfulDown: 4, rating: 4.9, ratingCount: 45, commentCount: 23, lastUpdated: '1 day ago', freshness: 'fresh' },
  4: { helpfulUp: 34, helpfulDown: 8, rating: 3.9, ratingCount: 22, commentCount: 6, lastUpdated: '1 week ago', freshness: 'recent' },
  5: { helpfulUp: 89, helpfulDown: 6, rating: 4.7, ratingCount: 56, commentCount: 34, lastUpdated: '3 days ago', freshness: 'fresh' },
  6: { helpfulUp: 41, helpfulDown: 7, rating: 4.1, ratingCount: 28, commentCount: 15, lastUpdated: '4 days ago', freshness: 'fresh' },
  7: { helpfulUp: 112, helpfulDown: 8, rating: 4.9, ratingCount: 78, commentCount: 45, lastUpdated: '5 days ago', freshness: 'fresh' },
  8: { helpfulUp: 56, helpfulDown: 12, rating: 4.0, ratingCount: 34, commentCount: 19, lastUpdated: '2 weeks ago', freshness: 'recent' },
  9: { helpfulUp: 38, helpfulDown: 6, rating: 4.3, ratingCount: 25, commentCount: 11, lastUpdated: '1 week ago', freshness: 'recent' },
  10: { helpfulUp: 156, helpfulDown: 10, rating: 4.9, ratingCount: 89, commentCount: 67, lastUpdated: '2 weeks ago', freshness: 'recent' },
  11: { helpfulUp: 78, helpfulDown: 9, rating: 4.5, ratingCount: 48, commentCount: 28, lastUpdated: '2 weeks ago', freshness: 'recent' },
  12: { helpfulUp: 98, helpfulDown: 14, rating: 4.4, ratingCount: 62, commentCount: 41, lastUpdated: '3 weeks ago', freshness: 'stale' },
})

// ============================================
// DISCOVERY DATA
// ============================================

// Featured Items Array (for carousel)
const featuredItems = ref([
  {
    articleId: 1,
    editorNote: 'Essential reading for all new team members - covers everything you need to hit the ground running.',
    editorPick: true,
    publishedDate: 'Dec 20, 2024',
    tags: ['onboarding', 'tutorial', 'getting-started'],
    relatedArticles: [
      { id: 3, title: 'Advanced Platform Features', readTime: '8 min' },
      { id: 5, title: 'Security Best Practices', readTime: '6 min' },
      { id: 8, title: 'Team Collaboration Guide', readTime: '5 min' }
    ]
  },
  {
    articleId: 3,
    editorNote: 'Deep dive into our most powerful features - unlock your full potential.',
    editorPick: false,
    publishedDate: 'Dec 18, 2024',
    tags: ['advanced', 'features', 'productivity'],
    relatedArticles: [
      { id: 1, title: 'Getting Started Guide', readTime: '10 min' },
      { id: 7, title: 'API Integration Tips', readTime: '12 min' },
      { id: 10, title: 'Workflow Automation', readTime: '7 min' }
    ]
  },
  {
    articleId: 5,
    editorNote: 'Keep your data safe with these essential security guidelines.',
    editorPick: true,
    publishedDate: 'Dec 15, 2024',
    tags: ['security', 'compliance', 'best-practices'],
    relatedArticles: [
      { id: 2, title: 'Data Privacy Policy', readTime: '4 min' },
      { id: 6, title: 'Access Control Setup', readTime: '6 min' },
      { id: 9, title: 'Audit Log Management', readTime: '5 min' }
    ]
  }
])

const activeFeaturedIndex = ref(0)

// Current featured item and article
const currentFeaturedItem = computed(() => featuredItems.value[activeFeaturedIndex.value])
const currentFeaturedArticle = computed(() => {
  const item = currentFeaturedItem.value
  return item ? articles.value.find(a => a.id === item.articleId) : null
})

// Carousel navigation
const prevFeatured = () => {
  if (activeFeaturedIndex.value > 0) {
    activeFeaturedIndex.value--
  }
}

const nextFeatured = () => {
  if (activeFeaturedIndex.value < featuredItems.value.length - 1) {
    activeFeaturedIndex.value++
  }
}

// Auto-play carousel
const carouselInterval = ref<ReturnType<typeof setInterval> | null>(null)
const carouselPaused = ref(false)
const autoPlayDelay = 5000 // 5 seconds

const startCarouselAutoPlay = () => {
  if (carouselInterval.value) return
  carouselInterval.value = setInterval(() => {
    if (!carouselPaused.value) {
      // Loop back to first when at end
      if (activeFeaturedIndex.value >= featuredItems.value.length - 1) {
        activeFeaturedIndex.value = 0
      } else {
        activeFeaturedIndex.value++
      }
    }
  }, autoPlayDelay)
}

const stopCarouselAutoPlay = () => {
  if (carouselInterval.value) {
    clearInterval(carouselInterval.value)
    carouselInterval.value = null
  }
}

const pauseCarousel = () => {
  carouselPaused.value = true
}

const resumeCarousel = () => {
  carouselPaused.value = false
}

// Top Contributors
const topContributors = ref([
  { id: 1, name: 'Sarah Chen', initials: 'SC', articleCount: 24, totalViews: 15000, avgRating: 4.8, badge: 'Top Contributor', gradient: 'from-teal-400 to-teal-600' },
  { id: 2, name: 'David Kim', initials: 'DK', articleCount: 18, totalViews: 12500, avgRating: 4.6, badge: 'Expert', gradient: 'from-blue-400 to-blue-600' },
  { id: 3, name: 'Alex Thompson', initials: 'AT', articleCount: 15, totalViews: 9800, avgRating: 4.7, badge: 'Rising Star', gradient: 'from-purple-400 to-purple-600' },
  { id: 4, name: 'Lisa Wong', initials: 'LW', articleCount: 12, totalViews: 7600, avgRating: 4.5, badge: null, gradient: 'from-pink-400 to-pink-600' },
  { id: 5, name: 'Mike Johnson', initials: 'MJ', articleCount: 10, totalViews: 5400, avgRating: 4.3, badge: null, gradient: 'from-orange-400 to-orange-600' },
])

// ============================================
// QUICK ACTIONS STATE
// ============================================

const showShareModal = ref(false)
const shareArticle = ref<typeof articles.value[0] | null>(null)
const showRequestModal = ref(false)
const categorySubscriptions = ref(['tutorials', 'tech'])

// ============================================
// COMPUTED PROPERTIES FOR DISCOVERY
// ============================================

// Trending This Week - Top articles by views + likes
const trendingThisWeek = computed(() => {
  return [...articles.value]
    .sort((a, b) => (b.views + b.likes * 5) - (a.views + a.likes * 5))
    .slice(0, 6)
})

// Continue Reading articles with full data
const continueReadingArticles = computed(() => {
  return continueReading.value.map(item => {
    const article = articles.value.find(a => a.id === item.articleId)
    return article ? { ...article, progress: item.progress, lastReadAt: item.lastReadAt } : null
  }).filter(Boolean)
})

// Recently Viewed articles with full data
const recentlyViewedArticles = computed(() => {
  return recentlyViewed.value
    .map(id => articles.value.find(a => a.id === id))
    .filter(Boolean)
    .slice(0, 6)
})

// Bookmarked articles with full data
const bookmarkedArticles = computed(() => {
  return bookmarks.value
    .map(id => articles.value.find(a => a.id === id))
    .filter(Boolean)
})

const filteredArticles = ref([...articles.value])

const activeFiltersCount = computed(() => {
  let count = 0
  if (searchQuery.value) count++
  if (selectedCategory.value) count++
  if (showFeaturedOnly.value) count++
  return count
})

const totalPages = computed(() => Math.ceil(filteredArticles.value.length / itemsPerPage.value))
const paginatedArticles = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  return filteredArticles.value.slice(start, start + itemsPerPage.value)
})
const paginationStart = computed(() => (currentPage.value - 1) * itemsPerPage.value + 1)
const paginationEnd = computed(() => Math.min(currentPage.value * itemsPerPage.value, filteredArticles.value.length))

const paginationPages = computed(() => {
  const pages: (number | string)[] = []
  const total = totalPages.value
  const current = currentPage.value
  if (total <= 7) {
    for (let i = 1; i <= total; i++) pages.push(i)
  } else {
    pages.push(1)
    if (current > 3) pages.push('...')
    for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) {
      pages.push(i)
    }
    if (current < total - 2) pages.push('...')
    pages.push(total)
  }
  return pages
})

// ============================================
// FUNCTIONS
// ============================================

function filterArticles() {
  currentPage.value = 1
  let result = [...articles.value]

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(a =>
      a.title.toLowerCase().includes(query) ||
      a.excerpt.toLowerCase().includes(query) ||
      a.category.toLowerCase().includes(query) ||
      a.tags?.some(t => t.toLowerCase().includes(query))
    )
  }

  if (selectedCategory.value) {
    result = result.filter(a => a.categoryId === selectedCategory.value)
  }

  if (showFeaturedOnly.value) {
    result = result.filter(a => a.featured)
  }

  switch (sortBy.value) {
    case 'popular':
      result.sort((a, b) => b.views - a.views)
      break
    case 'title':
      result.sort((a, b) => a.title.localeCompare(b.title))
      break
  }

  filteredArticles.value = result
}

function toggleCategoryFilter(categoryId: string) {
  selectedCategory.value = selectedCategory.value === categoryId ? '' : categoryId
  filterArticles()
}

function clearFilters() {
  searchQuery.value = ''
  selectedCategory.value = ''
  sortBy.value = 'recent'
  showFeaturedOnly.value = false
  filterArticles()
}

function getCategoryName(id: string) {
  const cat = categories.value.find(c => c.id === id)
  return cat ? cat.name : id
}

function formatNumber(num: number): string {
  if (num >= 1000000) return (num / 1000000).toFixed(1) + 'M'
  if (num >= 1000) return (num / 1000).toFixed(1) + 'K'
  return num.toString()
}

function goToArticle(id: number) {
  router.push({ name: 'ArticleView', params: { slug: id.toString() } })
}

function dismissToast(id: number) {
  toasts.value = toasts.value.filter(t => t.id !== id)
}

// ============================================
// QUICK ACTION FUNCTIONS
// ============================================

function toggleBookmark(articleId: number) {
  const index = bookmarks.value.indexOf(articleId)
  if (index > -1) {
    bookmarks.value.splice(index, 1)
  } else {
    bookmarks.value.push(articleId)
  }
}

function isBookmarked(articleId: number) {
  return bookmarks.value.includes(articleId)
}

function openShareModal(article: typeof articles.value[0]) {
  shareArticle.value = article
  showShareModal.value = true
}

function closeShareModal() {
  showShareModal.value = false
  shareArticle.value = null
}

function copyArticleLink(articleId: number) {
  const url = window.location.origin + `/articles/${articleId}`
  navigator.clipboard.writeText(url)
  // Show success toast
  toasts.value.push({ id: Date.now(), type: 'success', message: 'Link copied to clipboard!' })
  setTimeout(() => toasts.value.shift(), 3000)
}

function shareViaEmail(article: typeof articles.value[0]) {
  const subject = encodeURIComponent(article.title)
  const body = encodeURIComponent(`Check out this article: ${article.title}\n\n${window.location.origin}/articles/${article.id}`)
  window.open(`mailto:?subject=${subject}&body=${body}`)
}

function rateArticle(articleId: number, rating: number) {
  if (!userFeedback.value[articleId]) {
    userFeedback.value[articleId] = {}
  }
  userFeedback.value[articleId].rating = rating
}

function markHelpful(articleId: number, isHelpful: boolean) {
  if (!userFeedback.value[articleId]) {
    userFeedback.value[articleId] = {}
  }
  userFeedback.value[articleId].helpful = isHelpful
  // Update engagement counts
  if (articleEngagement.value[articleId]) {
    if (isHelpful) {
      articleEngagement.value[articleId].helpfulUp++
    } else {
      articleEngagement.value[articleId].helpfulDown++
    }
  }
}

function getEngagement(articleId: number) {
  return articleEngagement.value[articleId] || { helpfulUp: 0, helpfulDown: 0, rating: 0, ratingCount: 0, commentCount: 0, lastUpdated: 'N/A', freshness: 'recent' }
}

function getUserFeedback(articleId: number) {
  return userFeedback.value[articleId] || { helpful: null, rating: 0 }
}

function getHelpfulnessPercent(articleId: number) {
  const eng = getEngagement(articleId)
  const total = eng.helpfulUp + eng.helpfulDown
  return total > 0 ? Math.round((eng.helpfulUp / total) * 100) : 0
}

function toggleCategorySubscription(categoryId: string) {
  const index = categorySubscriptions.value.indexOf(categoryId)
  if (index > -1) {
    categorySubscriptions.value.splice(index, 1)
  } else {
    categorySubscriptions.value.push(categoryId)
  }
}

function isCategorySubscribed(categoryId: string) {
  return categorySubscriptions.value.includes(categoryId)
}

function navigateToEditor() {
  router.push({ name: 'ArticleCreate' })
}

// ============================================
// LIFECYCLE
// ============================================

onMounted(() => {
  startCarouselAutoPlay()

  // Add ripple effect to buttons
  document.querySelectorAll('.btn-primary, .btn-secondary, .filter-btn').forEach(btn => {
    btn.addEventListener('click', function(this: HTMLElement, e: MouseEvent) {
      const ripple = document.createElement('span')
      ripple.classList.add('ripple')
      const rect = this.getBoundingClientRect()
      const size = Math.max(rect.width, rect.height)
      ripple.style.width = ripple.style.height = size + 'px'
      ripple.style.left = e.clientX - rect.left - size / 2 + 'px'
      ripple.style.top = e.clientY - rect.top - size / 2 + 'px'
      this.appendChild(ripple)
      setTimeout(() => ripple.remove(), 600)
    })
  })
})

onUnmounted(() => {
  stopCarouselAutoPlay()
})
</script>

<template>
  <div class="page-view">
    <!-- Hero Section -->
    <div class="hero-section">
      <div class="hero-content">
        <div class="hero-text">
          <div class="hero-icon">
            <i class="fas fa-book-open"></i>
          </div>
          <h1 class="hero-title">Articles</h1>
          <p class="hero-subtitle">Discover articles, tutorials, and resources</p>
        </div>
        <button @click="navigateToEditor" class="btn-secondary">
          <i class="fas fa-plus"></i>
          <span>New Article</span>
        </button>
      </div>

      <!-- Stats -->
      <div class="hero-stats">
        <div class="stat-card-hero teal">
          <div class="stat-icon">
            <i class="fas fa-newspaper"></i>
          </div>
          <div class="stat-content">
            <span class="stat-value">{{ articles.length }}+</span>
            <span class="stat-label">Total Articles</span>
          </div>
        </div>
        <div class="stat-card-hero blue">
          <div class="stat-icon">
            <i class="fas fa-layer-group"></i>
          </div>
          <div class="stat-content">
            <span class="stat-value">{{ categories.length }}</span>
            <span class="stat-label">Categories</span>
          </div>
        </div>
        <div class="stat-card-hero orange">
          <div class="stat-icon">
            <i class="fas fa-eye"></i>
          </div>
          <div class="stat-content">
            <span class="stat-value">{{ totalViews }}+</span>
            <span class="stat-label">Total Views</span>
          </div>
        </div>
        <div class="stat-card-hero purple">
          <div class="stat-icon">
            <i class="fas fa-users"></i>
          </div>
          <div class="stat-content">
            <span class="stat-value">{{ contributors }}</span>
            <span class="stat-label">Contributors</span>
          </div>
        </div>
      </div>
    </div>

    <!-- ============================================
         PERSONALIZATION SECTION
         ============================================ -->
    <div class="px-6 pt-6">

      <!-- Recently Viewed Section -->
      <section v-if="recentlyViewedArticles.length > 0" class="mb-10 fade-in-up">
        <div class="section-header-row">
          <h2 class="section-title-sm">
            <i class="fas fa-clock text-teal-500"></i>
            Recently Viewed
          </h2>
          <router-link to="#" class="view-all-link">View All <i class="fas fa-arrow-right"></i></router-link>
        </div>
        <div class="section-scroll scrollbar-elegant">
          <div v-for="article in recentlyViewedArticles" :key="'recent-' + article!.id"
               class="mini-article-card" @click="goToArticle(article!.id)">
            <div class="mini-card-icon bg-gradient-to-br from-teal-50 to-teal-100">
              <i :class="article!.icon || 'fas fa-newspaper'" class="text-teal-600"></i>
            </div>
            <div class="flex-1 min-w-0">
              <h4 class="text-sm font-semibold text-gray-900 truncate">{{ article!.title }}</h4>
              <p class="text-xs text-gray-500">{{ article!.category }} · {{ article!.readTime }}</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Continue Reading & Bookmarks Side by Side -->
      <div class="personalization-row mb-10 fade-in-up" style="animation-delay: 0.1s">
        <!-- Continue Reading Section -->
        <section v-if="continueReadingArticles.length > 0" class="personalization-column">
          <div class="section-header-row">
            <h2 class="section-title-sm">
              <i class="fas fa-history text-purple-500"></i>
              Continue Reading
            </h2>
            <router-link to="#" class="view-all-link">View All <i class="fas fa-arrow-right"></i></router-link>
          </div>
          <div class="section-scroll scrollbar-elegant">
            <div v-for="article in continueReadingArticles" :key="'continue-' + article!.id"
                 class="continue-reading-card group" @click="goToArticle(article!.id)">
              <div class="continue-card-media">
                <div class="continue-card-placeholder">
                  <i :class="article!.icon || 'fas fa-newspaper'" class="text-teal-600 text-xl"></i>
                </div>
                <span class="resume-badge"><i class="fas fa-play mr-1"></i> Resume</span>
                <div class="reading-progress-bar">
                  <div class="reading-progress-fill" :style="{ width: (article as any).progress + '%' }"></div>
                </div>
              </div>
              <div class="continue-card-content">
                <div class="text-xs text-teal-600 font-semibold mb-1">{{ (article as any).progress }}% complete</div>
                <h4 class="text-sm font-semibold text-gray-900 line-clamp-2 group-hover:text-teal-600 transition-colors">{{ article!.title }}</h4>
                <div class="flex items-center gap-3 text-xs text-gray-400 mt-2">
                  <span><i class="fas fa-clock mr-1"></i>{{ article!.readTime }}</span>
                  <span>Last read {{ (article as any).lastReadAt }}</span>
                </div>
              </div>
            </div>
          </div>
        </section>

        <!-- Your Bookmarks Section -->
        <section v-if="bookmarkedArticles.length > 0" class="personalization-column">
          <div class="section-header-row">
            <h2 class="section-title-sm">
              <i class="fas fa-bookmark text-yellow-500"></i>
              Your Bookmarks
            </h2>
            <router-link to="#" class="view-all-link">View All ({{ bookmarkedArticles.length }}) <i class="fas fa-arrow-right"></i></router-link>
          </div>
          <div class="section-scroll scrollbar-elegant">
            <div v-for="article in bookmarkedArticles" :key="'bookmark-' + article!.id"
                 class="bookmark-card group" @click="goToArticle(article!.id)">
              <div class="bookmark-card-visual">
                <div class="bookmark-card-icon">
                  <i :class="article!.icon || 'fas fa-newspaper'" class="text-teal-600 text-xl"></i>
                </div>
                <span class="bookmark-indicator">
                  <i class="fas fa-bookmark"></i>
                </span>
                <button @click.stop="toggleBookmark(article!.id)" class="bookmark-remove-btn">
                  <i class="fas fa-times"></i>
                </button>
              </div>
              <div class="bookmark-card-content">
                <span class="category-pill-sm">{{ article!.category }}</span>
                <h4 class="text-sm font-semibold text-gray-900 line-clamp-2 mt-2 group-hover:text-teal-600 transition-colors">{{ article!.title }}</h4>
                <div class="flex items-center gap-3 text-xs text-gray-400 mt-2">
                  <span><i class="fas fa-clock mr-1"></i>{{ article!.readTime }}</span>
                  <span><i class="fas fa-eye mr-1"></i>{{ formatNumber(article!.views) }}</span>
                </div>
              </div>
            </div>
          </div>
        </section>
      </div>

    </div>

    <!-- ============================================
         DISCOVERY & TRENDS SECTION
         ============================================ -->
    <div class="px-6 pb-2">

      <!-- Featured Carousel + Top Contributors -->
      <section class="mb-10 fade-in-up" style="animation-delay: 0.3s">
        <div class="featured-grid">
          <!-- Featured Carousel -->
          <div class="featured-carousel" @mouseenter="pauseCarousel" @mouseleave="resumeCarousel">
            <!-- Carousel Items -->
            <div class="carousel-container">
              <Transition name="carousel-slide" mode="out-in">
                <div class="featured-spotlight" :key="activeFeaturedIndex" v-if="currentFeaturedArticle">
                  <div class="featured-spotlight-main">
                    <div class="featured-spotlight-content">
                      <div class="flex items-center gap-2 mb-4 flex-wrap">
                        <span class="featured-badge-lg"><i class="fas fa-star mr-1"></i> Featured</span>
                        <span class="editor-pick-badge" v-if="currentFeaturedItem.editorPick">Editor's Pick</span>
                        <span class="featured-category-badge">
                          <i class="fas fa-folder mr-1"></i> {{ currentFeaturedArticle.category }}
                        </span>
                      </div>
                      <h2 class="text-2xl font-bold text-white mb-3">{{ currentFeaturedArticle.title }}</h2>
                      <p class="text-teal-100 mb-4">{{ currentFeaturedArticle.excerpt }}</p>

                      <div v-if="currentFeaturedItem.editorNote" class="editor-note">
                        <i class="fas fa-quote-left mr-2 text-teal-300"></i>
                        {{ currentFeaturedItem.editorNote }}
                      </div>

                      <!-- Tags -->
                      <div class="featured-tags">
                        <span v-for="tag in currentFeaturedItem.tags" :key="tag" class="featured-tag">
                          #{{ tag }}
                        </span>
                      </div>

                      <div class="flex items-center gap-4 mt-4 flex-wrap">
                        <button @click="goToArticle(currentFeaturedArticle.id)" class="btn-featured-primary">
                          <i class="fas fa-book-open mr-2"></i> Read Article
                        </button>
                        <button @click="toggleBookmark(currentFeaturedArticle.id)" class="btn-featured-secondary">
                          <i :class="isBookmarked(currentFeaturedArticle.id) ? 'fas fa-bookmark' : 'far fa-bookmark'" class="mr-2"></i>
                          {{ isBookmarked(currentFeaturedArticle.id) ? 'Saved' : 'Save' }}
                        </button>
                        <button @click="openShareModal(currentFeaturedArticle)" class="btn-featured-secondary">
                          <i class="fas fa-share-alt mr-2"></i> Share
                        </button>
                      </div>
                      <div class="featured-author">
                        <div class="featured-author-avatar">{{ currentFeaturedArticle.author.initials }}</div>
                        <div>
                          <p class="text-white font-medium">{{ currentFeaturedArticle.author.name }}</p>
                          <p class="text-teal-200 text-sm">{{ currentFeaturedArticle.readTime }} read · {{ currentFeaturedItem.publishedDate }}</p>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="related-articles-panel">
                    <h3 class="related-articles-header">
                      <i class="fas fa-link"></i> Related Articles
                    </h3>
                    <div class="related-articles-list">
                      <a v-for="(related, index) in currentFeaturedItem.relatedArticles" :key="related.id"
                         href="#" class="related-article-card" @click.prevent="goToArticle(related.id)">
                        <span class="related-article-number">{{ index + 1 }}</span>
                        <div class="related-article-content">
                          <h4 class="related-article-title">{{ related.title }}</h4>
                          <p class="related-article-meta">
                            <i class="fas fa-clock"></i> {{ related.readTime || '5 min read' }}
                          </p>
                        </div>
                        <i class="fas fa-arrow-right related-article-arrow"></i>
                      </a>
                    </div>
                  </div>
                </div>
              </Transition>
            </div>

            <!-- Carousel Navigation -->
            <div class="carousel-nav">
              <button class="carousel-nav-btn" @click="prevFeatured" :disabled="activeFeaturedIndex === 0">
                <i class="fas fa-chevron-left"></i>
              </button>
              <div class="carousel-dots">
                <button v-for="(item, index) in featuredItems" :key="index"
                        :class="['carousel-dot', { active: activeFeaturedIndex === index }]"
                        @click="activeFeaturedIndex = index">
                </button>
              </div>
              <button class="carousel-nav-btn" @click="nextFeatured" :disabled="activeFeaturedIndex === featuredItems.length - 1">
                <i class="fas fa-chevron-right"></i>
              </button>
            </div>
          </div>

          <!-- Top Contributors -->
          <div class="contributors-card">
            <h3 class="font-semibold text-gray-900 mb-4 flex items-center gap-2">
              <i class="fas fa-trophy text-orange-500"></i> Top Contributors
            </h3>
            <div class="space-y-0">
              <div v-for="contributor in topContributors" :key="contributor.id" class="contributor-item">
                <div :class="['contributor-avatar', 'bg-gradient-to-br', contributor.gradient]">
                  {{ contributor.initials }}
                </div>
                <div class="flex-1 min-w-0">
                  <p class="font-medium text-gray-900 text-sm">{{ contributor.name }}</p>
                  <p class="text-xs text-gray-500">{{ contributor.articleCount }} articles</p>
                </div>
                <div class="text-right">
                  <span v-if="contributor.badge" class="contributor-badge">{{ contributor.badge }}</span>
                  <p class="text-xs text-gray-400 mt-1">
                    <i class="fas fa-star text-amber-400"></i> {{ contributor.avgRating }}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Trending -->
      <section class="mb-10 fade-in-up" style="animation-delay: 0.4s">
        <div class="section-header">
          <h2 class="section-title text-base">
            <i class="fas fa-fire text-red-500 text-sm"></i>
            Trending
          </h2>
          <router-link to="#" class="view-all-link text-xs">
            View All <i class="fas fa-arrow-right"></i>
          </router-link>
        </div>
        <div class="trending-scroll scrollbar-elegant">
          <div v-for="(article, index) in trendingThisWeek" :key="'trending-' + article.id"
               @click="goToArticle(article.id)"
               class="trending-card cursor-pointer group relative">
            <div class="relative aspect-video rounded-xl overflow-hidden shadow-md">
              <div class="absolute inset-0 bg-gradient-to-br from-teal-500 to-teal-700 flex items-center justify-center">
                <i :class="article.icon || 'fas fa-newspaper'" class="text-white text-2xl"></i>
              </div>
              <div class="absolute inset-0 bg-black/0 group-hover:bg-black/20 transition-colors"></div>
              <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
                <div class="w-12 h-12 rounded-full bg-white/95 flex items-center justify-center shadow-lg">
                  <i class="fas fa-arrow-right text-teal-600"></i>
                </div>
              </div>
              <div v-if="index < 3" class="absolute top-2 left-2 flex gap-1">
                <span class="new-badge bg-orange-500">
                  <i class="fas fa-fire"></i> Hot
                </span>
              </div>
              <div class="absolute bottom-2 right-2 px-2 py-0.5 rounded bg-black/70 text-white text-xs font-medium backdrop-blur-sm">
                {{ article.readTime }}
              </div>
            </div>
            <div class="mt-3">
              <h4 class="font-medium text-sm text-gray-900 line-clamp-2 group-hover:text-teal-700 transition-colors">{{ article.title }}</h4>
              <p class="text-xs text-gray-500 mt-1 flex items-center gap-2">
                <span><i class="fas fa-fire text-red-500 mr-1"></i>{{ formatNumber(article.views) }} views</span>
                <span class="text-gray-300">|</span>
                <span>{{ article.category }}</span>
              </p>
            </div>
          </div>
        </div>
      </section>

    </div>

    <!-- ============================================
         ALL ARTICLES SECTION
         ============================================ -->

    <!-- Main Content Area -->
    <div class="main-content">
      <!-- Toolbar -->
      <div class="toolbar">
        <div class="toolbar-left">
          <!-- Search Box -->
          <div class="search-box">
            <i class="fas fa-search"></i>
            <input
              type="text"
              v-model="searchQuery"
              @input="filterArticles"
              placeholder="Search articles..."
              class="search-input">
          </div>

          <!-- Filter Button -->
          <button
            @click="showFilters = !showFilters"
            :class="['filter-btn', { active: showFilters || activeFiltersCount > 0 }]">
            <i class="fas fa-filter"></i>
            <span>Filters</span>
            <span v-if="activeFiltersCount > 0" class="filter-badge">{{ activeFiltersCount }}</span>
          </button>
        </div>

        <div class="toolbar-right">
          <!-- Sort -->
          <select v-model="sortBy" @change="filterArticles" class="sort-select">
            <option v-for="opt in sortOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</option>
          </select>

          <!-- View Toggle -->
          <div class="view-toggle">
            <button @click="viewMode = 'grid'" :class="{ active: viewMode === 'grid' }">
              <i class="fas fa-th-large"></i>
            </button>
            <button @click="viewMode = 'list'" :class="{ active: viewMode === 'list' }">
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
                :key="cat.id"
                @click="toggleCategoryFilter(cat.id)"
                :class="['filter-chip', { active: selectedCategory === cat.id }]">
                {{ cat.name }}
                <span class="count">({{ cat.count }})</span>
              </button>
            </div>
          </div>

          <div class="filter-group">
            <div class="filter-group-label">Type</div>
            <div class="filter-chips">
              <button
                @click="showFeaturedOnly = !showFeaturedOnly; filterArticles()"
                :class="['filter-chip', { active: showFeaturedOnly }]">
                <i class="fas fa-star"></i>
                Featured
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Active Filters Bar -->
      <div v-if="activeFiltersCount > 0 && !showFilters" class="active-filters-bar">
        <span class="active-filters-label">Filters:</span>
        <div class="active-filter-tags">
          <span v-if="searchQuery" class="active-filter-tag">
            "{{ searchQuery }}"
            <button @click="searchQuery = ''; filterArticles()" class="remove-btn">
              <i class="fas fa-times"></i>
            </button>
          </span>
          <span v-if="selectedCategory" class="active-filter-tag">
            {{ getCategoryName(selectedCategory) }}
            <button @click="selectedCategory = ''; filterArticles()" class="remove-btn">
              <i class="fas fa-times"></i>
            </button>
          </span>
          <span v-if="showFeaturedOnly" class="active-filter-tag">
            Featured
            <button @click="showFeaturedOnly = false; filterArticles()" class="remove-btn">
              <i class="fas fa-times"></i>
            </button>
          </span>
        </div>
        <button @click="clearFilters" class="clear-all-btn">Clear All</button>
      </div>

      <!-- Content Area -->
      <div class="content-area">
        <div v-if="filteredArticles.length > 0">
          <!-- Grid View -->
          <div v-if="viewMode === 'grid'" class="articles-grid">
            <article
              v-for="(article, index) in paginatedArticles"
              :key="article.id"
              @click="goToArticle(article.id)"
              :class="['article-card card-animated', { featured: article.featured }, `delay-${(index % 4) + 1}`]">
              <div class="card-media">
                <div class="card-placeholder">
                  <i :class="article.icon || 'fas fa-newspaper'"></i>
                </div>
                <div class="card-badges">
                  <span v-if="article.featured" class="featured-badge">
                    <i class="fas fa-star"></i> Featured
                  </span>
                  <span class="category-badge">{{ article.category }}</span>
                </div>
                <div class="card-overlay">
                  <button class="overlay-btn">
                    <i class="fas fa-arrow-right"></i>
                    Read
                  </button>
                </div>
              </div>
              <div class="card-body">
                <div class="card-meta">
                  <span><i class="fas fa-clock"></i> {{ article.readTime }}</span>
                  <span><i class="fas fa-calendar"></i> {{ article.date }}</span>
                </div>
                <h3 class="card-title">{{ article.title }}</h3>
                <p class="card-excerpt">{{ article.excerpt }}</p>
                <div class="card-tags">
                  <span v-for="tag in article.tags?.slice(0, 3)" :key="tag" class="tag">{{ tag }}</span>
                </div>
                <div class="card-footer">
                  <div class="author-info">
                    <div class="author-avatar">
                      <span class="avatar-text">{{ article.author?.initials || 'U' }}</span>
                    </div>
                    <span class="author-name">{{ article.author?.name }}</span>
                  </div>
                  <div class="card-stats">
                    <span><i class="fas fa-eye"></i> {{ formatNumber(article.views) }}</span>
                    <span><i class="fas fa-heart"></i> {{ article.likes || 0 }}</span>
                  </div>
                </div>
              </div>
            </article>
          </div>

          <!-- List View -->
          <div v-else class="articles-list">
            <article
              v-for="(article, index) in paginatedArticles"
              :key="article.id"
              @click="goToArticle(article.id)"
              :class="['article-list-item list-item-animated', `delay-${(index % 4) + 1}`]">
              <div class="list-item-media">
                <i :class="article.icon || 'fas fa-newspaper'"></i>
              </div>
              <div class="list-item-content">
                <div class="list-item-header">
                  <span class="category-badge">{{ article.category }}</span>
                  <span class="card-meta"><i class="fas fa-clock"></i> {{ article.readTime }}</span>
                  <span v-if="article.featured" class="featured-badge">
                    <i class="fas fa-star"></i> Featured
                  </span>
                </div>
                <h3 class="list-item-title">{{ article.title }}</h3>
                <p class="list-item-excerpt">{{ article.excerpt }}</p>
                <div class="list-item-footer">
                  <div class="author-info">
                    <div class="author-avatar">
                      <span class="avatar-text">{{ article.author?.initials || 'U' }}</span>
                    </div>
                    <span class="author-name">{{ article.author?.name }} &middot; {{ article.date }}</span>
                  </div>
                  <div class="card-stats">
                    <span><i class="fas fa-eye"></i> {{ formatNumber(article.views) }}</span>
                  </div>
                </div>
              </div>
              <div class="list-item-actions">
                <button class="action-btn" title="Bookmark"><i class="fas fa-bookmark"></i></button>
                <button class="action-btn" title="Share"><i class="fas fa-share-alt"></i></button>
              </div>
            </article>
          </div>

          <!-- Pagination -->
          <div class="pagination-container">
            <div class="pagination-info">
              Showing {{ paginationStart }} to {{ paginationEnd }} of {{ filteredArticles.length }} articles
            </div>
            <div class="pagination-controls">
              <button
                class="pagination-btn"
                :disabled="currentPage === 1"
                @click="currentPage--">
                <i class="fas fa-chevron-left"></i>
              </button>
              <template v-for="page in paginationPages" :key="page">
                <span v-if="page === '...'" class="pagination-ellipsis">...</span>
                <button
                  v-else
                  :class="['pagination-btn', { active: currentPage === page }]"
                  @click="currentPage = page as number">
                  {{ page }}
                </button>
              </template>
              <button
                class="pagination-btn"
                :disabled="currentPage === totalPages"
                @click="currentPage++">
                <i class="fas fa-chevron-right"></i>
              </button>
            </div>
            <div class="per-page-control">
              <span class="per-page-label">Per page:</span>
              <select v-model="itemsPerPage" class="per-page-select" @change="currentPage = 1">
                <option :value="8">8</option>
                <option :value="12">12</option>
                <option :value="24">24</option>
                <option :value="48">48</option>
              </select>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-else class="empty-state">
          <div class="empty-state-icon">
            <i class="fas fa-search"></i>
          </div>
          <h3 class="empty-state-title">No articles found</h3>
          <p class="empty-state-text">Try adjusting your search or filters</p>
          <button @click="clearFilters" class="empty-state-btn">
            <i class="fas fa-rotate"></i>
            Clear Filters
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-view {
  padding: 0;
  min-height: 100vh;
  background: linear-gradient(180deg, #f0fdfa 0%, #f8fafc 15%, #ffffff 100%);
}

/* Hero Section */
.hero-section {
  padding: 2rem;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 50%, #e0f2fe 100%);
  border-bottom: 1px solid #e2e8f0;
}

.hero-content {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  max-width: 1600px;
  margin: 0 auto;
  margin-bottom: 1.5rem;
}

.hero-text {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.hero-icon {
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 0.75rem;
  color: white;
  font-size: 1.25rem;
}

.hero-title {
  font-size: 1.75rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0;
}

.hero-subtitle {
  font-size: 0.875rem;
  color: #64748b;
  margin: 0;
}

.hero-stats {
  display: flex;
  gap: 1rem;
  max-width: 1600px;
  margin: 0 auto;
}

.stat-card-hero {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem 1.25rem;
  background: white;
  border-radius: 0.75rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  flex: 1;
}

.stat-card-hero .stat-icon {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0.5rem;
  font-size: 1rem;
}

.stat-card-hero.teal .stat-icon {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  color: #0d9488;
}

.stat-card-hero.blue .stat-icon {
  background: linear-gradient(135deg, #eff6ff 0%, #dbeafe 100%);
  color: #2563eb;
}

.stat-card-hero.orange .stat-icon {
  background: linear-gradient(135deg, #fff7ed 0%, #ffedd5 100%);
  color: #ea580c;
}

.stat-card-hero.purple .stat-icon {
  background: linear-gradient(135deg, #faf5ff 0%, #f3e8ff 100%);
  color: #9333ea;
}

.stat-card-hero .stat-content {
  display: flex;
  flex-direction: column;
}

.stat-card-hero .stat-value {
  font-size: 1.25rem;
  font-weight: 700;
  color: #0f172a;
}

.stat-card-hero .stat-label {
  font-size: 0.75rem;
  color: #64748b;
}

.btn-secondary {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: white;
  border: 1.5px solid #e2e8f0;
  border-radius: 0.75rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: #475569;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.btn-secondary:hover {
  border-color: #14b8a6;
  color: #0d9488;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(20, 184, 166, 0.15);
}

.btn-secondary i {
  transition: transform 0.3s ease;
}

.btn-secondary:hover i {
  transform: rotate(15deg) scale(1.1);
}

/* ============================================
   MAIN CONTENT
   ============================================ */
.main-content {
  padding: 1.5rem 2rem 2rem;
  max-width: 1600px;
  margin: 0 auto;
}

/* Toolbar */
.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  gap: 1rem;
  flex-wrap: wrap;
  animation: fadeInUp 0.4s ease-out;
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
  height: 44px;
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
  box-shadow: 0 0 0 4px rgba(20, 184, 166, 0.1), 0 4px 12px rgba(20, 184, 166, 0.1);
  background: white;
  outline: none;
}

.search-input::placeholder {
  color: #94a3b8;
  transition: color 0.3s ease;
}

.search-input:focus::placeholder {
  color: #cbd5e1;
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
  animation: badgeBounce 0.5s ease;
}

@keyframes badgeBounce {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.2); }
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

.filter-chip .count {
  font-size: 0.625rem;
  opacity: 0.7;
  padding: 0.125rem 0.375rem;
  background: rgba(0, 0, 0, 0.06);
  border-radius: 100px;
}

.filter-chip.active .count {
  background: rgba(255, 255, 255, 0.2);
}

/* Active Filters Bar */
.active-filters-bar {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1.25rem;
  padding: 0.625rem 0.875rem;
  background: #f0fdfa;
  border: 1px solid #ccfbf1;
  border-radius: 0.625rem;
}

.active-filters-label {
  font-size: 0.75rem;
  font-weight: 500;
  color: #0f766e;
}

.active-filter-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
  flex: 1;
}

.active-filter-tag {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  background: white;
  border-radius: 100px;
  font-size: 0.6875rem;
  color: #0f766e;
  border: 1px solid #99f6e4;
}

.active-filter-tag .remove-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 12px;
  height: 12px;
  background: #ccfbf1;
  border: none;
  border-radius: 50%;
  color: #0d9488;
  cursor: pointer;
  font-size: 0.5rem;
}

.active-filter-tag .remove-btn:hover {
  background: #14b8a6;
  color: white;
}

.clear-all-btn {
  margin-left: auto;
  background: transparent;
  border: none;
  font-size: 0.75rem;
  font-weight: 500;
  color: #0d9488;
  cursor: pointer;
}

/* Content Area */
.content-area {
  background: white;
  border-radius: 0.875rem;
  border: 1px solid #f1f5f9;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04);
  overflow: hidden;
  animation: fadeInUp 0.5s ease-out;
}

/* Article Cards Grid */
.articles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1rem;
  padding: 1rem;
}

/* Article Card - Vibrant */
.article-card {
  display: flex;
  flex-direction: column;
  background: white;
  border-radius: 1rem;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  border: 1px solid #f1f5f9;
  position: relative;
}

.article-card::before {
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
}

.article-card:hover::before {
  background: linear-gradient(135deg, #14b8a6 0%, #06b6d4 50%, #10b981 100%);
  opacity: 1;
}

.article-card:hover {
  transform: translateY(-6px);
  box-shadow:
    0 20px 40px -15px rgba(20, 184, 166, 0.25),
    0 0 0 1px rgba(20, 184, 166, 0.1);
}

.article-card.featured {
  border: 2px solid transparent;
  background: linear-gradient(white, white) padding-box,
              linear-gradient(135deg, #5eead4 0%, #2dd4bf 100%) border-box;
}

.article-card.featured::after {
  content: '';
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: linear-gradient(
    45deg,
    transparent 30%,
    rgba(20, 184, 166, 0.03) 50%,
    transparent 70%
  );
  animation: cardShine 3s ease-in-out infinite;
  pointer-events: none;
}

@keyframes cardShine {
  0% { transform: translateX(-100%) rotate(45deg); }
  100% { transform: translateX(100%) rotate(45deg); }
}

.card-media {
  position: relative;
  height: 160px;
  overflow: hidden;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 50%, #e0f2fe 100%);
}

.card-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: transform 0.4s ease;
}

.article-card:hover .card-placeholder {
  transform: scale(1.05);
}

.card-placeholder i {
  font-size: 2.5rem;
  background: linear-gradient(135deg, #5eead4 0%, #2dd4bf 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  filter: drop-shadow(0 4px 6px rgba(20, 184, 166, 0.2));
  transition: transform 0.4s ease;
}

.article-card:hover .card-placeholder i {
  transform: scale(1.1) rotate(5deg);
}

.card-badges {
  position: absolute;
  top: 0.75rem;
  left: 0.75rem;
  display: flex;
  gap: 0.375rem;
  z-index: 2;
}

.featured-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: white;
  font-size: 0.625rem;
  font-weight: 700;
  border-radius: 0.375rem;
  text-transform: uppercase;
  box-shadow: 0 2px 8px rgba(245, 158, 11, 0.4);
  animation: badgePulse 2s ease-in-out infinite;
}

@keyframes badgePulse {
  0%, 100% { box-shadow: 0 2px 8px rgba(245, 158, 11, 0.4); }
  50% { box-shadow: 0 4px 15px rgba(245, 158, 11, 0.6); }
}

.category-badge {
  padding: 0.25rem 0.5rem;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(8px);
  color: #475569;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.375rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

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

.article-card:hover .card-overlay {
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

.article-card:hover .overlay-btn {
  transform: translateY(0);
}

.overlay-btn:hover {
  background: #f0fdfa;
  transform: translateY(-2px) !important;
}

.overlay-btn i {
  transition: transform 0.3s ease;
}

.overlay-btn:hover i {
  transform: translateX(3px);
}

.card-body {
  padding: 0.875rem;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  flex: 1;
}

.card-meta {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  font-size: 0.625rem;
  color: #94a3b8;
}

.card-meta span {
  display: flex;
  align-items: center;
  gap: 0.1875rem;
}

.card-title {
  margin: 0;
  font-size: 0.875rem;
  font-weight: 600;
  line-height: 1.4;
  color: #0f172a;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.article-card:hover .card-title {
  color: #0d9488;
}

.card-excerpt {
  margin: 0;
  font-size: 0.75rem;
  line-height: 1.5;
  color: #64748b;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.card-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.25rem;
}

.tag {
  padding: 0.125rem 0.375rem;
  background: #f0fdfa;
  color: #0d9488;
  font-size: 0.625rem;
  font-weight: 500;
  border-radius: 0.1875rem;
}

.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: auto;
  padding-top: 0.625rem;
  border-top: 1px solid #f1f5f9;
}

.author-info {
  display: flex;
  align-items: center;
  gap: 0.375rem;
}

.author-avatar {
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 50%;
}

.author-avatar .avatar-text {
  color: white;
  font-size: 0.5625rem;
  font-weight: 600;
}

.author-name {
  font-size: 0.6875rem;
  font-weight: 500;
  color: #334155;
}

.card-stats {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.625rem;
  color: #94a3b8;
}

.card-stats span {
  display: flex;
  align-items: center;
  gap: 0.1875rem;
}

/* Articles List View */
.articles-list {
  display: flex;
  flex-direction: column;
  padding: 1rem;
  gap: 0.625rem;
}

.article-list-item {
  display: flex;
  gap: 0.875rem;
  padding: 0.875rem;
  background: white;
  border-radius: 0.625rem;
  border: 1px solid #f1f5f9;
  cursor: pointer;
  transition: all 0.2s ease;
}

.article-list-item:hover {
  border-color: #99f6e4;
  background: #fafffe;
}

.list-item-media {
  width: 80px;
  height: 60px;
  border-radius: 0.375rem;
  overflow: hidden;
  flex-shrink: 0;
  background: linear-gradient(135deg, #f0fdfa 0%, #e0f2fe 100%);
  display: flex;
  align-items: center;
  justify-content: center;
}

.list-item-media i {
  font-size: 1rem;
  color: #5eead4;
}

.list-item-content {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.list-item-header {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  flex-wrap: wrap;
}

.list-item-title {
  margin: 0;
  font-size: 0.875rem;
  font-weight: 600;
  color: #0f172a;
}

.article-list-item:hover .list-item-title {
  color: #0d9488;
}

.list-item-excerpt {
  margin: 0;
  font-size: 0.75rem;
  color: #64748b;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.list-item-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: auto;
}

.list-item-actions {
  display: flex;
  gap: 0.25rem;
}

.action-btn {
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f8fafc;
  border: none;
  border-radius: 0.25rem;
  color: #64748b;
  cursor: pointer;
  transition: all 0.15s ease;
  font-size: 0.75rem;
}

.action-btn:hover {
  background: #f0fdfa;
  color: #0d9488;
}

/* Pagination */
.pagination-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border-top: 1px solid #f1f5f9;
  flex-wrap: wrap;
  gap: 1rem;
}

.pagination-info {
  font-size: 0.75rem;
  color: #64748b;
}

.pagination-controls {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.pagination-btn {
  min-width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 0.375rem;
  font-size: 0.75rem;
  color: #475569;
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

.pagination-ellipsis {
  padding: 0 0.5rem;
  color: #94a3b8;
}

/* Per Page Control */
.per-page-control {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.per-page-label {
  font-size: 0.8125rem;
  color: #64748b;
}

.per-page-select {
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

.per-page-select:focus {
  outline: none;
  border-color: #14b8a6;
}

/* Empty State */
.empty-state {
  text-align: center;
  padding: 2.5rem 1.5rem;
}

.empty-state-icon {
  width: 56px;
  height: 56px;
  margin: 0 auto 0.75rem;
  background: #f1f5f9;
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.empty-state-icon i {
  font-size: 1.5rem;
  color: #94a3b8;
}

.empty-state-title {
  font-size: 1rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0 0 0.375rem;
}

.empty-state-text {
  font-size: 0.875rem;
  color: #64748b;
  margin: 0 0 1rem;
}

.empty-state-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 1rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border: none;
  border-radius: 0.5rem;
  font-size: 0.8125rem;
  font-weight: 600;
  color: white;
  cursor: pointer;
}

/* Animations */
@keyframes fadeInUp {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes fadeInScale {
  from { opacity: 0; transform: scale(0.95); }
  to { opacity: 1; transform: scale(1); }
}

@keyframes slideInRight {
  from { opacity: 0; transform: translateX(20px); }
  to { opacity: 1; transform: translateX(0); }
}

.card-animated {
  animation: fadeInUp 0.5s cubic-bezier(0.4, 0, 0.2, 1) forwards;
}

.list-item-animated {
  animation: fadeInUp 0.5s cubic-bezier(0.4, 0, 0.2, 1) forwards;
}

.delay-1 { animation-delay: 0.05s; opacity: 0; }
.delay-2 { animation-delay: 0.1s; opacity: 0; }
.delay-3 { animation-delay: 0.15s; opacity: 0; }
.delay-4 { animation-delay: 0.2s; opacity: 0; }

.fade-in-up {
  animation: fadeInUp 0.5s ease-out;
}

/* Ripple effect for buttons */
.btn-primary, .btn-secondary, .filter-btn, .filter-chip {
  position: relative;
  overflow: hidden;
}

:deep(.ripple) {
  position: absolute;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.4);
  transform: scale(0);
  animation: ripple 0.6s linear;
  pointer-events: none;
}

@keyframes ripple {
  to { transform: scale(4); opacity: 0; }
}

/* ============================================
   PERSONALIZATION SECTION STYLES
   ============================================ */

.section-header-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.section-title-sm {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9375rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0;
}

.view-all-link {
  font-size: 0.75rem;
  font-weight: 500;
  color: #0d9488;
  text-decoration: none;
  display: flex;
  align-items: center;
  gap: 0.25rem;
  transition: color 0.2s ease;
}

.view-all-link:hover {
  color: #0f766e;
}

.section-scroll {
  display: flex;
  gap: 1rem;
  overflow-x: auto;
  padding-bottom: 0.5rem;
}

.scrollbar-elegant::-webkit-scrollbar {
  height: 6px;
}

.scrollbar-elegant::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 3px;
}

.scrollbar-elegant::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 3px;
}

.scrollbar-elegant::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}

.mini-article-card {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  background: white;
  border-radius: 0.625rem;
  border: 1px solid #f1f5f9;
  min-width: 280px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.mini-article-card:hover {
  border-color: #99f6e4;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.1);
}

.mini-card-icon {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0.5rem;
  flex-shrink: 0;
}

.personalization-row {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 2rem;
}

.personalization-column {
  min-width: 0;
}

.continue-reading-card {
  min-width: 240px;
  background: white;
  border-radius: 0.75rem;
  border: 1px solid #f1f5f9;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.2s ease;
}

.continue-reading-card:hover {
  border-color: #99f6e4;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.1);
}

.continue-card-media {
  position: relative;
  height: 100px;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
}

.continue-card-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.resume-badge {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  padding: 0.25rem 0.5rem;
  background: rgba(0, 0, 0, 0.7);
  color: white;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.25rem;
}

.reading-progress-bar {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: rgba(255, 255, 255, 0.3);
}

.reading-progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  transition: width 0.3s ease;
}

.continue-card-content {
  padding: 0.75rem;
}

.bookmark-card {
  min-width: 200px;
  background: white;
  border-radius: 0.75rem;
  border: 1px solid #f1f5f9;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.2s ease;
}

.bookmark-card:hover {
  border-color: #fde68a;
  box-shadow: 0 4px 12px rgba(251, 191, 36, 0.15);
}

.bookmark-card-visual {
  position: relative;
  height: 80px;
  background: linear-gradient(135deg, #fefce8 0%, #fef3c7 100%);
}

.bookmark-card-icon {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.bookmark-indicator {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  color: #f59e0b;
}

.bookmark-remove-btn {
  position: absolute;
  top: 0.5rem;
  left: 0.5rem;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.9);
  border: none;
  border-radius: 50%;
  color: #94a3b8;
  font-size: 0.625rem;
  cursor: pointer;
  opacity: 0;
  transition: all 0.2s ease;
}

.bookmark-card:hover .bookmark-remove-btn {
  opacity: 1;
}

.bookmark-remove-btn:hover {
  background: #ef4444;
  color: white;
}

.bookmark-card-content {
  padding: 0.75rem;
}

.category-pill-sm {
  display: inline-block;
  padding: 0.125rem 0.375rem;
  background: #f0fdfa;
  color: #0d9488;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.25rem;
}

/* ============================================
   FEATURED CAROUSEL STYLES
   ============================================ */

.featured-grid {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 1.5rem;
}

.featured-carousel {
  position: relative;
}

.carousel-container {
  border-radius: 1rem;
  overflow: hidden;
}

.featured-spotlight {
  display: grid;
  grid-template-columns: 1fr 280px;
  background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
  border-radius: 1rem;
  overflow: hidden;
}

.featured-spotlight-main {
  padding: 2rem;
}

.featured-spotlight-content {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.featured-badge-lg {
  display: inline-flex;
  align-items: center;
  padding: 0.375rem 0.75rem;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: white;
  font-size: 0.75rem;
  font-weight: 700;
  border-radius: 0.375rem;
  text-transform: uppercase;
}

.editor-pick-badge {
  display: inline-flex;
  align-items: center;
  padding: 0.375rem 0.75rem;
  background: rgba(255, 255, 255, 0.2);
  color: white;
  font-size: 0.75rem;
  font-weight: 600;
  border-radius: 0.375rem;
}

.featured-category-badge {
  display: inline-flex;
  align-items: center;
  padding: 0.375rem 0.75rem;
  background: rgba(255, 255, 255, 0.15);
  color: white;
  font-size: 0.75rem;
  font-weight: 500;
  border-radius: 0.375rem;
}

.editor-note {
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  font-size: 0.875rem;
  color: #ccfbf1;
  font-style: italic;
  margin-bottom: 1rem;
}

.featured-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.featured-tag {
  padding: 0.25rem 0.5rem;
  background: rgba(255, 255, 255, 0.15);
  color: #99f6e4;
  font-size: 0.75rem;
  border-radius: 0.25rem;
}

.btn-featured-primary {
  display: inline-flex;
  align-items: center;
  padding: 0.625rem 1.25rem;
  background: white;
  border: none;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: #0d9488;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-featured-primary:hover {
  background: #f0fdfa;
  transform: translateY(-2px);
}

.btn-featured-secondary {
  display: inline-flex;
  align-items: center;
  padding: 0.625rem 1.25rem;
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 0.5rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: white;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-featured-secondary:hover {
  background: rgba(255, 255, 255, 0.25);
}

.featured-author {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-top: auto;
  padding-top: 1.5rem;
}

.featured-author-avatar {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  color: white;
  font-weight: 600;
}

.related-articles-panel {
  background: rgba(0, 0, 0, 0.2);
  padding: 1.5rem;
}

.related-articles-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: white;
  margin-bottom: 1rem;
}

.related-articles-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.related-article-card {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  text-decoration: none;
  transition: all 0.2s ease;
}

.related-article-card:hover {
  background: rgba(255, 255, 255, 0.2);
}

.related-article-number {
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  color: white;
  font-size: 0.75rem;
  font-weight: 600;
}

.related-article-content {
  flex: 1;
  min-width: 0;
}

.related-article-title {
  font-size: 0.8125rem;
  font-weight: 500;
  color: white;
  margin: 0;
  line-height: 1.3;
}

.related-article-meta {
  font-size: 0.6875rem;
  color: #99f6e4;
  margin: 0.25rem 0 0;
}

.related-article-arrow {
  color: rgba(255, 255, 255, 0.5);
  font-size: 0.75rem;
  transition: transform 0.2s ease;
}

.related-article-card:hover .related-article-arrow {
  transform: translateX(3px);
  color: white;
}

.carousel-nav {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  margin-top: 1rem;
}

.carousel-nav-btn {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 50%;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
}

.carousel-nav-btn:hover:not(:disabled) {
  background: #f0fdfa;
  border-color: #14b8a6;
  color: #0d9488;
}

.carousel-nav-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.carousel-dots {
  display: flex;
  gap: 0.5rem;
}

.carousel-dot {
  width: 8px;
  height: 8px;
  background: #cbd5e1;
  border: none;
  border-radius: 50%;
  cursor: pointer;
  transition: all 0.2s ease;
}

.carousel-dot.active {
  background: #14b8a6;
  width: 24px;
  border-radius: 4px;
}

/* Carousel Transition */
.carousel-slide-enter-active,
.carousel-slide-leave-active {
  transition: all 0.4s ease;
}

.carousel-slide-enter-from {
  opacity: 0;
  transform: translateX(20px);
}

.carousel-slide-leave-to {
  opacity: 0;
  transform: translateX(-20px);
}

/* Contributors Card */
.contributors-card {
  background: white;
  border-radius: 1rem;
  border: 1px solid #f1f5f9;
  padding: 1.25rem;
}

.contributor-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 0;
  border-bottom: 1px solid #f1f5f9;
}

.contributor-item:last-child {
  border-bottom: none;
}

.contributor-avatar {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  color: white;
  font-size: 0.75rem;
  font-weight: 600;
}

.contributor-badge {
  display: inline-block;
  padding: 0.125rem 0.375rem;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  color: #0d9488;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.25rem;
}

/* ============================================
   TRENDING SECTION STYLES
   ============================================ */

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.section-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0;
}

.trending-scroll {
  display: flex;
  gap: 1rem;
  overflow-x: auto;
  padding-bottom: 0.5rem;
}

.trending-card {
  min-width: 240px;
  flex-shrink: 0;
}

.new-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  color: white;
  font-size: 0.625rem;
  font-weight: 700;
  border-radius: 0.25rem;
  text-transform: uppercase;
}

/* Responsive */
@media (max-width: 1200px) {
  .featured-grid {
    grid-template-columns: 1fr;
  }

  .contributors-card {
    display: none;
  }
}

@media (max-width: 768px) {
  .main-content {
    padding: 1rem;
  }

  .search-box {
    width: 100%;
  }

  .articles-grid {
    grid-template-columns: 1fr;
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

  .hero-content {
    flex-direction: column;
    gap: 1rem;
  }

  .hero-stats {
    flex-wrap: wrap;
  }

  .hero-stats .stat-card-hero {
    flex: 1 1 calc(50% - 0.5rem);
  }

  .personalization-row {
    grid-template-columns: 1fr;
  }

  .featured-spotlight {
    grid-template-columns: 1fr;
  }

  .related-articles-panel {
    display: none;
  }

  .pagination-container {
    flex-direction: column;
    align-items: center;
  }
}
</style>
