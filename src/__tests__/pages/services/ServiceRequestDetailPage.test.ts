import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { ref } from 'vue'
import ServiceRequestDetailPage from '@/pages/services/ServiceRequestDetailPage.vue'

// Mock vue-router
const mockBack = vi.fn()
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
    back: mockBack,
  }),
  useRoute: () => ({
    params: { id: 'sr-123' },
  }),
}))

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock common components
vi.mock('@/components/common', () => ({
  CommentsSection: { template: '<div class="comments"></div>' },
  RatingStars: { template: '<div class="rating"></div>' },
  BookmarkButton: { template: '<button class="bookmark"></button>' },
}))

// Mock composables
vi.mock('@/composables/useComments', () => ({
  useComments: () => ({
    comments: ref([]),
    isLoading: ref(false),
    loadComments: vi.fn(),
    addComment: vi.fn(),
  }),
}))

vi.mock('@/composables/useRatings', () => ({
  useRatings: () => ({
    rating: ref(null),
    submitRating: vi.fn(),
    loadRating: vi.fn(),
  }),
}))

function mountComponent() {
  return shallowMount(ServiceRequestDetailPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
        Teleport: true,
        'router-link': true,
      },
    },
  })
}

describe('ServiceRequestDetailPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render service request detail page', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Loading State', () => {
    it('should have loading state true initially', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isLoading).toBe(true)
    })
  })

  describe('Request State', () => {
    it('should have request ref that starts as null', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.request).toBeNull()
    })
  })

  describe('Modal States', () => {
    it('should have show cancel modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showCancelModal).toBe(false)
    })

    it('should have show rating modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showRatingModal).toBe(false)
    })
  })

  describe('Message State', () => {
    it('should have new message field', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.newMessage).toBe('')
    })

    it('should have is sending message state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isSendingMessage).toBe(false)
    })
  })

  describe('Computed Properties', () => {
    it('should have request id computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.requestId).toBe('sr-123')
    })

    it('should have status config computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.statusConfig).toBeDefined()
    })

    it('should return default status config when no request', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.statusConfig.label).toBe('')
      expect(vm.statusConfig.class).toBe('')
    })

    it('should have priority config computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.priorityConfig).toBeDefined()
    })

    it('should return default priority config when no request', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.priorityConfig.label).toBe('')
      expect(vm.priorityConfig.class).toBe('')
    })

    it('should have current step index computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.currentStepIndex).toBe(0)
    })

    it('should have progress percentage computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.progressPercentage).toBe(0)
    })

    it('should have can cancel computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.canCancel).toBe(false)
    })

    it('should have can rate computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.canRate).toBe(false)
    })
  })

  describe('Functions', () => {
    it('should have load request function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.loadRequest).toBe('function')
    })

    it('should have send message function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.sendMessage).toBe('function')
    })

    it('should have cancel request function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.cancelRequest).toBe('function')
    })

    it('should have handle rating function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleRating).toBe('function')
    })

    it('should have format date function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.formatDate).toBe('function')
    })

    it('should have format relative time function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.formatRelativeTime).toBe('function')
    })

    it('should have get file icon function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getFileIcon).toBe('function')
    })

    it('should have get timeline icon function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getTimelineIcon).toBe('function')
    })

    it('should have go back function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.goBack).toBe('function')
    })
  })

  describe('Format Date Function', () => {
    it('should format date correctly', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const date = new Date('2025-02-01T10:30:00')
      const formatted = vm.formatDate(date)
      expect(formatted).toContain('2025')
    })
  })

  describe('Format Relative Time Function', () => {
    it('should format minutes ago', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const date = new Date(Date.now() - 30 * 60 * 1000) // 30 minutes ago
      const formatted = vm.formatRelativeTime(date)
      expect(formatted).toContain('m ago')
    })

    it('should format hours ago', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const date = new Date(Date.now() - 5 * 60 * 60 * 1000) // 5 hours ago
      const formatted = vm.formatRelativeTime(date)
      expect(formatted).toContain('h ago')
    })

    it('should format days ago', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const date = new Date(Date.now() - 3 * 24 * 60 * 60 * 1000) // 3 days ago
      const formatted = vm.formatRelativeTime(date)
      expect(formatted).toContain('d ago')
    })
  })

  describe('Get File Icon Function', () => {
    it('should return image icon for image type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getFileIcon('image')).toContain('fa-image')
    })

    it('should return pdf icon for pdf type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getFileIcon('pdf')).toContain('fa-file-pdf')
    })

    it('should return email icon for email type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getFileIcon('email')).toContain('fa-envelope')
    })

    it('should return default icon for unknown type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getFileIcon('unknown')).toContain('fa-file')
    })
  })

  describe('Get Timeline Icon Function', () => {
    it('should return correct icon for status change', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTimelineIcon('status_change')).toContain('fa-exchange-alt')
    })

    it('should return correct icon for comment', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTimelineIcon('comment')).toContain('fa-comment')
    })

    it('should return correct icon for attachment', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTimelineIcon('attachment')).toContain('fa-paperclip')
    })

    it('should return correct icon for assignment', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTimelineIcon('assignment')).toContain('fa-user-check')
    })

    it('should return default icon for unknown type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTimelineIcon('unknown')).toContain('fa-circle')
    })
  })

  describe('Go Back Function', () => {
    it('should call router back', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.goBack()
      expect(mockBack).toHaveBeenCalled()
    })
  })

  describe('Comments Composable', () => {
    it('should have comments from composable', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.comments).toBeDefined()
      expect(Array.isArray(vm.comments)).toBe(true)
    })

    it('should have comments loading state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.commentsLoading).toBe('boolean')
    })

    it('should have load comments function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.loadComments).toBe('function')
    })

    it('should have add comment function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.addComment).toBe('function')
    })
  })

  describe('Ratings Composable', () => {
    it('should have rating from composable', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.rating).toBeDefined()
    })

    it('should have submit rating function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.submitRating).toBe('function')
    })
  })
})
