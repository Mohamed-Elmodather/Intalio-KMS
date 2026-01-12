<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import { useAIServicesStore } from '@/stores/aiServices'
import { AILoadingIndicator, AISuggestionChip, AIConfidenceBar } from '@/components/ai'

// Initialize AI store
const aiStore = useAIServicesStore()

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

const route = useRoute()
const router = useRouter()

// Loading state
const isLoading = ref(false)

// Search state
const searchQuery = ref('employee onboarding')
const displayQuery = ref('employee onboarding')
const viewMode = ref<'list' | 'grid'>('list')
const sortBy = ref('relevance')
const selectedDate = ref('all')
const authorFilter = ref('')
const totalResults = ref(127)
const searchTime = ref(42)
const currentPage = ref(1)

// AI Summary
const aiSummary = ref('Based on your search for "employee onboarding", I found comprehensive resources including the official Onboarding Guide, training modules for new hires, and HR policy documents. The most relevant content covers the 30-60-90 day onboarding process, required documentation, and team integration best practices.')

// Quick Filters
const quickFilters = ref<QuickFilter[]>([
  { id: 'articles', label: 'Articles', icon: 'fas fa-file-alt', active: true },
  { id: 'documents', label: 'Documents', icon: 'fas fa-file-pdf', active: false },
  { id: 'videos', label: 'Videos', icon: 'fas fa-video', active: false },
  { id: 'courses', label: 'Courses', icon: 'fas fa-graduation-cap', active: false }
])

// Content Types
const contentTypes = ref<ContentType[]>([
  { id: 'articles', label: 'Articles', count: 45, checked: true },
  { id: 'documents', label: 'Documents', count: 32, checked: true },
  { id: 'videos', label: 'Videos', count: 18, checked: true },
  { id: 'courses', label: 'Courses', count: 12, checked: false },
  { id: 'events', label: 'Events', count: 8, checked: false },
  { id: 'polls', label: 'Polls', count: 5, checked: false }
])

// Date Filters
const dateFilters = ref<DateFilter[]>([
  { id: 'all', label: 'Any time' },
  { id: 'today', label: 'Today' },
  { id: 'week', label: 'Past week' },
  { id: 'month', label: 'Past month' },
  { id: 'year', label: 'Past year' }
])

// Top Authors
const topAuthors = ref<Author[]>([
  { id: 1, name: 'Sarah Chen', initials: 'SC', color: '#8B5CF6', checked: false },
  { id: 2, name: 'Mike Johnson', initials: 'MJ', color: '#3B82F6', checked: false },
  { id: 3, name: 'HR Team', initials: 'HR', color: '#10B981', checked: true }
])

// Popular Tags
const popularTags = ref<Tag[]>([
  { id: 1, name: 'Onboarding', active: true },
  { id: 2, name: 'HR', active: false },
  { id: 3, name: 'Training', active: false },
  { id: 4, name: 'Policy', active: false },
  { id: 5, name: 'New Hire', active: true },
  { id: 6, name: 'Guide', active: false }
])

// Search Results
const results = ref<SearchResult[]>([
  {
    id: 1,
    type: 'Article',
    typeBadge: 'badge-blue',
    icon: 'fas fa-file-alt',
    iconBg: 'bg-blue-100',
    iconColor: 'text-blue-600',
    title: 'Complete Employee Onboarding Guide 2024',
    excerpt: 'A comprehensive guide to employee onboarding best practices, including the 30-60-90 day plan, documentation requirements, and team integration strategies...',
    author: { name: 'HR Team', initials: 'HR', color: '#10B981' },
    date: 'Dec 18, 2024',
    views: 1247,
    tags: ['Onboarding', 'HR', 'Guide'],
    isNew: true
  },
  {
    id: 2,
    type: 'Document',
    typeBadge: 'badge-purple',
    icon: 'fas fa-file-pdf',
    iconBg: 'bg-purple-100',
    iconColor: 'text-purple-600',
    title: 'New Employee Onboarding Checklist',
    excerpt: 'Essential checklist for managers and HR to ensure smooth onboarding of new team members. Includes pre-boarding, first day, and first week tasks...',
    author: { name: 'Sarah Chen', initials: 'SC', color: '#8B5CF6' },
    date: 'Dec 15, 2024',
    views: 856,
    tags: ['Checklist', 'Onboarding', 'Manager'],
    isNew: false
  },
  {
    id: 3,
    type: 'Video',
    typeBadge: 'badge-red',
    icon: 'fas fa-video',
    iconBg: 'bg-red-100',
    iconColor: 'text-red-600',
    title: 'Welcome to the Team: Onboarding Video Series',
    excerpt: 'Video introduction to company culture, values, and expectations for new employees. Part of the mandatory onboarding training program...',
    author: { name: 'Mike Johnson', initials: 'MJ', color: '#3B82F6' },
    date: 'Dec 10, 2024',
    views: 2341,
    tags: ['Video', 'Training', 'Culture'],
    isNew: false
  },
  {
    id: 4,
    type: 'Course',
    typeBadge: 'badge-green',
    icon: 'fas fa-graduation-cap',
    iconBg: 'bg-green-100',
    iconColor: 'text-green-600',
    title: 'New Hire Orientation Program',
    excerpt: 'Complete orientation program covering company policies, benefits, IT systems, and compliance training for all new employees...',
    author: { name: 'HR Team', initials: 'HR', color: '#10B981' },
    date: 'Dec 5, 2024',
    views: 1893,
    tags: ['Course', 'Orientation', 'Compliance'],
    isNew: false
  },
  {
    id: 5,
    type: 'Article',
    typeBadge: 'badge-blue',
    icon: 'fas fa-file-alt',
    iconBg: 'bg-blue-100',
    iconColor: 'text-blue-600',
    title: 'Remote Employee Onboarding Best Practices',
    excerpt: 'Special considerations and best practices for onboarding remote employees, including virtual introductions, equipment setup, and communication tools...',
    author: { name: 'Sarah Chen', initials: 'SC', color: '#8B5CF6' },
    date: 'Nov 28, 2024',
    views: 672,
    tags: ['Remote', 'Onboarding', 'Virtual'],
    isNew: false
  }
])

// Methods
function toggleQuickFilter(filter: QuickFilter): void {
  filter.active = !filter.active
}

function clearFilters(): void {
  contentTypes.value.forEach(t => t.checked = true)
  topAuthors.value.forEach(a => a.checked = false)
  popularTags.value.forEach(t => t.active = false)
  selectedDate.value = 'all'
}

function performSearch(): void {
  if (searchQuery.value.trim()) {
    isLoading.value = true
    displayQuery.value = searchQuery.value
    // Simulate search delay
    setTimeout(() => {
      isLoading.value = false
    }, 500)
  }
}

function highlightText(text: string): string {
  const query = displayQuery.value.toLowerCase()
  const words = query.split(' ').filter(w => w.length > 2)
  let result = text
  words.forEach(word => {
    const regex = new RegExp(`(${word})`, 'gi')
    result = result.replace(regex, '<mark class="bg-yellow-200 text-teal-900 rounded px-0.5">$1</mark>')
  })
  return result
}

function toggleTag(tag: Tag): void {
  tag.active = !tag.active
}

// ============================================================================
// AI Features State & Functions
// ============================================================================

// AI State
const showAIPanel = ref(true)
const isAnalyzingQuery = ref(false)
const showEntityFilter = ref(false)
const showDidYouMean = ref(true)

// AI Interfaces
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

// AI Data
const queryIntent = ref<QueryIntent>({
  type: 'informational',
  confidence: 0.92,
  description: 'Looking for learning resources and guides'
})

const extractedEntities = ref<ExtractedEntity[]>([
  { text: 'Employee', type: 'topic', confidence: 0.95 },
  { text: 'Onboarding', type: 'topic', confidence: 0.98 },
  { text: 'HR Team', type: 'organization', confidence: 0.85 }
])

const didYouMeanSuggestions = ref<DidYouMeanSuggestion[]>([
  { original: 'employee onboarding', suggestion: 'new employee onboarding process', reason: 'More specific results' },
  { original: 'employee onboarding', suggestion: 'employee onboarding checklist', reason: 'Popular search' }
])

const aiSearchInsights = ref<AISearchInsight[]>([
  { id: '1', type: 'tip', text: 'Try adding "2024" for the latest content', action: 'employee onboarding 2024' },
  { id: '2', type: 'related', text: 'Users also searched for: "new hire training"', action: 'new hire training' },
  { id: '3', type: 'refine', text: 'Filter by HR Team for official policies', action: 'filter:author:HR Team' }
])

const relatedSearches = ref([
  'new hire orientation',
  'onboarding checklist template',
  'first day employee guide',
  '30-60-90 day plan',
  'employee handbook'
])

// AI Functions
async function analyzeSearchQuery(query: string) {
  isAnalyzingQuery.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 500))

    // Simulate AI analysis
    if (query.toLowerCase().includes('onboarding')) {
      queryIntent.value = {
        type: 'informational',
        confidence: 0.92,
        description: 'Looking for learning resources and guides'
      }
      extractedEntities.value = [
        { text: 'Employee', type: 'topic', confidence: 0.95 },
        { text: 'Onboarding', type: 'topic', confidence: 0.98 }
      ]
    } else if (query.toLowerCase().includes('policy')) {
      queryIntent.value = {
        type: 'navigational',
        confidence: 0.88,
        description: 'Looking for specific policy documents'
      }
    } else {
      queryIntent.value = {
        type: 'informational',
        confidence: 0.75,
        description: 'General information search'
      }
    }
  } catch (error) {
    console.error('Query analysis failed:', error)
  } finally {
    isAnalyzingQuery.value = false
  }
}

function applyDidYouMean(suggestion: string) {
  searchQuery.value = suggestion
  performSearch()
  showDidYouMean.value = false
}

function applyEntityFilter(entity: ExtractedEntity) {
  // Add entity to search query or filter
  const filterQuery = `${searchQuery.value} ${entity.text}`
  searchQuery.value = filterQuery
  performSearch()
}

function applySearchInsight(insight: AISearchInsight) {
  if (insight.action) {
    if (insight.action.startsWith('filter:')) {
      // Handle filter action
      const [, filterType, filterValue] = insight.action.split(':')
      console.log('Apply filter:', filterType, filterValue)
    } else {
      searchQuery.value = insight.action
      performSearch()
    }
  }
}

function getEntityTypeColor(type: string) {
  switch (type) {
    case 'person': return 'bg-blue-100 text-blue-700'
    case 'organization': return 'bg-purple-100 text-purple-700'
    case 'topic': return 'bg-teal-100 text-teal-700'
    case 'date': return 'bg-amber-100 text-amber-700'
    case 'location': return 'bg-green-100 text-green-700'
    default: return 'bg-gray-100 text-gray-700'
  }
}

function getEntityTypeIcon(type: string) {
  switch (type) {
    case 'person': return 'fas fa-user'
    case 'organization': return 'fas fa-building'
    case 'topic': return 'fas fa-tag'
    case 'date': return 'fas fa-calendar'
    case 'location': return 'fas fa-map-marker-alt'
    default: return 'fas fa-circle'
  }
}

function getIntentTypeColor(type: string) {
  switch (type) {
    case 'informational': return 'bg-blue-100 text-blue-700'
    case 'navigational': return 'bg-purple-100 text-purple-700'
    case 'transactional': return 'bg-green-100 text-green-700'
    default: return 'bg-gray-100 text-gray-700'
  }
}

// Enhanced search with AI
function performAISearch(): void {
  if (searchQuery.value.trim()) {
    isLoading.value = true
    displayQuery.value = searchQuery.value
    analyzeSearchQuery(searchQuery.value)
    showDidYouMean.value = true

    setTimeout(() => {
      isLoading.value = false
    }, 800)
  }
}
</script>

<template>
  <div class="space-y-6">
    <!-- Loading Overlay -->
    <div v-if="isLoading" class="fixed inset-0 bg-white/50 backdrop-blur-sm z-50 flex items-center justify-center">
      <LoadingSpinner size="lg" text="Searching..." />
    </div>

    <!-- Search Header -->
    <div class="card-animated fade-in-up rounded-2xl p-6 mb-10" style="animation-delay: 0.1s;">
      <div class="max-w-3xl mx-auto">
        <div class="input-group">
          <i class="input-icon icon-vibrant fas fa-search text-lg"></i>
          <input
            type="text"
            v-model="searchQuery"
            @keydown.enter="performSearch"
            placeholder="Search across all content..."
            class="input text-lg py-4 pl-12"
          >
          <button
            @click="performSearch"
            class="absolute right-2 top-1/2 -translate-y-1/2 btn-vibrant ripple"
          >
            Search
          </button>
        </div>
        <div class="flex flex-wrap items-center gap-2 mt-4">
          <span class="text-sm text-teal-500">Quick filters:</span>
          <button
            v-for="filter in quickFilters"
            :key="filter.id"
            @click="toggleQuickFilter(filter)"
            :class="['px-3 py-1.5 rounded-full text-sm transition-all ripple',
                     filter.active ? 'bg-teal-500 text-white' : 'bg-teal-50 text-teal-700 hover:bg-teal-100']"
          >
            <i :class="[filter.icon, 'icon-soft mr-1.5']"></i>{{ filter.label }}
          </button>
        </div>

        <!-- AI: Did You Mean Suggestions -->
        <div v-if="showDidYouMean && didYouMeanSuggestions.length > 0" class="mt-4 p-3 bg-amber-50 rounded-xl border border-amber-200">
          <div class="flex items-center gap-2 mb-2">
            <i class="fas fa-lightbulb text-amber-500"></i>
            <span class="text-sm font-medium text-amber-700">Did you mean:</span>
          </div>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="suggestion in didYouMeanSuggestions"
              :key="suggestion.suggestion"
              @click="applyDidYouMean(suggestion.suggestion)"
              class="px-3 py-1.5 bg-white border border-amber-300 rounded-lg text-sm text-amber-800 hover:bg-amber-100 transition-colors flex items-center gap-2"
            >
              <span class="font-medium">{{ suggestion.suggestion }}</span>
              <span class="text-xs text-amber-600">({{ suggestion.reason }})</span>
            </button>
          </div>
        </div>

        <!-- AI: Extracted Entities Filter -->
        <div v-if="extractedEntities.length > 0" class="mt-4">
          <div class="flex items-center gap-2 mb-2">
            <i class="fas fa-wand-magic-sparkles text-teal-500"></i>
            <span class="text-sm font-medium text-gray-700">AI Detected Entities:</span>
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
      </div>
    </div>

    <div class="flex gap-6">
      <!-- Filters Sidebar -->
      <aside class="w-64 flex-shrink-0 hidden lg:block">
        <div class="card-animated fade-in-up rounded-2xl p-5 sticky top-24" style="animation-delay: 0.2s;">
          <div class="flex items-center justify-between mb-4">
            <h3 class="font-semibold text-teal-900">Filters</h3>
            <button @click="clearFilters" class="text-sm text-teal-500 hover:text-teal-700 ripple">Clear all</button>
          </div>

          <!-- Content Type -->
          <div class="mb-6">
            <h4 class="text-sm font-medium text-teal-700 mb-3">Content Type</h4>
            <div class="space-y-2">
              <label
                v-for="type in contentTypes"
                :key="type.id"
                class="flex items-center gap-3 cursor-pointer"
              >
                <input type="checkbox" v-model="type.checked" class="checkbox">
                <span class="text-sm text-teal-700">{{ type.label }}</span>
                <span class="ml-auto text-xs text-teal-400">({{ type.count }})</span>
              </label>
            </div>
          </div>

          <!-- Date Range -->
          <div class="mb-6">
            <h4 class="text-sm font-medium text-teal-700 mb-3">Date</h4>
            <div class="space-y-2">
              <label
                v-for="date in dateFilters"
                :key="date.id"
                class="flex items-center gap-3 cursor-pointer"
              >
                <input type="radio" v-model="selectedDate" :value="date.id" class="radio">
                <span class="text-sm text-teal-700">{{ date.label }}</span>
              </label>
            </div>
          </div>

          <!-- Author -->
          <div class="mb-6">
            <h4 class="text-sm font-medium text-teal-700 mb-3">Author</h4>
            <input
              type="text"
              v-model="authorFilter"
              placeholder="Filter by author..."
              class="input text-sm"
            >
            <div class="mt-2 space-y-1">
              <label
                v-for="author in topAuthors"
                :key="author.id"
                class="flex items-center gap-2 cursor-pointer p-1.5 rounded-lg hover:bg-teal-50"
              >
                <input type="checkbox" v-model="author.checked" class="checkbox">
                <div
                  class="w-6 h-6 rounded-full flex items-center justify-center text-white text-xs"
                  :style="{ backgroundColor: author.color }"
                >
                  {{ author.initials }}
                </div>
                <span class="text-sm text-teal-700">{{ author.name }}</span>
              </label>
            </div>
          </div>

          <!-- Tags -->
          <div>
            <h4 class="text-sm font-medium text-teal-700 mb-3">Tags</h4>
            <div class="flex flex-wrap gap-2">
              <button
                v-for="tag in popularTags"
                :key="tag.id"
                @click="toggleTag(tag)"
                :class="['px-2.5 py-1 rounded-full text-xs transition-all ripple',
                         tag.active ? 'bg-teal-500 text-white' : 'bg-teal-50 text-teal-600 hover:bg-teal-100']"
              >
                {{ tag.name }}
              </button>
            </div>
          </div>
        </div>
      </aside>

      <!-- Results -->
      <div class="flex-1">
        <!-- Results Header -->
        <div class="flex items-center justify-between mb-4">
          <div>
            <div class="flex items-center gap-3 mb-1">
              <p class="text-teal-600">
                <span class="font-semibold text-teal-900">{{ totalResults }}</span> results for
                "<span class="font-medium text-teal-800">{{ displayQuery }}</span>"
              </p>
              <!-- AI Query Intent Badge -->
              <span v-if="queryIntent" :class="['px-2.5 py-1 rounded-full text-xs font-medium flex items-center gap-1.5', getIntentTypeColor(queryIntent.type)]">
                <i class="fas fa-brain"></i>
                {{ queryIntent.type === 'informational' ? 'Looking for info' : queryIntent.type === 'navigational' ? 'Finding specific' : 'Taking action' }}
                <span class="opacity-75">({{ Math.round(queryIntent.confidence * 100) }}%)</span>
              </span>
            </div>
            <p class="text-sm text-teal-500">
              Search completed in {{ searchTime }}ms
              <span v-if="isAnalyzingQuery" class="ml-2 text-teal-600">
                <i class="fas fa-circle-notch fa-spin"></i> AI analyzing...
              </span>
            </p>
          </div>
          <div class="flex items-center gap-3">
            <select v-model="sortBy" class="input text-sm py-2">
              <option value="relevance">Most Relevant</option>
              <option value="date">Most Recent</option>
              <option value="views">Most Viewed</option>
            </select>
            <div class="flex border border-teal-200 rounded-lg overflow-hidden">
              <button
                @click="viewMode = 'list'"
                :class="['p-2 ripple', viewMode === 'list' ? 'bg-teal-500 text-white' : 'bg-white text-teal-600 hover:bg-teal-50']"
              >
                <i class="fas fa-list icon-soft"></i>
              </button>
              <button
                @click="viewMode = 'grid'"
                :class="['p-2 ripple', viewMode === 'grid' ? 'bg-teal-500 text-white' : 'bg-white text-teal-600 hover:bg-teal-50']"
              >
                <i class="fas fa-th icon-soft"></i>
              </button>
            </div>
          </div>
        </div>

        <!-- AI Summary -->
        <div class="card-animated fade-in-up rounded-2xl p-5 mb-10 border-l-4 border-teal-500" style="animation-delay: 0.3s;">
          <div class="flex items-start gap-4">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center flex-shrink-0">
              <i class="fas fa-robot text-white icon-vibrant"></i>
            </div>
            <div class="flex-1">
              <h3 class="font-semibold text-teal-900 mb-2">AI Summary</h3>
              <p class="text-teal-700 text-sm leading-relaxed">{{ aiSummary }}</p>
              <div class="flex items-center gap-4 mt-3">
                <button class="text-sm text-teal-600 hover:text-teal-800 ripple">
                  <i class="fas fa-expand-alt mr-1 icon-soft"></i>Show more
                </button>
                <button class="text-sm text-teal-500 hover:text-teal-700 ripple">
                  <i class="fas fa-copy mr-1 icon-soft"></i>Copy
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- AI Search Insights Panel -->
        <div v-if="showAIPanel && aiSearchInsights.length > 0" class="card-animated fade-in-up rounded-2xl p-5 mb-6" style="animation-delay: 0.35s;">
          <div class="flex items-center justify-between mb-3">
            <div class="flex items-center gap-2">
              <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-teal-400 to-emerald-500 flex items-center justify-center">
                <i class="fas fa-wand-magic-sparkles text-white text-sm"></i>
              </div>
              <h4 class="font-semibold text-gray-900">AI Search Insights</h4>
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
              :class="['w-full p-3 rounded-xl text-left transition-all flex items-start gap-3 group',
                       insight.type === 'tip' ? 'bg-amber-50 hover:bg-amber-100' :
                       insight.type === 'related' ? 'bg-blue-50 hover:bg-blue-100' : 'bg-purple-50 hover:bg-purple-100']"
            >
              <div :class="['w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0',
                            insight.type === 'tip' ? 'bg-amber-200' :
                            insight.type === 'related' ? 'bg-blue-200' : 'bg-purple-200']">
                <i :class="['text-sm',
                            insight.type === 'tip' ? 'fas fa-lightbulb text-amber-700' :
                            insight.type === 'related' ? 'fas fa-search-plus text-blue-700' : 'fas fa-filter text-purple-700']"></i>
              </div>
              <div class="flex-1">
                <p :class="['text-sm font-medium',
                            insight.type === 'tip' ? 'text-amber-800' :
                            insight.type === 'related' ? 'text-blue-800' : 'text-purple-800']">
                  {{ insight.text }}
                </p>
                <span v-if="insight.action" :class="['text-xs mt-0.5 inline-block',
                              insight.type === 'tip' ? 'text-amber-600' :
                              insight.type === 'related' ? 'text-blue-600' : 'text-purple-600']">
                  Click to apply →
                </span>
              </div>
            </button>
          </div>
        </div>

        <!-- Results List -->
        <div :class="viewMode === 'grid' ? 'grid grid-cols-1 md:grid-cols-2 gap-4' : 'space-y-4'">
          <div
            v-for="(result, index) in results"
            :key="result.id"
            class="list-item-animated card-animated fade-in-up rounded-xl p-5 hover:shadow-lg transition-all cursor-pointer group ripple"
            :style="{ animationDelay: (0.4 + index * 0.1) + 's' }"
          >
            <div class="flex items-start gap-4">
              <div :class="['w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0', result.iconBg]">
                <i :class="[result.icon, result.iconColor, 'text-xl icon-vibrant']"></i>
              </div>
              <div class="flex-1 min-w-0">
                <div class="flex items-center gap-2 mb-1">
                  <span :class="['badge', result.typeBadge]">{{ result.type }}</span>
                  <span v-if="result.isNew" class="badge badge-green">New</span>
                </div>
                <h3
                  class="font-semibold text-teal-900 group-hover:text-teal-700 mb-2"
                  v-html="highlightText(result.title)"
                ></h3>
                <p
                  class="text-sm text-teal-600 line-clamp-2"
                  v-html="highlightText(result.excerpt)"
                ></p>

                <div class="flex items-center gap-4 mt-3">
                  <div class="flex items-center gap-2">
                    <div
                      class="w-6 h-6 rounded-full flex items-center justify-center text-white text-xs"
                      :style="{ backgroundColor: result.author.color }"
                    >
                      {{ result.author.initials }}
                    </div>
                    <span class="text-sm text-teal-500">{{ result.author.name }}</span>
                  </div>
                  <span class="text-sm text-teal-400">{{ result.date }}</span>
                  <span class="text-sm text-teal-400">
                    <i class="fas fa-eye mr-1 icon-soft"></i>{{ result.views }}
                  </span>
                </div>

                <!-- Tags -->
                <div class="flex flex-wrap gap-2 mt-3">
                  <span
                    v-for="tag in result.tags"
                    :key="tag"
                    class="px-2 py-0.5 bg-teal-50 rounded-full text-xs text-teal-600"
                  >
                    {{ tag }}
                  </span>
                </div>
              </div>

              <div class="flex flex-col gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                <button class="p-2 rounded-lg hover:bg-teal-100 text-teal-500 ripple" title="Bookmark">
                  <i class="fas fa-bookmark icon-soft"></i>
                </button>
                <button class="p-2 rounded-lg hover:bg-teal-100 text-teal-500 ripple" title="Share">
                  <i class="fas fa-share-alt icon-soft"></i>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Pagination -->
        <div class="flex items-center justify-between mt-8 fade-in-up" style="animation-delay: 0.9s;">
          <p class="text-sm text-teal-500">Showing 1-10 of {{ totalResults }} results</p>
          <div class="flex items-center gap-2">
            <button
              class="p-2 rounded-lg border border-teal-200 text-teal-500 hover:bg-teal-50 disabled:opacity-50 ripple"
              disabled
            >
              <i class="fas fa-chevron-left icon-soft"></i>
            </button>
            <button class="w-10 h-10 rounded-lg bg-teal-500 text-white font-medium ripple">1</button>
            <button class="w-10 h-10 rounded-lg border border-teal-200 text-teal-600 hover:bg-teal-50 font-medium ripple">2</button>
            <button class="w-10 h-10 rounded-lg border border-teal-200 text-teal-600 hover:bg-teal-50 font-medium ripple">3</button>
            <span class="text-teal-400">...</span>
            <button class="w-10 h-10 rounded-lg border border-teal-200 text-teal-600 hover:bg-teal-50 font-medium ripple">12</button>
            <button class="p-2 rounded-lg border border-teal-200 text-teal-600 hover:bg-teal-50 ripple">
              <i class="fas fa-chevron-right icon-soft"></i>
            </button>
          </div>
        </div>

        <!-- AI Related Searches -->
        <div class="mt-8 card-animated fade-in-up rounded-2xl p-5" style="animation-delay: 1s;">
          <div class="flex items-center gap-2 mb-4">
            <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-teal-400 to-emerald-500 flex items-center justify-center">
              <i class="fas fa-wand-magic-sparkles text-white text-sm"></i>
            </div>
            <h4 class="font-semibold text-gray-900">AI Related Searches</h4>
          </div>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="search in relatedSearches"
              :key="search"
              @click="searchQuery = search; performAISearch()"
              class="px-4 py-2 bg-gradient-to-r from-teal-50 to-emerald-50 border border-teal-200 rounded-xl text-sm font-medium text-teal-700 hover:from-teal-100 hover:to-emerald-100 hover:border-teal-300 transition-all flex items-center gap-2"
            >
              <i class="fas fa-search text-teal-500 text-xs"></i>
              {{ search }}
            </button>
          </div>
          <p class="text-xs text-gray-500 mt-3">
            <i class="fas fa-info-circle mr-1"></i>
            Based on your search patterns and popular queries
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Input group positioning */
.input-group {
  position: relative;
}

.input-group .input-icon {
  position: absolute;
  left: 1rem;
  top: 50%;
  transform: translateY(-50%);
  z-index: 10;
}

.input-group .input {
  width: 100%;
  padding-right: 6rem;
}

/* AI Feature Animations */
@keyframes ai-pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}

@keyframes ai-shimmer {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}

.ai-analyzing {
  animation: ai-pulse 1.5s ease-in-out infinite;
}

.ai-shimmer {
  background: linear-gradient(
    90deg,
    transparent 0%,
    rgba(20, 184, 166, 0.1) 50%,
    transparent 100%
  );
  background-size: 200% 100%;
  animation: ai-shimmer 2s infinite;
}

/* Entity chip hover effects */
.entity-chip:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.2);
}

/* AI insight card hover */
.ai-insight-card:hover {
  transform: translateX(4px);
}

/* Related search button hover */
.related-search-btn:hover {
  transform: scale(1.02);
}
</style>
