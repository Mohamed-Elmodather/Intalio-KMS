/**
 * useScrollReveal Composable
 *
 * Reveals elements when they enter the viewport using Intersection Observer.
 * Respects reduced motion preferences.
 *
 * @example
 * const elementRef = ref<HTMLElement | null>(null)
 * const { isVisible, hasAnimated } = useScrollReveal(elementRef, {
 *   threshold: 0.2,
 *   once: true
 * })
 *
 * // In template
 * <div ref="elementRef" :class="{ 'is-visible': isVisible }">
 */
import { ref, watch, onMounted, onUnmounted, type Ref } from 'vue'
import { useReducedMotion } from './useReducedMotion'

export interface UseScrollRevealOptions {
  /** Intersection threshold (0-1, default: 0.1) */
  threshold?: number
  /** Root margin (default: '0px') */
  rootMargin?: string
  /** Only animate once (default: true) */
  once?: boolean
  /** Delay before animation in ms (default: 0) */
  delay?: number
  /** Disable the reveal effect */
  disabled?: boolean
}

export interface UseScrollRevealReturn {
  /** Whether element is currently visible */
  isVisible: Ref<boolean>
  /** Whether element has been animated (for once mode) */
  hasAnimated: Ref<boolean>
  /** Reset the animation state */
  reset: () => void
}

export function useScrollReveal(
  elementRef: Ref<HTMLElement | null>,
  options: UseScrollRevealOptions = {}
): UseScrollRevealReturn {
  const {
    threshold = 0.1,
    rootMargin = '0px',
    once = true,
    delay = 0,
    disabled = false
  } = options

  const prefersReducedMotion = useReducedMotion()
  const isVisible = ref(false)
  const hasAnimated = ref(false)

  let observer: IntersectionObserver | null = null
  let timeoutId: ReturnType<typeof setTimeout> | null = null

  const handleIntersection = (entries: IntersectionObserverEntry[]): void => {
    entries.forEach((entry) => {
      if (entry.isIntersecting) {
        // Clear any pending timeout
        if (timeoutId) {
          clearTimeout(timeoutId)
        }

        // Apply delay if specified
        if (delay > 0) {
          timeoutId = setTimeout(() => {
            isVisible.value = true
            hasAnimated.value = true
          }, delay)
        } else {
          isVisible.value = true
          hasAnimated.value = true
        }

        // Unobserve if once mode
        if (once && observer && elementRef.value) {
          observer.unobserve(elementRef.value)
        }
      } else if (!once) {
        // Clear timeout if element leaves viewport before delay completes
        if (timeoutId) {
          clearTimeout(timeoutId)
        }
        isVisible.value = false
      }
    })
  }

  const setupObserver = (): void => {
    if (!elementRef.value || disabled) return

    // If reduced motion, show immediately
    if (prefersReducedMotion.value) {
      isVisible.value = true
      hasAnimated.value = true
      return
    }

    observer = new IntersectionObserver(handleIntersection, {
      threshold,
      rootMargin
    })

    observer.observe(elementRef.value)
  }

  const cleanup = (): void => {
    if (observer) {
      observer.disconnect()
      observer = null
    }
    if (timeoutId) {
      clearTimeout(timeoutId)
      timeoutId = null
    }
  }

  const reset = (): void => {
    cleanup()
    isVisible.value = false
    hasAnimated.value = false

    // Re-setup observer
    if (elementRef.value) {
      setupObserver()
    }
  }

  // Watch for element ref changes
  watch(
    elementRef,
    (newEl, oldEl) => {
      if (oldEl && observer) {
        observer.unobserve(oldEl)
      }
      if (newEl) {
        setupObserver()
      }
    },
    { immediate: false }
  )

  // Watch for reduced motion changes
  watch(prefersReducedMotion, (reduced) => {
    if (reduced) {
      isVisible.value = true
      hasAnimated.value = true
      cleanup()
    }
  })

  onMounted(() => {
    if (elementRef.value) {
      setupObserver()
    }
  })

  onUnmounted(cleanup)

  return {
    isVisible,
    hasAnimated,
    reset
  }
}

export default useScrollReveal
