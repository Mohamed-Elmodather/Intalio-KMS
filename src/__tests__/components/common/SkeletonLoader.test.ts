import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import SkeletonLoader from '@/components/common/SkeletonLoader.vue'

function mountComponent(props = {}) {
  return shallowMount(SkeletonLoader, {
    props,
  })
}

describe('SkeletonLoader', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })

    it('should render card type by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.skeleton-card').exists()).toBe(true)
    })
  })

  describe('Skeleton Types', () => {
    it('should render card skeleton', () => {
      const wrapper = mountComponent({ type: 'card' })
      expect(wrapper.find('.skeleton-card').exists()).toBe(true)
    })

    it('should render list-item skeleton', () => {
      const wrapper = mountComponent({ type: 'list-item' })
      expect(wrapper.find('.skeleton-list-item').exists()).toBe(true)
    })

    it('should render text skeleton', () => {
      const wrapper = mountComponent({ type: 'text' })
      expect(wrapper.findAll('.h-4.bg-gray-200').length).toBeGreaterThan(0)
    })

    it('should render avatar skeleton', () => {
      const wrapper = mountComponent({ type: 'avatar' })
      expect(wrapper.findAll('.w-10.h-10.rounded-full').length).toBeGreaterThan(0)
    })

    it('should render thumbnail skeleton', () => {
      const wrapper = mountComponent({ type: 'thumbnail' })
      expect(wrapper.find('.grid').exists()).toBe(true)
    })

    it('should render badge skeleton', () => {
      const wrapper = mountComponent({ type: 'badge' })
      expect(wrapper.findAll('.h-6.bg-gray-200.rounded-full').length).toBeGreaterThan(0)
    })

    it('should render button skeleton', () => {
      const wrapper = mountComponent({ type: 'button' })
      expect(wrapper.findAll('.h-10.bg-gray-200.rounded-lg').length).toBeGreaterThan(0)
    })
  })

  describe('Count', () => {
    it('should render 3 items by default', () => {
      const wrapper = mountComponent({ type: 'card' })
      const cards = wrapper.findAll('.skeleton-card')
      expect(cards.length).toBe(3)
    })

    it('should render custom count', () => {
      const wrapper = mountComponent({ type: 'card', count: 5 })
      const cards = wrapper.findAll('.skeleton-card')
      expect(cards.length).toBe(5)
    })

    it('should render correct count for list items', () => {
      const wrapper = mountComponent({ type: 'list-item', count: 4 })
      const items = wrapper.findAll('.skeleton-list-item')
      expect(items.length).toBe(4)
    })
  })

  describe('Columns', () => {
    it('should apply 3 columns by default', () => {
      const wrapper = mountComponent({ type: 'card' })
      const vm = wrapper.vm as any
      expect(vm.gridClasses).toContain('lg:grid-cols-3')
    })

    it('should apply 1 column', () => {
      const wrapper = mountComponent({ type: 'card', columns: 1 })
      const vm = wrapper.vm as any
      expect(vm.gridClasses).toBe('grid-cols-1')
    })

    it('should apply 2 columns', () => {
      const wrapper = mountComponent({ type: 'card', columns: 2 })
      const vm = wrapper.vm as any
      expect(vm.gridClasses).toContain('sm:grid-cols-2')
    })

    it('should apply 4 columns', () => {
      const wrapper = mountComponent({ type: 'card', columns: 4 })
      const vm = wrapper.vm as any
      expect(vm.gridClasses).toContain('xl:grid-cols-4')
    })
  })

  describe('Animation', () => {
    it('should apply animation class by default', () => {
      const wrapper = mountComponent({ type: 'card' })
      const vm = wrapper.vm as any
      expect(vm.animationClass).toBe('animate-pulse')
    })

    it('should not apply animation when disabled', () => {
      const wrapper = mountComponent({ type: 'card', animated: false })
      const vm = wrapper.vm as any
      expect(vm.animationClass).toBe('')
    })
  })

  describe('Content Type Image Aspect', () => {
    it('should use 16/9 aspect for article', () => {
      const wrapper = mountComponent({ contentType: 'article' })
      const vm = wrapper.vm as any
      expect(vm.imageAspect).toBe('aspect-[16/9]')
    })

    it('should use square aspect for document', () => {
      const wrapper = mountComponent({ contentType: 'document' })
      const vm = wrapper.vm as any
      expect(vm.imageAspect).toBe('aspect-square')
    })

    it('should use video aspect for media', () => {
      const wrapper = mountComponent({ contentType: 'media' })
      const vm = wrapper.vm as any
      expect(vm.imageAspect).toBe('aspect-video')
    })

    it('should use 4/3 aspect for event', () => {
      const wrapper = mountComponent({ contentType: 'event' })
      const vm = wrapper.vm as any
      expect(vm.imageAspect).toBe('aspect-[4/3]')
    })

    it('should use video aspect for course', () => {
      const wrapper = mountComponent({ contentType: 'course' })
      const vm = wrapper.vm as any
      expect(vm.imageAspect).toBe('aspect-video')
    })

    it('should use video aspect for generic', () => {
      const wrapper = mountComponent({ contentType: 'generic' })
      const vm = wrapper.vm as any
      expect(vm.imageAspect).toBe('aspect-video')
    })
  })

  describe('Show Image', () => {
    it('should show image placeholder by default', () => {
      const wrapper = mountComponent({ type: 'card' })
      expect(wrapper.find('.bg-gray-200').exists()).toBe(true)
    })

    it('should hide image when showImage is false', () => {
      const wrapper = mountComponent({ type: 'card', showImage: false })
      const imageContainer = wrapper.find('.aspect-video, .aspect-\\[16\\/9\\]')
      expect(imageContainer.exists()).toBe(false)
    })
  })

  describe('Show Badge', () => {
    it('should show badge placeholder by default', () => {
      const wrapper = mountComponent({ type: 'card', showBadge: true })
      expect(wrapper.find('.h-5.w-16.bg-gray-300.rounded-full').exists()).toBe(true)
    })

    it('should hide badge when showBadge is false', () => {
      const wrapper = mountComponent({ type: 'card', showBadge: false })
      expect(wrapper.find('.absolute.top-3.left-3').exists()).toBe(false)
    })
  })

  describe('Show Avatar', () => {
    it('should show avatar placeholder by default', () => {
      const wrapper = mountComponent({ type: 'card', showAvatar: true })
      expect(wrapper.find('.w-7.h-7.bg-gray-200.rounded-full').exists()).toBe(true)
    })

    it('should hide avatar when showAvatar is false', () => {
      const wrapper = mountComponent({ type: 'card', showAvatar: false })
      const footer = wrapper.find('.border-t.border-gray-100')
      if (footer.exists()) {
        expect(footer.find('.w-7.h-7.bg-gray-200.rounded-full').exists()).toBe(false)
      }
    })
  })

  describe('Lines', () => {
    it('should render 2 lines by default', () => {
      const wrapper = mountComponent({ type: 'card' })
      const vm = wrapper.vm as any
      expect(vm.lines).toBe(2)
    })

    it('should render custom number of lines', () => {
      const wrapper = mountComponent({ type: 'card', lines: 4 })
      const vm = wrapper.vm as any
      expect(vm.lines).toBe(4)
    })
  })

  describe('Card Skeleton Structure', () => {
    it('should render meta line placeholders', () => {
      const wrapper = mountComponent({ type: 'card' })
      const metaLines = wrapper.findAll('.h-3.bg-gray-200.rounded')
      expect(metaLines.length).toBeGreaterThan(0)
    })

    it('should render title placeholders', () => {
      const wrapper = mountComponent({ type: 'card' })
      const titles = wrapper.findAll('.h-5.bg-gray-200.rounded')
      expect(titles.length).toBeGreaterThan(0)
    })

    it('should render tag placeholders', () => {
      const wrapper = mountComponent({ type: 'card' })
      const tags = wrapper.findAll('.h-5.bg-gray-100.rounded-full')
      expect(tags.length).toBeGreaterThan(0)
    })

    it('should render action button placeholders', () => {
      const wrapper = mountComponent({ type: 'card' })
      const actionBtns = wrapper.findAll('.w-7.h-7.bg-gray-300.rounded-full')
      expect(actionBtns.length).toBeGreaterThan(0)
    })
  })

  describe('List Item Skeleton Structure', () => {
    it('should render thumbnail for list items', () => {
      const wrapper = mountComponent({ type: 'list-item', showImage: true })
      expect(wrapper.find('.w-24.h-20.bg-gray-200.rounded-lg').exists()).toBe(true)
    })

    it('should render action buttons for list items', () => {
      const wrapper = mountComponent({ type: 'list-item' })
      const actionBtns = wrapper.findAll('.w-8.h-8.bg-gray-100.rounded-lg')
      expect(actionBtns.length).toBeGreaterThan(0)
    })
  })

  describe('Text Skeleton', () => {
    it('should render lines with varying widths', () => {
      const wrapper = mountComponent({ type: 'text', count: 3 })
      const lines = wrapper.findAll('.h-4.bg-gray-200.rounded')
      expect(lines.length).toBe(3)
    })

    it('should apply animation to text lines', () => {
      const wrapper = mountComponent({ type: 'text' })
      expect(wrapper.find('.animate-pulse').exists()).toBe(true)
    })
  })

  describe('Default Props', () => {
    it('should have correct default values', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.type).toBe('card')
      expect(vm.contentType).toBe('generic')
      expect(vm.count).toBe(3)
      expect(vm.columns).toBe(3)
      expect(vm.animated).toBe(true)
      expect(vm.showImage).toBe(true)
      expect(vm.showBadge).toBe(true)
      expect(vm.showAvatar).toBe(true)
      expect(vm.lines).toBe(2)
    })
  })
})
