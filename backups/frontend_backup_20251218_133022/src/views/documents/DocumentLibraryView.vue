<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'
import { useReducedMotion } from '@/composables/useReducedMotion'
import { PageHeader, StatsBar } from '@/components/base'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import type { StatItem } from '@/components/base/StatsBar.vue'
import ErrorState from '@/components/base/ErrorState.vue'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'
import Menu from 'primevue/menu'
import Tag from 'primevue/tag'
import Checkbox from 'primevue/checkbox'
import ProgressBar from 'primevue/progressbar'
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'

const { locale } = useI18n()
const route = useRoute()
const router = useRouter()
const prefersReducedMotion = useReducedMotion()

const isRTL = computed(() => locale.value === 'ar')

// Animation control
const isContentVisible = ref(false)
const shouldAnimate = computed(() => !prefersReducedMotion.value)
const LOADING_DELAY = 600

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: isRTL.value ? 'المستندات' : 'Documents', to: '/documents' },
  { label: isRTL.value ? library.value.nameArabic : library.value.name }
])

// Loading and error state
const isLoading = ref(true)
const error = ref<Error | null>(null)

const searchQuery = ref('')
const activeView = ref<'grid' | 'list'>('list')
const selectedDocuments = ref<string[]>([])
const isDragOver = ref(false)
const uploadProgress = ref(0)
const isUploading = ref(false)
const showDeleteDialog = ref(false)
const showShareDialog = ref(false)
const showRenameDialog = ref(false)
const showMoveDialog = ref(false)
const renameValue = ref('')

// Pagination
const currentPage = ref(1)
const itemsPerPage = ref(10)

// Filters
const filterType = ref<'all' | 'folders' | 'documents'>('all')
const filterStatus = ref<string>('all')
const filterFileType = ref<string>('all')
const showFilters = ref(false)

const typeOptions = computed(() => [
  { value: 'all', label: isRTL.value ? 'الكل' : 'All', icon: 'pi-th-large' },
  { value: 'folders', label: isRTL.value ? 'المجلدات' : 'Folders', icon: 'pi-folder' },
  { value: 'documents', label: isRTL.value ? 'المستندات' : 'Documents', icon: 'pi-file' }
])

const statusOptions = computed(() => [
  { value: 'all', label: isRTL.value ? 'جميع الحالات' : 'All Status' },
  { value: 'published', label: isRTL.value ? 'منشور' : 'Published' },
  { value: 'draft', label: isRTL.value ? 'مسودة' : 'Draft' },
  { value: 'review', label: isRTL.value ? 'قيد المراجعة' : 'In Review' },
  { value: 'archived', label: isRTL.value ? 'مؤرشف' : 'Archived' }
])

const fileTypeOptions = computed(() => [
  { value: 'all', label: isRTL.value ? 'جميع الأنواع' : 'All Types' },
  { value: 'pdf', label: 'PDF' },
  { value: 'docx', label: 'Word' },
  { value: 'xlsx', label: 'Excel' },
  { value: 'pptx', label: 'PowerPoint' },
  { value: 'image', label: isRTL.value ? 'صور' : 'Images' }
])

const perPageOptions = [
  { value: 5, label: '5' },
  { value: 10, label: '10' },
  { value: 20, label: '20' },
  { value: 50, label: '50' },
  { value: 100, label: '100' }
]

const activeFiltersCount = computed(() => {
  let count = 0
  if (filterType.value !== 'all') count++
  if (filterStatus.value !== 'all') count++
  if (filterFileType.value !== 'all') count++
  return count
})

const clearAllFilters = () => {
  filterType.value = 'all'
  filterStatus.value = 'all'
  filterFileType.value = 'all'
  searchQuery.value = ''
  currentPage.value = 1
}

interface Document {
  id: string
  name: string
  nameArabic?: string
  type: string
  size: string
  sizeBytes: number
  version: string
  status: 'published' | 'draft' | 'review' | 'archived'
  modifiedBy: string
  modifiedByAvatar?: string
  modifiedAt: Date
  isFolder?: boolean
  itemCount?: number
  isLocked?: boolean
  lockedBy?: string
}

const library = ref({
  id: route.params.id || 'operations',
  name: 'Operations Documents',
  nameArabic: 'مستندات العمليات',
  description: 'All operational documents, procedures, and guidelines for tournament management',
  descriptionArabic: 'جميع المستندات التشغيلية والإجراءات والإرشادات لإدارة البطولة',
  totalSize: '2.4 GB',
  lastUpdated: new Date()
})

const stats = ref({
  totalDocuments: 156,
  totalFolders: 24,
  totalSize: '2.4 GB',
  recentlyModified: 12,
  sharedWithMe: 8,
  pendingReview: 5
})

// StatsBar items
const statsBarItems = computed<StatItem[]>(() => [
  {
    icon: 'pi pi-file',
    value: stats.value.totalDocuments,
    label: 'Documents',
    labelArabic: 'مستند',
    colorClass: 'primary'
  },
  {
    icon: 'pi pi-folder',
    value: stats.value.totalFolders,
    label: 'Folders',
    labelArabic: 'مجلد',
    colorClass: 'info'
  },
  {
    icon: 'pi pi-database',
    value: stats.value.totalSize,
    label: 'Total Size',
    labelArabic: 'الحجم الكلي',
    colorClass: 'success'
  },
  {
    icon: 'pi pi-share-alt',
    value: stats.value.sharedWithMe,
    label: 'Shared with Me',
    labelArabic: 'مشاركة معي',
    colorClass: 'warning'
  }
])

const folders = ref<Document[]>([
  {
    id: 'f1',
    name: 'Stadium Operations',
    nameArabic: 'عمليات الملعب',
    type: 'folder',
    size: '450 MB',
    sizeBytes: 471859200,
    version: '-',
    status: 'published',
    modifiedBy: 'Ahmed Al-Harbi',
    modifiedAt: new Date(),
    isFolder: true,
    itemCount: 23
  },
  {
    id: 'f2',
    name: 'Security Protocols',
    nameArabic: 'بروتوكولات الأمن',
    type: 'folder',
    size: '280 MB',
    sizeBytes: 293601280,
    version: '-',
    status: 'published',
    modifiedBy: 'Sara Al-Mutairi',
    modifiedAt: new Date(Date.now() - 86400000),
    isFolder: true,
    itemCount: 15
  },
  {
    id: 'f3',
    name: 'Team Coordination',
    nameArabic: 'تنسيق الفرق',
    type: 'folder',
    size: '120 MB',
    sizeBytes: 125829120,
    version: '-',
    status: 'published',
    modifiedBy: 'Mohammed Al-Dosari',
    modifiedAt: new Date(Date.now() - 172800000),
    isFolder: true,
    itemCount: 8
  }
])

const documents = ref<Document[]>([
  {
    id: '1',
    name: 'Stadium Safety Protocol v2.1',
    nameArabic: 'بروتوكول سلامة الملعب 2.1',
    type: 'pdf',
    size: '2.4 MB',
    sizeBytes: 2516582,
    version: '2.1',
    status: 'published',
    modifiedBy: 'Ahmed Al-Harbi',
    modifiedByAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Ahmed',
    modifiedAt: new Date(),
    isLocked: false
  },
  {
    id: '2',
    name: 'Event Operations Checklist',
    nameArabic: 'قائمة فحص عمليات الفعاليات',
    type: 'xlsx',
    size: '156 KB',
    sizeBytes: 159744,
    version: '1.3',
    status: 'review',
    modifiedBy: 'Sara Al-Mutairi',
    modifiedByAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sara',
    modifiedAt: new Date(Date.now() - 86400000),
    isLocked: true,
    lockedBy: 'Sara Al-Mutairi'
  },
  {
    id: '3',
    name: 'Team Coordination Guide',
    nameArabic: 'دليل تنسيق الفرق',
    type: 'docx',
    size: '890 KB',
    sizeBytes: 911360,
    version: '3.0',
    status: 'published',
    modifiedBy: 'Mohammed Al-Dosari',
    modifiedByAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Mohammed',
    modifiedAt: new Date(Date.now() - 172800000),
    isLocked: false
  },
  {
    id: '4',
    name: 'Volunteer Training Presentation',
    nameArabic: 'عرض تدريب المتطوعين',
    type: 'pptx',
    size: '15.2 MB',
    sizeBytes: 15938355,
    version: '1.0',
    status: 'draft',
    modifiedBy: 'Fatima Al-Zahrani',
    modifiedByAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Fatima',
    modifiedAt: new Date(Date.now() - 259200000),
    isLocked: false
  },
  {
    id: '5',
    name: 'Media Access Guidelines',
    nameArabic: 'إرشادات الوصول الإعلامي',
    type: 'pdf',
    size: '3.8 MB',
    sizeBytes: 3984588,
    version: '2.0',
    status: 'published',
    modifiedBy: 'Khalid Al-Otaibi',
    modifiedByAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Khalid',
    modifiedAt: new Date(Date.now() - 345600000),
    isLocked: false
  },
  {
    id: '6',
    name: 'Emergency Response Plan',
    nameArabic: 'خطة الاستجابة للطوارئ',
    type: 'pdf',
    size: '5.1 MB',
    sizeBytes: 5347737,
    version: '4.2',
    status: 'archived',
    modifiedBy: 'Omar Al-Ghamdi',
    modifiedByAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Omar',
    modifiedAt: new Date(Date.now() - 604800000),
    isLocked: false
  }
])

const allItems = computed(() => [...folders.value, ...documents.value])

const filteredItems = computed(() => {
  let items = allItems.value

  // Filter by type (folders/documents)
  if (filterType.value === 'folders') {
    items = items.filter(item => item.isFolder)
  } else if (filterType.value === 'documents') {
    items = items.filter(item => !item.isFolder)
  }

  // Filter by status
  if (filterStatus.value !== 'all') {
    items = items.filter(item => item.status === filterStatus.value)
  }

  // Filter by file type
  if (filterFileType.value !== 'all') {
    if (filterFileType.value === 'image') {
      items = items.filter(item => ['jpg', 'jpeg', 'png', 'gif', 'webp'].includes(item.type))
    } else {
      items = items.filter(item => item.type === filterFileType.value || item.type === filterFileType.value.replace('x', ''))
    }
  }

  // Filter by search query
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    items = items.filter(item =>
      item.name.toLowerCase().includes(query) ||
      (item.nameArabic && item.nameArabic.includes(searchQuery.value))
    )
  }

  return items
})

// Pagination computed properties
const totalPages = computed(() => Math.ceil(filteredItems.value.length / itemsPerPage.value))

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredItems.value.slice(start, end)
})

const paginationInfo = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value + 1
  const end = Math.min(currentPage.value * itemsPerPage.value, filteredItems.value.length)
  const total = filteredItems.value.length
  return { start, end, total }
})

const visiblePages = computed(() => {
  const pages: (number | string)[] = []
  const total = totalPages.value
  const current = currentPage.value

  if (total <= 7) {
    for (let i = 1; i <= total; i++) pages.push(i)
  } else {
    pages.push(1)
    if (current > 3) pages.push('...')

    const start = Math.max(2, current - 1)
    const end = Math.min(total - 1, current + 1)

    for (let i = start; i <= end; i++) pages.push(i)

    if (current < total - 2) pages.push('...')
    pages.push(total)
  }

  return pages
})

const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
  }
}

// Reset pagination when filters change
const onFilterChange = () => {
  currentPage.value = 1
}

// Alias for search input
const onSearchChange = onFilterChange

const getName = (item: Document) => {
  return isRTL.value && item.nameArabic ? item.nameArabic : item.name
}

const formatDate = (date: Date) => {
  return date.toLocaleDateString(locale.value === 'ar' ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const getRelativeTime = (date: Date) => {
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))

  if (diffDays === 0) return isRTL.value ? 'اليوم' : 'Today'
  if (diffDays === 1) return isRTL.value ? 'أمس' : 'Yesterday'
  if (diffDays < 7) return isRTL.value ? `منذ ${diffDays} أيام` : `${diffDays} days ago`
  return formatDate(date)
}

const getFileIcon = (type: string): string => {
  const icons: Record<string, string> = {
    folder: 'pi-folder',
    pdf: 'pi-file-pdf',
    docx: 'pi-file-word',
    doc: 'pi-file-word',
    xlsx: 'pi-file-excel',
    xls: 'pi-file-excel',
    pptx: 'pi-file',
    ppt: 'pi-file',
    jpg: 'pi-image',
    jpeg: 'pi-image',
    png: 'pi-image',
    mp4: 'pi-video',
    zip: 'pi-file-export',
    default: 'pi-file'
  }
  return icons[type] || icons.default
}

const getFileColor = (type: string): string => {
  const colors: Record<string, string> = {
    folder: '#f59e0b',
    pdf: '#ef4444',
    docx: '#3b82f6',
    doc: '#3b82f6',
    xlsx: '#22c55e',
    xls: '#22c55e',
    pptx: '#f97316',
    ppt: '#f97316',
    jpg: '#8b5cf6',
    jpeg: '#8b5cf6',
    png: '#8b5cf6',
    mp4: '#ec4899',
    zip: '#6b7280',
    default: '#6b7280'
  }
  return colors[type] || colors.default
}

const getStatusSeverity = (status: string): "success" | "info" | "warn" | "danger" | "secondary" | "contrast" | undefined => {
  const severities: Record<string, "success" | "info" | "warn" | "danger" | "secondary" | "contrast"> = {
    'published': 'success',
    'draft': 'secondary',
    'review': 'warn',
    'archived': 'info'
  }
  return severities[status] || 'secondary'
}

const getStatusLabel = (status: string) => {
  const labels: Record<string, { en: string, ar: string }> = {
    'published': { en: 'Published', ar: 'منشور' },
    'draft': { en: 'Draft', ar: 'مسودة' },
    'review': { en: 'In Review', ar: 'قيد المراجعة' },
    'archived': { en: 'Archived', ar: 'مؤرشف' }
  }
  return isRTL.value ? labels[status]?.ar : labels[status]?.en || status
}

const actionMenu = ref()
const currentItem = ref<Document | null>(null)

const actionMenuItems = computed(() => [
  { label: isRTL.value ? 'فتح' : 'Open', icon: 'pi pi-external-link', command: () => openDocument(currentItem.value) },
  { label: isRTL.value ? 'تحميل' : 'Download', icon: 'pi pi-download', command: () => downloadDocument(currentItem.value) },
  { label: isRTL.value ? 'مشاركة' : 'Share', icon: 'pi pi-share-alt', command: () => shareDocument(currentItem.value) },
  { separator: true },
  { label: isRTL.value ? 'إعادة تسمية' : 'Rename', icon: 'pi pi-pencil', command: () => renameDocument(currentItem.value) },
  { label: isRTL.value ? 'نقل' : 'Move', icon: 'pi pi-folder-open', command: () => moveDocument(currentItem.value) },
  { separator: true },
  { label: isRTL.value ? 'سجل الإصدارات' : 'Version History', icon: 'pi pi-history', command: () => viewVersions(currentItem.value) },
  { label: isRTL.value ? 'الخصائص' : 'Properties', icon: 'pi pi-info-circle', command: () => viewProperties(currentItem.value) },
  { separator: true },
  { label: isRTL.value ? 'حذف' : 'Delete', icon: 'pi pi-trash', class: 'text-red-500', command: () => deleteDocument(currentItem.value) }
])

const toggleActionMenu = (event: Event, item: Document) => {
  currentItem.value = item
  actionMenu.value.toggle(event)
}

const openDocument = (item: Document | null) => {
  if (!item) return
  if (item.isFolder) {
    router.push({ name: 'document-library', params: { id: item.id } })
  } else {
    router.push({ name: 'document-detail', params: { id: item.id } })
  }
}

const downloadDocument = (item: Document | null) => {
  if (!item) return
  // Simulate download
  const link = document.createElement('a')
  link.href = '#'
  link.download = item.name
  link.click()
}

const shareDocument = (item: Document | null) => {
  if (!item) return
  currentItem.value = item
  showShareDialog.value = true
}

const renameDocument = (item: Document | null) => {
  if (!item) return
  currentItem.value = item
  renameValue.value = item.name
  showRenameDialog.value = true
}

const confirmRename = () => {
  if (currentItem.value && renameValue.value) {
    const doc = documents.value.find(d => d.id === currentItem.value?.id)
    if (doc) {
      doc.name = renameValue.value
    }
    showRenameDialog.value = false
    renameValue.value = ''
  }
}

const moveDocument = (item: Document | null) => {
  if (!item) return
  currentItem.value = item
  showMoveDialog.value = true
}

const viewVersions = (item: Document | null) => {
  if (!item) return
  // Navigate to versions view or open versions dialog
  router.push({ name: 'document-detail', params: { id: item.id }, query: { tab: 'versions' } })
}

const viewProperties = (item: Document | null) => {
  if (!item) return
  // Navigate to properties view
  router.push({ name: 'document-detail', params: { id: item.id }, query: { tab: 'properties' } })
}

const deleteDocument = (item: Document | null) => {
  if (!item) return
  currentItem.value = item
  showDeleteDialog.value = true
}

const confirmDelete = () => {
  if (currentItem.value) {
    documents.value = documents.value.filter(d => d.id !== currentItem.value?.id)
    showDeleteDialog.value = false
    currentItem.value = null
  }
}

const handleItemClick = (item: Document) => {
  openDocument(item)
}

const uploadDocument = () => {
  const input = document.createElement('input')
  input.type = 'file'
  input.multiple = true
  input.onchange = (e) => {
    const files = (e.target as HTMLInputElement).files
    if (files) handleFiles(files)
  }
  input.click()
}

const handleDrop = (e: DragEvent) => {
  e.preventDefault()
  isDragOver.value = false
  if (e.dataTransfer?.files) {
    handleFiles(e.dataTransfer.files)
  }
}

const handleFiles = (_files: FileList) => {
  isUploading.value = true
  uploadProgress.value = 0
  // _files would be processed here in production

  const interval = setInterval(() => {
    uploadProgress.value += 10
    if (uploadProgress.value >= 100) {
      clearInterval(interval)
      setTimeout(() => {
        isUploading.value = false
        uploadProgress.value = 0
      }, 500)
    }
  }, 200)
}

const toggleSelect = (id: string) => {
  const index = selectedDocuments.value.indexOf(id)
  if (index === -1) {
    selectedDocuments.value.push(id)
  } else {
    selectedDocuments.value.splice(index, 1)
  }
}

const isSelected = (id: string) => selectedDocuments.value.includes(id)

const selectAll = () => {
  if (selectedDocuments.value.length === allItems.value.length) {
    selectedDocuments.value = []
  } else {
    selectedDocuments.value = allItems.value.map(item => item.id)
  }
}

async function loadDocuments() {
  try {
    error.value = null
    isLoading.value = true

    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, LOADING_DELAY))

    isLoading.value = false

    if (shouldAnimate.value) {
      requestAnimationFrame(() => {
        isContentVisible.value = true
      })
    } else {
      isContentVisible.value = true
    }
  } catch (e) {
    error.value = e as Error
    isLoading.value = false
  }
}

async function handleRetry() {
  await loadDocuments()
}

onMounted(() => {
  loadDocuments()
})
</script>

<template>
  <div class="document-library-view" :class="{ rtl: isRTL }">
    <!-- Page Header with Gradient -->
    <PageHeader
      :title="isRTL ? library.nameArabic : library.name"
      :description="isRTL ? library.descriptionArabic : library.description"
      :breadcrumbs="breadcrumbs"
      padding-bottom="xl"
      background-variant="gradient"
      :animated="shouldAnimate"
    >
      <template #actions>
        <button class="btn-secondary" @click="() => {}">
          <i class="pi pi-folder-plus"></i>
          <span>{{ isRTL ? 'مجلد جديد' : 'New Folder' }}</span>
        </button>
        <button class="btn-create" @click="uploadDocument">
          <span class="btn-icon"><i class="pi pi-upload"></i></span>
          <span class="btn-text">{{ isRTL ? 'رفع ملفات' : 'Upload Files' }}</span>
        </button>
      </template>
    </PageHeader>

    <!-- Stats Bar -->
    <StatsBar
      :stats="statsBarItems"
      :loading="isLoading"
      overlap="lg"
      :animated="shouldAnimate"
      :animate-numbers="shouldAnimate"
      :counter-duration="1200"
    />

    <!-- Upload Drop Zone -->
    <div
      class="upload-zone"
      :class="{ 'drag-over': isDragOver, 'uploading': isUploading }"
      @dragover.prevent="isDragOver = true"
      @dragleave.prevent="isDragOver = false"
      @drop="handleDrop"
    >
      <div class="upload-content" v-if="!isUploading">
        <div class="upload-icon">
          <i class="pi pi-cloud-upload"></i>
        </div>
        <p class="upload-text">
          {{ isRTL ? 'اسحب الملفات وأفلتها هنا أو' : 'Drag and drop files here or' }}
          <button @click="uploadDocument" class="upload-link">
            {{ isRTL ? 'تصفح' : 'browse' }}
          </button>
        </p>
        <span class="upload-hint">{{ isRTL ? 'PDF, Word, Excel, PowerPoint حتى 100 ميجابايت' : 'PDF, Word, Excel, PowerPoint up to 100MB' }}</span>
      </div>
      <div class="upload-progress" v-else>
        <i class="pi pi-spin pi-spinner"></i>
        <span>{{ isRTL ? 'جاري الرفع...' : 'Uploading...' }}</span>
        <ProgressBar :value="uploadProgress" :showValue="true" />
      </div>
    </div>

    <!-- Toolbar -->
    <div class="toolbar">
      <div class="toolbar-row">
        <div class="toolbar-left">
          <div class="search-box">
            <i class="pi pi-search"></i>
            <InputText
              v-model="searchQuery"
              :placeholder="isRTL ? 'البحث في المستندات...' : 'Search documents...'"
              class="search-input"
              @input="onSearchChange"
            />
          </div>

          <!-- Type Toggle -->
          <div class="type-toggle">
            <button
              v-for="option in typeOptions"
              :key="option.value"
              :class="{ active: filterType === option.value }"
              @click="filterType = option.value as 'all' | 'folders' | 'documents'; onFilterChange()"
            >
              <i :class="['pi', option.icon]"></i>
              <span>{{ option.label }}</span>
            </button>
          </div>

          <!-- Filter Button -->
          <button
            class="filter-btn"
            :class="{ active: showFilters, 'has-filters': activeFiltersCount > 0 }"
            @click="showFilters = !showFilters"
          >
            <i class="pi pi-filter"></i>
            <span>{{ isRTL ? 'تصفية' : 'Filters' }}</span>
            <span v-if="activeFiltersCount > 0" class="filter-badge">{{ activeFiltersCount }}</span>
          </button>

          <div class="bulk-actions" v-if="selectedDocuments.length > 0">
            <span class="selection-count">
              {{ selectedDocuments.length }} {{ isRTL ? 'محدد' : 'selected' }}
            </span>
            <Button icon="pi pi-download" text size="small" v-tooltip.top="isRTL ? 'تحميل' : 'Download'" />
            <Button icon="pi pi-share-alt" text size="small" v-tooltip.top="isRTL ? 'مشاركة' : 'Share'" />
            <Button icon="pi pi-trash" text size="small" severity="danger" v-tooltip.top="isRTL ? 'حذف' : 'Delete'" />
          </div>
        </div>

        <div class="toolbar-right">
          <!-- Items Per Page -->
          <div class="per-page-control">
            <span class="per-page-label">{{ isRTL ? 'عرض' : 'Show' }}</span>
            <select
              v-model="itemsPerPage"
              @change="onFilterChange"
              class="per-page-select"
            >
              <option v-for="option in perPageOptions" :key="option.value" :value="option.value">
                {{ option.label }}
              </option>
            </select>
            <span class="per-page-label">{{ isRTL ? 'عنصر' : 'items' }}</span>
          </div>

          <div class="view-toggle">
            <button
              :class="{ active: activeView === 'list' }"
              @click="activeView = 'list'"
              :aria-label="isRTL ? 'عرض قائمة' : 'List view'"
            >
              <i class="pi pi-list"></i>
            </button>
            <button
              :class="{ active: activeView === 'grid' }"
              @click="activeView = 'grid'"
              :aria-label="isRTL ? 'عرض شبكة' : 'Grid view'"
            >
              <i class="pi pi-th-large"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Expanded Filters Panel -->
      <div v-if="showFilters" class="filters-panel">
        <div class="filter-group">
          <label class="filter-label">{{ isRTL ? 'الحالة' : 'Status' }}</label>
          <div class="filter-options">
            <button
              v-for="option in statusOptions"
              :key="option.value"
              :class="['filter-chip', { active: filterStatus === option.value }]"
              @click="filterStatus = option.value; onFilterChange()"
            >
              {{ option.label }}
            </button>
          </div>
        </div>

        <div class="filter-group">
          <label class="filter-label">{{ isRTL ? 'نوع الملف' : 'File Type' }}</label>
          <div class="filter-options">
            <button
              v-for="option in fileTypeOptions"
              :key="option.value"
              :class="['filter-chip', { active: filterFileType === option.value }]"
              @click="filterFileType = option.value; onFilterChange()"
            >
              {{ option.label }}
            </button>
          </div>
        </div>

        <div class="filter-actions" v-if="activeFiltersCount > 0 || searchQuery">
          <button class="clear-filters-btn" @click="clearAllFilters">
            <i class="pi pi-times"></i>
            {{ isRTL ? 'مسح الكل' : 'Clear All' }}
          </button>
        </div>
      </div>

      <!-- Active Filters Tags -->
      <div v-if="(activeFiltersCount > 0 || searchQuery) && !showFilters" class="active-filters">
        <span class="active-filters-label">{{ isRTL ? 'التصفية النشطة:' : 'Active filters:' }}</span>

        <span v-if="searchQuery" class="filter-tag">
          <i class="pi pi-search"></i>
          "{{ searchQuery }}"
          <button @click="searchQuery = ''; onFilterChange()"><i class="pi pi-times"></i></button>
        </span>

        <span v-if="filterType !== 'all'" class="filter-tag">
          <i :class="['pi', typeOptions.find(o => o.value === filterType)?.icon]"></i>
          {{ typeOptions.find(o => o.value === filterType)?.label }}
          <button @click="filterType = 'all'; onFilterChange()"><i class="pi pi-times"></i></button>
        </span>

        <span v-if="filterStatus !== 'all'" class="filter-tag">
          {{ statusOptions.find(o => o.value === filterStatus)?.label }}
          <button @click="filterStatus = 'all'; onFilterChange()"><i class="pi pi-times"></i></button>
        </span>

        <span v-if="filterFileType !== 'all'" class="filter-tag">
          {{ fileTypeOptions.find(o => o.value === filterFileType)?.label }}
          <button @click="filterFileType = 'all'; onFilterChange()"><i class="pi pi-times"></i></button>
        </span>

        <button class="clear-all-link" @click="clearAllFilters">
          {{ isRTL ? 'مسح الكل' : 'Clear all' }}
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="loading-state">
      <div v-if="activeView === 'list'" class="table-skeleton">
        <div class="skeleton-header">
          <Skeleton width="100%" height="48px" />
        </div>
        <div class="skeleton-row" v-for="i in 6" :key="i">
          <Skeleton width="100%" height="56px" />
        </div>
      </div>
      <div v-else class="grid-skeleton">
        <div class="skeleton-card" v-for="i in 9" :key="i">
          <Skeleton width="100%" height="120px" borderRadius="12px" />
        </div>
      </div>
    </div>

    <!-- Error State -->
    <ErrorState
      v-else-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل المستندات' : 'Failed to load documents'"
      show-retry
      @retry="handleRetry"
    />

    <!-- Content -->
    <div v-else class="content-area">
      <!-- List View -->
      <div v-if="activeView === 'list'" class="document-table">
        <DataTable
          :value="paginatedItems"
          stripedRows
          responsiveLayout="scroll"
          class="premium-table"
          :rowHover="true"
          @row-click="(e: any) => handleItemClick(e.data)"
        >
          <Column style="width: 48px">
            <template #header>
              <Checkbox
                :modelValue="selectedDocuments.length === allItems.length && allItems.length > 0"
                :binary="true"
                @change="selectAll"
              />
            </template>
            <template #body="{ data }">
              <Checkbox
                :modelValue="isSelected(data.id)"
                :binary="true"
                @change="toggleSelect(data.id)"
                @click.stop
              />
            </template>
          </Column>

          <Column field="name" :header="isRTL ? 'الاسم' : 'Name'" style="min-width: 300px">
            <template #body="{ data }">
              <div class="file-name-cell">
                <div class="file-icon" :style="{ backgroundColor: getFileColor(data.type) + '15', color: getFileColor(data.type) }">
                  <i :class="['pi', getFileIcon(data.type)]"></i>
                </div>
                <div class="file-info">
                  <span class="file-name">{{ getName(data) }}</span>
                  <span class="file-meta" v-if="data.isFolder">
                    {{ data.itemCount }} {{ isRTL ? 'عنصر' : 'items' }}
                  </span>
                </div>
                <div class="file-badges">
                  <span v-if="data.isLocked" class="lock-badge" v-tooltip.top="isRTL ? `مقفل بواسطة ${data.lockedBy}` : `Locked by ${data.lockedBy}`">
                    <i class="pi pi-lock"></i>
                  </span>
                </div>
              </div>
            </template>
          </Column>

          <Column field="status" :header="isRTL ? 'الحالة' : 'Status'" style="width: 120px">
            <template #body="{ data }">
              <Tag
                v-if="!data.isFolder"
                :value="getStatusLabel(data.status)"
                :severity="getStatusSeverity(data.status)"
                class="status-tag"
              />
              <span v-else class="folder-dash">-</span>
            </template>
          </Column>

          <Column field="version" :header="isRTL ? 'الإصدار' : 'Version'" style="width: 100px">
            <template #body="{ data }">
              <span v-if="!data.isFolder" class="version-badge">v{{ data.version }}</span>
              <span v-else class="folder-dash">-</span>
            </template>
          </Column>

          <Column field="size" :header="isRTL ? 'الحجم' : 'Size'" style="width: 100px">
            <template #body="{ data }">
              <span class="size-text">{{ data.size }}</span>
            </template>
          </Column>

          <Column field="modifiedBy" :header="isRTL ? 'التعديل بواسطة' : 'Modified By'" style="width: 180px">
            <template #body="{ data }">
              <div class="modified-by-cell">
                <img
                  v-if="data.modifiedByAvatar"
                  :src="data.modifiedByAvatar"
                  :alt="data.modifiedBy"
                  class="user-avatar"
                />
                <span class="user-initial" v-else>{{ data.modifiedBy.charAt(0) }}</span>
                <span class="user-name">{{ data.modifiedBy }}</span>
              </div>
            </template>
          </Column>

          <Column field="modifiedAt" :header="isRTL ? 'آخر تعديل' : 'Last Modified'" style="width: 140px">
            <template #body="{ data }">
              <span class="date-text" v-tooltip.top="formatDate(data.modifiedAt)">
                {{ getRelativeTime(data.modifiedAt) }}
              </span>
            </template>
          </Column>

          <Column style="width: 60px">
            <template #body="{ data }">
              <Button
                icon="pi pi-ellipsis-v"
                text
                rounded
                size="small"
                class="action-menu-btn"
                @click.stop="(e: Event) => toggleActionMenu(e, data)"
              />
            </template>
          </Column>
        </DataTable>

        <Menu ref="actionMenu" :model="actionMenuItems" popup class="action-menu" />
      </div>

      <!-- Grid View -->
      <div v-else class="document-grid">
        <div
          v-for="item in paginatedItems"
          :key="item.id"
          class="document-card"
          :class="{ selected: isSelected(item.id), folder: item.isFolder }"
          @click="handleItemClick(item)"
          role="button"
          tabindex="0"
          @keydown.enter="handleItemClick(item)"
        >
          <div class="card-select" @click.stop>
            <Checkbox
              :modelValue="isSelected(item.id)"
              :binary="true"
              @change="toggleSelect(item.id)"
            />
          </div>

          <div class="card-icon" :style="{ backgroundColor: getFileColor(item.type) + '15' }">
            <i :class="['pi', getFileIcon(item.type)]" :style="{ color: getFileColor(item.type) }"></i>
            <span v-if="item.isLocked" class="lock-indicator">
              <i class="pi pi-lock"></i>
            </span>
          </div>

          <div class="card-content">
            <h4 class="card-title">{{ getName(item) }}</h4>
            <div class="card-meta">
              <span v-if="item.isFolder" class="item-count">
                <i class="pi pi-file"></i>
                {{ item.itemCount }} {{ isRTL ? 'عنصر' : 'items' }}
              </span>
              <span v-else class="file-size">{{ item.size }}</span>
              <span class="separator">•</span>
              <span class="modified-time">{{ getRelativeTime(item.modifiedAt) }}</span>
            </div>
          </div>

          <div class="card-footer" v-if="!item.isFolder">
            <Tag
              :value="getStatusLabel(item.status)"
              :severity="getStatusSeverity(item.status)"
              class="status-tag small"
            />
            <span class="version-text">v{{ item.version }}</span>
          </div>

          <button
            class="card-actions"
            @click.stop="(e: Event) => toggleActionMenu(e, item)"
            :aria-label="isRTL ? 'خيارات' : 'Options'"
          >
            <i class="pi pi-ellipsis-v"></i>
          </button>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="filteredItems.length === 0 && !isLoading" class="empty-state">
        <div class="empty-icon">
          <i class="pi pi-folder-open"></i>
        </div>
        <h3>{{ isRTL ? 'لا توجد مستندات' : 'No Documents Found' }}</h3>
        <p>{{ isRTL ? 'ابدأ برفع ملفاتك الأولى' : 'Start by uploading your first files' }}</p>
        <Button
          :label="isRTL ? 'رفع ملفات' : 'Upload Files'"
          icon="pi pi-upload"
          @click="uploadDocument"
          class="upload-btn"
        />
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="filteredItems.length > 0 && totalPages > 1" class="pagination-wrapper">
      <div class="pagination-info">
        <span>
          {{ isRTL ? `عرض ${paginationInfo.start} إلى ${paginationInfo.end} من ${paginationInfo.total} عنصر` : `Showing ${paginationInfo.start} to ${paginationInfo.end} of ${paginationInfo.total} items` }}
        </span>
      </div>

      <div class="pagination-controls">
        <button
          class="page-btn nav-btn"
          :disabled="currentPage === 1"
          @click="goToPage(currentPage - 1)"
          :aria-label="isRTL ? 'الصفحة السابقة' : 'Previous page'"
        >
          <i :class="isRTL ? 'pi pi-angle-right' : 'pi pi-angle-left'"></i>
        </button>

        <template v-for="page in visiblePages" :key="page">
          <span v-if="page === '...'" class="page-ellipsis">...</span>
          <button
            v-else
            class="page-btn"
            :class="{ active: currentPage === page }"
            @click="goToPage(page as number)"
          >
            {{ page }}
          </button>
        </template>

        <button
          class="page-btn nav-btn"
          :disabled="currentPage === totalPages"
          @click="goToPage(currentPage + 1)"
          :aria-label="isRTL ? 'الصفحة التالية' : 'Next page'"
        >
          <i :class="isRTL ? 'pi pi-angle-left' : 'pi pi-angle-right'"></i>
        </button>
      </div>

    </div>

    <!-- Delete Confirmation Dialog -->
    <Dialog
      v-model:visible="showDeleteDialog"
      :header="isRTL ? 'تأكيد الحذف' : 'Confirm Delete'"
      :style="{ width: '400px' }"
      :modal="true"
      :dismissableMask="true"
      class="confirm-dialog"
    >
      <div class="dialog-content">
        <i class="pi pi-exclamation-triangle" style="font-size: 2rem; color: var(--red-500);"></i>
        <p>{{ isRTL ? 'هل أنت متأكد من حذف هذا العنصر؟' : 'Are you sure you want to delete this item?' }}</p>
        <p class="item-name" v-if="currentItem">{{ currentItem.name }}</p>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showDeleteDialog = false" />
        <Button :label="isRTL ? 'حذف' : 'Delete'" severity="danger" @click="confirmDelete" />
      </template>
    </Dialog>

    <!-- Share Dialog -->
    <Dialog
      v-model:visible="showShareDialog"
      :header="isRTL ? 'مشاركة' : 'Share'"
      :style="{ width: '500px' }"
      :modal="true"
      :dismissableMask="true"
      class="share-dialog"
    >
      <div class="dialog-content" v-if="currentItem">
        <p>{{ isRTL ? 'مشاركة' : 'Share' }}: <strong>{{ currentItem.name }}</strong></p>
        <div class="form-field">
          <label>{{ isRTL ? 'البريد الإلكتروني أو اسم المستخدم' : 'Email or username' }}</label>
          <InputText :placeholder="isRTL ? 'أدخل البريد الإلكتروني...' : 'Enter email...'" class="w-full" />
        </div>
        <div class="share-link">
          <label>{{ isRTL ? 'أو انسخ الرابط' : 'Or copy link' }}</label>
          <div class="link-field">
            <InputText :value="`https://kms.afc27.com/documents/${currentItem.id}`" readonly class="w-full" />
            <Button icon="pi pi-copy" severity="secondary" />
          </div>
        </div>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showShareDialog = false" />
        <Button :label="isRTL ? 'مشاركة' : 'Share'" @click="showShareDialog = false" />
      </template>
    </Dialog>

    <!-- Rename Dialog -->
    <Dialog
      v-model:visible="showRenameDialog"
      :header="isRTL ? 'إعادة تسمية' : 'Rename'"
      :style="{ width: '400px' }"
      :modal="true"
      :dismissableMask="true"
      class="rename-dialog"
    >
      <div class="dialog-content">
        <div class="form-field">
          <label>{{ isRTL ? 'الاسم الجديد' : 'New name' }}</label>
          <InputText v-model="renameValue" class="w-full" />
        </div>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showRenameDialog = false" />
        <Button :label="isRTL ? 'حفظ' : 'Save'" @click="confirmRename" />
      </template>
    </Dialog>

    <!-- Move Dialog -->
    <Dialog
      v-model:visible="showMoveDialog"
      :header="isRTL ? 'نقل إلى' : 'Move to'"
      :style="{ width: '500px' }"
      :modal="true"
      :dismissableMask="true"
      class="move-dialog"
    >
      <div class="dialog-content" v-if="currentItem">
        <p>{{ isRTL ? 'نقل' : 'Move' }}: <strong>{{ currentItem.name }}</strong></p>
        <div class="folder-list">
          <p class="placeholder-text">{{ isRTL ? 'قائمة المجلدات ستظهر هنا' : 'Folder list will appear here' }}</p>
        </div>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showMoveDialog = false" />
        <Button :label="isRTL ? 'نقل' : 'Move'" @click="showMoveDialog = false" />
      </template>
    </Dialog>
  </div>
</template>

<style lang="scss" scoped>
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

// ============================================
// DOCUMENT LIBRARY VIEW - MAIN CONTAINER
// Following Universal Spacing Guidelines from _mixins.scss
// ============================================

.document-library-view {
  @include page-view;

  &.rtl {
    direction: rtl;

    .separator {
      transform: rotate(180deg);
    }
  }
}

// Header action buttons (used in PageHeader slot)
.btn-create {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-3 $spacing-5;
  background: white;
  color: $intalio-teal-600;
  border: none;
  border-radius: $radius-lg;
  font-weight: $font-weight-semibold;
  font-size: $text-sm;
  cursor: pointer;
  transition: all $transition-fast;

  &:hover {
    background: rgba(255, 255, 255, 0.95);
    transform: translateY(-2px);
    box-shadow: $shadow-lg;
  }

  .btn-icon {
    width: 20px;
    height: 20px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: $intalio-teal-500;
    color: white;
    border-radius: $radius-md;
    font-size: $text-xs;
  }
}

.btn-secondary {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-3 $spacing-5;
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: #fff;
  border-radius: $radius-lg;
  font-weight: $font-weight-semibold;
  font-size: $text-sm;
  cursor: pointer;
  transition: all $transition-fast;

  &:hover {
    background: rgba(255, 255, 255, 0.25);
  }
}

// Upload Zone
.upload-zone {
  margin-bottom: $spacing-6;
  padding: $spacing-6;
  background: #fff;
  border: 2px dashed $slate-200;
  border-radius: $radius-xl;
  text-align: center;
  transition: all $transition-base;

  &.drag-over {
    border-color: $intalio-teal-400;
    background: rgba($intalio-teal-500, 0.05);

    .upload-icon {
      transform: scale(1.1);
      color: $intalio-teal-500;
    }
  }

  &.uploading {
    border-style: solid;
    border-color: $intalio-teal-400;
  }
}

.upload-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: $spacing-2;
}

.upload-icon {
  width: 64px;
  height: 64px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: $intalio-teal-50;
  color: $intalio-teal-500;
  border-radius: $radius-xl;
  font-size: 1.75rem;
  transition: all $transition-base;
}

.upload-text {
  font-size: $text-base;
  color: $slate-600;
  margin: 0;
}

.upload-link {
  background: none;
  border: none;
  color: $intalio-teal-500;
  font-weight: $font-weight-semibold;
  cursor: pointer;
  text-decoration: underline;

  &:hover {
    color: $intalio-teal-600;
  }
}

.upload-hint {
  font-size: $text-sm;
  color: $slate-400;
}

.upload-progress {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: $spacing-3;

  i {
    font-size: 2rem;
    color: $intalio-teal-500;
  }

  span {
    color: $slate-600;
    font-weight: $font-weight-medium;
  }

  :deep(.p-progressbar) {
    width: 300px;
    max-width: 100%;
    height: 8px;
    background: $slate-100;
    border-radius: $radius-full;

    .p-progressbar-value {
      background: $gradient-primary;
      border-radius: $radius-full;
    }
  }
}

// ============================================
// PREMIUM TOOLBAR - Using global mixins
// ============================================
.toolbar {
  @include toolbar-container;
}

.toolbar-row {
  @include toolbar-row;
}

.toolbar-left {
  @include toolbar-section-left;
}

.toolbar-right {
  @include toolbar-section-right;
}

.search-box {
  @include toolbar-search(280px);
}

.search-input {
  @include toolbar-search-input;
}

// Type Toggle
.type-toggle {
  @include type-toggle;
}

// Filter Button
.filter-btn {
  @include filter-btn;

  .filter-badge {
    @include filter-badge;
  }
}

// Per Page Control
.per-page-control {
  @include per-page-control;
}

.per-page-label {
  @include per-page-label;
}

.per-page-select {
  @include per-page-select;
}

// Filters Panel
.filters-panel {
  @include filters-panel;
}

.filter-group {
  @include filters-panel-group;
}

.filter-label {
  @include filters-panel-label;
}

.filter-options {
  @include filters-panel-options;
}

.filter-chip {
  @include filter-chip;
}

.filter-actions {
  display: flex;
  align-items: flex-end;
  margin-inline-start: auto;
}

.clear-filters-btn {
  @include clear-filters-btn;
}

// Active Filters Tags
.active-filters {
  @include active-filters-row;
}

.active-filters-label {
  @include active-filters-label;
}

.filter-tag {
  @include filter-tag;
}

.clear-all-link {
  @include clear-all-link;
}

.bulk-actions {
  @include bulk-actions;
}

.selection-count {
  @include selection-count;
}

.view-toggle {
  @include premium-view-toggle;
}

// Loading State
.loading-state {
  background: #fff;
  border-radius: $radius-xl;
  padding: $spacing-6;
  box-shadow: $shadow-card;
}

.table-skeleton {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;
}

.grid-skeleton {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
  gap: $spacing-4;
}

// Document Table
.document-table {
  background: #fff;
  border-radius: $radius-xl;
  box-shadow: $shadow-card;
  overflow: hidden;
}

.premium-table {
  :deep(.p-datatable-header) {
    background: $slate-50;
    border: none;
  }

  :deep(.p-datatable-thead > tr > th) {
    background: $slate-50;
    border: none;
    border-bottom: 1px solid $slate-100;
    padding: $spacing-4;
    font-size: $text-sm;
    font-weight: $font-weight-semibold;
    color: $slate-600;
    text-transform: uppercase;
    letter-spacing: 0.5px;
  }

  :deep(.p-datatable-tbody > tr) {
    cursor: pointer;
    transition: all $transition-fast;

    &:hover {
      background: $intalio-teal-50 !important;
    }

    > td {
      border: none;
      border-bottom: 1px solid $slate-100;
      padding: $spacing-4;
    }
  }

  :deep(.p-datatable-tbody > tr.p-row-odd) {
    background: $slate-50;
  }
}

.file-name-cell {
  display: flex;
  align-items: center;
  gap: $spacing-3;
}

.file-icon {
  width: 40px;
  height: 40px;
  border-radius: $radius-lg;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
  flex-shrink: 0;
}

.file-info {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.file-name {
  font-weight: $font-weight-medium;
  color: $slate-800;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.file-meta {
  font-size: $text-xs;
  color: $slate-500;
}

.file-badges {
  margin-inline-start: auto;
}

.lock-badge {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 24px;
  height: 24px;
  background: rgba($warning-500, 0.1);
  color: $warning-600;
  border-radius: $radius-full;
  font-size: 0.75rem;
}

.status-tag {
  font-size: $text-xs;
  font-weight: $font-weight-medium;
  padding: $spacing-1 $spacing-2;
  border-radius: $radius-md;
}

.version-badge {
  font-size: $text-sm;
  font-weight: $font-weight-medium;
  color: $slate-600;
  background: $slate-100;
  padding: $spacing-1 $spacing-2;
  border-radius: $radius-md;
}

.folder-dash {
  color: $slate-300;
}

.size-text {
  font-size: $text-sm;
  color: $slate-600;
}

.modified-by-cell {
  display: flex;
  align-items: center;
  gap: $spacing-2;
}

.user-avatar {
  width: 28px;
  height: 28px;
  border-radius: $radius-full;
  object-fit: cover;
}

.user-initial {
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: $intalio-teal-100;
  color: $intalio-teal-700;
  border-radius: $radius-full;
  font-size: $text-sm;
  font-weight: $font-weight-semibold;
}

.user-name {
  font-size: $text-sm;
  color: $slate-700;
}

.date-text {
  font-size: $text-sm;
  color: $slate-600;
}

.action-menu-btn {
  @include action-menu-btn;
}

// Document Grid
.document-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
  gap: $spacing-4;
}

.document-card {
  position: relative;
  background: #fff;
  border-radius: $radius-xl;
  padding: $spacing-5;
  box-shadow: $shadow-card;
  cursor: pointer;
  transition: all $transition-base;
  border: 2px solid transparent;

  &:hover {
    box-shadow: $shadow-card-hover;
    transform: translateY(-2px);

    .card-actions {
      opacity: 1;
    }
  }

  &.selected {
    border-color: $intalio-teal-400;
    background: $intalio-teal-50;
  }

  &.folder .card-icon {
    background: rgba(#f59e0b, 0.1);
  }
}

.card-select {
  position: absolute;
  top: $spacing-3;
  left: $spacing-3;
  z-index: 1;

  .rtl & {
    left: auto;
    right: $spacing-3;
  }
}

.card-icon {
  width: 64px;
  height: 64px;
  border-radius: $radius-xl;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.75rem;
  margin-bottom: $spacing-4;
  position: relative;
}

.lock-indicator {
  position: absolute;
  bottom: -4px;
  right: -4px;
  width: 20px;
  height: 20px;
  background: $warning-500;
  color: #fff;
  border-radius: $radius-full;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.625rem;
  border: 2px solid #fff;
}

.card-content {
  margin-bottom: $spacing-3;
}

.card-title {
  font-size: $text-base;
  font-weight: $font-weight-semibold;
  color: $slate-800;
  margin: 0 0 $spacing-2;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.card-meta {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  font-size: $text-sm;
  color: $slate-500;

  .item-count, .file-size {
    display: flex;
    align-items: center;
    gap: $spacing-1;
  }

  .separator {
    color: $slate-300;
  }
}

.card-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-top: $spacing-3;
  border-top: 1px solid $slate-100;

  .status-tag.small {
    font-size: 0.625rem;
    padding: 2px 6px;
  }

  .version-text {
    font-size: $text-xs;
    color: $slate-500;
    font-weight: $font-weight-medium;
  }
}

.card-actions {
  @include card-action-btn;
  position: absolute;
  top: $spacing-3;
  inset-inline-end: $spacing-3;
}

// Action Menu - Using standardized mixin
.action-menu {
  @include action-menu-popup;
}

// Empty State
.empty-state {
  text-align: center;
  padding: $spacing-12 $spacing-6;
  background: #fff;
  border-radius: $radius-xl;
  box-shadow: $shadow-card;
}

.empty-icon {
  width: 80px;
  height: 80px;
  margin: 0 auto $spacing-4;
  background: $slate-100;
  border-radius: $radius-xl;
  display: flex;
  align-items: center;
  justify-content: center;

  i {
    font-size: 2.5rem;
    color: $slate-400;
  }
}

.empty-state h3 {
  font-size: $text-xl;
  font-weight: $font-weight-semibold;
  color: $slate-800;
  margin: 0 0 $spacing-2;
}

.empty-state p {
  font-size: $text-base;
  color: $slate-500;
  margin: 0 0 $spacing-6;
}

.upload-btn {
  background: $gradient-primary;
  border: none;
  font-weight: $font-weight-semibold;
  padding: $spacing-3 $spacing-6;
  border-radius: $radius-lg;

  &:hover {
    background: $gradient-primary-hover;
  }
}

// ============================================
// RESPONSIVE - Match Content page exactly
// ============================================
@media (max-width: $breakpoint-lg) {
  .toolbar-row {
    flex-wrap: wrap;
  }

  .toolbar-left {
    order: 1;
    flex-basis: 100%;
  }

  .toolbar-right {
    order: 2;
    width: 100%;
    justify-content: space-between;
    margin-top: $spacing-2;
  }
}

@media (max-width: $breakpoint-md) {
  .btn-create,
  .btn-secondary {
    width: 100%;
    justify-content: center;
  }

  .toolbar-row {
    flex-direction: column;
    align-items: stretch;
    gap: $spacing-3;
  }

  .toolbar-left {
    flex-direction: column;
    align-items: stretch;
    gap: $spacing-3;
  }

  .toolbar-right {
    flex-direction: row;
    justify-content: space-between;
  }

  .search-box {
    width: 100%;
  }

  .type-toggle {
    width: 100%;
    justify-content: center;

    button {
      flex: 1;
      justify-content: center;
    }
  }

  .filter-btn {
    width: 100%;
    justify-content: center;
  }

  .per-page-control {
    flex: 1;
  }

  .filters-panel {
    flex-direction: column;
  }

  .filter-group {
    width: 100%;
  }

  .document-grid {
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  }

  .pagination-wrapper {
    flex-direction: column;
    gap: $spacing-3;
    padding: $spacing-4;
  }

  .pagination-info {
    order: 2;
    text-align: center;
  }

  .pagination-controls {
    order: 1;
    width: 100%;
    justify-content: center;
  }

  .page-btn {
    min-width: 36px;
    height: 36px;
  }
}

// Animations - Using global mixins
.document-card {
  @include card-item-animation;
}

@include staggered-animation-delays('.document-card', 20, 0.05s);

// ============================================
// PAGINATION - Using global mixins
// ============================================
.pagination-wrapper {
  @include pagination-wrapper;
}

.pagination-info {
  @include pagination-info;
}

.pagination-controls {
  @include pagination-controls;
}

.page-btn {
  @include pagination-btn;
}

.page-ellipsis {
  @include pagination-ellipsis;
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================

@media (prefers-reduced-motion: reduce) {
  .document-card,
  .stat-item {
    animation: none !important;
    opacity: 1;
    transform: none;
  }

  .upload-icon,
  .stat-item:hover .stat-icon {
    transform: none;
  }

  .document-card:hover {
    transform: none;
  }
}

</style>
