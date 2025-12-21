<script setup lang="ts">
/**
 * SkeletonAvatar Component
 *
 * Skeleton for avatar/profile images.
 *
 * @example
 * <SkeletonAvatar size="md" />
 * <SkeletonAvatar size="lg" shape="square" />
 */
import SkeletonBase from './SkeletonBase.vue'
import { computed } from 'vue'

export interface SkeletonAvatarProps {
  /** Avatar size */
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | string
  /** Avatar shape */
  shape?: 'circle' | 'square'
  /** Animation type */
  animation?: 'shimmer' | 'pulse' | 'none'
}

const props = withDefaults(defineProps<SkeletonAvatarProps>(), {
  size: 'md',
  shape: 'circle',
  animation: 'shimmer'
})

const sizeMap: Record<string, string> = {
  xs: '1.5rem',
  sm: '2rem',
  md: '2.5rem',
  lg: '3rem',
  xl: '4rem'
}

const computedSize = computed(() => {
  return sizeMap[props.size] || props.size
})

const rounded = computed(() => {
  return props.shape === 'circle' ? 'full' : 'md'
})
</script>

<template>
  <SkeletonBase
    :width="computedSize"
    :height="computedSize"
    :rounded="rounded"
    :animation="animation"
  />
</template>
