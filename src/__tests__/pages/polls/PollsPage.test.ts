import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import PollsPage from '@/pages/polls/PollsPage.vue'

// Mock vue-router
const mockPush = vi.fn()
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: mockPush,
  }),
}))

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock components
vi.mock('@/components/common/PageHeroHeader.vue', () => ({
  default: { template: '<div class="page-hero-header"><slot /></div>' },
}))
vi.mock('@/components/common/LoadingSpinner.vue', () => ({
  default: { template: '<div class="loading-spinner"><slot /></div>' },
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AISuggestionChip: { template: '<span class="ai-suggestion"></span>' },
  AISentimentBadge: { template: '<span class="ai-sentiment"></span>' },
}))

// Mock stores
vi.mock('@/stores/aiServices', () => ({
  useAIServicesStore: () => ({
    isLoading: false,
    error: null,
  }),
}))

describe('PollsPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render polls page', () => {
      const wrapper = mount(PollsPage)
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Core State', () => {
    it('should have loading state false', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.isLoading).toBe(false)
    })

    it('should have active tab as active', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.activeTab).toBe('active')
    })

    it('should have search query empty', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.searchQuery).toBe('')
    })

    it('should have view mode as grid', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.viewMode).toBe('grid')
    })

    it('should have sort by recent', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.sortBy).toBe('recent')
    })

    it('should have sort order desc', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.sortOrder).toBe('desc')
    })
  })

  describe('Text Constants', () => {
    it('should have text constants defined', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.textConstants).toBeDefined()
    })

    it('should have page title constant', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.textConstants.pageTitle).toBeDefined()
    })
  })

  describe('Stats', () => {
    it('should have stats object', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.stats).toBeDefined()
    })

    it('should have active polls count', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.stats.active).toBe(8)
    })

    it('should have responses count', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.stats.responses).toBe(2847)
    })

    it('should have created count', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.stats.created).toBe(45)
    })

    it('should have participation percentage', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.stats.participation).toBe(82)
    })
  })

  describe('Hero Stats', () => {
    it('should compute hero stats', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.heroStats).toBeDefined()
      expect(vm.heroStats.length).toBe(4)
    })
  })

  describe('Tabs', () => {
    it('should have tabs defined', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.tabs).toBeDefined()
      expect(vm.tabs.length).toBe(3)
    })

    it('should have active tab', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const tab = vm.tabs.find((t: any) => t.id === 'active')
      expect(tab).toBeDefined()
    })

    it('should have my-polls tab', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const tab = vm.tabs.find((t: any) => t.id === 'my-polls')
      expect(tab).toBeDefined()
    })

    it('should have completed tab', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const tab = vm.tabs.find((t: any) => t.id === 'completed')
      expect(tab).toBeDefined()
    })
  })

  describe('Sort Options', () => {
    it('should have sort options', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.sortOptions).toBeDefined()
      expect(vm.sortOptions.length).toBe(5)
    })

    it('should have recent sort option', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const option = vm.sortOptions.find((o: any) => o.value === 'recent')
      expect(option).toBeDefined()
    })

    it('should have popular sort option', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const option = vm.sortOptions.find((o: any) => o.value === 'popular')
      expect(option).toBeDefined()
    })

    it('should have votes sort option', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const option = vm.sortOptions.find((o: any) => o.value === 'votes')
      expect(option).toBeDefined()
    })

    it('should have ending sort option', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const option = vm.sortOptions.find((o: any) => o.value === 'ending')
      expect(option).toBeDefined()
    })
  })

  describe('Status Options', () => {
    it('should have status options', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.statusOptions).toBeDefined()
      expect(vm.statusOptions.length).toBe(4)
    })

    it('should have active status', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const status = vm.statusOptions.find((s: any) => s.id === 'active')
      expect(status).toBeDefined()
    })

    it('should have draft status', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const status = vm.statusOptions.find((s: any) => s.id === 'draft')
      expect(status).toBeDefined()
    })

    it('should have scheduled status', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const status = vm.statusOptions.find((s: any) => s.id === 'scheduled')
      expect(status).toBeDefined()
    })

    it('should have completed status', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const status = vm.statusOptions.find((s: any) => s.id === 'completed')
      expect(status).toBeDefined()
    })
  })

  describe('Featured Poll', () => {
    it('should have featured poll', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.featuredPoll).toBeDefined()
    })

    it('should have featured poll question', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.featuredPoll.question).toBeDefined()
    })

    it('should have featured poll options', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.featuredPoll.options).toBeDefined()
      expect(vm.featuredPoll.options.length).toBeGreaterThan(0)
    })

    it('should have featured poll total votes', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.featuredPoll.totalVotes).toBeGreaterThan(0)
    })

    it('should have featured poll hasVoted state', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(typeof vm.featuredPoll.hasVoted).toBe('boolean')
    })
  })

  describe('Trending Polls', () => {
    it('should have trending polls', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.trendingPolls).toBeDefined()
      expect(Array.isArray(vm.trendingPolls)).toBe(true)
    })

    it('should have trending poll properties', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const poll = vm.trendingPolls[0]
      expect(poll.id).toBeDefined()
      expect(poll.question).toBeDefined()
      expect(poll.totalVotes).toBeDefined()
      expect(poll.category).toBeDefined()
    })

    it('should have hot trending polls', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const hotPolls = vm.trendingPolls.filter((p: any) => p.isHot)
      expect(hotPolls.length).toBeGreaterThan(0)
    })
  })

  describe('Active Polls', () => {
    it('should have active polls', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.activePolls).toBeDefined()
      expect(Array.isArray(vm.activePolls)).toBe(true)
    })

    it('should have active poll properties', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const poll = vm.activePolls[0]
      expect(poll.id).toBeDefined()
      expect(poll.question).toBeDefined()
      expect(poll.category).toBeDefined()
      expect(poll.options).toBeDefined()
      expect(poll.totalVotes).toBeDefined()
    })

    it('should have poll with options', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      const poll = vm.activePolls[0]
      expect(poll.options.length).toBeGreaterThan(0)
      expect(poll.options[0].label).toBeDefined()
      expect(poll.options[0].percentage).toBeDefined()
    })
  })

  describe('Modal States', () => {
    it('should have quick poll modal state', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.showQuickPollModal).toBe(false)
    })

    it('should have status filter state', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.showStatusFilter).toBe(false)
    })

    it('should have sort dropdown state', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.showSortDropdown).toBe(false)
    })
  })

  describe('Filter State', () => {
    it('should have selected statuses empty', () => {
      const wrapper = mount(PollsPage)
      const vm = wrapper.vm as any
      expect(vm.selectedStatuses).toEqual([])
    })
  })
})
