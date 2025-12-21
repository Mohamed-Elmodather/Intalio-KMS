import { ref, computed, watch } from 'vue'
import { useApi } from '@/composables/useApi'

export interface ContentBlock {
  id: string
  articleId: string
  type: BlockType
  content: string
  contentArabic?: string
  order: number
  metadata?: string
  parentBlockId?: string
  children: ContentBlock[]
  createdAt: string
  modifiedAt: string
}

export type BlockType =
  | 'Paragraph'
  | 'Heading'
  | 'BulletList'
  | 'NumberedList'
  | 'ListItem'
  | 'Quote'
  | 'Code'
  | 'Image'
  | 'Video'
  | 'File'
  | 'Embed'
  | 'Divider'
  | 'Table'
  | 'TableRow'
  | 'TableCell'
  | 'Callout'
  | 'Toggle'
  | 'TwoColumn'
  | 'ThreeColumn'
  | 'Column'

export interface CreateBlockRequest {
  type?: string
  content?: string
  contentArabic?: string
  order?: number
  metadata?: string
  parentBlockId?: string
  insertAfterId?: string
}

export interface UpdateBlockRequest {
  content?: string
  contentArabic?: string
  metadata?: string
  type?: string
}

export interface BlockMetadata {
  // Heading
  level?: number

  // Code
  language?: string
  showLineNumbers?: boolean

  // Image
  url?: string
  altText?: string
  caption?: string
  width?: number
  height?: number
  alignment?: 'left' | 'center' | 'right' | 'full'

  // Video
  thumbnailUrl?: string
  autoPlay?: boolean
  showControls?: boolean

  // Callout
  calloutType?: 'info' | 'warning' | 'error' | 'success' | 'tip'
  icon?: string
}

export function useBlockEditor(articleId: string) {
  const api = useApi()

  const blocks = ref<ContentBlock[]>([])
  const isLoading = ref(false)
  const isSaving = ref(false)
  const selectedBlockId = ref<string | null>(null)
  const focusedBlockId = ref<string | null>(null)

  const flatBlocks = computed(() => {
    const result: ContentBlock[] = []
    const flatten = (blockList: ContentBlock[]) => {
      for (const block of blockList) {
        result.push(block)
        if (block.children?.length) {
          flatten(block.children)
        }
      }
    }
    flatten(blocks.value)
    return result
  })

  const selectedBlock = computed(() =>
    flatBlocks.value.find(b => b.id === selectedBlockId.value)
  )

  // Load blocks from API
  const loadBlocks = async () => {
    isLoading.value = true
    try {
      const response = await api.get<ContentBlock[]>(`/api/articles/${articleId}/blocks`)
      blocks.value = response.data
    } catch (error) {
      console.error('Failed to load blocks:', error)
      throw error
    } finally {
      isLoading.value = false
    }
  }

  // Create a new block
  const createBlock = async (request: CreateBlockRequest): Promise<ContentBlock> => {
    isSaving.value = true
    try {
      const response = await api.post<ContentBlock>(
        `/api/articles/${articleId}/blocks`,
        request
      )
      // Reload blocks to get updated order
      await loadBlocks()
      return response.data
    } catch (error) {
      console.error('Failed to create block:', error)
      throw error
    } finally {
      isSaving.value = false
    }
  }

  // Update a block
  const updateBlock = async (blockId: string, request: UpdateBlockRequest): Promise<void> => {
    isSaving.value = true
    try {
      await api.put(`/api/articles/${articleId}/blocks/${blockId}`, request)
      // Update local state
      const block = flatBlocks.value.find(b => b.id === blockId)
      if (block) {
        if (request.content !== undefined) block.content = request.content
        if (request.contentArabic !== undefined) block.contentArabic = request.contentArabic
        if (request.metadata !== undefined) block.metadata = request.metadata
        if (request.type !== undefined) block.type = request.type as BlockType
      }
    } catch (error) {
      console.error('Failed to update block:', error)
      throw error
    } finally {
      isSaving.value = false
    }
  }

  // Delete a block
  const deleteBlock = async (blockId: string): Promise<void> => {
    isSaving.value = true
    try {
      await api.delete(`/api/articles/${articleId}/blocks/${blockId}`)
      await loadBlocks()
    } catch (error) {
      console.error('Failed to delete block:', error)
      throw error
    } finally {
      isSaving.value = false
    }
  }

  // Reorder blocks
  const reorderBlocks = async (blockIds: string[]): Promise<void> => {
    isSaving.value = true
    try {
      await api.put(`/api/articles/${articleId}/blocks/reorder`, { blockIds })
      await loadBlocks()
    } catch (error) {
      console.error('Failed to reorder blocks:', error)
      throw error
    } finally {
      isSaving.value = false
    }
  }

  // Move a block
  const moveBlock = async (
    blockId: string,
    newPosition: number,
    newParentId?: string
  ): Promise<void> => {
    isSaving.value = true
    try {
      await api.put(`/api/articles/${articleId}/blocks/${blockId}/move`, {
        newPosition,
        newParentId
      })
      await loadBlocks()
    } catch (error) {
      console.error('Failed to move block:', error)
      throw error
    } finally {
      isSaving.value = false
    }
  }

  // Duplicate a block
  const duplicateBlock = async (blockId: string): Promise<ContentBlock> => {
    isSaving.value = true
    try {
      const response = await api.post<ContentBlock>(
        `/api/articles/${articleId}/blocks/${blockId}/duplicate`
      )
      await loadBlocks()
      return response.data
    } catch (error) {
      console.error('Failed to duplicate block:', error)
      throw error
    } finally {
      isSaving.value = false
    }
  }

  // Render blocks to HTML
  const renderToHtml = async (language: string = 'en'): Promise<string> => {
    try {
      const response = await api.get<string>(
        `/api/articles/${articleId}/blocks/render`,
        { params: { language } }
      )
      return response.data
    } catch (error) {
      console.error('Failed to render HTML:', error)
      throw error
    }
  }

  // Import from HTML
  const importFromHtml = async (html: string): Promise<ContentBlock[]> => {
    isSaving.value = true
    try {
      const response = await api.post<ContentBlock[]>(
        `/api/articles/${articleId}/blocks/import`,
        { html }
      )
      await loadBlocks()
      return response.data
    } catch (error) {
      console.error('Failed to import HTML:', error)
      throw error
    } finally {
      isSaving.value = false
    }
  }

  // Helper: Insert block after another
  const insertBlockAfter = async (
    afterBlockId: string,
    type: BlockType = 'Paragraph',
    content: string = ''
  ): Promise<ContentBlock> => {
    return createBlock({
      type,
      content,
      insertAfterId: afterBlockId
    })
  }

  // Helper: Insert block at end
  const appendBlock = async (
    type: BlockType = 'Paragraph',
    content: string = ''
  ): Promise<ContentBlock> => {
    return createBlock({ type, content })
  }

  // Helper: Change block type
  const changeBlockType = async (blockId: string, newType: BlockType): Promise<void> => {
    return updateBlock(blockId, { type: newType })
  }

  // Helper: Update block content
  const updateBlockContent = async (
    blockId: string,
    content: string,
    contentArabic?: string
  ): Promise<void> => {
    return updateBlock(blockId, { content, contentArabic })
  }

  // Helper: Update block metadata
  const updateBlockMetadata = async (
    blockId: string,
    metadata: BlockMetadata
  ): Promise<void> => {
    return updateBlock(blockId, { metadata: JSON.stringify(metadata) })
  }

  // Helper: Get parsed metadata
  const getBlockMetadata = (block: ContentBlock): BlockMetadata | null => {
    if (!block.metadata) return null
    try {
      return JSON.parse(block.metadata)
    } catch {
      return null
    }
  }

  // Helper: Find block by ID
  const findBlock = (blockId: string): ContentBlock | undefined => {
    return flatBlocks.value.find(b => b.id === blockId)
  }

  // Helper: Get previous block
  const getPreviousBlock = (blockId: string): ContentBlock | undefined => {
    const index = flatBlocks.value.findIndex(b => b.id === blockId)
    return index > 0 ? flatBlocks.value[index - 1] : undefined
  }

  // Helper: Get next block
  const getNextBlock = (blockId: string): ContentBlock | undefined => {
    const index = flatBlocks.value.findIndex(b => b.id === blockId)
    return index < flatBlocks.value.length - 1 ? flatBlocks.value[index + 1] : undefined
  }

  // Selection management
  const selectBlock = (blockId: string | null) => {
    selectedBlockId.value = blockId
  }

  const focusBlock = (blockId: string | null) => {
    focusedBlockId.value = blockId
  }

  return {
    // State
    blocks,
    flatBlocks,
    isLoading,
    isSaving,
    selectedBlockId,
    focusedBlockId,
    selectedBlock,

    // CRUD operations
    loadBlocks,
    createBlock,
    updateBlock,
    deleteBlock,
    reorderBlocks,
    moveBlock,
    duplicateBlock,

    // Import/Export
    renderToHtml,
    importFromHtml,

    // Helpers
    insertBlockAfter,
    appendBlock,
    changeBlockType,
    updateBlockContent,
    updateBlockMetadata,
    getBlockMetadata,
    findBlock,
    getPreviousBlock,
    getNextBlock,

    // Selection
    selectBlock,
    focusBlock
  }
}
