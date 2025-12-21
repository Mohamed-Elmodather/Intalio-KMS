<script setup lang="ts">
import type { ContentBlock } from '@/composables/collaboration/useBlockEditor'

const props = defineProps<{
  block: ContentBlock
  readonly?: boolean
}>()

const emit = defineEmits<{
  (e: 'focus'): void
  (e: 'blur'): void
}>()

const handleClick = () => {
  emit('focus')
}
</script>

<template>
  <div class="divider-block" @click="handleClick" tabindex="0" @focus="emit('focus')" @blur="emit('blur')">
    <hr />
  </div>
</template>

<style scoped>
.divider-block {
  padding: 1rem 0;
  outline: none;
  cursor: pointer;
}

.divider-block:focus hr {
  border-color: var(--primary-color);
}

.divider-block hr {
  border: none;
  border-top: 2px solid var(--surface-border);
  margin: 0;
  transition: border-color 0.2s;
}
</style>
