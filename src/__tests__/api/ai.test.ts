import { describe, it, expect, vi, beforeEach } from 'vitest'
import { aiApi } from '@/api/ai'

// Mock fetch for AI API tests
const mockFetch = vi.fn()
global.fetch = mockFetch

describe('AI API', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.useFakeTimers()
    localStorage.clear()
    localStorage.setItem('auth_token', 'test-token')
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  describe('getConversations', () => {
    it('should fetch all conversations', async () => {
      const mockConversations = [
        { id: '1', title: 'Conversation 1' },
        { id: '2', title: 'Conversation 2' },
      ]
      mockFetch.mockResolvedValue({
        json: () => Promise.resolve(mockConversations),
      })

      const result = await aiApi.getConversations()

      expect(mockFetch).toHaveBeenCalledWith(
        expect.stringContaining('/api/ai/conversations'),
        expect.objectContaining({
          headers: expect.objectContaining({
            Authorization: 'Bearer test-token',
          }),
        })
      )
      expect(result).toEqual(mockConversations)
    })
  })

  describe('getConversation', () => {
    it('should fetch single conversation by ID', async () => {
      const mockConversation = { id: 'conv-123', title: 'Test Conversation', messages: [] }
      mockFetch.mockResolvedValue({
        json: () => Promise.resolve(mockConversation),
      })

      const result = await aiApi.getConversation('conv-123')

      expect(mockFetch).toHaveBeenCalledWith(
        expect.stringContaining('/api/ai/conversations/conv-123'),
        expect.any(Object)
      )
      expect(result).toEqual(mockConversation)
    })
  })

  describe('deleteConversation', () => {
    it('should delete conversation', async () => {
      mockFetch.mockResolvedValue({})

      await aiApi.deleteConversation('conv-123')

      expect(mockFetch).toHaveBeenCalledWith(
        expect.stringContaining('/api/ai/conversations/conv-123'),
        expect.objectContaining({
          method: 'DELETE',
        })
      )
    })
  })

  describe('summarizeDocument', () => {
    it('should summarize document', async () => {
      const mockSummary = { summary: 'This is a summary of the document.' }
      mockFetch.mockResolvedValue({
        json: () => Promise.resolve(mockSummary),
      })

      const result = await aiApi.summarizeDocument('doc-123')

      expect(mockFetch).toHaveBeenCalledWith(
        expect.stringContaining('/api/ai/summarize'),
        expect.objectContaining({
          method: 'POST',
          body: JSON.stringify({ documentId: 'doc-123' }),
        })
      )
      expect(result).toEqual(mockSummary)
    })
  })

  describe('createMockMessage', () => {
    it('should create user message', () => {
      const message = aiApi.createMockMessage('Hello', 'user')

      expect(message).toMatchObject({
        role: 'user',
        content: 'Hello',
      })
      expect(message.id).toBeDefined()
      expect(message.timestamp).toBeDefined()
    })

    it('should create assistant message', () => {
      const message = aiApi.createMockMessage('Hi there!', 'assistant')

      expect(message).toMatchObject({
        role: 'assistant',
        content: 'Hi there!',
      })
    })
  })

  describe('summarizeContent', () => {
    it('should return brief summary', async () => {
      const promise = aiApi.summarizeContent('Some long content here', 'brief')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result).toHaveProperty('summary')
      expect(result).toHaveProperty('keyPoints')
      expect(result).toHaveProperty('wordCount')
      expect(result).toHaveProperty('compressionRatio')
      expect(result).toHaveProperty('processingTime')
    })

    it('should return detailed summary', async () => {
      const promise = aiApi.summarizeContent('Content to summarize', 'detailed')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result.summary.length).toBeGreaterThan(100)
    })

    it('should return bullet point summary', async () => {
      const promise = aiApi.summarizeContent('Content', 'bullet')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result.summary).toContain('•')
    })
  })

  describe('translateContent', () => {
    it('should translate to Arabic', async () => {
      const promise = aiApi.translateContent('Hello world', 'ar')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result).toHaveProperty('translatedText')
      expect(result.targetLanguage).toBe('ar')
      expect(result.sourceLanguage).toBe('en')
      expect(result).toHaveProperty('confidence')
    })

    it('should translate to French', async () => {
      const promise = aiApi.translateContent('Hello', 'fr')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result.targetLanguage).toBe('fr')
    })

    it('should translate to Spanish', async () => {
      const promise = aiApi.translateContent('Hello', 'es')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result.targetLanguage).toBe('es')
    })
  })

  describe('extractEntities', () => {
    it('should extract named entities', async () => {
      const promise = aiApi.extractEntities('Some content with entities')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result).toHaveProperty('entities')
      expect(result).toHaveProperty('processingTime')
      expect(Array.isArray(result.entities)).toBe(true)
    })

    it('should return entities with required fields', async () => {
      const promise = aiApi.extractEntities('Content')
      vi.advanceTimersByTime(2000)
      const result = await promise

      if (result.entities.length > 0) {
        const entity = result.entities[0]
        expect(entity).toHaveProperty('text')
        expect(entity).toHaveProperty('type')
        expect(entity).toHaveProperty('confidence')
      }
    })
  })

  describe('analyzeSentiment', () => {
    it('should analyze sentiment', async () => {
      const promise = aiApi.analyzeSentiment('Great content!')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result).toHaveProperty('overall')
      expect(result).toHaveProperty('score')
      expect(result).toHaveProperty('confidence')
      expect(result).toHaveProperty('emotions')
      expect(['positive', 'neutral', 'negative']).toContain(result.overall)
    })

    it('should include emotions array', async () => {
      const promise = aiApi.analyzeSentiment('Test content')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(Array.isArray(result.emotions)).toBe(true)
      result.emotions.forEach((emotion) => {
        expect(emotion).toHaveProperty('emotion')
        expect(emotion).toHaveProperty('score')
      })
    })
  })

  describe('classifyContent', () => {
    it('should classify content into categories', async () => {
      const promise = aiApi.classifyContent('Sports related content')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result).toHaveProperty('categories')
      expect(result).toHaveProperty('suggestedTags')
      expect(result).toHaveProperty('primaryCategory')
      expect(result).toHaveProperty('confidence')
      expect(Array.isArray(result.categories)).toBe(true)
      expect(Array.isArray(result.suggestedTags)).toBe(true)
    })
  })

  describe('generateTitles', () => {
    it('should generate title suggestions', async () => {
      const promise = aiApi.generateTitles('Article content about tournaments')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result).toHaveProperty('suggestions')
      expect(result).toHaveProperty('processingTime')
      expect(Array.isArray(result.suggestions)).toBe(true)
    })

    it('should return titles with styles', async () => {
      const promise = aiApi.generateTitles('Content')
      vi.advanceTimersByTime(2000)
      const result = await promise

      result.suggestions.forEach((suggestion) => {
        expect(suggestion).toHaveProperty('title')
        expect(suggestion).toHaveProperty('score')
        expect(suggestion).toHaveProperty('style')
      })
    })
  })

  describe('performOCR', () => {
    it('should perform OCR on file', async () => {
      const mockFile = new File(['content'], 'test.png', { type: 'image/png' })
      const promise = aiApi.performOCR(mockFile)
      vi.advanceTimersByTime(3000)
      const result = await promise

      expect(result).toHaveProperty('text')
      expect(result).toHaveProperty('blocks')
      expect(result).toHaveProperty('language')
      expect(result).toHaveProperty('confidence')
      expect(result.text).toContain('test.png')
    })
  })

  describe('autoTag', () => {
    it('should auto-generate tags', async () => {
      const promise = aiApi.autoTag('Content about cricket and tournaments')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result).toHaveProperty('tags')
      expect(result).toHaveProperty('processingTime')
      expect(Array.isArray(result.tags)).toBe(true)
    })

    it('should return tags with categories', async () => {
      const promise = aiApi.autoTag('Content')
      vi.advanceTimersByTime(2000)
      const result = await promise

      result.tags.forEach((tag) => {
        expect(tag).toHaveProperty('tag')
        expect(tag).toHaveProperty('confidence')
        expect(tag).toHaveProperty('category')
      })
    })
  })

  describe('smartSearch', () => {
    it('should perform smart search', async () => {
      const promise = aiApi.smartSearch('asia cup schedule')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result).toHaveProperty('intent')
      expect(result).toHaveProperty('suggestions')
      expect(result).toHaveProperty('processingTime')
    })

    it('should detect search intent', async () => {
      const promise = aiApi.smartSearch('find tournament')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result.intent).toHaveProperty('query')
      expect(result.intent).toHaveProperty('intent')
      expect(['find', 'learn', 'compare', 'navigate', 'answer']).toContain(result.intent.intent)
    })

    it('should provide search suggestions', async () => {
      const promise = aiApi.smartSearch('test')
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(Array.isArray(result.suggestions)).toBe(true)
      result.suggestions.forEach((suggestion) => {
        expect(suggestion).toHaveProperty('text')
        expect(suggestion).toHaveProperty('type')
        expect(suggestion).toHaveProperty('score')
      })
    })
  })

  describe('getRecommendations', () => {
    it('should get content recommendations', async () => {
      const promise = aiApi.getRecommendations()
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(Array.isArray(result)).toBe(true)
      expect(result.length).toBeLessThanOrEqual(5)
    })

    it('should respect limit parameter', async () => {
      const promise = aiApi.getRecommendations(undefined, 3)
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(result.length).toBeLessThanOrEqual(3)
    })

    it('should return recommendations with required fields', async () => {
      const promise = aiApi.getRecommendations()
      vi.advanceTimersByTime(2000)
      const result = await promise

      result.forEach((rec) => {
        expect(rec).toHaveProperty('id')
        expect(rec).toHaveProperty('contentType')
        expect(rec).toHaveProperty('title')
        expect(rec).toHaveProperty('description')
        expect(rec).toHaveProperty('relevanceScore')
        expect(rec).toHaveProperty('reason')
      })
    })
  })

  describe('getInsights', () => {
    it('should get AI insights', async () => {
      const promise = aiApi.getInsights()
      vi.advanceTimersByTime(2000)
      const result = await promise

      expect(Array.isArray(result)).toBe(true)
    })

    it('should return insights with required fields', async () => {
      const promise = aiApi.getInsights()
      vi.advanceTimersByTime(2000)
      const result = await promise

      result.forEach((insight) => {
        expect(insight).toHaveProperty('id')
        expect(insight).toHaveProperty('type')
        expect(insight).toHaveProperty('title')
        expect(insight).toHaveProperty('description')
        expect(insight).toHaveProperty('confidence')
        expect(insight).toHaveProperty('category')
        expect(insight).toHaveProperty('actionable')
        expect(['trend', 'anomaly', 'recommendation', 'prediction']).toContain(insight.type)
      })
    })
  })

  describe('streamChat', () => {
    it('should return abort controller', () => {
      mockFetch.mockImplementation(
        () =>
          new Promise(() => {
            // Never resolve to simulate streaming
          })
      )

      const onMessage = vi.fn()
      const controller = aiApi.streamChat('Hello', { onMessage })

      expect(controller).toBeInstanceOf(AbortController)
    })

    it('should call fetch with correct parameters', () => {
      mockFetch.mockImplementation(
        () =>
          new Promise(() => {
            // Never resolve
          })
      )

      const onMessage = vi.fn()
      aiApi.streamChat('Test message', {
        conversationId: 'conv-123',
        onMessage,
      })

      expect(mockFetch).toHaveBeenCalledWith(
        expect.any(URL),
        expect.objectContaining({
          method: 'POST',
          headers: expect.objectContaining({
            'Content-Type': 'application/json',
            Authorization: 'Bearer test-token',
          }),
          body: JSON.stringify({
            message: 'Test message',
            conversationId: 'conv-123',
          }),
        })
      )
    })
  })
})
