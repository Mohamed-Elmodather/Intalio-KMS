import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { ref } from 'vue'
import ArticleViewPage from '@/pages/articles/ArticleViewPage.vue'

// Mock vue-router
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
    back: vi.fn(),
  }),
  useRoute: () => ({
    params: { slug: 'test-article' },
  }),
}))

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock API
vi.mock('@/api', () => ({
  articlesApi: {
    getArticle: vi.fn(() => Promise.resolve({
      id: '1',
      slug: 'test-article',
      title: 'Test Article',
      content: '<p>Test content</p>',
      excerpt: 'Test excerpt',
      author: { id: 'a1', displayName: 'Test Author' },
      category: { id: 'c1', name: 'Tech' },
      tags: [],
      viewCount: 100,
      likeCount: 10,
      commentCount: 5,
      createdAt: new Date().toISOString(),
    })),
  },
}))

vi.mock('@/api/ai', () => ({
  aiApi: {
    summarizeContent: vi.fn(() => Promise.resolve({ summary: 'Summary', keyPoints: [], wordCount: 10 })),
    translateContent: vi.fn(() => Promise.resolve({ translatedText: 'Translated', confidence: 0.9 })),
    extractEntities: vi.fn(() => Promise.resolve({ entities: [] })),
    analyzeSentiment: vi.fn(() => Promise.resolve({ overall: 'positive', score: 0.8, confidence: 0.9 })),
  },
}))

// Mock components
vi.mock('@/components/common/LoadingSpinner.vue', () => ({
  default: { template: '<div class="loading-spinner"></div>' },
}))
vi.mock('@/components/common', () => ({
  CommentsSection: { template: '<div class="comments"></div>' },
  RatingStars: { template: '<div class="rating"></div>' },
  SocialShareButtons: { template: '<div class="share"></div>' },
  RelatedContentCarousel: { template: '<div class="related"></div>' },
  AuthorCard: { template: '<div class="author"></div>' },
  BookmarkButton: { template: '<button class="bookmark"></button>' },
  AddToCollectionModal: { template: '<div class="collection-modal"></div>' },
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AIActionButton: { template: '<button class="ai-action"></button>' },
  AIResultCard: { template: '<div class="ai-result"></div>' },
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AITranslateDropdown: { template: '<div class="ai-translate"></div>' },
  AIEntityHighlight: { template: '<span class="ai-entity"></span>' },
  AISentimentBadge: { template: '<span class="ai-sentiment"></span>' },
  AIConfidenceBar: { template: '<div class="ai-confidence"></div>' },
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
    rating: ref(null),
    submitRating: vi.fn(),
    loadRating: vi.fn(),
  }),
}))

vi.mock('@/composables/useRelatedContent', () => ({
  useRelatedContent: () => ({
    relatedItems: ref([]),
    isLoading: ref(false),
    loadRelatedContent: vi.fn(),
  }),
}))

function mountComponent() {
  return shallowMount(ArticleViewPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
        Transition: true,
        'router-link': true,
      },
    },
  })
}

describe('ArticleViewPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render article view page', () => {
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

    it('should have error state null initially', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.error).toBeNull()
    })
  })

  describe('Article State', () => {
    it('should have article ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // article is null initially before async load
      expect(vm.article).toBeDefined()
    })
  })

  describe('Table of Contents', () => {
    it('should have toc items array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.tocItems).toBeDefined()
      expect(Array.isArray(vm.tocItems)).toBe(true)
    })

    it('should have active toc id', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.activeTocId).toBe('')
    })

    it('should have show toc state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showToc).toBe(true)
    })
  })

  describe('Navigation', () => {
    it('should have previous article ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.previousArticle).toBeNull()
    })

    it('should have next article ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.nextArticle).toBeNull()
    })
  })

  describe('AI Features', () => {
    it('should have show AI panel state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showAIPanel).toBe(false)
    })

    it('should have active AI tab', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.activeAITab).toBe('summary')
    })

    it('should have summary result ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.summaryResult).toBeNull()
    })

    it('should have is summarizing state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isSummarizing).toBe(false)
    })

    it('should have summary type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.summaryType).toBe('brief')
    })

    it('should have translation result ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.translationResult).toBeNull()
    })

    it('should have is translating state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isTranslating).toBe(false)
    })

    it('should have target language', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.targetLanguage).toBe('ar')
    })

    it('should have entities result ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.entitiesResult).toBeNull()
    })

    it('should have is extracting entities state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isExtractingEntities).toBe(false)
    })

    it('should have show entities in content state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showEntitiesInContent).toBe(false)
    })
  })

  describe('Sentiment Analysis', () => {
    it('should have sentiment result ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sentimentResult).toBeNull()
    })

    it('should have is analyzing sentiment state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isAnalyzingSentiment).toBe(false)
    })
  })

  describe('Key Takeaways', () => {
    it('should have key takeaways array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.keyTakeaways).toBeDefined()
      expect(Array.isArray(vm.keyTakeaways)).toBe(true)
    })
  })

  describe('Collection Modal', () => {
    it('should have show add to collection state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showAddToCollection).toBe(false)
    })
  })

  describe('Computed Properties', () => {
    it('should have article content computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.articleContent).toBeDefined()
    })

    it('should have article plain text computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.articlePlainText).toBeDefined()
    })

    it('should have article author computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // articleAuthor is null when article is null
      expect(vm.articleAuthor).toBeDefined()
    })

    it('should have read time computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.readTime).toBeDefined()
    })

    it('should have ai insights computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiInsights).toBeDefined()
      expect(Array.isArray(vm.aiInsights)).toBe(true)
    })
  })

  describe('AI Functions', () => {
    it('should have generate summary function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.generateSummary).toBe('function')
    })

    it('should have translate article function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.translateArticle).toBe('function')
    })

    it('should have extract entities function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.extractEntities).toBe('function')
    })

    it('should have analyze sentiment function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.analyzeSentiment).toBe('function')
    })

    it('should have toggle AI panel function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.toggleAIPanel).toBe('function')
    })

    it('should have select AI tab function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.selectAITab).toBe('function')
    })
  })

  describe('Utility Functions', () => {
    it('should have copy to clipboard function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.copyToClipboard).toBe('function')
    })

    it('should have get language name function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getLanguageName).toBe('function')
    })

    it('should have format date function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.formatDate).toBe('function')
    })

    it('should have go back function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.goBack).toBe('function')
    })

    it('should have navigate to article function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.navigateToArticle).toBe('function')
    })
  })

  describe('TOC Functions', () => {
    it('should have generate TOC function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.generateTOC).toBe('function')
    })

    it('should have inject heading ids function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.injectHeadingIds).toBe('function')
    })

    it('should have scroll to heading function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.scrollToHeading).toBe('function')
    })

    it('should have handle scroll function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleScroll).toBe('function')
    })
  })

  describe('Rating Handler', () => {
    it('should have handle rating function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleRating).toBe('function')
    })
  })
})
