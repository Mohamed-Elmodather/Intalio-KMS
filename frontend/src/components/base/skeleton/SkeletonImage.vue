<script setup lang="ts">
/**
 * SkeletonImage Component
 *
 * Skeleton for images with aspect ratio support.
 *
 * @example
 * <SkeletonImage aspectRatio="16/9" />
 * <SkeletonImage aspectRatio="1/1" />
 */
import SkeletonBase from './SkeletonBase.vue'
import { computed } from 'vue'

export interface SkeletonImageProps {
  /** Aspect ratio (e.g., '16/9', '4/3', '1/1') */
  aspectRatio?: string
  /** Width */
  width?: string
  /** Height (overrides aspect ratio) */
  height?: string
  /** Border radius */
  rounded?: boolean | 'sm' | 'md' | 'lg'
  /** Animation type */
  animation?: 'shimmer' | 'pulse' | 'none'
}

const props = withDefaults(defineProps<SkeletonImageProps>(), {
  aspectRatio: '16/9',
  width: '100%',
  rounded: 'lg',
  animation: 'shimmer'
})

const containerStyle = computed(() => {
  if (props.height) {
    return {
      width: props.width,
      height: props.height
    }
  }
  return {
    width: props.width,
    aspectRatio: props.aspectRatio
  }
})
</script>

<template>
  <div class="skeleton-image" :style="containerStyle">
    <SkeletonBase
      width="100%"
      height="100%"
      :rounded="rounded"
      :animation="animation"
    />
  </div>
</template>

<style scoped lang="scss">
.skeleton-image {
  position: relative;
  overflow: hidden;
}
</style>
