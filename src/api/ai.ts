import type { AIMessage, AIConversation, AICitation } from '@/types'

const AI_BASE = '/api/ai'

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
}

export default aiApi
