<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useRouter, onBeforeRouteLeave } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useUIStore } from '@/stores/ui'
import { aiApi } from '@/api/ai'
import { sanitizeHtml } from '@/utils/sanitize'

const { t } = useI18n()
import type { TitleGenerationResult, ClassificationResult, AutoTagResult, SummarizationResult, SentimentResult } from '@/types/ai'
import {
  AIActionButton,
  AILoadingIndicator,
  AISuggestionChip,
  AIConfidenceBar,
  AISentimentBadge,
} from '@/components/ai'

const router = useRouter()
const uiStore = useUIStore()

// Form state
const title = ref('')
const excerpt = ref('')
const content = ref('')
const category = ref('')
const tags = ref<string[]>([])
const coverImage = ref('')
const isDraft = ref(true)
const isSubmitting = ref(false)

// Rich Text Editor state
const isMarkdownMode = ref(false)
const showPreview = ref(false)
const editorRef = ref<HTMLElement | null>(null)
const showHeadingDropdown = ref(false)

// AI Feature States - Existing
const showAITitleSuggestions = ref(false)
const titleSuggestions = ref<TitleGenerationResult | null>(null)
const isGeneratingTitles = ref(false)

const showAITagSuggestions = ref(false)
const tagSuggestions = ref<AutoTagResult | null>(null)
const isGeneratingTags = ref(false)

const showAIClassification = ref(false)
const classificationResult = ref<ClassificationResult | null>(null)
const isClassifying = ref(false)

// AI Feature States - NEW
const isSummarizing = ref(false)
const summaryResult = ref<SummarizationResult | null>(null)

const isAnalyzingSEO = ref(false)
const seoResult = ref<{ score: number; feedback: string; suggestions: string[] } | null>(null)

const isAnalyzingSentiment = ref(false)
const sentimentResult = ref<SentimentResult | null>(null)

// Form enhancement state
const touchedFields = ref<Set<string>>(new Set())
const isDragging = ref(false)
const coverImagePreview = ref<string | null>(null)
const tagInput = ref('')
const showTagSuggestions = ref(false)
const imageInputRef = ref<HTMLInputElement | null>(null)
const showUrlInput = ref(false)

// Auto-save state
const hasUnsavedChanges = ref(false)
const lastAutoSaved = ref<string | null>(null)
const autoSaveInterval = ref<ReturnType<typeof setInterval> | null>(null)
const draftKey = 'article_draft_new'

// Debounce timer for auto-suggestions
let autoSuggestTimer: ReturnType<typeof setTimeout> | null = null

// Categories (mock)
const categories = computed(() => [
  { id: '1', name: t('articles.categoryCompanyNews') },
  { id: '2', name: t('articles.categoryHRPolicies') },
  { id: '3', name: t('articles.categoryTechnology') },
  { id: '4', name: t('articles.categoryTraining') },
  { id: '5', name: t('articles.categorySports') },
  { id: '6', name: t('articles.categoryTournament') },
])

// Popular tags for autocomplete
const popularTags = ['asia-cup', 'tournament', 'saudi-arabia', 'football', 'cricket', 'sports', 'news', 'update', 'announcement', '2027']
const existingTags = ['asia-cup-2027', 'tournament-news', 'match-preview', 'team-updates', 'venue-info', 'tickets', 'schedule', 'results', 'highlights', 'interviews']

// Computed
const hasEnoughContent = computed(() => content.value.length >= 50)
const wordCount = computed(() => content.value.trim().split(/\s+/).filter(Boolean).length)
const characterCount = computed(() => content.value.length)
const readingTime = computed(() => Math.max(1, Math.ceil(wordCount.value / 200)))
const wordTarget = 300

const isAnyAIProcessing = computed(() =>
  isGeneratingTitles.value || isGeneratingTags.value || isClassifying.value ||
  isSummarizing.value || isAnalyzingSEO.value || isAnalyzingSentiment.value
)

const showAIResults = computed(() =>
  summaryResult.value || seoResult.value || sentimentResult.value
)

const contentChecklist = computed(() => [
  { label: t('articles.checklistTitleAdded'), complete: title.value.length >= 10, icon: 'fa-heading' },
  { label: t('articles.checklistCategorySelected'), complete: !!category.value, icon: 'fa-folder' },
  { label: t('articles.checklistTagsCount'), complete: tags.value.length >= 3, icon: 'fa-tags' },
  { label: t('articles.checklistExcerptWritten'), complete: excerpt.value.length >= 50, icon: 'fa-align-left' },
  { label: t('articles.checklistCoverImage'), complete: !!coverImage.value || !!coverImagePreview.value, icon: 'fa-image' },
])

const filteredExistingTags = computed(() => {
  if (!tagInput.value) return existingTags.slice(0, 5)
  const query = tagInput.value.toLowerCase()
  return existingTags.filter(t =>
    t.toLowerCase().includes(query) && !tags.value.includes(t.toLowerCase())
  ).slice(0, 5)
})

// Validation
function getFieldError(field: string): string | null {
  if (!touchedFields.value.has(field)) return null

  if (field === 'title') {
    if (!title.value) return t('articles.titleRequired')
    if (title.value.length < 10) return t('articles.titleMinLength')
    if (title.value.length > 100) return t('articles.titleMaxLength')
  }
  if (field === 'excerpt' && excerpt.value.length > 200) {
    return t('articles.excerptMaxLength')
  }
  return null
}

// Rich Text Commands
function execCommand(command: string, value: string | undefined = undefined) {
  document.execCommand(command, false, value)
  editorRef.value?.focus()
}

function isActive(command: string): boolean {
  return document.queryCommandState(command)
}

function setHeading(level: number) {
  execCommand('formatBlock', `h${level}`)
  showHeadingDropdown.value = false
}

function setParagraph() {
  execCommand('formatBlock', 'p')
  showHeadingDropdown.value = false
}

function insertLink() {
  const url = prompt('Enter URL:')
  if (url) execCommand('createLink', url)
}

function onEditorInput() {
  if (editorRef.value) {
    content.value = editorRef.value.innerHTML
  }
}

function handleKeydown(e: KeyboardEvent) {
  // Keyboard shortcuts
  if (e.ctrlKey || e.metaKey) {
    if (e.key === 'b') { e.preventDefault(); execCommand('bold') }
    if (e.key === 'i') { e.preventDefault(); execCommand('italic') }
    if (e.key === 'u') { e.preventDefault(); execCommand('underline') }
    if (e.key === 'k') { e.preventDefault(); insertLink() }
  }
}

// AI Actions - Existing
async function generateTitleSuggestions() {
  if (!hasEnoughContent.value || isGeneratingTitles.value) {
    if (!hasEnoughContent.value) {
      uiStore.showWarning(t('articles.needMoreContent'), t('articles.addMoreContentForTitles'))
    }
    return
  }

  isGeneratingTitles.value = true
  showAITitleSuggestions.value = true
  titleSuggestions.value = null

  try {
    titleSuggestions.value = await aiApi.generateTitles(content.value)
  } catch (err) {
    console.error('Title generation failed:', err)
    uiStore.showError(t('articles.aiError'), t('articles.failedGenerateTitles'))
  } finally {
    isGeneratingTitles.value = false
  }
}

async function generateTagSuggestions() {
  if (!hasEnoughContent.value || isGeneratingTags.value) {
    if (!hasEnoughContent.value) {
      uiStore.showWarning(t('articles.needMoreContent'), t('articles.addMoreContentForTags'))
    }
    return
  }

  isGeneratingTags.value = true
  showAITagSuggestions.value = true
  tagSuggestions.value = null

  try {
    tagSuggestions.value = await aiApi.autoTag(content.value)
  } catch (err) {
    console.error('Tag generation failed:', err)
    uiStore.showError(t('articles.aiError'), t('articles.failedGenerateTags'))
  } finally {
    isGeneratingTags.value = false
  }
}

async function classifyContent() {
  if (!hasEnoughContent.value || isClassifying.value) {
    if (!hasEnoughContent.value) {
      uiStore.showWarning(t('articles.needMoreContent'), t('articles.addMoreContentForClassify'))
    }
    return
  }

  isClassifying.value = true
  showAIClassification.value = true
  classificationResult.value = null

  try {
    classificationResult.value = await aiApi.classifyContent(content.value)
    if (!category.value && classificationResult.value?.primaryCategory) {
      const matchingCat = categories.value.find((c: { id: string; name: string }) =>
        c.name.toLowerCase().includes(classificationResult.value!.primaryCategory.toLowerCase())
      )
      if (matchingCat) {
        category.value = matchingCat.id
      }
    }
  } catch (err) {
    console.error('Classification failed:', err)
    uiStore.showError(t('articles.aiError'), t('articles.failedClassifyContent'))
  } finally {
    isClassifying.value = false
  }
}

// AI Actions - NEW
async function generateSummary() {
  if (!hasEnoughContent.value || isSummarizing.value) return

  isSummarizing.value = true
  try {
    summaryResult.value = await aiApi.summarizeContent(content.value, 'brief')
    // Optionally auto-fill excerpt
    if (!excerpt.value && summaryResult.value) {
      excerpt.value = summaryResult.value.summary.substring(0, 200)
      uiStore.showSuccess(t('articles.excerptGenerated'), t('articles.aiExcerptSet'))
    }
  } catch (err) {
    uiStore.showError(t('articles.aiError'), t('articles.failedGenerateSummary'))
  } finally {
    isSummarizing.value = false
  }
}

async function analyzeSEO() {
  if (!hasEnoughContent.value || isAnalyzingSEO.value) return

  isAnalyzingSEO.value = true
  try {
    let score = 0
    const suggestions: string[] = []

    // Title checks (20 pts)
    if (title.value.length >= 30 && title.value.length <= 60) {
      score += 20
    } else if (title.value.length >= 10) {
      score += 10
      suggestions.push('Optimize title length (30-60 characters)')
    } else {
      suggestions.push('Add a title (30-60 characters ideal)')
    }

    // Content length (20 pts)
    if (wordCount.value >= 300) {
      score += 20
    } else if (wordCount.value >= 150) {
      score += 10
      suggestions.push('Add more content (aim for 300+ words)')
    } else {
      suggestions.push('Content too short (aim for 300+ words)')
    }

    // Tags (15 pts)
    if (tags.value.length >= 3) {
      score += 15
    } else if (tags.value.length >= 1) {
      score += 5
      suggestions.push('Add more tags (at least 3)')
    } else {
      suggestions.push('Add relevant tags')
    }

    // Category (10 pts)
    if (category.value) {
      score += 10
    } else {
      suggestions.push('Select a category')
    }

    // Excerpt (15 pts)
    if (excerpt.value.length >= 50 && excerpt.value.length <= 160) {
      score += 15
    } else if (excerpt.value.length > 0) {
      score += 5
      suggestions.push('Optimize excerpt (50-160 characters)')
    } else {
      suggestions.push('Add an excerpt (50-160 characters)')
    }

    // Cover image (10 pts)
    if (coverImage.value || coverImagePreview.value) {
      score += 10
    } else {
      suggestions.push('Add a cover image')
    }

    // Headings (10 pts)
    if (content.value.includes('<h') || content.value.includes('##')) {
      score += 10
    } else {
      suggestions.push('Use headings to structure content')
    }

    await new Promise(r => setTimeout(r, 500))

    seoResult.value = {
      score,
      feedback: score >= 80 ? 'Excellent SEO!' :
                score >= 60 ? 'Good, some improvements possible' :
                score >= 40 ? 'Fair, consider suggestions' :
                'Needs improvement',
      suggestions
    }
  } finally {
    isAnalyzingSEO.value = false
  }
}

async function analyzeSentiment() {
  if (!hasEnoughContent.value || isAnalyzingSentiment.value) return

  isAnalyzingSentiment.value = true
  try {
    sentimentResult.value = await aiApi.analyzeSentiment(content.value)
  } catch (err) {
    uiStore.showError(t('articles.aiError'), t('articles.failedAnalyzeSentiment'))
  } finally {
    isAnalyzingSentiment.value = false
  }
}

function getSEOScoreClass(score: number): string {
  if (score >= 80) return 'text-green-600'
  if (score >= 60) return 'text-teal-600'
  if (score >= 40) return 'text-amber-600'
  return 'text-red-600'
}

// Title and Tag helpers
function selectTitle(suggestedTitle: string) {
  title.value = suggestedTitle
  showAITitleSuggestions.value = false
  uiStore.showSuccess(t('articles.titleApplied'), t('articles.aiTitleSet'))
}

function addTag(tag: string) {
  const normalizedTag = tag.toLowerCase().trim()
  if (normalizedTag && !tags.value.includes(normalizedTag)) {
    tags.value.push(normalizedTag)
  }
  tagInput.value = ''
  showTagSuggestions.value = false
}

function addCustomTag() {
  if (tagInput.value.trim()) {
    addTag(tagInput.value.trim())
  }
}

function removeTag(tag: string) {
  tags.value = tags.value.filter(t => t !== tag)
}

function handleBackspace() {
  if (!tagInput.value && tags.value.length > 0) {
    tags.value.pop()
  }
}

function hideTagSuggestions() {
  window.setTimeout(() => {
    showTagSuggestions.value = false
  }, 200)
}

function applyAllSuggestedTags() {
  if (tagSuggestions.value) {
    const tagCount = tagSuggestions.value.tags.length
    tagSuggestions.value.tags.forEach(tag => addTag(tag.tag))
    showAITagSuggestions.value = false
    uiStore.showSuccess(t('articles.tagsApplied'), t('articles.aiTagsAdded', { count: tagCount }))
  }
}

// Image handling
function handleImageDrop(e: DragEvent) {
  isDragging.value = false
  const file = e.dataTransfer?.files[0]
  if (file && file.type.startsWith('image/')) {
    processImageFile(file)
  }
}

function handleImageSelect(e: Event) {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (file) processImageFile(file)
}

function processImageFile(file: File) {
  if (file.size > 5 * 1024 * 1024) {
    uiStore.showError(t('articles.fileTooLarge'), t('articles.imageMaxSize'))
    return
  }
  const reader = new FileReader()
  reader.onload = (e) => {
    coverImagePreview.value = e.target?.result as string
    coverImage.value = '' // Clear URL when using file
  }
  reader.readAsDataURL(file)
}

function removeImage() {
  coverImagePreview.value = null
  coverImage.value = ''
}

// Auto-save
function autoSaveDraft() {
  if (!hasUnsavedChanges.value) return

  const draft = {
    title: title.value,
    excerpt: excerpt.value,
    content: content.value,
    category: category.value,
    tags: tags.value,
    coverImage: coverImage.value,
    savedAt: new Date().toISOString()
  }

  try {
    localStorage.setItem(draftKey, JSON.stringify(draft))
    lastAutoSaved.value = 'just now'
    hasUnsavedChanges.value = false
  } catch (err) {
    console.error('Auto-save failed:', err)
  }
}

function loadDraft() {
  const saved = localStorage.getItem(draftKey)
  if (saved) {
    try {
      const draft = JSON.parse(saved)
      const savedTime = new Date(draft.savedAt).toLocaleString()
      if (confirm(`Found a draft from ${savedTime}. Would you like to restore it?`)) {
        title.value = draft.title || ''
        excerpt.value = draft.excerpt || ''
        content.value = draft.content || ''
        category.value = draft.category || ''
        tags.value = draft.tags || []
        coverImage.value = draft.coverImage || ''
        uiStore.showSuccess(t('articles.draftRestored'), t('articles.previousWorkLoaded'))
      } else {
        localStorage.removeItem(draftKey)
      }
    } catch {
      localStorage.removeItem(draftKey)
    }
  }
}

// Watch for changes
watch([title, excerpt, content, category, tags, coverImage], () => {
  hasUnsavedChanges.value = true
}, { deep: true })

// Watch content for auto-suggestions
watch(content, (newContent) => {
  if (autoSuggestTimer) clearTimeout(autoSuggestTimer)

  if (newContent.length >= 100 && !title.value && !titleSuggestions.value) {
    autoSuggestTimer = setTimeout(() => {
      generateTitleSuggestions()
    }, 2000)
  }
})

// Lifecycle
onMounted(() => {
  loadDraft()
  autoSaveInterval.value = setInterval(autoSaveDraft, 30000)
})

onUnmounted(() => {
  if (autoSaveInterval.value) {
    clearInterval(autoSaveInterval.value)
  }
})

onBeforeRouteLeave((_to, _from, next) => {
  if (hasUnsavedChanges.value) {
    const answer = confirm('You have unsaved changes. Are you sure you want to leave?')
    if (!answer) {
      next(false)
      return
    }
  }
  next()
})

// Save/Submit
async function saveArticle(publish = false) {
  touchedFields.value.add('title')

  if (!title.value.trim()) {
    uiStore.showError(t('articles.titleRequired'))
    return
  }

  if (title.value.length < 10) {
    uiStore.showError(t('articles.titleMinLength'))
    return
  }

  isSubmitting.value = true

  await new Promise(resolve => setTimeout(resolve, 1000))

  if (publish) {
    uiStore.showSuccess(t('articles.articlePublished'), t('articles.articleNowLive'))
  } else {
    uiStore.showSuccess(t('articles.draftSaved'), t('articles.changesSaved'))
  }

  // Clear draft after successful save
  localStorage.removeItem(draftKey)
  hasUnsavedChanges.value = false

  isSubmitting.value = false
  router.push({ name: 'Articles' })
}

function goBack() {
  router.push({ name: 'Articles' })
}
</script>

<template>
  <div class="page-view fade-in">
    <!-- Hero Header -->
    <div class="hero-gradient relative overflow-hidden rounded-2xl mb-6">
      <!-- Decorative circles -->
      <div class="absolute top-0 end-0 w-64 h-64 bg-white/5 rounded-full -translate-y-1/2 translate-x-1/4 rtl:-translate-x-1/4"></div>
      <div class="absolute bottom-0 start-0 w-48 h-48 bg-white/5 rounded-full translate-y-1/2 -translate-x-1/4 rtl:translate-x-1/4"></div>
      <div class="absolute top-1/2 end-1/3 w-32 h-32 bg-white/10 rounded-full"></div>

      <div class="relative z-10 px-8 py-6">
        <!-- Breadcrumb -->
        <div class="flex items-center gap-2 text-white/70 text-sm mb-4">
          <router-link to="/articles" class="hover:text-white transition-colors">{{ $t('articles.title') }}</router-link>
          <i class="fas fa-chevron-right rtl:rotate-180 text-xs"></i>
          <span class="text-white">{{ $t('articles.newArticle') }}</span>
        </div>

        <!-- Title and actions row -->
        <div class="flex items-center justify-between flex-wrap gap-4">
          <div class="flex items-center gap-4">
            <button @click="goBack" class="w-10 h-10 rounded-xl bg-white/10 hover:bg-white/20 text-white flex items-center justify-center transition-all">
              <i class="fas fa-arrow-left"></i>
            </button>
            <div>
              <h1 class="text-2xl md:text-3xl font-bold text-white">{{ $t('articles.createNewArticle') }}</h1>
              <p class="text-white/70">{{ $t('articles.craftCompellingContent') }}</p>
            </div>
          </div>

          <!-- Action buttons -->
          <div class="flex items-center gap-3">
            <div v-if="hasUnsavedChanges" class="text-amber-200 text-sm flex items-center gap-1">
              <i class="fas fa-circle text-xs animate-pulse"></i>
              <span>{{ $t('articles.unsaved') }}</span>
            </div>
            <span v-if="lastAutoSaved" class="text-white/60 text-sm hidden sm:inline">
              <i class="fas fa-cloud-check me-1"></i>
              Auto-saved {{ lastAutoSaved }}
            </span>
            <button
              @click="saveArticle(false)"
              :disabled="isSubmitting"
              class="px-4 py-2 bg-transparent text-white border border-white/30 rounded-xl font-medium hover:bg-white/10 transition-all flex items-center gap-2"
            >
              <i class="fas fa-save"></i>
              <span class="hidden sm:inline">{{ $t('articles.saveDraft') }}</span>
            </button>
            <button
              @click="saveArticle(true)"
              :disabled="isSubmitting"
              class="px-4 py-2 bg-white text-teal-700 rounded-xl font-medium hover:bg-teal-50 transition-all shadow-sm flex items-center gap-2"
            >
              <i class="fas fa-paper-plane"></i>
              <span>{{ $t('articles.publish') }}</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content - Two Column Layout -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Left Column - Editor (2/3) -->
      <div class="lg:col-span-2 space-y-6">

        <!-- Title Card -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-6 py-4 border-b border-gray-100 bg-gradient-to-r from-gray-50 to-transparent">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-teal-100 flex items-center justify-center">
                <i class="fas fa-heading text-teal-600"></i>
              </div>
              <div class="flex-1">
                <h3 class="text-lg font-semibold text-gray-900">{{ $t('articles.titleLabel') }}</h3>
                <p class="text-sm text-gray-500">{{ $t('articles.titleDescription') }}</p>
              </div>
              <span class="text-xs text-gray-400">{{ title.length }}/100</span>
            </div>
          </div>
          <div class="p-6">
            <input
              v-model="title"
              type="text"
              @blur="touchedFields.add('title')"
              placeholder="Enter a compelling title..."
              :class="[
                'w-full text-2xl font-bold border-0 border-b-2 pb-2 focus:ring-0 outline-none transition-colors',
                getFieldError('title') ? 'border-red-300 focus:border-red-500' : 'border-gray-200 focus:border-teal-500'
              ]"
            >
            <p v-if="getFieldError('title')" class="mt-2 text-sm text-red-500 flex items-center gap-1">
              <i class="fas fa-exclamation-circle"></i>
              {{ getFieldError('title') }}
            </p>
          </div>
        </div>

        <!-- Content Editor Card -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-6 py-4 border-b border-gray-100 bg-gradient-to-r from-gray-50 to-transparent">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-purple-100 flex items-center justify-center">
                  <i class="fas fa-edit text-purple-600"></i>
                </div>
                <div>
                  <h3 class="text-lg font-semibold text-gray-900">{{ $t('articles.contentLabel') }}</h3>
                  <p class="text-sm text-gray-500">{{ $t('articles.contentDescription') }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Rich Text Toolbar -->
          <div class="flex items-center gap-1 p-3 bg-gray-50 border-b border-gray-200 flex-wrap">
            <!-- Text Formatting -->
            <button @click="execCommand('bold')" :class="['toolbar-btn', isActive('bold') && 'active']" title="Bold (Ctrl+B)">
              <i class="fas fa-bold"></i>
            </button>
            <button @click="execCommand('italic')" :class="['toolbar-btn', isActive('italic') && 'active']" title="Italic (Ctrl+I)">
              <i class="fas fa-italic"></i>
            </button>
            <button @click="execCommand('underline')" :class="['toolbar-btn', isActive('underline') && 'active']" title="Underline (Ctrl+U)">
              <i class="fas fa-underline"></i>
            </button>
            <button @click="execCommand('strikethrough')" :class="['toolbar-btn', isActive('strikeThrough') && 'active']" title="Strikethrough">
              <i class="fas fa-strikethrough"></i>
            </button>

            <div class="w-px h-6 bg-gray-300 mx-1"></div>

            <!-- Headings Dropdown -->
            <div class="relative">
              <button @click="showHeadingDropdown = !showHeadingDropdown" class="toolbar-btn flex items-center gap-1 px-2">
                <i class="fas fa-heading"></i>
                <i class="fas fa-chevron-down text-xs"></i>
              </button>
              <div v-if="showHeadingDropdown" class="absolute top-full start-0 mt-1 bg-white rounded-lg shadow-lg border border-gray-200 py-1 z-20 min-w-32">
                <button v-for="h in [1,2,3,4]" :key="h" @click="setHeading(h)" class="w-full px-3 py-2 text-start text-sm hover:bg-teal-50 hover:text-teal-700">
                  {{ $t('articles.heading') }} {{ h }}
                </button>
                <button @click="setParagraph" class="w-full px-3 py-2 text-start text-sm hover:bg-teal-50 hover:text-teal-700">{{ $t('articles.paragraph') }}</button>
              </div>
            </div>

            <!-- Lists -->
            <button @click="execCommand('insertUnorderedList')" class="toolbar-btn" title="Bullet List">
              <i class="fas fa-list-ul"></i>
            </button>
            <button @click="execCommand('insertOrderedList')" class="toolbar-btn" title="Numbered List">
              <i class="fas fa-list-ol"></i>
            </button>

            <div class="w-px h-6 bg-gray-300 mx-1"></div>

            <!-- Insert -->
            <button @click="insertLink" class="toolbar-btn" title="Insert Link (Ctrl+K)">
              <i class="fas fa-link"></i>
            </button>
            <button @click="execCommand('formatBlock', 'blockquote')" class="toolbar-btn" title="Block Quote">
              <i class="fas fa-quote-right"></i>
            </button>

            <div class="flex-1"></div>

            <!-- Mode Toggles -->
            <button
              @click="isMarkdownMode = !isMarkdownMode"
              :class="['toolbar-btn px-3', isMarkdownMode && 'active']"
              title="Toggle Markdown"
            >
              <i class="fab fa-markdown me-1"></i>
              <span class="text-xs hidden sm:inline">MD</span>
            </button>
            <button
              @click="showPreview = !showPreview"
              :class="['toolbar-btn px-3', showPreview && 'active']"
              title="Toggle Preview"
            >
              <i class="fas fa-eye me-1"></i>
              <span class="text-xs hidden sm:inline">{{ $t('articles.preview') }}</span>
            </button>
          </div>

          <!-- Editor Content -->
          <div :class="['editor-content-area', showPreview ? 'grid grid-cols-2 divide-x divide-gray-200' : '']">
            <!-- Editor Pane -->
            <div class="editor-pane">
              <div
                v-if="!isMarkdownMode"
                ref="editorRef"
                contenteditable="true"
                @input="onEditorInput"
                @keydown="handleKeydown"
                class="prose prose-teal max-w-none p-4 min-h-[400px] focus:outline-none"
                :data-placeholder="$t('articles.startWriting')"
              ></div>
              <textarea
                v-else
                v-model="content"
                class="w-full p-4 min-h-[400px] font-mono text-sm focus:outline-none resize-none"
                :placeholder="$t('articles.writeMarkdown')"
              ></textarea>
            </div>

            <!-- Preview Pane -->
            <div v-if="showPreview" class="preview-pane p-4 bg-gray-50 overflow-auto max-h-[500px]">
              <div class="prose prose-teal max-w-none text-sm" v-html="sanitizeHtml(content) || `<p class='text-gray-400'>${$t('articles.previewAppearHere')}</p>`"></div>
            </div>
          </div>

          <!-- Editor Footer -->
          <div class="px-4 py-3 bg-gray-50 border-t border-gray-200 flex items-center justify-between text-sm">
            <div class="flex items-center gap-4">
              <span class="text-gray-600">
                <i class="fas fa-font me-1"></i>
                {{ wordCount }} / {{ wordTarget }} {{ $t('articles.words') }}
                <i v-if="wordCount >= wordTarget" class="fas fa-check-circle text-green-500 ms-1"></i>
              </span>
              <span class="text-gray-500 hidden sm:inline">{{ characterCount }} {{ $t('articles.characters') }}</span>
              <span class="text-gray-500">
                <i class="fas fa-clock me-1"></i>
                ~{{ readingTime }} {{ $t('articles.minRead', { min: readingTime }) }}
              </span>
            </div>
            <div class="flex items-center gap-2">
              <span v-if="hasEnoughContent" class="text-teal-600 text-xs flex items-center gap-1">
                <i class="fas fa-check-circle"></i>
                AI available
              </span>
              <span v-else class="text-gray-400 text-xs flex items-center gap-1">
                <i class="fas fa-info-circle"></i>
                +{{ Math.max(0, 50 - content.length) }} chars for AI
              </span>
            </div>
          </div>
        </div>

        <!-- Excerpt Card -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-6 py-4 border-b border-gray-100 bg-gradient-to-r from-gray-50 to-transparent">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-amber-100 flex items-center justify-center">
                  <i class="fas fa-align-left text-amber-600"></i>
                </div>
                <div>
                  <h3 class="text-lg font-semibold text-gray-900">{{ $t('articles.excerptLabel') }}</h3>
                  <p class="text-sm text-gray-500">{{ $t('articles.excerptDescription') }}</p>
                </div>
              </div>
              <div class="flex items-center gap-2">
                <span class="text-xs text-gray-400">{{ excerpt.length }}/200</span>
                <button
                  v-if="hasEnoughContent && !excerpt"
                  @click="generateSummary"
                  class="text-xs text-teal-600 hover:text-teal-700 flex items-center gap-1"
                >
                  <i class="fas fa-wand-magic-sparkles"></i>
                  {{ $t('common.generate') }}
                </button>
              </div>
            </div>
          </div>
          <div class="p-6">
            <textarea
              v-model="excerpt"
              @blur="touchedFields.add('excerpt')"
              :placeholder="$t('articles.excerptPlaceholder')"
              rows="3"
              :class="[
                'w-full px-4 py-3 border rounded-xl focus:ring-2 focus:ring-teal-200 outline-none resize-none transition-colors',
                getFieldError('excerpt') ? 'border-red-300' : 'border-gray-200 focus:border-teal-500'
              ]"
            ></textarea>
            <p v-if="getFieldError('excerpt')" class="mt-2 text-sm text-red-500 flex items-center gap-1">
              <i class="fas fa-exclamation-circle"></i>
              {{ getFieldError('excerpt') }}
            </p>
          </div>
        </div>
      </div>

      <!-- Right Column - Sidebar (1/3) -->
      <div class="space-y-6">

        <!-- AI Assistant Panel -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-6 py-4 bg-gradient-to-r from-teal-50 to-transparent border-b border-teal-100">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center">
                  <i class="fas fa-wand-magic-sparkles text-white"></i>
                </div>
                <div>
                  <h3 class="text-lg font-semibold text-gray-900">{{ $t('ai.assistant') }}</h3>
                  <p class="text-sm text-gray-500">{{ $t('ai.poweredByIntalio') }}</p>
                </div>
              </div>
              <AILoadingIndicator v-if="isAnyAIProcessing" variant="pulse" size="sm" :show-text="false" />
            </div>
          </div>

          <div class="p-4 space-y-3">
            <!-- AI Quick Actions Grid -->
            <div class="grid grid-cols-2 gap-2">
              <AIActionButton
                label="Titles"
                icon="fas fa-heading"
                variant="secondary"
                size="sm"
                :loading="isGeneratingTitles"
                :disabled="!hasEnoughContent"
                @click="generateTitleSuggestions"
              />
              <AIActionButton
                label="Tags"
                icon="fas fa-tags"
                variant="secondary"
                size="sm"
                :loading="isGeneratingTags"
                :disabled="!hasEnoughContent"
                @click="generateTagSuggestions"
              />
              <AIActionButton
                label="Classify"
                icon="fas fa-layer-group"
                variant="secondary"
                size="sm"
                :loading="isClassifying"
                :disabled="!hasEnoughContent"
                @click="classifyContent"
              />
              <AIActionButton
                label="Summary"
                icon="fas fa-compress"
                variant="secondary"
                size="sm"
                :loading="isSummarizing"
                :disabled="!hasEnoughContent"
                @click="generateSummary"
              />
              <AIActionButton
                label="SEO"
                icon="fas fa-chart-line"
                variant="secondary"
                size="sm"
                :loading="isAnalyzingSEO"
                :disabled="!hasEnoughContent"
                @click="analyzeSEO"
              />
              <AIActionButton
                label="Tone"
                icon="fas fa-face-smile"
                variant="secondary"
                size="sm"
                :loading="isAnalyzingSentiment"
                :disabled="!hasEnoughContent"
                @click="analyzeSentiment"
              />
            </div>

            <!-- AI Results -->
            <Transition name="slide-fade">
              <div v-if="showAIResults" class="space-y-3 mt-4 pt-4 border-t border-gray-100">
                <!-- Summary Result -->
                <div v-if="summaryResult" class="p-3 bg-teal-50 rounded-xl">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-medium text-teal-700">{{ $t('articles.summary') }}</span>
                    <button @click="summaryResult = null" class="text-teal-400 hover:text-teal-600">
                      <i class="fas fa-times text-sm"></i>
                    </button>
                  </div>
                  <p class="text-sm text-teal-800">{{ summaryResult.summary }}</p>
                </div>

                <!-- SEO Result -->
                <div v-if="seoResult" class="p-3 bg-gray-50 rounded-xl">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-medium text-gray-700">{{ $t('articles.seoScore') }}</span>
                    <button @click="seoResult = null" class="text-gray-400 hover:text-gray-600">
                      <i class="fas fa-times text-sm"></i>
                    </button>
                  </div>
                  <div class="flex items-center gap-3 mb-2">
                    <span :class="[getSEOScoreClass(seoResult.score), 'text-2xl font-bold']">
                      {{ seoResult.score }}
                    </span>
                    <span class="text-gray-500">/100</span>
                  </div>
                  <AIConfidenceBar :value="seoResult.score / 100" :show-label="false" size="sm" />
                  <p class="text-xs text-gray-500 mt-2">{{ seoResult.feedback }}</p>
                  <ul v-if="seoResult.suggestions.length" class="mt-2 space-y-1">
                    <li v-for="(s, i) in seoResult.suggestions.slice(0, 3)" :key="i" class="text-xs text-gray-600 flex items-start gap-1">
                      <i class="fas fa-lightbulb text-amber-500 mt-0.5"></i>
                      {{ s }}
                    </li>
                  </ul>
                </div>

                <!-- Sentiment Result -->
                <div v-if="sentimentResult" class="p-3 bg-purple-50 rounded-xl">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-medium text-purple-700">{{ $t('articles.contentTone') }}</span>
                    <button @click="sentimentResult = null" class="text-purple-400 hover:text-purple-600">
                      <i class="fas fa-times text-sm"></i>
                    </button>
                  </div>
                  <AISentimentBadge
                    :sentiment="sentimentResult.overall"
                    :score="sentimentResult.score"
                    :confidence="sentimentResult.confidence"
                  />
                </div>
              </div>
            </Transition>

            <!-- AI Title Suggestions (inline) -->
            <Transition name="slide-fade">
              <div v-if="showAITitleSuggestions" class="mt-4 pt-4 border-t border-gray-100">
                <div class="flex items-center justify-between mb-3">
                  <h4 class="text-sm font-medium text-gray-700">
                    <i class="fas fa-lightbulb text-amber-500 me-1"></i>
                    {{ $t('ai.titleSuggestions') }}
                  </h4>
                  <button @click="showAITitleSuggestions = false" class="text-gray-400 hover:text-gray-600">
                    <i class="fas fa-times"></i>
                  </button>
                </div>
                <div v-if="isGeneratingTitles" class="py-4 text-center">
                  <AILoadingIndicator variant="dots" :text="$t('common.generating')" />
                </div>
                <div v-else-if="titleSuggestions" class="space-y-2">
                  <button
                    v-for="(suggestion, idx) in titleSuggestions.suggestions"
                    :key="idx"
                    @click="selectTitle(suggestion.title)"
                    class="w-full text-start p-2 bg-white rounded-lg border border-gray-100 hover:border-teal-300 hover:bg-teal-50 transition-all text-sm"
                  >
                    {{ suggestion.title }}
                  </button>
                </div>
              </div>
            </Transition>

            <!-- AI Tag Suggestions (inline) -->
            <Transition name="slide-fade">
              <div v-if="showAITagSuggestions" class="mt-4 pt-4 border-t border-gray-100">
                <div class="flex items-center justify-between mb-3">
                  <h4 class="text-sm font-medium text-gray-700">
                    <i class="fas fa-tags text-purple-500 me-1"></i>
                    {{ $t('ai.tagSuggestions') }}
                  </h4>
                  <button @click="showAITagSuggestions = false" class="text-gray-400 hover:text-gray-600">
                    <i class="fas fa-times"></i>
                  </button>
                </div>
                <div v-if="isGeneratingTags" class="py-4 text-center">
                  <AILoadingIndicator variant="dots" :text="$t('common.generating')" />
                </div>
                <div v-else-if="tagSuggestions" class="space-y-2">
                  <div class="flex flex-wrap gap-1">
                    <AISuggestionChip
                      v-for="tagItem in tagSuggestions.tags"
                      :key="tagItem.tag"
                      :label="tagItem.tag"
                      :confidence="tagItem.confidence"
                      :selected="tags.includes(tagItem.tag.toLowerCase())"
                      @click="addTag(tagItem.tag)"
                    />
                  </div>
                  <button @click="applyAllSuggestedTags" class="text-xs text-teal-600 hover:text-teal-700">
                    <i class="fas fa-plus-circle me-1"></i>{{ $t('common.addAll') }}
                  </button>
                </div>
              </div>
            </Transition>

            <!-- AI Classification (inline) -->
            <Transition name="slide-fade">
              <div v-if="showAIClassification" class="mt-4 pt-4 border-t border-gray-100">
                <div class="flex items-center justify-between mb-3">
                  <h4 class="text-sm font-medium text-gray-700">
                    <i class="fas fa-layer-group text-blue-500 me-1"></i>
                    {{ $t('ai.classification') }}
                  </h4>
                  <button @click="showAIClassification = false" class="text-gray-400 hover:text-gray-600">
                    <i class="fas fa-times"></i>
                  </button>
                </div>
                <div v-if="isClassifying" class="py-4 text-center">
                  <AILoadingIndicator variant="spinner" :text="$t('ai.classifying')" />
                </div>
                <div v-else-if="classificationResult" class="space-y-2">
                  <div class="p-2 bg-teal-50 rounded-lg">
                    <p class="text-sm font-semibold text-teal-700">{{ classificationResult.primaryCategory }}</p>
                    <p class="text-xs text-teal-600">{{ Math.round(classificationResult.confidence * 100) }}% {{ $t('articles.match') }}</p>
                  </div>
                </div>
              </div>
            </Transition>
          </div>
        </div>

        <!-- Article Settings Card -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-6 py-4 border-b border-gray-100 bg-gradient-to-r from-gray-50 to-transparent">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-blue-100 flex items-center justify-center">
                <i class="fas fa-cog text-blue-600"></i>
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900">{{ $t('settings.title') }}</h3>
                <p class="text-sm text-gray-500">{{ $t('articles.categoryAndTags') }}</p>
              </div>
            </div>
          </div>
          <div class="p-4 space-y-4">
            <!-- Category -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">{{ $t('articles.category') }}</label>
              <select v-model="category" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl focus:ring-2 focus:ring-teal-200 focus:border-teal-500 outline-none">
                <option value="">{{ $t('articles.selectCategory') }}</option>
                <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                  {{ cat.name }}
                </option>
              </select>
            </div>

            <!-- Tags with Autocomplete -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">{{ $t('articles.tags') }}</label>
              <div class="relative">
                <div class="flex flex-wrap gap-2 p-3 border border-gray-200 rounded-xl bg-white focus-within:border-teal-500 focus-within:ring-2 focus-within:ring-teal-200 min-h-[48px]">
                  <span
                    v-for="tag in tags"
                    :key="tag"
                    class="inline-flex items-center gap-1 px-2 py-1 bg-teal-50 text-teal-700 border border-teal-200 rounded-full text-xs font-medium"
                  >
                    {{ tag }}
                    <button @click="removeTag(tag)" class="hover:text-red-500">
                      <i class="fas fa-times text-xs"></i>
                    </button>
                  </span>
                  <input
                    v-model="tagInput"
                    @keydown.enter.prevent="addCustomTag"
                    @keydown.backspace="handleBackspace"
                    @focus="showTagSuggestions = true"
                    @blur="hideTagSuggestions"
                    type="text"
                    :placeholder="$t('articles.addTags')"
                    class="flex-1 min-w-[100px] outline-none text-sm"
                  >
                </div>

                <!-- Autocomplete Dropdown -->
                <Transition name="dropdown">
                  <div
                    v-if="showTagSuggestions && filteredExistingTags.length > 0"
                    class="absolute top-full start-0 end-0 mt-1 bg-white border border-gray-200 rounded-xl shadow-lg z-20 max-h-40 overflow-auto"
                  >
                    <button
                      v-for="suggestion in filteredExistingTags"
                      :key="suggestion"
                      @mousedown.prevent="addTag(suggestion)"
                      class="w-full px-4 py-2 text-start text-sm hover:bg-teal-50 hover:text-teal-700 transition-colors flex items-center gap-2"
                    >
                      <i class="fas fa-tag text-gray-400 text-xs"></i>
                      {{ suggestion }}
                    </button>
                  </div>
                </Transition>
              </div>

              <!-- Popular Tags -->
              <div class="mt-2 flex flex-wrap gap-1">
                <span class="text-xs text-gray-500">{{ $t('articles.popular') }}:</span>
                <button
                  v-for="tag in popularTags.slice(0, 4)"
                  :key="tag"
                  @click="addTag(tag)"
                  :disabled="tags.includes(tag.toLowerCase())"
                  class="text-xs text-teal-600 hover:text-teal-700 disabled:text-gray-400 disabled:cursor-not-allowed"
                >
                  #{{ tag }}
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Cover Image Card -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-6 py-4 border-b border-gray-100 bg-gradient-to-r from-gray-50 to-transparent">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-pink-100 flex items-center justify-center">
                <i class="fas fa-image text-pink-600"></i>
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900">{{ $t('articles.coverImage') }}</h3>
                <p class="text-sm text-gray-500">{{ $t('articles.addFeaturedImage') }}</p>
              </div>
            </div>
          </div>
          <div class="p-4">
            <div
              @dragover.prevent="isDragging = true"
              @dragleave="isDragging = false"
              @drop.prevent="handleImageDrop"
              @click="imageInputRef?.click()"
              :class="[
                'relative border-2 border-dashed rounded-xl p-4 text-center transition-all cursor-pointer',
                isDragging ? 'border-teal-400 bg-teal-50' : 'border-gray-200 hover:border-gray-300',
                coverImagePreview || coverImage ? 'p-2' : ''
              ]"
            >
              <input
                ref="imageInputRef"
                type="file"
                accept="image/*"
                class="hidden"
                @change="handleImageSelect"
              >

              <!-- Preview -->
              <div v-if="coverImagePreview || coverImage" class="relative group">
                <img
                  :src="coverImagePreview || coverImage"
                  alt="Cover preview"
                  class="w-full h-32 object-cover rounded-lg"
                >
                <div class="absolute inset-0 bg-black/50 opacity-0 group-hover:opacity-100 transition-opacity rounded-lg flex items-center justify-center gap-2">
                  <button @click.stop="imageInputRef?.click()" class="px-2 py-1 bg-white text-gray-700 rounded text-xs font-medium">
                    Change
                  </button>
                  <button @click.stop="removeImage" class="px-2 py-1 bg-white text-red-600 rounded text-xs font-medium">
                    Remove
                  </button>
                </div>
              </div>

              <!-- Upload Prompt -->
              <div v-else class="space-y-2 py-4">
                <div class="w-10 h-10 mx-auto rounded-xl bg-gray-100 flex items-center justify-center">
                  <i class="fas fa-cloud-upload-alt text-gray-400"></i>
                </div>
                <div>
                  <p class="text-sm font-medium text-gray-700">{{ $t('articles.dropImageOrClick') }}</p>
                  <p class="text-xs text-gray-500">{{ $t('articles.imageFormats') }}</p>
                </div>
              </div>
            </div>

            <!-- URL Input -->
            <div class="mt-3">
              <button @click="showUrlInput = !showUrlInput" class="text-xs text-teal-600 hover:text-teal-700">
                <i class="fas fa-link me-1"></i>
                {{ $t('articles.orUseUrl') }}
              </button>
              <Transition name="slide-fade">
                <input
                  v-if="showUrlInput"
                  v-model="coverImage"
                  type="url"
                  placeholder="https://example.com/image.jpg"
                  class="w-full mt-2 px-3 py-2 border border-gray-200 rounded-lg text-sm focus:ring-2 focus:ring-teal-200 focus:border-teal-500 outline-none"
                >
              </Transition>
            </div>
          </div>
        </div>

        <!-- Quality Insights Card -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-6 py-4 border-b border-gray-100 bg-gradient-to-r from-gray-50 to-transparent">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-emerald-100 flex items-center justify-center">
                <i class="fas fa-check-double text-emerald-600"></i>
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900">{{ $t('articles.quality') }}</h3>
                <p class="text-sm text-gray-500">{{ $t('articles.contentChecklist') }}</p>
              </div>
            </div>
          </div>
          <div class="p-4 space-y-4">
            <!-- Reading Time -->
            <div class="flex items-center justify-between">
              <span class="text-sm text-gray-600">{{ $t('articles.readingTime') }}</span>
              <span class="font-semibold text-gray-900">~{{ readingTime }} min</span>
            </div>

            <!-- Word Count Progress -->
            <div>
              <div class="flex items-center justify-between mb-1">
                <span class="text-sm text-gray-600">{{ $t('articles.wordCount') }}</span>
                <span class="text-sm text-gray-500">{{ wordCount }} / {{ wordTarget }}</span>
              </div>
              <div class="h-2 bg-gray-100 rounded-full overflow-hidden">
                <div
                  class="h-full rounded-full transition-all duration-300"
                  :class="wordCount >= wordTarget ? 'bg-green-500' : 'bg-teal-500'"
                  :style="{ width: `${Math.min(100, (wordCount / wordTarget) * 100)}%` }"
                ></div>
              </div>
            </div>

            <!-- Checklist -->
            <div class="space-y-2 pt-2 border-t border-gray-100">
              <div v-for="item in contentChecklist" :key="item.label" class="flex items-center gap-2">
                <i :class="[
                  item.complete ? 'fas fa-check-circle text-green-500' : 'far fa-circle text-gray-300',
                  'text-sm'
                ]"></i>
                <span :class="item.complete ? 'text-gray-700' : 'text-gray-400'" class="text-sm">
                  {{ item.label }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Hero Gradient */
.hero-gradient {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 50%, #0f766e 100%);
}

/* Toolbar Button */
.toolbar-btn {
  @apply w-8 h-8 rounded-lg flex items-center justify-center text-gray-600 hover:bg-gray-200 hover:text-gray-900 transition-all;
}

.toolbar-btn.active {
  @apply bg-teal-100 text-teal-700;
}

/* Editor placeholder */
[contenteditable]:empty:before {
  content: attr(data-placeholder);
  color: #9ca3af;
  pointer-events: none;
}

/* Transitions */
.slide-fade-enter-active,
.slide-fade-leave-active {
  transition: all 0.3s ease;
}

.slide-fade-enter-from,
.slide-fade-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.2s ease;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-5px);
}

/* Fade In Animation */
.fade-in {
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
