import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AIConfidenceBar from '@/components/ai/AIConfidenceBar.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

function mountComponent(props = {}) {
  return shallowMount(AIConfidenceBar, {
    props: { value: 0.8, ...props },
    global: {
      mocks: {
        $t: (key: string) => key,
      },
    },
  })
}

describe('AIConfidenceBar', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the confidence bar', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.ai-confidence-bar').exists()).toBe(true)
    })

    it('should render progress bar', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.bg-gray-100.rounded-full').exists()).toBe(true)
    })
  })

  describe('Percentage Calculation', () => {
    it('should calculate percentage from value', () => {
      const wrapper = mountComponent({ value: 0.75 })
      const vm = wrapper.vm as any
      expect(vm.percentage).toBe(75)
    })

    it('should round percentage', () => {
      const wrapper = mountComponent({ value: 0.756 })
      const vm = wrapper.vm as any
      expect(vm.percentage).toBe(76)
    })
  })

  describe('Bar Colors', () => {
    it('should show green for high confidence (>=0.8)', () => {
      const wrapper = mountComponent({ value: 0.85 })
      const vm = wrapper.vm as any
      expect(vm.barColorClass).toBe('bg-green-500')
    })

    it('should show teal for good confidence (>=0.6)', () => {
      const wrapper = mountComponent({ value: 0.7 })
      const vm = wrapper.vm as any
      expect(vm.barColorClass).toBe('bg-teal-500')
    })

    it('should show amber for moderate confidence (>=0.4)', () => {
      const wrapper = mountComponent({ value: 0.5 })
      const vm = wrapper.vm as any
      expect(vm.barColorClass).toBe('bg-amber-500')
    })

    it('should show red for low confidence (<0.4)', () => {
      const wrapper = mountComponent({ value: 0.3 })
      const vm = wrapper.vm as any
      expect(vm.barColorClass).toBe('bg-red-500')
    })
  })

  describe('Text Colors', () => {
    it('should use green text for high confidence', () => {
      const wrapper = mountComponent({ value: 0.85 })
      const vm = wrapper.vm as any
      expect(vm.textColorClass).toBe('text-green-600')
    })

    it('should use teal text for good confidence', () => {
      const wrapper = mountComponent({ value: 0.7 })
      const vm = wrapper.vm as any
      expect(vm.textColorClass).toBe('text-teal-600')
    })

    it('should use amber text for moderate confidence', () => {
      const wrapper = mountComponent({ value: 0.5 })
      const vm = wrapper.vm as any
      expect(vm.textColorClass).toBe('text-amber-600')
    })

    it('should use red text for low confidence', () => {
      const wrapper = mountComponent({ value: 0.3 })
      const vm = wrapper.vm as any
      expect(vm.textColorClass).toBe('text-red-600')
    })
  })

  describe('Sizes', () => {
    it('should apply small size classes', () => {
      const wrapper = mountComponent({ size: 'sm' })
      const vm = wrapper.vm as any
      expect(vm.sizeClasses.bar).toBe('h-1')
      expect(vm.sizeClasses.text).toBe('text-xs')
    })

    it('should apply medium size classes by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sizeClasses.bar).toBe('h-2')
      expect(vm.sizeClasses.text).toBe('text-sm')
    })

    it('should apply large size classes', () => {
      const wrapper = mountComponent({ size: 'lg' })
      const vm = wrapper.vm as any
      expect(vm.sizeClasses.bar).toBe('h-3')
      expect(vm.sizeClasses.text).toBe('text-base')
    })
  })

  describe('Confidence Labels', () => {
    it('should return very high for >=0.9', () => {
      const wrapper = mountComponent({ value: 0.95 })
      const vm = wrapper.vm as any
      expect(vm.confidenceLabel).toBe('ai.confidence.veryHigh')
    })

    it('should return high for >=0.75', () => {
      const wrapper = mountComponent({ value: 0.8 })
      const vm = wrapper.vm as any
      expect(vm.confidenceLabel).toBe('ai.confidence.high')
    })

    it('should return moderate for >=0.6', () => {
      const wrapper = mountComponent({ value: 0.65 })
      const vm = wrapper.vm as any
      expect(vm.confidenceLabel).toBe('ai.confidence.moderate')
    })

    it('should return low for >=0.4', () => {
      const wrapper = mountComponent({ value: 0.5 })
      const vm = wrapper.vm as any
      expect(vm.confidenceLabel).toBe('ai.confidence.low')
    })

    it('should return very low for <0.4', () => {
      const wrapper = mountComponent({ value: 0.3 })
      const vm = wrapper.vm as any
      expect(vm.confidenceLabel).toBe('ai.confidence.veryLow')
    })
  })

  describe('Show/Hide Options', () => {
    it('should show value by default', () => {
      const wrapper = mountComponent({ showValue: true })
      expect(wrapper.text()).toContain('%')
    })

    it('should show label when enabled', () => {
      const wrapper = mountComponent({ showLabel: true, label: 'Test Label' })
      expect(wrapper.text()).toContain('Test Label')
    })
  })
})
