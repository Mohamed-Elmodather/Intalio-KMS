<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import { PageHeader, StatsBar, Avatar } from '@/components/base'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import type { StatItem } from '@/components/base/StatsBar.vue'
import ErrorState from '@/components/base/ErrorState.vue'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Checkbox from 'primevue/checkbox'
import Dropdown from 'primevue/dropdown'
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'

const { locale } = useI18n()
const route = useRoute()
const router = useRouter()
const prefersReducedMotion = useReducedMotion()

// RTL support
const isRTL = computed(() => locale.value === 'ar')

// Animation control
const isContentVisible = ref(false)
const shouldAnimate = computed(() => !prefersReducedMotion.value)
const LOADING_DELAY = 600

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: isRTL.value ? 'البحث' : 'Search' }
])

// Loading and error state
const loading = ref(true)
const error = ref<Error | null>(null)
const searchQuery = ref('')
const lastQuery = ref('')
const isSearching = ref(false)
const hasSearched = ref(false)

// Results
const results = ref<SearchResult[]>([])
const totalResults = ref(0)
const currentPage = ref(1)
const pageSize = ref(20)
const executionTimeMs = ref(0)

// Filters
const selectedContentTypes = ref<string[]>([])
const selectedDateFilter = ref<string>('all')
const sortBy = ref('relevance')
// Save search
const showSaveSearchDialog = ref(false)
const savedSearchName = ref('')
const enableNotifications = ref(false)
const notificationFrequency = ref('daily')
const isSaving = ref(false)

// Type configurations
const CONTENT_TYPE_CONFIG: Record<string, { icon: string; color: string; bgColor: string; label: string; labelAr: string }> = {
  Article: { icon: 'pi pi-file-edit', color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.1)', label: 'Article', labelAr: 'مقال' },
  News: { icon: 'pi pi-megaphone', color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'News', labelAr: 'أخبار' },
  Document: { icon: 'pi pi-file', color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', label: 'Document', labelAr: 'مستند' },
  Discussion: { icon: 'pi pi-comments', color: '#8b5cf6', bgColor: 'rgba(139, 92, 246, 0.1)', label: 'Discussion', labelAr: 'نقاش' },
  Community: { icon: 'pi pi-users', color: '#ec4899', bgColor: 'rgba(236, 72, 153, 0.1)', label: 'Community', labelAr: 'مجتمع' },
  MediaItem: { icon: 'pi pi-image', color: '#06b6d4', bgColor: 'rgba(6, 182, 212, 0.1)', label: 'Media', labelAr: 'وسائط' },
  User: { icon: 'pi pi-user', color: '#6366f1', bgColor: 'rgba(99, 102, 241, 0.1)', label: 'People', labelAr: 'أشخاص' },
  Event: { icon: 'pi pi-calendar', color: '#ef4444', bgColor: 'rgba(239, 68, 68, 0.1)', label: 'Event', labelAr: 'فعالية' },
  LessonLearned: { icon: 'pi pi-lightbulb', color: '#eab308', bgColor: 'rgba(234, 179, 8, 0.1)', label: 'Lesson Learned', labelAr: 'درس مستفاد' }
}

const FILE_TYPE_CONFIG: Record<string, { icon: string; color: string }> = {
  pdf: { icon: 'pi pi-file-pdf', color: '#ef4444' },
  doc: { icon: 'pi pi-file-word', color: '#3b82f6' },
  docx: { icon: 'pi pi-file-word', color: '#3b82f6' },
  xls: { icon: 'pi pi-file-excel', color: '#10b981' },
  xlsx: { icon: 'pi pi-file-excel', color: '#10b981' },
  ppt: { icon: 'pi pi-file', color: '#f59e0b' },
  pptx: { icon: 'pi pi-file', color: '#f59e0b' },
  jpg: { icon: 'pi pi-image', color: '#8b5cf6' },
  png: { icon: 'pi pi-image', color: '#8b5cf6' },
  mp4: { icon: 'pi pi-video', color: '#06b6d4' },
  mp3: { icon: 'pi pi-volume-up', color: '#ec4899' }
}

// Interfaces
interface SearchResult {
  id: string
  sourceId: string
  contentType: string
  title: string
  titleAr?: string
  summary?: string
  summaryAr?: string
  highlightedTitle?: string
  highlightedSummary?: string
  author?: string
  authorAvatar?: string
  category?: string
  categoryAr?: string
  tags?: string[]
  fileType?: string
  fileSize?: number
  thumbnailUrl?: string
  duration?: number
  viewCount: number
  publishedAt: Date
  relevanceScore: number
}

interface Facet {
  value: string
  label: string
  labelAr: string
  count: number
}

// Facets data
const facets = ref<{
  contentTypes: Facet[]
  categories: Facet[]
  fileTypes: Facet[]
}>({
  contentTypes: [],
  categories: [],
  fileTypes: []
})

// Stats
const statsBarItems = computed<StatItem[]>(() => [
  {
    icon: 'pi pi-search',
    value: totalResults.value,
    label: 'Results',
    labelArabic: 'نتيجة',
    colorClass: 'primary'
  },
  {
    icon: 'pi pi-folder',
    value: facets.value.contentTypes.length,
    label: 'Types',
    labelArabic: 'أنواع',
    colorClass: 'success'
  },
  {
    icon: 'pi pi-clock',
    value: executionTimeMs.value,
    label: 'ms',
    labelArabic: 'مللي ثانية',
    colorClass: 'warning'
  }
])

// Trending searches
const trendingSearches = ref<string[]>([
  'Stadium Operations',
  'Security Protocols',
  'Event Management',
  'Volunteer Training',
  'Media Guidelines'
])

// Sort options
const sortOptions = computed(() => [
  { label: isRTL.value ? 'الأكثر صلة' : 'Most Relevant', value: 'relevance' },
  { label: isRTL.value ? 'الأحدث' : 'Newest', value: 'date_desc' },
  { label: isRTL.value ? 'الأقدم' : 'Oldest', value: 'date_asc' },
  { label: isRTL.value ? 'الأكثر مشاهدة' : 'Most Viewed', value: 'views' }
])

// Date filter options
const dateFilterOptions = computed(() => [
  { label: isRTL.value ? 'أي وقت' : 'Any Time', value: 'all' },
  { label: isRTL.value ? 'اليوم' : 'Today', value: 'today' },
  { label: isRTL.value ? 'هذا الأسبوع' : 'This Week', value: 'week' },
  { label: isRTL.value ? 'هذا الشهر' : 'This Month', value: 'month' },
  { label: isRTL.value ? 'هذه السنة' : 'This Year', value: 'year' }
])

// Notification frequency options
const frequencyOptions = computed(() => [
  { label: isRTL.value ? 'فوري' : 'Immediately', value: 'immediately' },
  { label: isRTL.value ? 'يومي' : 'Daily', value: 'daily' },
  { label: isRTL.value ? 'أسبوعي' : 'Weekly', value: 'weekly' }
])

// Computed
const totalPages = computed(() => Math.ceil(totalResults.value / pageSize.value))

const hasActiveFilters = computed(() =>
  selectedContentTypes.value.length > 0 || selectedDateFilter.value !== 'all'
)

// Helper functions
const getContentTypeConfig = (type: string) => CONTENT_TYPE_CONFIG[type] || CONTENT_TYPE_CONFIG.Article
const getFileTypeConfig = (type: string) => FILE_TYPE_CONFIG[type.toLowerCase()] || { icon: 'pi pi-file', color: '#6b7280' }

const getResultTitle = (result: SearchResult) => {
  if (result.highlightedTitle) return result.highlightedTitle
  return isRTL.value && result.titleAr ? result.titleAr : result.title
}

const getResultSummary = (result: SearchResult) => {
  if (result.highlightedSummary) return result.highlightedSummary
  return isRTL.value && result.summaryAr ? result.summaryAr : result.summary
}

const getResultCategory = (result: SearchResult) => {
  return isRTL.value && result.categoryAr ? result.categoryAr : result.category
}

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString(isRTL.value ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const formatRelativeTime = (date: Date) => {
  const now = new Date()
  const diffMs = now.getTime() - new Date(date).getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMins / 60)
  const diffDays = Math.floor(diffHours / 24)

  if (diffMins < 60) {
    return isRTL.value ? `منذ ${diffMins} دقيقة` : `${diffMins}m ago`
  } else if (diffHours < 24) {
    return isRTL.value ? `منذ ${diffHours} ساعة` : `${diffHours}h ago`
  } else if (diffDays < 7) {
    return isRTL.value ? `منذ ${diffDays} يوم` : `${diffDays}d ago`
  } else {
    return formatDate(date)
  }
}

const formatFileSize = (bytes: number) => {
  if (bytes < 1024) return bytes + ' B'
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB'
  if (bytes < 1024 * 1024 * 1024) return (bytes / (1024 * 1024)).toFixed(1) + ' MB'
  return (bytes / (1024 * 1024 * 1024)).toFixed(1) + ' GB'
}

const formatDuration = (seconds: number) => {
  const mins = Math.floor(seconds / 60)
  const secs = seconds % 60
  return `${mins}:${secs.toString().padStart(2, '0')}`
}

const formatNumber = (num: number) => {
  if (num >= 1000000) return (num / 1000000).toFixed(1) + 'M'
  if (num >= 1000) return (num / 1000).toFixed(1) + 'K'
  return num.toString()
}

// Search methods
const performSearch = async () => {
  if (!searchQuery.value.trim()) return

  isSearching.value = true
  hasSearched.value = true
  lastQuery.value = searchQuery.value

  try {
    await new Promise(resolve => setTimeout(resolve, 800))

    // Mock results
    results.value = [
      {
        id: '1',
        sourceId: 'art-001',
        contentType: 'Article',
        title: 'Stadium Operations Guide for AFC Asian Cup 2027',
        titleAr: 'دليل عمليات الملعب لكأس آسيا 2027',
        summary: 'Comprehensive guide covering all aspects of stadium operations including security, logistics, and fan experience management.',
        summaryAr: 'دليل شامل يغطي جميع جوانب عمليات الملعب بما في ذلك الأمن واللوجستيات وإدارة تجربة المشجعين.',
        author: 'Mohammed Al-Rashid',
        authorAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Mohammed',
        category: 'Operations',
        categoryAr: 'العمليات',
        tags: ['Stadium', 'Operations', 'Guide'],
        viewCount: 1250,
        publishedAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
        relevanceScore: 0.95
      },
      {
        id: '2',
        sourceId: 'doc-001',
        contentType: 'Document',
        title: 'Security Protocol Manual v2.0',
        titleAr: 'دليل بروتوكول الأمن الإصدار 2.0',
        summary: 'Official security protocols and procedures for all AFC Asian Cup 2027 venues and events.',
        summaryAr: 'بروتوكولات وإجراءات الأمن الرسمية لجميع أماكن وفعاليات كأس آسيا 2027.',
        author: 'Security Department',
        category: 'Security',
        categoryAr: 'الأمن',
        fileType: 'pdf',
        fileSize: 2500000,
        viewCount: 890,
        publishedAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
        relevanceScore: 0.88
      },
      {
        id: '3',
        sourceId: 'news-001',
        contentType: 'News',
        title: 'Volunteer Training Program Kicks Off',
        titleAr: 'انطلاق برنامج تدريب المتطوعين',
        summary: 'Over 5,000 volunteers begin their training journey for the upcoming tournament.',
        summaryAr: 'أكثر من 5000 متطوع يبدأون رحلتهم التدريبية للبطولة القادمة.',
        author: 'Sara Ali',
        authorAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sara',
        category: 'Volunteers',
        categoryAr: 'المتطوعون',
        tags: ['Volunteers', 'Training'],
        viewCount: 2100,
        publishedAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
        relevanceScore: 0.82
      },
      {
        id: '4',
        sourceId: 'disc-001',
        contentType: 'Discussion',
        title: 'Best practices for media center operations',
        titleAr: 'أفضل الممارسات لعمليات المركز الإعلامي',
        summary: 'Community discussion on optimizing media center workflows and technology setup.',
        summaryAr: 'نقاش مجتمعي حول تحسين سير عمل المركز الإعلامي وإعداد التكنولوجيا.',
        author: 'Ahmed Hassan',
        authorAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Ahmed',
        category: 'Media',
        categoryAr: 'الإعلام',
        viewCount: 456,
        publishedAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000),
        relevanceScore: 0.78
      },
      {
        id: '5',
        sourceId: 'media-001',
        contentType: 'MediaItem',
        title: 'Stadium Tour Video - Lusail Stadium',
        titleAr: 'فيديو جولة الملعب - ملعب لوسيل',
        summary: 'Virtual tour of Lusail Stadium showcasing facilities and capacity.',
        summaryAr: 'جولة افتراضية في ملعب لوسيل تعرض المرافق والسعة.',
        thumbnailUrl: 'https://picsum.photos/seed/stadium/400/225',
        duration: 420,
        viewCount: 3500,
        publishedAt: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000),
        relevanceScore: 0.75
      },
      {
        id: '6',
        sourceId: 'lesson-001',
        contentType: 'LessonLearned',
        title: 'Crowd Management During Peak Hours',
        titleAr: 'إدارة الحشود في أوقات الذروة',
        summary: 'Key insights from previous events on managing large crowds effectively.',
        summaryAr: 'رؤى رئيسية من الأحداث السابقة حول إدارة الحشود الكبيرة بفعالية.',
        author: 'Operations Team',
        category: 'Operations',
        categoryAr: 'العمليات',
        tags: ['Crowd Management', 'Safety'],
        viewCount: 780,
        publishedAt: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
        relevanceScore: 0.72
      }
    ]

    totalResults.value = 156
    executionTimeMs.value = 145

    facets.value = {
      contentTypes: [
        { value: 'Article', label: 'Articles', labelAr: 'مقالات', count: 45 },
        { value: 'Document', label: 'Documents', labelAr: 'مستندات', count: 38 },
        { value: 'News', label: 'News', labelAr: 'أخبار', count: 28 },
        { value: 'Discussion', label: 'Discussions', labelAr: 'نقاشات', count: 22 },
        { value: 'MediaItem', label: 'Media', labelAr: 'وسائط', count: 15 },
        { value: 'LessonLearned', label: 'Lessons Learned', labelAr: 'دروس مستفادة', count: 8 }
      ],
      categories: [
        { value: 'Operations', label: 'Operations', labelAr: 'العمليات', count: 42 },
        { value: 'Security', label: 'Security', labelAr: 'الأمن', count: 35 },
        { value: 'Media', label: 'Media', labelAr: 'الإعلام', count: 28 },
        { value: 'Volunteers', label: 'Volunteers', labelAr: 'المتطوعون', count: 21 }
      ],
      fileTypes: [
        { value: 'pdf', label: 'PDF', labelAr: 'PDF', count: 25 },
        { value: 'docx', label: 'Word', labelAr: 'وورد', count: 18 },
        { value: 'xlsx', label: 'Excel', labelAr: 'إكسل', count: 12 }
      ]
    }

    router.replace({
      query: {
        q: searchQuery.value,
        types: selectedContentTypes.value.length > 0 ? selectedContentTypes.value.join(',') : undefined,
        date: selectedDateFilter.value !== 'all' ? selectedDateFilter.value : undefined,
        sort: sortBy.value !== 'relevance' ? sortBy.value : undefined,
        page: currentPage.value > 1 ? currentPage.value.toString() : undefined
      }
    })
  } catch (error) {
    console.error('Search failed:', error)
  } finally {
    isSearching.value = false
  }
}

const searchFor = (query: string) => {
  searchQuery.value = query
  performSearch()
}

const clearSearch = () => {
  searchQuery.value = ''
  hasSearched.value = false
  results.value = []
  totalResults.value = 0
}

const clearFilters = () => {
  selectedContentTypes.value = []
  selectedDateFilter.value = 'all'
  if (hasSearched.value) {
    performSearch()
  }
}

const toggleContentType = (type: string) => {
  const index = selectedContentTypes.value.indexOf(type)
  if (index > -1) {
    selectedContentTypes.value.splice(index, 1)
  } else {
    selectedContentTypes.value.push(type)
  }
  currentPage.value = 1
  performSearch()
}

const openResult = (result: SearchResult) => {
  const routes: Record<string, string> = {
    Article: 'content-view',
    News: 'content-view',
    Document: 'document-library',
    Discussion: 'discussion',
    Community: 'community-detail',
    MediaItem: 'media-gallery-detail',
    User: 'profile',
    Event: 'calendar',
    LessonLearned: 'lesson-detail'
  }

  const routeName = routes[result.contentType] || 'dashboard'
  router.push({ name: routeName, params: { id: result.sourceId } })
}

const goToPage = (page: number) => {
  currentPage.value = page
  performSearch()
}

const saveSearch = async () => {
  if (!savedSearchName.value.trim()) return

  isSaving.value = true
  try {
    await new Promise(resolve => setTimeout(resolve, 500))
    showSaveSearchDialog.value = false
    savedSearchName.value = ''
    enableNotifications.value = false
  } catch (error) {
    console.error('Failed to save search:', error)
  } finally {
    isSaving.value = false
  }
}

// Lifecycle
async function loadSearchResults() {
  try {
    error.value = null
    loading.value = true

    await new Promise(resolve => setTimeout(resolve, LOADING_DELAY))

    if (route.query.q) {
      searchQuery.value = route.query.q as string
      if (route.query.types) {
        selectedContentTypes.value = (route.query.types as string).split(',')
      }
      if (route.query.date) {
        selectedDateFilter.value = route.query.date as string
      }
      if (route.query.sort) {
        sortBy.value = route.query.sort as string
      }
      if (route.query.page) {
        currentPage.value = parseInt(route.query.page as string) || 1
      }
      performSearch()
    }

    loading.value = false

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
  await loadSearchResults()
}

onMounted(() => {
  loadSearchResults()
})

// Watch for route changes
watch(() => route.query.q, (newQuery) => {
  if (newQuery && newQuery !== searchQuery.value) {
    searchQuery.value = newQuery as string
    performSearch()
  }
})
</script>

<template>
  <div class="search-results-view" :class="{ rtl: isRTL }">
    <!-- Page Header -->
    <PageHeader
      :title="isRTL ? 'البحث المتقدم' : 'Advanced Search'"
      :description="isRTL ? 'ابحث في جميع المحتويات والمستندات والنقاشات' : 'Search across all content, documents, and discussions'"
      :breadcrumbs="breadcrumbs"
      padding-bottom="xl"
      background-variant="gradient"
      :animated="shouldAnimate"
    >
      <template #actions>
        <Button
          v-if="hasSearched && totalResults > 0"
          :label="isRTL ? 'حفظ البحث' : 'Save Search'"
          icon="pi pi-bookmark"
          class="header-btn-secondary"
          @click="showSaveSearchDialog = true"
        />
      </template>
    </PageHeader>

    <!-- Search Box -->
    <div class="search-box-section">
      <div class="search-box-container">
        <div class="search-box">
          <i class="pi pi-search search-icon"></i>
          <InputText
            v-model="searchQuery"
            :placeholder="isRTL ? 'ابحث عن أي شيء...' : 'Search for anything...'"
            class="search-input"
            @keyup.enter="performSearch"
          />
          <Button
            v-if="searchQuery"
            icon="pi pi-times"
            text
            rounded
            class="clear-btn"
            @click="clearSearch"
          />
          <Button
            :label="isRTL ? 'بحث' : 'Search'"
            icon="pi pi-search"
            class="search-btn"
            :loading="isSearching"
            @click="performSearch"
          />
        </div>
      </div>
    </div>

    <!-- Stats Bar (only show after search) -->
    <StatsBar
      v-if="hasSearched && !isSearching"
      :stats="statsBarItems"
      :loading="false"
      overlap="lg"
      :animated="shouldAnimate"
      :animate-numbers="shouldAnimate"
      :counter-duration="1200"
    />

    <!-- Main Content -->
    <div class="main-content">
      <!-- Empty State / Initial -->
      <div v-if="!hasSearched && !loading" class="initial-state">
        <div class="trending-section">
          <h3>{{ isRTL ? 'عمليات البحث الشائعة' : 'Trending Searches' }}</h3>
          <div class="trending-tags">
            <button
              v-for="term in trendingSearches"
              :key="term"
              class="trending-tag"
              @click="searchFor(term)"
            >
              <i class="pi pi-search"></i>
              {{ term }}
            </button>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-else-if="isSearching" class="loading-state">
        <div class="loading-content">
          <Skeleton height="80px" class="mb-4" borderRadius="16px" />
          <div v-for="i in 5" :key="i" class="mb-3">
            <Skeleton height="140px" borderRadius="16px" />
          </div>
        </div>
      </div>

      <!-- Error State -->
      <ErrorState
        v-else-if="error"
        :error="error"
        :title="isRTL ? 'فشل البحث' : 'Search failed'"
        show-retry
        @retry="handleRetry"
      />

      <!-- Results -->
      <div v-else-if="hasSearched && results.length > 0" class="results-content">
        <!-- Sidebar Filters -->
        <aside class="filters-sidebar">
          <div class="sidebar-card">
            <div class="card-header">
              <h4>{{ isRTL ? 'التصفية' : 'Filters' }}</h4>
              <Button
                v-if="hasActiveFilters"
                :label="isRTL ? 'مسح' : 'Clear'"
                text
                size="small"
                @click="clearFilters"
              />
            </div>

            <!-- Content Type Filter -->
            <div class="filter-group">
              <h5>{{ isRTL ? 'نوع المحتوى' : 'Content Type' }}</h5>
              <div class="filter-options">
                <label
                  v-for="facet in facets.contentTypes"
                  :key="facet.value"
                  class="filter-option"
                >
                  <Checkbox
                    v-model="selectedContentTypes"
                    :value="facet.value"
                    @change="performSearch"
                  />
                  <span class="filter-label">{{ isRTL ? facet.labelAr : facet.label }}</span>
                  <span class="filter-count">{{ facet.count }}</span>
                </label>
              </div>
            </div>

            <!-- Date Filter -->
            <div class="filter-group">
              <h5>{{ isRTL ? 'التاريخ' : 'Date' }}</h5>
              <Dropdown
                v-model="selectedDateFilter"
                :options="dateFilterOptions"
                optionLabel="label"
                optionValue="value"
                class="w-full"
                @change="performSearch"
              />
            </div>
          </div>
        </aside>

        <!-- Results List -->
        <div class="results-main">
          <!-- Toolbar -->
          <div class="results-toolbar">
            <div class="toolbar-info">
              <span class="results-count">
                {{ isRTL ? `${totalResults} نتيجة لـ "${lastQuery}"` : `${totalResults} results for "${lastQuery}"` }}
              </span>
            </div>
            <div class="toolbar-actions">
              <Dropdown
                v-model="sortBy"
                :options="sortOptions"
                optionLabel="label"
                optionValue="value"
                class="sort-dropdown"
                @change="performSearch"
              />
            </div>
          </div>

          <!-- Active Filters -->
          <div v-if="hasActiveFilters" class="active-filters">
            <span class="active-filters-label">{{ isRTL ? 'الفلاتر النشطة:' : 'Active filters:' }}</span>
            <div class="filter-tags">
              <span
                v-for="type in selectedContentTypes"
                :key="type"
                class="filter-tag"
              >
                {{ isRTL ? getContentTypeConfig(type).labelAr : getContentTypeConfig(type).label }}
                <button @click="toggleContentType(type)">
                  <i class="pi pi-times"></i>
                </button>
              </span>
              <span v-if="selectedDateFilter !== 'all'" class="filter-tag">
                {{ dateFilterOptions.find(d => d.value === selectedDateFilter)?.label }}
                <button @click="selectedDateFilter = 'all'; performSearch()">
                  <i class="pi pi-times"></i>
                </button>
              </span>
            </div>
          </div>

          <!-- Results Cards -->
          <div class="results-list">
            <div
              v-for="result in results"
              :key="result.id"
              class="result-card"
              @click="openResult(result)"
            >
              <!-- Type Badge -->
              <div
                class="type-indicator"
                :style="{ backgroundColor: getContentTypeConfig(result.contentType).color }"
              ></div>

              <div class="result-content">
                <!-- Header -->
                <div class="result-header">
                  <div
                    class="type-badge"
                    :style="{
                      backgroundColor: getContentTypeConfig(result.contentType).bgColor,
                      color: getContentTypeConfig(result.contentType).color
                    }"
                  >
                    <i :class="getContentTypeConfig(result.contentType).icon"></i>
                    <span>{{ isRTL ? getContentTypeConfig(result.contentType).labelAr : getContentTypeConfig(result.contentType).label }}</span>
                  </div>
                  <span v-if="result.category" class="category-badge">
                    {{ getResultCategory(result) }}
                  </span>
                </div>

                <!-- Title -->
                <h3 class="result-title" v-html="getResultTitle(result)"></h3>

                <!-- Summary -->
                <p class="result-summary" v-html="getResultSummary(result)"></p>

                <!-- Tags -->
                <div v-if="result.tags?.length" class="result-tags">
                  <span v-for="tag in result.tags.slice(0, 3)" :key="tag" class="tag">
                    {{ tag }}
                  </span>
                </div>

                <!-- Meta -->
                <div class="result-meta">
                  <div v-if="result.author" class="meta-author">
                    <Avatar
                      :image="result.authorAvatar"
                      :name="result.author"
                      shape="circle"
                      size="sm"
                    />
                    <span>{{ result.author }}</span>
                  </div>

                  <div class="meta-stats">
                    <span v-if="result.fileType" class="meta-item">
                      <i :class="getFileTypeConfig(result.fileType).icon"></i>
                      {{ result.fileType.toUpperCase() }}
                      <template v-if="result.fileSize">• {{ formatFileSize(result.fileSize) }}</template>
                    </span>
                    <span v-if="result.duration" class="meta-item">
                      <i class="pi pi-clock"></i>
                      {{ formatDuration(result.duration) }}
                    </span>
                    <span class="meta-item">
                      <i class="pi pi-eye"></i>
                      {{ formatNumber(result.viewCount) }}
                    </span>
                    <span class="meta-item">
                      <i class="pi pi-calendar"></i>
                      {{ formatRelativeTime(result.publishedAt) }}
                    </span>
                  </div>
                </div>
              </div>

              <!-- Thumbnail (for media) -->
              <div v-if="result.thumbnailUrl" class="result-thumbnail">
                <img :src="result.thumbnailUrl" :alt="result.title" />
                <div v-if="result.duration" class="duration-badge">
                  {{ formatDuration(result.duration) }}
                </div>
              </div>
            </div>
          </div>

          <!-- Pagination -->
          <div v-if="totalPages > 1" class="pagination">
            <Button
              icon="pi pi-chevron-left"
              text
              rounded
              :disabled="currentPage === 1"
              @click="goToPage(currentPage - 1)"
            />
            <div class="page-numbers">
              <button
                v-for="page in Math.min(totalPages, 5)"
                :key="page"
                :class="['page-btn', { active: page === currentPage }]"
                @click="goToPage(page)"
              >
                {{ page }}
              </button>
            </div>
            <Button
              icon="pi pi-chevron-right"
              text
              rounded
              :disabled="currentPage === totalPages"
              @click="goToPage(currentPage + 1)"
            />
          </div>
        </div>
      </div>

      <!-- No Results -->
      <div v-else-if="hasSearched && results.length === 0" class="empty-state">
        <div class="empty-icon">
          <i class="pi pi-search"></i>
        </div>
        <h4>{{ isRTL ? 'لا توجد نتائج' : 'No results found' }}</h4>
        <p>{{ isRTL ? `لم نتمكن من إيجاد نتائج لـ "${lastQuery}"` : `We couldn't find any results for "${lastQuery}"` }}</p>
        <div class="empty-suggestions">
          <p>{{ isRTL ? 'جرب:' : 'Try:' }}</p>
          <ul>
            <li>{{ isRTL ? 'استخدام كلمات مختلفة' : 'Using different keywords' }}</li>
            <li>{{ isRTL ? 'إزالة الفلاتر' : 'Removing filters' }}</li>
            <li>{{ isRTL ? 'التحقق من الإملاء' : 'Checking your spelling' }}</li>
          </ul>
        </div>
      </div>
    </div>

    <!-- Save Search Dialog -->
    <Dialog
      v-model:visible="showSaveSearchDialog"
      :header="isRTL ? 'حفظ البحث' : 'Save Search'"
      :style="{ width: '450px' }"
      modal
      class="premium-dialog"
    >
      <div class="dialog-form">
        <div class="form-group">
          <label class="form-label">{{ isRTL ? 'اسم البحث' : 'Search Name' }}</label>
          <InputText
            v-model="savedSearchName"
            :placeholder="isRTL ? 'أدخل اسمًا لهذا البحث' : 'Enter a name for this search'"
            class="w-full"
          />
        </div>

        <div class="form-group checkbox-group">
          <Checkbox v-model="enableNotifications" inputId="enableNotifs" binary />
          <label for="enableNotifs">{{ isRTL ? 'إشعارات النتائج الجديدة' : 'Notify me of new results' }}</label>
        </div>

        <div v-if="enableNotifications" class="form-group">
          <label class="form-label">{{ isRTL ? 'تكرار الإشعارات' : 'Notification Frequency' }}</label>
          <Dropdown
            v-model="notificationFrequency"
            :options="frequencyOptions"
            optionLabel="label"
            optionValue="value"
            class="w-full"
          />
        </div>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <Button
            :label="isRTL ? 'إلغاء' : 'Cancel'"
            severity="secondary"
            outlined
            @click="showSaveSearchDialog = false"
          />
          <Button
            :label="isRTL ? 'حفظ' : 'Save'"
            icon="pi pi-check"
            :loading="isSaving"
            @click="saveSearch"
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
// SEARCH VIEW - MAIN CONTAINER
// Following Universal Spacing Guidelines from _mixins.scss
// ============================================

.search-results-view {
  @include page-view;

  &.rtl {
    direction: rtl;

    .breadcrumb-separator {
      transform: rotate(180deg);
    }

    .result-card:hover {
      transform: translateX(-4px);
    }
  }
}

// Header button styles (used in PageHeader slot)
.header-btn-secondary {
  @include header-btn-secondary;
}

// ============================================
// SEARCH BOX
// ============================================

.search-box-container {
  position: relative;
  z-index: 1;
}

.search-box {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  padding: $spacing-2 $spacing-3;
  background: white;
  border-radius: $radius-xl;
  box-shadow: $shadow-lg;
}

.search-icon {
  color: $slate-400;
  font-size: 1.25rem;
  margin-left: $spacing-2;
}

.search-input {
  flex: 1;
  border: none;
  font-size: $font-size-lg;
  padding: $spacing-3;

  &:focus {
    box-shadow: none;
  }
}

.clear-btn {
  color: $slate-400;

  &:hover {
    color: $slate-600;
  }
}

.search-btn {
  @include button-primary;
  padding: $spacing-3 $spacing-6;
}

// ============================================
// MAIN CONTENT - Using standardized page-content mixin
// ============================================

.main-content {
  @include page-content;
}

// ============================================
// INITIAL STATE
// ============================================

.initial-state {
  max-width: 800px;
  margin: 0 auto;
  padding: $spacing-8;
  text-align: center;
}

.trending-section {
  h3 {
    margin: 0 0 $spacing-4;
    font-size: $font-size-lg;
    font-weight: $font-weight-semibold;
    color: $slate-700;
  }
}

.trending-tags {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: $spacing-3;
}

.trending-tag {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-3 $spacing-4;
  background: white;
  border: 1px solid $slate-200;
  border-radius: $radius-lg;
  color: $slate-700;
  font-size: $font-size-sm;
  cursor: pointer;
  transition: all $transition-fast;

  &:hover {
    background: $intalio-teal-50;
    border-color: $intalio-teal-300;
    color: $intalio-teal-700;
  }

  i {
    font-size: 0.875rem;
    color: $slate-400;
  }
}

// ============================================
// LOADING STATE
// ============================================

.loading-state {
  max-width: 900px;
  margin: 0 auto;
}

// ============================================
// RESULTS CONTENT
// ============================================

.results-content {
  display: grid;
  grid-template-columns: 280px 1fr;
  gap: $spacing-6;

  @include mobile {
    grid-template-columns: 1fr;
  }
}

// ============================================
// FILTERS SIDEBAR
// ============================================

.filters-sidebar {
  @include mobile {
    display: none;
  }
}

.sidebar-card {
  @include card-base;
  padding: $spacing-4;
  position: sticky;
  top: $spacing-4;
}

.card-header {
  @include flex-between;
  margin-bottom: $spacing-4;
  padding-bottom: $spacing-3;
  border-bottom: 1px solid $slate-100;

  h4 {
    margin: 0;
    font-size: $font-size-base;
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }
}

.filter-group {
  margin-bottom: $spacing-5;

  &:last-child {
    margin-bottom: 0;
  }

  h5 {
    margin: 0 0 $spacing-3;
    font-size: $font-size-sm;
    font-weight: $font-weight-semibold;
    color: $slate-700;
  }
}

.filter-options {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;
}

.filter-option {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-2;
  border-radius: $radius-md;
  cursor: pointer;
  transition: background $transition-fast;

  &:hover {
    background: $slate-50;
  }
}

.filter-label {
  flex: 1;
  font-size: $font-size-sm;
  color: $slate-700;
}

.filter-count {
  font-size: $font-size-xs;
  color: $slate-400;
  background: $slate-100;
  padding: 2px $spacing-2;
  border-radius: $radius-sm;
}

// ============================================
// RESULTS MAIN
// ============================================

.results-main {
  min-width: 0;
}

.results-toolbar {
  @include flex-between;
  padding: $spacing-4;
  background: white;
  border-radius: $radius-xl;
  box-shadow: $shadow-sm;
  margin-bottom: $spacing-4;
  gap: $spacing-4;
  flex-wrap: wrap;
}

.toolbar-info {
  .results-count {
    font-size: $font-size-sm;
    color: $slate-600;
  }
}

.sort-dropdown {
  min-width: 160px;
}

// ============================================
// ACTIVE FILTERS
// ============================================

.active-filters {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  margin-bottom: $spacing-4;
  flex-wrap: wrap;
}

.active-filters-label {
  font-size: $font-size-sm;
  color: $slate-500;
}

.filter-tags {
  display: flex;
  flex-wrap: wrap;
  gap: $spacing-2;
}

.filter-tag {
  display: inline-flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-1 $spacing-3;
  background: $intalio-teal-50;
  color: $intalio-teal-700;
  border-radius: $radius-full;
  font-size: $font-size-sm;

  button {
    background: none;
    border: none;
    color: inherit;
    cursor: pointer;
    padding: 0;
    display: flex;
    opacity: 0.7;

    &:hover {
      opacity: 1;
    }
  }
}

// ============================================
// RESULTS LIST
// ============================================

.results-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-4;
}

.result-card {
  display: flex;
  background: white;
  border-radius: $radius-xl;
  box-shadow: $shadow-sm;
  overflow: hidden;
  cursor: pointer;
  transition: all $transition-base;

  @include card-item-animation;

  &:hover {
    box-shadow: $shadow-md;
    transform: translateX(4px);
  }
}

.type-indicator {
  width: 4px;
  flex-shrink: 0;
}

.result-content {
  flex: 1;
  padding: $spacing-5;
  min-width: 0;
}

.result-header {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  margin-bottom: $spacing-3;
}

.type-badge {
  display: inline-flex;
  align-items: center;
  gap: $spacing-1;
  padding: $spacing-1 $spacing-2;
  border-radius: $radius-md;
  font-size: $font-size-xs;
  font-weight: $font-weight-medium;

  i {
    font-size: 0.75rem;
  }
}

.category-badge {
  font-size: $font-size-xs;
  color: $slate-500;
  background: $slate-100;
  padding: $spacing-1 $spacing-2;
  border-radius: $radius-sm;
}

.result-title {
  margin: 0 0 $spacing-2;
  font-size: $font-size-lg;
  font-weight: $font-weight-semibold;
  color: $slate-900;
  line-height: 1.4;

  :deep(mark), :deep(em) {
    background: rgba($intalio-teal-500, 0.2);
    color: inherit;
    font-style: normal;
    padding: 0 2px;
    border-radius: 2px;
  }
}

.result-summary {
  margin: 0 0 $spacing-3;
  font-size: $font-size-sm;
  color: $slate-600;
  line-height: 1.6;
  @include line-clamp(2);

  :deep(mark), :deep(em) {
    background: rgba($intalio-teal-500, 0.2);
    color: inherit;
    font-style: normal;
    padding: 0 2px;
    border-radius: 2px;
  }
}

.result-tags {
  display: flex;
  flex-wrap: wrap;
  gap: $spacing-2;
  margin-bottom: $spacing-3;
}

.tag {
  font-size: $font-size-xs;
  padding: $spacing-1 $spacing-2;
  background: $slate-100;
  color: $slate-600;
  border-radius: $radius-sm;
}

.result-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: $spacing-4;
  flex-wrap: wrap;
}

.meta-author {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  font-size: $font-size-sm;
  color: $slate-700;
}

.meta-stats {
  display: flex;
  align-items: center;
  gap: $spacing-4;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: $spacing-1;
  font-size: $font-size-xs;
  color: $slate-500;

  i {
    font-size: 0.75rem;
  }
}

// ============================================
// RESULT THUMBNAIL
// ============================================

.result-thumbnail {
  position: relative;
  width: 200px;
  flex-shrink: 0;

  @include mobile {
    display: none;
  }

  img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
}

.duration-badge {
  position: absolute;
  bottom: $spacing-2;
  right: $spacing-2;
  padding: 2px $spacing-2;
  background: rgba(0, 0, 0, 0.75);
  color: white;
  font-size: $font-size-xs;
  font-weight: $font-weight-medium;
  border-radius: $radius-sm;
}

// ============================================
// PAGINATION
// ============================================

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: $spacing-2;
  margin-top: $spacing-6;
  padding: $spacing-4;
  background: white;
  border-radius: $radius-xl;
  box-shadow: $shadow-sm;
}

.page-numbers {
  display: flex;
  gap: $spacing-1;
}

.page-btn {
  width: 36px;
  height: 36px;
  border: none;
  background: transparent;
  color: $slate-600;
  font-weight: $font-weight-medium;
  border-radius: $radius-md;
  cursor: pointer;
  transition: all $transition-fast;

  &:hover {
    background: $slate-100;
  }

  &.active {
    background: $intalio-teal-500;
    color: white;
  }
}

// ============================================
// EMPTY STATE
// ============================================

.empty-state {
  @include empty-state;
  max-width: 500px;
  margin: 0 auto;
  padding: $spacing-12;
  background: white;
  border-radius: $radius-2xl;
  box-shadow: $shadow-sm;
  text-align: center;
}

.empty-icon {
  @include empty-state-icon;
}

.empty-suggestions {
  text-align: left;
  margin-top: $spacing-4;
  padding: $spacing-4;
  background: $slate-50;
  border-radius: $radius-lg;

  p {
    margin: 0 0 $spacing-2;
    font-weight: $font-weight-medium;
    color: $slate-700;
  }

  ul {
    margin: 0;
    padding-left: $spacing-5;
    color: $slate-600;

    li {
      margin-bottom: $spacing-1;
    }
  }
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

.form-label {
  font-size: $font-size-sm;
  font-weight: $font-weight-medium;
  color: $slate-700;
}

.checkbox-group {
  flex-direction: row;
  align-items: center;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: $spacing-3;
}

// ============================================
// ANIMATIONS
// ============================================

@include staggered-animation-delays('.result-card', 10, 0.05s);
</style>
