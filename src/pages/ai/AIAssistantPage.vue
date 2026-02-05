<script setup lang="ts">
import { ref, computed, nextTick, onMounted, onUnmounted, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import { sanitizeHtml } from '@/utils/sanitize'
import { useAIServicesStore } from '@/stores/aiServices'
import { useComparisonStore } from '@/stores/comparison'
import { useAIWorkflowsStore } from '@/stores/aiWorkflows'
import { useAIPreferencesStore } from '@/stores/aiPreferences'
import { useUIStore } from '@/stores/ui'
import {
  AILoadingIndicator,
  AIMessageContent,
  AIVoiceInput,
  AIOperationProgress,
  AIContentSuggestions,
  AIWorkflowBuilder
} from '@/components/ai'
import ComparisonPanel from '@/components/ai/ComparisonPanel.vue'
import ComparisonModal from '@/components/ai/ComparisonModal.vue'
import { useSlashCommands } from '@/composables/useSlashCommands'
import { useVoiceInput } from '@/composables/useVoiceInput'
import type { AIOperationType } from '@/types/ai'

const { t } = useI18n()
const aiStore = useAIServicesStore()
const comparisonStore = useComparisonStore()
const workflowsStore = useAIWorkflowsStore()
const preferencesStore = useAIPreferencesStore()
const uiStore = useUIStore()

// Sidebar state from main layout
const isSidebarCollapsed = computed(() => uiStore.sidebarCollapsed)

// Keyboard shortcuts
function handleKeyboard(e: KeyboardEvent) {
  // Ctrl+K for tools palette
  if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') {
    e.preventDefault()
    e.stopPropagation()
    showToolsPalette.value = !showToolsPalette.value
    console.log('Ctrl+K pressed, showToolsPalette:', showToolsPalette.value)
  }
  // Escape to close modals
  if (e.key === 'Escape') {
    showToolsPalette.value = false
    showWorkflowBuilder.value = false
  }
}

// Initialize stores and keyboard listener
onMounted(() => {
  workflowsStore.initialize()
  preferencesStore.initialize()
  window.addEventListener('keydown', handleKeyboard)
  console.log('Keyboard listener registered')
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleKeyboard)
})

// Slash commands
const {
  parseInput: parseCommand,
  filterCommands: getCommandSuggestions,
  SLASH_COMMANDS: availableCommands
} = useSlashCommands()

// Voice input
const {
  isListening,
  isSupported: isVoiceSupported,
  transcript,
  startListening,
  stopListening
} = useVoiceInput()

// Types
interface Chat {
  id: number
  title: string
  preview: string
  timestamp?: Date
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
const isHistorySidebarCollapsed = ref(false)
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

// ============================================================================
// Configurable Text Constants - All UI text in one place for easy localization
// ============================================================================
const textConstants = computed(() => ({
  // Header
  appTitle: t('ai.appTitle'),
  appSubtitle: t('ai.appSubtitle'),

  // Buttons & Actions
  tools: t('ai.tools'),
  workflows: t('ai.workflows'),
  newChat: t('ai.newChat'),
  saveSettings: t('ai.saveSettings'),
  addMore: t('ai.addMore'),

  // History Sidebar
  chats: t('ai.chats'),
  today: t('ai.today'),
  yesterday: t('ai.yesterday'),
  previous7Days: t('ai.previous7Days'),
  noConversationsYet: t('ai.noConversationsYet'),
  searchPlaceholder: t('ai.searchPlaceholder'),
  expand: t('ai.expand'),
  collapse: t('ai.collapse'),

  // Welcome Screen
  welcomeTitle: t('ai.welcomeTitle'),
  welcomeSubtitle: t('ai.welcomeSubtitle'),

  // Input Area
  inputPlaceholder: t('ai.inputPlaceholder'),
  slashCommands: t('ai.slashCommands'),
  suggestedFollowUps: t('ai.suggestedFollowUps'),
  attachPlatformContent: t('ai.attachPlatformContent'),
  voiceInputNotSupported: t('ai.voiceInputNotSupported'),

  // Messages
  sources: t('ai.sources'),
  analyzingContent: t('ai.analyzingContent'),

  // Disclaimers
  aiDisclaimer: t('ai.aiDisclaimer'),

  // Settings Panel
  settingsTitle: t('ai.settingsTitle'),
  settingsSubtitle: t('ai.settingsSubtitle'),
  responseStyle: t('ai.responseStyle'),
  language: t('ai.language'),
  preferences: t('ai.preferences'),
  concise: t('ai.concise'),
  conciseDesc: t('ai.conciseDesc'),
  detailed: t('ai.detailed'),
  detailedDesc: t('ai.detailedDesc'),
  conversational: t('ai.conversational'),
  conversationalDesc: t('ai.conversationalDesc'),
  includeSourceReferences: t('ai.includeSourceReferences'),
  saveConversationHistory: t('ai.saveConversationHistory'),

  // Context Panel
  contextIntelligence: t('ai.contextIntelligence'),
  contextSubtitle: t('ai.contextSubtitle'),
  messages: t('ai.messages'),
  entities: t('ai.entities'),
  topics: t('ai.topics'),
  detectedIntent: t('ai.detectedIntent'),
  conversationMood: t('ai.conversationMood'),
  positive: t('ai.positive'),
  negative: t('ai.negative'),
  helpful: t('ai.helpful'),
  informative: t('ai.informative'),
  collaborative: t('ai.collaborative'),
  activeTopics: t('ai.activeTopics'),
  recognizedEntities: t('ai.recognizedEntities'),
  keyInsights: t('ai.keyInsights'),
  referencedDocuments: t('ai.referencedDocuments')
}))

// Chat history - stored conversations (empty by default, populated from store/API)
const chatHistory = ref<Chat[]>([])

// Computed chat history grouped by date
const todayChats = computed(() => {
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  return chatHistory.value.filter(chat => {
    const chatDate = chat.timestamp ? new Date(chat.timestamp) : new Date()
    chatDate.setHours(0, 0, 0, 0)
    return chatDate.getTime() === today.getTime()
  })
})

const yesterdayChats = computed(() => {
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  const yesterday = new Date(today)
  yesterday.setDate(yesterday.getDate() - 1)
  return chatHistory.value.filter(chat => {
    const chatDate = chat.timestamp ? new Date(chat.timestamp) : new Date()
    chatDate.setHours(0, 0, 0, 0)
    return chatDate.getTime() === yesterday.getTime()
  })
})

const olderChats = computed(() => {
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  const yesterday = new Date(today)
  yesterday.setDate(yesterday.getDate() - 1)
  return chatHistory.value.filter(chat => {
    const chatDate = chat.timestamp ? new Date(chat.timestamp) : new Date()
    chatDate.setHours(0, 0, 0, 0)
    return chatDate.getTime() < yesterday.getTime()
  })
})

// Suggested prompts - configurable
const suggestedPrompts = computed<SuggestedPrompt[]>(() => [
  { title: 'Search documents', text: 'Search documents', icon: 'fas fa-search', iconBg: 'bg-blue-100', iconColor: 'text-blue-600' },
  { title: 'Summarize content', text: 'Summarize content', icon: 'fas fa-compress-alt', iconBg: 'bg-purple-100', iconColor: 'text-purple-600' },
  { title: 'Translate text', text: 'Translate text', icon: 'fas fa-language', iconBg: 'bg-green-100', iconColor: 'text-green-600' },
  { title: 'Analyze data', text: 'Analyze data', icon: 'fas fa-chart-bar', iconBg: 'bg-orange-100', iconColor: 'text-orange-600' }
])

// Quick actions for input area
const quickActions = computed<QuickAction[]>(() => [
  { id: 'search', label: 'Search', icon: 'fas fa-search' },
  { id: 'summarize', label: 'Summarize', icon: 'fas fa-compress-alt' },
  { id: 'translate', label: 'Translate', icon: 'fas fa-language' },
  { id: 'analyze', label: 'Analyze', icon: 'fas fa-chart-bar' }
])

// ============================================================================
// Enhanced AI Features - State
// ============================================================================

// Tools Palette & Sidebar
const showToolsPalette = ref(false)
const showWorkflowBuilder = ref(false)
const toolsSearchQuery = ref('')

// Sidebar Tools Configuration
interface SidebarTool {
  id: AIOperationType
  name: string
  description: string
  icon: string
  category: 'analysis' | 'content' | 'extraction' | 'search'
  color: string
  bgColor: string
  hoverBg: string
  options?: { value: string; label: string; icon: string }[]
}

const sidebarTools: SidebarTool[] = [
  {
    id: 'extract-entities',
    name: 'Entity Recognition',
    description: 'Extract people, places, dates',
    icon: 'fas fa-tags',
    category: 'analysis',
    color: 'text-blue-500',
    bgColor: 'bg-blue-50',
    hoverBg: 'group-hover:bg-blue-100'
  },
  {
    id: 'analyze-sentiment',
    name: 'Sentiment Analysis',
    description: 'Analyze emotional tone',
    icon: 'fas fa-smile',
    category: 'analysis',
    color: 'text-pink-500',
    bgColor: 'bg-pink-50',
    hoverBg: 'group-hover:bg-pink-100'
  },
  {
    id: 'classify',
    name: 'Classify Content',
    description: 'Categorize with confidence',
    icon: 'fas fa-folder-tree',
    category: 'analysis',
    color: 'text-purple-500',
    bgColor: 'bg-purple-50',
    hoverBg: 'group-hover:bg-purple-100'
  },
  {
    id: 'summarize',
    name: 'Summarize',
    description: 'Generate concise summaries',
    icon: 'fas fa-compress-alt',
    category: 'content',
    color: 'text-teal-500',
    bgColor: 'bg-teal-50',
    hoverBg: 'hover:bg-teal-100',
    options: [
      { value: 'brief', label: 'Brief', icon: 'fas fa-minus' },
      { value: 'detailed', label: 'Detailed', icon: 'fas fa-align-left' },
      { value: 'bullet', label: 'Bullets', icon: 'fas fa-list-ul' }
    ]
  },
  {
    id: 'translate',
    name: 'Translate',
    description: 'Translate to multiple languages',
    icon: 'fas fa-language',
    category: 'content',
    color: 'text-green-500',
    bgColor: 'bg-green-50',
    hoverBg: 'hover:bg-green-100',
    options: [
      { value: 'ar', label: 'Arabic', icon: '🇸🇦' },
      { value: 'en', label: 'English', icon: '🇺🇸' },
      { value: 'fr', label: 'French', icon: '🇫🇷' },
      { value: 'es', label: 'Spanish', icon: '🇪🇸' },
      { value: 'de', label: 'German', icon: '🇩🇪' },
      { value: 'zh', label: 'Chinese', icon: '🇨🇳' }
    ]
  },
  {
    id: 'generate-title',
    name: 'Generate Titles',
    description: 'Create title suggestions',
    icon: 'fas fa-heading',
    category: 'content',
    color: 'text-indigo-500',
    bgColor: 'bg-indigo-50',
    hoverBg: 'hover:bg-indigo-100',
    options: [
      { value: 'formal', label: 'Formal', icon: 'fas fa-briefcase' },
      { value: 'casual', label: 'Casual', icon: 'fas fa-smile' },
      { value: 'seo', label: 'SEO', icon: 'fas fa-search' },
      { value: 'creative', label: 'Creative', icon: 'fas fa-lightbulb' }
    ]
  },
  {
    id: 'ocr',
    name: 'OCR - Extract Text',
    description: 'Extract text from images',
    icon: 'fas fa-file-image',
    category: 'extraction',
    color: 'text-amber-500',
    bgColor: 'bg-amber-50',
    hoverBg: 'group-hover:bg-amber-100'
  },
  {
    id: 'auto-tag',
    name: 'Auto-Tag',
    description: 'Generate relevant tags',
    icon: 'fas fa-hashtag',
    category: 'extraction',
    color: 'text-cyan-500',
    bgColor: 'bg-cyan-50',
    hoverBg: 'group-hover:bg-cyan-100'
  },
  {
    id: 'smart-search',
    name: 'Smart Search',
    description: 'AI-powered intent detection',
    icon: 'fas fa-search',
    category: 'search',
    color: 'text-orange-500',
    bgColor: 'bg-orange-50',
    hoverBg: 'group-hover:bg-orange-100'
  }
]

const sidebarCategories = {
  analysis: { label: 'Analysis', icon: 'fas fa-chart-bar', iconColor: 'text-blue-400' },
  content: { label: 'Content', icon: 'fas fa-file-alt', iconColor: 'text-teal-400' },
  extraction: { label: 'Extraction', icon: 'fas fa-magic', iconColor: 'text-amber-400' },
  search: { label: 'Search & Discovery', icon: 'fas fa-compass', iconColor: 'text-orange-400' }
}

// Filter tools based on search query
const filteredSidebarTools = computed(() => {
  if (!toolsSearchQuery.value) return sidebarTools

  const query = toolsSearchQuery.value.toLowerCase()
  return sidebarTools.filter(tool =>
    tool.name.toLowerCase().includes(query) ||
    tool.description.toLowerCase().includes(query) ||
    tool.category.toLowerCase().includes(query)
  )
})

// Group tools by category
const groupedSidebarTools = computed(() => {
  const groups: Record<string, SidebarTool[]> = {}

  for (const tool of filteredSidebarTools.value) {
    if (!groups[tool.category]) {
      groups[tool.category] = []
    }
    groups[tool.category].push(tool)
  }

  return groups
})

// Current AI Operation
const currentOperation = ref<{
  type: AIOperationType
  status: 'pending' | 'processing' | 'success' | 'error'
  progress: number
  message?: string
} | null>(null)

// Slash command suggestions
const showCommandSuggestions = ref(false)
const commandSuggestions = computed(() => {
  if (!inputMessage.value.startsWith('/')) return []
  return getCommandSuggestions(inputMessage.value)
})

// Content suggestions based on conversation
const contentSuggestions = ref<any[]>([])
const loadingContentSuggestions = ref(false)

// Voice input integration
watch(transcript, (newTranscript) => {
  if (newTranscript) {
    inputMessage.value = newTranscript
  }
})

// Watch for slash commands
watch(inputMessage, (newValue) => {
  // Show suggestions when typing / (even just /)
  showCommandSuggestions.value = newValue.startsWith('/') && !newValue.includes(' ')
  console.log('Input changed:', newValue, 'Show suggestions:', showCommandSuggestions.value, 'Suggestions count:', commandSuggestions.value.length)
})

// ============================================================================
// Advanced AI Features - State
// ============================================================================

const showContextPanel = ref(false)
const showContentBrowser = ref(false)
const showSummaryModal = ref(false)
const isAnalyzingContent = ref(false)
const isGeneratingSummary = ref(false)
const showAttachmentMenu = ref(false)

// Conversation Context - computed from messages
const conversationContext = computed<ConversationContext>(() => {
  const userMessages = messages.value.filter(m => m.role === 'user')
  const allContent = userMessages.map(m => m.content).join(' ').toLowerCase()

  // Extract topics from message content
  const topicKeywords = ['document', 'policy', 'report', 'data', 'analysis', 'summary', 'search', 'translate']
  const foundTopics = topicKeywords.filter(t => allContent.includes(t))
    .map(t => t.charAt(0).toUpperCase() + t.slice(1))

  // Extract potential entities (simple extraction)
  const entities: { text: string; type: string; count: number }[] = []

  // Determine intent based on last message
  const lastUserMessage = userMessages[userMessages.length - 1]
  let intent = 'General inquiry'
  if (lastUserMessage) {
    const content = lastUserMessage.content.toLowerCase()
    if (content.includes('search') || content.includes('find')) intent = 'Search'
    else if (content.includes('summarize') || content.includes('summary')) intent = 'Summarization'
    else if (content.includes('translate')) intent = 'Translation'
    else if (content.includes('analyze') || content.includes('analysis')) intent = 'Analysis'
    else if (content.includes('help') || content.includes('how')) intent = 'Help request'
  }

  // Generate summary points from recent messages
  const summaryPoints = userMessages.slice(-3).map(m =>
    m.content.length > 50 ? m.content.substring(0, 50) + '...' : m.content
  )

  // Get document references from attachments
  const documentReferences = messages.value
    .filter(m => m.attachments && m.attachments.length > 0)
    .flatMap(m => m.attachments?.map(a => a.title) || [])

  return {
    topics: foundTopics.length > 0 ? foundTopics : ['General'],
    entities,
    intent,
    summaryPoints,
    documentReferences
  }
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

// Dynamic follow-up suggestions based on conversation context
const followUpSuggestions = computed(() => {
  if (messages.value.length === 0) return []

  const lastMessage = messages.value[messages.value.length - 1]
  if (!lastMessage || lastMessage.role !== 'assistant') return []

  const suggestions: string[] = []
  const intent = conversationContext.value.intent

  // Add context-aware suggestions based on intent
  if (intent === 'Search') {
    suggestions.push('Show more results', 'Refine search')
  } else if (intent === 'Summarization') {
    suggestions.push('Make it shorter', 'Add more details')
  } else if (intent === 'Translation') {
    suggestions.push('Translate to another language', 'Explain translation')
  } else if (intent === 'Analysis') {
    suggestions.push('Explain further', 'Show data breakdown')
  } else {
    suggestions.push('Tell me more', 'Give an example')
  }

  return suggestions.slice(0, 3)
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

  // Check if this is a slash command
  if (userQuery.startsWith('/')) {
    const result = await executeSlashCommand(userQuery)
    if (result) {
      // Add user message
      messages.value.push({
        role: 'user',
        content: userQuery,
        timestamp: new Date()
      })
      // Add AI response only if result is a string (not true - which means message was already added)
      if (typeof result === 'string') {
        messages.value.push({
          role: 'assistant',
          content: result,
          timestamp: new Date()
        })
      }
    }
    inputMessage.value = ''
    pendingAttachments.value = []
    // Clear operation after a delay
    setTimeout(() => {
      currentOperation.value = null
    }, 2000)
    return
  }

  // Check if there's a pending operation from toolbar
  if (currentOperation.value && currentOperation.value.status === 'pending') {
    const operation = currentOperation.value.type
    currentOperation.value = {
      type: operation,
      status: 'processing',
      progress: 0,
      message: `Running ${operation}...`
    }

    // Simulate progress
    const progressInterval = setInterval(() => {
      if (currentOperation.value && currentOperation.value.progress < 90) {
        currentOperation.value.progress += 10
      }
    }, 200)

    // Simulate AI operation
    setTimeout(async () => {
      clearInterval(progressInterval)

      // Add user message
      messages.value.push({
        role: 'user',
        content: userQuery,
        attachments: attachments.length > 0 ? attachments : undefined,
        timestamp: new Date()
      })

      // Generate response based on operation and user query
      const generateOperationResponse = (op: AIOperationType, query: string): string => {
        const responses: Record<AIOperationType, string> = {
          'summarize': `<p><strong>Summary of "${query}":</strong></p><p>Here is a concise summary of the content you requested.</p>`,
          'translate': `<p><strong>Translation:</strong></p><p>Translation of "${query}" completed.</p>`,
          'extract-entities': `<p><strong>Entities found in "${query}":</strong></p><ul><li>Processing entities...</li></ul>`,
          'analyze-sentiment': `<p><strong>Sentiment Analysis for "${query}":</strong></p><p>Analysis complete.</p>`,
          'classify': `<p><strong>Classification of "${query}":</strong></p><p>Content categorized.</p>`,
          'ocr': `<p><strong>Text extracted:</strong></p><p>OCR processing complete for your content.</p>`,
          'generate-title': `<p><strong>Title suggestions for "${query}":</strong></p><ol><li>Option 1</li><li>Option 2</li><li>Option 3</li></ol>`,
          'auto-tag': `<p><strong>Tags for "${query}":</strong></p><p>Tags generated based on content.</p>`,
          'smart-search': `<p><strong>Search results for "${query}":</strong></p><ul><li>Searching...</li></ul>`,
          'chat': `<p>Processing your request: "${query}"</p>`
        }
        return responses[op] || `<p>Operation "${op}" completed for: ${query}</p>`
      }
      const operationResponse = generateOperationResponse(operation, userQuery)

      currentOperation.value = {
        type: operation,
        status: 'success',
        progress: 100
      }

      messages.value.push({
        role: 'assistant',
        content: operationResponse,
        timestamp: new Date()
      })

      inputMessage.value = ''
      pendingAttachments.value = []

      // Clear operation after delay
      setTimeout(() => {
        currentOperation.value = null
      }, 2000)
    }, 1500)

    return
  }

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
      responseContent = `<p>I understand you're asking about "${userQuery}".</p>
        <p>Here's what I can help you with:</p>
        <ul>
          <li>I can search for relevant documents and information</li>
          <li>I can summarize content for you</li>
          <li>I can help analyze or translate text</li>
        </ul>
        <p>Would you like me to perform any of these actions?</p>`
    }

    // Only add sources if we have actual references
    const sources = pendingAttachments.value.length > 0
      ? pendingAttachments.value.map(a => ({ title: a.title, icon: a.icon, url: '#' }))
      : undefined

    messages.value.push({
      role: 'assistant',
      content: responseContent,
      sources,
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

// ============================================================================
// Enhanced AI Features - Functions
// ============================================================================

// Handle AI tool selection from palette
function handleToolSelect(tool: { id: AIOperationType; name: string }) {
  showToolsPalette.value = false

  // Track operation
  preferencesStore.trackOperation(tool.id)

  // Set current operation
  currentOperation.value = {
    type: tool.id,
    status: 'pending',
    progress: 0
  }

  // Add placeholder based on tool type
  const toolPrefixes: Record<AIOperationType, string> = {
    'extract-entities': 'Extract entities from: ',
    'analyze-sentiment': 'Analyze sentiment of: ',
    'summarize': 'Summarize: ',
    'classify': 'Classify: ',
    'ocr': 'Extract text from this image: ',
    'translate': 'Translate to Arabic: ',
    'generate-title': 'Generate titles for: ',
    'auto-tag': 'Suggest tags for: ',
    'smart-search': 'Search for: ',
    'chat': ''
  }

  inputMessage.value = toolPrefixes[tool.id] || ''
}

// Handle AI tool selection from sidebar
function selectToolFromSidebar(tool: { id: AIOperationType; name: string }, option?: string) {
  showToolsPalette.value = false
  toolsSearchQuery.value = ''
  handleToolbarClick(tool.id, option)
}

// Handle AI tool selection from toolbar
function handleToolbarClick(toolId: AIOperationType, option?: string) {
  // Track operation
  preferencesStore.trackOperation(toolId)

  // Set current operation
  currentOperation.value = {
    type: toolId,
    status: 'pending',
    progress: 0
  }

  // Add placeholder based on tool type with option
  const toolPrefixes: Record<AIOperationType, string> = {
    'extract-entities': 'Extract entities from: ',
    'analyze-sentiment': 'Analyze sentiment of: ',
    'summarize': option ? `Summarize (${option}): ` : 'Summarize: ',
    'classify': 'Classify: ',
    'ocr': 'Extract text from this image: ',
    'translate': option ? `Translate to ${option}: ` : 'Translate: ',
    'generate-title': option ? `Generate ${option} titles for: ` : 'Generate titles for: ',
    'auto-tag': 'Suggest tags for: ',
    'smart-search': 'Search for: ',
    'chat': ''
  }

  inputMessage.value = toolPrefixes[toolId] || ''
  console.log('Toolbar click:', toolId, option)
}

// Handle slash command selection
function handleCommandSelect(command: { command: string; description: string }) {
  showCommandSuggestions.value = false

  // Parse the command to get the operation
  const parsed = parseCommand(command.command + ' ')
  if (parsed) {
    inputMessage.value = `/${parsed.command} `
  }
}

// Execute a slash command
async function executeSlashCommand(message: string) {
  const parsed = parseCommand(message)
  if (!parsed.isCommand || !parsed.command) return false

  const slashCommand = parsed.command
  const args = parsed.input
  const commandName = slashCommand.command.slice(1) // Remove leading /

  // Check if it's a recognized operation
  const operation = slashCommand.operation
  if (operation === 'help') {
    // Show help message
    messages.value.push({
      role: 'assistant',
      content: `<p><strong>Available Slash Commands:</strong></p>
        <ul>
          ${availableCommands.map(cmd => `<li><code>${cmd.command}</code> - ${cmd.description}</li>`).join('')}
        </ul>
        <p>Type a command followed by your content, e.g., <code>/summarize your text here</code></p>`,
      timestamp: new Date()
    })
    return true
  }

  if (operation === 'chat') {
    // Handle compare command
    if (commandName === 'compare') {
      if (comparisonStore.canCompare) {
        comparisonStore.startComparison()
      } else {
        openContentBrowser()
      }
      return true
    }
    return false
  }

  // It's an AI operation
  const aiOperation = operation as AIOperationType

  // Track the operation
  preferencesStore.trackOperation(aiOperation)

  // Set current operation
  currentOperation.value = {
    type: aiOperation,
    status: 'processing',
    progress: 0,
    message: `Running ${commandName}...`
  }

  // Simulate progress
  const progressInterval = setInterval(() => {
    if (currentOperation.value && currentOperation.value.progress < 90) {
      currentOperation.value.progress += 10
    }
  }, 200)

  try {
    // Simulate AI operation (in real implementation, call actual AI service)
    await new Promise(resolve => setTimeout(resolve, 1500))

    clearInterval(progressInterval)
    currentOperation.value = {
      type: aiOperation,
      status: 'success',
      progress: 100
    }

    // Generate mock response based on operation
    const operationResponses: Record<AIOperationType, string> = {
      summarize: `<p><strong>Summary:</strong></p><p>${args ? `This is a summary of: "${args}"` : 'Please provide content to summarize.'}</p>`,
      translate: `<p><strong>Translation:</strong></p><p>${args ? `Translated: "${args}"` : 'Please provide text to translate.'}</p>`,
      'extract-entities': `<p><strong>Entities Found:</strong></p><ul><li>Organization: AFC</li><li>Event: Asian Cup 2027</li><li>Location: Saudi Arabia</li></ul>`,
      'analyze-sentiment': `<p><strong>Sentiment Analysis:</strong></p><p>Overall: <span class="text-green-600 font-medium">Positive (85%)</span></p>`,
      classify: `<p><strong>Classification:</strong></p><p>Category: <span class="font-medium">Sports/Tournament</span> (92% confidence)</p>`,
      ocr: `<p><strong>Extracted Text:</strong></p><p>Text extracted from the document...</p>`,
      'generate-title': `<p><strong>Suggested Titles:</strong></p><ol><li>Comprehensive Guide to AFC Asian Cup 2027</li><li>Everything You Need to Know About the Tournament</li><li>Asian Cup 2027: A Complete Overview</li></ol>`,
      'auto-tag': `<p><strong>Suggested Tags:</strong></p><p><span class="px-2 py-1 bg-teal-100 text-teal-700 rounded mr-1">tournament</span><span class="px-2 py-1 bg-blue-100 text-blue-700 rounded mr-1">football</span><span class="px-2 py-1 bg-purple-100 text-purple-700 rounded">asia</span></p>`,
      'smart-search': `<p><strong>Search Results:</strong></p><ul><li>Tournament Handbook - Main reference document</li><li>Venue Guidelines - Stadium information</li><li>Media Accreditation Guide</li></ul>`,
      'chat': '<p>Chat response</p>'
    }

    return operationResponses[aiOperation] || '<p>Operation completed.</p>'
  } catch (error) {
    clearInterval(progressInterval)
    currentOperation.value = {
      type: aiOperation,
      status: 'error',
      progress: 0,
      message: 'Operation failed'
    }
    return null
  }
}

// Handle voice input toggle
function toggleVoiceInput() {
  if (isListening.value) {
    stopListening()
  } else {
    startListening()
  }
}

// Handle voice transcript
function handleVoiceTranscript(text: string, isFinal: boolean) {
  inputMessage.value = text
  console.log('Voice transcript:', text, 'Final:', isFinal)
}

// Cancel current operation
function cancelOperation() {
  currentOperation.value = null
}

// Run a workflow
async function runWorkflow(workflowId: string) {
  const input = inputMessage.value || 'Sample input for workflow'

  try {
    await workflowsStore.executeWorkflow(workflowId, input, (step, total, result) => {
      currentOperation.value = {
        type: result.operation,
        status: 'processing',
        progress: Math.round((step / total) * 100),
        message: `Step ${step}/${total}: ${result.output}`
      }
    })

    currentOperation.value = null

    // Add workflow result to messages
    messages.value.push({
      role: 'assistant',
      content: `<p><strong>Workflow completed!</strong></p><p>All ${workflowsStore.workflows.find(w => w.id === workflowId)?.steps.length || 0} steps executed successfully.</p>`,
      timestamp: new Date()
    })
  } catch (error) {
    currentOperation.value = {
      type: 'summarize', // fallback
      status: 'error',
      progress: 0,
      message: 'Workflow execution failed'
    }
  }
}

// Generate content suggestions based on conversation context
function generateContentSuggestions() {
  loadingContentSuggestions.value = true

  setTimeout(() => {
    contentSuggestions.value = [
      {
        id: '1',
        type: 'article',
        title: 'Tournament Operations Guide',
        reason: 'Related to your discussion about event management',
        relevanceScore: 0.92,
        suggestionType: 'related',
        metadata: { category: 'Operations', readTime: '8 min' }
      },
      {
        id: '2',
        type: 'document',
        title: 'AFC Asian Cup 2027 Handbook',
        reason: 'Referenced in conversation',
        relevanceScore: 0.88,
        suggestionType: 'referenced',
        metadata: { category: 'Official', readTime: '15 min' }
      },
      {
        id: '3',
        type: 'article',
        title: 'Venue Management Best Practices',
        reason: 'Similar to content you viewed',
        relevanceScore: 0.75,
        suggestionType: 'similar',
        metadata: { category: 'Venues', readTime: '5 min' }
      }
    ]
    loadingContentSuggestions.value = false
  }, 800)
}

// Handle content suggestion selection
function handleSuggestionSelect(suggestion: any) {
  attachContent({
    id: suggestion.id,
    type: suggestion.type,
    title: suggestion.title,
    excerpt: suggestion.reason,
    icon: suggestion.type === 'article' ? 'fas fa-file-alt' : 'fas fa-file-pdf',
    iconBg: suggestion.type === 'article' ? 'bg-blue-100' : 'bg-red-100'
  })
}

// Handle entity click (from AIMessageContent)
function handleEntityClick(entity: { text: string; type: string }) {
  inputMessage.value = `Tell me more about ${entity.text}`
  sendMessage()
}
</script>

<template>
  <div class="fixed right-0 bottom-0 top-[64px] flex overflow-hidden bg-gradient-to-br from-teal-50/30 via-white to-emerald-50/20 transition-all duration-300" :class="isSidebarCollapsed ? 'left-20' : 'left-64'">
    <!-- Conversation History Sidebar -->
    <aside
      :class="[
        'border-r border-gray-200 bg-gray-50 flex-shrink-0 flex flex-col h-full transition-all duration-300',
        isHistorySidebarCollapsed ? 'w-14' : 'w-72'
      ]"
    >
      <!-- Header -->
      <div class="p-3 flex items-center" :class="isHistorySidebarCollapsed ? 'justify-center' : 'justify-between'">
        <span v-if="!isHistorySidebarCollapsed" class="text-sm font-medium text-gray-700">{{ textConstants.chats }}</span>
        <button
          @click="isHistorySidebarCollapsed = !isHistorySidebarCollapsed"
          class="w-8 h-8 flex items-center justify-center rounded-lg hover:bg-gray-200 text-gray-500 transition-colors"
          :title="isHistorySidebarCollapsed ? textConstants.expand : textConstants.collapse"
        >
          <i :class="['fas text-xs', isHistorySidebarCollapsed ? 'fa-angles-right' : 'fa-angles-left']"></i>
        </button>
      </div>

      <!-- New Chat Button -->
      <div :class="isHistorySidebarCollapsed ? 'px-2 pb-3' : 'px-3 pb-3'">
        <button
          @click="startNewChat"
          :class="[
            'flex items-center justify-center gap-2 rounded-lg font-medium transition-all',
            isHistorySidebarCollapsed
              ? 'w-10 h-10 bg-teal-500 hover:bg-teal-600 text-white'
              : 'w-full py-2.5 bg-teal-500 hover:bg-teal-600 text-white'
          ]"
          :title="isHistorySidebarCollapsed ? textConstants.newChat : ''"
        >
          <i class="fas fa-plus text-sm"></i>
          <span v-if="!isHistorySidebarCollapsed" class="text-sm">{{ textConstants.newChat }}</span>
        </button>
      </div>

      <!-- Search -->
      <div v-if="!isHistorySidebarCollapsed" class="px-3 pb-3">
        <div class="relative">
          <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-xs"></i>
          <input
            type="text"
            v-model="searchHistory"
            :placeholder="textConstants.searchPlaceholder"
            class="w-full pl-9 pr-3 py-2 bg-white border border-gray-200 rounded-lg text-sm focus:outline-none focus:border-teal-400 focus:ring-1 focus:ring-teal-400"
          />
        </div>
      </div>

      <!-- Conversations List -->
      <div class="flex-1 overflow-y-auto">
        <!-- Today Section -->
        <div v-if="todayChats.length > 0">
          <div v-if="!isHistorySidebarCollapsed" class="px-3 py-2">
            <span class="text-xs font-medium text-gray-400 uppercase">{{ textConstants.today }}</span>
          </div>
          <div v-for="chat in todayChats" :key="chat.id"
               @click="selectChat(chat)"
               :class="[
                 'group cursor-pointer transition-colors',
                 isHistorySidebarCollapsed ? 'px-2 py-2' : 'px-3 py-2',
                 activeChat?.id === chat.id ? 'bg-teal-100' : 'hover:bg-gray-100'
               ]"
               :title="isHistorySidebarCollapsed ? chat.title : ''">
            <div class="flex items-center gap-3">
              <div :class="[
                'flex-shrink-0 flex items-center justify-center rounded-lg',
                isHistorySidebarCollapsed ? 'w-10 h-10' : 'w-8 h-8',
                activeChat?.id === chat.id ? 'bg-teal-500 text-white' : 'bg-gray-200 text-gray-500 group-hover:bg-gray-300'
              ]">
                <i class="fas fa-message text-xs"></i>
              </div>
              <div v-if="!isHistorySidebarCollapsed" class="flex-1 min-w-0">
                <p class="text-sm text-gray-800 truncate">{{ chat.title }}</p>
                <p class="text-xs text-gray-400 truncate">{{ chat.preview }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Yesterday Section -->
        <div v-if="yesterdayChats.length > 0">
          <div v-if="!isHistorySidebarCollapsed" class="px-3 py-2 mt-2">
            <span class="text-xs font-medium text-gray-400 uppercase">{{ textConstants.yesterday }}</span>
          </div>
          <div v-else class="mx-2 my-2 h-px bg-gray-200"></div>
          <div v-for="chat in yesterdayChats" :key="chat.id"
               @click="selectChat(chat)"
               :class="[
                 'group cursor-pointer transition-colors',
                 isHistorySidebarCollapsed ? 'px-2 py-2' : 'px-3 py-2',
                 activeChat?.id === chat.id ? 'bg-teal-100' : 'hover:bg-gray-100'
               ]"
               :title="isHistorySidebarCollapsed ? chat.title : ''">
            <div class="flex items-center gap-3">
              <div :class="[
                'flex-shrink-0 flex items-center justify-center rounded-lg',
                isHistorySidebarCollapsed ? 'w-10 h-10' : 'w-8 h-8',
                activeChat?.id === chat.id ? 'bg-teal-500 text-white' : 'bg-gray-200 text-gray-500 group-hover:bg-gray-300'
              ]">
                <i class="fas fa-message text-xs"></i>
              </div>
              <div v-if="!isHistorySidebarCollapsed" class="flex-1 min-w-0">
                <p class="text-sm text-gray-800 truncate">{{ chat.title }}</p>
                <p class="text-xs text-gray-400 truncate">{{ chat.preview }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Older Section -->
        <div v-if="olderChats.length > 0">
          <div v-if="!isHistorySidebarCollapsed" class="px-3 py-2 mt-2">
            <span class="text-xs font-medium text-gray-400 uppercase">{{ textConstants.previous7Days }}</span>
          </div>
          <div v-else class="mx-2 my-2 h-px bg-gray-200"></div>
          <div v-for="chat in olderChats" :key="chat.id"
               @click="selectChat(chat)"
               :class="[
                 'group cursor-pointer transition-colors',
                 isHistorySidebarCollapsed ? 'px-2 py-2' : 'px-3 py-2',
                 activeChat?.id === chat.id ? 'bg-teal-100' : 'hover:bg-gray-100'
               ]"
               :title="isHistorySidebarCollapsed ? chat.title : ''">
            <div class="flex items-center gap-3">
              <div :class="[
                'flex-shrink-0 flex items-center justify-center rounded-lg',
                isHistorySidebarCollapsed ? 'w-10 h-10' : 'w-8 h-8',
                activeChat?.id === chat.id ? 'bg-teal-500 text-white' : 'bg-gray-200 text-gray-500 group-hover:bg-gray-300'
              ]">
                <i class="fas fa-message text-xs"></i>
              </div>
              <div v-if="!isHistorySidebarCollapsed" class="flex-1 min-w-0">
                <p class="text-sm text-gray-800 truncate">{{ chat.title }}</p>
                <p class="text-xs text-gray-400 truncate">{{ chat.preview }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-if="todayChats.length === 0 && yesterdayChats.length === 0 && olderChats.length === 0"
             class="px-3 py-8 text-center">
          <div v-if="!isHistorySidebarCollapsed">
            <i class="fas fa-comments text-2xl text-gray-300 mb-2"></i>
            <p class="text-sm text-gray-400">{{ textConstants.noConversationsYet }}</p>
          </div>
        </div>
      </div>
    </aside>

    <!-- Main Chat Area -->
    <div class="flex-1 flex flex-col h-full overflow-hidden">
      <!-- Chat Header -->
      <div class="flex-shrink-0 p-4 border-b border-teal-100 bg-white/80 backdrop-blur-sm flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center ai-glow">
            <i class="fas fa-robot text-white text-lg icon-vibrant"></i>
          </div>
          <div>
            <h1 class="font-semibold text-teal-900">{{ textConstants.appTitle }}</h1>
            <p class="text-xs text-teal-500">{{ textConstants.appSubtitle }}</p>
          </div>
        </div>
        <div class="flex items-center gap-2">
          <!-- Tools Palette (Ctrl+K) -->
          <button @click="showToolsPalette = true"
                  class="px-3 py-1.5 rounded-lg bg-teal-50 hover:bg-teal-100 text-teal-700 text-sm font-medium flex items-center gap-2 transition-colors ripple"
                  title="AI Tools (Ctrl+K)">
            <i class="fas fa-wand-magic-sparkles"></i>
            <span class="hidden sm:inline">{{ textConstants.tools }}</span>
            <kbd class="hidden md:inline px-1.5 py-0.5 bg-white rounded text-xs text-gray-500 border border-gray-200">⌘K</kbd>
          </button>
          <!-- Workflows -->
          <button @click="showWorkflowBuilder = true"
                  class="px-3 py-1.5 rounded-lg bg-purple-50 hover:bg-purple-100 text-purple-700 text-sm font-medium flex items-center gap-2 transition-colors ripple"
                  title="AI Workflows">
            <i class="fas fa-stream"></i>
            <span class="hidden sm:inline">{{ textConstants.workflows }}</span>
          </button>
          <div class="w-px h-6 bg-teal-200"></div>
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
      <div class="flex-1 min-h-0 overflow-y-auto p-6 scrollbar-thin">
        <!-- Welcome State -->
        <div v-if="messages.length === 0" class="h-full flex flex-col items-center justify-center text-center">
          <h2 class="text-xl font-semibold text-gray-800 mb-1">{{ textConstants.welcomeTitle }}</h2>
          <p class="text-gray-500 text-sm mb-8 max-w-sm">{{ textConstants.welcomeSubtitle }}</p>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-2 max-w-xl w-full px-4">
            <button
              v-for="(prompt, idx) in suggestedPrompts"
              :key="idx"
              @click="usePrompt(prompt.text)"
              class="p-3 rounded-lg bg-white border border-gray-200 hover:border-teal-300 hover:bg-teal-50 cursor-pointer transition-all text-left group"
            >
              <div class="flex items-center gap-3">
                <i :class="[prompt.icon, 'text-gray-400 group-hover:text-teal-500']"></i>
                <span class="text-sm text-gray-700 group-hover:text-gray-900">{{ prompt.title }}</span>
              </div>
            </button>
          </div>
        </div>

        <!-- Messages -->
        <div v-else class="space-y-6 max-w-4xl mx-auto">
          <div v-for="(msg, idx) in messages" :key="idx" class="animate-fadeIn fade-in-up">
            <!-- User Message -->
            <div v-if="msg.role === 'user'" class="flex gap-4 justify-end mb-4">
              <div class="flex-1 flex justify-end">
                <div class="max-w-[90%]">
                  <!-- Attachments -->
                  <div v-if="msg.attachments && msg.attachments.length > 0" class="flex flex-wrap gap-2 justify-end mb-2">
                    <div v-for="attachment in msg.attachments" :key="attachment.id"
                         class="inline-flex items-center gap-2 px-3 py-1.5 bg-teal-100 rounded-lg text-sm text-teal-700">
                      <i :class="attachment.icon"></i>
                      <span class="font-medium">{{ attachment.title }}</span>
                    </div>
                  </div>
                  <div class="bg-white/80 backdrop-blur-sm p-4 rounded-2xl rounded-tr-md shadow-sm border border-teal-100 transition-all duration-300 hover:shadow-md">
                    <p class="whitespace-pre-wrap text-teal-900">{{ msg.content }}</p>
                  </div>
                </div>
              </div>
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center flex-shrink-0">
                <i class="fas fa-user text-white"></i>
              </div>
            </div>

            <!-- AI Message -->
            <div v-else class="flex gap-4">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center flex-shrink-0 ai-glow">
                <i class="fas fa-robot text-white icon-vibrant"></i>
              </div>
              <div class="flex-1">
                <div class="bg-white/80 backdrop-blur-sm p-4 rounded-2xl rounded-tl-md shadow-sm border border-teal-100 max-w-[90%] transition-all duration-300 hover:shadow-md">
                  <div class="prose prose-teal text-teal-900" v-html="sanitizeHtml(msg.content)"></div>

                  <!-- Sources/References -->
                  <div v-if="msg.sources && msg.sources.length" class="mt-4 pt-4 border-t border-teal-100">
                    <p class="text-xs font-semibold text-teal-500 uppercase mb-2">{{ textConstants.sources }}</p>
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
            <div class="bg-white/80 backdrop-blur-sm p-4 rounded-2xl rounded-tl-md shadow-sm border border-teal-100">
              <div class="flex items-center gap-2">
                <div class="flex items-center gap-1">
                  <div class="w-2 h-2 bg-teal-400 rounded-full animate-bounce" style="animation-delay: 0ms"></div>
                  <div class="w-2 h-2 bg-teal-400 rounded-full animate-bounce" style="animation-delay: 150ms"></div>
                  <div class="w-2 h-2 bg-teal-400 rounded-full animate-bounce" style="animation-delay: 300ms"></div>
                </div>
                <span v-if="isAnalyzingContent" class="text-xs text-teal-600 ml-2">
                  <i class="fas fa-chart-bar mr-1"></i>{{ textConstants.analyzingContent }}
                </span>
              </div>
            </div>
          </div>

          <!-- Follow-up Suggestions -->
          <div v-if="followUpSuggestions.length > 0 && !isTyping" class="flex flex-wrap gap-2 mt-4 fade-in-up">
            <span class="text-xs text-gray-500 mr-2">{{ textConstants.suggestedFollowUps }}</span>
            <button v-for="suggestion in followUpSuggestions" :key="suggestion"
                    @click="useFollowUp(suggestion)"
                    class="px-3 py-1.5 bg-gradient-to-r from-teal-50 to-emerald-50 border border-teal-200 rounded-full text-sm text-teal-700 hover:from-teal-100 hover:to-emerald-100 transition-all">
              {{ suggestion }}
            </button>
          </div>
        </div>
      </div>

      <!-- Input Area -->
      <div class="flex-shrink-0 p-4 border-t border-teal-100 bg-white/80 backdrop-blur-sm">
        <div class="max-w-4xl mx-auto">
          <!-- Current Operation Progress -->
          <Transition name="fade">
            <AIOperationProgress
              v-if="currentOperation"
              :operation="currentOperation.type"
              :status="currentOperation.status"
              :progress="currentOperation.progress"
              :message="currentOperation.message"
              @cancel="cancelOperation"
              class="mb-3"
            />
          </Transition>

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
              {{ textConstants.addMore }}
            </button>
          </div>

          <div class="chat-input-container flex items-end gap-3">
            <div class="flex-1 relative group">
              <!-- Slash Command Suggestions -->
              <Transition name="fade-up">
                <div v-if="showCommandSuggestions && commandSuggestions.length > 0"
                     class="absolute bottom-full left-0 right-0 mb-2 bg-white/95 backdrop-blur-lg rounded-2xl shadow-2xl border border-gray-200/50 overflow-hidden z-10">
                  <div class="p-3 border-b border-gray-100 bg-gradient-to-r from-teal-50/50 to-transparent">
                    <span class="text-xs font-semibold text-teal-600 uppercase tracking-wider">{{ textConstants.slashCommands }}</span>
                  </div>
                  <div class="max-h-56 overflow-y-auto p-1">
                    <button v-for="cmd in commandSuggestions" :key="cmd.command"
                            @click="handleCommandSelect(cmd)"
                            class="w-full px-3 py-2.5 text-left hover:bg-gradient-to-r hover:from-teal-50 hover:to-transparent rounded-xl flex items-center gap-3 transition-all duration-200 group/cmd">
                      <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 text-white flex items-center justify-center shadow-md group-hover/cmd:scale-110 transition-transform duration-200">
                        <i :class="cmd.icon || 'fas fa-terminal'"></i>
                      </div>
                      <div class="flex-1">
                        <p class="text-sm font-semibold text-gray-800">{{ cmd.command }}</p>
                        <p class="text-xs text-gray-500">{{ cmd.description }}</p>
                      </div>
                      <i class="fas fa-chevron-right text-gray-300 group-hover/cmd:text-teal-500 group-hover/cmd:translate-x-1 transition-all duration-200"></i>
                    </button>
                  </div>
                </div>
              </Transition>

              <!-- Main Input Container -->
              <div class="chat-input-wrapper relative bg-white rounded-2xl shadow-lg border border-gray-200/80
                          hover:shadow-xl hover:border-teal-300/50
                          focus-within:shadow-xl focus-within:border-teal-400 focus-within:ring-4 focus-within:ring-teal-500/10
                          transition-all duration-300 ease-out overflow-hidden">
                <!-- Gradient accent bar -->
                <div class="absolute top-0 left-0 right-0 h-0.5 bg-gradient-to-r from-teal-400 via-teal-500 to-emerald-500 opacity-0 group-focus-within:opacity-100 transition-opacity duration-300"></div>

                <textarea v-model="inputMessage"
                          @keydown.enter.exact.prevent="sendMessage"
                          @keydown.shift.enter="() => {}"
                          :placeholder="textConstants.inputPlaceholder"
                          rows="1"
                          class="chat-textarea w-full px-4 py-4 pr-32 resize-none min-h-[56px] max-h-40
                                 bg-transparent text-gray-800 placeholder-gray-400
                                 focus:outline-none text-[15px] leading-relaxed"
                          :style="{ height: inputHeight }"></textarea>

                <!-- Action buttons inside input -->
                <div class="absolute right-2 bottom-2 flex items-center gap-1 bg-white/80 backdrop-blur-sm rounded-xl p-1">
                  <button @click="openContentBrowser"
                          class="chat-action-btn p-2.5 rounded-xl hover:bg-teal-50 text-gray-400 hover:text-teal-500 transition-all duration-200 relative"
                          :title="textConstants.attachPlatformContent">
                    <i class="fas fa-paperclip"></i>
                    <span v-if="pendingAttachments.length > 0"
                          class="absolute -top-1 -right-1 w-5 h-5 bg-gradient-to-br from-teal-400 to-teal-600 text-white text-[10px] font-bold rounded-full flex items-center justify-center shadow-md animate-pulse">
                      {{ pendingAttachments.length }}
                    </span>
                  </button>
                  <!-- Voice Input -->
                  <AIVoiceInput
                    v-if="isVoiceSupported"
                    @transcript="handleVoiceTranscript"
                    class="inline-flex"
                  />
                  <button v-else
                          class="p-2.5 rounded-xl text-gray-300 cursor-not-allowed"
                          :title="textConstants.voiceInputNotSupported"
                          disabled>
                    <i class="fas fa-microphone"></i>
                  </button>
                </div>
              </div>
            </div>

            <!-- Send Button -->
            <button @click="sendMessage"
                    :disabled="!inputMessage.trim() || isTyping"
                    :class="[
                      'send-button relative h-14 w-14 rounded-2xl flex items-center justify-center transition-all duration-300 overflow-hidden',
                      (!inputMessage.trim() || isTyping)
                        ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                        : 'bg-gradient-to-br from-teal-400 to-teal-600 text-white shadow-lg hover:shadow-xl hover:scale-105 active:scale-95'
                    ]">
              <i :class="['fas fa-paper-plane text-lg transition-transform duration-300', inputMessage.trim() && !isTyping ? 'group-hover:translate-x-0.5 group-hover:-translate-y-0.5' : '']"></i>
              <!-- Ripple effect background -->
              <div v-if="inputMessage.trim() && !isTyping" class="absolute inset-0 bg-gradient-to-br from-white/20 to-transparent opacity-0 hover:opacity-100 transition-opacity duration-300"></div>
            </button>
          </div>

          <p class="text-xs text-teal-400 text-center mt-3">
            {{ textConstants.aiDisclaimer }}
          </p>
        </div>
      </div>
    </div>

    <!-- Settings Sidebar -->
    <Teleport to="body">
      <!-- Backdrop -->
      <Transition name="fade">
        <div
          v-if="showSettings"
          class="fixed inset-0 bg-black/30 z-40"
          @click="showSettings = false"
        ></div>
      </Transition>

      <!-- Sidebar Panel -->
      <Transition name="slide-right">
        <div v-if="showSettings" class="fixed right-0 top-0 h-full w-80 bg-gradient-to-b from-gray-50 to-white shadow-2xl z-50 flex flex-col border-l border-gray-200">
          <!-- Header -->
          <div class="p-4 bg-gradient-to-b from-white to-gray-50/80 border-b border-gray-100">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-9 h-9 rounded-xl bg-teal-50 flex items-center justify-center">
                  <i class="fas fa-cog text-teal-500"></i>
                </div>
                <div>
                  <h3 class="font-semibold text-gray-800">{{ textConstants.settingsTitle }}</h3>
                  <p class="text-xs text-gray-400">{{ textConstants.settingsSubtitle }}</p>
                </div>
              </div>
              <button @click="showSettings = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors text-gray-400 hover:text-gray-600" title="Close">
                <i class="fas fa-times"></i>
              </button>
            </div>
          </div>

          <!-- Settings Content -->
          <div class="flex-1 overflow-y-auto p-4 space-y-5">
            <!-- Response Style -->
            <div class="bg-white rounded-xl p-4 border border-gray-100">
              <div class="flex items-center gap-2 mb-3">
                <i class="fas fa-comment-alt text-teal-500"></i>
                <label class="text-sm font-medium text-gray-700">{{ textConstants.responseStyle }}</label>
              </div>
              <div class="space-y-2">
                <label class="flex items-center gap-3 p-2 rounded-lg hover:bg-gray-50 cursor-pointer transition-colors">
                  <input type="radio" v-model="settings.style" value="concise" class="w-4 h-4 text-teal-500">
                  <div>
                    <p class="text-sm font-medium text-gray-700">{{ textConstants.concise }}</p>
                    <p class="text-xs text-gray-400">{{ textConstants.conciseDesc }}</p>
                  </div>
                </label>
                <label class="flex items-center gap-3 p-2 rounded-lg hover:bg-gray-50 cursor-pointer transition-colors">
                  <input type="radio" v-model="settings.style" value="detailed" class="w-4 h-4 text-teal-500">
                  <div>
                    <p class="text-sm font-medium text-gray-700">{{ textConstants.detailed }}</p>
                    <p class="text-xs text-gray-400">{{ textConstants.detailedDesc }}</p>
                  </div>
                </label>
                <label class="flex items-center gap-3 p-2 rounded-lg hover:bg-gray-50 cursor-pointer transition-colors">
                  <input type="radio" v-model="settings.style" value="conversational" class="w-4 h-4 text-teal-500">
                  <div>
                    <p class="text-sm font-medium text-gray-700">{{ textConstants.conversational }}</p>
                    <p class="text-xs text-gray-400">{{ textConstants.conversationalDesc }}</p>
                  </div>
                </label>
              </div>
            </div>

            <!-- Language -->
            <div class="bg-white rounded-xl p-4 border border-gray-100">
              <div class="flex items-center gap-2 mb-3">
                <i class="fas fa-globe text-green-500"></i>
                <label class="text-sm font-medium text-gray-700">{{ textConstants.language }}</label>
              </div>
              <div class="grid grid-cols-2 gap-2">
                <label class="flex items-center gap-2 p-2 rounded-lg hover:bg-gray-50 cursor-pointer transition-colors border" :class="settings.language === 'en' ? 'border-teal-300 bg-teal-50' : 'border-transparent'">
                  <input type="radio" v-model="settings.language" value="en" class="hidden">
                  <span>🇺🇸</span>
                  <span class="text-sm text-gray-700">English</span>
                </label>
                <label class="flex items-center gap-2 p-2 rounded-lg hover:bg-gray-50 cursor-pointer transition-colors border" :class="settings.language === 'ar' ? 'border-teal-300 bg-teal-50' : 'border-transparent'">
                  <input type="radio" v-model="settings.language" value="ar" class="hidden">
                  <span>🇸🇦</span>
                  <span class="text-sm text-gray-700">Arabic</span>
                </label>
                <label class="flex items-center gap-2 p-2 rounded-lg hover:bg-gray-50 cursor-pointer transition-colors border" :class="settings.language === 'fr' ? 'border-teal-300 bg-teal-50' : 'border-transparent'">
                  <input type="radio" v-model="settings.language" value="fr" class="hidden">
                  <span>🇫🇷</span>
                  <span class="text-sm text-gray-700">French</span>
                </label>
                <label class="flex items-center gap-2 p-2 rounded-lg hover:bg-gray-50 cursor-pointer transition-colors border" :class="settings.language === 'es' ? 'border-teal-300 bg-teal-50' : 'border-transparent'">
                  <input type="radio" v-model="settings.language" value="es" class="hidden">
                  <span>🇪🇸</span>
                  <span class="text-sm text-gray-700">Spanish</span>
                </label>
              </div>
            </div>

            <!-- Toggles -->
            <div class="bg-white rounded-xl p-4 border border-gray-100 space-y-4">
              <div class="flex items-center gap-2 mb-1">
                <i class="fas fa-sliders-h text-purple-500"></i>
                <span class="text-sm font-medium text-gray-700">{{ textConstants.preferences }}</span>
              </div>

              <div class="flex items-center justify-between py-2">
                <div class="flex items-center gap-2">
                  <i class="fas fa-link text-gray-400 text-sm"></i>
                  <span class="text-sm text-gray-700">{{ textConstants.includeSourceReferences }}</span>
                </div>
                <label class="relative inline-flex items-center cursor-pointer">
                  <input type="checkbox" v-model="settings.showSources" class="sr-only peer">
                  <div class="w-9 h-5 bg-gray-200 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-4 after:w-4 after:transition-all peer-checked:bg-teal-500"></div>
                </label>
              </div>

              <div class="flex items-center justify-between py-2">
                <div class="flex items-center gap-2">
                  <i class="fas fa-history text-gray-400 text-sm"></i>
                  <span class="text-sm text-gray-700">{{ textConstants.saveConversationHistory }}</span>
                </div>
                <label class="relative inline-flex items-center cursor-pointer">
                  <input type="checkbox" v-model="settings.saveHistory" class="sr-only peer">
                  <div class="w-9 h-5 bg-gray-200 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-4 after:w-4 after:transition-all peer-checked:bg-teal-500"></div>
                </label>
              </div>
            </div>
          </div>

          <!-- Footer -->
          <div class="p-4 border-t border-gray-100 bg-white">
            <button
              @click="saveSettings"
              class="w-full py-2.5 bg-teal-500 hover:bg-teal-600 text-white font-medium rounded-xl transition-colors flex items-center justify-center gap-2"
            >
              <i class="fas fa-check"></i>
              {{ textConstants.saveSettings }}
            </button>
          </div>
        </div>
      </Transition>
    </Teleport>

    <!-- Context Panel (Sidebar) -->
    <Teleport to="body">
      <!-- Backdrop -->
      <Transition name="fade">
        <div
          v-if="showContextPanel"
          class="fixed inset-0 bg-black/30 z-40"
          @click="showContextPanel = false"
        ></div>
      </Transition>

      <!-- Sidebar Panel -->
      <Transition name="slide-right">
        <div v-if="showContextPanel" class="fixed right-0 top-0 h-full w-96 bg-gradient-to-b from-gray-50 to-white shadow-2xl z-50 flex flex-col border-l border-gray-200">
        <!-- Header -->
        <div class="p-5 bg-gradient-to-b from-white to-gray-50/80 border-b border-gray-100">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-teal-50 flex items-center justify-center">
                <i class="fas fa-brain text-teal-500 text-lg"></i>
              </div>
              <div>
                <h3 class="font-semibold text-gray-800">{{ textConstants.contextIntelligence }}</h3>
                <p class="text-xs text-gray-400">{{ textConstants.contextSubtitle }}</p>
              </div>
            </div>
            <button @click="showContextPanel = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors text-gray-400 hover:text-gray-600" title="Close panel">
              <i class="fas fa-times"></i>
            </button>
          </div>

          <!-- Quick Stats -->
          <div class="flex items-center gap-3 mt-4 pt-4 border-t border-gray-100">
            <div class="flex-1 text-center p-2 bg-teal-50/50 rounded-lg">
              <p class="text-xl font-semibold text-teal-600">{{ messages.length }}</p>
              <p class="text-xs text-gray-400">{{ textConstants.messages }}</p>
            </div>
            <div class="flex-1 text-center p-2 bg-teal-50/50 rounded-lg">
              <p class="text-xl font-semibold text-teal-600">{{ conversationContext.entities.length }}</p>
              <p class="text-xs text-gray-400">{{ textConstants.entities }}</p>
            </div>
            <div class="flex-1 text-center p-2 bg-teal-50/50 rounded-lg">
              <p class="text-xl font-semibold text-teal-600">{{ conversationContext.topics.length }}</p>
              <p class="text-xs text-gray-400">{{ textConstants.topics }}</p>
            </div>
          </div>
        </div>

        <div class="flex-1 overflow-y-auto">
          <!-- Intent Card -->
          <div class="p-4">
            <div class="bg-teal-50/50 rounded-xl p-4 border border-teal-100/50">
              <div class="flex items-center gap-2 mb-2">
                <i class="fas fa-crosshairs text-teal-400"></i>
                <span class="text-xs font-semibold uppercase tracking-wider text-gray-400">{{ textConstants.detectedIntent }}</span>
              </div>
              <p class="text-base font-semibold text-gray-700">{{ conversationContext.intent }}</p>
              <div class="flex items-center gap-2 mt-3 pt-3 border-t border-teal-100/30">
                <div class="flex-1 bg-teal-100/50 rounded-full h-1.5">
                  <div class="bg-teal-400 rounded-full h-1.5 w-4/5"></div>
                </div>
                <span class="text-xs text-gray-400">85%</span>
              </div>
            </div>
          </div>

          <!-- Conversation Sentiment -->
          <div class="px-4 pb-4">
            <div class="bg-gray-50/30 rounded-xl p-4 border border-gray-100/50">
              <div class="flex items-center justify-between mb-3">
                <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider flex items-center gap-2">
                  <i class="fas fa-heart text-pink-300"></i>
                  {{ textConstants.conversationMood }}
                </h4>
                <span class="px-2 py-0.5 bg-green-50 text-green-500 rounded-full text-xs font-medium">{{ textConstants.positive }}</span>
              </div>
              <div class="flex items-center gap-3">
                <div class="flex-1">
                  <div class="flex items-center justify-between text-xs text-gray-400 mb-1">
                    <span>{{ textConstants.negative }}</span>
                    <span>{{ textConstants.positive }}</span>
                  </div>
                  <div class="h-2 bg-gradient-to-r from-red-100 via-yellow-100 to-green-100 rounded-full relative">
                    <div class="absolute top-1/2 -translate-y-1/2 w-3 h-3 bg-white border-2 border-green-400 rounded-full shadow-sm" style="left: 75%"></div>
                  </div>
                </div>
              </div>
              <div class="flex items-center justify-center gap-6 mt-3 pt-3 border-t border-gray-100/50">
                <div class="text-center">
                  <i class="fas fa-smile text-green-300"></i>
                  <p class="text-xs text-gray-400 mt-1">{{ textConstants.helpful }}</p>
                </div>
                <div class="text-center">
                  <i class="fas fa-lightbulb text-amber-300"></i>
                  <p class="text-xs text-gray-400 mt-1">{{ textConstants.informative }}</p>
                </div>
                <div class="text-center">
                  <i class="fas fa-handshake text-blue-300"></i>
                  <p class="text-xs text-gray-400 mt-1">{{ textConstants.collaborative }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Active Topics -->
          <div class="px-4 pb-4">
            <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3 flex items-center gap-2">
              <i class="fas fa-hashtag text-teal-400"></i>
              {{ textConstants.activeTopics }}
            </h4>
            <div class="flex flex-wrap gap-2">
              <button v-for="topic in conversationContext.topics" :key="topic"
                      class="group px-3 py-1.5 bg-teal-50/50 hover:bg-teal-100/50 border border-teal-100 text-teal-600 rounded-lg text-sm font-medium transition-all hover:shadow-sm">
                <i class="fas fa-tag text-teal-300 mr-2 group-hover:text-teal-500"></i>
                {{ topic }}
              </button>
            </div>
          </div>

          <!-- Recognized Entities -->
          <div class="px-4 pb-4">
            <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3 flex items-center gap-2">
              <i class="fas fa-cube text-teal-500"></i>
              {{ textConstants.recognizedEntities }}
            </h4>
            <div class="space-y-2">
              <button v-for="entity in conversationContext.entities" :key="entity.text"
                      class="w-full flex items-center justify-between p-3 bg-gray-50/50 hover:bg-teal-50/50 rounded-xl border border-gray-100/50 hover:border-teal-200/50 transition-all group">
                <div class="flex items-center gap-3">
                  <div :class="[
                    'w-10 h-10 rounded-xl flex items-center justify-center',
                    entity.type === 'organization' ? 'bg-blue-50 text-blue-500' :
                    entity.type === 'location' ? 'bg-green-50 text-green-500' :
                    entity.type === 'person' ? 'bg-orange-50 text-orange-500' :
                    entity.type === 'event' ? 'bg-pink-50 text-pink-500' :
                    'bg-gray-50 text-gray-500'
                  ]">
                    <i :class="[
                      entity.type === 'organization' ? 'fas fa-building' :
                      entity.type === 'location' ? 'fas fa-map-marker-alt' :
                      entity.type === 'person' ? 'fas fa-user' :
                      entity.type === 'event' ? 'fas fa-calendar-star' :
                      'fas fa-tag'
                    ]"></i>
                  </div>
                  <div class="text-left">
                    <p class="text-sm font-medium text-gray-800">{{ entity.text }}</p>
                    <p class="text-xs text-gray-400 capitalize">{{ entity.type }}</p>
                  </div>
                </div>
                <div class="flex items-center gap-2">
                  <span class="px-2 py-1 bg-gray-100 group-hover:bg-teal-100 rounded-lg text-xs font-medium text-gray-600 group-hover:text-teal-700">
                    {{ entity.count }}x
                  </span>
                  <i class="fas fa-search text-gray-300 group-hover:text-teal-500"></i>
                </div>
              </button>
            </div>
          </div>

          <!-- Key Insights -->
          <div class="px-4 pb-4">
            <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3 flex items-center gap-2">
              <i class="fas fa-lightbulb text-amber-400"></i>
              {{ textConstants.keyInsights }}
            </h4>
            <div class="bg-amber-50/30 rounded-2xl p-4 border border-amber-100/50">
              <ul class="space-y-3">
                <li v-for="(point, idx) in conversationContext.summaryPoints" :key="idx"
                    class="flex items-start gap-3">
                  <div class="w-6 h-6 rounded-full bg-amber-100/70 text-amber-600 flex items-center justify-center flex-shrink-0 text-xs font-bold">
                    {{ idx + 1 }}
                  </div>
                  <span class="text-sm text-gray-600">{{ point }}</span>
                </li>
              </ul>
            </div>
          </div>

          <!-- Referenced Documents -->
          <div v-if="conversationContext.documentReferences.length > 0" class="px-4 pb-4">
            <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3 flex items-center gap-2">
              <i class="fas fa-folder-open text-amber-400"></i>
              {{ textConstants.referencedDocuments }}
            </h4>
            <div class="space-y-2">
              <button v-for="doc in conversationContext.documentReferences" :key="doc"
                      class="w-full flex items-center gap-3 p-3 bg-gray-50/50 hover:bg-amber-50/50 rounded-xl border border-gray-100/50 hover:border-amber-200/50 transition-all group">
                <div class="w-10 h-10 rounded-xl bg-amber-50 text-amber-500 flex items-center justify-center">
                  <i class="fas fa-file-alt"></i>
                </div>
                <div class="flex-1 text-left">
                  <p class="text-sm font-medium text-gray-700">{{ doc }}</p>
                  <p class="text-xs text-gray-400">Click to view document</p>
                </div>
                <i class="fas fa-external-link-alt text-gray-300 group-hover:text-amber-400"></i>
              </button>
            </div>
          </div>

          <!-- AI Suggestions -->
          <div class="px-4 pb-6">
            <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3 flex items-center gap-2">
              <i class="fas fa-magic text-teal-400"></i>
              AI Suggestions
            </h4>
            <div class="bg-teal-50/30 rounded-2xl p-4 border border-teal-100/50">
              <p class="text-sm text-gray-500 mb-3">Based on this conversation, you might want to:</p>
              <div class="space-y-2">
                <button class="w-full flex items-center gap-3 p-2.5 bg-white/70 hover:bg-teal-50 rounded-xl text-left transition-colors group border border-gray-100/50">
                  <i class="fas fa-file-export text-teal-400"></i>
                  <span class="text-sm text-gray-600 group-hover:text-teal-700">Export conversation summary</span>
                </button>
                <button class="w-full flex items-center gap-3 p-2.5 bg-white/70 hover:bg-teal-50 rounded-xl text-left transition-colors group border border-gray-100/50">
                  <i class="fas fa-tasks text-teal-400"></i>
                  <span class="text-sm text-gray-600 group-hover:text-teal-700">Create action items</span>
                </button>
                <button class="w-full flex items-center gap-3 p-2.5 bg-white/70 hover:bg-teal-50 rounded-xl text-left transition-colors group border border-gray-100/50">
                  <i class="fas fa-search-plus text-teal-400"></i>
                  <span class="text-sm text-gray-600 group-hover:text-teal-700">Deep dive into topics</span>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div class="p-4 border-t border-gray-100 bg-white">
          <button class="w-full py-3 bg-teal-50 hover:bg-teal-100 text-teal-700 border border-teal-200 rounded-xl font-medium transition-all flex items-center justify-center gap-2 hover:shadow-md">
            <i class="fas fa-download"></i>
            Export Context Report
          </button>
        </div>
      </div>
    </Transition>
    </Teleport>

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

    <!-- AI Tools Sidebar -->
    <Teleport to="body">
      <!-- Backdrop -->
      <Transition name="fade">
        <div
          v-if="showToolsPalette"
          class="fixed inset-0 bg-black/30 z-40"
          @click="showToolsPalette = false"
        ></div>
      </Transition>

      <!-- Sidebar Panel -->
      <Transition name="slide-right">
        <div v-if="showToolsPalette" class="fixed right-0 top-0 h-full w-80 bg-gradient-to-b from-gray-50 to-white shadow-2xl z-50 flex flex-col border-l border-gray-200">
          <!-- Header -->
          <div class="p-4 bg-gradient-to-b from-white to-gray-50/80 border-b border-gray-100">
            <div class="flex items-center justify-between mb-3">
              <div class="flex items-center gap-3">
                <div class="w-9 h-9 rounded-xl bg-teal-50 flex items-center justify-center">
                  <i class="fas fa-wand-magic-sparkles text-teal-500"></i>
                </div>
                <div>
                  <h3 class="font-semibold text-gray-800">AI Tools</h3>
                  <p class="text-xs text-gray-400">Select a tool to use</p>
                </div>
              </div>
              <button @click="showToolsPalette = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors text-gray-400 hover:text-gray-600" title="Close">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <!-- Search -->
            <div class="relative">
              <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-gray-300"></i>
              <input
                v-model="toolsSearchQuery"
                type="text"
                placeholder="Search tools..."
                class="w-full pl-9 pr-3 py-2 bg-gray-50 border border-gray-100 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-teal-500/20 focus:border-teal-300"
              />
            </div>
          </div>

          <!-- Tools List -->
          <div class="flex-1 overflow-y-auto p-3">
            <!-- Dynamic Categories -->
            <template v-for="(categoryKey) in Object.keys(sidebarCategories)" :key="categoryKey">
              <div v-if="groupedSidebarTools[categoryKey]?.length" class="mb-4">
                <h4 class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-2 px-2 flex items-center gap-2">
                  <i :class="[sidebarCategories[categoryKey as keyof typeof sidebarCategories].icon, sidebarCategories[categoryKey as keyof typeof sidebarCategories].iconColor]"></i>
                  {{ sidebarCategories[categoryKey as keyof typeof sidebarCategories].label }}
                </h4>
                <div class="space-y-1">
                  <template v-for="tool in groupedSidebarTools[categoryKey]" :key="tool.id">
                    <!-- Tool with options -->
                    <div v-if="tool.options" class="rounded-xl overflow-hidden">
                      <div :class="['flex items-center gap-3 p-2.5', tool.bgColor + '/30']">
                        <div :class="['w-9 h-9 rounded-lg flex items-center justify-center', tool.bgColor, tool.color]">
                          <i :class="tool.icon"></i>
                        </div>
                        <div class="flex-1 text-left">
                          <p class="text-sm font-medium text-gray-700">{{ tool.name }}</p>
                          <p class="text-xs text-gray-400">{{ tool.description }}</p>
                        </div>
                      </div>
                      <div :class="[
                        'gap-1 px-2 pb-2 pt-1',
                        tool.bgColor + '/30',
                        tool.id === 'translate' ? 'grid grid-cols-3' : tool.id === 'generate-title' ? 'grid grid-cols-2' : 'flex'
                      ]">
                        <button
                          v-for="option in tool.options"
                          :key="option.value"
                          @click="selectToolFromSidebar({ id: tool.id, name: tool.name }, option.value)"
                          :class="[
                            'px-2 py-1.5 text-xs font-medium bg-white text-gray-600 rounded-lg transition-colors flex items-center justify-center gap-1',
                            tool.hoverBg,
                            tool.id !== 'translate' && tool.id !== 'generate-title' ? 'flex-1' : ''
                          ]"
                        >
                          <span v-if="!option.icon.startsWith('fas')">{{ option.icon }}</span>
                          <i v-else :class="[option.icon, 'text-[10px]']"></i>
                          {{ option.label }}
                        </button>
                      </div>
                    </div>

                    <!-- Tool without options -->
                    <button
                      v-else
                      @click="selectToolFromSidebar({ id: tool.id, name: tool.name })"
                      class="w-full flex items-center gap-3 p-2.5 rounded-xl hover:bg-teal-50/50 transition-colors group"
                    >
                      <div :class="['w-9 h-9 rounded-lg flex items-center justify-center', tool.bgColor, tool.color, tool.hoverBg]">
                        <i :class="tool.icon"></i>
                      </div>
                      <div class="flex-1 text-left">
                        <p class="text-sm font-medium text-gray-700">{{ tool.name }}</p>
                        <p class="text-xs text-gray-400">{{ tool.description }}</p>
                      </div>
                    </button>
                  </template>
                </div>
              </div>
            </template>

            <!-- No Results -->
            <div v-if="filteredSidebarTools.length === 0" class="py-8 text-center">
              <div class="w-12 h-12 rounded-full bg-gray-100 flex items-center justify-center mx-auto mb-3">
                <i class="fas fa-search text-gray-400"></i>
              </div>
              <p class="text-sm text-gray-500">No tools found</p>
              <p class="text-xs text-gray-400 mt-1">Try a different search term</p>
            </div>
          </div>

          <!-- Footer -->
          <div class="p-3 border-t border-gray-100 bg-white">
            <p class="text-xs text-gray-400 text-center">
              Press <kbd class="px-1.5 py-0.5 bg-gray-100 rounded text-gray-500">Ctrl+K</kbd> to toggle
            </p>
          </div>
        </div>
      </Transition>
    </Teleport>

    <!-- AI Workflow Builder Modal -->
    <AIWorkflowBuilder
      v-model="showWorkflowBuilder"
    />

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

/* Fade transition */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* Fade up transition */
.fade-up-enter-active,
.fade-up-leave-active {
  transition: opacity 0.2s ease, transform 0.2s ease;
}

.fade-up-enter-from,
.fade-up-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

/* Slide Left Animation (for left sidebar) */
.slide-left-enter-active,
.slide-left-leave-active {
  transition: transform 0.3s ease, opacity 0.3s ease;
}

.slide-left-enter-from,
.slide-left-leave-to {
  transform: translateX(-100%);
  opacity: 0;
}

/* Search Container Styles */
.search-container {
  position: relative;
}

.search-container input {
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.search-container input:focus {
  transform: scale(1.01);
}

.search-container input::placeholder {
  transition: opacity 0.2s ease, transform 0.2s ease;
}

.search-container input:focus::placeholder {
  opacity: 0.5;
  transform: translateX(4px);
}

/* Search icon animation */
.search-container .fa-search {
  transition: transform 0.2s ease, color 0.2s ease;
}

.search-container:focus-within .fa-search {
  transform: scale(1.1);
}

/* Clear button animation */
.search-container button {
  transition: transform 0.15s ease, opacity 0.15s ease;
  opacity: 0.7;
}

.search-container button:hover {
  transform: scale(1.1);
  opacity: 1;
}

/* Keyboard shortcut hint */
.search-container kbd {
  transition: opacity 0.2s ease;
}

.search-container:focus-within kbd {
  opacity: 0;
}

/* Chat Input Container Styles */
.chat-input-container {
  position: relative;
}

.chat-input-wrapper {
  background: linear-gradient(to bottom, rgba(255,255,255,0.95), rgba(255,255,255,1));
}

.chat-textarea {
  font-family: inherit;
  line-height: 1.6;
}

.chat-textarea::placeholder {
  transition: all 0.3s ease;
}

.chat-textarea:focus::placeholder {
  opacity: 0.5;
  transform: translateX(8px);
}

/* Chat action buttons */
.chat-action-btn {
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.chat-action-btn:hover {
  transform: scale(1.1);
}

.chat-action-btn:active {
  transform: scale(0.95);
}

/* Send button animations */
.send-button {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.send-button:not(:disabled):hover {
  box-shadow: 0 10px 40px -10px rgba(20, 184, 166, 0.5);
}

.send-button:not(:disabled):active {
  transform: scale(0.92);
}

.send-button i {
  transition: transform 0.3s ease;
}

.send-button:not(:disabled):hover i {
  transform: translate(2px, -2px);
}

/* Slash command dropdown animation */
.chat-input-container .fade-up-enter-active {
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
}

.chat-input-container .fade-up-leave-active {
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.chat-input-container .fade-up-enter-from {
  opacity: 0;
  transform: translateY(10px) scale(0.98);
}

.chat-input-container .fade-up-leave-to {
  opacity: 0;
  transform: translateY(5px) scale(0.98);
}
</style>
