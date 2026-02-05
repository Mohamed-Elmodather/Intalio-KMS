import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import PageHeroHeader from '@/components/common/PageHeroHeader.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

const mockStats = [
  { icon: 'fas fa-users', value: '24', label: 'Teams' },
  { icon: 'fas fa-trophy', value: '51', label: 'Matches' },
  { icon: 'fas fa-stadium', value: '8', label: 'Venues' },
]

function mountComponent(props = {}) {
  return shallowMount(PageHeroHeader, {
    props: {
      stats: mockStats,
      title: 'AFC Asian Cup 2027',
      subtitle: 'Experience the best of Asian football',
      ...props,
    },
  })
}

describe('PageHeroHeader', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.hero-gradient').exists()).toBe(true)
    })

    it('should render title', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('AFC Asian Cup 2027')
    })

    it('should render subtitle', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Experience the best of Asian football')
    })
  })

  describe('Badge', () => {
    it('should render badge with default icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-trophy').exists()).toBe(true)
    })

    it('should render badge with custom icon', () => {
      const wrapper = mountComponent({ badgeIcon: 'fas fa-star' })
      expect(wrapper.find('.fa-star').exists()).toBe(true)
    })

    it('should render badge text', () => {
      const wrapper = mountComponent({ badgeText: 'Featured Event' })
      expect(wrapper.text()).toContain('Featured Event')
    })

    it('should use default badge text', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('AFC Asian Cup 2027')
    })
  })

  describe('Stats', () => {
    it('should render all stats', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('24')
      expect(wrapper.text()).toContain('51')
      expect(wrapper.text()).toContain('8')
    })

    it('should render stat labels', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Teams')
      expect(wrapper.text()).toContain('Matches')
      expect(wrapper.text()).toContain('Venues')
    })

    it('should render stat icons', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-users').exists()).toBe(true)
      expect(wrapper.find('.fa-trophy').exists()).toBe(true)
      expect(wrapper.find('.fa-stadium').exists()).toBe(true)
    })

    it('should render stat cards', () => {
      const wrapper = mountComponent()
      const statCards = wrapper.findAll('.stat-card-square')
      expect(statCards.length).toBe(3)
    })
  })

  describe('Decorative Elements', () => {
    it('should render circle decorations', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.circle-drift-1').exists()).toBe(true)
      expect(wrapper.find('.circle-drift-2').exists()).toBe(true)
      expect(wrapper.find('.circle-drift-3').exists()).toBe(true)
    })
  })

  describe('Actions Slot', () => {
    it('should render actions slot content', () => {
      const wrapper = shallowMount(PageHeroHeader, {
        props: {
          stats: mockStats,
          title: 'Test Title',
          subtitle: 'Test Subtitle',
        },
        slots: {
          actions: '<button class="custom-action">Action Button</button>',
        },
      })
      expect(wrapper.find('.custom-action').exists()).toBe(true)
    })
  })

  describe('Stats Container', () => {
    it('should position stats container correctly', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.stats-top-right').exists()).toBe(true)
    })
  })

  describe('Stat Icon Box', () => {
    it('should render icon boxes for stats', () => {
      const wrapper = mountComponent()
      const iconBoxes = wrapper.findAll('.stat-icon-box')
      expect(iconBoxes.length).toBe(3)
    })
  })

  describe('Gradient Background', () => {
    it('should have gradient background class', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.hero-gradient').exists()).toBe(true)
    })
  })

  describe('Typography', () => {
    it('should render h1 for title', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('h1').exists()).toBe(true)
    })

    it('should apply title styling', () => {
      const wrapper = mountComponent()
      const title = wrapper.find('h1')
      expect(title.classes()).toContain('text-3xl')
      expect(title.classes()).toContain('font-bold')
      expect(title.classes()).toContain('text-white')
    })
  })

  describe('Empty Stats', () => {
    it('should handle empty stats array', () => {
      const wrapper = mountComponent({ stats: [] })
      const statCards = wrapper.findAll('.stat-card-square')
      expect(statCards.length).toBe(0)
    })
  })

  describe('Badge Styling', () => {
    it('should have backdrop blur on badge', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.backdrop-blur-sm').exists()).toBe(true)
    })

    it('should have rounded-full badge', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.rounded-full').exists()).toBe(true)
    })
  })
})
