import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { ref } from 'vue'
import CourseViewPage from '@/pages/learning/CourseViewPage.vue'

// Mock vue-router
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
    back: vi.fn(),
  }),
  useRoute: () => ({
    params: { id: '1' },
  }),
}))

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AIConfidenceBar: { template: '<div class="ai-confidence"></div>' },
  AISuggestionChip: { template: '<span class="ai-suggestion"></span>' },
}))

// Mock common components
vi.mock('@/components/common', () => ({
  CommentsSection: { template: '<div class="comments"></div>' },
  RatingStars: { template: '<div class="rating"></div>' },
  RelatedContentCarousel: { template: '<div class="related"></div>' },
  BookmarkButton: { template: '<button class="bookmark"></button>' },
  SocialShareButtons: { template: '<div class="share"></div>' },
}))

// Mock stores
vi.mock('@/stores/aiServices', () => ({
  useAIServicesStore: () => ({
    isLoading: false,
    error: null,
  }),
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
  return shallowMount(CourseViewPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
        'router-link': true,
      },
    },
  })
}

describe('CourseViewPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render course view page', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Sidebar Tab', () => {
    it('should have active sidebar tab as ai', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.activeSidebarTab).toBe('ai')
    })
  })

  describe('Certificate State', () => {
    it('should have show certificate preview state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showCertificatePreview).toBe(false)
    })
  })

  describe('Course ID', () => {
    it('should compute course id from route', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.courseId).toBe(1)
    })
  })

  describe('Course Data', () => {
    it('should have course data', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course).toBeDefined()
    })

    it('should have course title', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.title).toBeDefined()
    })

    it('should have course description', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.description).toBeDefined()
    })

    it('should have course instructor', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.instructor).toBeDefined()
    })

    it('should have course level', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.level).toBeDefined()
    })

    it('should have course duration', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.duration).toBeDefined()
    })

    it('should have course rating', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.rating).toBeDefined()
    })

    it('should have course students count', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.students).toBeDefined()
    })

    it('should have course progress', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.progress).toBeDefined()
    })

    it('should have course tags', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.tags).toBeDefined()
      expect(Array.isArray(vm.course.tags)).toBe(true)
    })

    it('should have course objectives', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.objectives).toBeDefined()
      expect(Array.isArray(vm.course.objectives)).toBe(true)
    })
  })

  describe('Syllabus', () => {
    it('should have course syllabus', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.syllabus).toBeDefined()
      expect(Array.isArray(vm.course.syllabus)).toBe(true)
    })

    it('should have lesson properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const lesson = vm.course.syllabus[0]
      expect(lesson.id).toBeDefined()
      expect(lesson.title).toBeDefined()
      expect(lesson.duration).toBeDefined()
      expect(typeof lesson.completed).toBe('boolean')
      expect(lesson.type).toBeDefined()
    })
  })

  describe('Lesson Progress', () => {
    it('should have completed lessons count', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.completedLessons).toBeDefined()
    })

    it('should have total lessons count', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.totalLessons).toBeDefined()
    })

    it('should have current lesson ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.currentLesson).toBeDefined()
    })
  })

  describe('Functions', () => {
    it('should have handle course rating function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleCourseRating).toBe('function')
    })
  })

  describe('Instructor Info', () => {
    it('should have instructor initials', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.instructorInitials).toBeDefined()
    })

    it('should have instructor bio', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.course.instructorBio).toBeDefined()
    })
  })

  describe('AI Features', () => {
    it('should have is generating summary state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isGeneratingSummary).toBe('boolean')
    })

    it('should have is extracting concepts state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isExtractingConcepts).toBe('boolean')
    })

    it('should have is generating quiz state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isGeneratingQuiz).toBe('boolean')
    })

    it('should have active AI tab', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.activeAITab).toBe('summary')
    })
  })

  describe('UI State', () => {
    it('should have show syllabus state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showSyllabus).toBe('boolean')
    })

    it('should have show AI sidebar state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showAISidebar).toBe('boolean')
    })
  })

  describe('Discussion', () => {
    it('should have discussion comments', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.discussionComments).toBeDefined()
    })
  })
})
