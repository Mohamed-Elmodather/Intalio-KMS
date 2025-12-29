<script setup lang="ts">
import { ref, computed } from 'vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

// Types
interface Chat {
  id: number
  title: string
  preview: string
}

interface Message {
  role: 'user' | 'assistant'
  content: string
  sources?: { title: string; icon: string; url: string }[]
  rating?: 'up' | 'down' | null
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
  { id: 'write', label: 'Help write', icon: 'fas fa-pen' }
])

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

function sendMessage() {
  if (!inputMessage.value.trim() || isTyping.value) return

  messages.value.push({ role: 'user', content: inputMessage.value })
  const userQuery = inputMessage.value
  inputMessage.value = ''
  isTyping.value = true

  // Simulate AI response
  setTimeout(() => {
    messages.value.push({
      role: 'assistant',
      content: `<p>Thank you for your question about "${userQuery}".</p><p>Based on my analysis of the knowledge base, here's what I found:</p><ul><li>The relevant policy documentation indicates...</li><li>According to recent updates...</li><li>Best practices suggest...</li></ul><p>Is there anything specific you'd like me to clarify?</p>`,
      sources: [
        { title: 'Knowledge Base Article', icon: 'fas fa-file-alt', url: '#' },
        { title: 'Policy Document', icon: 'fas fa-book', url: '#' }
      ]
    })
    isTyping.value = false
  }, 1500)
}

function usePrompt(text: string) {
  inputMessage.value = text
  sendMessage()
}

function triggerAction(action: QuickAction) {
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
            <p class="text-xs text-teal-500">Powered by advanced AI</p>
          </div>
        </div>
        <div class="flex items-center gap-2">
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
              <div class="max-w-[80%] bg-gradient-to-r from-teal-500 to-teal-600 text-white p-4 rounded-2xl rounded-br-md shadow-lg card-animated">
                <p>{{ msg.content }}</p>
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
              <div class="flex items-center gap-1">
                <div class="w-2 h-2 bg-teal-400 rounded-full animate-bounce" style="animation-delay: 0ms"></div>
                <div class="w-2 h-2 bg-teal-400 rounded-full animate-bounce" style="animation-delay: 150ms"></div>
                <div class="w-2 h-2 bg-teal-400 rounded-full animate-bounce" style="animation-delay: 300ms"></div>
              </div>
            </div>
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

          <div class="flex items-end gap-3">
            <div class="flex-1 relative">
              <textarea v-model="inputMessage"
                        @keydown.enter.exact.prevent="sendMessage"
                        @keydown.shift.enter="() => {}"
                        placeholder="Ask me anything..."
                        rows="1"
                        class="input resize-none pr-24 min-h-[48px] max-h-32"
                        :style="{ height: inputHeight }"></textarea>
              <div class="absolute right-2 bottom-2 flex items-center gap-1">
                <button class="p-2 rounded-lg hover:bg-teal-100 text-teal-500 ripple" title="Attach file">
                  <i class="fas fa-paperclip icon-soft"></i>
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
            AI can make mistakes. Consider checking important information.
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
</style>
