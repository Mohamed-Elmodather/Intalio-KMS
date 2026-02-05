import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AISentimentBadge from '@/components/ai/AISentimentBadge.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

function mountComponent(props = {}) {
  return shallowMount(AISentimentBadge, {
    props: { sentiment: 'positive', ...props },
    global: {
      mocks: {
        $t: (key: string) => key,
      },
    },
  })
}

describe('AISentimentBadge', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the badge', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.ai-sentiment-badge').exists()).toBe(true)
    })
  })

  describe('Sentiment Config', () => {
    it('should show positive sentiment config', () => {
      const wrapper = mountComponent({ sentiment: 'positive' })
      const vm = wrapper.vm as any
      expect(vm.sentimentConfig.icon).toBe('fas fa-smile')
      expect(vm.sentimentConfig.bgClass).toBe('bg-green-100')
      expect(vm.sentimentConfig.textClass).toBe('text-green-700')
    })

    it('should show negative sentiment config', () => {
      const wrapper = mountComponent({ sentiment: 'negative' })
      const vm = wrapper.vm as any
      expect(vm.sentimentConfig.icon).toBe('fas fa-frown')
      expect(vm.sentimentConfig.bgClass).toBe('bg-red-100')
      expect(vm.sentimentConfig.textClass).toBe('text-red-700')
    })

    it('should show neutral sentiment config', () => {
      const wrapper = mountComponent({ sentiment: 'neutral' })
      const vm = wrapper.vm as any
      expect(vm.sentimentConfig.icon).toBe('fas fa-meh')
      expect(vm.sentimentConfig.bgClass).toBe('bg-gray-100')
      expect(vm.sentimentConfig.textClass).toBe('text-gray-700')
    })
  })

  describe('Sizes', () => {
    it('should apply small size classes', () => {
      const wrapper = mountComponent({ size: 'sm' })
      const vm = wrapper.vm as any
      expect(vm.sizeClasses.container).toContain('px-2')
      expect(vm.sizeClasses.container).toContain('text-xs')
    })

    it('should apply medium size classes by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sizeClasses.container).toContain('px-3')
      expect(vm.sizeClasses.container).toContain('text-sm')
    })

    it('should apply large size classes', () => {
      const wrapper = mountComponent({ size: 'lg' })
      const vm = wrapper.vm as any
      expect(vm.sizeClasses.container).toContain('px-4')
      expect(vm.sizeClasses.container).toContain('text-base')
    })
  })

  describe('Normalized Score', () => {
    it('should normalize score from -1 to 1 range', () => {
      const wrapper = mountComponent({ score: 0.5 })
      const vm = wrapper.vm as any
      expect(vm.normalizedScore).toBe(75)
    })

    it('should handle score of -1', () => {
      const wrapper = mountComponent({ score: -1 })
      const vm = wrapper.vm as any
      expect(vm.normalizedScore).toBe(0)
    })

    it('should handle score of 1', () => {
      const wrapper = mountComponent({ score: 1 })
      const vm = wrapper.vm as any
      expect(vm.normalizedScore).toBe(100)
    })

    it('should return 50 when score is undefined', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.normalizedScore).toBe(50)
    })
  })

  describe('Display Options', () => {
    it('should show label by default', () => {
      const wrapper = mountComponent({ showLabel: true })
      expect(wrapper.text()).toContain('comparison.positive')
    })

    it('should hide label when showLabel is false', () => {
      const wrapper = mountComponent({ showLabel: false })
      expect(wrapper.text()).not.toContain('comparison.positive')
    })

    it('should show score when showScore is true', () => {
      const wrapper = mountComponent({ showScore: true, score: 0.5 })
      expect(wrapper.text()).toContain('75%')
    })

    it('should show confidence when provided', () => {
      const wrapper = mountComponent({ confidence: 0.9 })
      expect(wrapper.text()).toContain('90%')
    })
  })

  describe('Emotions Display', () => {
    it('should show emotions when provided', () => {
      const emotions = [
        { emotion: 'joy', score: 0.8 },
        { emotion: 'sadness', score: 0.2 },
      ]
      const wrapper = mountComponent({ showEmotions: true, emotions })
      expect(wrapper.text()).toContain('joy')
      expect(wrapper.text()).toContain('sadness')
    })

    it('should not show emotions by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.space-y-1\\.5').exists()).toBe(false)
    })
  })

  describe('Interactive Mode', () => {
    it('should apply interactive styles when interactive', () => {
      const wrapper = mountComponent({ interactive: true })
      const badge = wrapper.find('.inline-flex')
      expect(badge.classes()).toContain('cursor-pointer')
    })
  })
})
