<script setup lang="ts">
/**
 * SkeletonBase Component
 *
 * Base skeleton loading component with shimmer animation.
 * Used as foundation for other skeleton variants.
 *
 * @example
 * <SkeletonBase width="200px" height="20px" />
 * <SkeletonBase :rounded="true" />
 */

export interface SkeletonBaseProps {
  /** Width (CSS value) */
  width?: string
  /** Height (CSS value) */
  height?: string
  /** Border radius */
  rounded?: boolean | 'sm' | 'md' | 'lg' | 'full'
  /** Custom border radius value */
  radius?: string
  /** Animation type */
  animation?: 'shimmer' | 'pulse' | 'none'
  /** Custom class */
  class?: string
}

const props = withDefaults(defineProps<SkeletonBaseProps>(), {
  width: '100%',
  height: '1rem',
  rounded: 'md',
  animation: 'shimmer'
})

const radiusClass = computed(() => {
  if (props.radius) return ''
  if (props.rounded === true || props.rounded === 'md') return 'skeleton--radius-md'
  if (props.rounded === 'sm') return 'skeleton--radius-sm'
  if (props.rounded === 'lg') return 'skeleton--radius-lg'
  if (props.rounded === 'full') return 'skeleton--radius-full'
  return ''
})

const animationClass = computed(() => {
  if (props.animation === 'shimmer') return 'skeleton--shimmer'
  if (props.animation === 'pulse') return 'skeleton--pulse'
  return ''
})

import { computed } from 'vue'
</script>

<template>
  <div
    class="skeleton"
    :class="[radiusClass, animationClass, props.class]"
    :style="{
      width: props.width,
      height: props.height,
      borderRadius: props.radius
    }"
  />
</template>

<style scoped lang="scss">
@use '@/design-system/mixins' as *;

.skeleton {
  background: $slate-200;
  position: relative;
  overflow: hidden;

  // Radius variants
  &--radius-sm {
    border-radius: $radius-sm;
  }

  &--radius-md {
    border-radius: $radius-md;
  }

  &--radius-lg {
    border-radius: $radius-lg;
  }

  &--radius-full {
    border-radius: $radius-full;
  }

  // Shimmer animation
  &--shimmer {
    background: linear-gradient(
      90deg,
      $slate-200 0%,
      $slate-100 40%,
      $slate-200 80%
    );
    background-size: 200% 100%;
    animation: skeletonWave 1.5s ease-in-out infinite;
  }

  // Pulse animation
  &--pulse {
    animation: pulseSoft 1.5s ease-in-out infinite;
  }
}

// Reduced motion
@media (prefers-reduced-motion: reduce) {
  .skeleton {
    animation: none !important;
    background: $slate-200;
  }
}
</style>
