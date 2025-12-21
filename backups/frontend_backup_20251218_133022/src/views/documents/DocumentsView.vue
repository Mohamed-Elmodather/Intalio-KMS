<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useUIStore } from '@/stores/ui.store'
import { useReducedMotion } from '@/composables/useReducedMotion'
import { PageHeader } from '@/components/base'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Menu from 'primevue/menu'
import Dialog from 'primevue/dialog'
import FileUpload from 'primevue/fileupload'
import Tag from 'primevue/tag'
import type { Document, DocumentLibrary, Folder } from '@/types'

const { t, locale } = useI18n()
const uiStore = useUIStore()
const prefersReducedMotion = useReducedMotion()

const isRTL = computed(() => uiStore.locale === 'ar')
const shouldAnimate = computed(() => !prefersReducedMotion.value)

// Breadcrumbs for PageHeader
const pageBreadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: t('documents.title') }
])

const loading = ref(true)
const search = ref('')
const showUploadDialog = ref(false)
const selectedDocuments = ref<Document[]>([])

// Current navigation state
const currentLibrary = ref<DocumentLibrary | null>(null)
const currentFolder = ref<Folder | null>(null)
const breadcrumbs = ref<any[]>([])
const folders = ref<Folder[]>([])
const documents = ref<Document[]>([])

// Libraries list
const libraries = ref<DocumentLibrary[]>([])

const contextMenu = ref()
const contextMenuItems = computed(() => [
  { label: t('documents.download'), icon: 'pi pi-download', command: () => downloadSelected() },
  { label: t('documents.move'), icon: 'pi pi-folder', command: () => moveSelected() },
  { label: t('documents.copy'), icon: 'pi pi-copy', command: () => copySelected() },
  { separator: true },
  { label: t('common.delete'), icon: 'pi pi-trash', class: 'text-red-500', command: () => deleteSelected() }
])

const getFileIcon = (extension: string) => {
  const iconMap: Record<string, string> = {
    '.pdf': 'pi pi-file-pdf text-red-500',
    '.doc': 'pi pi-file-word text-blue-500',
    '.docx': 'pi pi-file-word text-blue-500',
    '.xls': 'pi pi-file-excel text-green-500',
    '.xlsx': 'pi pi-file-excel text-green-500',
    '.ppt': 'pi pi-file text-orange-500',
    '.pptx': 'pi pi-file text-orange-500',
    '.jpg': 'pi pi-image text-purple-500',
    '.jpeg': 'pi pi-image text-purple-500',
    '.png': 'pi pi-image text-purple-500',
    '.mp4': 'pi pi-video text-pink-500',
    '.zip': 'pi pi-file-archive text-yellow-500'
  }
  return iconMap[extension.toLowerCase()] || 'pi pi-file text-gray-500'
}

const formatFileSize = (bytes: number) => {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i]
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString(locale.value === 'ar' ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const getStatusSeverity = (status: string) => {
  switch (status.toLowerCase()) {
    case 'published': return 'success'
    case 'checkedout': return 'warn'
    case 'draft': return 'secondary'
    default: return 'info'
  }
}

const navigateToLibrary = (library: DocumentLibrary) => {
  currentLibrary.value = library
  currentFolder.value = null
  updateBreadcrumbs()
  loadContents()
}

const navigateToFolder = (folder: Folder) => {
  currentFolder.value = folder
  updateBreadcrumbs()
  loadContents()
}

const updateBreadcrumbs = () => {
  const items: any[] = [{ label: t('documents.libraries'), command: () => { currentLibrary.value = null; currentFolder.value = null; loadContents() } }]

  if (currentLibrary.value) {
    items.push({
      label: locale.value === 'ar' ? currentLibrary.value.nameArabic : currentLibrary.value.name,
      command: () => { currentFolder.value = null; loadContents() }
    })
  }

  if (currentFolder.value) {
    items.push({
      label: locale.value === 'ar' ? currentFolder.value.nameArabic : currentFolder.value.name
    })
  }

  breadcrumbs.value = items
}

const downloadSelected = () => {
  console.log('Download:', selectedDocuments.value)
}

const moveSelected = () => {
  console.log('Move:', selectedDocuments.value)
}

const copySelected = () => {
  console.log('Copy:', selectedDocuments.value)
}

const deleteSelected = () => {
  console.log('Delete:', selectedDocuments.value)
}

const loadContents = async () => {
  loading.value = true
  await new Promise(resolve => setTimeout(resolve, 500))

  if (!currentLibrary.value) {
    // Load libraries
    folders.value = []
    documents.value = []
  } else {
    // Load folders and documents
    folders.value = [
      { id: '1', name: 'Manuals', nameArabic: 'الأدلة', documentCount: 25, childFolderCount: 3 },
      { id: '2', name: 'Policies', nameArabic: 'السياسات', documentCount: 18, childFolderCount: 0 },
      { id: '3', name: 'Templates', nameArabic: 'القوالب', documentCount: 12, childFolderCount: 2 }
    ] as Folder[]

    documents.value = [
      {
        id: '1',
        name: 'Tournament Operations Manual',
        nameArabic: 'دليل عمليات البطولة',
        fileName: 'tournament-operations-manual-v2.pdf',
        fileExtension: '.pdf',
        fileSize: 5242880,
        version: '2.1',
        status: 'published',
        isCheckedOut: false,
        createdByName: 'Mohammed Al-Rashid',
        createdAt: '2024-10-01T10:00:00Z',
        updatedAt: '2024-12-01T14:30:00Z'
      },
      {
        id: '2',
        name: 'Venue Specifications',
        nameArabic: 'مواصفات الملاعب',
        fileName: 'venue-specifications.xlsx',
        fileExtension: '.xlsx',
        fileSize: 2097152,
        version: '1.3',
        status: 'checkedout',
        isCheckedOut: true,
        checkedOutByName: 'Sara Ali',
        createdByName: 'Ahmed Hassan',
        createdAt: '2024-09-15T09:00:00Z',
        updatedAt: '2024-12-05T11:00:00Z'
      }
    ] as Document[]
  }

  loading.value = false
}

onMounted(async () => {
  libraries.value = [
    {
      id: '1',
      name: 'Operations Documents',
      nameArabic: 'وثائق العمليات',
      iconName: 'pi pi-folder',
      color: '#2E7D32',
      type: 'department',
      documentCount: 156,
      totalSize: 524288000
    },
    {
      id: '2',
      name: 'Media Assets',
      nameArabic: 'الملفات الإعلامية',
      iconName: 'pi pi-images',
      color: '#D4AF37',
      type: 'general',
      documentCount: 2500,
      totalSize: 10737418240
    },
    {
      id: '3',
      name: 'Brand Guidelines',
      nameArabic: 'إرشادات العلامة التجارية',
      iconName: 'pi pi-palette',
      color: '#1976D2',
      type: 'general',
      documentCount: 45,
      totalSize: 157286400
    }
  ] as DocumentLibrary[]

  updateBreadcrumbs()
  loading.value = false
})
</script>

<template>
  <div class="documents-view" :class="{ rtl: isRTL }">
    <!-- Header -->
    <PageHeader
      :title="t('documents.title')"
      :description="isRTL ? 'إدارة وتنظيم مستندات المشروع' : 'Manage and organize project documents'"
      :breadcrumbs="pageBreadcrumbs"
      padding-bottom="xl"
      background-variant="gradient"
      :animated="shouldAnimate"
    >
      <template #actions>
        <Button
          :label="t('documents.newFolder')"
          icon="pi pi-folder-plus"
          class="header-btn-secondary"
        />
        <Button
          :label="t('documents.upload')"
          icon="pi pi-upload"
          class="header-btn-primary"
          @click="showUploadDialog = true"
        />
      </template>
    </PageHeader>

    <!-- Search Bar -->
    <div class="search-bar">
      <div class="search-box">
        <i class="pi pi-search"></i>
        <InputText
          v-model="search"
          :placeholder="t('documents.searchDocuments')"
          class="search-input"
        />
      </div>
    </div>

    <!-- Libraries Grid (when no library selected) -->
    <div v-if="!currentLibrary" class="libraries-grid">
      <div
        v-for="library in libraries"
        :key="library.id"
        class="library-card"
        @click="navigateToLibrary(library)"
      >
        <div class="library-icon" :style="{ backgroundColor: library.color + '20' }">
          <i :class="library.iconName" :style="{ color: library.color }"></i>
        </div>
        <div class="library-info">
          <h3>{{ locale === 'ar' && library.nameArabic ? library.nameArabic : library.name }}</h3>
          <span class="doc-count">{{ library.documentCount }} {{ t('documents.documents') }}</span>
          <span class="size">{{ formatFileSize(library.totalSize || 0) }}</span>
        </div>
      </div>
    </div>

    <!-- Folders & Documents Table -->
    <div v-else class="contents-section">
      <!-- Folders -->
      <div v-if="folders.length > 0" class="folders-grid">
        <div
          v-for="folder in folders"
          :key="folder.id"
          class="folder-card"
          @click="navigateToFolder(folder)"
        >
          <i class="pi pi-folder folder-icon"></i>
          <div class="folder-info">
            <span class="folder-name">{{ locale === 'ar' && folder.nameArabic ? folder.nameArabic : folder.name }}</span>
            <span class="folder-meta">{{ folder.documentCount }} {{ t('documents.items') }}</span>
          </div>
        </div>
      </div>

      <!-- Documents Table -->
      <DataTable
        v-if="documents.length > 0"
        v-model:selection="selectedDocuments"
        :value="documents"
        :loading="loading"
        paginator
        :rows="10"
        dataKey="id"
        :rowHover="true"
        class="documents-table"
        @contextmenu="contextMenu.toggle($event)"
      >
        <Column selectionMode="multiple" headerStyle="width: 3rem" />

        <Column field="name" :header="t('documents.name')" sortable>
          <template #body="{ data }">
            <div class="doc-name-cell">
              <i :class="getFileIcon(data.fileExtension)" class="file-icon"></i>
              <div class="doc-details">
                <span class="doc-name">{{ locale === 'ar' && data.nameArabic ? data.nameArabic : data.name }}</span>
                <span class="doc-filename">{{ data.fileName }}</span>
              </div>
            </div>
          </template>
        </Column>

        <Column field="updatedAt" :header="t('documents.modified')" sortable>
          <template #body="{ data }">
            {{ formatDate(data.updatedAt || data.createdAt) }}
          </template>
        </Column>

        <Column field="fileSize" :header="t('documents.size')" sortable>
          <template #body="{ data }">
            {{ formatFileSize(data.fileSize) }}
          </template>
        </Column>

        <Column field="version" :header="t('documents.version')" sortable>
          <template #body="{ data }">
            <span class="version-badge">v{{ data.version }}</span>
          </template>
        </Column>

        <Column field="status" :header="t('documents.status')">
          <template #body="{ data }">
            <Tag
              :value="data.isCheckedOut ? `${t('documents.checkedOut')} - ${data.checkedOutByName}` : t(`documents.${data.status}`)"
              :severity="getStatusSeverity(data.status)"
            />
          </template>
        </Column>

        <Column headerStyle="width: 8rem">
          <template #body>
            <div class="action-buttons">
              <Button icon="pi pi-download" severity="secondary" text rounded />
              <Button icon="pi pi-ellipsis-v" severity="secondary" text rounded />
            </div>
          </template>
        </Column>
      </DataTable>

      <Menu ref="contextMenu" :model="contextMenuItems" :popup="true" />
    </div>

    <!-- Upload Dialog -->
    <Dialog
      v-model:visible="showUploadDialog"
      :header="t('documents.uploadDocuments')"
      :style="{ width: '500px' }"
      modal
    >
      <FileUpload
        mode="advanced"
        :multiple="true"
        accept="*/*"
        :maxFileSize="104857600"
        :chooseLabel="t('documents.chooseFiles')"
        :uploadLabel="t('documents.upload')"
        :cancelLabel="t('common.cancel')"
      >
        <template #empty>
          <div class="upload-placeholder">
            <i class="pi pi-cloud-upload"></i>
            <p>{{ t('documents.dragDropFiles') }}</p>
          </div>
        </template>
      </FileUpload>
    </Dialog>
  </div>
</template>

<style scoped lang="scss">
// Design System
@use '@/design-system/tokens' as *;
@use '@/design-system/mixins' as *;
@use '@/assets/styles/_mixins.scss' as legacy;

.documents-view {
  padding: 0;

  &.rtl {
    direction: rtl;
  }
}

// Header action buttons (used in PageHeader slot)
.header-btn-primary {
  @include legacy.header-btn-primary;
}

.header-btn-secondary {
  @include legacy.header-btn-secondary;
}

.search-bar {
  margin-bottom: 1.5rem;

  .search-box {
    position: relative;
    max-width: 400px;

    i {
      position: absolute;
      inset-inline-start: 1rem;
      top: 50%;
      transform: translateY(-50%);
      color: var(--text-color-secondary);
    }

    .search-input {
      width: 100%;
      padding-inline-start: 2.5rem;
    }
  }
}

.libraries-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1rem;
}

.library-card {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1.25rem;
  background: var(--surface-card);
  border: 1px solid var(--surface-border);
  border-radius: var(--border-radius);
  cursor: pointer;
  transition: all 0.2s;

  &:hover {
    border-color: var(--primary-color);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  }

  .library-icon {
    width: 48px;
    height: 48px;
    border-radius: 12px;
    display: flex;
    align-items: center;
    justify-content: center;

    i {
      font-size: 1.5rem;
    }
  }

  .library-info {
    flex: 1;

    h3 {
      margin: 0 0 0.25rem 0;
      font-size: 1rem;
      font-weight: 600;
    }

    .doc-count, .size {
      font-size: 0.875rem;
      color: var(--text-color-secondary);
    }

    .doc-count::after {
      content: ' • ';
    }
  }
}

.folders-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.folder-card {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  background: var(--surface-card);
  border: 1px solid var(--surface-border);
  border-radius: var(--border-radius);
  cursor: pointer;
  transition: all 0.2s;

  &:hover {
    border-color: var(--primary-color);
    background: var(--surface-hover);
  }

  .folder-icon {
    font-size: 1.5rem;
    color: #ffc107;
  }

  .folder-info {
    display: flex;
    flex-direction: column;

    .folder-name {
      font-weight: 500;
    }

    .folder-meta {
      font-size: 0.75rem;
      color: var(--text-color-secondary);
    }
  }
}

.documents-table {
  .doc-name-cell {
    display: flex;
    align-items: center;
    gap: 0.75rem;

    .file-icon {
      font-size: 1.5rem;
    }

    .doc-details {
      display: flex;
      flex-direction: column;

      .doc-name {
        font-weight: 500;
      }

      .doc-filename {
        font-size: 0.75rem;
        color: var(--text-color-secondary);
      }
    }
  }

  .version-badge {
    padding: 0.25rem 0.5rem;
    background: var(--surface-ground);
    border-radius: 4px;
    font-size: 0.75rem;
    font-family: monospace;
  }

  .action-buttons {
    display: flex;
    gap: 0.25rem;
  }
}

.upload-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 2rem;
  color: var(--text-color-secondary);

  i {
    font-size: 3rem;
    margin-bottom: 1rem;
  }
}

</style>
