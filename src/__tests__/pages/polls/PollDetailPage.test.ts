import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { ref } from 'vue'
import PollDetailPage from '@/pages/polls/PollDetailPage.vue'

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

// Mock components
vi.mock('@/components/common', () => ({
  CommentsSection: { template: '<div class="comments"></div>' },
  SocialShareButtons: { template: '<div class="share"></div>' },
  RelatedContentCarousel: { template: '<div class="related"></div>' },
  AuthorCard: { template: '<div class="author"></div>' },
  BookmarkButton: { template: '<button class="bookmark"></button>' },
  AddToCollectionModal: { template: '<div class="collection-modal"></div>' },
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

vi.mock('@/composables/useRelatedContent', () => ({
  useRelatedContent: () => ({
    relatedItems: ref([]),
    isLoading: ref(false),
    loadRelatedContent: vi.fn(),
  }),
}))

vi.mock('@/composables/useSharing', () => ({
  useSharing: () => ({
    share: vi.fn(),
    copyLink: vi.fn(),
    shareToTwitter: vi.fn(),
    shareToLinkedIn: vi.fn(),
  }),
}))

function mountComponent() {
  return shallowMount(PollDetailPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
        'router-link': true,
      },
    },
  })
}

describe('PollDetailPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render poll detail page', () => {
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

    it('should have loading text', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.loadingPoll).toBeDefined()
    })

    it('should have status texts', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.statusActive).toBeDefined()
      expect(vm.textConstants.statusCompleted).toBeDefined()
    })

    it('should have voting texts', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.castYourVote).toBeDefined()
      expect(vm.textConstants.submitVote).toBeDefined()
    })

    it('should have navigation texts', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.back).toBeDefined()
      expect(vm.textConstants.polls).toBeDefined()
    })
  })

  describe('Poll State', () => {
    it('should have poll ref that starts as null', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // poll is null initially before async load
      expect(vm.poll).toBeNull()
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

  describe('Voting State', () => {
    it('should have selected option ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // selectedOption starts as null
      expect(vm.selectedOption).toBeNull()
    })

    it('should have is voting state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isVoting).toBe('boolean')
    })

    it('should not be voting initially', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isVoting).toBe(false)
    })
  })

  describe('Modal States', () => {
    it('should have show share modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showShareModal).toBe('boolean')
    })

    it('should have show share modal false initially', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showShareModal).toBe(false)
    })

    it('should have show add to collection state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showAddToCollection).toBe('boolean')
    })

    it('should have show add to collection false initially', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showAddToCollection).toBe(false)
    })

    it('should have show analytics state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showAnalytics).toBe('boolean')
    })
  })

  describe('Computed Properties', () => {
    it('should have poll id computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.pollId).toBe('1')
    })

    it('should have status config computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.statusConfig).toBeDefined()
    })

    it('should have time remaining computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // time remaining is null when poll is null
      expect(vm.timeRemaining).toBeNull()
    })

    it('should have can vote computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // canVote returns false when poll is null
      expect(vm.canVote).toBe(false)
    })

    it('should have can see results computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // canSeeResults returns false when poll is null
      expect(vm.canSeeResults).toBe(false)
    })

    it('should have winning option computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // winningOption is null when poll is null
      expect(vm.winningOption).toBeNull()
    })
  })

  describe('Voting Functions', () => {
    it('should have submit vote function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.submitVote).toBe('function')
    })

    it('should have select option function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.selectOption).toBe('function')
    })

    it('should have load poll function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.loadPoll).toBe('function')
    })
  })

  describe('Utility Functions', () => {
    it('should have format date function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.formatDate).toBe('function')
    })

    it('should have get bar color function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getBarColor).toBe('function')
    })

    it('should have go back function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.goBack).toBe('function')
    })

    it('should have export poll function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.exportPoll).toBe('function')
    })

    it('should have share poll function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.sharePoll).toBe('function')
    })
  })

  describe('Vote Distribution', () => {
    it('should have vote distribution computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.voteDistribution).toBeDefined()
      expect(Array.isArray(vm.voteDistribution)).toBe(true)
    })

    it('should have max votes computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.maxVotes).toBeDefined()
    })
  })

  describe('Related Content Composable', () => {
    it('should have related items from composable', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.relatedItems).toBeDefined()
    })
  })

  describe('Comments Composable', () => {
    it('should have comments from composable', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.comments).toBeDefined()
    })

    it('should have comments loading state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.commentsLoading).toBeDefined()
    })
  })
})
