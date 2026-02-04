import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import AnalyticsPage from '@/pages/analytics/AnalyticsPage.vue'

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

// Mock AI components
vi.mock('@/components/ai', () => ({
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
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
  return shallowMount(AnalyticsPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
      },
    },
  })
}

describe('AnalyticsPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render analytics page', () => {
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

  describe('Period Selection', () => {
    it('should have selected period', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedPeriod).toBe('30d')
    })

    it('should have period options', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.periodOptions).toBeDefined()
      expect(vm.periodOptions.length).toBe(4)
    })
  })

  describe('Filters', () => {
    it('should have selected metric', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedMetric).toBe('views')
    })

    it('should have selected department', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedDepartment).toBe('all')
    })

    it('should have selected content type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedContentType).toBe('all')
    })

    it('should have departments list', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.departments).toBeDefined()
      expect(vm.departments.length).toBeGreaterThan(0)
    })

    it('should have content types list', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.contentTypes).toBeDefined()
      expect(vm.contentTypes.length).toBeGreaterThan(0)
    })
  })

  describe('Stats', () => {
    it('should have stats object', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.stats).toBeDefined()
    })

    it('should have platform adoption stat', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.stats.platformAdoption).toBe(78)
    })

    it('should have active users stat', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.stats.activeUsers).toBe(842)
    })

    it('should have total content stat', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.stats.totalContent).toBe(1247)
    })

    it('should have engagement score stat', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.stats.engagementScore).toBe(8.4)
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

  describe('Content Engagement', () => {
    it('should have content engagement data', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.contentEngagement).toBeDefined()
      expect(vm.contentEngagement.length).toBeGreaterThan(0)
    })

    it('should have engagement item properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const item = vm.contentEngagement[0]
      expect(item.name).toBeDefined()
      expect(item.views).toBeDefined()
      expect(item.interactions).toBeDefined()
      expect(item.color).toBeDefined()
    })

    it('should compute max engagement value', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.maxEngagementValue).toBeGreaterThan(0)
    })
  })

  describe('KPI Cards', () => {
    it('should have KPI cards', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.kpiCards).toBeDefined()
      expect(vm.kpiCards.length).toBe(4)
    })

    it('should have KPI card properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const card = vm.kpiCards[0]
      expect(card.id).toBeDefined()
      expect(card.title).toBeDefined()
      expect(card.value).toBeDefined()
      expect(card.icon).toBeDefined()
      expect(card.color).toBeDefined()
      expect(card.sparkline).toBeDefined()
    })
  })

  describe('Tournament Records', () => {
    it('should have tournament records', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.tournamentRecords).toBeDefined()
      expect(vm.tournamentRecords.length).toBe(4)
    })

    it('should have record properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const record = vm.tournamentRecords[0]
      expect(record.id).toBeDefined()
      expect(record.label).toBeDefined()
      expect(record.value).toBeDefined()
      expect(record.icon).toBeDefined()
    })
  })

  describe('Tournament Insights', () => {
    it('should have tournament insights', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.tournamentInsights).toBeDefined()
      expect(vm.tournamentInsights.length).toBeGreaterThan(0)
    })

    it('should have insight properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const insight = vm.tournamentInsights[0]
      expect(insight.id).toBeDefined()
      expect(insight.icon).toBeDefined()
      expect(insight.text).toBeDefined()
    })
  })

  describe('Helper Functions', () => {
    it('should have safeMax function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.safeMax).toBe('function')
    })

    it('should have safeGet function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.safeGet).toBe('function')
    })
  })
})
