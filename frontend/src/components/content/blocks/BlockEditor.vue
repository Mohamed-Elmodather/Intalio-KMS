<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch, provide } from 'vue'
import { useBlockEditor, type ContentBlock, type BlockType } from '@/composables/collaboration/useBlockEditor'
import { useCollaboration, type Participant } from '@/composables/collaboration/useCollaboration'
import BlockWrapper from './BlockWrapper.vue'
import BlockToolbar from './BlockToolbar.vue'
import CollaborationBar from './CollaborationBar.vue'
import Button from 'primevue/button'
import ProgressSpinner from 'primevue/progressspinner'

const props = defineProps<{
  articleId: string
  readonly?: boolean
  enableCollaboration?: boolean
}>()

const emit = defineEmits<{
  (e: 'save'): void
  (e: 'change', blocks: ContentBlock[]): void
}>()

// Initialize composables
const editor = useBlockEditor(props.articleId)
const collaboration = props.enableCollaboration
  ? useCollaboration(props.articleId)
  : null

// Local state
const editorRef = ref<HTMLElement | null>(null)
const showToolbar = ref(false)
const toolbarPosition = ref({ x: 0, y: 0 })

// Provide editor context to child components
provide('blockEditor', editor)
provide('collaboration', collaboration)
provide('readonly', props.readonly)

// Computed
const hasBlocks = computed(() => editor.blocks.value.length > 0)

// Load blocks on mount
onMounted(async () => {
  await editor.loadBlocks()

  // Connect to collaboration if enabled
  if (collaboration) {
    try {
      await collaboration.connect()

      // Setup sync update handler
      collaboration.onSyncUpdate((update) => {
        // Handle incoming CRDT updates
        console.log('Received sync update:', update.length, 'bytes')
        // In a full implementation, apply update to local CRDT state
      })

      collaboration.onStateSynced((state) => {
        if (state) {
          console.log('Received full state:', state.length, 'bytes')
          // In a full implementation, merge with local state
        }
      })
    } catch (error) {
      console.error('Failed to connect to collaboration:', error)
    }
  }
})

onUnmounted(() => {
  if (collaboration) {
    collaboration.disconnect()
  }
})

// Watch for block changes
watch(
  () => editor.blocks.value,
  (newBlocks) => {
    emit('change', newBlocks)
  },
  { deep: true }
)

// Block operations
const handleAddBlock = async (afterBlockId?: string, type: BlockType = 'Paragraph') => {
  if (props.readonly) return

  if (afterBlockId) {
    await editor.insertBlockAfter(afterBlockId, type, '')
  } else {
    await editor.appendBlock(type, '')
  }
}

const handleDeleteBlock = async (blockId: string) => {
  if (props.readonly) return
  await editor.deleteBlock(blockId)
}

const handleBlockUpdate = async (blockId: string, content: string) => {
  if (props.readonly) return
  await editor.updateBlockContent(blockId, content)

  // Sync update to other participants
  if (collaboration) {
    // In a full implementation, send CRDT update
    // collaboration.syncUpdate(crdtUpdate)
  }
}

const handleBlockTypeChange = async (blockId: string, newType: BlockType) => {
  if (props.readonly) return
  await editor.changeBlockType(blockId, newType)
}

const handleBlockMove = async (blockId: string, direction: 'up' | 'down') => {
  if (props.readonly) return

  const currentBlock = editor.findBlock(blockId)
  if (!currentBlock) return

  const newPosition = direction === 'up'
    ? currentBlock.order - 1
    : currentBlock.order + 1

  if (newPosition >= 0 && newPosition < editor.flatBlocks.value.length) {
    await editor.moveBlock(blockId, newPosition)
  }
}

const handleBlockDuplicate = async (blockId: string) => {
  if (props.readonly) return
  await editor.duplicateBlock(blockId)
}

const handleBlockFocus = (blockId: string) => {
  editor.focusBlock(blockId)

  // Update cursor position for collaboration
  if (collaboration) {
    collaboration.updateCursor(blockId, 0)
    collaboration.updateAwareness({
      isTyping: true,
      focusedBlockId: blockId,
      status: 'editing'
    })
  }
}

const handleBlockBlur = () => {
  editor.focusBlock(null)

  if (collaboration) {
    collaboration.updateAwareness({
      isTyping: false,
      focusedBlockId: undefined,
      status: 'viewing'
    })
  }
}

const handleSelectionChange = () => {
  // Handle text selection changes for collaboration
  const selection = window.getSelection()
  if (!selection || selection.isCollapsed) {
    if (collaboration) {
      collaboration.updateSelection(undefined)
    }
    return
  }

  // In a full implementation, extract selection range and send to collaboration
}

// Keyboard shortcuts
const handleKeyDown = async (event: KeyboardEvent, blockId: string) => {
  if (props.readonly) return

  // Enter at end of block: create new block
  if (event.key === 'Enter' && !event.shiftKey) {
    event.preventDefault()
    await handleAddBlock(blockId)
  }

  // Backspace at start of empty block: delete block
  if (event.key === 'Backspace') {
    const block = editor.findBlock(blockId)
    if (block && !block.content) {
      event.preventDefault()
      await handleDeleteBlock(blockId)
    }
  }

  // Arrow keys for navigation
  if (event.key === 'ArrowUp' && event.altKey) {
    event.preventDefault()
    await handleBlockMove(blockId, 'up')
  }

  if (event.key === 'ArrowDown' && event.altKey) {
    event.preventDefault()
    await handleBlockMove(blockId, 'down')
  }

  // Duplicate with Ctrl+D
  if (event.key === 'd' && (event.ctrlKey || event.metaKey)) {
    event.preventDefault()
    await handleBlockDuplicate(blockId)
  }
}

// Handle click on empty editor area
const handleEditorClick = async (event: MouseEvent) => {
  if (props.readonly) return

  const target = event.target as HTMLElement
  if (target === editorRef.value && !hasBlocks.value) {
    await handleAddBlock()
  }
}
</script>

<template>
  <div class="block-editor" ref="editorRef" @click="handleEditorClick">
    <!-- Collaboration Bar -->
    <CollaborationBar
      v-if="enableCollaboration && collaboration"
      :participants="collaboration.participants.value"
      :is-connected="collaboration.isConnected.value"
    />

    <!-- Loading State -->
    <div v-if="editor.isLoading.value" class="editor-loading">
      <ProgressSpinner style="width: 50px; height: 50px" />
      <span>Loading content...</span>
    </div>

    <!-- Empty State -->
    <div v-else-if="!hasBlocks && !readonly" class="editor-empty">
      <p>Click here or press Enter to start writing...</p>
      <Button
        label="Add Block"
        icon="pi pi-plus"
        @click="handleAddBlock()"
        severity="secondary"
        outlined
      />
    </div>

    <!-- Blocks -->
    <div v-else class="blocks-container">
      <BlockWrapper
        v-for="block in editor.blocks.value"
        :key="block.id"
        :block="block"
        :is-selected="editor.selectedBlockId.value === block.id"
        :is-focused="editor.focusedBlockId.value === block.id"
        :readonly="readonly"
        @select="editor.selectBlock(block.id)"
        @focus="handleBlockFocus(block.id)"
        @blur="handleBlockBlur"
        @update="(content) => handleBlockUpdate(block.id, content)"
        @delete="handleDeleteBlock(block.id)"
        @add-after="(type) => handleAddBlock(block.id, type)"
        @change-type="(type) => handleBlockTypeChange(block.id, type)"
        @move="(direction) => handleBlockMove(block.id, direction)"
        @duplicate="handleBlockDuplicate(block.id)"
        @keydown="(e) => handleKeyDown(e, block.id)"
      />
    </div>

    <!-- Block Toolbar (floating) -->
    <BlockToolbar
      v-if="showToolbar && !readonly"
      :position="toolbarPosition"
      @add-block="handleAddBlock"
    />

    <!-- Saving Indicator -->
    <div v-if="editor.isSaving.value" class="saving-indicator">
      <i class="pi pi-spin pi-spinner"></i>
      <span>Saving...</span>
    </div>
  </div>
</template>

<style scoped>
.block-editor {
  position: relative;
  min-height: 400px;
  padding: 1rem;
  background: var(--surface-ground);
  border-radius: var(--border-radius);
}

.editor-loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  gap: 1rem;
  color: var(--text-color-secondary);
}

.editor-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  gap: 1rem;
  color: var(--text-color-secondary);
  border: 2px dashed var(--surface-border);
  border-radius: var(--border-radius);
  cursor: text;
}

.editor-empty:hover {
  border-color: var(--primary-color);
  background: var(--surface-hover);
}

.blocks-container {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.saving-indicator {
  position: fixed;
  bottom: 1rem;
  right: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: var(--surface-card);
  border-radius: var(--border-radius);
  box-shadow: var(--card-shadow);
  color: var(--text-color-secondary);
  font-size: 0.875rem;
}
</style>
