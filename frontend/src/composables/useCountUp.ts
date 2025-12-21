/**
 * useCountUp Composable
 *
 * Animates a number from 0 (or start value) to a target value.
 * Respects reduced motion preferences.
 *
 * @example
 * const { displayValue, isAnimating, start, reset } = useCountUp(1234, {
 *   duration: 2000,
 *   decimals: 0,
 *   separator: ','
 * })
 *
 * // In template
 * <span>{{ displayValue }}</span>
 */
import { ref, computed, watch, onMounted, onUnmounted, type Ref } from 'vue'
import { useReducedMotion } from './useReducedMotion'

export interface UseCountUpOptions {
  /** Animation duration in ms (default: 1000) */
  duration?: number
  /** Starting value (default: 0) */
  startValue?: number
  /** Number of decimal places (default: 0) */
  decimals?: number
  /** Thousands separator (default: ',') */
  separator?: string
  /** Decimal separator (default: '.') */
  decimal?: string
  /** Prefix string (e.g., '$') */
  prefix?: string
  /** Suffix string (e.g., '%') */
  suffix?: string
  /** Easing function */
  easing?: 'linear' | 'easeOut' | 'easeInOut' | 'spring'
  /** Auto-start on mount (default: true) */
  autoStart?: boolean
  /** Start when element is visible */
  startOnVisible?: boolean
}

export interface UseCountUpReturn {
  /** Formatted display value */
  displayValue: Ref<string>
  /** Raw numeric value */
  currentValue: Ref<number>
  /** Whether animation is in progress */
  isAnimating: Ref<boolean>
  /** Start or restart animation */
  start: () => void
  /** Reset to start value */
  reset: () => void
  /** Update target value */
  update: (newValue: number) => void
}

// Easing functions
const easingFunctions = {
  linear: (t: number): number => t,
  easeOut: (t: number): number => 1 - Math.pow(1 - t, 3),
  easeInOut: (t: number): number =>
    t < 0.5 ? 4 * t * t * t : 1 - Math.pow(-2 * t + 2, 3) / 2,
  spring: (t: number): number => {
    const c4 = (2 * Math.PI) / 3
    return t === 0
      ? 0
      : t === 1
        ? 1
        : Math.pow(2, -10 * t) * Math.sin((t * 10 - 0.75) * c4) + 1
  }
}

export function useCountUp(
  targetValue: number | Ref<number>,
  options: UseCountUpOptions = {}
): UseCountUpReturn {
  const {
    duration = 1000,
    startValue = 0,
    decimals = 0,
    separator = ',',
    decimal = '.',
    prefix = '',
    suffix = '',
    easing = 'easeOut',
    autoStart = true
  } = options

  const prefersReducedMotion = useReducedMotion()
  const target = ref(typeof targetValue === 'number' ? targetValue : targetValue.value)
  const currentValue = ref(startValue)
  const isAnimating = ref(false)

  let animationFrameId: number | null = null
  let startTime: number | null = null

  // Format number with separators
  const formatNumber = (num: number): string => {
    const fixed = num.toFixed(decimals)
    const parts = fixed.split('.')
    const intPart = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, separator)
    const decPart = parts[1]

    let result = intPart
    if (decimals > 0 && decPart) {
      result += decimal + decPart
    }

    return `${prefix}${result}${suffix}`
  }

  const displayValue = computed(() => formatNumber(currentValue.value))

  const animate = (timestamp: number): void => {
    if (!startTime) {
      startTime = timestamp
    }

    const elapsed = timestamp - startTime
    const progress = Math.min(elapsed / duration, 1)
    const easedProgress = easingFunctions[easing](progress)

    currentValue.value = startValue + (target.value - startValue) * easedProgress

    if (progress < 1) {
      animationFrameId = requestAnimationFrame(animate)
    } else {
      currentValue.value = target.value
      isAnimating.value = false
      animationFrameId = null
      startTime = null
    }
  }

  const start = (): void => {
    // Cancel any existing animation
    if (animationFrameId) {
      cancelAnimationFrame(animationFrameId)
    }

    // If reduced motion, jump to end
    if (prefersReducedMotion.value) {
      currentValue.value = target.value
      return
    }

    isAnimating.value = true
    startTime = null
    animationFrameId = requestAnimationFrame(animate)
  }

  const reset = (): void => {
    if (animationFrameId) {
      cancelAnimationFrame(animationFrameId)
      animationFrameId = null
    }
    currentValue.value = startValue
    isAnimating.value = false
    startTime = null
  }

  const update = (newValue: number): void => {
    target.value = newValue
    start()
  }

  // Watch for target changes if ref
  if (typeof targetValue !== 'number') {
    watch(targetValue, (newVal) => {
      target.value = newVal
      if (autoStart) {
        start()
      }
    })
  }

  // Watch for reduced motion changes
  watch(prefersReducedMotion, (reduced) => {
    if (reduced && isAnimating.value) {
      if (animationFrameId) {
        cancelAnimationFrame(animationFrameId)
      }
      currentValue.value = target.value
      isAnimating.value = false
    }
  })

  onMounted(() => {
    if (autoStart) {
      start()
    }
  })

  onUnmounted(() => {
    if (animationFrameId) {
      cancelAnimationFrame(animationFrameId)
    }
  })

  return {
    displayValue,
    currentValue,
    isAnimating,
    start,
    reset,
    update
  }
}

export default useCountUp
