/**
 * useRipple Composable
 *
 * Creates a Material Design-like ripple effect on an element.
 * Respects reduced motion preferences.
 *
 * @example
 * const buttonRef = ref<HTMLElement | null>(null)
 * const { createRipple } = useRipple(buttonRef, {
 *   color: 'rgba(255, 255, 255, 0.3)',
 *   duration: 600
 * })
 *
 * // In template
 * <button ref="buttonRef" @click="createRipple">Click me</button>
 */
import { ref, onUnmounted, type Ref } from 'vue'
import { useReducedMotion } from './useReducedMotion'

export interface UseRippleOptions {
  /** Ripple color (default: 'rgba(255, 255, 255, 0.3)') */
  color?: string
  /** Animation duration in ms (default: 600) */
  duration?: number
  /** Disable ripple effect */
  disabled?: boolean
}

export interface UseRippleReturn {
  /** Trigger ripple at event position */
  createRipple: (event: MouseEvent | TouchEvent) => void
  /** Clean up all ripples */
  cleanup: () => void
}

export function useRipple(
  elementRef: Ref<HTMLElement | null>,
  options: UseRippleOptions = {}
): UseRippleReturn {
  const { color = 'rgba(255, 255, 255, 0.3)', duration = 600, disabled = false } = options

  const prefersReducedMotion = useReducedMotion()
  const ripples = ref<HTMLSpanElement[]>([])

  const getPosition = (event: MouseEvent | TouchEvent): { x: number; y: number } => {
    if ('touches' in event && event.touches.length > 0) {
      return { x: event.touches[0].clientX, y: event.touches[0].clientY }
    }
    if ('clientX' in event) {
      return { x: event.clientX, y: event.clientY }
    }
    return { x: 0, y: 0 }
  }

  const createRipple = (event: MouseEvent | TouchEvent): void => {
    if (disabled || prefersReducedMotion.value || !elementRef.value) return

    const element = elementRef.value
    const rect = element.getBoundingClientRect()
    const { x, y } = getPosition(event)

    // Calculate ripple size (should cover the entire element)
    const size = Math.max(rect.width, rect.height) * 2

    // Calculate position relative to element
    const left = x - rect.left - size / 2
    const top = y - rect.top - size / 2

    // Create ripple element
    const ripple = document.createElement('span')
    ripple.style.cssText = `
      position: absolute;
      width: ${size}px;
      height: ${size}px;
      left: ${left}px;
      top: ${top}px;
      background: ${color};
      border-radius: 50%;
      transform: scale(0);
      opacity: 1;
      pointer-events: none;
      animation: ripple-effect ${duration}ms ease-out forwards;
    `

    // Ensure parent has proper positioning
    const computedStyle = window.getComputedStyle(element)
    if (computedStyle.position === 'static') {
      element.style.position = 'relative'
    }
    element.style.overflow = 'hidden'

    element.appendChild(ripple)
    ripples.value.push(ripple)

    // Remove ripple after animation
    setTimeout(() => {
      ripple.remove()
      ripples.value = ripples.value.filter((r) => r !== ripple)
    }, duration)
  }

  const cleanup = (): void => {
    ripples.value.forEach((ripple) => ripple.remove())
    ripples.value = []
  }

  // Add keyframes if not already present
  const styleId = 'ripple-effect-keyframes'
  if (!document.getElementById(styleId)) {
    const style = document.createElement('style')
    style.id = styleId
    style.textContent = `
      @keyframes ripple-effect {
        0% {
          transform: scale(0);
          opacity: 1;
        }
        100% {
          transform: scale(1);
          opacity: 0;
        }
      }
    `
    document.head.appendChild(style)
  }

  // Cleanup on unmount
  onUnmounted(cleanup)

  return {
    createRipple,
    cleanup
  }
}

export default useRipple
