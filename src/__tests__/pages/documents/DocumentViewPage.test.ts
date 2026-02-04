import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { ref } from 'vue'
import DocumentViewPage from '@/pages/documents/DocumentViewPage.vue'

// Mock vue-router
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
    back: vi.fn(),
  }),
  useRoute: () => ({
    params: { id: '1' },
  }),
}))

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AIConfidenceBar: { template: '<div class="ai-confidence"></div>' },
  AIEntityHighlight: { template: '<span class="ai-entity"></span>' },
  AITranslateDropdown: { template: '<div class="ai-translate"></div>' },
  AIActionButton: { template: '<button class="ai-action"></button>' },
  AIResultCard: { template: '<div class="ai-result"></div>' },
}))

// Mock common components
vi.mock('@/components/common', () => ({
  CommentsSection: { template: '<div class="comments"></div>' },
  RatingStars: { template: '<div class="rating"></div>' },
  SocialShareButtons: { template: '<div class="share"></div>' },
  BookmarkButton: { template: '<button class="bookmark"></button>' },
  AddToCollectionModal: { template: '<div class="collection-modal"></div>' },
}))

// Mock stores
vi.mock('@/stores/aiServices', () => ({
  useAIServicesStore: () => ({
    isLoading: false,
    error: null,
  }),
}))

// Mock composables
vi.mock('@/composables/useComments', () => ({
  useComments: () => ({
    comments: ref([]),
    isLoading: ref(false),
    loadComments: vi.fn(),
    addComment: vi.fn(),
  }),
}))

vi.mock('@/composables/useRatings', () => ({
  useRatings: () => ({
    rating: ref(0),
    submitRating: vi.fn(),
    loadRating: vi.fn(),
  }),
}))

function mountComponent() {
  return shallowMount(DocumentViewPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
        'router-link': true,
      },
    },
  })
}

describe('DocumentViewPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render document view page', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Loading State', () => {
    it('should have loading state true initially', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isLoading).toBe(true)
    })
  })

  describe('Document State', () => {
    it('should have document ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.document).toBeDefined()
    })
  })

  describe('Version History', () => {
    it('should have versions array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.versions).toBeDefined()
      expect(Array.isArray(vm.versions)).toBe(true)
    })

    it('should have show version history state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showVersionHistory).toBe(false)
    })

    it('should have active version ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.activeVersion).toBeNull()
    })
  })

  describe('Sidebar Tab', () => {
    it('should have active detail tab as details', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.activeDetailTab).toBe('details')
    })
  })

  describe('Collection Modal', () => {
    it('should have show add to collection state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showAddToCollection).toBe(false)
    })
  })

  describe('Mock Documents', () => {
    it('should have mock documents', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.mockDocuments).toBeDefined()
      expect(Array.isArray(vm.mockDocuments)).toBe(true)
    })

    it('should have document properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const doc = vm.mockDocuments[0]
      expect(doc.id).toBeDefined()
      expect(doc.name).toBeDefined()
      expect(doc.description).toBeDefined()
      expect(doc.size).toBeDefined()
      expect(doc.type).toBeDefined()
    })

    it('should have document metadata', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const doc = vm.mockDocuments[0]
      expect(doc.createdAt).toBeDefined()
      expect(doc.updatedAt).toBeDefined()
      expect(doc.author).toBeDefined()
      expect(doc.library).toBeDefined()
      expect(doc.tags).toBeDefined()
    })

    it('should have document stats', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const doc = vm.mockDocuments[0]
      expect(doc.downloads).toBeDefined()
      expect(doc.views).toBeDefined()
      expect(doc.version).toBeDefined()
    })
  })

  describe('Document ID', () => {
    it('should compute document id from route', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.documentId).toBe('1')
    })
  })
})
