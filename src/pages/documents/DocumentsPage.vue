<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import ContentActionsDropdown from '@/components/common/ContentActionsDropdown.vue'
import AddToCollectionModal from '@/components/common/AddToCollectionModal.vue'
import { useAIServicesStore } from '@/stores/aiServices'
import { useComparisonStore } from '@/stores/comparison'
import { AISuggestionChip, AILoadingIndicator, AIConfidenceBar } from '@/components/ai'
import ComparisonPanel from '@/components/ai/ComparisonPanel.vue'
import ComparisonModal from '@/components/ai/ComparisonModal.vue'
import type { ComparisonItem } from '@/types'
import type { ClassificationResult } from '@/types/ai'

const router = useRouter()
const aiStore = useAIServicesStore()
const comparisonStore = useComparisonStore()

// View state
const isLoading = ref(false)
const selectedLibrary = ref<string | null>(null)
const searchQuery = ref('')
const viewMode = ref<'grid' | 'list'>('grid')
const selectedFileTypes = ref<string[]>([])
const sortBy = ref<'name' | 'date' | 'size'>('date')
const sortOrder = ref<'asc' | 'desc'>('desc')
const selectedFolder = ref<string | null>(null)
const expandedFolders = ref(new Set<string>(['root', 'tournament', 'media']))
const isSidebarCollapsed = ref(false)
const showFileTypeFilter = ref(false)
const showCategoryFilter = ref(false)
const showTagFilter = ref(false)
const showSortDropdown = ref(false)
const showStatusFilter = ref(false)
const selectedStatusFilters = ref<string[]>([])

// Status filter options
const statusFilterOptions = [
  { id: 'starred', label: 'My Saved', icon: 'fas fa-bookmark', color: 'text-amber-500' },
  { id: 'shared', label: 'Shared with me', icon: 'fas fa-share-alt', color: 'text-purple-500' }
]

// Sort options with icons
const sortOptionsList = ref([
  { value: 'date', label: 'Date Modified', icon: 'fas fa-clock' },
  { value: 'name', label: 'Name', icon: 'fas fa-font' },
  { value: 'size', label: 'Size', icon: 'fas fa-weight-hanging' }
])

const currentSortOption = computed(() => {
  return sortOptionsList.value.find(opt => opt.value === sortBy.value) ?? sortOptionsList.value[0]!
})

function selectSortOption(value: string) {
  sortBy.value = value as 'name' | 'date' | 'size'
  showSortDropdown.value = false
}
const selectedCategories = ref<string[]>([])
const selectedTags = ref<string[]>([])
const currentView = ref<'all' | 'shared' | 'team' | 'starred' | 'trash'>('all')
const isSelectionMode = ref(false)
const selectedDocuments = ref(new Set<string>())

// Add to Collection modal
const showAddToCollectionModal = ref(false)
const selectedItemForCollection = ref<{
  id: string
  title: string
  thumbnail?: string
} | null>(null)

// ============================================
// AI FEATURES STATE
// ============================================
const showAIFeatures = ref(true)
const showSearchSuggestions = ref(false)
const searchSuggestions = ref<string[]>([])
const isLoadingSearchSuggestions = ref(false)
const searchDebounceTimeout = ref<ReturnType<typeof setTimeout> | null>(null)
const showFolderSuggestions = ref(true)
const isLoadingFolderSuggestions = ref(false)

// Unified Search State
const unifiedSearchQuery = ref('')
const isAISearchMode = ref(false)
const showAISuggestions = ref(false)
const naturalLanguageQuery = ref('')
const aiSearchResults = ref<any[]>([])
const isProcessingNLSearch = ref(false)
const nlSearchSuggestions = [
  'Find tournament regulations',
  'Show me stadium layout documents',
  'Documents about team rosters',
  'Recent press releases',
  'Files shared with me this week'
]

// Mock AI search suggestions for documents
const mockDocSearchSuggestions = [
  'Tournament regulations 2027',
  'Opening ceremony documents',
  'Stadium seating charts',
  'Media accreditation guidelines',
  'Team presentation templates',
  'Brand guidelines and assets',
  'Match day operations checklist',
  'Press conference schedules',
  'Volunteer training manual',
  'WiFi network documentation',
]

// Mock AI classifications for documents (by document ID)
interface AIDocClassification {
  category: string
  confidence: number
  suggestedTags: string[]
  suggestedFolder: string
}

const mockDocClassifications: Record<string, AIDocClassification> = {
  '1': { category: 'Legal & Regulations', confidence: 0.95, suggestedTags: ['Official', 'Legal', 'Tournament'], suggestedFolder: 'regulations' },
  '2': { category: 'Event Planning', confidence: 0.88, suggestedTags: ['Ceremony', 'Script', 'Event'], suggestedFolder: 'ceremonies' },
  '3': { category: 'Venue Operations', confidence: 0.92, suggestedTags: ['Stadium', 'Layout', 'Seating'], suggestedFolder: 'stadiums' },
  '4': { category: 'Media & Press', confidence: 0.90, suggestedTags: ['Media', 'Accreditation', 'Guidelines'], suggestedFolder: 'press' },
  '5': { category: 'Team Resources', confidence: 0.85, suggestedTags: ['Template', 'Presentation', 'Brand'], suggestedFolder: 'teams' },
  '6': { category: 'Brand & Marketing', confidence: 0.94, suggestedTags: ['Brand', 'Assets', 'Guidelines'], suggestedFolder: 'brand' },
  '7': { category: 'Operations', confidence: 0.91, suggestedTags: ['Operations', 'Checklist', 'Match Day'], suggestedFolder: 'procedures' },
  '8': { category: 'Media & Press', confidence: 0.87, suggestedTags: ['Schedule', 'Press', 'Media'], suggestedFolder: 'press' },
  '9': { category: 'Training & HR', confidence: 0.89, suggestedTags: ['Training', 'Volunteers', 'Manual'], suggestedFolder: 'training' },
  '10': { category: 'Technical', confidence: 0.86, suggestedTags: ['Technical', 'WiFi', 'Infrastructure'], suggestedFolder: 'stadiums' },
  '11': { category: 'Team Resources', confidence: 0.93, suggestedTags: ['Photo', 'Team', 'Media'], suggestedFolder: 'photos' },
  '12': { category: 'Media & Press', confidence: 0.82, suggestedTags: ['Audio', 'Anthem', 'Media'], suggestedFolder: 'media' },
}

// Mock AI folder organization suggestions
interface FolderSuggestion {
  documentId: string
  documentName: string
  currentFolder: string
  suggestedFolder: string
  reason: string
  confidence: number
}

const folderSuggestions = ref<FolderSuggestion[]>([
  {
    documentId: '2',
    documentName: 'Opening Ceremony Script - Final Draft.docx',
    currentFolder: 'Tournament Documents',
    suggestedFolder: 'Ceremonies',
    reason: 'Content analysis indicates this is a ceremony-related document',
    confidence: 0.88
  },
  {
    documentId: '8',
    documentName: 'Press Conference Schedule - Group Stage.xlsx',
    currentFolder: 'Media Assets',
    suggestedFolder: 'Press Releases',
    reason: 'Schedule documents are better organized with press materials',
    confidence: 0.85
  },
  {
    documentId: '10',
    documentName: 'Stadium WiFi Network Guide.pdf',
    currentFolder: 'Venue Information',
    suggestedFolder: 'Stadiums',
    reason: 'Technical infrastructure document belongs with stadium files',
    confidence: 0.82
  }
])

// Pagination state
const currentPage = ref(1)
const itemsPerPage = ref(10)
const itemsPerPageOptions = [5, 10, 20, 50, 100]

// View Navigation Items
const viewNavItems = [
  { id: 'all', name: 'All Files', icon: 'fas fa-folder', color: 'text-teal-500' },
  { id: 'shared', name: 'Shared with me', icon: 'fas fa-share-alt', color: 'text-blue-500' },
  { id: 'team', name: 'Team Files', icon: 'fas fa-users', color: 'text-purple-500' },
  { id: 'starred', name: 'Starred', icon: 'fas fa-star', color: 'text-amber-500' },
  { id: 'trash', name: 'Trash', icon: 'fas fa-trash-alt', color: 'text-red-500' }
]

// Folder Tree Structure
const folderTree = ref([
  {
    id: 'root',
    name: 'AFC Asian Cup 2027',
    icon: 'fas fa-trophy',
    children: [
      {
        id: 'tournament',
        name: 'Tournament Documents',
        icon: 'fas fa-folder',
        children: [
          { id: 'regulations', name: 'Regulations', icon: 'fas fa-folder', children: [] },
          { id: 'schedules', name: 'Schedules', icon: 'fas fa-folder', children: [] },
          { id: 'ceremonies', name: 'Ceremonies', icon: 'fas fa-folder', children: [] }
        ]
      },
      {
        id: 'media',
        name: 'Media Assets',
        icon: 'fas fa-folder',
        children: [
          { id: 'brand', name: 'Brand Guidelines', icon: 'fas fa-folder', children: [] },
          { id: 'press', name: 'Press Releases', icon: 'fas fa-folder', children: [] },
          { id: 'photos', name: 'Photos', icon: 'fas fa-folder', children: [] }
        ]
      },
      {
        id: 'venues',
        name: 'Venue Information',
        icon: 'fas fa-folder',
        children: [
          { id: 'stadiums', name: 'Stadiums', icon: 'fas fa-folder', children: [] },
          { id: 'maps', name: 'Maps & Layouts', icon: 'fas fa-folder', children: [] }
        ]
      },
      {
        id: 'teams',
        name: 'Team Resources',
        icon: 'fas fa-folder',
        children: [
          { id: 'rosters', name: 'Rosters', icon: 'fas fa-folder', children: [] },
          { id: 'profiles', name: 'Player Profiles', icon: 'fas fa-folder', children: [] }
        ]
      },
      {
        id: 'operations',
        name: 'Operations',
        icon: 'fas fa-folder',
        children: [
          { id: 'procedures', name: 'Procedures', icon: 'fas fa-folder', children: [] },
          { id: 'training', name: 'Training', icon: 'fas fa-folder', children: [] }
        ]
      },
      {
        id: 'accreditation',
        name: 'Accreditation',
        icon: 'fas fa-folder',
        children: []
      }
    ]
  }
])

function toggleFolder(folderId: string) {
  if (expandedFolders.value.has(folderId)) {
    expandedFolders.value.delete(folderId)
  } else {
    expandedFolders.value.add(folderId)
  }
}

function selectFolder(folderId: string) {
  selectedFolder.value = selectedFolder.value === folderId ? null : folderId
}

function isFolderExpanded(folderId: string): boolean {
  return expandedFolders.value.has(folderId)
}

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
    isShared: true,
    isSharedWithMe: false,
    isTeamFile: true,
    isStarred: true,
    isTrashed: false,
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
    isShared: false,
    isSharedWithMe: true,
    isTeamFile: false,
    isStarred: false,
    isTrashed: false,
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
    isShared: true,
    isSharedWithMe: true,
    isTeamFile: true,
    isStarred: true,
    isTrashed: false,
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
    isShared: true,
    isSharedWithMe: true,
    isTeamFile: false,
    isStarred: false,
    isTrashed: false,
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
    isShared: false,
    isSharedWithMe: false,
    isTeamFile: true,
    isStarred: true,
    isTrashed: false,
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
    isShared: true,
    isSharedWithMe: false,
    isTeamFile: true,
    isStarred: false,
    isTrashed: false,
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
    isShared: false,
    isSharedWithMe: false,
    isTeamFile: false,
    isStarred: false,
    isTrashed: true,
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
    isShared: true,
    isSharedWithMe: true,
    isTeamFile: true,
    isStarred: false,
    isTrashed: false,
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
    isShared: true,
    isSharedWithMe: false,
    isTeamFile: true,
    isStarred: true,
    isTrashed: false,
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
    isShared: false,
    isSharedWithMe: false,
    isTeamFile: false,
    isStarred: false,
    isTrashed: true,
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
    isShared: true,
    isSharedWithMe: true,
    isTeamFile: false,
    isStarred: false,
    isTrashed: false,
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
    isShared: true,
    isSharedWithMe: false,
    isTeamFile: true,
    isStarred: true,
    isTrashed: false,
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

// Computed - All unique tags from documents
const allTags = computed(() => {
  const tags = new Set<string>()
  documents.value.forEach(doc => {
    doc.tags.forEach(tag => tags.add(tag))
  })
  return Array.from(tags).sort()
})

// View counts for sidebar navigation
const viewCounts = computed(() => ({
  all: documents.value.filter(d => !d.isTrashed).length,
  shared: documents.value.filter(d => d.isSharedWithMe && !d.isTrashed).length,
  team: documents.value.filter(d => d.isTeamFile && !d.isTrashed).length,
  starred: documents.value.filter(d => d.isStarred && !d.isTrashed).length,
  trash: documents.value.filter(d => d.isTrashed).length
}))

const filteredDocuments = computed(() => {
  // If AI search mode is active and has results, use those
  if (isAISearchMode.value && aiSearchResults.value.length > 0) {
    return aiSearchResults.value
  }

  let result = [...documents.value]

  // First apply view filter
  switch (currentView.value) {
    case 'all':
      result = result.filter(d => !d.isTrashed)
      break
    case 'shared':
      result = result.filter(d => d.isSharedWithMe && !d.isTrashed)
      break
    case 'team':
      result = result.filter(d => d.isTeamFile && !d.isTrashed)
      break
    case 'starred':
      result = result.filter(d => d.isStarred && !d.isTrashed)
      break
    case 'trash':
      result = result.filter(d => d.isTrashed)
      break
  }

  if (selectedLibrary.value) {
    result = result.filter(d => d.libraryId === selectedLibrary.value)
  }

  if (selectedCategories.value.length > 0) {
    result = result.filter(d => selectedCategories.value.includes(d.libraryId))
  }

  if (selectedFileTypes.value.length > 0) {
    result = result.filter(d => selectedFileTypes.value.includes(d.type))
  }

  if (selectedTags.value.length > 0) {
    result = result.filter(d =>
      selectedTags.value.some(tag => d.tags.includes(tag))
    )
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(d =>
      d.name.toLowerCase().includes(query) ||
      d.tags.some(tag => tag.toLowerCase().includes(query))
    )
  }

  // Filter by status (saved/shared)
  if (selectedStatusFilters.value.length > 0) {
    result = result.filter(d => {
      if (selectedStatusFilters.value.includes('starred') && d.isStarred) return true
      if (selectedStatusFilters.value.includes('shared') && d.isSharedWithMe) return true
      return false
    })
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

// Pagination computed properties
const totalPages = computed(() => Math.ceil(filteredDocuments.value.length / itemsPerPage.value))

const paginatedDocuments = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredDocuments.value.slice(start, end)
})

// Pagination functions
function goToPage(page: number) {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
  }
}

function nextPage() {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

function prevPage() {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

function changeItemsPerPage(count: number) {
  itemsPerPage.value = count
  currentPage.value = 1 // Reset to first page when changing items per page
}

// Reset to first page when filters change
watch(
  [searchQuery, selectedFileTypes, selectedCategories, selectedTags, selectedFolder, currentView, sortBy, sortOrder],
  () => {
    currentPage.value = 1
  }
)

const starredDocuments = computed(() => documents.value.filter(d => d.isStarred && !d.isTrashed))

const recentFiles = computed(() => {
  return [...documents.value]
    .filter(d => !d.isTrashed)
    .sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime())
    .slice(0, 8)
})

const documentStats = computed(() => ({
  totalDocuments: documents.value.filter(d => !d.isTrashed).length,
  totalLibraries: libraries.value.length,
  totalSize: documents.value.filter(d => !d.isTrashed).reduce((acc, doc) => acc + (doc.size || 0), 0),
  recentUploads: documents.value.filter(d => {
    if (d.isTrashed) return false
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
  // Could open share modal or copy link
  alert(`Share link copied for: ${doc.name}`)
}

function copyDocumentLink(docId: string) {
  if (typeof navigator !== 'undefined' && navigator.clipboard) {
    navigator.clipboard.writeText(window.location.origin + '/documents/' + docId)
  }
}

function openAddToCollection(doc: any) {
  selectedItemForCollection.value = {
    id: doc.id,
    title: doc.name,
    thumbnail: doc.thumbnail || undefined
  }
  showAddToCollectionModal.value = true
}

function handleAddedToCollection(collectionIds: string[]) {
  if (collectionIds.length > 0) {
    alert(`Added to ${collectionIds.length} collection(s)!`)
  }
  showAddToCollectionModal.value = false
  selectedItemForCollection.value = null
}

function toggleStar(doc: any) {
  doc.isStarred = !doc.isStarred
  console.log(doc.isStarred ? 'Starred:' : 'Unstarred:', doc.name)
}

function moveToTrash(doc: any) {
  doc.isTrashed = true
  console.log('Moved to trash:', doc.name)
}

function restoreFromTrash(doc: any) {
  doc.isTrashed = false
  console.log('Restored from trash:', doc.name)
}

function permanentlyDelete(doc: any) {
  const index = documents.value.findIndex(d => d.id === doc.id)
  if (index > -1) {
    documents.value.splice(index, 1)
    console.log('Permanently deleted:', doc.name)
  }
}

function viewDocument(doc: any) {
  // Always navigate to document view (checkbox handles selection)
  router.push({ name: 'DocumentView', params: { id: doc.id } })
}

// Selection Functions
function toggleSelectionMode() {
  isSelectionMode.value = !isSelectionMode.value
  if (!isSelectionMode.value) {
    selectedDocuments.value.clear()
  }
}

function toggleDocumentSelection(docId: string) {
  if (selectedDocuments.value.has(docId)) {
    selectedDocuments.value.delete(docId)
  } else {
    selectedDocuments.value.add(docId)
  }
  // Force reactivity update
  selectedDocuments.value = new Set(selectedDocuments.value)
}

function isDocumentSelected(docId: string): boolean {
  return selectedDocuments.value.has(docId)
}

function selectAllDocuments() {
  filteredDocuments.value.forEach(doc => {
    selectedDocuments.value.add(doc.id)
  })
  selectedDocuments.value = new Set(selectedDocuments.value)
}

function clearDocumentSelection() {
  selectedDocuments.value.clear()
  selectedDocuments.value = new Set(selectedDocuments.value)
}

const isAllSelected = computed(() => {
  if (filteredDocuments.value.length === 0) return false
  return filteredDocuments.value.every(doc => selectedDocuments.value.has(doc.id))
})

const isSomeSelected = computed(() => {
  return selectedDocuments.value.size > 0 && !isAllSelected.value
})

const selectedCount = computed(() => selectedDocuments.value.size)

// Bulk Actions
function bulkDownload() {
  const selectedDocs = documents.value.filter(d => selectedDocuments.value.has(d.id))
  console.log('Downloading:', selectedDocs.map(d => d.name))
  alert(`Downloading ${selectedDocs.length} files...`)
}

function bulkShare() {
  const selectedDocs = documents.value.filter(d => selectedDocuments.value.has(d.id))
  console.log('Sharing:', selectedDocs.map(d => d.name))
  alert(`Sharing ${selectedDocs.length} files...`)
}

function bulkExport() {
  const selectedDocs = documents.value.filter(d => selectedDocuments.value.has(d.id))
  console.log('Exporting:', selectedDocs.map(d => d.name))
  alert(`Exporting ${selectedDocs.length} files to ZIP...`)
}

function bulkStar() {
  documents.value.forEach(doc => {
    if (selectedDocuments.value.has(doc.id)) {
      doc.isStarred = true
    }
  })
  clearDocumentSelection()
}

function bulkMoveToTrash() {
  documents.value.forEach(doc => {
    if (selectedDocuments.value.has(doc.id)) {
      doc.isTrashed = true
    }
  })
  clearDocumentSelection()
  isSelectionMode.value = false
}

function bulkRestore() {
  documents.value.forEach(doc => {
    if (selectedDocuments.value.has(doc.id)) {
      doc.isTrashed = false
    }
  })
  clearDocumentSelection()
}

// Comparison Functions
function addToComparison(doc: typeof documents.value[0]) {
  const comparisonItem: ComparisonItem = {
    id: doc.id,
    type: 'document',
    title: doc.name,
    thumbnail: doc.thumbnail || undefined,
    description: `${doc.type.toUpperCase()} document - ${formatFileSize(doc.size)}`,
    metadata: {
      author: doc.author?.name || 'Unknown',
      date: doc.updatedAt,
      size: doc.size,
      category: doc.libraryId,
      tags: doc.tags,
      fileType: doc.type,
    },
  }
  comparisonStore.addItem(comparisonItem)
}

function isInComparison(docId: string): boolean {
  return comparisonStore.isItemSelected(docId)
}

function toggleComparison(doc: typeof documents.value[0]) {
  if (isInComparison(doc.id)) {
    comparisonStore.removeItem(doc.id)
  } else {
    addToComparison(doc)
  }
}

function bulkPermanentDelete() {
  const idsToDelete = Array.from(selectedDocuments.value)
  documents.value = documents.value.filter(d => !idsToDelete.includes(d.id))
  clearDocumentSelection()
  isSelectionMode.value = false
}

function clearFilters() {
  selectedLibrary.value = null
  selectedFileTypes.value = []
  selectedCategories.value = []
  selectedTags.value = []
  searchQuery.value = ''
}

function toggleFileType(typeId: string) {
  const index = selectedFileTypes.value.indexOf(typeId)
  if (index > -1) {
    selectedFileTypes.value.splice(index, 1)
  } else {
    selectedFileTypes.value.push(typeId)
  }
}

function isFileTypeSelected(typeId: string): boolean {
  return selectedFileTypes.value.includes(typeId)
}

function toggleCategory(categoryId: string) {
  const index = selectedCategories.value.indexOf(categoryId)
  if (index > -1) {
    selectedCategories.value.splice(index, 1)
  } else {
    selectedCategories.value.push(categoryId)
  }
}

function isCategorySelected(categoryId: string): boolean {
  return selectedCategories.value.includes(categoryId)
}

function toggleTag(tag: string) {
  const index = selectedTags.value.indexOf(tag)
  if (index > -1) {
    selectedTags.value.splice(index, 1)
  } else {
    selectedTags.value.push(tag)
  }
}

function isTagSelected(tag: string): boolean {
  return selectedTags.value.includes(tag)
}

function toggleStatusFilter(status: string) {
  const index = selectedStatusFilters.value.indexOf(status)
  if (index > -1) {
    selectedStatusFilters.value.splice(index, 1)
  } else {
    selectedStatusFilters.value.push(status)
  }
}

function isStatusSelected(status: string): boolean {
  return selectedStatusFilters.value.includes(status)
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

// ============================================
// AI FEATURES FUNCTIONS
// ============================================

// Get AI search suggestions
async function fetchSearchSuggestions(query: string) {
  if (query.length < 2) {
    showSearchSuggestions.value = false
    searchSuggestions.value = []
    return
  }

  isLoadingSearchSuggestions.value = true
  showSearchSuggestions.value = true

  // Simulate AI processing delay
  await new Promise(resolve => setTimeout(resolve, 300))

  // Filter mock suggestions based on query
  const lowerQuery = query.toLowerCase()
  searchSuggestions.value = mockDocSearchSuggestions
    .filter(s => s.toLowerCase().includes(lowerQuery))
    .slice(0, 5)

  // If no matches, generate suggestions based on query
  if (searchSuggestions.value.length === 0) {
    searchSuggestions.value = [
      `${query} documents`,
      `${query} templates`,
      `Files containing ${query}`,
      `${query} guidelines`,
    ]
  }

  isLoadingSearchSuggestions.value = false
}

// Handle search input with debounce
function handleSearchInput() {
  if (isAISearchMode.value) {
    // In AI mode, don't auto-search, wait for enter or button click
    showAISuggestions.value = !unifiedSearchQuery.value
  } else {
    // In normal mode, apply filter immediately
    searchQuery.value = unifiedSearchQuery.value
  }

  // Debounced suggestions fetch
  if (searchDebounceTimeout.value) {
    clearTimeout(searchDebounceTimeout.value)
  }
  searchDebounceTimeout.value = setTimeout(() => {
    fetchSearchSuggestions(unifiedSearchQuery.value || searchQuery.value)
  }, 300)
}

// Process Natural Language Search
async function processNaturalLanguageSearch() {
  if (!naturalLanguageQuery.value || isProcessingNLSearch.value) return

  isProcessingNLSearch.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 1000))

    const query = naturalLanguageQuery.value.toLowerCase()
    const allDocs = documents.value

    // Simple NL processing
    let results = allDocs

    if (query.includes('regulation') || query.includes('rules')) {
      results = results.filter(d => d.tags?.some((t: string) => t.toLowerCase().includes('regulation')) || d.name.toLowerCase().includes('regulation'))
    }
    if (query.includes('stadium') || query.includes('venue') || query.includes('layout')) {
      results = results.filter(d => d.tags?.some((t: string) => t.toLowerCase().includes('stadium') || t.toLowerCase().includes('venue') || t.toLowerCase().includes('seating')))
    }
    if (query.includes('team') || query.includes('roster')) {
      results = results.filter(d => d.tags?.some((t: string) => t.toLowerCase().includes('team')) || d.name.toLowerCase().includes('team'))
    }
    if (query.includes('press') || query.includes('media')) {
      results = results.filter(d => d.tags?.some((t: string) => t.toLowerCase().includes('media') || t.toLowerCase().includes('press')))
    }
    if (query.includes('shared') || query.includes('shared with me')) {
      results = results.filter(d => d.isShared || d.isSharedWithMe)
    }
    if (query.includes('recent') || query.includes('this week')) {
      results = [...results].sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime())
    }

    aiSearchResults.value = results.slice(0, 12)
  } finally {
    isProcessingNLSearch.value = false
  }
}

// Unified Search Handlers
function handleUnifiedSearch() {
  if (!unifiedSearchQuery.value) return

  if (isAISearchMode.value) {
    // Use AI-powered search
    naturalLanguageQuery.value = unifiedSearchQuery.value
    processNaturalLanguageSearch()
  } else {
    // Use normal text search
    searchQuery.value = unifiedSearchQuery.value
  }
  showAISuggestions.value = false
}

function clearUnifiedSearch() {
  unifiedSearchQuery.value = ''
  searchQuery.value = ''
  naturalLanguageQuery.value = ''
  aiSearchResults.value = []
  showAISuggestions.value = false
}

// Watch for AI mode toggle to show suggestions
watch(isAISearchMode, (newValue) => {
  if (newValue && !unifiedSearchQuery.value) {
    showAISuggestions.value = true
  } else {
    showAISuggestions.value = false
  }
  // Sync the search query when switching modes
  if (!newValue) {
    searchQuery.value = unifiedSearchQuery.value
  }
})

// Select a search suggestion
function selectSearchSuggestion(suggestion: string) {
  unifiedSearchQuery.value = suggestion
  searchQuery.value = suggestion
  showSearchSuggestions.value = false
  showAISuggestions.value = false
}

// Hide search suggestions
function hideSearchSuggestions() {
  setTimeout(() => {
    showSearchSuggestions.value = false
  }, 200)
}

// Get AI classification for a document
function getDocClassification(docId: string): AIDocClassification | null {
  return mockDocClassifications[docId] || null
}

// Apply folder suggestion
function applyFolderSuggestion(suggestion: FolderSuggestion) {
  const doc = documents.value.find(d => d.id === suggestion.documentId)
  if (doc) {
    // In a real app, this would move the document
    console.log(`Moving ${doc.name} to ${suggestion.suggestedFolder}`)
    // Remove from suggestions
    folderSuggestions.value = folderSuggestions.value.filter(
      s => s.documentId !== suggestion.documentId
    )
  }
}

// Dismiss folder suggestion
function dismissFolderSuggestion(documentId: string) {
  folderSuggestions.value = folderSuggestions.value.filter(
    s => s.documentId !== documentId
  )
}

// Toggle AI features visibility
function toggleAIFeatures() {
  showAIFeatures.value = !showAIFeatures.value
}

// Dismiss all folder suggestions
function dismissAllFolderSuggestions() {
  showFolderSuggestions.value = false
}

// Get category color for AI classification
function getCategoryColor(category: string): string {
  const colors: Record<string, string> = {
    'Legal & Regulations': 'bg-red-50 text-red-700 border-red-200',
    'Event Planning': 'bg-purple-50 text-purple-700 border-purple-200',
    'Venue Operations': 'bg-amber-50 text-amber-700 border-amber-200',
    'Media & Press': 'bg-blue-50 text-blue-700 border-blue-200',
    'Team Resources': 'bg-emerald-50 text-emerald-700 border-emerald-200',
    'Brand & Marketing': 'bg-pink-50 text-pink-700 border-pink-200',
    'Operations': 'bg-indigo-50 text-indigo-700 border-indigo-200',
    'Training & HR': 'bg-orange-50 text-orange-700 border-orange-200',
    'Technical': 'bg-cyan-50 text-cyan-700 border-cyan-200',
  }
  return colors[category] || 'bg-gray-50 text-gray-700 border-gray-200'
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
      <!-- Quick Access & Recent Files Row -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Starred Documents -->
        <div v-if="starredDocuments.length > 0" class="bg-white rounded-2xl p-6 shadow-sm border border-gray-100">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-400 to-amber-500 flex items-center justify-center shadow-lg shadow-amber-200">
                <i class="fas fa-star text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Starred</span>
                <span class="text-xs font-medium text-gray-500">Quick access to important files</span>
              </div>
            </h2>
            <button @click="currentView = 'starred'" class="text-sm text-teal-600 hover:text-teal-700 font-medium flex items-center gap-1">
              View all
              <i class="fas fa-arrow-right text-xs"></i>
            </button>
          </div>

          <div class="space-y-2">
            <div
              v-for="doc in starredDocuments.slice(0, 5)"
              :key="'starred-' + doc.id"
              @click="viewDocument(doc)"
              class="group flex items-center gap-3 p-3 rounded-xl bg-white/80 hover:bg-white border border-gray-100 hover:border-teal-200 hover:shadow-lg cursor-pointer transition-all"
            >
              <div :class="['w-10 h-10 rounded-lg flex items-center justify-center flex-shrink-0 transition-transform group-hover:scale-110 group-hover:shadow-md', getFileIconBg(doc.type)]">
                <i :class="[getFileIcon(doc.type), getFileIconColor(doc.type), 'text-lg']"></i>
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium text-gray-900 truncate group-hover:text-teal-600 transition-colors">{{ doc.name }}</p>
                <div class="flex items-center gap-2 mt-0.5">
                  <span class="text-[10px] text-gray-400">{{ formatFileSize(doc.size) }}</span>
                  <span class="text-[10px] text-gray-300">•</span>
                  <span class="text-[10px] text-gray-400 flex items-center gap-1">
                    <i class="fas fa-download text-[8px]"></i>
                    {{ doc.downloads }}
                  </span>
                </div>
              </div>
              <div class="opacity-0 group-hover:opacity-100 transition-opacity flex items-center gap-1">
                <button
                  @click.stop="toggleStar(doc)"
                  class="w-7 h-7 rounded-lg bg-amber-100 text-amber-600 hover:bg-amber-200 flex items-center justify-center transition-all"
                  title="Remove from Starred"
                >
                  <i class="fas fa-star text-xs"></i>
                </button>
                <button
                  @click.stop="shareDocument(doc)"
                  class="w-7 h-7 rounded-lg bg-gray-100 text-gray-400 hover:bg-blue-100 hover:text-blue-600 flex items-center justify-center transition-all"
                  title="Share"
                >
                  <i class="fas fa-share-alt text-xs"></i>
                </button>
                <button
                  @click.stop="downloadDocument(doc)"
                  class="w-7 h-7 rounded-lg bg-gray-100 text-gray-400 hover:bg-teal-100 hover:text-teal-600 flex items-center justify-center transition-all"
                  title="Download"
                >
                  <i class="fas fa-download text-xs"></i>
                </button>
                <ContentActionsDropdown
                  :show-download="true"
                  @add-to-collection="openAddToCollection(doc)"
                  @share="shareDocument(doc)"
                  @download="downloadDocument(doc)"
                  @copy-link="() => copyDocumentLink(doc.id)"
                />
              </div>
            </div>
          </div>
        </div>

        <!-- Recent Files Section -->
        <div class="bg-white rounded-2xl p-6 shadow-sm border border-gray-100">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
                <i class="fas fa-clock-rotate-left text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Recent Files</span>
                <span class="text-xs font-medium text-gray-500">Recently modified</span>
              </div>
            </h2>
            <button class="text-sm text-teal-600 hover:text-teal-700 font-medium flex items-center gap-1">
              View all
              <i class="fas fa-arrow-right text-xs"></i>
            </button>
          </div>

          <div class="space-y-2">
            <div
              v-for="doc in recentFiles.slice(0, 5)"
              :key="'recent-' + doc.id"
              @click="viewDocument(doc)"
              class="group flex items-center gap-3 p-3 rounded-xl bg-white/80 hover:bg-white border border-gray-100 hover:border-teal-200 hover:shadow-lg cursor-pointer transition-all"
            >
              <div :class="['w-10 h-10 rounded-lg flex items-center justify-center flex-shrink-0 transition-transform group-hover:scale-110 group-hover:shadow-md', getFileIconBg(doc.type)]">
                <i :class="[getFileIcon(doc.type), getFileIconColor(doc.type), 'text-lg']"></i>
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium text-gray-900 truncate group-hover:text-teal-600 transition-colors">{{ doc.name }}</p>
                <div class="flex items-center gap-2 mt-0.5">
                  <span class="text-[10px] text-gray-400 flex items-center gap-1">
                    <i class="fas fa-clock text-[8px]"></i>
                    {{ getRelativeTime(doc.updatedAt) }}
                  </span>
                  <span class="text-[10px] text-gray-300">•</span>
                  <span class="text-[10px] text-gray-400">{{ formatFileSize(doc.size) }}</span>
                </div>
              </div>
              <div class="opacity-0 group-hover:opacity-100 transition-opacity flex items-center gap-1">
                <button
                  @click.stop="toggleStar(doc)"
                  :class="[
                    'w-7 h-7 rounded-lg flex items-center justify-center transition-all',
                    doc.isStarred ? 'bg-amber-100 text-amber-600 hover:bg-amber-200' : 'bg-gray-100 text-gray-400 hover:bg-amber-100 hover:text-amber-600'
                  ]"
                  :title="doc.isStarred ? 'Remove from Starred' : 'Add to Starred'"
                >
                  <i class="fas fa-star text-xs"></i>
                </button>
                <button
                  @click.stop="shareDocument(doc)"
                  class="w-7 h-7 rounded-lg bg-gray-100 text-gray-400 hover:bg-blue-100 hover:text-blue-600 flex items-center justify-center transition-all"
                  title="Share"
                >
                  <i class="fas fa-share-alt text-xs"></i>
                </button>
                <button
                  @click.stop="downloadDocument(doc)"
                  class="w-7 h-7 rounded-lg bg-gray-100 text-gray-400 hover:bg-teal-100 hover:text-teal-600 flex items-center justify-center transition-all"
                  title="Download"
                >
                  <i class="fas fa-download text-xs"></i>
                </button>
                <ContentActionsDropdown
                  :show-download="true"
                  @add-to-collection="openAddToCollection(doc)"
                  @share="shareDocument(doc)"
                  @download="downloadDocument(doc)"
                  @copy-link="() => copyDocumentLink(doc.id)"
                />
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- AI Folder Organization Suggestions -->
      <div v-if="showAIFeatures && showFolderSuggestions && folderSuggestions.length > 0" class="bg-white rounded-2xl p-5 shadow-sm border border-gray-100 mb-6">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-base font-bold text-gray-900 flex items-center gap-3">
            <div class="w-9 h-9 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-wand-magic-sparkles text-white text-sm"></i>
            </div>
            <div>
              <span class="block">AI Organization Suggestions</span>
              <span class="text-xs font-medium text-gray-500">Smart folder recommendations</span>
            </div>
            <span class="ai-powered-badge-doc">AI Powered</span>
          </h2>
          <div class="flex items-center gap-2">
            <button @click="dismissAllFolderSuggestions" class="text-xs text-gray-500 hover:text-gray-700 px-2 py-1">
              Dismiss all
            </button>
            <button @click="toggleAIFeatures" class="w-7 h-7 rounded-lg bg-gray-100 text-gray-400 hover:bg-red-50 hover:text-red-500 flex items-center justify-center transition-all">
              <i class="fas fa-times text-xs"></i>
            </button>
          </div>
        </div>

        <div class="space-y-3">
          <div
            v-for="suggestion in folderSuggestions"
            :key="suggestion.documentId"
            class="ai-folder-suggestion-card"
          >
            <div class="flex items-start gap-4">
              <!-- Document Info -->
              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium text-gray-900 truncate">{{ suggestion.documentName }}</p>
                <div class="flex items-center gap-2 mt-1 text-xs text-gray-500">
                  <span class="flex items-center gap-1">
                    <i class="fas fa-folder text-gray-400"></i>
                    {{ suggestion.currentFolder }}
                  </span>
                  <i class="fas fa-arrow-right text-teal-500"></i>
                  <span class="flex items-center gap-1 text-teal-600 font-medium">
                    <i class="fas fa-folder text-teal-500"></i>
                    {{ suggestion.suggestedFolder }}
                  </span>
                </div>
                <p class="text-xs text-gray-400 mt-1 flex items-center gap-1">
                  <i class="fas fa-lightbulb text-amber-400"></i>
                  {{ suggestion.reason }}
                </p>
              </div>

              <!-- Confidence & Actions -->
              <div class="flex items-center gap-3">
                <div class="text-right">
                  <span class="text-xs text-gray-400">Confidence</span>
                  <div class="flex items-center gap-1">
                    <div class="w-16 h-1.5 bg-gray-200 rounded-full overflow-hidden">
                      <div
                        class="h-full bg-gradient-to-r from-teal-400 to-teal-500 rounded-full"
                        :style="{ width: `${suggestion.confidence * 100}%` }"
                      ></div>
                    </div>
                    <span class="text-xs font-semibold text-teal-600">{{ Math.round(suggestion.confidence * 100) }}%</span>
                  </div>
                </div>
                <div class="flex items-center gap-1">
                  <button
                    @click="applyFolderSuggestion(suggestion)"
                    class="px-3 py-1.5 bg-teal-500 text-white text-xs font-medium rounded-lg hover:bg-teal-600 transition-all flex items-center gap-1"
                  >
                    <i class="fas fa-check"></i>
                    Apply
                  </button>
                  <button
                    @click="dismissFolderSuggestion(suggestion.documentId)"
                    class="w-8 h-8 rounded-lg bg-gray-100 text-gray-400 hover:bg-gray-200 hover:text-gray-600 flex items-center justify-center transition-all"
                  >
                    <i class="fas fa-times text-xs"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- All Files Section with Folder Tree -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
        <!-- Section Header / Toolbar -->
        <div class="border-b border-gray-100">
          <!-- Top Row - Title and Primary Actions -->
          <div class="px-4 py-3 flex items-center justify-between">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
                <i class="fas fa-folder-tree text-white text-sm"></i>
              </div>
              <div>
                <span class="block">All Files</span>
                <span class="text-xs font-medium text-gray-500">{{ filteredDocuments.length }} items • {{ formatTotalSize(documentStats.totalSize) }}</span>
              </div>
            </h2>
            <div class="flex items-center gap-2">
              <!-- Primary Actions -->
              <button class="px-4 py-2 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-lg text-sm font-medium hover:from-teal-600 hover:to-teal-700 transition-all flex items-center gap-2 shadow-sm shadow-teal-200">
                <i class="fas fa-cloud-arrow-up"></i>
                Upload Files
              </button>
              <button class="px-3 py-2 bg-white border border-gray-200 text-gray-700 rounded-lg text-sm font-medium hover:bg-gray-50 hover:border-gray-300 transition-all flex items-center gap-2">
                <i class="fas fa-folder-plus text-amber-500"></i>
                New Folder
              </button>

              <!-- Divider -->
              <div class="w-px h-8 bg-gray-200 mx-1"></div>

              <!-- Secondary Actions -->
              <button class="w-9 h-9 rounded-lg bg-gray-100 text-gray-600 hover:bg-gray-200 hover:text-gray-800 flex items-center justify-center transition-all" title="Refresh">
                <i class="fas fa-sync-alt text-sm"></i>
              </button>
            </div>
          </div>

          <!-- Bottom Row - Search, Filters, View Options -->
          <div class="px-4 py-2 bg-gray-50/50 flex flex-wrap items-center gap-3">
            <!-- Unified Search with AI Integration -->
            <div class="relative flex-1 max-w-xl">
              <div class="flex items-stretch">
                <!-- AI Mode Toggle -->
                <button
                  v-if="showAIFeatures"
                  @click="isAISearchMode = !isAISearchMode"
                  :class="[
                    'px-3 rounded-l-lg border border-r-0 flex items-center gap-1.5 text-xs font-medium transition-all',
                    isAISearchMode
                      ? 'bg-gradient-to-r from-teal-500 to-cyan-500 border-teal-500 text-white'
                      : 'bg-gray-100 border-gray-200 text-gray-500 hover:bg-gray-200'
                  ]"
                  title="Toggle AI Search"
                >
                  <i class="fas fa-wand-magic-sparkles"></i>
                  <span class="hidden sm:inline">AI</span>
                </button>

                <!-- Search Input -->
                <div class="relative flex-1">
                  <i :class="[
                    'absolute left-3 top-1/2 -translate-y-1/2 text-sm transition-colors',
                    isAISearchMode ? 'fas fa-brain text-teal-500' : 'fas fa-search text-gray-400'
                  ]"></i>
                  <input
                    v-model="unifiedSearchQuery"
                    type="text"
                    :placeholder="isAISearchMode ? 'Ask AI: Find tournament regulations...' : 'Search files and folders...'"
                    @keyup.enter="handleUnifiedSearch"
                    @input="handleSearchInput"
                    @focus="unifiedSearchQuery.length >= 2 && (showSearchSuggestions = true)"
                    @blur="hideSearchSuggestions"
                    :class="[
                      'w-full pl-9 pr-20 py-2 text-sm focus:outline-none transition-all',
                      showAIFeatures ? 'rounded-r-lg' : 'rounded-lg',
                      isAISearchMode
                        ? 'bg-gradient-to-r from-teal-50 to-cyan-50 border border-teal-200 focus:ring-2 focus:ring-teal-400 focus:border-transparent placeholder:text-teal-400'
                        : 'bg-white border border-gray-200 focus:ring-2 focus:ring-teal-500 focus:border-transparent',
                      !showAIFeatures && 'rounded-l-lg'
                    ]"
                  >
                  <!-- Clear & Search Buttons -->
                  <div class="absolute right-2 top-1/2 -translate-y-1/2 flex items-center gap-1">
                    <button
                      v-if="unifiedSearchQuery"
                      @click="clearUnifiedSearch"
                      :class="['p-1 rounded transition-colors', isAISearchMode ? 'text-teal-400 hover:text-teal-600' : 'text-gray-400 hover:text-gray-600']"
                    >
                      <i class="fas fa-times text-xs"></i>
                    </button>
                    <button
                      v-if="isAISearchMode && unifiedSearchQuery"
                      @click="handleUnifiedSearch"
                      :disabled="isProcessingNLSearch"
                      class="p-1 rounded text-teal-500 hover:text-teal-600 disabled:opacity-50"
                    >
                      <i :class="isProcessingNLSearch ? 'fas fa-spinner animate-spin' : 'fas fa-arrow-right'" class="text-sm"></i>
                    </button>
                  </div>
                </div>
              </div>

              <!-- AI Search Suggestions Dropdown -->
              <div
                v-if="showAIFeatures && isAISearchMode && showAISuggestions && !unifiedSearchQuery"
                class="absolute left-0 top-full mt-2 w-full bg-white rounded-xl shadow-lg border border-teal-100 py-2 z-50"
              >
                <div class="px-3 py-1.5 text-xs font-semibold text-teal-500 flex items-center gap-2">
                  <i class="fas fa-lightbulb"></i>
                  Try asking:
                </div>
                <button
                  v-for="suggestion in nlSearchSuggestions"
                  :key="suggestion"
                  @click="unifiedSearchQuery = suggestion; handleUnifiedSearch()"
                  class="w-full px-3 py-2 text-left text-sm text-gray-700 hover:bg-teal-50 flex items-center gap-2"
                >
                  <i class="fas fa-search text-teal-400 text-xs"></i>
                  {{ suggestion }}
                </button>
              </div>

              <!-- Regular Search Suggestions Dropdown -->
              <div
                v-if="!isAISearchMode && showSearchSuggestions && searchSuggestions.length > 0"
                class="ai-suggestions-dropdown-doc"
              >
                <div class="ai-suggestions-header-doc">
                  <i class="fas fa-wand-magic-sparkles text-teal-500"></i>
                  <span>AI Suggestions</span>
                </div>
                <div class="ai-suggestions-list-doc">
                  <button
                    v-for="suggestion in searchSuggestions"
                    :key="suggestion"
                    @click="selectSearchSuggestion(suggestion)"
                    class="ai-suggestion-item-doc"
                  >
                    <i class="fas fa-search text-gray-400 text-xs"></i>
                    <span>{{ suggestion }}</span>
                    <i class="fas fa-arrow-right text-teal-500 text-xs ml-auto opacity-0 group-hover:opacity-100"></i>
                  </button>
                </div>
              </div>

              <!-- AI Processing Indicator -->
              <div
                v-if="isProcessingNLSearch"
                class="absolute left-0 top-full mt-2 w-full bg-gradient-to-r from-teal-50 to-cyan-50 rounded-xl shadow-lg border border-teal-100 p-4 z-50"
              >
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-teal-500 to-cyan-500 flex items-center justify-center">
                    <i class="fas fa-brain text-white text-sm animate-pulse"></i>
                  </div>
                  <div>
                    <div class="text-sm font-medium text-teal-700">AI is searching...</div>
                    <div class="text-xs text-teal-500">Analyzing your query</div>
                  </div>
                </div>
              </div>
            </div>

            <!-- File Type Filter -->
            <div class="relative">
              <button
                @click="showFileTypeFilter = !showFileTypeFilter"
                :class="[
                  'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                  selectedFileTypes.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                ]"
              >
                <i class="fas fa-file text-sm"></i>
                <span>{{ selectedFileTypes.length > 0 ? `${selectedFileTypes.length} Types` : 'File Type' }}</span>
                <i :class="showFileTypeFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
              </button>

              <!-- Dropdown Menu -->
              <div
                v-if="showFileTypeFilter"
                class="absolute left-0 top-full mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
              >
                <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Select File Types</div>
                <div class="max-h-48 overflow-y-auto">
                  <button
                    v-for="type in fileTypes"
                    :key="type.id"
                    @click="toggleFileType(type.id)"
                    :class="[
                      'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                      isFileTypeSelected(type.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                    ]"
                  >
                    <div :class="[
                      'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                      isFileTypeSelected(type.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                    ]">
                      <i v-if="isFileTypeSelected(type.id)" class="fas fa-check text-white text-[8px]"></i>
                    </div>
                    <i :class="[type.icon, type.color, 'text-sm']"></i>
                    <span class="flex-1">{{ type.label }}</span>
                  </button>
                </div>

                <div class="my-2 border-t border-gray-100"></div>

                <div class="px-3 flex gap-2">
                  <button
                    @click="selectedFileTypes = []; showFileTypeFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                  >
                    Clear All
                  </button>
                  <button
                    @click="showFileTypeFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                  >
                    Apply
                  </button>
                </div>
              </div>

              <!-- Click outside to close -->
              <div v-if="showFileTypeFilter" @click="showFileTypeFilter = false" class="fixed inset-0 z-40"></div>
            </div>

            <!-- Category Filter -->
            <div class="relative">
              <button
                @click="showCategoryFilter = !showCategoryFilter"
                :class="[
                  'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                  selectedCategories.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                ]"
              >
                <i class="fas fa-layer-group text-sm"></i>
                <span>{{ selectedCategories.length > 0 ? `${selectedCategories.length} Categories` : 'Category' }}</span>
                <i :class="showCategoryFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
              </button>

              <!-- Dropdown Menu -->
              <div
                v-if="showCategoryFilter"
                class="absolute left-0 top-full mt-2 w-64 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
              >
                <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Select Categories</div>
                <div class="max-h-48 overflow-y-auto">
                  <button
                    v-for="lib in libraries"
                    :key="lib.id"
                    @click="toggleCategory(lib.id)"
                    :class="[
                      'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                      isCategorySelected(lib.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                    ]"
                  >
                    <div :class="[
                      'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                      isCategorySelected(lib.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                    ]">
                      <i v-if="isCategorySelected(lib.id)" class="fas fa-check text-white text-[8px]"></i>
                    </div>
                    <div
                      class="w-6 h-6 rounded-lg flex items-center justify-center text-white text-xs"
                      :style="{ backgroundColor: lib.color }"
                    >
                      <i :class="lib.icon"></i>
                    </div>
                    <span class="flex-1">{{ lib.name }}</span>
                    <span class="text-xs text-gray-400">{{ lib.documentCount }}</span>
                  </button>
                </div>

                <div class="my-2 border-t border-gray-100"></div>

                <div class="px-3 flex gap-2">
                  <button
                    @click="selectedCategories = []; showCategoryFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                  >
                    Clear All
                  </button>
                  <button
                    @click="showCategoryFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                  >
                    Apply
                  </button>
                </div>
              </div>

              <!-- Click outside to close -->
              <div v-if="showCategoryFilter" @click="showCategoryFilter = false" class="fixed inset-0 z-40"></div>
            </div>

            <!-- Tags Filter -->
            <div class="relative">
              <button
                @click="showTagFilter = !showTagFilter"
                :class="[
                  'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                  selectedTags.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                ]"
              >
                <i class="fas fa-tags text-sm"></i>
                <span>{{ selectedTags.length > 0 ? `${selectedTags.length} Tags` : 'Tags' }}</span>
                <i :class="showTagFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
              </button>

              <!-- Dropdown Menu -->
              <div
                v-if="showTagFilter"
                class="absolute left-0 top-full mt-2 w-64 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
              >
                <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Select Tags</div>
                <div class="max-h-48 overflow-y-auto">
                  <button
                    v-for="tag in allTags"
                    :key="tag"
                    @click="toggleTag(tag)"
                    :class="[
                      'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                      isTagSelected(tag) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                    ]"
                  >
                    <div :class="[
                      'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                      isTagSelected(tag) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                    ]">
                      <i v-if="isTagSelected(tag)" class="fas fa-check text-white text-[8px]"></i>
                    </div>
                    <span class="flex-1">{{ tag }}</span>
                  </button>
                </div>

                <div class="my-2 border-t border-gray-100"></div>

                <div class="px-3 flex gap-2">
                  <button
                    @click="selectedTags = []; showTagFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                  >
                    Clear All
                  </button>
                  <button
                    @click="showTagFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                  >
                    Apply
                  </button>
                </div>
              </div>

              <!-- Click outside to close -->
              <div v-if="showTagFilter" @click="showTagFilter = false" class="fixed inset-0 z-40"></div>
            </div>

            <!-- Saved & Shared Filter Dropdown -->
            <div class="relative">
              <button
                @click="showStatusFilter = !showStatusFilter"
                :class="[
                  'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                  selectedStatusFilters.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                ]"
              >
                <i class="fas fa-bookmark text-sm"></i>
                <span>{{ selectedStatusFilters.length > 0 ? `${selectedStatusFilters.length} Saved & Shared` : 'Saved & Shared' }}</span>
                <i :class="showStatusFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
              </button>

              <!-- Dropdown Menu -->
              <div
                v-if="showStatusFilter"
                class="absolute left-0 top-full mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
              >
                <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Filter by Status</div>
                <div class="max-h-48 overflow-y-auto">
                  <button
                    v-for="option in statusFilterOptions"
                    :key="option.id"
                    @click="toggleStatusFilter(option.id)"
                    :class="[
                      'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                      isStatusSelected(option.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                    ]"
                  >
                    <div :class="[
                      'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                      isStatusSelected(option.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                    ]">
                      <i v-if="isStatusSelected(option.id)" class="fas fa-check text-white text-[8px]"></i>
                    </div>
                    <i :class="[option.icon, option.color]"></i>
                    <span class="flex-1">{{ option.label }}</span>
                  </button>
                </div>

                <div class="my-2 border-t border-gray-100"></div>

                <div class="px-3 flex gap-2">
                  <button
                    @click="selectedStatusFilters = []; showStatusFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                  >
                    Clear All
                  </button>
                  <button
                    @click="showStatusFilter = false"
                    class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                  >
                    Apply
                  </button>
                </div>
              </div>

              <!-- Click outside to close -->
              <div v-if="showStatusFilter" @click="showStatusFilter = false" class="fixed inset-0 z-40"></div>
            </div>

            <!-- Sort Options with Order Toggle -->
            <div class="relative ml-auto flex items-center">
              <button
                @click="showSortDropdown = !showSortDropdown"
                class="flex items-center gap-2 px-3 py-1.5 rounded-l-lg text-xs font-medium transition-all border border-r-0 bg-white border-gray-200 text-gray-600 hover:bg-gray-50"
              >
                <i :class="[currentSortOption.icon, 'text-sm text-teal-500']"></i>
                <span>{{ currentSortOption.label }}</span>
                <i :class="showSortDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
              </button>
              <button
                @click="sortOrder = sortOrder === 'asc' ? 'desc' : 'asc'"
                class="flex items-center justify-center w-8 h-8 rounded-r-lg text-xs font-medium transition-all border bg-white border-gray-200 text-gray-600 hover:bg-gray-50 hover:text-teal-600"
                :title="sortOrder === 'asc' ? 'Ascending order - Click for descending' : 'Descending order - Click for ascending'"
              >
                <i :class="sortOrder === 'asc' ? 'fas fa-arrow-up' : 'fas fa-arrow-down'" class="text-sm text-teal-500"></i>
              </button>

              <!-- Dropdown Menu -->
              <div
                v-if="showSortDropdown"
                class="absolute left-0 top-full mt-2 w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
              >
                <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Sort By</div>
                <div class="max-h-64 overflow-y-auto">
                  <button
                    v-for="option in sortOptionsList"
                    :key="option.value"
                    @click="selectSortOption(option.value)"
                    :class="[
                      'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                      sortBy === option.value ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                    ]"
                  >
                    <i :class="[option.icon, 'text-sm w-4', sortBy === option.value ? 'text-teal-500' : 'text-gray-400']"></i>
                    <span class="flex-1">{{ option.label }}</span>
                    <i v-if="sortBy === option.value" class="fas fa-check text-teal-500 text-xs"></i>
                  </button>
                </div>
              </div>
              <div v-if="showSortDropdown" @click="showSortDropdown = false" class="fixed inset-0 z-40"></div>
            </div>

            <!-- View Toggle -->
            <div class="flex items-center gap-0.5 bg-white border border-gray-200 rounded-lg p-1">
              <button
                @click="viewMode = 'grid'"
                :class="['px-2.5 py-1 rounded-md transition-all', viewMode === 'grid' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
                title="Grid view"
              >
                <i class="fas fa-th-large text-xs"></i>
              </button>
              <button
                @click="viewMode = 'list'"
                :class="['px-2.5 py-1 rounded-md transition-all', viewMode === 'list' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
                title="List view"
              >
                <i class="fas fa-list text-xs"></i>
              </button>
            </div>
          </div>
        </div>

        <div class="flex min-h-[500px]">
          <!-- Folder Tree Sidebar -->
          <div
            :class="[
              'border-r border-gray-100 bg-gray-50/50 transition-all duration-300 relative',
              isSidebarCollapsed ? 'w-12 min-w-[48px]' : 'w-64 min-w-[256px]'
            ]"
          >
            <!-- Collapse/Expand Button -->
            <button
              @click="isSidebarCollapsed = !isSidebarCollapsed"
              class="absolute -right-3 top-4 w-6 h-6 bg-white border border-gray-200 rounded-full shadow-sm flex items-center justify-center text-gray-500 hover:text-teal-600 hover:border-teal-300 transition-all z-10"
              :title="isSidebarCollapsed ? 'Expand sidebar' : 'Collapse sidebar'"
            >
              <i :class="isSidebarCollapsed ? 'fas fa-chevron-right' : 'fas fa-chevron-left'" class="text-[10px]"></i>
            </button>

            <!-- Collapsed State - Icons Only -->
            <div v-if="isSidebarCollapsed" class="p-2 pt-4 space-y-2">
              <!-- View Navigation Icons -->
              <button
                v-for="view in viewNavItems"
                :key="'collapsed-' + view.id"
                @click="currentView = view.id as any; isSidebarCollapsed = false"
                :class="[
                  'w-8 h-8 rounded-lg flex items-center justify-center transition-all',
                  currentView === view.id ? 'bg-teal-100 text-teal-600' : 'hover:bg-gray-100 ' + view.color
                ]"
                :title="view.name"
              >
                <i :class="[view.icon, 'text-sm']"></i>
              </button>
              <div class="border-t border-gray-200 my-2"></div>
              <!-- Folder Icons -->
              <button
                v-for="folder in folderTree[0].children"
                :key="folder.id"
                @click="selectFolder(folder.id); isSidebarCollapsed = false"
                :class="[
                  'w-8 h-8 rounded-lg flex items-center justify-center transition-all',
                  selectedFolder === folder.id ? 'bg-teal-100 text-teal-600' : 'hover:bg-gray-100 text-amber-500'
                ]"
                :title="folder.name"
              >
                <i class="fas fa-folder text-sm"></i>
              </button>
            </div>

            <!-- Expanded State - Full Tree -->
            <div v-else class="p-4">
              <!-- View Navigation -->
              <div class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-3">Views</div>
              <div class="space-y-1 mb-4">
                <button
                  v-for="view in viewNavItems"
                  :key="view.id"
                  @click="currentView = view.id as any"
                  :class="[
                    'w-full flex items-center gap-3 px-3 py-2 rounded-lg transition-all text-left',
                    currentView === view.id
                      ? 'bg-teal-100 text-teal-700'
                      : 'hover:bg-gray-100 text-gray-600'
                  ]"
                >
                  <i :class="[view.icon, 'text-sm w-4', currentView === view.id ? 'text-teal-600' : view.color]"></i>
                  <span class="text-sm font-medium flex-1">{{ view.name }}</span>
                  <span :class="[
                    'text-xs px-1.5 py-0.5 rounded-full min-w-[20px] text-center',
                    currentView === view.id ? 'bg-teal-200 text-teal-700' : 'bg-gray-100 text-gray-500'
                  ]">
                    {{ viewCounts[view.id as keyof typeof viewCounts] }}
                  </span>
                </button>
              </div>

              <div class="border-t border-gray-200 my-4"></div>

              <div class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-3">Folders</div>

              <!-- Recursive Folder Tree -->
              <div class="space-y-1">
                <template v-for="folder in folderTree" :key="folder.id">
                  <!-- Root Folder -->
                  <div>
                    <div
                      @click="selectFolder(folder.id)"
                      :class="[
                        'flex items-center gap-2 px-2 py-1.5 rounded-lg cursor-pointer transition-all group',
                        selectedFolder === folder.id ? 'bg-teal-100 text-teal-700' : 'hover:bg-gray-100 text-gray-700'
                      ]"
                    >
                      <button
                        v-if="folder.children && folder.children.length > 0"
                        @click.stop="toggleFolder(folder.id)"
                        class="w-4 h-4 flex items-center justify-center text-gray-400 hover:text-gray-600"
                      >
                        <i :class="isFolderExpanded(folder.id) ? 'fas fa-chevron-down' : 'fas fa-chevron-right'" class="text-[10px]"></i>
                      </button>
                      <span v-else class="w-4"></span>
                      <i :class="[folder.icon, selectedFolder === folder.id ? 'text-teal-600' : 'text-amber-500']" class="text-sm"></i>
                      <span class="text-sm font-medium truncate">{{ folder.name }}</span>
                    </div>

                    <!-- Level 1 Children -->
                    <div v-if="isFolderExpanded(folder.id) && folder.children" class="ml-4 mt-1 space-y-1">
                      <template v-for="child in folder.children" :key="child.id">
                        <div>
                          <div
                            @click="selectFolder(child.id)"
                            :class="[
                              'flex items-center gap-2 px-2 py-1.5 rounded-lg cursor-pointer transition-all',
                              selectedFolder === child.id ? 'bg-teal-100 text-teal-700' : 'hover:bg-gray-100 text-gray-600'
                            ]"
                          >
                            <button
                              v-if="child.children && child.children.length > 0"
                              @click.stop="toggleFolder(child.id)"
                              class="w-4 h-4 flex items-center justify-center text-gray-400 hover:text-gray-600"
                            >
                              <i :class="isFolderExpanded(child.id) ? 'fas fa-chevron-down' : 'fas fa-chevron-right'" class="text-[10px]"></i>
                            </button>
                            <span v-else class="w-4"></span>
                            <i :class="[selectedFolder === child.id ? 'fas fa-folder-open text-teal-500' : 'fas fa-folder text-amber-400']" class="text-sm"></i>
                            <span class="text-sm truncate">{{ child.name }}</span>
                          </div>

                          <!-- Level 2 Children -->
                          <div v-if="isFolderExpanded(child.id) && child.children && child.children.length > 0" class="ml-4 mt-1 space-y-1">
                            <div
                              v-for="subChild in child.children"
                              :key="subChild.id"
                              @click="selectFolder(subChild.id)"
                              :class="[
                                'flex items-center gap-2 px-2 py-1.5 rounded-lg cursor-pointer transition-all',
                                selectedFolder === subChild.id ? 'bg-teal-100 text-teal-700' : 'hover:bg-gray-100 text-gray-500'
                              ]"
                            >
                              <span class="w-4"></span>
                              <i :class="[selectedFolder === subChild.id ? 'fas fa-folder-open text-teal-500' : 'fas fa-folder text-amber-300']" class="text-sm"></i>
                              <span class="text-sm truncate">{{ subChild.name }}</span>
                            </div>
                          </div>
                        </div>
                      </template>
                    </div>
                  </div>
                </template>
              </div>

              <!-- Storage Info -->
              <div class="mt-6 pt-4 border-t border-gray-200">
                <div class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-2">Storage</div>
                <div class="bg-gray-200 rounded-full h-2 overflow-hidden">
                  <div class="bg-gradient-to-r from-teal-500 to-teal-400 h-full rounded-full" style="width: 45%"></div>
                </div>
                <p class="text-xs text-gray-500 mt-2">{{ formatTotalSize(documentStats.totalSize) }} used</p>
              </div>
            </div>
          </div>

          <!-- Files Content Area -->
          <div class="flex-1 min-w-0 p-5 bg-gradient-to-br from-gray-50/50 to-white">
            <!-- Active Filters -->
            <div v-if="selectedFolder || selectedFileTypes.length > 0 || searchQuery || selectedCategories.length > 0 || selectedTags.length > 0" class="flex items-center gap-3 mb-5 p-3 bg-white rounded-xl border border-gray-100 shadow-sm">
              <div class="flex items-center gap-2 px-2 py-1 bg-gray-100 rounded-lg">
                <i class="fas fa-filter text-gray-400 text-xs"></i>
                <span class="text-xs font-medium text-gray-600">Active Filters</span>
              </div>
              <div class="flex flex-wrap gap-2 flex-1">
                <span v-if="selectedFolder" class="px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-teal-100">
                  <i class="fas fa-folder text-[10px]"></i>
                  {{ selectedFolder }}
                  <button @click="selectedFolder = null" class="ml-1 hover:text-teal-900 hover:bg-teal-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
                </span>
                <span
                  v-for="categoryId in selectedCategories"
                  :key="'filter-category-' + categoryId"
                  class="px-2.5 py-1 bg-purple-50 text-purple-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-purple-100"
                >
                  <i class="fas fa-layer-group text-[10px]"></i>
                  {{ libraries.find(l => l.id === categoryId)?.name }}
                  <button @click="toggleCategory(categoryId)" class="ml-1 hover:text-purple-900 hover:bg-purple-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
                </span>
                <span
                  v-for="typeId in selectedFileTypes"
                  :key="'filter-type-' + typeId"
                  class="px-2.5 py-1 bg-blue-50 text-blue-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-blue-100"
                >
                  <i :class="[fileTypes.find(t => t.id === typeId)?.icon, 'text-[10px]']"></i>
                  {{ fileTypes.find(t => t.id === typeId)?.label }}
                  <button @click="toggleFileType(typeId)" class="ml-1 hover:text-blue-900 hover:bg-blue-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
                </span>
                <span
                  v-for="tag in selectedTags"
                  :key="'filter-tag-' + tag"
                  class="px-2.5 py-1 bg-amber-50 text-amber-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-amber-100"
                >
                  <i class="fas fa-tag text-[10px]"></i>
                  {{ tag }}
                  <button @click="toggleTag(tag)" class="ml-1 hover:text-amber-900 hover:bg-amber-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
                </span>
                <span v-if="searchQuery" class="px-2.5 py-1 bg-gray-100 text-gray-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-gray-200">
                  <i class="fas fa-search text-[10px]"></i>
                  "{{ searchQuery }}"
                  <button @click="searchQuery = ''" class="ml-1 hover:text-gray-900 hover:bg-gray-200 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
                </span>
              </div>
              <button @click="clearFilters(); selectedFolder = null" class="px-3 py-1.5 text-xs font-medium text-red-600 hover:text-red-700 hover:bg-red-50 rounded-lg transition-colors flex items-center gap-1.5">
                <i class="fas fa-times-circle"></i>
                Clear all
              </button>
            </div>

            <!-- Breadcrumb -->
            <div v-if="!isSelectionMode || selectedCount === 0" class="flex items-center justify-between mb-5">
              <div class="flex items-center gap-2 text-sm bg-white px-4 py-2 rounded-xl border border-gray-100 shadow-sm">
                <button @click="currentView = 'all'; selectedFolder = null" class="w-7 h-7 rounded-lg bg-gray-100 text-gray-500 hover:bg-teal-100 hover:text-teal-600 flex items-center justify-center transition-all">
                  <i class="fas fa-home text-xs"></i>
                </button>
                <i class="fas fa-chevron-right text-gray-300 text-xs"></i>
                <span class="text-gray-700 font-medium flex items-center gap-2 px-2 py-1 bg-gray-50 rounded-lg">
                  <i :class="[viewNavItems.find(v => v.id === currentView)?.icon, viewNavItems.find(v => v.id === currentView)?.color, 'text-sm']"></i>
                  {{ viewNavItems.find(v => v.id === currentView)?.name }}
                </span>
                <template v-if="selectedFolder">
                  <i class="fas fa-chevron-right text-gray-300 text-xs"></i>
                  <span class="text-gray-600 px-2 py-1 bg-gray-50 rounded-lg">{{ selectedFolder }}</span>
                </template>
              </div>
              <div class="flex items-center gap-2 text-xs text-gray-500 bg-white px-3 py-2 rounded-lg border border-gray-100">
                <i class="fas fa-file-alt text-teal-500"></i>
                <span class="font-medium">{{ filteredDocuments.length }}</span>
                <span>items</span>
              </div>
            </div>

            <!-- Selection Toolbar -->
            <div v-if="isSelectionMode && selectedCount > 0" class="flex items-center justify-between mb-5 p-3 bg-gradient-to-r from-teal-500 to-teal-600 rounded-xl shadow-lg shadow-teal-200/50 animate-slideDown">
              <div class="flex items-center gap-4">
                <!-- Select All Checkbox -->
                <button
                  @click="isAllSelected ? clearDocumentSelection() : selectAllDocuments()"
                  class="flex items-center gap-2 text-white hover:bg-white/10 px-3 py-1.5 rounded-lg transition-all"
                >
                  <div :class="[
                    'w-5 h-5 rounded border-2 flex items-center justify-center transition-all',
                    isAllSelected ? 'bg-white border-white' : isSomeSelected ? 'bg-white/50 border-white' : 'border-white/70'
                  ]">
                    <i v-if="isAllSelected" class="fas fa-check text-teal-600 text-[10px]"></i>
                    <i v-else-if="isSomeSelected" class="fas fa-minus text-teal-600 text-[10px]"></i>
                  </div>
                  <span class="text-sm font-medium">{{ isAllSelected ? 'Deselect all' : 'Select all' }}</span>
                </button>

                <!-- Selected Count -->
                <div class="flex items-center gap-2 px-3 py-1.5 bg-white/20 rounded-lg">
                  <i class="fas fa-check-circle text-white/80"></i>
                  <span class="text-white text-sm font-semibold">{{ selectedCount }} selected</span>
                </div>
              </div>

              <!-- Bulk Actions -->
              <div class="flex items-center gap-2">
                <template v-if="currentView !== 'trash'">
                  <button
                    @click="bulkStar"
                    class="flex items-center gap-2 px-3 py-2 bg-white/20 hover:bg-white/30 text-white rounded-lg transition-all text-sm font-medium"
                    title="Star selected"
                  >
                    <i class="fas fa-star"></i>
                    <span class="hidden sm:inline">Star</span>
                  </button>
                  <button
                    @click="bulkDownload"
                    class="flex items-center gap-2 px-3 py-2 bg-white/20 hover:bg-white/30 text-white rounded-lg transition-all text-sm font-medium"
                    title="Download selected"
                  >
                    <i class="fas fa-download"></i>
                    <span class="hidden sm:inline">Download</span>
                  </button>
                  <button
                    @click="bulkShare"
                    class="flex items-center gap-2 px-3 py-2 bg-white/20 hover:bg-white/30 text-white rounded-lg transition-all text-sm font-medium"
                    title="Share selected"
                  >
                    <i class="fas fa-share-alt"></i>
                    <span class="hidden sm:inline">Share</span>
                  </button>
                  <button
                    @click="bulkExport"
                    class="flex items-center gap-2 px-3 py-2 bg-white/20 hover:bg-white/30 text-white rounded-lg transition-all text-sm font-medium"
                    title="Export selected"
                  >
                    <i class="fas fa-file-export"></i>
                    <span class="hidden sm:inline">Export</span>
                  </button>
                  <div class="w-px h-6 bg-white/30 mx-1"></div>
                  <button
                    @click="bulkMoveToTrash"
                    class="flex items-center gap-2 px-3 py-2 bg-red-500/80 hover:bg-red-500 text-white rounded-lg transition-all text-sm font-medium"
                    title="Move to trash"
                  >
                    <i class="fas fa-trash-alt"></i>
                    <span class="hidden sm:inline">Delete</span>
                  </button>
                </template>
                <template v-else>
                  <button
                    @click="bulkRestore"
                    class="flex items-center gap-2 px-3 py-2 bg-white/20 hover:bg-white/30 text-white rounded-lg transition-all text-sm font-medium"
                    title="Restore selected"
                  >
                    <i class="fas fa-undo"></i>
                    <span class="hidden sm:inline">Restore</span>
                  </button>
                  <button
                    @click="bulkPermanentDelete"
                    class="flex items-center gap-2 px-3 py-2 bg-red-500/80 hover:bg-red-500 text-white rounded-lg transition-all text-sm font-medium"
                    title="Delete permanently"
                  >
                    <i class="fas fa-trash"></i>
                    <span class="hidden sm:inline">Delete Forever</span>
                  </button>
                </template>

                <!-- Cancel -->
                <button
                  @click="toggleSelectionMode"
                  class="flex items-center gap-2 px-3 py-2 bg-white text-teal-600 hover:bg-gray-100 rounded-lg transition-all text-sm font-medium ml-2"
                >
                  <i class="fas fa-times"></i>
                  <span class="hidden sm:inline">Cancel</span>
                </button>
              </div>
            </div>

            <!-- Loading State -->
            <div v-if="isLoading" class="flex flex-col items-center justify-center py-20">
              <div class="relative">
                <div class="w-16 h-16 border-4 border-teal-100 rounded-full"></div>
                <div class="w-16 h-16 border-4 border-teal-500 border-t-transparent rounded-full animate-spin absolute top-0 left-0"></div>
              </div>
              <p class="text-gray-500 text-sm mt-4 font-medium">Loading documents...</p>
            </div>

            <!-- Empty State -->
            <div v-else-if="filteredDocuments.length === 0" class="flex flex-col items-center justify-center py-16 bg-white rounded-2xl border border-gray-100">
              <div class="w-24 h-24 bg-gradient-to-br from-gray-100 to-gray-50 rounded-3xl flex items-center justify-center mb-6 shadow-inner">
                <i class="fas fa-folder-open text-4xl text-gray-300"></i>
              </div>
              <h3 class="text-lg font-semibold text-gray-700 mb-2">No files found</h3>
              <p class="text-gray-500 text-sm mb-6 max-w-xs text-center">Try adjusting your filters or upload a new file to get started</p>
              <div class="flex items-center gap-3">
                <button @click="clearFilters; selectedFolder = null" class="px-5 py-2.5 bg-gray-100 text-gray-700 rounded-xl text-sm font-medium hover:bg-gray-200 transition-colors flex items-center gap-2">
                  <i class="fas fa-filter-circle-xmark"></i>
                  Clear Filters
                </button>
                <button class="px-5 py-2.5 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-xl text-sm font-medium hover:from-teal-600 hover:to-teal-700 transition-all shadow-sm shadow-teal-200 flex items-center gap-2">
                  <i class="fas fa-cloud-arrow-up"></i>
                  Upload Files
                </button>
              </div>
            </div>

            <!-- Documents Grid View -->
            <div v-else-if="viewMode === 'grid'">
              <div class="grid grid-cols-[repeat(auto-fill,minmax(320px,1fr))] gap-4">
                <div
                  v-for="doc in paginatedDocuments"
                :key="doc.id"
                @click="viewDocument(doc)"
                :class="[
                  'group bg-white rounded-2xl p-4 cursor-pointer transition-all duration-300 border-2',
                  isDocumentSelected(doc.id)
                    ? 'border-teal-500 shadow-lg shadow-teal-100 ring-2 ring-teal-200'
                    : 'border-gray-100 hover:border-teal-200 hover:shadow-xl hover:-translate-y-1'
                ]"
              >
                <!-- Thumbnail / Icon -->
                <div class="relative mb-3">
                  <div v-if="doc.thumbnail" class="h-32 rounded-xl overflow-hidden shadow-sm">
                    <img :src="doc.thumbnail" :alt="doc.name" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500">
                    <div class="absolute inset-0 bg-gradient-to-t from-black/20 to-transparent opacity-0 group-hover:opacity-100 transition-opacity"></div>
                  </div>
                  <div v-else :class="['h-32 rounded-xl flex items-center justify-center relative overflow-hidden', getFileIconBg(doc.type)]">
                    <div class="absolute inset-0 opacity-30" :class="getFileIconBg(doc.type)"></div>
                    <i :class="[getFileIcon(doc.type), getFileIconColor(doc.type), 'text-4xl relative z-10 group-hover:scale-110 transition-transform duration-300']"></i>
                  </div>

                  <!-- Selection Checkbox (Always visible) -->
                  <div
                    @click.stop="toggleDocumentSelection(doc.id); if (!isSelectionMode) isSelectionMode = true"
                    :class="[
                      'absolute top-2 left-2 w-6 h-6 rounded-lg border-2 flex items-center justify-center cursor-pointer transition-all z-20',
                      isDocumentSelected(doc.id)
                        ? 'bg-teal-500 border-teal-500 text-white'
                        : isSelectionMode
                          ? 'bg-white/90 border-gray-300 hover:border-teal-400'
                          : 'bg-white/90 border-gray-300 hover:border-teal-400 opacity-0 group-hover:opacity-100'
                    ]"
                  >
                    <i v-if="isDocumentSelected(doc.id)" class="fas fa-check text-xs"></i>
                  </div>

                  <!-- File Type Badge -->
                  <div class="absolute bottom-2 left-2">
                    <span :class="['px-2 py-1 rounded-lg text-[10px] font-bold uppercase tracking-wide shadow-sm', getFileIconBg(doc.type), getFileIconColor(doc.type)]">
                      {{ doc.type }}
                    </span>
                  </div>

                  <!-- Quick Actions -->
                  <div class="absolute top-2 right-2 flex gap-1.5 opacity-0 group-hover:opacity-100 transition-all duration-200">
                    <button
                      v-if="currentView !== 'trash'"
                      @click.stop="toggleStar(doc)"
                      :class="[
                        'w-7 h-7 rounded-lg flex items-center justify-center transition-all shadow-sm backdrop-blur-sm',
                        doc.isStarred ? 'bg-amber-400 text-white' : 'bg-white/95 text-gray-500 hover:bg-amber-400 hover:text-white'
                      ]"
                      :title="doc.isStarred ? 'Remove from Starred' : 'Add to Starred'"
                    >
                      <i class="fas fa-star text-xs"></i>
                    </button>
                    <button
                      v-if="currentView === 'trash'"
                      @click.stop="restoreFromTrash(doc)"
                      class="w-7 h-7 rounded-lg bg-teal-500 text-white hover:bg-teal-600 flex items-center justify-center transition-all shadow-sm"
                      title="Restore"
                    >
                      <i class="fas fa-undo text-xs"></i>
                    </button>
                    <button
                      v-if="currentView === 'trash'"
                      @click.stop="permanentlyDelete(doc)"
                      class="w-7 h-7 rounded-lg bg-red-500 text-white hover:bg-red-600 flex items-center justify-center transition-all shadow-sm"
                      title="Delete permanently"
                    >
                      <i class="fas fa-trash text-xs"></i>
                    </button>
                    <button
                      v-if="currentView !== 'trash'"
                      @click.stop="downloadDocument(doc)"
                      class="w-7 h-7 rounded-lg bg-white/95 text-gray-500 hover:bg-teal-500 hover:text-white flex items-center justify-center transition-all shadow-sm backdrop-blur-sm"
                      title="Download"
                    >
                      <i class="fas fa-download text-xs"></i>
                    </button>
                    <button
                      v-if="currentView !== 'trash'"
                      @click.stop="shareDocument(doc)"
                      class="w-7 h-7 rounded-lg bg-white/95 text-gray-500 hover:bg-blue-500 hover:text-white flex items-center justify-center transition-all shadow-sm backdrop-blur-sm"
                      title="Share"
                    >
                      <i class="fas fa-share-alt text-xs"></i>
                    </button>
                    <button
                      v-if="currentView !== 'trash'"
                      @click.stop="toggleComparison(doc)"
                      :class="[
                        'w-7 h-7 rounded-lg flex items-center justify-center transition-all shadow-sm backdrop-blur-sm',
                        isInComparison(doc.id)
                          ? 'bg-purple-500 text-white'
                          : 'bg-white/95 text-gray-500 hover:bg-purple-500 hover:text-white'
                      ]"
                      :title="isInComparison(doc.id) ? 'Remove from Compare' : 'Add to Compare'"
                    >
                      <i class="fas fa-layer-group text-xs"></i>
                    </button>
                    <ContentActionsDropdown
                      v-if="currentView !== 'trash'"
                      :show-download="true"
                      @add-to-collection="openAddToCollection(doc)"
                      @share="shareDocument(doc)"
                      @download="downloadDocument(doc)"
                      @copy-link="() => copyDocumentLink(doc.id)"
                    />
                  </div>

                </div>

                <!-- Info -->
                <h4 class="font-semibold text-gray-900 text-sm truncate group-hover:text-teal-600 transition-colors mb-2">{{ doc.name }}</h4>

                <!-- Tags -->
                <div v-if="doc.tags && doc.tags.length > 0" class="flex flex-wrap gap-1 mb-2">
                  <span v-for="tag in doc.tags.slice(0, 2)" :key="tag" class="px-1.5 py-0.5 bg-gray-100 text-gray-500 text-[10px] rounded-md font-medium">
                    {{ tag }}
                  </span>
                  <span v-if="doc.tags.length > 2" class="px-1.5 py-0.5 bg-gray-100 text-gray-400 text-[10px] rounded-md font-medium">
                    +{{ doc.tags.length - 2 }}
                  </span>
                </div>

                <!-- AI Classification Badge -->
                <div v-if="getDocClassification(doc.id)" class="mb-2">
                  <div class="ai-classification-badge-doc" :class="getCategoryColor(getDocClassification(doc.id)!.category)">
                    <i class="fas fa-wand-magic-sparkles text-[8px]"></i>
                    <span>{{ getDocClassification(doc.id)!.category }}</span>
                    <span class="ai-confidence-mini">{{ Math.round(getDocClassification(doc.id)!.confidence * 100) }}%</span>
                  </div>
                </div>

                <!-- Meta Row -->
                <div class="flex items-center justify-between pt-2 border-t border-gray-100">
                  <!-- Author -->
                  <div class="flex items-center gap-2">
                    <div
                      class="w-7 h-7 rounded-full flex items-center justify-center text-white text-[10px] font-bold shadow-sm"
                      :style="{ backgroundColor: doc.author.color }"
                    >
                      {{ doc.author.initials }}
                    </div>
                    <span class="text-xs text-gray-500 truncate max-w-[70px]">{{ doc.author.name }}</span>
                  </div>
                  <!-- Size & Date -->
                  <div class="flex items-center gap-2 text-[11px] text-gray-400">
                    <span>{{ formatFileSize(doc.size) }}</span>
                    <span class="w-1 h-1 bg-gray-300 rounded-full"></span>
                    <span>{{ getRelativeTime(doc.updatedAt) }}</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Grid View Pagination Footer -->
              <div class="mt-4 px-4 py-3 bg-white rounded-2xl border border-gray-100 shadow-sm">
                <div class="flex items-center justify-between flex-wrap gap-3">
                  <!-- Left: Stats & Items Per Page -->
                  <div class="flex items-center gap-4 flex-wrap">
                    <span class="text-xs text-gray-500">
                      Showing <span class="font-semibold text-gray-700">{{ Math.min((currentPage - 1) * itemsPerPage + 1, filteredDocuments.length) }}</span>
                      to <span class="font-semibold text-gray-700">{{ Math.min(currentPage * itemsPerPage, filteredDocuments.length) }}</span>
                      of <span class="font-semibold text-gray-700">{{ filteredDocuments.length }}</span> files
                    </span>

                    <!-- Items Per Page Selector -->
                    <div class="flex items-center gap-2">
                      <span class="text-xs text-gray-500">Show:</span>
                      <select
                        v-model="itemsPerPage"
                        @change="changeItemsPerPage(Number(($event.target as HTMLSelectElement).value))"
                        class="text-xs px-2 py-1.5 rounded-lg border border-gray-200 bg-white text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-teal-500 cursor-pointer"
                      >
                        <option v-for="option in itemsPerPageOptions" :key="option" :value="option">
                          {{ option }}
                        </option>
                      </select>
                      <span class="text-xs text-gray-500">per page</span>
                    </div>
                  </div>

                  <!-- Right: Pagination Controls -->
                  <div class="flex items-center gap-2">
                    <button
                      @click="prevPage"
                      :disabled="currentPage === 1"
                      :class="[
                        'px-3 py-1.5 text-xs rounded-lg transition-colors flex items-center gap-1.5 border',
                        currentPage === 1
                          ? 'text-gray-300 border-gray-100 cursor-not-allowed'
                          : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100 border-gray-200'
                      ]"
                    >
                      <i class="fas fa-chevron-left text-[10px]"></i>
                      Previous
                    </button>

                    <!-- Page Numbers -->
                    <div class="flex items-center gap-1">
                      <template v-for="page in totalPages" :key="page">
                        <button
                          v-if="page === 1 || page === totalPages || (page >= currentPage - 1 && page <= currentPage + 1)"
                          @click="goToPage(page)"
                          :class="[
                            'w-8 h-8 text-xs rounded-lg transition-colors flex items-center justify-center',
                            page === currentPage
                              ? 'font-medium text-teal-600 bg-teal-50'
                              : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100'
                          ]"
                        >
                          {{ page }}
                        </button>
                        <span
                          v-else-if="page === currentPage - 2 || page === currentPage + 2"
                          class="text-xs text-gray-400 px-1"
                        >...</span>
                      </template>
                    </div>

                    <button
                      @click="nextPage"
                      :disabled="currentPage === totalPages"
                      :class="[
                        'px-3 py-1.5 text-xs rounded-lg transition-colors flex items-center gap-1.5 border',
                        currentPage === totalPages
                          ? 'text-gray-300 border-gray-100 cursor-not-allowed'
                          : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100 border-gray-200'
                      ]"
                    >
                      Next
                      <i class="fas fa-chevron-right text-[10px]"></i>
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Documents List View (Enhanced) -->
            <div v-else class="w-full bg-white rounded-2xl border border-gray-100 overflow-hidden shadow-sm">
              <!-- Table Header -->
              <div class="list-row px-4 py-3 bg-gradient-to-r from-gray-50 to-white border-b border-gray-100">
                <!-- Select All Checkbox -->
                <div class="col-checkbox flex items-center">
                  <button
                    @click="if (!isSelectionMode) isSelectionMode = true; isAllSelected ? clearDocumentSelection() : selectAllDocuments()"
                    :class="[
                      'w-5 h-5 rounded border-2 flex items-center justify-center transition-all',
                      isAllSelected ? 'bg-teal-500 border-teal-500 text-white' : isSomeSelected ? 'bg-teal-200 border-teal-400' : 'border-gray-300 hover:border-teal-400'
                    ]"
                  >
                    <i v-if="isAllSelected" class="fas fa-check text-[10px]"></i>
                    <i v-else-if="isSomeSelected" class="fas fa-minus text-[10px] text-teal-600"></i>
                  </button>
                </div>
                <div class="col-file flex items-center gap-2 text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  <i class="fas fa-file-alt text-teal-500"></i>
                  File Details
                </div>
                <div class="col-author flex items-center gap-2 text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  <i class="fas fa-user text-blue-500"></i>
                  Author
                </div>
                <div class="col-category flex items-center gap-2 text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  <i class="fas fa-layer-group text-teal-500"></i>
                  Category
                </div>
                <div class="col-engagement flex items-center gap-2 text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  <i class="fas fa-chart-line text-green-500"></i>
                  Engagement
                </div>
                <div class="col-size flex items-center gap-2 text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  <i class="fas fa-hdd text-amber-500"></i>
                  Size
                </div>
                <div class="col-modified flex items-center gap-2 text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  <i class="fas fa-clock text-gray-400"></i>
                  Modified
                </div>
                <div class="col-actions text-right text-xs font-semibold text-gray-600 uppercase tracking-wider">Actions</div>
              </div>

              <!-- Table Body -->
              <div class="w-full divide-y divide-gray-100/80">
                <div
                  v-for="doc in paginatedDocuments"
                  :key="doc.id"
                  @click="viewDocument(doc)"
                  :class="[
                    'group list-row list-row-enhanced px-5 py-5 cursor-pointer transition-all duration-300',
                    isDocumentSelected(doc.id)
                      ? 'bg-gradient-to-r from-teal-50 to-teal-25 shadow-sm'
                      : 'bg-white hover:bg-gradient-to-r hover:from-slate-50/80 hover:to-white hover:shadow-sm'
                  ]"
                >
                  <!-- Selection Checkbox -->
                  <div class="col-checkbox flex items-center">
                    <div
                      @click.stop="toggleDocumentSelection(doc.id); if (!isSelectionMode) isSelectionMode = true"
                      :class="[
                        'w-6 h-6 rounded-lg border-2 flex items-center justify-center cursor-pointer transition-all duration-200',
                        isDocumentSelected(doc.id)
                          ? 'bg-teal-500 border-teal-500 text-white shadow-md shadow-teal-200'
                          : 'border-gray-300 hover:border-teal-400 hover:bg-teal-50 group-hover:border-teal-300'
                      ]"
                    >
                      <i v-if="isDocumentSelected(doc.id)" class="fas fa-check text-xs"></i>
                    </div>
                  </div>

                  <!-- File Details Column (Name, Type, Description, Tags) -->
                  <div class="col-file flex items-center gap-4">
                    <!-- File Icon with Glow Effect -->
                    <div class="relative flex-shrink-0">
                      <div :class="['file-icon-glow w-14 h-14 rounded-2xl flex items-center justify-center transition-all duration-300 group-hover:scale-105 group-hover:shadow-xl', getFileIconBg(doc.type)]">
                        <i :class="[getFileIcon(doc.type), getFileIconColor(doc.type), 'text-2xl']"></i>
                      </div>
                      <span :class="['absolute -bottom-1.5 -right-1.5 px-2 py-0.5 text-[9px] font-bold uppercase rounded-lg shadow-md border-2 border-white', getFileIconBg(doc.type), getFileIconColor(doc.type)]">
                        {{ doc.type }}
                      </span>
                    </div>
                    <!-- File Info -->
                    <div class="min-w-0 flex-1 space-y-1">
                      <!-- Title Row -->
                      <div class="flex items-center gap-2">
                        <h4 class="font-semibold text-gray-900 text-base truncate group-hover:text-teal-600 transition-colors">{{ doc.name }}</h4>
                        <!-- Status Badges -->
                        <div class="flex items-center gap-1.5 flex-shrink-0">
                          <span v-if="doc.isStarred" class="w-5 h-5 bg-gradient-to-br from-amber-100 to-amber-50 text-amber-500 rounded-md flex items-center justify-center shadow-sm">
                            <i class="fas fa-star text-[9px]"></i>
                          </span>
                          <span v-if="doc.isShared" class="w-5 h-5 bg-gradient-to-br from-blue-100 to-blue-50 text-blue-500 rounded-md flex items-center justify-center shadow-sm">
                            <i class="fas fa-share-alt text-[9px]"></i>
                          </span>
                          <span v-if="doc.isTeamFile" class="w-5 h-5 bg-gradient-to-br from-teal-100 to-teal-50 text-teal-500 rounded-md flex items-center justify-center shadow-sm">
                            <i class="fas fa-users text-[9px]"></i>
                          </span>
                        </div>
                      </div>
                      <!-- Description -->
                      <p class="text-xs text-gray-500 truncate">{{ (doc as any).description || 'Official document for AFC Asian Cup 2027 tournament operations' }}</p>
                      <!-- Tags Row -->
                      <div class="flex items-center gap-2 flex-wrap">
                        <span v-for="tag in doc.tags.slice(0, 3)" :key="tag" class="px-2 py-0.5 bg-gradient-to-r from-gray-100 to-gray-50 text-gray-600 text-[10px] rounded-md font-medium hover:from-teal-100 hover:to-teal-50 hover:text-teal-700 transition-all cursor-pointer border border-gray-100 hover:border-teal-200">
                          {{ tag }}
                        </span>
                        <span v-if="doc.tags.length > 3" class="text-[10px] text-gray-400 font-medium">
                          +{{ doc.tags.length - 3 }}
                        </span>
                      </div>
                    </div>
                  </div>

                  <!-- Author Column - Enhanced with department -->
                  <div class="col-author flex items-center gap-3">
                    <div class="relative">
                      <div
                        class="w-11 h-11 rounded-xl flex items-center justify-center text-white text-sm font-bold shadow-lg ring-2 ring-white"
                        :style="{ backgroundColor: doc.author.color }"
                      >
                        {{ doc.author.initials }}
                      </div>
                      <span class="absolute -bottom-0.5 -right-0.5 w-3.5 h-3.5 bg-green-400 border-2 border-white rounded-full flex items-center justify-center">
                        <i class="fas fa-check text-white text-[8px]"></i>
                      </span>
                    </div>
                    <div class="min-w-0 space-y-0.5">
                      <p class="text-sm font-medium text-gray-800 truncate">{{ doc.author.name }}</p>
                      <p class="text-[10px] text-gray-500 truncate">{{ (doc.author as any).department || 'Tournament Ops' }}</p>
                      <div class="flex items-center gap-1">
                        <i class="fas fa-shield-alt text-teal-500 text-[8px]"></i>
                        <span class="text-[9px] text-teal-600 font-medium">Owner</span>
                      </div>
                    </div>
                  </div>

                  <!-- Category Column - Enhanced -->
                  <div class="col-category flex items-center">
                    <div class="flex items-center gap-2.5 px-3 py-2 bg-gradient-to-r from-teal-50 to-emerald-50 rounded-xl border border-teal-100/50">
                      <div class="w-7 h-7 bg-teal-100 rounded-lg flex items-center justify-center">
                        <i class="fas fa-folder text-teal-500 text-xs"></i>
                      </div>
                      <span class="text-sm text-teal-700 font-medium truncate">{{ (doc as any).category || 'General' }}</span>
                    </div>
                  </div>

                  <!-- Engagement Column - Downloads, Views, Comments -->
                  <div class="col-engagement flex flex-col justify-center">
                    <div class="space-y-1.5">
                      <!-- Downloads -->
                      <div class="flex items-center gap-2">
                        <div class="w-6 h-6 rounded-md bg-emerald-50 flex items-center justify-center">
                          <i class="fas fa-download text-emerald-500 text-[10px]"></i>
                        </div>
                        <span class="text-xs font-semibold text-gray-700">{{ doc.downloads.toLocaleString() }}</span>
                      </div>
                      <!-- Views -->
                      <div class="flex items-center gap-2">
                        <div class="w-6 h-6 rounded-md bg-blue-50 flex items-center justify-center">
                          <i class="fas fa-eye text-blue-500 text-[10px]"></i>
                        </div>
                        <span class="text-xs font-semibold text-gray-700">{{ (doc.downloads * 3 + 127).toLocaleString() }}</span>
                      </div>
                      <!-- Comments -->
                      <div class="flex items-center gap-2">
                        <div class="w-6 h-6 rounded-md bg-amber-50 flex items-center justify-center">
                          <i class="fas fa-comment text-amber-500 text-[10px]"></i>
                        </div>
                        <span class="text-xs font-semibold text-gray-700">{{ Math.floor(doc.downloads / 100) + 2 }}</span>
                      </div>
                    </div>
                  </div>

                  <!-- Size Column - Enhanced with version -->
                  <div class="col-size flex flex-col justify-center">
                    <div class="space-y-1">
                      <div class="flex items-center gap-1.5 text-gray-700">
                        <i class="fas fa-weight-hanging text-amber-500 text-[10px]"></i>
                        <span class="text-sm font-semibold">{{ formatFileSize(doc.size) }}</span>
                      </div>
                      <div class="flex items-center gap-1.5">
                        <span class="px-1.5 py-0.5 bg-gray-100 text-gray-500 text-[9px] rounded font-medium">v{{ Math.floor(Math.random() * 3) + 1 }}.{{ Math.floor(Math.random() * 9) }}</span>
                      </div>
                    </div>
                  </div>

                  <!-- Modified Column - Enhanced with created date -->
                  <div class="col-modified flex flex-col justify-center">
                    <div class="space-y-1">
                      <div class="flex items-center gap-1.5 text-gray-700">
                        <i class="fas fa-pen text-blue-400 text-[10px]"></i>
                        <span class="text-xs font-medium">{{ getRelativeTime(doc.updatedAt) }}</span>
                      </div>
                      <div class="flex items-center gap-1.5 text-gray-400">
                        <i class="fas fa-plus-circle text-[9px]"></i>
                        <span class="text-[10px]">{{ new Date(doc.createdAt).toLocaleDateString('en-US', { month: 'short', day: 'numeric' }) }}</span>
                      </div>
                    </div>
                  </div>

                  <!-- Actions Column - Enhanced -->
                  <div class="col-actions flex items-center justify-end gap-1">
                    <div class="flex items-center gap-1.5 opacity-0 group-hover:opacity-100 transition-all duration-300 transform translate-x-2 group-hover:translate-x-0">
                      <template v-if="currentView !== 'trash'">
                        <button
                          @click.stop="toggleStar(doc)"
                          :class="[
                            'w-9 h-9 rounded-xl flex items-center justify-center transition-all duration-200 shadow-sm',
                            doc.isStarred ? 'bg-amber-100 text-amber-600 hover:bg-amber-200 hover:shadow-md' : 'bg-gray-100 text-gray-500 hover:bg-amber-100 hover:text-amber-600 hover:shadow-md'
                          ]"
                          :title="doc.isStarred ? 'Remove from Starred' : 'Add to Starred'"
                        >
                          <i class="fas fa-star text-sm"></i>
                        </button>
                        <button
                          @click.stop="downloadDocument(doc)"
                          class="w-9 h-9 rounded-xl bg-gray-100 text-gray-500 hover:bg-teal-100 hover:text-teal-600 flex items-center justify-center transition-all duration-200 shadow-sm hover:shadow-md"
                          title="Download"
                        >
                          <i class="fas fa-download text-sm"></i>
                        </button>
                        <button
                          @click.stop="shareDocument(doc)"
                          class="w-9 h-9 rounded-xl bg-gray-100 text-gray-500 hover:bg-blue-100 hover:text-blue-600 flex items-center justify-center transition-all duration-200 shadow-sm hover:shadow-md"
                          title="Share"
                        >
                          <i class="fas fa-share-alt text-sm"></i>
                        </button>
                        <button
                          @click.stop="toggleComparison(doc)"
                          :class="[
                            'w-9 h-9 rounded-xl flex items-center justify-center transition-all duration-200 shadow-sm hover:shadow-md',
                            isInComparison(doc.id)
                              ? 'bg-purple-500 text-white'
                              : 'bg-gray-100 text-gray-500 hover:bg-purple-100 hover:text-purple-600'
                          ]"
                          :title="isInComparison(doc.id) ? 'Remove from Compare' : 'Add to Compare'"
                        >
                          <i class="fas fa-layer-group text-sm"></i>
                        </button>
                        <ContentActionsDropdown
                          :show-download="true"
                          @add-to-collection="openAddToCollection(doc)"
                          @share="shareDocument(doc)"
                          @download="downloadDocument(doc)"
                          @copy-link="() => copyDocumentLink(doc.id)"
                        />
                      </template>
                      <template v-else>
                        <button
                          @click.stop="restoreFromTrash(doc)"
                          class="w-9 h-9 rounded-xl bg-teal-100 text-teal-600 hover:bg-teal-200 flex items-center justify-center transition-all duration-200 shadow-sm hover:shadow-md"
                          title="Restore"
                        >
                          <i class="fas fa-undo text-sm"></i>
                        </button>
                        <button
                          @click.stop="permanentlyDelete(doc)"
                          class="w-9 h-9 rounded-xl bg-red-100 text-red-600 hover:bg-red-200 flex items-center justify-center transition-all duration-200 shadow-sm hover:shadow-md"
                          title="Delete permanently"
                        >
                          <i class="fas fa-trash text-sm"></i>
                        </button>
                      </template>
                    </div>
                    <!-- Always visible quick preview button -->
                    <button
                      @click.stop="viewDocument(doc)"
                      class="w-9 h-9 rounded-xl bg-gradient-to-br from-teal-50 to-teal-100 text-teal-600 hover:from-teal-100 hover:to-teal-200 flex items-center justify-center transition-all duration-200 shadow-sm hover:shadow-md"
                      title="Quick Preview"
                    >
                      <i class="fas fa-eye text-sm"></i>
                    </button>
                  </div>
                </div>
              </div>

              <!-- Table Footer with Stats & Pagination -->
              <div class="w-full px-4 py-3 bg-gradient-to-r from-gray-50 to-white border-t border-gray-100">
                <div class="flex items-center justify-between flex-wrap gap-3">
                  <!-- Left: Stats & Items Per Page -->
                  <div class="flex items-center gap-4 flex-wrap">
                    <span class="text-xs text-gray-500">
                      Showing <span class="font-semibold text-gray-700">{{ Math.min((currentPage - 1) * itemsPerPage + 1, filteredDocuments.length) }}</span>
                      to <span class="font-semibold text-gray-700">{{ Math.min(currentPage * itemsPerPage, filteredDocuments.length) }}</span>
                      of <span class="font-semibold text-gray-700">{{ filteredDocuments.length }}</span> files
                    </span>

                    <!-- Items Per Page Selector -->
                    <div class="flex items-center gap-2">
                      <span class="text-xs text-gray-500">Show:</span>
                      <select
                        v-model="itemsPerPage"
                        @change="changeItemsPerPage(Number(($event.target as HTMLSelectElement).value))"
                        class="text-xs px-2 py-1.5 rounded-lg border border-gray-200 bg-white text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-teal-500 cursor-pointer"
                      >
                        <option v-for="option in itemsPerPageOptions" :key="option" :value="option">
                          {{ option }}
                        </option>
                      </select>
                      <span class="text-xs text-gray-500">per page</span>
                    </div>

                    <div class="hidden sm:flex items-center gap-3 text-xs text-gray-400">
                      <span class="flex items-center gap-1">
                        <i class="fas fa-hdd text-amber-500"></i>
                        {{ formatTotalSize(filteredDocuments.reduce((sum, d) => sum + d.size, 0)) }} total
                      </span>
                      <span class="flex items-center gap-1">
                        <i class="fas fa-download text-green-500"></i>
                        {{ filteredDocuments.reduce((sum, d) => sum + d.downloads, 0).toLocaleString() }} downloads
                      </span>
                    </div>
                  </div>

                  <!-- Right: Pagination Controls -->
                  <div class="flex items-center gap-2">
                    <button
                      @click="prevPage"
                      :disabled="currentPage === 1"
                      :class="[
                        'px-3 py-1.5 text-xs rounded-lg transition-colors flex items-center gap-1.5 border',
                        currentPage === 1
                          ? 'text-gray-300 border-gray-100 cursor-not-allowed'
                          : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100 border-gray-200'
                      ]"
                    >
                      <i class="fas fa-chevron-left text-[10px]"></i>
                      Previous
                    </button>

                    <!-- Page Numbers -->
                    <div class="flex items-center gap-1">
                      <template v-for="page in totalPages" :key="page">
                        <button
                          v-if="page === 1 || page === totalPages || (page >= currentPage - 1 && page <= currentPage + 1)"
                          @click="goToPage(page)"
                          :class="[
                            'w-8 h-8 text-xs rounded-lg transition-colors flex items-center justify-center',
                            page === currentPage
                              ? 'font-medium text-teal-600 bg-teal-50'
                              : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100'
                          ]"
                        >
                          {{ page }}
                        </button>
                        <span
                          v-else-if="page === currentPage - 2 || page === currentPage + 2"
                          class="text-xs text-gray-400 px-1"
                        >...</span>
                      </template>
                    </div>

                    <button
                      @click="nextPage"
                      :disabled="currentPage === totalPages"
                      :class="[
                        'px-3 py-1.5 text-xs rounded-lg transition-colors flex items-center gap-1.5 border',
                        currentPage === totalPages
                          ? 'text-gray-300 border-gray-100 cursor-not-allowed'
                          : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100 border-gray-200'
                      ]"
                    >
                      Next
                      <i class="fas fa-chevron-right text-[10px]"></i>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Add to Collection Modal -->
    <AddToCollectionModal
      :show="showAddToCollectionModal"
      content-type="document"
      :content-id="selectedItemForCollection?.id ?? ''"
      :content-title="selectedItemForCollection?.title ?? ''"
      :content-thumbnail="selectedItemForCollection?.thumbnail"
      @close="showAddToCollectionModal = false; selectedItemForCollection = null"
      @added="handleAddedToCollection"
    />

    <!-- Comparison Components -->
    <ComparisonPanel />
    <ComparisonModal />
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

/* Selection Mode Animations */
@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-slideDown {
  animation: slideDown 0.3s ease-out;
}

/* Custom grid for 13 columns in selection mode */
.grid-cols-13 {
  grid-template-columns: 40px repeat(11, minmax(0, 1fr));
}

/* List view table row */
.list-row {
  display: flex;
  align-items: center;
  width: 100%;
  gap: 16px;
}

.list-row > * {
  flex-shrink: 0;
}

.col-checkbox { width: 40px; }
.col-file { flex: 1 1 auto; min-width: 300px; }
.col-author { width: 180px; }
.col-category { width: 150px; }
.col-engagement { width: 140px; }
.col-size { width: 100px; }
.col-modified { width: 140px; }
.col-actions { width: 120px; flex-shrink: 0; }

/* Hide columns on smaller screens */
@media (max-width: 640px) {
  .col-engagement { display: none !important; }
  .col-category { display: none !important; }
  .col-author { display: none !important; }
  .col-size { display: none !important; }
  .col-modified { display: none !important; }
}

/* Enhanced list row hover */
.list-row-enhanced {
  position: relative;
  overflow: hidden;
}

.list-row-enhanced::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 3px;
  background: linear-gradient(180deg, #14b8a6, #0d9488);
  transform: scaleY(0);
  transition: transform 0.2s ease;
}

.list-row-enhanced:hover::before {
  transform: scaleY(1);
}

/* File icon glow effect */
.file-icon-glow {
  position: relative;
}

.file-icon-glow::after {
  content: '';
  position: absolute;
  inset: -4px;
  border-radius: 16px;
  background: inherit;
  opacity: 0;
  filter: blur(8px);
  transition: opacity 0.3s ease;
  z-index: -1;
}

.group:hover .file-icon-glow::after {
  opacity: 0.4;
}

/* Download progress bar */
.download-bar {
  height: 4px;
  border-radius: 2px;
  background: #e5e7eb;
  overflow: hidden;
}

.download-bar-fill {
  height: 100%;
  border-radius: 2px;
  background: linear-gradient(90deg, #10b981, #14b8a6);
  transition: width 0.3s ease;
}

/* ============================================
   AI FEATURES STYLES
   ============================================ */

/* AI Search Badge */
.ai-search-badge-doc {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 18px;
  height: 18px;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 4px;
  color: white;
}

/* AI Suggestions Dropdown */
.ai-suggestions-dropdown-doc {
  position: absolute;
  top: calc(100% + 8px);
  left: 0;
  right: 0;
  background: white;
  border-radius: 0.75rem;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
  border: 1px solid #e2e8f0;
  z-index: 50;
  overflow: hidden;
  animation: docSuggestionSlideIn 0.2s ease-out;
}

@keyframes docSuggestionSlideIn {
  from { opacity: 0; transform: translateY(-8px); }
  to { opacity: 1; transform: translateY(0); }
}

.ai-suggestions-header-doc {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  background: #f0fdfa;
  border-bottom: 1px solid #ccfbf1;
  font-size: 0.75rem;
  font-weight: 600;
  color: #0d9488;
}

.ai-suggestions-list-doc {
  padding: 0.5rem 0;
}

.ai-suggestion-item-doc {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  width: 100%;
  padding: 0.625rem 1rem;
  background: none;
  border: none;
  text-align: left;
  font-size: 0.875rem;
  color: #334155;
  cursor: pointer;
  transition: all 0.15s ease;
}

.ai-suggestion-item-doc:hover {
  background: #f0fdfa;
  color: #0d9488;
}

.ai-suggestion-item-doc span {
  flex: 1;
}

/* AI Powered Badge */
.ai-powered-badge-doc {
  display: inline-flex;
  align-items: center;
  padding: 0.25rem 0.5rem;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border: 1px solid #99f6e4;
  border-radius: 100px;
  font-size: 0.625rem;
  font-weight: 600;
  color: #0d9488;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

/* AI Folder Suggestion Card */
.ai-folder-suggestion-card {
  padding: 1rem;
  background: #fafffe;
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  transition: all 0.2s ease;
}

.ai-folder-suggestion-card:hover {
  border-color: #99f6e4;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.1);
}

/* AI Classification Badge */
.ai-classification-badge-doc {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.25rem 0.5rem;
  border-radius: 0.375rem;
  font-size: 0.625rem;
  font-weight: 600;
  border: 1px solid;
}

.ai-confidence-mini {
  padding: 0.125rem 0.25rem;
  background: rgba(0, 0, 0, 0.05);
  border-radius: 0.25rem;
  font-size: 0.5625rem;
  font-weight: 700;
}

/* Responsive */
@media (max-width: 768px) {
  .ai-suggestions-dropdown-doc {
    left: -1rem;
    right: -1rem;
  }
}
</style>
