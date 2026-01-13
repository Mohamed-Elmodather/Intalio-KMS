<script setup lang="ts">
import { ref, computed, nextTick } from 'vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import { useAIServicesStore } from '@/stores/aiServices'
import { useComparisonStore } from '@/stores/comparison'
import { AILoadingIndicator } from '@/components/ai'
import ComparisonPanel from '@/components/ai/ComparisonPanel.vue'
import ComparisonModal from '@/components/ai/ComparisonModal.vue'

const aiStore = useAIServicesStore()
const comparisonStore = useComparisonStore()

// Types
interface Chat {
  id: number
  title: string
  preview: string
  context?: ConversationContext
}

interface Message {
  role: 'user' | 'assistant'
  content: string
  sources?: { title: string; icon: string; url: string }[]
  rating?: 'up' | 'down' | null
  attachments?: AttachedContent[]
  analysis?: ContentAnalysis
  timestamp?: Date
}

interface SuggestedPrompt {
  title: string
  text: string
  icon: string
  iconBg: string
  iconColor: string
}

interface QuickAction {
  id: string
  label: string
  icon: string
}

interface Settings {
  style: 'concise' | 'detailed' | 'conversational'
  language: 'en' | 'ar' | 'fr' | 'es'
  showSources: boolean
  saveHistory: boolean
}

// ============================================================================
// Advanced AI Features - Types
// ============================================================================

interface AttachedContent {
  id: string
  type: 'document' | 'article' | 'media' | 'course' | 'url'
  title: string
  icon: string
  preview?: string
  url?: string
}

interface ContentAnalysis {
  summary?: string
  keyPoints?: string[]
  entities?: { text: string; type: string }[]
  sentiment?: { label: string; score: number }
  topics?: string[]
}

interface ConversationContext {
  topics: string[]
  entities: { text: string; type: string; count: number }[]
  intent: string
  summaryPoints: string[]
  documentReferences: string[]
}

interface PlatformContent {
  id: string
  type: 'article' | 'document' | 'video' | 'course' | 'event'
  title: string
  excerpt: string
  icon: string
  iconBg: string
}

interface ConversationSummary {
  title: string
  keyTopics: string[]
  questionsAsked: number
  mainInsights: string[]
  generatedAt: Date
}

// State
const showSettings = ref(false)
const searchHistory = ref('')
const inputMessage = ref('')
const isTyping = ref(false)
const activeChat = ref<Chat | null>(null)
const inputHeight = ref('48px')

const settings = ref<Settings>({
  style: 'detailed',
  language: 'en',
  showSources: true,
  saveHistory: true
})

const messages = ref<Message[]>([])

// Mock data for conversation history
const todayChats = ref<Chat[]>([
  { id: 1, title: 'Company Policy Questions', preview: 'What is the remote work policy?' },
  { id: 2, title: 'Document Summary', preview: 'Summarize the Q4 report...' }
])

const yesterdayChats = ref<Chat[]>([
  { id: 3, title: 'Training Resources', preview: 'Find training materials for new hires' },
  { id: 4, title: 'Meeting Notes Help', preview: 'Help me format meeting notes' }
])

const olderChats = ref<Chat[]>([
  { id: 5, title: 'Project Planning', preview: 'Create a project timeline' },
  { id: 6, title: 'Benefits Information', preview: 'Explain health insurance options' }
])

const suggestedPrompts = ref<SuggestedPrompt[]>([
  { title: 'Search Knowledge Base', text: 'Find articles about employee onboarding', icon: 'fas fa-search', iconBg: 'bg-blue-100', iconColor: 'text-blue-600' },
  { title: 'Summarize Document', text: 'Summarize the latest quarterly report', icon: 'fas fa-file-alt', iconBg: 'bg-purple-100', iconColor: 'text-purple-600' },
  { title: 'Policy Questions', text: 'What is the company travel policy?', icon: 'fas fa-book', iconBg: 'bg-green-100', iconColor: 'text-green-600' },
  { title: 'Help with Tasks', text: 'Help me write a project proposal', icon: 'fas fa-tasks', iconBg: 'bg-orange-100', iconColor: 'text-orange-600' }
])

const quickActions = ref<QuickAction[]>([
  { id: 'search', label: 'Search docs', icon: 'fas fa-search' },
  { id: 'summarize', label: 'Summarize', icon: 'fas fa-compress-alt' },
  { id: 'translate', label: 'Translate', icon: 'fas fa-language' },
  { id: 'write', label: 'Help write', icon: 'fas fa-pen' },
  { id: 'analyze', label: 'Analyze content', icon: 'fas fa-chart-bar' },
  { id: 'compare', label: 'Compare docs', icon: 'fas fa-columns' }
])

// ============================================================================
// Advanced AI Features - State
// ============================================================================

const showContextPanel = ref(false)
const showContentBrowser = ref(false)
const showSummaryModal = ref(false)
const isAnalyzingContent = ref(false)
const isGeneratingSummary = ref(false)
const showAttachmentMenu = ref(false)

// Conversation Context
const conversationContext = ref<ConversationContext>({
  topics: ['AFC Asian Cup 2027', 'Tournament Management'],
  entities: [
    { text: 'AFC', type: 'organization', count: 3 },
    { text: 'Saudi Arabia', type: 'location', count: 2 }
  ],
  intent: 'Information seeking',
  summaryPoints: [
    'User is exploring tournament management features',
    'Interest in document and policy information'
  ],
  documentReferences: ['Tournament Handbook', 'Venue Guidelines']
})

// Attached content for current message
const pendingAttachments = ref<AttachedContent[]>([])

// Platform content browser
const platformContentSearch = ref('')
const platformContentResults = ref<PlatformContent[]>([])
const isSearchingContent = ref(false)

// Conversation summary
const conversationSummary = ref<ConversationSummary | null>(null)

// Mock platform content for browser
const mockPlatformContent: PlatformContent[] = [
  { id: '1', type: 'article', title: 'AFC Asian Cup 2027 Overview', excerpt: 'Complete guide to the tournament structure and format...', icon: 'fas fa-file-alt', iconBg: 'bg-blue-100' },
  { id: '2', type: 'document', title: 'Tournament Regulations 2027', excerpt: 'Official regulations governing the AFC Asian Cup...', icon: 'fas fa-file-pdf', iconBg: 'bg-red-100' },
  { id: '3', type: 'document', title: 'Venue Operations Manual', excerpt: 'Guidelines for stadium operations during matches...', icon: 'fas fa-file-pdf', iconBg: 'bg-red-100' },
  { id: '4', type: 'video', title: 'Stadium Tour - King Fahd Stadium', excerpt: 'Virtual tour of the main tournament venue...', icon: 'fas fa-video', iconBg: 'bg-purple-100' },
  { id: '5', type: 'course', title: 'Tournament Staff Training', excerpt: 'Essential training for event staff members...', icon: 'fas fa-graduation-cap', iconBg: 'bg-green-100' },
  { id: '6', type: 'article', title: 'Media Accreditation Process', excerpt: 'Step-by-step guide for media registration...', icon: 'fas fa-file-alt', iconBg: 'bg-blue-100' },
  { id: '7', type: 'document', title: 'Ticketing Policy Document', excerpt: 'Official ticketing guidelines and procedures...', icon: 'fas fa-file-pdf', iconBg: 'bg-red-100' },
  { id: '8', type: 'event', title: 'Opening Ceremony Planning', excerpt: 'Details about the tournament opening event...', icon: 'fas fa-calendar', iconBg: 'bg-amber-100' }
]

// Smart follow-up suggestions based on context
const followUpSuggestions = computed(() => {
  if (messages.value.length === 0) return []

  const lastMessage = messages.value[messages.value.length - 1]
  if (!lastMessage || lastMessage.role !== 'assistant') return []

  return [
    'Tell me more about this',
    'Can you provide examples?',
    'What are the related policies?',
    'Summarize the key points'
  ]
})

// ============================================================================
// Advanced AI Features - Functions
// ============================================================================

// Search platform content
async function searchPlatformContent() {
  if (!platformContentSearch.value.trim()) {
    platformContentResults.value = mockPlatformContent.slice(0, 6)
    return
  }

  isSearchingContent.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 500))

    const query = platformContentSearch.value.toLowerCase()
    platformContentResults.value = mockPlatformContent.filter(
      item => item.title.toLowerCase().includes(query) ||
              item.excerpt.toLowerCase().includes(query)
    )
  } finally {
    isSearchingContent.value = false
  }
}

// Attach content to message
function attachContent(content: PlatformContent) {
  const attachment: AttachedContent = {
    id: content.id,
    type: content.type as AttachedContent['type'],
    title: content.title,
    icon: content.icon,
    preview: content.excerpt
  }

  pendingAttachments.value.push(attachment)
  showContentBrowser.value = false
}

// Remove attachment
function removeAttachment(id: string) {
  pendingAttachments.value = pendingAttachments.value.filter(a => a.id !== id)
}

// Analyze attached content
async function analyzeAttachedContent(attachment: AttachedContent): Promise<ContentAnalysis> {
  await new Promise(resolve => setTimeout(resolve, 1000))

  return {
    summary: `This ${attachment.type} discusses key aspects of ${attachment.title}. It covers important guidelines and procedures relevant to the AFC Asian Cup 2027.`,
    keyPoints: [
      'Main objective and scope outlined',
      'Step-by-step procedures provided',
      'Compliance requirements specified',
      'Timeline and deadlines mentioned'
    ],
    entities: [
      { text: 'AFC', type: 'organization' },
      { text: 'Asian Cup 2027', type: 'event' },
      { text: 'Saudi Arabia', type: 'location' }
    ],
    sentiment: { label: 'Neutral/Informative', score: 0.85 },
    topics: ['Tournament Management', 'Regulations', 'Operations']
  }
}

// Generate conversation summary
async function generateConversationSummary() {
  if (messages.value.length < 2) return

  isGeneratingSummary.value = true
  showSummaryModal.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 1200))

    const userMessages = messages.value.filter(m => m.role === 'user')

    conversationSummary.value = {
      title: activeChat.value?.title || 'Current Conversation',
      keyTopics: conversationContext.value.topics,
      questionsAsked: userMessages.length,
      mainInsights: [
        'Discussed tournament management procedures',
        'Explored document and policy guidelines',
        'Reviewed venue operations requirements',
        'Identified key compliance areas'
      ],
      generatedAt: new Date()
    }
  } finally {
    isGeneratingSummary.value = false
  }
}

// Update conversation context based on new message
function updateConversationContext(message: string) {
  // Extract potential topics
  const topicKeywords = ['tournament', 'venue', 'ticket', 'media', 'team', 'match', 'policy', 'document']
  const foundTopics = topicKeywords.filter(topic =>
    message.toLowerCase().includes(topic)
  )

  if (foundTopics.length > 0) {
    const newTopics = foundTopics.map(t => t.charAt(0).toUpperCase() + t.slice(1))
    conversationContext.value.topics = [
      ...new Set([...conversationContext.value.topics, ...newTopics])
    ].slice(0, 5)
  }

  // Update summary points
  conversationContext.value.summaryPoints.push(
    `User asked about: ${message.substring(0, 50)}...`
  )
  if (conversationContext.value.summaryPoints.length > 5) {
    conversationContext.value.summaryPoints.shift()
  }
}

// Use follow-up suggestion
function useFollowUp(suggestion: string) {
  inputMessage.value = suggestion
  sendMessage()
}

// Get content type icon
function getContentTypeIcon(type: string): string {
  const icons: Record<string, string> = {
    article: 'fas fa-file-alt',
    document: 'fas fa-file-pdf',
    video: 'fas fa-video',
    media: 'fas fa-photo-video',
    course: 'fas fa-graduation-cap',
    event: 'fas fa-calendar',
    url: 'fas fa-link'
  }
  return icons[type] || 'fas fa-file'
}

// Get content type color
function getContentTypeColor(type: string): string {
  const colors: Record<string, string> = {
    article: 'bg-blue-100 text-blue-600',
    document: 'bg-red-100 text-red-600',
    video: 'fas fa-purple-100 text-purple-600',
    media: 'bg-pink-100 text-pink-600',
    course: 'bg-green-100 text-green-600',
    event: 'bg-amber-100 text-amber-600',
    url: 'bg-gray-100 text-gray-600'
  }
  return colors[type] || 'bg-gray-100 text-gray-600'
}

// Open content browser
function openContentBrowser() {
  showContentBrowser.value = true
  platformContentResults.value = mockPlatformContent.slice(0, 6)
}

// Methods
function startNewChat() {
  activeChat.value = null
  messages.value = []
}

function selectChat(chat: Chat) {
  activeChat.value = chat
  // Load chat messages (mock)
  messages.value = [
    { role: 'user', content: chat.preview },
    {
      role: 'assistant',
      content: `<p>I'd be happy to help you with that! Here's what I found about "${chat.title}":</p><ul><li>First relevant point about the topic</li><li>Second important detail</li><li>Additional context and information</li></ul><p>Would you like me to elaborate on any of these points?</p>`,
      sources: [
        { title: 'Company Handbook', icon: 'fas fa-book', url: '#' },
        { title: 'HR Policies', icon: 'fas fa-file-alt', url: '#' }
      ]
    }
  ]
}

async function sendMessage() {
  if (!inputMessage.value.trim() || isTyping.value) return

  const userQuery = inputMessage.value
  const attachments = [...pendingAttachments.value]

  // Create user message with attachments
  const userMessage: Message = {
    role: 'user',
    content: inputMessage.value,
    attachments: attachments.length > 0 ? attachments : undefined,
    timestamp: new Date()
  }

  messages.value.push(userMessage)
  inputMessage.value = ''
  pendingAttachments.value = []
  isTyping.value = true

  // Update conversation context
  updateConversationContext(userQuery)

  // Analyze attachments if present
  let analysisResults: ContentAnalysis | undefined
  const firstAttachment = attachments[0]
  if (attachments.length > 0 && firstAttachment) {
    isAnalyzingContent.value = true
    try {
      analysisResults = await analyzeAttachedContent(firstAttachment)
    } finally {
      isAnalyzingContent.value = false
    }
  }

  // Simulate AI response
  setTimeout(() => {
    let responseContent = ''

    if (firstAttachment && analysisResults) {
      responseContent = `<p>I've analyzed the ${firstAttachment.type} "<strong>${firstAttachment.title}</strong>" along with your question about "${userQuery}".</p>
        <div class="ai-analysis-box">
          <p class="font-semibold mb-2"><i class="fas fa-chart-bar mr-2"></i>Content Analysis:</p>
          <p class="mb-2">${analysisResults.summary}</p>
          <p class="font-semibold mb-1">Key Points:</p>
          <ul>${analysisResults.keyPoints?.map(p => `<li>${p}</li>`).join('')}</ul>
        </div>
        <p>Is there anything specific about this document you'd like me to elaborate on?</p>`
    } else {
      responseContent = `<p>Thank you for your question about "${userQuery}".</p>
        <p>Based on my analysis of the knowledge base, here's what I found:</p>
        <ul>
          <li>The relevant policy documentation indicates comprehensive guidelines are available</li>
          <li>According to recent updates, procedures have been streamlined</li>
          <li>Best practices suggest following the official handbook</li>
        </ul>
        <p>Is there anything specific you'd like me to clarify?</p>`
    }

    messages.value.push({
      role: 'assistant',
      content: responseContent,
      sources: [
        { title: 'Knowledge Base Article', icon: 'fas fa-file-alt', url: '#' },
        { title: 'Policy Document', icon: 'fas fa-book', url: '#' }
      ],
      analysis: analysisResults,
      timestamp: new Date()
    })
    isTyping.value = false
  }, attachments.length > 0 ? 2500 : 1500)
}

function usePrompt(text: string) {
  inputMessage.value = text
  sendMessage()
}

function triggerAction(action: QuickAction) {
  if (action.id === 'analyze') {
    openContentBrowser()
    return
  }

  if (action.id === 'compare') {
    // If we have items selected for comparison, open the comparison modal
    if (comparisonStore.canCompare) {
      comparisonStore.startComparison()
    } else if (comparisonStore.hasItems) {
      // Show a hint that more items need to be selected
      inputMessage.value = 'Select at least 2 items to compare. You currently have ' + comparisonStore.itemCount + ' item(s) selected.'
    } else {
      // Open content browser to select items
      openContentBrowser()
      inputMessage.value = 'Select documents, articles, or other content to compare. Use the "Add to Compare" button on items across the portal.'
    }
    return
  }

  const prefixes: Record<string, string> = {
    'search': 'Search the knowledge base for: ',
    'summarize': 'Please summarize: ',
    'translate': 'Translate the following to Arabic: ',
    'write': 'Help me write: '
  }
  inputMessage.value = prefixes[action.id] || ''
}

function copyMessage(msg: Message) {
  navigator.clipboard.writeText(msg.content.replace(/<[^>]*>/g, ''))
}

function regenerateMessage(idx: number) {
  console.log('Regenerating message at index', idx)
}

function rateMessage(msg: Message, rating: 'up' | 'down') {
  msg.rating = msg.rating === rating ? null : rating
}

function exportChat() {
  alert('Exporting conversation...')
}

function shareChat() {
  alert('Sharing conversation...')
}

function saveSettings() {
  showSettings.value = false
}
</script>

<template>
  <div class="flex min-h-[calc(100vh-80px)]">
    <!-- Conversation History Sidebar -->
    <aside class="w-80 border-r border-teal-100 bg-white/50 flex-shrink-0 flex flex-col h-[calc(100vh-80px)] card-animated fade-in-up">
      <div class="p-4 border-b border-teal-100">
        <button @click="startNewChat" class="btn btn-vibrant w-full ripple">
          <i class="fas fa-plus icon-vibrant"></i> New Conversation
        </button>
      </div>

      <div class="p-4">
        <div class="input-group">
          <i class="input-icon fas fa-search"></i>
          <input type="text" v-model="searchHistory" placeholder="Search conversations..." class="input text-sm">
        </div>
      </div>

      <div class="flex-1 overflow-y-auto scrollbar-thin">
        <div class="px-4 py-2">
          <span class="text-xs font-semibold text-teal-500 uppercase tracking-wider">Today</span>
        </div>
        <div v-for="chat in todayChats" :key="chat.id"
             @click="selectChat(chat)"
             :class="['mx-2 p-3 rounded-xl cursor-pointer transition-all mb-1 list-item-animated ripple',
                      activeChat?.id === chat.id ? 'bg-teal-100' : 'hover:bg-teal-50']">
          <div class="flex items-start gap-3">
            <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center flex-shrink-0 ai-glow">
              <i class="fas fa-comment text-white text-xs icon-soft"></i>
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-teal-900 truncate">{{ chat.title }}</p>
              <p class="text-xs text-teal-500 truncate">{{ chat.preview }}</p>
            </div>
          </div>
        </div>

        <div class="px-4 py-2 mt-2">
          <span class="text-xs font-semibold text-teal-500 uppercase tracking-wider">Yesterday</span>
        </div>
        <div v-for="chat in yesterdayChats" :key="chat.id"
             @click="selectChat(chat)"
             :class="['mx-2 p-3 rounded-xl cursor-pointer transition-all mb-1 list-item-animated ripple',
                      activeChat?.id === chat.id ? 'bg-teal-100' : 'hover:bg-teal-50']">
          <div class="flex items-start gap-3">
            <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center flex-shrink-0 ai-glow">
              <i class="fas fa-comment text-white text-xs icon-soft"></i>
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-teal-900 truncate">{{ chat.title }}</p>
              <p class="text-xs text-teal-500 truncate">{{ chat.preview }}</p>
            </div>
          </div>
        </div>

        <div class="px-4 py-2 mt-2">
          <span class="text-xs font-semibold text-teal-500 uppercase tracking-wider">Previous 7 Days</span>
        </div>
        <div v-for="chat in olderChats" :key="chat.id"
             @click="selectChat(chat)"
             :class="['mx-2 p-3 rounded-xl cursor-pointer transition-all mb-1 list-item-animated ripple',
                      activeChat?.id === chat.id ? 'bg-teal-100' : 'hover:bg-teal-50']">
          <div class="flex items-start gap-3">
            <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center flex-shrink-0 ai-glow">
              <i class="fas fa-comment text-white text-xs icon-soft"></i>
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-teal-900 truncate">{{ chat.title }}</p>
              <p class="text-xs text-teal-500 truncate">{{ chat.preview }}</p>
            </div>
          </div>
        </div>
      </div>
    </aside>

    <!-- Main Chat Area -->
    <div class="flex-1 flex flex-col h-[calc(100vh-80px)]">
      <!-- Chat Header -->
      <div class="p-4 border-b border-teal-100 bg-white/50 flex items-center justify-between card-animated fade-in-up">
        <div class="flex items-center gap-3">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center ai-glow">
            <i class="fas fa-robot text-white text-lg icon-vibrant"></i>
          </div>
          <div>
            <h1 class="font-semibold text-teal-900">Intalio AI Assistant</h1>
            <p class="text-xs text-teal-500">Powered by advanced AI • Multi-turn context enabled</p>
          </div>
        </div>
        <div class="flex items-center gap-2">
          <!-- Context Panel Toggle -->
          <button @click="showContextPanel = !showContextPanel"
                  :class="['p-2 rounded-lg transition-colors ripple',
                           showContextPanel ? 'bg-teal-100 text-teal-700' : 'hover:bg-teal-100 text-teal-600']"
                  title="Conversation Context">
            <i class="fas fa-brain icon-soft"></i>
          </button>
          <!-- Summarize Conversation -->
          <button @click="generateConversationSummary"
                  :disabled="messages.length < 2"
                  :class="['p-2 rounded-lg hover:bg-teal-100 text-teal-600 ripple',
                           messages.length < 2 ? 'opacity-50 cursor-not-allowed' : '']"
                  title="Summarize Conversation">
            <i class="fas fa-list-alt icon-soft"></i>
          </button>
          <button @click="showSettings = true" class="p-2 rounded-lg hover:bg-teal-100 text-teal-600 ripple" title="Settings">
            <i class="fas fa-cog icon-soft"></i>
          </button>
          <button @click="exportChat" class="p-2 rounded-lg hover:bg-teal-100 text-teal-600 ripple" title="Export">
            <i class="fas fa-download icon-soft"></i>
          </button>
          <button @click="shareChat" class="p-2 rounded-lg hover:bg-teal-100 text-teal-600 ripple" title="Share">
            <i class="fas fa-share-alt icon-soft"></i>
          </button>
        </div>
      </div>

      <!-- Messages Area -->
      <div class="flex-1 overflow-y-auto p-6 scrollbar-thin">
        <!-- Welcome State -->
        <div v-if="messages.length === 0" class="h-full flex flex-col items-center justify-center text-center fade-in-up">
          <div class="w-20 h-20 rounded-2xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center mb-10 shadow-xl ai-glow">
            <i class="fas fa-robot text-white text-3xl icon-vibrant"></i>
          </div>
          <h2 class="text-2xl font-bold text-teal-900 mb-2">How can I help you today?</h2>
          <p class="text-teal-600 mb-10 max-w-md typing-cursor">I can help you find information, answer questions about company policies, summarize documents, and much more.</p>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 max-w-2xl w-full">
            <div v-for="(prompt, idx) in suggestedPrompts" :key="idx"
                 @click="usePrompt(prompt.text)"
                 class="p-4 rounded-xl bg-white/60 hover:bg-white hover:shadow-lg border border-teal-100 cursor-pointer transition-all text-left group card-animated ripple">
              <div class="flex items-start gap-3">
                <div :class="['w-10 h-10 rounded-lg flex items-center justify-center flex-shrink-0', prompt.iconBg]">
                  <i :class="[prompt.icon, prompt.iconColor, 'icon-soft']"></i>
                </div>
                <div>
                  <p class="font-medium text-teal-900 group-hover:text-teal-700">{{ prompt.title }}</p>
                  <p class="text-sm text-teal-500">{{ prompt.text }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Messages -->
        <div v-else class="space-y-6 max-w-4xl mx-auto">
          <div v-for="(msg, idx) in messages" :key="idx" class="animate-fadeIn fade-in-up">
            <!-- User Message -->
            <div v-if="msg.role === 'user'" class="flex justify-end mb-4">
              <div class="max-w-[80%]">
                <!-- Attachments -->
                <div v-if="msg.attachments && msg.attachments.length > 0" class="flex flex-wrap gap-2 justify-end mb-2">
                  <div v-for="attachment in msg.attachments" :key="attachment.id"
                       class="inline-flex items-center gap-2 px-3 py-1.5 bg-teal-100 rounded-lg text-sm text-teal-700">
                    <i :class="attachment.icon"></i>
                    <span class="font-medium">{{ attachment.title }}</span>
                  </div>
                </div>
                <div class="bg-gradient-to-r from-teal-500 to-teal-600 text-white p-4 rounded-2xl rounded-br-md shadow-lg card-animated">
                  <p>{{ msg.content }}</p>
                </div>
              </div>
            </div>

            <!-- AI Message -->
            <div v-else class="flex gap-4">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center flex-shrink-0 ai-glow">
                <i class="fas fa-robot text-white icon-vibrant"></i>
              </div>
              <div class="flex-1">
                <div class="bg-white/80 p-4 rounded-2xl rounded-tl-md shadow-sm border border-teal-100 max-w-[90%] card-animated">
                  <div class="prose prose-teal text-teal-900" v-html="msg.content"></div>

                  <!-- Sources/References -->
                  <div v-if="msg.sources && msg.sources.length" class="mt-4 pt-4 border-t border-teal-100">
                    <p class="text-xs font-semibold text-teal-500 uppercase mb-2">Sources</p>
                    <div class="flex flex-wrap gap-2">
                      <a v-for="(source, sidx) in msg.sources" :key="sidx"
                         :href="source.url"
                         class="inline-flex items-center gap-2 px-3 py-1.5 bg-teal-50 rounded-lg text-sm text-teal-700 hover:bg-teal-100 transition-colors ripple">
                        <i :class="[source.icon, 'icon-soft']"></i>
                        {{ source.title }}
                      </a>
                    </div>
                  </div>
                </div>

                <!-- Message Actions -->
                <div class="flex items-center gap-2 mt-2 ml-2">
                  <button @click="copyMessage(msg)" class="p-1.5 rounded-lg hover:bg-teal-100 text-teal-400 hover:text-teal-600 text-sm ripple" title="Copy">
                    <i class="fas fa-copy icon-soft"></i>
                  </button>
                  <button @click="regenerateMessage(idx)" class="p-1.5 rounded-lg hover:bg-teal-100 text-teal-400 hover:text-teal-600 text-sm ripple" title="Regenerate">
                    <i class="fas fa-redo icon-soft"></i>
                  </button>
                  <button @click="rateMessage(msg, 'up')" :class="['p-1.5 rounded-lg hover:bg-teal-100 text-sm ripple', msg.rating === 'up' ? 'text-teal-600' : 'text-teal-400 hover:text-teal-600']" title="Helpful">
                    <i class="fas fa-thumbs-up icon-soft"></i>
                  </button>
                  <button @click="rateMessage(msg, 'down')" :class="['p-1.5 rounded-lg hover:bg-teal-100 text-sm ripple', msg.rating === 'down' ? 'text-red-500' : 'text-teal-400 hover:text-teal-600']" title="Not helpful">
                    <i class="fas fa-thumbs-down icon-soft"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Typing Indicator -->
          <div v-if="isTyping" class="flex gap-4 fade-in-up">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center flex-shrink-0 ai-glow">
              <i class="fas fa-robot text-white icon-vibrant"></i>
            </div>
            <div class="bg-white/80 p-4 rounded-2xl rounded-tl-md shadow-sm border border-teal-100 card-animated">
              <div class="flex items-center gap-2">
                <div class="flex items-center gap-1">
                  <div class="w-2 h-2 bg-teal-400 rounded-full animate-bounce" style="animation-delay: 0ms"></div>
                  <div class="w-2 h-2 bg-teal-400 rounded-full animate-bounce" style="animation-delay: 150ms"></div>
                  <div class="w-2 h-2 bg-teal-400 rounded-full animate-bounce" style="animation-delay: 300ms"></div>
                </div>
                <span v-if="isAnalyzingContent" class="text-xs text-teal-600 ml-2">
                  <i class="fas fa-chart-bar mr-1"></i>Analyzing content...
                </span>
              </div>
            </div>
          </div>

          <!-- Follow-up Suggestions -->
          <div v-if="followUpSuggestions.length > 0 && !isTyping" class="flex flex-wrap gap-2 mt-4 fade-in-up">
            <span class="text-xs text-gray-500 mr-2">Suggested follow-ups:</span>
            <button v-for="suggestion in followUpSuggestions" :key="suggestion"
                    @click="useFollowUp(suggestion)"
                    class="px-3 py-1.5 bg-gradient-to-r from-teal-50 to-emerald-50 border border-teal-200 rounded-full text-sm text-teal-700 hover:from-teal-100 hover:to-emerald-100 transition-all">
              {{ suggestion }}
            </button>
          </div>
        </div>
      </div>

      <!-- Input Area -->
      <div class="p-4 border-t border-teal-100 bg-white/50 card-animated">
        <div class="max-w-4xl mx-auto">
          <!-- Quick Actions -->
          <div class="flex flex-wrap gap-2 mb-3">
            <button v-for="action in quickActions" :key="action.id"
                    @click="triggerAction(action)"
                    class="inline-flex items-center gap-2 px-3 py-1.5 bg-teal-50 rounded-full text-sm text-teal-700 hover:bg-teal-100 transition-colors ripple">
              <i :class="[action.icon, 'icon-soft']"></i>
              {{ action.label }}
            </button>
          </div>

          <!-- Pending Attachments -->
          <div v-if="pendingAttachments.length > 0" class="flex flex-wrap gap-2 mb-3 p-3 bg-teal-50 rounded-xl">
            <div v-for="attachment in pendingAttachments" :key="attachment.id"
                 class="inline-flex items-center gap-2 px-3 py-2 bg-white rounded-lg border border-teal-200 shadow-sm">
              <div :class="['w-8 h-8 rounded-lg flex items-center justify-center', getContentTypeColor(attachment.type)]">
                <i :class="attachment.icon"></i>
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium text-gray-900 truncate">{{ attachment.title }}</p>
                <p class="text-xs text-gray-500 capitalize">{{ attachment.type }}</p>
              </div>
              <button @click="removeAttachment(attachment.id)"
                      class="p-1 hover:bg-red-100 rounded-full text-red-500 transition-colors">
                <i class="fas fa-times text-xs"></i>
              </button>
            </div>
            <button @click="openContentBrowser"
                    class="inline-flex items-center gap-2 px-3 py-2 border-2 border-dashed border-teal-300 rounded-lg text-sm text-teal-600 hover:bg-teal-100 transition-colors">
              <i class="fas fa-plus"></i>
              Add more
            </button>
          </div>

          <div class="flex items-end gap-3">
            <div class="flex-1 relative">
              <textarea v-model="inputMessage"
                        @keydown.enter.exact.prevent="sendMessage"
                        @keydown.shift.enter="() => {}"
                        placeholder="Ask me anything... Attach documents for AI analysis"
                        rows="1"
                        class="input resize-none pr-24 min-h-[48px] max-h-32"
                        :style="{ height: inputHeight }"></textarea>
              <div class="absolute right-2 bottom-2 flex items-center gap-1">
                <button @click="openContentBrowser"
                        class="p-2 rounded-lg hover:bg-teal-100 text-teal-500 ripple relative"
                        title="Attach platform content">
                  <i class="fas fa-paperclip icon-soft"></i>
                  <span v-if="pendingAttachments.length > 0"
                        class="absolute -top-1 -right-1 w-4 h-4 bg-teal-500 text-white text-xs rounded-full flex items-center justify-center">
                    {{ pendingAttachments.length }}
                  </span>
                </button>
                <button class="p-2 rounded-lg hover:bg-teal-100 text-teal-500 ripple" title="Voice input">
                  <i class="fas fa-microphone icon-soft"></i>
                </button>
              </div>
            </div>
            <button @click="sendMessage"
                    :disabled="!inputMessage.trim() || isTyping"
                    :class="['btn btn-vibrant btn-icon h-12 w-12 ripple',
                             (!inputMessage.trim() || isTyping) ? 'opacity-50 cursor-not-allowed' : '']">
              <i class="fas fa-paper-plane icon-vibrant"></i>
            </button>
          </div>

          <p class="text-xs text-teal-400 text-center mt-3">
            AI can make mistakes. Consider checking important information. • Attach documents for AI-powered analysis.
          </p>
        </div>
      </div>
    </div>

    <!-- Settings Modal -->
    <div v-if="showSettings" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div class="glass-card rounded-2xl w-full max-w-md card-animated fade-in-up">
        <div class="p-5 border-b border-teal-100 flex items-center justify-between">
          <h2 class="text-xl font-semibold text-teal-900">AI Settings</h2>
          <button @click="showSettings = false" class="p-2 rounded-lg hover:bg-teal-100 text-teal-500 ripple">
            <i class="fas fa-times icon-soft"></i>
          </button>
        </div>
        <div class="p-6 space-y-5">
          <div>
            <label class="block text-sm font-medium text-teal-700 mb-2">Response Style</label>
            <select v-model="settings.style" class="input">
              <option value="concise">Concise</option>
              <option value="detailed">Detailed</option>
              <option value="conversational">Conversational</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-teal-700 mb-2">Language</label>
            <select v-model="settings.language" class="input">
              <option value="en">English</option>
              <option value="ar">Arabic</option>
              <option value="fr">French</option>
              <option value="es">Spanish</option>
            </select>
          </div>
          <div class="flex items-center justify-between">
            <span class="text-sm text-teal-700">Include source references</span>
            <label class="toggle">
              <input type="checkbox" v-model="settings.showSources">
              <span class="toggle-slider"></span>
            </label>
          </div>
          <div class="flex items-center justify-between">
            <span class="text-sm text-teal-700">Save conversation history</span>
            <label class="toggle">
              <input type="checkbox" v-model="settings.saveHistory">
              <span class="toggle-slider"></span>
            </label>
          </div>
        </div>
        <div class="p-5 border-t border-teal-100 flex justify-end gap-3">
          <button @click="showSettings = false" class="btn btn-secondary ripple">Cancel</button>
          <button @click="saveSettings" class="btn btn-vibrant ripple">Save Settings</button>
        </div>
      </div>
    </div>

    <!-- Context Panel (Sidebar) -->
    <Transition name="slide-right">
      <div v-if="showContextPanel" class="fixed right-0 top-0 h-full w-80 bg-white shadow-2xl z-40 flex flex-col border-l border-gray-200">
        <div class="p-4 border-b border-gray-100 bg-gradient-to-r from-teal-500 to-emerald-500">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-white/20 flex items-center justify-center">
                <i class="fas fa-brain text-white"></i>
              </div>
              <div>
                <h3 class="font-semibold text-white">Conversation Context</h3>
                <p class="text-xs text-white/80">AI understands your intent</p>
              </div>
            </div>
            <button @click="showContextPanel = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
              <i class="fas fa-times text-white"></i>
            </button>
          </div>
        </div>

        <div class="flex-1 overflow-y-auto p-4 space-y-6">
          <!-- Intent -->
          <div>
            <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">Detected Intent</h4>
            <div class="flex items-center gap-2 p-3 bg-teal-50 rounded-xl">
              <i class="fas fa-compass text-teal-600"></i>
              <span class="text-sm font-medium text-teal-900">{{ conversationContext.intent }}</span>
            </div>
          </div>

          <!-- Topics -->
          <div>
            <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">Active Topics</h4>
            <div class="flex flex-wrap gap-2">
              <span v-for="topic in conversationContext.topics" :key="topic"
                    class="px-3 py-1.5 bg-blue-100 text-blue-700 rounded-full text-sm font-medium">
                {{ topic }}
              </span>
            </div>
          </div>

          <!-- Entities -->
          <div>
            <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">Recognized Entities</h4>
            <div class="space-y-2">
              <div v-for="entity in conversationContext.entities" :key="entity.text"
                   class="flex items-center justify-between p-2 bg-gray-50 rounded-lg">
                <div class="flex items-center gap-2">
                  <i :class="[entity.type === 'organization' ? 'fas fa-building' :
                              entity.type === 'location' ? 'fas fa-map-marker-alt' : 'fas fa-tag',
                              'text-gray-400 text-sm']"></i>
                  <span class="text-sm text-gray-700">{{ entity.text }}</span>
                </div>
                <span class="text-xs text-gray-400">{{ entity.count }}x</span>
              </div>
            </div>
          </div>

          <!-- Summary Points -->
          <div>
            <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">Context Summary</h4>
            <ul class="space-y-2">
              <li v-for="(point, idx) in conversationContext.summaryPoints" :key="idx"
                  class="flex items-start gap-2 text-sm text-gray-600">
                <i class="fas fa-check-circle text-teal-500 mt-0.5"></i>
                <span>{{ point }}</span>
              </li>
            </ul>
          </div>

          <!-- Referenced Documents -->
          <div v-if="conversationContext.documentReferences.length > 0">
            <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">Referenced Documents</h4>
            <div class="space-y-2">
              <div v-for="doc in conversationContext.documentReferences" :key="doc"
                   class="flex items-center gap-2 p-2 bg-amber-50 rounded-lg text-sm text-amber-700">
                <i class="fas fa-file-alt"></i>
                <span>{{ doc }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Content Browser Modal -->
    <Teleport to="body">
      <div v-if="showContentBrowser" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-3xl max-h-[80vh] overflow-hidden">
          <div class="p-5 border-b border-gray-100 bg-gradient-to-r from-teal-500 to-emerald-500">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-white/20 flex items-center justify-center">
                  <i class="fas fa-folder-open text-white"></i>
                </div>
                <div>
                  <h3 class="font-semibold text-white">Browse Platform Content</h3>
                  <p class="text-xs text-white/80">Select content for AI analysis</p>
                </div>
              </div>
              <button @click="showContentBrowser = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <i class="fas fa-times text-white"></i>
              </button>
            </div>
          </div>

          <div class="p-4 border-b border-gray-100">
            <div class="relative">
              <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-gray-400"></i>
              <input v-model="platformContentSearch"
                     @input="searchPlatformContent"
                     type="text"
                     placeholder="Search articles, documents, videos..."
                     class="w-full pl-10 pr-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-teal-500 focus:border-teal-500">
            </div>
          </div>

          <div class="p-4 overflow-y-auto max-h-[50vh]">
            <div v-if="isSearchingContent" class="text-center py-8">
              <i class="fas fa-circle-notch fa-spin text-teal-500 text-2xl mb-2"></i>
              <p class="text-gray-500">Searching platform content...</p>
            </div>

            <div v-else-if="platformContentResults.length === 0" class="text-center py-8">
              <i class="fas fa-search text-gray-300 text-4xl mb-3"></i>
              <p class="text-gray-500">No content found. Try a different search.</p>
            </div>

            <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-3">
              <button v-for="content in platformContentResults" :key="content.id"
                      @click="attachContent(content)"
                      class="p-4 border border-gray-200 rounded-xl text-left hover:border-teal-300 hover:bg-teal-50 transition-all group">
                <div class="flex items-start gap-3">
                  <div :class="['w-10 h-10 rounded-lg flex items-center justify-center flex-shrink-0', content.iconBg]">
                    <i :class="[content.icon, 'text-gray-600']"></i>
                  </div>
                  <div class="flex-1 min-w-0">
                    <p class="font-medium text-gray-900 group-hover:text-teal-700 truncate">{{ content.title }}</p>
                    <p class="text-xs text-gray-500 line-clamp-2 mt-1">{{ content.excerpt }}</p>
                    <span class="inline-flex items-center gap-1 mt-2 text-xs text-gray-400 capitalize">
                      <i :class="content.icon"></i>
                      {{ content.type }}
                    </span>
                  </div>
                </div>
              </button>
            </div>
          </div>

          <div class="p-4 border-t border-gray-100 flex justify-between items-center">
            <p class="text-sm text-gray-500">
              <i class="fas fa-info-circle mr-1"></i>
              Select content to analyze with AI
            </p>
            <button @click="showContentBrowser = false"
                    class="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors">
              Cancel
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- Conversation Summary Modal -->
    <Teleport to="body">
      <div v-if="showSummaryModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-lg overflow-hidden">
          <div class="p-5 border-b border-gray-100 bg-gradient-to-r from-teal-500 to-emerald-500">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-white/20 flex items-center justify-center">
                  <i class="fas fa-list-alt text-white"></i>
                </div>
                <div>
                  <h3 class="font-semibold text-white">Conversation Summary</h3>
                  <p class="text-xs text-white/80">AI-generated overview</p>
                </div>
              </div>
              <button @click="showSummaryModal = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <i class="fas fa-times text-white"></i>
              </button>
            </div>
          </div>

          <div class="p-6">
            <div v-if="isGeneratingSummary" class="text-center py-8">
              <i class="fas fa-circle-notch fa-spin text-teal-500 text-3xl mb-3"></i>
              <p class="text-gray-600">Generating conversation summary...</p>
            </div>

            <div v-else-if="conversationSummary" class="space-y-5">
              <div>
                <h4 class="text-lg font-semibold text-gray-900 mb-1">{{ conversationSummary.title }}</h4>
                <p class="text-xs text-gray-500">
                  Generated {{ conversationSummary.generatedAt.toLocaleTimeString() }}
                </p>
              </div>

              <div class="flex items-center gap-4 p-3 bg-teal-50 rounded-xl">
                <div class="text-center">
                  <p class="text-2xl font-bold text-teal-600">{{ conversationSummary.questionsAsked }}</p>
                  <p class="text-xs text-gray-500">Questions Asked</p>
                </div>
                <div class="w-px h-10 bg-teal-200"></div>
                <div class="text-center">
                  <p class="text-2xl font-bold text-teal-600">{{ conversationSummary.keyTopics.length }}</p>
                  <p class="text-xs text-gray-500">Topics Covered</p>
                </div>
                <div class="w-px h-10 bg-teal-200"></div>
                <div class="text-center">
                  <p class="text-2xl font-bold text-teal-600">{{ conversationSummary.mainInsights.length }}</p>
                  <p class="text-xs text-gray-500">Key Insights</p>
                </div>
              </div>

              <div>
                <h5 class="text-sm font-semibold text-gray-700 mb-2">Key Topics</h5>
                <div class="flex flex-wrap gap-2">
                  <span v-for="topic in conversationSummary.keyTopics" :key="topic"
                        class="px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-sm">
                    {{ topic }}
                  </span>
                </div>
              </div>

              <div>
                <h5 class="text-sm font-semibold text-gray-700 mb-2">Main Insights</h5>
                <ul class="space-y-2">
                  <li v-for="(insight, idx) in conversationSummary.mainInsights" :key="idx"
                      class="flex items-start gap-2 text-sm text-gray-600">
                    <i class="fas fa-lightbulb text-amber-500 mt-0.5"></i>
                    <span>{{ insight }}</span>
                  </li>
                </ul>
              </div>
            </div>
          </div>

          <div class="p-4 border-t border-gray-100 flex justify-end gap-3">
            <button @click="showSummaryModal = false"
                    class="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors">
              Close
            </button>
            <button class="px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors flex items-center gap-2">
              <i class="fas fa-download"></i>
              Export Summary
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- Comparison Components -->
    <ComparisonPanel />
    <ComparisonModal />
  </div>
</template>

<style scoped>
/* Typing cursor animation */
.typing-cursor::after {
  content: '|';
  animation: blink 1s infinite;
}

@keyframes blink {
  0%, 50% { opacity: 1; }
  51%, 100% { opacity: 0; }
}

/* Prose styling for AI responses */
.prose ul {
  list-style-type: disc;
  padding-left: 1.5rem;
  margin: 0.5rem 0;
}

.prose li {
  margin: 0.25rem 0;
}

.prose p {
  margin: 0.5rem 0;
}

.prose p:first-child {
  margin-top: 0;
}

.prose p:last-child {
  margin-bottom: 0;
}

/* AI Analysis Box */
.prose :deep(.ai-analysis-box) {
  background: linear-gradient(135deg, rgba(20, 184, 166, 0.1), rgba(16, 185, 129, 0.1));
  border: 1px solid rgba(20, 184, 166, 0.3);
  border-radius: 12px;
  padding: 1rem;
  margin: 1rem 0;
}

.prose :deep(.ai-analysis-box) ul {
  margin: 0.5rem 0;
}

/* Line clamp */
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Context Panel Slide Animation */
.slide-right-enter-active,
.slide-right-leave-active {
  transition: transform 0.3s ease, opacity 0.3s ease;
}

.slide-right-enter-from,
.slide-right-leave-to {
  transform: translateX(100%);
  opacity: 0;
}

/* Follow-up suggestions animation */
@keyframes fadeSlideUp {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.fade-in-up {
  animation: fadeSlideUp 0.3s ease forwards;
}

/* Attachment badge pulse */
@keyframes badgePulse {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.1);
  }
}

.attachment-badge {
  animation: badgePulse 2s ease infinite;
}

/* Content browser card hover */
.content-card:hover {
  transform: translateY(-2px);
}
</style>
