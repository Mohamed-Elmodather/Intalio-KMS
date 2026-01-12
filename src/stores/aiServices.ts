import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { aiApi } from '@/api/ai'
import type {
  AIOperation,
  AIOperationType,
  AISettings,
  NERResult,
  SentimentResult,
  ClassificationResult,
  OCRResult,
  SummarizationResult,
  TranslationResult,
  TitleGenerationResult,
  AutoTagResult,
  SmartSearchResult,
  SummaryType,
  SupportedLanguage,
  AIInsight,
  ContentRecommendation,
} from '@/types/ai'
import { DEFAULT_AI_SETTINGS } from '@/types/ai'

export const useAIServicesStore = defineStore('aiServices', () => {
  // ============================================================================
  // State
  // ============================================================================

  // Active AI operations
  const operations = ref<Map<string, AIOperation>>(new Map())

  // Cached results (keyed by content hash or ID)
  const summaryCache = ref<Map<string, SummarizationResult>>(new Map())
  const translationCache = ref<Map<string, TranslationResult>>(new Map())
  const entityCache = ref<Map<string, NERResult>>(new Map())
  const sentimentCache = ref<Map<string, SentimentResult>>(new Map())
  const classificationCache = ref<Map<string, ClassificationResult>>(new Map())

  // User settings
  const settings = ref<AISettings>(loadSettings())

  // AI Insights & Recommendations
  const insights = ref<AIInsight[]>([])
  const recommendations = ref<ContentRecommendation[]>([])

  // ============================================================================
  // Computed
  // ============================================================================

  const activeOperations = computed(() =>
    Array.from(operations.value.values()).filter(
      op => op.status === 'pending' || op.status === 'processing'
    )
  )

  const isProcessing = computed(() => activeOperations.value.length > 0)

  const hasErrors = computed(() =>
    Array.from(operations.value.values()).some(op => op.status === 'error')
  )

  // ============================================================================
  // Helper Functions
  // ============================================================================

  function loadSettings(): AISettings {
    const stored = localStorage.getItem('ai_settings')
    if (stored) {
      try {
        return JSON.parse(stored) as AISettings
      } catch {
        return { ...DEFAULT_AI_SETTINGS }
      }
    }
    return { ...DEFAULT_AI_SETTINGS }
  }

  function saveSettings() {
    localStorage.setItem('ai_settings', JSON.stringify(settings.value))
  }

  function createOperation(type: AIOperationType): string {
    const id = crypto.randomUUID()
    const operation: AIOperation = {
      id,
      type,
      status: 'pending',
      startedAt: new Date().toISOString(),
    }
    operations.value.set(id, operation)
    return id
  }

  function updateOperation(id: string, updates: Partial<AIOperation>) {
    const op = operations.value.get(id)
    if (op) {
      operations.value.set(id, { ...op, ...updates })
    }
  }

  function generateCacheKey(content: string, ...args: string[]): string {
    // Simple hash function for cache key
    const str = content + args.join('|')
    let hash = 0
    for (let i = 0; i < str.length; i++) {
      const char = str.charCodeAt(i)
      hash = ((hash << 5) - hash) + char
      hash = hash & hash
    }
    return hash.toString(36)
  }

  // ============================================================================
  // AI Service Actions
  // ============================================================================

  /**
   * Summarize content
   */
  async function summarize(
    content: string,
    type: SummaryType = 'brief',
    useCache: boolean = true
  ): Promise<SummarizationResult | null> {
    if (!settings.value.summarization.enabled) return null

    const cacheKey = generateCacheKey(content, type)
    if (useCache && summaryCache.value.has(cacheKey)) {
      return summaryCache.value.get(cacheKey)!
    }

    const opId = createOperation('summarize')
    updateOperation(opId, { status: 'processing' })

    try {
      const result = await aiApi.summarizeContent(content, type)
      summaryCache.value.set(cacheKey, result)
      updateOperation(opId, { status: 'success', result, completedAt: new Date().toISOString() })
      return result
    } catch (error) {
      updateOperation(opId, {
        status: 'error',
        error: error instanceof Error ? error.message : 'Summarization failed',
        completedAt: new Date().toISOString(),
      })
      return null
    }
  }

  /**
   * Translate content
   */
  async function translate(
    content: string,
    targetLang: SupportedLanguage,
    useCache: boolean = true
  ): Promise<TranslationResult | null> {
    if (!settings.value.translation.enabled) return null

    const cacheKey = generateCacheKey(content, targetLang)
    if (useCache && translationCache.value.has(cacheKey)) {
      return translationCache.value.get(cacheKey)!
    }

    const opId = createOperation('translate')
    updateOperation(opId, { status: 'processing' })

    try {
      const result = await aiApi.translateContent(content, targetLang)
      translationCache.value.set(cacheKey, result)
      updateOperation(opId, { status: 'success', result, completedAt: new Date().toISOString() })
      return result
    } catch (error) {
      updateOperation(opId, {
        status: 'error',
        error: error instanceof Error ? error.message : 'Translation failed',
        completedAt: new Date().toISOString(),
      })
      return null
    }
  }

  /**
   * Extract named entities
   */
  async function extractEntities(
    content: string,
    useCache: boolean = true
  ): Promise<NERResult | null> {
    if (!settings.value.entityExtraction.enabled) return null

    const cacheKey = generateCacheKey(content)
    if (useCache && entityCache.value.has(cacheKey)) {
      return entityCache.value.get(cacheKey)!
    }

    const opId = createOperation('extract-entities')
    updateOperation(opId, { status: 'processing' })

    try {
      const result = await aiApi.extractEntities(content)
      entityCache.value.set(cacheKey, result)
      updateOperation(opId, { status: 'success', result, completedAt: new Date().toISOString() })
      return result
    } catch (error) {
      updateOperation(opId, {
        status: 'error',
        error: error instanceof Error ? error.message : 'Entity extraction failed',
        completedAt: new Date().toISOString(),
      })
      return null
    }
  }

  /**
   * Analyze sentiment
   */
  async function analyzeSentiment(
    content: string,
    useCache: boolean = true
  ): Promise<SentimentResult | null> {
    if (!settings.value.sentimentAnalysis.enabled) return null

    const cacheKey = generateCacheKey(content)
    if (useCache && sentimentCache.value.has(cacheKey)) {
      return sentimentCache.value.get(cacheKey)!
    }

    const opId = createOperation('analyze-sentiment')
    updateOperation(opId, { status: 'processing' })

    try {
      const result = await aiApi.analyzeSentiment(content)
      sentimentCache.value.set(cacheKey, result)
      updateOperation(opId, { status: 'success', result, completedAt: new Date().toISOString() })
      return result
    } catch (error) {
      updateOperation(opId, {
        status: 'error',
        error: error instanceof Error ? error.message : 'Sentiment analysis failed',
        completedAt: new Date().toISOString(),
      })
      return null
    }
  }

  /**
   * Classify content
   */
  async function classify(
    content: string,
    useCache: boolean = true
  ): Promise<ClassificationResult | null> {
    if (!settings.value.classification.enabled) return null

    const cacheKey = generateCacheKey(content)
    if (useCache && classificationCache.value.has(cacheKey)) {
      return classificationCache.value.get(cacheKey)!
    }

    const opId = createOperation('classify')
    updateOperation(opId, { status: 'processing' })

    try {
      const result = await aiApi.classifyContent(content)
      classificationCache.value.set(cacheKey, result)
      updateOperation(opId, { status: 'success', result, completedAt: new Date().toISOString() })
      return result
    } catch (error) {
      updateOperation(opId, {
        status: 'error',
        error: error instanceof Error ? error.message : 'Classification failed',
        completedAt: new Date().toISOString(),
      })
      return null
    }
  }

  /**
   * Generate title suggestions
   */
  async function generateTitles(content: string): Promise<TitleGenerationResult | null> {
    if (!settings.value.titleGeneration.enabled) return null

    const opId = createOperation('generate-title')
    updateOperation(opId, { status: 'processing' })

    try {
      const result = await aiApi.generateTitles(content)
      updateOperation(opId, { status: 'success', result, completedAt: new Date().toISOString() })
      return result
    } catch (error) {
      updateOperation(opId, {
        status: 'error',
        error: error instanceof Error ? error.message : 'Title generation failed',
        completedAt: new Date().toISOString(),
      })
      return null
    }
  }

  /**
   * Perform OCR on file
   */
  async function performOCR(file: File): Promise<OCRResult | null> {
    if (!settings.value.ocr.enabled) return null

    const opId = createOperation('ocr')
    updateOperation(opId, { status: 'processing' })

    try {
      const result = await aiApi.performOCR(file)
      updateOperation(opId, { status: 'success', result, completedAt: new Date().toISOString() })
      return result
    } catch (error) {
      updateOperation(opId, {
        status: 'error',
        error: error instanceof Error ? error.message : 'OCR failed',
        completedAt: new Date().toISOString(),
      })
      return null
    }
  }

  /**
   * Auto-tag content
   */
  async function autoTag(content: string): Promise<AutoTagResult | null> {
    if (!settings.value.autoTagging.enabled) return null

    const opId = createOperation('auto-tag')
    updateOperation(opId, { status: 'processing' })

    try {
      const result = await aiApi.autoTag(content)
      updateOperation(opId, { status: 'success', result, completedAt: new Date().toISOString() })
      return result
    } catch (error) {
      updateOperation(opId, {
        status: 'error',
        error: error instanceof Error ? error.message : 'Auto-tagging failed',
        completedAt: new Date().toISOString(),
      })
      return null
    }
  }

  /**
   * Smart search
   */
  async function smartSearch(query: string): Promise<SmartSearchResult | null> {
    if (!settings.value.smartSearch.enabled) return null

    const opId = createOperation('smart-search')
    updateOperation(opId, { status: 'processing' })

    try {
      const result = await aiApi.smartSearch(query)
      updateOperation(opId, { status: 'success', result, completedAt: new Date().toISOString() })
      return result
    } catch (error) {
      updateOperation(opId, {
        status: 'error',
        error: error instanceof Error ? error.message : 'Smart search failed',
        completedAt: new Date().toISOString(),
      })
      return null
    }
  }

  /**
   * Fetch AI insights
   */
  async function fetchInsights(): Promise<void> {
    try {
      const result = await aiApi.getInsights()
      insights.value = result.map(i => ({
        ...i,
        createdAt: new Date().toISOString(),
      }))
    } catch (error) {
      console.error('Failed to fetch AI insights:', error)
    }
  }

  /**
   * Fetch recommendations
   */
  async function fetchRecommendations(contentType?: string, limit?: number): Promise<void> {
    try {
      const result = await aiApi.getRecommendations(contentType, limit)
      recommendations.value = result.map(r => ({
        id: r.id,
        contentType: r.contentType as 'article' | 'document' | 'course' | 'event',
        title: r.title,
        description: r.description,
        relevanceScore: r.relevanceScore,
        reason: r.reason,
        url: `/${r.contentType}s/${r.id}`,
      }))
    } catch (error) {
      console.error('Failed to fetch recommendations:', error)
    }
  }

  // ============================================================================
  // Settings Actions
  // ============================================================================

  function updateSettings(newSettings: Partial<AISettings>) {
    settings.value = { ...settings.value, ...newSettings }
    saveSettings()
  }

  function toggleFeature(feature: keyof AISettings, enabled: boolean) {
    if (settings.value[feature]) {
      settings.value[feature].enabled = enabled
      saveSettings()
    }
  }

  function resetSettings() {
    settings.value = { ...DEFAULT_AI_SETTINGS }
    saveSettings()
  }

  // ============================================================================
  // Cache Actions
  // ============================================================================

  function clearCache(type?: AIOperationType) {
    if (type) {
      switch (type) {
        case 'summarize':
          summaryCache.value.clear()
          break
        case 'translate':
          translationCache.value.clear()
          break
        case 'extract-entities':
          entityCache.value.clear()
          break
        case 'analyze-sentiment':
          sentimentCache.value.clear()
          break
        case 'classify':
          classificationCache.value.clear()
          break
      }
    } else {
      summaryCache.value.clear()
      translationCache.value.clear()
      entityCache.value.clear()
      sentimentCache.value.clear()
      classificationCache.value.clear()
    }
  }

  function clearOperations() {
    operations.value.clear()
  }

  function getOperation(id: string): AIOperation | undefined {
    return operations.value.get(id)
  }

  function dismissOperation(id: string) {
    operations.value.delete(id)
  }

  // ============================================================================
  // Return
  // ============================================================================

  return {
    // State
    operations,
    settings,
    insights,
    recommendations,

    // Computed
    activeOperations,
    isProcessing,
    hasErrors,

    // AI Service Actions
    summarize,
    translate,
    extractEntities,
    analyzeSentiment,
    classify,
    generateTitles,
    performOCR,
    autoTag,
    smartSearch,
    fetchInsights,
    fetchRecommendations,

    // Settings Actions
    updateSettings,
    toggleFeature,
    resetSettings,

    // Cache & Operation Actions
    clearCache,
    clearOperations,
    getOperation,
    dismissOperation,
  }
})
