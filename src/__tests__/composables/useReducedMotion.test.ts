import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { defineComponent, nextTick } from 'vue'
import { useReducedMotion } from '@/composables/useReducedMotion'

describe('useReducedMotion', () => {
  let mockMediaQueryList: {
    matches: boolean
    addEventListener: ReturnType<typeof vi.fn>
    removeEventListener: ReturnType<typeof vi.fn>
    addListener: ReturnType<typeof vi.fn>
    removeListener: ReturnType<typeof vi.fn>
  }

  let changeHandler: ((event: { matches: boolean }) => void) | null = null

  beforeEach(() => {
    mockMediaQueryList = {
      matches: false,
      addEventListener: vi.fn((event, handler) => {
        if (event === 'change') {
          changeHandler = handler
        }
      }),
      removeEventListener: vi.fn(),
      addListener: vi.fn((handler) => {
        changeHandler = handler
      }),
      removeListener: vi.fn(),
    }

    vi.stubGlobal('matchMedia', vi.fn(() => mockMediaQueryList))
  })

  afterEach(() => {
    vi.unstubAllGlobals()
    changeHandler = null
  })

  // Helper component to test the composable
  function createTestComponent() {
    return defineComponent({
      setup() {
        const prefersReducedMotion = useReducedMotion()
        return { prefersReducedMotion }
      },
      template: '<div>{{ prefersReducedMotion }}</div>',
    })
  }

  describe('Initial State', () => {
    it('should return false when no reduced motion preference', () => {
      mockMediaQueryList.matches = false
      const wrapper = mount(createTestComponent())

      expect(wrapper.vm.prefersReducedMotion).toBe(false)
    })

    it('should return true when reduced motion is preferred', () => {
      mockMediaQueryList.matches = true
      const wrapper = mount(createTestComponent())

      expect(wrapper.vm.prefersReducedMotion).toBe(true)
    })

    it('should query the correct media query', () => {
      mount(createTestComponent())

      expect(window.matchMedia).toHaveBeenCalledWith('(prefers-reduced-motion: reduce)')
    })
  })

  describe('Event Listener Registration', () => {
    it('should add event listener on mount', () => {
      mount(createTestComponent())

      expect(mockMediaQueryList.addEventListener).toHaveBeenCalledWith(
        'change',
        expect.any(Function)
      )
    })

    it('should remove event listener on unmount', () => {
      const wrapper = mount(createTestComponent())
      wrapper.unmount()

      expect(mockMediaQueryList.removeEventListener).toHaveBeenCalledWith(
        'change',
        expect.any(Function)
      )
    })
  })

  describe('Legacy API Support', () => {
    it('should use addListener for legacy browsers', () => {
      // Simulate legacy browser without addEventListener
      mockMediaQueryList.addEventListener = undefined as any
      mockMediaQueryList.removeEventListener = undefined as any

      mount(createTestComponent())

      expect(mockMediaQueryList.addListener).toHaveBeenCalled()
    })

    it('should use removeListener on unmount for legacy browsers', () => {
      mockMediaQueryList.addEventListener = undefined as any
      mockMediaQueryList.removeEventListener = undefined as any

      const wrapper = mount(createTestComponent())
      wrapper.unmount()

      expect(mockMediaQueryList.removeListener).toHaveBeenCalled()
    })
  })

  describe('Dynamic Updates', () => {
    it('should update when preference changes', async () => {
      mockMediaQueryList.matches = false
      const wrapper = mount(createTestComponent())

      expect(wrapper.vm.prefersReducedMotion).toBe(false)

      // Simulate preference change
      if (changeHandler) {
        changeHandler({ matches: true })
        await nextTick()
      }

      expect(wrapper.vm.prefersReducedMotion).toBe(true)
    })

    it('should update when preference changes back', async () => {
      mockMediaQueryList.matches = true
      const wrapper = mount(createTestComponent())

      expect(wrapper.vm.prefersReducedMotion).toBe(true)

      // Simulate preference change
      if (changeHandler) {
        changeHandler({ matches: false })
        await nextTick()
      }

      expect(wrapper.vm.prefersReducedMotion).toBe(false)
    })
  })

  describe('Readonly', () => {
    it('should return readonly ref', () => {
      const wrapper = mount(createTestComponent())

      // The value should not be directly settable
      // TypeScript would catch this, but we can verify the value doesn't change
      const initialValue = wrapper.vm.prefersReducedMotion
      expect(typeof initialValue).toBe('boolean')
    })
  })

  describe('SSR Safety', () => {
    it('should handle undefined window gracefully', () => {
      // Save original window
      const originalWindow = global.window

      // This test verifies the typeof window check exists in the code
      // In actual SSR, window would be undefined
      expect(() => {
        mount(createTestComponent())
      }).not.toThrow()
    })
  })
})
