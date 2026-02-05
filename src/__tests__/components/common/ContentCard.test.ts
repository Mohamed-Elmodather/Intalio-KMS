import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import ContentCard from '@/components/common/ContentCard.vue'

function mountComponent(props = {}) {
  return shallowMount(ContentCard, {
    props: {
      title: 'Test Article',
      ...props,
    },
    global: {
      stubs: {
        CategoryBadge: true,
        StatusBadge: true,
        TagBadge: true,
      },
    },
  })
}

describe('ContentCard', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.content-card').exists()).toBe(true)
    })

    it('should render title', () => {
      const wrapper = mountComponent({ title: 'My Article' })
      expect(wrapper.text()).toContain('My Article')
    })

    it('should render as article element', () => {
      const wrapper = mountComponent()
      expect(wrapper.element.tagName).toBe('ARTICLE')
    })
  })

  describe('Variants', () => {
    it('should render grid variant by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.variant).toBe('grid')
    })

    it('should render list variant', () => {
      const wrapper = mountComponent({ variant: 'list' })
      expect(wrapper.find('.content-card.flex').exists()).toBe(true)
    })

    it('should render compact variant', () => {
      const wrapper = mountComponent({ variant: 'compact' })
      expect(wrapper.find('.content-card.flex').exists()).toBe(true)
    })

    it('should render featured variant', () => {
      const wrapper = mountComponent({ variant: 'featured' })
      const vm = wrapper.vm as any
      expect(vm.variant).toBe('featured')
    })
  })

  describe('Content Types', () => {
    it('should apply article content type', () => {
      const wrapper = mountComponent({ contentType: 'article' })
      const vm = wrapper.vm as any
      expect(vm.contentType).toBe('article')
    })

    it('should apply document content type', () => {
      const wrapper = mountComponent({ contentType: 'document' })
      const vm = wrapper.vm as any
      expect(vm.contentType).toBe('document')
    })

    it('should apply media content type', () => {
      const wrapper = mountComponent({ contentType: 'media' })
      const vm = wrapper.vm as any
      expect(vm.contentType).toBe('media')
    })

    it('should apply event content type', () => {
      const wrapper = mountComponent({ contentType: 'event' })
      const vm = wrapper.vm as any
      expect(vm.contentType).toBe('event')
    })

    it('should apply course content type', () => {
      const wrapper = mountComponent({ contentType: 'course' })
      const vm = wrapper.vm as any
      expect(vm.contentType).toBe('course')
    })
  })

  describe('Default Icons', () => {
    it('should return newspaper icon for article', () => {
      const wrapper = mountComponent({ contentType: 'article' })
      const vm = wrapper.vm as any
      expect(vm.defaultIcon).toBe('fas fa-newspaper')
    })

    it('should return file-alt icon for document', () => {
      const wrapper = mountComponent({ contentType: 'document' })
      const vm = wrapper.vm as any
      expect(vm.defaultIcon).toBe('fas fa-file-alt')
    })

    it('should return play-circle icon for media', () => {
      const wrapper = mountComponent({ contentType: 'media' })
      const vm = wrapper.vm as any
      expect(vm.defaultIcon).toBe('fas fa-play-circle')
    })

    it('should return calendar-alt icon for event', () => {
      const wrapper = mountComponent({ contentType: 'event' })
      const vm = wrapper.vm as any
      expect(vm.defaultIcon).toBe('fas fa-calendar-alt')
    })

    it('should return graduation-cap icon for course', () => {
      const wrapper = mountComponent({ contentType: 'course' })
      const vm = wrapper.vm as any
      expect(vm.defaultIcon).toBe('fas fa-graduation-cap')
    })

    it('should use custom icon when provided', () => {
      const wrapper = mountComponent({ icon: 'fas fa-star' })
      const vm = wrapper.vm as any
      expect(vm.defaultIcon).toBe('fas fa-star')
    })
  })

  describe('Fallback Gradient', () => {
    it('should return teal gradient for article', () => {
      const wrapper = mountComponent({ contentType: 'article' })
      const vm = wrapper.vm as any
      expect(vm.fallbackGradient).toContain('from-teal-500')
    })

    it('should return blue gradient for document', () => {
      const wrapper = mountComponent({ contentType: 'document' })
      const vm = wrapper.vm as any
      expect(vm.fallbackGradient).toContain('from-blue-500')
    })

    it('should return purple gradient for media', () => {
      const wrapper = mountComponent({ contentType: 'media' })
      const vm = wrapper.vm as any
      expect(vm.fallbackGradient).toContain('from-purple-500')
    })

    it('should return orange gradient for event', () => {
      const wrapper = mountComponent({ contentType: 'event' })
      const vm = wrapper.vm as any
      expect(vm.fallbackGradient).toContain('from-orange-500')
    })

    it('should return green gradient for course', () => {
      const wrapper = mountComponent({ contentType: 'course' })
      const vm = wrapper.vm as any
      expect(vm.fallbackGradient).toContain('from-green-500')
    })
  })

  describe('Format Number', () => {
    it('should format thousands with K', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatNumber(1500)).toBe('1.5K')
    })

    it('should format millions with M', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatNumber(2500000)).toBe('2.5M')
    })

    it('should return number as string for small numbers', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatNumber(500)).toBe('500')
    })
  })

  describe('Image', () => {
    it('should display image when provided', () => {
      const wrapper = mountComponent({ image: 'https://example.com/image.jpg' })
      const img = wrapper.find('img')
      expect(img.exists()).toBe(true)
      expect(img.attributes('src')).toBe('https://example.com/image.jpg')
    })

    it('should display fallback gradient when no image', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.bg-gradient-to-br').exists()).toBe(true)
    })
  })

  describe('Featured State', () => {
    it('should apply featured styling', () => {
      const wrapper = mountComponent({ featured: true })
      const vm = wrapper.vm as any
      expect(vm.cardClasses).toContain('border-teal-200')
    })

    it('should not apply featured styling when false', () => {
      const wrapper = mountComponent({ featured: false })
      const vm = wrapper.vm as any
      expect(vm.cardClasses).toContain('border-gray-100')
    })
  })

  describe('Clickable', () => {
    it('should apply cursor-pointer when clickable', () => {
      const wrapper = mountComponent({ clickable: true })
      const vm = wrapper.vm as any
      expect(vm.cardClasses).toContain('cursor-pointer')
    })

    it('should emit click event when clicked', async () => {
      const wrapper = mountComponent({ clickable: true })
      await wrapper.trigger('click')
      expect(wrapper.emitted('click')).toBeTruthy()
    })

    it('should not emit click when not clickable', async () => {
      const wrapper = mountComponent({ clickable: false })
      await wrapper.trigger('click')
      expect(wrapper.emitted('click')).toBeFalsy()
    })
  })

  describe('Action Events', () => {
    it('should emit bookmark event', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const mockEvent = { stopPropagation: vi.fn() }
      vm.handleAction('bookmark', mockEvent)
      expect(wrapper.emitted('bookmark')).toBeTruthy()
      expect(wrapper.emitted('action')).toBeTruthy()
    })

    it('should emit like event', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const mockEvent = { stopPropagation: vi.fn() }
      vm.handleAction('like', mockEvent)
      expect(wrapper.emitted('like')).toBeTruthy()
    })

    it('should emit share event', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const mockEvent = { stopPropagation: vi.fn() }
      vm.handleAction('share', mockEvent)
      expect(wrapper.emitted('share')).toBeTruthy()
    })

    it('should stop event propagation', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const mockEvent = { stopPropagation: vi.fn() }
      vm.handleAction('share', mockEvent)
      expect(mockEvent.stopPropagation).toHaveBeenCalled()
    })
  })

  describe('Author', () => {
    it('should display author name', () => {
      const wrapper = mountComponent({
        author: { name: 'John Doe', initials: 'JD' },
      })
      expect(wrapper.text()).toContain('John Doe')
    })

    it('should display author avatar', () => {
      const wrapper = mountComponent({
        author: { name: 'John', avatar: 'https://example.com/avatar.jpg' },
      })
      const avatar = wrapper.find('img[alt="John"]')
      expect(avatar.exists()).toBe(true)
    })
  })

  describe('Tags', () => {
    it('should display tags', () => {
      const wrapper = mountComponent({
        tags: ['football', 'sports', 'asia'],
      })
      const tagBadges = wrapper.findAllComponents({ name: 'TagBadge' })
      expect(tagBadges.length).toBeGreaterThan(0)
    })

    it('should limit tags by maxTags', () => {
      const wrapper = mountComponent({
        tags: ['tag1', 'tag2', 'tag3', 'tag4', 'tag5'],
        maxTags: 2,
      })
      const tagBadges = wrapper.findAllComponents({ name: 'TagBadge' })
      expect(tagBadges.length).toBe(2)
    })
  })

  describe('Stats', () => {
    it('should display views count', () => {
      const wrapper = mountComponent({
        stats: { views: 1500 },
      })
      expect(wrapper.text()).toContain('1.5K')
    })

    it('should display likes count', () => {
      const wrapper = mountComponent({
        stats: { likes: 250 },
      })
      expect(wrapper.text()).toContain('250')
    })

    it('should display comments count', () => {
      const wrapper = mountComponent({
        stats: { comments: 50 },
      })
      expect(wrapper.text()).toContain('50')
    })
  })

  describe('Image Aspect Ratios', () => {
    it('should use 16/10 aspect for article', () => {
      const wrapper = mountComponent({ contentType: 'article' })
      const vm = wrapper.vm as any
      expect(vm.imageContainerClasses).toContain('aspect-[16/10]')
    })

    it('should use square aspect for document', () => {
      const wrapper = mountComponent({ contentType: 'document' })
      const vm = wrapper.vm as any
      expect(vm.imageContainerClasses).toContain('aspect-[4/3]')
    })

    it('should use video aspect for media', () => {
      const wrapper = mountComponent({ contentType: 'media' })
      const vm = wrapper.vm as any
      expect(vm.imageContainerClasses).toContain('aspect-video')
    })
  })

  describe('Event Date', () => {
    it('should display event date badge', () => {
      const wrapper = mountComponent({
        eventDate: { day: 15, month: 'Jan' },
      })
      expect(wrapper.text()).toContain('15')
      expect(wrapper.text()).toContain('Jan')
    })
  })

  describe('Progress Bar', () => {
    it('should show progress bar for courses', () => {
      const wrapper = mountComponent({
        contentType: 'course',
        progress: 50,
      })
      expect(wrapper.find('.h-1.bg-gray-200').exists()).toBe(true)
    })

    it('should not show progress bar when progress is 0', () => {
      const wrapper = mountComponent({
        contentType: 'course',
        progress: 0,
      })
      expect(wrapper.find('.h-1.bg-gray-200').exists()).toBe(false)
    })
  })

  describe('Rating', () => {
    it('should display rating stars', () => {
      const wrapper = mountComponent({
        rating: 4.5,
      })
      expect(wrapper.findAll('.fa-star').length).toBeGreaterThan(0)
    })
  })
})
