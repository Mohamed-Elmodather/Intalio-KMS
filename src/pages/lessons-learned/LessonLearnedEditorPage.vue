<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { AILoadingIndicator, AISuggestionChip } from '@/components/ai'
import type { LessonLearned, LessonPriority, LessonCategory } from '@/types/lessons-learned'
import { LESSON_CATEGORIES, LESSON_PRIORITIES, getPriorityColor, getCategoryColor } from '@/types/lessons-learned'

const router = useRouter()
const route = useRoute()

// ============================================
// STATE
// ============================================
const isEditMode = computed(() => !!route.params.id)
const lessonId = computed(() => route.params.id as string)
const isLoading = ref(false)
const isSaving = ref(false)
const showPreview = ref(false)
const hasUnsavedChanges = ref(false)

// Form fields
const title = ref('')
const summary = ref('')
const content = ref('')
const category = ref<LessonCategory>('technical')
const priority = ref<LessonPriority>('medium')
const context = ref('')
const impact = ref('')
const recommendations = ref<string[]>([''])
const tags = ref<string[]>([])
const tagInput = ref('')
const status = ref<'draft' | 'published'>('draft')

// AI Features
const isGeneratingTitle = ref(false)
const isGeneratingTags = ref(false)
const suggestedTitles = ref<string[]>([])
const suggestedTags = ref<string[]>([])
const showTitleSuggestions = ref(false)
const showTagSuggestions = ref(false)

// Validation
const errors = ref<Record<string, string>>({})

// ============================================
// TEXT CONSTANTS
// ============================================
const text = {
  createTitle: 'New Lesson Learned',
  editTitle: 'Edit Lesson Learned',
  back: 'Back',
  saveDraft: 'Save Draft',
  publish: 'Publish',
  preview: 'Preview',
  editMode: 'Edit',
  titleLabel: 'Title',
  titlePlaceholder: 'What did you learn?',
  summaryLabel: 'Summary',
  summaryPlaceholder: 'Brief description (max 200 characters)',
  categoryLabel: 'Category',
  priorityLabel: 'Priority',
  contentLabel: 'Full Details',
  contentPlaceholder: 'Describe the lesson in detail. You can use markdown formatting.',
  contextLabel: 'Context (Optional)',
  contextPlaceholder: 'Where or when did you learn this?',
  impactLabel: 'Business Impact (Optional)',
  impactPlaceholder: 'What was the impact of applying this lesson?',
  recommendationsLabel: 'Recommendations (Optional)',
  recommendationPlaceholder: 'Add an action item or recommendation',
  addRecommendation: 'Add Recommendation',
  tagsLabel: 'Tags',
  tagsPlaceholder: 'Add tags...',
  statusLabel: 'Status',
  draft: 'Draft',
  published: 'Published',
  aiSuggestTitle: 'Suggest Title',
  aiSuggestTags: 'Suggest Tags',
  required: 'Required',
  characterCount: 'characters',
  saving: 'Saving...',
  publishing: 'Publishing...'
}

// ============================================
// MOCK DATA FOR EDIT MODE
// ============================================
const mockLesson: LessonLearned = {
  id: '1',
  title: 'Always Document API Changes Before Deployment',
  summary: 'Learned the hard way that undocumented API changes cause major integration issues across dependent teams.',
  content: `## The Problem

During our Q3 platform migration, we made several breaking API changes without proper documentation.

## The Solution

We implemented a new API change management process with changelogs and versioning.`,
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
}

// ============================================
// COMPUTED
// ============================================
const pageTitle = computed(() => isEditMode.value ? text.editTitle : text.createTitle)

const summaryCharCount = computed(() => summary.value.length)
const summaryMaxChars = 200

const isFormValid = computed(() => {
  return title.value.trim() !== '' &&
    summary.value.trim() !== '' &&
    content.value.trim() !== ''
})

const activeRecommendations = computed(() => recommendations.value.filter(r => r.trim() !== ''))

// ============================================
// METHODS
// ============================================
function goBack() {
  if (hasUnsavedChanges.value) {
    if (confirm('You have unsaved changes. Are you sure you want to leave?')) {
      router.push({ name: 'LessonsLearned' })
    }
  } else {
    router.push({ name: 'LessonsLearned' })
  }
}

function validateForm(): boolean {
  errors.value = {}

  if (!title.value.trim()) {
    errors.value.title = 'Title is required'
  }
  if (!summary.value.trim()) {
    errors.value.summary = 'Summary is required'
  }
  if (summary.value.length > summaryMaxChars) {
    errors.value.summary = `Summary must be ${summaryMaxChars} characters or less`
  }
  if (!content.value.trim()) {
    errors.value.content = 'Content is required'
  }

  return Object.keys(errors.value).length === 0
}

async function saveDraft() {
  if (!validateForm()) return

  isSaving.value = true
  status.value = 'draft'

  // Simulate save
  await new Promise(resolve => setTimeout(resolve, 1000))

  isSaving.value = false
  hasUnsavedChanges.value = false

  // Show success and redirect
  router.push({ name: 'LessonsLearned' })
}

async function publish() {
  if (!validateForm()) return

  isSaving.value = true
  status.value = 'published'

  // Simulate save
  await new Promise(resolve => setTimeout(resolve, 1000))

  isSaving.value = false
  hasUnsavedChanges.value = false

  // Redirect to detail page
  const newId = isEditMode.value ? lessonId.value : 'new-' + Date.now()
  router.push({ name: 'LessonLearnedDetail', params: { id: newId } })
}

function addRecommendation() {
  recommendations.value.push('')
}

function removeRecommendation(index: number) {
  recommendations.value.splice(index, 1)
  if (recommendations.value.length === 0) {
    recommendations.value.push('')
  }
}

function addTag() {
  const tag = tagInput.value.trim()
  if (tag && !tags.value.includes(tag)) {
    tags.value.push(tag)
    tagInput.value = ''
    hasUnsavedChanges.value = true
  }
}

function removeTag(index: number) {
  tags.value.splice(index, 1)
  hasUnsavedChanges.value = true
}

function addSuggestedTag(tag: string) {
  if (!tags.value.includes(tag)) {
    tags.value.push(tag)
    hasUnsavedChanges.value = true
  }
  // Remove from suggestions
  suggestedTags.value = suggestedTags.value.filter(t => t !== tag)
}

function selectSuggestedTitle(suggestedTitle: string) {
  title.value = suggestedTitle
  showTitleSuggestions.value = false
  hasUnsavedChanges.value = true
}

async function generateTitleSuggestions() {
  isGeneratingTitle.value = true

  // Simulate AI processing
  await new Promise(resolve => setTimeout(resolve, 1500))

  suggestedTitles.value = [
    'Key Learnings from ' + (context.value || 'Recent Project'),
    'Best Practices: ' + (category.value.charAt(0).toUpperCase() + category.value.slice(1)) + ' Insights',
    'How to Improve ' + (category.value === 'technical' ? 'Development' : 'Team') + ' Processes'
  ]
  showTitleSuggestions.value = true
  isGeneratingTitle.value = false
}

async function generateTagSuggestions() {
  isGeneratingTags.value = true

  // Simulate AI processing
  await new Promise(resolve => setTimeout(resolve, 1200))

  const categoryTags: Record<LessonCategory, string[]> = {
    technical: ['Development', 'Architecture', 'Code Quality', 'Performance', 'Security'],
    process: ['Workflow', 'Automation', 'Efficiency', 'Agile', 'Best Practices'],
    communication: ['Team', 'Meetings', 'Documentation', 'Collaboration', 'Feedback'],
    leadership: ['Management', 'Strategy', 'Decision Making', 'Mentoring', 'Vision'],
    project: ['Planning', 'Risk Management', 'Delivery', 'Stakeholders', 'Timeline'],
    other: ['General', 'Tips', 'Insights', 'Learning', 'Growth']
  }

  suggestedTags.value = categoryTags[category.value].filter(t => !tags.value.includes(t))
  showTagSuggestions.value = true
  isGeneratingTags.value = false
}

// Watch for changes
watch([title, summary, content, category, priority, context, impact, tags], () => {
  hasUnsavedChanges.value = true
})

watch(recommendations, () => {
  hasUnsavedChanges.value = true
}, { deep: true })

// ============================================
// LIFECYCLE
// ============================================
onMounted(async () => {
  if (isEditMode.value) {
    isLoading.value = true

    // Simulate loading existing lesson
    await new Promise(resolve => setTimeout(resolve, 500))

    // Populate form with mock data
    title.value = mockLesson.title
    summary.value = mockLesson.summary
    content.value = mockLesson.content
    category.value = mockLesson.category
    priority.value = mockLesson.priority
    context.value = mockLesson.context || ''
    impact.value = mockLesson.impact || ''
    recommendations.value = mockLesson.recommendations?.length ? [...mockLesson.recommendations] : ['']
    tags.value = [...mockLesson.tags]
    status.value = mockLesson.status

    isLoading.value = false
    hasUnsavedChanges.value = false
  }
})
</script>

<template>
  <div class="lesson-editor-page min-h-screen bg-gray-50">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-screen">
      <div class="text-center">
        <i class="fas fa-spinner fa-spin text-4xl text-amber-500 mb-4"></i>
        <p class="text-gray-500">Loading...</p>
      </div>
    </div>

    <template v-else>
      <!-- Header -->
      <header class="bg-white border-b border-gray-200 sticky top-0 z-20">
        <div class="max-w-4xl mx-auto px-6 py-4">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-4">
              <button
                @click="goBack"
                class="p-2 text-gray-500 hover:text-gray-700 hover:bg-gray-100 rounded-lg transition-colors"
              >
                <i class="fas fa-arrow-left"></i>
              </button>
              <div>
                <h1 class="text-xl font-bold text-gray-900">{{ pageTitle }}</h1>
                <p class="text-sm text-gray-500">Capture your professional insights</p>
              </div>
            </div>

            <div class="flex items-center gap-3">
              <button
                @click="showPreview = !showPreview"
                :class="[
                  'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
                  showPreview
                    ? 'bg-amber-100 text-amber-700'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                ]"
              >
                <i :class="showPreview ? 'fas fa-edit' : 'fas fa-eye'" class="mr-1.5"></i>
                {{ showPreview ? text.editMode : text.preview }}
              </button>
              <button
                @click="saveDraft"
                :disabled="isSaving || !isFormValid"
                class="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg text-sm font-medium hover:bg-gray-200 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <i class="fas fa-save mr-1.5"></i>
                {{ isSaving && status === 'draft' ? text.saving : text.saveDraft }}
              </button>
              <button
                @click="publish"
                :disabled="isSaving || !isFormValid"
                class="px-4 py-2 bg-amber-500 text-white rounded-lg text-sm font-medium hover:bg-amber-600 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <i class="fas fa-paper-plane mr-1.5"></i>
                {{ isSaving && status === 'published' ? text.publishing : text.publish }}
              </button>
            </div>
          </div>
        </div>
      </header>

      <!-- Main Content -->
      <div class="max-w-4xl mx-auto px-6 py-8">
        <!-- Preview Mode -->
        <div v-if="showPreview" class="bg-white rounded-2xl shadow-sm border border-gray-200 p-8">
          <div class="flex items-center gap-2 mb-4">
            <span :class="['px-2.5 py-1 rounded-full text-xs font-semibold border', getPriorityColor(priority)]">
              {{ LESSON_PRIORITIES.find(p => p.value === priority)?.label }}
            </span>
            <span :class="['px-2.5 py-1 rounded-full text-xs font-semibold', getCategoryColor(category)]">
              {{ LESSON_CATEGORIES.find(c => c.value === category)?.label }}
            </span>
          </div>

          <h1 class="text-2xl font-bold text-gray-900 mb-4">{{ title || 'Untitled Lesson' }}</h1>

          <div class="bg-amber-50 border border-amber-200 rounded-xl p-4 mb-6">
            <p class="text-amber-800">{{ summary || 'No summary provided' }}</p>
          </div>

          <div v-if="context" class="mb-6">
            <h3 class="font-semibold text-gray-900 mb-2">Context</h3>
            <p class="text-gray-600">{{ context }}</p>
          </div>

          <div class="prose prose-gray max-w-none mb-6">
            <div v-html="content.replace(/\n/g, '<br>')"></div>
          </div>

          <div v-if="impact" class="mb-6">
            <h3 class="font-semibold text-gray-900 mb-2">Business Impact</h3>
            <p class="text-gray-600">{{ impact }}</p>
          </div>

          <div v-if="activeRecommendations.length" class="mb-6">
            <h3 class="font-semibold text-gray-900 mb-3">Recommendations</h3>
            <ul class="space-y-2">
              <li v-for="(rec, index) in activeRecommendations" :key="index" class="flex items-start gap-2">
                <span class="w-5 h-5 rounded-full bg-purple-100 flex items-center justify-center text-xs font-semibold text-purple-600 shrink-0">
                  {{ index + 1 }}
                </span>
                <span class="text-gray-700">{{ rec }}</span>
              </li>
            </ul>
          </div>

          <div v-if="tags.length" class="flex flex-wrap gap-2">
            <span
              v-for="tag in tags"
              :key="tag"
              class="px-3 py-1.5 bg-gray-100 text-gray-700 rounded-lg text-sm"
            >
              {{ tag }}
            </span>
          </div>
        </div>

        <!-- Edit Mode -->
        <div v-else class="space-y-6">
          <!-- Title -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
            <div class="flex items-center justify-between mb-2">
              <label class="block text-sm font-semibold text-gray-900">
                {{ text.titleLabel }}
                <span class="text-red-500 ml-1">*</span>
              </label>
              <button
                @click="generateTitleSuggestions"
                :disabled="isGeneratingTitle"
                class="text-xs text-amber-600 hover:text-amber-700 font-medium flex items-center gap-1"
              >
                <i :class="isGeneratingTitle ? 'fas fa-spinner fa-spin' : 'fas fa-wand-magic-sparkles'"></i>
                {{ text.aiSuggestTitle }}
              </button>
            </div>
            <input
              v-model="title"
              type="text"
              :placeholder="text.titlePlaceholder"
              :class="[
                'w-full px-4 py-3 border rounded-xl text-lg focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-colors',
                errors.title ? 'border-red-300 bg-red-50' : 'border-gray-200'
              ]"
            />
            <p v-if="errors.title" class="mt-1 text-sm text-red-500">{{ errors.title }}</p>

            <!-- Title Suggestions -->
            <div v-if="showTitleSuggestions && suggestedTitles.length" class="mt-3 space-y-2">
              <p class="text-xs text-gray-500 font-medium">AI Suggestions:</p>
              <div class="flex flex-wrap gap-2">
                <AISuggestionChip
                  v-for="suggestion in suggestedTitles"
                  :key="suggestion"
                  :label="suggestion"
                  @click="selectSuggestedTitle(suggestion)"
                />
              </div>
            </div>
          </div>

          <!-- Summary -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
            <div class="flex items-center justify-between mb-2">
              <label class="block text-sm font-semibold text-gray-900">
                {{ text.summaryLabel }}
                <span class="text-red-500 ml-1">*</span>
              </label>
              <span :class="['text-xs', summaryCharCount > summaryMaxChars ? 'text-red-500' : 'text-gray-500']">
                {{ summaryCharCount }}/{{ summaryMaxChars }} {{ text.characterCount }}
              </span>
            </div>
            <textarea
              v-model="summary"
              :placeholder="text.summaryPlaceholder"
              rows="3"
              :class="[
                'w-full px-4 py-3 border rounded-xl focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-colors resize-none',
                errors.summary ? 'border-red-300 bg-red-50' : 'border-gray-200'
              ]"
            ></textarea>
            <p v-if="errors.summary" class="mt-1 text-sm text-red-500">{{ errors.summary }}</p>
          </div>

          <!-- Category & Priority -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Category -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <label class="block text-sm font-semibold text-gray-900 mb-3">
                {{ text.categoryLabel }}
                <span class="text-red-500 ml-1">*</span>
              </label>
              <div class="grid grid-cols-2 gap-2">
                <button
                  v-for="cat in LESSON_CATEGORIES"
                  :key="cat.value"
                  @click="category = cat.value"
                  :class="[
                    'p-3 rounded-xl border-2 text-left transition-all',
                    category === cat.value
                      ? 'border-amber-500 bg-amber-50'
                      : 'border-gray-200 hover:border-gray-300'
                  ]"
                >
                  <i :class="[cat.icon, 'mr-2', category === cat.value ? 'text-amber-600' : 'text-gray-500']"></i>
                  <span :class="category === cat.value ? 'text-amber-900 font-medium' : 'text-gray-700'">
                    {{ cat.label }}
                  </span>
                </button>
              </div>
            </div>

            <!-- Priority -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <label class="block text-sm font-semibold text-gray-900 mb-3">
                {{ text.priorityLabel }}
                <span class="text-red-500 ml-1">*</span>
              </label>
              <div class="space-y-2">
                <button
                  v-for="pri in LESSON_PRIORITIES"
                  :key="pri.value"
                  @click="priority = pri.value"
                  :class="[
                    'w-full p-3 rounded-xl border-2 text-left transition-all flex items-center gap-3',
                    priority === pri.value
                      ? 'border-amber-500 bg-amber-50'
                      : 'border-gray-200 hover:border-gray-300'
                  ]"
                >
                  <span :class="['w-3 h-3 rounded-full', {
                    'bg-red-500': pri.value === 'critical',
                    'bg-orange-500': pri.value === 'high',
                    'bg-amber-400': pri.value === 'medium',
                    'bg-green-500': pri.value === 'low'
                  }]"></span>
                  <span :class="priority === pri.value ? 'text-amber-900 font-medium' : 'text-gray-700'">
                    {{ pri.label }}
                  </span>
                </button>
              </div>
            </div>
          </div>

          <!-- Content -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
            <label class="block text-sm font-semibold text-gray-900 mb-2">
              {{ text.contentLabel }}
              <span class="text-red-500 ml-1">*</span>
            </label>
            <p class="text-xs text-gray-500 mb-3">Supports markdown formatting</p>
            <textarea
              v-model="content"
              :placeholder="text.contentPlaceholder"
              rows="12"
              :class="[
                'w-full px-4 py-3 border rounded-xl focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-colors font-mono text-sm',
                errors.content ? 'border-red-300 bg-red-50' : 'border-gray-200'
              ]"
            ></textarea>
            <p v-if="errors.content" class="mt-1 text-sm text-red-500">{{ errors.content }}</p>
          </div>

          <!-- Context -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
            <label class="block text-sm font-semibold text-gray-900 mb-2">
              {{ text.contextLabel }}
            </label>
            <input
              v-model="context"
              type="text"
              :placeholder="text.contextPlaceholder"
              class="w-full px-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-colors"
            />
          </div>

          <!-- Impact -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
            <label class="block text-sm font-semibold text-gray-900 mb-2">
              {{ text.impactLabel }}
            </label>
            <textarea
              v-model="impact"
              :placeholder="text.impactPlaceholder"
              rows="3"
              class="w-full px-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-colors resize-none"
            ></textarea>
          </div>

          <!-- Recommendations -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
            <label class="block text-sm font-semibold text-gray-900 mb-3">
              {{ text.recommendationsLabel }}
            </label>
            <div class="space-y-3">
              <div
                v-for="(rec, index) in recommendations"
                :key="index"
                class="flex items-center gap-3"
              >
                <span class="w-6 h-6 rounded-full bg-purple-100 flex items-center justify-center text-xs font-semibold text-purple-600 shrink-0">
                  {{ index + 1 }}
                </span>
                <input
                  v-model="recommendations[index]"
                  type="text"
                  :placeholder="text.recommendationPlaceholder"
                  class="flex-1 px-4 py-2.5 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-colors"
                />
                <button
                  v-if="recommendations.length > 1"
                  @click="removeRecommendation(index)"
                  class="p-2 text-gray-400 hover:text-red-500 transition-colors"
                >
                  <i class="fas fa-times"></i>
                </button>
              </div>
            </div>
            <button
              @click="addRecommendation"
              class="mt-3 text-sm text-amber-600 hover:text-amber-700 font-medium flex items-center gap-1"
            >
              <i class="fas fa-plus"></i>
              {{ text.addRecommendation }}
            </button>
          </div>

          <!-- Tags -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
            <div class="flex items-center justify-between mb-3">
              <label class="block text-sm font-semibold text-gray-900">
                {{ text.tagsLabel }}
              </label>
              <button
                @click="generateTagSuggestions"
                :disabled="isGeneratingTags"
                class="text-xs text-amber-600 hover:text-amber-700 font-medium flex items-center gap-1"
              >
                <i :class="isGeneratingTags ? 'fas fa-spinner fa-spin' : 'fas fa-wand-magic-sparkles'"></i>
                {{ text.aiSuggestTags }}
              </button>
            </div>

            <!-- Existing Tags -->
            <div v-if="tags.length" class="flex flex-wrap gap-2 mb-3">
              <span
                v-for="(tag, index) in tags"
                :key="tag"
                class="px-3 py-1.5 bg-amber-100 text-amber-800 rounded-lg text-sm flex items-center gap-2"
              >
                {{ tag }}
                <button @click="removeTag(index)" class="text-amber-600 hover:text-amber-800">
                  <i class="fas fa-times text-xs"></i>
                </button>
              </span>
            </div>

            <!-- Tag Input -->
            <div class="flex gap-2">
              <input
                v-model="tagInput"
                type="text"
                :placeholder="text.tagsPlaceholder"
                @keydown.enter.prevent="addTag"
                class="flex-1 px-4 py-2.5 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-colors"
              />
              <button
                @click="addTag"
                :disabled="!tagInput.trim()"
                class="px-4 py-2.5 bg-gray-100 text-gray-700 rounded-lg font-medium hover:bg-gray-200 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Add
              </button>
            </div>

            <!-- Tag Suggestions -->
            <div v-if="showTagSuggestions && suggestedTags.length" class="mt-3 space-y-2">
              <p class="text-xs text-gray-500 font-medium">AI Suggestions:</p>
              <div class="flex flex-wrap gap-2">
                <button
                  v-for="tag in suggestedTags"
                  :key="tag"
                  @click="addSuggestedTag(tag)"
                  class="px-3 py-1.5 bg-gray-100 text-gray-700 rounded-lg text-sm hover:bg-amber-100 hover:text-amber-800 transition-colors"
                >
                  <i class="fas fa-plus text-xs mr-1"></i>
                  {{ tag }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.prose {
  @apply text-gray-700;
}
</style>
