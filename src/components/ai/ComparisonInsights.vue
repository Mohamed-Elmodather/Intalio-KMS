<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useComparisonStore } from '@/stores/comparison'

const { t } = useI18n()

interface Props {
  section: 'summary' | 'entities' | 'sentiment' | 'topics'
}

const props = defineProps<Props>()
const comparisonStore = useComparisonStore()

const analysis = computed(() => comparisonStore.analysisResults)
const isLoading = computed(() => comparisonStore.isAnalyzing)

// Entity type icons
const entityTypeIcons: Record<string, string> = {
  person: 'fas fa-user',
  organization: 'fas fa-building',
  location: 'fas fa-map-marker-alt',
  date: 'fas fa-calendar',
  topic: 'fas fa-hashtag',
}

const entityTypeColors: Record<string, string> = {
  person: 'bg-blue-100 text-blue-700 border-blue-200',
  organization: 'bg-purple-100 text-purple-700 border-purple-200',
  location: 'bg-green-100 text-green-700 border-green-200',
  date: 'bg-orange-100 text-orange-700 border-orange-200',
  topic: 'bg-teal-100 text-teal-700 border-teal-200',
}

function getSentimentColor(score: number): string {
  if (score > 0.3) return 'bg-green-500'
  if (score < -0.3) return 'bg-red-500'
  return 'bg-gray-400'
}

function getSentimentLabel(score: number): string {
  if (score > 0.3) return t('comparison.positive')
  if (score < -0.3) return t('comparison.negative')
  return t('comparison.neutral')
}
</script>

<template>
  <div class="comparison-insights">
    <!-- Loading State -->
    <div v-if="isLoading" class="text-center py-16">
      <div class="w-16 h-16 rounded-full bg-purple-100 mx-auto mb-4 flex items-center justify-center">
        <i class="fas fa-brain text-2xl text-purple-600 animate-pulse" />
      </div>
      <h3 class="text-lg font-medium text-gray-700 mb-2">{{ $t('comparison.analyzingContent') }}</h3>
      <p class="text-gray-500">{{ $t('comparison.generatingInsights') }}</p>
      <div class="mt-4 flex justify-center gap-1">
        <div class="w-2 h-2 rounded-full bg-purple-400 animate-bounce" style="animation-delay: 0s" />
        <div class="w-2 h-2 rounded-full bg-purple-400 animate-bounce" style="animation-delay: 0.1s" />
        <div class="w-2 h-2 rounded-full bg-purple-400 animate-bounce" style="animation-delay: 0.2s" />
      </div>
    </div>

    <!-- No Analysis State -->
    <div v-else-if="!analysis" class="text-center py-16">
      <div class="w-16 h-16 rounded-full bg-gray-100 mx-auto mb-4 flex items-center justify-center">
        <i class="fas fa-magic text-2xl text-gray-400" />
      </div>
      <h3 class="text-lg font-medium text-gray-700 mb-2">{{ $t('comparison.noAnalysisYet') }}</h3>
      <p class="text-gray-500 mb-4">{{ $t('comparison.clickGenerateAnalysis') }}</p>
      <button
        class="px-6 py-2.5 bg-purple-500 text-white rounded-xl font-medium hover:bg-purple-600 transition-colors inline-flex items-center gap-2"
        @click="comparisonStore.generateAnalysis()"
      >
        <i class="fas fa-magic" />
        <span>{{ $t('comparison.generateAnalysis') }}</span>
      </button>
    </div>

    <!-- Analysis Content -->
    <div v-else>
      <!-- Summary Section -->
      <div v-if="section === 'summary'" class="space-y-6">
        <!-- Similarity Score -->
        <div class="bg-gradient-to-r from-teal-50 to-cyan-50 rounded-xl p-6 border border-teal-100">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-800">{{ $t('comparison.similarityScore') }}</h3>
            <span class="text-3xl font-bold text-teal-600">{{ analysis.similarity }}%</span>
          </div>
          <div class="h-3 bg-white/50 rounded-full overflow-hidden">
            <div
              class="h-full bg-gradient-to-r from-teal-400 to-teal-600 rounded-full transition-all duration-1000"
              :style="{ width: `${analysis.similarity}%` }"
            />
          </div>
          <p class="mt-2 text-sm text-teal-700">
            {{ $t('comparison.basedOnContentThemes') }}
          </p>
        </div>

        <!-- AI Summary -->
        <div class="bg-white rounded-xl p-6 border border-gray-200">
          <div class="flex items-center gap-3 mb-4">
            <div class="w-10 h-10 rounded-lg bg-purple-100 flex items-center justify-center">
              <i class="fas fa-brain text-purple-600" />
            </div>
            <h3 class="text-lg font-semibold text-gray-800">{{ $t('comparison.aiSummary') }}</h3>
          </div>
          <p class="text-gray-700 leading-relaxed">{{ analysis.summary }}</p>
        </div>

        <!-- Key Differences -->
        <div class="bg-white rounded-xl p-6 border border-gray-200">
          <div class="flex items-center gap-3 mb-4">
            <div class="w-10 h-10 rounded-lg bg-amber-100 flex items-center justify-center">
              <i class="fas fa-not-equal text-amber-600" />
            </div>
            <h3 class="text-lg font-semibold text-gray-800">{{ $t('comparison.keyDifferences') }}</h3>
          </div>
          <ul class="space-y-2">
            <li
              v-for="(diff, idx) in analysis.differences"
              :key="idx"
              class="flex items-start gap-3"
            >
              <span class="w-6 h-6 rounded-full bg-amber-100 text-amber-600 flex items-center justify-center text-xs flex-shrink-0 mt-0.5">
                {{ idx + 1 }}
              </span>
              <span class="text-gray-700">{{ diff }}</span>
            </li>
          </ul>
        </div>

        <!-- Key Points per Item -->
        <div class="bg-white rounded-xl p-6 border border-gray-200">
          <div class="flex items-center gap-3 mb-4">
            <div class="w-10 h-10 rounded-lg bg-blue-100 flex items-center justify-center">
              <i class="fas fa-list-check text-blue-600" />
            </div>
            <h3 class="text-lg font-semibold text-gray-800">{{ $t('comparison.keyPoints') }}</h3>
          </div>
          <div class="space-y-4">
            <div
              v-for="keyPoint in analysis.keyPoints"
              :key="keyPoint.itemId"
              class="p-4 bg-gray-50 rounded-lg"
            >
              <h4 class="font-medium text-gray-800 mb-2">
                {{ comparisonStore.selectedItems.find(i => i.id === keyPoint.itemId)?.title || 'Item' }}
              </h4>
              <ul class="space-y-1">
                <li
                  v-for="(point, idx) in keyPoint.points"
                  :key="idx"
                  class="flex items-center gap-2 text-sm text-gray-600"
                >
                  <i class="fas fa-check text-green-500 text-xs" />
                  <span>{{ point }}</span>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>

      <!-- Entities Section -->
      <div v-else-if="section === 'entities'" class="space-y-6">
        <div class="bg-white rounded-xl p-6 border border-gray-200">
          <div class="flex items-center gap-3 mb-4">
            <div class="w-10 h-10 rounded-lg bg-teal-100 flex items-center justify-center">
              <i class="fas fa-tags text-teal-600" />
            </div>
            <h3 class="text-lg font-semibold text-gray-800">{{ $t('comparison.commonEntities') }}</h3>
            <span class="ml-auto text-sm text-gray-500">
              {{ $t('comparison.entitiesFound', { count: analysis.commonEntities.length }) }}
            </span>
          </div>

          <div class="grid gap-3">
            <div
              v-for="entity in analysis.commonEntities"
              :key="entity.name"
              class="flex items-center gap-4 p-3 rounded-lg border"
              :class="entityTypeColors[entity.type]"
            >
              <div class="w-10 h-10 rounded-lg bg-white/50 flex items-center justify-center">
                <i :class="entityTypeIcons[entity.type]" />
              </div>
              <div class="flex-1">
                <div class="font-medium">{{ entity.name }}</div>
                <div class="text-sm opacity-75 capitalize">{{ entity.type }}</div>
              </div>
              <div class="text-right">
                <div class="font-semibold">{{ entity.occurrences }}</div>
                <div class="text-xs opacity-75">{{ $t('comparison.occurrences') }}</div>
              </div>
              <div class="text-right">
                <div class="font-semibold">{{ entity.items.length }}</div>
                <div class="text-xs opacity-75">{{ $t('comparison.items') }}</div>
              </div>
            </div>
          </div>

          <div v-if="analysis.commonEntities.length === 0" class="text-center py-8 text-gray-500">
            {{ $t('comparison.noCommonEntities') }}
          </div>
        </div>
      </div>

      <!-- Sentiment Section -->
      <div v-else-if="section === 'sentiment'" class="space-y-6">
        <div class="bg-white rounded-xl p-6 border border-gray-200">
          <div class="flex items-center gap-3 mb-6">
            <div class="w-10 h-10 rounded-lg bg-pink-100 flex items-center justify-center">
              <i class="fas fa-smile text-pink-600" />
            </div>
            <h3 class="text-lg font-semibold text-gray-800">{{ $t('comparison.sentimentComparison') }}</h3>
          </div>

          <div class="space-y-4">
            <div
              v-for="sentiment in analysis.sentimentComparison"
              :key="sentiment.itemId"
              class="p-4 bg-gray-50 rounded-lg"
            >
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-medium text-gray-800 truncate max-w-[70%]">
                  {{ sentiment.itemTitle }}
                </h4>
                <span
                  class="px-3 py-1 rounded-full text-sm font-medium"
                  :class="{
                    'bg-green-100 text-green-700': sentiment.score > 0.3,
                    'bg-red-100 text-red-700': sentiment.score < -0.3,
                    'bg-gray-100 text-gray-700': sentiment.score >= -0.3 && sentiment.score <= 0.3,
                  }"
                >
                  {{ getSentimentLabel(sentiment.score) }}
                </span>
              </div>
              <div class="flex items-center gap-3">
                <div class="flex-1 h-2 bg-gray-200 rounded-full overflow-hidden">
                  <div
                    class="h-full rounded-full transition-all duration-500"
                    :class="getSentimentColor(sentiment.score)"
                    :style="{ width: `${(sentiment.score + 1) * 50}%` }"
                  />
                </div>
                <span class="text-sm text-gray-500 w-16 text-right">
                  {{ Math.round(sentiment.confidence * 100) }}% conf
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Sentiment Summary -->
        <div class="bg-gradient-to-r from-pink-50 to-rose-50 rounded-xl p-6 border border-pink-100">
          <h4 class="font-semibold text-gray-800 mb-3">{{ $t('comparison.overallSentiment') }}</h4>
          <div class="flex items-center gap-4">
            <div class="flex gap-2">
              <div class="text-center">
                <div class="w-12 h-12 rounded-full bg-green-100 flex items-center justify-center mb-1">
                  <i class="fas fa-smile text-green-600" />
                </div>
                <span class="text-xs text-gray-600">
                  {{ analysis.sentimentComparison.filter(s => s.score > 0.3).length }}
                </span>
              </div>
              <div class="text-center">
                <div class="w-12 h-12 rounded-full bg-gray-100 flex items-center justify-center mb-1">
                  <i class="fas fa-meh text-gray-600" />
                </div>
                <span class="text-xs text-gray-600">
                  {{ analysis.sentimentComparison.filter(s => s.score >= -0.3 && s.score <= 0.3).length }}
                </span>
              </div>
              <div class="text-center">
                <div class="w-12 h-12 rounded-full bg-red-100 flex items-center justify-center mb-1">
                  <i class="fas fa-frown text-red-600" />
                </div>
                <span class="text-xs text-gray-600">
                  {{ analysis.sentimentComparison.filter(s => s.score < -0.3).length }}
                </span>
              </div>
            </div>
            <div class="flex-1 text-sm text-gray-600">
              {{ $t('comparison.sentimentSummary') }}
            </div>
          </div>
        </div>
      </div>

      <!-- Topics Section -->
      <div v-else-if="section === 'topics'" class="space-y-6">
        <div class="bg-white rounded-xl p-6 border border-gray-200">
          <div class="flex items-center gap-3 mb-6">
            <div class="w-10 h-10 rounded-lg bg-indigo-100 flex items-center justify-center">
              <i class="fas fa-lightbulb text-indigo-600" />
            </div>
            <h3 class="text-lg font-semibold text-gray-800">{{ $t('comparison.commonTopics') }}</h3>
          </div>

          <div class="space-y-4">
            <div
              v-for="topic in analysis.commonTopics"
              :key="topic.topic"
              class="p-4 bg-gray-50 rounded-lg"
            >
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-medium text-gray-800">{{ topic.topic }}</h4>
                <span class="text-sm text-gray-500">
                  {{ topic.items.length }} / {{ comparisonStore.itemCount }} items
                </span>
              </div>
              <div class="flex items-center gap-3">
                <div class="flex-1 h-2 bg-gray-200 rounded-full overflow-hidden">
                  <div
                    class="h-full bg-gradient-to-r from-indigo-400 to-indigo-600 rounded-full transition-all duration-500"
                    :style="{ width: `${topic.relevance * 100}%` }"
                  />
                </div>
                <span class="text-sm font-medium text-indigo-600 w-12 text-right">
                  {{ Math.round(topic.relevance * 100) }}%
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Classifications -->
        <div class="bg-white rounded-xl p-6 border border-gray-200">
          <div class="flex items-center gap-3 mb-4">
            <div class="w-10 h-10 rounded-lg bg-cyan-100 flex items-center justify-center">
              <i class="fas fa-layer-group text-cyan-600" />
            </div>
            <h3 class="text-lg font-semibold text-gray-800">{{ $t('comparison.contentClassifications') }}</h3>
          </div>

          <div class="grid gap-3 sm:grid-cols-2">
            <div
              v-for="classification in analysis.classifications"
              :key="classification.itemId"
              class="p-4 bg-gray-50 rounded-lg"
            >
              <div class="text-sm text-gray-500 mb-1 truncate">
                {{ comparisonStore.selectedItems.find(i => i.id === classification.itemId)?.title || 'Item' }}
              </div>
              <div class="flex items-center justify-between">
                <span class="font-medium text-gray-800">{{ classification.category }}</span>
                <span class="text-sm text-cyan-600">
                  {{ Math.round(classification.confidence * 100) }}%
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
@keyframes bounce {
  0%, 100% {
    transform: translateY(-25%);
    animation-timing-function: cubic-bezier(0.8, 0, 1, 1);
  }
  50% {
    transform: translateY(0);
    animation-timing-function: cubic-bezier(0, 0, 0.2, 1);
  }
}

.animate-bounce {
  animation: bounce 1s infinite;
}
</style>
