<script setup lang="ts">
import { ref, computed, watch } from 'vue'

interface Collection {
  id: string
  name: string
  itemCount: number
  thumbnail: string | null
  visibility: 'private' | 'shared'
  containsItem?: boolean
}

interface Props {
  show: boolean
  contentType: 'article' | 'document' | 'media'
  contentId: string | number
  contentTitle: string
  contentThumbnail?: string
}

const props = defineProps<Props>()

const emit = defineEmits<{
  close: []
  added: [collectionIds: string[]]
}>()

// State
const searchQuery = ref('')
const selectedCollections = ref<Set<string>>(new Set())
const showCreateNew = ref(false)
const newCollectionName = ref('')
const newCollectionVisibility = ref<'private' | 'shared'>('private')

// Mock collections data
const collections = ref<Collection[]>([
  { id: 'col-1', name: 'Tournament Highlights 2027', itemCount: 15, thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=100', visibility: 'shared', containsItem: false },
  { id: 'col-2', name: 'Team Research - Group A', itemCount: 22, thumbnail: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=100', visibility: 'private', containsItem: false },
  { id: 'col-3', name: 'Media Kit 2027', itemCount: 45, thumbnail: 'https://images.unsplash.com/photo-1560272564-c83b66b1ad12?w=100', visibility: 'shared', containsItem: true },
  { id: 'col-4', name: 'Fan Engagement Ideas', itemCount: 8, thumbnail: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=100', visibility: 'shared', containsItem: false },
  { id: 'col-5', name: 'Venue Documentation', itemCount: 32, thumbnail: 'https://images.unsplash.com/photo-1577223625816-7546f13df25d?w=100', visibility: 'private', containsItem: false },
  { id: 'col-6', name: 'Training Content Library', itemCount: 18, thumbnail: 'https://images.unsplash.com/photo-1434030216411-0b793f4b4173?w=100', visibility: 'shared', containsItem: false }
])

// Computed
const filteredCollections = computed(() => {
  if (!searchQuery.value) return collections.value
  const query = searchQuery.value.toLowerCase()
  return collections.value.filter(c => c.name.toLowerCase().includes(query))
})

const selectedCount = computed(() => selectedCollections.value.size)

const getContentTypeIcon = computed(() => {
  switch (props.contentType) {
    case 'article': return 'fas fa-newspaper'
    case 'document': return 'fas fa-file-alt'
    case 'media': return 'fas fa-photo-video'
    default: return 'fas fa-file'
  }
})

const getContentTypeLabel = computed(() => {
  switch (props.contentType) {
    case 'article': return 'Article'
    case 'document': return 'Document'
    case 'media': return 'Media'
    default: return 'Item'
  }
})

// Methods
function toggleCollection(collectionId: string) {
  if (selectedCollections.value.has(collectionId)) {
    selectedCollections.value.delete(collectionId)
  } else {
    selectedCollections.value.add(collectionId)
  }
}

function isSelected(collectionId: string): boolean {
  return selectedCollections.value.has(collectionId)
}

function createNewCollection() {
  if (!newCollectionName.value.trim()) return

  const newCollection: Collection = {
    id: `col-${Date.now()}`,
    name: newCollectionName.value.trim(),
    itemCount: 0,
    thumbnail: null,
    visibility: newCollectionVisibility.value,
    containsItem: false
  }

  collections.value.unshift(newCollection)
  selectedCollections.value.add(newCollection.id)

  // Reset form
  newCollectionName.value = ''
  newCollectionVisibility.value = 'private'
  showCreateNew.value = false
}

function addToCollections() {
  const collectionIds = Array.from(selectedCollections.value)

  // Update item counts
  collectionIds.forEach(id => {
    const collection = collections.value.find(c => c.id === id)
    if (collection && !collection.containsItem) {
      collection.itemCount++
      collection.containsItem = true
    }
  })

  emit('added', collectionIds)
  closeModal()
}

function closeModal() {
  emit('close')
}

// Reset state when modal opens
watch(() => props.show, (newValue) => {
  if (newValue) {
    searchQuery.value = ''
    selectedCollections.value = new Set()
    showCreateNew.value = false
    newCollectionName.value = ''

    // Pre-select collections that already contain this item
    collections.value.forEach(c => {
      if (c.containsItem) {
        selectedCollections.value.add(c.id)
      }
    })
  }
})
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="show" class="modal-overlay" @click.self="closeModal">
        <div class="modal-content">
          <!-- Header -->
          <div class="modal-header">
            <h3 class="modal-title">
              <i class="fas fa-layer-group text-teal-500 mr-2"></i>
              Add to Collection
            </h3>
            <button @click="closeModal" class="modal-close">
              <i class="fas fa-times"></i>
            </button>
          </div>

          <!-- Content Preview -->
          <div class="content-preview">
            <div class="preview-thumbnail">
              <img v-if="contentThumbnail" :src="contentThumbnail" :alt="contentTitle" />
              <div v-else class="thumbnail-placeholder">
                <i :class="getContentTypeIcon"></i>
              </div>
            </div>
            <div class="preview-info">
              <span class="preview-type">
                <i :class="getContentTypeIcon"></i>
                {{ getContentTypeLabel }}
              </span>
              <h4 class="preview-title">{{ contentTitle }}</h4>
            </div>
          </div>

          <!-- Body -->
          <div class="modal-body">
            <!-- Create New Button -->
            <button
              v-if="!showCreateNew"
              @click="showCreateNew = true"
              class="create-new-btn"
            >
              <i class="fas fa-plus"></i>
              <span>Create New Collection</span>
            </button>

            <!-- Create New Form -->
            <div v-else class="create-new-form">
              <input
                v-model="newCollectionName"
                type="text"
                class="new-collection-input"
                placeholder="Collection name..."
                autofocus
                @keydown.enter="createNewCollection"
              />
              <div class="create-form-actions">
                <div class="visibility-toggle">
                  <button
                    @click="newCollectionVisibility = 'private'"
                    :class="['visibility-btn', newCollectionVisibility === 'private' ? 'active' : '']"
                    title="Private"
                  >
                    <i class="fas fa-lock"></i>
                  </button>
                  <button
                    @click="newCollectionVisibility = 'shared'"
                    :class="['visibility-btn', newCollectionVisibility === 'shared' ? 'active' : '']"
                    title="Shared"
                  >
                    <i class="fas fa-globe"></i>
                  </button>
                </div>
                <div class="create-btns">
                  <button @click="showCreateNew = false" class="cancel-create-btn">
                    Cancel
                  </button>
                  <button
                    @click="createNewCollection"
                    class="confirm-create-btn"
                    :disabled="!newCollectionName.trim()"
                  >
                    Create
                  </button>
                </div>
              </div>
            </div>

            <!-- Search -->
            <div class="search-box">
              <i class="fas fa-search"></i>
              <input
                v-model="searchQuery"
                type="text"
                class="search-input"
                placeholder="Search collections..."
              />
              <button v-if="searchQuery" @click="searchQuery = ''" class="clear-search">
                <i class="fas fa-times"></i>
              </button>
            </div>

            <!-- Collections List -->
            <div class="collections-list">
              <div
                v-for="collection in filteredCollections"
                :key="collection.id"
                @click="toggleCollection(collection.id)"
                :class="['collection-item', { selected: isSelected(collection.id), 'already-added': collection.containsItem }]"
              >
                <div class="collection-checkbox">
                  <i v-if="isSelected(collection.id)" class="fas fa-check"></i>
                </div>
                <div class="collection-thumbnail">
                  <img v-if="collection.thumbnail" :src="collection.thumbnail" :alt="collection.name" />
                  <div v-else class="thumb-placeholder">
                    <i class="fas fa-layer-group"></i>
                  </div>
                </div>
                <div class="collection-info">
                  <span class="collection-name">{{ collection.name }}</span>
                  <span class="collection-meta">
                    {{ collection.itemCount }} items
                    <span v-if="collection.containsItem" class="already-badge">
                      <i class="fas fa-check-circle"></i>
                      Already added
                    </span>
                  </span>
                </div>
                <div class="collection-visibility">
                  <i :class="collection.visibility === 'private' ? 'fas fa-lock' : 'fas fa-globe'"></i>
                </div>
              </div>

              <!-- Empty State -->
              <div v-if="filteredCollections.length === 0" class="empty-state">
                <i class="fas fa-search"></i>
                <span>No collections found</span>
              </div>
            </div>
          </div>

          <!-- Footer -->
          <div class="modal-footer">
            <button @click="closeModal" class="btn-cancel">
              Cancel
            </button>
            <button
              @click="addToCollections"
              class="btn-add"
              :disabled="selectedCount === 0"
            >
              <i class="fas fa-plus mr-2"></i>
              Add to {{ selectedCount }} Collection{{ selectedCount !== 1 ? 's' : '' }}
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
}

.modal-content {
  background: white;
  border-radius: 16px;
  width: 100%;
  max-width: 480px;
  max-height: 85vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25);
}

.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid #f1f5f9;
}

.modal-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1e293b;
  display: flex;
  align-items: center;
}

.modal-close {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f1f5f9;
  border: none;
  border-radius: 8px;
  color: #64748b;
  cursor: pointer;
  transition: all 0.15s;
}

.modal-close:hover {
  background: #e5e7eb;
  color: #1e293b;
}

/* Content Preview */
.content-preview {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem 1.5rem;
  background: #f8fafc;
  border-bottom: 1px solid #f1f5f9;
}

.preview-thumbnail {
  width: 56px;
  height: 56px;
  border-radius: 10px;
  overflow: hidden;
  flex-shrink: 0;
}

.preview-thumbnail img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.thumbnail-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #e2e8f0 0%, #cbd5e1 100%);
  color: #94a3b8;
  font-size: 1.25rem;
}

.preview-info {
  flex: 1;
  min-width: 0;
}

.preview-type {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.6875rem;
  font-weight: 600;
  text-transform: uppercase;
  color: #94a3b8;
  margin-bottom: 0.25rem;
}

.preview-title {
  font-size: 0.875rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Modal Body */
.modal-body {
  padding: 1rem 1.5rem;
  overflow-y: auto;
  flex: 1;
}

/* Create New Button */
.create-new-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  width: 100%;
  padding: 0.875rem 1rem;
  background: #f0fdfa;
  border: 2px dashed #99f6e4;
  border-radius: 12px;
  font-size: 0.875rem;
  font-weight: 600;
  color: #0f766e;
  cursor: pointer;
  transition: all 0.2s;
  margin-bottom: 1rem;
}

.create-new-btn:hover {
  background: #ccfbf1;
  border-color: #14b8a6;
}

/* Create New Form */
.create-new-form {
  padding: 1rem;
  background: #f8fafc;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  margin-bottom: 1rem;
}

.new-collection-input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 0.9375rem;
  margin-bottom: 0.75rem;
}

.new-collection-input:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.create-form-actions {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.visibility-toggle {
  display: flex;
  gap: 0.25rem;
}

.visibility-btn {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  color: #94a3b8;
  cursor: pointer;
  transition: all 0.15s;
}

.visibility-btn:hover {
  border-color: #cbd5e1;
}

.visibility-btn.active {
  background: #14b8a6;
  border-color: #14b8a6;
  color: white;
}

.create-btns {
  display: flex;
  gap: 0.5rem;
}

.cancel-create-btn {
  padding: 0.5rem 0.875rem;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #64748b;
  cursor: pointer;
}

.cancel-create-btn:hover {
  background: #f8fafc;
}

.confirm-create-btn {
  padding: 0.5rem 0.875rem;
  background: #14b8a6;
  border: none;
  border-radius: 6px;
  font-size: 0.8125rem;
  font-weight: 600;
  color: white;
  cursor: pointer;
}

.confirm-create-btn:hover:not(:disabled) {
  background: #0d9488;
}

.confirm-create-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Search Box */
.search-box {
  position: relative;
  margin-bottom: 1rem;
}

.search-box i {
  position: absolute;
  left: 0.875rem;
  top: 50%;
  transform: translateY(-50%);
  color: #94a3b8;
  font-size: 0.875rem;
}

.search-input {
  width: 100%;
  padding: 0.625rem 2.5rem 0.625rem 2.5rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 0.875rem;
}

.search-input:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.clear-search {
  position: absolute;
  right: 0.5rem;
  top: 50%;
  transform: translateY(-50%);
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f1f5f9;
  border: none;
  border-radius: 50%;
  color: #64748b;
  cursor: pointer;
  font-size: 0.75rem;
}

.clear-search:hover {
  background: #e5e7eb;
}

/* Collections List */
.collections-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  max-height: 280px;
  overflow-y: auto;
}

.collection-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.15s;
}

.collection-item:hover {
  border-color: #cbd5e1;
  background: #f8fafc;
}

.collection-item.selected {
  border-color: #14b8a6;
  background: #f0fdfa;
}

.collection-item.already-added {
  opacity: 0.7;
}

.collection-checkbox {
  width: 20px;
  height: 20px;
  border: 2px solid #e5e7eb;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  transition: all 0.15s;
}

.collection-item.selected .collection-checkbox {
  background: #14b8a6;
  border-color: #14b8a6;
  color: white;
}

.collection-checkbox i {
  font-size: 0.625rem;
}

.collection-thumbnail {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  overflow: hidden;
  flex-shrink: 0;
}

.collection-thumbnail img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.thumb-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #e2e8f0 0%, #cbd5e1 100%);
  color: #94a3b8;
  font-size: 0.875rem;
}

.collection-info {
  flex: 1;
  min-width: 0;
}

.collection-name {
  display: block;
  font-size: 0.875rem;
  font-weight: 500;
  color: #1e293b;
  margin-bottom: 0.125rem;
}

.collection-meta {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.75rem;
  color: #94a3b8;
}

.already-badge {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  color: #0f766e;
}

.collection-visibility {
  color: #94a3b8;
  font-size: 0.75rem;
}

/* Empty State */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 2rem;
  color: #94a3b8;
  text-align: center;
}

.empty-state i {
  font-size: 1.5rem;
}

/* Modal Footer */
.modal-footer {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 1rem 1.5rem;
  border-top: 1px solid #f1f5f9;
  background: #f8fafc;
}

.btn-cancel {
  padding: 0.625rem 1.25rem;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 500;
  color: #64748b;
  cursor: pointer;
  transition: all 0.15s;
}

.btn-cancel:hover {
  background: #f8fafc;
  border-color: #cbd5e1;
}

.btn-add {
  display: flex;
  align-items: center;
  padding: 0.625rem 1.25rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border: none;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  color: white;
  cursor: pointer;
  transition: all 0.15s;
}

.btn-add:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.btn-add:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Modal Transitions */
.modal-enter-active,
.modal-leave-active {
  transition: all 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .modal-content,
.modal-leave-to .modal-content {
  transform: scale(0.95) translateY(20px);
}
</style>
