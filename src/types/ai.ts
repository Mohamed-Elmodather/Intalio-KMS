// AI Service Types for Intalio AI Engine Integration

// ============================================================================
// Entity Recognition (NER)
// ============================================================================

export type EntityType = 'person' | 'organization' | 'location' | 'date' | 'event' | 'product' | 'amount' | 'money'

export interface Entity {
  text: string
  type: EntityType
  confidence: number
  // Support both naming conventions
  startIndex?: number
  endIndex?: number
  startOffset?: number
  endOffset?: number
}

// Alias for backwards compatibility
export type NamedEntity = Entity

export interface NERResult {
  entities: Entity[]
  processingTime: number
  modelVersion?: string
}

// ============================================================================
// Sentiment Analysis
// ============================================================================

export type SentimentType = 'positive' | 'neutral' | 'negative'
export type EmotionType = 'joy' | 'sadness' | 'anger' | 'fear' | 'surprise' | 'disgust' | 'trust' | 'anticipation'

export interface EmotionScore {
  emotion: EmotionType
  score: number
}

export interface SentimentResult {
  overall: SentimentType
  score: number // -1 to 1
  confidence: number
  emotions: EmotionScore[]
  processingTime: number
}

// ============================================================================
// Content Classification
// ============================================================================

export interface ClassificationCategory {
  name: string
  confidence: number
  id?: string
}

export interface ClassificationResult {
  categories: ClassificationCategory[]
  suggestedTags: string[]
  primaryCategory: string
  confidence: number
  processingTime: number
}

// ============================================================================
// OCR (Optical Character Recognition)
// ============================================================================

export interface OCRTextBlock {
  text: string
  confidence: number
  boundingBox?: {
    x: number
    y: number
    width: number
    height: number
  }
}

export interface OCRResult {
  text: string
  blocks: OCRTextBlock[]
  language: string
  confidence: number
  processingTime: number
  pages?: number
  wordCount?: number
  characterCount?: number
}

// ============================================================================
// Summarization
// ============================================================================

export type SummaryType = 'brief' | 'detailed' | 'bullet'

export interface SummarizationResult {
  summary: string
  keyPoints?: string[]
  wordCount: number
  compressionRatio?: number
  processingTime: number
  confidence?: number
}

// ============================================================================
// Translation
// ============================================================================

export type SupportedLanguage =
  | 'en' | 'ar' | 'fr' | 'es' | 'de' | 'it' | 'pt' | 'ru'
  | 'zh' | 'ja' | 'ko' | 'hi' | 'tr' | 'nl' | 'pl' | 'sv'

export interface TranslationResult {
  translatedText: string
  sourceLanguage: SupportedLanguage
  targetLanguage: SupportedLanguage
  confidence: number
  processingTime: number
}

export interface LanguageOption {
  code: SupportedLanguage
  name: string
  nativeName: string
  flag?: string
}

// ============================================================================
// Title Generation
// ============================================================================

export interface TitleSuggestion {
  title: string
  score: number
  style: 'formal' | 'casual' | 'seo' | 'creative'
}

export interface TitleGenerationResult {
  suggestions: TitleSuggestion[]
  processingTime: number
}

// ============================================================================
// Auto-Tagging
// ============================================================================

export interface TagSuggestion {
  tag: string
  confidence: number
  category?: string
}

export interface AutoTagResult {
  tags: TagSuggestion[]
  processingTime: number
}

// ============================================================================
// Smart Search
// ============================================================================

export interface SearchIntent {
  query: string
  intent: 'find' | 'learn' | 'compare' | 'navigate' | 'answer'
  entities: Entity[]
  suggestedFilters?: Record<string, string[]>
}

export interface SearchSuggestion {
  text: string
  type: 'query' | 'entity' | 'filter' | 'correction'
  score: number
}

export interface SmartSearchResult {
  intent: SearchIntent
  suggestions: SearchSuggestion[]
  didYouMean?: string
  processingTime: number
}

// ============================================================================
// AI Operation State
// ============================================================================

export type AIOperationType =
  | 'summarize'
  | 'translate'
  | 'extract-entities'
  | 'analyze-sentiment'
  | 'classify'
  | 'generate-title'
  | 'ocr'
  | 'auto-tag'
  | 'smart-search'
  | 'chat'

export type AIOperationStatus = 'idle' | 'pending' | 'processing' | 'success' | 'error'

export interface AIOperation<T = unknown> {
  id: string
  type: AIOperationType
  status: AIOperationStatus
  progress?: number
  result?: T
  error?: string
  startedAt?: string
  completedAt?: string
}

// ============================================================================
// AI Feature Configuration
// ============================================================================

export interface AIFeatureConfig {
  enabled: boolean
  autoRun?: boolean
  showConfidence?: boolean
  minConfidence?: number
}

export interface AISettings {
  summarization: AIFeatureConfig & { defaultType: SummaryType }
  translation: AIFeatureConfig & { preferredLanguage: SupportedLanguage }
  entityExtraction: AIFeatureConfig
  sentimentAnalysis: AIFeatureConfig
  classification: AIFeatureConfig
  titleGeneration: AIFeatureConfig & { defaultStyle: 'formal' | 'casual' | 'seo' | 'creative' }
  ocr: AIFeatureConfig
  autoTagging: AIFeatureConfig
  smartSearch: AIFeatureConfig
  chatbot: AIFeatureConfig
}

// ============================================================================
// AI Insights & Recommendations
// ============================================================================

export interface AIInsight {
  id: string
  type: 'trend' | 'anomaly' | 'recommendation' | 'prediction'
  title: string
  description: string
  confidence: number
  category: string
  actionable: boolean
  action?: {
    label: string
    route?: string
    handler?: string
  }
  createdAt: string
}

export interface ContentRecommendation {
  id: string
  contentType: 'article' | 'document' | 'course' | 'event'
  title: string
  description: string
  thumbnail?: string
  relevanceScore: number
  reason: string
  url: string
}

// ============================================================================
// AI Chat Enhanced Types
// ============================================================================

export interface AIContext {
  contentId?: string
  contentType?: 'article' | 'document' | 'page'
  contentTitle?: string
  userQuery?: string
}

export interface AIResponseMetadata {
  tokensUsed?: number
  model?: string
  responseTime: number
  citations?: Array<{
    id: string
    title: string
    url: string
    snippet: string
    source: string
  }>
}

// ============================================================================
// Export all supported languages
// ============================================================================

export const SUPPORTED_LANGUAGES: LanguageOption[] = [
  { code: 'en', name: 'English', nativeName: 'English', flag: '🇬🇧' },
  { code: 'ar', name: 'Arabic', nativeName: 'العربية', flag: '🇸🇦' },
  { code: 'fr', name: 'French', nativeName: 'Français', flag: '🇫🇷' },
  { code: 'es', name: 'Spanish', nativeName: 'Español', flag: '🇪🇸' },
  { code: 'de', name: 'German', nativeName: 'Deutsch', flag: '🇩🇪' },
  { code: 'it', name: 'Italian', nativeName: 'Italiano', flag: '🇮🇹' },
  { code: 'pt', name: 'Portuguese', nativeName: 'Português', flag: '🇵🇹' },
  { code: 'ru', name: 'Russian', nativeName: 'Русский', flag: '🇷🇺' },
  { code: 'zh', name: 'Chinese', nativeName: '中文', flag: '🇨🇳' },
  { code: 'ja', name: 'Japanese', nativeName: '日本語', flag: '🇯🇵' },
  { code: 'ko', name: 'Korean', nativeName: '한국어', flag: '🇰🇷' },
  { code: 'hi', name: 'Hindi', nativeName: 'हिन्दी', flag: '🇮🇳' },
  { code: 'tr', name: 'Turkish', nativeName: 'Türkçe', flag: '🇹🇷' },
  { code: 'nl', name: 'Dutch', nativeName: 'Nederlands', flag: '🇳🇱' },
  { code: 'pl', name: 'Polish', nativeName: 'Polski', flag: '🇵🇱' },
  { code: 'sv', name: 'Swedish', nativeName: 'Svenska', flag: '🇸🇪' },
]

// Default AI settings
export const DEFAULT_AI_SETTINGS: AISettings = {
  summarization: { enabled: true, autoRun: false, showConfidence: true, defaultType: 'brief' },
  translation: { enabled: true, autoRun: false, showConfidence: true, preferredLanguage: 'en' },
  entityExtraction: { enabled: true, autoRun: false, showConfidence: true, minConfidence: 0.7 },
  sentimentAnalysis: { enabled: true, autoRun: false, showConfidence: true },
  classification: { enabled: true, autoRun: true, showConfidence: true, minConfidence: 0.6 },
  titleGeneration: { enabled: true, autoRun: false, showConfidence: true, defaultStyle: 'formal' },
  ocr: { enabled: true, autoRun: false, showConfidence: true },
  autoTagging: { enabled: true, autoRun: true, showConfidence: true, minConfidence: 0.5 },
  smartSearch: { enabled: true, autoRun: true, showConfidence: false },
  chatbot: { enabled: true, autoRun: false, showConfidence: false },
}
