<script setup lang="ts">
import { ref, computed, inject } from 'vue'
import type { ContentBlock, BlockType } from '@/composables/collaboration/useBlockEditor'
import type { Participant } from '@/composables/collaboration/useCollaboration'
import ParagraphBlock from './ParagraphBlock.vue'
import HeadingBlock from './HeadingBlock.vue'
import ListBlock from './ListBlock.vue'
import QuoteBlock from './QuoteBlock.vue'
import CodeBlock from './CodeBlock.vue'
import ImageBlock from './ImageBlock.vue'
import DividerBlock from './DividerBlock.vue'
import CalloutBlock from './CalloutBlock.vue'
import Button from 'primevue/button'
import Menu from 'primevue/menu'

const props = defineProps<{
  block: ContentBlock
  isSelected: boolean
  isFocused: boolean
  readonly?: boolean
}>()

const emit = defineEmits<{
  (e: 'select'): void
  (e: 'focus'): void
  (e: 'blur'): void
  (e: 'update', content: string): void
  (e: 'delete'): void
  (e: 'add-after', type?: BlockType): void
  (e: 'change-type', type: BlockType): void
  (e: 'move', direction: 'up' | 'down'): void
  (e: 'duplicate'): void
  (e: 'keydown', event: KeyboardEvent): void
}>()

const collaboration = inject<any>('collaboration')

const blockActionsMenu = ref<InstanceType<typeof Menu>>()
const blockTypeMenu = ref<InstanceType<typeof Menu>>()

// Get other users' cursors on this block
const remoteCursors = computed(() => {
  if (!collaboration) return []

  const cursors: { userId: string; userName: string; color: string }[] = []
  for (const [userId, cursor] of collaboration.state.value.cursors) {
    if (cursor.blockId === props.block.id) {
      const participant = collaboration.state.value.participants.find(
        (p: Participant) => p.userId === userId
      )
      if (participant) {
        cursors.push({
          userId,
          userName: participant.userName,
          color: participant.color
        })
      }
    }
  }
  return cursors
})

// Block type component mapping
const blockComponent = computed(() => {
  switch (props.block.type) {
    case 'Heading':
      return HeadingBlock
    case 'BulletList':
    case 'NumberedList':
      return ListBlock
    case 'Quote':
      return QuoteBlock
    case 'Code':
      return CodeBlock
    case 'Image':
      return ImageBlock
    case 'Divider':
      return DividerBlock
    case 'Callout':
      return CalloutBlock
    default:
      return ParagraphBlock
  }
})

// Block actions menu items
const blockActionsItems = [
  {
    label: 'Delete',
    icon: 'pi pi-trash',
    command: () => emit('delete')
  },
  {
    label: 'Duplicate',
    icon: 'pi pi-copy',
    command: () => emit('duplicate')
  },
  { separator: true },
  {
    label: 'Move Up',
    icon: 'pi pi-arrow-up',
    command: () => emit('move', 'up')
  },
  {
    label: 'Move Down',
    icon: 'pi pi-arrow-down',
    command: () => emit('move', 'down')
  }
]

// Block type menu items
const blockTypeItems = [
  {
    label: 'Text',
    items: [
      { label: 'Paragraph', icon: 'pi pi-align-left', command: () => emit('change-type', 'Paragraph') },
      { label: 'Heading 1', icon: 'pi pi-heading', command: () => emit('change-type', 'Heading') },
      { label: 'Heading 2', icon: 'pi pi-heading', command: () => emit('change-type', 'Heading') },
      { label: 'Heading 3', icon: 'pi pi-heading', command: () => emit('change-type', 'Heading') }
    ]
  },
  {
    label: 'Lists',
    items: [
      { label: 'Bullet List', icon: 'pi pi-list', command: () => emit('change-type', 'BulletList') },
      { label: 'Numbered List', icon: 'pi pi-sort-numeric-up', command: () => emit('change-type', 'NumberedList') }
    ]
  },
  {
    label: 'Media',
    items: [
      { label: 'Image', icon: 'pi pi-image', command: () => emit('change-type', 'Image') },
      { label: 'Video', icon: 'pi pi-video', command: () => emit('change-type', 'Video') },
      { label: 'Code', icon: 'pi pi-code', command: () => emit('change-type', 'Code') }
    ]
  },
  {
    label: 'Other',
    items: [
      { label: 'Quote', icon: 'pi pi-comment', command: () => emit('change-type', 'Quote') },
      { label: 'Callout', icon: 'pi pi-info-circle', command: () => emit('change-type', 'Callout') },
      { label: 'Divider', icon: 'pi pi-minus', command: () => emit('change-type', 'Divider') }
    ]
  }
]

const toggleActionsMenu = (event: Event) => {
  blockActionsMenu.value?.toggle(event)
}

const toggleTypeMenu = (event: Event) => {
  blockTypeMenu.value?.toggle(event)
}

const handleClick = () => {
  emit('select')
}

const handleFocus = () => {
  emit('focus')
}

const handleBlur = () => {
  emit('blur')
}

const handleUpdate = (content: string) => {
  emit('update', content)
}

const handleKeydown = (event: KeyboardEvent) => {
  emit('keydown', event)
}
</script>

<template>
  <div
    class="block-wrapper"
    :class="{
      'is-selected': isSelected,
      'is-focused': isFocused,
      'has-remote-cursors': remoteCursors.length > 0
    }"
    @click="handleClick"
  >
    <!-- Remote Cursors Indicators -->
    <div v-if="remoteCursors.length > 0" class="remote-cursors">
      <div
        v-for="cursor in remoteCursors"
        :key="cursor.userId"
        class="remote-cursor-indicator"
        :style="{ backgroundColor: cursor.color }"
        :title="cursor.userName"
      >
        {{ cursor.userName.charAt(0).toUpperCase() }}
      </div>
    </div>

    <!-- Block Controls (left side) -->
    <div v-if="!readonly" class="block-controls">
      <Button
        icon="pi pi-plus"
        severity="secondary"
        text
        rounded
        size="small"
        class="add-block-btn"
        @click.stop="emit('add-after')"
        v-tooltip.left="'Add block below'"
      />
      <Button
        icon="pi pi-ellipsis-v"
        severity="secondary"
        text
        rounded
        size="small"
        class="block-menu-btn"
        @click.stop="toggleActionsMenu"
        v-tooltip.left="'Block actions'"
      />
    </div>

    <!-- Block Content -->
    <div class="block-content">
      <component
        :is="blockComponent"
        :block="block"
        :readonly="readonly"
        @update="handleUpdate"
        @focus="handleFocus"
        @blur="handleBlur"
        @keydown="handleKeydown"
      />
    </div>

    <!-- Block Type Indicator (right side) -->
    <div v-if="!readonly && isSelected" class="block-type-control">
      <Button
        :label="block.type"
        icon="pi pi-chevron-down"
        iconPos="right"
        severity="secondary"
        text
        size="small"
        @click.stop="toggleTypeMenu"
      />
    </div>

    <!-- Menus -->
    <Menu ref="blockActionsMenu" :model="blockActionsItems" :popup="true" />
    <Menu ref="blockTypeMenu" :model="blockTypeItems" :popup="true" />
  </div>
</template>

<style scoped>
.block-wrapper {
  position: relative;
  display: flex;
  align-items: flex-start;
  gap: 0.5rem;
  padding: 0.25rem 0;
  border-radius: var(--border-radius);
  transition: background-color 0.2s;
}

.block-wrapper:hover {
  background: var(--surface-hover);
}

.block-wrapper.is-selected {
  background: var(--highlight-bg);
}

.block-wrapper.is-focused {
  outline: 2px solid var(--primary-color);
  outline-offset: 2px;
}

.block-wrapper.has-remote-cursors {
  border-left: 3px solid var(--primary-color);
}

.remote-cursors {
  position: absolute;
  top: -0.5rem;
  right: 0;
  display: flex;
  gap: 0.25rem;
}

.remote-cursor-indicator {
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 50%;
  color: white;
  font-size: 0.75rem;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
}

.block-controls {
  display: flex;
  flex-direction: column;
  opacity: 0;
  transition: opacity 0.2s;
}

.block-wrapper:hover .block-controls {
  opacity: 1;
}

.block-content {
  flex: 1;
  min-width: 0;
}

.block-type-control {
  opacity: 0;
  transition: opacity 0.2s;
}

.block-wrapper:hover .block-type-control,
.block-wrapper.is-selected .block-type-control {
  opacity: 1;
}

.add-block-btn,
.block-menu-btn {
  width: 1.5rem !important;
  height: 1.5rem !important;
}
</style>
