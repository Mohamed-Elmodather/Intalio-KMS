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

const listRef = ref<HTMLElement | null>(null)

const isOrdered = computed(() => props.block.type === 'NumberedList')

const listItems = computed(() => {
  if (!props.block.content) return ['']
  return props.block.content.split('\n')
})

onMounted(() => {
  updateListContent()
})

watch(() => props.block.content, () => {
  updateListContent()
})

const updateListContent = () => {
  if (!listRef.value) return

  // Clear existing items
  listRef.value.innerHTML = ''

  // Add list items
  for (const item of listItems.value) {
    const li = document.createElement('li')
    li.textContent = item
    li.contentEditable = props.readonly ? 'false' : 'true'
    listRef.value.appendChild(li)
  }
}

const handleInput = () => {
  if (!listRef.value) return

  const items = Array.from(listRef.value.querySelectorAll('li'))
  const content = items.map(li => li.textContent || '').join('\n')
  emit('update', content)
}

const handleFocus = () => {
  emit('focus')
}

const handleBlur = () => {
  emit('blur')
}

const handleKeydown = (event: KeyboardEvent) => {
  if (event.key === 'Enter') {
    // Default browser behavior handles new list item
  } else if (event.key === 'Backspace') {
    const target = event.target as HTMLElement
    if (target.textContent === '' && listRef.value?.children.length === 1) {
      event.preventDefault()
      emit('keydown', event)
    }
  } else {
    emit('keydown', event)
  }
}
</script>

<template>
  <component
    :is="isOrdered ? 'ol' : 'ul'"
    ref="listRef"
    class="list-block"
    :class="{ ordered: isOrdered }"
    @input="handleInput"
    @focus="handleFocus"
    @blur="handleBlur"
    @keydown="handleKeydown"
  >
  </component>
</template>

<style scoped>
.list-block {
  margin: 0;
  padding: 0.5rem 0.5rem 0.5rem 2rem;
  outline: none;
}

.list-block :deep(li) {
  margin: 0.25rem 0;
  padding: 0.25rem;
  outline: none;
  line-height: 1.6;
}

.list-block :deep(li:empty::before) {
  content: 'List item';
  color: var(--text-color-secondary);
  pointer-events: none;
}

.list-block.ordered {
  list-style-type: decimal;
}

.list-block:not(.ordered) {
  list-style-type: disc;
}
</style>
