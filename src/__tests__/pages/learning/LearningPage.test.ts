import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import LearningPage from '@/pages/learning/LearningPage.vue'

// Mock vue-router
const mockPush = vi.fn()
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: mockPush,
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
vi.mock('@/components/common/ViewAllButton.vue', () => ({
  default: { template: '<button class="view-all-btn"><slot /></button>' },
}))
vi.mock('@/components/common/EmptyState.vue', () => ({
  default: { template: '<div class="empty-state"><slot /></div>' },
}))
vi.mock('@/components/common/Pagination.vue', () => ({
  default: { template: '<div class="pagination"><slot /></div>' },
}))
vi.mock('@/components/common/CategoryBadge.vue', () => ({
  default: { template: '<span class="category-badge"><slot /></span>' },
}))
vi.mock('@/components/common/StatusBadge.vue', () => ({
  default: { template: '<span class="status-badge"><slot /></span>' },
}))
vi.mock('@/components/common/TagBadge.vue', () => ({
  default: { template: '<span class="tag-badge"><slot /></span>' },
}))
vi.mock('@/components/common/ShareContentModal.vue', () => ({
  default: { template: '<div class="share-modal"><slot /></div>' },
}))
vi.mock('@/components/common/SkeletonLoader.vue', () => ({
  default: { template: '<div class="skeleton-loader"><slot /></div>' },
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AIConfidenceBar: { template: '<div class="ai-confidence"></div>' },
  AISuggestionChip: { template: '<span class="ai-suggestion"></span>' },
}))

// Mock stores
vi.mock('@/stores/aiServices', () => ({
  useAIServicesStore: () => ({
    isLoading: false,
    error: null,
  }),
}))

describe('LearningPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render learning page', () => {
      const wrapper = mount(LearningPage)
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Loading State', () => {
    it('should have loading state false initially', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.isLoading).toBe(false)
    })
  })

  describe('AI Features State', () => {
    it('should have AI analyzing state defined', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(typeof vm.isAIAnalyzing).toBe('boolean')
    })

    it('should have recommendations generation state defined', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      // onMounted calls generateAIRecommendations which sets this to true
      expect(typeof vm.isGeneratingRecommendations).toBe('boolean')
    })

    it('should have skill gaps analysis state defined', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(typeof vm.isAnalyzingSkillGaps).toBe('boolean')
    })

    it('should have learning path generation state defined', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(typeof vm.isGeneratingLearningPath).toBe('boolean')
    })

    it('should have AI insights panel state defined', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(typeof vm.showAIInsightsPanel).toBe('boolean')
    })

    it('should have skill gap modal state defined', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(typeof vm.showSkillGapModal).toBe('boolean')
    })

    it('should have AI learning path modal state defined', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(typeof vm.showAILearningPathModal).toBe('boolean')
    })
  })

  describe('AI Personalized Recommendations', () => {
    it('should have personalized recommendations array', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.aiPersonalizedRecommendations).toBeDefined()
      expect(Array.isArray(vm.aiPersonalizedRecommendations)).toBe(true)
    })

    it('should have mock AI recommendations', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.mockAIRecommendations).toBeDefined()
      expect(vm.mockAIRecommendations.length).toBeGreaterThan(0)
    })

    it('should have recommendation properties', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      const rec = vm.mockAIRecommendations[0]
      expect(rec.courseId).toBeDefined()
      expect(rec.title).toBeDefined()
      expect(rec.reason).toBeDefined()
      expect(rec.matchScore).toBeDefined()
      expect(rec.skillsGained).toBeDefined()
      expect(rec.priority).toBeDefined()
    })
  })

  describe('AI Skill Gap Analysis', () => {
    it('should have skill gap analysis ref', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.skillGapAnalysis).toBeDefined()
    })

    it('should have mock skill gap analysis', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.mockSkillGapAnalysis).toBeDefined()
    })

    it('should have overall readiness score', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.mockSkillGapAnalysis.overallReadiness).toBe(72)
    })

    it('should have strengths array', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.mockSkillGapAnalysis.strengths).toBeDefined()
      expect(vm.mockSkillGapAnalysis.strengths.length).toBeGreaterThan(0)
    })

    it('should have weaknesses array', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.mockSkillGapAnalysis.weaknesses).toBeDefined()
      expect(vm.mockSkillGapAnalysis.weaknesses.length).toBeGreaterThan(0)
    })

    it('should have gaps array', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.mockSkillGapAnalysis.gaps).toBeDefined()
      expect(vm.mockSkillGapAnalysis.gaps.length).toBeGreaterThan(0)
    })

    it('should have gap properties', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      const gap = vm.mockSkillGapAnalysis.gaps[0]
      expect(gap.skill).toBeDefined()
      expect(gap.currentLevel).toBeDefined()
      expect(gap.targetLevel).toBeDefined()
      expect(gap.gap).toBeDefined()
      expect(gap.priority).toBeDefined()
    })
  })

  describe('AI Learning Paths', () => {
    it('should have AI learning paths array', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.aiLearningPaths).toBeDefined()
      expect(Array.isArray(vm.aiLearningPaths)).toBe(true)
    })

    it('should have selected AI path ref', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.selectedAIPath).toBeDefined()
    })

    it('should have mock AI learning paths', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.mockAILearningPaths).toBeDefined()
      expect(vm.mockAILearningPaths.length).toBe(3)
    })

    it('should have learning path properties', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      const path = vm.mockAILearningPaths[0]
      expect(path.id).toBeDefined()
      expect(path.title).toBeDefined()
      expect(path.description).toBeDefined()
      expect(path.goal).toBeDefined()
      expect(path.totalDuration).toBeDefined()
      expect(path.steps).toBeDefined()
      expect(path.confidence).toBeDefined()
    })

    it('should have learning path steps', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      const path = vm.mockAILearningPaths[0]
      expect(path.steps.length).toBeGreaterThan(0)
      const step = path.steps[0]
      expect(step.order).toBeDefined()
      expect(step.courseId).toBeDefined()
      expect(step.title).toBeDefined()
      expect(step.duration).toBeDefined()
      expect(step.skills).toBeDefined()
    })
  })

  describe('AI Insights', () => {
    it('should have AI insights array', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.aiInsights).toBeDefined()
      expect(Array.isArray(vm.aiInsights)).toBe(true)
    })

    it('should have mock AI insights', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(vm.mockAIInsights).toBeDefined()
      expect(vm.mockAIInsights.length).toBeGreaterThan(0)
    })

    it('should have insight properties', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      const insight = vm.mockAIInsights[0]
      expect(insight.id).toBeDefined()
      expect(insight.type).toBeDefined()
      expect(insight.title).toBeDefined()
      expect(insight.message).toBeDefined()
      expect(insight.icon).toBeDefined()
      expect(insight.color).toBeDefined()
    })

    it('should have different insight types', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      const types = vm.mockAIInsights.map((i: any) => i.type)
      expect(types).toContain('milestone')
      expect(types).toContain('recommendation')
      expect(types).toContain('tip')
    })
  })

  describe('AI Functions', () => {
    it('should have generateAIRecommendations function', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(typeof vm.generateAIRecommendations).toBe('function')
    })

    it('should have analyzeSkillGaps function', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(typeof vm.analyzeSkillGaps).toBe('function')
    })

    it('should have generateAILearningPaths function', () => {
      const wrapper = mount(LearningPage)
      const vm = wrapper.vm as any
      expect(typeof vm.generateAILearningPaths).toBe('function')
    })
  })
})
