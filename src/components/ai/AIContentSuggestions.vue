<script setup lang="ts">
import { ref, computed } from 'vue'

export interface ContentSuggestion {
  id: string
  type: 'article' | 'document' | 'media' | 'course' | 'event'
  title: string
  description?: string
  thumbnail?: string
  reason: string
  relevanceScore: number
  suggestionType: 'related' | 'referenced' | 'similar' | 'trending'
  metadata?: {
    author?: string
    date?: string
    category?: string
    readTime?: string
    views?: number
  }
}

interface Props {
  suggestions: ContentSuggestion[]
  loading?: boolean
  title?: string
  maxVisible?: number
  showRelevance?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  title: 'You might be interested in',
  maxVisible: 4,
  showRelevance: true
})

const emit = defineEmits<{
  select: [suggestion: ContentSuggestion]
  dismiss: [suggestionId: string]
  refresh: []
}>()

const showAll = ref(false)

const visibleSuggestions = computed(() => {
  if (showAll.value) return props.suggestions
  return props.suggestions.slice(0, props.maxVisible)
})

const hasMore = computed(() => props.suggestions.length > props.maxVisible)

function getTypeIcon(type: string): string {
  const icons: Record<string, string> = {
    article: 'fas fa-newspaper',
    document: 'fas fa-file-pdf',
    media: 'fas fa-photo-video',
    course: 'fas fa-graduation-cap',
    event: 'fas fa-calendar-alt'
  }
  return icons[type] || 'fas fa-file'
}

function getTypeColor(type: string): string {
  const colors: Record<string, string> = {
    article: 'bg-blue-100 text-blue-600',
    document: 'bg-red-100 text-red-600',
    media: 'bg-purple-100 text-purple-600',
    course: 'bg-green-100 text-green-600',
    event: 'bg-amber-100 text-amber-600'
  }
  return colors[type] || 'bg-gray-100 text-gray-600'
}

function getSuggestionTypeBadge(type: string): { label: string; color: string; icon: string } {
  const badges: Record<string, { label: string; color: string; icon: string }> = {
    related: { label: 'Related', color: 'bg-teal-100 text-teal-700', icon: 'fas fa-link' },
    referenced: { label: 'Referenced', color: 'bg-purple-100 text-purple-700', icon: 'fas fa-quote-right' },
    similar: { label: 'Similar', color: 'bg-blue-100 text-blue-700', icon: 'fas fa-clone' },
    trending: { label: 'Trending', color: 'bg-orange-100 text-orange-700', icon: 'fas fa-fire' }
  }
  return badges[type] || { label: 'Suggested', color: 'bg-gray-100 text-gray-700', icon: 'fas fa-lightbulb' }
}

function getRelevanceColor(score: number): string {
  if (score >= 0.8) return 'text-green-600'
  if (score >= 0.6) return 'text-teal-600'
  if (score >= 0.4) return 'text-amber-600'
  return 'text-gray-500'
}
</script>

<template>
  <div class="ai-content-suggestions">
    <!-- Header -->
    <div class="flex items-center justify-between mb-3">
      <h4 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
        <i class="fas fa-lightbulb text-amber-500"></i>
        {{ title }}
      </h4>
      <button
        v-if="!loading"
        @click="emit('refresh')"
        class="text-xs text-gray-400 hover:text-teal-600 flex items-center gap-1 transition-colors"
        title="Refresh suggestions"
      >
        <i class="fas fa-sync-alt"></i>
        Refresh
      </button>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="space-y-3">
      <div v-for="i in 3" :key="i" class="suggestion-skeleton">
        <div class="flex gap-3">
          <div class="w-12 h-12 bg-gray-200 rounded-lg animate-pulse"></div>
          <div class="flex-1 space-y-2">
            <div class="h-4 bg-gray-200 rounded w-3/4 animate-pulse"></div>
            <div class="h-3 bg-gray-200 rounded w-1/2 animate-pulse"></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else-if="suggestions.length === 0" class="empty-state text-center py-6">
      <div class="w-12 h-12 rounded-full bg-gray-100 flex items-center justify-center mx-auto mb-3">
        <i class="fas fa-inbox text-gray-400"></i>
      </div>
      <p class="text-sm text-gray-500">No suggestions available</p>
      <p class="text-xs text-gray-400 mt-1">Continue the conversation to get recommendations</p>
    </div>

    <!-- Suggestions List -->
    <div v-else class="space-y-2">
      <TransitionGroup name="suggestion-list">
        <div
          v-for="suggestion in visibleSuggestions"
          :key="suggestion.id"
          class="suggestion-card group"
        >
          <div class="flex gap-3">
            <!-- Thumbnail -->
            <div class="relative flex-shrink-0">
              <div
                v-if="suggestion.thumbnail"
                class="w-14 h-14 rounded-lg overflow-hidden bg-gray-100"
              >
                <img
                  :src="suggestion.thumbnail"
                  :alt="suggestion.title"
                  class="w-full h-full object-cover"
                />
              </div>
              <div
                v-else
                :class="[
                  'w-14 h-14 rounded-lg flex items-center justify-center',
                  getTypeColor(suggestion.type)
                ]"
              >
                <i :class="[getTypeIcon(suggestion.type), 'text-lg']"></i>
              </div>

              <!-- Type Badge -->
              <div
                :class="[
                  'absolute -bottom-1 -right-1 w-5 h-5 rounded-full flex items-center justify-center text-[10px] shadow-sm',
                  getTypeColor(suggestion.type)
                ]"
              >
                <i :class="getTypeIcon(suggestion.type)"></i>
              </div>
            </div>

            <!-- Content -->
            <div class="flex-1 min-w-0">
              <!-- Title & Suggestion Type -->
              <div class="flex items-start justify-between gap-2">
                <button
                  @click="emit('select', suggestion)"
                  class="text-sm font-medium text-gray-800 hover:text-teal-600 text-left line-clamp-1 transition-colors"
                >
                  {{ suggestion.title }}
                </button>
                <span
                  :class="[
                    'flex-shrink-0 px-1.5 py-0.5 rounded text-[10px] font-medium flex items-center gap-1',
                    getSuggestionTypeBadge(suggestion.suggestionType).color
                  ]"
                >
                  <i :class="[getSuggestionTypeBadge(suggestion.suggestionType).icon, 'text-[8px]']"></i>
                  {{ getSuggestionTypeBadge(suggestion.suggestionType).label }}
                </span>
              </div>

              <!-- Reason -->
              <p class="text-xs text-gray-500 line-clamp-1 mt-0.5">
                {{ suggestion.reason }}
              </p>

              <!-- Meta Info -->
              <div class="flex items-center gap-3 mt-1.5">
                <!-- Relevance Score -->
                <span
                  v-if="showRelevance"
                  :class="['text-[10px] font-medium flex items-center gap-1', getRelevanceColor(suggestion.relevanceScore)]"
                >
                  <i class="fas fa-chart-line"></i>
                  {{ Math.round(suggestion.relevanceScore * 100) }}% match
                </span>

                <!-- Category -->
                <span v-if="suggestion.metadata?.category" class="text-[10px] text-gray-400">
                  {{ suggestion.metadata.category }}
                </span>

                <!-- Read Time -->
                <span v-if="suggestion.metadata?.readTime" class="text-[10px] text-gray-400 flex items-center gap-1">
                  <i class="fas fa-clock text-[8px]"></i>
                  {{ suggestion.metadata.readTime }}
                </span>
              </div>
            </div>

            <!-- Actions -->
            <div class="flex flex-col gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
              <button
                @click="emit('select', suggestion)"
                class="w-7 h-7 rounded-lg bg-teal-100 text-teal-600 hover:bg-teal-200 flex items-center justify-center transition-colors"
                title="View"
              >
                <i class="fas fa-arrow-right text-xs"></i>
              </button>
              <button
                @click.stop="emit('dismiss', suggestion.id)"
                class="w-7 h-7 rounded-lg bg-gray-100 text-gray-400 hover:bg-gray-200 hover:text-gray-600 flex items-center justify-center transition-colors"
                title="Dismiss"
              >
                <i class="fas fa-times text-xs"></i>
              </button>
            </div>
          </div>
        </div>
      </TransitionGroup>

      <!-- Show More/Less Button -->
      <button
        v-if="hasMore"
        @click="showAll = !showAll"
        class="w-full py-2 text-xs text-gray-500 hover:text-teal-600 flex items-center justify-center gap-1 transition-colors"
      >
        <span>{{ showAll ? 'Show less' : `Show ${suggestions.length - maxVisible} more` }}</span>
        <i :class="['fas', showAll ? 'fa-chevron-up' : 'fa-chevron-down', 'text-[10px]']"></i>
      </button>
    </div>
  </div>
</template>

<style scoped>
.ai-content-suggestions {
  @apply bg-white rounded-xl p-4 border border-gray-100;
}

.suggestion-card {
  @apply p-3 rounded-xl bg-gray-50 hover:bg-gray-100/80 transition-all cursor-pointer;
}

/* List transition */
.suggestion-list-enter-active,
.suggestion-list-leave-active {
  transition: all 0.3s ease;
}

.suggestion-list-enter-from,
.suggestion-list-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

.suggestion-list-move {
  transition: transform 0.3s ease;
}
</style>
