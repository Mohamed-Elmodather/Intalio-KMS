<script setup lang="ts">
/**
 * Badge Component
 *
 * A versatile badge/tag component for status indicators, labels, and counts.
 * Supports multiple variants, sizes, and styles.
 *
 * @example
 * <Badge variant="success">Active</Badge>
 *
 * @example
 * <Badge variant="error" dot>3 Errors</Badge>
 *
 * @example
 * <Badge variant="info" outline size="lg">New Feature</Badge>
 */
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

export interface BadgeProps {
  /** Color variant */
  variant?: 'default' | 'success' | 'warning' | 'error' | 'info' | 'brand' | 'neutral'
  /** Size variant */
  size?: 'xs' | 'sm' | 'md' | 'lg'
  /** Pill/rounded style */
  rounded?: boolean
  /** Show status dot */
  dot?: boolean
  /** Outline style */
  outline?: boolean
  /** Soft/subtle background */
  soft?: boolean
  /** Icon class (PrimeIcons) */
  icon?: string
  /** Pulsing animation for attention */
  pulse?: boolean
  /** Removable badge */
  removable?: boolean
}

withDefaults(defineProps<BadgeProps>(), {
  variant: 'default',
  size: 'md',
  rounded: true,
  dot: false,
  outline: false,
  soft: true,
  pulse: false,
  removable: false
})

const emit = defineEmits<{
  remove: []
}>()

const { locale } = useI18n()

const isRTL = computed(() => locale.value === 'ar')

const handleRemove = (event: MouseEvent) => {
  event.stopPropagation()
  emit('remove')
}
</script>

<template>
  <span
    class="badge"
    :class="[
      `badge--${variant}`,
      `badge--${size}`,
      {
        'badge--rounded': rounded,
        'badge--dot': dot,
        'badge--outline': outline,
        'badge--soft': soft && !outline,
        'badge--pulse': pulse,
        'badge--rtl': isRTL
      }
    ]"
  >
    <!-- Status Dot -->
    <span v-if="dot" class="badge__dot" aria-hidden="true" />

    <!-- Icon -->
    <i v-if="icon" :class="['pi', icon, 'badge__icon']" aria-hidden="true" />

    <!-- Content -->
    <span class="badge__content">
      <slot />
    </span>

    <!-- Remove Button -->
    <button
      v-if="removable"
      class="badge__remove"
      type="button"
      aria-label="Remove"
      @click="handleRemove"
    >
      <i class="pi pi-times" aria-hidden="true" />
    </button>
  </span>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.badge {
  display: inline-flex;
  align-items: center;
  gap: $spacing-1-5;
  font-weight: $font-weight-medium;
  line-height: 1;
  white-space: nowrap;
  vertical-align: middle;
  transition: all $duration-fast $ease-default;

  // Sizes
  &--xs {
    padding: $spacing-0-5 $spacing-1-5;
    font-size: 10px;
    gap: $spacing-1;

    .badge__dot {
      width: 4px;
      height: 4px;
    }

    .badge__icon {
      font-size: 10px;
    }

    .badge__remove {
      width: 12px;
      height: 12px;
      font-size: 8px;
    }
  }

  &--sm {
    padding: $spacing-1 $spacing-2;
    font-size: $font-size-xs;

    .badge__dot {
      width: 5px;
      height: 5px;
    }

    .badge__icon {
      font-size: $font-size-xs;
    }

    .badge__remove {
      width: 14px;
      height: 14px;
      font-size: 10px;
    }
  }

  &--md {
    padding: $spacing-1 $spacing-2-5;
    font-size: $font-size-xs;

    .badge__dot {
      width: 6px;
      height: 6px;
    }

    .badge__icon {
      font-size: $font-size-sm;
    }

    .badge__remove {
      width: 16px;
      height: 16px;
      font-size: 10px;
    }
  }

  &--lg {
    padding: $spacing-1-5 $spacing-3;
    font-size: $font-size-sm;

    .badge__dot {
      width: 8px;
      height: 8px;
    }

    .badge__icon {
      font-size: $font-size-base;
    }

    .badge__remove {
      width: 18px;
      height: 18px;
      font-size: 12px;
    }
  }

  // Border radius
  &--rounded {
    border-radius: $radius-full;
  }

  &:not(.badge--rounded) {
    border-radius: $radius-md;
  }

  // RTL
  &--rtl {
    direction: rtl;
    flex-direction: row-reverse;
  }

  // ================================
  // Variant Colors
  // ================================

  // Default (gray)
  &--default {
    --badge-bg: #{$gray-100};
    --badge-color: #{$gray-700};
    --badge-border: #{$gray-200};
    --badge-dot: #{$gray-500};
  }

  // Neutral (darker gray)
  &--neutral {
    --badge-bg: #{$gray-200};
    --badge-color: #{$gray-800};
    --badge-border: #{$gray-300};
    --badge-dot: #{$gray-600};
  }

  // Brand (teal)
  &--brand {
    --badge-bg: #{rgba($intalio-teal-500, 0.1)};
    --badge-color: #{$intalio-teal-700};
    --badge-border: #{$intalio-teal-200};
    --badge-dot: #{$intalio-teal-500};
    --badge-solid-bg: #{$intalio-teal-500};
  }

  // Success (green)
  &--success {
    --badge-bg: #{rgba($success-500, 0.1)};
    --badge-color: #{$success-700};
    --badge-border: #{$success-100};
    --badge-dot: #{$success-500};
    --badge-solid-bg: #{$success-500};
  }

  // Warning (yellow/amber)
  &--warning {
    --badge-bg: #{rgba($warning-500, 0.1)};
    --badge-color: #{$warning-700};
    --badge-border: #{$warning-200};
    --badge-dot: #{$warning-500};
    --badge-solid-bg: #{$warning-500};
  }

  // Error (red)
  &--error {
    --badge-bg: #{rgba($error-500, 0.1)};
    --badge-color: #{$error-700};
    --badge-border: #{$error-100};
    --badge-dot: #{$error-500};
    --badge-solid-bg: #{$error-500};
  }

  // Info (blue)
  &--info {
    --badge-bg: #{rgba($info-500, 0.1)};
    --badge-color: #{$info-700};
    --badge-border: #{$info-100};
    --badge-dot: #{$info-500};
    --badge-solid-bg: #{$info-500};
  }

  // ================================
  // Style Modifiers
  // ================================

  // Soft (default filled style)
  &--soft {
    background: var(--badge-bg);
    color: var(--badge-color);
    border: 1px solid transparent;
  }

  // Outline style
  &--outline {
    background: transparent;
    color: var(--badge-color);
    border: 1px solid var(--badge-border);
  }

  // Pulse animation
  &--pulse {
    .badge__dot {
      animation: badgePulse 2s ease-in-out infinite;
    }
  }

  // ================================
  // Elements
  // ================================

  &__dot {
    flex-shrink: 0;
    border-radius: 50%;
    background: var(--badge-dot);
  }

  &__icon {
    flex-shrink: 0;
    color: var(--badge-color);
  }

  &__content {
    flex: 1;
  }

  &__remove {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    padding: 0;
    margin-inline-start: $spacing-0-5;
    margin-inline-end: -$spacing-1;
    background: transparent;
    border: none;
    border-radius: 50%;
    color: var(--badge-color);
    opacity: 0.6;
    cursor: pointer;
    transition: all $duration-fast $ease-default;

    &:hover {
      opacity: 1;
      background: rgba(0, 0, 0, 0.1);
    }

    &:focus-visible {
      outline: 2px solid var(--badge-color);
      outline-offset: 1px;
    }

    i {
      font-size: inherit;
    }
  }
}

// ================================
// Keyframe Animations
// ================================

@keyframes badgePulse {
  0%, 100% {
    opacity: 1;
    transform: scale(1);
  }
  50% {
    opacity: 0.6;
    transform: scale(1.1);
  }
}

// ================================
// Reduced Motion
// ================================

@media (prefers-reduced-motion: reduce) {
  .badge {
    &--pulse .badge__dot {
      animation: none !important;
    }
  }
}
</style>
