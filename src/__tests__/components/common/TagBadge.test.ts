import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import TagBadge from '@/components/common/TagBadge.vue'

describe('TagBadge', () => {
  describe('Rendering', () => {
    it('should render tag text', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'javascript' },
      })
      expect(wrapper.find('.tag-text').text()).toBe('javascript')
    })

    it('should show hash when showHash is true', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'vue', showHash: true },
      })
      expect(wrapper.find('.tag-hash').exists()).toBe(true)
      expect(wrapper.find('.tag-hash').text()).toBe('#')
    })

    it('should not show hash by default', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'vue' },
      })
      expect(wrapper.find('.tag-hash').exists()).toBe(false)
    })
  })

  describe('Size Variants', () => {
    it('should apply xs size classes', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test', size: 'xs' },
      })
      const badge = wrapper.find('.tag-badge')
      expect(badge.classes()).toContain('px-1.5')
      expect(badge.classes()).toContain('py-0.5')
    })

    it('should apply sm size classes by default', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test' },
      })
      const badge = wrapper.find('.tag-badge')
      expect(badge.classes()).toContain('px-2')
    })

    it('should apply md size classes', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test', size: 'md' },
      })
      const badge = wrapper.find('.tag-badge')
      expect(badge.classes()).toContain('px-2.5')
      expect(badge.classes()).toContain('py-1')
    })
  })

  describe('Variants', () => {
    it('should apply default variant styles', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test' },
      })
      const badge = wrapper.find('.tag-badge')
      const style = badge.attributes('style')
      expect(style).toContain('#f1f5f9') // default bg
    })

    it('should apply outlined variant', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test', variant: 'outlined' },
      })
      const badge = wrapper.find('.tag-badge')
      const style = badge.attributes('style')
      expect(style).toContain('transparent')
    })

    it('should apply gradient variant', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test', variant: 'gradient' },
      })
      const badge = wrapper.find('.tag-badge')
      const style = badge.attributes('style')
      // Gradient variant uses specific colors
      expect(style).toContain('#475569') // gradient text color
    })

    it('should apply colored variant', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test', variant: 'colored' },
      })
      const badge = wrapper.find('.tag-badge')
      const style = badge.attributes('style')
      expect(style).toContain('#0d9488') // teal color
    })

    it('should apply glass variant', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test', variant: 'glass' },
      })
      const badge = wrapper.find('.tag-badge')
      const style = badge.attributes('style')
      expect(style).toContain('rgba')
    })
  })

  describe('Custom Color', () => {
    it('should use custom color when provided', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test', color: '#ff5500' },
      })
      const badge = wrapper.find('.tag-badge')
      const style = badge.attributes('style')
      expect(style).toContain('#ff5500')
    })
  })

  describe('Clickable', () => {
    it('should not be clickable by default', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test' },
      })
      expect(wrapper.find('.tag-badge').classes()).not.toContain('tag-badge--clickable')
    })

    it('should be clickable when prop is true', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test', clickable: true },
      })
      expect(wrapper.find('.tag-badge').classes()).toContain('tag-badge--clickable')
    })

    it('should emit click event with tag when clickable', async () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'javascript', clickable: true },
      })
      await wrapper.find('.tag-badge').trigger('click')
      expect(wrapper.emitted('click')).toHaveLength(1)
      expect(wrapper.emitted('click')![0]).toEqual(['javascript'])
    })

    it('should not emit click when not clickable', async () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test', clickable: false },
      })
      await wrapper.find('.tag-badge').trigger('click')
      expect(wrapper.emitted('click')).toBeUndefined()
    })
  })

  describe('Removable', () => {
    it('should not show remove button by default', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test' },
      })
      expect(wrapper.find('.tag-remove').exists()).toBe(false)
    })

    it('should show remove button when removable', () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test', removable: true },
      })
      expect(wrapper.find('.tag-remove').exists()).toBe(true)
    })

    it('should emit remove event with tag when remove clicked', async () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'vue', removable: true },
      })
      await wrapper.find('.tag-remove').trigger('click')
      expect(wrapper.emitted('remove')).toHaveLength(1)
      expect(wrapper.emitted('remove')![0]).toEqual(['vue'])
    })

    it('should stop propagation on remove click', async () => {
      const wrapper = mount(TagBadge, {
        props: { tag: 'test', removable: true, clickable: true },
      })
      await wrapper.find('.tag-remove').trigger('click')
      // Click should only emit remove, not click
      expect(wrapper.emitted('remove')).toHaveLength(1)
      expect(wrapper.emitted('click')).toBeUndefined()
    })
  })
})
