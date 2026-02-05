<script setup lang="ts">
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import DOMPurify from 'dompurify'
import AISentimentBadge from './AISentimentBadge.vue'
import AIConfidenceBar from './AIConfidenceBar.vue'
import type { Entity, SentimentResult, ClassificationResult, SummarizationResult } from '@/types/ai'

const { t } = useI18n()

interface MessageAnalysis {
  entities?: Entity[]
  sentiment?: SentimentResult
  classification?: ClassificationResult
  summary?: SummarizationResult
  keyPoints?: string[]
}

interface Props {
  content: string
  isStreaming?: boolean
  analysis?: MessageAnalysis
  showCopyButton?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  isStreaming: false,
  showCopyButton: true
})

const emit = defineEmits<{
  entityClick: [entity: Entity]
  copy: [content: string]
}>()

const copiedBlocks = ref<Set<number>>(new Set())

// Parse and render markdown-like content
const renderedContent = computed(() => {
  let html = props.content

  // Escape HTML first
  html = html
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')

  // Code blocks (```code```)
  html = html.replace(/```(\w+)?\n([\s\S]*?)```/g, (_, lang, code) => {
    const language = lang || 'text'
    return `<div class="code-block" data-language="${language}">
      <div class="code-header">
        <span class="code-language">${language}</span>
        <button class="copy-code-btn" title="Copy code"><i class="fas fa-copy"></i></button>
      </div>
      <pre><code class="language-${language}">${code.trim()}</code></pre>
    </div>`
  })

  // Inline code (`code`)
  html = html.replace(/`([^`]+)`/g, '<code class="inline-code">$1</code>')

  // Bold (**text**)
  html = html.replace(/\*\*([^*]+)\*\*/g, '<strong>$1</strong>')

  // Italic (*text*)
  html = html.replace(/\*([^*]+)\*/g, '<em>$1</em>')

  // Headers (### Header)
  html = html.replace(/^### (.+)$/gm, '<h3 class="ai-heading">$1</h3>')
  html = html.replace(/^## (.+)$/gm, '<h2 class="ai-heading">$1</h2>')
  html = html.replace(/^# (.+)$/gm, '<h1 class="ai-heading">$1</h1>')

  // Unordered lists (- item)
  html = html.replace(/^- (.+)$/gm, '<li class="ai-list-item">$1</li>')
  html = html.replace(/(<li class="ai-list-item">.*<\/li>\n?)+/g, '<ul class="ai-list">$&</ul>')

  // Ordered lists (1. item)
  html = html.replace(/^\d+\. (.+)$/gm, '<li class="ai-list-item-ordered">$1</li>')
  html = html.replace(/(<li class="ai-list-item-ordered">.*<\/li>\n?)+/g, '<ol class="ai-list-ordered">$&</ol>')

  // Links [text](url)
  html = html.replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2" target="_blank" rel="noopener" class="ai-link">$1</a>')

  // Blockquotes (> text)
  html = html.replace(/^> (.+)$/gm, '<blockquote class="ai-blockquote">$1</blockquote>')

  // Horizontal rule (---)
  html = html.replace(/^---$/gm, '<hr class="ai-divider">')

  // Tables (simple markdown tables)
  html = html.replace(/\|(.+)\|\n\|[-|]+\|\n((?:\|.+\|\n?)+)/g, (_, header, body) => {
    const headers = header.split('|').filter((h: string) => h.trim()).map((h: string) => `<th>${h.trim()}</th>`).join('')
    const rows = body.trim().split('\n').map((row: string) => {
      const cells = row.split('|').filter((c: string) => c.trim()).map((c: string) => `<td>${c.trim()}</td>`).join('')
      return `<tr>${cells}</tr>`
    }).join('')
    return `<table class="ai-table"><thead><tr>${headers}</tr></thead><tbody>${rows}</tbody></table>`
  })

  // Paragraphs (double newline)
  html = html.replace(/\n\n/g, '</p><p class="ai-paragraph">')

  // Single newlines to <br>
  html = html.replace(/\n/g, '<br>')

  // Wrap in paragraph if not already wrapped
  if (!html.startsWith('<')) {
    html = `<p class="ai-paragraph">${html}</p>`
  }

  // Sanitize final output to prevent any XSS
  return DOMPurify.sanitize(html, {
    ALLOWED_TAGS: ['p', 'br', 'strong', 'em', 'code', 'pre', 'h1', 'h2', 'h3', 'ul', 'ol', 'li',
                   'a', 'blockquote', 'hr', 'table', 'thead', 'tbody', 'tr', 'th', 'td', 'div', 'span', 'i', 'button'],
    ALLOWED_ATTR: ['class', 'href', 'target', 'rel', 'data-language', 'title'],
    ADD_ATTR: ['target'],
  })
})

// Get unique entity types for color coding
function getEntityColor(type: string): string {
  const colors: Record<string, string> = {
    person: 'bg-blue-100 text-blue-700 border-blue-200',
    organization: 'bg-purple-100 text-purple-700 border-purple-200',
    location: 'bg-green-100 text-green-700 border-green-200',
    date: 'bg-amber-100 text-amber-700 border-amber-200',
    event: 'bg-pink-100 text-pink-700 border-pink-200',
    product: 'bg-cyan-100 text-cyan-700 border-cyan-200',
    amount: 'bg-orange-100 text-orange-700 border-orange-200',
    money: 'bg-emerald-100 text-emerald-700 border-emerald-200'
  }
  return colors[type] || 'bg-gray-100 text-gray-700 border-gray-200'
}

function getEntityIcon(type: string): string {
  const icons: Record<string, string> = {
    person: 'fas fa-user',
    organization: 'fas fa-building',
    location: 'fas fa-map-marker-alt',
    date: 'fas fa-calendar',
    event: 'fas fa-calendar-check',
    product: 'fas fa-box',
    amount: 'fas fa-hashtag',
    money: 'fas fa-dollar-sign'
  }
  return icons[type] || 'fas fa-tag'
}

function handleEntityClick(entity: Entity) {
  emit('entityClick', entity)
}

function copyContent() {
  navigator.clipboard.writeText(props.content)
  emit('copy', props.content)
}

function copyCodeBlock(index: number, code: string) {
  navigator.clipboard.writeText(code)
  copiedBlocks.value.add(index)
  setTimeout(() => {
    copiedBlocks.value.delete(index)
  }, 2000)
}
</script>

<template>
  <div class="ai-message-content">
    <!-- Main Content -->
    <div
      class="prose prose-teal max-w-none"
      :class="{ 'streaming': isStreaming }"
      v-html="renderedContent"
    />

    <!-- Streaming Indicator -->
    <span v-if="isStreaming" class="streaming-cursor">▊</span>

    <!-- Analysis Results -->
    <div v-if="analysis" class="ai-analysis-section mt-4 space-y-4">
      <!-- Entities -->
      <div v-if="analysis.entities?.length" class="analysis-block">
        <h4 class="analysis-title">
          <i class="fas fa-tags mr-2"></i>
          {{ $t('ai.messageContent.extractedEntities') }}
        </h4>
        <div class="flex flex-wrap gap-2 mt-2">
          <button
            v-for="entity in analysis.entities"
            :key="entity.text"
            @click="handleEntityClick(entity)"
            :class="['entity-chip', getEntityColor(entity.type)]"
          >
            <i :class="[getEntityIcon(entity.type), 'text-xs mr-1']"></i>
            <span>{{ entity.text }}</span>
            <span v-if="entity.confidence" class="entity-confidence">
              {{ Math.round(entity.confidence * 100) }}%
            </span>
          </button>
        </div>
      </div>

      <!-- Sentiment -->
      <div v-if="analysis.sentiment" class="analysis-block">
        <h4 class="analysis-title">
          <i class="fas fa-smile mr-2"></i>
          {{ $t('ai.messageContent.sentimentAnalysis') }}
        </h4>
        <div class="mt-2">
          <AISentimentBadge
            :sentiment="analysis.sentiment.overall"
            :score="analysis.sentiment.score"
            size="md"
          />
        </div>
      </div>

      <!-- Classification -->
      <div v-if="analysis.classification" class="analysis-block">
        <h4 class="analysis-title">
          <i class="fas fa-folder-tree mr-2"></i>
          {{ $t('ai.messageContent.classification') }}
        </h4>
        <div class="mt-2 flex items-center gap-3">
          <span class="classification-badge">
            {{ analysis.classification.primaryCategory }}
          </span>
          <AIConfidenceBar
            :value="analysis.classification.confidence"
            size="sm"
            class="flex-1 max-w-xs"
          />
        </div>
        <div v-if="analysis.classification.suggestedTags?.length" class="flex flex-wrap gap-1.5 mt-2">
          <span
            v-for="tag in analysis.classification.suggestedTags"
            :key="tag"
            class="tag-chip"
          >
            #{{ tag }}
          </span>
        </div>
      </div>

      <!-- Key Points -->
      <div v-if="analysis.keyPoints?.length" class="analysis-block">
        <h4 class="analysis-title">
          <i class="fas fa-list-check mr-2"></i>
          {{ $t('ai.messageContent.keyPoints') }}
        </h4>
        <ul class="key-points-list mt-2">
          <li v-for="point in analysis.keyPoints" :key="point" class="key-point-item">
            <i class="fas fa-check-circle text-teal-500 mr-2"></i>
            <span>{{ point }}</span>
          </li>
        </ul>
      </div>
    </div>

    <!-- Copy Button -->
    <button
      v-if="showCopyButton && !isStreaming"
      @click="copyContent"
      class="copy-content-btn"
      :title="$t('ai.messageContent.copyMessage')"
    >
      <i class="fas fa-copy"></i>
    </button>
  </div>
</template>

<style scoped>
.ai-message-content {
  position: relative;
}

/* Prose styling */
.prose :deep(h1.ai-heading),
.prose :deep(h2.ai-heading),
.prose :deep(h3.ai-heading) {
  @apply font-semibold text-gray-800 mt-4 mb-2;
}

.prose :deep(h1.ai-heading) {
  @apply text-xl;
}

.prose :deep(h2.ai-heading) {
  @apply text-lg;
}

.prose :deep(h3.ai-heading) {
  @apply text-base;
}

.prose :deep(.ai-paragraph) {
  @apply text-gray-700 leading-relaxed mb-3;
}

.prose :deep(.ai-list),
.prose :deep(.ai-list-ordered) {
  @apply my-3 pl-5 space-y-1;
}

.prose :deep(.ai-list-item),
.prose :deep(.ai-list-item-ordered) {
  @apply text-gray-700;
}

.prose :deep(.ai-list) {
  @apply list-disc;
}

.prose :deep(.ai-list-ordered) {
  @apply list-decimal;
}

.prose :deep(.ai-link) {
  @apply text-teal-600 hover:text-teal-700 underline;
}

.prose :deep(.ai-blockquote) {
  @apply border-l-4 border-teal-300 pl-4 py-1 my-3 text-gray-600 italic bg-teal-50/50 rounded-r;
}

.prose :deep(.ai-divider) {
  @apply border-gray-200 my-4;
}

/* Code blocks */
.prose :deep(.code-block) {
  @apply rounded-lg overflow-hidden my-3 bg-gray-900;
}

.prose :deep(.code-header) {
  @apply flex items-center justify-between px-4 py-2 bg-gray-800 text-xs;
}

.prose :deep(.code-language) {
  @apply text-gray-400 font-medium;
}

.prose :deep(.copy-code-btn) {
  @apply text-gray-400 hover:text-white transition-colors p-1;
}

.prose :deep(pre) {
  @apply p-4 overflow-x-auto;
}

.prose :deep(code) {
  @apply text-sm text-gray-100 font-mono;
}

.prose :deep(.inline-code) {
  @apply px-1.5 py-0.5 bg-gray-100 text-teal-700 rounded text-sm font-mono;
}

/* Tables */
.prose :deep(.ai-table) {
  @apply w-full border-collapse my-3 text-sm;
}

.prose :deep(.ai-table th) {
  @apply bg-gray-100 border border-gray-200 px-3 py-2 text-left font-semibold text-gray-700;
}

.prose :deep(.ai-table td) {
  @apply border border-gray-200 px-3 py-2 text-gray-600;
}

.prose :deep(.ai-table tr:hover td) {
  @apply bg-gray-50;
}

/* Streaming */
.streaming {
  @apply min-h-[1.5em];
}

.streaming-cursor {
  @apply inline-block text-teal-500 animate-pulse ml-0.5;
}

/* Analysis Section */
.ai-analysis-section {
  @apply border-t border-gray-100 pt-4;
}

.analysis-block {
  @apply p-3 bg-gray-50 rounded-xl;
}

.analysis-title {
  @apply text-xs font-semibold text-gray-500 uppercase tracking-wide flex items-center;
}

/* Entity chips */
.entity-chip {
  @apply px-2.5 py-1 rounded-lg text-xs font-medium border flex items-center gap-1
         hover:shadow-sm transition-all cursor-pointer;
}

.entity-confidence {
  @apply ml-1 opacity-60 text-[10px];
}

/* Classification */
.classification-badge {
  @apply px-3 py-1 bg-teal-100 text-teal-700 rounded-full text-sm font-medium;
}

.tag-chip {
  @apply px-2 py-0.5 bg-gray-100 text-gray-600 rounded-full text-xs;
}

/* Key Points */
.key-points-list {
  @apply space-y-2;
}

.key-point-item {
  @apply flex items-start text-sm text-gray-700;
}

/* Copy button */
.copy-content-btn {
  @apply absolute top-2 right-2 w-7 h-7 rounded-lg bg-gray-100 text-gray-400
         hover:bg-gray-200 hover:text-gray-600 flex items-center justify-center
         opacity-0 transition-all;
}

.ai-message-content:hover .copy-content-btn {
  @apply opacity-100;
}
</style>
