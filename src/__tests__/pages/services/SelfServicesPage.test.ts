import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import SelfServicesPage from '@/pages/services/SelfServicesPage.vue'

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
vi.mock('@/components/common/LoadingSpinner.vue', () => ({
  default: { template: '<div class="loading-spinner"></div>' },
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AISuggestionChip: { template: '<span class="ai-suggestion"></span>' },
}))

// Mock stores
vi.mock('@/stores/aiServices', () => ({
  useAIServicesStore: () => ({
    isLoading: false,
    error: null,
  }),
}))

function mountComponent() {
  return shallowMount(SelfServicesPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
      },
    },
  })
}

describe('SelfServicesPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render services page', () => {
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

  describe('Modal States', () => {
    it('should have new request modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showNewRequest).toBe(false)
    })
  })

  describe('Filter State', () => {
    it('should have filter category as all', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.filterCategory).toBe('all')
    })

    it('should have request filter as all', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.requestFilter).toBe('all')
    })

    it('should have AI query empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiQuery).toBe('')
    })
  })

  describe('Stats', () => {
    it('should have stats object', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.stats).toBeDefined()
    })

    it('should have IT requests count', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.stats.itRequests).toBe(12)
    })

    it('should have HR requests count', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.stats.hrRequests).toBe(8)
    })

    it('should have facilities requests count', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.stats.facilitiesRequests).toBe(5)
    })

    it('should have finance requests count', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.stats.financeRequests).toBe(3)
    })
  })

  describe('Page Stats', () => {
    it('should have open requests count', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.openRequestsCount).toBe(3)
    })

    it('should have completed requests count', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.completedRequestsCount).toBe(24)
    })

    it('should have avg resolution time', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.avgResolutionTime).toBe(4)
    })

    it('should have total services count', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.totalServices).toBe(15)
    })
  })

  describe('Hero Stats', () => {
    it('should compute hero stats', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.heroStats).toBeDefined()
      expect(vm.heroStats.length).toBe(4)
    })
  })

  describe('Categories', () => {
    it('should have categories', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.categories).toBeDefined()
      expect(vm.categories.length).toBe(5)
    })

    it('should have all category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const category = vm.categories.find((c: any) => c.id === 'all')
      expect(category).toBeDefined()
    })

    it('should have IT category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const category = vm.categories.find((c: any) => c.id === 'it')
      expect(category).toBeDefined()
    })

    it('should have HR category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const category = vm.categories.find((c: any) => c.id === 'hr')
      expect(category).toBeDefined()
    })

    it('should have facilities category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const category = vm.categories.find((c: any) => c.id === 'facilities')
      expect(category).toBeDefined()
    })

    it('should have finance category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const category = vm.categories.find((c: any) => c.id === 'finance')
      expect(category).toBeDefined()
    })
  })

  describe('Services', () => {
    it('should have services array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.services).toBeDefined()
      expect(Array.isArray(vm.services)).toBe(true)
    })

    it('should have service properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const service = vm.services[0]
      expect(service.id).toBeDefined()
      expect(service.name).toBeDefined()
      expect(service.description).toBeDefined()
      expect(service.category).toBeDefined()
      expect(service.icon).toBeDefined()
    })

    it('should have IT services', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const itServices = vm.services.filter((s: any) => s.category === 'it')
      expect(itServices.length).toBeGreaterThan(0)
    })

    it('should have HR services', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const hrServices = vm.services.filter((s: any) => s.category === 'hr')
      expect(hrServices.length).toBeGreaterThan(0)
    })
  })

  describe('My Requests', () => {
    it('should have my requests array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.myRequests).toBeDefined()
      expect(Array.isArray(vm.myRequests)).toBe(true)
    })

    it('should have request properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const request = vm.myRequests[0]
      expect(request.id).toBeDefined()
      expect(request.title).toBeDefined()
      expect(request.status).toBeDefined()
      expect(request.date).toBeDefined()
    })

    it('should have request steps', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const request = vm.myRequests[0]
      expect(request.steps).toBeDefined()
      expect(Array.isArray(request.steps)).toBe(true)
    })
  })

  describe('Selected Service', () => {
    it('should have selected service as null', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedService).toBeNull()
    })
  })
})
