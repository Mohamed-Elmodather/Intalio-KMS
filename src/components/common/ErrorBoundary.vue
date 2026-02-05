<script setup lang="ts">
import { ref, onErrorCaptured } from 'vue'
import { useI18n } from 'vue-i18n'
import { handleError, getUserMessage, type AppError } from '@/utils/errorHandler'

interface Props {
  fallbackMessage?: string
  showRetry?: boolean
  showDetails?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  fallbackMessage: '',
  showRetry: true,
  showDetails: false,
})

const emit = defineEmits<{
  error: [error: AppError]
  retry: []
}>()

const { t } = useI18n()
const hasError = ref(false)
const errorInfo = ref<AppError | null>(null)
const showErrorDetails = ref(false)

// Capture errors from child components
onErrorCaptured((err, instance, info) => {
  const componentName = instance?.$options?.name || instance?.$options?.__name || 'Unknown'

  const appError = handleError(err, {
    component: componentName,
    action: info,
  }) as AppError

  hasError.value = true
  errorInfo.value = appError
  emit('error', appError)

  // Return false to prevent the error from propagating further
  return false
})

function retry() {
  hasError.value = false
  errorInfo.value = null
  emit('retry')
}

function toggleDetails() {
  showErrorDetails.value = !showErrorDetails.value
}

function getDisplayMessage(): string {
  if (props.fallbackMessage) {
    return props.fallbackMessage
  }
  if (errorInfo.value) {
    return getUserMessage(errorInfo.value)
  }
  return t('errors.general')
}
</script>

<template>
  <slot v-if="!hasError" />

  <div v-else class="error-boundary">
    <div class="error-boundary__content">
      <div class="error-boundary__icon">
        <i class="fas fa-exclamation-triangle"></i>
      </div>

      <h3 class="error-boundary__title">
        {{ t('errors.somethingWentWrong') }}
      </h3>

      <p class="error-boundary__message">
        {{ getDisplayMessage() }}
      </p>

      <div class="error-boundary__actions">
        <button v-if="showRetry" class="error-boundary__retry-btn" @click="retry">
          <i class="fas fa-redo mr-2"></i>
          {{ t('common.tryAgain') }}
        </button>

        <button
          v-if="showDetails && errorInfo"
          class="error-boundary__details-btn"
          @click="toggleDetails"
        >
          <i :class="['fas', showErrorDetails ? 'fa-chevron-up' : 'fa-chevron-down', 'mr-2']"></i>
          {{ showErrorDetails ? t('common.hideDetails') : t('common.showDetails') }}
        </button>
      </div>

      <div v-if="showErrorDetails && errorInfo" class="error-boundary__details">
        <div class="error-boundary__detail-item">
          <strong>{{ t('errors.category') }}:</strong>
          <span class="error-boundary__badge" :class="`error-boundary__badge--${errorInfo.severity}`">
            {{ errorInfo.category }}
          </span>
        </div>
        <div class="error-boundary__detail-item">
          <strong>{{ t('errors.message') }}:</strong>
          {{ errorInfo.message }}
        </div>
        <div v-if="errorInfo.context.component" class="error-boundary__detail-item">
          <strong>{{ t('errors.component') }}:</strong>
          {{ errorInfo.context.component }}
        </div>
        <div v-if="errorInfo.stack" class="error-boundary__stack">
          <strong>{{ t('errors.stackTrace') }}:</strong>
          <pre>{{ errorInfo.stack }}</pre>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.error-boundary {
  @apply flex items-center justify-center min-h-[200px] p-6;
}

.error-boundary__content {
  @apply text-center max-w-md;
}

.error-boundary__icon {
  @apply w-16 h-16 mx-auto mb-4 rounded-full bg-red-100 flex items-center justify-center;
}

.error-boundary__icon i {
  @apply text-3xl text-red-500;
}

.error-boundary__title {
  @apply text-xl font-semibold text-gray-900 mb-2;
}

.error-boundary__message {
  @apply text-gray-600 mb-6;
}

.error-boundary__actions {
  @apply flex items-center justify-center gap-3 flex-wrap;
}

.error-boundary__retry-btn {
  @apply px-4 py-2 bg-teal-600 text-white rounded-lg hover:bg-teal-700 transition-colors;
}

.error-boundary__details-btn {
  @apply px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors;
}

.error-boundary__details {
  @apply mt-6 text-left bg-gray-50 rounded-lg p-4 text-sm;
}

.error-boundary__detail-item {
  @apply mb-2;
}

.error-boundary__badge {
  @apply inline-block px-2 py-0.5 rounded text-xs font-medium ml-2;
}

.error-boundary__badge--low {
  @apply bg-blue-100 text-blue-700;
}

.error-boundary__badge--medium {
  @apply bg-yellow-100 text-yellow-700;
}

.error-boundary__badge--high {
  @apply bg-orange-100 text-orange-700;
}

.error-boundary__badge--critical {
  @apply bg-red-100 text-red-700;
}

.error-boundary__stack {
  @apply mt-4;
}

.error-boundary__stack pre {
  @apply mt-2 p-3 bg-gray-800 text-gray-100 rounded text-xs overflow-auto max-h-40;
}
</style>
