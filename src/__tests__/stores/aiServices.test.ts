import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'

// Mock the AI API
vi.mock('@/api/ai', () => ({
  aiApi: {
    summarizeContent: vi.fn(),
    translateContent: vi.fn(),
    extractEntities: vi.fn(),
    analyzeSentiment: vi.fn(),
    classifyContent: vi.fn(),
  },
}))

// Mock crypto.randomUUID
vi.stubGlobal('crypto', {
  randomUUID: () => 'test-uuid-' + Math.random().toString(36).substr(2, 9),
})

import { aiApi } from '@/api/ai'

describe('AI Services Store', () => {
  let store: any

  beforeEach(async () => {
    vi.resetModules()
    vi.clearAllMocks()
    localStorage.clear()

    setActivePinia(createPinia())

    const { useAIServicesStore } = await import('@/stores/aiServices')
    store = useAIServicesStore()
  })

  describe('Initial State', () => {
    it('should have empty operations', () => {
      expect(store.operations.size).toBe(0)
    })

    it('should have default settings', () => {
      expect(store.settings).toBeDefined()
      expect(store.settings.summarization).toBeDefined()
      expect(store.settings.translation).toBeDefined()
    })

    it('should have empty insights and recommendations', () => {
      expect(store.insights).toEqual([])
      expect(store.recommendations).toEqual([])
    })
  })

  describe('Computed Properties', () => {
    it('should return active operations', () => {
      store.operations.set('op-1', {
        id: 'op-1',
        type: 'summarize',
        status: 'processing',
      })
      store.operations.set('op-2', {
        id: 'op-2',
        type: 'translate',
        status: 'success',
      })

      expect(store.activeOperations.length).toBe(1)
      expect(store.activeOperations[0].id).toBe('op-1')
    })

    it('should detect when processing', () => {
      expect(store.isProcessing).toBe(false)

      store.operations.set('op-1', {
        id: 'op-1',
        type: 'summarize',
        status: 'processing',
      })

      expect(store.isProcessing).toBe(true)
    })

    it('should detect errors', () => {
      expect(store.hasErrors).toBe(false)

      store.operations.set('op-1', {
        id: 'op-1',
        type: 'summarize',
        status: 'error',
        error: 'Failed',
      })

      expect(store.hasErrors).toBe(true)
    })
  })

  describe('summarize', () => {
    it('should return null if summarization is disabled', async () => {
      store.settings.summarization.enabled = false

      const result = await store.summarize('Test content')

      expect(result).toBeNull()
      expect(aiApi.summarizeContent).not.toHaveBeenCalled()
    })

    it('should call API and return result', async () => {
      store.settings.summarization.enabled = true
      const mockResult = {
        summary: 'Test summary',
        keyPoints: ['Point 1'],
        confidence: 0.95,
      }
      vi.mocked(aiApi.summarizeContent).mockResolvedValue(mockResult)

      const result = await store.summarize('Test content', 'brief')

      expect(result).toEqual(mockResult)
      expect(aiApi.summarizeContent).toHaveBeenCalledWith('Test content', 'brief')
    })

    it('should return cached result on second call', async () => {
      store.settings.summarization.enabled = true
      const mockResult = { summary: 'Cached summary' }
      vi.mocked(aiApi.summarizeContent).mockResolvedValue(mockResult)

      await store.summarize('Test content', 'brief')
      await store.summarize('Test content', 'brief')

      expect(aiApi.summarizeContent).toHaveBeenCalledTimes(1)
    })

    it('should bypass cache when useCache is false', async () => {
      store.settings.summarization.enabled = true
      vi.mocked(aiApi.summarizeContent).mockResolvedValue({ summary: 'New' })

      await store.summarize('Test content', 'brief', true)
      await store.summarize('Test content', 'brief', false)

      expect(aiApi.summarizeContent).toHaveBeenCalledTimes(2)
    })

    it('should handle API errors', async () => {
      store.settings.summarization.enabled = true
      vi.mocked(aiApi.summarizeContent).mockRejectedValue(
        new Error('API Error')
      )

      const result = await store.summarize('Test content')

      expect(result).toBeNull()
      expect(store.hasErrors).toBe(true)
    })

    it('should track operation status', async () => {
      store.settings.summarization.enabled = true
      vi.mocked(aiApi.summarizeContent).mockResolvedValue({ summary: 'Test' })

      await store.summarize('Test content')

      const operations = Array.from(store.operations.values())
      const summarizeOp = operations.find((op: any) => op.type === 'summarize')
      expect(summarizeOp?.status).toBe('success')
    })
  })

  describe('translate', () => {
    it('should return null if translation is disabled', async () => {
      store.settings.translation.enabled = false

      const result = await store.translate('Test content', 'ar')

      expect(result).toBeNull()
    })

    it('should call API and cache result', async () => {
      store.settings.translation.enabled = true
      const mockResult = {
        translatedText: 'نص مترجم',
        sourceLanguage: 'en',
        targetLanguage: 'ar',
        confidence: 0.98,
      }
      vi.mocked(aiApi.translateContent).mockResolvedValue(mockResult)

      const result = await store.translate('Test content', 'ar')

      expect(result).toEqual(mockResult)
      expect(aiApi.translateContent).toHaveBeenCalledWith('Test content', 'ar')
    })
  })

  describe('extractEntities', () => {
    it('should return null if entity extraction is disabled', async () => {
      store.settings.entityExtraction.enabled = false

      const result = await store.extractEntities('Test content')

      expect(result).toBeNull()
    })

    it('should call API when enabled', async () => {
      store.settings.entityExtraction.enabled = true
      const mockResult = {
        entities: [
          { name: 'John', type: 'person', confidence: 0.95 },
        ],
      }
      vi.mocked(aiApi.extractEntities).mockResolvedValue(mockResult)

      const result = await store.extractEntities('John works at Google')

      expect(result).toEqual(mockResult)
    })
  })

  describe('Settings Persistence', () => {
    it('should load settings from localStorage', async () => {
      localStorage.setItem(
        'ai_settings',
        JSON.stringify({
          summarization: { enabled: false },
        })
      )

      // Re-import to get fresh store with loaded settings
      vi.resetModules()
      setActivePinia(createPinia())
      const { useAIServicesStore } = await import('@/stores/aiServices')
      const freshStore = useAIServicesStore()

      expect(freshStore.settings.summarization.enabled).toBe(false)
    })
  })

  describe('Operation Management', () => {
    it('should create operation with pending status', async () => {
      store.settings.summarization.enabled = true

      // Don't resolve the promise immediately
      let resolvePromise: any
      vi.mocked(aiApi.summarizeContent).mockImplementation(
        () =>
          new Promise((resolve) => {
            resolvePromise = resolve
          })
      )

      const promise = store.summarize('Test')

      // Check operation is created
      expect(store.operations.size).toBe(1)
      const op = Array.from(store.operations.values())[0] as any
      expect(op.status).toBe('processing')

      // Resolve and clean up
      resolvePromise({ summary: 'Done' })
      await promise
    })

    it('should track operation timestamps', async () => {
      store.settings.summarization.enabled = true
      vi.mocked(aiApi.summarizeContent).mockResolvedValue({ summary: 'Test' })

      await store.summarize('Test content')

      const operations = Array.from(store.operations.values())
      const op = operations[0] as any
      expect(op.startedAt).toBeDefined()
      expect(op.completedAt).toBeDefined()
    })
  })
})
