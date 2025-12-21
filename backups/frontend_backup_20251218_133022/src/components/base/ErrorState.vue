<script setup lang="ts">
/**
 * ErrorState Component
 *
 * A consistent error state display with icon, message, and retry action.
 * Used when data fetching fails or an error occurs.
 *
 * @example
 * <ErrorState
 *   :error="error"
 *   :title="t('errors.loadFailed')"
 *   show-retry
 *   @retry="handleRetry"
 * />
 */
import { computed, ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'

export interface ErrorStateProps {
  /** Error object for detailed message */
  error?: Error | null
  /** Custom error title */
  title?: string
  /** Arabic title */
  titleArabic?: string
  /** Custom error message (overrides error.message) */
  message?: string
  /** Arabic message */
  messageArabic?: string
  /** Show retry button */
  showRetry?: boolean
  /** Custom retry button label */
  retryLabel?: string
  /** Arabic retry label */
  retryLabelArabic?: string
  /** Show error details (for development) */
  showDetails?: boolean
  /** Size variant */
  size?: 'sm' | 'md' | 'lg'
  /** Style variant */
  variant?: 'default' | 'inline' | 'banner'
  /** Enable entrance animation */
  animated?: boolean
}

const props = withDefaults(defineProps<ErrorStateProps>(), {
  showRetry: true,
  showDetails: false,
  size: 'md',
  variant: 'default',
  animated: true
})

const emit = defineEmits<{
  retry: []
}>()

const prefersReducedMotion = useReducedMotion()
const isVisible = ref(false)
const isRetrying = ref(false)
const showDetailsExpanded = ref(false)
const { locale } = useI18n()

const isRTL = computed(() => locale.value === 'ar')

const displayTitle = computed(() => {
  if (props.title) {
    return isRTL.value && props.titleArabic ? props.titleArabic : props.title
  }
  return isRTL.value ? 'حدث خطأ' : 'Something went wrong'
})

const displayMessage = computed(() => {
  if (props.message) {
    return isRTL.value && props.messageArabic ? props.messageArabic : props.message
  }
  if (props.error?.message) {
    return props.error.message
  }
  return isRTL.value
    ? 'حدث خطأ غير متوقع. يرجى المحاولة مرة أخرى.'
    : 'An unexpected error occurred. Please try again.'
})

const displayRetryLabel = computed(() => {
  if (props.retryLabel) {
    return isRTL.value && props.retryLabelArabic ? props.retryLabelArabic : props.retryLabel
  }
  return isRTL.value ? 'إعادة المحاولة' : 'Try Again'
})

onMounted(() => {
  if (props.animated && !prefersReducedMotion.value) {
    requestAnimationFrame(() => {
      isVisible.value = true
    })
  } else {
    isVisible.value = true
  }
})

const handleRetry = async () => {
  isRetrying.value = true
  emit('retry')
  // Allow parent to handle async retry
  setTimeout(() => {
    isRetrying.value = false
  }, 1000)
}

const toggleDetails = () => {
  showDetailsExpanded.value = !showDetailsExpanded.value
}
</script>

<template>
  <div
    class="error-state"
    :class="[
      `error-state--${variant}`,
      `error-state--${size}`,
      {
        'error-state--rtl': isRTL,
        'error-state--animated': animated && !prefersReducedMotion,
        'error-state--visible': isVisible
      }
    ]"
    role="alert"
    aria-live="assertive"
  >
    <!-- Icon -->
    <div class="error-state__icon-container">
      <div class="error-state__icon-bg">
        <i class="pi pi-exclamation-triangle error-state__icon" aria-hidden="true" />
      </div>
    </div>

    <!-- Content -->
    <div class="error-state__content">
      <h3 class="error-state__title">{{ displayTitle }}</h3>
      <p class="error-state__message">{{ displayMessage }}</p>

      <!-- Error Details (Development) -->
      <div v-if="showDetails && error" class="error-state__details">
        <button
          class="error-state__details-toggle"
          @click="toggleDetails"
        >
          <i :class="['pi', showDetailsExpanded ? 'pi-chevron-up' : 'pi-chevron-down']" />
          <span>{{ showDetailsExpanded ? 'Hide Details' : 'Show Details' }}</span>
        </button>
        <pre v-if="showDetailsExpanded" class="error-state__details-content">
{{ error.stack || error.message }}
        </pre>
      </div>
    </div>

    <!-- Actions -->
    <div v-if="showRetry" class="error-state__actions">
      <button
        class="error-state__retry"
        :disabled="isRetrying"
        @click="handleRetry"
      >
        <i
          :class="['pi', isRetrying ? 'pi-spin pi-spinner' : 'pi-refresh']"
          aria-hidden="true"
        />
        <span>{{ displayRetryLabel }}</span>
      </button>
    </div>

    <!-- Custom slot -->
    <div v-if="$slots.default" class="error-state__extra">
      <slot />
    </div>
  </div>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  padding: $spacing-8;

  // Size variants
  &--sm {
    padding: $spacing-6;

    .error-state__icon-bg {
      width: 48px;
      height: 48px;
    }

    .error-state__icon {
      font-size: $font-size-xl;
    }

    .error-state__title {
      font-size: $font-size-base;
    }

    .error-state__message {
      font-size: $font-size-sm;
    }
  }

  &--md {
    .error-state__icon-bg {
      width: 64px;
      height: 64px;
    }

    .error-state__icon {
      font-size: $font-size-2xl;
    }

    .error-state__title {
      font-size: $font-size-lg;
    }

    .error-state__message {
      font-size: $font-size-base;
    }
  }

  &--lg {
    padding: $spacing-12;

    .error-state__icon-bg {
      width: 80px;
      height: 80px;
    }

    .error-state__icon {
      font-size: $font-size-3xl;
    }

    .error-state__title {
      font-size: $font-size-xl;
    }
  }

  // RTL support
  &--rtl {
    direction: rtl;
    text-align: right;

    .error-state__retry,
    .error-state__details-toggle {
      flex-direction: row-reverse;
    }
  }

  // Variant: Inline
  &--inline {
    flex-direction: row;
    padding: $spacing-4;
    background: rgba($error-500, 0.05);
    border: 1px solid rgba($error-500, 0.2);
    border-radius: $radius-lg;
    text-align: start;
    gap: $spacing-4;

    .error-state__icon-container {
      margin-bottom: 0;
    }

    .error-state__icon-bg {
      width: 40px;
      height: 40px;
    }

    .error-state__content {
      flex: 1;
      margin-bottom: 0;
    }

    .error-state__title {
      font-size: $font-size-sm;
      margin-bottom: $spacing-1;
    }

    .error-state__message {
      font-size: $font-size-xs;
    }

    .error-state__actions {
      margin-top: 0;
    }
  }

  // Variant: Banner
  &--banner {
    flex-direction: row;
    padding: $spacing-4 $spacing-6;
    background: $error-50;
    border-bottom: 2px solid $error-500;
    border-radius: 0;
    width: 100%;
    gap: $spacing-4;

    .error-state__icon-container {
      margin-bottom: 0;
    }

    .error-state__icon-bg {
      width: 32px;
      height: 32px;
      background: transparent;
    }

    .error-state__content {
      flex: 1;
      text-align: start;
      margin-bottom: 0;
    }

    .error-state__title {
      font-size: $font-size-sm;
      margin-bottom: 0;
    }

    .error-state__message {
      display: none;
    }

    .error-state__actions {
      margin-top: 0;
    }

    .error-state__retry {
      padding: $spacing-2 $spacing-4;
      font-size: $font-size-xs;
    }
  }

  // Icon container
  &__icon-container {
    margin-bottom: $spacing-4;
  }

  &__icon-bg {
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: $radius-full;
    background: rgba($error-500, 0.1);
  }

  &__icon {
    color: $error-500;
  }

  // Content
  &__content {
    margin-bottom: $spacing-4;
  }

  &__title {
    font-weight: $font-weight-semibold;
    color: $gray-800;
    margin: 0 0 $spacing-2;
    line-height: 1.3;
  }

  &__message {
    color: $gray-600;
    margin: 0;
    line-height: 1.5;
    max-width: 400px;
  }

  // Details
  &__details {
    margin-top: $spacing-4;
    width: 100%;
    max-width: 500px;
  }

  &__details-toggle {
    display: inline-flex;
    align-items: center;
    gap: $spacing-2;
    padding: $spacing-2;
    background: transparent;
    border: none;
    color: $gray-500;
    font-size: $font-size-xs;
    cursor: pointer;

    &:hover {
      color: $gray-700;
    }
  }

  &__details-content {
    margin-top: $spacing-2;
    padding: $spacing-3;
    background: $gray-100;
    border-radius: $radius-md;
    font-size: $font-size-xs;
    font-family: monospace;
    text-align: left;
    white-space: pre-wrap;
    word-break: break-word;
    max-height: 200px;
    overflow: auto;
  }

  // Actions
  &__actions {
    margin-top: $spacing-2;
  }

  &__retry {
    display: inline-flex;
    align-items: center;
    gap: $spacing-2;
    padding: $spacing-2-5 $spacing-5;
    background: $error-500;
    color: white;
    border: none;
    border-radius: $radius-lg;
    font-size: $font-size-sm;
    font-weight: $font-weight-medium;
    cursor: pointer;
    transition: all $duration-fast $ease-default;

    &:hover:not(:disabled) {
      background: $error-600;
      transform: translateY(-1px);
    }

    &:active:not(:disabled) {
      transform: translateY(0);
    }

    &:disabled {
      opacity: 0.7;
      cursor: not-allowed;
    }

    &:focus-visible {
      outline: 2px solid $error-500;
      outline-offset: 2px;
    }

    i {
      font-size: $font-size-sm;
    }
  }

  // Extra slot
  &__extra {
    margin-top: $spacing-4;
  }

  // ================================
  // Animation States
  // ================================

  &--animated {
    .error-state__icon-container,
    .error-state__content,
    .error-state__actions,
    .error-state__extra {
      opacity: 0;
      transform: translateY(16px);
    }

    &.error-state--visible {
      .error-state__icon-container {
        animation: errorStateFadeIn 0.5s ease-out 0.1s forwards;
      }

      .error-state__content {
        animation: errorStateFadeIn 0.5s ease-out 0.2s forwards;
      }

      .error-state__actions {
        animation: errorStateFadeIn 0.5s ease-out 0.3s forwards;
      }

      .error-state__extra {
        animation: errorStateFadeIn 0.5s ease-out 0.35s forwards;
      }
    }
  }
}

// ================================
// Keyframe Animations
// ================================

@keyframes errorStateFadeIn {
  from {
    opacity: 0;
    transform: translateY(16px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

// Icon shake animation for attention
@keyframes errorShake {
  0%, 100% { transform: translateX(0); }
  10%, 30%, 50%, 70%, 90% { transform: translateX(-2px); }
  20%, 40%, 60%, 80% { transform: translateX(2px); }
}

// ================================
// Reduced Motion
// ================================

@media (prefers-reduced-motion: reduce) {
  .error-state {
    &--animated {
      .error-state__icon-container,
      .error-state__content,
      .error-state__actions,
      .error-state__extra {
        opacity: 1;
        transform: none;
        animation: none !important;
      }
    }

    &__retry:hover {
      transform: none !important;
    }
  }
}
</style>
