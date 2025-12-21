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

const calloutTypes = [
  { label: 'Info', value: 'info', icon: 'pi-info-circle', color: '#3b82f6' },
  { label: 'Success', value: 'success', icon: 'pi-check-circle', color: '#22c55e' },
  { label: 'Warning', value: 'warning', icon: 'pi-exclamation-triangle', color: '#f59e0b' },
  { label: 'Error', value: 'error', icon: 'pi-times-circle', color: '#ef4444' },
  { label: 'Tip', value: 'tip', icon: 'pi-lightbulb', color: '#8b5cf6' }
]

const metadata = computed(() => {
  if (!props.block.metadata) return { type: 'info' }
  try {
    return JSON.parse(props.block.metadata)
  } catch {
    return { type: 'info' }
  }
})

const selectedType = ref(metadata.value.type || 'info')

const currentType = computed(() =>
  calloutTypes.find(t => t.value === selectedType.value) || calloutTypes[0]
)

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
  <div
    class="callout-block"
    :class="`callout-${currentType.value}`"
    :style="{ '--callout-color': currentType.color }"
  >
    <div class="callout-header">
      <i :class="`pi ${currentType.icon}`" class="callout-icon"></i>
      <Dropdown
        v-if="!readonly"
        v-model="selectedType"
        :options="calloutTypes"
        optionLabel="label"
        optionValue="value"
        size="small"
        class="type-select"
      />
      <span v-else class="callout-type-label">{{ currentType.label }}</span>
    </div>
    <div
      ref="contentRef"
      class="callout-content"
      :contenteditable="!readonly"
      @input="handleInput"
      @focus="handleFocus"
      @blur="handleBlur"
      @keydown="handleKeydown"
      :data-placeholder="'Type your callout message...'"
    ></div>
  </div>
</template>

<style scoped>
.callout-block {
  padding: 1rem;
  border-left: 4px solid var(--callout-color, var(--primary-color));
  background: color-mix(in srgb, var(--callout-color) 10%, var(--surface-ground));
  border-radius: 0 var(--border-radius) var(--border-radius) 0;
}

.callout-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}

.callout-icon {
  color: var(--callout-color);
  font-size: 1.25rem;
}

.type-select {
  width: auto;
  min-width: 100px;
}

.callout-type-label {
  font-weight: 600;
  color: var(--callout-color);
}

.callout-content {
  outline: none;
  line-height: 1.6;
  min-height: 1.5em;
}

.callout-content:empty::before {
  content: attr(data-placeholder);
  color: var(--text-color-secondary);
  pointer-events: none;
}

/* Type-specific styles */
.callout-info {
  --callout-color: #3b82f6;
}

.callout-success {
  --callout-color: #22c55e;
}

.callout-warning {
  --callout-color: #f59e0b;
}

.callout-error {
  --callout-color: #ef4444;
}

.callout-tip {
  --callout-color: #8b5cf6;
}
</style>
