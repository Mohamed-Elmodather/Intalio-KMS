import { apiClient } from './client'

// Types
export interface AIStatus {
  isAvailable: boolean
  provider: string
  model: string
  isMock: boolean
  indexedDocuments: number
  lastIndexUpdate?: string
}

export interface ChatRequest {
  message: string
  conversationId?: string
  useRAG?: boolean
  stream?: boolean
  restrictToDocuments?: string[]
}

export interface ChatResponse {
  conversationId: string
  answer: string
  citations: Citation[]
  sources: DocumentChunk[]
  suggestedFollowUp?: string
  tokensUsed: number
}

export interface Citation {
  index: number
  documentId: string
  documentName: string
  chunkId: string
  quote: string
  pageNumber?: number
}

export interface DocumentChunk {
  documentId: string
  documentName: string
  chunkId: string
  content: string
  relevanceScore: number
  pageNumber?: number
  section?: string
  metadata: Record<string, string>
}

export interface Conversation {
  id: string
  title: string
  userId: string
  createdAt: string
  lastMessageAt: string
  messageCount: number
}

export interface Message {
  id: string
  conversationId: string
  role: string
  content: string
  citations?: Citation[]
  createdAt: string
}

// SSE Event Types
export type ChatStreamEvent =
  | { type: 'conversation'; conversationId: string }
  | { type: 'sources'; sources: DocumentChunk[] }
  | { type: 'chunk'; delta: string }
  | { type: 'done'; citations?: Citation[]; fullResponse: string }
  | { type: 'title'; title: string }
  | { type: 'error'; error: string }

// AI API
export const aiApi = {
  async getStatus(): Promise<AIStatus> {
    const response = await apiClient.get('/api/ai/status')
    return response.data
  },

  async chat(request: ChatRequest): Promise<ChatResponse> {
    const response = await apiClient.post('/api/ai/chat', {
      ...request,
      stream: false
    })
    return response.data
  },

  chatStream(
    request: ChatRequest,
    onEvent: (event: ChatStreamEvent) => void,
    onError?: (error: Error) => void
  ): AbortController {
    const controller = new AbortController()

    const fetchStream = async () => {
      try {
        const response = await fetch('/api/ai/chat/stream', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('access_token')}`
          },
          body: JSON.stringify({
            ...request,
            stream: true
          }),
          signal: controller.signal
        })

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

          // Parse SSE events
          const lines = buffer.split('\n')
          buffer = lines.pop() || ''

          let eventType = ''
          let eventData = ''

          for (const line of lines) {
            if (line.startsWith('event: ')) {
              eventType = line.slice(7)
            } else if (line.startsWith('data: ')) {
              eventData = line.slice(6)

              if (eventType && eventData) {
                try {
                  const parsed = JSON.parse(eventData)
                  onEvent({ type: eventType, ...parsed } as ChatStreamEvent)
                } catch (e) {
                  console.error('Failed to parse SSE data:', e)
                }
                eventType = ''
                eventData = ''
              }
            }
          }
        }
      } catch (error) {
        if ((error as Error).name !== 'AbortError') {
          onError?.(error as Error)
        }
      }
    }

    fetchStream()

    return controller
  },

  async search(query: string, maxResults = 10): Promise<DocumentChunk[]> {
    const response = await apiClient.get('/api/ai/search', {
      params: { query, maxResults }
    })
    return response.data
  },

  async getConversations(limit = 50): Promise<Conversation[]> {
    const response = await apiClient.get('/api/ai/conversations', {
      params: { limit }
    })
    return response.data
  },

  async getConversation(id: string): Promise<Conversation> {
    const response = await apiClient.get(`/api/ai/conversations/${id}`)
    return response.data
  },

  async getMessages(conversationId: string): Promise<Message[]> {
    const response = await apiClient.get(`/api/ai/conversations/${conversationId}/messages`)
    return response.data
  },

  async deleteConversation(id: string): Promise<void> {
    await apiClient.delete(`/api/ai/conversations/${id}`)
  },

  async updateConversationTitle(id: string, title: string): Promise<void> {
    await apiClient.patch(`/api/ai/conversations/${id}`, { title })
  }
}
