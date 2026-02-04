import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import EmptyState from '@/components/common/EmptyState.vue'

describe('EmptyState', () => {
  describe('Rendering', () => {
    it('should render with required title prop', () => {
      const wrapper = mount(EmptyState, {
        props: { title: 'No items found' },
      })
      expect(wrapper.find('.empty-state__title').text()).toBe('No items found')
    })

    it('should render default icon', () => {
      const wrapper = mount(EmptyState, {
        props: { title: 'Test' },
      })
      expect(wrapper.find('.empty-state__icon i').classes()).toContain('fas')
      expect(wrapper.find('.empty-state__icon i').classes()).toContain('fa-inbox')
    })

    it('should render custom icon', () => {
      const wrapper = mount(EmptyState, {
        props: { title: 'Test', icon: 'fas fa-search' },
      })
      const iconEl = wrapper.find('.empty-state__icon i')
      expect(iconEl.classes()).toContain('fas')
      expect(iconEl.classes()).toContain('fa-search')
    })

    it('should render description when provided', () => {
      const wrapper = mount(EmptyState, {
        props: { title: 'Test', description: 'No results match your criteria' },
      })
      expect(wrapper.find('.empty-state__description').text()).toBe(
        'No results match your criteria'
      )
    })

    it('should not render description when not provided', () => {
      const wrapper = mount(EmptyState, {
        props: { title: 'Test' },
      })
      expect(wrapper.find('.empty-state__description').exists()).toBe(false)
    })
  })

  describe('Size Variants', () => {
    it('should apply small size class', () => {
      const wrapper = mount(EmptyState, {
        props: { title: 'Test', size: 'sm' },
      })
      expect(wrapper.find('.empty-state').classes()).toContain('empty-state--sm')
    })

    it('should apply medium size class by default', () => {
      const wrapper = mount(EmptyState, {
        props: { title: 'Test' },
      })
      expect(wrapper.find('.empty-state').classes()).toContain('empty-state--md')
    })

    it('should apply large size class', () => {
      const wrapper = mount(EmptyState, {
        props: { title: 'Test', size: 'lg' },
      })
      expect(wrapper.find('.empty-state').classes()).toContain('empty-state--lg')
    })
  })

  describe('Action Button', () => {
    it('should render action button when actionLabel provided', () => {
      const wrapper = mount(EmptyState, {
        props: { title: 'Test', actionLabel: 'Create New' },
      })
      expect(wrapper.find('.empty-state__action').exists()).toBe(true)
      expect(wrapper.find('.empty-state__action').text()).toBe('Create New')
    })

    it('should not render action button when actionLabel not provided', () => {
      const wrapper = mount(EmptyState, {
        props: { title: 'Test' },
      })
      expect(wrapper.find('.empty-state__action').exists()).toBe(false)
    })

    it('should render action icon when provided', () => {
      const wrapper = mount(EmptyState, {
        props: { title: 'Test', actionLabel: 'Add', actionIcon: 'fas fa-plus' },
      })
      const actionIcon = wrapper.find('.empty-state__action i')
      expect(actionIcon.exists()).toBe(true)
      expect(actionIcon.classes()).toContain('fa-plus')
    })

    it('should emit action event when button clicked', async () => {
      const wrapper = mount(EmptyState, {
        props: { title: 'Test', actionLabel: 'Create' },
      })
      await wrapper.find('.empty-state__action').trigger('click')
      expect(wrapper.emitted('action')).toHaveLength(1)
    })
  })

  describe('Slot Content', () => {
    it('should render slot content', () => {
      const wrapper = mount(EmptyState, {
        props: { title: 'Test' },
        slots: {
          default: '<div class="custom-content">Custom content here</div>',
        },
      })
      expect(wrapper.find('.custom-content').exists()).toBe(true)
      expect(wrapper.find('.custom-content').text()).toBe('Custom content here')
    })
  })
})
