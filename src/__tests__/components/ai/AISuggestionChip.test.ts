import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AISuggestionChip from '@/components/ai/AISuggestionChip.vue'

function mountComponent(props = {}) {
  return shallowMount(AISuggestionChip, {
    props: { label: 'Test Chip', ...props },
  })
}

describe('AISuggestionChip', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the chip', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('button').exists()).toBe(true)
    })

    it('should render label', () => {
      const wrapper = mountComponent({ label: 'Test Label' })
      expect(wrapper.text()).toContain('Test Label')
    })

    it('should render default AI icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('i.fas.fa-wand-magic-sparkles').exists()).toBe(true)
    })

    it('should render custom icon', () => {
      const wrapper = mountComponent({ icon: 'fas fa-tag' })
      expect(wrapper.find('i.fas.fa-tag').exists()).toBe(true)
    })
  })

  describe('Variants', () => {
    it('should apply default variant classes', () => {
      const wrapper = mountComponent({ variant: 'default' })
      const vm = wrapper.vm as any
      expect(vm.variantClasses).toContain('bg-gray-50')
      expect(vm.variantClasses).toContain('text-gray-700')
    })

    it('should apply primary variant classes', () => {
      const wrapper = mountComponent({ variant: 'primary' })
      const vm = wrapper.vm as any
      expect(vm.variantClasses).toContain('bg-teal-50')
      expect(vm.variantClasses).toContain('text-teal-700')
    })

    it('should apply success variant classes', () => {
      const wrapper = mountComponent({ variant: 'success' })
      const vm = wrapper.vm as any
      expect(vm.variantClasses).toContain('bg-green-50')
      expect(vm.variantClasses).toContain('text-green-700')
    })

    it('should apply warning variant classes', () => {
      const wrapper = mountComponent({ variant: 'warning' })
      const vm = wrapper.vm as any
      expect(vm.variantClasses).toContain('bg-amber-50')
      expect(vm.variantClasses).toContain('text-amber-700')
    })
  })

  describe('Selected State', () => {
    it('should apply selected classes when selected', () => {
      const wrapper = mountComponent({ selected: true })
      const vm = wrapper.vm as any
      expect(vm.variantClasses).toContain('bg-teal-100')
      expect(vm.variantClasses).toContain('ring-2')
    })
  })

  describe('Confidence Badge', () => {
    it('should show confidence when provided', () => {
      const wrapper = mountComponent({ confidence: 0.85 })
      expect(wrapper.text()).toContain('85%')
    })

    it('should not show confidence when undefined', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).not.toContain('%')
    })
  })

  describe('Removable', () => {
    it('should show remove button when removable', () => {
      const wrapper = mountComponent({ removable: true })
      expect(wrapper.findAll('button').length).toBe(2) // main button + remove button
    })

    it('should not show remove button by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.findAll('button').length).toBe(1)
    })

    it('should emit remove event when remove button clicked', async () => {
      const wrapper = mountComponent({ removable: true })
      const buttons = wrapper.findAll('button')
      await buttons[1].trigger('click')
      expect(wrapper.emitted('remove')).toBeTruthy()
    })
  })

  describe('Disabled State', () => {
    it('should disable button when disabled', () => {
      const wrapper = mountComponent({ disabled: true })
      expect(wrapper.find('button').attributes('disabled')).toBeDefined()
    })

    it('should apply disabled styles', () => {
      const wrapper = mountComponent({ disabled: true })
      expect(wrapper.find('button').classes()).toContain('opacity-50')
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
  })
})
