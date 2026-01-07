<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// ============================================
// CORE STATE
// ============================================
const searchQuery = ref('')
const selectedCategories = ref<string[]>([])
const selectedTypes = ref<string[]>([])
const selectedTags = ref<string[]>([])
const sortBy = ref('recent')
const sortOrder = ref<'asc' | 'desc'>('desc')
const viewMode = ref<'grid' | 'list'>('grid')
const showFilters = ref(false)
const showCategoryFilter = ref(false)
const showTypeFilter = ref(false)
const showTagFilter = ref(false)
const showSortDropdown = ref(false)
const currentPage = ref(1)
const itemsPerPage = ref(10)
const itemsPerPageOptions = [5, 10, 20, 50, 100]
const toasts = ref<Array<{ id: number; type: string; message: string }>>([])

const sortOptions = ref([
  { value: 'recent', label: 'Recent', icon: 'fas fa-clock' },
  { value: 'popular', label: 'Popular', icon: 'fas fa-fire' },
  { value: 'title', label: 'A-Z', icon: 'fas fa-sort-alpha-down' }
])

const currentSortOption = computed(() => {
  return sortOptions.value.find(opt => opt.value === sortBy.value) ?? sortOptions.value[0]!
})

function selectSortOption(value: string) {
  sortBy.value = value
  showSortDropdown.value = false
  filterArticles()
}

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
  { id: 'featured', name: 'Featured', count: 8, icon: 'fas fa-star', color: '#f59e0b', isSpecial: true },
  { id: 'getting-started', name: 'Getting Started', count: 15, icon: 'fas fa-rocket', color: '#14b8a6' },
  { id: 'tutorials', name: 'Tutorials', count: 28, icon: 'fas fa-graduation-cap', color: '#8b5cf6' },
  { id: 'best-practices', name: 'Best Practices', count: 22, icon: 'fas fa-lightbulb', color: '#f59e0b' },
  { id: 'policies', name: 'Policies', count: 18, icon: 'fas fa-file-contract', color: '#6366f1' },
  { id: 'tech', name: 'Technology', count: 34, icon: 'fas fa-microchip', color: '#3b82f6' },
  { id: 'hr', name: 'HR & Benefits', count: 12, icon: 'fas fa-heart', color: '#ec4899' },
  { id: 'security', name: 'Security', count: 9, icon: 'fas fa-shield-halved', color: '#ef4444' },
])

const articleTypes = ref([
  { id: 'guide', name: 'Guide', icon: 'fas fa-book', color: '#14b8a6' },
  { id: 'tutorial', name: 'Tutorial', icon: 'fas fa-graduation-cap', color: '#8b5cf6' },
  { id: 'news', name: 'News', icon: 'fas fa-newspaper', color: '#3b82f6' },
  { id: 'announcement', name: 'Announcement', icon: 'fas fa-bullhorn', color: '#f59e0b' },
  { id: 'policy', name: 'Policy', icon: 'fas fa-file-contract', color: '#6366f1' },
  { id: 'how-to', name: 'How-To', icon: 'fas fa-list-check', color: '#10b981' },
  { id: 'faq', name: 'FAQ', icon: 'fas fa-circle-question', color: '#f97316' },
])

// All available tags (computed from articles)
const allTags = computed(() => {
  const tags = new Set<string>()
  articles.value.forEach(article => {
    article.tags?.forEach(tag => tags.add(tag))
  })
  return Array.from(tags).sort()
})

const articles = ref([
  {
    id: 1,
    title: 'Getting Started with the Knowledge Hub',
    excerpt: 'Learn how to navigate and make the most of our knowledge management platform.',
    category: 'Getting Started',
    readTime: '5 min',
    icon: 'fas fa-rocket',
    image: 'https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=400&h=300&fit=crop',
    author: { name: 'Sarah Chen', initials: 'SC' },
    date: '2h ago',
    views: 1250,
    likes: 48,
    featured: true,
    categoryId: 'getting-started',
    type: 'guide',
    tags: ['Onboarding', 'Guide', 'Basics']
  },
  {
    id: 2,
    title: 'Best Practices for Remote Team Collaboration',
    excerpt: 'Discover proven strategies to enhance productivity and communication.',
    category: 'Best Practices',
    readTime: '8 min',
    icon: 'fas fa-users',
    image: 'https://images.unsplash.com/photo-1522071820081-009f0129c71c?w=400&h=300&fit=crop',
    author: { name: 'Mike Johnson', initials: 'MJ' },
    date: '5h ago',
    views: 890,
    likes: 32,
    featured: true,
    categoryId: 'best-practices',
    type: 'guide',
    tags: ['Remote', 'Collaboration']
  },
  {
    id: 3,
    title: 'Q4 Security Compliance Guidelines',
    excerpt: 'Updated security protocols and compliance requirements.',
    category: 'Security',
    readTime: '12 min',
    icon: 'fas fa-shield-halved',
    image: 'https://images.unsplash.com/photo-1563986768609-322da13575f3?w=400&h=300&fit=crop',
    author: { name: 'Emily Davis', initials: 'ED' },
    date: 'Yesterday',
    views: 2100,
    likes: 67,
    featured: true,
    categoryId: 'security',
    type: 'policy',
    tags: ['Security', 'Compliance']
  },
  {
    id: 4,
    title: 'Understanding Our Benefits Package',
    excerpt: 'A guide to employee benefits, insurance, and wellness programs.',
    category: 'HR & Benefits',
    readTime: '10 min',
    icon: 'fas fa-heart',
    image: 'https://images.unsplash.com/photo-1521791136064-7986c2920216?w=400&h=300&fit=crop',
    author: { name: 'Lisa Wong', initials: 'LW' },
    date: '2 days ago',
    views: 1567,
    likes: 45,
    featured: false,
    categoryId: 'hr',
    type: 'guide',
    tags: ['Benefits', 'Insurance']
  },
  {
    id: 5,
    title: 'API Integration Tutorial: Part 1',
    excerpt: 'Step-by-step guide to integrating with our REST API.',
    category: 'Tutorials',
    readTime: '15 min',
    icon: 'fas fa-code',
    image: 'https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=400&h=300&fit=crop',
    author: { name: 'Alex Thompson', initials: 'AT' },
    date: '3 days ago',
    views: 3200,
    likes: 89,
    featured: false,
    categoryId: 'tutorials',
    type: 'tutorial',
    tags: ['API', 'Development']
  },
  {
    id: 6,
    title: 'Cloud Infrastructure Best Practices',
    excerpt: 'Optimize your cloud deployments with proven patterns.',
    category: 'Technology',
    readTime: '20 min',
    icon: 'fas fa-cloud',
    image: 'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=400&h=300&fit=crop',
    author: { name: 'David Kim', initials: 'DK' },
    date: '4 days ago',
    views: 1890,
    likes: 56,
    featured: false,
    categoryId: 'tech',
    type: 'how-to',
    tags: ['Cloud', 'AWS']
  },
  {
    id: 7,
    title: 'New Employee Onboarding Checklist',
    excerpt: 'Everything new team members need to know.',
    category: 'Getting Started',
    readTime: '7 min',
    icon: 'fas fa-clipboard-check',
    image: 'https://images.unsplash.com/photo-1553877522-43269d4ea984?w=400&h=300&fit=crop',
    author: { name: 'Rachel Green', initials: 'RG' },
    date: '5 days ago',
    views: 4500,
    likes: 112,
    featured: false,
    categoryId: 'getting-started',
    type: 'how-to',
    tags: ['Onboarding', 'Checklist']
  },
  {
    id: 8,
    title: 'Data Privacy and GDPR Compliance',
    excerpt: 'Understanding data protection regulations.',
    category: 'Policies',
    readTime: '18 min',
    icon: 'fas fa-user-shield',
    image: 'https://images.unsplash.com/photo-1633265486064-086b219458ec?w=400&h=300&fit=crop',
    author: { name: 'James Wilson', initials: 'JW' },
    date: '1 week ago',
    views: 2340,
    likes: 78,
    featured: false,
    categoryId: 'policies',
    type: 'policy',
    tags: ['GDPR', 'Privacy']
  },
  {
    id: 9,
    title: 'Effective Meeting Management',
    excerpt: 'Tips for running productive meetings.',
    category: 'Best Practices',
    readTime: '6 min',
    icon: 'fas fa-video',
    image: 'https://images.unsplash.com/photo-1517245386807-bb43f82c33c4?w=400&h=300&fit=crop',
    author: { name: 'Nancy Drew', initials: 'ND' },
    date: '1 week ago',
    views: 1678,
    likes: 41,
    featured: false,
    categoryId: 'best-practices',
    type: 'how-to',
    tags: ['Meetings', 'Tips']
  },
  {
    id: 10,
    title: 'Building Microservices with Docker',
    excerpt: 'A hands-on tutorial for containerizing applications.',
    category: 'Tutorials',
    readTime: '25 min',
    icon: 'fas fa-cube',
    image: 'https://images.unsplash.com/photo-1605745341112-85968b19335b?w=400&h=300&fit=crop',
    author: { name: 'Chris Evans', initials: 'CE' },
    date: '2 weeks ago',
    views: 5600,
    likes: 156,
    featured: false,
    categoryId: 'tutorials',
    type: 'tutorial',
    tags: ['Docker', 'Microservices']
  },
  {
    id: 11,
    title: 'Annual Leave Policy Updates',
    excerpt: 'Recent changes to vacation and time-off policies.',
    category: 'HR & Benefits',
    readTime: '4 min',
    icon: 'fas fa-umbrella-beach',
    image: 'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=400&h=300&fit=crop',
    author: { name: 'Lisa Wong', initials: 'LW' },
    date: '2 weeks ago',
    views: 3400,
    likes: 95,
    featured: false,
    categoryId: 'hr',
    type: 'announcement',
    tags: ['Leave', 'Policy']
  },
  {
    id: 12,
    title: 'Machine Learning Fundamentals',
    excerpt: 'Introduction to ML concepts and applications.',
    category: 'Technology',
    readTime: '30 min',
    icon: 'fas fa-brain',
    image: 'https://images.unsplash.com/photo-1677442136019-21780ecad995?w=400&h=300&fit=crop',
    author: { name: 'Dr. Sarah Mitchell', initials: 'SM' },
    date: '2 weeks ago',
    views: 4200,
    likes: 128,
    featured: false,
    categoryId: 'tech',
    type: 'tutorial',
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
  count += selectedCategories.value.length
  count += selectedTypes.value.length
  count += selectedTags.value.length
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
// PAGINATION FUNCTIONS
// ============================================

function goToPage(page: number) {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
  }
}

function nextPage() {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

function prevPage() {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

function changeItemsPerPage(count: number) {
  itemsPerPage.value = count
  currentPage.value = 1
}

// ============================================
// FUNCTIONS
// ============================================

function filterArticles() {
  currentPage.value = 1
  let result = [...articles.value]

  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(a =>
      a.title.toLowerCase().includes(query) ||
      a.excerpt.toLowerCase().includes(query) ||
      a.category.toLowerCase().includes(query) ||
      a.tags?.some(t => t.toLowerCase().includes(query))
    )
  }

  // Category filter (multi-select)
  if (selectedCategories.value.length > 0) {
    result = result.filter(a => selectedCategories.value.includes(a.categoryId))
  }

  // Type filter (multi-select)
  if (selectedTypes.value.length > 0) {
    result = result.filter(a => a.type && selectedTypes.value.includes(a.type))
  }

  // Tags filter (multi-select)
  if (selectedTags.value.length > 0) {
    result = result.filter(a => a.tags?.some(tag => selectedTags.value.includes(tag)))
  }

  // Featured filter (when selected as a category)
  if (selectedCategories.value.includes('featured')) {
    result = result.filter(a => a.featured)
  }

  // Sorting
  switch (sortBy.value) {
    case 'popular':
      result.sort((a, b) => sortOrder.value === 'desc' ? b.views - a.views : a.views - b.views)
      break
    case 'title':
      result.sort((a, b) => sortOrder.value === 'desc' ? b.title.localeCompare(a.title) : a.title.localeCompare(b.title))
      break
    case 'recent':
    default:
      // Keep default order (most recent first for desc)
      if (sortOrder.value === 'asc') {
        result.reverse()
      }
      break
  }

  filteredArticles.value = result
}

// Toggle functions for multi-select filters
function toggleCategoryFilter(categoryId: string) {
  const index = selectedCategories.value.indexOf(categoryId)
  if (index > -1) {
    selectedCategories.value.splice(index, 1)
  } else {
    selectedCategories.value.push(categoryId)
  }
  filterArticles()
}

function toggleTypeFilter(typeId: string) {
  const index = selectedTypes.value.indexOf(typeId)
  if (index > -1) {
    selectedTypes.value.splice(index, 1)
  } else {
    selectedTypes.value.push(typeId)
  }
  filterArticles()
}

function toggleTagFilter(tag: string) {
  const index = selectedTags.value.indexOf(tag)
  if (index > -1) {
    selectedTags.value.splice(index, 1)
  } else {
    selectedTags.value.push(tag)
  }
  filterArticles()
}

// Check functions for filter state
function isCategorySelected(categoryId: string) {
  return selectedCategories.value.includes(categoryId)
}

function isTypeSelected(typeId: string) {
  return selectedTypes.value.includes(typeId)
}

function isTagSelected(tag: string) {
  return selectedTags.value.includes(tag)
}

function clearFilters() {
  searchQuery.value = ''
  selectedCategories.value = []
  selectedTypes.value = []
  selectedTags.value = []
  sortBy.value = 'recent'
  sortOrder.value = 'desc'
  filterArticles()
}

function getCategoryName(id: string) {
  const cat = categories.value.find(c => c.id === id)
  return cat ? cat.name : id
}

function getTypeName(id: string) {
  const type = articleTypes.value.find(t => t.id === id)
  return type ? type.name : id
}

function formatNumber(num: number): string {
  if (num >= 1000000) return (num / 1000000).toFixed(1) + 'M'
  if (num >= 1000) return (num / 1000).toFixed(1) + 'K'
  return num.toString()
}

function goToArticle(id: number) {
  router.push({ name: 'ArticleView', params: { slug: id.toString() } })
}

function goToProfile(userId: number) {
  router.push({ name: 'Profile', query: { user: userId.toString() } })
}

function scrollToBookmarks() {
  const bookmarksSection = document.querySelector('.bookmarks-section')
  if (bookmarksSection) {
    bookmarksSection.scrollIntoView({ behavior: 'smooth', block: 'start' })
  }
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
    <div class="hero-gradient relative overflow-hidden">
      <!-- Decorative elements with animations -->
      <div class="circle-drift-1 absolute top-0 right-0 w-96 h-96 bg-white/5 rounded-full"></div>
      <div class="circle-drift-2 absolute bottom-0 left-0 w-64 h-64 bg-white/5 rounded-full"></div>
      <div class="circle-drift-3 absolute top-1/2 right-1/4 w-32 h-32 bg-white/10 rounded-full"></div>

      <!-- Stats - Absolute Top Right -->
      <div class="stats-top-right">
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-newspaper"></i>
          </div>
          <p class="stat-value-mini">{{ articles.length }}</p>
          <p class="stat-label-mini">Articles</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-layer-group"></i>
          </div>
          <p class="stat-value-mini">{{ categories.length }}</p>
          <p class="stat-label-mini">Categories</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-eye"></i>
          </div>
          <p class="stat-value-mini">{{ totalViews }}</p>
          <p class="stat-label-mini">Total Views</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-users"></i>
          </div>
          <p class="stat-value-mini">{{ contributors }}</p>
          <p class="stat-label-mini">Contributors</p>
        </div>
      </div>

      <div class="relative px-8 py-8">
        <div class="px-3 py-1 bg-white/20 backdrop-blur-sm rounded-full text-white text-xs font-semibold inline-flex items-center gap-2 mb-4">
          <i class="fas fa-book-open"></i>
          AFC Asian Cup 2027
        </div>

        <h1 class="text-3xl font-bold text-white mb-2">Articles Hub</h1>
        <p class="text-teal-100 max-w-lg">Discover articles, tutorials, best practices, and knowledge resources.</p>

        <div class="flex flex-wrap gap-3 mt-6">
          <button @click="navigateToEditor" class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
            <i class="fas fa-plus"></i>
            New Article
          </button>
          <button @click="scrollToBookmarks" class="px-5 py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-xl font-semibold text-sm hover:bg-white/30 transition-all flex items-center gap-2">
            <i class="fas fa-bookmark"></i>
            My Bookmarks
          </button>
        </div>
      </div>
    </div>

    <!-- ============================================
         PERSONALIZATION SECTION
         ============================================ -->
    <div class="px-6 pt-6">

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
                <img
                  v-if="article!.image"
                  :src="article!.image"
                  :alt="article!.title"
                  class="continue-card-image"
                >
                <div v-else class="continue-card-placeholder">
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
        <section v-if="bookmarkedArticles.length > 0" class="personalization-column bookmarks-section">
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
                <img
                  v-if="article!.image"
                  :src="article!.image"
                  :alt="article!.title"
                  class="bookmark-card-image"
                >
                <div v-else class="bookmark-card-icon">
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
              <div
                v-for="contributor in topContributors"
                :key="contributor.id"
                @click="goToProfile(contributor.id)"
                class="contributor-item cursor-pointer"
              >
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

      <!-- Trending & Recently Viewed Side by Side -->
      <div class="discovery-row mb-10 fade-in-up" style="animation-delay: 0.4s">
        <!-- Trending -->
        <section class="discovery-column">
          <div class="section-header-row">
            <h2 class="section-title-sm">
              <i class="fas fa-fire text-red-500"></i>
              Trending
            </h2>
            <router-link to="#" class="view-all-link">
              View All <i class="fas fa-arrow-right"></i>
            </router-link>
          </div>
          <div class="trending-scroll scrollbar-elegant">
            <div v-for="(article, index) in trendingThisWeek" :key="'trending-' + article.id"
                 @click="goToArticle(article.id)"
                 class="trending-card cursor-pointer group relative">
              <div class="relative aspect-video rounded-xl overflow-hidden shadow-md">
                <img
                  v-if="article.image"
                  :src="article.image"
                  :alt="article.title"
                  class="absolute inset-0 w-full h-full object-cover"
                >
                <div v-else class="absolute inset-0 bg-gradient-to-br from-teal-500 to-teal-700 flex items-center justify-center">
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

        <!-- Recently Viewed -->
        <section class="discovery-column">
          <div class="section-header-row">
            <h2 class="section-title-sm">
              <i class="fas fa-history text-blue-500"></i>
              Recently Viewed
            </h2>
            <router-link to="#" class="view-all-link">
              View All <i class="fas fa-arrow-right"></i>
            </router-link>
          </div>
          <div class="recently-viewed-scroll scrollbar-elegant">
            <div v-for="article in recentlyViewedArticles" :key="'recent-' + article.id"
                 @click="goToArticle(article.id)"
                 class="recently-viewed-card cursor-pointer group">
              <div class="relative aspect-video rounded-xl overflow-hidden shadow-md">
                <img
                  v-if="article.image"
                  :src="article.image"
                  :alt="article.title"
                  class="absolute inset-0 w-full h-full object-cover"
                >
                <div v-else class="absolute inset-0 bg-gradient-to-br from-blue-500 to-blue-700 flex items-center justify-center">
                  <i :class="article.icon || 'fas fa-newspaper'" class="text-white text-2xl"></i>
                </div>
                <div class="absolute inset-0 bg-black/0 group-hover:bg-black/20 transition-colors"></div>
                <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
                  <div class="w-12 h-12 rounded-full bg-white/95 flex items-center justify-center shadow-lg">
                    <i class="fas fa-arrow-right text-blue-600"></i>
                  </div>
                </div>
                <div class="absolute bottom-2 right-2 px-2 py-0.5 rounded bg-black/70 text-white text-xs font-medium backdrop-blur-sm">
                  {{ article.readTime }}
                </div>
              </div>
              <div class="mt-3">
                <h4 class="font-medium text-sm text-gray-900 line-clamp-2 group-hover:text-blue-700 transition-colors">{{ article.title }}</h4>
                <p class="text-xs text-gray-500 mt-1 flex items-center gap-2">
                  <span>{{ article.category }}</span>
                  <span class="text-gray-300">|</span>
                  <span><i class="fas fa-eye text-gray-400 mr-1"></i>{{ formatNumber(article.views) }}</span>
                </p>
              </div>
            </div>
          </div>
        </section>
      </div>

    </div>

    <!-- ============================================
         ALL ARTICLES SECTION
         ============================================ -->

    <!-- Main Content Area -->
    <div class="px-6 pb-6">
      <!-- Section Header -->
      <div class="flex items-center justify-between mb-4">
        <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
            <i class="fas fa-newspaper text-white text-sm"></i>
          </div>
          <div>
            <span class="block">All Articles</span>
            <span class="text-xs font-medium text-gray-500">{{ filteredArticles.length }} articles found</span>
          </div>
        </h2>
      </div>

      <!-- Toolbar (Documents Style) -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-100 mb-4">
        <div class="px-4 py-3 bg-gray-50/50 rounded-2xl flex flex-wrap items-center gap-3">
          <!-- Search -->
          <div class="flex-1 min-w-[200px] max-w-md relative">
            <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-sm"></i>
            <input
              v-model="searchQuery"
              @input="filterArticles"
              type="text"
              placeholder="Search articles..."
              class="w-full pl-9 pr-4 py-2 bg-white border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-all"
            >
            <button v-if="searchQuery" @click="searchQuery = ''; filterArticles()" class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600">
              <i class="fas fa-times text-xs"></i>
            </button>
          </div>

          <!-- Type Filter -->
          <div class="relative">
            <button
              @click="showTypeFilter = !showTypeFilter"
              :class="[
                'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                selectedTypes.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
              ]"
            >
              <i class="fas fa-file-alt text-sm"></i>
              <span>{{ selectedTypes.length > 0 ? `Type (${selectedTypes.length})` : 'Type' }}</span>
              <i :class="showTypeFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
            </button>

            <!-- Click outside to close (must be before dropdown) -->
            <div v-if="showTypeFilter" @click="showTypeFilter = false" class="fixed inset-0 z-40"></div>

            <!-- Dropdown Menu -->
            <div
              v-if="showTypeFilter"
              class="absolute left-0 top-full mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
            >
              <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Select Types</div>
              <div class="max-h-48 overflow-y-auto">
                <button
                  v-for="type in articleTypes"
                  :key="type.id"
                  @click="toggleTypeFilter(type.id)"
                  :class="[
                    'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                    isTypeSelected(type.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                  ]"
                >
                  <div :class="[
                    'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                    isTypeSelected(type.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                  ]">
                    <i v-if="isTypeSelected(type.id)" class="fas fa-check text-white text-[8px]"></i>
                  </div>
                  <i :class="[type.icon, 'text-teal-500 text-sm']"></i>
                  <span class="flex-1">{{ type.name }}</span>
                </button>
              </div>

              <div class="my-2 border-t border-gray-100"></div>

              <div class="px-3 flex gap-2">
                <button
                  @click="selectedTypes = []; filterArticles(); showTypeFilter = false"
                  class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                >
                  Clear
                </button>
                <button
                  @click="showTypeFilter = false"
                  class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                >
                  Apply
                </button>
              </div>
            </div>
          </div>

          <!-- Category Filter -->
          <div class="relative">
            <button
              @click="showCategoryFilter = !showCategoryFilter"
              :class="[
                'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                selectedCategories.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
              ]"
            >
              <i class="fas fa-layer-group text-sm"></i>
              <span>{{ selectedCategories.length > 0 ? `Category (${selectedCategories.length})` : 'Category' }}</span>
              <i :class="showCategoryFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
            </button>

            <!-- Click outside to close (must be before dropdown) -->
            <div v-if="showCategoryFilter" @click="showCategoryFilter = false" class="fixed inset-0 z-40"></div>

            <!-- Dropdown Menu -->
            <div
              v-if="showCategoryFilter"
              class="absolute left-0 top-full mt-2 w-64 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
            >
              <!-- Featured Option (Special) -->
              <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Special Filter</div>
              <button
                @click="toggleCategoryFilter('featured')"
                :class="[
                  'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                  isCategorySelected('featured') ? 'bg-amber-50 text-amber-700' : 'text-gray-700 hover:bg-gray-50'
                ]"
              >
                <div :class="[
                  'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                  isCategorySelected('featured') ? 'bg-amber-500 border-amber-500' : 'border-gray-300'
                ]">
                  <i v-if="isCategorySelected('featured')" class="fas fa-check text-white text-[8px]"></i>
                </div>
                <i class="fas fa-star text-amber-500 text-sm"></i>
                <span class="flex-1">Featured</span>
                <span class="text-xs text-gray-400">{{ categories.find(c => c.id === 'featured')?.count }}</span>
              </button>

              <div class="my-2 border-t border-gray-100"></div>

              <!-- Regular Categories -->
              <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Categories</div>
              <div class="max-h-48 overflow-y-auto">
                <button
                  v-for="cat in categories.filter(c => c.id !== 'featured')"
                  :key="cat.id"
                  @click="toggleCategoryFilter(cat.id)"
                  :class="[
                    'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                    isCategorySelected(cat.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                  ]"
                >
                  <div :class="[
                    'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                    isCategorySelected(cat.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                  ]">
                    <i v-if="isCategorySelected(cat.id)" class="fas fa-check text-white text-[8px]"></i>
                  </div>
                  <i :class="[cat.icon, 'text-teal-500 text-sm']"></i>
                  <span class="flex-1">{{ cat.name }}</span>
                  <span class="text-xs text-gray-400">{{ cat.count }}</span>
                </button>
              </div>

              <div class="my-2 border-t border-gray-100"></div>

              <div class="px-3 flex gap-2">
                <button
                  @click="selectedCategories = []; filterArticles(); showCategoryFilter = false"
                  class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                >
                  Clear
                </button>
                <button
                  @click="showCategoryFilter = false"
                  class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                >
                  Apply
                </button>
              </div>
            </div>
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
              <span>{{ selectedTags.length > 0 ? `Tags (${selectedTags.length})` : 'Tags' }}</span>
              <i :class="showTagFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
            </button>

            <!-- Click outside to close (must be before dropdown) -->
            <div v-if="showTagFilter" @click="showTagFilter = false" class="fixed inset-0 z-40"></div>

            <!-- Dropdown Menu -->
            <div
              v-if="showTagFilter"
              class="absolute left-0 top-full mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
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
                  <i class="fas fa-tag text-teal-500 text-sm"></i>
                  <span class="flex-1">{{ tag }}</span>
                </button>
              </div>

              <div class="my-2 border-t border-gray-100"></div>

              <div class="px-3 flex gap-2">
                <button
                  @click="selectedTags = []; filterArticles(); showTagFilter = false"
                  class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                >
                  Clear
                </button>
                <button
                  @click="showTagFilter = false"
                  class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                >
                  Apply
                </button>
              </div>
            </div>
          </div>

          <!-- Sort Options -->
          <div class="relative">
            <button
              @click="showSortDropdown = !showSortDropdown"
              class="flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border bg-white border-gray-200 text-gray-600 hover:bg-gray-50"
            >
              <i :class="[currentSortOption.icon, 'text-sm text-teal-500']"></i>
              <span>{{ currentSortOption.label }}</span>
              <i :class="showSortDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
            </button>

            <!-- Dropdown Menu -->
            <div
              v-if="showSortDropdown"
              class="absolute left-0 top-full mt-2 w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
            >
              <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Sort By</div>
              <div class="max-h-64 overflow-y-auto">
                <button
                  v-for="option in sortOptions"
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

          <!-- Sort Order Toggle Button -->
          <button
            @click="sortOrder = sortOrder === 'asc' ? 'desc' : 'asc'; filterArticles()"
            class="flex items-center justify-center w-8 h-8 rounded-lg text-xs font-medium transition-all border bg-white border-gray-200 text-gray-600 hover:bg-gray-50 hover:text-teal-600 hover:border-teal-300"
            :title="sortOrder === 'asc' ? 'Ascending order - Click for descending' : 'Descending order - Click for ascending'"
          >
            <i :class="sortOrder === 'asc' ? 'fas fa-arrow-up' : 'fas fa-arrow-down'" class="text-sm text-teal-500"></i>
          </button>

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
          <span v-if="searchQuery" class="px-2.5 py-1 bg-gray-100 text-gray-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-gray-200">
            <i class="fas fa-search text-[10px]"></i>
            "{{ searchQuery }}"
            <button @click="searchQuery = ''; filterArticles()" class="ml-1 hover:text-gray-900 hover:bg-gray-200 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
          </span>
          <!-- Category Filters (multiple) -->
          <span
            v-for="catId in selectedCategories"
            :key="'cat-' + catId"
            class="px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-teal-100"
          >
            <i class="fas fa-layer-group text-[10px]"></i>
            {{ getCategoryName(catId) }}
            <button @click="toggleCategoryFilter(catId)" class="ml-1 hover:text-teal-900 hover:bg-teal-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
          </span>
          <!-- Type Filters (multiple) -->
          <span
            v-for="typeId in selectedTypes"
            :key="'type-' + typeId"
            class="px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-teal-100"
          >
            <i class="fas fa-file-alt text-[10px]"></i>
            {{ getTypeName(typeId) }}
            <button @click="toggleTypeFilter(typeId)" class="ml-1 hover:text-teal-900 hover:bg-teal-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
          </span>
          <!-- Tag Filters (multiple) -->
          <span
            v-for="tag in selectedTags"
            :key="'tag-' + tag"
            class="px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-teal-100"
          >
            <i class="fas fa-tag text-[10px]"></i>
            {{ tag }}
            <button @click="toggleTagFilter(tag)" class="ml-1 hover:text-teal-900 hover:bg-teal-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
          </span>
        </div>
        <button @click="clearFilters" class="px-3 py-1.5 text-xs font-medium text-red-600 hover:text-red-700 hover:bg-red-50 rounded-lg transition-colors flex items-center gap-1.5">
          <i class="fas fa-times-circle"></i>
          Clear all
        </button>
      </div>

      <!-- Content Area -->
      <div class="bg-white rounded-2xl border border-gray-100 shadow-sm p-5">
        <div v-if="filteredArticles.length > 0">
          <!-- Grid View -->
          <div v-if="viewMode === 'grid'" class="grid grid-cols-[repeat(auto-fill,minmax(320px,1fr))] gap-5">
            <article
              v-for="article in paginatedArticles"
              :key="article.id"
              @click="goToArticle(article.id)"
              :class="[
                'group bg-white rounded-2xl overflow-hidden cursor-pointer transition-all duration-300 hover:-translate-y-1.5 border',
                article.featured
                  ? 'border-teal-200 shadow-lg shadow-teal-100/50 hover:shadow-xl hover:shadow-teal-200/50'
                  : 'border-gray-100 shadow-sm hover:shadow-lg hover:border-teal-200'
              ]"
            >
              <!-- Card Image -->
              <div class="relative h-44 overflow-hidden bg-gradient-to-br from-teal-50 to-cyan-50">
                <img
                  v-if="article.image"
                  :src="article.image"
                  :alt="article.title"
                  class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110"
                  @error="($event.target as HTMLImageElement).style.display = 'none'"
                >
                <div v-if="!article.image" class="absolute inset-0 flex items-center justify-center">
                  <i :class="[article.icon || 'fas fa-newspaper', 'text-4xl text-teal-300']"></i>
                </div>

                <!-- Gradient Overlay -->
                <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-black/20 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>

                <!-- Top Badges -->
                <div class="absolute top-3 left-3 right-3 flex items-start justify-between">
                  <div class="flex flex-wrap gap-1.5">
                    <!-- Featured Badge -->
                    <span v-if="article.featured" class="px-2 py-1 bg-amber-500 text-white text-[10px] font-semibold rounded-full flex items-center gap-1 shadow-sm">
                      <i class="fas fa-star text-[8px]"></i> Featured
                    </span>
                    <!-- Article Type Badge -->
                    <span
                      v-if="article.type"
                      class="px-2 py-1 bg-white/90 backdrop-blur-sm text-teal-700 text-[10px] font-semibold rounded-full flex items-center gap-1 shadow-sm"
                    >
                      <i :class="[articleTypes.find(t => t.id === article.type)?.icon || 'fas fa-file', 'text-[8px]']"></i>
                      {{ getTypeName(article.type) }}
                    </span>
                  </div>
                  <!-- Bookmark Button -->
                  <button
                    @click.stop="toggleBookmark(article.id)"
                    :class="[
                      'w-7 h-7 bg-white/90 backdrop-blur-sm rounded-full flex items-center justify-center transition-all shadow-sm opacity-0 group-hover:opacity-100',
                      bookmarks.includes(article.id) ? 'text-teal-600' : 'text-gray-400 hover:text-teal-600 hover:bg-white'
                    ]"
                  >
                    <i :class="bookmarks.includes(article.id) ? 'fas fa-bookmark' : 'far fa-bookmark'" class="text-xs"></i>
                  </button>
                </div>

                <!-- Category Badge (Bottom) -->
                <div class="absolute bottom-3 left-3">
                  <span class="px-2.5 py-1 bg-teal-500 text-white text-[10px] font-semibold rounded-full shadow-sm">
                    {{ article.category }}
                  </span>
                </div>

                <!-- Read Button (Hover) -->
                <div class="absolute bottom-3 right-3 opacity-0 group-hover:opacity-100 transition-all duration-300 translate-y-2 group-hover:translate-y-0">
                  <button @click.stop="goToArticle(article.id)" class="px-3 py-1.5 bg-white text-teal-600 text-xs font-semibold rounded-full flex items-center gap-1.5 shadow-md hover:bg-teal-500 hover:text-white transition-colors">
                    <span>Read</span>
                    <i class="fas fa-arrow-right text-[10px]"></i>
                  </button>
                </div>
              </div>

              <!-- Card Content -->
              <div class="p-4">
                <!-- Meta Info -->
                <div class="flex items-center gap-3 text-[11px] text-gray-400 mb-2">
                  <span class="flex items-center gap-1">
                    <i class="fas fa-clock text-[9px]"></i>
                    {{ article.readTime }}
                  </span>
                  <span class="flex items-center gap-1">
                    <i class="fas fa-calendar text-[9px]"></i>
                    {{ article.date }}
                  </span>
                </div>

                <!-- Title -->
                <h3 class="text-sm font-semibold text-gray-800 mb-2 line-clamp-2 group-hover:text-teal-600 transition-colors leading-snug">
                  {{ article.title }}
                </h3>

                <!-- Excerpt -->
                <p class="text-xs text-gray-500 mb-3 line-clamp-2 leading-relaxed">
                  {{ article.excerpt }}
                </p>

                <!-- Tags -->
                <div class="flex flex-wrap gap-1.5 mb-3">
                  <span
                    v-for="tag in article.tags?.slice(0, 3)"
                    :key="tag"
                    class="px-2 py-0.5 bg-gray-100 text-gray-600 text-[10px] font-medium rounded-full hover:bg-teal-50 hover:text-teal-600 transition-colors"
                  >
                    {{ tag }}
                  </span>
                </div>

                <!-- Footer -->
                <div class="flex items-center justify-between pt-3 border-t border-gray-100">
                  <!-- Author -->
                  <div class="flex items-center gap-2">
                    <div class="w-7 h-7 rounded-full bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white text-[10px] font-semibold shadow-sm">
                      {{ article.author?.initials || 'U' }}
                    </div>
                    <span class="text-xs text-gray-600 font-medium">{{ article.author?.name }}</span>
                  </div>

                  <!-- Stats -->
                  <div class="flex items-center gap-3 text-[11px] text-gray-400">
                    <span class="flex items-center gap-1 hover:text-teal-500 transition-colors">
                      <i class="fas fa-eye text-[9px]"></i>
                      {{ formatNumber(article.views) }}
                    </span>
                    <span class="flex items-center gap-1 hover:text-red-400 transition-colors">
                      <i class="fas fa-heart text-[9px]"></i>
                      {{ article.likes || 0 }}
                    </span>
                  </div>
                </div>
              </div>
            </article>
          </div>

          <!-- List View -->
          <div v-else class="space-y-3">
            <article
              v-for="article in paginatedArticles"
              :key="article.id"
              @click="goToArticle(article.id)"
              :class="[
                'group flex gap-4 p-4 bg-white rounded-2xl cursor-pointer transition-all duration-300 border',
                article.featured
                  ? 'border-teal-200 shadow-md hover:shadow-lg hover:border-teal-300'
                  : 'border-gray-100 shadow-sm hover:shadow-md hover:border-teal-200'
              ]"
            >
              <!-- Thumbnail -->
              <div class="relative w-32 h-24 flex-shrink-0 rounded-xl overflow-hidden bg-gradient-to-br from-teal-50 to-cyan-50">
                <img
                  v-if="article.image"
                  :src="article.image"
                  :alt="article.title"
                  class="w-full h-full object-cover transition-transform duration-300 group-hover:scale-110"
                >
                <div v-else class="absolute inset-0 flex items-center justify-center">
                  <i :class="[article.icon || 'fas fa-newspaper', 'text-2xl text-teal-300']"></i>
                </div>
                <!-- Featured Star -->
                <div v-if="article.featured" class="absolute top-1.5 left-1.5">
                  <span class="w-5 h-5 bg-amber-500 rounded-full flex items-center justify-center shadow-sm">
                    <i class="fas fa-star text-white text-[8px]"></i>
                  </span>
                </div>
              </div>

              <!-- Content -->
              <div class="flex-1 min-w-0">
                <!-- Header Badges -->
                <div class="flex flex-wrap items-center gap-2 mb-1.5">
                  <span class="px-2 py-0.5 bg-teal-500 text-white text-[10px] font-semibold rounded-full">
                    {{ article.category }}
                  </span>
                  <span
                    v-if="article.type"
                    class="px-2 py-0.5 bg-teal-50 text-teal-700 text-[10px] font-semibold rounded-full flex items-center gap-1"
                  >
                    <i :class="[articleTypes.find(t => t.id === article.type)?.icon || 'fas fa-file', 'text-[8px]']"></i>
                    {{ getTypeName(article.type) }}
                  </span>
                  <span class="text-[10px] text-gray-400 flex items-center gap-1">
                    <i class="fas fa-clock text-[8px]"></i>
                    {{ article.readTime }}
                  </span>
                </div>

                <!-- Title -->
                <h3 class="text-sm font-semibold text-gray-800 mb-1 truncate group-hover:text-teal-600 transition-colors">
                  {{ article.title }}
                </h3>

                <!-- Excerpt -->
                <p class="text-xs text-gray-500 mb-2 line-clamp-1">
                  {{ article.excerpt }}
                </p>

                <!-- Footer -->
                <div class="flex items-center justify-between">
                  <!-- Author & Date -->
                  <div class="flex items-center gap-2">
                    <div class="w-5 h-5 rounded-full bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white text-[8px] font-semibold">
                      {{ article.author?.initials || 'U' }}
                    </div>
                    <span class="text-[11px] text-gray-500">
                      {{ article.author?.name }} <span class="text-gray-300">·</span> {{ article.date }}
                    </span>
                  </div>

                  <!-- Stats & Tags -->
                  <div class="flex items-center gap-3">
                    <div class="hidden sm:flex items-center gap-1.5">
                      <span
                        v-for="tag in article.tags?.slice(0, 2)"
                        :key="tag"
                        class="px-1.5 py-0.5 bg-gray-100 text-gray-500 text-[9px] rounded-full"
                      >
                        {{ tag }}
                      </span>
                    </div>
                    <div class="flex items-center gap-2 text-[10px] text-gray-400">
                      <span class="flex items-center gap-1">
                        <i class="fas fa-eye text-[8px]"></i>
                        {{ formatNumber(article.views) }}
                      </span>
                      <span class="flex items-center gap-1">
                        <i class="fas fa-heart text-[8px]"></i>
                        {{ article.likes || 0 }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Actions -->
              <div class="flex flex-col gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                <button
                  @click.stop="toggleBookmark(article.id)"
                  :class="[
                    'w-8 h-8 rounded-lg flex items-center justify-center transition-colors',
                    bookmarks.includes(article.id) ? 'bg-teal-50 text-teal-600' : 'bg-gray-50 text-gray-400 hover:bg-teal-50 hover:text-teal-600'
                  ]"
                  title="Bookmark"
                >
                  <i :class="bookmarks.includes(article.id) ? 'fas fa-bookmark' : 'far fa-bookmark'" class="text-xs"></i>
                </button>
                <button
                  @click.stop="openShareModal(article)"
                  class="w-8 h-8 rounded-lg bg-gray-50 text-gray-400 hover:bg-teal-50 hover:text-teal-600 flex items-center justify-center transition-colors"
                  title="Share"
                >
                  <i class="fas fa-share-alt text-xs"></i>
                </button>
              </div>
            </article>
          </div>

          <!-- Pagination Footer (Documents Style) -->
          <div class="mt-4 px-4 py-3 bg-white rounded-2xl border border-gray-100 shadow-sm">
            <div class="flex items-center justify-between flex-wrap gap-3">
              <!-- Left: Stats & Items Per Page -->
              <div class="flex items-center gap-4 flex-wrap">
                <span class="text-xs text-gray-500">
                  Showing <span class="font-semibold text-gray-700">{{ Math.min((currentPage - 1) * itemsPerPage + 1, filteredArticles.length) }}</span>
                  to <span class="font-semibold text-gray-700">{{ Math.min(currentPage * itemsPerPage, filteredArticles.length) }}</span>
                  of <span class="font-semibold text-gray-700">{{ filteredArticles.length }}</span> articles
                </span>

                <!-- Items Per Page Selector -->
                <div class="flex items-center gap-2">
                  <span class="text-xs text-gray-500">Show:</span>
                  <select
                    v-model="itemsPerPage"
                    @change="changeItemsPerPage(Number(($event.target as HTMLSelectElement).value))"
                    class="text-xs px-2 py-1.5 rounded-lg border border-gray-200 bg-white text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-teal-500 cursor-pointer"
                  >
                    <option v-for="option in itemsPerPageOptions" :key="option" :value="option">
                      {{ option }}
                    </option>
                  </select>
                  <span class="text-xs text-gray-500">per page</span>
                </div>
              </div>

              <!-- Right: Pagination Controls -->
              <div class="flex items-center gap-2">
                <button
                  @click="prevPage"
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
                      @click="goToPage(page)"
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
                  @click="nextPage"
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
/* Utility classes */
.hidden {
  display: none !important;
}

.page-view {
  padding: 0;
  min-height: 100vh;
  background: linear-gradient(180deg, #f0fdfa 0%, #f8fafc 15%, #ffffff 100%);
}

/* ============================================
   HERO SECTION (Matching Documents Page)
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

/* ============================================
   MAIN CONTENT
   ============================================ */
.main-content {
  padding: 1.5rem;
  width: 100%;
}

/* Toolbar */
.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
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
  height: 40px;
  padding: 0 2rem 0 0.875rem;
  border: 1.5px solid #e2e8f0;
  border-radius: 0.75rem;
  background: white url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='14' height='14' viewBox='0 0 24 24' fill='none' stroke='%2394a3b8' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpolyline points='6 9 12 15 18 9'%3E%3C/polyline%3E%3C/svg%3E") no-repeat right 0.625rem center;
  font-size: 0.875rem;
  font-weight: 500;
  color: #334155;
  cursor: pointer;
  appearance: none;
  transition: all 0.2s ease;
}

.sort-select:hover {
  border-color: #5eead4;
  background-color: #f0fdfa;
}

.sort-select:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

/* View Toggle */
.view-toggle {
  display: flex;
  background: linear-gradient(135deg, #f0fdfa 0%, #f1f5f9 100%);
  border-radius: 0.75rem;
  padding: 0.25rem;
  border: 1px solid #e2e8f0;
}

.view-toggle button {
  padding: 0.5rem 0.75rem;
  border: none;
  background: transparent;
  color: #64748b;
  border-radius: 0.5rem;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 0.875rem;
}

.view-toggle button:hover {
  color: #0d9488;
  background: rgba(20, 184, 166, 0.1);
}

.view-toggle button.active {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.3);
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
  border-radius: 1.25rem;
  border: 1px solid #e5e7eb;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05), 0 2px 4px -2px rgba(0, 0, 0, 0.05);
  overflow: hidden;
  animation: fadeInUp 0.5s ease-out;
}

/* Article Cards Grid */
.articles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 1.25rem;
  padding: 1.25rem;
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

.card-image {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.article-card:hover .card-image {
  transform: scale(1.08);
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
  font-size: 1.125rem;
  font-weight: 700;
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
  gap: 4rem;
}

.personalization-column {
  min-width: 0;
}

.discovery-row {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 4rem;
}

.discovery-column {
  min-width: 0;
}

@media (max-width: 1024px) {
  .discovery-row {
    grid-template-columns: 1fr;
  }
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
  overflow: hidden;
  border-radius: 0.75rem 0.75rem 0 0;
}

.continue-card-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.continue-reading-card:hover .continue-card-image {
  transform: scale(1.05);
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
  overflow: hidden;
  border-radius: 0.75rem 0.75rem 0 0;
}

.bookmark-card-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.bookmark-card:hover .bookmark-card-image {
  transform: scale(1.05);
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
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.carousel-container {
  position: relative;
  overflow: hidden;
}

.featured-spotlight {
  background: white;
  border-radius: 1.5rem;
  overflow: hidden;
  border: 1px solid #f1f5f9;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
  display: grid;
  grid-template-columns: 1.5fr 1fr;
}

.featured-spotlight-main {
  display: flex;
  flex-direction: column;
  background: linear-gradient(135deg, #0f766e 0%, #0d9488 50%, #14b8a6 100%);
  color: white;
  position: relative;
  overflow: hidden;
}

.featured-spotlight-main::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -50%;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle, rgba(255,255,255,0.1) 0%, transparent 70%);
  pointer-events: none;
}

.featured-spotlight-content {
  flex: 1;
  padding: 2rem;
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
  padding: 1.5rem;
  background: #f8fafc;
  display: flex;
  flex-direction: column;
  height: 100%;
}

.related-articles-header {
  font-size: 1rem;
  font-weight: 700;
  color: #1e293b;
  display: flex;
  align-items: center;
  gap: 0.625rem;
  margin: 0 0 1.25rem 0;
}

.related-articles-header i {
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 0.5rem;
  color: white;
  font-size: 0.875rem;
}

.related-articles-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  flex: 1;
}

.related-article-card {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: white;
  border-radius: 0.875rem;
  border: 1px solid #e2e8f0;
  text-decoration: none;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
}

.related-article-card::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 3px;
  background: linear-gradient(180deg, #14b8a6 0%, #0d9488 100%);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.related-article-card:hover {
  border-color: #99f6e4;
  background: #fafffe;
  transform: translateX(4px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.1);
}

.related-article-card:hover::before {
  opacity: 1;
}

.related-article-number {
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-radius: 50%;
  color: #0d9488;
  font-size: 0.75rem;
  font-weight: 700;
  flex-shrink: 0;
}

.related-article-content {
  flex: 1;
  min-width: 0;
}

.related-article-title {
  font-size: 0.875rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.related-article-card:hover .related-article-title {
  color: #0d9488;
}

.related-article-meta {
  font-size: 0.75rem;
  color: #64748b;
  margin: 0.375rem 0 0;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.related-article-meta i {
  color: #94a3b8;
}

.related-article-arrow {
  color: #cbd5e1;
  font-size: 0.875rem;
  transition: all 0.2s ease;
  flex-shrink: 0;
}

.related-article-card:hover .related-article-arrow {
  transform: translateX(4px);
  color: #0d9488;
}

.carousel-nav {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
}

.carousel-nav-btn {
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

.carousel-nav-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-color: transparent;
  color: white;
  transform: scale(1.05);
}

.carousel-nav-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.carousel-dots {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.carousel-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background: #e2e8f0;
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
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  width: 28px;
  border-radius: 5px;
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
  transition: all 0.3s ease;
}

.contributors-card:hover {
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.08);
  border-color: rgba(20, 184, 166, 0.2);
}

.contributors-card h3 i {
  animation: trophyBounce 2s ease-in-out infinite;
}

@keyframes trophyBounce {
  0%, 100% { transform: rotate(-5deg) scale(1); }
  50% { transform: rotate(5deg) scale(1.1); }
}

.contributor-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  margin: 0 -0.5rem;
  border-radius: 0.75rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.contributor-item:hover {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
}

.contributor-item:nth-child(1) { animation: contributorSlideIn 0.4s ease-out backwards; animation-delay: 0.1s; }
.contributor-item:nth-child(2) { animation: contributorSlideIn 0.4s ease-out backwards; animation-delay: 0.2s; }
.contributor-item:nth-child(3) { animation: contributorSlideIn 0.4s ease-out backwards; animation-delay: 0.3s; }
.contributor-item:nth-child(4) { animation: contributorSlideIn 0.4s ease-out backwards; animation-delay: 0.4s; }
.contributor-item:nth-child(5) { animation: contributorSlideIn 0.4s ease-out backwards; animation-delay: 0.5s; }

@keyframes contributorSlideIn {
  from {
    opacity: 0;
    transform: translateX(-10px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.contributor-avatar {
  width: 40px;
  height: 40px;
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  color: white;
  font-size: 0.875rem;
  flex-shrink: 0;
}

.contributor-badge {
  padding: 0.25rem 0.5rem;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: white;
  font-size: 0.625rem;
  font-weight: 700;
  border-radius: 100px;
  text-transform: uppercase;
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
  font-size: 1.125rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0;
}

.trending-scroll {
  display: flex;
  gap: 1.5rem;
  overflow-x: auto;
  overflow-y: hidden;
  padding: 1rem 0 0.5rem 1rem;
  scroll-snap-type: x mandatory;
}

.trending-scroll::-webkit-scrollbar {
  height: 3px;
}

.trending-scroll::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 3px;
}

.trending-scroll::-webkit-scrollbar-thumb {
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  border-radius: 3px;
}

.trending-card {
  flex: 0 0 240px;
  scroll-snap-align: start;
  transition: all 0.3s ease;
}

.trending-card:hover {
  transform: translateY(-6px);
}

.recently-viewed-scroll {
  display: flex;
  gap: 1.5rem;
  overflow-x: auto;
  overflow-y: hidden;
  padding: 1rem 0 0.5rem 1rem;
  scroll-snap-type: x mandatory;
}

.recently-viewed-scroll::-webkit-scrollbar {
  height: 3px;
}

.recently-viewed-scroll::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 3px;
}

.recently-viewed-scroll::-webkit-scrollbar-thumb {
  background: linear-gradient(90deg, #3b82f6, #2563eb);
  border-radius: 3px;
}

.recently-viewed-card {
  flex: 0 0 240px;
  scroll-snap-align: start;
  transition: all 0.3s ease;
}

.recently-viewed-card:hover {
  transform: translateY(-6px);
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

  .hero-stats {
    flex-wrap: wrap;
  }

  .stat-card-hero {
    min-width: calc(50% - 0.5rem);
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
