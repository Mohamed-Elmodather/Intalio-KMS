<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useUIStore } from '@/stores/ui'
import { aiApi } from '@/api/ai'
import type { TitleGenerationResult, ClassificationResult, AutoTagResult } from '@/types/ai'
import {
  AIActionButton,
  AILoadingIndicator,
  AISuggestionChip,
  AIConfidenceBar,
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

// AI Feature States
const showAITitleSuggestions = ref(false)
const titleSuggestions = ref<TitleGenerationResult | null>(null)
const isGeneratingTitles = ref(false)

const showAITagSuggestions = ref(false)
const tagSuggestions = ref<AutoTagResult | null>(null)
const isGeneratingTags = ref(false)

const showAIClassification = ref(false)
const classificationResult = ref<ClassificationResult | null>(null)
const isClassifying = ref(false)

// Debounce timer for auto-suggestions
let autoSuggestTimer: ReturnType<typeof setTimeout> | null = null

// Categories (mock)
const categories = [
  { id: '1', name: 'Company News' },
  { id: '2', name: 'HR & Policies' },
  { id: '3', name: 'Technology' },
  { id: '4', name: 'Training' },
  { id: '5', name: 'Sports & Athletics' },
  { id: '6', name: 'Tournament Coverage' },
]

// Computed
const hasEnoughContent = computed(() => content.value.length >= 50)

// AI Actions
async function generateTitleSuggestions() {
  if (!hasEnoughContent.value || isGeneratingTitles.value) {
    if (!hasEnoughContent.value) {
      uiStore.showWarning('Need more content', 'Add at least 50 characters of content to generate titles')
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
    uiStore.showError('AI Error', 'Failed to generate title suggestions')
  } finally {
    isGeneratingTitles.value = false
  }
}

async function generateTagSuggestions() {
  if (!hasEnoughContent.value || isGeneratingTags.value) {
    if (!hasEnoughContent.value) {
      uiStore.showWarning('Need more content', 'Add at least 50 characters of content to generate tags')
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
    uiStore.showError('AI Error', 'Failed to generate tag suggestions')
  } finally {
    isGeneratingTags.value = false
  }
}

async function classifyContent() {
  if (!hasEnoughContent.value || isClassifying.value) {
    if (!hasEnoughContent.value) {
      uiStore.showWarning('Need more content', 'Add at least 50 characters of content to classify')
    }
    return
  }

  isClassifying.value = true
  showAIClassification.value = true
  classificationResult.value = null

  try {
    classificationResult.value = await aiApi.classifyContent(content.value)
    // Auto-select the primary category if none selected
    if (!category.value && classificationResult.value?.primaryCategory) {
      const matchingCat = categories.find(c =>
        c.name.toLowerCase().includes(classificationResult.value!.primaryCategory.toLowerCase())
      )
      if (matchingCat) {
        category.value = matchingCat.id
      }
    }
  } catch (err) {
    console.error('Classification failed:', err)
    uiStore.showError('AI Error', 'Failed to classify content')
  } finally {
    isClassifying.value = false
  }
}

function selectTitle(suggestedTitle: string) {
  title.value = suggestedTitle
  showAITitleSuggestions.value = false
  uiStore.showSuccess('Title applied', 'AI-suggested title has been set')
}

function addTag(tag: string) {
  const normalizedTag = tag.toLowerCase().trim()
  if (!tags.value.includes(normalizedTag)) {
    tags.value.push(normalizedTag)
  }
}

function removeTag(tag: string) {
  tags.value = tags.value.filter(t => t !== tag)
}

function applyAllSuggestedTags() {
  if (tagSuggestions.value) {
    tagSuggestions.value.tags.forEach(t => addTag(t.tag))
    showAITagSuggestions.value = false
    uiStore.showSuccess('Tags applied', `${tagSuggestions.value.tags.length} AI-suggested tags added`)
  }
}

// Watch content for auto-suggestions
watch(content, (newContent) => {
  if (autoSuggestTimer) clearTimeout(autoSuggestTimer)

  if (newContent.length >= 100 && !title.value && !titleSuggestions.value) {
    // Auto-suggest titles after user stops typing for 2 seconds
    autoSuggestTimer = setTimeout(() => {
      generateTitleSuggestions()
    }, 2000)
  }
})

async function saveArticle(publish = false) {
  if (!title.value.trim()) {
    uiStore.showError('Title is required')
    return
  }

  isSubmitting.value = true

  // Simulate API call
  await new Promise(resolve => setTimeout(resolve, 1000))

  if (publish) {
    uiStore.showSuccess('Article published', 'Your article is now live')
  } else {
    uiStore.showSuccess('Draft saved', 'Your changes have been saved')
  }

  isSubmitting.value = false
  router.push({ name: 'Articles' })
}

function goBack() {
  router.push({ name: 'Articles' })
}
</script>

<template>
  <div class="max-w-4xl mx-auto space-y-6 fade-in">
    <!-- Header -->
    <div class="flex items-center justify-between flex-wrap gap-4">
      <div class="flex items-center gap-4">
        <button @click="goBack" class="btn-icon btn-ghost">
          <i class="fas fa-arrow-left"></i>
        </button>
        <div>
          <h1 class="text-2xl font-bold text-gray-900">New Article</h1>
          <p class="text-gray-500">Create a new knowledge article</p>
        </div>
      </div>
      <div class="flex items-center gap-3">
        <span v-if="isDraft" class="badge badge-secondary">Draft</span>
        <button @click="saveArticle(false)" :disabled="isSubmitting" class="btn btn-secondary">
          <i class="fas fa-save"></i>
          <span>Save Draft</span>
        </button>
        <button @click="saveArticle(true)" :disabled="isSubmitting" class="btn btn-primary">
          <i class="fas fa-paper-plane"></i>
          <span>Publish</span>
        </button>
      </div>
    </div>

    <!-- AI Assistant Bar -->
    <div class="bg-gradient-to-r from-teal-50 to-transparent border border-teal-100 rounded-xl p-4">
      <div class="flex items-center justify-between flex-wrap gap-3">
        <div class="flex items-center gap-2">
          <div class="w-8 h-8 rounded-lg bg-teal-100 flex items-center justify-center">
            <i class="fas fa-wand-magic-sparkles text-teal-600"></i>
          </div>
          <div>
            <h3 class="text-sm font-semibold text-gray-800">AI Writing Assistant</h3>
            <p class="text-xs text-gray-500">Let AI help you create better content</p>
          </div>
        </div>
        <div class="flex items-center gap-2">
          <AIActionButton
            label="Generate Titles"
            icon="fas fa-heading"
            variant="secondary"
            size="sm"
            :loading="isGeneratingTitles"
            :disabled="!hasEnoughContent"
            @click="generateTitleSuggestions"
          />
          <AIActionButton
            label="Suggest Tags"
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
        </div>
      </div>

      <!-- AI Title Suggestions -->
      <Transition
        enter-active-class="transition-all duration-300 ease-out"
        enter-from-class="opacity-0 -translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition-all duration-200 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
      >
        <div v-if="showAITitleSuggestions" class="mt-4 pt-4 border-t border-teal-100">
          <div class="flex items-center justify-between mb-3">
            <h4 class="text-sm font-medium text-gray-700">
              <i class="fas fa-lightbulb text-amber-500 mr-1"></i>
              AI Title Suggestions
            </h4>
            <button @click="showAITitleSuggestions = false" class="text-gray-400 hover:text-gray-600">
              <i class="fas fa-times"></i>
            </button>
          </div>

          <div v-if="isGeneratingTitles" class="py-4 text-center">
            <AILoadingIndicator variant="dots" text="Generating titles..." />
          </div>

          <div v-else-if="titleSuggestions" class="space-y-2">
            <button
              v-for="(suggestion, idx) in titleSuggestions.suggestions"
              :key="idx"
              @click="selectTitle(suggestion.title)"
              class="w-full text-left p-3 bg-white rounded-lg border border-gray-100 hover:border-teal-300 hover:bg-teal-50 transition-all group"
            >
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-800 group-hover:text-teal-700">{{ suggestion.title }}</span>
                <div class="flex items-center gap-2">
                  <span class="text-xs text-gray-400 capitalize">{{ suggestion.style }}</span>
                  <span class="text-xs text-teal-600 bg-teal-100 px-2 py-0.5 rounded-full">
                    {{ Math.round(suggestion.score * 100) }}%
                  </span>
                </div>
              </div>
            </button>
          </div>
        </div>
      </Transition>

      <!-- AI Tag Suggestions -->
      <Transition
        enter-active-class="transition-all duration-300 ease-out"
        enter-from-class="opacity-0 -translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition-all duration-200 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
      >
        <div v-if="showAITagSuggestions" class="mt-4 pt-4 border-t border-teal-100">
          <div class="flex items-center justify-between mb-3">
            <h4 class="text-sm font-medium text-gray-700">
              <i class="fas fa-tags text-purple-500 mr-1"></i>
              AI Tag Suggestions
            </h4>
            <button @click="showAITagSuggestions = false" class="text-gray-400 hover:text-gray-600">
              <i class="fas fa-times"></i>
            </button>
          </div>

          <div v-if="isGeneratingTags" class="py-4 text-center">
            <AILoadingIndicator variant="dots" text="Generating tags..." />
          </div>

          <div v-else-if="tagSuggestions" class="space-y-3">
            <div class="flex flex-wrap gap-2">
              <AISuggestionChip
                v-for="tagItem in tagSuggestions.tags"
                :key="tagItem.tag"
                :label="tagItem.tag"
                :confidence="tagItem.confidence"
                :selected="tags.includes(tagItem.tag.toLowerCase())"
                @click="addTag(tagItem.tag)"
              />
            </div>
            <button
              @click="applyAllSuggestedTags"
              class="text-xs text-teal-600 hover:text-teal-700 font-medium"
            >
              <i class="fas fa-plus-circle mr-1"></i>
              Add all suggested tags
            </button>
          </div>
        </div>
      </Transition>

      <!-- AI Classification -->
      <Transition
        enter-active-class="transition-all duration-300 ease-out"
        enter-from-class="opacity-0 -translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition-all duration-200 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
      >
        <div v-if="showAIClassification" class="mt-4 pt-4 border-t border-teal-100">
          <div class="flex items-center justify-between mb-3">
            <h4 class="text-sm font-medium text-gray-700">
              <i class="fas fa-layer-group text-blue-500 mr-1"></i>
              AI Content Classification
            </h4>
            <button @click="showAIClassification = false" class="text-gray-400 hover:text-gray-600">
              <i class="fas fa-times"></i>
            </button>
          </div>

          <div v-if="isClassifying" class="py-4 text-center">
            <AILoadingIndicator variant="spinner" text="Classifying content..." />
          </div>

          <div v-else-if="classificationResult" class="space-y-3">
            <div class="p-3 bg-white rounded-lg border border-gray-100">
              <div class="flex items-center justify-between mb-2">
                <span class="text-sm font-medium text-gray-700">Primary Category</span>
                <span class="text-xs text-teal-600 bg-teal-100 px-2 py-0.5 rounded-full">
                  {{ Math.round(classificationResult.confidence * 100) }}% match
                </span>
              </div>
              <p class="text-lg font-semibold text-teal-600">{{ classificationResult.primaryCategory }}</p>
            </div>
            <div class="space-y-2">
              <p class="text-xs font-medium text-gray-500 uppercase">Other possible categories:</p>
              <div v-for="cat in classificationResult.categories.slice(1)" :key="cat.name" class="flex items-center justify-between text-sm">
                <span class="text-gray-600">{{ cat.name }}</span>
                <AIConfidenceBar :value="cat.confidence" size="sm" :show-value="true" :show-label="false" class="w-24" />
              </div>
            </div>
          </div>
        </div>
      </Transition>
    </div>

    <!-- Editor -->
    <div class="card p-6 space-y-6">
      <!-- Title -->
      <div>
        <div class="flex items-center justify-between mb-2">
          <label class="block text-sm font-medium text-gray-700">Title</label>
          <button
            v-if="hasEnoughContent && !title"
            @click="generateTitleSuggestions"
            class="text-xs text-teal-600 hover:text-teal-700 flex items-center gap-1"
          >
            <i class="fas fa-wand-magic-sparkles"></i>
            Generate with AI
          </button>
        </div>
        <input
          v-model="title"
          type="text"
          placeholder="Enter article title..."
          class="w-full text-2xl font-bold border-0 border-b border-gray-200 pb-2 focus:border-teal-500 focus:ring-0 outline-none"
        >
      </div>

      <!-- Excerpt -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">Excerpt</label>
        <textarea
          v-model="excerpt"
          placeholder="Write a brief summary..."
          rows="2"
          class="input resize-none"
        ></textarea>
      </div>

      <!-- Category & Tags -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <div class="flex items-center justify-between mb-2">
            <label class="block text-sm font-medium text-gray-700">Category</label>
            <button
              v-if="hasEnoughContent && !category"
              @click="classifyContent"
              class="text-xs text-teal-600 hover:text-teal-700 flex items-center gap-1"
            >
              <i class="fas fa-wand-magic-sparkles"></i>
              Auto-detect
            </button>
          </div>
          <select v-model="category" class="input">
            <option value="">Select category...</option>
            <option v-for="cat in categories" :key="cat.id" :value="cat.id">
              {{ cat.name }}
            </option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Cover Image URL</label>
          <input
            v-model="coverImage"
            type="url"
            placeholder="https://example.com/image.jpg"
            class="input"
          >
        </div>
      </div>

      <!-- Tags -->
      <div>
        <div class="flex items-center justify-between mb-2">
          <label class="block text-sm font-medium text-gray-700">Tags</label>
          <button
            v-if="hasEnoughContent"
            @click="generateTagSuggestions"
            class="text-xs text-teal-600 hover:text-teal-700 flex items-center gap-1"
          >
            <i class="fas fa-wand-magic-sparkles"></i>
            Suggest tags
          </button>
        </div>
        <div class="flex flex-wrap gap-2 mb-2">
          <span
            v-for="tag in tags"
            :key="tag"
            class="inline-flex items-center gap-1 px-3 py-1 bg-gray-100 text-gray-700 rounded-full text-sm"
          >
            {{ tag }}
            <button @click="removeTag(tag)" class="hover:text-red-500">
              <i class="fas fa-times text-xs"></i>
            </button>
          </span>
          <span v-if="tags.length === 0" class="text-sm text-gray-400">No tags added yet</span>
        </div>
      </div>

      <!-- Content Editor -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">Content</label>
        <div class="border border-gray-200 rounded-xl overflow-hidden">
          <!-- Toolbar -->
          <div class="flex items-center gap-1 p-2 bg-gray-50 border-b border-gray-200 flex-wrap">
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-bold"></i></button>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-italic"></i></button>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-underline"></i></button>
            <div class="w-px h-6 bg-gray-200 mx-1"></div>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-heading"></i></button>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-list-ul"></i></button>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-list-ol"></i></button>
            <div class="w-px h-6 bg-gray-200 mx-1"></div>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-link"></i></button>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-image"></i></button>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-code"></i></button>
            <div class="flex-1"></div>
            <!-- AI Content Indicator -->
            <div v-if="hasEnoughContent" class="flex items-center gap-1 text-xs text-teal-600">
              <i class="fas fa-check-circle"></i>
              <span>AI features available</span>
            </div>
            <div v-else class="flex items-center gap-1 text-xs text-gray-400">
              <i class="fas fa-info-circle"></i>
              <span>Add more content for AI features</span>
            </div>
          </div>
          <!-- Editor Area -->
          <textarea
            v-model="content"
            placeholder="Start writing your article..."
            rows="15"
            class="w-full p-4 text-gray-800 outline-none resize-none"
          ></textarea>
        </div>
        <p class="mt-2 text-xs text-gray-400">{{ content.length }} characters</p>
      </div>
    </div>
  </div>
</template>
