<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// View state
const isLoading = ref(false)
const selectedLibrary = ref<string | null>(null)
const searchQuery = ref('')
const viewMode = ref<'grid' | 'list'>('grid')
const selectedFileType = ref<string | null>(null)
const sortBy = ref<'name' | 'date' | 'size'>('date')
const sortOrder = ref<'asc' | 'desc'>('desc')

// Mock Libraries Data
const libraries = ref([
  {
    id: 'tournament-docs',
    name: 'Tournament Documents',
    icon: 'fas fa-trophy',
    color: '#14b8a6',
    documentCount: 45,
    description: 'Official AFC Asian Cup 2027 documents',
    lastUpdated: '2 hours ago'
  },
  {
    id: 'media-assets',
    name: 'Media Assets',
    icon: 'fas fa-photo-film',
    color: '#8b5cf6',
    documentCount: 128,
    description: 'Press kits, logos, and brand assets',
    lastUpdated: '1 day ago'
  },
  {
    id: 'venue-info',
    name: 'Venue Information',
    icon: 'fas fa-stadium',
    color: '#f59e0b',
    documentCount: 32,
    description: 'Stadium guides and facility maps',
    lastUpdated: '3 days ago'
  },
  {
    id: 'team-resources',
    name: 'Team Resources',
    icon: 'fas fa-users',
    color: '#ef4444',
    documentCount: 67,
    description: 'Team rosters and player profiles',
    lastUpdated: '5 hours ago'
  },
  {
    id: 'operations',
    name: 'Operations Manual',
    icon: 'fas fa-cogs',
    color: '#3b82f6',
    documentCount: 24,
    description: 'Standard operating procedures',
    lastUpdated: '1 week ago'
  },
  {
    id: 'accreditation',
    name: 'Accreditation',
    icon: 'fas fa-id-badge',
    color: '#10b981',
    documentCount: 18,
    description: 'Access passes and credentials',
    lastUpdated: '4 days ago'
  }
])

// Mock Documents Data
const documents = ref([
  {
    id: '1',
    name: 'AFC Asian Cup 2027 - Tournament Regulations.pdf',
    type: 'pdf',
    size: 4521984,
    libraryId: 'tournament-docs',
    createdAt: '2027-01-03T10:30:00Z',
    updatedAt: '2027-01-05T14:20:00Z',
    author: { name: 'AFC Legal', initials: 'AL', color: '#006847' },
    tags: ['Regulations', 'Official'],
    downloads: 1245,
    isPinned: true,
    isShared: true,
    thumbnail: null
  },
  {
    id: '2',
    name: 'Opening Ceremony Script - Final Draft.docx',
    type: 'word',
    size: 2156789,
    libraryId: 'tournament-docs',
    createdAt: '2027-01-02T09:15:00Z',
    updatedAt: '2027-01-04T16:45:00Z',
    author: { name: 'Events Team', initials: 'ET', color: '#8b5cf6' },
    tags: ['Ceremony', 'Script'],
    downloads: 89,
    isPinned: true,
    isShared: false,
    thumbnail: null
  },
  {
    id: '3',
    name: 'King Fahd Stadium - Seating Chart.xlsx',
    type: 'excel',
    size: 1876543,
    libraryId: 'venue-info',
    createdAt: '2027-01-01T11:00:00Z',
    updatedAt: '2027-01-03T10:30:00Z',
    author: { name: 'Venue Ops', initials: 'VO', color: '#f59e0b' },
    tags: ['Stadium', 'Seating'],
    downloads: 456,
    isPinned: false,
    isShared: true,
    thumbnail: null
  },
  {
    id: '4',
    name: 'Media Accreditation Guidelines 2027.pdf',
    type: 'pdf',
    size: 3245678,
    libraryId: 'accreditation',
    createdAt: '2026-12-28T14:00:00Z',
    updatedAt: '2027-01-02T09:00:00Z',
    author: { name: 'Media Dept', initials: 'MD', color: '#3b82f6' },
    tags: ['Media', 'Guidelines'],
    downloads: 2341,
    isPinned: false,
    isShared: true,
    thumbnail: null
  },
  {
    id: '5',
    name: 'Team Presentation Template.pptx',
    type: 'powerpoint',
    size: 8765432,
    libraryId: 'team-resources',
    createdAt: '2026-12-25T10:00:00Z',
    updatedAt: '2027-01-01T15:30:00Z',
    author: { name: 'Brand Team', initials: 'BT', color: '#ef4444' },
    tags: ['Template', 'Presentation'],
    downloads: 567,
    isPinned: true,
    isShared: false,
    thumbnail: null
  },
  {
    id: '6',
    name: 'AFC Brand Guidelines - Full Kit.zip',
    type: 'archive',
    size: 156789012,
    libraryId: 'media-assets',
    createdAt: '2026-12-20T09:00:00Z',
    updatedAt: '2026-12-30T11:00:00Z',
    author: { name: 'AFC Media', initials: 'AM', color: '#006847' },
    tags: ['Brand', 'Assets'],
    downloads: 3456,
    isPinned: true,
    isShared: true,
    thumbnail: null
  },
  {
    id: '7',
    name: 'Match Day Operations Checklist.pdf',
    type: 'pdf',
    size: 1234567,
    libraryId: 'operations',
    createdAt: '2026-12-18T08:00:00Z',
    updatedAt: '2027-01-04T12:00:00Z',
    author: { name: 'Ops Manager', initials: 'OM', color: '#10b981' },
    tags: ['Operations', 'Checklist'],
    downloads: 789,
    isPinned: false,
    isShared: false,
    thumbnail: null
  },
  {
    id: '8',
    name: 'Press Conference Schedule - Group Stage.xlsx',
    type: 'excel',
    size: 987654,
    libraryId: 'media-assets',
    createdAt: '2027-01-04T16:00:00Z',
    updatedAt: '2027-01-05T10:00:00Z',
    author: { name: 'Press Office', initials: 'PO', color: '#8b5cf6' },
    tags: ['Schedule', 'Press'],
    downloads: 234,
    isPinned: false,
    isShared: true,
    thumbnail: null
  },
  {
    id: '9',
    name: 'Volunteer Training Manual.pdf',
    type: 'pdf',
    size: 5678901,
    libraryId: 'operations',
    createdAt: '2026-12-15T10:00:00Z',
    updatedAt: '2027-01-03T14:00:00Z',
    author: { name: 'HR Team', initials: 'HR', color: '#f59e0b' },
    tags: ['Training', 'Volunteers'],
    downloads: 1567,
    isPinned: false,
    isShared: true,
    thumbnail: null
  },
  {
    id: '10',
    name: 'Stadium WiFi Network Guide.pdf',
    type: 'pdf',
    size: 2345678,
    libraryId: 'venue-info',
    createdAt: '2027-01-02T11:00:00Z',
    updatedAt: '2027-01-05T09:00:00Z',
    author: { name: 'IT Support', initials: 'IT', color: '#3b82f6' },
    tags: ['Technical', 'WiFi'],
    downloads: 432,
    isPinned: false,
    isShared: false,
    thumbnail: null
  },
  {
    id: '11',
    name: 'Saudi Arabia Team Photo - High Res.jpg',
    type: 'image',
    size: 12345678,
    libraryId: 'team-resources',
    createdAt: '2027-01-05T08:00:00Z',
    updatedAt: '2027-01-05T08:00:00Z',
    author: { name: 'Photo Team', initials: 'PT', color: '#ef4444' },
    tags: ['Photo', 'Saudi Arabia'],
    downloads: 876,
    isPinned: false,
    isShared: true,
    thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=200&h=150&fit=crop'
  },
  {
    id: '12',
    name: 'Tournament Anthem - Preview.mp3',
    type: 'audio',
    size: 8765432,
    libraryId: 'media-assets',
    createdAt: '2027-01-04T14:00:00Z',
    updatedAt: '2027-01-04T14:00:00Z',
    author: { name: 'Audio Team', initials: 'AT', color: '#8b5cf6' },
    tags: ['Audio', 'Anthem'],
    downloads: 2345,
    isPinned: true,
    isShared: true,
    thumbnail: null
  }
])

// File type filters
const fileTypes = [
  { id: 'pdf', label: 'PDF', icon: 'fas fa-file-pdf', color: 'text-red-500' },
  { id: 'word', label: 'Word', icon: 'fas fa-file-word', color: 'text-blue-500' },
  { id: 'excel', label: 'Excel', icon: 'fas fa-file-excel', color: 'text-green-500' },
  { id: 'powerpoint', label: 'PowerPoint', icon: 'fas fa-file-powerpoint', color: 'text-orange-500' },
  { id: 'image', label: 'Images', icon: 'fas fa-file-image', color: 'text-purple-500' },
  { id: 'audio', label: 'Audio', icon: 'fas fa-file-audio', color: 'text-indigo-500' },
  { id: 'archive', label: 'Archives', icon: 'fas fa-file-archive', color: 'text-yellow-600' }
]

// Computed
const filteredDocuments = computed(() => {
  let result = [...documents.value]

  if (selectedLibrary.value) {
    result = result.filter(d => d.libraryId === selectedLibrary.value)
  }

  if (selectedFileType.value) {
    result = result.filter(d => d.type === selectedFileType.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(d =>
      d.name.toLowerCase().includes(query) ||
      d.tags.some(tag => tag.toLowerCase().includes(query))
    )
  }

  // Sort
  result.sort((a, b) => {
    let comparison = 0
    if (sortBy.value === 'name') {
      comparison = a.name.localeCompare(b.name)
    } else if (sortBy.value === 'date') {
      comparison = new Date(a.updatedAt).getTime() - new Date(b.updatedAt).getTime()
    } else if (sortBy.value === 'size') {
      comparison = a.size - b.size
    }
    return sortOrder.value === 'asc' ? comparison : -comparison
  })

  return result
})

const pinnedDocuments = computed(() => documents.value.filter(d => d.isPinned))

const documentStats = computed(() => ({
  totalDocuments: documents.value.length,
  totalLibraries: libraries.value.length,
  totalSize: documents.value.reduce((acc, doc) => acc + (doc.size || 0), 0),
  recentUploads: documents.value.filter(d => {
    const uploadDate = new Date(d.createdAt)
    const weekAgo = new Date()
    weekAgo.setDate(weekAgo.getDate() - 7)
    return uploadDate > weekAgo
  }).length
}))

const selectedLibraryInfo = computed(() => {
  if (!selectedLibrary.value) return null
  return libraries.value.find(lib => lib.id === selectedLibrary.value)
})

// Interactions
const favoriteDocuments = ref(new Set<string>())

function toggleFavorite(id: string) {
  if (favoriteDocuments.value.has(id)) {
    favoriteDocuments.value.delete(id)
  } else {
    favoriteDocuments.value.add(id)
  }
}

function isFavorite(id: string): boolean {
  return favoriteDocuments.value.has(id)
}

function downloadDocument(doc: any) {
  console.log('Downloading:', doc.name)
}

function shareDocument(doc: any) {
  console.log('Sharing:', doc.name)
}

function viewDocument(doc: any) {
  router.push({ name: 'DocumentView', params: { id: doc.id } })
}

function clearFilters() {
  selectedLibrary.value = null
  selectedFileType.value = null
  searchQuery.value = ''
}

// Helpers
function formatFileSize(bytes: number): string {
  if (bytes < 1024) return bytes + ' B'
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB'
  if (bytes < 1024 * 1024 * 1024) return (bytes / (1024 * 1024)).toFixed(1) + ' MB'
  return (bytes / (1024 * 1024 * 1024)).toFixed(1) + ' GB'
}

function formatTotalSize(bytes: number): string {
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(0) + ' KB'
  if (bytes < 1024 * 1024 * 1024) return (bytes / (1024 * 1024)).toFixed(1) + ' MB'
  return (bytes / (1024 * 1024 * 1024)).toFixed(1) + ' GB'
}

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  })
}

function getRelativeTime(dateString: string): string {
  const date = new Date(dateString)
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffHours = Math.floor(diffMs / (1000 * 60 * 60))
  const diffDays = Math.floor(diffHours / 24)

  if (diffHours < 1) return 'Just now'
  if (diffHours < 24) return `${diffHours}h ago`
  if (diffDays < 7) return `${diffDays}d ago`
  return formatDate(dateString)
}

function getFileIcon(type: string): string {
  const icons: Record<string, string> = {
    pdf: 'fas fa-file-pdf',
    word: 'fas fa-file-word',
    excel: 'fas fa-file-excel',
    powerpoint: 'fas fa-file-powerpoint',
    image: 'fas fa-file-image',
    video: 'fas fa-file-video',
    audio: 'fas fa-file-audio',
    archive: 'fas fa-file-archive',
  }
  return icons[type] || 'fas fa-file'
}

function getFileIconColor(type: string): string {
  const colors: Record<string, string> = {
    pdf: 'text-red-500',
    word: 'text-blue-500',
    excel: 'text-green-500',
    powerpoint: 'text-orange-500',
    image: 'text-purple-500',
    video: 'text-pink-500',
    audio: 'text-indigo-500',
    archive: 'text-yellow-600',
  }
  return colors[type] || 'text-gray-500'
}

function getFileIconBg(type: string): string {
  const colors: Record<string, string> = {
    pdf: 'bg-red-50',
    word: 'bg-blue-50',
    excel: 'bg-green-50',
    powerpoint: 'bg-orange-50',
    image: 'bg-purple-50',
    video: 'bg-pink-50',
    audio: 'bg-indigo-50',
    archive: 'bg-yellow-50',
  }
  return colors[type] || 'bg-gray-50'
}
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Hero Section -->
    <div class="hero-gradient relative overflow-hidden">
      <!-- Decorative elements with animations -->
      <div class="circle-drift-1 absolute top-0 right-0 w-96 h-96 bg-white/5 rounded-full"></div>
      <div class="circle-drift-2 absolute bottom-0 left-0 w-64 h-64 bg-white/5 rounded-full"></div>
      <div class="circle-drift-3 absolute top-1/2 right-1/4 w-32 h-32 bg-white/10 rounded-full"></div>

      <!-- Stats - Absolute Top Right -->
      <div class="stats-top-right">
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-file-alt"></i>
          </div>
          <p class="stat-value-mini">{{ documentStats.totalDocuments }}</p>
          <p class="stat-label-mini">Documents</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-folder"></i>
          </div>
          <p class="stat-value-mini">{{ documentStats.totalLibraries }}</p>
          <p class="stat-label-mini">Libraries</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-hdd"></i>
          </div>
          <p class="stat-value-mini">{{ formatTotalSize(documentStats.totalSize) }}</p>
          <p class="stat-label-mini">Total Size</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-clock"></i>
          </div>
          <p class="stat-value-mini">{{ documentStats.recentUploads }}</p>
          <p class="stat-label-mini">This Week</p>
        </div>
      </div>

      <div class="relative px-8 py-8">
        <div class="px-3 py-1 bg-white/20 backdrop-blur-sm rounded-full text-white text-xs font-semibold inline-flex items-center gap-2 mb-4">
          <i class="fas fa-folder-open"></i>
          AFC Asian Cup 2027
        </div>

        <h1 class="text-3xl font-bold text-white mb-2">Document Library</h1>
        <p class="text-teal-100 max-w-lg">Access official tournament documents, media assets, and operational resources.</p>

        <div class="flex flex-wrap gap-3 mt-6">
          <button class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
            <i class="fas fa-upload"></i>
            Upload Document
          </button>
          <button class="px-5 py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-xl font-semibold text-sm hover:bg-white/30 transition-all flex items-center gap-2">
            <i class="fas fa-folder-plus"></i>
            New Folder
          </button>
        </div>
      </div>
    </div>

    <div class="px-8 py-6 space-y-6">
      <!-- Quick Access / Pinned Documents -->
      <div v-if="pinnedDocuments.length > 0" class="bg-white rounded-2xl p-6 shadow-sm border border-gray-100">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-400 to-orange-500 flex items-center justify-center shadow-lg">
              <i class="fas fa-thumbtack text-white text-sm"></i>
            </div>
            <div>
              <span class="block">Quick Access</span>
              <span class="text-xs font-medium text-gray-500">Pinned documents</span>
            </div>
          </h2>
        </div>

        <div class="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-3">
          <div
            v-for="doc in pinnedDocuments"
            :key="'pinned-' + doc.id"
            @click="viewDocument(doc)"
            class="group p-3 rounded-xl border border-gray-100 hover:border-teal-200 hover:bg-teal-50/50 cursor-pointer transition-all"
          >
            <div :class="['w-10 h-10 rounded-lg flex items-center justify-center mb-2', getFileIconBg(doc.type)]">
              <i :class="[getFileIcon(doc.type), getFileIconColor(doc.type), 'text-lg']"></i>
            </div>
            <p class="text-xs font-medium text-gray-900 truncate group-hover:text-teal-700">{{ doc.name }}</p>
            <p class="text-[10px] text-gray-400 mt-0.5">{{ formatFileSize(doc.size) }}</p>
          </div>
        </div>
      </div>

      <!-- Libraries Section -->
      <div class="bg-white rounded-2xl p-6 shadow-sm border border-gray-100">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-folder-tree text-white text-sm"></i>
            </div>
            <div>
              <span class="block">Libraries</span>
              <span class="text-xs font-medium text-teal-600">Browse by category</span>
            </div>
          </h2>
          <button v-if="selectedLibrary" @click="selectedLibrary = null" class="text-sm text-teal-600 hover:text-teal-700 font-medium flex items-center gap-1">
            <i class="fas fa-times"></i>
            Clear selection
          </button>
        </div>

        <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
          <button
            v-for="lib in libraries"
            :key="lib.id"
            @click="selectedLibrary = selectedLibrary === lib.id ? null : lib.id"
            :class="[
              'p-4 rounded-xl text-left transition-all border-2',
              selectedLibrary === lib.id
                ? 'border-teal-500 bg-teal-50 shadow-lg shadow-teal-100'
                : 'border-transparent bg-gray-50 hover:bg-gray-100 hover:shadow-md'
            ]"
          >
            <div
              class="w-12 h-12 rounded-xl flex items-center justify-center text-white mb-3 shadow-lg"
              :style="{ backgroundColor: lib.color }"
            >
              <i :class="[lib.icon, 'text-lg']"></i>
            </div>
            <p class="font-semibold text-gray-900 text-sm truncate">{{ lib.name }}</p>
            <p class="text-xs text-gray-500 mt-0.5">{{ lib.documentCount }} files</p>
            <p class="text-[10px] text-gray-400 mt-1">{{ lib.lastUpdated }}</p>
          </button>
        </div>
      </div>

      <!-- Filters & Search -->
      <div class="bg-white rounded-2xl p-6 shadow-sm border border-gray-100">
        <div class="flex flex-col lg:flex-row gap-4">
          <!-- Search -->
          <div class="flex-1 relative">
            <i class="fas fa-search absolute left-4 top-1/2 -translate-y-1/2 text-gray-400"></i>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search documents by name or tags..."
              class="w-full pl-11 pr-4 py-3 bg-gray-50 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-all"
            >
          </div>

          <!-- File Type Filter -->
          <div class="flex items-center gap-2 overflow-x-auto pb-2 lg:pb-0">
            <button
              v-for="type in fileTypes"
              :key="type.id"
              @click="selectedFileType = selectedFileType === type.id ? null : type.id"
              :class="[
                'flex items-center gap-2 px-3 py-2 rounded-lg text-xs font-medium whitespace-nowrap transition-all',
                selectedFileType === type.id
                  ? 'bg-teal-500 text-white shadow-lg shadow-teal-200'
                  : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
              ]"
            >
              <i :class="[type.icon, selectedFileType === type.id ? 'text-white' : type.color]"></i>
              {{ type.label }}
            </button>
          </div>

          <!-- Sort & View Toggle -->
          <div class="flex items-center gap-2">
            <select
              v-model="sortBy"
              class="px-3 py-2 bg-gray-100 border-0 rounded-lg text-sm text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500"
            >
              <option value="date">Sort by Date</option>
              <option value="name">Sort by Name</option>
              <option value="size">Sort by Size</option>
            </select>

            <button
              @click="sortOrder = sortOrder === 'asc' ? 'desc' : 'asc'"
              class="p-2.5 bg-gray-100 rounded-lg text-gray-600 hover:bg-gray-200 transition-all"
            >
              <i :class="sortOrder === 'asc' ? 'fas fa-sort-amount-up' : 'fas fa-sort-amount-down'"></i>
            </button>

            <div class="flex gap-1 bg-gray-100 p-1 rounded-lg">
              <button
                @click="viewMode = 'grid'"
                :class="['p-2 rounded-md transition-all', viewMode === 'grid' ? 'bg-white shadow-sm text-teal-600' : 'text-gray-500 hover:text-gray-700']"
              >
                <i class="fas fa-th-large"></i>
              </button>
              <button
                @click="viewMode = 'list'"
                :class="['p-2 rounded-md transition-all', viewMode === 'list' ? 'bg-white shadow-sm text-teal-600' : 'text-gray-500 hover:text-gray-700']"
              >
                <i class="fas fa-list"></i>
              </button>
            </div>
          </div>
        </div>

        <!-- Active Filters -->
        <div v-if="selectedLibrary || selectedFileType || searchQuery" class="flex items-center gap-2 mt-4 pt-4 border-t border-gray-100">
          <span class="text-xs text-gray-500">Active filters:</span>
          <div class="flex flex-wrap gap-2">
            <span v-if="selectedLibraryInfo" class="px-2.5 py-1 bg-teal-100 text-teal-700 rounded-full text-xs font-medium flex items-center gap-1">
              <i :class="selectedLibraryInfo.icon" class="text-[10px]"></i>
              {{ selectedLibraryInfo.name }}
              <button @click="selectedLibrary = null" class="ml-1 hover:text-teal-900"><i class="fas fa-times"></i></button>
            </span>
            <span v-if="selectedFileType" class="px-2.5 py-1 bg-blue-100 text-blue-700 rounded-full text-xs font-medium flex items-center gap-1">
              {{ fileTypes.find(t => t.id === selectedFileType)?.label }}
              <button @click="selectedFileType = null" class="ml-1 hover:text-blue-900"><i class="fas fa-times"></i></button>
            </span>
            <span v-if="searchQuery" class="px-2.5 py-1 bg-purple-100 text-purple-700 rounded-full text-xs font-medium flex items-center gap-1">
              "{{ searchQuery }}"
              <button @click="searchQuery = ''" class="ml-1 hover:text-purple-900"><i class="fas fa-times"></i></button>
            </span>
          </div>
          <button @click="clearFilters" class="text-xs text-gray-500 hover:text-gray-700 ml-auto">Clear all</button>
        </div>
      </div>

      <!-- Results Count -->
      <div class="flex items-center justify-between">
        <p class="text-sm text-gray-500">
          Showing <span class="font-semibold text-gray-900">{{ filteredDocuments.length }}</span> documents
        </p>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="bg-white rounded-2xl p-12 shadow-sm border border-gray-100 text-center">
        <div class="w-12 h-12 border-4 border-teal-500 border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
        <p class="text-gray-500">Loading documents...</p>
      </div>

      <!-- Empty State -->
      <div v-else-if="filteredDocuments.length === 0" class="bg-white rounded-2xl p-12 shadow-sm border border-gray-100 text-center">
        <div class="w-20 h-20 bg-gray-100 rounded-2xl flex items-center justify-center mx-auto mb-4">
          <i class="fas fa-folder-open text-3xl text-gray-300"></i>
        </div>
        <h3 class="text-lg font-semibold text-gray-700">No documents found</h3>
        <p class="text-gray-500 mt-1 mb-4">Try adjusting your filters or upload a new document</p>
        <button @click="clearFilters" class="px-4 py-2 bg-teal-500 text-white rounded-lg text-sm font-medium hover:bg-teal-600 transition-colors">
          Clear Filters
        </button>
      </div>

      <!-- Documents Grid View -->
      <div v-else-if="viewMode === 'grid'" class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-4">
        <div
          v-for="doc in filteredDocuments"
          :key="doc.id"
          @click="viewDocument(doc)"
          class="group bg-white rounded-2xl p-4 shadow-sm border border-gray-100 hover:shadow-lg hover:border-teal-200 cursor-pointer transition-all"
        >
          <!-- Thumbnail / Icon -->
          <div class="relative mb-3">
            <div v-if="doc.thumbnail" class="h-32 rounded-xl overflow-hidden">
              <img :src="doc.thumbnail" :alt="doc.name" class="w-full h-full object-cover group-hover:scale-105 transition-transform">
            </div>
            <div v-else :class="['h-32 rounded-xl flex items-center justify-center', getFileIconBg(doc.type)]">
              <i :class="[getFileIcon(doc.type), getFileIconColor(doc.type), 'text-4xl']"></i>
            </div>

            <!-- Quick Actions -->
            <div class="absolute top-2 right-2 flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
              <button
                @click.stop="toggleFavorite(doc.id)"
                :class="[
                  'w-8 h-8 rounded-lg flex items-center justify-center transition-all',
                  isFavorite(doc.id) ? 'bg-amber-500 text-white' : 'bg-white/90 text-gray-600 hover:bg-white'
                ]"
              >
                <i :class="isFavorite(doc.id) ? 'fas fa-star' : 'far fa-star'" class="text-sm"></i>
              </button>
              <button
                @click.stop="downloadDocument(doc)"
                class="w-8 h-8 rounded-lg bg-white/90 text-gray-600 hover:bg-white flex items-center justify-center transition-all"
              >
                <i class="fas fa-download text-sm"></i>
              </button>
            </div>

            <!-- Badges -->
            <div class="absolute top-2 left-2 flex flex-col gap-1">
              <span v-if="doc.isPinned" class="px-2 py-0.5 bg-amber-500 text-white text-[10px] font-bold rounded-full">
                <i class="fas fa-thumbtack mr-0.5"></i> Pinned
              </span>
              <span v-if="doc.isShared" class="px-2 py-0.5 bg-blue-500 text-white text-[10px] font-bold rounded-full">
                <i class="fas fa-share-alt mr-0.5"></i> Shared
              </span>
            </div>
          </div>

          <!-- Info -->
          <h4 class="font-semibold text-gray-900 text-sm truncate group-hover:text-teal-600 transition-colors">{{ doc.name }}</h4>

          <!-- Tags -->
          <div class="flex flex-wrap gap-1 mt-2">
            <span v-for="tag in doc.tags.slice(0, 2)" :key="tag" class="px-1.5 py-0.5 bg-gray-100 text-gray-500 rounded text-[10px]">
              {{ tag }}
            </span>
          </div>

          <!-- Meta -->
          <div class="flex items-center justify-between mt-3 pt-3 border-t border-gray-100">
            <div class="flex items-center gap-2">
              <div
                class="w-6 h-6 rounded-md flex items-center justify-center text-white text-[10px] font-bold"
                :style="{ backgroundColor: doc.author.color }"
              >
                {{ doc.author.initials }}
              </div>
              <span class="text-[10px] text-gray-400">{{ getRelativeTime(doc.updatedAt) }}</span>
            </div>
            <span class="text-[10px] text-gray-400">{{ formatFileSize(doc.size) }}</span>
          </div>
        </div>
      </div>

      <!-- Documents List View -->
      <div v-else class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
        <div
          v-for="(doc, index) in filteredDocuments"
          :key="doc.id"
          @click="viewDocument(doc)"
          :class="[
            'flex items-center gap-4 p-4 cursor-pointer hover:bg-teal-50/50 transition-colors',
            index !== filteredDocuments.length - 1 ? 'border-b border-gray-100' : ''
          ]"
        >
          <!-- Icon -->
          <div :class="['w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0', getFileIconBg(doc.type)]">
            <i :class="[getFileIcon(doc.type), getFileIconColor(doc.type), 'text-xl']"></i>
          </div>

          <!-- Info -->
          <div class="flex-1 min-w-0">
            <div class="flex items-center gap-2">
              <h4 class="font-semibold text-gray-900 text-sm truncate hover:text-teal-600 transition-colors">{{ doc.name }}</h4>
              <span v-if="doc.isPinned" class="px-1.5 py-0.5 bg-amber-100 text-amber-700 text-[10px] font-bold rounded-full">
                <i class="fas fa-thumbtack"></i>
              </span>
              <span v-if="doc.isShared" class="px-1.5 py-0.5 bg-blue-100 text-blue-700 text-[10px] font-bold rounded-full">
                <i class="fas fa-share-alt"></i>
              </span>
            </div>
            <div class="flex items-center gap-3 mt-1 text-xs text-gray-500">
              <span class="flex items-center gap-1">
                <div
                  class="w-4 h-4 rounded flex items-center justify-center text-white text-[8px] font-bold"
                  :style="{ backgroundColor: doc.author.color }"
                >
                  {{ doc.author.initials }}
                </div>
                {{ doc.author.name }}
              </span>
              <span>{{ formatFileSize(doc.size) }}</span>
              <span>{{ getRelativeTime(doc.updatedAt) }}</span>
              <span class="flex items-center gap-1"><i class="fas fa-download text-gray-400"></i> {{ doc.downloads }}</span>
            </div>
          </div>

          <!-- Tags -->
          <div class="hidden lg:flex items-center gap-1">
            <span v-for="tag in doc.tags" :key="tag" class="px-2 py-1 bg-gray-100 text-gray-600 rounded-lg text-xs">
              {{ tag }}
            </span>
          </div>

          <!-- Actions -->
          <div class="flex items-center gap-1">
            <button
              @click.stop="toggleFavorite(doc.id)"
              :class="[
                'w-9 h-9 rounded-lg flex items-center justify-center transition-all',
                isFavorite(doc.id) ? 'bg-amber-100 text-amber-600' : 'bg-gray-100 text-gray-500 hover:bg-gray-200'
              ]"
            >
              <i :class="isFavorite(doc.id) ? 'fas fa-star' : 'far fa-star'"></i>
            </button>
            <button
              @click.stop="downloadDocument(doc)"
              class="w-9 h-9 rounded-lg bg-gray-100 text-gray-500 hover:bg-teal-100 hover:text-teal-600 flex items-center justify-center transition-all"
            >
              <i class="fas fa-download"></i>
            </button>
            <button
              @click.stop="shareDocument(doc)"
              class="w-9 h-9 rounded-lg bg-gray-100 text-gray-500 hover:bg-blue-100 hover:text-blue-600 flex items-center justify-center transition-all"
            >
              <i class="fas fa-share-alt"></i>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.hero-gradient {
  background: linear-gradient(135deg, #0d9488 0%, #14b8a6 50%, #10b981 100%);
}

.stats-top-right {
  position: absolute;
  top: 24px;
  right: 32px;
  display: flex;
  align-items: flex-start;
  gap: 12px;
  z-index: 10;
}

.stat-card-square {
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(8px);
  border-radius: 16px;
  width: 115px;
  height: 115px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 8px;
  text-align: center;
  transition: all 0.3s ease;
  cursor: pointer;
}

.stat-card-square:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: translateY(-4px) scale(1.02);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
}

.stat-card-square:hover .stat-icon-box {
  transform: scale(1.1);
}

.stat-icon-box {
  color: white;
  font-size: 20px;
  transition: transform 0.3s ease;
}

.stat-value-mini {
  font-size: 24px;
  font-weight: 700;
  color: white;
  line-height: 1;
}

.stat-label-mini {
  font-size: 11px;
  color: rgba(255, 255, 255, 0.7);
  line-height: 1;
}

/* Circle Drift Animations */
.circle-drift-1 {
  animation: drift-1 20s ease-in-out infinite;
}

.circle-drift-2 {
  animation: drift-2 25s ease-in-out infinite;
}

.circle-drift-3 {
  animation: drift-3 18s ease-in-out infinite;
}

@keyframes drift-1 {
  0%, 100% {
    transform: translate(33%, -50%);
  }
  25% {
    transform: translate(28%, -45%);
  }
  50% {
    transform: translate(35%, -55%);
  }
  75% {
    transform: translate(30%, -48%);
  }
}

@keyframes drift-2 {
  0%, 100% {
    transform: translate(-33%, 50%);
  }
  33% {
    transform: translate(-28%, 45%);
  }
  66% {
    transform: translate(-38%, 55%);
  }
}

@keyframes drift-3 {
  0%, 100% {
    transform: translate(0, 0) scale(1);
  }
  50% {
    transform: translate(10px, -10px) scale(1.1);
  }
}

@media (max-width: 1023px) {
  .stats-top-right {
    position: relative;
    top: auto;
    right: auto;
    margin: 24px 32px 0;
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 8px;
  }

  .stat-card-square {
    width: 100%;
    height: 80px;
  }

  .stat-icon-box {
    font-size: 14px;
  }

  .stat-value-mini {
    font-size: 16px;
  }

  .stat-label-mini {
    font-size: 8px;
  }
}
</style>
