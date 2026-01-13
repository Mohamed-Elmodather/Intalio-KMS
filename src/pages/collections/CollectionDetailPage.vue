<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()

// Types
interface Collaborator {
  id: string
  name: string
  initials: string
  color: string
  email: string
  role: 'owner' | 'editor' | 'viewer'
  addedAt: string
}

interface Comment {
  id: string
  author: { id: string; name: string; initials: string; color: string }
  content: string
  createdAt: string
  mentions: string[]
}

interface CollectionItem {
  id: string
  contentType: 'article' | 'document' | 'media'
  contentId: string | number
  addedAt: string
  addedBy: string
  order: number
  title: string
  thumbnail?: string
  description?: string
  metadata?: Record<string, any>
}

interface Collection {
  id: string
  name: string
  description: string
  thumbnail: string | null
  createdAt: string
  updatedAt: string
  author: { id: string; name: string; initials: string; color: string }
  visibility: 'private' | 'shared'
  itemCount: number
  items: CollectionItem[]
  collaborators: Collaborator[]
  comments: Comment[]
}

// Current user (mock)
const currentUser = ref({
  id: 'user-1',
  name: 'Ahmed Imam',
  initials: 'AI',
  color: '#14b8a6',
  email: 'ahmed@afc.com'
})

// UI State
const isEditing = ref(false)
const showCollaboratorModal = ref(false)
const showDeleteConfirm = ref(false)
const showShareModal = ref(false)
const newComment = ref('')
const inviteEmail = ref('')
const inviteRole = ref<'editor' | 'viewer'>('viewer')
const draggedItem = ref<string | null>(null)

// Editable fields
const editName = ref('')
const editDescription = ref('')

// Mock collection data (in real app, fetch by ID)
const collection = ref<Collection>({
  id: 'col-1',
  name: 'Tournament Highlights 2027',
  description: 'Collection of best moments, match highlights, and memorable plays from AFC Asian Cup 2027',
  thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400',
  createdAt: '2027-01-05T10:00:00Z',
  updatedAt: '2027-01-07T08:30:00Z',
  author: { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6' },
  visibility: 'shared',
  itemCount: 8,
  items: [
    {
      id: 'item-1',
      contentType: 'media',
      contentId: 5,
      addedAt: '2027-01-05T10:00:00Z',
      addedBy: 'user-1',
      order: 1,
      title: 'Saudi Arabia vs Japan: Opening Match Preview',
      description: 'Comprehensive preview of the opening match between host nation Saudi Arabia and Japan.',
      thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400',
      metadata: { duration: '12:45', views: '45.2K', type: 'video' }
    },
    {
      id: 'item-2',
      contentType: 'article',
      contentId: 1,
      addedAt: '2027-01-05T11:00:00Z',
      addedBy: 'user-1',
      order: 2,
      title: 'Match Analysis: Opening Day Surprises',
      description: 'In-depth analysis of the tactical battles and standout performances from matchday one.',
      thumbnail: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=400',
      metadata: { readTime: '8 min', author: 'Sports Desk' }
    },
    {
      id: 'item-3',
      contentType: 'media',
      contentId: 13,
      addedAt: '2027-01-06T09:00:00Z',
      addedBy: 'user-2',
      order: 3,
      title: 'AFC Asian Cup 2027 Official Draw Ceremony',
      description: 'Highlights from the official draw ceremony held in Riyadh.',
      thumbnail: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=400',
      metadata: { duration: '25:30', views: '120K', type: 'video' }
    },
    {
      id: 'item-4',
      contentType: 'document',
      contentId: 'd-1',
      addedAt: '2027-01-06T10:00:00Z',
      addedBy: 'user-1',
      order: 4,
      title: 'Tournament Schedule & Fixtures',
      description: 'Complete match schedule with dates, times, and venues for all tournament matches.',
      thumbnail: undefined,
      metadata: { size: '2.4 MB', format: 'PDF' }
    },
    {
      id: 'item-5',
      contentType: 'media',
      contentId: 6,
      addedAt: '2027-01-06T14:00:00Z',
      addedBy: 'user-2',
      order: 5,
      title: 'King Fahd Stadium: Behind the Scenes Tour',
      description: 'Exclusive tour of the renovated King Fahd International Stadium.',
      thumbnail: 'https://images.unsplash.com/photo-1577223625816-7546f13df25d?w=400',
      metadata: { duration: '18:20', views: '32.1K', type: 'video' }
    },
    {
      id: 'item-6',
      contentType: 'article',
      contentId: 2,
      addedAt: '2027-01-07T08:00:00Z',
      addedBy: 'user-1',
      order: 6,
      title: 'Top 10 Players to Watch at AFC Asian Cup 2027',
      description: 'Our picks for the most exciting talents to follow during the tournament.',
      thumbnail: 'https://images.unsplash.com/photo-1508098682722-e99c643e7f76?w=400',
      metadata: { readTime: '12 min', author: 'Editorial Team' }
    },
    {
      id: 'item-7',
      contentType: 'media',
      contentId: 'm-7',
      addedAt: '2027-01-07T09:00:00Z',
      addedBy: 'user-2',
      order: 7,
      title: 'Fan Zone Highlights: Opening Weekend',
      description: 'Capturing the excitement from fan zones across Saudi Arabia.',
      thumbnail: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=400',
      metadata: { photoCount: 45, type: 'gallery' }
    },
    {
      id: 'item-8',
      contentType: 'document',
      contentId: 'd-2',
      addedAt: '2027-01-07T10:00:00Z',
      addedBy: 'user-1',
      order: 8,
      title: 'Media Accreditation Guidelines',
      description: 'Official guidelines for media personnel covering the tournament.',
      thumbnail: undefined,
      metadata: { size: '1.8 MB', format: 'PDF' }
    }
  ],
  collaborators: [
    { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6', email: 'ahmed@afc.com', role: 'owner', addedAt: '2027-01-05T10:00:00Z' },
    { id: 'user-2', name: 'Sarah Chen', initials: 'SC', color: '#8b5cf6', email: 'sarah@afc.com', role: 'editor', addedAt: '2027-01-05T12:00:00Z' },
    { id: 'user-3', name: 'Mohammed Al-Rashid', initials: 'MR', color: '#f59e0b', email: 'mohammed@afc.com', role: 'viewer', addedAt: '2027-01-06T09:00:00Z' }
  ],
  comments: [
    {
      id: 'comment-1',
      author: { id: 'user-2', name: 'Sarah Chen', initials: 'SC', color: '#8b5cf6' },
      content: 'Great collection! Added the draw ceremony video and stadium tour.',
      createdAt: '2027-01-06T09:30:00Z',
      mentions: []
    },
    {
      id: 'comment-2',
      author: { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6' },
      content: '@Sarah Chen Thanks! Can you also add the fan zone photos from the opening weekend?',
      createdAt: '2027-01-06T11:00:00Z',
      mentions: ['user-2']
    },
    {
      id: 'comment-3',
      author: { id: 'user-2', name: 'Sarah Chen', initials: 'SC', color: '#8b5cf6' },
      content: 'Done! Added the fan zone gallery. Let me know if you need anything else.',
      createdAt: '2027-01-07T09:15:00Z',
      mentions: []
    }
  ]
})

// Computed
const isOwner = computed(() => collection.value.author.id === currentUser.value.id)
const canEdit = computed(() => {
  if (isOwner.value) return true
  const userCollab = collection.value.collaborators.find(c => c.id === currentUser.value.id)
  return userCollab?.role === 'editor'
})

const sortedItems = computed(() => {
  return [...collection.value.items].sort((a, b) => a.order - b.order)
})

// Helper functions
function formatDate(dateString: string): string {
  const date = new Date(dateString)
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / (1000 * 60))
  const diffHours = Math.floor(diffMs / (1000 * 60 * 60))
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))

  if (diffMins < 1) return 'Just now'
  if (diffMins < 60) return `${diffMins}m ago`
  if (diffHours < 24) return `${diffHours}h ago`
  if (diffDays === 1) return 'Yesterday'
  if (diffDays < 7) return `${diffDays} days ago`
  return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
}

function formatFullDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    month: 'long',
    day: 'numeric',
    year: 'numeric',
    hour: 'numeric',
    minute: '2-digit'
  })
}

function getContentTypeIcon(type: string): string {
  switch (type) {
    case 'article': return 'fas fa-newspaper'
    case 'document': return 'fas fa-file-alt'
    case 'media': return 'fas fa-photo-video'
    default: return 'fas fa-file'
  }
}

function getContentTypeColor(type: string): string {
  switch (type) {
    case 'article': return 'bg-amber-100 text-amber-700'
    case 'document': return 'bg-blue-100 text-blue-700'
    case 'media': return 'bg-purple-100 text-purple-700'
    default: return 'bg-gray-100 text-gray-700'
  }
}

function getContentTypeLabel(type: string): string {
  switch (type) {
    case 'article': return 'Article'
    case 'document': return 'Document'
    case 'media': return 'Media'
    default: return 'Item'
  }
}

function getRoleColor(role: string): string {
  switch (role) {
    case 'owner': return 'bg-teal-100 text-teal-700'
    case 'editor': return 'bg-blue-100 text-blue-700'
    case 'viewer': return 'bg-gray-100 text-gray-600'
    default: return 'bg-gray-100 text-gray-600'
  }
}

function getRoleLabel(role: string): string {
  return role.charAt(0).toUpperCase() + role.slice(1)
}

// Actions
function startEditing() {
  editName.value = collection.value.name
  editDescription.value = collection.value.description
  isEditing.value = true
}

function cancelEditing() {
  isEditing.value = false
  editName.value = ''
  editDescription.value = ''
}

function saveEditing() {
  if (!editName.value.trim()) return
  collection.value.name = editName.value.trim()
  collection.value.description = editDescription.value.trim()
  collection.value.updatedAt = new Date().toISOString()
  isEditing.value = false
}

function toggleVisibility() {
  collection.value.visibility = collection.value.visibility === 'private' ? 'shared' : 'private'
  collection.value.updatedAt = new Date().toISOString()
}

function duplicateCollection() {
  alert('Collection duplicated! (In a real app, this would create a copy)')
}

function deleteCollection() {
  showDeleteConfirm.value = false
  router.push('/collections')
}

function removeItem(itemId: string) {
  const index = collection.value.items.findIndex(i => i.id === itemId)
  if (index > -1) {
    collection.value.items.splice(index, 1)
    collection.value.itemCount = collection.value.items.length
    collection.value.updatedAt = new Date().toISOString()
  }
}

function openItem(item: CollectionItem) {
  const routes: Record<string, string> = {
    article: '/articles/',
    document: '/documents/',
    media: '/media/'
  }
  router.push(routes[item.contentType] + item.contentId)
}

// Drag and drop
function handleDragStart(e: DragEvent, itemId: string) {
  draggedItem.value = itemId
  if (e.dataTransfer) {
    e.dataTransfer.effectAllowed = 'move'
  }
}

function handleDragOver(e: DragEvent) {
  e.preventDefault()
  if (e.dataTransfer) {
    e.dataTransfer.dropEffect = 'move'
  }
}

function handleDrop(e: DragEvent, targetItemId: string) {
  e.preventDefault()
  if (!draggedItem.value || draggedItem.value === targetItemId) return

  const items = collection.value.items
  const draggedIndex = items.findIndex(i => i.id === draggedItem.value)
  const targetIndex = items.findIndex(i => i.id === targetItemId)

  if (draggedIndex > -1 && targetIndex > -1) {
    const [draggedElement] = items.splice(draggedIndex, 1)
    items.splice(targetIndex, 0, draggedElement)

    // Update order values
    items.forEach((item, index) => {
      item.order = index + 1
    })

    collection.value.updatedAt = new Date().toISOString()
  }

  draggedItem.value = null
}

function handleDragEnd() {
  draggedItem.value = null
}

// Collaborators
function inviteCollaborator() {
  if (!inviteEmail.value.trim()) return

  const newCollaborator: Collaborator = {
    id: `user-${Date.now()}`,
    name: inviteEmail.value.split('@')[0],
    initials: inviteEmail.value.substring(0, 2).toUpperCase(),
    color: '#' + Math.floor(Math.random() * 16777215).toString(16),
    email: inviteEmail.value,
    role: inviteRole.value,
    addedAt: new Date().toISOString()
  }

  collection.value.collaborators.push(newCollaborator)
  inviteEmail.value = ''
  inviteRole.value = 'viewer'
  showCollaboratorModal.value = false
}

function removeCollaborator(collaboratorId: string) {
  const index = collection.value.collaborators.findIndex(c => c.id === collaboratorId)
  if (index > -1 && collection.value.collaborators[index].role !== 'owner') {
    collection.value.collaborators.splice(index, 1)
  }
}

function updateCollaboratorRole(collaboratorId: string, newRole: 'editor' | 'viewer') {
  const collaborator = collection.value.collaborators.find(c => c.id === collaboratorId)
  if (collaborator && collaborator.role !== 'owner') {
    collaborator.role = newRole
  }
}

// Comments
function addComment() {
  if (!newComment.value.trim()) return

  const comment: Comment = {
    id: `comment-${Date.now()}`,
    author: { ...currentUser.value },
    content: newComment.value.trim(),
    createdAt: new Date().toISOString(),
    mentions: extractMentions(newComment.value)
  }

  collection.value.comments.push(comment)
  newComment.value = ''
}

function extractMentions(text: string): string[] {
  const mentions: string[] = []
  const regex = /@(\w+)/g
  let match
  while ((match = regex.exec(text)) !== null) {
    const collaborator = collection.value.collaborators.find(
      c => c.name.toLowerCase().includes(match[1].toLowerCase())
    )
    if (collaborator) {
      mentions.push(collaborator.id)
    }
  }
  return mentions
}

function formatCommentContent(content: string): string {
  return content.replace(/@(\w+\s?\w*)/g, '<span class="mention">@$1</span>')
}

function goBack() {
  router.push('/collections')
}

onMounted(() => {
  // In real app, fetch collection by route.params.id
  console.log('Collection ID:', route.params.id)
})
</script>

<template>
  <div class="collection-detail-page">
    <!-- Header -->
    <div class="page-header">
      <div class="header-content">
        <!-- Back Button -->
        <button @click="goBack" class="back-btn">
          <i class="fas fa-arrow-left"></i>
        </button>

        <!-- Collection Info -->
        <div class="collection-info">
          <div v-if="!isEditing" class="info-display">
            <div class="title-row">
              <h1 class="collection-title">{{ collection.name }}</h1>
              <div :class="['visibility-badge', collection.visibility]">
                <i :class="collection.visibility === 'private' ? 'fas fa-lock' : 'fas fa-globe'"></i>
                <span>{{ collection.visibility === 'private' ? 'Private' : 'Shared' }}</span>
              </div>
            </div>
            <p class="collection-description">{{ collection.description }}</p>
          </div>
          <div v-else class="info-edit">
            <input
              v-model="editName"
              type="text"
              class="edit-title-input"
              placeholder="Collection name"
              autofocus
            />
            <textarea
              v-model="editDescription"
              class="edit-description-input"
              rows="2"
              placeholder="Add a description..."
            ></textarea>
          </div>
        </div>

        <!-- Actions -->
        <div class="header-actions">
          <template v-if="!isEditing">
            <button v-if="canEdit" @click="startEditing" class="action-btn">
              <i class="fas fa-pen"></i>
              <span>Edit</span>
            </button>
            <button @click="showShareModal = true" class="action-btn">
              <i class="fas fa-share-alt"></i>
              <span>Share</span>
            </button>
            <button @click="duplicateCollection" class="action-btn">
              <i class="fas fa-copy"></i>
              <span>Duplicate</span>
            </button>
            <button v-if="canEdit" @click="toggleVisibility" class="action-btn">
              <i :class="collection.visibility === 'private' ? 'fas fa-globe' : 'fas fa-lock'"></i>
              <span>{{ collection.visibility === 'private' ? 'Make Shared' : 'Make Private' }}</span>
            </button>
            <button v-if="isOwner" @click="showDeleteConfirm = true" class="action-btn danger">
              <i class="fas fa-trash-alt"></i>
              <span>Delete</span>
            </button>
          </template>
          <template v-else>
            <button @click="cancelEditing" class="action-btn">
              <i class="fas fa-times"></i>
              <span>Cancel</span>
            </button>
            <button @click="saveEditing" class="action-btn primary" :disabled="!editName.trim()">
              <i class="fas fa-check"></i>
              <span>Save</span>
            </button>
          </template>
        </div>
      </div>

      <!-- Meta Info -->
      <div class="header-meta">
        <div class="meta-item">
          <div class="author-avatar" :style="{ backgroundColor: collection.author.color }">
            {{ collection.author.initials }}
          </div>
          <span>Created by {{ collection.author.name }}</span>
        </div>
        <div class="meta-item">
          <i class="fas fa-calendar-alt"></i>
          <span>Created {{ formatDate(collection.createdAt) }}</span>
        </div>
        <div class="meta-item">
          <i class="fas fa-clock"></i>
          <span>Updated {{ formatDate(collection.updatedAt) }}</span>
        </div>
        <div class="meta-item">
          <i class="fas fa-file-alt"></i>
          <span>{{ collection.itemCount }} items</span>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="main-content">
      <!-- Items Section -->
      <div class="items-section">
        <div class="section-header">
          <h2 class="section-title">
            <i class="fas fa-layer-group"></i>
            Collection Items
          </h2>
          <span class="item-count">{{ sortedItems.length }} items</span>
        </div>

        <div v-if="sortedItems.length > 0" class="items-grid">
          <div
            v-for="item in sortedItems"
            :key="item.id"
            class="item-card"
            :class="{ 'dragging': draggedItem === item.id }"
            :draggable="canEdit"
            @dragstart="handleDragStart($event, item.id)"
            @dragover="handleDragOver"
            @drop="handleDrop($event, item.id)"
            @dragend="handleDragEnd"
            @click="openItem(item)"
          >
            <!-- Drag Handle -->
            <div v-if="canEdit" class="drag-handle">
              <i class="fas fa-grip-vertical"></i>
            </div>

            <!-- Thumbnail -->
            <div class="item-thumbnail">
              <img v-if="item.thumbnail" :src="item.thumbnail" :alt="item.title" />
              <div v-else class="thumbnail-placeholder">
                <i :class="getContentTypeIcon(item.contentType)"></i>
              </div>
              <div :class="['type-badge', getContentTypeColor(item.contentType)]">
                <i :class="getContentTypeIcon(item.contentType)"></i>
                <span>{{ getContentTypeLabel(item.contentType) }}</span>
              </div>
            </div>

            <!-- Content -->
            <div class="item-content">
              <h3 class="item-title">{{ item.title }}</h3>
              <p class="item-description">{{ item.description }}</p>
              <div class="item-meta">
                <span v-if="item.metadata?.duration" class="meta-tag">
                  <i class="fas fa-clock"></i>
                  {{ item.metadata.duration }}
                </span>
                <span v-if="item.metadata?.views" class="meta-tag">
                  <i class="fas fa-eye"></i>
                  {{ item.metadata.views }}
                </span>
                <span v-if="item.metadata?.readTime" class="meta-tag">
                  <i class="fas fa-book-open"></i>
                  {{ item.metadata.readTime }}
                </span>
                <span v-if="item.metadata?.size" class="meta-tag">
                  <i class="fas fa-file"></i>
                  {{ item.metadata.size }}
                </span>
                <span v-if="item.metadata?.photoCount" class="meta-tag">
                  <i class="fas fa-images"></i>
                  {{ item.metadata.photoCount }} photos
                </span>
              </div>
            </div>

            <!-- Remove Button -->
            <button
              v-if="canEdit"
              @click.stop="removeItem(item.id)"
              class="remove-btn"
              title="Remove from collection"
            >
              <i class="fas fa-times"></i>
            </button>
          </div>
        </div>

        <div v-else class="empty-items">
          <div class="empty-icon">
            <i class="fas fa-layer-group"></i>
          </div>
          <h3>No items yet</h3>
          <p>Add articles, documents, and media to this collection from their respective pages.</p>
        </div>
      </div>

      <!-- Sidebar -->
      <div class="sidebar">
        <!-- Collaborators Panel -->
        <div class="panel collaborators-panel">
          <div class="panel-header">
            <h3 class="panel-title">
              <i class="fas fa-users"></i>
              Collaborators
            </h3>
            <button v-if="isOwner" @click="showCollaboratorModal = true" class="add-btn">
              <i class="fas fa-plus"></i>
            </button>
          </div>
          <div class="collaborators-list">
            <div
              v-for="collaborator in collection.collaborators"
              :key="collaborator.id"
              class="collaborator-item"
            >
              <div class="collaborator-avatar" :style="{ backgroundColor: collaborator.color }">
                {{ collaborator.initials }}
              </div>
              <div class="collaborator-info">
                <span class="collaborator-name">{{ collaborator.name }}</span>
                <span class="collaborator-email">{{ collaborator.email }}</span>
              </div>
              <div class="collaborator-actions">
                <span :class="['role-badge', getRoleColor(collaborator.role)]">
                  {{ getRoleLabel(collaborator.role) }}
                </span>
                <button
                  v-if="isOwner && collaborator.role !== 'owner'"
                  @click="removeCollaborator(collaborator.id)"
                  class="remove-collaborator-btn"
                  title="Remove collaborator"
                >
                  <i class="fas fa-times"></i>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Comments Panel -->
        <div class="panel comments-panel">
          <div class="panel-header">
            <h3 class="panel-title">
              <i class="fas fa-comments"></i>
              Comments
            </h3>
            <span class="comment-count">{{ collection.comments.length }}</span>
          </div>
          <div class="comments-list">
            <div
              v-for="comment in collection.comments"
              :key="comment.id"
              class="comment-item"
            >
              <div class="comment-avatar" :style="{ backgroundColor: comment.author.color }">
                {{ comment.author.initials }}
              </div>
              <div class="comment-content">
                <div class="comment-header">
                  <span class="comment-author">{{ comment.author.name }}</span>
                  <span class="comment-time">{{ formatDate(comment.createdAt) }}</span>
                </div>
                <p class="comment-text" v-html="formatCommentContent(comment.content)"></p>
              </div>
            </div>
          </div>
          <div class="comment-input-wrapper">
            <div class="comment-input-avatar" :style="{ backgroundColor: currentUser.color }">
              {{ currentUser.initials }}
            </div>
            <div class="comment-input-container">
              <textarea
                v-model="newComment"
                class="comment-input"
                placeholder="Add a comment... Use @name to mention"
                rows="2"
                @keydown.enter.ctrl="addComment"
              ></textarea>
              <button
                @click="addComment"
                class="send-comment-btn"
                :disabled="!newComment.trim()"
              >
                <i class="fas fa-paper-plane"></i>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Invite Collaborator Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="showCollaboratorModal" class="modal-overlay" @click.self="showCollaboratorModal = false">
          <div class="modal-content">
            <div class="modal-header">
              <h3 class="modal-title">
                <i class="fas fa-user-plus text-teal-500 mr-2"></i>
                Invite Collaborator
              </h3>
              <button @click="showCollaboratorModal = false" class="modal-close">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <div class="modal-body">
              <div class="form-group">
                <label class="form-label">Email Address</label>
                <input
                  v-model="inviteEmail"
                  type="email"
                  class="form-input"
                  placeholder="colleague@company.com"
                  autofocus
                />
              </div>
              <div class="form-group">
                <label class="form-label">Role</label>
                <div class="role-options">
                  <button
                    @click="inviteRole = 'editor'"
                    :class="['role-option', inviteRole === 'editor' ? 'active' : '']"
                  >
                    <i class="fas fa-pen"></i>
                    <span>Editor</span>
                    <small>Can add, remove, and reorder items</small>
                  </button>
                  <button
                    @click="inviteRole = 'viewer'"
                    :class="['role-option', inviteRole === 'viewer' ? 'active' : '']"
                  >
                    <i class="fas fa-eye"></i>
                    <span>Viewer</span>
                    <small>Can only view the collection</small>
                  </button>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button @click="showCollaboratorModal = false" class="btn-cancel">
                Cancel
              </button>
              <button
                @click="inviteCollaborator"
                class="btn-save"
                :disabled="!inviteEmail.trim()"
              >
                <i class="fas fa-paper-plane mr-2"></i>
                Send Invitation
              </button>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>

    <!-- Delete Confirmation Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="showDeleteConfirm" class="modal-overlay" @click.self="showDeleteConfirm = false">
          <div class="modal-content modal-sm">
            <div class="modal-header">
              <h3 class="modal-title">
                <i class="fas fa-exclamation-triangle text-red-500 mr-2"></i>
                Delete Collection?
              </h3>
              <button @click="showDeleteConfirm = false" class="modal-close">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <div class="modal-body">
              <p class="text-gray-600">
                Are you sure you want to delete "<strong>{{ collection.name }}</strong>"?
                This action cannot be undone. The items will remain in their original locations.
              </p>
            </div>
            <div class="modal-footer">
              <button @click="showDeleteConfirm = false" class="btn-cancel">
                Cancel
              </button>
              <button @click="deleteCollection" class="btn-delete">
                <i class="fas fa-trash-alt mr-2"></i>
                Delete Collection
              </button>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<style scoped>
.collection-detail-page {
  min-height: 100vh;
  background: #f8fafc;
}

/* Header */
.page-header {
  background: white;
  border-bottom: 1px solid #e5e7eb;
  padding: 1.5rem 2rem;
}

.header-content {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  margin-bottom: 1rem;
}

.back-btn {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f1f5f9;
  border: none;
  border-radius: 10px;
  color: #64748b;
  cursor: pointer;
  transition: all 0.15s;
  flex-shrink: 0;
}

.back-btn:hover {
  background: #e5e7eb;
  color: #1e293b;
}

.collection-info {
  flex: 1;
  min-width: 0;
}

.title-row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.375rem;
}

.collection-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0;
}

.collection-description {
  font-size: 0.9375rem;
  color: #64748b;
  margin: 0;
}

.visibility-badge {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.25rem 0.625rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 500;
}

.visibility-badge.private {
  background: #1e293b;
  color: white;
}

.visibility-badge.shared {
  background: #14b8a6;
  color: white;
}

/* Edit Mode */
.edit-title-input {
  width: 100%;
  padding: 0.5rem 0.75rem;
  border: 2px solid #14b8a6;
  border-radius: 8px;
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.edit-title-input:focus {
  outline: none;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.edit-description-input {
  width: 100%;
  padding: 0.5rem 0.75rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 0.9375rem;
  resize: none;
}

.edit-description-input:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

/* Header Actions */
.header-actions {
  display: flex;
  gap: 0.5rem;
  flex-shrink: 0;
}

.action-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: #f1f5f9;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #475569;
  cursor: pointer;
  transition: all 0.15s;
}

.action-btn:hover {
  background: #e5e7eb;
  border-color: #cbd5e1;
}

.action-btn.primary {
  background: #14b8a6;
  border-color: #14b8a6;
  color: white;
}

.action-btn.primary:hover:not(:disabled) {
  background: #0d9488;
}

.action-btn.primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.action-btn.danger {
  color: #dc2626;
}

.action-btn.danger:hover {
  background: #fef2f2;
  border-color: #fecaca;
}

/* Header Meta */
.header-meta {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  flex-wrap: wrap;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.8125rem;
  color: #64748b;
}

.meta-item i {
  color: #94a3b8;
}

.author-avatar {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.625rem;
  font-weight: 600;
  color: white;
}

/* Main Content */
.main-content {
  display: flex;
  gap: 1.5rem;
  padding: 1.5rem 2rem;
  max-width: 1600px;
  margin: 0 auto;
}

/* Items Section */
.items-section {
  flex: 1;
  min-width: 0;
}

.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.section-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 1.125rem;
  font-weight: 600;
  color: #1e293b;
}

.section-title i {
  color: #14b8a6;
}

.item-count {
  font-size: 0.8125rem;
  color: #94a3b8;
}

/* Items Grid */
.items-grid {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.item-card {
  display: flex;
  gap: 1rem;
  padding: 1rem;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s;
  position: relative;
}

.item-card:hover {
  border-color: #99f6e4;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}

.item-card.dragging {
  opacity: 0.5;
  border-color: #14b8a6;
}

.drag-handle {
  display: flex;
  align-items: center;
  padding: 0 0.25rem;
  color: #cbd5e1;
  cursor: grab;
}

.drag-handle:active {
  cursor: grabbing;
}

.item-thumbnail {
  width: 140px;
  height: 90px;
  border-radius: 8px;
  overflow: hidden;
  flex-shrink: 0;
  position: relative;
}

.item-thumbnail img {
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
  font-size: 1.5rem;
}

.type-badge {
  position: absolute;
  bottom: 0.375rem;
  left: 0.375rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
  font-size: 0.625rem;
  font-weight: 600;
}

.item-content {
  flex: 1;
  min-width: 0;
}

.item-title {
  font-size: 0.9375rem;
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 0.25rem;
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.item-description {
  font-size: 0.8125rem;
  color: #64748b;
  margin-bottom: 0.5rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.item-meta {
  display: flex;
  gap: 0.75rem;
}

.meta-tag {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.75rem;
  color: #94a3b8;
}

.remove-btn {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #fef2f2;
  border: none;
  border-radius: 50%;
  color: #dc2626;
  cursor: pointer;
  opacity: 0;
  transition: all 0.15s;
}

.item-card:hover .remove-btn {
  opacity: 1;
}

.remove-btn:hover {
  background: #dc2626;
  color: white;
}

/* Empty Items */
.empty-items {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  background: white;
  border: 2px dashed #e5e7eb;
  border-radius: 12px;
  text-align: center;
}

.empty-icon {
  width: 64px;
  height: 64px;
  border-radius: 50%;
  background: #f1f5f9;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  color: #94a3b8;
  margin-bottom: 1rem;
}

.empty-items h3 {
  font-size: 1rem;
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 0.25rem;
}

.empty-items p {
  font-size: 0.875rem;
  color: #64748b;
  max-width: 300px;
}

/* Sidebar */
.sidebar {
  width: 360px;
  flex-shrink: 0;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.panel {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  overflow: hidden;
}

.panel-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem;
  border-bottom: 1px solid #f1f5f9;
}

.panel-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9375rem;
  font-weight: 600;
  color: #1e293b;
}

.panel-title i {
  color: #14b8a6;
}

.add-btn {
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f0fdfa;
  border: 1px solid #99f6e4;
  border-radius: 50%;
  color: #0f766e;
  cursor: pointer;
  transition: all 0.15s;
}

.add-btn:hover {
  background: #14b8a6;
  border-color: #14b8a6;
  color: white;
}

.comment-count {
  background: #f1f5f9;
  padding: 0.125rem 0.5rem;
  border-radius: 10px;
  font-size: 0.75rem;
  font-weight: 500;
  color: #64748b;
}

/* Collaborators List */
.collaborators-list {
  padding: 0.5rem;
}

.collaborator-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.625rem 0.5rem;
  border-radius: 8px;
  transition: background 0.15s;
}

.collaborator-item:hover {
  background: #f8fafc;
}

.collaborator-avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: 600;
  color: white;
  flex-shrink: 0;
}

.collaborator-info {
  flex: 1;
  min-width: 0;
}

.collaborator-name {
  display: block;
  font-size: 0.875rem;
  font-weight: 500;
  color: #1e293b;
}

.collaborator-email {
  display: block;
  font-size: 0.75rem;
  color: #94a3b8;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.collaborator-actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.role-badge {
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
  font-size: 0.6875rem;
  font-weight: 600;
  text-transform: uppercase;
}

.remove-collaborator-btn {
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: none;
  border-radius: 50%;
  color: #94a3b8;
  cursor: pointer;
  opacity: 0;
  transition: all 0.15s;
}

.collaborator-item:hover .remove-collaborator-btn {
  opacity: 1;
}

.remove-collaborator-btn:hover {
  background: #fef2f2;
  color: #dc2626;
}

/* Comments Panel */
.comments-panel {
  display: flex;
  flex-direction: column;
  max-height: 500px;
}

.comments-list {
  flex: 1;
  overflow-y: auto;
  padding: 0.75rem;
}

.comment-item {
  display: flex;
  gap: 0.625rem;
  padding: 0.5rem 0;
}

.comment-item:not(:last-child) {
  border-bottom: 1px solid #f1f5f9;
  padding-bottom: 0.75rem;
  margin-bottom: 0.5rem;
}

.comment-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.6875rem;
  font-weight: 600;
  color: white;
  flex-shrink: 0;
}

.comment-content {
  flex: 1;
  min-width: 0;
}

.comment-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.25rem;
}

.comment-author {
  font-size: 0.8125rem;
  font-weight: 600;
  color: #1e293b;
}

.comment-time {
  font-size: 0.6875rem;
  color: #94a3b8;
}

.comment-text {
  font-size: 0.8125rem;
  color: #475569;
  line-height: 1.5;
  margin: 0;
}

.comment-text :deep(.mention) {
  color: #0d9488;
  font-weight: 500;
}

.comment-input-wrapper {
  display: flex;
  gap: 0.625rem;
  padding: 0.75rem;
  border-top: 1px solid #f1f5f9;
  background: #f8fafc;
}

.comment-input-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.6875rem;
  font-weight: 600;
  color: white;
  flex-shrink: 0;
}

.comment-input-container {
  flex: 1;
  position: relative;
}

.comment-input {
  width: 100%;
  padding: 0.5rem 2.5rem 0.5rem 0.75rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 0.8125rem;
  resize: none;
  background: white;
}

.comment-input:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.send-comment-btn {
  position: absolute;
  right: 0.5rem;
  bottom: 0.5rem;
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #14b8a6;
  border: none;
  border-radius: 50%;
  color: white;
  cursor: pointer;
  transition: all 0.15s;
}

.send-comment-btn:hover:not(:disabled) {
  background: #0d9488;
  transform: scale(1.05);
}

.send-comment-btn:disabled {
  background: #e5e7eb;
  cursor: not-allowed;
}

/* Modal Styles */
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
  max-width: 440px;
  max-height: 90vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25);
}

.modal-content.modal-sm {
  max-width: 380px;
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

.modal-body {
  padding: 1.5rem;
  overflow-y: auto;
}

.form-group {
  margin-bottom: 1.25rem;
}

.form-group:last-child {
  margin-bottom: 0;
}

.form-label {
  display: block;
  font-size: 0.875rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.5rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  font-size: 0.9375rem;
  transition: all 0.2s;
}

.form-input:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.role-options {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.role-option {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 0.25rem;
  padding: 1rem;
  background: #f8fafc;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s;
  text-align: left;
}

.role-option:hover {
  border-color: #cbd5e1;
}

.role-option.active {
  background: #f0fdfa;
  border-color: #14b8a6;
}

.role-option i {
  font-size: 1.25rem;
  color: #64748b;
  margin-bottom: 0.25rem;
}

.role-option.active i {
  color: #0f766e;
}

.role-option span {
  font-size: 0.9375rem;
  font-weight: 600;
  color: #374151;
}

.role-option small {
  font-size: 0.75rem;
  color: #94a3b8;
}

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

.btn-save {
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

.btn-save:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.btn-save:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-delete {
  display: flex;
  align-items: center;
  padding: 0.625rem 1.25rem;
  background: #dc2626;
  border: none;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  color: white;
  cursor: pointer;
  transition: all 0.15s;
}

.btn-delete:hover {
  background: #b91c1c;
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

/* Responsive */
@media (max-width: 1024px) {
  .main-content {
    flex-direction: column;
  }

  .sidebar {
    width: 100%;
  }
}

@media (max-width: 768px) {
  .header-content {
    flex-direction: column;
    align-items: flex-start;
  }

  .header-actions {
    width: 100%;
    flex-wrap: wrap;
  }

  .action-btn span {
    display: none;
  }

  .item-thumbnail {
    width: 100px;
    height: 70px;
  }
}
</style>
