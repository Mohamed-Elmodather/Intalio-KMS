<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { BookmarkButton, SocialShareButtons } from '@/components/common'
import type {
  LessonLearned,
  LessonPriority,
  LessonCategory
} from '@/types/lessons-learned'
import {
  getPriorityColor,
  getCategoryColor,
  getCategoryIcon,
  getPriorityIcon,
  LESSON_CATEGORIES,
  LESSON_PRIORITIES
} from '@/types/lessons-learned'

const router = useRouter()

// ============================================
// STATE
// ============================================
const searchQuery = ref('')
const selectedCategory = ref<LessonCategory | 'all'>('all')
const selectedPriority = ref<LessonPriority | 'all'>('all')
const sortBy = ref<'recent' | 'popular' | 'priority'>('recent')
const viewMode = ref<'grid' | 'list'>('grid')
const showCategoryDropdown = ref(false)
const showPriorityDropdown = ref(false)
const showSortDropdown = ref(false)
const currentPage = ref(1)
const itemsPerPage = ref(12)

// ============================================
// TEXT CONSTANTS
// ============================================
const text = {
  pageTitle: 'Lessons Learned',
  pageSubtitle: 'Capture and share professional insights from your experiences',
  newLesson: 'New Lesson',
  searchPlaceholder: 'Search lessons...',
  allCategories: 'All Categories',
  allPriorities: 'All Priorities',
  sortBy: 'Sort by',
  recent: 'Most Recent',
  popular: 'Most Popular',
  priority: 'Highest Priority',
  gridView: 'Grid View',
  listView: 'List View',
  views: 'views',
  likes: 'likes',
  comments: 'comments',
  noResults: 'No lessons found',
  noResultsDesc: 'Try adjusting your filters or create a new lesson',
  statsTotal: 'Total Lessons',
  statsThisMonth: 'This Month',
  statsCritical: 'Critical',
  statsHigh: 'High Priority'
}

// ============================================
// MOCK DATA
// ============================================
const mockLessons = ref<LessonLearned[]>([
  {
    id: '1',
    title: 'Always Document API Changes Before Deployment',
    summary: 'Learned the hard way that undocumented API changes cause major integration issues across dependent teams.',
    content: 'During our Q3 platform migration, we made several breaking API changes without proper documentation...',
    category: 'technical',
    priority: 'critical',
    tags: ['API', 'Documentation', 'Best Practices'],
    status: 'published',
    context: 'Q3 2024 Platform Migration Project',
    impact: 'Reduced integration issues by 60% in subsequent releases',
    recommendations: ['Create API changelog', 'Notify dependent teams 2 weeks before', 'Version all endpoints'],
    author: { id: '1', name: 'Ahmed Hassan', initials: 'AH' },
    createdAt: '2024-01-15',
    updatedAt: '2024-01-15',
    viewCount: 234,
    likeCount: 45,
    commentCount: 12
  },
  {
    id: '2',
    title: 'Clear Communication Reduces Meeting Time by 40%',
    summary: 'Implementing structured agendas and pre-reads dramatically improved our meeting efficiency.',
    content: 'After tracking meeting effectiveness for 3 months, we discovered that most meetings ran over time...',
    category: 'communication',
    priority: 'high',
    tags: ['Meetings', 'Productivity', 'Communication'],
    status: 'published',
    context: 'Team Productivity Initiative',
    impact: 'Saved average of 5 hours per week per team member',
    recommendations: ['Always share agenda 24h before', 'Define clear outcomes', 'Assign timekeeper role'],
    author: { id: '2', name: 'Sarah Chen', initials: 'SC' },
    createdAt: '2024-01-10',
    updatedAt: '2024-01-12',
    viewCount: 567,
    likeCount: 89,
    commentCount: 23
  },
  {
    id: '3',
    title: 'Code Reviews Should Focus on Logic, Not Style',
    summary: 'Automated linting and formatting tools free up code reviews for meaningful architectural discussions.',
    content: 'Our code review process was taking too long because reviewers were spending time on style issues...',
    category: 'process',
    priority: 'medium',
    tags: ['Code Review', 'Development', 'Automation'],
    status: 'published',
    context: 'Engineering Process Improvement',
    impact: 'Code review time reduced by 35%',
    recommendations: ['Implement Prettier/ESLint', 'Create style guide', 'Use pre-commit hooks'],
    author: { id: '3', name: 'Michael Park', initials: 'MP' },
    createdAt: '2024-01-08',
    updatedAt: '2024-01-08',
    viewCount: 345,
    likeCount: 67,
    commentCount: 15
  },
  {
    id: '4',
    title: 'Stakeholder Alignment is Critical for Project Success',
    summary: 'Getting stakeholder buy-in early prevents scope creep and ensures project stays on track.',
    content: 'On the Customer Portal redesign project, we learned that unclear stakeholder expectations...',
    category: 'leadership',
    priority: 'high',
    tags: ['Stakeholder Management', 'Project Management', 'Leadership'],
    status: 'published',
    context: 'Customer Portal Redesign Project',
    impact: 'Delivered project 2 weeks ahead of schedule',
    recommendations: ['Weekly stakeholder updates', 'Document decisions in writing', 'Create RACI matrix'],
    author: { id: '4', name: 'Lisa Wang', initials: 'LW' },
    createdAt: '2024-01-05',
    updatedAt: '2024-01-07',
    viewCount: 412,
    likeCount: 78,
    commentCount: 19
  },
  {
    id: '5',
    title: 'Feature Flags Enable Safer Deployments',
    summary: 'Using feature flags allows gradual rollouts and instant rollbacks without redeployment.',
    content: 'After a major production incident caused by a faulty release, we implemented feature flags...',
    category: 'technical',
    priority: 'high',
    tags: ['DevOps', 'Feature Flags', 'Deployment'],
    status: 'published',
    context: 'Production Incident Post-Mortem',
    impact: 'Zero downtime deployments achieved',
    recommendations: ['Use LaunchDarkly or similar', 'Default flags to off', 'Clean up old flags quarterly'],
    author: { id: '1', name: 'Ahmed Hassan', initials: 'AH' },
    createdAt: '2024-01-03',
    updatedAt: '2024-01-03',
    viewCount: 289,
    likeCount: 56,
    commentCount: 8
  },
  {
    id: '6',
    title: 'Risk Assessment Should Be Continuous, Not One-Time',
    summary: 'Regular risk reviews catch emerging threats before they become critical issues.',
    content: 'Our annual risk assessment missed several emerging threats that caused issues mid-year...',
    category: 'project',
    priority: 'critical',
    tags: ['Risk Management', 'Project Management', 'Governance'],
    status: 'published',
    context: 'Enterprise Risk Management Review',
    impact: 'Identified and mitigated 3 critical risks before impact',
    recommendations: ['Monthly risk reviews', 'Use risk heat maps', 'Assign risk owners'],
    author: { id: '5', name: 'David Kim', initials: 'DK' },
    createdAt: '2024-01-01',
    updatedAt: '2024-01-02',
    viewCount: 198,
    likeCount: 34,
    commentCount: 7
  }
])

// ============================================
// COMPUTED
// ============================================
const filteredLessons = computed(() => {
  let result = [...mockLessons.value]

  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(lesson =>
      lesson.title.toLowerCase().includes(query) ||
      lesson.summary.toLowerCase().includes(query) ||
      lesson.tags.some(tag => tag.toLowerCase().includes(query))
    )
  }

  // Category filter
  if (selectedCategory.value !== 'all') {
    result = result.filter(lesson => lesson.category === selectedCategory.value)
  }

  // Priority filter
  if (selectedPriority.value !== 'all') {
    result = result.filter(lesson => lesson.priority === selectedPriority.value)
  }

  // Sort
  switch (sortBy.value) {
    case 'recent':
      result.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      break
    case 'popular':
      result.sort((a, b) => b.viewCount - a.viewCount)
      break
    case 'priority':
      const priorityOrder = { critical: 0, high: 1, medium: 2, low: 3 }
      result.sort((a, b) => priorityOrder[a.priority] - priorityOrder[b.priority])
      break
  }

  return result
})

const paginatedLessons = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredLessons.value.slice(start, end)
})

const totalPages = computed(() => Math.ceil(filteredLessons.value.length / itemsPerPage.value))

const stats = computed(() => {
  const now = new Date()
  const thisMonth = mockLessons.value.filter(l => {
    const created = new Date(l.createdAt)
    return created.getMonth() === now.getMonth() && created.getFullYear() === now.getFullYear()
  })
  const critical = mockLessons.value.filter(l => l.priority === 'critical')
  const high = mockLessons.value.filter(l => l.priority === 'high')

  return {
    total: mockLessons.value.length,
    thisMonth: thisMonth.length,
    critical: critical.length,
    high: high.length
  }
})

const sortOptions = [
  { value: 'recent', label: text.recent, icon: 'fas fa-clock' },
  { value: 'popular', label: text.popular, icon: 'fas fa-fire' },
  { value: 'priority', label: text.priority, icon: 'fas fa-exclamation-triangle' }
]

const currentSortOption = computed(() => sortOptions.find(o => o.value === sortBy.value) || sortOptions[0])

// ============================================
// METHODS
// ============================================
function navigateToCreate() {
  router.push({ name: 'LessonLearnedCreate' })
}

function navigateToDetail(id: string) {
  router.push({ name: 'LessonLearnedDetail', params: { id } })
}

function selectCategory(category: LessonCategory | 'all') {
  selectedCategory.value = category
  showCategoryDropdown.value = false
  currentPage.value = 1
}

function selectPriority(priority: LessonPriority | 'all') {
  selectedPriority.value = priority
  showPriorityDropdown.value = false
  currentPage.value = 1
}

function selectSort(sort: 'recent' | 'popular' | 'priority') {
  sortBy.value = sort
  showSortDropdown.value = false
}

function formatDate(dateStr: string) {
  const date = new Date(dateStr)
  const now = new Date()
  const diffDays = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60 * 24))

  if (diffDays === 0) return 'Today'
  if (diffDays === 1) return 'Yesterday'
  if (diffDays < 7) return `${diffDays} days ago`
  if (diffDays < 30) return `${Math.floor(diffDays / 7)} weeks ago`
  return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
}

function closeDropdowns(event: MouseEvent) {
  const target = event.target as HTMLElement
  if (!target.closest('.dropdown-container')) {
    showCategoryDropdown.value = false
    showPriorityDropdown.value = false
    showSortDropdown.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', closeDropdowns)
})
</script>

<template>
  <div class="lessons-learned-page min-h-screen bg-gray-50">
    <!-- Hero Section -->
    <header class="relative">
      <div class="h-[180px] w-full bg-gradient-to-br from-amber-500 via-amber-400 to-yellow-400"></div>

      <!-- Header Content -->
      <div class="absolute bottom-0 left-0 right-0 px-6 py-5">
        <div class="max-w-7xl mx-auto">
          <div class="flex items-end justify-between">
            <div>
              <h1 class="text-3xl font-bold text-white mb-2">{{ text.pageTitle }}</h1>
              <p class="text-white/90 text-sm">{{ text.pageSubtitle }}</p>
            </div>
            <button
              @click="navigateToCreate"
              class="px-4 py-2.5 bg-white text-amber-600 rounded-xl font-medium shadow-lg hover:shadow-xl transition-all flex items-center gap-2"
            >
              <i class="fas fa-plus"></i>
              {{ text.newLesson }}
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Stats Bar -->
    <div class="bg-white border-b border-gray-200 sticky top-0 z-20">
      <div class="max-w-7xl mx-auto px-6 py-4">
        <div class="flex items-center justify-between">
          <!-- Stats -->
          <div class="flex items-center gap-6">
            <div class="flex items-center gap-2">
              <div class="w-10 h-10 rounded-lg bg-amber-100 flex items-center justify-center">
                <i class="fas fa-lightbulb text-amber-600"></i>
              </div>
              <div>
                <p class="text-2xl font-bold text-gray-900">{{ stats.total }}</p>
                <p class="text-xs text-gray-500">{{ text.statsTotal }}</p>
              </div>
            </div>
            <div class="h-8 w-px bg-gray-200"></div>
            <div class="flex items-center gap-2">
              <div class="w-10 h-10 rounded-lg bg-blue-100 flex items-center justify-center">
                <i class="fas fa-calendar text-blue-600"></i>
              </div>
              <div>
                <p class="text-2xl font-bold text-gray-900">{{ stats.thisMonth }}</p>
                <p class="text-xs text-gray-500">{{ text.statsThisMonth }}</p>
              </div>
            </div>
            <div class="h-8 w-px bg-gray-200"></div>
            <div class="flex items-center gap-2">
              <div class="w-10 h-10 rounded-lg bg-red-100 flex items-center justify-center">
                <i class="fas fa-exclamation-circle text-red-600"></i>
              </div>
              <div>
                <p class="text-2xl font-bold text-gray-900">{{ stats.critical }}</p>
                <p class="text-xs text-gray-500">{{ text.statsCritical }}</p>
              </div>
            </div>
            <div class="h-8 w-px bg-gray-200"></div>
            <div class="flex items-center gap-2">
              <div class="w-10 h-10 rounded-lg bg-orange-100 flex items-center justify-center">
                <i class="fas fa-arrow-up text-orange-600"></i>
              </div>
              <div>
                <p class="text-2xl font-bold text-gray-900">{{ stats.high }}</p>
                <p class="text-xs text-gray-500">{{ text.statsHigh }}</p>
              </div>
            </div>
          </div>

          <!-- View Toggle -->
          <div class="flex items-center gap-2 bg-gray-100 rounded-lg p-1">
            <button
              @click="viewMode = 'grid'"
              :class="[
                'px-3 py-1.5 rounded-md text-sm font-medium transition-all',
                viewMode === 'grid' ? 'bg-white shadow text-gray-900' : 'text-gray-500 hover:text-gray-700'
              ]"
            >
              <i class="fas fa-th-large mr-1.5"></i>
              Grid
            </button>
            <button
              @click="viewMode = 'list'"
              :class="[
                'px-3 py-1.5 rounded-md text-sm font-medium transition-all',
                viewMode === 'list' ? 'bg-white shadow text-gray-900' : 'text-gray-500 hover:text-gray-700'
              ]"
            >
              <i class="fas fa-list mr-1.5"></i>
              List
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters Bar -->
    <div class="bg-white border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-6 py-3">
        <div class="flex items-center gap-4">
          <!-- Search -->
          <div class="flex-1 relative">
            <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-gray-400"></i>
            <input
              v-model="searchQuery"
              type="text"
              :placeholder="text.searchPlaceholder"
              class="w-full pl-10 pr-4 py-2 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent"
            />
          </div>

          <!-- Category Filter -->
          <div class="relative dropdown-container">
            <button
              @click="showCategoryDropdown = !showCategoryDropdown"
              class="px-4 py-2 border border-gray-200 rounded-lg flex items-center gap-2 hover:bg-gray-50 transition-colors min-w-[160px]"
            >
              <i :class="selectedCategory === 'all' ? 'fas fa-folder' : getCategoryIcon(selectedCategory as LessonCategory)" class="text-gray-500"></i>
              <span class="text-sm text-gray-700">
                {{ selectedCategory === 'all' ? text.allCategories : LESSON_CATEGORIES.find(c => c.value === selectedCategory)?.label }}
              </span>
              <i class="fas fa-chevron-down text-gray-400 ml-auto text-xs"></i>
            </button>
            <div
              v-if="showCategoryDropdown"
              class="absolute top-full left-0 mt-1 w-48 bg-white rounded-lg shadow-lg border border-gray-200 py-1 z-30"
            >
              <button
                @click="selectCategory('all')"
                class="w-full px-4 py-2 text-left text-sm hover:bg-gray-50 flex items-center gap-2"
              >
                <i class="fas fa-folder text-gray-400"></i>
                {{ text.allCategories }}
              </button>
              <button
                v-for="cat in LESSON_CATEGORIES"
                :key="cat.value"
                @click="selectCategory(cat.value)"
                class="w-full px-4 py-2 text-left text-sm hover:bg-gray-50 flex items-center gap-2"
              >
                <i :class="cat.icon" class="text-gray-500"></i>
                {{ cat.label }}
              </button>
            </div>
          </div>

          <!-- Priority Filter -->
          <div class="relative dropdown-container">
            <button
              @click="showPriorityDropdown = !showPriorityDropdown"
              class="px-4 py-2 border border-gray-200 rounded-lg flex items-center gap-2 hover:bg-gray-50 transition-colors min-w-[150px]"
            >
              <i :class="selectedPriority === 'all' ? 'fas fa-filter' : getPriorityIcon(selectedPriority as LessonPriority)" class="text-gray-500"></i>
              <span class="text-sm text-gray-700">
                {{ selectedPriority === 'all' ? text.allPriorities : LESSON_PRIORITIES.find(p => p.value === selectedPriority)?.label }}
              </span>
              <i class="fas fa-chevron-down text-gray-400 ml-auto text-xs"></i>
            </button>
            <div
              v-if="showPriorityDropdown"
              class="absolute top-full left-0 mt-1 w-40 bg-white rounded-lg shadow-lg border border-gray-200 py-1 z-30"
            >
              <button
                @click="selectPriority('all')"
                class="w-full px-4 py-2 text-left text-sm hover:bg-gray-50 flex items-center gap-2"
              >
                <i class="fas fa-filter text-gray-400"></i>
                {{ text.allPriorities }}
              </button>
              <button
                v-for="pri in LESSON_PRIORITIES"
                :key="pri.value"
                @click="selectPriority(pri.value)"
                class="w-full px-4 py-2 text-left text-sm hover:bg-gray-50 flex items-center gap-2"
              >
                <i :class="pri.icon" class="text-gray-500"></i>
                {{ pri.label }}
              </button>
            </div>
          </div>

          <!-- Sort -->
          <div class="relative dropdown-container">
            <button
              @click="showSortDropdown = !showSortDropdown"
              class="px-4 py-2 border border-gray-200 rounded-lg flex items-center gap-2 hover:bg-gray-50 transition-colors"
            >
              <i :class="currentSortOption?.icon" class="text-gray-500"></i>
              <span class="text-sm text-gray-700">{{ currentSortOption?.label }}</span>
              <i class="fas fa-chevron-down text-gray-400 text-xs"></i>
            </button>
            <div
              v-if="showSortDropdown"
              class="absolute top-full right-0 mt-1 w-44 bg-white rounded-lg shadow-lg border border-gray-200 py-1 z-30"
            >
              <button
                v-for="opt in sortOptions"
                :key="opt.value"
                @click="selectSort(opt.value as 'recent' | 'popular' | 'priority')"
                :class="[
                  'w-full px-4 py-2 text-left text-sm flex items-center gap-2',
                  sortBy === opt.value ? 'bg-amber-50 text-amber-700' : 'hover:bg-gray-50'
                ]"
              >
                <i :class="opt.icon"></i>
                {{ opt.label }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div class="max-w-7xl mx-auto px-6 py-6">
      <!-- Results Count -->
      <div class="flex items-center justify-between mb-4">
        <p class="text-sm text-gray-500">
          Showing <span class="font-medium text-gray-900">{{ filteredLessons.length }}</span> lessons
        </p>
      </div>

      <!-- Grid View -->
      <div v-if="viewMode === 'grid' && paginatedLessons.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="lesson in paginatedLessons"
          :key="lesson.id"
          @click="navigateToDetail(lesson.id)"
          class="bg-white rounded-2xl shadow-sm border border-gray-200 p-5 hover:shadow-md hover:border-amber-200 transition-all cursor-pointer group"
        >
          <!-- Header -->
          <div class="flex items-start justify-between mb-3">
            <div class="flex items-center gap-2">
              <span :class="['px-2 py-0.5 rounded-full text-xs font-medium border', getPriorityColor(lesson.priority)]">
                {{ lesson.priority }}
              </span>
              <span :class="['px-2 py-0.5 rounded-full text-xs font-medium', getCategoryColor(lesson.category)]">
                {{ LESSON_CATEGORIES.find(c => c.value === lesson.category)?.label }}
              </span>
            </div>
            <BookmarkButton
              :content-id="lesson.id"
              content-type="lesson"
              size="sm"
              @click.stop
            />
          </div>

          <!-- Title & Summary -->
          <h3 class="font-semibold text-gray-900 mb-2 line-clamp-2 group-hover:text-amber-600 transition-colors">
            {{ lesson.title }}
          </h3>
          <p class="text-sm text-gray-600 line-clamp-2 mb-4">
            {{ lesson.summary }}
          </p>

          <!-- Tags -->
          <div class="flex flex-wrap gap-1.5 mb-4">
            <span
              v-for="tag in lesson.tags.slice(0, 3)"
              :key="tag"
              class="px-2 py-0.5 bg-gray-100 text-gray-600 rounded text-xs"
            >
              {{ tag }}
            </span>
            <span v-if="lesson.tags.length > 3" class="px-2 py-0.5 bg-gray-100 text-gray-500 rounded text-xs">
              +{{ lesson.tags.length - 3 }}
            </span>
          </div>

          <!-- Footer -->
          <div class="flex items-center justify-between pt-3 border-t border-gray-100">
            <div class="flex items-center gap-2">
              <div class="w-7 h-7 rounded-full bg-gradient-to-br from-amber-400 to-amber-600 flex items-center justify-center text-white text-xs font-medium">
                {{ lesson.author.initials }}
              </div>
              <div>
                <p class="text-xs font-medium text-gray-900">{{ lesson.author.name }}</p>
                <p class="text-xs text-gray-500">{{ formatDate(lesson.createdAt) }}</p>
              </div>
            </div>
            <div class="flex items-center gap-3 text-xs text-gray-500">
              <span><i class="fas fa-eye mr-1"></i>{{ lesson.viewCount }}</span>
              <span><i class="fas fa-heart mr-1"></i>{{ lesson.likeCount }}</span>
              <span><i class="fas fa-comment mr-1"></i>{{ lesson.commentCount }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- List View -->
      <div v-else-if="viewMode === 'list' && paginatedLessons.length > 0" class="space-y-4">
        <div
          v-for="lesson in paginatedLessons"
          :key="lesson.id"
          @click="navigateToDetail(lesson.id)"
          class="bg-white rounded-2xl shadow-sm border border-gray-200 p-5 hover:shadow-md hover:border-amber-200 transition-all cursor-pointer group"
        >
          <div class="flex items-start gap-4">
            <!-- Left: Priority indicator -->
            <div :class="['w-1 h-full min-h-[80px] rounded-full', {
              'bg-red-500': lesson.priority === 'critical',
              'bg-orange-500': lesson.priority === 'high',
              'bg-amber-400': lesson.priority === 'medium',
              'bg-green-500': lesson.priority === 'low'
            }]"></div>

            <!-- Content -->
            <div class="flex-1">
              <div class="flex items-start justify-between">
                <div class="flex-1">
                  <!-- Badges -->
                  <div class="flex items-center gap-2 mb-2">
                    <span :class="['px-2 py-0.5 rounded-full text-xs font-medium border', getPriorityColor(lesson.priority)]">
                      {{ lesson.priority }}
                    </span>
                    <span :class="['px-2 py-0.5 rounded-full text-xs font-medium', getCategoryColor(lesson.category)]">
                      {{ LESSON_CATEGORIES.find(c => c.value === lesson.category)?.label }}
                    </span>
                  </div>

                  <!-- Title -->
                  <h3 class="font-semibold text-gray-900 mb-1 group-hover:text-amber-600 transition-colors">
                    {{ lesson.title }}
                  </h3>

                  <!-- Summary -->
                  <p class="text-sm text-gray-600 mb-3">
                    {{ lesson.summary }}
                  </p>

                  <!-- Tags -->
                  <div class="flex flex-wrap gap-1.5">
                    <span
                      v-for="tag in lesson.tags"
                      :key="tag"
                      class="px-2 py-0.5 bg-gray-100 text-gray-600 rounded text-xs"
                    >
                      {{ tag }}
                    </span>
                  </div>
                </div>

                <!-- Right: Actions & Meta -->
                <div class="flex flex-col items-end gap-3 ml-4">
                  <BookmarkButton
                    :content-id="lesson.id"
                    content-type="lesson"
                    size="sm"
                    @click.stop
                  />
                  <div class="text-right">
                    <div class="flex items-center gap-2 mb-1">
                      <div class="w-6 h-6 rounded-full bg-gradient-to-br from-amber-400 to-amber-600 flex items-center justify-center text-white text-xs font-medium">
                        {{ lesson.author.initials }}
                      </div>
                      <span class="text-xs font-medium text-gray-900">{{ lesson.author.name }}</span>
                    </div>
                    <p class="text-xs text-gray-500">{{ formatDate(lesson.createdAt) }}</p>
                  </div>
                  <div class="flex items-center gap-3 text-xs text-gray-500">
                    <span><i class="fas fa-eye mr-1"></i>{{ lesson.viewCount }}</span>
                    <span><i class="fas fa-heart mr-1"></i>{{ lesson.likeCount }}</span>
                    <span><i class="fas fa-comment mr-1"></i>{{ lesson.commentCount }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="text-center py-16">
        <div class="w-20 h-20 mx-auto mb-4 rounded-full bg-amber-100 flex items-center justify-center">
          <i class="fas fa-lightbulb text-amber-500 text-3xl"></i>
        </div>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">{{ text.noResults }}</h3>
        <p class="text-gray-500 mb-6">{{ text.noResultsDesc }}</p>
        <button
          @click="navigateToCreate"
          class="px-4 py-2 bg-amber-500 text-white rounded-lg font-medium hover:bg-amber-600 transition-colors"
        >
          <i class="fas fa-plus mr-2"></i>
          {{ text.newLesson }}
        </button>
      </div>

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="flex items-center justify-center gap-2 mt-8">
        <button
          @click="currentPage--"
          :disabled="currentPage === 1"
          class="px-3 py-2 rounded-lg border border-gray-200 text-sm font-medium disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
        >
          <i class="fas fa-chevron-left"></i>
        </button>
        <div class="flex items-center gap-1">
          <button
            v-for="page in totalPages"
            :key="page"
            @click="currentPage = page"
            :class="[
              'w-10 h-10 rounded-lg text-sm font-medium transition-colors',
              currentPage === page
                ? 'bg-amber-500 text-white'
                : 'border border-gray-200 hover:bg-gray-50'
            ]"
          >
            {{ page }}
          </button>
        </div>
        <button
          @click="currentPage++"
          :disabled="currentPage === totalPages"
          class="px-3 py-2 rounded-lg border border-gray-200 text-sm font-medium disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
        >
          <i class="fas fa-chevron-right"></i>
        </button>
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
</style>
