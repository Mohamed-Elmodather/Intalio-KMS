import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import RelatedContentCarousel from '@/components/common/RelatedContentCarousel.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock vue-router
const mockPush = vi.fn()
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: mockPush,
  }),
}))

// Mock related content composable
const mockRelatedItems = [
  { id: '1', type: 'article', title: 'Related Article 1', thumbnail: 'https://example.com/1.jpg', description: 'Description 1', metadata: '2 hours ago' },
  { id: '2', type: 'document', title: 'Related Document', thumbnail: null, description: 'Description 2', metadata: '1 day ago' },
  { id: '3', type: 'media', title: 'Related Video', thumbnail: 'https://example.com/3.jpg', description: '', metadata: '3 days ago' },
]

const mockLoadRelatedContent = vi.fn()
const mockLoadMixedContent = vi.fn()

vi.mock('@/composables/useRelatedContent', () => ({
  useRelatedContent: () => ({
    relatedItems: mockRelatedItems,
    isLoading: false,
    loadRelatedContent: mockLoadRelatedContent,
    loadMixedContent: mockLoadMixedContent,
    getContentTypeIcon: (type: string) => {
      const icons: Record<string, string> = {
        article: 'fas fa-newspaper',
        document: 'fas fa-file-alt',
        media: 'fas fa-play-circle',
      }
      return icons[type] || 'fas fa-file'
    },
    getContentTypeColor: (type: string) => `bg-${type}-100 text-${type}-600`,
    getContentTypeLabel: (type: string) => type.charAt(0).toUpperCase() + type.slice(1),
  }),
}))

function mountComponent(props = {}) {
  return shallowMount(RelatedContentCarousel, {
    props: {
      contentType: 'article',
      contentId: '123',
      ...props,
    },
    global: {
      mocks: {
        $t: (key: string) => key,
      },
    },
  })
}

describe('RelatedContentCarousel', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.related-content-carousel').exists()).toBe(true)
    })

    it('should render header', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('h3').exists()).toBe(true)
    })

    it('should display default title', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('common.relatedContent')
    })

    it('should display custom title', () => {
      const wrapper = mountComponent({ title: 'Similar Articles' })
      expect(wrapper.text()).toContain('Similar Articles')
    })
  })

  describe('Navigation Buttons', () => {
    it('should render scroll left button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-chevron-left').exists()).toBe(true)
    })

    it('should render scroll right button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-chevron-right').exists()).toBe(true)
    })

    it('should have scrollLeft function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.scrollLeft).toBe('function')
    })

    it('should have scrollRight function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.scrollRight).toBe('function')
    })
  })

  describe('Related Items', () => {
    it('should render related items', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Related Article 1')
      expect(wrapper.text()).toContain('Related Document')
      expect(wrapper.text()).toContain('Related Video')
    })

    it('should display item descriptions', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Description 1')
      expect(wrapper.text()).toContain('Description 2')
    })

    it('should display item metadata', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('2 hours ago')
      expect(wrapper.text()).toContain('1 day ago')
    })

    it('should display thumbnails when available', () => {
      const wrapper = mountComponent()
      const images = wrapper.findAll('img')
      expect(images.length).toBeGreaterThan(0)
    })
  })

  describe('Content Type Badges', () => {
    it('should display type badges', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Article')
      expect(wrapper.text()).toContain('Document')
      expect(wrapper.text()).toContain('Media')
    })
  })

  describe('Navigation', () => {
    it('should have navigateToItem function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.navigateToItem).toBe('function')
    })

    it('should navigate to article view', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.navigateToItem({ id: '1', type: 'article' })
      expect(mockPush).toHaveBeenCalledWith({ name: 'ArticleView', params: { id: '1' } })
    })

    it('should navigate to document view', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.navigateToItem({ id: '2', type: 'document' })
      expect(mockPush).toHaveBeenCalledWith({ name: 'DocumentView', params: { id: '2' } })
    })

    it('should navigate to media player', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.navigateToItem({ id: '3', type: 'media' })
      expect(mockPush).toHaveBeenCalledWith({ name: 'MediaPlayer', params: { id: '3' } })
    })

    it('should navigate to course view', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.navigateToItem({ id: '4', type: 'course' })
      expect(mockPush).toHaveBeenCalledWith({ name: 'CourseView', params: { id: '4' } })
    })

    it('should navigate to event detail', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.navigateToItem({ id: '5', type: 'event' })
      expect(mockPush).toHaveBeenCalledWith({ name: 'EventDetail', params: { id: '5' } })
    })

    it('should navigate to poll detail', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.navigateToItem({ id: '6', type: 'poll' })
      expect(mockPush).toHaveBeenCalledWith({ name: 'PollDetail', params: { id: '6' } })
    })
  })

  describe('Scroll Controls', () => {
    it('should track canScrollLeft state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.canScrollLeft).toBe('boolean')
    })

    it('should track canScrollRight state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.canScrollRight).toBe('boolean')
    })

    it('should have checkScrollButtons function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.checkScrollButtons).toBe('function')
    })
  })

  describe('Load Content', () => {
    it('should load related content on mount', () => {
      mountComponent()
      expect(mockLoadRelatedContent).toHaveBeenCalled()
    })

    it('should load mixed content when mixed prop is true', () => {
      mountComponent({ mixed: true })
      expect(mockLoadMixedContent).toHaveBeenCalled()
    })

    it('should respect limit prop', () => {
      mountComponent({ limit: 6 })
      expect(mockLoadRelatedContent).toHaveBeenCalledWith(6)
    })
  })

  describe('Default Props', () => {
    it('should have default limit of 4', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.limit).toBe(4)
    })

    it('should have mixed as false by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.mixed).toBe(false)
    })
  })

  describe('Fallback Icons', () => {
    it('should show fallback icon when no thumbnail', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-file-alt').exists()).toBe(true)
    })
  })

  describe('Card Styling', () => {
    it('should apply hover styling classes', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.hover\\:shadow-lg').exists()).toBe(true)
    })

    it('should apply cursor pointer for clickable items', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.cursor-pointer').exists()).toBe(true)
    })
  })
})
