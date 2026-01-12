<script setup lang="ts">
import { computed } from 'vue'
import type { SentimentType, EmotionScore } from '@/types/ai'

interface Props {
  sentiment: SentimentType
  score?: number
  confidence?: number
  emotions?: EmotionScore[]
  size?: 'sm' | 'md' | 'lg'
  showLabel?: boolean
  showScore?: boolean
  showEmotions?: boolean
  interactive?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  showLabel: true,
  showScore: false,
  showEmotions: false,
  interactive: false,
})

const sentimentConfig = computed(() => {
  switch (props.sentiment) {
    case 'positive':
      return {
        icon: 'fas fa-smile',
        label: 'Positive',
        bgClass: 'bg-green-100',
        textClass: 'text-green-700',
        borderClass: 'border-green-300',
        barColor: 'bg-green-500',
      }
    case 'negative':
      return {
        icon: 'fas fa-frown',
        label: 'Negative',
        bgClass: 'bg-red-100',
        textClass: 'text-red-700',
        borderClass: 'border-red-300',
        barColor: 'bg-red-500',
      }
    default:
      return {
        icon: 'fas fa-meh',
        label: 'Neutral',
        bgClass: 'bg-gray-100',
        textClass: 'text-gray-700',
        borderClass: 'border-gray-300',
        barColor: 'bg-gray-500',
      }
  }
})

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'sm':
      return {
        container: 'px-2 py-1 text-xs gap-1',
        icon: 'text-xs',
      }
    case 'lg':
      return {
        container: 'px-4 py-2 text-base gap-2',
        icon: 'text-lg',
      }
    default:
      return {
        container: 'px-3 py-1.5 text-sm gap-1.5',
        icon: 'text-sm',
      }
  }
})

const normalizedScore = computed(() => {
  if (props.score === undefined) return 50
  // Convert -1 to 1 range to 0 to 100
  return Math.round((props.score + 1) * 50)
})

const emotionIcons: Record<string, string> = {
  joy: 'fas fa-laugh-beam',
  sadness: 'fas fa-sad-tear',
  anger: 'fas fa-angry',
  fear: 'fas fa-grimace',
  surprise: 'fas fa-surprise',
  disgust: 'fas fa-dizzy',
  trust: 'fas fa-handshake',
  anticipation: 'fas fa-hourglass-half',
}
</script>

<template>
  <div class="ai-sentiment-badge inline-block">
    <!-- Main badge -->
    <div
      class="inline-flex items-center rounded-full border font-medium"
      :class="[
        sizeClasses.container,
        sentimentConfig.bgClass,
        sentimentConfig.textClass,
        sentimentConfig.borderClass,
        interactive ? 'cursor-pointer hover:shadow-sm transition-shadow' : '',
      ]"
    >
      <!-- Icon -->
      <i :class="[sentimentConfig.icon, sizeClasses.icon]" />

      <!-- Label -->
      <span v-if="showLabel">{{ sentimentConfig.label }}</span>

      <!-- Score -->
      <span v-if="showScore && score !== undefined" class="opacity-75">
        ({{ normalizedScore }}%)
      </span>
    </div>

    <!-- Emotions breakdown -->
    <div v-if="showEmotions && emotions?.length" class="mt-2 space-y-1.5">
      <div
        v-for="emotion in emotions"
        :key="emotion.emotion"
        class="flex items-center gap-2"
      >
        <div class="w-24 flex items-center gap-1.5">
          <i :class="[emotionIcons[emotion.emotion] || 'fas fa-circle', 'text-xs text-gray-400']" />
          <span class="text-xs text-gray-600 capitalize">{{ emotion.emotion }}</span>
        </div>
        <div class="flex-1 h-1.5 bg-gray-100 rounded-full overflow-hidden">
          <div
            class="h-full rounded-full transition-all duration-500"
            :class="sentimentConfig.barColor"
            :style="{ width: `${Math.round(emotion.score * 100)}%` }"
          />
        </div>
        <span class="text-xs text-gray-500 w-10 text-right">
          {{ Math.round(emotion.score * 100) }}%
        </span>
      </div>
    </div>

    <!-- Confidence indicator -->
    <div v-if="confidence !== undefined" class="mt-1">
      <span class="text-xs text-gray-400">
        Confidence: {{ Math.round(confidence * 100) }}%
      </span>
    </div>
  </div>
</template>

<style scoped>
.ai-sentiment-badge {
  /* No additional styles needed */
}
</style>
