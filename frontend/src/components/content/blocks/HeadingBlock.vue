<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
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

const level = computed(() => {
  if (!props.block.metadata) return 1
  try {
    const meta = JSON.parse(props.block.metadata)
    return meta.level || 1
  } catch {
    return 1
  }
})

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
  <component
    :is="`h${level}`"
    ref="contentRef"
    class="heading-block"
    :class="`heading-${level}`"
    :contenteditable="!readonly"
    @input="handleInput"
    @focus="handleFocus"
    @blur="handleBlur"
    @keydown="handleKeydown"
    :data-placeholder="`Heading ${level}`"
  ></component>
</template>

<style scoped>
.heading-block {
  margin: 0;
  padding: 0.5rem;
  min-height: 1.5em;
  outline: none;
  word-wrap: break-word;
}

.heading-1 {
  font-size: 2rem;
  font-weight: 700;
  line-height: 1.2;
}

.heading-2 {
  font-size: 1.5rem;
  font-weight: 600;
  line-height: 1.3;
}

.heading-3 {
  font-size: 1.25rem;
  font-weight: 600;
  line-height: 1.4;
}

.heading-4 {
  font-size: 1.125rem;
  font-weight: 500;
  line-height: 1.4;
}

.heading-5 {
  font-size: 1rem;
  font-weight: 500;
  line-height: 1.5;
}

.heading-6 {
  font-size: 0.875rem;
  font-weight: 500;
  line-height: 1.5;
}

.heading-block:empty::before {
  content: attr(data-placeholder);
  color: var(--text-color-secondary);
  pointer-events: none;
}
</style>
