import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { ref } from 'vue'
import MediaPlayerPage from '@/pages/media/MediaPlayerPage.vue'

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
}))

// Mock common components
vi.mock('@/components/common', () => ({
  CommentsSection: { template: '<div class="comments"></div>' },
  RatingStars: { template: '<div class="rating"></div>' },
  SocialShareButtons: { template: '<div class="share"></div>' },
  RelatedContentCarousel: { template: '<div class="related"></div>' },
  BookmarkButton: { template: '<button class="bookmark"></button>' },
  AddToCollectionModal: { template: '<div class="collection-modal"></div>' },
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
  return shallowMount(MediaPlayerPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
        'router-link': true,
      },
    },
  })
}

describe('MediaPlayerPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render media player page', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Text Constants', () => {
    it('should have text constants defined', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants).toBeDefined()
    })

    it('should have navigation text', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.back).toBeDefined()
      expect(vm.textConstants.mediaCenter).toBeDefined()
    })

    it('should have player text', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.video).toBeDefined()
      expect(vm.textConstants.audio).toBeDefined()
      expect(vm.textConstants.image).toBeDefined()
      expect(vm.textConstants.gallery).toBeDefined()
    })

    it('should have AI feature text', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.aiFeatures).toBeDefined()
      expect(vm.textConstants.transcript).toBeDefined()
    })
  })

  describe('Loading State', () => {
    it('should have loading state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isLoading).toBe('boolean')
    })

    it('should be loading initially', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isLoading).toBe(true)
    })
  })

  describe('Media State', () => {
    it('should have media ref that starts as null', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // media is null initially before async load
      expect(vm.media).toBeNull()
    })
  })

  describe('Player State', () => {
    it('should have is playing state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isPlaying).toBe('boolean')
    })

    it('should have current time state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.currentTime).toBeDefined()
    })

    it('should have duration state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.duration).toBeDefined()
    })

    it('should have volume state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.volume).toBeDefined()
    })

    it('should have is muted state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isMuted).toBe('boolean')
    })

    it('should have playback speed state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.playbackSpeed).toBeDefined()
    })
  })

  describe('Modal States', () => {
    it('should have show add to collection state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showAddToCollection).toBe('boolean')
    })

    it('should have show transcript state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showTranscript).toBe('boolean')
    })
  })

  describe('Media Type Computed', () => {
    it('should have is video computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isVideo).toBe('boolean')
    })

    it('should have is audio computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isAudio).toBe('boolean')
    })

    it('should have is image computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isImage).toBe('boolean')
    })

    it('should have is gallery computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isGallery).toBe('boolean')
    })

    it('should have has playback controls computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.hasPlaybackControls).toBe('boolean')
    })
  })

  describe('Image Viewer State', () => {
    it('should have image zoom state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.imageZoom).toBeDefined()
    })

    it('should have image pan state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.imagePan).toBeDefined()
    })

    it('should have is annotating state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isAnnotating).toBe('boolean')
    })
  })

  describe('Comments and Ratings', () => {
    it('should have comments from composable', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.comments).toBeDefined()
    })

    it('should have rating from composable', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.rating).toBeDefined()
    })

    it('should have handle rating function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleRating).toBe('function')
    })
  })
})
