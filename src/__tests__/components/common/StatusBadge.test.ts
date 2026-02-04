import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import StatusBadge from '@/components/common/StatusBadge.vue'

describe('StatusBadge', () => {
  describe('Rendering', () => {
    it('should render status label', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'featured' },
      })
      expect(wrapper.text()).toContain('Featured')
    })

    it('should render custom label when provided', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'new', label: 'Brand New' },
      })
      expect(wrapper.text()).toBe('Brand New')
    })

    it('should convert hyphenated status to title case', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'in-progress' },
      })
      expect(wrapper.text()).toContain('In Progress')
    })

    it('should show icon by default', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'featured' },
      })
      expect(wrapper.find('i').exists()).toBe(true)
      expect(wrapper.find('i').classes()).toContain('fa-star')
    })

    it('should hide icon when showIcon is false', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'featured', showIcon: false },
      })
      expect(wrapper.find('i').exists()).toBe(false)
    })
  })

  describe('Status Types', () => {
    const statusIcons: Record<string, string> = {
      featured: 'fa-star',
      new: 'fa-sparkles',
      hot: 'fa-fire',
      trending: 'fa-arrow-trend-up',
      popular: 'fa-fire-flame-curved',
      recommended: 'fa-thumbs-up',
      'ai-powered': 'fa-wand-magic-sparkles',
      virtual: 'fa-video',
      'in-person': 'fa-location-dot',
      draft: 'fa-file-pen',
      published: 'fa-check-circle',
      completed: 'fa-circle-check',
      'in-progress': 'fa-spinner',
      pending: 'fa-clock',
    }

    Object.entries(statusIcons).forEach(([status, iconClass]) => {
      it(`should render correct icon for ${status} status`, () => {
        const wrapper = mount(StatusBadge, {
          props: { status },
        })
        expect(wrapper.find('i').classes()).toContain(iconClass)
      })
    })

    it('should use default icon for unknown status', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'unknown-status' },
      })
      expect(wrapper.find('i').classes()).toContain('fa-tag')
    })
  })

  describe('Size Variants', () => {
    it('should apply xs size classes', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'new', size: 'xs' },
      })
      const badge = wrapper.find('.status-badge')
      expect(badge.classes()).toContain('px-1.5')
      expect(badge.classes()).toContain('py-0.5')
    })

    it('should apply sm size classes by default', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'new' },
      })
      const badge = wrapper.find('.status-badge')
      expect(badge.classes()).toContain('px-2')
    })

    it('should apply md size classes', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'new', size: 'md' },
      })
      const badge = wrapper.find('.status-badge')
      expect(badge.classes()).toContain('px-2.5')
      expect(badge.classes()).toContain('py-1')
    })

    it('should apply lg size classes', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'new', size: 'lg' },
      })
      const badge = wrapper.find('.status-badge')
      expect(badge.classes()).toContain('px-3')
      expect(badge.classes()).toContain('py-1.5')
    })
  })

  describe('Variants', () => {
    it('should apply solid variant by default', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'featured' },
      })
      const badge = wrapper.find('.status-badge')
      const style = badge.attributes('style')
      expect(style).toContain('background')
      expect(style).toContain('#f59e0b') // featured color
    })

    it('should apply gradient variant', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'featured', variant: 'gradient' },
      })
      const badge = wrapper.find('.status-badge')
      const style = badge.attributes('style')
      // Gradient variant adds box-shadow
      expect(style).toContain('box-shadow')
    })

    it('should apply outline variant', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'featured', variant: 'outline' },
      })
      const badge = wrapper.find('.status-badge')
      const style = badge.attributes('style')
      expect(style).toContain('transparent')
    })

    it('should apply glass variant', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'featured', variant: 'glass' },
      })
      const badge = wrapper.find('.status-badge')
      const style = badge.attributes('style')
      expect(style).toContain('backdrop-filter')
    })
  })

  describe('Color Variations', () => {
    it('should use green colors for published status', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'published' },
      })
      const style = wrapper.find('.status-badge').attributes('style')
      expect(style).toContain('#dcfce7') // green bg
      expect(style).toContain('#16a34a') // green text
    })

    it('should use yellow colors for pending status', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'pending' },
      })
      const style = wrapper.find('.status-badge').attributes('style')
      expect(style).toContain('#fef3c7') // yellow bg
    })

    it('should use red colors for hot status', () => {
      const wrapper = mount(StatusBadge, {
        props: { status: 'hot' },
      })
      const style = wrapper.find('.status-badge').attributes('style')
      expect(style).toContain('#ef4444') // red
    })
  })
})
