import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import CardActionButtons from '@/components/common/CardActionButtons.vue'

const mockActions = [
  { type: 'like' as const, active: false },
  { type: 'bookmark' as const, active: true },
  { type: 'share' as const },
]

function mountComponent(props = {}) {
  return shallowMount(CardActionButtons, {
    props: {
      actions: mockActions,
      ...props,
    },
  })
}

describe('CardActionButtons', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.card-action-buttons').exists()).toBe(true)
    })

    it('should render action buttons', () => {
      const wrapper = mountComponent()
      const buttons = wrapper.findAll('button')
      expect(buttons.length).toBe(3)
    })

    it('should filter out hidden actions', () => {
      const wrapper = mountComponent({
        actions: [
          { type: 'like' as const },
          { type: 'bookmark' as const, hidden: true },
        ],
      })
      const buttons = wrapper.findAll('button')
      expect(buttons.length).toBe(1)
    })
  })

  describe('Action Types', () => {
    it('should have like action configuration', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const config = vm.getActionConfig('like')
      expect(config.icon).toBe('far fa-heart')
      expect(config.activeIcon).toBe('fas fa-heart')
    })

    it('should have bookmark action configuration', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const config = vm.getActionConfig('bookmark')
      expect(config.icon).toBe('far fa-bookmark')
      expect(config.activeIcon).toBe('fas fa-bookmark')
    })

    it('should have share action configuration', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const config = vm.getActionConfig('share')
      expect(config.icon).toBe('fas fa-share-alt')
    })

    it('should have download action configuration', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const config = vm.getActionConfig('download')
      expect(config.icon).toBe('fas fa-download')
    })

    it('should have star action configuration', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const config = vm.getActionConfig('star')
      expect(config.icon).toBe('far fa-star')
      expect(config.activeIcon).toBe('fas fa-star')
    })

    it('should have compare action configuration', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const config = vm.getActionConfig('compare')
      expect(config.icon).toBe('fas fa-code-compare')
    })

    it('should have ai-analyze action configuration', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const config = vm.getActionConfig('ai-analyze')
      expect(config.icon).toBe('fas fa-wand-magic-sparkles')
    })
  })

  describe('Size Variants', () => {
    it('should apply xs size class', () => {
      const wrapper = mountComponent({ size: 'xs' })
      const vm = wrapper.vm as any
      expect(vm.sizeClasses).toContain('w-6')
      expect(vm.sizeClasses).toContain('h-6')
    })

    it('should apply sm size class by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sizeClasses).toContain('w-7')
      expect(vm.sizeClasses).toContain('h-7')
    })

    it('should apply md size class', () => {
      const wrapper = mountComponent({ size: 'md' })
      const vm = wrapper.vm as any
      expect(vm.sizeClasses).toContain('w-8')
      expect(vm.sizeClasses).toContain('h-8')
    })

    it('should apply lg size class', () => {
      const wrapper = mountComponent({ size: 'lg' })
      const vm = wrapper.vm as any
      expect(vm.sizeClasses).toContain('w-10')
      expect(vm.sizeClasses).toContain('h-10')
    })
  })

  describe('Variant Classes', () => {
    it('should apply rounded variant by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.variantClasses).toBe('rounded-lg')
    })

    it('should apply circular variant', () => {
      const wrapper = mountComponent({ variant: 'circular' })
      const vm = wrapper.vm as any
      expect(vm.variantClasses).toBe('rounded-full')
    })

    it('should apply square variant', () => {
      const wrapper = mountComponent({ variant: 'square' })
      const vm = wrapper.vm as any
      expect(vm.variantClasses).toBe('rounded-md')
    })
  })

  describe('Layout', () => {
    it('should apply horizontal layout by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.layoutClasses).toBe('flex-row')
    })

    it('should apply vertical layout', () => {
      const wrapper = mountComponent({ layout: 'vertical' })
      const vm = wrapper.vm as any
      expect(vm.layoutClasses).toBe('flex-col')
    })
  })

  describe('Position', () => {
    it('should apply top-right position by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.positionClasses).toBe('top-2 right-2')
    })

    it('should apply top-left position', () => {
      const wrapper = mountComponent({ position: 'top-left' })
      const vm = wrapper.vm as any
      expect(vm.positionClasses).toBe('top-2 left-2')
    })

    it('should apply bottom-left position', () => {
      const wrapper = mountComponent({ position: 'bottom-left' })
      const vm = wrapper.vm as any
      expect(vm.positionClasses).toBe('bottom-2 left-2')
    })

    it('should apply bottom-right position', () => {
      const wrapper = mountComponent({ position: 'bottom-right' })
      const vm = wrapper.vm as any
      expect(vm.positionClasses).toBe('bottom-2 right-2')
    })
  })

  describe('Show On Hover', () => {
    it('should apply hover opacity class when showOnHover is true', () => {
      const wrapper = mountComponent({ showOnHover: true })
      expect(wrapper.find('.opacity-0').exists()).toBe(true)
    })

    it('should not apply hover opacity class when showOnHover is false', () => {
      const wrapper = mountComponent({ showOnHover: false })
      expect(wrapper.find('.opacity-0').exists()).toBe(false)
    })
  })

  describe('Icon Display', () => {
    it('should show inactive icon when action is not active', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const icon = vm.getIcon({ type: 'like', active: false })
      expect(icon).toBe('far fa-heart')
    })

    it('should show active icon when action is active', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const icon = vm.getIcon({ type: 'like', active: true })
      expect(icon).toBe('fas fa-heart')
    })
  })

  describe('Button Classes', () => {
    it('should apply active class when action is active', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const classes = vm.getButtonClasses({ type: 'bookmark', active: true })
      expect(classes).toContain('bg-teal-500')
      expect(classes).toContain('text-white')
    })

    it('should apply inactive class when action is not active', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const classes = vm.getButtonClasses({ type: 'like', active: false })
      expect(classes).toContain('bg-white/95')
      expect(classes).toContain('text-gray-500')
    })

    it('should apply backdrop blur when backdrop is true', () => {
      const wrapper = mountComponent({ backdrop: true })
      const vm = wrapper.vm as any
      const classes = vm.getButtonClasses({ type: 'like', active: false })
      expect(classes).toContain('backdrop-blur-sm')
    })
  })

  describe('Action Events', () => {
    it('should emit action event on click', async () => {
      const wrapper = mountComponent()
      const button = wrapper.find('button')
      await button.trigger('click')
      expect(wrapper.emitted('action')).toBeTruthy()
      expect(wrapper.emitted('action')![0][0]).toBe('like')
    })

    it('should not emit action for disabled button', async () => {
      const wrapper = mountComponent({
        actions: [{ type: 'like' as const, disabled: true }],
      })
      const vm = wrapper.vm as any
      const mockEvent = { stopPropagation: vi.fn() }
      vm.handleAction({ type: 'like', disabled: true }, mockEvent)
      expect(wrapper.emitted('action')).toBeFalsy()
    })

    it('should stop event propagation', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const mockEvent = { stopPropagation: vi.fn() }
      vm.handleAction({ type: 'like' }, mockEvent)
      expect(mockEvent.stopPropagation).toHaveBeenCalled()
    })
  })

  describe('Tooltip', () => {
    it('should show tooltip with label', () => {
      const wrapper = mountComponent()
      const button = wrapper.find('button')
      expect(button.attributes('title')).toBe('Like')
    })

    it('should show custom tooltip when provided', () => {
      const wrapper = mountComponent({
        actions: [{ type: 'like' as const, tooltip: 'Custom Tooltip' }],
      })
      const button = wrapper.find('button')
      expect(button.attributes('title')).toBe('Custom Tooltip')
    })
  })
})
