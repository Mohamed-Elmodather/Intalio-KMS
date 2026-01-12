<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { articlesApi } from '@/api'
import { aiApi } from '@/api/ai'
import type { Article } from '@/types'
import type { SummarizationResult, TranslationResult, NERResult, SupportedLanguage } from '@/types/ai'
import { SUPPORTED_LANGUAGES } from '@/types/ai'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import {
  AIActionButton,
  AIResultCard,
  AILoadingIndicator,
  AITranslateDropdown,
  AIEntityHighlight,
  AISentimentBadge,
  AIConfidenceBar,
} from '@/components/ai'

const route = useRoute()
const router = useRouter()

const article = ref<Article | null>(null)
const isLoading = ref(true)
const error = ref<string | null>(null)

// AI Feature States
const showAIPanel = ref(false)
const activeAITab = ref<'summary' | 'translate' | 'entities' | 'insights'>('summary')

// Summarization
const summaryResult = ref<SummarizationResult | null>(null)
const isSummarizing = ref(false)
const summaryType = ref<'brief' | 'detailed' | 'bullet'>('brief')

// Translation
const translationResult = ref<TranslationResult | null>(null)
const isTranslating = ref(false)
const targetLanguage = ref<SupportedLanguage>('ar')

// Entity Extraction
const entitiesResult = ref<NERResult | null>(null)
const isExtractingEntities = ref(false)
const showEntitiesInContent = ref(false)

// Sentiment Analysis (for insights)
const sentimentResult = ref<{ overall: 'positive' | 'neutral' | 'negative'; score: number; confidence: number } | null>(null)
const isAnalyzingSentiment = ref(false)

// Key Takeaways (generated from summary)
const keyTakeaways = ref<string[]>([])

// Computed
const articleContent = computed(() => article.value?.content || '')
const articlePlainText = computed(() => {
  // Strip HTML tags for AI processing
  const div = document.createElement('div')
  div.innerHTML = articleContent.value
  return div.textContent || div.innerText || ''
})

// AI Actions
async function generateSummary() {
  if (!articlePlainText.value || isSummarizing.value) return

  isSummarizing.value = true
  summaryResult.value = null

  try {
    summaryResult.value = await aiApi.summarizeContent(articlePlainText.value, summaryType.value)
    if (summaryResult.value?.keyPoints) {
      keyTakeaways.value = summaryResult.value.keyPoints
    }
  } catch (err) {
    console.error('Summarization failed:', err)
  } finally {
    isSummarizing.value = false
  }
}

async function translateArticle() {
  if (!articlePlainText.value || isTranslating.value) return

  isTranslating.value = true
  translationResult.value = null

  try {
    translationResult.value = await aiApi.translateContent(articlePlainText.value, targetLanguage.value)
  } catch (err) {
    console.error('Translation failed:', err)
  } finally {
    isTranslating.value = false
  }
}

async function extractEntities() {
  if (!articlePlainText.value || isExtractingEntities.value) return

  isExtractingEntities.value = true
  entitiesResult.value = null

  try {
    entitiesResult.value = await aiApi.extractEntities(articlePlainText.value)
  } catch (err) {
    console.error('Entity extraction failed:', err)
  } finally {
    isExtractingEntities.value = false
  }
}

async function analyzeSentiment() {
  if (!articlePlainText.value || isAnalyzingSentiment.value) return

  isAnalyzingSentiment.value = true
  sentimentResult.value = null

  try {
    const result = await aiApi.analyzeSentiment(articlePlainText.value)
    sentimentResult.value = {
      overall: result.overall,
      score: result.score,
      confidence: result.confidence,
    }
  } catch (err) {
    console.error('Sentiment analysis failed:', err)
  } finally {
    isAnalyzingSentiment.value = false
  }
}

function toggleAIPanel() {
  showAIPanel.value = !showAIPanel.value
  if (showAIPanel.value && !summaryResult.value) {
    // Auto-generate summary when panel opens
    generateSummary()
  }
}

function selectAITab(tab: 'summary' | 'translate' | 'entities' | 'insights') {
  activeAITab.value = tab
  // Auto-trigger the appropriate AI action
  if (tab === 'summary' && !summaryResult.value) generateSummary()
  if (tab === 'translate' && !translationResult.value) translateArticle()
  if (tab === 'entities' && !entitiesResult.value) extractEntities()
  if (tab === 'insights' && !sentimentResult.value) analyzeSentiment()
}

function copyToClipboard(text: string) {
  navigator.clipboard.writeText(text)
}

function getLanguageName(code: SupportedLanguage): string {
  return SUPPORTED_LANGUAGES.find(l => l.code === code)?.name || code
}

onMounted(async () => {
  const slug = route.params.slug as string
  try {
    article.value = await articlesApi.getArticle(slug)
  } catch {
    error.value = 'Article not found'
  } finally {
    isLoading.value = false
  }
})

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    month: 'long',
    day: 'numeric',
    year: 'numeric',
  })
}

function goBack() {
  router.push({ name: 'Articles' })
}
</script>

<template>
  <div class="max-w-6xl mx-auto px-4">
    <!-- Loading -->
    <div v-if="isLoading" class="card p-12">
      <LoadingSpinner size="lg" text="Loading article..." />
    </div>

    <!-- Error -->
    <div v-else-if="error" class="card p-12 text-center">
      <i class="fas fa-exclamation-triangle text-4xl text-red-300 mb-4"></i>
      <h3 class="text-lg font-semibold text-gray-700">{{ error }}</h3>
      <button @click="goBack" class="btn btn-secondary mt-4">
        <i class="fas fa-arrow-left"></i>
        <span>Back to Articles</span>
      </button>
    </div>

    <!-- Article with AI Panel -->
    <div v-else-if="article" class="fade-in flex gap-6">
      <!-- Main Article Content -->
      <article class="flex-1 min-w-0">
        <!-- Back Button -->
        <button @click="goBack" class="btn btn-ghost mb-6">
          <i class="fas fa-arrow-left"></i>
          <span>Back to Articles</span>
        </button>

        <!-- Header -->
        <header class="mb-8">
          <div class="flex items-center gap-2 mb-4 flex-wrap">
            <span v-if="article.category" class="badge badge-primary">{{ article.category.name }}</span>
            <span v-for="tag in article.tags" :key="tag.id" class="badge badge-secondary">
              {{ tag.name }}
            </span>
            <!-- AI Toggle Button -->
            <button
              @click="toggleAIPanel"
              class="ml-auto inline-flex items-center gap-2 px-3 py-1.5 rounded-full text-sm font-medium transition-all"
              :class="showAIPanel ? 'bg-teal-100 text-teal-700' : 'bg-gray-100 text-gray-600 hover:bg-teal-50 hover:text-teal-600'"
            >
              <i class="fas fa-wand-magic-sparkles"></i>
              AI Assist
            </button>
          </div>

          <h1 class="text-3xl md:text-4xl font-bold text-gray-900 leading-tight">
            {{ article.title }}
          </h1>

          <p class="text-xl text-gray-500 mt-4">{{ article.excerpt }}</p>

          <!-- Author & Meta -->
          <div class="flex items-center justify-between mt-6 pt-6 border-t border-gray-100 flex-wrap gap-4">
            <div class="flex items-center gap-3">
              <div class="w-12 h-12 rounded-full bg-teal-100 flex items-center justify-center text-teal-600 font-bold">
                {{ article.author.displayName.split(' ').map((n: string) => n[0]).join('') }}
              </div>
              <div>
                <p class="font-semibold text-gray-900">{{ article.author.displayName }}</p>
                <p class="text-sm text-gray-500">
                  {{ formatDate(article.publishedAt || article.createdAt) }}
                  <span class="mx-2">•</span>
                  {{ Math.ceil(article.content.length / 1000) }} min read
                </p>
              </div>
            </div>
            <div class="flex items-center gap-2">
              <button class="btn-icon btn-secondary">
                <i class="fas fa-heart" :class="article.isLiked ? 'text-red-500' : ''"></i>
              </button>
              <button class="btn-icon btn-secondary">
                <i class="fas fa-bookmark" :class="article.isBookmarked ? 'text-amber-500' : ''"></i>
              </button>
              <button class="btn-icon btn-secondary">
                <i class="fas fa-share-alt"></i>
              </button>
            </div>
          </div>
        </header>

        <!-- AI Key Takeaways (always visible if available) -->
        <div v-if="keyTakeaways.length > 0" class="mb-6 p-4 bg-gradient-to-r from-teal-50 to-transparent border border-teal-100 rounded-xl">
          <div class="flex items-center gap-2 mb-3">
            <div class="w-8 h-8 rounded-lg bg-teal-100 flex items-center justify-center">
              <i class="fas fa-lightbulb text-teal-600"></i>
            </div>
            <h3 class="font-semibold text-gray-800">AI Key Takeaways</h3>
            <span class="text-xs text-teal-600 bg-teal-100 px-2 py-0.5 rounded-full">Powered by Intalio AI</span>
          </div>
          <ul class="space-y-2">
            <li v-for="(point, idx) in keyTakeaways" :key="idx" class="flex items-start gap-2 text-sm text-gray-700">
              <i class="fas fa-check-circle text-teal-500 mt-0.5"></i>
              <span>{{ point }}</span>
            </li>
          </ul>
        </div>

        <!-- Cover Image -->
        <div v-if="article.coverImage" class="mb-8 rounded-2xl overflow-hidden">
          <img :src="article.coverImage" :alt="article.title" class="w-full">
        </div>

        <!-- Content -->
        <div class="card p-8 md:p-12">
          <div class="prose prose-lg max-w-none" v-html="article.content"></div>
        </div>

        <!-- Footer Stats -->
        <div class="flex items-center justify-between mt-6 p-4 bg-gray-50 rounded-xl">
          <div class="flex items-center gap-6 text-sm text-gray-500">
            <span><i class="fas fa-eye mr-2"></i> {{ article.viewCount }} views</span>
            <span><i class="fas fa-heart mr-2"></i> {{ article.likeCount }} likes</span>
            <span><i class="fas fa-comment mr-2"></i> {{ article.commentCount }} comments</span>
          </div>
        </div>
      </article>

      <!-- AI Assistant Panel (Sidebar) -->
      <Transition
        enter-active-class="transition-all duration-300 ease-out"
        enter-from-class="opacity-0 translate-x-4"
        enter-to-class="opacity-100 translate-x-0"
        leave-active-class="transition-all duration-200 ease-in"
        leave-from-class="opacity-100 translate-x-0"
        leave-to-class="opacity-0 translate-x-4"
      >
        <aside v-if="showAIPanel" class="w-96 flex-shrink-0 sticky top-20 h-fit">
          <div class="bg-white rounded-xl border border-gray-200 shadow-lg overflow-hidden">
            <!-- Panel Header -->
            <div class="px-4 py-3 bg-gradient-to-r from-teal-500 to-teal-600 text-white">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-2">
                  <i class="fas fa-wand-magic-sparkles"></i>
                  <span class="font-semibold">AI Assistant</span>
                </div>
                <button @click="showAIPanel = false" class="p-1 hover:bg-white/20 rounded transition-colors">
                  <i class="fas fa-times"></i>
                </button>
              </div>
            </div>

            <!-- Tab Navigation -->
            <div class="flex border-b border-gray-200">
              <button
                v-for="tab in [
                  { id: 'summary', icon: 'fas fa-compress-alt', label: 'Summary' },
                  { id: 'translate', icon: 'fas fa-language', label: 'Translate' },
                  { id: 'entities', icon: 'fas fa-tags', label: 'Entities' },
                  { id: 'insights', icon: 'fas fa-chart-line', label: 'Insights' },
                ]"
                :key="tab.id"
                @click="selectAITab(tab.id as any)"
                class="flex-1 px-3 py-2.5 text-xs font-medium transition-colors"
                :class="activeAITab === tab.id ? 'text-teal-600 border-b-2 border-teal-500 bg-teal-50' : 'text-gray-500 hover:text-gray-700'"
              >
                <i :class="tab.icon" class="mr-1"></i>
                {{ tab.label }}
              </button>
            </div>

            <!-- Tab Content -->
            <div class="p-4 max-h-[calc(100vh-300px)] overflow-y-auto">
              <!-- Summary Tab -->
              <div v-if="activeAITab === 'summary'">
                <!-- Summary Type Selector -->
                <div class="flex gap-2 mb-4">
                  <button
                    v-for="type in [
                      { id: 'brief', label: 'Brief' },
                      { id: 'detailed', label: 'Detailed' },
                      { id: 'bullet', label: 'Bullets' },
                    ]"
                    :key="type.id"
                    @click="summaryType = type.id as any; generateSummary()"
                    class="flex-1 px-3 py-1.5 text-xs font-medium rounded-lg transition-colors"
                    :class="summaryType === type.id ? 'bg-teal-100 text-teal-700' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'"
                  >
                    {{ type.label }}
                  </button>
                </div>

                <!-- Loading -->
                <div v-if="isSummarizing" class="flex flex-col items-center py-8">
                  <AILoadingIndicator variant="dots" text="Generating summary..." />
                </div>

                <!-- Result -->
                <div v-else-if="summaryResult" class="space-y-3">
                  <div class="p-3 bg-gray-50 rounded-lg">
                    <p class="text-sm text-gray-700 whitespace-pre-wrap">{{ summaryResult.summary }}</p>
                  </div>
                  <div class="flex items-center justify-between text-xs text-gray-500">
                    <span>{{ summaryResult.wordCount }} words</span>
                    <span>{{ summaryResult.compressionRatio.toFixed(1) }}x compression</span>
                  </div>
                  <div class="flex gap-2">
                    <button @click="copyToClipboard(summaryResult.summary)" class="flex-1 px-3 py-1.5 text-xs bg-gray-100 text-gray-600 rounded-lg hover:bg-gray-200 transition-colors">
                      <i class="fas fa-copy mr-1"></i> Copy
                    </button>
                    <button @click="generateSummary()" class="flex-1 px-3 py-1.5 text-xs bg-teal-100 text-teal-700 rounded-lg hover:bg-teal-200 transition-colors">
                      <i class="fas fa-sync-alt mr-1"></i> Regenerate
                    </button>
                  </div>
                </div>

                <!-- Empty State -->
                <div v-else class="text-center py-8">
                  <i class="fas fa-compress-alt text-3xl text-gray-300 mb-3"></i>
                  <p class="text-sm text-gray-500">Click a summary type to generate</p>
                </div>
              </div>

              <!-- Translate Tab -->
              <div v-if="activeAITab === 'translate'">
                <!-- Language Selector -->
                <div class="mb-4">
                  <label class="block text-xs font-medium text-gray-600 mb-2">Translate to:</label>
                  <select
                    v-model="targetLanguage"
                    @change="translateArticle()"
                    class="w-full px-3 py-2 text-sm border border-gray-200 rounded-lg focus:border-teal-500 focus:ring-1 focus:ring-teal-200"
                  >
                    <option v-for="lang in SUPPORTED_LANGUAGES" :key="lang.code" :value="lang.code">
                      {{ lang.flag }} {{ lang.name }} ({{ lang.nativeName }})
                    </option>
                  </select>
                </div>

                <!-- Loading -->
                <div v-if="isTranslating" class="flex flex-col items-center py-8">
                  <AILoadingIndicator variant="spinner" text="Translating..." />
                </div>

                <!-- Result -->
                <div v-else-if="translationResult" class="space-y-3">
                  <div class="p-3 bg-gray-50 rounded-lg" :dir="targetLanguage === 'ar' ? 'rtl' : 'ltr'">
                    <p class="text-sm text-gray-700">{{ translationResult.translatedText }}</p>
                  </div>
                  <AIConfidenceBar :value="translationResult.confidence" size="sm" />
                  <div class="flex gap-2">
                    <button @click="copyToClipboard(translationResult.translatedText)" class="flex-1 px-3 py-1.5 text-xs bg-gray-100 text-gray-600 rounded-lg hover:bg-gray-200 transition-colors">
                      <i class="fas fa-copy mr-1"></i> Copy
                    </button>
                    <button @click="translateArticle()" class="flex-1 px-3 py-1.5 text-xs bg-teal-100 text-teal-700 rounded-lg hover:bg-teal-200 transition-colors">
                      <i class="fas fa-sync-alt mr-1"></i> Retranslate
                    </button>
                  </div>
                </div>
              </div>

              <!-- Entities Tab -->
              <div v-if="activeAITab === 'entities'">
                <!-- Loading -->
                <div v-if="isExtractingEntities" class="flex flex-col items-center py-8">
                  <AILoadingIndicator variant="pulse" text="Extracting entities..." />
                </div>

                <!-- Result -->
                <div v-else-if="entitiesResult && entitiesResult.entities.length > 0" class="space-y-3">
                  <div v-for="(entity, idx) in entitiesResult.entities" :key="idx" class="flex items-center justify-between p-2 bg-gray-50 rounded-lg">
                    <div class="flex items-center gap-2">
                      <span
                        class="px-2 py-1 text-xs font-medium rounded capitalize"
                        :class="{
                          'bg-blue-100 text-blue-700': entity.type === 'person',
                          'bg-purple-100 text-purple-700': entity.type === 'organization',
                          'bg-green-100 text-green-700': entity.type === 'location',
                          'bg-amber-100 text-amber-700': entity.type === 'date',
                          'bg-teal-100 text-teal-700': entity.type === 'event',
                        }"
                      >
                        {{ entity.type }}
                      </span>
                      <span class="text-sm text-gray-700">{{ entity.text }}</span>
                    </div>
                    <span class="text-xs text-gray-400">{{ Math.round(entity.confidence * 100) }}%</span>
                  </div>
                  <button @click="extractEntities()" class="w-full px-3 py-1.5 text-xs bg-teal-100 text-teal-700 rounded-lg hover:bg-teal-200 transition-colors">
                    <i class="fas fa-sync-alt mr-1"></i> Re-extract
                  </button>
                </div>

                <!-- Empty State -->
                <div v-else class="text-center py-8">
                  <i class="fas fa-tags text-3xl text-gray-300 mb-3"></i>
                  <p class="text-sm text-gray-500">No entities found</p>
                  <button @click="extractEntities()" class="mt-2 px-3 py-1.5 text-xs bg-teal-100 text-teal-700 rounded-lg hover:bg-teal-200 transition-colors">
                    Extract Entities
                  </button>
                </div>
              </div>

              <!-- Insights Tab -->
              <div v-if="activeAITab === 'insights'">
                <!-- Loading -->
                <div v-if="isAnalyzingSentiment" class="flex flex-col items-center py-8">
                  <AILoadingIndicator variant="shimmer" text="Analyzing content..." />
                </div>

                <!-- Result -->
                <div v-else-if="sentimentResult" class="space-y-4">
                  <div class="p-4 bg-gray-50 rounded-lg">
                    <h4 class="text-xs font-semibold text-gray-500 uppercase mb-3">Content Sentiment</h4>
                    <div class="flex items-center gap-3 mb-3">
                      <AISentimentBadge :sentiment="sentimentResult.overall" size="md" />
                      <span class="text-sm text-gray-600">
                        This article has a {{ sentimentResult.overall }} tone
                      </span>
                    </div>
                    <AIConfidenceBar :value="sentimentResult.confidence" size="sm" label="Analysis Confidence" show-label />
                  </div>

                  <div class="p-4 bg-teal-50 rounded-lg">
                    <h4 class="text-xs font-semibold text-teal-700 uppercase mb-2">AI Insights</h4>
                    <ul class="space-y-2 text-sm text-gray-700">
                      <li class="flex items-start gap-2">
                        <i class="fas fa-lightbulb text-teal-500 mt-0.5"></i>
                        <span>Article is well-structured for knowledge sharing</span>
                      </li>
                      <li class="flex items-start gap-2">
                        <i class="fas fa-users text-teal-500 mt-0.5"></i>
                        <span>Suitable for team-wide distribution</span>
                      </li>
                      <li class="flex items-start gap-2">
                        <i class="fas fa-clock text-teal-500 mt-0.5"></i>
                        <span>Estimated read time is optimal for engagement</span>
                      </li>
                    </ul>
                  </div>

                  <button @click="analyzeSentiment()" class="w-full px-3 py-1.5 text-xs bg-teal-100 text-teal-700 rounded-lg hover:bg-teal-200 transition-colors">
                    <i class="fas fa-sync-alt mr-1"></i> Re-analyze
                  </button>
                </div>
              </div>
            </div>

            <!-- Panel Footer -->
            <div class="px-4 py-3 bg-gray-50 border-t border-gray-200">
              <p class="text-xs text-gray-400 text-center">
                <i class="fas fa-shield-alt mr-1"></i>
                Powered by Intalio AI Engine
              </p>
            </div>
          </div>
        </aside>
      </Transition>
    </div>
  </div>
</template>
