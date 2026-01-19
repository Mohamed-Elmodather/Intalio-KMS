<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import type { AIOperationType } from '@/types/ai'

const { t } = useI18n()

interface Props {
  operation: AIOperationType
  progress?: number
  status: 'pending' | 'processing' | 'success' | 'error'
  message?: string
  showCancel?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  progress: 0,
  showCancel: true
})

const emit = defineEmits<{
  cancel: []
}>()

// Operation metadata
const operationMeta = computed<Record<AIOperationType, { name: string; icon: string; color: string }>>(() => ({
  'extract-entities': { name: t('ai.operations.extractingEntities'), icon: 'fas fa-tags', color: 'blue' },
  'analyze-sentiment': { name: t('ai.operations.analyzingSentiment'), icon: 'fas fa-smile', color: 'pink' },
  'summarize': { name: t('ai.operations.generatingSummary'), icon: 'fas fa-compress-alt', color: 'teal' },
  'classify': { name: t('ai.operations.classifyingContent'), icon: 'fas fa-folder-tree', color: 'purple' },
  'ocr': { name: t('ai.operations.extractingTextOCR'), icon: 'fas fa-file-image', color: 'amber' },
  'translate': { name: t('ai.operations.translating'), icon: 'fas fa-language', color: 'green' },
  'generate-title': { name: t('ai.operations.generatingTitles'), icon: 'fas fa-heading', color: 'indigo' },
  'auto-tag': { name: t('ai.operations.autoTagging'), icon: 'fas fa-hashtag', color: 'cyan' },
  'smart-search': { name: t('ai.operations.searching'), icon: 'fas fa-search', color: 'orange' },
  'chat': { name: t('ai.operations.chatting'), icon: 'fas fa-comment', color: 'teal' }
}))

const meta = computed(() => operationMeta.value[props.operation] || {
  name: t('ai.operations.processing'),
  icon: 'fas fa-cog',
  color: 'gray'
})

const statusMessage = computed(() => {
  if (props.message) return props.message

  switch (props.status) {
    case 'pending':
      return t('ai.status.waitingToStart')
    case 'processing':
      return props.progress > 0 ? t('ai.status.percentComplete', { percent: props.progress }) : t('ai.status.processing')
    case 'success':
      return t('ai.status.completedSuccessfully')
    case 'error':
      return t('ai.status.errorOccurred')
    default:
      return ''
  }
})

const colorClasses = computed(() => {
  const colors: Record<string, { bg: string; progress: string; icon: string }> = {
    blue: { bg: 'from-blue-50 to-blue-100', progress: 'bg-blue-500', icon: 'bg-blue-500' },
    pink: { bg: 'from-pink-50 to-pink-100', progress: 'bg-pink-500', icon: 'bg-pink-500' },
    teal: { bg: 'from-teal-50 to-emerald-100', progress: 'bg-teal-500', icon: 'bg-teal-500' },
    purple: { bg: 'from-purple-50 to-purple-100', progress: 'bg-purple-500', icon: 'bg-purple-500' },
    amber: { bg: 'from-amber-50 to-amber-100', progress: 'bg-amber-500', icon: 'bg-amber-500' },
    green: { bg: 'from-green-50 to-green-100', progress: 'bg-green-500', icon: 'bg-green-500' },
    indigo: { bg: 'from-indigo-50 to-indigo-100', progress: 'bg-indigo-500', icon: 'bg-indigo-500' },
    cyan: { bg: 'from-cyan-50 to-cyan-100', progress: 'bg-cyan-500', icon: 'bg-cyan-500' },
    orange: { bg: 'from-orange-50 to-orange-100', progress: 'bg-orange-500', icon: 'bg-orange-500' },
    gray: { bg: 'from-gray-50 to-gray-100', progress: 'bg-gray-500', icon: 'bg-gray-500' }
  }
  return colors[meta.value.color] || colors.gray
})
</script>

<template>
  <div
    :class="[
      'ai-operation-progress rounded-xl p-4 bg-gradient-to-r border transition-all',
      colorClasses.bg,
      status === 'error' ? 'border-red-200' : 'border-transparent'
    ]"
  >
    <div class="flex items-center gap-3">
      <!-- Icon -->
      <div
        :class="[
          'w-10 h-10 rounded-lg flex items-center justify-center text-white',
          colorClasses.icon,
          status === 'processing' && 'animate-pulse'
        ]"
      >
        <i v-if="status === 'success'" class="fas fa-check"></i>
        <i v-else-if="status === 'error'" class="fas fa-exclamation-triangle"></i>
        <i v-else :class="[meta.icon, status === 'processing' && 'animate-spin']"></i>
      </div>

      <!-- Content -->
      <div class="flex-1 min-w-0">
        <div class="flex items-center justify-between mb-1">
          <span class="font-medium text-gray-800">{{ meta.name }}</span>
          <span
            v-if="status === 'processing' && progress > 0"
            class="text-xs text-gray-500"
          >
            {{ progress }}%
          </span>
        </div>

        <!-- Progress Bar -->
        <div
          v-if="status === 'processing'"
          class="w-full bg-white/50 rounded-full h-2 overflow-hidden"
        >
          <div
            :class="['h-full rounded-full transition-all duration-300', colorClasses.progress, { 'animate-progress-indeterminate': progress === 0 }]"
            :style="{ width: progress > 0 ? `${progress}%` : '100%' }"
          ></div>
        </div>

        <!-- Status Message -->
        <p
          :class="[
            'text-xs mt-1',
            status === 'error' ? 'text-red-600' : 'text-gray-500'
          ]"
        >
          {{ statusMessage }}
        </p>
      </div>

      <!-- Cancel Button -->
      <button
        v-if="showCancel && status === 'processing'"
        @click="emit('cancel')"
        class="w-8 h-8 rounded-lg bg-white/50 text-gray-400 hover:text-gray-600 hover:bg-white flex items-center justify-center transition-colors"
        :title="$t('common.cancel')"
      >
        <i class="fas fa-times"></i>
      </button>

      <!-- Status Icon -->
      <div v-else-if="status === 'success'" class="w-8 h-8 rounded-lg bg-green-100 text-green-600 flex items-center justify-center">
        <i class="fas fa-check"></i>
      </div>
      <div v-else-if="status === 'error'" class="w-8 h-8 rounded-lg bg-red-100 text-red-600 flex items-center justify-center">
        <i class="fas fa-times"></i>
      </div>
    </div>
  </div>
</template>

<style scoped>
@keyframes progress-indeterminate {
  0% {
    transform: translateX(-100%);
  }
  100% {
    transform: translateX(200%);
  }
}

.animate-progress-indeterminate {
  width: 50% !important;
  animation: progress-indeterminate 1.5s ease-in-out infinite;
}
</style>
