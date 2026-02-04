import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import RatingStars from '@/components/common/RatingStars.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => {
      const translations: Record<string, string> = {
        'common.rating': 'rating',
        'common.ratings': 'ratings',
      }
      return translations[key] || key
    },
  }),
}))

describe('RatingStars', () => {
  describe('Rendering', () => {
    it('should render 5 star buttons', () => {
      const wrapper = mount(RatingStars)
      const buttons = wrapper.findAll('button')
      expect(buttons).toHaveLength(5)
    })

    it('should render all stars with star icon', () => {
      const wrapper = mount(RatingStars)
      const stars = wrapper.findAll('.fa-star')
      expect(stars).toHaveLength(5)
    })
  })

  describe('Display Rating', () => {
    it('should highlight stars based on modelValue', () => {
      const wrapper = mount(RatingStars, {
        props: { modelValue: 3 },
      })
      const stars = wrapper.findAll('.fa-star')
      expect(stars[0].classes()).toContain('text-yellow-400')
      expect(stars[1].classes()).toContain('text-yellow-400')
      expect(stars[2].classes()).toContain('text-yellow-400')
      expect(stars[3].classes()).toContain('text-gray-300')
      expect(stars[4].classes()).toContain('text-gray-300')
    })

    it('should highlight stars based on average when no modelValue', () => {
      const wrapper = mount(RatingStars, {
        props: { average: 4 },
      })
      const stars = wrapper.findAll('.fa-star')
      expect(stars[0].classes()).toContain('text-yellow-400')
      expect(stars[3].classes()).toContain('text-yellow-400')
      expect(stars[4].classes()).toContain('text-gray-300')
    })

    it('should show average value when showAverage is true', () => {
      const wrapper = mount(RatingStars, {
        props: { average: 4.5, showAverage: true },
      })
      expect(wrapper.text()).toContain('4.5')
    })

    it('should not show average value when showAverage is false', () => {
      const wrapper = mount(RatingStars, {
        props: { average: 4.5, showAverage: false },
      })
      expect(wrapper.text()).not.toContain('4.5')
    })

    it('should show count when showCount is true', () => {
      const wrapper = mount(RatingStars, {
        props: { count: 100, showCount: true },
      })
      expect(wrapper.text()).toContain('100')
      expect(wrapper.text()).toContain('ratings')
    })

    it('should show singular rating label for count of 1', () => {
      const wrapper = mount(RatingStars, {
        props: { count: 1, showCount: true },
      })
      expect(wrapper.text()).toContain('rating')
    })

    it('should not show count when showCount is false', () => {
      const wrapper = mount(RatingStars, {
        props: { count: 100, showCount: false },
      })
      expect(wrapper.text()).not.toContain('100')
    })
  })

  describe('Size Variants', () => {
    it('should apply sm size classes', () => {
      const wrapper = mount(RatingStars, {
        props: { size: 'sm' },
      })
      expect(wrapper.find('.rating-stars').classes()).toContain('text-sm')
    })

    it('should apply md size classes by default', () => {
      const wrapper = mount(RatingStars)
      expect(wrapper.find('.rating-stars').classes()).toContain('text-lg')
    })

    it('should apply lg size classes', () => {
      const wrapper = mount(RatingStars, {
        props: { size: 'lg' },
      })
      expect(wrapper.find('.rating-stars').classes()).toContain('text-2xl')
    })
  })

  describe('Interactive Mode', () => {
    it('should emit update:modelValue when star clicked', async () => {
      const wrapper = mount(RatingStars, {
        props: { readonly: false },
      })
      const buttons = wrapper.findAll('button')
      await buttons[2].trigger('click')

      expect(wrapper.emitted('update:modelValue')).toHaveLength(1)
      expect(wrapper.emitted('update:modelValue')![0]).toEqual([3])
    })

    it('should emit rate event when star clicked', async () => {
      const wrapper = mount(RatingStars, {
        props: { readonly: false },
      })
      const buttons = wrapper.findAll('button')
      await buttons[4].trigger('click')

      expect(wrapper.emitted('rate')).toHaveLength(1)
      expect(wrapper.emitted('rate')![0]).toEqual([5])
    })

    it('should not emit events in readonly mode', async () => {
      const wrapper = mount(RatingStars, {
        props: { readonly: true },
      })
      const buttons = wrapper.findAll('button')
      await buttons[0].trigger('click')

      expect(wrapper.emitted('update:modelValue')).toBeUndefined()
      expect(wrapper.emitted('rate')).toBeUndefined()
    })

    it('should have disabled buttons in readonly mode', () => {
      const wrapper = mount(RatingStars, {
        props: { readonly: true },
      })
      const buttons = wrapper.findAll('button')
      buttons.forEach((btn) => {
        expect(btn.attributes('disabled')).toBeDefined()
      })
    })

    it('should have cursor-pointer class when not readonly', () => {
      const wrapper = mount(RatingStars, {
        props: { readonly: false },
      })
      const buttons = wrapper.findAll('button')
      expect(buttons[0].classes()).toContain('cursor-pointer')
    })

    it('should have cursor-default class when readonly', () => {
      const wrapper = mount(RatingStars, {
        props: { readonly: true },
      })
      const buttons = wrapper.findAll('button')
      expect(buttons[0].classes()).toContain('cursor-default')
    })
  })

  describe('Hover State', () => {
    it('should highlight stars on hover', async () => {
      const wrapper = mount(RatingStars, {
        props: { readonly: false },
      })
      const buttons = wrapper.findAll('button')
      await buttons[3].trigger('mouseenter')

      const stars = wrapper.findAll('.fa-star')
      expect(stars[0].classes()).toContain('text-yellow-400')
      expect(stars[3].classes()).toContain('text-yellow-400')
      expect(stars[4].classes()).toContain('text-gray-300')
    })

    it('should reset on mouse leave', async () => {
      const wrapper = mount(RatingStars, {
        props: { readonly: false, modelValue: 2 },
      })
      const buttons = wrapper.findAll('button')

      await buttons[4].trigger('mouseenter')
      await buttons[4].trigger('mouseleave')

      const stars = wrapper.findAll('.fa-star')
      // Should revert to modelValue of 2
      expect(stars[0].classes()).toContain('text-yellow-400')
      expect(stars[1].classes()).toContain('text-yellow-400')
      expect(stars[2].classes()).toContain('text-gray-300')
    })

    it('should not update hover state in readonly mode', async () => {
      const wrapper = mount(RatingStars, {
        props: { readonly: true, average: 3 },
      })
      const buttons = wrapper.findAll('button')
      await buttons[4].trigger('mouseenter')

      // Should stay at average of 3
      const stars = wrapper.findAll('.fa-star')
      expect(stars[3].classes()).toContain('text-gray-300')
      expect(stars[4].classes()).toContain('text-gray-300')
    })
  })
})
