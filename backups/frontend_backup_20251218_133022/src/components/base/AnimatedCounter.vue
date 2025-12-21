<script setup lang="ts">
/**
 * AnimatedCounter Component
 *
 * Displays a number that animates from 0 to the target value.
 * Supports decimal places, separators, and prefix/suffix.
 *
 * @example
 * <AnimatedCounter :value="1234" prefix="$" separator="," />
 * <AnimatedCounter :value="85.5" suffix="%" :decimals="1" />
 */
import { watch, toRef } from 'vue'
import { useCountUp } from '@/composables/useCountUp'
import { useScrollReveal } from '@/composables/useScrollReveal'
import { ref } from 'vue'

export interface AnimatedCounterProps {
  /** Target number value */
  value: number
  /** Animation duration in ms */
  duration?: number
  /** Starting value */
  startValue?: number
  /** Decimal places */
  decimals?: number
  /** Prefix (e.g., '$') */
  prefix?: string
  /** Suffix (e.g., '%') */
  suffix?: string
  /** Thousands separator */
  separator?: string
  /** Decimal separator */
  decimal?: string
  /** Easing function */
  easing?: 'linear' | 'easeOut' | 'easeInOut' | 'spring'
  /** Start animation when visible */
  startOnVisible?: boolean
  /** Custom class for the counter */
  class?: string
}

const props = withDefaults(defineProps<AnimatedCounterProps>(), {
  duration: 1000,
  startValue: 0,
  decimals: 0,
  prefix: '',
  suffix: '',
  separator: ',',
  decimal: '.',
  easing: 'easeOut',
  startOnVisible: true
})

const counterRef = ref<HTMLElement | null>(null)

// Use scroll reveal if startOnVisible
const { isVisible } = useScrollReveal(counterRef, {
  threshold: 0.3,
  once: true,
  disabled: !props.startOnVisible
})

// Use count up animation
const { displayValue, start, reset } = useCountUp(toRef(props, 'value'), {
  duration: props.duration,
  startValue: props.startValue,
  decimals: props.decimals,
  prefix: props.prefix,
  suffix: props.suffix,
  separator: props.separator,
  decimal: props.decimal,
  easing: props.easing,
  autoStart: !props.startOnVisible
})

// Start animation when visible
watch(isVisible, (visible) => {
  if (visible && props.startOnVisible) {
    start()
  }
})

// Re-animate when value changes
watch(
  () => props.value,
  () => {
    if (!props.startOnVisible || isVisible.value) {
      reset()
      start()
    }
  }
)

defineExpose({
  start,
  reset
})
</script>

<template>
  <span
    ref="counterRef"
    class="animated-counter"
    :class="props.class"
  >
    {{ displayValue }}
  </span>
</template>

<style scoped lang="scss">
@use '@/design-system/mixins' as *;

.animated-counter {
  display: inline-block;
  font-variant-numeric: tabular-nums;
  font-feature-settings: 'tnum';
}
</style>
