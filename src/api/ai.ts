import type { AIMessage, AIConversation, AICitation } from '@/types'
import type {
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
  Entity,
  EntityType,
} from '@/types/ai'

const AI_BASE = '/api/ai'

// Helper function to simulate API delay
const delay = (ms: number) => new Promise(resolve => setTimeout(resolve, ms))

export interface StreamingChatOptions {
  conversationId?: string
  onMessage: (content: string, isComplete: boolean) => void
  onCitations?: (citations: AICitation[]) => void
  onError?: (error: Error) => void
}

export const aiApi = {
  // Stream chat response (SSE)
  streamChat(message: string, options: StreamingChatOptions): AbortController {
    const controller = new AbortController()
    const baseUrl = import.meta.env.VITE_API_BASE_URL || ''

    const url = new URL(`${baseUrl}${AI_BASE}/chat/stream`)

    fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('auth_token') || ''}`,
      },
      body: JSON.stringify({
        message,
        conversationId: options.conversationId,
      }),
      signal: controller.signal,
    })
      .then(async (response) => {
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`)
        }

        const reader = response.body?.getReader()
        if (!reader) {
          throw new Error('No response body')
        }

        const decoder = new TextDecoder()
        let buffer = ''

        while (true) {
          const { done, value } = await reader.read()
          if (done) break

          buffer += decoder.decode(value, { stream: true })
          const lines = buffer.split('\n')
          buffer = lines.pop() || ''

          for (const line of lines) {
            if (line.startsWith('data: ')) {
              const data = line.slice(6)
              if (data === '[DONE]') {
                options.onMessage('', true)
                return
              }

              try {
                const parsed = JSON.parse(data)
                if (parsed.content) {
                  options.onMessage(parsed.content, false)
                }
                if (parsed.citations && options.onCitations) {
                  options.onCitations(parsed.citations)
                }
              } catch {
                // Non-JSON line, might be content directly
                options.onMessage(data, false)
              }
            }
          }
        }

        options.onMessage('', true)
      })
      .catch((error) => {
        if (error.name !== 'AbortError' && options.onError) {
          options.onError(error)
        }
      })

    return controller
  },

  // Get conversations
  async getConversations(): Promise<AIConversation[]> {
    const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}${AI_BASE}/conversations`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('auth_token') || ''}`,
      },
    })
    return response.json()
  },

  // Get single conversation
  async getConversation(id: string): Promise<AIConversation> {
    const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}${AI_BASE}/conversations/${id}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('auth_token') || ''}`,
      },
    })
    return response.json()
  },

  // Delete conversation
  async deleteConversation(id: string): Promise<void> {
    await fetch(`${import.meta.env.VITE_API_BASE_URL}${AI_BASE}/conversations/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('auth_token') || ''}`,
      },
    })
  },

  // Generate document summary
  async summarizeDocument(documentId: string): Promise<{ summary: string }> {
    const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}${AI_BASE}/summarize`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('auth_token') || ''}`,
      },
      body: JSON.stringify({ documentId }),
    })
    return response.json()
  },

  // Create mock message for development
  createMockMessage(content: string, role: 'user' | 'assistant'): AIMessage {
    return {
      id: crypto.randomUUID(),
      role,
      content,
      timestamp: new Date().toISOString(),
    }
  },

  // ============================================================================
  // MOCK AI SERVICES (To be replaced with real Intalio AI Engine calls)
  // ============================================================================

  /**
   * Summarize content using AI
   */
  async summarizeContent(content: string, type: SummaryType = 'brief'): Promise<SummarizationResult> {
    await delay(800 + Math.random() * 700)

    const wordCount = content.split(/\s+/).length
    const summaryLength = type === 'brief' ? 50 : type === 'detailed' ? 150 : 100

    // Mock summary generation
    const mockSummaries: Record<SummaryType, string> = {
      brief: `This content discusses key aspects of the topic, highlighting the main points and conclusions. It covers essential information in a concise manner.`,
      detailed: `This comprehensive content provides an in-depth analysis of the subject matter. The author explores multiple facets including background context, current developments, and future implications. Key themes include innovation, collaboration, and strategic planning. The content emphasizes the importance of understanding fundamental concepts while also addressing practical applications and real-world scenarios.`,
      bullet: `• Main topic overview and context\n• Key findings and insights\n• Important recommendations\n• Next steps and action items\n• Conclusion and summary`,
    }

    return {
      summary: mockSummaries[type],
      keyPoints: type === 'bullet' ? mockSummaries.bullet.split('\n').map(p => p.replace('• ', '')) : [
        'Key insight about the main topic',
        'Important finding from the analysis',
        'Recommendation for next steps',
      ],
      wordCount: summaryLength,
      compressionRatio: wordCount / summaryLength,
      processingTime: 800 + Math.random() * 700,
    }
  },

  /**
   * Translate content to target language
   */
  async translateContent(content: string, targetLang: SupportedLanguage): Promise<TranslationResult> {
    await delay(600 + Math.random() * 600)

    // Mock translations (in real implementation, this would call Intalio AI Engine)
    const mockTranslations: Partial<Record<SupportedLanguage, string>> = {
      ar: 'هذا هو المحتوى المترجم إلى اللغة العربية. يحتوي على المعلومات الأصلية مع الحفاظ على المعنى والسياق.',
      fr: 'Ceci est le contenu traduit en français. Il contient les informations originales tout en préservant le sens et le contexte.',
      es: 'Este es el contenido traducido al español. Contiene la información original preservando el significado y el contexto.',
      de: 'Dies ist der ins Deutsche übersetzte Inhalt. Er enthält die ursprünglichen Informationen unter Beibehaltung der Bedeutung und des Kontexts.',
      zh: '这是翻译成中文的内容。它包含原始信息，同时保留了含义和上下文。',
      ja: 'これは日本語に翻訳されたコンテンツです。意味と文脈を保持しながら、元の情報を含んでいます。',
    }

    return {
      translatedText: mockTranslations[targetLang] || `[Translated to ${targetLang}] ${content.substring(0, 200)}...`,
      sourceLanguage: 'en',
      targetLanguage: targetLang,
      confidence: 0.92 + Math.random() * 0.07,
      processingTime: 600 + Math.random() * 600,
    }
  },

  /**
   * Extract named entities from content
   */
  async extractEntities(content: string): Promise<NERResult> {
    await delay(500 + Math.random() * 500)

    // Mock entity extraction
    const mockEntities: Entity[] = [
      { text: 'AFC Asian Cup 2027', type: 'event', confidence: 0.95, startIndex: 0, endIndex: 18 },
      { text: 'Saudi Arabia', type: 'location', confidence: 0.98, startIndex: 50, endIndex: 62 },
      { text: 'Virat Kohli', type: 'person', confidence: 0.97, startIndex: 100, endIndex: 111 },
      { text: 'Cricket Australia', type: 'organization', confidence: 0.94, startIndex: 150, endIndex: 167 },
      { text: 'January 2027', type: 'date', confidence: 0.96, startIndex: 200, endIndex: 212 },
      { text: 'Intalio', type: 'organization', confidence: 0.99, startIndex: 250, endIndex: 257 },
    ]

    return {
      entities: mockEntities.slice(0, Math.floor(Math.random() * 4) + 2),
      processingTime: 500 + Math.random() * 500,
    }
  },

  /**
   * Analyze sentiment of content
   */
  async analyzeSentiment(content: string): Promise<SentimentResult> {
    await delay(400 + Math.random() * 400)

    const sentiments: Array<'positive' | 'neutral' | 'negative'> = ['positive', 'neutral', 'negative']
    const randomSentiment = sentiments[Math.floor(Math.random() * 3)] as 'positive' | 'neutral' | 'negative'
    const score = randomSentiment === 'positive' ? 0.3 + Math.random() * 0.7 :
                  randomSentiment === 'negative' ? -0.3 - Math.random() * 0.7 :
                  -0.2 + Math.random() * 0.4

    return {
      overall: randomSentiment,
      score,
      confidence: 0.85 + Math.random() * 0.14,
      emotions: [
        { emotion: 'joy', score: Math.random() * 0.8 },
        { emotion: 'trust', score: Math.random() * 0.7 },
        { emotion: 'anticipation', score: Math.random() * 0.6 },
        { emotion: 'surprise', score: Math.random() * 0.3 },
      ],
      processingTime: 400 + Math.random() * 400,
    }
  },

  /**
   * Classify content into categories
   */
  async classifyContent(content: string): Promise<ClassificationResult> {
    await delay(500 + Math.random() * 500)

    const categories = [
      { name: 'Sports & Athletics', confidence: 0.89 },
      { name: 'Tournament Coverage', confidence: 0.76 },
      { name: 'News & Updates', confidence: 0.65 },
      { name: 'Team Management', confidence: 0.52 },
    ]

    const tags = ['cricket', 'asia-cup', 'tournament', 'sports', 'teams', 'competition', 'international']

    return {
      categories: categories.slice(0, 3),
      suggestedTags: tags.slice(0, Math.floor(Math.random() * 3) + 3),
      primaryCategory: categories[0]!.name,
      confidence: categories[0]!.confidence,
      processingTime: 500 + Math.random() * 500,
    }
  },

  /**
   * Generate title suggestions from content
   */
  async generateTitles(content: string): Promise<TitleGenerationResult> {
    await delay(700 + Math.random() * 600)

    const suggestions = [
      { title: 'AFC Asian Cup 2027: Complete Tournament Guide', score: 0.92, style: 'formal' as const },
      { title: 'Everything You Need to Know About Asia Cup 2027', score: 0.88, style: 'casual' as const },
      { title: 'Asia Cup 2027 Tournament Overview & Key Highlights', score: 0.85, style: 'seo' as const },
      { title: 'The Ultimate Guide to the Biggest Cricket Event of 2027', score: 0.82, style: 'creative' as const },
    ]

    return {
      suggestions,
      processingTime: 700 + Math.random() * 600,
    }
  },

  /**
   * Perform OCR on uploaded file
   */
  async performOCR(file: File): Promise<OCRResult> {
    await delay(1000 + Math.random() * 1000)

    // Mock OCR result
    return {
      text: `Extracted text from ${file.name}:\n\nThis document contains important information about the AFC Asian Cup 2027. The tournament will feature teams from across Asia competing for the prestigious title.\n\nKey Details:\n- Date: January 2027\n- Location: Saudi Arabia\n- Teams: 24 participating nations\n- Format: Group stage followed by knockout rounds`,
      blocks: [
        { text: 'AFC Asian Cup 2027', confidence: 0.98 },
        { text: 'Tournament Information', confidence: 0.95 },
        { text: 'Key Details', confidence: 0.97 },
      ],
      language: 'en',
      confidence: 0.94 + Math.random() * 0.05,
      processingTime: 1000 + Math.random() * 1000,
    }
  },

  /**
   * Auto-generate tags for content
   */
  async autoTag(content: string): Promise<AutoTagResult> {
    await delay(400 + Math.random() * 400)

    const allTags = [
      { tag: 'asia-cup', confidence: 0.95, category: 'Event' },
      { tag: 'cricket', confidence: 0.93, category: 'Sport' },
      { tag: 'tournament', confidence: 0.88, category: 'Type' },
      { tag: '2027', confidence: 0.92, category: 'Year' },
      { tag: 'saudi-arabia', confidence: 0.85, category: 'Location' },
      { tag: 'international', confidence: 0.78, category: 'Scope' },
      { tag: 'teams', confidence: 0.72, category: 'Topic' },
      { tag: 'competition', confidence: 0.69, category: 'Type' },
    ]

    return {
      tags: allTags.slice(0, Math.floor(Math.random() * 4) + 4),
      processingTime: 400 + Math.random() * 400,
    }
  },

  /**
   * Smart search with intent detection
   */
  async smartSearch(query: string): Promise<SmartSearchResult> {
    await delay(300 + Math.random() * 300)

    const intents: Array<'find' | 'learn' | 'compare' | 'navigate' | 'answer'> = ['find', 'learn', 'compare', 'navigate', 'answer']
    const randomIntent = intents[Math.floor(Math.random() * intents.length)] as 'find' | 'learn' | 'compare' | 'navigate' | 'answer'

    const suggestions = [
      { text: `${query} schedule`, type: 'query' as const, score: 0.9 },
      { text: `${query} teams`, type: 'query' as const, score: 0.85 },
      { text: `${query} results`, type: 'query' as const, score: 0.8 },
      { text: 'AFC Asian Cup 2027', type: 'entity' as const, score: 0.95 },
      { text: 'Category: Sports', type: 'filter' as const, score: 0.7 },
    ]

    return {
      intent: {
        query,
        intent: randomIntent,
        entities: [
          { text: query, type: 'event' as EntityType, confidence: 0.85, startIndex: 0, endIndex: query.length },
        ],
        suggestedFilters: {
          category: ['Sports', 'Events'],
          type: ['Article', 'Document'],
        },
      },
      suggestions: suggestions.slice(0, 4),
      didYouMean: query.length < 5 ? `Did you mean "${query} tournament"?` : undefined,
      processingTime: 300 + Math.random() * 300,
    }
  },

  /**
   * Generate AI-powered content recommendations
   */
  async getRecommendations(contentType?: string, limit: number = 5): Promise<Array<{
    id: string
    contentType: string
    title: string
    description: string
    relevanceScore: number
    reason: string
  }>> {
    await delay(500 + Math.random() * 500)

    const recommendations = [
      {
        id: '1',
        contentType: 'article',
        title: 'Top Performers of Asia Cup History',
        description: 'A comprehensive look at the greatest players in tournament history.',
        relevanceScore: 0.95,
        reason: 'Based on your recent reading activity',
      },
      {
        id: '2',
        contentType: 'document',
        title: 'Tournament Rules & Regulations 2027',
        description: 'Official documentation for the upcoming tournament.',
        relevanceScore: 0.88,
        reason: 'Related to your recent searches',
      },
      {
        id: '3',
        contentType: 'course',
        title: 'Understanding Cricket Analytics',
        description: 'Learn how to analyze cricket statistics and performance data.',
        relevanceScore: 0.82,
        reason: 'Popular in your department',
      },
      {
        id: '4',
        contentType: 'event',
        title: 'Asia Cup 2027 Opening Ceremony',
        description: 'Join us for the grand opening of the tournament.',
        relevanceScore: 0.79,
        reason: 'Upcoming event you might be interested in',
      },
      {
        id: '5',
        contentType: 'article',
        title: 'Team Preparations for Asia Cup',
        description: 'How teams are preparing for the biggest tournament.',
        relevanceScore: 0.75,
        reason: 'Trending in your network',
      },
    ]

    return recommendations.slice(0, limit)
  },

  /**
   * Generate AI insights for dashboard
   */
  async getInsights(): Promise<Array<{
    id: string
    type: 'trend' | 'anomaly' | 'recommendation' | 'prediction'
    title: string
    description: string
    confidence: number
    category: string
    actionable: boolean
  }>> {
    await delay(600 + Math.random() * 400)

    return [
      {
        id: '1',
        type: 'trend',
        title: 'Article engagement up 23%',
        description: 'Sports-related articles have seen a significant increase in engagement this week.',
        confidence: 0.92,
        category: 'Content',
        actionable: true,
      },
      {
        id: '2',
        type: 'prediction',
        title: 'Expected traffic spike',
        description: 'Based on historical data, expect 40% more traffic during the tournament dates.',
        confidence: 0.87,
        category: 'Analytics',
        actionable: true,
      },
      {
        id: '3',
        type: 'recommendation',
        title: 'Content gap detected',
        description: 'Users are searching for team profiles but limited content exists.',
        confidence: 0.85,
        category: 'Content',
        actionable: true,
      },
      {
        id: '4',
        type: 'anomaly',
        title: 'Unusual download pattern',
        description: 'Document downloads from media center increased 150% yesterday.',
        confidence: 0.78,
        category: 'Usage',
        actionable: false,
      },
    ]
  },
}

export default aiApi
