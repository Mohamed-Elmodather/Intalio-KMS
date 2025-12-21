<script setup lang="ts">
/**
 * AnimatedReveal Component
 *
 * Wrapper component that reveals its content with animation
 * when it enters the viewport.
 *
 * @example
 * <AnimatedReveal animation="fadeUp" :delay="200">
 *   <MyContent />
 * </AnimatedReveal>
 */
import { ref, computed, watch } from 'vue'
import { useScrollReveal } from '@/composables/useScrollReveal'
import { useReducedMotion } from '@/composables/useReducedMotion'

export interface AnimatedRevealProps {
  /** Animation type */
  animation?: 'fadeUp' | 'fadeIn' | 'slideLeft' | 'slideRight' | 'scaleIn' | 'blur'
  /** Animation duration in ms */
  duration?: number
  /** Delay before animation in ms */
  delay?: number
  /** Intersection threshold (0-1) */
  threshold?: number
  /** Only animate once */
  once?: boolean
  /** HTML tag for wrapper */
  tag?: string
  /** Disable animation */
  disabled?: boolean
}

const props = withDefaults(defineProps<AnimatedRevealProps>(), {
  animation: 'fadeUp',
  duration: 500,
  delay: 0,
  threshold: 0.15,
  once: true,
  tag: 'div',
  disabled: false
})

const emit = defineEmits<{
  visible: []
  hidden: []
}>()

const revealRef = ref<HTMLElement | null>(null)
const prefersReducedMotion = useReducedMotion()

const { isVisible, hasAnimated, reset } = useScrollReveal(revealRef, {
  threshold: props.threshold,
  once: props.once,
  delay: props.delay,
  disabled: props.disabled
})

// Emit events
watch(isVisible, (visible) => {
  if (visible) {
    emit('visible')
  } else {
    emit('hidden')
  }
})

// Compute animation styles
const animationStyles = computed(() => ({
  '--reveal-duration': `${props.duration}ms`,
  '--reveal-delay': `${props.delay}ms`
}))

// Compute animation class based on type
const animationClass = computed(() => {
  if (prefersReducedMotion.value || props.disabled) return ''

  const baseClass = `reveal-${props.animation}`
  return baseClass
})

defineExpose({
  reset,
  isVisible,
  hasAnimated
})
</script>

<template>
  <component
    :is="tag"
    ref="revealRef"
    class="animated-reveal"
    :class="[animationClass, { 'is-visible': isVisible, 'is-disabled': disabled }]"
    :style="animationStyles"
  >
    <slot />
  </component>
</template>

<style scoped lang="scss">
@use '@/design-system/mixins' as *;

.animated-reveal {
  transition: opacity var(--reveal-duration, 500ms) $ease-out-expo,
    transform var(--reveal-duration, 500ms) $ease-out-expo,
    filter var(--reveal-duration, 500ms) $ease-out-expo;
  transition-delay: var(--reveal-delay, 0ms);

  // Fade Up
  &.reveal-fadeUp {
    opacity: 0;
    transform: translateY(30px);

    &.is-visible {
      opacity: 1;
      transform: translateY(0);
    }
  }

  // Fade In
  &.reveal-fadeIn {
    opacity: 0;

    &.is-visible {
      opacity: 1;
    }
  }

  // Slide Left
  &.reveal-slideLeft {
    opacity: 0;
    transform: translateX(-40px);

    &.is-visible {
      opacity: 1;
      transform: translateX(0);
    }
  }

  // Slide Right
  &.reveal-slideRight {
    opacity: 0;
    transform: translateX(40px);

    &.is-visible {
      opacity: 1;
      transform: translateX(0);
    }
  }

  // Scale In
  &.reveal-scaleIn {
    opacity: 0;
    transform: scale(0.9);

    &.is-visible {
      opacity: 1;
      transform: scale(1);
    }
  }

  // Blur
  &.reveal-blur {
    opacity: 0;
    filter: blur(10px);

    &.is-visible {
      opacity: 1;
      filter: blur(0);
    }
  }

  // Disabled state
  &.is-disabled {
    opacity: 1 !important;
    transform: none !important;
    filter: none !important;
  }
}

// RTL support
[dir="rtl"] {
  .animated-reveal {
    &.reveal-slideLeft {
      transform: translateX(40px);

      &.is-visible {
        transform: translateX(0);
      }
    }

    &.reveal-slideRight {
      transform: translateX(-40px);

      &.is-visible {
        transform: translateX(0);
      }
    }
  }
}

// Reduced motion support
@media (prefers-reduced-motion: reduce) {
  .animated-reveal {
    opacity: 1 !important;
    transform: none !important;
    filter: none !important;
    transition: none !important;
  }
}
</style>
