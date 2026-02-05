import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AppToast from '@/components/common/AppToast.vue'

// Mock UI store
const mockToasts = [
  { id: '1', type: 'success', title: 'Success', message: 'Operation completed' },
  { id: '2', type: 'error', title: 'Error', message: 'Something went wrong' },
]
const mockRemoveToast = vi.fn()

vi.mock('@/stores/ui', () => ({
  useUIStore: () => ({
    toasts: mockToasts,
    removeToast: mockRemoveToast,
  }),
}))

function mountComponent() {
  return shallowMount(AppToast, {
    global: {
      stubs: {
        TransitionGroup: true,
      },
    },
  })
}

describe('AppToast', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })

    it('should render toast container with fixed positioning', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fixed.top-20.right-4').exists()).toBe(true)
    })

    it('should render all toasts from store', () => {
      const wrapper = mountComponent()
      const toasts = wrapper.findAll('.pointer-events-auto')
      expect(toasts.length).toBe(2)
    })
  })

  describe('Toast Types', () => {
    it('should display toast titles', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Success')
      expect(wrapper.text()).toContain('Error')
    })

    it('should display toast messages', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Operation completed')
      expect(wrapper.text()).toContain('Something went wrong')
    })
  })

  describe('Icons', () => {
    it('should have getIcon function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getIcon).toBe('function')
    })

    it('should return check-circle icon for success', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getIcon('success')).toBe('fas fa-check-circle')
    })

    it('should return exclamation-circle icon for error', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getIcon('error')).toBe('fas fa-exclamation-circle')
    })

    it('should return exclamation-triangle icon for warning', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getIcon('warning')).toBe('fas fa-exclamation-triangle')
    })

    it('should return info-circle icon for info', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getIcon('info')).toBe('fas fa-info-circle')
    })
  })

  describe('Styles', () => {
    it('should have getStyles function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getStyles).toBe('function')
    })

    it('should return emerald styles for success', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getStyles('success')).toContain('bg-emerald-50')
      expect(vm.getStyles('success')).toContain('border-emerald-200')
      expect(vm.getStyles('success')).toContain('text-emerald-800')
    })

    it('should return red styles for error', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getStyles('error')).toContain('bg-red-50')
      expect(vm.getStyles('error')).toContain('border-red-200')
      expect(vm.getStyles('error')).toContain('text-red-800')
    })

    it('should return amber styles for warning', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getStyles('warning')).toContain('bg-amber-50')
      expect(vm.getStyles('warning')).toContain('border-amber-200')
      expect(vm.getStyles('warning')).toContain('text-amber-800')
    })

    it('should return blue styles for info', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getStyles('info')).toContain('bg-blue-50')
      expect(vm.getStyles('info')).toContain('border-blue-200')
      expect(vm.getStyles('info')).toContain('text-blue-800')
    })
  })

  describe('Icon Styles', () => {
    it('should have getIconStyles function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getIconStyles).toBe('function')
    })

    it('should return emerald icon style for success', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getIconStyles('success')).toBe('text-emerald-500')
    })

    it('should return red icon style for error', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getIconStyles('error')).toBe('text-red-500')
    })

    it('should return amber icon style for warning', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getIconStyles('warning')).toBe('text-amber-500')
    })

    it('should return blue icon style for info', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getIconStyles('info')).toBe('text-blue-500')
    })
  })

  describe('Close Button', () => {
    it('should render close button for each toast', () => {
      const wrapper = mountComponent()
      const closeButtons = wrapper.findAll('.fa-times')
      expect(closeButtons.length).toBe(2)
    })

    it('should call removeToast when close clicked', async () => {
      const wrapper = mountComponent()
      const closeButton = wrapper.find('.fa-times').element.parentElement
      await closeButton?.click()
      expect(mockRemoveToast).toHaveBeenCalledWith('1')
    })
  })

  describe('Z-Index', () => {
    it('should have high z-index for visibility', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.z-\\[100\\]').exists()).toBe(true)
    })
  })
})
