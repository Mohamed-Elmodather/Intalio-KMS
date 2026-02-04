import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import DashboardPage from '@/pages/DashboardPage.vue'

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
    t: (key: string, params?: Record<string, any>) => {
      const translations: Record<string, string> = {
        'dashboard.greeting.morning': 'Good morning',
        'dashboard.greeting.afternoon': 'Good afternoon',
        'dashboard.greeting.evening': 'Good evening',
        'dashboard.youVotedFor': `You voted for ${params?.option || ''}`,
        'dashboard.selectOptionFirst': 'Please select an option first',
      }
      return translations[key] || key
    },
  }),
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AISuggestionChip: { template: '<div class="ai-suggestion"></div>' },
  AIConfidenceBar: { template: '<div class="ai-confidence"></div>' },
}))

// Mock ViewAllButton
vi.mock('@/components/common/ViewAllButton.vue', () => ({
  default: {
    template: '<button class="view-all-btn"><slot /></button>',
    props: ['to', 'text'],
  },
}))

// Mock AI services store
vi.mock('@/stores/aiServices', () => ({
  useAIServicesStore: () => ({
    isLoading: false,
    error: null,
  }),
}))

describe('DashboardPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render dashboard page', () => {
      const wrapper = mount(DashboardPage)
      expect(wrapper.exists()).toBe(true)
    })

    it('should display user greeting', () => {
      const wrapper = mount(DashboardPage)
      expect(wrapper.text()).toContain('Ahmed')
    })
  })

  describe('Current User', () => {
    it('should have current user data', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.currentUser).toBeDefined()
      expect(vm.currentUser.name).toBe('Ahmed')
      expect(vm.currentUser.fullName).toBe('Ahmed Imam')
      expect(vm.currentUser.role).toBe('Media Director')
    })

    it('should have user initials', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.currentUser.initials).toBe('AI')
    })
  })

  describe('Time of Day Greeting', () => {
    it('should compute time of day', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(['morning', 'afternoon', 'evening']).toContain(vm.timeOfDay)
    })
  })

  describe('Statistics', () => {
    it('should have stats array', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.stats).toBeDefined()
      expect(Array.isArray(vm.stats)).toBe(true)
      expect(vm.stats.length).toBe(4)
    })

    it('should have articles stat', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const articlesStat = vm.stats.find((s: any) => s.label === 'Total Articles')
      expect(articlesStat).toBeDefined()
      expect(articlesStat.numValue).toBe(2847)
    })

    it('should have active users stat', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const usersStat = vm.stats.find((s: any) => s.label === 'Active Users')
      expect(usersStat).toBeDefined()
      expect(usersStat.numValue).toBe(1234)
    })

    it('should have documents stat', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const docsStat = vm.stats.find((s: any) => s.label === 'Documents')
      expect(docsStat).toBeDefined()
      expect(docsStat.numValue).toBe(8492)
    })

    it('should have courses stat', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const coursesStat = vm.stats.find((s: any) => s.label === 'Courses Completed')
      expect(coursesStat).toBeDefined()
      expect(coursesStat.numValue).toBe(456)
    })

    it('should have trend information', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      vm.stats.forEach((stat: any) => {
        expect(stat.trend).toBeDefined()
        expect(typeof stat.trendUp).toBe('boolean')
      })
    })

    it('should format numbers with commas', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.formatNumber(1234)).toBe('1,234')
      expect(vm.formatNumber(1000000)).toBe('1,000,000')
    })
  })

  describe('Recent Articles', () => {
    it('should have recent articles', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.recentArticles).toBeDefined()
      expect(Array.isArray(vm.recentArticles)).toBe(true)
      expect(vm.recentArticles.length).toBe(4)
    })

    it('should have article properties', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const article = vm.recentArticles[0]
      expect(article.id).toBeDefined()
      expect(article.title).toBeDefined()
      expect(article.excerpt).toBeDefined()
      expect(article.category).toBeDefined()
      expect(article.readTime).toBeDefined()
      expect(article.author).toBeDefined()
    })

    it('should have author information', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const article = vm.recentArticles[0]
      expect(article.author.name).toBeDefined()
      expect(article.author.initials).toBeDefined()
    })
  })

  describe('Upcoming Events', () => {
    it('should have upcoming events', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.upcomingEvents).toBeDefined()
      expect(Array.isArray(vm.upcomingEvents)).toBe(true)
      expect(vm.upcomingEvents.length).toBe(4)
    })

    it('should have event properties', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const event = vm.upcomingEvents[0]
      expect(event.id).toBeDefined()
      expect(event.title).toBeDefined()
      expect(event.month).toBeDefined()
      expect(event.day).toBeDefined()
      expect(event.time).toBeDefined()
      expect(event.location).toBeDefined()
      expect(event.category).toBeDefined()
    })

    it('should have featured event', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const featuredEvent = vm.upcomingEvents.find((e: any) => e.isFeatured)
      expect(featuredEvent).toBeDefined()
    })

    it('should track registered events', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.registeredEvents).toBeDefined()
      expect(vm.registeredEvents.has(1)).toBe(true)
      expect(vm.registeredEvents.has(3)).toBe(true)
    })

    it('should toggle event registration', async () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.registeredEvents.has(2)).toBe(false)

      const mockEvent = { stopPropagation: vi.fn() }
      vm.toggleEventRegistration(2, mockEvent)

      expect(vm.registeredEvents.has(2)).toBe(true)

      vm.toggleEventRegistration(2, mockEvent)

      expect(vm.registeredEvents.has(2)).toBe(false)
    })

    it('should check if event is registered', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.isEventRegistered(1)).toBe(true)
      expect(vm.isEventRegistered(2)).toBe(false)
    })
  })

  describe('Active Polls', () => {
    it('should have active polls', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.activePolls).toBeDefined()
      expect(Array.isArray(vm.activePolls)).toBe(true)
      expect(vm.activePolls.length).toBe(2)
    })

    it('should have poll properties', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const poll = vm.activePolls[0]
      expect(poll.id).toBeDefined()
      expect(poll.question).toBeDefined()
      expect(poll.options).toBeDefined()
      expect(poll.totalVotes).toBeDefined()
      expect(poll.endsIn).toBeDefined()
    })

    it('should have poll options with votes', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const poll = vm.activePolls[0]
      expect(poll.options.length).toBeGreaterThan(0)
      poll.options.forEach((option: any) => {
        expect(option.label).toBeDefined()
        expect(option.votes).toBeDefined()
        expect(option.color).toBeDefined()
      })
    })

    it('should track selected poll options', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.selectedPollOptions).toBeDefined()
    })

    it('should select poll option', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      vm.selectPollOption(1, 'Saudi Arabia')

      expect(vm.selectedPollOptions[1]).toBe('Saudi Arabia')
    })

    it('should check if option is selected', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      vm.selectPollOption(1, 'Japan')

      expect(vm.isOptionSelected(1, 'Japan')).toBe(true)
      expect(vm.isOptionSelected(1, 'Saudi Arabia')).toBe(false)
    })

    it('should vote on poll', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const poll = vm.activePolls[0]
      vm.selectPollOption(poll.id, 'Saudi Arabia')

      // Mock alert
      const alertSpy = vi.spyOn(window, 'alert').mockImplementation(() => {})

      vm.votePoll(poll)

      expect(poll.hasVoted).toBe(true)
      expect(alertSpy).toHaveBeenCalled()

      alertSpy.mockRestore()
    })

    it('should navigate to poll on view results', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      // Use poll that already has voted
      const poll = vm.activePolls[1]
      expect(poll.hasVoted).toBe(true)

      vm.votePoll(poll)

      expect(mockPush).toHaveBeenCalledWith(`/polls/${poll.id}`)
    })

    it('should navigate to poll details', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      vm.viewPoll(1)

      expect(mockPush).toHaveBeenCalledWith('/polls/1')
    })
  })

  describe('Learning Courses', () => {
    it('should have learning courses', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.learningCourses).toBeDefined()
      expect(Array.isArray(vm.learningCourses)).toBe(true)
      expect(vm.learningCourses.length).toBeGreaterThan(0)
    })

    it('should have course properties', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const course = vm.learningCourses[0]
      expect(course.id).toBeDefined()
      expect(course.title).toBeDefined()
      expect(course.instructor).toBeDefined()
      expect(course.progress).toBeDefined()
      expect(course.lessonsCompleted).toBeDefined()
      expect(course.totalLessons).toBeDefined()
      expect(course.duration).toBeDefined()
    })

    it('should have course progress', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const course = vm.learningCourses[0]
      expect(course.progress).toBeGreaterThanOrEqual(0)
      expect(course.progress).toBeLessThanOrEqual(100)
    })

    it('should have bookmarked courses', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const bookmarkedCourses = vm.learningCourses.filter((c: any) => c.isBookmarked)
      expect(bookmarkedCourses.length).toBeGreaterThan(0)
    })
  })

  describe('Dynamic Counts', () => {
    it('should have pending tasks count', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.pendingTasksCount).toBeDefined()
      expect(vm.pendingTasksCount).toBe(3)
    })

    it('should have new updates count', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(vm.newUpdatesCount).toBeDefined()
      expect(vm.newUpdatesCount).toBe(5)
    })
  })

  describe('Counter Animation', () => {
    it('should have animate counter function', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      expect(typeof vm.animateCounter).toBe('function')
    })

    it('should animate stat counter', async () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const stat = { numValue: 100, displayValue: 0 }

      // Mock requestAnimationFrame to execute immediately
      const rafSpy = vi.spyOn(window, 'requestAnimationFrame').mockImplementation((callback) => {
        callback(performance.now() + 3000) // Simulate time passed
        return 0
      })

      vm.animateCounter(stat, 100)

      // After animation completes, displayValue should equal numValue
      expect(stat.displayValue).toBe(100)

      rafSpy.mockRestore()
    })
  })

  describe('Event Actions', () => {
    it('should set event reminder', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const alertSpy = vi.spyOn(window, 'alert').mockImplementation(() => {})
      const mockEvent = { stopPropagation: vi.fn() }

      vm.setEventReminder(1, mockEvent)

      expect(mockEvent.stopPropagation).toHaveBeenCalled()
      expect(alertSpy).toHaveBeenCalledWith('Reminder set for event #1')

      alertSpy.mockRestore()
    })

    it('should share event', () => {
      const wrapper = mount(DashboardPage)
      const vm = wrapper.vm as any

      const alertSpy = vi.spyOn(window, 'alert').mockImplementation(() => {})
      const mockEvent = { stopPropagation: vi.fn() }

      vm.shareEvent(1, mockEvent)

      expect(mockEvent.stopPropagation).toHaveBeenCalled()
      expect(alertSpy).toHaveBeenCalledWith('Share event #1')

      alertSpy.mockRestore()
    })
  })
})
