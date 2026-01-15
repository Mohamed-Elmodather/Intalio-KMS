<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { BookmarkButton, SocialShareButtons } from '@/components/common'

const route = useRoute()
const router = useRouter()

// Text constants for i18n
const textConstants = {
  // Header
  backToCollections: 'Back to Collections',
  editCollection: 'Edit',
  download: 'Download',
  share: 'Share',
  collection: 'Collection',
  save: 'Save',
  duplicate: 'Duplicate',
  delete: 'Delete',
  cancel: 'Cancel',
  saveChanges: 'Save',

  // Visibility
  private: 'Private',
  shared: 'Shared',
  makeShared: 'Make Shared',
  makePrivate: 'Make Private',

  // Stats
  items: 'items',
  collaborators: 'collaborators',
  comments: 'comments',
  lastUpdated: 'Last updated',

  // Views
  gridView: 'Grid View',
  listView: 'List View',

  // Sorting
  sortBy: 'Sort by',
  sortRecent: 'Recently Added',
  sortOldest: 'Oldest First',
  sortAZ: 'A-Z',
  sortZA: 'Z-A',
  sortType: 'By Type',

  // Filter
  filterBy: 'Filter',
  allTypes: 'All Types',
  articles: 'Articles',
  documents: 'Documents',
  media: 'Media',

  // Content types
  article: 'Article',
  document: 'Document',
  mediaItem: 'Media',
  item: 'Item',

  // Actions
  removeFromCollection: 'Remove from collection',
  openItem: 'Open',
  dragToReorder: 'Drag to reorder',

  // Export
  exportCollection: 'Export Collection',
  exportAsZip: 'Download as ZIP',
  exportAsPdf: 'Export as PDF',
  exportAsJson: 'Export metadata (JSON)',
  copyLinks: 'Copy all links',

  // Activity
  activity: 'Activity',
  recentActivity: 'Recent Activity',
  addedItem: 'added',
  removedItem: 'removed',
  reorderedItems: 'reordered items',
  updatedCollection: 'updated collection',
  invitedCollaborator: 'invited',

  // Empty state
  noItems: 'No items yet',
  noItemsDesc: 'Add articles, documents, and media to this collection from their respective pages.',

  // Collaborators
  inviteCollaborator: 'Invite Collaborator',
  emailAddress: 'Email Address',
  role: 'Role',
  editor: 'Editor',
  editorDesc: 'Can add, remove, and reorder items',
  viewer: 'Viewer',
  viewerDesc: 'Can only view the collection',
  owner: 'Owner',
  sendInvitation: 'Send Invitation',
  removeCollaborator: 'Remove collaborator',

  // Delete
  deleteCollection: 'Delete Collection?',
  deleteWarning: 'Are you sure you want to delete',
  deleteNote: 'This action cannot be undone. The items will remain in their original locations.',

  // Misc
  createdBy: 'Created by',
  created: 'Created',
  updated: 'Updated',
  collectionItems: 'Collection Items',
  addComment: 'Add a comment... Use @name to mention'
}

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

// Activity interface
interface Activity {
  id: string
  type: 'added' | 'removed' | 'reordered' | 'updated' | 'invited' | 'commented'
  user: { id: string; name: string; initials: string; color: string }
  target?: string
  timestamp: string
}

// UI State
const isEditing = ref(false)
const showCollaboratorModal = ref(false)
const showDeleteConfirm = ref(false)
const showShareModal = ref(false)
const showExportMenu = ref(false)
const newComment = ref('')
const inviteEmail = ref('')
const inviteRole = ref<'editor' | 'viewer'>('viewer')
const draggedItem = ref<string | null>(null)

// View and filter state
const viewMode = ref<'list' | 'grid'>('list')
const sortBy = ref<'recent' | 'oldest' | 'az' | 'za' | 'type'>('recent')
const filterType = ref<'all' | 'article' | 'document' | 'media'>('all')

// Activity log (mock data)
const activities = ref<Activity[]>([
  {
    id: 'act-1',
    type: 'added',
    user: { id: 'user-2', name: 'Sarah Chen', initials: 'SC', color: '#8b5cf6' },
    target: 'Fan Zone Highlights: Opening Weekend',
    timestamp: '2027-01-07T09:00:00Z'
  },
  {
    id: 'act-2',
    type: 'commented',
    user: { id: 'user-2', name: 'Sarah Chen', initials: 'SC', color: '#8b5cf6' },
    target: 'Added the fan zone gallery',
    timestamp: '2027-01-07T09:15:00Z'
  },
  {
    id: 'act-3',
    type: 'invited',
    user: { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6' },
    target: 'Mohammed Al-Rashid',
    timestamp: '2027-01-06T09:00:00Z'
  },
  {
    id: 'act-4',
    type: 'added',
    user: { id: 'user-2', name: 'Sarah Chen', initials: 'SC', color: '#8b5cf6' },
    target: 'King Fahd Stadium: Behind the Scenes Tour',
    timestamp: '2027-01-06T14:00:00Z'
  },
  {
    id: 'act-5',
    type: 'updated',
    user: { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6' },
    target: 'collection description',
    timestamp: '2027-01-05T15:00:00Z'
  }
])

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

// Filtered and sorted items
const filteredAndSortedItems = computed(() => {
  let items = [...collection.value.items]

  // Apply filter
  if (filterType.value !== 'all') {
    items = items.filter(item => item.contentType === filterType.value)
  }

  // Apply sort
  switch (sortBy.value) {
    case 'recent':
      items.sort((a, b) => new Date(b.addedAt).getTime() - new Date(a.addedAt).getTime())
      break
    case 'oldest':
      items.sort((a, b) => new Date(a.addedAt).getTime() - new Date(b.addedAt).getTime())
      break
    case 'az':
      items.sort((a, b) => a.title.localeCompare(b.title))
      break
    case 'za':
      items.sort((a, b) => b.title.localeCompare(a.title))
      break
    case 'type':
      items.sort((a, b) => a.contentType.localeCompare(b.contentType))
      break
    default:
      items.sort((a, b) => a.order - b.order)
  }

  return items
})

// Keep sortedItems for backward compatibility
const sortedItems = computed(() => {
  return [...collection.value.items].sort((a, b) => a.order - b.order)
})

// Item counts by type
const itemCounts = computed(() => ({
  all: collection.value.items.length,
  article: collection.value.items.filter(i => i.contentType === 'article').length,
  document: collection.value.items.filter(i => i.contentType === 'document').length,
  media: collection.value.items.filter(i => i.contentType === 'media').length
}))

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
  let match: RegExpExecArray | null
  while ((match = regex.exec(text)) !== null) {
    const matchedName = match[1]
    const collaborator = collection.value.collaborators.find(
      c => c.name.toLowerCase().includes(matchedName.toLowerCase())
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

// Export functions
function exportAsJson() {
  const exportData = {
    collection: {
      id: collection.value.id,
      name: collection.value.name,
      description: collection.value.description,
      createdAt: collection.value.createdAt,
      updatedAt: collection.value.updatedAt,
      visibility: collection.value.visibility,
      author: collection.value.author.name
    },
    items: collection.value.items.map(item => ({
      id: item.id,
      title: item.title,
      type: item.contentType,
      addedAt: item.addedAt
    })),
    collaborators: collection.value.collaborators.map(c => ({
      name: c.name,
      role: c.role
    }))
  }

  const blob = new Blob([JSON.stringify(exportData, null, 2)], { type: 'application/json' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `${collection.value.name.replace(/\s+/g, '-').toLowerCase()}-metadata.json`
  document.body.appendChild(a)
  a.click()
  document.body.removeChild(a)
  URL.revokeObjectURL(url)
  showExportMenu.value = false
}

function copyAllLinks() {
  const baseUrl = window.location.origin
  const routes: Record<string, string> = {
    article: '/articles/',
    document: '/documents/',
    media: '/media/'
  }

  const links = collection.value.items.map(item => {
    return `${item.title}: ${baseUrl}${routes[item.contentType]}${item.contentId}`
  }).join('\n')

  navigator.clipboard.writeText(links)
  alert('All item links copied to clipboard!')
  showExportMenu.value = false
}

function exportAsPdf() {
  // In production, this would generate a PDF
  alert('PDF export would be generated here. This feature requires a PDF generation library.')
  showExportMenu.value = false
}

function downloadAsZip() {
  // In production, this would trigger a server-side ZIP generation
  alert('ZIP download would be triggered here. This feature requires server-side support.')
  showExportMenu.value = false
}

// Share function using Web Share API
function shareCollection() {
  const shareData = {
    title: collection.value.name,
    text: collection.value.description,
    url: window.location.href
  }

  if (navigator.share) {
    navigator.share(shareData).catch(() => {
      // Fallback to clipboard
      navigator.clipboard.writeText(window.location.href)
      alert('Link copied to clipboard!')
    })
  } else {
    navigator.clipboard.writeText(window.location.href)
    alert('Link copied to clipboard!')
  }
}

// Activity helpers
function getActivityIcon(type: Activity['type']): string {
  switch (type) {
    case 'added': return 'fas fa-plus-circle'
    case 'removed': return 'fas fa-minus-circle'
    case 'reordered': return 'fas fa-arrows-alt'
    case 'updated': return 'fas fa-edit'
    case 'invited': return 'fas fa-user-plus'
    case 'commented': return 'fas fa-comment'
    default: return 'fas fa-circle'
  }
}

function getActivityColor(type: Activity['type']): string {
  switch (type) {
    case 'added': return 'text-green-500'
    case 'removed': return 'text-red-500'
    case 'reordered': return 'text-blue-500'
    case 'updated': return 'text-amber-500'
    case 'invited': return 'text-purple-500'
    case 'commented': return 'text-teal-500'
    default: return 'text-gray-500'
  }
}

function getActivityText(activity: Activity): string {
  switch (activity.type) {
    case 'added': return `added "${activity.target}"`
    case 'removed': return `removed "${activity.target}"`
    case 'reordered': return 'reordered collection items'
    case 'updated': return `updated ${activity.target}`
    case 'invited': return `invited ${activity.target}`
    case 'commented': return `commented: "${activity.target}"`
    default: return 'performed an action'
  }
}

// Close export menu on outside click
function handleOutsideClick(e: MouseEvent) {
  const target = e.target as HTMLElement
  if (!target.closest('.export-menu-container')) {
    showExportMenu.value = false
  }
}

onMounted(() => {
  // In real app, fetch collection by route.params.id
  console.log('Collection ID:', route.params.id)
  document.addEventListener('click', handleOutsideClick)
})

onUnmounted(() => {
  document.removeEventListener('click', handleOutsideClick)
})
</script>

<template>
  <div class="collection-detail-page">
    <!-- Hero Header with Cover Image -->
    <div class="hero-header">
      <!-- Cover Image Background -->
      <div class="hero-bg">
        <img
          v-if="collection.thumbnail"
          :src="collection.thumbnail"
          alt=""
          class="hero-bg-image"
        />
        <div class="hero-overlay"></div>
      </div>

      <!-- Header Content -->
      <div class="hero-content">
        <!-- Top Navigation -->
        <div class="hero-nav">
          <button @click="goBack" class="back-btn-hero">
            <i class="fas fa-arrow-left"></i>
            <span>{{ textConstants.backToCollections }}</span>
          </button>

          <!-- Edit Mode Controls -->
          <div v-if="isEditing" class="edit-controls">
            <button @click="cancelEditing" class="px-4 py-2 bg-white/10 text-white border border-white/30 rounded-xl font-medium hover:bg-white/20 transition-all flex items-center gap-2">
              <i class="fas fa-times"></i>
              <span class="hidden sm:inline">{{ textConstants.cancel }}</span>
            </button>
            <button @click="saveEditing" :disabled="!editName.trim()" class="px-4 py-2 bg-teal-500 text-white rounded-xl font-medium hover:bg-teal-600 transition-all flex items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed">
              <i class="fas fa-check"></i>
              <span class="hidden sm:inline">{{ textConstants.saveChanges }}</span>
            </button>
          </div>
        </div>

        <!-- Collection Info -->
        <div class="hero-info">
          <div v-if="!isEditing" class="info-display">
            <div class="flex items-center gap-3 mb-2">
              <span :class="['visibility-badge-hero', collection.visibility]">
                <i :class="collection.visibility === 'private' ? 'fas fa-lock' : 'fas fa-globe'"></i>
                {{ collection.visibility === 'private' ? textConstants.private : textConstants.shared }}
              </span>
            </div>
            <h1 class="hero-title">{{ collection.name }}</h1>
            <p class="hero-description">{{ collection.description }}</p>
          </div>
          <div v-else class="info-edit-hero">
            <input
              v-model="editName"
              type="text"
              class="edit-title-hero"
              placeholder="Collection name"
              autofocus
            />
            <textarea
              v-model="editDescription"
              class="edit-description-hero"
              rows="2"
              placeholder="Add a description..."
            ></textarea>
          </div>
        </div>

        <!-- Meta & Stats Row -->
        <div class="hero-meta">
          <div class="meta-author">
            <div class="author-avatar-hero" :style="{ backgroundColor: collection.author.color }">
              {{ collection.author.initials }}
            </div>
            <div class="author-info">
              <span class="author-name">{{ collection.author.name }}</span>
              <span class="author-role">{{ textConstants.owner }}</span>
            </div>
          </div>
          <div class="meta-stats">
            <div class="stat-item">
              <i class="fas fa-layer-group"></i>
              <span>{{ collection.itemCount }} {{ textConstants.items }}</span>
            </div>
            <div class="stat-item">
              <i class="fas fa-users"></i>
              <span>{{ collection.collaborators.length }} {{ textConstants.collaborators }}</span>
            </div>
            <div class="stat-item">
              <i class="fas fa-comments"></i>
              <span>{{ collection.comments.length }} {{ textConstants.comments }}</span>
            </div>
            <div class="stat-item">
              <i class="fas fa-clock"></i>
              <span>{{ textConstants.lastUpdated }} {{ formatDate(collection.updatedAt) }}</span>
            </div>
          </div>
        </div>

        <!-- Action Buttons (Unified Style) -->
        <div v-if="!isEditing" class="hero-actions">
          <!-- Export/Download Dropdown -->
          <div class="export-menu-container relative">
            <button @click.stop="showExportMenu = !showExportMenu" class="px-4 py-2 bg-white/10 text-white border border-white/30 rounded-xl font-medium hover:bg-white/20 transition-all flex items-center gap-2">
              <i class="fas fa-download"></i>
              <span class="hidden sm:inline">{{ textConstants.download }}</span>
              <i class="fas fa-chevron-down text-xs ml-1"></i>
            </button>
            <Transition name="dropdown">
              <div v-if="showExportMenu" class="export-dropdown">
                <button @click="downloadAsZip" class="dropdown-item">
                  <i class="fas fa-file-archive"></i>
                  {{ textConstants.exportAsZip }}
                </button>
                <button @click="exportAsPdf" class="dropdown-item">
                  <i class="fas fa-file-pdf"></i>
                  {{ textConstants.exportAsPdf }}
                </button>
                <button @click="exportAsJson" class="dropdown-item">
                  <i class="fas fa-code"></i>
                  {{ textConstants.exportAsJson }}
                </button>
                <div class="dropdown-divider"></div>
                <button @click="copyAllLinks" class="dropdown-item">
                  <i class="fas fa-link"></i>
                  {{ textConstants.copyLinks }}
                </button>
              </div>
            </Transition>
          </div>

          <button @click="shareCollection" class="px-4 py-2 bg-white/10 text-white border border-white/30 rounded-xl font-medium hover:bg-white/20 transition-all flex items-center gap-2">
            <i class="fas fa-share-alt"></i>
            <span class="hidden sm:inline">{{ textConstants.share }}</span>
          </button>

          <button @click="duplicateCollection" class="px-4 py-2 bg-white/10 text-white border border-white/30 rounded-xl font-medium hover:bg-white/20 transition-all flex items-center gap-2">
            <i class="fas fa-copy"></i>
            <span class="hidden sm:inline">{{ textConstants.duplicate }}</span>
          </button>

          <BookmarkButton
            content-type="collection"
            :content-id="collection.id"
            size="md"
            variant="icon"
          />

          <button v-if="canEdit" @click="startEditing" class="px-4 py-2 bg-white/10 text-white border border-white/30 rounded-xl font-medium hover:bg-white/20 transition-all flex items-center gap-2">
            <i class="fas fa-pen"></i>
            <span class="hidden sm:inline">{{ textConstants.editCollection }}</span>
          </button>

          <button v-if="canEdit" @click="toggleVisibility" class="px-4 py-2 bg-white/10 text-white border border-white/30 rounded-xl font-medium hover:bg-white/20 transition-all flex items-center gap-2">
            <i :class="collection.visibility === 'private' ? 'fas fa-globe' : 'fas fa-lock'"></i>
            <span class="hidden sm:inline">{{ collection.visibility === 'private' ? textConstants.makeShared : textConstants.makePrivate }}</span>
          </button>

          <button v-if="isOwner" @click="showDeleteConfirm = true" class="px-4 py-2 bg-red-500/20 text-red-300 border border-red-500/30 rounded-xl font-medium hover:bg-red-500/30 transition-all flex items-center gap-2">
            <i class="fas fa-trash-alt"></i>
            <span class="hidden sm:inline">{{ textConstants.delete }}</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="main-content">
      <!-- Items Section -->
      <div class="items-section">
        <div class="section-header-enhanced">
          <div class="section-title-row">
            <h2 class="section-title">
              <i class="fas fa-layer-group"></i>
              {{ textConstants.collectionItems }}
            </h2>
            <span class="item-count-badge">{{ filteredAndSortedItems.length }} {{ textConstants.items }}</span>
          </div>

          <!-- Controls Row -->
          <div class="controls-row">
            <!-- Filter Tabs -->
            <div class="filter-tabs">
              <button
                @click="filterType = 'all'"
                :class="['filter-tab', filterType === 'all' ? 'active' : '']"
              >
                {{ textConstants.allTypes }}
                <span class="count">{{ itemCounts.all }}</span>
              </button>
              <button
                @click="filterType = 'article'"
                :class="['filter-tab', filterType === 'article' ? 'active' : '']"
              >
                <i class="fas fa-newspaper"></i>
                {{ textConstants.articles }}
                <span class="count">{{ itemCounts.article }}</span>
              </button>
              <button
                @click="filterType = 'document'"
                :class="['filter-tab', filterType === 'document' ? 'active' : '']"
              >
                <i class="fas fa-file-alt"></i>
                {{ textConstants.documents }}
                <span class="count">{{ itemCounts.document }}</span>
              </button>
              <button
                @click="filterType = 'media'"
                :class="['filter-tab', filterType === 'media' ? 'active' : '']"
              >
                <i class="fas fa-photo-video"></i>
                {{ textConstants.media }}
                <span class="count">{{ itemCounts.media }}</span>
              </button>
            </div>

            <!-- Sort & View Controls -->
            <div class="view-controls">
              <select v-model="sortBy" class="sort-select">
                <option value="recent">{{ textConstants.sortRecent }}</option>
                <option value="oldest">{{ textConstants.sortOldest }}</option>
                <option value="az">{{ textConstants.sortAZ }}</option>
                <option value="za">{{ textConstants.sortZA }}</option>
                <option value="type">{{ textConstants.sortType }}</option>
              </select>

              <div class="view-toggle">
                <button
                  @click="viewMode = 'list'"
                  :class="['view-btn', viewMode === 'list' ? 'active' : '']"
                  :title="textConstants.listView"
                >
                  <i class="fas fa-list"></i>
                </button>
                <button
                  @click="viewMode = 'grid'"
                  :class="['view-btn', viewMode === 'grid' ? 'active' : '']"
                  :title="textConstants.gridView"
                >
                  <i class="fas fa-th"></i>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- List View -->
        <div v-if="filteredAndSortedItems.length > 0 && viewMode === 'list'" class="items-list">
          <div
            v-for="item in filteredAndSortedItems"
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
            <div v-if="canEdit" class="drag-handle" :title="textConstants.dragToReorder">
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
              :title="textConstants.removeFromCollection"
            >
              <i class="fas fa-times"></i>
            </button>
          </div>
        </div>

        <!-- Grid View -->
        <div v-else-if="filteredAndSortedItems.length > 0 && viewMode === 'grid'" class="items-grid-view">
          <div
            v-for="item in filteredAndSortedItems"
            :key="item.id"
            class="grid-item-card"
            @click="openItem(item)"
          >
            <!-- Thumbnail -->
            <div class="grid-item-thumbnail">
              <img v-if="item.thumbnail" :src="item.thumbnail" :alt="item.title" />
              <div v-else class="grid-thumbnail-placeholder">
                <i :class="getContentTypeIcon(item.contentType)"></i>
              </div>
              <div :class="['grid-type-badge', getContentTypeColor(item.contentType)]">
                <i :class="getContentTypeIcon(item.contentType)"></i>
              </div>
              <!-- Remove Button -->
              <button
                v-if="canEdit"
                @click.stop="removeItem(item.id)"
                class="grid-remove-btn"
                :title="textConstants.removeFromCollection"
              >
                <i class="fas fa-times"></i>
              </button>
            </div>

            <!-- Content -->
            <div class="grid-item-content">
              <h3 class="grid-item-title">{{ item.title }}</h3>
              <div class="grid-item-meta">
                <span v-if="item.metadata?.duration">
                  <i class="fas fa-clock"></i>
                  {{ item.metadata.duration }}
                </span>
                <span v-else-if="item.metadata?.readTime">
                  <i class="fas fa-book-open"></i>
                  {{ item.metadata.readTime }}
                </span>
                <span v-else-if="item.metadata?.size">
                  <i class="fas fa-file"></i>
                  {{ item.metadata.size }}
                </span>
                <span v-else-if="item.metadata?.photoCount">
                  <i class="fas fa-images"></i>
                  {{ item.metadata.photoCount }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <div v-else class="empty-items">
          <div class="empty-icon">
            <i class="fas fa-layer-group"></i>
          </div>
          <h3>{{ textConstants.noItems }}</h3>
          <p>{{ textConstants.noItemsDesc }}</p>
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

        <!-- Activity Timeline Panel -->
        <div class="panel activity-panel">
          <div class="panel-header">
            <h3 class="panel-title">
              <i class="fas fa-history"></i>
              {{ textConstants.recentActivity }}
            </h3>
          </div>
          <div class="activity-list">
            <div
              v-for="activity in activities"
              :key="activity.id"
              class="activity-item"
            >
              <div class="activity-icon-wrapper" :class="getActivityColor(activity.type)">
                <i :class="getActivityIcon(activity.type)"></i>
              </div>
              <div class="activity-content">
                <p class="activity-text">
                  <span class="activity-user">{{ activity.user.name }}</span>
                  {{ getActivityText(activity) }}
                </p>
                <span class="activity-time">{{ formatDate(activity.timestamp) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Comments Panel -->
        <div class="panel comments-panel">
          <div class="panel-header">
            <h3 class="panel-title">
              <i class="fas fa-comments"></i>
              {{ textConstants.comments }}
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
                :placeholder="textConstants.addComment"
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
  width: 100%;
}

.collection-detail-page *,
.collection-detail-page *::before,
.collection-detail-page *::after {
  box-sizing: border-box;
}

/* ============================================================================
   HERO HEADER
   ============================================================================ */
.hero-header {
  position: relative;
  min-height: 320px;
  display: flex;
  flex-direction: column;
  background: #0f172a;
}

.hero-bg {
  position: absolute;
  inset: 0;
  overflow: hidden;
}

.hero-bg-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.hero-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    180deg,
    rgba(15, 23, 42, 0.7) 0%,
    rgba(15, 23, 42, 0.85) 50%,
    rgba(15, 23, 42, 0.95) 100%
  );
}

.hero-content {
  position: relative;
  z-index: 10;
  width: 100%;
  max-width: 1600px;
  margin: 0 auto;
  padding: 1.5rem 2rem 2rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  box-sizing: border-box;
}

/* Navigation */
.hero-nav {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.back-btn-hero {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 10px;
  color: white;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.back-btn-hero:hover {
  background: rgba(255, 255, 255, 0.2);
}

.edit-controls {
  display: flex;
  gap: 0.5rem;
}

/* Hero Info */
.hero-info {
  max-width: 800px;
}

.visibility-badge-hero {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
}

.visibility-badge-hero.private {
  background: rgba(30, 41, 59, 0.8);
  color: white;
}

.visibility-badge-hero.shared {
  background: rgba(20, 184, 166, 0.9);
  color: white;
}

.hero-title {
  font-size: 2rem;
  font-weight: 700;
  color: white;
  margin: 0 0 0.5rem;
  line-height: 1.2;
}

.hero-description {
  font-size: 1rem;
  color: rgba(255, 255, 255, 0.8);
  margin: 0;
  max-width: 600px;
}

/* Edit Mode in Hero */
.info-edit-hero {
  max-width: 600px;
}

.edit-title-hero {
  width: 100%;
  padding: 0.75rem 1rem;
  background: rgba(255, 255, 255, 0.1);
  border: 2px solid rgba(20, 184, 166, 0.5);
  border-radius: 10px;
  font-size: 1.5rem;
  font-weight: 600;
  color: white;
  margin-bottom: 0.75rem;
}

.edit-title-hero::placeholder {
  color: rgba(255, 255, 255, 0.5);
}

.edit-title-hero:focus {
  outline: none;
  border-color: #14b8a6;
  background: rgba(255, 255, 255, 0.15);
}

.edit-description-hero {
  width: 100%;
  padding: 0.75rem 1rem;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 10px;
  font-size: 0.9375rem;
  color: white;
  resize: none;
}

.edit-description-hero::placeholder {
  color: rgba(255, 255, 255, 0.5);
}

.edit-description-hero:focus {
  outline: none;
  border-color: rgba(20, 184, 166, 0.5);
}

/* Meta & Stats */
.hero-meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 1rem;
}

.meta-author {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.author-avatar-hero {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.875rem;
  font-weight: 600;
  color: white;
  border: 2px solid rgba(255, 255, 255, 0.3);
}

.author-info {
  display: flex;
  flex-direction: column;
}

.author-name {
  color: white;
  font-weight: 600;
  font-size: 0.9375rem;
}

.author-role {
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.75rem;
  text-transform: uppercase;
}

.meta-stats {
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  color: rgba(255, 255, 255, 0.8);
  font-size: 0.8125rem;
}

.stat-item i {
  color: rgba(255, 255, 255, 0.5);
}

/* Hero Actions */
.hero-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

/* Export Dropdown */
.export-dropdown {
  position: absolute;
  top: calc(100% + 0.5rem);
  left: 0;
  min-width: 200px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  overflow: hidden;
  z-index: 100;
}

.dropdown-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  width: 100%;
  padding: 0.75rem 1rem;
  background: none;
  border: none;
  font-size: 0.875rem;
  color: #374151;
  cursor: pointer;
  text-align: left;
  transition: background 0.15s;
}

.dropdown-item:hover {
  background: #f3f4f6;
}

.dropdown-item i {
  width: 16px;
  color: #6b7280;
}

.dropdown-divider {
  height: 1px;
  background: #e5e7eb;
  margin: 0.25rem 0;
}

.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.2s ease;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-8px);
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
  flex-wrap: nowrap;
  gap: 1.5rem;
  padding: 1.5rem;
  max-width: 100%;
  overflow: visible;
}

/* Items Section */
.items-section {
  flex: 1 1 0;
  min-width: 0;
  overflow: visible;
}

/* ============================================================================
   SECTION HEADER WITH CONTROLS
   ============================================================================ */
.section-header-enhanced {
  background: white;
  border-radius: 16px;
  padding: 1.25rem;
  margin-bottom: 1rem;
  border: 1px solid #e5e7eb;
  box-sizing: border-box;
  overflow: visible;
}

.section-title-row {
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
  margin: 0;
}

.section-title i {
  color: #14b8a6;
}

.item-count-badge {
  background: #f0fdfa;
  color: #0f766e;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.8125rem;
  font-weight: 600;
}

/* Controls Row */
.controls-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 1rem;
}

/* Filter Tabs */
.filter-tabs {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
  overflow: visible;
}

.filter-tab {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 0.875rem;
  background: #f8fafc;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #64748b;
  cursor: pointer;
  transition: all 0.15s;
}

.filter-tab:hover {
  background: #f1f5f9;
  border-color: #cbd5e1;
}

.filter-tab.active {
  background: #0f766e;
  border-color: #0f766e;
  color: white;
}

.filter-tab .count {
  background: rgba(255, 255, 255, 0.2);
  padding: 0.125rem 0.375rem;
  border-radius: 4px;
  font-size: 0.6875rem;
}

.filter-tab:not(.active) .count {
  background: #e5e7eb;
  color: #64748b;
}

/* View Controls */
.view-controls {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.sort-select {
  padding: 0.5rem 2rem 0.5rem 0.75rem;
  background: white url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 20 20' fill='%2364748b'%3E%3Cpath fill-rule='evenodd' d='M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z' clip-rule='evenodd'/%3E%3C/svg%3E") right 0.5rem center no-repeat;
  background-size: 16px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 0.8125rem;
  color: #374151;
  cursor: pointer;
  appearance: none;
}

.sort-select:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.view-toggle {
  display: flex;
  background: #f1f5f9;
  border-radius: 8px;
  padding: 0.25rem;
}

.view-btn {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: none;
  border-radius: 6px;
  color: #64748b;
  cursor: pointer;
  transition: all 0.15s;
}

.view-btn:hover {
  color: #374151;
}

.view-btn.active {
  background: white;
  color: #14b8a6;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

/* ============================================================================
   LIST VIEW
   ============================================================================ */
.items-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  overflow: visible;
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
  box-sizing: border-box;
  overflow: visible;
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

/* ============================================================================
   GRID VIEW
   ============================================================================ */
.items-grid-view {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1rem;
  overflow: visible;
}

.grid-item-card {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.2s;
}

.grid-item-card:hover {
  border-color: #99f6e4;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  transform: translateY(-2px);
}

.grid-item-thumbnail {
  position: relative;
  aspect-ratio: 16/10;
  overflow: hidden;
}

.grid-item-thumbnail img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s;
}

.grid-item-card:hover .grid-item-thumbnail img {
  transform: scale(1.05);
}

.grid-thumbnail-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #e2e8f0 0%, #cbd5e1 100%);
  color: #94a3b8;
  font-size: 2rem;
}

.grid-type-badge {
  position: absolute;
  bottom: 0.5rem;
  left: 0.5rem;
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 6px;
  font-size: 0.75rem;
}

.grid-remove-btn {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.5);
  border: none;
  border-radius: 50%;
  color: white;
  cursor: pointer;
  opacity: 0;
  transition: all 0.15s;
}

.grid-item-card:hover .grid-remove-btn {
  opacity: 1;
}

.grid-remove-btn:hover {
  background: #dc2626;
}

.grid-item-content {
  padding: 0.75rem;
}

.grid-item-title {
  font-size: 0.875rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0 0 0.25rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.grid-item-meta {
  font-size: 0.75rem;
  color: #94a3b8;
}

.grid-item-meta span {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

/* ============================================================================
   ACTIVITY PANEL
   ============================================================================ */
.activity-panel {
  max-height: 320px;
  display: flex;
  flex-direction: column;
}

.activity-list {
  padding: 0.5rem;
  overflow-y: auto;
  flex: 1;
}

.activity-item {
  display: flex;
  gap: 0.75rem;
  padding: 0.625rem 0.5rem;
  border-radius: 8px;
  transition: background 0.15s;
}

.activity-item:hover {
  background: #f8fafc;
}

.activity-icon-wrapper {
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  font-size: 0.75rem;
}

.activity-content {
  flex: 1;
  min-width: 0;
}

.activity-text {
  font-size: 0.8125rem;
  color: #475569;
  margin: 0 0 0.25rem;
  line-height: 1.4;
}

.activity-user {
  font-weight: 600;
  color: #1e293b;
}

.activity-time {
  font-size: 0.6875rem;
  color: #94a3b8;
}

/* Sidebar */
.sidebar {
  flex: 0 0 340px;
  width: 340px;
  min-width: 280px;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  overflow: visible;
}

/* Panel base styles */
.panel {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 0;
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

/* ============================================================================
   RESPONSIVE
   ============================================================================ */
@media (max-width: 1200px) {
  .sidebar {
    flex: 0 0 300px;
    width: 300px;
  }
}

@media (max-width: 1024px) {
  .main-content {
    flex-direction: column;
    padding: 1rem;
  }

  .sidebar {
    flex: none;
    width: 100%;
  }

  .hero-content {
    padding: 1.5rem 1rem 2rem;
  }

  .hero-meta {
    flex-direction: column;
    align-items: flex-start;
  }

  .controls-row {
    flex-direction: column;
    align-items: stretch;
  }

  .filter-tabs {
    overflow-x: auto;
    padding-bottom: 0.5rem;
  }

  .view-controls {
    justify-content: space-between;
  }
}

@media (max-width: 768px) {
  .hero-content {
    padding: 1rem 1rem 1.5rem;
  }

  .hero-title {
    font-size: 1.5rem;
  }

  .hero-actions {
    flex-wrap: wrap;
  }

  .hero-actions button span {
    display: none;
  }

  .back-btn-hero span {
    display: none;
  }

  .section-header-enhanced {
    padding: 1rem;
  }

  .item-card {
    flex-direction: column;
    gap: 0.75rem;
  }

  .item-thumbnail {
    width: 100%;
    height: 160px;
  }

  .drag-handle {
    display: none;
  }

  .items-grid-view {
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
    gap: 0.75rem;
  }

  .filter-tab {
    padding: 0.375rem 0.625rem;
    font-size: 0.75rem;
  }

  .filter-tab i {
    display: none;
  }
}

@media (max-width: 480px) {
  .hero-header {
    min-height: 280px;
  }

  .hero-title {
    font-size: 1.25rem;
  }

  .hero-description {
    font-size: 0.875rem;
  }

  .meta-stats {
    gap: 0.75rem;
  }

  .stat-item {
    font-size: 0.75rem;
  }
}
</style>
