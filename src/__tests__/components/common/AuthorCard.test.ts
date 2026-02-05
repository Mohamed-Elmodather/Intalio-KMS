import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AuthorCard from '@/components/common/AuthorCard.vue'

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

const mockAuthor = {
  id: '1',
  name: 'John Doe',
  initials: 'JD',
  role: 'Senior Editor',
  department: 'Sports',
  bio: 'Experienced sports journalist',
  avatar: null,
  articlesCount: 25,
  followersCount: 150,
  isFollowing: false,
}

function mountComponent(props = {}) {
  return shallowMount(AuthorCard, {
    props: {
      author: mockAuthor,
      ...props,
    },
    global: {
      mocks: {
        $t: (key: string) => key,
      },
    },
  })
}

describe('AuthorCard', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })

    it('should render author name', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('John Doe')
    })

    it('should render author role', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Senior Editor')
    })
  })

  describe('Variants', () => {
    it('should render full variant by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.author-card').exists()).toBe(true)
    })

    it('should render inline variant', () => {
      const wrapper = mountComponent({ variant: 'inline' })
      expect(wrapper.find('.flex.items-center.gap-3').exists()).toBe(true)
    })

    it('should render compact variant', () => {
      const wrapper = mountComponent({ variant: 'compact' })
      expect(wrapper.find('.flex.items-center.gap-4.p-4.bg-gray-50').exists()).toBe(true)
    })
  })

  describe('Avatar', () => {
    it('should show initials when no avatar', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('JD')
    })

    it('should show avatar image when provided', () => {
      const wrapper = mountComponent({
        author: { ...mockAuthor, avatar: 'https://example.com/avatar.jpg' },
      })
      const img = wrapper.find('img')
      expect(img.exists()).toBe(true)
      expect(img.attributes('src')).toBe('https://example.com/avatar.jpg')
    })
  })

  describe('Initials Color', () => {
    it('should have getInitialsColor function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getInitialsColor).toBe('function')
    })

    it('should return gradient color class', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const color = vm.getInitialsColor('JD')
      expect(color).toContain('from-')
      expect(color).toContain('to-')
    })
  })

  describe('Follow Button', () => {
    it('should show follow button when showFollow is true', () => {
      const wrapper = mountComponent({ showFollow: true })
      expect(wrapper.text()).toContain('user.follow')
    })

    it('should not show follow button when showFollow is false', () => {
      const wrapper = mountComponent({ showFollow: false })
      // Check that the follow button text (not followERS in stats) is not shown
      const buttons = wrapper.findAll('button')
      const followButton = buttons.filter(btn => {
        const text = btn.text()
        return text.includes('user.follow') && !text.includes('user.followers')
      })
      expect(followButton.length).toBe(0)
    })

    it('should show following state when isFollowing is true', () => {
      const wrapper = mountComponent({
        author: { ...mockAuthor, isFollowing: true },
      })
      expect(wrapper.text()).toContain('user.following')
    })

    it('should toggle follow state', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isFollowing).toBe(false)
      await vm.toggleFollow()
      expect(vm.isFollowing).toBe(true)
    })

    it('should show loading state during follow toggle', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const promise = vm.toggleFollow()
      expect(vm.isLoading).toBe(true)
      await promise
      expect(vm.isLoading).toBe(false)
    })
  })

  describe('Stats', () => {
    it('should show stats when showStats is true', () => {
      const wrapper = mountComponent({ showStats: true })
      expect(wrapper.text()).toContain('25')
      expect(wrapper.text()).toContain('150')
    })

    it('should not show stats when showStats is false', () => {
      const wrapper = mountComponent({ showStats: false })
      expect(wrapper.text()).not.toContain('nav.articles')
    })

    it('should show articles count', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('25')
    })

    it('should show followers count', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('150')
    })
  })

  describe('Bio', () => {
    it('should show bio when showBio is true', () => {
      const wrapper = mountComponent({ showBio: true })
      expect(wrapper.text()).toContain('Experienced sports journalist')
    })

    it('should not show bio when showBio is false', () => {
      const wrapper = mountComponent({ showBio: false })
      expect(wrapper.text()).not.toContain('Experienced sports journalist')
    })
  })

  describe('Department', () => {
    it('should show department when provided', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Sports')
    })
  })

  describe('Navigation', () => {
    it('should have viewProfile function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.viewProfile).toBe('function')
    })

    it('should navigate to profile on viewProfile', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.viewProfile()
      expect(mockPush).toHaveBeenCalledWith({ name: 'Profile', params: { id: '1' } })
    })
  })

  describe('View Profile Link', () => {
    it('should show view profile link', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('user.viewProfile')
    })
  })
})
