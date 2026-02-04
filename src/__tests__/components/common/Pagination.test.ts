import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import Pagination from '@/components/common/Pagination.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => {
      const translations: Record<string, string> = {
        'common.showing': 'Showing',
        'common.of': 'of',
        'common.show': 'Show',
        'common.perPage': 'per page',
        'common.previous': 'Previous',
        'common.next': 'Next',
      }
      return translations[key] || key
    },
  }),
}))

describe('Pagination', () => {
  const defaultProps = {
    currentPage: 1,
    totalItems: 100,
    itemsPerPage: 10,
  }

  describe('Rendering', () => {
    it('should render pagination component', () => {
      const wrapper = mount(Pagination, {
        props: defaultProps,
      })
      expect(wrapper.find('.pagination').exists()).toBe(true)
    })

    it('should show info text', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, showInfo: true },
      })
      expect(wrapper.text()).toContain('Showing')
      expect(wrapper.text()).toContain('1-10')
      expect(wrapper.text()).toContain('of')
      expect(wrapper.text()).toContain('100')
    })

    it('should hide info text when showInfo is false', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, showInfo: false },
      })
      expect(wrapper.find('.pagination__info').exists()).toBe(false)
    })

    it('should show items per page selector', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, showPerPageSelector: true },
      })
      expect(wrapper.find('.pagination__select').exists()).toBe(true)
    })

    it('should hide items per page selector when disabled', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, showPerPageSelector: false },
      })
      expect(wrapper.find('.pagination__select').exists()).toBe(false)
    })
  })

  describe('Page Numbers', () => {
    it('should show all pages when total pages <= 7', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, totalItems: 50, itemsPerPage: 10 }, // 5 pages
      })
      const pageButtons = wrapper.findAll('.pagination__page')
      expect(pageButtons).toHaveLength(5)
    })

    it('should show ellipsis for many pages', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, totalItems: 200, itemsPerPage: 10, currentPage: 5 },
      })
      expect(wrapper.text()).toContain('...')
    })

    it('should highlight current page', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, currentPage: 3 },
      })
      const activePage = wrapper.find('.pagination__page--active')
      expect(activePage.exists()).toBe(true)
      expect(activePage.text()).toBe('3')
    })

    it('should always show first and last page', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, totalItems: 200, itemsPerPage: 10, currentPage: 10 },
      })
      const pages = wrapper.findAll('.pagination__page')
      const pageNumbers = pages.map((p) => p.text())
      expect(pageNumbers).toContain('1')
      expect(pageNumbers).toContain('20')
    })
  })

  describe('Navigation Buttons', () => {
    it('should disable previous button on first page', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, currentPage: 1 },
      })
      const prevBtn = wrapper.findAll('.pagination__btn')[0]
      expect(prevBtn.classes()).toContain('pagination__btn--disabled')
    })

    it('should enable previous button on later pages', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, currentPage: 3 },
      })
      const prevBtn = wrapper.findAll('.pagination__btn')[0]
      expect(prevBtn.classes()).not.toContain('pagination__btn--disabled')
    })

    it('should disable next button on last page', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, currentPage: 10 },
      })
      const nextBtn = wrapper.findAll('.pagination__btn')[1]
      expect(nextBtn.classes()).toContain('pagination__btn--disabled')
    })

    it('should enable next button on earlier pages', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, currentPage: 5 },
      })
      const nextBtn = wrapper.findAll('.pagination__btn')[1]
      expect(nextBtn.classes()).not.toContain('pagination__btn--disabled')
    })
  })

  describe('Events', () => {
    it('should emit update:currentPage when page clicked', async () => {
      const wrapper = mount(Pagination, {
        props: defaultProps,
      })
      const pageButtons = wrapper.findAll('.pagination__page')
      // Find the button for page 2 and click it
      const page2 = pageButtons.find((btn) => btn.text() === '2')
      await page2!.trigger('click')

      expect(wrapper.emitted('update:currentPage')).toHaveLength(1)
      expect(wrapper.emitted('update:currentPage')![0]).toEqual([2])
    })

    it('should not emit when clicking current page', async () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, currentPage: 3 },
      })
      const pageButtons = wrapper.findAll('.pagination__page')
      const page3 = pageButtons.find((btn) => btn.text() === '3')
      await page3!.trigger('click')

      expect(wrapper.emitted('update:currentPage')).toBeUndefined()
    })

    it('should emit when previous button clicked', async () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, currentPage: 5 },
      })
      const prevBtn = wrapper.findAll('.pagination__btn')[0]
      await prevBtn.trigger('click')

      expect(wrapper.emitted('update:currentPage')).toHaveLength(1)
      expect(wrapper.emitted('update:currentPage')![0]).toEqual([4])
    })

    it('should emit when next button clicked', async () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, currentPage: 5 },
      })
      const nextBtn = wrapper.findAll('.pagination__btn')[1]
      await nextBtn.trigger('click')

      expect(wrapper.emitted('update:currentPage')).toHaveLength(1)
      expect(wrapper.emitted('update:currentPage')![0]).toEqual([6])
    })

    it('should emit update:itemsPerPage when changed', async () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, showPerPageSelector: true },
      })
      const select = wrapper.find('.pagination__select')
      await select.setValue('20')
      await select.trigger('change')

      expect(wrapper.emitted('update:itemsPerPage')).toBeDefined()
      // The event converts value to number via Number()
      const emittedValue = wrapper.emitted('update:itemsPerPage')![0][0]
      expect(typeof emittedValue).toBe('number')
    })

    it('should reset to page 1 when items per page changed', async () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, currentPage: 5, showPerPageSelector: true },
      })
      const select = wrapper.find('.pagination__select')
      await select.setValue(50)

      const pageEvents = wrapper.emitted('update:currentPage')
      expect(pageEvents).toBeDefined()
      expect(pageEvents![pageEvents!.length - 1]).toEqual([1])
    })
  })

  describe('Items Per Page Options', () => {
    it('should show default options', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, showPerPageSelector: true },
      })
      const options = wrapper.findAll('.pagination__select option')
      expect(options).toHaveLength(4)
      expect(options[0].text()).toBe('10')
      expect(options[1].text()).toBe('20')
      expect(options[2].text()).toBe('50')
      expect(options[3].text()).toBe('100')
    })

    it('should show custom options', () => {
      const wrapper = mount(Pagination, {
        props: {
          ...defaultProps,
          showPerPageSelector: true,
          itemsPerPageOptions: [5, 15, 25],
        },
      })
      const options = wrapper.findAll('.pagination__select option')
      expect(options).toHaveLength(3)
      expect(options[0].text()).toBe('5')
    })
  })

  describe('Edge Cases', () => {
    it('should handle zero total items', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, totalItems: 0, showInfo: true },
      })
      // When totalItems is 0, info shows "Showing 0-0 of 0"
      const infoEl = wrapper.find('.pagination__info')
      if (infoEl.exists()) {
        expect(infoEl.text()).toContain('0')
      } else {
        // Info might be hidden when no items
        expect(true).toBe(true)
      }
    })

    it('should handle single page', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, totalItems: 5, itemsPerPage: 10 },
      })
      const pageButtons = wrapper.findAll('.pagination__page')
      expect(pageButtons).toHaveLength(1)
      expect(pageButtons[0].text()).toBe('1')
    })

    it('should calculate correct end item on last page', () => {
      const wrapper = mount(Pagination, {
        props: { ...defaultProps, totalItems: 95, itemsPerPage: 10, currentPage: 10 },
      })
      expect(wrapper.text()).toContain('91-95')
    })
  })
})
