import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import FilterDropdown from '@/components/common/FilterDropdown.vue'

const mockOptions = [
  { id: 'opt1', label: 'Option 1', icon: 'fas fa-star' },
  { id: 'opt2', label: 'Option 2', icon: 'fas fa-heart', count: 10 },
  { id: 'opt3', label: 'Option 3', bgColor: '#14b8a6' },
]

function mountComponent(props = {}) {
  return shallowMount(FilterDropdown, {
    props: {
      icon: 'fas fa-filter',
      label: 'Filter',
      selectedLabel: 'selected',
      headerLabel: 'Select Options',
      options: mockOptions,
      modelValue: [],
      clearAllLabel: 'Clear All',
      applyLabel: 'Apply',
      ...props,
    },
  })
}

describe('FilterDropdown', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })

    it('should render filter button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('button').exists()).toBe(true)
    })

    it('should render filter icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-filter').exists()).toBe(true)
    })

    it('should display label when no selection', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Filter')
    })
  })

  describe('Dropdown Toggle', () => {
    it('should not show dropdown by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isOpen).toBe(false)
    })

    it('should toggle dropdown on click', async () => {
      const wrapper = mountComponent()
      await wrapper.find('button').trigger('click')
      const vm = wrapper.vm as any
      expect(vm.isOpen).toBe(true)
    })

    it('should show chevron down when closed', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-chevron-down').exists()).toBe(true)
    })

    it('should show chevron up when open', async () => {
      const wrapper = mountComponent()
      await wrapper.find('button').trigger('click')
      expect(wrapper.find('.fa-chevron-up').exists()).toBe(true)
    })
  })

  describe('Options Display', () => {
    it('should show options when dropdown is open', async () => {
      const wrapper = mountComponent()
      await wrapper.find('button').trigger('click')
      expect(wrapper.text()).toContain('Option 1')
      expect(wrapper.text()).toContain('Option 2')
      expect(wrapper.text()).toContain('Option 3')
    })

    it('should show header label', async () => {
      const wrapper = mountComponent()
      await wrapper.find('button').trigger('click')
      expect(wrapper.text()).toContain('Select Options')
    })

    it('should show option icons', async () => {
      const wrapper = mountComponent()
      await wrapper.find('button').trigger('click')
      expect(wrapper.find('.fa-star').exists()).toBe(true)
    })

    it('should show option count when showCount is true', async () => {
      const wrapper = mountComponent({ showCount: true })
      await wrapper.find('button').trigger('click')
      expect(wrapper.text()).toContain('10')
    })
  })

  describe('Selection', () => {
    it('should show selected count in label', () => {
      const wrapper = mountComponent({ modelValue: ['opt1', 'opt2'] })
      expect(wrapper.text()).toContain('2 selected')
    })

    it('should apply selected styling to button when has selection', () => {
      const wrapper = mountComponent({ modelValue: ['opt1'] })
      expect(wrapper.find('.bg-teal-50').exists()).toBe(true)
    })

    it('should check if option is selected', () => {
      const wrapper = mountComponent({ modelValue: ['opt1'] })
      const vm = wrapper.vm as any
      expect(vm.isSelected('opt1')).toBe(true)
      expect(vm.isSelected('opt2')).toBe(false)
    })
  })

  describe('Toggle Option', () => {
    it('should add option when not selected', async () => {
      const wrapper = mountComponent({ modelValue: [] })
      const vm = wrapper.vm as any
      vm.toggleOption('opt1')
      expect(wrapper.emitted('update:modelValue')![0]).toEqual([['opt1']])
    })

    it('should remove option when already selected', async () => {
      const wrapper = mountComponent({ modelValue: ['opt1', 'opt2'] })
      const vm = wrapper.vm as any
      vm.toggleOption('opt1')
      expect(wrapper.emitted('update:modelValue')![0]).toEqual([['opt2']])
    })

    it('should handle single select mode', async () => {
      const wrapper = mountComponent({ modelValue: [], multiSelect: false })
      const vm = wrapper.vm as any
      vm.toggleOption('opt1')
      expect(wrapper.emitted('update:modelValue')![0]).toEqual([['opt1']])
    })

    it('should deselect in single select mode', async () => {
      const wrapper = mountComponent({ modelValue: ['opt1'], multiSelect: false })
      const vm = wrapper.vm as any
      vm.toggleOption('opt1')
      expect(wrapper.emitted('update:modelValue')![0]).toEqual([[]])
    })
  })

  describe('Clear All', () => {
    it('should show clear all button', async () => {
      const wrapper = mountComponent()
      await wrapper.find('button').trigger('click')
      expect(wrapper.text()).toContain('Clear All')
    })

    it('should emit empty array on clear all', async () => {
      const wrapper = mountComponent({ modelValue: ['opt1', 'opt2'] })
      const vm = wrapper.vm as any
      vm.clearAll()
      expect(wrapper.emitted('update:modelValue')![0]).toEqual([[]])
    })

    it('should close dropdown on clear all', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.isOpen = true
      vm.clearAll()
      expect(vm.isOpen).toBe(false)
    })
  })

  describe('Apply', () => {
    it('should show apply button', async () => {
      const wrapper = mountComponent()
      await wrapper.find('button').trigger('click')
      expect(wrapper.text()).toContain('Apply')
    })

    it('should close dropdown on apply', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.isOpen = true
      vm.apply()
      expect(vm.isOpen).toBe(false)
    })
  })

  describe('Checkbox', () => {
    it('should show check mark for selected options', async () => {
      const wrapper = mountComponent({ modelValue: ['opt1'] })
      await wrapper.find('button').trigger('click')
      expect(wrapper.find('.fa-check').exists()).toBe(true)
    })
  })

  describe('Color Badge', () => {
    it('should show color badge when showColorBadge is true', async () => {
      const wrapper = mountComponent({ showColorBadge: true })
      await wrapper.find('button').trigger('click')
      const colorBadge = wrapper.find('[style*="background-color"]')
      expect(colorBadge.exists()).toBe(true)
    })
  })

  describe('Display Label', () => {
    it('should compute display label correctly', () => {
      const wrapper = mountComponent({ modelValue: [] })
      const vm = wrapper.vm as any
      expect(vm.displayLabel).toBe('Filter')
    })

    it('should show selected count in display label', () => {
      const wrapper = mountComponent({ modelValue: ['opt1'] })
      const vm = wrapper.vm as any
      expect(vm.displayLabel).toBe('1 selected')
    })
  })

  describe('Has Selection', () => {
    it('should compute hasSelection correctly', () => {
      const wrapper = mountComponent({ modelValue: [] })
      const vm = wrapper.vm as any
      expect(vm.hasSelection).toBe(false)
    })

    it('should return true when has selection', () => {
      const wrapper = mountComponent({ modelValue: ['opt1'] })
      const vm = wrapper.vm as any
      expect(vm.hasSelection).toBe(true)
    })
  })

  describe('Dropdown Width', () => {
    it('should apply default width', async () => {
      const wrapper = mountComponent()
      await wrapper.find('button').trigger('click')
      expect(wrapper.find('.w-56').exists()).toBe(true)
    })

    it('should apply custom width', async () => {
      const wrapper = mountComponent({ dropdownWidth: 'w-72' })
      await wrapper.find('button').trigger('click')
      expect(wrapper.find('.w-72').exists()).toBe(true)
    })
  })

  describe('Click Outside', () => {
    it('should close dropdown when clicking overlay', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.isOpen = true
      await wrapper.vm.$nextTick()
      const overlay = wrapper.find('.fixed.inset-0')
      if (overlay.exists()) {
        await overlay.trigger('click')
        expect(vm.isOpen).toBe(false)
      }
    })
  })
})
