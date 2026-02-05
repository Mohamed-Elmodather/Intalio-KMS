import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AIActionButton from '@/components/ai/AIActionButton.vue'

function mountComponent(props = {}) {
  return shallowMount(AIActionButton, {
    props,
  })
}

describe('AIActionButton', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the button', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('button').exists()).toBe(true)
    })

    it('should render with label', () => {
      const wrapper = mountComponent({ label: 'Test Label' })
      expect(wrapper.text()).toContain('Test Label')
    })

    it('should render slot content when no label', () => {
      const wrapper = shallowMount(AIActionButton, {
        slots: {
          default: 'Slot Content',
        },
      })
      expect(wrapper.text()).toContain('Slot Content')
    })

    it('should render icon by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('i.fas.fa-wand-magic-sparkles').exists()).toBe(true)
    })

    it('should render custom icon', () => {
      const wrapper = mountComponent({ icon: 'fas fa-star' })
      expect(wrapper.find('i.fas.fa-star').exists()).toBe(true)
    })
  })

  describe('Variants', () => {
    it('should apply primary variant classes', () => {
      const wrapper = mountComponent({ variant: 'primary' })
      const vm = wrapper.vm as any
      expect(vm.variantClasses).toContain('bg-gradient-to-r')
      expect(vm.variantClasses).toContain('from-teal-500')
    })

    it('should apply secondary variant classes', () => {
      const wrapper = mountComponent({ variant: 'secondary' })
      const vm = wrapper.vm as any
      expect(vm.variantClasses).toContain('bg-teal-50')
      expect(vm.variantClasses).toContain('text-teal-700')
    })

    it('should apply outline variant classes', () => {
      const wrapper = mountComponent({ variant: 'outline' })
      const vm = wrapper.vm as any
      expect(vm.variantClasses).toContain('bg-white')
      expect(vm.variantClasses).toContain('border-teal-300')
    })

    it('should apply ghost variant classes', () => {
      const wrapper = mountComponent({ variant: 'ghost' })
      const vm = wrapper.vm as any
      expect(vm.variantClasses).toContain('bg-transparent')
    })
  })

  describe('Sizes', () => {
    it('should apply small size classes', () => {
      const wrapper = mountComponent({ size: 'sm' })
      const vm = wrapper.vm as any
      expect(vm.sizeClasses.button).toContain('px-2.5')
      expect(vm.sizeClasses.button).toContain('text-xs')
    })

    it('should apply medium size classes by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sizeClasses.button).toContain('px-3.5')
      expect(vm.sizeClasses.button).toContain('text-sm')
    })

    it('should apply large size classes', () => {
      const wrapper = mountComponent({ size: 'lg' })
      const vm = wrapper.vm as any
      expect(vm.sizeClasses.button).toContain('px-5')
      expect(vm.sizeClasses.button).toContain('text-base')
    })
  })

  describe('Loading State', () => {
    it('should show spinner when loading', () => {
      const wrapper = mountComponent({ loading: true })
      expect(wrapper.find('svg.animate-spin').exists()).toBe(true)
    })

    it('should hide icon when loading', () => {
      const wrapper = mountComponent({ loading: true })
      expect(wrapper.find('i.fas').exists()).toBe(false)
    })

    it('should disable button when loading', () => {
      const wrapper = mountComponent({ loading: true })
      expect(wrapper.find('button').attributes('disabled')).toBeDefined()
    })
  })

  describe('Disabled State', () => {
    it('should disable button when disabled prop is true', () => {
      const wrapper = mountComponent({ disabled: true })
      expect(wrapper.find('button').attributes('disabled')).toBeDefined()
    })

    it('should apply disabled styles', () => {
      const wrapper = mountComponent({ disabled: true })
      expect(wrapper.find('button').classes()).toContain('opacity-60')
      expect(wrapper.find('button').classes()).toContain('cursor-not-allowed')
    })
  })

  describe('Click Events', () => {
    it('should emit click event when clicked', async () => {
      const wrapper = mountComponent()
      await wrapper.find('button').trigger('click')
      expect(wrapper.emitted('click')).toBeTruthy()
    })

    it('should not emit click when disabled', async () => {
      const wrapper = mountComponent({ disabled: true })
      await wrapper.find('button').trigger('click')
      expect(wrapper.emitted('click')).toBeFalsy()
    })

    it('should not emit click when loading', async () => {
      const wrapper = mountComponent({ loading: true })
      await wrapper.find('button').trigger('click')
      expect(wrapper.emitted('click')).toBeFalsy()
    })
  })

  describe('Tooltip', () => {
    it('should set title attribute for tooltip', () => {
      const wrapper = mountComponent({ tooltip: 'Test Tooltip' })
      expect(wrapper.find('button').attributes('title')).toBe('Test Tooltip')
    })
  })
})
