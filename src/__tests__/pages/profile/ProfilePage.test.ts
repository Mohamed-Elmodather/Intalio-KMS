import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import ProfilePage from '@/pages/profile/ProfilePage.vue'

// Mock vue-router
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
  useRoute: () => ({
    query: {},
  }),
}))

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock components
vi.mock('@/components/common/LoadingSpinner.vue', () => ({
  default: { template: '<div class="loading-spinner"></div>' },
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AISuggestionChip: { template: '<span class="ai-suggestion"></span>' },
  AIConfidenceBar: { template: '<div class="ai-confidence"></div>' },
}))

// Mock stores
vi.mock('@/stores/aiServices', () => ({
  useAIServicesStore: () => ({
    isLoading: false,
    error: null,
  }),
}))

function mountComponent() {
  return shallowMount(ProfilePage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
      },
    },
  })
}

describe('ProfilePage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render profile page', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Loading State', () => {
    it('should have loading state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isLoading).toBe('boolean')
    })
  })

  describe('Modal State', () => {
    it('should have edit profile modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showEditProfile).toBe(false)
    })
  })

  describe('Activity Filter', () => {
    it('should have activity filter', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.activityFilter).toBe('all')
    })
  })

  describe('User Profile', () => {
    it('should have current user id', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.currentUserId).toBeDefined()
    })

    it('should compute is own profile', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isOwnProfile).toBe('boolean')
    })

    it('should have users database', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.usersDatabase).toBeDefined()
      expect(Array.isArray(vm.usersDatabase)).toBe(true)
    })
  })

  describe('User Data', () => {
    it('should have user with basic info', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const user = vm.usersDatabase[0]
      expect(user.id).toBeDefined()
      expect(user.name).toBeDefined()
      expect(user.initials).toBeDefined()
      expect(user.role).toBeDefined()
    })

    it('should have user with contact info', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const user = vm.usersDatabase[0]
      expect(user.email).toBeDefined()
      expect(user.phone).toBeDefined()
      expect(user.location).toBeDefined()
    })

    it('should have user with skills', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const user = vm.usersDatabase[0]
      expect(user.skills).toBeDefined()
      expect(Array.isArray(user.skills)).toBe(true)
      expect(user.skills.length).toBeGreaterThan(0)
    })

    it('should have user with stats', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const user = vm.usersDatabase[0]
      expect(user.stats).toBeDefined()
      expect(user.stats.articles).toBeDefined()
      expect(user.stats.comments).toBeDefined()
      expect(user.stats.documents).toBeDefined()
      expect(user.stats.courses).toBeDefined()
    })

    it('should have user with social counts', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const user = vm.usersDatabase[0]
      expect(user.followersCount).toBeDefined()
      expect(user.followingCount).toBeDefined()
      expect(user.mutualConnections).toBeDefined()
    })
  })

  describe('User Activities', () => {
    it('should have user activities', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const user = vm.usersDatabase[0]
      expect(user.activities).toBeDefined()
      expect(Array.isArray(user.activities)).toBe(true)
    })

    it('should have activity properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const activity = vm.usersDatabase[0].activities[0]
      expect(activity.id).toBeDefined()
      expect(activity.type).toBeDefined()
      expect(activity.title).toBeDefined()
      expect(activity.time).toBeDefined()
    })
  })

  describe('User Badges', () => {
    it('should have user badges', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const user = vm.usersDatabase[0]
      expect(user.badges).toBeDefined()
      expect(Array.isArray(user.badges)).toBe(true)
    })

    it('should have badge properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const badge = vm.usersDatabase[0].badges[0]
      expect(badge.id).toBeDefined()
      expect(badge.name).toBeDefined()
      expect(badge.icon).toBeDefined()
      expect(badge.description).toBeDefined()
    })
  })

  describe('User Authored Content', () => {
    it('should have authored content', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const user = vm.usersDatabase[0]
      expect(user.authoredContent).toBeDefined()
    })

    it('should have authored articles', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const content = vm.usersDatabase[0].authoredContent
      expect(content.articles).toBeDefined()
      expect(Array.isArray(content.articles)).toBe(true)
    })

    it('should have authored documents', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const content = vm.usersDatabase[0].authoredContent
      expect(content.documents).toBeDefined()
      expect(Array.isArray(content.documents)).toBe(true)
    })

    it('should have authored polls', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const content = vm.usersDatabase[0].authoredContent
      expect(content.polls).toBeDefined()
      expect(Array.isArray(content.polls)).toBe(true)
    })
  })

  describe('Team Members', () => {
    it('should have team members', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const user = vm.usersDatabase[0]
      expect(user.teamMembers).toBeDefined()
      expect(Array.isArray(user.teamMembers)).toBe(true)
    })

    it('should have team member properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const member = vm.usersDatabase[0].teamMembers[0]
      expect(member.id).toBeDefined()
      expect(member.name).toBeDefined()
      expect(member.initials).toBeDefined()
      expect(member.role).toBeDefined()
      expect(typeof member.online).toBe('boolean')
    })
  })

  describe('Contribution Data', () => {
    it('should have contribution data', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const user = vm.usersDatabase[0]
      expect(user.contributionData).toBeDefined()
      expect(Array.isArray(user.contributionData)).toBe(true)
      expect(user.contributionData.length).toBe(12)
    })
  })
})
