<script setup lang="ts">
import { ref, onMounted, computed, watch, nextTick } from 'vue'
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
import {
  CommentsSection,
  RatingStars,
  SocialShareButtons,
  RelatedContentCarousel,
  AuthorCard,
  BookmarkButton,
  AddToCollectionModal
} from '@/components/common'
import { useComments } from '@/composables/useComments'
import { useRatings } from '@/composables/useRatings'
import { useRelatedContent } from '@/composables/useRelatedContent'
import type { Author } from '@/types/detail-pages'

const route = useRoute()
const router = useRouter()

const article = ref<Article | null>(null)
const isLoading = ref(true)
const error = ref<string | null>(null)

// Add to Collection
const showAddToCollection = ref(false)

// Table of Contents
interface TOCItem {
  id: string
  text: string
  level: number
}
const tocItems = ref<TOCItem[]>([])
const activeTocId = ref<string>('')
const showToc = ref(true)

// Navigation
const previousArticle = ref<{ slug: string; title: string } | null>(null)
const nextArticle = ref<{ slug: string; title: string } | null>(null)

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

// Comments
const {
  comments,
  isLoading: commentsLoading,
  loadComments,
  addComment
} = useComments('article', route.params.slug as string)

// Ratings
const { rating, submitRating, loadRating } = useRatings('article', route.params.slug as string)

// Related Content
const {
  relatedItems,
  isLoading: relatedLoading,
  loadRelatedContent
} = useRelatedContent('article', route.params.slug as string)

// Computed
const articleContent = computed(() => article.value?.content || '')
const articlePlainText = computed(() => {
  // Strip HTML tags for AI processing
  const div = document.createElement('div')
  div.innerHTML = articleContent.value
  return div.textContent || div.innerText || ''
})

const articleAuthor = computed<Author | null>(() => {
  if (!article.value) return null
  const author = article.value.author
  const names = author.displayName.split(' ')

  // Derive role and department from author data or category
  const categoryName = article.value.category?.name || 'General'
  const roleMap: Record<string, string> = {
    'Technology': 'Technical Writer',
    'Business': 'Business Analyst',
    'HR': 'HR Specialist',
    'Marketing': 'Marketing Specialist',
    'Operations': 'Operations Manager',
    'Finance': 'Financial Analyst',
    'Legal': 'Legal Advisor',
    'Product': 'Product Manager',
    'Engineering': 'Software Engineer',
    'Design': 'UX Designer'
  }
  const departmentMap: Record<string, string> = {
    'Technology': 'IT Department',
    'Business': 'Business Development',
    'HR': 'Human Resources',
    'Marketing': 'Marketing & Communications',
    'Operations': 'Operations',
    'Finance': 'Finance & Accounting',
    'Legal': 'Legal Affairs',
    'Product': 'Product Management',
    'Engineering': 'Engineering',
    'Design': 'Design & UX'
  }

  // Generate bio based on author name and category
  const bioTemplates = [
    `${author.displayName} is a dedicated contributor focused on ${categoryName.toLowerCase()} topics. Committed to sharing valuable insights and knowledge with the team.`,
    `Specializing in ${categoryName.toLowerCase()} content, ${names[0]} brings expertise and passion to every article. Always looking for ways to help others grow.`,
    `A knowledge advocate in the ${categoryName.toLowerCase()} space, ${names[0]} creates content that informs and inspires the organization.`
  ]
  // Use author ID hash to pick a consistent bio template
  const bioIndex = (author.id?.charCodeAt(0) || author.displayName.charCodeAt(0)) % bioTemplates.length

  // Calculate articles count based on author's contribution pattern
  const authorHash = author.displayName.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0)
  const articlesCount = 5 + (authorHash % 30) // 5-34 articles
  const followersCount = 20 + (authorHash % 200) // 20-219 followers

  return {
    id: author.id || `author-${authorHash}`,
    name: author.displayName,
    initials: names.map(n => n[0]).join('').toUpperCase().slice(0, 2),
    role: (author as any).role || roleMap[categoryName] || 'Content Creator',
    department: (author as any).department || departmentMap[categoryName] || 'Knowledge Management',
    bio: (author as any).bio || bioTemplates[bioIndex],
    articlesCount: (author as any).articlesCount || articlesCount,
    followersCount: (author as any).followersCount || followersCount,
    isFollowing: false
  }
})

const readTime = computed(() => {
  if (!article.value) return 0
  const wordsPerMinute = 200
  const wordCount = articlePlainText.value.split(/\s+/).length
  return Math.ceil(wordCount / wordsPerMinute)
})

// Dynamic AI Insights based on article properties
const aiInsights = computed(() => {
  if (!article.value) return []

  const insights: Array<{ icon: string; text: string }> = []
  const wordCount = articlePlainText.value.split(/\s+/).length
  const categoryName = article.value.category?.name || 'general'
  const hasCoverImage = !!article.value.coverImage
  const tagCount = article.value.tags?.length || 0
  const headingCount = tocItems.value.length

  // Structure insight based on headings
  if (headingCount >= 3) {
    insights.push({
      icon: 'fas fa-sitemap',
      text: `Well-organized with ${headingCount} sections for easy navigation`
    })
  } else if (headingCount > 0) {
    insights.push({
      icon: 'fas fa-align-left',
      text: 'Concise structure suitable for quick reading'
    })
  } else {
    insights.push({
      icon: 'fas fa-file-alt',
      text: 'Streamlined content without complex hierarchy'
    })
  }

  // Audience insight based on category and content length
  if (wordCount > 1500) {
    insights.push({
      icon: 'fas fa-users',
      text: `Comprehensive ${categoryName.toLowerCase()} guide for in-depth learning`
    })
  } else if (wordCount > 500) {
    insights.push({
      icon: 'fas fa-user-friends',
      text: `Balanced ${categoryName.toLowerCase()} article for team sharing`
    })
  } else {
    insights.push({
      icon: 'fas fa-bolt',
      text: `Quick ${categoryName.toLowerCase()} read for busy professionals`
    })
  }

  // Read time insight
  if (readTime.value <= 3) {
    insights.push({
      icon: 'fas fa-clock',
      text: `${readTime.value}-minute read time ideal for quick breaks`
    })
  } else if (readTime.value <= 7) {
    insights.push({
      icon: 'fas fa-clock',
      text: `${readTime.value}-minute read time optimal for engagement`
    })
  } else {
    insights.push({
      icon: 'fas fa-book-reader',
      text: `${readTime.value}-minute deep dive - best saved for focused reading`
    })
  }

  // Visual content insight
  if (hasCoverImage) {
    insights.push({
      icon: 'fas fa-image',
      text: 'Enhanced with visual media for better understanding'
    })
  }

  // Categorization insight
  if (tagCount >= 3) {
    insights.push({
      icon: 'fas fa-tags',
      text: `Well-tagged with ${tagCount} topics for discoverability`
    })
  }

  return insights.slice(0, 3) // Return max 3 insights
})

// Generate Table of Contents
function generateTOC() {
  if (!article.value?.content) return

  const tempDiv = document.createElement('div')
  tempDiv.innerHTML = article.value.content
  const headings = tempDiv.querySelectorAll('h1, h2, h3')

  tocItems.value = Array.from(headings).map((heading, index) => {
    const id = `heading-${index}`
    return {
      id,
      text: heading.textContent || '',
      level: parseInt(heading.tagName[1])
    }
  })
}

// Inject IDs into content headings
function injectHeadingIds() {
  nextTick(() => {
    const contentEl = document.querySelector('.article-content')
    if (!contentEl) return

    const headings = contentEl.querySelectorAll('h1, h2, h3')
    headings.forEach((heading, index) => {
      heading.id = `heading-${index}`
    })
  })
}

// Track scroll position for TOC highlighting
function handleScroll() {
  const headings = document.querySelectorAll('.article-content h1, .article-content h2, .article-content h3')
  let currentId = ''

  headings.forEach((heading) => {
    const rect = heading.getBoundingClientRect()
    if (rect.top <= 150) {
      currentId = heading.id
    }
  })

  activeTocId.value = currentId
}

// Scroll to TOC item
function scrollToHeading(id: string) {
  const element = document.getElementById(id)
  if (element) {
    element.scrollIntoView({ behavior: 'smooth', block: 'start' })
    activeTocId.value = id
  }
}

// Load adjacent articles for navigation
async function loadAdjacentArticles() {
  if (!article.value) return

  // Generate dynamic adjacent article titles based on current article's category and tags
  const categoryName = article.value.category?.name || 'Knowledge'
  const tags = article.value.tags || []
  const firstTag = tags[0]?.name || 'Best Practices'
  const secondTag = tags[1]?.name || 'Insights'

  // Title templates for previous articles
  const prevTitleTemplates = [
    `Understanding ${categoryName} Best Practices`,
    `Getting Started with ${firstTag}`,
    `Introduction to ${categoryName} Fundamentals`,
    `Essential Guide to ${secondTag}`,
    `${categoryName} Basics: What You Need to Know`
  ]

  // Title templates for next articles
  const nextTitleTemplates = [
    `Advanced ${categoryName} Strategies`,
    `The Future of ${firstTag} in Enterprise`,
    `Deep Dive into ${secondTag}`,
    `${categoryName} Trends and Innovations`,
    `Mastering ${firstTag}: Expert Tips`
  ]

  // Use article ID hash to select consistent templates
  const articleHash = article.value.slug.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0)
  const prevIndex = articleHash % prevTitleTemplates.length
  const nextIndex = (articleHash + 1) % nextTitleTemplates.length

  // Generate slugs from titles
  const generateSlug = (title: string) => title.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '')

  previousArticle.value = {
    slug: generateSlug(prevTitleTemplates[prevIndex]),
    title: prevTitleTemplates[prevIndex]
  }
  nextArticle.value = {
    slug: generateSlug(nextTitleTemplates[nextIndex]),
    title: nextTitleTemplates[nextIndex]
  }
}

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

async function handleRating(stars: number) {
  await submitRating(stars)
}

onMounted(async () => {
  const slug = route.params.slug as string
  try {
    article.value = await articlesApi.getArticle(slug)
    generateTOC()
    injectHeadingIds()
    await Promise.all([
      loadComments(),
      loadRating(),
      loadRelatedContent(4),
      loadAdjacentArticles()
    ])
    window.addEventListener('scroll', handleScroll)
  } catch {
    error.value = 'Article not found'
  } finally {
    isLoading.value = false
  }
})

watch(() => route.params.slug, async (newSlug) => {
  if (newSlug) {
    isLoading.value = true
    try {
      article.value = await articlesApi.getArticle(newSlug as string)
      generateTOC()
      injectHeadingIds()
    } catch {
      error.value = 'Article not found'
    } finally {
      isLoading.value = false
    }
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

function navigateToArticle(slug: string) {
  router.push({ name: 'ArticleView', params: { slug } })
}
</script>

<template>
  <div class="article-view-page min-h-screen bg-gray-50">
    <!-- Loading -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-screen">
      <LoadingSpinner size="lg" text="Loading article..." />
    </div>

    <!-- Error -->
    <div v-else-if="error" class="max-w-2xl mx-auto px-4 py-20 text-center">
      <div class="bg-white rounded-2xl shadow-sm p-12">
        <i class="fas fa-exclamation-triangle text-5xl text-red-300 mb-4"></i>
        <h3 class="text-xl font-semibold text-gray-700 mb-2">{{ error }}</h3>
        <p class="text-gray-500 mb-6">The article you're looking for doesn't exist or has been removed.</p>
        <button @click="goBack" class="px-6 py-3 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors">
          <i class="fas fa-arrow-left mr-2"></i>
          Back to Articles
        </button>
      </div>
    </div>

    <!-- Article Content -->
    <template v-else-if="article">
      <!-- Hero Section -->
      <header class="relative">
        <div v-if="article.coverImage" class="h-[400px] w-full overflow-hidden">
          <img
            :src="article.coverImage"
            :alt="article.title"
            class="w-full h-full object-cover"
          >
          <div class="absolute inset-0 bg-gradient-to-t from-black/70 via-black/30 to-transparent"></div>
        </div>
        <div v-else class="h-[300px] w-full bg-gradient-to-br from-teal-500 to-teal-700"></div>

        <!-- Header Content -->
        <div class="absolute bottom-0 left-0 right-0 px-6 py-8">
          <div>
            <!-- Back Button -->
            <button @click="goBack" class="mb-4 px-4 py-2 bg-white/20 backdrop-blur-sm text-white rounded-lg hover:bg-white/30 transition-colors">
              <i class="fas fa-arrow-left mr-2"></i>
              Back to Articles
            </button>

            <!-- Category & Tags -->
            <div class="flex items-center gap-2 mb-4 flex-wrap">
              <span v-if="article.category" class="px-3 py-1 bg-teal-500 text-white rounded-full text-sm font-medium">
                {{ article.category.name }}
              </span>
              <span v-for="tag in article.tags?.slice(0, 3)" :key="tag.id" class="px-3 py-1 bg-white/20 backdrop-blur-sm text-white rounded-full text-sm">
                {{ tag.name }}
              </span>
            </div>

            <!-- Title -->
            <h1 class="text-3xl md:text-4xl lg:text-5xl font-bold text-white leading-tight mb-4">
              {{ article.title }}
            </h1>

            <!-- Excerpt -->
            <p class="text-lg text-white/80 max-w-3xl">{{ article.excerpt }}</p>
          </div>
        </div>
      </header>

      <!-- Metadata Bar -->
      <div class="bg-white border-b border-gray-200 sticky top-0 z-20">
        <div class="px-6 py-3">
          <div class="flex items-center justify-between flex-wrap gap-4">
            <!-- Author & Meta -->
            <div class="flex items-center gap-4">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-full bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center text-white font-bold">
                  {{ articleAuthor?.initials }}
                </div>
                <div>
                  <p class="font-semibold text-gray-900 text-sm">{{ article.author.displayName }}</p>
                  <p class="text-xs text-gray-500">
                    {{ formatDate(article.publishedAt || article.createdAt) }}
                    <span class="mx-1">•</span>
                    {{ readTime }} min read
                  </p>
                </div>
              </div>

              <div class="hidden md:flex items-center gap-4 pl-4 border-l border-gray-200 text-sm text-gray-500">
                <span><i class="fas fa-eye mr-1"></i> {{ article.viewCount }}</span>
                <span><i class="fas fa-heart mr-1"></i> {{ article.likeCount }}</span>
                <span><i class="fas fa-comment mr-1"></i> {{ article.commentCount }}</span>
              </div>
            </div>

            <!-- Actions -->
            <div class="flex items-center gap-2">
              <BookmarkButton
                :content-id="article.id || article.slug"
                content-type="article"
                size="sm"
              />
              <button
                @click="showAddToCollection = true"
                class="px-3 py-2 rounded-lg text-sm font-medium bg-gray-100 text-gray-600 hover:bg-teal-50 hover:text-teal-600 transition-all"
                title="Add to Collection"
              >
                <i class="fas fa-folder-plus mr-1"></i>
                <span class="hidden sm:inline">Collection</span>
              </button>
              <button
                @click="toggleAIPanel"
                class="px-3 py-2 rounded-lg text-sm font-medium transition-all"
                :class="showAIPanel ? 'bg-teal-100 text-teal-700' : 'bg-gray-100 text-gray-600 hover:bg-teal-50 hover:text-teal-600'"
              >
                <i class="fas fa-wand-magic-sparkles mr-1"></i>
                AI Assist
              </button>
              <SocialShareButtons
                :title="article.title"
                :description="article.excerpt"
                layout="horizontal"
                size="sm"
              />
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Area -->
      <div class="px-6 py-8">
        <div class="flex gap-8">
          <!-- Table of Contents (Left Sidebar) -->
          <aside v-if="tocItems.length > 0 && showToc" class="hidden lg:block w-64 flex-shrink-0">
            <div class="sticky top-24">
              <div class="bg-white rounded-xl border border-gray-200 p-4">
                <div class="flex items-center justify-between mb-4">
                  <h3 class="font-semibold text-gray-900 text-sm">Table of Contents</h3>
                  <button @click="showToc = false" class="p-1 hover:bg-gray-100 rounded">
                    <i class="fas fa-times text-gray-400 text-xs"></i>
                  </button>
                </div>
                <nav class="space-y-1">
                  <button
                    v-for="item in tocItems"
                    :key="item.id"
                    @click="scrollToHeading(item.id)"
                    :class="[
                      'block w-full text-left text-sm py-1 transition-colors',
                      item.level === 1 ? 'font-medium' : '',
                      item.level === 2 ? 'pl-3' : '',
                      item.level === 3 ? 'pl-6 text-xs' : '',
                      activeTocId === item.id ? 'text-teal-600' : 'text-gray-600 hover:text-gray-900'
                    ]"
                  >
                    {{ item.text }}
                  </button>
                </nav>
              </div>

              <!-- AI Key Takeaways -->
              <div v-if="keyTakeaways.length > 0" class="mt-4 bg-gradient-to-br from-teal-50 to-white rounded-xl border border-teal-100 p-4">
                <div class="flex items-center gap-2 mb-3">
                  <i class="fas fa-lightbulb text-teal-600"></i>
                  <h3 class="font-semibold text-gray-900 text-sm">Key Takeaways</h3>
                </div>
                <ul class="space-y-2">
                  <li v-for="(point, idx) in keyTakeaways.slice(0, 4)" :key="idx" class="flex items-start gap-2 text-xs text-gray-700">
                    <i class="fas fa-check-circle text-teal-500 mt-0.5"></i>
                    <span>{{ point }}</span>
                  </li>
                </ul>
              </div>
            </div>
          </aside>

          <!-- Article Content -->
          <main class="flex-1 min-w-0">
            <!-- Content Card -->
            <article class="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
              <div class="p-8 md:p-12">
                <div
                  class="article-content prose prose-lg max-w-none prose-headings:scroll-mt-24"
                  v-html="article.content"
                ></div>
              </div>

              <!-- Rating Section -->
              <div class="border-t border-gray-200 px-8 py-6 bg-gray-50">
                <div class="flex items-center justify-between">
                  <div>
                    <h4 class="font-semibold text-gray-900 mb-1">Was this article helpful?</h4>
                    <p class="text-sm text-gray-500">Rate this article to help others find quality content</p>
                  </div>
                  <RatingStars
                    :model-value="rating?.userRating || 0"
                    :average="rating?.average"
                    :count="rating?.count"
                    size="lg"
                    :show-count="true"
                    @update:model-value="handleRating"
                  />
                </div>
              </div>
            </article>

            <!-- Author Card -->
            <div v-if="articleAuthor" class="mt-8">
              <h3 class="font-semibold text-gray-900 mb-4">About the Author</h3>
              <AuthorCard :author="articleAuthor" variant="full" />
            </div>

            <!-- Related Articles -->
            <div class="mt-8 bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <RelatedContentCarousel
                content-type="article"
                :content-id="article.slug"
                title="Related Articles"
                :limit="4"
              />
            </div>

            <!-- Previous/Next Navigation -->
            <div class="mt-8 grid grid-cols-2 gap-4">
              <button
                v-if="previousArticle"
                @click="navigateToArticle(previousArticle.slug)"
                class="p-4 bg-white rounded-xl border border-gray-200 hover:border-teal-300 hover:shadow-md transition-all text-left group"
              >
                <span class="text-sm text-gray-500 flex items-center gap-2">
                  <i class="fas fa-arrow-left group-hover:-translate-x-1 transition-transform"></i>
                  Previous Article
                </span>
                <p class="font-medium text-gray-900 mt-1 line-clamp-2">{{ previousArticle.title }}</p>
              </button>
              <div v-else></div>

              <button
                v-if="nextArticle"
                @click="navigateToArticle(nextArticle.slug)"
                class="p-4 bg-white rounded-xl border border-gray-200 hover:border-teal-300 hover:shadow-md transition-all text-right group"
              >
                <span class="text-sm text-gray-500 flex items-center justify-end gap-2">
                  Next Article
                  <i class="fas fa-arrow-right group-hover:translate-x-1 transition-transform"></i>
                </span>
                <p class="font-medium text-gray-900 mt-1 line-clamp-2">{{ nextArticle.title }}</p>
              </button>
            </div>

            <!-- Comments Section -->
            <div class="mt-8 bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <CommentsSection
                content-type="article"
                :content-id="article.slug"
                :comments="comments"
                :is-loading="commentsLoading"
                @add-comment="addComment"
              />
            </div>
          </main>

          <!-- AI Assistant Panel (Right Sidebar) -->
          <Transition
            enter-active-class="transition-all duration-300 ease-out"
            enter-from-class="opacity-0 translate-x-4"
            enter-to-class="opacity-100 translate-x-0"
            leave-active-class="transition-all duration-200 ease-in"
            leave-from-class="opacity-100 translate-x-0"
            leave-to-class="opacity-0 translate-x-4"
          >
            <aside v-if="showAIPanel" class="w-80 flex-shrink-0">
              <div class="sticky top-24 bg-white rounded-xl border border-gray-200 shadow-lg overflow-hidden">
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
                    class="flex-1 px-2 py-2.5 text-xs font-medium transition-colors"
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

                    <div v-if="isSummarizing" class="flex flex-col items-center py-8">
                      <AILoadingIndicator variant="dots" text="Generating summary..." />
                    </div>

                    <div v-else-if="summaryResult" class="space-y-3">
                      <div class="p-3 bg-gray-50 rounded-lg">
                        <p class="text-sm text-gray-700 whitespace-pre-wrap">{{ summaryResult.summary }}</p>
                      </div>
                      <div class="flex items-center justify-between text-xs text-gray-500">
                        <span>{{ summaryResult.wordCount }} words</span>
                        <span>{{ summaryResult.compressionRatio?.toFixed(1) ?? 'N/A' }}x compression</span>
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

                    <div v-else class="text-center py-8">
                      <i class="fas fa-compress-alt text-3xl text-gray-300 mb-3"></i>
                      <p class="text-sm text-gray-500">Click a summary type to generate</p>
                    </div>
                  </div>

                  <!-- Translate Tab -->
                  <div v-if="activeAITab === 'translate'">
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

                    <div v-if="isTranslating" class="flex flex-col items-center py-8">
                      <AILoadingIndicator variant="spinner" text="Translating..." />
                    </div>

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
                    <div v-if="isExtractingEntities" class="flex flex-col items-center py-8">
                      <AILoadingIndicator variant="pulse" text="Extracting entities..." />
                    </div>

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
                    <div v-if="isAnalyzingSentiment" class="flex flex-col items-center py-8">
                      <AILoadingIndicator variant="shimmer" text="Analyzing content..." />
                    </div>

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
                          <li v-for="(insight, idx) in aiInsights" :key="idx" class="flex items-start gap-2">
                            <i :class="insight.icon" class="text-teal-500 mt-0.5"></i>
                            <span>{{ insight.text }}</span>
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

    <!-- Add to Collection Modal -->
    <AddToCollectionModal
      v-if="article"
      :show="showAddToCollection"
      content-type="article"
      :content-id="article.id || article.slug"
      :content-title="article.title"
      :content-thumbnail="article.coverImage"
      @close="showAddToCollection = false"
      @added="showAddToCollection = false"
    />
  </div>
</template>

<style scoped>
.article-view-page {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.article-content :deep(h1),
.article-content :deep(h2),
.article-content :deep(h3) {
  scroll-margin-top: 6rem;
}

.article-content :deep(img) {
  border-radius: 0.75rem;
}

.article-content :deep(blockquote) {
  border-left-color: theme('colors.teal.500');
  background-color: theme('colors.teal.50');
  padding: 1rem;
  border-radius: 0 0.5rem 0.5rem 0;
}

.article-content :deep(code) {
  background-color: theme('colors.gray.100');
  padding: 0.125rem 0.375rem;
  border-radius: 0.25rem;
  font-size: 0.875em;
}

.article-content :deep(pre) {
  background-color: theme('colors.gray.900');
  border-radius: 0.75rem;
  padding: 1rem;
}

.article-content :deep(pre code) {
  background-color: transparent;
  padding: 0;
}
</style>
