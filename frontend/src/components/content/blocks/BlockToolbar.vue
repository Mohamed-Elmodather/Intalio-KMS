<script setup lang="ts">
import type { BlockType } from '@/composables/collaboration/useBlockEditor'
import Button from 'primevue/button'

const props = defineProps<{
  position: { x: number; y: number }
}>()

const emit = defineEmits<{
  (e: 'add-block', type?: BlockType): void
}>()

const blockTypes: { type: BlockType; icon: string; label: string }[] = [
  { type: 'Paragraph', icon: 'pi-align-left', label: 'Text' },
  { type: 'Heading', icon: 'pi-heading', label: 'Heading' },
  { type: 'BulletList', icon: 'pi-list', label: 'Bullet List' },
  { type: 'NumberedList', icon: 'pi-sort-numeric-up', label: 'Numbered List' },
  { type: 'Quote', icon: 'pi-comment', label: 'Quote' },
  { type: 'Code', icon: 'pi-code', label: 'Code' },
  { type: 'Image', icon: 'pi-image', label: 'Image' },
  { type: 'Callout', icon: 'pi-info-circle', label: 'Callout' },
  { type: 'Divider', icon: 'pi-minus', label: 'Divider' }
]

const addBlock = (type: BlockType) => {
  emit('add-block', type)
}
</script>

<template>
  <div
    class="block-toolbar"
    :style="{
      left: `${position.x}px`,
      top: `${position.y}px`
    }"
  >
    <div class="toolbar-content">
      <Button
        v-for="blockType in blockTypes"
        :key="blockType.type"
        :icon="`pi ${blockType.icon}`"
        :title="blockType.label"
        severity="secondary"
        text
        rounded
        size="small"
        @click="addBlock(blockType.type)"
      />
    </div>
  </div>
</template>

<style scoped>
.block-toolbar {
  position: absolute;
  z-index: 1000;
  background: var(--surface-card);
  border-radius: var(--border-radius);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
  padding: 0.5rem;
}

.toolbar-content {
  display: flex;
  gap: 0.25rem;
  flex-wrap: wrap;
  max-width: 280px;
}
</style>
