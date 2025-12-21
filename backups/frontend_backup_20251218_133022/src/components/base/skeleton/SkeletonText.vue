<script setup lang="ts">
/**
 * SkeletonText Component
 *
 * Skeleton for text content with configurable line count.
 *
 * @example
 * <SkeletonText :lines="3" />
 * <SkeletonText :lines="2" :last-width="60" />
 */
import SkeletonBase from './SkeletonBase.vue'

export interface SkeletonTextProps {
  /** Number of lines */
  lines?: number
  /** Height of each line */
  lineHeight?: string
  /** Gap between lines */
  gap?: string
  /** Width of last line (percentage) */
  lastWidth?: number
  /** Animation type */
  animation?: 'shimmer' | 'pulse' | 'none'
}

const props = withDefaults(defineProps<SkeletonTextProps>(), {
  lines: 3,
  lineHeight: '0.875rem',
  gap: '0.75rem',
  lastWidth: 70,
  animation: 'shimmer'
})
</script>

<template>
  <div class="skeleton-text" :style="{ gap: props.gap }">
    <SkeletonBase
      v-for="i in lines"
      :key="i"
      :height="lineHeight"
      :width="i === lines && lines > 1 ? `${lastWidth}%` : '100%'"
      :animation="animation"
      rounded="sm"
    />
  </div>
</template>

<style scoped lang="scss">
.skeleton-text {
  display: flex;
  flex-direction: column;
}
</style>
