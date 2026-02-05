import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AIContentSuggestions from '@/components/ai/AIContentSuggestions.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string, params?: any) => params ? `${key} ${JSON.stringify(params)}` : key,
  }),
}))

const mockSuggestions = [
  {
    id: '1',
    type: 'article' as const,
    title: 'Test Article',
    description: 'A test article description',
    reason: 'Related to your search',
    relevanceScore: 0.85,
    suggestionType: 'related' as const,
    metadata: { author: 'John Doe', category: 'Tech', readTime: '5 min' },
  },
  {
    id: '2',
    type: 'document' as const,
    title: 'Test Document',
    description: 'A test document description',
    thumbnail: 'https://example.com/thumb.jpg',
    reason: 'Referenced in this content',
    relevanceScore: 0.72,
    suggestionType: 'referenced' as const,
    metadata: { date: '2024-01-15' },
  },
  {
    id: '3',
    type: 'media' as const,
    title: 'Test Media',
    reason: 'Similar topic',
    relevanceScore: 0.65,
    suggestionType: 'similar' as const,
  },
  {
    id: '4',
    type: 'course' as const,
    title: 'Test Course',
    reason: 'Trending now',
    relevanceScore: 0.55,
    suggestionType: 'trending' as const,
  },
  {
    id: '5',
    type: 'event' as const,
    title: 'Test Event',
    reason: 'Upcoming',
    relevanceScore: 0.45,
    suggestionType: 'related' as const,
  },
]

function mountComponent(props = {}) {
  return shallowMount(AIContentSuggestions, {
    props: {
      suggestions: mockSuggestions,
      ...props,
    },
    global: {
      mocks: {
        $t: (key: string) => key,
      },
      stubs: {
        TransitionGroup: true,
      },
    },
  })
}

describe('AIContentSuggestions', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.ai-content-suggestions').exists()).toBe(true)
    })

    it('should render header with lightbulb icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-lightbulb').exists()).toBe(true)
    })

    it('should render refresh button', () => {
      const wrapper = mountComponent({ loading: false })
      expect(wrapper.find('.fa-sync-alt').exists()).toBe(true)
    })

    it('should hide refresh button when loading', () => {
      const wrapper = mountComponent({ loading: true })
      expect(wrapper.find('.fa-sync-alt').exists()).toBe(false)
    })
  })

  describe('Title', () => {
    it('should show default title', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.displayTitle).toContain('ai.suggestions.youMightBeInterestedIn')
    })

    it('should show custom title when provided', () => {
      const wrapper = mountComponent({ title: 'Custom Title' })
      const vm = wrapper.vm as any
      expect(vm.displayTitle).toBe('Custom Title')
    })
  })

  describe('Loading State', () => {
    it('should show skeleton loaders when loading', () => {
      const wrapper = mountComponent({ loading: true })
      expect(wrapper.find('.suggestion-skeleton').exists()).toBe(true)
    })

    it('should show 3 skeleton items', () => {
      const wrapper = mountComponent({ loading: true })
      const skeletons = wrapper.findAll('.suggestion-skeleton')
      expect(skeletons.length).toBe(3)
    })
  })

  describe('Empty State', () => {
    it('should show empty state when no suggestions', () => {
      const wrapper = mountComponent({ suggestions: [] })
      expect(wrapper.find('.empty-state').exists()).toBe(true)
      expect(wrapper.text()).toContain('ai.suggestions.noSuggestions')
    })

    it('should show inbox icon in empty state', () => {
      const wrapper = mountComponent({ suggestions: [] })
      expect(wrapper.find('.fa-inbox').exists()).toBe(true)
    })
  })

  describe('Suggestions List', () => {
    it('should render suggestion cards', () => {
      const wrapper = mountComponent()
      const cards = wrapper.findAll('.suggestion-card')
      expect(cards.length).toBe(4) // maxVisible defaults to 4
    })

    it('should show suggestion titles', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Test Article')
      expect(wrapper.text()).toContain('Test Document')
    })

    it('should show suggestion reasons', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Related to your search')
    })
  })

  describe('Visible Suggestions', () => {
    it('should limit to maxVisible by default', () => {
      const wrapper = mountComponent({ maxVisible: 2 })
      const vm = wrapper.vm as any
      expect(vm.visibleSuggestions.length).toBe(2)
    })

    it('should show all when showAll is true', async () => {
      const wrapper = mountComponent({ maxVisible: 2 })
      const vm = wrapper.vm as any
      vm.showAll = true
      await wrapper.vm.$nextTick()
      expect(vm.visibleSuggestions.length).toBe(5)
    })

    it('should compute hasMore correctly', () => {
      const wrapper = mountComponent({ maxVisible: 3 })
      const vm = wrapper.vm as any
      expect(vm.hasMore).toBe(true)
    })

    it('should not have more when all are visible', () => {
      const wrapper = mountComponent({ maxVisible: 10 })
      const vm = wrapper.vm as any
      expect(vm.hasMore).toBe(false)
    })
  })

  describe('Type Icons', () => {
    it('should return article icon', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTypeIcon('article')).toBe('fas fa-newspaper')
    })

    it('should return document icon', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTypeIcon('document')).toBe('fas fa-file-pdf')
    })

    it('should return media icon', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTypeIcon('media')).toBe('fas fa-photo-video')
    })

    it('should return course icon', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTypeIcon('course')).toBe('fas fa-graduation-cap')
    })

    it('should return event icon', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTypeIcon('event')).toBe('fas fa-calendar-alt')
    })

    it('should return default icon for unknown type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTypeIcon('unknown')).toBe('fas fa-file')
    })
  })

  describe('Type Colors', () => {
    it('should return article colors', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTypeColor('article')).toContain('bg-blue-100')
    })

    it('should return document colors', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTypeColor('document')).toContain('bg-red-100')
    })

    it('should return default colors for unknown type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getTypeColor('unknown')).toContain('bg-gray-100')
    })
  })

  describe('Suggestion Type Badges', () => {
    it('should return related badge', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const badge = vm.getSuggestionTypeBadge('related')
      expect(badge.color).toContain('bg-teal-100')
      expect(badge.icon).toBe('fas fa-link')
    })

    it('should return referenced badge', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const badge = vm.getSuggestionTypeBadge('referenced')
      expect(badge.color).toContain('bg-purple-100')
      expect(badge.icon).toBe('fas fa-quote-right')
    })

    it('should return similar badge', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const badge = vm.getSuggestionTypeBadge('similar')
      expect(badge.color).toContain('bg-blue-100')
      expect(badge.icon).toBe('fas fa-clone')
    })

    it('should return trending badge', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const badge = vm.getSuggestionTypeBadge('trending')
      expect(badge.color).toContain('bg-orange-100')
      expect(badge.icon).toBe('fas fa-fire')
    })

    it('should return default badge for unknown type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const badge = vm.getSuggestionTypeBadge('unknown')
      expect(badge.color).toContain('bg-gray-100')
      expect(badge.icon).toBe('fas fa-lightbulb')
    })
  })

  describe('Relevance Colors', () => {
    it('should return green for high relevance', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getRelevanceColor(0.85)).toBe('text-green-600')
    })

    it('should return teal for good relevance', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getRelevanceColor(0.65)).toBe('text-teal-600')
    })

    it('should return amber for moderate relevance', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getRelevanceColor(0.45)).toBe('text-amber-600')
    })

    it('should return gray for low relevance', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getRelevanceColor(0.3)).toBe('text-gray-500')
    })
  })

  describe('Events', () => {
    it('should emit refresh when refresh clicked', async () => {
      const wrapper = mountComponent({ loading: false })
      const refreshBtn = wrapper.find('.fa-sync-alt').element.parentElement
      await refreshBtn?.click()
      expect(wrapper.emitted('refresh')).toBeTruthy()
    })

    it('should emit select when suggestion clicked', async () => {
      const wrapper = mountComponent()
      const selectBtn = wrapper.findAll('button').find(b => b.text().includes('Test Article'))
      await selectBtn?.trigger('click')
      expect(wrapper.emitted('select')).toBeTruthy()
    })

    it('should emit dismiss when dismiss clicked', async () => {
      const wrapper = mountComponent()
      const dismissBtn = wrapper.find('.fa-times.text-xs').element.parentElement
      await dismissBtn?.click()
      expect(wrapper.emitted('dismiss')).toBeTruthy()
    })
  })

  describe('Show More/Less', () => {
    it('should show "show more" button when hasMore', () => {
      const wrapper = mountComponent({ maxVisible: 2 })
      expect(wrapper.text()).toContain('ai.suggestions.showMore')
    })

    it('should toggle showAll when button clicked', async () => {
      const wrapper = mountComponent({ maxVisible: 2 })
      const vm = wrapper.vm as any
      expect(vm.showAll).toBe(false)

      const showMoreBtn = wrapper.findAll('button').find(b => b.text().includes('ai.suggestions.showMore'))
      await showMoreBtn?.trigger('click')
      expect(vm.showAll).toBe(true)
    })
  })

  describe('Relevance Display', () => {
    it('should show relevance when showRelevance is true', () => {
      const wrapper = mountComponent({ showRelevance: true })
      expect(wrapper.find('.fa-chart-line').exists()).toBe(true)
    })

    it('should hide relevance when showRelevance is false', () => {
      const wrapper = mountComponent({ showRelevance: false })
      expect(wrapper.find('.fa-chart-line').exists()).toBe(false)
    })
  })

  describe('Thumbnail Display', () => {
    it('should show thumbnail image when available', () => {
      const wrapper = mountComponent()
      const img = wrapper.find('img')
      expect(img.exists()).toBe(true)
    })
  })
})
