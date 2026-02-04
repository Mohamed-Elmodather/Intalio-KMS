import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import ViewAllButton from '@/components/common/ViewAllButton.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => {
      const translations: Record<string, string> = {
        'common.viewAll': 'View All',
      }
      return translations[key] || key
    },
  }),
}))

// Mock router-link
const mockRouterLink = {
  template: '<a :href="to"><slot /></a>',
  props: ['to'],
}

describe('ViewAllButton', () => {
  describe('Rendering', () => {
    it('should render as button by default', () => {
      const wrapper = mount(ViewAllButton, {
        global: {
          mocks: {
            $t: (key: string) => (key === 'common.viewAll' ? 'View All' : key),
          },
        },
      })
      expect(wrapper.element.tagName).toBe('BUTTON')
    })

    it('should render as router-link when to prop provided', () => {
      const wrapper = mount(ViewAllButton, {
        props: { to: '/articles' },
        global: {
          stubs: {
            'router-link': mockRouterLink,
          },
          mocks: {
            $t: (key: string) => (key === 'common.viewAll' ? 'View All' : key),
          },
        },
      })
      expect(wrapper.element.tagName).toBe('A')
    })

    it('should show default label', () => {
      const wrapper = mount(ViewAllButton, {
        global: {
          mocks: {
            $t: (key: string) => (key === 'common.viewAll' ? 'View All' : key),
          },
        },
      })
      expect(wrapper.text()).toContain('View All')
    })

    it('should show custom label', () => {
      const wrapper = mount(ViewAllButton, {
        props: { label: 'See More' },
        global: {
          mocks: {
            $t: (key: string) => (key === 'common.viewAll' ? 'View All' : key),
          },
        },
      })
      expect(wrapper.text()).toContain('See More')
    })

    it('should show slot content', () => {
      const wrapper = mount(ViewAllButton, {
        slots: {
          default: 'Browse All Items',
        },
        global: {
          mocks: {
            $t: (key: string) => (key === 'common.viewAll' ? 'View All' : key),
          },
        },
      })
      expect(wrapper.text()).toContain('Browse All Items')
    })

    it('should show count when provided', () => {
      const wrapper = mount(ViewAllButton, {
        props: { count: 42 },
        global: {
          mocks: {
            $t: (key: string) => (key === 'common.viewAll' ? 'View All' : key),
          },
        },
      })
      expect(wrapper.text()).toContain('(42)')
    })

    it('should not show count when not provided', () => {
      const wrapper = mount(ViewAllButton, {
        global: {
          mocks: {
            $t: (key: string) => (key === 'common.viewAll' ? 'View All' : key),
          },
        },
      })
      expect(wrapper.find('.view-all-btn__count').exists()).toBe(false)
    })

    it('should show arrow icon', () => {
      const wrapper = mount(ViewAllButton, {
        global: {
          mocks: {
            $t: (key: string) => (key === 'common.viewAll' ? 'View All' : key),
          },
        },
      })
      expect(wrapper.find('.fa-arrow-right').exists()).toBe(true)
    })
  })

  describe('Size Variants', () => {
    it('should apply sm size class by default', () => {
      const wrapper = mount(ViewAllButton, {
        global: {
          mocks: {
            $t: (key: string) => (key === 'common.viewAll' ? 'View All' : key),
          },
        },
      })
      expect(wrapper.classes()).toContain('view-all-btn--sm')
    })

    it('should apply md size class', () => {
      const wrapper = mount(ViewAllButton, {
        props: { size: 'md' },
        global: {
          mocks: {
            $t: (key: string) => (key === 'common.viewAll' ? 'View All' : key),
          },
        },
      })
      expect(wrapper.classes()).toContain('view-all-btn--md')
    })
  })

  describe('Events', () => {
    it('should emit click event when button clicked', async () => {
      const wrapper = mount(ViewAllButton, {
        global: {
          mocks: {
            $t: (key: string) => (key === 'common.viewAll' ? 'View All' : key),
          },
        },
      })
      await wrapper.trigger('click')

      expect(wrapper.emitted('click')).toHaveLength(1)
    })

    it('should not emit click when rendered as link', async () => {
      const wrapper = mount(ViewAllButton, {
        props: { to: '/articles' },
        global: {
          stubs: {
            'router-link': mockRouterLink,
          },
          mocks: {
            $t: (key: string) => (key === 'common.viewAll' ? 'View All' : key),
          },
        },
      })
      await wrapper.trigger('click')

      expect(wrapper.emitted('click')).toBeUndefined()
    })
  })

  describe('Styling', () => {
    it('should have view-all-btn class', () => {
      const wrapper = mount(ViewAllButton, {
        global: {
          mocks: {
            $t: (key: string) => (key === 'common.viewAll' ? 'View All' : key),
          },
        },
      })
      expect(wrapper.classes()).toContain('view-all-btn')
    })
  })
})
