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
  <p
    ref="contentRef"
    class="paragraph-block"
    :contenteditable="!readonly"
    @input="handleInput"
    @focus="handleFocus"
    @blur="handleBlur"
    @keydown="handleKeydown"
    :data-placeholder="'Type something...'"
  ></p>
</template>

<style scoped>
.paragraph-block {
  margin: 0;
  padding: 0.5rem;
  min-height: 1.5em;
  outline: none;
  line-height: 1.6;
  word-wrap: break-word;
}

.paragraph-block:empty::before {
  content: attr(data-placeholder);
  color: var(--text-color-secondary);
  pointer-events: none;
}

.paragraph-block:focus {
  background: transparent;
}
</style>
