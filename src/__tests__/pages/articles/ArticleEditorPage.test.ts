import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import ArticleEditorPage from '@/pages/articles/ArticleEditorPage.vue'

// Mock vue-router
const mockPush = vi.fn()
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: mockPush,
    back: vi.fn(),
  }),
  onBeforeRouteLeave: vi.fn(),
}))

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock AI API
vi.mock('@/api/ai', () => ({
  aiApi: {
    generateTitles: vi.fn(),
    autoTag: vi.fn(),
    classifyContent: vi.fn(),
    summarizeContent: vi.fn(),
    analyzeSentiment: vi.fn(),
  },
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AIActionButton: { template: '<button class="ai-action"></button>' },
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AISuggestionChip: { template: '<span class="ai-suggestion"></span>' },
  AIConfidenceBar: { template: '<div class="ai-confidence"></div>' },
  AISentimentBadge: { template: '<div class="ai-sentiment"></div>' },
}))

// Mock stores
vi.mock('@/stores/ui', () => ({
  useUIStore: () => ({
    showWarning: vi.fn(),
    showError: vi.fn(),
    showSuccess: vi.fn(),
  }),
}))

// Mock localStorage
const mockLocalStorage = {
  store: {} as Record<string, string>,
  getItem: vi.fn((key: string) => mockLocalStorage.store[key] || null),
  setItem: vi.fn((key: string, value: string) => {
    mockLocalStorage.store[key] = value
  }),
  removeItem: vi.fn((key: string) => {
    delete mockLocalStorage.store[key]
  }),
  clear: vi.fn(() => {
    mockLocalStorage.store = {}
  }),
}

Object.defineProperty(window, 'localStorage', { value: mockLocalStorage })

// Mock document methods for rich text editor
document.queryCommandState = vi.fn(() => false)
document.execCommand = vi.fn(() => true)

function mountComponent() {
  return shallowMount(ArticleEditorPage, {
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

describe('ArticleEditorPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
    mockLocalStorage.clear()
    vi.useFakeTimers()
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  describe('Rendering', () => {
    it('should render article editor page', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Form State', () => {
    it('should have title field', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.title).toBe('')
    })

    it('should have excerpt field', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.excerpt).toBe('')
    })

    it('should have content field', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.content).toBe('')
    })

    it('should have category field', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.category).toBe('')
    })

    it('should have tags array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.tags).toBeDefined()
      expect(Array.isArray(vm.tags)).toBe(true)
      expect(vm.tags.length).toBe(0)
    })

    it('should have cover image field', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.coverImage).toBe('')
    })

    it('should have is draft state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isDraft).toBe(true)
    })

    it('should have is submitting state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isSubmitting).toBe(false)
    })
  })

  describe('Rich Text Editor State', () => {
    it('should have is markdown mode state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isMarkdownMode).toBe(false)
    })

    it('should have show preview state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showPreview).toBe(false)
    })

    it('should have editor ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.editorRef).toBeDefined()
    })

    it('should have show heading dropdown state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showHeadingDropdown).toBe(false)
    })
  })

  describe('AI Title Features', () => {
    it('should have show AI title suggestions state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showAITitleSuggestions).toBe(false)
    })

    it('should have title suggestions ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.titleSuggestions).toBeNull()
    })

    it('should have is generating titles state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isGeneratingTitles).toBe(false)
    })
  })

  describe('AI Tag Features', () => {
    it('should have show AI tag suggestions state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showAITagSuggestions).toBe(false)
    })

    it('should have tag suggestions ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.tagSuggestions).toBeNull()
    })

    it('should have is generating tags state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isGeneratingTags).toBe(false)
    })
  })

  describe('AI Classification Features', () => {
    it('should have show AI classification state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showAIClassification).toBe(false)
    })

    it('should have classification result ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.classificationResult).toBeNull()
    })

    it('should have is classifying state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isClassifying).toBe(false)
    })
  })

  describe('AI Summary Features', () => {
    it('should have is summarizing state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isSummarizing).toBe(false)
    })

    it('should have summary result ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.summaryResult).toBeNull()
    })
  })

  describe('AI SEO Features', () => {
    it('should have is analyzing SEO state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isAnalyzingSEO).toBe(false)
    })

    it('should have SEO result ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.seoResult).toBeNull()
    })
  })

  describe('AI Sentiment Features', () => {
    it('should have is analyzing sentiment state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isAnalyzingSentiment).toBe(false)
    })

    it('should have sentiment result ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.sentimentResult).toBeNull()
    })
  })

  describe('Form Enhancement State', () => {
    it('should have touched fields set', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.touchedFields).toBeDefined()
      expect(vm.touchedFields.size).toBe(0)
    })

    it('should have is dragging state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isDragging).toBe(false)
    })

    it('should have cover image preview ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.coverImagePreview).toBeNull()
    })

    it('should have tag input field', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.tagInput).toBe('')
    })

    it('should have show tag suggestions state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showTagSuggestions).toBe(false)
    })

    it('should have show url input state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showUrlInput).toBe(false)
    })
  })

  describe('Auto-save State', () => {
    it('should have has unsaved changes state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.hasUnsavedChanges).toBe(false)
    })

    it('should have last auto saved ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.lastAutoSaved).toBeNull()
    })
  })

  describe('Computed Properties', () => {
    it('should have has enough content computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.hasEnoughContent).toBe(false)
    })

    it('should return true when content is 50+ chars', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.content = 'a'.repeat(50)
      await wrapper.vm.$nextTick()
      expect(vm.hasEnoughContent).toBe(true)
    })

    it('should have word count computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.wordCount).toBe(0)
    })

    it('should calculate word count correctly', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.content = 'This is a test with seven words'
      await wrapper.vm.$nextTick()
      expect(vm.wordCount).toBe(7)
    })

    it('should have character count computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.characterCount).toBe(0)
    })

    it('should have reading time computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.readingTime).toBe(1) // Minimum 1 minute
    })

    it('should have is any AI processing computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isAnyAIProcessing).toBe(false)
    })

    it('should have show AI results computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showAIResults).toBeFalsy()
    })

    it('should have content checklist computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.contentChecklist).toBeDefined()
      expect(Array.isArray(vm.contentChecklist)).toBe(true)
      expect(vm.contentChecklist.length).toBe(5)
    })

    it('should have categories computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.categories).toBeDefined()
      expect(Array.isArray(vm.categories)).toBe(true)
    })

    it('should have filtered existing tags computed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.filteredExistingTags).toBeDefined()
      expect(Array.isArray(vm.filteredExistingTags)).toBe(true)
    })
  })

  describe('Functions', () => {
    it('should have get field error function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getFieldError).toBe('function')
    })

    it('should have exec command function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.execCommand).toBe('function')
    })

    it('should have is active function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.isActive).toBe('function')
    })

    it('should have set heading function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.setHeading).toBe('function')
    })

    it('should have set paragraph function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.setParagraph).toBe('function')
    })

    it('should have insert link function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.insertLink).toBe('function')
    })

    it('should have on editor input function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.onEditorInput).toBe('function')
    })

    it('should have handle keydown function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleKeydown).toBe('function')
    })
  })

  describe('AI Functions', () => {
    it('should have generate title suggestions function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.generateTitleSuggestions).toBe('function')
    })

    it('should have generate tag suggestions function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.generateTagSuggestions).toBe('function')
    })

    it('should have classify content function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.classifyContent).toBe('function')
    })

    it('should have generate summary function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.generateSummary).toBe('function')
    })

    it('should have analyze SEO function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.analyzeSEO).toBe('function')
    })

    it('should have analyze sentiment function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.analyzeSentiment).toBe('function')
    })

    it('should have get SEO score class function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getSEOScoreClass).toBe('function')
    })

    it('should return correct SEO score classes', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getSEOScoreClass(90)).toBe('text-green-600')
      expect(vm.getSEOScoreClass(70)).toBe('text-teal-600')
      expect(vm.getSEOScoreClass(50)).toBe('text-amber-600')
      expect(vm.getSEOScoreClass(30)).toBe('text-red-600')
    })
  })

  describe('Tag Functions', () => {
    it('should have select title function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.selectTitle).toBe('function')
    })

    it('should have add tag function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.addTag).toBe('function')
    })

    it('should add tag to tags array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.addTag('test-tag')
      expect(vm.tags).toContain('test-tag')
    })

    it('should normalize tag to lowercase', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.addTag('TEST-TAG')
      expect(vm.tags).toContain('test-tag')
    })

    it('should not add duplicate tags', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.addTag('test-tag')
      vm.addTag('test-tag')
      expect(vm.tags.length).toBe(1)
    })

    it('should have add custom tag function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.addCustomTag).toBe('function')
    })

    it('should have remove tag function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.removeTag).toBe('function')
    })

    it('should remove tag from tags array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.addTag('test-tag')
      expect(vm.tags.length).toBe(1)
      vm.removeTag('test-tag')
      expect(vm.tags.length).toBe(0)
    })

    it('should have handle backspace function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleBackspace).toBe('function')
    })

    it('should have hide tag suggestions function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.hideTagSuggestions).toBe('function')
    })

    it('should have apply all suggested tags function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.applyAllSuggestedTags).toBe('function')
    })
  })

  describe('Image Functions', () => {
    it('should have handle image drop function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleImageDrop).toBe('function')
    })

    it('should have handle image select function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleImageSelect).toBe('function')
    })

    it('should have process image file function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.processImageFile).toBe('function')
    })

    it('should have remove image function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.removeImage).toBe('function')
    })

    it('should clear image on remove', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.coverImagePreview = 'data:image/png;base64,test'
      vm.coverImage = 'https://example.com/image.jpg'
      vm.removeImage()
      expect(vm.coverImagePreview).toBeNull()
      expect(vm.coverImage).toBe('')
    })
  })

  describe('Auto-save Functions', () => {
    it('should have auto save draft function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.autoSaveDraft).toBe('function')
    })

    it('should have load draft function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.loadDraft).toBe('function')
    })
  })

  describe('Save Functions', () => {
    it('should have save article function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.saveArticle).toBe('function')
    })

    it('should have go back function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.goBack).toBe('function')
    })

    it('should navigate to articles on go back', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.goBack()
      expect(mockPush).toHaveBeenCalledWith({ name: 'Articles' })
    })
  })

  describe('Validation', () => {
    it('should return null for untouched fields', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getFieldError('title')).toBeNull()
    })

    it('should return error for empty title when touched', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.touchedFields.add('title')
      expect(vm.getFieldError('title')).toBe('articles.titleRequired')
    })

    it('should return error for short title', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.title = 'Short'
      vm.touchedFields.add('title')
      expect(vm.getFieldError('title')).toBe('articles.titleMinLength')
    })

    it('should return error for long title', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.title = 'a'.repeat(101)
      vm.touchedFields.add('title')
      expect(vm.getFieldError('title')).toBe('articles.titleMaxLength')
    })

    it('should return error for long excerpt', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.excerpt = 'a'.repeat(201)
      vm.touchedFields.add('excerpt')
      expect(vm.getFieldError('excerpt')).toBe('articles.excerptMaxLength')
    })
  })

  describe('Content Checklist', () => {
    it('should mark title as incomplete when short', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.title = 'Short'
      const titleItem = vm.contentChecklist.find((item: any) => item.icon === 'fa-heading')
      expect(titleItem.complete).toBe(false)
    })

    it('should mark title as complete when 10+ chars', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.title = 'This is a long enough title'
      await wrapper.vm.$nextTick()
      const titleItem = vm.contentChecklist.find((item: any) => item.icon === 'fa-heading')
      expect(titleItem.complete).toBe(true)
    })

    it('should mark category as incomplete when empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const categoryItem = vm.contentChecklist.find((item: any) => item.icon === 'fa-folder')
      expect(categoryItem.complete).toBe(false)
    })

    it('should mark tags as incomplete when less than 3', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.tags = ['tag1', 'tag2']
      const tagsItem = vm.contentChecklist.find((item: any) => item.icon === 'fa-tags')
      expect(tagsItem.complete).toBe(false)
    })

    it('should mark tags as complete when 3+', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.tags = ['tag1', 'tag2', 'tag3']
      await wrapper.vm.$nextTick()
      const tagsItem = vm.contentChecklist.find((item: any) => item.icon === 'fa-tags')
      expect(tagsItem.complete).toBe(true)
    })
  })
})
