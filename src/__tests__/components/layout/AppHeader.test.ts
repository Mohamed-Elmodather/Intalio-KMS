import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AppHeader from '@/components/layout/AppHeader.vue'

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
const mockMarkAsRead = vi.fn()
const mockMarkAllAsRead = vi.fn()
const mockFetchUnreadCount = vi.fn()

vi.mock('@/stores/auth', () => ({
  useAuthStore: () => ({
    user: mockUser,
    logout: mockLogout,
  }),
}))

vi.mock('@/stores/ui', () => ({
  useUIStore: () => ({
    toggleSidebar: mockToggleSidebar,
    sidebarCollapsed: false,
  }),
}))

vi.mock('@/stores/notifications', () => ({
  useNotificationsStore: () => ({
    unreadCount: 5,
    recentNotifications: [
      { id: '1', title: 'Test Notification', message: 'Test message', type: 'info', icon: 'fas fa-bell', isRead: false },
      { id: '2', title: 'Read Notification', message: 'Already read', type: 'success', icon: 'fas fa-check', isRead: true },
    ],
    markAsRead: mockMarkAsRead,
    markAllAsRead: mockMarkAllAsRead,
    fetchUnreadCount: mockFetchUnreadCount,
  }),
}))

function mountComponent() {
  return shallowMount(AppHeader, {
    global: {
      mocks: {
        $t: (key: string) => key,
      },
      stubs: {
        'router-link': true,
        Transition: true,
        ViewAllButton: true,
      },
    },
  })
}

describe('AppHeader', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the header', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('header').exists()).toBe(true)
    })

    it('should render sidebar toggle button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-bars').exists()).toBe(true)
    })

    it('should render search input', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('input[type="text"]').exists()).toBe(true)
    })

    it('should render AI assistant button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-wand-magic-sparkles').exists()).toBe(true)
    })

    it('should render create button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-plus').exists()).toBe(true)
    })

    it('should render notifications button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-bell').exists()).toBe(true)
    })

    it('should render user menu area', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('John Doe')
    })
  })

  describe('User Initials', () => {
    it('should compute user initials from display name', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.userInitials).toBe('JD')
    })
  })

  describe('Notification Badge', () => {
    it('should display unread count', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('5')
    })
  })

  describe('Search', () => {
    it('should have searchQuery ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.searchQuery).toBe('')
    })

    it('should have handleSearch function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleSearch).toBe('function')
    })

    it('should navigate to search results on search', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.searchQuery = 'test query'
      vm.handleSearch()
      expect(mockPush).toHaveBeenCalledWith({
        name: 'SearchResults',
        query: { q: 'test query' },
      })
    })

    it('should not search with empty query', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.searchQuery = '   '
      vm.handleSearch()
      expect(mockPush).not.toHaveBeenCalled()
    })

    it('should clear search query after search', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.searchQuery = 'test'
      vm.handleSearch()
      expect(vm.searchQuery).toBe('')
    })
  })

  describe('AI Assistant', () => {
    it('should have openAIAssistant function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.openAIAssistant).toBe('function')
    })

    it('should navigate to AI Assistant', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.openAIAssistant()
      expect(mockPush).toHaveBeenCalledWith({ name: 'AIAssistant' })
    })
  })

  describe('Create Menu', () => {
    it('should have showCreateMenu state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showCreateMenu).toBe(false)
    })

    it('should toggle create menu', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.toggleCreate()
      expect(vm.showCreateMenu).toBe(true)
      vm.toggleCreate()
      expect(vm.showCreateMenu).toBe(false)
    })

    it('should close other menus when opening create menu', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showNotifications = true
      vm.showUserMenu = true
      vm.toggleCreate()
      expect(vm.showNotifications).toBe(false)
      expect(vm.showUserMenu).toBe(false)
    })
  })

  describe('Notifications Menu', () => {
    it('should have showNotifications state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showNotifications).toBe(false)
    })

    it('should toggle notifications menu', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.toggleNotifications()
      expect(vm.showNotifications).toBe(true)
    })

    it('should close other menus when opening notifications', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showCreateMenu = true
      vm.showUserMenu = true
      vm.toggleNotifications()
      expect(vm.showCreateMenu).toBe(false)
      expect(vm.showUserMenu).toBe(false)
    })

    it('should have markAllRead function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.markAllRead).toBe('function')
    })

    it('should call store markAllAsRead', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.markAllRead()
      expect(mockMarkAllAsRead).toHaveBeenCalled()
    })

    it('should have viewAllNotifications function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.viewAllNotifications).toBe('function')
    })

    it('should navigate to dashboard on viewAllNotifications', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.viewAllNotifications()
      expect(vm.showNotifications).toBe(false)
      expect(mockPush).toHaveBeenCalledWith({ name: 'Dashboard' })
    })
  })

  describe('User Menu', () => {
    it('should have showUserMenu state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showUserMenu).toBe(false)
    })

    it('should toggle user menu', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.toggleUserMenu()
      expect(vm.showUserMenu).toBe(true)
    })

    it('should close other menus when opening user menu', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showCreateMenu = true
      vm.showNotifications = true
      vm.toggleUserMenu()
      expect(vm.showCreateMenu).toBe(false)
      expect(vm.showNotifications).toBe(false)
    })
  })

  describe('Navigation', () => {
    it('should have navigateTo function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.navigateTo).toBe('function')
    })

    it('should navigate and close menus', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showCreateMenu = true
      vm.showUserMenu = true
      vm.navigateTo('Profile')
      expect(mockPush).toHaveBeenCalledWith({ name: 'Profile' })
      expect(vm.showCreateMenu).toBe(false)
      expect(vm.showUserMenu).toBe(false)
    })
  })

  describe('Logout', () => {
    it('should have handleLogout function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleLogout).toBe('function')
    })

    it('should logout and navigate to login', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      await vm.handleLogout()
      expect(mockLogout).toHaveBeenCalled()
      expect(mockPush).toHaveBeenCalledWith({ name: 'Login' })
    })
  })

  describe('Sidebar Toggle', () => {
    it('should call toggleSidebar on button click', async () => {
      const wrapper = mountComponent()
      const toggleBtn = wrapper.find('button')
      await toggleBtn.trigger('click')
      expect(mockToggleSidebar).toHaveBeenCalled()
    })
  })

  describe('Messages', () => {
    it('should have openMessages function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.openMessages).toBe('function')
    })

    it('should navigate to Collaboration on openMessages', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.openMessages()
      expect(mockPush).toHaveBeenCalledWith({ name: 'Collaboration' })
    })
  })

  describe('Notification Click Handler', () => {
    it('should have handleNotificationClick function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleNotificationClick).toBe('function')
    })

    it('should mark notification as read if unread', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleNotificationClick({ id: '1', isRead: false })
      expect(mockMarkAsRead).toHaveBeenCalledWith('1')
    })

    it('should not mark notification if already read', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleNotificationClick({ id: '2', isRead: true })
      expect(mockMarkAsRead).not.toHaveBeenCalled()
    })

    it('should navigate if notification has link', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showNotifications = true
      vm.handleNotificationClick({ id: '1', isRead: true, link: '/articles/1' })
      expect(mockPush).toHaveBeenCalledWith('/articles/1')
      expect(vm.showNotifications).toBe(false)
    })

    it('should not navigate if no link', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleNotificationClick({ id: '1', isRead: true })
      expect(mockPush).not.toHaveBeenCalled()
    })
  })

  describe('Close Dropdowns', () => {
    it('should have closeDropdowns function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.closeDropdowns).toBe('function')
    })

    it('should close all dropdowns when clicking outside', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showCreateMenu = true
      vm.showNotifications = true
      vm.showUserMenu = true

      const mockEvent = {
        target: {
          closest: vi.fn(() => null),
        },
      }
      vm.closeDropdowns(mockEvent as any)

      expect(vm.showCreateMenu).toBe(false)
      expect(vm.showNotifications).toBe(false)
      expect(vm.showUserMenu).toBe(false)
    })

    it('should not close dropdowns when clicking on trigger', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showCreateMenu = true

      const mockEvent = {
        target: {
          closest: vi.fn(() => document.createElement('div')),
        },
      }
      vm.closeDropdowns(mockEvent as any)

      expect(vm.showCreateMenu).toBe(true)
    })
  })

  describe('Computed Properties', () => {
    it('should compute user from authStore', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.user).toEqual(mockUser)
    })

    it('should compute unreadCount from notificationsStore', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.unreadCount).toBe(5)
    })

    it('should compute recentNotifications from notificationsStore', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.recentNotifications.length).toBe(2)
    })
  })

  describe('Lifecycle', () => {
    it('should fetch unread count on mount', () => {
      mountComponent()
      expect(mockFetchUnreadCount).toHaveBeenCalled()
    })
  })
})
