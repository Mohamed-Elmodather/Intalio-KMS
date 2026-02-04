import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import CategoryBadge from '@/components/common/CategoryBadge.vue'

describe('CategoryBadge', () => {
  describe('Rendering', () => {
    it('should render category text', () => {
      const wrapper = mount(CategoryBadge, {
        props: { category: 'Technology' },
      })
      expect(wrapper.text()).toBe('Technology')
    })

    it('should render icon when provided', () => {
      const wrapper = mount(CategoryBadge, {
        props: { category: 'Tech', icon: 'fas fa-laptop' },
      })
      const iconEl = wrapper.find('i')
      expect(iconEl.exists()).toBe(true)
      expect(iconEl.classes()).toContain('fa-laptop')
    })

    it('should not render icon when not provided', () => {
      const wrapper = mount(CategoryBadge, {
        props: { category: 'Tech' },
      })
      expect(wrapper.find('i').exists()).toBe(false)
    })
  })

  describe('Size Variants', () => {
    it('should apply xs size classes', () => {
      const wrapper = mount(CategoryBadge, {
        props: { category: 'Test', size: 'xs' },
      })
      const badge = wrapper.find('.category-badge')
      expect(badge.classes()).toContain('px-1.5')
      expect(badge.classes()).toContain('py-0.5')
    })

    it('should apply sm size classes by default', () => {
      const wrapper = mount(CategoryBadge, {
        props: { category: 'Test' },
      })
      const badge = wrapper.find('.category-badge')
      expect(badge.classes()).toContain('px-2')
      expect(badge.classes()).toContain('py-0.5')
    })

    it('should apply md size classes', () => {
      const wrapper = mount(CategoryBadge, {
        props: { category: 'Test', size: 'md' },
      })
      const badge = wrapper.find('.category-badge')
      expect(badge.classes()).toContain('px-3')
      expect(badge.classes()).toContain('py-1.5')
    })
  })

  describe('Predefined Categories', () => {
    it('should apply correct color for tutorials category', () => {
      const wrapper = mount(CategoryBadge, {
        props: { category: 'Tutorials', categoryId: 'tutorials' },
      })
      const badge = wrapper.find('.category-badge')
      const style = badge.attributes('style')
      expect(style).toContain('#ede9fe') // tutorials bg color
    })

    it('should apply correct color for tech category', () => {
      const wrapper = mount(CategoryBadge, {
        props: { category: 'Technology', categoryId: 'tech' },
      })
      const badge = wrapper.find('.category-badge')
      const style = badge.attributes('style')
      expect(style).toContain('#dbeafe') // tech bg color
    })

    it('should apply default color for unknown category', () => {
      const wrapper = mount(CategoryBadge, {
        props: { category: 'Unknown', categoryId: 'unknown-category' },
      })
      const badge = wrapper.find('.category-badge')
      const style = badge.attributes('style')
      expect(style).toContain('#f1f5f9') // default bg color
    })
  })

  describe('Custom Color', () => {
    it('should use custom color when provided', () => {
      const wrapper = mount(CategoryBadge, {
        props: { category: 'Custom', color: '#ff0000' },
      })
      const badge = wrapper.find('.category-badge')
      const style = badge.attributes('style')
      expect(style).toContain('#ff0000')
    })
  })

  describe('Variant Styles', () => {
    it('should apply solid variant by default', () => {
      const wrapper = mount(CategoryBadge, {
        props: { category: 'Test' },
      })
      const badge = wrapper.find('.category-badge')
      const style = badge.attributes('style')
      expect(style).toContain('background')
    })

    it('should apply outline variant', () => {
      const wrapper = mount(CategoryBadge, {
        props: { category: 'Test', variant: 'outline' },
      })
      const badge = wrapper.find('.category-badge')
      const style = badge.attributes('style')
      expect(style).toContain('transparent')
    })

    it('should apply gradient variant', () => {
      const wrapper = mount(CategoryBadge, {
        props: { category: 'Test', variant: 'gradient' },
      })
      const badge = wrapper.find('.category-badge')
      const style = badge.attributes('style')
      // Gradient is applied via background property, check it's not the solid style
      expect(style).toContain('border: none')
    })
  })
})
