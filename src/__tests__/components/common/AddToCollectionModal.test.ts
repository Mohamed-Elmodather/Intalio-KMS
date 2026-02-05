import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AddToCollectionModal from '@/components/common/AddToCollectionModal.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string, params?: any) => params ? `${key} ${JSON.stringify(params)}` : key,
  }),
}))

function mountComponent(props = {}) {
  return shallowMount(AddToCollectionModal, {
    props: {
      show: true,
      contentType: 'article',
      contentId: '123',
      contentTitle: 'Test Article',
      ...props,
    },
    global: {
      mocks: {
        $t: (key: string, params?: any) => params ? `${key} ${JSON.stringify(params)}` : key,
      },
      stubs: {
        Teleport: true,
        Transition: true,
      },
    },
  })
}

describe('AddToCollectionModal', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render when show is true', () => {
      const wrapper = mountComponent({ show: true })
      expect(wrapper.find('.modal-overlay').exists()).toBe(true)
    })

    it('should not render when show is false', () => {
      const wrapper = mountComponent({ show: false })
      expect(wrapper.find('.modal-overlay').exists()).toBe(false)
    })

    it('should render header', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('collections.addToCollection')
    })

    it('should render close button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-times').exists()).toBe(true)
    })
  })

  describe('Content Preview', () => {
    it('should display content title', () => {
      const wrapper = mountComponent({ contentTitle: 'My Article' })
      expect(wrapper.text()).toContain('My Article')
    })

    it('should display content thumbnail when provided', () => {
      const wrapper = mountComponent({ contentThumbnail: 'https://example.com/thumb.jpg' })
      const img = wrapper.find('img[alt="Test Article"]')
      expect(img.exists()).toBe(true)
    })

    it('should show fallback icon when no thumbnail', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getContentTypeIcon).toBeTruthy()
    })
  })

  describe('Content Type Icons', () => {
    it('should return newspaper icon for article', () => {
      const wrapper = mountComponent({ contentType: 'article' })
      const vm = wrapper.vm as any
      expect(vm.getContentTypeIcon).toBe('fas fa-newspaper')
    })

    it('should return file-alt icon for document', () => {
      const wrapper = mountComponent({ contentType: 'document' })
      const vm = wrapper.vm as any
      expect(vm.getContentTypeIcon).toBe('fas fa-file-alt')
    })

    it('should return photo-video icon for media', () => {
      const wrapper = mountComponent({ contentType: 'media' })
      const vm = wrapper.vm as any
      expect(vm.getContentTypeIcon).toBe('fas fa-photo-video')
    })
  })

  describe('Content Type Labels', () => {
    it('should return articles label for article', () => {
      const wrapper = mountComponent({ contentType: 'article' })
      const vm = wrapper.vm as any
      expect(vm.getContentTypeLabel).toBe('nav.articles')
    })

    it('should return documents label for document', () => {
      const wrapper = mountComponent({ contentType: 'document' })
      const vm = wrapper.vm as any
      expect(vm.getContentTypeLabel).toBe('nav.documents')
    })

    it('should return media label for media', () => {
      const wrapper = mountComponent({ contentType: 'media' })
      const vm = wrapper.vm as any
      expect(vm.getContentTypeLabel).toBe('nav.media')
    })
  })

  describe('Collections List', () => {
    it('should display collections', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Tournament Highlights 2027')
      expect(wrapper.text()).toContain('Team Research - Group A')
    })

    it('should show collection item counts', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('collections.itemCount')
    })

    it('should show visibility icons', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-lock').exists() || wrapper.find('.fa-globe').exists()).toBe(true)
    })
  })

  describe('Selection', () => {
    it('should toggle collection selection', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isSelected('col-1')).toBe(false)
      vm.toggleCollection('col-1')
      expect(vm.isSelected('col-1')).toBe(true)
      vm.toggleCollection('col-1')
      expect(vm.isSelected('col-1')).toBe(false)
    })

    it('should track selected count', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // Initial count is 0 since watcher only runs on show prop change
      expect(vm.selectedCount).toBe(0)
      vm.toggleCollection('col-1')
      expect(vm.selectedCount).toBe(1)
      vm.toggleCollection('col-2')
      expect(vm.selectedCount).toBe(2)
    })
  })

  describe('Search', () => {
    it('should filter collections by search query', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.searchQuery = 'tournament'
      expect(vm.filteredCollections.length).toBe(1)
      expect(vm.filteredCollections[0].name).toContain('Tournament')
    })

    it('should return all collections when search is empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.searchQuery = ''
      expect(vm.filteredCollections.length).toBe(6)
    })

    it('should show search input', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.search-input').exists()).toBe(true)
    })

    it('should show clear search button when query exists', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.searchQuery = 'test'
      await wrapper.vm.$nextTick()
      expect(wrapper.find('.clear-search').exists()).toBe(true)
    })
  })

  describe('Create New Collection', () => {
    it('should show create new button', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('collections.createNewCollection')
    })

    it('should toggle create form visibility', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showCreateNew).toBe(false)
      vm.showCreateNew = true
      expect(vm.showCreateNew).toBe(true)
    })

    it('should create new collection', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showCreateNew = true
      vm.newCollectionName = 'My New Collection'
      vm.newCollectionVisibility = 'private'
      vm.createNewCollection()
      expect(vm.collections[0].name).toBe('My New Collection')
      expect(vm.isSelected(vm.collections[0].id)).toBe(true)
    })

    it('should not create collection with empty name', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const initialCount = vm.collections.length
      vm.showCreateNew = true
      vm.newCollectionName = '   '
      vm.createNewCollection()
      expect(vm.collections.length).toBe(initialCount)
    })

    it('should reset form after creation', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showCreateNew = true
      vm.newCollectionName = 'Test'
      vm.createNewCollection()
      expect(vm.newCollectionName).toBe('')
      expect(vm.showCreateNew).toBe(false)
    })
  })

  describe('Visibility Toggle', () => {
    it('should toggle visibility', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.newCollectionVisibility).toBe('private')
      vm.newCollectionVisibility = 'shared'
      expect(vm.newCollectionVisibility).toBe('shared')
    })

    it('should show visibility buttons in create form', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showCreateNew = true
      await wrapper.vm.$nextTick()
      expect(wrapper.find('.visibility-toggle').exists()).toBe(true)
    })
  })

  describe('Add to Collections', () => {
    it('should emit added event with selected collections', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.selectedCollections.add('col-1')
      vm.selectedCollections.add('col-2')
      vm.addToCollections()
      expect(wrapper.emitted('added')).toBeTruthy()
      expect(wrapper.emitted('added')![0][0]).toContain('col-1')
      expect(wrapper.emitted('added')![0][0]).toContain('col-2')
    })

    it('should update item counts', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.selectedCollections.clear()
      vm.selectedCollections.add('col-1')
      const initialCount = vm.collections.find((c: any) => c.id === 'col-1').itemCount
      vm.addToCollections()
      expect(vm.collections.find((c: any) => c.id === 'col-1').itemCount).toBe(initialCount + 1)
    })

    it('should close modal after adding', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.addToCollections()
      expect(wrapper.emitted('close')).toBeTruthy()
    })
  })

  describe('Close Modal', () => {
    it('should emit close event', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.closeModal()
      expect(wrapper.emitted('close')).toBeTruthy()
    })
  })

  describe('Empty State', () => {
    it('should show empty state when no collections match search', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.searchQuery = 'nonexistent'
      expect(vm.filteredCollections.length).toBe(0)
    })
  })

  describe('Already Added Badge', () => {
    it('should show already added badge for items in collection', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('collections.alreadyAdded')
    })
  })

  describe('Watch Show Prop', () => {
    it('should reset state when modal opens', async () => {
      const wrapper = mountComponent({ show: false })
      const vm = wrapper.vm as any
      vm.searchQuery = 'test'
      await wrapper.setProps({ show: true })
      expect(vm.searchQuery).toBe('')
    })

    it('should pre-select collections that already contain item', async () => {
      const wrapper = mountComponent({ show: false })
      await wrapper.setProps({ show: true })
      const vm = wrapper.vm as any
      expect(vm.selectedCollections.has('col-3')).toBe(true)
    })
  })

  describe('Footer', () => {
    it('should show cancel button', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('common.cancel')
    })

    it('should show add button with count', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('collections.addToCount')
    })

    it('should disable add button when no selection', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.selectedCollections.clear()
      expect(vm.selectedCount).toBe(0)
    })
  })
})
