<script setup lang="ts">
/**
 * AnimatedList Component
 *
 * Renders a list with staggered animation for each item.
 * Items animate in sequence when the list becomes visible.
 *
 * @example
 * <AnimatedList :items="users" animation="fadeUp" :stagger-delay="100">
 *   <template #item="{ item, index }">
 *     <UserCard :user="item" />
 *   </template>
 * </AnimatedList>
 */
import { ref, computed, watch } from 'vue'
import { useScrollReveal } from '@/composables/useScrollReveal'
import { useReducedMotion } from '@/composables/useReducedMotion'

export interface AnimatedListProps {
  /** Array of items to render */
  items: any[]
  /** Stagger delay between items in ms */
  staggerDelay?: number
  /** Animation type */
  animation?: 'fadeUp' | 'fadeIn' | 'slideIn' | 'scaleIn'
  /** Start animation when visible */
  startOnVisible?: boolean
  /** Animation duration in ms */
  duration?: number
  /** HTML tag for the list container */
  tag?: string
  /** HTML tag for list items */
  itemTag?: string
  /** Item key property name */
  itemKey?: string
}

const props = withDefaults(defineProps<AnimatedListProps>(), {
  staggerDelay: 100,
  animation: 'fadeUp',
  startOnVisible: true,
  duration: 400,
  tag: 'div',
  itemTag: 'div',
  itemKey: 'id'
})

const listRef = ref<HTMLElement | null>(null)
const prefersReducedMotion = useReducedMotion()
const hasAnimated = ref(false)

const { isVisible } = useScrollReveal(listRef, {
  threshold: 0.1,
  once: true,
  disabled: !props.startOnVisible
})

// Compute animation classes based on type
const animationClass = computed(() => {
  switch (props.animation) {
    case 'fadeUp':
      return 'animate-fade-up'
    case 'fadeIn':
      return 'animate-fade-in'
    case 'slideIn':
      return 'animate-slide-in'
    case 'scaleIn':
      return 'animate-scale-in'
    default:
      return 'animate-fade-up'
  }
})

// Control animation state
const shouldAnimate = computed(() => {
  if (prefersReducedMotion.value) return false
  if (!props.startOnVisible) return true
  return isVisible.value
})

// Get item key
const getItemKey = (item: any, index: number): string | number => {
  if (typeof item === 'object' && item !== null && props.itemKey in item) {
    return item[props.itemKey]
  }
  return index
}

// Mark as animated after first animation
watch(shouldAnimate, (animate) => {
  if (animate) {
    hasAnimated.value = true
  }
})
</script>

<template>
  <component
    :is="tag"
    ref="listRef"
    class="animated-list"
    :class="{ 'is-visible': shouldAnimate || hasAnimated }"
  >
    <component
      :is="itemTag"
      v-for="(item, index) in items"
      :key="getItemKey(item, index)"
      class="animated-list__item"
      :class="[animationClass, { 'is-visible': shouldAnimate || hasAnimated }]"
      :style="{
        '--item-index': index,
        '--stagger-delay': `${index * staggerDelay}ms`,
        '--animation-duration': `${duration}ms`
      }"
    >
      <slot name="item" :item="item" :index="index" />
    </component>
  </component>
</template>

<style scoped lang="scss">
@use '@/design-system/mixins' as *;

.animated-list {
  &__item {
    opacity: 0;
    transform: translateY(0);
    transition: opacity var(--animation-duration, 400ms) $ease-out-expo,
      transform var(--animation-duration, 400ms) $ease-out-expo;
    transition-delay: var(--stagger-delay, 0ms);

    // Fade Up Animation
    &.animate-fade-up {
      transform: translateY(20px);

      &.is-visible {
        opacity: 1;
        transform: translateY(0);
      }
    }

    // Fade In Animation
    &.animate-fade-in {
      &.is-visible {
        opacity: 1;
      }
    }

    // Slide In Animation
    &.animate-slide-in {
      transform: translateX(-20px);

      &.is-visible {
        opacity: 1;
        transform: translateX(0);
      }
    }

    // Scale In Animation
    &.animate-scale-in {
      transform: scale(0.95);

      &.is-visible {
        opacity: 1;
        transform: scale(1);
      }
    }
  }
}

// Reduced motion support
@media (prefers-reduced-motion: reduce) {
  .animated-list__item {
    opacity: 1 !important;
    transform: none !important;
    transition: none !important;
  }
}
</style>
