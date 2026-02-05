import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AILoadingIndicator from '@/components/ai/AILoadingIndicator.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

function mountComponent(props = {}) {
  return shallowMount(AILoadingIndicator, {
    props,
    global: {
      mocks: {
        $t: (key: string) => key,
      },
    },
  })
}

describe('AILoadingIndicator', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the loading indicator', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.ai-loading-indicator').exists()).toBe(true)
    })
  })

  describe('Variants', () => {
    it('should render spinner variant by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.ai-spinner').exists()).toBe(true)
      expect(wrapper.find('svg.animate-spin').exists()).toBe(true)
    })

    it('should render dots variant', () => {
      const wrapper = mountComponent({ variant: 'dots' })
      expect(wrapper.findAll('.ai-dot').length).toBe(3)
    })

    it('should render pulse variant', () => {
      const wrapper = mountComponent({ variant: 'pulse' })
      expect(wrapper.find('.animate-ping').exists()).toBe(true)
    })

    it('should render shimmer variant', () => {
      const wrapper = mountComponent({ variant: 'shimmer' })
      expect(wrapper.find('.ai-shimmer').exists()).toBe(true)
    })
  })

  describe('Sizes', () => {
    it('should apply small size classes', () => {
      const wrapper = mountComponent({ size: 'sm' })
      const vm = wrapper.vm as any
      expect(vm.sizeClasses).toBe('w-4 h-4')
    })

    it('should apply medium size classes by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sizeClasses).toBe('w-6 h-6')
    })

    it('should apply large size classes', () => {
      const wrapper = mountComponent({ size: 'lg' })
      const vm = wrapper.vm as any
      expect(vm.sizeClasses).toBe('w-8 h-8')
    })
  })

  describe('Text Size Classes', () => {
    it('should apply small text size', () => {
      const wrapper = mountComponent({ size: 'sm' })
      const vm = wrapper.vm as any
      expect(vm.textSizeClass).toBe('text-xs')
    })

    it('should apply medium text size by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textSizeClass).toBe('text-sm')
    })

    it('should apply large text size', () => {
      const wrapper = mountComponent({ size: 'lg' })
      const vm = wrapper.vm as any
      expect(vm.textSizeClass).toBe('text-base')
    })
  })

  describe('Text Display', () => {
    it('should show default text when showText is true', () => {
      const wrapper = mountComponent({ showText: true })
      expect(wrapper.text()).toContain('ai.thinking')
    })

    it('should show custom text', () => {
      const wrapper = mountComponent({ text: 'Loading data...' })
      expect(wrapper.text()).toContain('Loading data...')
    })

    it('should hide text when showText is false', () => {
      const wrapper = mountComponent({ showText: false })
      expect(wrapper.text()).not.toContain('ai.thinking')
    })
  })

  describe('Dots Variant Animation', () => {
    it('should have animation delays on dots', () => {
      const wrapper = mountComponent({ variant: 'dots' })
      const dots = wrapper.findAll('.ai-dot')
      expect(dots[0].attributes('style')).toContain('animation-delay')
      expect(dots[1].attributes('style')).toContain('animation-delay')
      expect(dots[2].attributes('style')).toContain('animation-delay')
    })
  })
})
