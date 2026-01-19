<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import PageHeroHeader from '@/components/common/PageHeroHeader.vue'

const router = useRouter()
const { t } = useI18n()

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
  metadata?: Record<string, any>
}

interface Collection {
  id: string
  name: string
  description: string
  thumbnail: string | null | undefined
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
  color: '#14b8a6'
})

// View & Filters
const currentView = ref<'all' | 'my' | 'shared' | 'recent'>('all')
const searchQuery = ref('')
const selectedType = ref<'all' | 'mixed' | 'articles' | 'documents' | 'media'>('all')
const sortBy = ref<'updated' | 'created' | 'name' | 'items'>('updated')
const showCreateModal = ref(false)
const showTypeFilter = ref(false)
const showSortDropdown = ref(false)

// View options
const viewOptions = ref([
  { id: 'all', name: 'All Collections', icon: 'fas fa-layer-group', color: 'text-teal-500' },
  { id: 'my', name: 'My Collections', icon: 'fas fa-user', color: 'text-blue-500' },
  { id: 'shared', name: 'Shared with Me', icon: 'fas fa-share-alt', color: 'text-purple-500' },
  { id: 'recent', name: 'Recently Viewed', icon: 'fas fa-clock', color: 'text-amber-500' }
])

// Type filter options
const typeOptions = ref([
  { id: 'all', label: 'All Types', icon: 'fas fa-asterisk' },
  { id: 'mixed', label: 'Mixed Content', icon: 'fas fa-layer-group' },
  { id: 'articles', label: 'Articles Only', icon: 'fas fa-newspaper' },
  { id: 'documents', label: 'Documents Only', icon: 'fas fa-file-alt' },
  { id: 'media', label: 'Media Only', icon: 'fas fa-photo-video' }
])

// Sort options
const sortOptions = ref([
  { id: 'updated', label: 'Recently Updated', icon: 'fas fa-clock' },
  { id: 'created', label: 'Date Created', icon: 'fas fa-calendar' },
  { id: 'name', label: 'Name (A-Z)', icon: 'fas fa-sort-alpha-down' },
  { id: 'items', label: 'Most Items', icon: 'fas fa-sort-numeric-down' }
])

// Recently viewed collections (for the "Recent" view)
const recentlyViewed = ref<Set<string>>(new Set(['col-1', 'col-3', 'col-5']))

// Mock collections data
const collections = ref<Collection[]>([
  {
    id: 'col-1',
    name: 'Tournament Highlights 2027',
    description: 'Collection of best moments, match highlights, and memorable plays from AFC Asian Cup 2027',
    thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400',
    createdAt: '2027-01-05T10:00:00Z',
    updatedAt: '2027-01-07T08:30:00Z',
    author: { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6' },
    visibility: 'shared',
    itemCount: 15,
    items: [
      { id: 'item-1', contentType: 'media', contentId: 5, addedAt: '2027-01-05T10:00:00Z', addedBy: 'user-1', order: 1, title: 'Saudi Arabia vs Japan: Opening Match Preview', thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=200' },
      { id: 'item-2', contentType: 'article', contentId: 1, addedAt: '2027-01-05T11:00:00Z', addedBy: 'user-1', order: 2, title: 'Match Analysis: Opening Day', thumbnail: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=200' },
      { id: 'item-3', contentType: 'media', contentId: 13, addedAt: '2027-01-06T09:00:00Z', addedBy: 'user-2', order: 3, title: 'AFC Asian Cup 2027 Official Draw Ceremony', thumbnail: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=200' }
    ],
    collaborators: [
      { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6', email: 'ahmed@afc.com', role: 'owner', addedAt: '2027-01-05T10:00:00Z' },
      { id: 'user-2', name: 'Sarah Chen', initials: 'SC', color: '#8b5cf6', email: 'sarah@afc.com', role: 'editor', addedAt: '2027-01-05T12:00:00Z' }
    ],
    comments: [
      { id: 'comment-1', author: { id: 'user-2', name: 'Sarah Chen', initials: 'SC', color: '#8b5cf6' }, content: 'Great collection! Added the draw ceremony video.', createdAt: '2027-01-06T09:30:00Z', mentions: [] }
    ]
  },
  {
    id: 'col-2',
    name: 'Team Research - Group A',
    description: 'Comprehensive research materials on Group A teams including player profiles, tactics, and historical performance',
    thumbnail: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=400',
    createdAt: '2027-01-03T14:00:00Z',
    updatedAt: '2027-01-06T16:45:00Z',
    author: { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6' },
    visibility: 'private',
    itemCount: 22,
    items: [
      { id: 'item-4', contentType: 'document', contentId: 'd-1', addedAt: '2027-01-03T14:00:00Z', addedBy: 'user-1', order: 1, title: 'Saudi Arabia Team Analysis', thumbnail: undefined },
      { id: 'item-5', contentType: 'document', contentId: 'd-2', addedAt: '2027-01-03T15:00:00Z', addedBy: 'user-1', order: 2, title: 'Japan Squad Overview', thumbnail: undefined },
      { id: 'item-6', contentType: 'article', contentId: 2, addedAt: '2027-01-04T10:00:00Z', addedBy: 'user-1', order: 3, title: 'Group A Preview: Key Players to Watch', thumbnail: 'https://images.unsplash.com/photo-1508098682722-e99c643e7f76?w=200' }
    ],
    collaborators: [
      { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6', email: 'ahmed@afc.com', role: 'owner', addedAt: '2027-01-03T14:00:00Z' }
    ],
    comments: []
  },
  {
    id: 'col-3',
    name: 'Media Kit 2027',
    description: 'Official media assets, logos, graphics, and press materials for AFC Asian Cup 2027',
    thumbnail: 'https://images.unsplash.com/photo-1560272564-c83b66b1ad12?w=400',
    createdAt: '2026-12-15T09:00:00Z',
    updatedAt: '2027-01-07T11:20:00Z',
    author: { id: 'user-3', name: 'Mohammed Al-Rashid', initials: 'MR', color: '#f59e0b' },
    visibility: 'shared',
    itemCount: 45,
    items: [
      { id: 'item-7', contentType: 'media', contentId: 'm-1', addedAt: '2026-12-15T09:00:00Z', addedBy: 'user-3', order: 1, title: 'Official Logo Pack', thumbnail: 'https://images.unsplash.com/photo-1560272564-c83b66b1ad12?w=200' },
      { id: 'item-8', contentType: 'media', contentId: 'm-2', addedAt: '2026-12-16T10:00:00Z', addedBy: 'user-3', order: 2, title: 'Stadium Photos Collection', thumbnail: 'https://images.unsplash.com/photo-1577223625816-7546f13df25d?w=200' },
      { id: 'item-9', contentType: 'document', contentId: 'd-3', addedAt: '2026-12-20T14:00:00Z', addedBy: 'user-3', order: 3, title: 'Brand Guidelines PDF', thumbnail: undefined }
    ],
    collaborators: [
      { id: 'user-3', name: 'Mohammed Al-Rashid', initials: 'MR', color: '#f59e0b', email: 'mohammed@afc.com', role: 'owner', addedAt: '2026-12-15T09:00:00Z' },
      { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6', email: 'ahmed@afc.com', role: 'viewer', addedAt: '2026-12-20T11:00:00Z' }
    ],
    comments: []
  },
  {
    id: 'col-4',
    name: 'Fan Engagement Ideas',
    description: 'Creative concepts and successful fan engagement initiatives from past tournaments',
    thumbnail: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=400',
    createdAt: '2027-01-02T11:00:00Z',
    updatedAt: '2027-01-05T09:15:00Z',
    author: { id: 'user-2', name: 'Sarah Chen', initials: 'SC', color: '#8b5cf6' },
    visibility: 'shared',
    itemCount: 8,
    items: [
      { id: 'item-10', contentType: 'article', contentId: 3, addedAt: '2027-01-02T11:00:00Z', addedBy: 'user-2', order: 1, title: 'Fan Zone Experience Design', thumbnail: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=200' },
      { id: 'item-11', contentType: 'media', contentId: 'm-3', addedAt: '2027-01-03T08:00:00Z', addedBy: 'user-2', order: 2, title: 'Social Media Campaign Videos', thumbnail: 'https://images.unsplash.com/photo-1611162617213-7d7a39e9b1d7?w=200' }
    ],
    collaborators: [
      { id: 'user-2', name: 'Sarah Chen', initials: 'SC', color: '#8b5cf6', email: 'sarah@afc.com', role: 'owner', addedAt: '2027-01-02T11:00:00Z' },
      { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6', email: 'ahmed@afc.com', role: 'editor', addedAt: '2027-01-02T14:00:00Z' }
    ],
    comments: [
      { id: 'comment-2', author: { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6' }, content: '@Sarah Chen these ideas are fantastic! Can we add the AR experience concept?', createdAt: '2027-01-03T10:00:00Z', mentions: ['user-2'] }
    ]
  },
  {
    id: 'col-5',
    name: 'Venue Documentation',
    description: 'Complete documentation for all tournament venues including maps, facilities, and technical specs',
    thumbnail: 'https://images.unsplash.com/photo-1577223625816-7546f13df25d?w=400',
    createdAt: '2026-11-20T10:00:00Z',
    updatedAt: '2027-01-04T14:30:00Z',
    author: { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6' },
    visibility: 'private',
    itemCount: 32,
    items: [
      { id: 'item-12', contentType: 'document', contentId: 'd-4', addedAt: '2026-11-20T10:00:00Z', addedBy: 'user-1', order: 1, title: 'King Fahd Stadium Technical Specs', thumbnail: undefined },
      { id: 'item-13', contentType: 'media', contentId: 6, addedAt: '2026-11-25T09:00:00Z', addedBy: 'user-1', order: 2, title: 'King Fahd Stadium: Behind the Scenes Tour', thumbnail: 'https://images.unsplash.com/photo-1577223625816-7546f13df25d?w=200' }
    ],
    collaborators: [
      { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6', email: 'ahmed@afc.com', role: 'owner', addedAt: '2026-11-20T10:00:00Z' }
    ],
    comments: []
  },
  {
    id: 'col-6',
    name: 'Training Content Library',
    description: 'Educational materials and training resources for staff onboarding',
    thumbnail: 'https://images.unsplash.com/photo-1434030216411-0b793f4b4173?w=400',
    createdAt: '2026-12-01T09:00:00Z',
    updatedAt: '2027-01-06T11:00:00Z',
    author: { id: 'user-4', name: 'Yuki Tanaka', initials: 'YT', color: '#ef4444' },
    visibility: 'shared',
    itemCount: 18,
    items: [
      { id: 'item-14', contentType: 'media', contentId: 'm-4', addedAt: '2026-12-01T09:00:00Z', addedBy: 'user-4', order: 1, title: 'Welcome & Orientation Video', thumbnail: 'https://images.unsplash.com/photo-1434030216411-0b793f4b4173?w=200' },
      { id: 'item-15', contentType: 'document', contentId: 'd-5', addedAt: '2026-12-05T10:00:00Z', addedBy: 'user-4', order: 2, title: 'Staff Handbook 2027', thumbnail: undefined }
    ],
    collaborators: [
      { id: 'user-4', name: 'Yuki Tanaka', initials: 'YT', color: '#ef4444', email: 'yuki@afc.com', role: 'owner', addedAt: '2026-12-01T09:00:00Z' },
      { id: 'user-1', name: 'Ahmed Imam', initials: 'AI', color: '#14b8a6', email: 'ahmed@afc.com', role: 'viewer', addedAt: '2026-12-10T11:00:00Z' }
    ],
    comments: []
  }
])

// Computed: Filtered collections
const filteredCollections = computed(() => {
  let result = [...collections.value]

  // Filter by view
  if (currentView.value === 'my') {
    result = result.filter(c => c.author.id === currentUser.value.id)
  } else if (currentView.value === 'shared') {
    result = result.filter(c =>
      c.visibility === 'shared' &&
      c.author.id !== currentUser.value.id &&
      c.collaborators.some(col => col.id === currentUser.value.id)
    )
  } else if (currentView.value === 'recent') {
    result = result.filter(c => recentlyViewed.value.has(c.id))
  }

  // Filter by search
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(c =>
      c.name.toLowerCase().includes(query) ||
      c.description.toLowerCase().includes(query)
    )
  }

  // Filter by content type
  if (selectedType.value !== 'all') {
    if (selectedType.value === 'mixed') {
      result = result.filter(c => {
        const types = new Set(c.items.map(i => i.contentType))
        return types.size > 1
      })
    } else {
      const typeMap: Record<string, string> = {
        articles: 'article',
        documents: 'document',
        media: 'media'
      }
      const targetType = typeMap[selectedType.value]
      result = result.filter(c =>
        c.items.some(i => i.contentType === targetType)
      )
    }
  }

  // Sort
  result.sort((a, b) => {
    switch (sortBy.value) {
      case 'updated':
        return new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()
      case 'created':
        return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      case 'name':
        return a.name.localeCompare(b.name)
      case 'items':
        return b.itemCount - a.itemCount
      default:
        return 0
    }
  })

  return result
})

// Stats
const stats = computed(() => ({
  total: collections.value.length,
  myCollections: collections.value.filter(c => c.author.id === currentUser.value.id).length,
  sharedWithMe: collections.value.filter(c =>
    c.visibility === 'shared' &&
    c.author.id !== currentUser.value.id &&
    c.collaborators.some(col => col.id === currentUser.value.id)
  ).length,
  totalItems: collections.value.reduce((sum, c) => sum + c.itemCount, 0)
}))

// Hero stats for PageHeroHeader component
const heroStats = computed(() => [
  { icon: 'fas fa-layer-group', value: stats.value.total, label: t('collections.title') },
  { icon: 'fas fa-user', value: stats.value.myCollections, label: t('collections.myCollections') },
  { icon: 'fas fa-share-alt', value: stats.value.sharedWithMe, label: t('common.share') },
  { icon: 'fas fa-file-alt', value: stats.value.totalItems, label: t('collections.totalItems') }
])

// Helper functions
function formatDate(dateString: string): string {
  const date = new Date(dateString)
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))

  if (diffDays === 0) return 'Today'
  if (diffDays === 1) return 'Yesterday'
  if (diffDays < 7) return `${diffDays} days ago`
  if (diffDays < 30) return `${Math.floor(diffDays / 7)} weeks ago`
  return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
}

function getContentTypeIcon(type: string): string {
  switch (type) {
    case 'article': return 'fas fa-newspaper'
    case 'document': return 'fas fa-file-alt'
    case 'media': return 'fas fa-photo-video'
    default: return 'fas fa-file'
  }
}

function getContentTypeCounts(collection: Collection): { articles: number; documents: number; media: number } {
  return {
    articles: collection.items.filter(i => i.contentType === 'article').length,
    documents: collection.items.filter(i => i.contentType === 'document').length,
    media: collection.items.filter(i => i.contentType === 'media').length
  }
}

function openCollection(collection: Collection) {
  recentlyViewed.value.add(collection.id)
  router.push(`/collections/${collection.id}`)
}

function createNewCollection() {
  showCreateModal.value = true
}

function getCurrentSortLabel(): string {
  return sortOptions.value.find(o => o.id === sortBy.value)?.label || 'Recently Updated'
}

function getCurrentTypeLabel(): string {
  return typeOptions.value.find(o => o.id === selectedType.value)?.label || 'All Types'
}

// New collection form
const newCollectionName = ref('')
const newCollectionDescription = ref('')
const newCollectionVisibility = ref<'private' | 'shared'>('private')

function saveNewCollection() {
  if (!newCollectionName.value.trim()) return

  const newCollection: Collection = {
    id: `col-${Date.now()}`,
    name: newCollectionName.value.trim(),
    description: newCollectionDescription.value.trim(),
    thumbnail: undefined,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
    author: { ...currentUser.value },
    visibility: newCollectionVisibility.value,
    itemCount: 0,
    items: [],
    collaborators: [{
      ...currentUser.value,
      email: 'ahmed@afc.com',
      role: 'owner',
      addedAt: new Date().toISOString()
    }],
    comments: []
  }

  collections.value.unshift(newCollection)

  // Reset form
  newCollectionName.value = ''
  newCollectionDescription.value = ''
  newCollectionVisibility.value = 'private'
  showCreateModal.value = false

  // Navigate to the new collection
  router.push(`/collections/${newCollection.id}`)
}
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Hero Section -->
    <PageHeroHeader
      :stats="heroStats"
      badge-icon="fas fa-layer-group"
      :title="$t('collections.title')"
      :subtitle="$t('collections.subtitle')"
    >
      <template #actions>
        <button @click="createNewCollection" class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
          <i class="fas fa-plus"></i>
          {{ $t('collections.newCollection') }}
        </button>
        <button class="px-5 py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-xl font-semibold text-sm hover:bg-white/30 transition-all flex items-center gap-2">
          <i class="fas fa-bookmark"></i>
          {{ $t('collections.mySaved') }}
        </button>
      </template>
    </PageHeroHeader>

    <!-- View Navigation -->
    <div class="view-nav">
      <button
        v-for="view in viewOptions"
        :key="view.id"
        @click="currentView = view.id as any"
        :class="[
          'view-nav-btn',
          currentView === view.id ? 'active' : ''
        ]"
      >
        <i :class="[view.icon, view.color]"></i>
        <span>{{ view.name }}</span>
      </button>
    </div>

    <!-- Toolbar -->
    <div class="toolbar">
      <!-- Search -->
      <div class="search-box">
        <i class="fas fa-search"></i>
        <input
          v-model="searchQuery"
          type="text"
          :placeholder="$t('collections.searchCollections')"
          class="search-input"
        />
        <button v-if="searchQuery" @click="searchQuery = ''" class="clear-search">
          <i class="fas fa-times"></i>
        </button>
      </div>

      <div class="toolbar-filters">
        <!-- Type Filter -->
        <div class="filter-dropdown">
          <button
            @click="showTypeFilter = !showTypeFilter"
            :class="['filter-btn', selectedType !== 'all' ? 'active' : '']"
          >
            <i class="fas fa-filter"></i>
            <span>{{ getCurrentTypeLabel() }}</span>
            <i :class="showTypeFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="ml-1"></i>
          </button>
          <div v-if="showTypeFilter" class="dropdown-menu">
            <button
              v-for="option in typeOptions"
              :key="option.id"
              @click="selectedType = option.id as any; showTypeFilter = false"
              :class="['dropdown-item', selectedType === option.id ? 'active' : '']"
            >
              <i :class="option.icon"></i>
              <span>{{ option.label }}</span>
              <i v-if="selectedType === option.id" class="fas fa-check ml-auto"></i>
            </button>
          </div>
          <div v-if="showTypeFilter" @click="showTypeFilter = false" class="dropdown-backdrop"></div>
        </div>

        <!-- Sort Dropdown -->
        <div class="filter-dropdown">
          <button
            @click="showSortDropdown = !showSortDropdown"
            class="filter-btn"
          >
            <i class="fas fa-sort"></i>
            <span>{{ getCurrentSortLabel() }}</span>
            <i :class="showSortDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="ml-1"></i>
          </button>
          <div v-if="showSortDropdown" class="dropdown-menu">
            <button
              v-for="option in sortOptions"
              :key="option.id"
              @click="sortBy = option.id as any; showSortDropdown = false"
              :class="['dropdown-item', sortBy === option.id ? 'active' : '']"
            >
              <i :class="option.icon"></i>
              <span>{{ option.label }}</span>
              <i v-if="sortBy === option.id" class="fas fa-check ml-auto"></i>
            </button>
          </div>
          <div v-if="showSortDropdown" @click="showSortDropdown = false" class="dropdown-backdrop"></div>
        </div>
      </div>
    </div>

    <!-- Collections Grid -->
    <div class="collections-grid">
      <div
        v-for="collection in filteredCollections"
        :key="collection.id"
        class="collection-card"
        @click="openCollection(collection)"
      >
        <!-- Thumbnail Mosaic -->
        <div class="card-thumbnails">
          <template v-if="collection.items.length >= 3">
            <div class="thumb-main">
              <img
                v-if="collection.items[0]?.thumbnail"
                :src="collection.items[0].thumbnail"
                :alt="collection.items[0].title"
              />
              <div v-else class="thumb-placeholder">
                <i :class="getContentTypeIcon(collection.items[0]?.contentType || 'document')"></i>
              </div>
            </div>
            <div class="thumb-side">
              <div class="thumb-small">
                <img
                  v-if="collection.items[1]?.thumbnail"
                  :src="collection.items[1].thumbnail"
                  :alt="collection.items[1].title"
                />
                <div v-else class="thumb-placeholder small">
                  <i :class="getContentTypeIcon(collection.items[1]?.contentType || 'document')"></i>
                </div>
              </div>
              <div class="thumb-small">
                <img
                  v-if="collection.items[2]?.thumbnail"
                  :src="collection.items[2].thumbnail"
                  :alt="collection.items[2].title"
                />
                <div v-else class="thumb-placeholder small">
                  <i :class="getContentTypeIcon(collection.items[2]?.contentType || 'document')"></i>
                </div>
              </div>
            </div>
          </template>
          <template v-else-if="collection.items.length > 0">
            <div class="thumb-full">
              <img
                v-if="collection.items[0]?.thumbnail"
                :src="collection.items[0].thumbnail"
                :alt="collection.items[0].title"
              />
              <div v-else class="thumb-placeholder full">
                <i :class="getContentTypeIcon(collection.items[0]?.contentType || 'document')"></i>
              </div>
            </div>
          </template>
          <template v-else>
            <div class="thumb-empty">
              <i class="fas fa-layer-group"></i>
              <span>Empty Collection</span>
            </div>
          </template>

          <!-- Visibility Badge -->
          <div :class="['visibility-badge', collection.visibility]">
            <i :class="collection.visibility === 'private' ? 'fas fa-lock' : 'fas fa-globe'"></i>
          </div>
        </div>

        <!-- Card Content -->
        <div class="card-content">
          <h3 class="card-title">{{ collection.name }}</h3>
          <p class="card-description">{{ collection.description }}</p>

          <!-- Item Type Counts -->
          <div class="card-type-counts">
            <span v-if="getContentTypeCounts(collection).articles > 0" class="type-count articles">
              <i class="fas fa-newspaper"></i>
              {{ getContentTypeCounts(collection).articles }}
            </span>
            <span v-if="getContentTypeCounts(collection).documents > 0" class="type-count documents">
              <i class="fas fa-file-alt"></i>
              {{ getContentTypeCounts(collection).documents }}
            </span>
            <span v-if="getContentTypeCounts(collection).media > 0" class="type-count media">
              <i class="fas fa-photo-video"></i>
              {{ getContentTypeCounts(collection).media }}
            </span>
          </div>

          <!-- Meta -->
          <div class="card-meta">
            <div class="meta-author">
              <div
                class="author-avatar"
                :style="{ backgroundColor: collection.author.color }"
              >
                {{ collection.author.initials }}
              </div>
              <span class="author-name">{{ collection.author.name }}</span>
            </div>
            <span class="meta-date">{{ formatDate(collection.updatedAt) }}</span>
          </div>

          <!-- Collaborators -->
          <div v-if="collection.collaborators.length > 1" class="card-collaborators">
            <div class="collaborator-avatars">
              <div
                v-for="(collaborator, idx) in collection.collaborators.slice(0, 3)"
                :key="collaborator.id"
                class="collaborator-avatar"
                :style="{ backgroundColor: collaborator.color, zIndex: 3 - idx }"
                :title="collaborator.name"
              >
                {{ collaborator.initials }}
              </div>
              <div
                v-if="collection.collaborators.length > 3"
                class="collaborator-avatar more"
              >
                +{{ collection.collaborators.length - 3 }}
              </div>
            </div>
            <span class="collaborators-label">{{ collection.collaborators.length }} collaborators</span>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="filteredCollections.length === 0" class="empty-state">
        <div class="empty-icon">
          <i class="fas fa-layer-group"></i>
        </div>
        <h3 class="empty-title">No collections found</h3>
        <p class="empty-description">
          {{ currentView === 'my' ? "You haven't created any collections yet." :
             currentView === 'shared' ? "No collections have been shared with you." :
             searchQuery ? "Try adjusting your search or filters." :
             "Create your first collection to get started." }}
        </p>
        <button @click="createNewCollection" class="btn-create-empty">
          <i class="fas fa-plus mr-2"></i>
          Create Collection
        </button>
      </div>
    </div>

    <!-- Create Collection Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="showCreateModal" class="modal-overlay" @click.self="showCreateModal = false">
          <div class="modal-content">
            <div class="modal-header">
              <h3 class="modal-title">
                <i class="fas fa-layer-group text-teal-500 mr-2"></i>
                Create New Collection
              </h3>
              <button @click="showCreateModal = false" class="modal-close">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <div class="modal-body">
              <div class="form-group">
                <label class="form-label">Collection Name</label>
                <input
                  v-model="newCollectionName"
                  type="text"
                  class="form-input"
                  placeholder="e.g., Tournament Highlights"
                  autofocus
                />
              </div>
              <div class="form-group">
                <label class="form-label">Description (optional)</label>
                <textarea
                  v-model="newCollectionDescription"
                  class="form-textarea"
                  rows="3"
                  placeholder="What's this collection about?"
                ></textarea>
              </div>
              <div class="form-group">
                <label class="form-label">Visibility</label>
                <div class="visibility-options">
                  <button
                    @click="newCollectionVisibility = 'private'"
                    :class="['visibility-option', newCollectionVisibility === 'private' ? 'active' : '']"
                  >
                    <i class="fas fa-lock"></i>
                    <span>Private</span>
                    <small>Only you can see this collection</small>
                  </button>
                  <button
                    @click="newCollectionVisibility = 'shared'"
                    :class="['visibility-option', newCollectionVisibility === 'shared' ? 'active' : '']"
                  >
                    <i class="fas fa-globe"></i>
                    <span>Shared</span>
                    <small>Invite collaborators to view or edit</small>
                  </button>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button @click="showCreateModal = false" class="btn-cancel">
                Cancel
              </button>
              <button
                @click="saveNewCollection"
                class="btn-save"
                :disabled="!newCollectionName.trim()"
              >
                <i class="fas fa-plus mr-2"></i>
                Create Collection
              </button>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<style scoped>
/* View Navigation */
.view-nav {
  display: flex;
  gap: 0.5rem;
  padding: 1rem 2rem;
  background: white;
  border-bottom: 1px solid #e5e7eb;
  margin: 0;
}

.view-nav-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #64748b;
  background: transparent;
  border: 1px solid transparent;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.view-nav-btn:hover {
  background: #f1f5f9;
  color: #334155;
}

.view-nav-btn.active {
  background: #f0fdfa;
  color: #0f766e;
  border-color: #99f6e4;
}

/* Toolbar */
.toolbar {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem 2rem;
  background: white;
  border-bottom: 1px solid #e5e7eb;
}

.search-box {
  position: relative;
  flex: 1;
  max-width: 400px;
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
  transition: all 0.2s;
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
  color: #334155;
}

.toolbar-filters {
  display: flex;
  gap: 0.75rem;
  margin-left: auto;
}

.filter-dropdown {
  position: relative;
}

.filter-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s;
}

.filter-btn:hover {
  border-color: #cbd5e1;
  background: #f8fafc;
}

.filter-btn.active {
  background: #f0fdfa;
  border-color: #99f6e4;
  color: #0f766e;
}

.dropdown-menu {
  position: absolute;
  top: calc(100% + 4px);
  right: 0;
  min-width: 200px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  z-index: 50;
  overflow: hidden;
}

.dropdown-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  width: 100%;
  padding: 0.75rem 1rem;
  font-size: 0.875rem;
  color: #475569;
  background: transparent;
  border: none;
  cursor: pointer;
  transition: background 0.15s;
}

.dropdown-item:hover {
  background: #f8fafc;
}

.dropdown-item.active {
  background: #f0fdfa;
  color: #0f766e;
}

.dropdown-backdrop {
  position: fixed;
  inset: 0;
  z-index: 40;
}

/* Collections Grid */
.collections-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(340px, 1fr));
  gap: 1.5rem;
  padding: 1.5rem 2rem 3rem;
}

.collection-card {
  background: white;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #e5e7eb;
  cursor: pointer;
  transition: all 0.2s;
}

.collection-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.1);
  border-color: #99f6e4;
}

/* Card Thumbnails */
.card-thumbnails {
  height: 160px;
  background: #f1f5f9;
  position: relative;
  display: flex;
  gap: 2px;
  overflow: hidden;
}

.thumb-main {
  flex: 2;
  height: 100%;
}

.thumb-main img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.thumb-side {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.thumb-small {
  flex: 1;
}

.thumb-small img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.thumb-full {
  width: 100%;
  height: 100%;
}

.thumb-full img {
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
  font-size: 2rem;
}

.thumb-placeholder.small {
  font-size: 1.25rem;
}

.thumb-placeholder.full {
  font-size: 3rem;
}

.thumb-empty {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  background: linear-gradient(135deg, #e2e8f0 0%, #cbd5e1 100%);
  color: #94a3b8;
}

.thumb-empty i {
  font-size: 2rem;
}

.thumb-empty span {
  font-size: 0.75rem;
  font-weight: 500;
}

.visibility-badge {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
}

.visibility-badge.private {
  background: rgba(30, 41, 59, 0.8);
  color: white;
}

.visibility-badge.shared {
  background: rgba(20, 184, 166, 0.9);
  color: white;
}

/* Card Content */
.card-content {
  padding: 1rem 1.25rem 1.25rem;
}

.card-title {
  font-size: 1rem;
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 0.375rem;
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.card-description {
  font-size: 0.8125rem;
  color: #64748b;
  line-height: 1.5;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  margin-bottom: 0.75rem;
}

.card-type-counts {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
}

.type-count {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
  font-size: 0.75rem;
  font-weight: 500;
}

.type-count.articles {
  background: #fef3c7;
  color: #b45309;
}

.type-count.documents {
  background: #dbeafe;
  color: #1d4ed8;
}

.type-count.media {
  background: #f3e8ff;
  color: #7c3aed;
}

.card-meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-top: 0.75rem;
  border-top: 1px solid #f1f5f9;
}

.meta-author {
  display: flex;
  align-items: center;
  gap: 0.5rem;
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

.author-name {
  font-size: 0.8125rem;
  font-weight: 500;
  color: #475569;
}

.meta-date {
  font-size: 0.75rem;
  color: #94a3b8;
}

.card-collaborators {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-top: 0.75rem;
  padding-top: 0.75rem;
  border-top: 1px solid #f1f5f9;
}

.collaborator-avatars {
  display: flex;
}

.collaborator-avatar {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.5625rem;
  font-weight: 600;
  color: white;
  border: 2px solid white;
  margin-left: -8px;
}

.collaborator-avatar:first-child {
  margin-left: 0;
}

.collaborator-avatar.more {
  background: #e5e7eb;
  color: #64748b;
  font-size: 0.625rem;
}

.collaborators-label {
  font-size: 0.75rem;
  color: #94a3b8;
}

/* Empty State */
.empty-state {
  grid-column: 1 / -1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
}

.empty-icon {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: #f1f5f9;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2rem;
  color: #94a3b8;
  margin-bottom: 1.5rem;
}

.empty-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 0.5rem;
}

.empty-description {
  font-size: 0.9375rem;
  color: #64748b;
  max-width: 400px;
  margin-bottom: 1.5rem;
}

.btn-create-empty {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-weight: 600;
  font-size: 0.875rem;
  border-radius: 10px;
  border: none;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-create-empty:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
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
  max-width: 480px;
  max-height: 90vh;
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

.form-input,
.form-textarea {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  font-size: 0.9375rem;
  transition: all 0.2s;
}

.form-input:focus,
.form-textarea:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.form-textarea {
  resize: vertical;
  min-height: 80px;
}

.visibility-options {
  display: flex;
  gap: 0.75rem;
}

.visibility-option {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.375rem;
  padding: 1rem;
  background: #f8fafc;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s;
}

.visibility-option:hover {
  border-color: #cbd5e1;
}

.visibility-option.active {
  background: #f0fdfa;
  border-color: #14b8a6;
}

.visibility-option i {
  font-size: 1.25rem;
  color: #64748b;
}

.visibility-option.active i {
  color: #0f766e;
}

.visibility-option span {
  font-size: 0.875rem;
  font-weight: 600;
  color: #374151;
}

.visibility-option small {
  font-size: 0.75rem;
  color: #94a3b8;
  text-align: center;
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
