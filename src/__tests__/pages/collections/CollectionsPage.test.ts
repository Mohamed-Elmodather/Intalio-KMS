import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import CollectionsPage from '@/pages/collections/CollectionsPage.vue'

// Mock vue-router
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
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
vi.mock('@/components/common/EmptyState.vue', () => ({
  default: { template: '<div class="empty-state"><slot /></div>' },
}))

function mountComponent() {
  return shallowMount(CollectionsPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
      },
    },
  })
}

describe('CollectionsPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render collections page', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Current User', () => {
    it('should have current user', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.currentUser).toBeDefined()
    })

    it('should have user properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.currentUser.id).toBeDefined()
      expect(vm.currentUser.name).toBeDefined()
      expect(vm.currentUser.initials).toBeDefined()
      expect(vm.currentUser.color).toBeDefined()
    })
  })

  describe('View State', () => {
    it('should have current view as all', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.currentView).toBe('all')
    })

    it('should have search query empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.searchQuery).toBe('')
    })

    it('should have selected type as all', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedType).toBe('all')
    })

    it('should have sort by updated', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sortBy).toBe('updated')
    })
  })

  describe('Modal States', () => {
    it('should have create modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showCreateModal).toBe(false)
    })

    it('should have type filter state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showTypeFilter).toBe(false)
    })

    it('should have sort dropdown state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showSortDropdown).toBe(false)
    })
  })

  describe('View Options', () => {
    it('should have view options', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.viewOptions).toBeDefined()
      expect(vm.viewOptions.length).toBe(4)
    })

    it('should have all view option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const option = vm.viewOptions.find((o: any) => o.id === 'all')
      expect(option).toBeDefined()
    })

    it('should have my collections view option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const option = vm.viewOptions.find((o: any) => o.id === 'my')
      expect(option).toBeDefined()
    })

    it('should have shared view option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const option = vm.viewOptions.find((o: any) => o.id === 'shared')
      expect(option).toBeDefined()
    })

    it('should have recent view option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const option = vm.viewOptions.find((o: any) => o.id === 'recent')
      expect(option).toBeDefined()
    })
  })

  describe('Type Options', () => {
    it('should have type options', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.typeOptions).toBeDefined()
      expect(vm.typeOptions.length).toBe(5)
    })
  })

  describe('Sort Options', () => {
    it('should have sort options', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sortOptions).toBeDefined()
      expect(vm.sortOptions.length).toBe(4)
    })

    it('should have updated sort option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const option = vm.sortOptions.find((o: any) => o.id === 'updated')
      expect(option).toBeDefined()
    })

    it('should have created sort option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const option = vm.sortOptions.find((o: any) => o.id === 'created')
      expect(option).toBeDefined()
    })

    it('should have name sort option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const option = vm.sortOptions.find((o: any) => o.id === 'name')
      expect(option).toBeDefined()
    })

    it('should have items sort option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const option = vm.sortOptions.find((o: any) => o.id === 'items')
      expect(option).toBeDefined()
    })
  })

  describe('Collections Data', () => {
    it('should have collections array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.collections).toBeDefined()
      expect(Array.isArray(vm.collections)).toBe(true)
    })

    it('should have collection properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const collection = vm.collections[0]
      expect(collection.id).toBeDefined()
      expect(collection.name).toBeDefined()
      expect(collection.description).toBeDefined()
      expect(collection.author).toBeDefined()
      expect(collection.itemCount).toBeDefined()
    })

    it('should have collection items', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const collection = vm.collections[0]
      expect(collection.items).toBeDefined()
      expect(Array.isArray(collection.items)).toBe(true)
    })

    it('should have collection collaborators', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const collection = vm.collections[0]
      expect(collection.collaborators).toBeDefined()
      expect(Array.isArray(collection.collaborators)).toBe(true)
    })
  })

  describe('Recently Viewed', () => {
    it('should have recently viewed set', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.recentlyViewed).toBeDefined()
    })
  })
})
