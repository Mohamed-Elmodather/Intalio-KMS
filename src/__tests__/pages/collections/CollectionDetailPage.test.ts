import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { ref } from 'vue'
import CollectionDetailPage from '@/pages/collections/CollectionDetailPage.vue'

// Mock vue-router
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
    back: vi.fn(),
  }),
  useRoute: () => ({
    params: { id: 'col-1' },
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
  BookmarkButton: { template: '<button class="bookmark"></button>' },
  SocialShareButtons: { template: '<div class="share"></div>' },
  CommentsSection: { template: '<div class="comments"></div>' },
}))

function mountComponent() {
  return shallowMount(CollectionDetailPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
        'router-link': true,
      },
    },
  })
}

describe('CollectionDetailPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render collection detail page', () => {
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

    it('should have header text', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.backToCollections).toBeDefined()
      expect(vm.textConstants.editCollection).toBeDefined()
    })

    it('should have view text', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.gridView).toBeDefined()
      expect(vm.textConstants.listView).toBeDefined()
    })

    it('should have sort text', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.textConstants.sortBy).toBeDefined()
      expect(vm.textConstants.sortRecent).toBeDefined()
    })
  })

  describe('UI State', () => {
    it('should have is editing state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isEditing).toBe('boolean')
    })

    it('should have show collaborator modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showCollaboratorModal).toBe('boolean')
    })

    it('should have show delete confirm state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showDeleteConfirm).toBe('boolean')
    })

    it('should have show share modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showShareModal).toBe('boolean')
    })

    it('should have show export menu state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showExportMenu).toBe('boolean')
    })
  })

  describe('View State', () => {
    it('should have view mode', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.viewMode).toBeDefined()
    })

    it('should have sort by state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sortBy).toBeDefined()
    })

    it('should have filter type state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.filterType).toBeDefined()
    })
  })

  describe('Add Items Modal', () => {
    it('should have show add items modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.showAddItemsModal).toBe('boolean')
    })

    it('should have add items search state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.addItemsSearch).toBeDefined()
    })

    it('should have add items filter state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.addItemsFilter).toBeDefined()
    })
  })

  describe('Selection Mode', () => {
    it('should have selected items set', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedItems).toBeDefined()
    })

    it('should have dragged item state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.draggedItem).toBeDefined()
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
    })
  })

  describe('Invite Collaborator', () => {
    it('should have invite email state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.inviteEmail).toBeDefined()
    })

    it('should have invite role state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.inviteRole).toBeDefined()
    })
  })
})
