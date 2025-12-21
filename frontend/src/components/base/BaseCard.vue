<script setup lang="ts">
/**
 * BaseCard Component
 *
 * A flexible card component with multiple variants and interactive states.
 * Uses design system tokens for consistent styling.
 * Enhanced with 3D hover effects and glassmorphism.
 *
 * @example
 * <BaseCard variant="elevated" interactive hover3d>
 *   <template #header>Card Title</template>
 *   Card content here
 *   <template #footer>Footer actions</template>
 * </BaseCard>
 */
import { ref, computed } from 'vue'
import { useReducedMotion } from '@/composables/useReducedMotion'

export interface BaseCardProps {
  /** Card style variant */
  variant?: 'base' | 'elevated' | 'interactive' | 'flat' | 'glass'
  /** Add padding to card body */
  padded?: boolean
  /** Custom padding size */
  padding?: 'none' | 'sm' | 'md' | 'lg' | 'xl'
  /** Show loading skeleton */
  loading?: boolean
  /** Disable hover effects */
  noHover?: boolean
  /** HTML tag to render */
  tag?: string
  /** Enable 3D tilt on hover */
  hover3d?: boolean
  /** 3D tilt intensity (degrees) */
  tiltIntensity?: number
  /** Glassmorphism blur level */
  glassBlur?: 'sm' | 'md' | 'lg'
  /** Show glow on hover */
  glowHover?: boolean
  /** Skeleton variant for loading state */
  skeletonVariant?: 'simple' | 'content' | 'media'
}

const props = withDefaults(defineProps<BaseCardProps>(), {
  variant: 'base',
  padded: true,
  padding: 'md',
  loading: false,
  noHover: false,
  tag: 'div',
  hover3d: false,
  tiltIntensity: 5,
  skeletonVariant: 'simple',
  glowHover: false
})

const emit = defineEmits<{
  click: [event: MouseEvent]
}>()

const cardRef = ref<HTMLElement | null>(null)
const prefersReducedMotion = useReducedMotion()
const tiltX = ref(0)
const tiltY = ref(0)

const handleClick = (event: MouseEvent) => {
  if (props.variant === 'interactive') {
    emit('click', event)
  }
}

// 3D Tilt effect handlers
const handleMouseMove = (event: MouseEvent) => {
  if (!props.hover3d || prefersReducedMotion.value || !cardRef.value) return

  const rect = cardRef.value.getBoundingClientRect()
  const x = event.clientX - rect.left
  const y = event.clientY - rect.top
  const centerX = rect.width / 2
  const centerY = rect.height / 2

  tiltX.value = ((y - centerY) / centerY) * props.tiltIntensity
  tiltY.value = ((centerX - x) / centerX) * props.tiltIntensity
}

const handleMouseLeave = () => {
  tiltX.value = 0
  tiltY.value = 0
}

const tiltStyle = computed(() => {
  if (!props.hover3d || prefersReducedMotion.value) return {}
  return {
    transform: `perspective(1000px) rotateX(${tiltX.value}deg) rotateY(${tiltY.value}deg)`
  }
})

const glassClass = computed(() => {
  if (props.variant !== 'glass' || !props.glassBlur) return ''
  return `base-card--glass-${props.glassBlur}`
})
</script>

<template>
  <component
    :is="tag"
    ref="cardRef"
    class="base-card"
    :class="[
      `base-card--${variant}`,
      `base-card--padding-${padding}`,
      glassClass,
      {
        'base-card--no-hover': noHover,
        'base-card--loading': loading,
        'base-card--hover3d': hover3d,
        'base-card--glow-hover': glowHover
      }
    ]"
    :style="tiltStyle"
    @click="handleClick"
    @mousemove="handleMouseMove"
    @mouseleave="handleMouseLeave"
  >
    <!-- Header Slot -->
    <div v-if="$slots.header" class="base-card__header">
      <slot name="header" />
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="base-card__loading">
      <div class="skeleton skeleton--title" />
      <div class="skeleton skeleton--text" />
      <div class="skeleton skeleton--text skeleton--short" />
    </div>

    <!-- Main Content -->
    <div v-else class="base-card__body" :class="{ 'base-card__body--padded': padded }">
      <slot />
    </div>

    <!-- Footer Slot -->
    <div v-if="$slots.footer && !loading" class="base-card__footer">
      <slot name="footer" />
    </div>
  </component>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.base-card {
  background: white;
  border-radius: $radius-xl;
  border: 1px solid $slate-100;
  box-shadow: $shadow-card;
  transition:
    box-shadow $duration-normal $ease-default,
    transform $duration-normal $ease-default,
    border-color $duration-fast $ease-default;
  overflow: hidden;

  // Variants
  &--base {
    // Default styles applied above
  }

  &--elevated {
    &:not(.base-card--no-hover):hover {
      box-shadow: $shadow-card-hover;
      border-color: $slate-200;
      transform: translateY(-$spacing-0-5);
    }
  }

  &--interactive {
    cursor: pointer;

    &:not(.base-card--no-hover):hover {
      box-shadow: $shadow-card-hover;
      border-color: $slate-200;
      transform: translateY(-$spacing-0-5);
    }

    &:active {
      transform: translateY(0);
      box-shadow: $shadow-card;
    }
  }

  &--flat {
    box-shadow: none;
    border-color: $slate-200;

    &:not(.base-card--no-hover):hover {
      background: $slate-50;
    }
  }

  &--glass {
    background: rgba(white, 0.85);
    backdrop-filter: blur(12px);
    -webkit-backdrop-filter: blur(12px);
    border: 1px solid rgba(white, 0.3);
  }

  // Glass blur variants
  &--glass-sm {
    backdrop-filter: blur(8px);
    -webkit-backdrop-filter: blur(8px);
  }

  &--glass-md {
    backdrop-filter: blur(12px);
    -webkit-backdrop-filter: blur(12px);
  }

  &--glass-lg {
    backdrop-filter: blur(20px);
    -webkit-backdrop-filter: blur(20px);
  }

  // 3D hover effect
  &--hover3d {
    transform-style: preserve-3d;
    transition:
      transform $duration-normal $ease-default,
      box-shadow $duration-normal $ease-default;
    will-change: transform;

    &:hover {
      box-shadow: $shadow-card-hover;
    }
  }

  // Glow on hover
  &--glow-hover {
    &:hover {
      box-shadow:
        $shadow-card-hover,
        0 0 20px rgba($brand-500, 0.15),
        0 0 40px rgba($brand-500, 0.1);
    }
  }

  // Padding variants
  &--padding-none .base-card__body--padded {
    padding: 0;
  }

  &--padding-sm .base-card__body--padded {
    padding: $spacing-3;
  }

  &--padding-md .base-card__body--padded {
    padding: $spacing-5;
  }

  &--padding-lg .base-card__body--padded {
    padding: $spacing-6;
  }

  &--padding-xl .base-card__body--padded {
    padding: $spacing-8;
  }

  // Header
  &__header {
    padding: $spacing-5;
    border-bottom: 1px solid $slate-100;
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }

  // Body
  &__body {
    &--padded {
      padding: $spacing-5;
    }
  }

  // Footer
  &__footer {
    padding: $spacing-4 $spacing-5;
    border-top: 1px solid $slate-100;
    background: $slate-50;
  }

  // Loading state
  &--loading {
    pointer-events: none;
  }

  &__loading {
    padding: $spacing-5;
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }
}

// Skeleton loading
.skeleton {
  background: linear-gradient(
    90deg,
    $slate-100 0%,
    $slate-200 50%,
    $slate-100 100%
  );
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  border-radius: $radius-md;

  &--title {
    height: $spacing-6;
    width: 60%;
  }

  &--text {
    height: $spacing-4;
    width: 100%;
  }

  &--short {
    width: 40%;
  }
}

@keyframes shimmer {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}

// Reduced motion support
@media (prefers-reduced-motion: reduce) {
  .base-card {
    transition: none !important;

    &--hover3d {
      transform: none !important;
    }

    &--elevated,
    &--interactive {
      &:hover {
        transform: none !important;
      }
    }
  }

  .skeleton {
    animation: none !important;
  }
}
</style>
