<script setup lang="ts">
import { computed } from 'vue'
import AIConfidenceBar from './AIConfidenceBar.vue'

interface Props {
  title?: string
  content: string
  confidence?: number
  type?: 'summary' | 'translation' | 'classification' | 'general'
  showCopy?: boolean
  showRegenerate?: boolean
  showDismiss?: boolean
  loading?: boolean
  error?: string
}

const props = withDefaults(defineProps<Props>(), {
  title: 'AI Result',
  type: 'general',
  showCopy: true,
  showRegenerate: true,
  showDismiss: true,
  loading: false,
})

const emit = defineEmits<{
  copy: []
  regenerate: []
  dismiss: []
}>()

const typeConfig = computed(() => {
  switch (props.type) {
    case 'summary':
      return { icon: 'fas fa-compress-alt', color: 'teal', label: 'AI Summary' }
    case 'translation':
      return { icon: 'fas fa-language', color: 'blue', label: 'AI Translation' }
    case 'classification':
      return { icon: 'fas fa-tags', color: 'purple', label: 'AI Classification' }
    default:
      return { icon: 'fas fa-wand-magic-sparkles', color: 'teal', label: 'AI Result' }
  }
})

async function copyToClipboard() {
  try {
    await navigator.clipboard.writeText(props.content)
    emit('copy')
  } catch (err) {
    console.error('Failed to copy:', err)
  }
}
</script>

<template>
  <div
    class="ai-result-card bg-gradient-to-br from-white to-gray-50 rounded-xl border border-gray-100 shadow-sm overflow-hidden"
    :class="{ 'animate-pulse': loading }"
  >
    <!-- Header -->
    <div class="px-4 py-3 bg-gradient-to-r from-teal-50 to-transparent border-b border-gray-100">
      <div class="flex items-center justify-between">
        <div class="flex items-center gap-2">
          <div class="w-8 h-8 rounded-lg bg-teal-100 flex items-center justify-center">
            <i :class="[typeConfig.icon, 'text-teal-600']" />
          </div>
          <div>
            <h4 class="text-sm font-semibold text-gray-800">{{ title || typeConfig.label }}</h4>
            <p class="text-xs text-gray-500">Powered by Intalio AI</p>
          </div>
        </div>

        <!-- Dismiss button -->
        <button
          v-if="showDismiss"
          type="button"
          class="p-1.5 rounded-lg text-gray-400 hover:text-gray-600 hover:bg-gray-100 transition-colors"
          @click="emit('dismiss')"
        >
          <i class="fas fa-times" />
        </button>
      </div>
    </div>

    <!-- Content -->
    <div class="p-4">
      <!-- Error state -->
      <div v-if="error" class="flex items-center gap-3 text-red-600 bg-red-50 rounded-lg p-3">
        <i class="fas fa-exclamation-circle" />
        <span class="text-sm">{{ error }}</span>
      </div>

      <!-- Loading state -->
      <div v-else-if="loading" class="space-y-2">
        <div class="h-4 bg-gray-200 rounded w-full" />
        <div class="h-4 bg-gray-200 rounded w-5/6" />
        <div class="h-4 bg-gray-200 rounded w-4/6" />
      </div>

      <!-- Result content -->
      <div v-else>
        <p class="text-gray-700 text-sm leading-relaxed whitespace-pre-wrap">
          {{ content }}
        </p>

        <!-- Confidence bar -->
        <div v-if="confidence !== undefined" class="mt-4">
          <AIConfidenceBar
            :value="confidence"
            size="sm"
            show-label
            label="AI Confidence"
          />
        </div>
      </div>
    </div>

    <!-- Actions -->
    <div v-if="!loading && !error" class="px-4 py-3 bg-gray-50 border-t border-gray-100 flex items-center gap-2">
      <button
        v-if="showCopy"
        type="button"
        class="inline-flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium text-gray-600 hover:text-gray-800 hover:bg-gray-100 rounded-lg transition-colors"
        @click="copyToClipboard"
      >
        <i class="fas fa-copy" />
        Copy
      </button>

      <button
        v-if="showRegenerate"
        type="button"
        class="inline-flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium text-teal-600 hover:text-teal-700 hover:bg-teal-50 rounded-lg transition-colors"
        @click="emit('regenerate')"
      >
        <i class="fas fa-sync-alt" />
        Regenerate
      </button>
    </div>
  </div>
</template>

<style scoped>
.ai-result-card {
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
