<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

interface Props {
  value: number // 0 to 1
  showValue?: boolean
  showLabel?: boolean
  label?: string
  size?: 'sm' | 'md' | 'lg'
  animated?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  showValue: true,
  showLabel: false,
  size: 'md',
  animated: true,
})

const percentage = computed(() => Math.round(props.value * 100))

const barColorClass = computed(() => {
  if (props.value >= 0.8) return 'bg-green-500'
  if (props.value >= 0.6) return 'bg-teal-500'
  if (props.value >= 0.4) return 'bg-amber-500'
  return 'bg-red-500'
})

const textColorClass = computed(() => {
  if (props.value >= 0.8) return 'text-green-600'
  if (props.value >= 0.6) return 'text-teal-600'
  if (props.value >= 0.4) return 'text-amber-600'
  return 'text-red-600'
})

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'sm': return { bar: 'h-1', text: 'text-xs' }
    case 'lg': return { bar: 'h-3', text: 'text-base' }
    default: return { bar: 'h-2', text: 'text-sm' }
  }
})

const confidenceLabel = computed(() => {
  if (props.value >= 0.9) return t('ai.confidence.veryHigh')
  if (props.value >= 0.75) return t('ai.confidence.high')
  if (props.value >= 0.6) return t('ai.confidence.moderate')
  if (props.value >= 0.4) return t('ai.confidence.low')
  return t('ai.confidence.veryLow')
})
</script>

<template>
  <div class="ai-confidence-bar">
    <!-- Label row -->
    <div v-if="showLabel || showValue" class="flex items-center justify-between mb-1">
      <span v-if="showLabel" :class="[sizeClasses.text, 'text-gray-600 font-medium']">
        {{ label || $t('ai.aiConfidence') }}
      </span>
      <div v-if="showValue" class="flex items-center gap-1.5">
        <span :class="[sizeClasses.text, textColorClass, 'font-semibold']">
          {{ percentage }}%
        </span>
        <span :class="[sizeClasses.text, 'text-gray-400']">
          ({{ confidenceLabel }})
        </span>
      </div>
    </div>

    <!-- Progress bar -->
    <div :class="[sizeClasses.bar, 'w-full bg-gray-100 rounded-full overflow-hidden']">
      <div
        :class="[
          sizeClasses.bar,
          barColorClass,
          'rounded-full transition-all duration-500',
          animated ? 'ai-confidence-fill' : '',
        ]"
        :style="{ width: `${percentage}%` }"
      />
    </div>
  </div>
</template>

<style scoped>
.ai-confidence-fill {
  animation: fillBar 0.8s ease-out forwards;
}

@keyframes fillBar {
  from {
    width: 0;
  }
}
</style>
