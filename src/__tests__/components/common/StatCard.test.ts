import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import StatCard from '@/components/common/StatCard.vue'

describe('StatCard', () => {
  const defaultProps = {
    title: 'Total Users',
    value: 1234,
    icon: 'fas fa-users',
  }

  describe('Rendering', () => {
    it('should render title', () => {
      const wrapper = mount(StatCard, {
        props: defaultProps,
      })
      expect(wrapper.text()).toContain('Total Users')
    })

    it('should render numeric value', () => {
      const wrapper = mount(StatCard, {
        props: defaultProps,
      })
      expect(wrapper.text()).toContain('1234')
    })

    it('should render string value', () => {
      const wrapper = mount(StatCard, {
        props: { ...defaultProps, value: '$10,000' },
      })
      expect(wrapper.text()).toContain('$10,000')
    })

    it('should render icon', () => {
      const wrapper = mount(StatCard, {
        props: defaultProps,
      })
      expect(wrapper.find('i').classes()).toContain('fa-users')
    })

    it('should apply custom icon class', () => {
      const wrapper = mount(StatCard, {
        props: { ...defaultProps, iconClass: 'icon-box-success-soft' },
      })
      expect(wrapper.find('.icon-box').classes()).toContain('icon-box-success-soft')
    })

    it('should apply default icon class when not provided', () => {
      const wrapper = mount(StatCard, {
        props: defaultProps,
      })
      expect(wrapper.find('.icon-box').classes()).toContain('icon-box-primary-soft')
    })
  })

  describe('Trend Display', () => {
    it('should not show trend when not provided', () => {
      const wrapper = mount(StatCard, {
        props: defaultProps,
      })
      expect(wrapper.find('.text-emerald-600').exists()).toBe(false)
      expect(wrapper.find('.text-red-600').exists()).toBe(false)
    })

    it('should show upward trend with green color', () => {
      const wrapper = mount(StatCard, {
        props: {
          ...defaultProps,
          trend: { value: 12, label: 'vs last month', direction: 'up' },
        },
      })
      const trendEl = wrapper.find('.text-emerald-600')
      expect(trendEl.exists()).toBe(true)
      expect(trendEl.text()).toContain('12%')
      expect(trendEl.text()).toContain('vs last month')
    })

    it('should show downward trend with red color', () => {
      const wrapper = mount(StatCard, {
        props: {
          ...defaultProps,
          trend: { value: 5, label: 'vs last week', direction: 'down' },
        },
      })
      const trendEl = wrapper.find('.text-red-600')
      expect(trendEl.exists()).toBe(true)
      expect(trendEl.text()).toContain('5%')
    })

    it('should show up arrow icon for upward trend', () => {
      const wrapper = mount(StatCard, {
        props: {
          ...defaultProps,
          trend: { value: 10, label: 'increase', direction: 'up' },
        },
      })
      expect(wrapper.find('.fa-arrow-up').exists()).toBe(true)
    })

    it('should show down arrow icon for downward trend', () => {
      const wrapper = mount(StatCard, {
        props: {
          ...defaultProps,
          trend: { value: 10, label: 'decrease', direction: 'down' },
        },
      })
      expect(wrapper.find('.fa-arrow-down').exists()).toBe(true)
    })
  })

  describe('Structure', () => {
    it('should have card class', () => {
      const wrapper = mount(StatCard, {
        props: defaultProps,
      })
      expect(wrapper.find('.card').exists()).toBe(true)
    })

    it('should have proper padding', () => {
      const wrapper = mount(StatCard, {
        props: defaultProps,
      })
      expect(wrapper.find('.card').classes()).toContain('p-6')
    })

    it('should have icon-box-lg class on icon container', () => {
      const wrapper = mount(StatCard, {
        props: defaultProps,
      })
      expect(wrapper.find('.icon-box').classes()).toContain('icon-box-lg')
    })
  })
})
