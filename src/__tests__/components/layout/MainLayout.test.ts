import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import MainLayout from '@/components/layout/MainLayout.vue'

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

// Mock stores
const mockUser = {
  displayName: 'John Doe',
  email: 'john@example.com',
  jobTitle: 'Developer',
  role: 'admin',
  avatar: null,
}

const mockToggleSidebar = vi.fn()
const mockLogout = vi.fn()
const mockCheckAuth = vi.fn()
const mockMarkAllAsRead = vi.fn()
const mockFetchUnreadCount = vi.fn()

let mockSidebarCollapsed = false

vi.mock('@/stores/auth', () => ({
  useAuthStore: () => ({
    user: mockUser,
    logout: mockLogout,
    checkAuth: mockCheckAuth,
  }),
}))

vi.mock('@/stores/ui', () => ({
  useUIStore: () => ({
    toggleSidebar: mockToggleSidebar,
    sidebarCollapsed: mockSidebarCollapsed,
  }),
}))

vi.mock('@/stores/notifications', () => ({
  useNotificationsStore: () => ({
    unreadCount: 3,
    recentNotifications: [
      { id: '1', title: 'Notification 1', message: 'Message 1', type: 'info', icon: 'fas fa-bell', isRead: false },
    ],
    markAllAsRead: mockMarkAllAsRead,
    fetchUnreadCount: mockFetchUnreadCount,
  }),
}))

function mountComponent() {
  return shallowMount(MainLayout, {
    global: {
      mocks: {
        $t: (key: string) => key,
      },
      stubs: {
        UnifiedHeader: true,
        UnifiedSidebar: true,
        AppToast: true,
      },
    },
    slots: {
      default: '<div class="test-content">Test Content</div>',
    },
  })
}

describe('MainLayout', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockSidebarCollapsed = false
  })

  describe('Rendering', () => {
    it('should render the layout', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.elegant-bg').exists()).toBe(true)
    })

    it('should render UnifiedHeader', () => {
      const wrapper = mountComponent()
      expect(wrapper.findComponent({ name: 'UnifiedHeader' }).exists()).toBe(true)
    })

    it('should render UnifiedSidebar', () => {
      const wrapper = mountComponent()
      expect(wrapper.findComponent({ name: 'UnifiedSidebar' }).exists()).toBe(true)
    })

    it('should render AppToast', () => {
      const wrapper = mountComponent()
      expect(wrapper.findComponent({ name: 'AppToast' }).exists()).toBe(true)
    })

    it('should render main content area', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('main').exists()).toBe(true)
    })

    it('should render slot content', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.test-content').exists()).toBe(true)
    })

    it('should render particles background', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.particles').exists()).toBe(true)
    })
  })

  describe('User Computed', () => {
    it('should compute user from authStore', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.user).toBeDefined()
      expect(vm.user.displayName).toBe('John Doe')
    })

    it('should include isOnline in user object', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.user.isOnline).toBe(true)
    })
  })

  describe('Navigation Items', () => {
    it('should have navigationItems computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.navigationItems).toBeDefined()
      expect(Array.isArray(vm.navigationItems)).toBe(true)
    })

    it('should have dashboard in navigation', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const dashboard = vm.navigationItems.find((item: any) => item.id === 'dashboard')
      expect(dashboard).toBeDefined()
    })

    it('should have collections in navigation', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const collections = vm.navigationItems.find((item: any) => item.id === 'collections')
      expect(collections).toBeDefined()
    })

    it('should have analytics in navigation', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const analytics = vm.navigationItems.find((item: any) => item.id === 'analytics')
      expect(analytics).toBeDefined()
    })
  })

  describe('Workspace Items', () => {
    it('should have workspaceItems computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.workspaceItems).toBeDefined()
      expect(vm.workspaceItems.length).toBe(3)
    })
  })

  describe('Bottom Actions', () => {
    it('should have bottomActions computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.bottomActions).toBeDefined()
      expect(vm.bottomActions.length).toBe(2)
    })

    it('should have settings action', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const settings = vm.bottomActions.find((action: any) => action.id === 'settings')
      expect(settings).toBeDefined()
      expect(settings.hoverEffect).toBe('rotate')
    })

    it('should have AI assistant action', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const ai = vm.bottomActions.find((action: any) => action.id === 'ai-assistant')
      expect(ai).toBeDefined()
      expect(ai.hoverEffect).toBe('scale')
    })
  })

  describe('Create Menu Items', () => {
    it('should have createMenuItems computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.createMenuItems).toBeDefined()
      expect(vm.createMenuItems.length).toBe(4)
    })

    it('should have article create item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const article = vm.createMenuItems.find((item: any) => item.id === 'article')
      expect(article).toBeDefined()
    })

    it('should have poll create item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const poll = vm.createMenuItems.find((item: any) => item.id === 'poll')
      expect(poll).toBeDefined()
    })

    it('should have event create item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const event = vm.createMenuItems.find((item: any) => item.id === 'event')
      expect(event).toBeDefined()
    })

    it('should have document create item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const doc = vm.createMenuItems.find((item: any) => item.id === 'document')
      expect(doc).toBeDefined()
    })
  })

  describe('Notifications', () => {
    it('should have notifications computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.notifications).toBeDefined()
      expect(Array.isArray(vm.notifications)).toBe(true)
    })

    it('should have unreadCount computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.unreadCount).toBe(3)
    })
  })

  describe('Header Actions', () => {
    it('should have headerActions computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.headerActions).toBeDefined()
    })

    it('should have messages action', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const messages = vm.headerActions.find((action: any) => action.id === 'messages')
      expect(messages).toBeDefined()
      expect(messages.badge).toBe(3)
    })
  })

  describe('Event Handlers', () => {
    it('should have handleToggleSidebar function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleToggleSidebar).toBe('function')
    })

    it('should call toggleSidebar on handleToggleSidebar', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleToggleSidebar()
      expect(mockToggleSidebar).toHaveBeenCalled()
    })

    it('should have handleSearch function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleSearch).toBe('function')
    })

    it('should navigate on handleSearch', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleSearch('test query')
      expect(mockPush).toHaveBeenCalledWith({
        name: 'SearchResults',
        query: { q: 'test query' },
      })
    })

    it('should have handleAiClick function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleAiClick).toBe('function')
    })

    it('should navigate to AI assistant on handleAiClick', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleAiClick()
      expect(mockPush).toHaveBeenCalledWith({ name: 'AIAssistant' })
    })

    it('should have handleCreateItemClick function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleCreateItemClick).toBe('function')
    })

    it('should navigate to ArticleCreate on article click', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleCreateItemClick('article')
      expect(mockPush).toHaveBeenCalledWith({ name: 'ArticleCreate' })
    })

    it('should navigate to PollCreate on poll click', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleCreateItemClick('poll')
      expect(mockPush).toHaveBeenCalledWith({ name: 'PollCreate' })
    })

    it('should navigate to Events on event click', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleCreateItemClick('event')
      expect(mockPush).toHaveBeenCalledWith({ name: 'Events' })
    })

    it('should navigate to Documents on document click', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleCreateItemClick('document')
      expect(mockPush).toHaveBeenCalledWith({ name: 'Documents' })
    })

    it('should not navigate for unknown create item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      mockPush.mockClear()
      vm.handleCreateItemClick('unknown')
      expect(mockPush).not.toHaveBeenCalled()
    })

    it('should have handleMarkAllRead function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleMarkAllRead).toBe('function')
    })

    it('should call markAllAsRead on handleMarkAllRead', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleMarkAllRead()
      expect(mockMarkAllAsRead).toHaveBeenCalled()
    })

    it('should have handleViewAllNotifications function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleViewAllNotifications).toBe('function')
    })

    it('should navigate to notifications on handleViewAllNotifications', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleViewAllNotifications()
      expect(mockPush).toHaveBeenCalledWith('/notifications')
    })

    it('should have handleProfileClick function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleProfileClick).toBe('function')
    })

    it('should navigate to Profile on handleProfileClick', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleProfileClick()
      expect(mockPush).toHaveBeenCalledWith({ name: 'Profile' })
    })

    it('should have handleSettingsClick function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleSettingsClick).toBe('function')
    })

    it('should navigate to Settings on handleSettingsClick', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleSettingsClick()
      expect(mockPush).toHaveBeenCalledWith({ name: 'Settings' })
    })

    it('should have handleLogout function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleLogout).toBe('function')
    })

    it('should logout and navigate to Login', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      await vm.handleLogout()
      expect(mockLogout).toHaveBeenCalled()
      expect(mockPush).toHaveBeenCalledWith({ name: 'Login' })
    })
  })

  describe('Sidebar Collapsed State', () => {
    it('should compute isCollapsed from store', () => {
      mockSidebarCollapsed = false
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isCollapsed).toBe(false)
    })

    it('should apply collapsed class to sidebar', () => {
      mockSidebarCollapsed = true
      const wrapper = mountComponent()
      const sidebar = wrapper.find('aside')
      expect(sidebar.classes()).toContain('w-20')
    })

    it('should apply expanded class to sidebar when not collapsed', () => {
      mockSidebarCollapsed = false
      const wrapper = mountComponent()
      const sidebar = wrapper.find('aside')
      expect(sidebar.classes()).toContain('w-64')
    })
  })

  describe('Lifecycle', () => {
    it('should check auth on mount', async () => {
      mountComponent()
      // Wait for onMounted to complete
      await vi.waitFor(() => {
        expect(mockCheckAuth).toHaveBeenCalled()
      })
    })

    it('should fetch unread count on mount', async () => {
      mountComponent()
      await vi.waitFor(() => {
        expect(mockFetchUnreadCount).toHaveBeenCalled()
      })
    })
  })

  describe('Layout Structure', () => {
    it('should have fixed sidebar', () => {
      const wrapper = mountComponent()
      const sidebar = wrapper.find('aside')
      expect(sidebar.classes()).toContain('fixed')
    })

    it('should have main content with correct margins', () => {
      const wrapper = mountComponent()
      const main = wrapper.find('main')
      expect(main.classes()).toContain('layout-main')
    })
  })
})
