<script setup lang="ts">
import { ref, computed, nextTick, onMounted, onUnmounted, watch } from 'vue'
import { useRoute } from 'vue-router'

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------
interface Message {
  id: number
  role: 'user' | 'assistant'
  content: string
  timestamp: Date
  isStreaming?: boolean
}

interface QuickAction {
  id: string
  label: string
  icon: string
  prompt: string
}

// ---------------------------------------------------------------------------
// State
// ---------------------------------------------------------------------------
const route = useRoute()

const isOpen = ref(false)
const isPinned = ref(false)
const isMinimized = ref(false)
const inputText = ref('')
const messages = ref<Message[]>([])
const isTyping = ref(false)
const messagesContainer = ref<HTMLElement | null>(null)
const inputRef = ref<HTMLTextAreaElement | null>(null)
let nextId = 1

// ---------------------------------------------------------------------------
// Route-based context awareness
// ---------------------------------------------------------------------------
const routeContext = computed(() => {
  const path = route.path
  const name = (route.name as string) || ''

  const contexts: Record<string, { area: string; noun: string }> = {
    '/articles': { area: 'Articles', noun: 'articles' },
    '/documents': { area: 'Documents', noun: 'documents' },
    '/media': { area: 'Media Center', noun: 'media items' },
    '/collections': { area: 'Collections', noun: 'collections' },
    '/spaces': { area: 'Spaces', noun: 'spaces' },
    '/learning': { area: 'Learning', noun: 'courses' },
    '/lessons-learned': { area: 'Lessons Learned', noun: 'lessons' },
    '/events': { area: 'Events', noun: 'events' },
    '/collaboration': { area: 'Collaboration', noun: 'discussions' },
    '/polls': { area: 'Polls', noun: 'polls' },
    '/self-services': { area: 'Self Services', noun: 'service requests' },
    '/analytics': { area: 'Analytics', noun: 'reports' },
    '/settings': { area: 'Settings', noun: 'settings' },
    '/': { area: 'Dashboard', noun: 'dashboard widgets' },
  }

  // Match the deepest path first, then fall back to top-level
  const match = Object.entries(contexts)
    .sort((a, b) => b[0].length - a[0].length)
    .find(([prefix]) => path.startsWith(prefix))

  return match ? match[1] : { area: 'Portal', noun: 'content' }
})

const quickActions = computed<QuickAction[]>(() => {
  const ctx = routeContext.value
  const actions: QuickAction[] = [
    {
      id: 'summarize',
      label: `Summarize this page`,
      icon: 'fas fa-compress-alt',
      prompt: `Summarize what is currently shown on the ${ctx.area} page.`,
    },
    {
      id: 'search-kb',
      label: 'Search knowledge base',
      icon: 'fas fa-search',
      prompt: 'Search the knowledge base for relevant information.',
    },
    {
      id: 'ask',
      label: 'Ask a question',
      icon: 'fas fa-question-circle',
      prompt: '',
    },
  ]

  // Add context-specific actions
  if (ctx.area === 'Articles') {
    actions.push({
      id: 'draft',
      label: 'Draft a new article',
      icon: 'fas fa-pen-fancy',
      prompt: 'Help me draft a new knowledge base article.',
    })
  } else if (ctx.area === 'Documents') {
    actions.push({
      id: 'compare',
      label: 'Compare documents',
      icon: 'fas fa-columns',
      prompt: 'Help me compare two documents for differences.',
    })
  } else if (ctx.area === 'Analytics') {
    actions.push({
      id: 'insights',
      label: 'Generate insights',
      icon: 'fas fa-lightbulb',
      prompt: 'Analyze the current analytics data and provide key insights.',
    })
  } else if (ctx.area === 'Learning') {
    actions.push({
      id: 'recommend',
      label: 'Recommend courses',
      icon: 'fas fa-graduation-cap',
      prompt: 'Recommend learning courses based on my profile and interests.',
    })
  } else if (ctx.area === 'Dashboard') {
    actions.push({
      id: 'brief',
      label: 'Daily briefing',
      icon: 'fas fa-sun',
      prompt: 'Give me a quick daily briefing of what needs my attention today.',
    })
  }

  return actions
})

// ---------------------------------------------------------------------------
// Mock AI responses
// ---------------------------------------------------------------------------
const mockResponses: Record<string, string> = {
  summarize:
    'Here is a summary of what you are viewing:\n\n' +
    '- This page displays the main content for the current section.\n' +
    '- There are several items that may require your attention.\n' +
    '- Key metrics and recent activity are highlighted at the top.\n\n' +
    'Would you like me to dive deeper into any specific area?',
  'search-kb':
    'I searched the knowledge base and found **12 relevant results**:\n\n' +
    '1. **Getting Started Guide** — Overview of portal features\n' +
    '2. **Best Practices for Knowledge Sharing** — Tips and templates\n' +
    '3. **FAQ: Common Questions** — Frequently asked questions\n\n' +
    'Would you like me to open any of these or refine the search?',
  draft:
    'I can help you draft an article. Let me start with a template:\n\n' +
    '**Title:** [Your Article Title]\n\n' +
    '**Summary:** A brief overview of the topic...\n\n' +
    '**Content:**\n- Key point 1\n- Key point 2\n- Key point 3\n\n' +
    'What topic would you like to write about?',
  compare:
    'To compare documents, please tell me which two documents you would like to compare, or I can suggest pairs with similar titles from your recent activity.',
  insights:
    'Based on the current analytics data, here are the key insights:\n\n' +
    '- **Content engagement** is up 15% this month.\n' +
    '- **Top contributor:** Ahmed I. with 8 new articles.\n' +
    '- **Most viewed topic:** Cloud Infrastructure (342 views).\n' +
    '- **Action needed:** 3 articles are pending review.\n\n' +
    'Shall I generate a detailed report?',
  recommend:
    'Based on your profile and activity, I recommend:\n\n' +
    '1. **Advanced Data Analytics** — Matches your recent searches\n' +
    '2. **Leadership Fundamentals** — Popular in your department\n' +
    '3. **AI & Automation** — Trending across the organization\n\n' +
    'Would you like to enroll in any of these?',
  brief:
    'Good morning! Here is your daily briefing:\n\n' +
    '- **3 articles** pending your review\n' +
    '- **1 event** starting in 2 hours: "Q1 Knowledge Sync"\n' +
    '- **5 new lessons learned** submitted this week\n' +
    '- **2 polls** closing today\n\n' +
    'What would you like to focus on first?',
  default:
    'Thank you for your question. Let me look into that for you.\n\n' +
    'Based on the information available in the knowledge base, here is what I found:\n\n' +
    '- The portal supports full-text search across all content types.\n' +
    '- You can use filters to narrow down results by date, author, or category.\n' +
    '- AI-powered suggestions are available on most pages.\n\n' +
    'Is there anything specific you would like to know more about?',
}

function getMockResponse(input: string): string {
  const lower = input.toLowerCase()
  if (lower.includes('summarize') || lower.includes('summary')) return mockResponses.summarize
  if (lower.includes('search') || lower.includes('find') || lower.includes('knowledge base')) return mockResponses['search-kb']
  if (lower.includes('draft') || lower.includes('write') || lower.includes('article')) return mockResponses.draft
  if (lower.includes('compare')) return mockResponses.compare
  if (lower.includes('insight') || lower.includes('analytics') || lower.includes('data')) return mockResponses.insights
  if (lower.includes('recommend') || lower.includes('course') || lower.includes('learn')) return mockResponses.recommend
  if (lower.includes('briefing') || lower.includes('today') || lower.includes('attention')) return mockResponses.brief
  return mockResponses.default
}

// ---------------------------------------------------------------------------
// Actions
// ---------------------------------------------------------------------------
function toggle() {
  if (isOpen.value) {
    close()
  } else {
    open()
  }
}

function open() {
  isOpen.value = true
  isMinimized.value = false
  nextTick(() => {
    inputRef.value?.focus()
    scrollToBottom()
  })
}

function close() {
  if (isPinned.value) return
  isOpen.value = false
}

function togglePin() {
  isPinned.value = !isPinned.value
}

function toggleMinimize() {
  isMinimized.value = !isMinimized.value
  if (!isMinimized.value) {
    nextTick(() => scrollToBottom())
  }
}

function scrollToBottom() {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
    }
  })
}

async function sendMessage(text?: string) {
  const content = (text || inputText.value).trim()
  if (!content || isTyping.value) return

  // Add user message
  messages.value.push({
    id: nextId++,
    role: 'user',
    content,
    timestamp: new Date(),
  })
  inputText.value = ''
  scrollToBottom()

  // Simulate AI typing
  isTyping.value = true
  const responseText = getMockResponse(content)

  // Add placeholder assistant message for streaming
  const assistantMsg: Message = {
    id: nextId++,
    role: 'assistant',
    content: '',
    timestamp: new Date(),
    isStreaming: true,
  }
  messages.value.push(assistantMsg)
  scrollToBottom()

  // Stream characters one by one
  await streamResponse(assistantMsg, responseText)

  assistantMsg.isStreaming = false
  isTyping.value = false
  scrollToBottom()
}

function streamResponse(msg: Message, fullText: string): Promise<void> {
  return new Promise((resolve) => {
    let index = 0
    const chunkSize = 3 // characters per tick for natural feel
    const interval = setInterval(() => {
      if (index < fullText.length) {
        const end = Math.min(index + chunkSize, fullText.length)
        msg.content += fullText.slice(index, end)
        index = end
        scrollToBottom()
      } else {
        clearInterval(interval)
        resolve()
      }
    }, 18)
  })
}

function handleQuickAction(action: QuickAction) {
  if (action.id === 'ask') {
    // Focus input instead of sending
    nextTick(() => inputRef.value?.focus())
    return
  }
  sendMessage(action.prompt)
}

function handleInputKeydown(e: KeyboardEvent) {
  if (e.key === 'Enter' && !e.shiftKey) {
    e.preventDefault()
    sendMessage()
  }
}

function clearChat() {
  messages.value = []
}

function formatTime(date: Date): string {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

// ---------------------------------------------------------------------------
// Keyboard shortcut: Ctrl+J
// ---------------------------------------------------------------------------
function handleGlobalKeydown(e: KeyboardEvent) {
  if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'j') {
    e.preventDefault()
    toggle()
  }
  if (e.key === 'Escape' && isOpen.value && !isPinned.value) {
    close()
  }
}

onMounted(() => {
  window.addEventListener('keydown', handleGlobalKeydown)
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleGlobalKeydown)
})

// Auto-scroll when messages change
watch(
  () => messages.value.length,
  () => scrollToBottom(),
)
</script>

<template>
  <!-- Floating Action Button -->
  <Transition name="fab">
    <button
      v-show="!isOpen"
      class="global-ai-fab"
      :title="`AI Assistant (Ctrl+J)`"
      @click="open"
    >
      <!-- Pulse ring -->
      <span class="fab-ring" />
      <!-- Icon -->
      <i class="fas fa-wand-magic-sparkles fab-icon" />
    </button>
  </Transition>

  <!-- Backdrop (only when open + not pinned) -->
  <Transition name="fade">
    <div
      v-if="isOpen && !isPinned"
      class="global-ai-backdrop"
      @click="close"
    />
  </Transition>

  <!-- Slide-over Panel -->
  <Transition name="slide">
    <aside
      v-if="isOpen"
      class="global-ai-panel"
      :class="{ 'is-pinned': isPinned, 'is-minimized': isMinimized }"
    >
      <!-- Header -->
      <header class="panel-header">
        <div class="header-left">
          <div class="header-icon-wrapper">
            <i class="fas fa-wand-magic-sparkles" />
          </div>
          <div>
            <h2 class="header-title">AI Assistant</h2>
            <p class="header-context">{{ routeContext.area }}</p>
          </div>
        </div>
        <div class="header-actions">
          <button
            class="header-btn"
            :class="{ active: isPinned }"
            :title="isPinned ? 'Unpin sidebar' : 'Pin as sidebar'"
            @click="togglePin"
          >
            <i class="fas fa-thumbtack" />
          </button>
          <button
            class="header-btn"
            :title="isMinimized ? 'Expand' : 'Minimize'"
            @click="toggleMinimize"
          >
            <i :class="isMinimized ? 'fas fa-expand-alt' : 'fas fa-minus'" />
          </button>
          <button
            v-if="messages.length > 0"
            class="header-btn"
            title="Clear chat"
            @click="clearChat"
          >
            <i class="fas fa-trash-alt" />
          </button>
          <button
            class="header-btn close-btn"
            title="Close (Esc)"
            @click="isOpen = false; isPinned = false"
          >
            <i class="fas fa-times" />
          </button>
        </div>
      </header>

      <!-- Body (hidden when minimized) -->
      <div v-show="!isMinimized" class="panel-body">
        <!-- Messages -->
        <div ref="messagesContainer" class="messages-area">
          <!-- Empty state -->
          <div v-if="messages.length === 0" class="empty-state">
            <div class="empty-icon">
              <i class="fas fa-robot" />
            </div>
            <h3 class="empty-title">How can I help you?</h3>
            <p class="empty-text">
              Ask me anything about your {{ routeContext.noun }} or use a quick action below.
            </p>
            <p class="empty-shortcut">
              <kbd>Ctrl</kbd> + <kbd>J</kbd> to toggle anytime
            </p>
          </div>

          <!-- Message bubbles -->
          <div
            v-for="msg in messages"
            :key="msg.id"
            class="message-row"
            :class="msg.role"
          >
            <!-- Avatar -->
            <div v-if="msg.role === 'assistant'" class="msg-avatar assistant-avatar">
              <i class="fas fa-robot" />
            </div>

            <div class="msg-bubble" :class="msg.role">
              <div class="msg-content" v-html="formatMarkdown(msg.content)" />
              <span v-if="msg.isStreaming" class="streaming-cursor" />
              <div class="msg-time">{{ formatTime(msg.timestamp) }}</div>
            </div>

            <div v-if="msg.role === 'user'" class="msg-avatar user-avatar">
              <i class="fas fa-user" />
            </div>
          </div>

          <!-- Typing indicator -->
          <div v-if="isTyping && messages.length > 0 && !messages[messages.length - 1]?.isStreaming" class="typing-indicator">
            <span /><span /><span />
          </div>
        </div>

        <!-- Quick Actions -->
        <div class="quick-actions-bar">
          <button
            v-for="action in quickActions"
            :key="action.id"
            class="quick-action-chip"
            @click="handleQuickAction(action)"
          >
            <i :class="action.icon" />
            <span>{{ action.label }}</span>
          </button>
        </div>

        <!-- Input area -->
        <div class="input-area">
          <textarea
            ref="inputRef"
            v-model="inputText"
            class="chat-input"
            :placeholder="`Ask about ${routeContext.noun}...`"
            rows="1"
            :disabled="isTyping"
            @keydown="handleInputKeydown"
          />
          <button
            class="send-btn"
            :disabled="!inputText.trim() || isTyping"
            @click="sendMessage()"
          >
            <i class="fas fa-paper-plane" />
          </button>
        </div>
      </div>
    </aside>
  </Transition>
</template>

<script lang="ts">
// Simple markdown-lite formatter (bold, line breaks, lists)
function formatMarkdown(text: string): string {
  if (!text) return ''
  return text
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>')
    .replace(/^- (.+)$/gm, '<li>$1</li>')
    .replace(/(<li>.*<\/li>)/gs, '<ul>$1</ul>')
    .replace(/(<\/ul>\s*<ul>)/g, '')
    .replace(/^\d+\.\s+(.+)$/gm, '<li>$1</li>')
    .replace(/\n{2,}/g, '<br/><br/>')
    .replace(/\n/g, '<br/>')
}
</script>

<style scoped>
/* =============================================
   Floating Action Button
   ============================================= */
.global-ai-fab {
  position: fixed;
  bottom: 1rem;
  right: 1rem;
  z-index: 9999;
  width: 3.5rem;
  height: 3.5rem;
  border-radius: 50%;
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #14b8a6, #0d9488, #0f766e);
  color: #fff;
  box-shadow:
    0 4px 14px rgba(20, 184, 166, 0.45),
    0 1px 3px rgba(0, 0, 0, 0.12);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.global-ai-fab:hover {
  transform: scale(1.08);
  box-shadow:
    0 6px 20px rgba(20, 184, 166, 0.55),
    0 2px 6px rgba(0, 0, 0, 0.15);
}

.global-ai-fab:active {
  transform: scale(0.96);
}

.fab-icon {
  font-size: 1.2rem;
  position: relative;
  z-index: 1;
}

.fab-ring {
  position: absolute;
  inset: -4px;
  border-radius: 50%;
  border: 2px solid rgba(20, 184, 166, 0.5);
  animation: fabPulse 2.5s ease-in-out infinite;
}

@keyframes fabPulse {
  0%, 100% { opacity: 0.6; transform: scale(1); }
  50% { opacity: 0; transform: scale(1.35); }
}

/* FAB transition */
.fab-enter-active,
.fab-leave-active {
  transition: all 0.25s ease;
}
.fab-enter-from,
.fab-leave-to {
  opacity: 0;
  transform: scale(0.5);
}

/* =============================================
   Backdrop
   ============================================= */
.global-ai-backdrop {
  position: fixed;
  inset: 0;
  z-index: 9998;
  background: rgba(0, 0, 0, 0.2);
  backdrop-filter: blur(2px);
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.25s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* =============================================
   Panel
   ============================================= */
.global-ai-panel {
  position: fixed;
  top: 0;
  right: 0;
  z-index: 10000;
  width: 24rem;
  height: 100vh;
  background: #ffffff;
  box-shadow: -4px 0 24px rgba(0, 0, 0, 0.12);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.global-ai-panel.is-pinned {
  box-shadow: -2px 0 12px rgba(0, 0, 0, 0.08);
}

.global-ai-panel.is-minimized {
  height: auto;
  top: auto;
  bottom: 1rem;
  right: 1rem;
  width: 20rem;
  border-radius: 0.75rem;
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.15);
}

/* Slide transition */
.slide-enter-active,
.slide-leave-active {
  transition: transform 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}
.slide-enter-from,
.slide-leave-to {
  transform: translateX(100%);
}

/* =============================================
   Header
   ============================================= */
.panel-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem 1rem;
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: #fff;
  flex-shrink: 0;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 0.625rem;
}

.header-icon-wrapper {
  width: 2rem;
  height: 2rem;
  border-radius: 0.5rem;
  background: rgba(255, 255, 255, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.85rem;
}

.header-title {
  font-size: 0.9rem;
  font-weight: 600;
  line-height: 1.2;
}

.header-context {
  font-size: 0.7rem;
  opacity: 0.85;
  line-height: 1.2;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.header-btn {
  width: 1.75rem;
  height: 1.75rem;
  border-radius: 0.375rem;
  border: none;
  background: rgba(255, 255, 255, 0.15);
  color: #fff;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  transition: background 0.15s ease;
}

.header-btn:hover {
  background: rgba(255, 255, 255, 0.3);
}

.header-btn.active {
  background: rgba(255, 255, 255, 0.35);
}

.close-btn:hover {
  background: rgba(239, 68, 68, 0.7);
}

/* =============================================
   Body
   ============================================= */
.panel-body {
  display: flex;
  flex-direction: column;
  flex: 1;
  min-height: 0;
}

/* =============================================
   Messages area
   ============================================= */
.messages-area {
  flex: 1;
  overflow-y: auto;
  padding: 1rem;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  scroll-behavior: smooth;
}

.messages-area::-webkit-scrollbar {
  width: 5px;
}
.messages-area::-webkit-scrollbar-track {
  background: transparent;
}
.messages-area::-webkit-scrollbar-thumb {
  background: #d1d5db;
  border-radius: 10px;
}

/* Empty state */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  flex: 1;
  padding: 2rem 1rem;
  gap: 0.5rem;
}

.empty-icon {
  width: 3.5rem;
  height: 3.5rem;
  border-radius: 50%;
  background: linear-gradient(135deg, #ccfbf1, #99f6e4);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  color: #0d9488;
  margin-bottom: 0.25rem;
}

.empty-title {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
}

.empty-text {
  font-size: 0.8rem;
  color: #6b7280;
  max-width: 18rem;
  line-height: 1.4;
}

.empty-shortcut {
  margin-top: 0.5rem;
  font-size: 0.7rem;
  color: #9ca3af;
}

.empty-shortcut kbd {
  display: inline-block;
  padding: 0.1rem 0.35rem;
  font-size: 0.65rem;
  font-family: inherit;
  background: #f3f4f6;
  border: 1px solid #d1d5db;
  border-radius: 0.25rem;
  box-shadow: 0 1px 0 #d1d5db;
  color: #374151;
}

/* Message rows */
.message-row {
  display: flex;
  align-items: flex-end;
  gap: 0.5rem;
}

.message-row.user {
  justify-content: flex-end;
}

.message-row.assistant {
  justify-content: flex-start;
}

.msg-avatar {
  width: 1.75rem;
  height: 1.75rem;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.7rem;
  flex-shrink: 0;
}

.assistant-avatar {
  background: linear-gradient(135deg, #ccfbf1, #99f6e4);
  color: #0d9488;
}

.user-avatar {
  background: linear-gradient(135deg, #e0e7ff, #c7d2fe);
  color: #4f46e5;
}

.msg-bubble {
  max-width: 80%;
  padding: 0.625rem 0.875rem;
  border-radius: 1rem;
  font-size: 0.8125rem;
  line-height: 1.5;
  position: relative;
  word-break: break-word;
}

.msg-bubble.user {
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: #fff;
  border-bottom-right-radius: 0.25rem;
}

.msg-bubble.assistant {
  background: #f3f4f6;
  color: #1f2937;
  border-bottom-left-radius: 0.25rem;
}

.msg-bubble.assistant :deep(strong) {
  color: #0d9488;
  font-weight: 600;
}

.msg-bubble.assistant :deep(ul),
.msg-bubble.assistant :deep(ol) {
  margin: 0.25rem 0;
  padding-left: 1rem;
}

.msg-bubble.assistant :deep(li) {
  margin-bottom: 0.15rem;
}

.streaming-cursor {
  display: inline-block;
  width: 2px;
  height: 0.9em;
  background: #0d9488;
  margin-left: 1px;
  vertical-align: text-bottom;
  animation: blink 0.7s step-end infinite;
}

@keyframes blink {
  50% { opacity: 0; }
}

.msg-time {
  font-size: 0.625rem;
  opacity: 0.55;
  margin-top: 0.25rem;
}

.msg-bubble.user .msg-time {
  text-align: right;
}

/* Typing indicator */
.typing-indicator {
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 0.5rem 0.75rem;
  width: fit-content;
  background: #f3f4f6;
  border-radius: 1rem;
}

.typing-indicator span {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: #9ca3af;
  animation: typingBounce 1.2s ease-in-out infinite;
}

.typing-indicator span:nth-child(2) {
  animation-delay: 0.15s;
}
.typing-indicator span:nth-child(3) {
  animation-delay: 0.3s;
}

@keyframes typingBounce {
  0%, 60%, 100% { transform: translateY(0); opacity: 0.4; }
  30% { transform: translateY(-4px); opacity: 1; }
}

/* =============================================
   Quick Actions
   ============================================= */
.quick-actions-bar {
  display: flex;
  gap: 0.375rem;
  padding: 0.5rem 1rem;
  overflow-x: auto;
  flex-shrink: 0;
  border-top: 1px solid #f3f4f6;
  scrollbar-width: none;
}

.quick-actions-bar::-webkit-scrollbar {
  display: none;
}

.quick-action-chip {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.35rem 0.75rem;
  border-radius: 9999px;
  border: 1px solid #e5e7eb;
  background: #fff;
  color: #374151;
  font-size: 0.7rem;
  white-space: nowrap;
  cursor: pointer;
  transition: all 0.15s ease;
  flex-shrink: 0;
}

.quick-action-chip:hover {
  background: #f0fdfa;
  border-color: #14b8a6;
  color: #0d9488;
}

.quick-action-chip i {
  font-size: 0.65rem;
  color: #14b8a6;
}

/* =============================================
   Input area
   ============================================= */
.input-area {
  display: flex;
  align-items: flex-end;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  border-top: 1px solid #e5e7eb;
  background: #fafafa;
  flex-shrink: 0;
}

.chat-input {
  flex: 1;
  resize: none;
  border: 1px solid #e5e7eb;
  border-radius: 0.75rem;
  padding: 0.5rem 0.75rem;
  font-size: 0.8125rem;
  font-family: inherit;
  line-height: 1.4;
  min-height: 2.25rem;
  max-height: 6rem;
  outline: none;
  background: #fff;
  color: #1f2937;
  transition: border-color 0.15s ease, box-shadow 0.15s ease;
}

.chat-input:focus {
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.chat-input:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.chat-input::placeholder {
  color: #9ca3af;
}

.send-btn {
  width: 2.25rem;
  height: 2.25rem;
  border-radius: 0.625rem;
  border: none;
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: #fff;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8rem;
  flex-shrink: 0;
  transition: opacity 0.15s ease, transform 0.15s ease;
}

.send-btn:hover:not(:disabled) {
  transform: scale(1.05);
}

.send-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

/* =============================================
   Responsive
   ============================================= */
@media (max-width: 480px) {
  .global-ai-panel {
    width: 100vw;
  }

  .global-ai-panel.is-minimized {
    width: calc(100vw - 2rem);
  }
}
</style>
