<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import type { ContentBlock } from '@/composables/collaboration/useBlockEditor'

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
  emit('keydown', event)
}
</script>

<template>
  <blockquote
    ref="contentRef"
    class="quote-block"
    :contenteditable="!readonly"
    @input="handleInput"
    @focus="handleFocus"
    @blur="handleBlur"
    @keydown="handleKeydown"
    :data-placeholder="'Type a quote...'"
  ></blockquote>
</template>

<style scoped>
.quote-block {
  margin: 0;
  padding: 1rem 1.5rem;
  min-height: 1.5em;
  outline: none;
  border-left: 4px solid var(--primary-color);
  background: var(--surface-ground);
  font-style: italic;
  line-height: 1.6;
  word-wrap: break-word;
  border-radius: 0 var(--border-radius) var(--border-radius) 0;
}

.quote-block:empty::before {
  content: attr(data-placeholder);
  color: var(--text-color-secondary);
  pointer-events: none;
}
</style>
