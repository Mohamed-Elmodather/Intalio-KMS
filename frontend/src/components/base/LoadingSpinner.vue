<script setup lang="ts">
/**
 * LoadingSpinner Component
 *
 * A versatile loading indicator with multiple variants and sizes.
 * Includes accessibility support with screen reader announcements.
 *
 * @example
 * <LoadingSpinner size="lg" variant="spinner" label="Loading data..." />
 *
 * @example
 * <LoadingSpinner overlay>
 *   <template #default>Loading your content...</template>
 * </LoadingSpinner>
 */
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'

export interface LoadingSpinnerProps {
  /** Size of the spinner */
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl'
  /** Animation variant */
  variant?: 'spinner' | 'dots' | 'pulse' | 'bars'
  /** Color variant */
  color?: 'brand' | 'white' | 'gray' | 'current'
  /** Screen reader label */
  label?: string
  /** Arabic label */
  labelArabic?: string
  /** Show label text visually */
  showLabel?: boolean
  /** Display as full-screen overlay */
  overlay?: boolean
  /** Overlay is semi-transparent */
  overlayTransparent?: boolean
  /** Center in parent container */
  centered?: boolean
}

const props = withDefaults(defineProps<LoadingSpinnerProps>(), {
  size: 'md',
  variant: 'spinner',
  color: 'brand',
  showLabel: false,
  overlay: false,
  overlayTransparent: true,
  centered: false
})

const prefersReducedMotion = useReducedMotion()
const { locale } = useI18n()

const isRTL = computed(() => locale.value === 'ar')

const displayLabel = computed(() => {
  if (props.label) {
    return isRTL.value && props.labelArabic ? props.labelArabic : props.label
  }
  return isRTL.value ? 'جارٍ التحميل...' : 'Loading...'
})

// For dots variant
const dotCount = 3

// For bars variant
const barCount = 4
</script>

<template>
  <div
    class="loading-spinner"
    :class="[
      `loading-spinner--${size}`,
      `loading-spinner--${variant}`,
      `loading-spinner--${color}`,
      {
        'loading-spinner--overlay': overlay,
        'loading-spinner--overlay-transparent': overlay && overlayTransparent,
        'loading-spinner--centered': centered,
        'loading-spinner--reduced-motion': prefersReducedMotion
      }
    ]"
    role="status"
    :aria-label="displayLabel"
  >
    <div class="loading-spinner__container">
      <!-- Spinner variant -->
      <div v-if="variant === 'spinner'" class="loading-spinner__spinner">
        <svg viewBox="0 0 50 50" class="loading-spinner__svg">
          <circle
            class="loading-spinner__track"
            cx="25"
            cy="25"
            r="20"
            fill="none"
            stroke-width="4"
          />
          <circle
            class="loading-spinner__circle"
            cx="25"
            cy="25"
            r="20"
            fill="none"
            stroke-width="4"
            stroke-linecap="round"
          />
        </svg>
      </div>

      <!-- Dots variant -->
      <div v-else-if="variant === 'dots'" class="loading-spinner__dots">
        <span
          v-for="i in dotCount"
          :key="i"
          class="loading-spinner__dot"
          :style="{ '--dot-index': i - 1 }"
        />
      </div>

      <!-- Pulse variant -->
      <div v-else-if="variant === 'pulse'" class="loading-spinner__pulse">
        <span class="loading-spinner__pulse-ring loading-spinner__pulse-ring--1" />
        <span class="loading-spinner__pulse-ring loading-spinner__pulse-ring--2" />
        <span class="loading-spinner__pulse-core" />
      </div>

      <!-- Bars variant -->
      <div v-else-if="variant === 'bars'" class="loading-spinner__bars">
        <span
          v-for="i in barCount"
          :key="i"
          class="loading-spinner__bar"
          :style="{ '--bar-index': i - 1 }"
        />
      </div>

      <!-- Label -->
      <span
        :class="['loading-spinner__label', { 'sr-only': !showLabel }]"
      >
        <slot>{{ displayLabel }}</slot>
      </span>
    </div>
  </div>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.loading-spinner {
  display: inline-flex;
  align-items: center;
  justify-content: center;

  // Sizes
  &--xs {
    --spinner-size: 16px;
    --dot-size: 4px;
    --bar-width: 2px;
    --bar-height: 12px;
    --font-size: #{$font-size-xs};
  }

  &--sm {
    --spinner-size: 24px;
    --dot-size: 6px;
    --bar-width: 3px;
    --bar-height: 16px;
    --font-size: #{$font-size-sm};
  }

  &--md {
    --spinner-size: 32px;
    --dot-size: 8px;
    --bar-width: 4px;
    --bar-height: 20px;
    --font-size: #{$font-size-base};
  }

  &--lg {
    --spinner-size: 48px;
    --dot-size: 10px;
    --bar-width: 5px;
    --bar-height: 28px;
    --font-size: #{$font-size-lg};
  }

  &--xl {
    --spinner-size: 64px;
    --dot-size: 12px;
    --bar-width: 6px;
    --bar-height: 36px;
    --font-size: #{$font-size-xl};
  }

  // Colors
  &--brand {
    --spinner-color: #{$intalio-teal-500};
    --spinner-track: #{$gray-200};
  }

  &--white {
    --spinner-color: white;
    --spinner-track: rgba(white, 0.3);
  }

  &--gray {
    --spinner-color: #{$gray-500};
    --spinner-track: #{$gray-200};
  }

  &--current {
    --spinner-color: currentColor;
    --spinner-track: currentColor;
  }

  // Layout modifiers
  &--centered {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
  }

  &--overlay {
    position: fixed;
    inset: 0;
    z-index: $z-modal;
    background: white;

    &.loading-spinner--overlay-transparent {
      background: rgba(white, 0.9);
      backdrop-filter: blur(4px);
    }
  }

  // Container
  &__container {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: $spacing-3;
  }

  // ================================
  // Spinner Variant
  // ================================

  &__spinner {
    width: var(--spinner-size);
    height: var(--spinner-size);
  }

  &__svg {
    width: 100%;
    height: 100%;
    animation: spinnerRotate 1.4s linear infinite;
  }

  &__track {
    stroke: var(--spinner-track);
    opacity: 0.3;
  }

  &__circle {
    stroke: var(--spinner-color);
    stroke-dasharray: 80, 200;
    stroke-dashoffset: 0;
    animation: spinnerDash 1.4s ease-in-out infinite;
  }

  // ================================
  // Dots Variant
  // ================================

  &__dots {
    display: flex;
    gap: calc(var(--dot-size) * 0.75);
  }

  &__dot {
    width: var(--dot-size);
    height: var(--dot-size);
    border-radius: 50%;
    background: var(--spinner-color);
    animation: dotBounce 1.4s ease-in-out infinite;
    animation-delay: calc(var(--dot-index, 0) * 0.16s);
  }

  // ================================
  // Pulse Variant
  // ================================

  &__pulse {
    position: relative;
    width: var(--spinner-size);
    height: var(--spinner-size);
  }

  &__pulse-ring {
    position: absolute;
    inset: 0;
    border: 2px solid var(--spinner-color);
    border-radius: 50%;
    opacity: 0;

    &--1 {
      animation: pulseRing 1.5s ease-out infinite;
    }

    &--2 {
      animation: pulseRing 1.5s ease-out 0.5s infinite;
    }
  }

  &__pulse-core {
    position: absolute;
    top: 50%;
    left: 50%;
    width: calc(var(--spinner-size) * 0.3);
    height: calc(var(--spinner-size) * 0.3);
    margin: calc(var(--spinner-size) * -0.15);
    background: var(--spinner-color);
    border-radius: 50%;
    animation: pulseCore 1.5s ease-in-out infinite;
  }

  // ================================
  // Bars Variant
  // ================================

  &__bars {
    display: flex;
    align-items: center;
    gap: calc(var(--bar-width) * 1.5);
    height: var(--bar-height);
  }

  &__bar {
    width: var(--bar-width);
    height: 100%;
    background: var(--spinner-color);
    border-radius: var(--bar-width);
    animation: barScale 1s ease-in-out infinite;
    animation-delay: calc(var(--bar-index, 0) * 0.1s);
  }

  // ================================
  // Label
  // ================================

  &__label {
    font-size: var(--font-size);
    color: $gray-600;
    font-weight: $font-weight-medium;
  }

  // Screen reader only
  .sr-only {
    position: absolute;
    width: 1px;
    height: 1px;
    padding: 0;
    margin: -1px;
    overflow: hidden;
    clip: rect(0, 0, 0, 0);
    white-space: nowrap;
    border: 0;
  }

  // ================================
  // Reduced Motion
  // ================================

  &--reduced-motion {
    .loading-spinner__svg {
      animation: none;
    }

    .loading-spinner__circle {
      animation: none;
      stroke-dasharray: 60, 200;
    }

    .loading-spinner__dot {
      animation: none;
      opacity: 0.6;

      &:nth-child(2) {
        opacity: 0.8;
      }

      &:nth-child(3) {
        opacity: 1;
      }
    }

    .loading-spinner__pulse-ring {
      animation: none;
      opacity: 0.3;

      &--2 {
        transform: scale(1.3);
        opacity: 0.15;
      }
    }

    .loading-spinner__pulse-core {
      animation: none;
    }

    .loading-spinner__bar {
      animation: none;
      height: calc(100% * (0.4 + var(--bar-index, 0) * 0.2));
    }
  }
}

// ================================
// Keyframe Animations
// ================================

@keyframes spinnerRotate {
  100% {
    transform: rotate(360deg);
  }
}

@keyframes spinnerDash {
  0% {
    stroke-dasharray: 1, 200;
    stroke-dashoffset: 0;
  }
  50% {
    stroke-dasharray: 89, 200;
    stroke-dashoffset: -35px;
  }
  100% {
    stroke-dasharray: 89, 200;
    stroke-dashoffset: -124px;
  }
}

@keyframes dotBounce {
  0%, 80%, 100% {
    transform: scale(0.6);
    opacity: 0.5;
  }
  40% {
    transform: scale(1);
    opacity: 1;
  }
}

@keyframes pulseRing {
  0% {
    transform: scale(0.5);
    opacity: 0;
  }
  50% {
    opacity: 0.5;
  }
  100% {
    transform: scale(1.2);
    opacity: 0;
  }
}

@keyframes pulseCore {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(0.8);
  }
}

@keyframes barScale {
  0%, 40%, 100% {
    transform: scaleY(0.4);
  }
  20% {
    transform: scaleY(1);
  }
}

// ================================
// Prefers Reduced Motion (CSS)
// ================================

@media (prefers-reduced-motion: reduce) {
  .loading-spinner {
    &__svg {
      animation: none !important;
    }

    &__circle {
      animation: none !important;
    }

    &__dot,
    &__pulse-ring,
    &__pulse-core,
    &__bar {
      animation: none !important;
    }
  }
}
</style>
