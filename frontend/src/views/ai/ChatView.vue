<template>
  <div class="chat-view">
    <!-- Sidebar -->
    <aside class="chat-sidebar" :class="{ collapsed: sidebarCollapsed }">
      <div class="sidebar-header">
        <Button
          icon="pi pi-plus"
          :label="!sidebarCollapsed ? $t('ai.newChat') : undefined"
          class="new-chat-btn"
          @click="startNewChat"
        />
        <Button
          :icon="sidebarCollapsed ? 'pi pi-angle-right' : 'pi pi-angle-left'"
          text
          rounded
          @click="sidebarCollapsed = !sidebarCollapsed"
        />
      </div>

      <div v-if="!sidebarCollapsed" class="conversations-list">
        <div class="list-header">
          <span>{{ $t('ai.recentChats') }}</span>
        </div>
        <div
          v-for="conv in conversations"
          :key="conv.id"
          class="conversation-item"
          :class="{ active: currentConversationId === conv.id }"
          @click="loadConversation(conv.id)"
        >
          <i class="pi pi-comments"></i>
          <span class="conv-title">{{ conv.title }}</span>
          <Button
            icon="pi pi-trash"
            text
            rounded
            size="small"
            severity="danger"
            @click.stop="confirmDeleteConversation(conv)"
            v-tooltip="$t('common.delete')"
          />
        </div>

        <div v-if="conversations.length === 0" class="empty-list">
          {{ $t('ai.noConversations') }}
        </div>
      </div>
    </aside>

    <!-- Main Chat Area -->
    <main class="chat-main">
      <!-- Chat Header -->
      <header class="chat-header">
        <div class="header-left">
          <h2 v-if="currentConversationId">{{ currentTitle }}</h2>
          <h2 v-else>{{ $t('ai.assistant') }}</h2>
        </div>
        <div class="header-right">
          <Tag
            v-if="aiStatus"
            :value="aiStatus.isMock ? 'Mock AI' : aiStatus.provider"
            :severity="aiStatus.isAvailable ? 'success' : 'danger'"
          />
          <Button
            icon="pi pi-cog"
            text
            rounded
            @click="settingsDialog = true"
            v-tooltip="$t('common.settings')"
          />
        </div>
      </header>

      <!-- Messages -->
      <div ref="messagesContainer" class="messages-container">
        <div v-if="messages.length === 0" class="welcome-screen">
          <div class="welcome-icon">
            <i class="pi pi-sparkles"></i>
          </div>
          <h3>{{ $t('ai.welcomeTitle') }}</h3>
          <p>{{ $t('ai.welcomeMessage') }}</p>

          <div class="quick-actions">
            <Button
              v-for="action in quickActions"
              :key="action.key"
              :label="action.label"
              severity="secondary"
              outlined
              @click="sendMessage(action.prompt)"
            />
          </div>
        </div>

        <div
          v-for="(msg, idx) in messages"
          :key="msg.id || idx"
          class="message"
          :class="msg.role"
        >
          <div class="message-avatar">
            <Avatar
              v-if="msg.role === 'user'"
              :label="userInitials"
              shape="circle"
            />
            <div v-else class="ai-avatar">
              <i class="pi pi-sparkles"></i>
            </div>
          </div>

          <div class="message-content">
            <div class="message-header">
              <span class="sender">{{ msg.role === 'user' ? $t('ai.you') : $t('ai.assistant') }}</span>
              <span class="timestamp">{{ formatTime(msg.createdAt) }}</span>
            </div>

            <div class="message-text" v-html="formatMessage(msg.content)"></div>

            <!-- Citations -->
            <div v-if="msg.citations && msg.citations.length > 0" class="citations">
              <div class="citations-header">
                <i class="pi pi-file"></i>
                {{ $t('ai.sources') }}
              </div>
              <div class="citation-list">
                <div
                  v-for="citation in msg.citations"
                  :key="citation.index"
                  class="citation-item"
                  @click="viewSource(citation)"
                >
                  <span class="citation-index">[{{ citation.index }}]</span>
                  <span class="citation-name">{{ citation.documentName }}</span>
                  <span v-if="citation.pageNumber" class="citation-page">
                    p.{{ citation.pageNumber }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Streaming response -->
        <div v-if="isStreaming" class="message assistant streaming">
          <div class="message-avatar">
            <div class="ai-avatar">
              <i class="pi pi-sparkles"></i>
            </div>
          </div>
          <div class="message-content">
            <div class="message-header">
              <span class="sender">{{ $t('ai.assistant') }}</span>
            </div>
            <div class="message-text" v-html="formatMessage(streamingContent)"></div>
            <div class="typing-indicator">
              <span></span><span></span><span></span>
            </div>
          </div>
        </div>
      </div>

      <!-- Input Area -->
      <div class="input-area">
        <div class="input-options">
          <div class="option">
            <Checkbox
              v-model="useRAG"
              inputId="useRAG"
              binary
            />
            <label for="useRAG">{{ $t('ai.useDocuments') }}</label>
          </div>
        </div>

        <div class="input-wrapper">
          <Textarea
            v-model="inputMessage"
            :placeholder="$t('ai.placeholder')"
            rows="1"
            autoResize
            @keydown.enter.exact="handleEnter"
            :disabled="isStreaming"
          />
          <Button
            :icon="isStreaming ? 'pi pi-stop' : 'pi pi-send'"
            :disabled="!inputMessage.trim() && !isStreaming"
            @click="isStreaming ? stopStreaming() : sendMessage()"
            :class="{ streaming: isStreaming }"
          />
        </div>
      </div>
    </main>

    <!-- Settings Dialog -->
    <Dialog
      v-model:visible="settingsDialog"
      :header="$t('ai.settings')"
      :style="{ width: '400px' }"
      modal
    >
      <div class="settings-content">
        <div class="setting-item">
          <label>{{ $t('ai.provider') }}</label>
          <span>{{ aiStatus?.provider || 'Unknown' }}</span>
        </div>
        <div class="setting-item">
          <label>{{ $t('ai.model') }}</label>
          <span>{{ aiStatus?.model || 'Unknown' }}</span>
        </div>
        <div class="setting-item">
          <label>{{ $t('ai.indexedDocuments') }}</label>
          <span>{{ aiStatus?.indexedDocuments || 0 }}</span>
        </div>
        <div v-if="aiStatus?.lastIndexUpdate" class="setting-item">
          <label>{{ $t('ai.lastIndexUpdate') }}</label>
          <span>{{ formatDate(aiStatus.lastIndexUpdate) }}</span>
        </div>
      </div>

      <template #footer>
        <Button
          :label="$t('common.close')"
          @click="settingsDialog = false"
        />
      </template>
    </Dialog>

    <!-- Delete Confirmation -->
    <ConfirmDialog />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, nextTick, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import { useAuthStore } from '@/stores/auth'
import { aiApi, type AIStatus, type Conversation, type Message, type Citation, type ChatStreamEvent } from '@/api/ai'
import { marked } from 'marked'
import DOMPurify from 'dompurify'

const { t } = useI18n()
const toast = useToast()
const confirm = useConfirm()
const authStore = useAuthStore()

// State
const sidebarCollapsed = ref(false)
const conversations = ref<Conversation[]>([])
const messages = ref<Message[]>([])
const currentConversationId = ref<string | null>(null)
const currentTitle = ref('')
const inputMessage = ref('')
const isStreaming = ref(false)
const streamingContent = ref('')
const useRAG = ref(true)
const settingsDialog = ref(false)
const aiStatus = ref<AIStatus | null>(null)
const messagesContainer = ref<HTMLElement | null>(null)
let streamController: AbortController | null = null

// Computed
const userInitials = computed(() => {
  const name = authStore.user?.displayName || 'User'
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
})

const quickActions = computed(() => [
  { key: 'help', label: t('ai.quickHelp'), prompt: t('ai.quickHelpPrompt') },
  { key: 'search', label: t('ai.quickSearch'), prompt: t('ai.quickSearchPrompt') },
  { key: 'summary', label: t('ai.quickSummary'), prompt: t('ai.quickSummaryPrompt') }
])

// Methods
const loadConversations = async () => {
  try {
    conversations.value = await aiApi.getConversations()
  } catch (error) {
    console.error('Failed to load conversations:', error)
  }
}

const loadConversation = async (id: string) => {
  try {
    currentConversationId.value = id
    const conv = conversations.value.find(c => c.id === id)
    currentTitle.value = conv?.title || 'Chat'
    messages.value = await aiApi.getMessages(id)
    scrollToBottom()
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: t('common.error'),
      detail: t('ai.loadError'),
      life: 5000
    })
  }
}

const startNewChat = () => {
  currentConversationId.value = null
  currentTitle.value = ''
  messages.value = []
  inputMessage.value = ''
}

const sendMessage = async (text?: string) => {
  const message = text || inputMessage.value.trim()
  if (!message) return

  inputMessage.value = ''

  // Add user message to UI
  messages.value.push({
    id: `temp-${Date.now()}`,
    conversationId: currentConversationId.value || '',
    role: 'user',
    content: message,
    createdAt: new Date().toISOString()
  })

  scrollToBottom()

  // Start streaming
  isStreaming.value = true
  streamingContent.value = ''

  streamController = aiApi.chatStream(
    {
      message,
      conversationId: currentConversationId.value || undefined,
      useRAG: useRAG.value,
      stream: true
    },
    handleStreamEvent,
    handleStreamError
  )
}

const handleStreamEvent = (event: ChatStreamEvent) => {
  switch (event.type) {
    case 'conversation':
      currentConversationId.value = event.conversationId
      break

    case 'sources':
      // Store sources for later
      break

    case 'chunk':
      streamingContent.value += event.delta
      scrollToBottom()
      break

    case 'done':
      // Add assistant message
      messages.value.push({
        id: `msg-${Date.now()}`,
        conversationId: currentConversationId.value || '',
        role: 'assistant',
        content: event.fullResponse,
        citations: event.citations,
        createdAt: new Date().toISOString()
      })
      isStreaming.value = false
      streamingContent.value = ''
      loadConversations() // Refresh list
      scrollToBottom()
      break

    case 'title':
      currentTitle.value = event.title
      break

    case 'error':
      toast.add({
        severity: 'error',
        summary: t('common.error'),
        detail: event.error,
        life: 5000
      })
      isStreaming.value = false
      break
  }
}

const handleStreamError = (error: Error) => {
  toast.add({
    severity: 'error',
    summary: t('common.error'),
    detail: error.message,
    life: 5000
  })
  isStreaming.value = false
}

const stopStreaming = () => {
  streamController?.abort()
  isStreaming.value = false

  // Save partial response
  if (streamingContent.value) {
    messages.value.push({
      id: `msg-${Date.now()}`,
      conversationId: currentConversationId.value || '',
      role: 'assistant',
      content: streamingContent.value + '\n\n*[Response stopped]*',
      createdAt: new Date().toISOString()
    })
    streamingContent.value = ''
  }
}

const handleEnter = (event: KeyboardEvent) => {
  if (!event.shiftKey) {
    event.preventDefault()
    sendMessage()
  }
}

const confirmDeleteConversation = (conv: Conversation) => {
  confirm.require({
    message: t('ai.deleteConfirm', { title: conv.title }),
    header: t('common.confirm'),
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: t('common.delete'),
    rejectLabel: t('common.cancel'),
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        await aiApi.deleteConversation(conv.id)
        if (currentConversationId.value === conv.id) {
          startNewChat()
        }
        loadConversations()
        toast.add({
          severity: 'success',
          summary: t('common.success'),
          detail: t('ai.deleteSuccess'),
          life: 3000
        })
      } catch (error) {
        toast.add({
          severity: 'error',
          summary: t('common.error'),
          detail: t('ai.deleteError'),
          life: 5000
        })
      }
    }
  })
}

const viewSource = (citation: Citation) => {
  // Navigate to document or show preview
  window.open(`/documents/${citation.documentId}`, '_blank')
}

const formatMessage = (content: string) => {
  // Convert markdown to HTML and sanitize
  const html = marked.parse(content) as string
  return DOMPurify.sanitize(html)
}

const formatTime = (dateString: string) => {
  return new Date(dateString).toLocaleTimeString([], {
    hour: '2-digit',
    minute: '2-digit'
  })
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString()
}

const scrollToBottom = () => {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
    }
  })
}

const loadStatus = async () => {
  try {
    aiStatus.value = await aiApi.getStatus()
  } catch (error) {
    console.error('Failed to load AI status:', error)
  }
}

// Watch for new messages to scroll
watch(messages, () => {
  scrollToBottom()
})

onMounted(() => {
  loadConversations()
  loadStatus()
})
</script>

<style scoped lang="scss">
.chat-view {
  display: flex;
  height: calc(100vh - 60px);
  background: var(--surface-ground);
}

.chat-sidebar {
  width: 280px;
  background: var(--surface-card);
  border-right: 1px solid var(--surface-border);
  display: flex;
  flex-direction: column;
  transition: width 0.2s ease;

  &.collapsed {
    width: 60px;
  }
}

.sidebar-header {
  display: flex;
  gap: 0.5rem;
  padding: 1rem;
  border-bottom: 1px solid var(--surface-border);

  .new-chat-btn {
    flex: 1;
  }
}

.conversations-list {
  flex: 1;
  overflow-y: auto;
  padding: 0.5rem;
}

.list-header {
  padding: 0.5rem;
  font-size: 0.75rem;
  font-weight: 600;
  color: var(--text-color-secondary);
  text-transform: uppercase;
}

.conversation-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem;
  border-radius: 8px;
  cursor: pointer;
  transition: background 0.2s;

  &:hover {
    background: var(--surface-hover);
  }

  &.active {
    background: var(--primary-color);
    color: white;

    .p-button {
      color: white;
    }
  }

  .conv-title {
    flex: 1;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }
}

.empty-list {
  padding: 1rem;
  text-align: center;
  color: var(--text-color-secondary);
  font-size: 0.875rem;
}

.chat-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.chat-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.5rem;
  background: var(--surface-card);
  border-bottom: 1px solid var(--surface-border);

  h2 {
    margin: 0;
    font-size: 1.125rem;
  }

  .header-right {
    display: flex;
    align-items: center;
    gap: 0.5rem;
  }
}

.messages-container {
  flex: 1;
  overflow-y: auto;
  padding: 1.5rem;
}

.welcome-screen {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  text-align: center;
  padding: 2rem;

  .welcome-icon {
    width: 80px;
    height: 80px;
    border-radius: 50%;
    background: linear-gradient(135deg, var(--primary-color), var(--primary-700));
    display: flex;
    align-items: center;
    justify-content: center;
    margin-bottom: 1.5rem;

    i {
      font-size: 2.5rem;
      color: white;
    }
  }

  h3 {
    margin: 0 0 0.5rem 0;
    font-size: 1.5rem;
  }

  p {
    color: var(--text-color-secondary);
    max-width: 400px;
    margin-bottom: 2rem;
  }

  .quick-actions {
    display: flex;
    gap: 0.75rem;
    flex-wrap: wrap;
    justify-content: center;
  }
}

.message {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;

  &.user {
    .message-content {
      background: var(--primary-color);
      color: white;
    }
  }

  &.assistant {
    .message-content {
      background: var(--surface-card);
    }
  }

  &.streaming {
    .message-content {
      opacity: 0.9;
    }
  }
}

.message-avatar {
  flex-shrink: 0;
}

.ai-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--primary-color), var(--primary-700));
  display: flex;
  align-items: center;
  justify-content: center;

  i {
    color: white;
    font-size: 1.25rem;
  }
}

.message-content {
  flex: 1;
  padding: 1rem;
  border-radius: 12px;
  max-width: 80%;
}

.message-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.5rem;
  font-size: 0.75rem;

  .sender {
    font-weight: 600;
  }

  .timestamp {
    color: var(--text-color-secondary);
  }
}

.message-text {
  line-height: 1.6;

  :deep(p) {
    margin: 0 0 0.5rem 0;

    &:last-child {
      margin-bottom: 0;
    }
  }

  :deep(code) {
    background: rgba(0, 0, 0, 0.1);
    padding: 0.125rem 0.375rem;
    border-radius: 4px;
    font-size: 0.875rem;
  }

  :deep(pre) {
    background: rgba(0, 0, 0, 0.1);
    padding: 0.75rem;
    border-radius: 6px;
    overflow-x: auto;

    code {
      background: none;
      padding: 0;
    }
  }
}

.typing-indicator {
  display: flex;
  gap: 4px;
  margin-top: 0.5rem;

  span {
    width: 8px;
    height: 8px;
    border-radius: 50%;
    background: var(--text-color-secondary);
    animation: typing 1.4s infinite;

    &:nth-child(2) {
      animation-delay: 0.2s;
    }

    &:nth-child(3) {
      animation-delay: 0.4s;
    }
  }
}

@keyframes typing {
  0%, 60%, 100% {
    transform: translateY(0);
    opacity: 0.4;
  }
  30% {
    transform: translateY(-4px);
    opacity: 1;
  }
}

.citations {
  margin-top: 1rem;
  padding-top: 0.75rem;
  border-top: 1px solid var(--surface-border);

  .citations-header {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-size: 0.75rem;
    font-weight: 600;
    color: var(--text-color-secondary);
    margin-bottom: 0.5rem;
  }

  .citation-list {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
  }

  .citation-item {
    display: flex;
    align-items: center;
    gap: 0.25rem;
    padding: 0.25rem 0.5rem;
    background: var(--surface-hover);
    border-radius: 4px;
    font-size: 0.75rem;
    cursor: pointer;
    transition: background 0.2s;

    &:hover {
      background: var(--surface-border);
    }

    .citation-index {
      font-weight: 600;
      color: var(--primary-color);
    }

    .citation-page {
      color: var(--text-color-secondary);
    }
  }
}

.input-area {
  padding: 1rem 1.5rem;
  background: var(--surface-card);
  border-top: 1px solid var(--surface-border);
}

.input-options {
  display: flex;
  gap: 1rem;
  margin-bottom: 0.75rem;

  .option {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-size: 0.875rem;
  }
}

.input-wrapper {
  display: flex;
  gap: 0.75rem;
  align-items: flex-end;

  :deep(.p-textarea) {
    flex: 1;
    max-height: 150px;
  }

  .p-button {
    flex-shrink: 0;

    &.streaming {
      background: var(--red-500);
      border-color: var(--red-500);
    }
  }
}

.settings-content {
  .setting-item {
    display: flex;
    justify-content: space-between;
    padding: 0.75rem 0;
    border-bottom: 1px solid var(--surface-border);

    &:last-child {
      border-bottom: none;
    }

    label {
      font-weight: 500;
    }
  }
}
</style>
