<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useUIStore } from '@/stores/ui'
import { useAIServicesStore } from '@/stores/aiServices'
import { AIVoiceInput } from '@/components/ai'
import EmptyState from '@/components/common/EmptyState.vue'
import Pagination from '@/components/common/Pagination.vue'

// Stores
const uiStore = useUIStore()
const aiStore = useAIServicesStore()
const route = useRoute()
const router = useRouter()
const { t } = useI18n()

// Layout state
const isSidebarCollapsed = computed(() => uiStore.sidebarCollapsed)
const isFilterSidebarCollapsed = ref(false)
const showMobileFilters = ref(false)

// ============================================================================
// Configurable Text Constants
// ============================================================================
const textConstants = {
  // Header
  pageTitle: 'Search',
  searchPlaceholder: 'Search across all content...',
  searchButton: 'Search',
  searching: 'Searching...',

  // Quick filters
  quickFiltersLabel: 'Quick filters:',
  quickFilterAll: 'All',
  quickFilterArticles: 'Articles',
  quickFilterDocuments: 'Documents',
  quickFilterVideos: 'Videos',
  quickFilterCourses: 'Courses',

  // Filter sidebar
  filters: 'Filters',
  clearAll: 'Clear all',
  contentType: 'Content Type',
  dateRange: 'Date Range',
  author: 'Author',
  filterByAuthor: 'Filter by author...',
  tags: 'Tags',
  expandFilters: 'Expand filters',
  collapseFilters: 'Collapse filters',

  // Content types
  contentTypeArticles: 'Articles',
  contentTypeDocuments: 'Documents',
  contentTypeVideos: 'Videos',
  contentTypeCourses: 'Courses',
  contentTypeEvents: 'Events',
  contentTypePolls: 'Polls',

  // Date options
  anyTime: 'Any time',
  today: 'Today',
  pastWeek: 'Past week',
  pastMonth: 'Past month',
  pastYear: 'Past year',

  // Results
  resultsFor: 'results for',
  searchCompletedIn: 'Search completed in',
  ms: 'ms',
  aiAnalyzing: 'AI analyzing...',

  // Sort options
  mostRelevant: 'Most Relevant',
  mostRecent: 'Most Recent',
  mostViewed: 'Most Viewed',

  // AI features
  didYouMean: 'Did you mean:',
  aiDetectedEntities: 'AI Detected Entities:',
  aiSummary: 'AI Summary',
  showMore: 'Show more',
  copy: 'Copy',
  aiSearchInsights: 'AI Search Insights',
  clickToApply: 'Click to apply',
  aiRelatedSearches: 'AI Related Searches',
  basedOnPatterns: 'Based on your search patterns and popular queries',

  // Result card
  newBadge: 'New',
  bookmark: 'Bookmark',
  share: 'Share',
  preview: 'Preview',

  // Result types
  typeArticle: 'Article',
  typeDocument: 'Document',
  typeVideo: 'Video',
  typeCourse: 'Course',

  // Pagination
  showing: 'Showing',
  of: 'of',
  results: 'results',

  // Empty state
  noResults: 'No results found',
  noResultsDesc: 'Try adjusting your search or filters',

  // Initial state
  initialStateTitle: 'Start your search',
  initialStateDesc: 'Enter a search term to find content',

  // Intent types
  lookingForInfo: 'Looking for info',
  findingSpecific: 'Finding specific',
  takingAction: 'Taking action',

  // AI suggestions
  moreSpecific: 'More specific',
  popularSearch: 'Popular search',

  // AI insight templates
  insightTryAdding: 'Try adding',
  insightForLatest: 'for the latest',
  insightContent: 'content',
  insightUsersSearched: 'Users also searched for:',
  insightFilterBy: 'Filter by',
  insightForOfficial: 'for official content',
  insightTutorial: 'tutorial',
  insightExamples: 'examples',
  insightBestPractices: 'best practices',
  insightHowTo: 'how to',
  insightGuide: 'guide',

  // AI analysis descriptions
  intentLookingForGuides: 'Looking for learning resources and guides',
  intentLookingForSpecific: 'Looking for specific content',
  intentGeneralSearch: 'General information search',

  // AI summary template
  aiSummaryPrefix: 'Based on your search for',
  aiSummaryFound: 'I found',
  aiSummaryRelevantResources: 'relevant resources including articles, documents, and training materials. The most relevant content covers comprehensive guides, tutorials, and best practices related to',

  // Mock data labels
  authorContentTeam: 'Content Team',
  authorAdminUser: 'Admin User',
  authorSystemAdmin: 'System Admin',
  tagGuide: 'Guide'
}

// ============================================================================
// Types
// ============================================================================
interface QuickFilter {
  id: string
  label: string
  icon: string
  active: boolean
}

interface ContentType {
  id: string
  label: string
  count: number
  checked: boolean
}

interface DateFilter {
  id: string
  label: string
}

interface Author {
  id: number
  name: string
  initials: string
  color: string
  checked: boolean
}

interface Tag {
  id: number
  name: string
  active: boolean
}

interface ResultAuthor {
  name: string
  initials: string
  color: string
}

interface SearchResult {
  id: number
  type: string
  typeBadge: string
  icon: string
  iconBg: string
  iconColor: string
  title: string
  excerpt: string
  author: ResultAuthor
  date: string
  views: number
  tags: string[]
  isNew: boolean
}

interface QueryIntent {
  type: 'informational' | 'navigational' | 'transactional'
  confidence: number
  description: string
}

interface ExtractedEntity {
  text: string
  type: 'person' | 'organization' | 'topic' | 'date' | 'location'
  confidence: number
}

interface DidYouMeanSuggestion {
  original: string
  suggestion: string
  reason: string
}

interface AISearchInsight {
  id: string
  type: 'tip' | 'related' | 'refine'
  text: string
  action?: string
}

// ============================================================================
// Search State
// ============================================================================
const isLoading = ref(false)
const searchQuery = ref('')
const displayQuery = ref('')
const viewMode = ref<'list' | 'grid'>('list')
const sortBy = ref('relevance')
const selectedDate = ref('all')
const authorFilter = ref('')
const currentPage = ref(1)
const pageSize = ref(10)

// ============================================================================
// Filter Data - Dynamic (computed from text constants)
// ============================================================================
const quickFilters = ref<QuickFilter[]>([
  { id: 'all', label: textConstants.quickFilterAll, icon: 'fas fa-globe', active: true },
  { id: 'articles', label: textConstants.quickFilterArticles, icon: 'fas fa-file-alt', active: false },
  { id: 'documents', label: textConstants.quickFilterDocuments, icon: 'fas fa-file-pdf', active: false },
  { id: 'videos', label: textConstants.quickFilterVideos, icon: 'fas fa-video', active: false },
  { id: 'courses', label: textConstants.quickFilterCourses, icon: 'fas fa-graduation-cap', active: false }
])

const contentTypes = ref<ContentType[]>([
  { id: 'articles', label: textConstants.contentTypeArticles, count: 0, checked: true },
  { id: 'documents', label: textConstants.contentTypeDocuments, count: 0, checked: true },
  { id: 'videos', label: textConstants.contentTypeVideos, count: 0, checked: true },
  { id: 'courses', label: textConstants.contentTypeCourses, count: 0, checked: false },
  { id: 'events', label: textConstants.contentTypeEvents, count: 0, checked: false },
  { id: 'polls', label: textConstants.contentTypePolls, count: 0, checked: false }
])

const dateFilters = computed<DateFilter[]>(() => [
  { id: 'all', label: textConstants.anyTime },
  { id: 'today', label: textConstants.today },
  { id: 'week', label: textConstants.pastWeek },
  { id: 'month', label: textConstants.pastMonth },
  { id: 'year', label: textConstants.pastYear }
])

const topAuthors = ref<Author[]>([])

const popularTags = ref<Tag[]>([])

// ============================================================================
// Results Data - Initially empty
// ============================================================================
const results = ref<SearchResult[]>([])
const totalResults = ref(0)
const searchTime = ref(0)

// Computed pagination
const totalPages = computed(() => Math.ceil(totalResults.value / pageSize.value))
const startResult = computed(() => (currentPage.value - 1) * pageSize.value + 1)
const endResult = computed(() => Math.min(currentPage.value * pageSize.value, totalResults.value))

const paginationPages = computed(() => {
  const pages: (number | string)[] = []
  const total = totalPages.value
  const current = currentPage.value

  if (total <= 7) {
    for (let i = 1; i <= total; i++) pages.push(i)
  } else {
    pages.push(1)
    if (current > 3) pages.push('...')

    const start = Math.max(2, current - 1)
    const end = Math.min(total - 1, current + 1)

    for (let i = start; i <= end; i++) pages.push(i)

    if (current < total - 2) pages.push('...')
    pages.push(total)
  }

  return pages
})

// Active filters count
const activeFiltersCount = computed(() => {
  let count = 0
  if (selectedDate.value !== 'all') count++
  count += topAuthors.value.filter(a => a.checked).length
  count += popularTags.value.filter(t => t.active).length
  count += contentTypes.value.filter(t => !t.checked).length
  return count
})

// ============================================================================
// AI Features State
// ============================================================================
const showAIPanel = ref(true)
const isAnalyzingQuery = ref(false)
const showDidYouMean = ref(true)

const queryIntent = ref<QueryIntent | null>(null)
const extractedEntities = ref<ExtractedEntity[]>([])
const didYouMeanSuggestions = ref<DidYouMeanSuggestion[]>([])
const aiSearchInsights = ref<AISearchInsight[]>([])
const relatedSearches = ref<string[]>([])
const aiSummary = ref('')

// ============================================================================
// Methods
// ============================================================================

// Initialize from route query
onMounted(() => {
  const q = route.query.q as string
  if (q) {
    searchQuery.value = q
    performSearch()
  }
})

// Watch route changes
watch(() => route.query.q, (newQuery) => {
  if (newQuery && newQuery !== searchQuery.value) {
    searchQuery.value = newQuery as string
    performSearch()
  }
})

function toggleQuickFilter(filter: QuickFilter): void {
  if (filter.id === 'all') {
    quickFilters.value.forEach(f => f.active = f.id === 'all')
  } else {
    const allFilter = quickFilters.value.find(f => f.id === 'all')
    if (allFilter) allFilter.active = false
    filter.active = !filter.active

    // If no filters active, activate 'all'
    if (!quickFilters.value.some(f => f.active)) {
      if (allFilter) allFilter.active = true
    }
  }
  performSearch()
}

function clearFilters(): void {
  contentTypes.value.forEach(t => t.checked = true)
  topAuthors.value.forEach(a => a.checked = false)
  popularTags.value.forEach(t => t.active = false)
  selectedDate.value = 'all'
  quickFilters.value.forEach(f => f.active = f.id === 'all')
  performSearch()
}

async function performSearch(): Promise<void> {
  if (!searchQuery.value.trim()) return

  isLoading.value = true
  displayQuery.value = searchQuery.value
  currentPage.value = 1
  showDidYouMean.value = true

  const startTime = performance.now()

  // Update URL
  router.replace({ query: { q: searchQuery.value } })

  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 600))

    // Generate mock results based on query
    generateMockResults()

    // Analyze query with AI
    await analyzeSearchQuery(searchQuery.value)

    searchTime.value = Math.round(performance.now() - startTime)
  } catch (error) {
    console.error('Search failed:', error)
  } finally {
    isLoading.value = false
  }
}

function generateMockResults(): void {
  const query = searchQuery.value.toLowerCase()

  // Generate dynamic results based on query
  const mockResults: SearchResult[] = []
  const types = [
    { type: textConstants.typeArticle, badge: 'badge-blue', icon: 'fas fa-file-alt', bg: 'bg-blue-100', color: 'text-blue-600' },
    { type: textConstants.typeDocument, badge: 'badge-purple', icon: 'fas fa-file-pdf', bg: 'bg-purple-100', color: 'text-purple-600' },
    { type: textConstants.typeVideo, badge: 'badge-red', icon: 'fas fa-video', bg: 'bg-red-100', color: 'text-red-600' },
    { type: textConstants.typeCourse, badge: 'badge-green', icon: 'fas fa-graduation-cap', bg: 'bg-green-100', color: 'text-green-600' }
  ]

  const authors = [
    { name: textConstants.authorContentTeam, initials: 'CT', color: '#10B981' },
    { name: textConstants.authorAdminUser, initials: 'AU', color: '#8B5CF6' },
    { name: textConstants.authorSystemAdmin, initials: 'SA', color: '#3B82F6' }
  ]

  // Generate 10-20 results
  const count = Math.floor(Math.random() * 11) + 10
  const totalCount = Math.floor(Math.random() * 100) + count

  for (let i = 0; i < count; i++) {
    const typeInfo = types[i % types.length]
    const author = authors[i % authors.length]
    const daysAgo = Math.floor(Math.random() * 30)
    const date = new Date()
    date.setDate(date.getDate() - daysAgo)

    mockResults.push({
      id: i + 1,
      type: typeInfo.type,
      typeBadge: typeInfo.badge,
      icon: typeInfo.icon,
      iconBg: typeInfo.bg,
      iconColor: typeInfo.color,
      title: `${query.charAt(0).toUpperCase() + query.slice(1)} - ${typeInfo.type} ${i + 1}`,
      excerpt: `This is a comprehensive ${typeInfo.type.toLowerCase()} about ${query}. It covers all the essential information and best practices related to ${query}...`,
      author: author,
      date: date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' }),
      views: Math.floor(Math.random() * 2000) + 100,
      tags: [query.split(' ')[0], typeInfo.type, textConstants.tagGuide].filter(Boolean),
      isNew: daysAgo < 3
    })
  }

  results.value = mockResults
  totalResults.value = totalCount

  // Update content type counts
  contentTypes.value.forEach(ct => {
    ct.count = mockResults.filter(r => r.type.toLowerCase() === ct.id.slice(0, -1) ||
               (ct.id === 'articles' && r.type === 'Article') ||
               (ct.id === 'documents' && r.type === 'Document') ||
               (ct.id === 'videos' && r.type === 'Video') ||
               (ct.id === 'courses' && r.type === 'Course')).length
  })

  // Update authors
  const uniqueAuthors = [...new Set(mockResults.map(r => r.author.name))]
  topAuthors.value = uniqueAuthors.map((name, idx) => {
    const authorInfo = authors.find(a => a.name === name) || authors[0]
    return {
      id: idx + 1,
      name: authorInfo.name,
      initials: authorInfo.initials,
      color: authorInfo.color,
      checked: false
    }
  })

  // Update tags
  const allTags = [...new Set(mockResults.flatMap(r => r.tags))]
  popularTags.value = allTags.slice(0, 8).map((tag, idx) => ({
    id: idx + 1,
    name: tag,
    active: false
  }))
}

async function analyzeSearchQuery(query: string): Promise<void> {
  isAnalyzingQuery.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 400))

    // Detect intent
    const lowerQuery = query.toLowerCase()
    if (lowerQuery.includes('how') || lowerQuery.includes('what') || lowerQuery.includes('guide')) {
      queryIntent.value = {
        type: 'informational',
        confidence: 0.92,
        description: textConstants.intentLookingForGuides
      }
    } else if (lowerQuery.includes('find') || lowerQuery.includes('download') || lowerQuery.includes('document')) {
      queryIntent.value = {
        type: 'navigational',
        confidence: 0.88,
        description: textConstants.intentLookingForSpecific
      }
    } else {
      queryIntent.value = {
        type: 'informational',
        confidence: 0.75,
        description: textConstants.intentGeneralSearch
      }
    }

    // Extract entities
    const words = query.split(' ').filter(w => w.length > 2)
    extractedEntities.value = words.slice(0, 3).map(word => ({
      text: word.charAt(0).toUpperCase() + word.slice(1),
      type: 'topic' as const,
      confidence: 0.85 + Math.random() * 0.15
    }))

    // Generate suggestions
    didYouMeanSuggestions.value = [
      { original: query, suggestion: `${query} guide`, reason: textConstants.moreSpecific },
      { original: query, suggestion: `${query} best practices`, reason: textConstants.popularSearch }
    ]

    // Generate insights
    aiSearchInsights.value = [
      { id: '1', type: 'tip', text: `${textConstants.insightTryAdding} "2024" ${textConstants.insightForLatest} ${query} ${textConstants.insightContent}`, action: `${query} 2024` },
      { id: '2', type: 'related', text: `${textConstants.insightUsersSearched} "${query} ${textConstants.insightTutorial}"`, action: `${query} ${textConstants.insightTutorial}` },
      { id: '3', type: 'refine', text: `${textConstants.insightFilterBy} ${textConstants.authorContentTeam} ${textConstants.insightForOfficial}`, action: `filter:author:${textConstants.authorContentTeam}` }
    ]

    // Related searches
    relatedSearches.value = [
      `${query} ${textConstants.insightTutorial}`,
      `${query} ${textConstants.insightExamples}`,
      `${query} ${textConstants.insightBestPractices}`,
      `${textConstants.insightHowTo} ${query}`,
      `${query} ${textConstants.insightGuide}`
    ]

    // AI Summary
    aiSummary.value = `${textConstants.aiSummaryPrefix} "${query}", ${textConstants.aiSummaryFound} ${totalResults.value} ${textConstants.aiSummaryRelevantResources} ${query}.`

  } catch (error) {
    console.error('Query analysis failed:', error)
  } finally {
    isAnalyzingQuery.value = false
  }
}

function highlightText(text: string): string {
  if (!displayQuery.value) return text
  const words = displayQuery.value.toLowerCase().split(' ').filter(w => w.length > 2)
  let result = text
  words.forEach(word => {
    const regex = new RegExp(`(${word})`, 'gi')
    result = result.replace(regex, '<mark class="bg-yellow-200 text-gray-900 rounded px-0.5">$1</mark>')
  })
  return result
}

function toggleTag(tag: Tag): void {
  tag.active = !tag.active
  performSearch()
}

function applyDidYouMean(suggestion: string): void {
  searchQuery.value = suggestion
  performSearch()
  showDidYouMean.value = false
}

function applyEntityFilter(entity: ExtractedEntity): void {
  searchQuery.value = `${searchQuery.value} ${entity.text}`
  performSearch()
}

function applySearchInsight(insight: AISearchInsight): void {
  if (insight.action) {
    if (insight.action.startsWith('filter:')) {
      const [, filterType, filterValue] = insight.action.split(':')
      console.log('Apply filter:', filterType, filterValue)
    } else {
      searchQuery.value = insight.action
      performSearch()
    }
  }
}

function goToPage(page: number | string): void {
  if (typeof page === 'number' && page >= 1 && page <= totalPages.value) {
    currentPage.value = page
    // Scroll to top of results
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

function handleVoiceTranscript(text: string, isFinal: boolean): void {
  searchQuery.value = text
  if (isFinal) {
    performSearch()
  }
}

function copyAISummary(): void {
  navigator.clipboard.writeText(aiSummary.value)
}

// Helper functions
function getEntityTypeColor(type: string): string {
  const colors: Record<string, string> = {
    person: 'bg-blue-100 text-blue-700',
    organization: 'bg-purple-100 text-purple-700',
    topic: 'bg-teal-100 text-teal-700',
    date: 'bg-amber-100 text-amber-700',
    location: 'bg-green-100 text-green-700'
  }
  return colors[type] || 'bg-gray-100 text-gray-700'
}

function getEntityTypeIcon(type: string): string {
  const icons: Record<string, string> = {
    person: 'fas fa-user',
    organization: 'fas fa-building',
    topic: 'fas fa-tag',
    date: 'fas fa-calendar',
    location: 'fas fa-map-marker-alt'
  }
  return icons[type] || 'fas fa-circle'
}

function getIntentTypeColor(type: string): string {
  const colors: Record<string, string> = {
    informational: 'bg-blue-100 text-blue-700',
    navigational: 'bg-purple-100 text-purple-700',
    transactional: 'bg-green-100 text-green-700'
  }
  return colors[type] || 'bg-gray-100 text-gray-700'
}

function getIntentLabel(type: string): string {
  const labels: Record<string, string> = {
    informational: textConstants.lookingForInfo,
    navigational: textConstants.findingSpecific,
    transactional: textConstants.takingAction
  }
  return labels[type] || type
}
</script>

<template>
  <div
    class="fixed right-0 bottom-0 top-[64px] flex overflow-hidden bg-gradient-to-br from-gray-50 via-white to-teal-50/20 transition-all duration-300"
    :class="isSidebarCollapsed ? 'left-20' : 'left-64'"
  >
    <!-- Filter Sidebar -->
    <aside
      :class="[
        'border-r border-gray-200 bg-white flex-shrink-0 flex flex-col h-full transition-all duration-300',
        isFilterSidebarCollapsed ? 'w-14' : 'w-72'
      ]"
    >
      <!-- Sidebar Header -->
      <div class="p-3 flex items-center border-b border-gray-100" :class="isFilterSidebarCollapsed ? 'justify-center' : 'justify-between'">
        <div v-if="!isFilterSidebarCollapsed" class="flex items-center gap-2">
          <i class="fas fa-filter text-teal-500"></i>
          <span class="text-sm font-medium text-gray-700">{{ textConstants.filters }}</span>
          <span v-if="activeFiltersCount > 0" class="px-1.5 py-0.5 bg-teal-500 text-white text-xs rounded-full">
            {{ activeFiltersCount }}
          </span>
        </div>
        <button
          @click="isFilterSidebarCollapsed = !isFilterSidebarCollapsed"
          class="w-8 h-8 flex items-center justify-center rounded-lg hover:bg-gray-100 text-gray-500 transition-colors"
          :title="isFilterSidebarCollapsed ? textConstants.expandFilters : textConstants.collapseFilters"
        >
          <i :class="['fas text-xs', isFilterSidebarCollapsed ? 'fa-angles-right' : 'fa-angles-left']"></i>
        </button>
      </div>

      <!-- Clear Filters -->
      <div v-if="!isFilterSidebarCollapsed && activeFiltersCount > 0" class="px-4 py-2 border-b border-gray-100">
        <button @click="clearFilters" class="text-sm text-teal-600 hover:text-teal-800 font-medium">
          <i class="fas fa-times mr-1"></i>{{ textConstants.clearAll }}
        </button>
      </div>

      <!-- Filter Content -->
      <div class="flex-1 overflow-y-auto" v-if="!isFilterSidebarCollapsed">
        <!-- Content Type -->
        <div class="p-4 border-b border-gray-100">
          <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3">{{ textConstants.contentType }}</h4>
          <div class="space-y-2">
            <label
              v-for="type in contentTypes"
              :key="type.id"
              class="flex items-center gap-3 cursor-pointer group"
            >
              <input type="checkbox" v-model="type.checked" @change="performSearch" class="w-4 h-4 rounded border-gray-300 text-teal-500 focus:ring-teal-500">
              <span class="text-sm text-gray-700 group-hover:text-gray-900">{{ type.label }}</span>
              <span class="ml-auto text-xs text-gray-400">({{ type.count }})</span>
            </label>
          </div>
        </div>

        <!-- Date Range -->
        <div class="p-4 border-b border-gray-100">
          <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3">{{ textConstants.dateRange }}</h4>
          <div class="space-y-2">
            <label
              v-for="date in dateFilters"
              :key="date.id"
              class="flex items-center gap-3 cursor-pointer group"
            >
              <input type="radio" v-model="selectedDate" :value="date.id" @change="performSearch" class="w-4 h-4 border-gray-300 text-teal-500 focus:ring-teal-500">
              <span class="text-sm text-gray-700 group-hover:text-gray-900">{{ date.label }}</span>
            </label>
          </div>
        </div>

        <!-- Author -->
        <div v-if="topAuthors.length > 0" class="p-4 border-b border-gray-100">
          <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3">{{ textConstants.author }}</h4>
          <div class="space-y-2">
            <label
              v-for="author in topAuthors"
              :key="author.id"
              class="flex items-center gap-2 cursor-pointer p-1.5 rounded-lg hover:bg-gray-50 group"
            >
              <input type="checkbox" v-model="author.checked" @change="performSearch" class="w-4 h-4 rounded border-gray-300 text-teal-500 focus:ring-teal-500">
              <div
                class="w-6 h-6 rounded-full flex items-center justify-center text-white text-xs font-medium"
                :style="{ backgroundColor: author.color }"
              >
                {{ author.initials }}
              </div>
              <span class="text-sm text-gray-700 group-hover:text-gray-900">{{ author.name }}</span>
            </label>
          </div>
        </div>

        <!-- Tags -->
        <div v-if="popularTags.length > 0" class="p-4">
          <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3">{{ textConstants.tags }}</h4>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="tag in popularTags"
              :key="tag.id"
              @click="toggleTag(tag)"
              :class="[
                'px-2.5 py-1 rounded-full text-xs font-medium transition-all',
                tag.active
                  ? 'bg-teal-500 text-white'
                  : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
              ]"
            >
              {{ tag.name }}
            </button>
          </div>
        </div>
      </div>

      <!-- Collapsed state icons -->
      <div v-if="isFilterSidebarCollapsed" class="flex-1 flex flex-col items-center pt-4 gap-3">
        <button
          v-for="type in contentTypes.slice(0, 4)"
          :key="type.id"
          class="w-10 h-10 rounded-lg flex items-center justify-center text-gray-500 hover:bg-gray-100 transition-colors"
          :class="type.checked ? 'bg-teal-50 text-teal-600' : ''"
          :title="type.label"
        >
          <i :class="[
            type.id === 'articles' ? 'fas fa-file-alt' :
            type.id === 'documents' ? 'fas fa-file-pdf' :
            type.id === 'videos' ? 'fas fa-video' :
            'fas fa-graduation-cap'
          ]"></i>
        </button>
      </div>
    </aside>

    <!-- Main Content -->
    <div class="flex-1 flex flex-col h-full overflow-hidden">
      <!-- Search Header -->
      <div class="flex-shrink-0 p-4 border-b border-gray-200 bg-white">
        <div class="max-w-4xl mx-auto">
          <!-- Search Input -->
          <div class="relative">
            <i class="fas fa-search absolute left-4 top-1/2 -translate-y-1/2 text-gray-400"></i>
            <input
              type="text"
              v-model="searchQuery"
              @keydown.enter="performSearch"
              :placeholder="textConstants.searchPlaceholder"
              class="w-full pl-12 pr-32 py-3.5 bg-gray-50 border border-gray-200 rounded-xl text-gray-800 placeholder-gray-400 focus:outline-none focus:border-teal-400 focus:ring-2 focus:ring-teal-500/20 transition-all"
            >
            <div class="absolute right-2 top-1/2 -translate-y-1/2 flex items-center gap-1">
              <AIVoiceInput @transcript="handleVoiceTranscript" class="inline-flex" />
              <button
                @click="performSearch"
                class="px-4 py-2 bg-teal-500 hover:bg-teal-600 text-white rounded-lg font-medium transition-colors"
              >
                {{ textConstants.searchButton }}
              </button>
            </div>
          </div>

          <!-- Quick Filters -->
          <div class="flex flex-wrap items-center gap-2 mt-3">
            <span class="text-sm text-gray-500">{{ textConstants.quickFiltersLabel }}</span>
            <button
              v-for="filter in quickFilters"
              :key="filter.id"
              @click="toggleQuickFilter(filter)"
              :class="[
                'px-3 py-1.5 rounded-full text-sm font-medium transition-all',
                filter.active
                  ? 'bg-teal-500 text-white'
                  : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
              ]"
            >
              <i :class="[filter.icon, 'mr-1.5']"></i>{{ filter.label }}
            </button>
          </div>
        </div>
      </div>

      <!-- Results Area -->
      <div class="flex-1 overflow-y-auto p-6">
        <div class="max-w-4xl mx-auto">
          <!-- Loading State -->
          <div v-if="isLoading" class="flex items-center justify-center py-20">
            <div class="text-center">
              <div class="w-12 h-12 border-4 border-teal-200 border-t-teal-500 rounded-full animate-spin mx-auto mb-4"></div>
              <p class="text-gray-500">{{ textConstants.searching }}</p>
            </div>
          </div>

          <!-- Results -->
          <template v-else-if="results.length > 0">
            <!-- Results Header -->
            <div class="flex items-center justify-between mb-4">
              <div>
                <div class="flex items-center gap-3 flex-wrap">
                  <p class="text-gray-600">
                    <span class="font-semibold text-gray-900">{{ totalResults }}</span> {{ textConstants.resultsFor }}
                    "<span class="font-medium text-teal-700">{{ displayQuery }}</span>"
                  </p>
                  <span
                    v-if="queryIntent"
                    :class="['px-2.5 py-1 rounded-full text-xs font-medium flex items-center gap-1.5', getIntentTypeColor(queryIntent.type)]"
                  >
                    <i class="fas fa-brain"></i>
                    {{ getIntentLabel(queryIntent.type) }}
                    <span class="opacity-75">({{ Math.round(queryIntent.confidence * 100) }}%)</span>
                  </span>
                </div>
                <p class="text-sm text-gray-400 mt-0.5">
                  {{ textConstants.searchCompletedIn }} {{ searchTime }}{{ textConstants.ms }}
                  <span v-if="isAnalyzingQuery" class="ml-2 text-teal-600">
                    <i class="fas fa-circle-notch fa-spin"></i> {{ textConstants.aiAnalyzing }}
                  </span>
                </p>
              </div>
              <div class="flex items-center gap-3">
                <select v-model="sortBy" class="px-3 py-2 bg-white border border-gray-200 rounded-lg text-sm focus:outline-none focus:border-teal-400">
                  <option value="relevance">{{ textConstants.mostRelevant }}</option>
                  <option value="date">{{ textConstants.mostRecent }}</option>
                  <option value="views">{{ textConstants.mostViewed }}</option>
                </select>
                <div class="flex border border-gray-200 rounded-lg overflow-hidden">
                  <button
                    @click="viewMode = 'list'"
                    :class="['p-2', viewMode === 'list' ? 'bg-teal-500 text-white' : 'bg-white text-gray-600 hover:bg-gray-50']"
                  >
                    <i class="fas fa-list"></i>
                  </button>
                  <button
                    @click="viewMode = 'grid'"
                    :class="['p-2', viewMode === 'grid' ? 'bg-teal-500 text-white' : 'bg-white text-gray-600 hover:bg-gray-50']"
                  >
                    <i class="fas fa-th"></i>
                  </button>
                </div>
              </div>
            </div>

            <!-- Did You Mean -->
            <div v-if="showDidYouMean && didYouMeanSuggestions.length > 0" class="mb-4 p-3 bg-amber-50 rounded-xl border border-amber-200">
              <div class="flex items-center gap-2 mb-2">
                <i class="fas fa-lightbulb text-amber-500"></i>
                <span class="text-sm font-medium text-amber-700">{{ textConstants.didYouMean }}</span>
              </div>
              <div class="flex flex-wrap gap-2">
                <button
                  v-for="suggestion in didYouMeanSuggestions"
                  :key="suggestion.suggestion"
                  @click="applyDidYouMean(suggestion.suggestion)"
                  class="px-3 py-1.5 bg-white border border-amber-300 rounded-lg text-sm text-amber-800 hover:bg-amber-100 transition-colors"
                >
                  <span class="font-medium">{{ suggestion.suggestion }}</span>
                  <span class="text-xs text-amber-600 ml-1">({{ suggestion.reason }})</span>
                </button>
              </div>
            </div>

            <!-- Extracted Entities -->
            <div v-if="extractedEntities.length > 0" class="mb-4">
              <div class="flex items-center gap-2 mb-2">
                <i class="fas fa-wand-magic-sparkles text-teal-500"></i>
                <span class="text-sm font-medium text-gray-700">{{ textConstants.aiDetectedEntities }}</span>
              </div>
              <div class="flex flex-wrap gap-2">
                <button
                  v-for="entity in extractedEntities"
                  :key="entity.text"
                  @click="applyEntityFilter(entity)"
                  :class="['px-3 py-1.5 rounded-lg text-sm font-medium transition-all flex items-center gap-2 hover:shadow-md', getEntityTypeColor(entity.type)]"
                >
                  <i :class="getEntityTypeIcon(entity.type)"></i>
                  <span>{{ entity.text }}</span>
                  <span class="text-xs opacity-75">{{ Math.round(entity.confidence * 100) }}%</span>
                </button>
              </div>
            </div>

            <!-- AI Summary -->
            <div v-if="aiSummary" class="mb-6 p-5 bg-white rounded-2xl border border-gray-200 shadow-sm">
              <div class="flex items-start gap-4">
                <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center flex-shrink-0">
                  <i class="fas fa-robot text-white"></i>
                </div>
                <div class="flex-1">
                  <h3 class="font-semibold text-gray-900 mb-2">{{ textConstants.aiSummary }}</h3>
                  <p class="text-gray-600 text-sm leading-relaxed">{{ aiSummary }}</p>
                  <div class="flex items-center gap-4 mt-3">
                    <button class="text-sm text-teal-600 hover:text-teal-800">
                      <i class="fas fa-expand-alt mr-1"></i>{{ textConstants.showMore }}
                    </button>
                    <button @click="copyAISummary" class="text-sm text-gray-500 hover:text-gray-700">
                      <i class="fas fa-copy mr-1"></i>{{ textConstants.copy }}
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- AI Search Insights -->
            <div v-if="showAIPanel && aiSearchInsights.length > 0" class="mb-6 p-4 bg-white rounded-2xl border border-gray-200 shadow-sm">
              <div class="flex items-center justify-between mb-3">
                <div class="flex items-center gap-2">
                  <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-teal-400 to-emerald-500 flex items-center justify-center">
                    <i class="fas fa-wand-magic-sparkles text-white text-sm"></i>
                  </div>
                  <h4 class="font-semibold text-gray-900">{{ textConstants.aiSearchInsights }}</h4>
                </div>
                <button @click="showAIPanel = false" class="p-1.5 hover:bg-gray-100 rounded-lg transition-colors">
                  <i class="fas fa-times text-gray-400 text-sm"></i>
                </button>
              </div>
              <div class="space-y-2">
                <button
                  v-for="insight in aiSearchInsights"
                  :key="insight.id"
                  @click="applySearchInsight(insight)"
                  :class="[
                    'w-full p-3 rounded-xl text-left transition-all flex items-start gap-3 group',
                    insight.type === 'tip' ? 'bg-amber-50 hover:bg-amber-100' :
                    insight.type === 'related' ? 'bg-blue-50 hover:bg-blue-100' : 'bg-purple-50 hover:bg-purple-100'
                  ]"
                >
                  <div :class="[
                    'w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0',
                    insight.type === 'tip' ? 'bg-amber-200' :
                    insight.type === 'related' ? 'bg-blue-200' : 'bg-purple-200'
                  ]">
                    <i :class="[
                      'text-sm',
                      insight.type === 'tip' ? 'fas fa-lightbulb text-amber-700' :
                      insight.type === 'related' ? 'fas fa-search-plus text-blue-700' : 'fas fa-filter text-purple-700'
                    ]"></i>
                  </div>
                  <div class="flex-1">
                    <p :class="[
                      'text-sm font-medium',
                      insight.type === 'tip' ? 'text-amber-800' :
                      insight.type === 'related' ? 'text-blue-800' : 'text-purple-800'
                    ]">
                      {{ insight.text }}
                    </p>
                    <span v-if="insight.action" :class="[
                      'text-xs mt-0.5 inline-block',
                      insight.type === 'tip' ? 'text-amber-600' :
                      insight.type === 'related' ? 'text-blue-600' : 'text-purple-600'
                    ]">
                      {{ textConstants.clickToApply }} →
                    </span>
                  </div>
                </button>
              </div>
            </div>

            <!-- Results List -->
            <div :class="viewMode === 'grid' ? 'grid grid-cols-1 md:grid-cols-2 gap-4' : 'space-y-4'">
              <div
                v-for="result in results"
                :key="result.id"
                class="bg-white rounded-xl p-5 border border-gray-200 hover:border-teal-300 hover:shadow-lg transition-all cursor-pointer group"
              >
                <div class="flex items-start gap-4">
                  <div :class="['w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0', result.iconBg]">
                    <i :class="[result.icon, result.iconColor, 'text-xl']"></i>
                  </div>
                  <div class="flex-1 min-w-0">
                    <div class="flex items-center gap-2 mb-1">
                      <span :class="['px-2 py-0.5 rounded text-xs font-medium', result.typeBadge]">{{ result.type }}</span>
                      <span v-if="result.isNew" class="px-2 py-0.5 rounded text-xs font-medium bg-green-100 text-green-700">{{ textConstants.newBadge }}</span>
                    </div>
                    <h3 class="font-semibold text-gray-900 group-hover:text-teal-700 mb-2" v-html="highlightText(result.title)"></h3>
                    <p class="text-sm text-gray-600 line-clamp-2" v-html="highlightText(result.excerpt)"></p>

                    <div class="flex items-center gap-4 mt-3">
                      <div class="flex items-center gap-2">
                        <div
                          class="w-6 h-6 rounded-full flex items-center justify-center text-white text-xs font-medium"
                          :style="{ backgroundColor: result.author.color }"
                        >
                          {{ result.author.initials }}
                        </div>
                        <span class="text-sm text-gray-500">{{ result.author.name }}</span>
                      </div>
                      <span class="text-sm text-gray-400">{{ result.date }}</span>
                      <span class="text-sm text-gray-400">
                        <i class="fas fa-eye mr-1"></i>{{ result.views.toLocaleString() }}
                      </span>
                    </div>

                    <!-- Tags -->
                    <div class="flex flex-wrap gap-2 mt-3">
                      <span
                        v-for="tag in result.tags"
                        :key="tag"
                        class="px-2 py-0.5 bg-gray-100 rounded-full text-xs text-gray-600"
                      >
                        {{ tag }}
                      </span>
                    </div>
                  </div>

                  <div class="flex flex-col gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button class="p-2 rounded-lg hover:bg-teal-100 text-gray-400 hover:text-teal-600" :title="textConstants.bookmark">
                      <i class="fas fa-bookmark"></i>
                    </button>
                    <button class="p-2 rounded-lg hover:bg-teal-100 text-gray-400 hover:text-teal-600" :title="textConstants.share">
                      <i class="fas fa-share-alt"></i>
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Pagination -->
            <Pagination
              v-if="totalPages > 1"
              v-model:current-page="currentPage"
              v-model:items-per-page="pageSize"
              :total-items="totalResults"
              :show-per-page-selector="false"
              class="mt-8"
            />

            <!-- Related Searches -->
            <div v-if="relatedSearches.length > 0" class="mt-8 p-5 bg-white rounded-2xl border border-gray-200 shadow-sm">
              <div class="flex items-center gap-2 mb-4">
                <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-teal-400 to-emerald-500 flex items-center justify-center">
                  <i class="fas fa-wand-magic-sparkles text-white text-sm"></i>
                </div>
                <h4 class="font-semibold text-gray-900">{{ textConstants.aiRelatedSearches }}</h4>
              </div>
              <div class="flex flex-wrap gap-2">
                <button
                  v-for="search in relatedSearches"
                  :key="search"
                  @click="searchQuery = search; performSearch()"
                  class="px-4 py-2 bg-gradient-to-r from-teal-50 to-emerald-50 border border-teal-200 rounded-xl text-sm font-medium text-teal-700 hover:from-teal-100 hover:to-emerald-100 hover:border-teal-300 transition-all flex items-center gap-2"
                >
                  <i class="fas fa-search text-teal-500 text-xs"></i>
                  {{ search }}
                </button>
              </div>
              <p class="text-xs text-gray-500 mt-3">
                <i class="fas fa-info-circle mr-1"></i>
                {{ textConstants.basedOnPatterns }}
              </p>
            </div>
          </template>

          <!-- Empty State -->
          <EmptyState
            v-else-if="displayQuery"
            icon="fas fa-search"
            :title="textConstants.noResults"
            :description="textConstants.noResultsDesc"
            size="lg"
          />

          <!-- Initial State -->
          <EmptyState
            v-else
            icon="fas fa-search"
            :title="textConstants.initialStateTitle"
            :description="textConstants.initialStateDesc"
            size="lg"
          />
        </div>
      </div>
    </div>

    <!-- Mobile Filter Toggle -->
    <button
      @click="showMobileFilters = !showMobileFilters"
      class="fixed bottom-6 right-6 lg:hidden w-14 h-14 bg-teal-500 text-white rounded-full shadow-lg flex items-center justify-center z-50"
    >
      <i class="fas fa-filter"></i>
      <span v-if="activeFiltersCount > 0" class="absolute -top-1 -right-1 w-5 h-5 bg-red-500 text-white text-xs rounded-full flex items-center justify-center">
        {{ activeFiltersCount }}
      </span>
    </button>
  </div>
</template>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Badge styles */
.badge-blue {
  @apply bg-blue-100 text-blue-700;
}

.badge-purple {
  @apply bg-purple-100 text-purple-700;
}

.badge-red {
  @apply bg-red-100 text-red-700;
}

.badge-green {
  @apply bg-green-100 text-green-700;
}

/* Custom scrollbar */
.overflow-y-auto::-webkit-scrollbar {
  width: 6px;
}

.overflow-y-auto::-webkit-scrollbar-track {
  background: transparent;
}

.overflow-y-auto::-webkit-scrollbar-thumb {
  background: #e5e7eb;
  border-radius: 3px;
}

.overflow-y-auto::-webkit-scrollbar-thumb:hover {
  background: #d1d5db;
}
</style>
