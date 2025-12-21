/**
 * useReducedMotion Composable
 *
 * Detects and reacts to the user's reduced motion preference.
 * Returns a reactive ref that updates when the system preference changes.
 *
 * @example
 * const prefersReducedMotion = useReducedMotion()
 *
 * // Use in template
 * <div :class="{ 'no-animation': prefersReducedMotion }">
 *
 * // Use in script
 * const duration = computed(() => prefersReducedMotion.value ? 0 : 300)
 */
import { ref, onMounted, onUnmounted, readonly, type Ref } from 'vue'

export function useReducedMotion(): Readonly<Ref<boolean>> {
  const prefersReducedMotion = ref(false)
  let mediaQuery: MediaQueryList | null = null

  const updateMotionPreference = (event: MediaQueryListEvent | MediaQueryList) => {
    prefersReducedMotion.value = event.matches
  }

  onMounted(() => {
    if (typeof window === 'undefined') return

    mediaQuery = window.matchMedia('(prefers-reduced-motion: reduce)')
    prefersReducedMotion.value = mediaQuery.matches

    // Modern browsers
    if (mediaQuery.addEventListener) {
      mediaQuery.addEventListener('change', updateMotionPreference)
    } else {
      // Legacy support
      mediaQuery.addListener(updateMotionPreference)
    }
  })

  onUnmounted(() => {
    if (!mediaQuery) return

    if (mediaQuery.removeEventListener) {
      mediaQuery.removeEventListener('change', updateMotionPreference)
    } else {
      mediaQuery.removeListener(updateMotionPreference)
    }
  })

  return readonly(prefersReducedMotion)
}

export default useReducedMotion
