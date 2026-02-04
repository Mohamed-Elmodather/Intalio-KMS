import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

describe('LoadingSpinner', () => {
  describe('Rendering', () => {
    it('should render spinner element', () => {
      const wrapper = mount(LoadingSpinner)
      expect(wrapper.find('.spinner').exists()).toBe(true)
    })

    it('should not render text by default', () => {
      const wrapper = mount(LoadingSpinner)
      expect(wrapper.find('p').exists()).toBe(false)
    })

    it('should render text when provided', () => {
      const wrapper = mount(LoadingSpinner, {
        props: { text: 'Loading...' },
      })
      expect(wrapper.find('p').exists()).toBe(true)
      expect(wrapper.find('p').text()).toBe('Loading...')
    })
  })

  describe('Size Variants', () => {
    it('should apply small size classes', () => {
      const wrapper = mount(LoadingSpinner, {
        props: { size: 'sm' },
      })
      const spinner = wrapper.find('.spinner')
      expect(spinner.classes()).toContain('w-5')
      expect(spinner.classes()).toContain('h-5')
      expect(spinner.classes()).toContain('border-2')
    })

    it('should apply medium size classes by default', () => {
      const wrapper = mount(LoadingSpinner)
      const spinner = wrapper.find('.spinner')
      expect(spinner.classes()).toContain('w-8')
      expect(spinner.classes()).toContain('h-8')
      expect(spinner.classes()).toContain('border-3')
    })

    it('should apply medium size classes when specified', () => {
      const wrapper = mount(LoadingSpinner, {
        props: { size: 'md' },
      })
      const spinner = wrapper.find('.spinner')
      expect(spinner.classes()).toContain('w-8')
      expect(spinner.classes()).toContain('h-8')
    })

    it('should apply large size classes', () => {
      const wrapper = mount(LoadingSpinner, {
        props: { size: 'lg' },
      })
      const spinner = wrapper.find('.spinner')
      expect(spinner.classes()).toContain('w-12')
      expect(spinner.classes()).toContain('h-12')
      expect(spinner.classes()).toContain('border-4')
    })
  })

  describe('Container Structure', () => {
    it('should have flex container with centering', () => {
      const wrapper = mount(LoadingSpinner)
      const container = wrapper.find('div')
      expect(container.classes()).toContain('flex')
      expect(container.classes()).toContain('flex-col')
      expect(container.classes()).toContain('items-center')
      expect(container.classes()).toContain('justify-center')
    })
  })
})
