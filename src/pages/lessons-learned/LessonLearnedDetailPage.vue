<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import {
  CommentsSection,
  RatingStars,
  BookmarkButton,
  SocialShareButtons
} from '@/components/common'
import { useComments } from '@/composables/useComments'
import { useRatings } from '@/composables/useRatings'
import type { LessonLearned } from '@/types/lessons-learned'
import {
  getPriorityColor,
  getCategoryColor,
  getCategoryIcon,
  LESSON_CATEGORIES,
  LESSON_PRIORITIES
} from '@/types/lessons-learned'

const router = useRouter()
const route = useRoute()

// ============================================
// STATE
// ============================================
const lessonId = computed(() => route.params.id as string)
const isLoading = ref(true)
const isLiked = ref(false)
const likeCount = ref(0)

// Comments & Ratings
const {
  comments,
  isLoading: commentsLoading,
  loadComments,
  addComment
} = useComments('lesson', lessonId.value)

const {
  rating,
  submitRating,
  loadRating
} = useRatings('lesson', lessonId.value)

// ============================================
// TEXT CONSTANTS
// ============================================
const text = {
  back: 'Back',
  breadcrumbLessons: 'Lessons Learned',
  edit: 'Edit',
  context: 'Context',
  impact: 'Business Impact',
  recommendations: 'Recommendations',
  aboutSection: 'Full Details',
  relatedLessons: 'Related Lessons',
  comments: 'Discussion',
  rateLesson: 'Rate this lesson',
  views: 'views',
  likes: 'likes',
  share: 'Share',
  tags: 'Tags',
  author: 'Author'
}

// ============================================
// MOCK DATA
// ============================================
const mockLessons: Record<string, LessonLearned> = {
  '1': {
    id: '1',
    title: 'Always Document API Changes Before Deployment',
    summary: 'Learned the hard way that undocumented API changes cause major integration issues across dependent teams.',
    content: `## The Problem

During our Q3 platform migration, we made several breaking API changes without proper documentation. This led to:

- **3 critical production incidents** in the first week
- **12 dependent services** experiencing failures
- **48 hours** of emergency debugging across teams

## Root Cause Analysis

The root cause was a combination of factors:

1. **No formal change management** - API changes were merged without review
2. **Missing documentation** - Changes weren't reflected in API docs
3. **No communication** - Dependent teams weren't notified

## The Solution

We implemented a new API change management process:

1. **API Changelog** - Every change is documented in a structured changelog
2. **Breaking Change Reviews** - All breaking changes require approval from dependent team leads
3. **2-Week Notice** - Major changes are communicated 2 weeks before deployment
4. **Versioning** - All endpoints are versioned, allowing gradual migration

## Results

After implementing these changes:

- Zero API-related incidents in Q4
- 60% reduction in integration issues
- Improved trust between teams`,
    category: 'technical',
    priority: 'critical',
    tags: ['API', 'Documentation', 'Best Practices', 'DevOps', 'Communication'],
    status: 'published',
    context: 'Q3 2024 Platform Migration Project - This lesson emerged from a post-mortem analysis after experiencing significant production issues during our platform migration.',
    impact: 'Reduced integration issues by 60% in subsequent releases. Saved an estimated 200+ engineering hours per quarter that were previously spent on debugging integration issues.',
    recommendations: [
      'Create and maintain an API changelog for every service',
      'Notify dependent teams at least 2 weeks before any breaking changes',
      'Version all API endpoints to allow gradual migration',
      'Implement automated API contract testing',
      'Conduct regular API review meetings with stakeholders'
    ],
    author: { id: '1', name: 'Ahmed Hassan', initials: 'AH' },
    createdAt: '2024-01-15',
    updatedAt: '2024-01-15',
    viewCount: 234,
    likeCount: 45,
    commentCount: 12
  },
  '2': {
    id: '2',
    title: 'Clear Communication Reduces Meeting Time by 40%',
    summary: 'Implementing structured agendas and pre-reads dramatically improved our meeting efficiency.',
    content: `## The Challenge

Our team was spending an average of 15 hours per week in meetings, with many running over time and lacking clear outcomes.

## What We Changed

1. **Required Agendas** - No meeting without a shared agenda 24 hours in advance
2. **Pre-reads** - Background materials shared beforehand
3. **Timeboxing** - Each agenda item has a time limit
4. **Clear Outcomes** - Every meeting ends with documented action items

## Results

- Meetings reduced from 15 to 9 hours per week
- 95% of meetings now end on time
- Action item completion rate increased by 45%`,
    category: 'communication',
    priority: 'high',
    tags: ['Meetings', 'Productivity', 'Communication', 'Team Management'],
    status: 'published',
    context: 'Team Productivity Initiative - Part of our Q2 objective to improve team efficiency.',
    impact: 'Saved average of 5 hours per week per team member, totaling 200+ hours monthly for the department.',
    recommendations: [
      'Always share agenda 24h before meetings',
      'Define clear outcomes for each meeting',
      'Assign a timekeeper role',
      'Send meeting notes within 24 hours'
    ],
    author: { id: '2', name: 'Sarah Chen', initials: 'SC' },
    createdAt: '2024-01-10',
    updatedAt: '2024-01-12',
    viewCount: 567,
    likeCount: 89,
    commentCount: 23
  }
}

const lesson = ref<LessonLearned | null>(null)

const relatedLessons = ref([
  {
    id: '3',
    title: 'Code Reviews Should Focus on Logic, Not Style',
    category: 'process',
    priority: 'medium',
    author: { initials: 'MP', name: 'Michael Park' },
    viewCount: 345
  },
  {
    id: '4',
    title: 'Stakeholder Alignment is Critical for Project Success',
    category: 'leadership',
    priority: 'high',
    author: { initials: 'LW', name: 'Lisa Wang' },
    viewCount: 412
  },
  {
    id: '5',
    title: 'Feature Flags Enable Safer Deployments',
    category: 'technical',
    priority: 'high',
    author: { initials: 'AH', name: 'Ahmed Hassan' },
    viewCount: 289
  }
])

// ============================================
// COMPUTED
// ============================================
const categoryLabel = computed(() => {
  if (!lesson.value) return ''
  return LESSON_CATEGORIES.find(c => c.value === lesson.value!.category)?.label || lesson.value.category
})

const priorityLabel = computed(() => {
  if (!lesson.value) return ''
  return LESSON_PRIORITIES.find(p => p.value === lesson.value!.priority)?.label || lesson.value.priority
})

// ============================================
// METHODS
// ============================================
function goBack() {
  router.push({ name: 'LessonsLearned' })
}

function navigateToEdit() {
  router.push({ name: 'LessonLearnedEdit', params: { id: lessonId.value } })
}

function navigateToRelated(id: string) {
  router.push({ name: 'LessonLearnedDetail', params: { id } })
}

function toggleLike() {
  isLiked.value = !isLiked.value
  likeCount.value += isLiked.value ? 1 : -1
}

async function handleRating(stars: number) {
  await submitRating(stars)
}

function formatDate(dateStr: string) {
  const date = new Date(dateStr)
  return date.toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' })
}

// ============================================
// LIFECYCLE
// ============================================
onMounted(async () => {
  // Simulate loading
  isLoading.value = true
  await new Promise(resolve => setTimeout(resolve, 500))

  // Load lesson data
  lesson.value = mockLessons[lessonId.value] || mockLessons['1']
  likeCount.value = lesson.value?.likeCount || 0

  // Load comments and ratings
  await Promise.all([
    loadComments(),
    loadRating()
  ])

  isLoading.value = false
})
</script>

<template>
  <div class="lesson-detail-page min-h-screen bg-gray-50">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-screen">
      <div class="text-center">
        <i class="fas fa-spinner fa-spin text-4xl text-amber-500 mb-4"></i>
        <p class="text-gray-500">Loading lesson...</p>
      </div>
    </div>

    <template v-else-if="lesson">
      <!-- Hero Section -->
      <header class="relative">
        <div class="h-[200px] w-full bg-gradient-to-br from-amber-500 via-amber-400 to-yellow-400"></div>

        <!-- Header Content -->
        <div class="absolute bottom-0 left-0 right-0 px-6 py-5">
          <div class="max-w-6xl mx-auto">
            <!-- Navigation -->
            <div class="flex items-center gap-3 mb-4">
              <button
                @click="goBack"
                class="flex items-center gap-2 text-white/80 hover:text-white transition-colors text-sm"
              >
                <i class="fas fa-arrow-left"></i>
                {{ text.back }}
              </button>
              <span class="text-white/50">/</span>
              <router-link to="/lessons-learned" class="text-white/80 hover:text-white transition-colors text-sm">
                {{ text.breadcrumbLessons }}
              </router-link>
              <span class="text-white/50">/</span>
              <span class="text-white text-sm truncate max-w-[300px]">{{ lesson.title }}</span>
            </div>

            <!-- Title & Badges -->
            <div class="flex items-start justify-between gap-4">
              <div class="flex-1">
                <div class="flex items-center gap-2 mb-2">
                  <span :class="['px-2.5 py-1 rounded-full text-xs font-semibold border', getPriorityColor(lesson.priority)]">
                    {{ priorityLabel }}
                  </span>
                  <span :class="['px-2.5 py-1 rounded-full text-xs font-semibold', getCategoryColor(lesson.category)]">
                    <i :class="[getCategoryIcon(lesson.category), 'mr-1']"></i>
                    {{ categoryLabel }}
                  </span>
                </div>
                <h1 class="text-2xl md:text-3xl font-bold text-white leading-tight">
                  {{ lesson.title }}
                </h1>
              </div>

              <!-- Actions -->
              <div class="flex items-center gap-2 shrink-0">
                <BookmarkButton
                  :content-id="lesson.id"
                  content-type="lesson"
                  size="md"
                />
                <button
                  @click="navigateToEdit"
                  class="px-3 py-2 bg-white/20 backdrop-blur text-white rounded-lg hover:bg-white/30 transition-colors text-sm font-medium"
                >
                  <i class="fas fa-edit mr-1.5"></i>
                  {{ text.edit }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </header>

      <!-- Meta Bar -->
      <div class="bg-white border-b border-gray-200 sticky top-0 z-20">
        <div class="max-w-6xl mx-auto px-6 py-3">
          <div class="flex items-center justify-between">
            <!-- Author & Date -->
            <div class="flex items-center gap-4">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-full bg-gradient-to-br from-amber-400 to-amber-600 flex items-center justify-center text-white font-bold">
                  {{ lesson.author.initials }}
                </div>
                <div>
                  <p class="font-semibold text-gray-900 text-sm">{{ lesson.author.name }}</p>
                  <p class="text-xs text-gray-500">{{ formatDate(lesson.createdAt) }}</p>
                </div>
              </div>

              <div class="h-8 w-px bg-gray-200 hidden md:block"></div>

              <div class="hidden md:flex items-center gap-4 text-sm text-gray-500">
                <span><i class="fas fa-eye mr-1.5"></i>{{ lesson.viewCount }} {{ text.views }}</span>
                <span><i class="fas fa-heart mr-1.5"></i>{{ likeCount }} {{ text.likes }}</span>
                <span><i class="fas fa-comment mr-1.5"></i>{{ comments.length }} comments</span>
              </div>
            </div>

            <!-- Social Share -->
            <div class="flex items-center gap-3">
              <button
                @click="toggleLike"
                :class="[
                  'flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm font-medium transition-colors',
                  isLiked ? 'bg-red-100 text-red-600' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                ]"
              >
                <i :class="isLiked ? 'fas fa-heart' : 'far fa-heart'"></i>
                {{ isLiked ? 'Liked' : 'Like' }}
              </button>
              <SocialShareButtons
                :title="lesson.title"
                :description="lesson.summary"
                layout="horizontal"
                size="sm"
              />
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content -->
      <div class="max-w-6xl mx-auto px-6 py-8">
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <!-- Left Column: Main Content -->
          <div class="lg:col-span-2 space-y-6">
            <!-- Summary Card -->
            <div class="bg-amber-50 border border-amber-200 rounded-2xl p-5">
              <div class="flex items-start gap-3">
                <div class="w-10 h-10 rounded-lg bg-amber-100 flex items-center justify-center shrink-0">
                  <i class="fas fa-lightbulb text-amber-600"></i>
                </div>
                <div>
                  <h3 class="font-semibold text-amber-900 mb-1">Key Insight</h3>
                  <p class="text-amber-800">{{ lesson.summary }}</p>
                </div>
              </div>
            </div>

            <!-- Context -->
            <div v-if="lesson.context" class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-3 flex items-center gap-2">
                <i class="fas fa-map-marker-alt text-blue-500"></i>
                {{ text.context }}
              </h3>
              <p class="text-gray-600">{{ lesson.context }}</p>
            </div>

            <!-- Full Content -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-4 flex items-center gap-2">
                <i class="fas fa-file-alt text-teal-500"></i>
                {{ text.aboutSection }}
              </h3>
              <div class="prose prose-gray max-w-none" v-html="renderMarkdown(lesson.content)"></div>
            </div>

            <!-- Impact -->
            <div v-if="lesson.impact" class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-3 flex items-center gap-2">
                <i class="fas fa-chart-line text-green-500"></i>
                {{ text.impact }}
              </h3>
              <p class="text-gray-600">{{ lesson.impact }}</p>
            </div>

            <!-- Recommendations -->
            <div v-if="lesson.recommendations?.length" class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-4 flex items-center gap-2">
                <i class="fas fa-clipboard-check text-purple-500"></i>
                {{ text.recommendations }}
              </h3>
              <ul class="space-y-3">
                <li
                  v-for="(rec, index) in lesson.recommendations"
                  :key="index"
                  class="flex items-start gap-3"
                >
                  <div class="w-6 h-6 rounded-full bg-purple-100 flex items-center justify-center shrink-0 mt-0.5">
                    <span class="text-xs font-semibold text-purple-600">{{ index + 1 }}</span>
                  </div>
                  <p class="text-gray-700">{{ rec }}</p>
                </li>
              </ul>
            </div>

            <!-- Comments Section -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-4 flex items-center gap-2">
                <i class="fas fa-comments text-teal-500"></i>
                {{ text.comments }}
                <span class="text-xs text-gray-500 bg-gray-100 px-2 py-0.5 rounded-full ml-2">
                  {{ comments.length }}
                </span>
              </h3>
              <CommentsSection
                content-type="lesson"
                :content-id="lesson.id"
                :comments="comments"
                :is-loading="commentsLoading"
                @add-comment="addComment"
              />
            </div>
          </div>

          <!-- Right Column: Sidebar -->
          <div class="space-y-6">
            <!-- Rating Card -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-5">
              <h4 class="font-semibold text-gray-900 mb-3">{{ text.rateLesson }}</h4>
              <RatingStars
                :model-value="rating?.userRating || 0"
                :average="rating?.average"
                :count="rating?.count"
                size="lg"
                :show-count="true"
                @update:model-value="handleRating"
              />
            </div>

            <!-- Author Card -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-5">
              <h4 class="font-semibold text-gray-900 mb-4">{{ text.author }}</h4>
              <div class="flex items-center gap-3 mb-3">
                <div class="w-12 h-12 rounded-full bg-gradient-to-br from-amber-400 to-amber-600 flex items-center justify-center text-white font-bold text-lg">
                  {{ lesson.author.initials }}
                </div>
                <div>
                  <p class="font-semibold text-gray-900">{{ lesson.author.name }}</p>
                  <p class="text-sm text-gray-500">Contributor</p>
                </div>
              </div>
              <button class="w-full py-2 bg-amber-100 text-amber-700 rounded-lg text-sm font-medium hover:bg-amber-200 transition-colors">
                View Profile
              </button>
            </div>

            <!-- Tags -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-5">
              <h4 class="font-semibold text-gray-900 mb-3">{{ text.tags }}</h4>
              <div class="flex flex-wrap gap-2">
                <span
                  v-for="tag in lesson.tags"
                  :key="tag"
                  class="px-3 py-1.5 bg-gray-100 text-gray-700 rounded-lg text-sm hover:bg-gray-200 cursor-pointer transition-colors"
                >
                  {{ tag }}
                </span>
              </div>
            </div>

            <!-- Related Lessons -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-5">
              <h4 class="font-semibold text-gray-900 mb-4">{{ text.relatedLessons }}</h4>
              <div class="space-y-3">
                <div
                  v-for="related in relatedLessons"
                  :key="related.id"
                  @click="navigateToRelated(related.id)"
                  class="p-3 rounded-xl bg-gray-50 hover:bg-gray-100 cursor-pointer transition-colors"
                >
                  <div class="flex items-center gap-2 mb-1.5">
                    <span :class="['px-1.5 py-0.5 rounded text-xs font-medium', getCategoryColor(related.category as any)]">
                      {{ LESSON_CATEGORIES.find(c => c.value === related.category)?.label }}
                    </span>
                  </div>
                  <h5 class="text-sm font-medium text-gray-900 line-clamp-2 mb-2">{{ related.title }}</h5>
                  <div class="flex items-center justify-between text-xs text-gray-500">
                    <span>{{ related.author.name }}</span>
                    <span><i class="fas fa-eye mr-1"></i>{{ related.viewCount }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<script lang="ts">
// Simple markdown renderer
function renderMarkdown(content: string): string {
  if (!content) return ''

  return content
    // Headers
    .replace(/^### (.*$)/gim, '<h3 class="text-lg font-semibold text-gray-900 mt-6 mb-3">$1</h3>')
    .replace(/^## (.*$)/gim, '<h2 class="text-xl font-semibold text-gray-900 mt-8 mb-4">$1</h2>')
    .replace(/^# (.*$)/gim, '<h1 class="text-2xl font-bold text-gray-900 mt-8 mb-4">$1</h1>')
    // Bold
    .replace(/\*\*(.*)\*\*/gim, '<strong class="font-semibold">$1</strong>')
    // Italic
    .replace(/\*(.*)\*/gim, '<em>$1</em>')
    // Unordered lists
    .replace(/^\- (.*$)/gim, '<li class="ml-4 text-gray-700">$1</li>')
    // Ordered lists
    .replace(/^\d+\. (.*$)/gim, '<li class="ml-4 text-gray-700">$1</li>')
    // Line breaks
    .replace(/\n\n/gim, '</p><p class="mb-4 text-gray-700">')
    .replace(/\n/gim, '<br>')
    // Wrap in paragraph if not already wrapped
}

export default {
  methods: {
    renderMarkdown
  }
}
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.prose h2 {
  @apply text-xl font-semibold text-gray-900 mt-8 mb-4;
}

.prose h3 {
  @apply text-lg font-semibold text-gray-900 mt-6 mb-3;
}

.prose p {
  @apply mb-4 text-gray-700;
}

.prose ul {
  @apply mb-4 space-y-2;
}

.prose li {
  @apply ml-4 text-gray-700;
}

.prose strong {
  @apply font-semibold;
}
</style>
