import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import ComparisonInsights from '@/components/ai/ComparisonInsights.vue'
import { ref, computed } from 'vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string, params?: any) => params ? `${key} ${JSON.stringify(params)}` : key,
  }),
}))

const mockAnalysisResults = {
  similarity: 75,
  summary: 'These documents share common themes about technology.',
  differences: ['Document 1 focuses on AI', 'Document 2 covers databases'],
  keyPoints: [
    { itemId: '1', points: ['Point A', 'Point B'] },
    { itemId: '2', points: ['Point C', 'Point D'] },
  ],
  commonEntities: [
    { name: 'Machine Learning', type: 'topic', occurrences: 5, items: ['1', '2'] },
    { name: 'John Smith', type: 'person', occurrences: 3, items: ['1'] },
  ],
  sentimentComparison: [
    { itemId: '1', itemTitle: 'Document 1', score: 0.5, confidence: 0.85 },
    { itemId: '2', itemTitle: 'Document 2', score: -0.5, confidence: 0.78 },
  ],
  commonTopics: [
    { topic: 'Technology', items: ['1', '2'], relevance: 0.9 },
    { topic: 'Innovation', items: ['1'], relevance: 0.7 },
  ],
  classifications: [
    { itemId: '1', category: 'Tech', confidence: 0.92 },
    { itemId: '2', category: 'Science', confidence: 0.88 },
  ],
}

const mockSelectedItems = [
  { id: '1', title: 'Document 1', type: 'document' },
  { id: '2', title: 'Document 2', type: 'article' },
]

const mockGenerateAnalysis = vi.fn()

vi.mock('@/stores/comparison', () => ({
  useComparisonStore: () => ({
    analysisResults: mockAnalysisResults,
    isAnalyzing: false,
    selectedItems: mockSelectedItems,
    itemCount: 2,
    generateAnalysis: mockGenerateAnalysis,
  }),
}))

function mountComponent(props = {}) {
  return shallowMount(ComparisonInsights, {
    props: {
      section: 'summary',
      ...props,
    },
    global: {
      mocks: {
        $t: (key: string) => key,
      },
    },
  })
}

describe('ComparisonInsights', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.comparison-insights').exists()).toBe(true)
    })
  })

  describe('Entity Type Icons', () => {
    it('should have person icon', () => {
      const wrapper = mountComponent({ section: 'entities' })
      expect(wrapper.find('.fa-user').exists()).toBe(true)
    })

    it('should have correct entity type icons mapping', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.entityTypeIcons.person).toBe('fas fa-user')
      expect(vm.entityTypeIcons.organization).toBe('fas fa-building')
      expect(vm.entityTypeIcons.location).toBe('fas fa-map-marker-alt')
      expect(vm.entityTypeIcons.date).toBe('fas fa-calendar')
      expect(vm.entityTypeIcons.topic).toBe('fas fa-hashtag')
    })
  })

  describe('Entity Type Colors', () => {
    it('should have correct entity type colors mapping', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.entityTypeColors.person).toContain('bg-blue-100')
      expect(vm.entityTypeColors.organization).toContain('bg-purple-100')
      expect(vm.entityTypeColors.location).toContain('bg-green-100')
      expect(vm.entityTypeColors.date).toContain('bg-orange-100')
      expect(vm.entityTypeColors.topic).toContain('bg-teal-100')
    })
  })

  describe('Sentiment Functions', () => {
    it('should return green for positive sentiment', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getSentimentColor(0.5)).toBe('bg-green-500')
    })

    it('should return red for negative sentiment', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getSentimentColor(-0.5)).toBe('bg-red-500')
    })

    it('should return gray for neutral sentiment', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getSentimentColor(0)).toBe('bg-gray-400')
    })

    it('should return positive label', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getSentimentLabel(0.5)).toContain('comparison.positive')
    })

    it('should return negative label', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getSentimentLabel(-0.5)).toContain('comparison.negative')
    })

    it('should return neutral label', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getSentimentLabel(0)).toContain('comparison.neutral')
    })
  })

  describe('Summary Section', () => {
    it('should show similarity score', () => {
      const wrapper = mountComponent({ section: 'summary' })
      expect(wrapper.text()).toContain('75%')
    })

    it('should show AI summary', () => {
      const wrapper = mountComponent({ section: 'summary' })
      expect(wrapper.text()).toContain('These documents share common themes')
    })

    it('should show key differences', () => {
      const wrapper = mountComponent({ section: 'summary' })
      expect(wrapper.text()).toContain('Document 1 focuses on AI')
    })

    it('should show key points', () => {
      const wrapper = mountComponent({ section: 'summary' })
      expect(wrapper.text()).toContain('comparison.keyPoints')
    })

    it('should show brain icon', () => {
      const wrapper = mountComponent({ section: 'summary' })
      expect(wrapper.find('.fa-brain').exists()).toBe(true)
    })
  })

  describe('Entities Section', () => {
    it('should show entities when section is entities', () => {
      const wrapper = mountComponent({ section: 'entities' })
      expect(wrapper.text()).toContain('comparison.commonEntities')
    })

    it('should show entity names', () => {
      const wrapper = mountComponent({ section: 'entities' })
      expect(wrapper.text()).toContain('Machine Learning')
      expect(wrapper.text()).toContain('John Smith')
    })

    it('should show entity occurrences', () => {
      const wrapper = mountComponent({ section: 'entities' })
      expect(wrapper.text()).toContain('comparison.occurrences')
    })

    it('should show tags icon', () => {
      const wrapper = mountComponent({ section: 'entities' })
      expect(wrapper.find('.fa-tags').exists()).toBe(true)
    })
  })

  describe('Sentiment Section', () => {
    it('should show sentiment comparison', () => {
      const wrapper = mountComponent({ section: 'sentiment' })
      expect(wrapper.text()).toContain('comparison.sentimentComparison')
    })

    it('should show item titles', () => {
      const wrapper = mountComponent({ section: 'sentiment' })
      expect(wrapper.text()).toContain('Document 1')
      expect(wrapper.text()).toContain('Document 2')
    })

    it('should show overall sentiment section', () => {
      const wrapper = mountComponent({ section: 'sentiment' })
      expect(wrapper.text()).toContain('comparison.overallSentiment')
    })

    it('should show smile icon', () => {
      const wrapper = mountComponent({ section: 'sentiment' })
      expect(wrapper.find('.fa-smile').exists()).toBe(true)
    })

    it('should show sentiment faces', () => {
      const wrapper = mountComponent({ section: 'sentiment' })
      expect(wrapper.find('.fa-meh').exists()).toBe(true)
      expect(wrapper.find('.fa-frown').exists()).toBe(true)
    })
  })

  describe('Topics Section', () => {
    it('should show common topics', () => {
      const wrapper = mountComponent({ section: 'topics' })
      expect(wrapper.text()).toContain('comparison.commonTopics')
    })

    it('should show topic names', () => {
      const wrapper = mountComponent({ section: 'topics' })
      expect(wrapper.text()).toContain('Technology')
      expect(wrapper.text()).toContain('Innovation')
    })

    it('should show classifications', () => {
      const wrapper = mountComponent({ section: 'topics' })
      expect(wrapper.text()).toContain('comparison.contentClassifications')
    })

    it('should show lightbulb icon', () => {
      const wrapper = mountComponent({ section: 'topics' })
      expect(wrapper.find('.fa-lightbulb').exists()).toBe(true)
    })

    it('should show layer-group icon', () => {
      const wrapper = mountComponent({ section: 'topics' })
      expect(wrapper.find('.fa-layer-group').exists()).toBe(true)
    })
  })
})
