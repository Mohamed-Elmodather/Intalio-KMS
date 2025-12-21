<script setup lang="ts">
/**
 * EmptyState Component
 *
 * A consistent empty state display with icon, title, description, and optional action.
 * Used when lists, search results, or data fetches return no items.
 *
 * @example
 * <EmptyState
 *   icon="pi-inbox"
 *   :title="t('content.noItems')"
 *   :description="t('content.noItemsDescription')"
 *   action-label="Create New"
 *   action-icon="pi-plus"
 *   @action="handleCreate"
 * />
 */
import { computed, ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'

export interface EmptyStateProps {
  /** PrimeIcons class for the main icon */
  icon?: string
  /** Main empty state title */
  title: string
  /** Arabic title */
  titleArabic?: string
  /** Secondary description text */
  description?: string
  /** Arabic description */
  descriptionArabic?: string
  /** Action button label */
  actionLabel?: string
  /** Arabic action label */
  actionLabelArabic?: string
  /** Action button icon (PrimeIcons class) */
  actionIcon?: string
  /** Style variant */
  variant?: 'default' | 'search' | 'error' | 'success' | 'minimal'
  /** Size variant */
  size?: 'sm' | 'md' | 'lg'
  /** Enable entrance animation */
  animated?: boolean
}

const props = withDefaults(defineProps<EmptyStateProps>(), {
  icon: 'pi-inbox',
  variant: 'default',
  size: 'md',
  animated: true
})

const emit = defineEmits<{
  action: []
}>()

const prefersReducedMotion = useReducedMotion()
const isVisible = ref(false)
const { locale } = useI18n()

const isRTL = computed(() => locale.value === 'ar')

const displayTitle = computed(() => {
  return isRTL.value && props.titleArabic ? props.titleArabic : props.title
})

const displayDescription = computed(() => {
  if (!props.description) return null
  return isRTL.value && props.descriptionArabic ? props.descriptionArabic : props.description
})

const displayActionLabel = computed(() => {
  if (!props.actionLabel) return null
  return isRTL.value && props.actionLabelArabic ? props.actionLabelArabic : props.actionLabel
})

const variantIcon = computed(() => {
  const iconMap: Record<string, string> = {
    default: props.icon,
    search: 'pi-search',
    error: 'pi-exclamation-triangle',
    success: 'pi-check-circle',
    minimal: props.icon
  }
  return iconMap[props.variant] || props.icon
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

const handleAction = () => {
  emit('action')
}
</script>

<template>
  <div
    class="empty-state"
    :class="[
      `empty-state--${variant}`,
      `empty-state--${size}`,
      {
        'empty-state--rtl': isRTL,
        'empty-state--animated': animated && !prefersReducedMotion,
        'empty-state--visible': isVisible
      }
    ]"
    role="status"
    aria-live="polite"
  >
    <!-- Icon Container -->
    <div class="empty-state__icon-container">
      <div class="empty-state__icon-bg">
        <i :class="['pi', variantIcon, 'empty-state__icon']" aria-hidden="true" />
      </div>
    </div>

    <!-- Text Content -->
    <div class="empty-state__content">
      <h3 class="empty-state__title">{{ displayTitle }}</h3>
      <p v-if="displayDescription" class="empty-state__description">
        {{ displayDescription }}
      </p>
    </div>

    <!-- Action Button -->
    <button
      v-if="displayActionLabel"
      class="empty-state__action"
      @click="handleAction"
    >
      <i v-if="actionIcon" :class="['pi', actionIcon]" aria-hidden="true" />
      <span>{{ displayActionLabel }}</span>
    </button>

    <!-- Custom slot for additional content -->
    <div v-if="$slots.default" class="empty-state__extra">
      <slot />
    </div>
  </div>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  padding: $spacing-8;

  // Size variants
  &--sm {
    padding: $spacing-6;

    .empty-state__icon-bg {
      width: 48px;
      height: 48px;
    }

    .empty-state__icon {
      font-size: $font-size-xl;
    }

    .empty-state__title {
      font-size: $font-size-base;
    }

    .empty-state__description {
      font-size: $font-size-sm;
    }
  }

  &--md {
    .empty-state__icon-bg {
      width: 64px;
      height: 64px;
    }

    .empty-state__icon {
      font-size: $font-size-2xl;
    }

    .empty-state__title {
      font-size: $font-size-lg;
    }

    .empty-state__description {
      font-size: $font-size-base;
    }
  }

  &--lg {
    padding: $spacing-12;

    .empty-state__icon-bg {
      width: 80px;
      height: 80px;
    }

    .empty-state__icon {
      font-size: $font-size-3xl;
    }

    .empty-state__title {
      font-size: $font-size-xl;
    }

    .empty-state__description {
      font-size: $font-size-base;
      max-width: 400px;
    }
  }

  // RTL support
  &--rtl {
    direction: rtl;
    text-align: right;

    .empty-state__action {
      flex-direction: row-reverse;
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
    background: $gray-100;
    transition: all $duration-normal $ease-default;
  }

  &__icon {
    color: $gray-400;
    transition: color $duration-normal $ease-default;
  }

  // Content
  &__content {
    margin-bottom: $spacing-4;
  }

  &__title {
    font-weight: $font-weight-semibold;
    color: $gray-700;
    margin: 0 0 $spacing-2;
    line-height: 1.3;
  }

  &__description {
    color: $gray-500;
    margin: 0;
    line-height: 1.5;
    max-width: 320px;
  }

  // Action button
  &__action {
    display: inline-flex;
    align-items: center;
    gap: $spacing-2;
    padding: $spacing-2-5 $spacing-5;
    background: $intalio-teal-500;
    color: white;
    border: none;
    border-radius: $radius-lg;
    font-size: $font-size-sm;
    font-weight: $font-weight-medium;
    cursor: pointer;
    transition: all $duration-fast $ease-default;

    &:hover {
      background: $intalio-teal-600;
      transform: translateY(-1px);
    }

    &:active {
      transform: translateY(0);
    }

    &:focus-visible {
      outline: 2px solid $intalio-teal-500;
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
  // Variant Styles
  // ================================

  &--default {
    .empty-state__icon-bg {
      background: $gray-100;
    }

    .empty-state__icon {
      color: $gray-400;
    }
  }

  &--search {
    .empty-state__icon-bg {
      background: rgba($intalio-teal-500, 0.1);
    }

    .empty-state__icon {
      color: $intalio-teal-500;
    }
  }

  &--error {
    .empty-state__icon-bg {
      background: rgba($error-500, 0.1);
    }

    .empty-state__icon {
      color: $error-500;
    }

    .empty-state__action {
      background: $error-500;

      &:hover {
        background: $error-600;
      }
    }
  }

  &--success {
    .empty-state__icon-bg {
      background: rgba($success-500, 0.1);
    }

    .empty-state__icon {
      color: $success-500;
    }

    .empty-state__action {
      background: $success-500;

      &:hover {
        background: $success-600;
      }
    }
  }

  &--minimal {
    padding: $spacing-4;

    .empty-state__icon-bg {
      background: transparent;
      width: auto;
      height: auto;
    }

    .empty-state__icon {
      font-size: $font-size-xl;
      color: $gray-300;
    }

    .empty-state__title {
      font-size: $font-size-sm;
      color: $gray-500;
    }

    .empty-state__description {
      font-size: $font-size-xs;
    }
  }

  // ================================
  // Animation States
  // ================================

  &--animated {
    .empty-state__icon-container,
    .empty-state__content,
    .empty-state__action,
    .empty-state__extra {
      opacity: 0;
      transform: translateY(16px);
    }

    &.empty-state--visible {
      .empty-state__icon-container {
        animation: emptyStateFadeIn 0.5s ease-out 0.1s forwards;
      }

      .empty-state__content {
        animation: emptyStateFadeIn 0.5s ease-out 0.2s forwards;
      }

      .empty-state__action {
        animation: emptyStateFadeIn 0.5s ease-out 0.3s forwards;
      }

      .empty-state__extra {
        animation: emptyStateFadeIn 0.5s ease-out 0.35s forwards;
      }
    }
  }
}

// ================================
// Keyframe Animations
// ================================

@keyframes emptyStateFadeIn {
  from {
    opacity: 0;
    transform: translateY(16px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

// ================================
// Reduced Motion
// ================================

@media (prefers-reduced-motion: reduce) {
  .empty-state {
    &--animated {
      .empty-state__icon-container,
      .empty-state__content,
      .empty-state__action,
      .empty-state__extra {
        opacity: 1;
        transform: none;
        animation: none !important;
      }
    }

    &__action:hover {
      transform: none !important;
    }
  }
}
</style>
