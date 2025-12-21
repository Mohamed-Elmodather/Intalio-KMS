<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import type { ContentBlock } from '@/composables/collaboration/useBlockEditor'
import Dropdown from 'primevue/dropdown'

const props = defineProps<{
  block: ContentBlock
  readonly?: boolean
}>()

const emit = defineEmits<{
  (e: 'update', content: string): void
  (e: 'focus'): void
  (e: 'blur'): void
  (e: 'keydown', event: KeyboardEvent): void
}>()

const contentRef = ref<HTMLElement | null>(null)

const languages = [
  { label: 'Plain Text', value: 'text' },
  { label: 'JavaScript', value: 'javascript' },
  { label: 'TypeScript', value: 'typescript' },
  { label: 'Python', value: 'python' },
  { label: 'Java', value: 'java' },
  { label: 'C#', value: 'csharp' },
  { label: 'HTML', value: 'html' },
  { label: 'CSS', value: 'css' },
  { label: 'SQL', value: 'sql' },
  { label: 'JSON', value: 'json' },
  { label: 'Bash', value: 'bash' }
]

const metadata = computed(() => {
  if (!props.block.metadata) return { language: 'text' }
  try {
    return JSON.parse(props.block.metadata)
  } catch {
    return { language: 'text' }
  }
})

const selectedLanguage = ref(metadata.value.language || 'text')

onMounted(() => {
  if (contentRef.value) {
    contentRef.value.textContent = props.block.content
  }
})

watch(() => props.block.content, (newContent) => {
  if (contentRef.value && contentRef.value.textContent !== newContent) {
    contentRef.value.textContent = newContent
  }
})

const handleInput = () => {
  if (contentRef.value) {
    emit('update', contentRef.value.textContent || '')
  }
}

const handleFocus = () => {
  emit('focus')
}

const handleBlur = () => {
  emit('blur')
}

const handleKeydown = (event: KeyboardEvent) => {
  // Allow Tab for indentation in code blocks
  if (event.key === 'Tab') {
    event.preventDefault()
    document.execCommand('insertText', false, '  ')
    return
  }
  emit('keydown', event)
}
</script>

<template>
  <div class="code-block">
    <div class="code-header">
      <Dropdown
        v-model="selectedLanguage"
        :options="languages"
        optionLabel="label"
        optionValue="value"
        :disabled="readonly"
        size="small"
        class="language-select"
      />
    </div>
    <pre class="code-content">
      <code
        ref="contentRef"
        :class="`language-${selectedLanguage}`"
        :contenteditable="!readonly"
        @input="handleInput"
        @focus="handleFocus"
        @blur="handleBlur"
        @keydown="handleKeydown"
        :data-placeholder="'// Enter code here...'"
      ></code>
    </pre>
  </div>
</template>

<style scoped>
.code-block {
  background: var(--surface-900);
  border-radius: var(--border-radius);
  overflow: hidden;
}

.code-header {
  display: flex;
  justify-content: flex-end;
  padding: 0.5rem;
  background: var(--surface-800);
  border-bottom: 1px solid var(--surface-700);
}

.language-select {
  width: auto;
  min-width: 120px;
}

.code-content {
  margin: 0;
  padding: 1rem;
  overflow-x: auto;
}

.code-content code {
  display: block;
  font-family: 'Fira Code', 'Monaco', 'Menlo', monospace;
  font-size: 0.875rem;
  line-height: 1.6;
  color: var(--surface-100);
  outline: none;
  white-space: pre;
  min-height: 1.5em;
}

.code-content code:empty::before {
  content: attr(data-placeholder);
  color: var(--surface-500);
  pointer-events: none;
}
</style>
