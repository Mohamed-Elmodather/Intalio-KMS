import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AppSidebar from '@/components/layout/AppSidebar.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock vue-router
const mockRoute = { path: '/' }
vi.mock('vue-router', () => ({
  useRoute: () => mockRoute,
}))

// Mock UI store
let mockSidebarCollapsed = false
vi.mock('@/stores/ui', () => ({
  useUIStore: () => ({
    sidebarCollapsed: mockSidebarCollapsed,
  }),
}))

function mountComponent() {
  return shallowMount(AppSidebar, {
    global: {
      mocks: {
        $t: (key: string) => key,
      },
      stubs: {
        'router-link': {
          template: '<a :href="to" :class="$attrs.class"><slot /></a>',
          props: ['to'],
        },
      },
    },
  })
}

describe('AppSidebar', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockSidebarCollapsed = false
    mockRoute.path = '/'
  })

  describe('Rendering', () => {
    it('should render the sidebar', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('nav').exists()).toBe(true)
    })

    it('should render navigation items', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.navigationItems.length).toBeGreaterThan(0)
    })

    it('should render workspace items', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.workspaceItems.length).toBe(3)
    })

    it('should render settings link', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-cog').exists()).toBe(true)
    })

    it('should render AI assistant link', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-wand-magic-sparkles').exists()).toBe(true)
    })
  })

  describe('Navigation Items', () => {
    it('should have dashboard item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const dashboard = vm.navigationItems.find((item: any) => item.id === 'dashboard')
      expect(dashboard).toBeDefined()
      expect(dashboard.route).toBe('/')
    })

    it('should have articles item with badge', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const articles = vm.navigationItems.find((item: any) => item.id === 'articles')
      expect(articles).toBeDefined()
      expect(articles.badge).toBe('24')
    })

    it('should have documents item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const docs = vm.navigationItems.find((item: any) => item.id === 'documents')
      expect(docs).toBeDefined()
      expect(docs.route).toBe('/documents')
    })

    it('should have media item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const media = vm.navigationItems.find((item: any) => item.id === 'media')
      expect(media).toBeDefined()
    })

    it('should have learning item with badge', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const learning = vm.navigationItems.find((item: any) => item.id === 'learning')
      expect(learning).toBeDefined()
      expect(learning.badge).toBe('2')
    })

    it('should have events item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const events = vm.navigationItems.find((item: any) => item.id === 'events')
      expect(events).toBeDefined()
    })

    it('should have collaboration item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const collab = vm.navigationItems.find((item: any) => item.id === 'collaboration')
      expect(collab).toBeDefined()
    })

    it('should have polls item with badge', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const polls = vm.navigationItems.find((item: any) => item.id === 'polls')
      expect(polls).toBeDefined()
      expect(polls.badge).toBe('3')
    })

    it('should have services item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const services = vm.navigationItems.find((item: any) => item.id === 'services')
      expect(services).toBeDefined()
    })
  })

  describe('Workspace Items', () => {
    it('should have HR Portal workspace', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const hr = vm.workspaceItems.find((item: any) => item.id === 'ws-hr')
      expect(hr).toBeDefined()
      expect(hr.initials).toBe('HR')
    })

    it('should have IT Knowledge workspace', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const it = vm.workspaceItems.find((item: any) => item.id === 'ws-it')
      expect(it).toBeDefined()
      expect(it.initials).toBe('IT')
    })

    it('should have Sales Enablement workspace', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const sales = vm.workspaceItems.find((item: any) => item.id === 'ws-sales')
      expect(sales).toBeDefined()
      expect(sales.initials).toBe('SE')
    })

    it('should have colors for workspaces', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.workspaceItems.forEach((item: any) => {
        expect(item.color).toBeDefined()
        expect(item.color).toMatch(/^#[0-9A-Fa-f]{6}$/)
      })
    })
  })

  describe('isActive Function', () => {
    it('should have isActive function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isActive).toBe('function')
    })

    it('should return true for dashboard when on root path', () => {
      mockRoute.path = '/'
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isActive({ route: '/', id: 'dashboard' })).toBe(true)
    })

    it('should return false for dashboard when on other path', () => {
      mockRoute.path = '/articles'
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isActive({ route: '/', id: 'dashboard' })).toBe(false)
    })

    it('should return true for articles when path starts with /articles', () => {
      mockRoute.path = '/articles'
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isActive({ route: '/articles', id: 'articles' })).toBe(true)
    })

    it('should return true for nested article path', () => {
      mockRoute.path = '/articles/123'
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isActive({ route: '/articles', id: 'articles' })).toBe(true)
    })

    it('should return false for non-matching path', () => {
      mockRoute.path = '/documents'
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isActive({ route: '/articles', id: 'articles' })).toBe(false)
    })
  })

  describe('Collapsed State', () => {
    it('should compute isCollapsed from store', () => {
      mockSidebarCollapsed = false
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isCollapsed).toBe(false)
    })

    it('should reflect collapsed state', () => {
      mockSidebarCollapsed = true
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isCollapsed).toBe(true)
    })
  })

  describe('Navigation Item Structure', () => {
    it('should have icon for each item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.navigationItems.forEach((item: any) => {
        expect(item.icon).toBeDefined()
        expect(item.icon).toContain('fa')
      })
    })

    it('should have label for each item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.navigationItems.forEach((item: any) => {
        expect(item.label).toBeDefined()
      })
    })

    it('should have route for each item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.navigationItems.forEach((item: any) => {
        expect(item.route).toBeDefined()
        expect(item.route.startsWith('/')).toBe(true)
      })
    })

    it('should have id for each item', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.navigationItems.forEach((item: any) => {
        expect(item.id).toBeDefined()
      })
    })
  })

  describe('Workspace Item Structure', () => {
    it('should have id for each workspace', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.workspaceItems.forEach((item: any) => {
        expect(item.id).toBeDefined()
      })
    })

    it('should have label for each workspace', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.workspaceItems.forEach((item: any) => {
        expect(item.label).toBeDefined()
      })
    })

    it('should have initials for each workspace', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.workspaceItems.forEach((item: any) => {
        expect(item.initials).toBeDefined()
        expect(item.initials.length).toBeLessThanOrEqual(2)
      })
    })
  })

  describe('Badge Classes', () => {
    it('should have badge class for articles', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const articles = vm.navigationItems.find((item: any) => item.id === 'articles')
      expect(articles.badgeClass).toContain('teal')
    })

    it('should have badge class for learning', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const learning = vm.navigationItems.find((item: any) => item.id === 'learning')
      expect(learning.badgeClass).toContain('blue')
    })

    it('should have badge class for polls', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const polls = vm.navigationItems.find((item: any) => item.id === 'polls')
      expect(polls.badgeClass).toContain('orange')
    })
  })
})
