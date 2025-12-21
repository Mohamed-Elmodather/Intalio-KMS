<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import ErrorState from '@/components/base/ErrorState.vue'

const route = useRoute()
const router = useRouter()
const { locale } = useI18n()
const prefersReducedMotion = useReducedMotion()

// RTL support
const isRTL = computed(() => locale.value === 'ar')
const isLoading = ref(true)
const error = ref<Error | null>(null)
const isContentVisible = ref(false)
const shouldAnimate = computed(() => !prefersReducedMotion.value)

const menuRef = ref()
const mediaMenuRef = ref()
const showUploadDialog = ref(false)
const showMediaViewer = ref(false)
const showDeleteDialog = ref(false)
const currentMedia = ref<any>(null)
const viewMode = ref('grid')
const selectedItems = ref<string[]>([])

const viewModes = [
  { value: 'grid', icon: 'pi pi-th-large' },
  { value: 'list', icon: 'pi pi-list' }
]

const filters = ref({
  search: '',
  type: null as string | null
})

const breadcrumbItems = computed(() => [
  { label: isRTL.value ? 'معرض الوسائط' : 'Media Gallery', route: { name: 'media-gallery' } },
  { label: isRTL.value ? 'تفاصيل المعرض' : 'Gallery Detail' }
])

const menuItems = [
  { label: isRTL.value ? 'تعديل المعرض' : 'Edit Gallery', icon: 'pi pi-pencil', command: () => editGallery() },
  { label: isRTL.value ? 'تعيين صورة الغلاف' : 'Set Cover Image', icon: 'pi pi-image', command: () => setCoverImage() },
  { separator: true },
  { label: isRTL.value ? 'تنزيل الكل' : 'Download All', icon: 'pi pi-download', command: () => downloadAll() },
  { separator: true },
  { label: isRTL.value ? 'أرشفة المعرض' : 'Archive Gallery', icon: 'pi pi-folder', command: () => archiveGallery() },
  { label: isRTL.value ? 'حذف المعرض' : 'Delete Gallery', icon: 'pi pi-trash', class: 'text-red-500', command: () => deleteGallery() }
]

const mediaMenuItems = computed(() => [
  { label: isRTL.value ? 'عرض' : 'View', icon: 'pi pi-eye', command: () => openMediaViewer(currentMedia.value) },
  { label: isRTL.value ? 'تنزيل' : 'Download', icon: 'pi pi-download', command: () => downloadItem(currentMedia.value) },
  { label: isRTL.value ? 'تعيين كغلاف' : 'Set as Cover', icon: 'pi pi-image', command: () => setAsCover(currentMedia.value) },
  { separator: true },
  { label: isRTL.value ? 'تعديل' : 'Edit', icon: 'pi pi-pencil', command: () => editMediaItem(currentMedia.value) },
  { label: isRTL.value ? 'حذف' : 'Delete', icon: 'pi pi-trash', class: 'text-red-500', command: () => confirmDeleteItem(currentMedia.value) }
])

const mediaTypes = [
  { label: 'All', value: null },
  { label: 'Images', value: 'Image' },
  { label: 'Videos', value: 'Video' },
  { label: 'Audio', value: 'Audio' }
]

const gallery = ref({
  id: route.params.id,
  name: 'Opening Ceremony Photos',
  nameArabic: 'صور حفل الافتتاح',
  description: 'Official photos from the AFC Asian Cup 2027 opening ceremony rehearsal and preparation events.',
  type: 'Photos',
  visibility: 'Organization',
  coverImageUrl: '/media/galleries/opening/cover.jpg',
  itemCount: 156,
  totalSizeFormatted: '500 MB',
  ownerName: 'Mohammed Al-Rashid'
})

const mediaItems = ref([
  {
    id: '1',
    fileName: 'ceremony_stage_01.jpg',
    title: 'Main Stage Setup',
    type: 'Image',
    mimeType: 'image/jpeg',
    fileSizeFormatted: '4 MB',
    thumbnailUrl: '/media/thumbnails/stage_01_thumb.jpg',
    url: '/media/images/stage_01.jpg',
    width: 4000,
    height: 3000,
    isFeatured: true,
    tags: ['Stage', 'Setup', 'Ceremony'],
    createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '2',
    fileName: 'opening_video.mp4',
    title: 'Opening Sequence',
    type: 'Video',
    mimeType: 'video/mp4',
    fileSizeFormatted: '200 MB',
    thumbnailUrl: '/media/thumbnails/video_thumb.jpg',
    url: '/media/videos/opening_video.mp4',
    width: 1920,
    height: 1080,
    durationFormatted: '3:00',
    tags: ['Video', 'Opening', 'Official'],
    createdAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '3',
    fileName: 'crowd_aerial.jpg',
    title: 'Aerial Crowd View',
    type: 'Image',
    mimeType: 'image/jpeg',
    fileSizeFormatted: '6 MB',
    thumbnailUrl: '/media/thumbnails/aerial_thumb.jpg',
    url: '/media/images/aerial.jpg',
    width: 5000,
    height: 3333,
    tags: ['Aerial', 'Crowd'],
    createdAt: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '4',
    fileName: 'speech_audio.mp3',
    title: 'Opening Speech',
    type: 'Audio',
    mimeType: 'audio/mpeg',
    fileSizeFormatted: '15 MB',
    url: '/media/audio/speech.mp3',
    durationFormatted: '12:30',
    tags: ['Speech', 'Audio'],
    createdAt: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000).toISOString()
  }
])

function formatDate(dateString: string) {
  const date = new Date(dateString)
  return new Intl.DateTimeFormat(locale.value, {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  }).format(date)
}

function getTypeSeverity(type: string) {
  const severities: Record<string, string> = {
    General: 'secondary',
    Photos: 'info',
    Videos: 'warning',
    EventMedia: 'success',
    ProjectMedia: 'contrast',
    TeamMedia: 'danger'
  }
  return severities[type] || 'secondary'
}

function getMediaTypeSeverity(type: string) {
  const severities: Record<string, string> = {
    Image: 'info',
    Video: 'warning',
    Audio: 'success',
    Document: 'secondary'
  }
  return severities[type] || 'secondary'
}

function getMediaIcon(type: string) {
  const icons: Record<string, string> = {
    Image: 'pi pi-image',
    Video: 'pi pi-video',
    Audio: 'pi pi-volume-up',
    Document: 'pi pi-file'
  }
  return icons[type] || 'pi pi-file'
}

function toggleMenu(event: Event) {
  menuRef.value.toggle(event)
}

function toggleSelection(id: string, event: MouseEvent) {
  if (event.ctrlKey || event.metaKey) {
    const index = selectedItems.value.indexOf(id)
    if (index > -1) {
      selectedItems.value.splice(index, 1)
    } else {
      selectedItems.value.push(id)
    }
  }
}

function clearSelection() {
  selectedItems.value = []
}

function openMediaViewer(item: any) {
  currentMedia.value = item
  showMediaViewer.value = true
}

function downloadItem(item: any) {
  if (item) {
    // In a real app, this would trigger a download
    const link = document.createElement('a')
    link.href = item.url || item.thumbnailUrl
    link.download = item.fileName
    link.click()
  }
}

function deleteItem(item: any) {
  if (item) {
    mediaItems.value = mediaItems.value.filter((m: any) => m.id !== item.id)
    gallery.value.itemCount = mediaItems.value.length
  }
}

function confirmDeleteItem(item: any) {
  if (item) {
    currentMedia.value = item
    showDeleteDialog.value = true
  }
}

function confirmDelete() {
  if (currentMedia.value) {
    deleteItem(currentMedia.value)
    showDeleteDialog.value = false
    currentMedia.value = null
  }
}

function toggleMediaMenu(event: Event, item: any) {
  currentMedia.value = item
  mediaMenuRef.value.toggle(event)
}

function setAsCover(item: any) {
  if (item && (item.type === 'Image' || item.thumbnailUrl)) {
    gallery.value.coverImageUrl = item.thumbnailUrl || item.url
  }
}

function editMediaItem(item: any) {
  if (item) {
    currentMedia.value = item
    // Open edit dialog - placeholder for now
    console.log('Editing:', item.fileName)
  }
}

function editGallery() {
  // Open gallery edit dialog
  console.log('Edit gallery')
}

function setCoverImage() {
  // Open cover image selection
  console.log('Set cover image')
}

function downloadAll() {
  // Download all media items
  mediaItems.value.forEach((item: any) => {
    downloadItem(item)
  })
}

function archiveGallery() {
  // Archive the gallery
  console.log('Archive gallery')
}

function deleteGallery() {
  // Delete the gallery and navigate back
  router.push({ name: 'media-gallery' })
}

function goToVideoEditor(item: any) {
  router.push({ name: 'video-editor', query: { mediaId: item.id } })
}

// Data loading with error handling
async function loadGalleryData() {
  try {
    error.value = null
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 600))
    // In real implementation: fetch gallery and media items from API
    isLoading.value = false

    // Trigger entrance animations
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

function handleRetry() {
  isLoading.value = true
  isContentVisible.value = false
  loadGalleryData()
}

onMounted(() => {
  loadGalleryData()
})
</script>

<template>
  <div class="gallery-detail-view" :class="{ 'rtl': isRTL }">
    <!-- Error State -->
    <ErrorState
      v-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل تفاصيل المعرض' : 'Failed to load gallery details'"
      show-retry
      size="lg"
      @retry="handleRetry"
    />

    <template v-else>
    <!-- Breadcrumb -->
    <Breadcrumb :model="breadcrumbItems" class="breadcrumb">
      <template #item="{ item }">
        <router-link v-if="item.route" :to="item.route" class="breadcrumb-link">
          {{ item.label }}
        </router-link>
        <span v-else>{{ item.label }}</span>
      </template>
    </Breadcrumb>

    <!-- Gallery Header -->
    <div class="gallery-header">
      <div class="header-cover" :style="{ backgroundImage: gallery.coverImageUrl ? `url(${gallery.coverImageUrl})` : 'none' }">
        <div class="header-overlay">
          <div class="header-content">
            <div class="header-info">
              <div class="header-badges">
                <Tag :value="gallery.type" :severity="getTypeSeverity(gallery.type)" />
                <Tag :value="gallery.visibility" severity="secondary" />
              </div>
              <h1>{{ gallery.name }}</h1>
              <p v-if="gallery.nameArabic" class="name-arabic">{{ gallery.nameArabic }}</p>
              <p class="description" v-if="gallery.description">{{ gallery.description }}</p>
              <div class="gallery-stats">
                <span><i class="pi pi-images"></i> {{ gallery.itemCount }} {{ $t('media.items') }}</span>
                <span><i class="pi pi-database"></i> {{ gallery.totalSizeFormatted }}</span>
                <span><i class="pi pi-user"></i> {{ gallery.ownerName }}</span>
              </div>
            </div>
            <div class="header-actions">
              <Button
                :label="$t('media.upload')"
                icon="pi pi-upload"
                @click="showUploadDialog = true"
              />
              <Button
                icon="pi pi-ellipsis-v"
                severity="secondary"
                @click="toggleMenu"
              />
              <Menu ref="menuRef" :model="menuItems" :popup="true" />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Toolbar -->
    <div class="toolbar">
      <div class="filters">
        <IconField class="search-field">
          <InputIcon class="pi pi-search" />
          <InputText
            v-model="filters.search"
            :placeholder="$t('media.searchMedia')"
          />
        </IconField>
        <Select
          v-model="filters.type"
          :options="mediaTypes"
          optionLabel="label"
          optionValue="value"
          :placeholder="$t('media.mediaType')"
          showClear
        />
      </div>
      <div class="toolbar-actions">
        <SelectButton
          v-model="viewMode"
          :options="viewModes"
          optionLabel="icon"
          optionValue="value"
          dataKey="value"
        >
          <template #option="{ option }">
            <i :class="option.icon"></i>
          </template>
        </SelectButton>
        <Button
          v-if="selectedItems.length > 0"
          :label="`${selectedItems.length} selected`"
          severity="secondary"
          icon="pi pi-times"
          @click="clearSelection"
        />
      </div>
    </div>

    <!-- Media Grid -->
    <div v-if="viewMode === 'grid'" class="media-grid">
      <div
        v-for="item in mediaItems"
        :key="item.id"
        class="media-card"
        :class="{ selected: selectedItems.includes(item.id) }"
        @click="toggleSelection(item.id, $event)"
        @dblclick="openMediaViewer(item)"
      >
        <div class="media-thumb">
          <img
            v-if="item.type === 'Image' || item.type === 'Video'"
            :src="item.thumbnailUrl"
            :alt="item.title || item.fileName"
          />
          <div v-else class="thumb-placeholder" :class="item.type.toLowerCase()">
            <i :class="getMediaIcon(item.type)"></i>
          </div>
          <div class="media-overlay">
            <Checkbox
              v-model="selectedItems"
              :value="item.id"
              @click.stop
              class="selection-checkbox"
            />
            <Button
              icon="pi pi-ellipsis-v"
              text
              rounded
              size="small"
              class="card-action-btn"
              @click.stop="(e: Event) => toggleMediaMenu(e, item)"
            />
            <div v-if="item.type === 'Video'" class="duration-badge">
              <i class="pi pi-play"></i>
              {{ item.durationFormatted }}
            </div>
            <Tag
              v-if="item.isFeatured"
              icon="pi pi-star-fill"
              value="Featured"
              severity="warning"
              class="featured-badge"
            />
          </div>
        </div>
        <div class="media-info">
          <span class="file-name" :title="item.fileName">{{ item.title || item.fileName }}</span>
          <div class="media-meta">
            <span>{{ item.fileSizeFormatted }}</span>
            <span v-if="item.width && item.height">{{ item.width }}x{{ item.height }}</span>
          </div>
        </div>
      </div>

      <!-- Media Action Menu -->
      <Menu ref="mediaMenuRef" :model="mediaMenuItems" :popup="true" class="action-menu" />
    </div>

    <!-- Media List -->
    <div v-else class="media-list">
      <DataTable
        v-model:selection="selectedItems"
        :value="mediaItems"
        dataKey="id"
        stripedRows
        @row-dblclick="(e) => openMediaViewer(e.data)"
      >
        <Column selectionMode="multiple" headerStyle="width: 3rem" />
        <Column field="fileName" :header="$t('common.name')">
          <template #body="{ data }">
            <div class="media-name-cell">
              <div class="media-thumb-small">
                <img v-if="data.thumbnailUrl" :src="data.thumbnailUrl" :alt="data.fileName" />
                <i v-else :class="getMediaIcon(data.type)"></i>
              </div>
              <div class="media-name-info">
                <span class="name">{{ data.title || data.fileName }}</span>
                <span class="original-name">{{ data.fileName }}</span>
              </div>
            </div>
          </template>
        </Column>
        <Column field="type" :header="$t('common.type')">
          <template #body="{ data }">
            <Tag :value="data.type" :severity="getMediaTypeSeverity(data.type)" />
          </template>
        </Column>
        <Column field="fileSizeFormatted" :header="$t('media.size')" />
        <Column :header="$t('media.dimensions')">
          <template #body="{ data }">
            <span v-if="data.width && data.height">{{ data.width }}x{{ data.height }}</span>
            <span v-else-if="data.durationFormatted">{{ data.durationFormatted }}</span>
            <span v-else>-</span>
          </template>
        </Column>
        <Column field="createdAt" :header="$t('common.uploaded')">
          <template #body="{ data }">
            {{ formatDate(data.createdAt) }}
          </template>
        </Column>
        <Column :header="$t('common.actions')" style="width: 100px">
          <template #body="{ data }">
            <div class="action-buttons">
              <Button
                icon="pi pi-download"
                severity="secondary"
                text
                rounded
                @click.stop="downloadItem(data)"
              />
              <Button
                icon="pi pi-trash"
                severity="danger"
                text
                rounded
                @click.stop="deleteItem(data)"
              />
            </div>
          </template>
        </Column>
      </DataTable>
    </div>

    <!-- Empty State -->
    <div v-if="mediaItems.length === 0" class="empty-state">
      <i class="pi pi-images"></i>
      <h3>{{ $t('media.noMediaItems') }}</h3>
      <p>{{ $t('media.uploadFirstMedia') }}</p>
      <Button
        :label="$t('media.upload')"
        icon="pi pi-upload"
        @click="showUploadDialog = true"
      />
    </div>

    <!-- Media Viewer Dialog -->
    <Dialog
      v-model:visible="showMediaViewer"
      :header="currentMedia?.title || currentMedia?.fileName"
      :modal="true"
      :maximizable="true"
      :style="{ width: '90vw', maxWidth: '1200px' }"
      class="media-viewer-dialog"
    >
      <div class="media-viewer" v-if="currentMedia">
        <div class="viewer-main">
          <img
            v-if="currentMedia.type === 'Image'"
            :src="currentMedia.url || currentMedia.thumbnailUrl"
            :alt="currentMedia.title"
          />
          <video
            v-else-if="currentMedia.type === 'Video'"
            :src="currentMedia.url"
            controls
            class="video-player"
          />
          <audio
            v-else-if="currentMedia.type === 'Audio'"
            :src="currentMedia.url"
            controls
            class="audio-player"
          />
        </div>
        <div class="viewer-sidebar">
          <div class="info-section">
            <h4>{{ $t('common.details') }}</h4>
            <div class="info-grid">
              <div class="info-item">
                <span class="label">{{ $t('common.type') }}</span>
                <span class="value">{{ currentMedia.type }}</span>
              </div>
              <div class="info-item">
                <span class="label">{{ $t('media.size') }}</span>
                <span class="value">{{ currentMedia.fileSizeFormatted }}</span>
              </div>
              <div class="info-item" v-if="currentMedia.width">
                <span class="label">{{ $t('media.dimensions') }}</span>
                <span class="value">{{ currentMedia.width }}x{{ currentMedia.height }}</span>
              </div>
              <div class="info-item" v-if="currentMedia.durationFormatted">
                <span class="label">{{ $t('media.duration') }}</span>
                <span class="value">{{ currentMedia.durationFormatted }}</span>
              </div>
              <div class="info-item">
                <span class="label">{{ $t('common.uploaded') }}</span>
                <span class="value">{{ formatDate(currentMedia.createdAt) }}</span>
              </div>
            </div>
          </div>
          <div class="info-section" v-if="currentMedia.tags?.length">
            <h4>{{ $t('content.tags') }}</h4>
            <div class="tags-list">
              <Tag
                v-for="tag in currentMedia.tags"
                :key="tag"
                :value="tag"
                severity="secondary"
              />
            </div>
          </div>
          <div class="viewer-actions">
            <Button
              :label="$t('media.download')"
              icon="pi pi-download"
              class="w-full"
              @click="downloadItem(currentMedia)"
            />
            <Button
              v-if="currentMedia.type === 'Video'"
              :label="$t('media.editVideo')"
              icon="pi pi-video"
              severity="secondary"
              class="w-full"
              @click="goToVideoEditor(currentMedia)"
            />
          </div>
        </div>
      </div>
    </Dialog>

    <!-- Upload Dialog -->
    <Dialog
      v-model:visible="showUploadDialog"
      :header="$t('media.uploadMedia')"
      :modal="true"
      :style="{ width: '600px' }"
    >
      <FileUpload
        mode="advanced"
        :multiple="true"
        accept="image/*,video/*,audio/*"
        :maxFileSize="104857600"
        :auto="false"
        class="upload-zone"
      >
        <template #empty>
          <div class="upload-empty">
            <i class="pi pi-cloud-upload"></i>
            <p>{{ $t('media.dragDropFiles') }}</p>
            <span>{{ $t('media.maxFileSize') }}: 100MB</span>
          </div>
        </template>
      </FileUpload>
    </Dialog>

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
        <p class="file-name" v-if="currentMedia">{{ currentMedia.fileName }}</p>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" severity="secondary" @click="showDeleteDialog = false" />
        <Button :label="isRTL ? 'حذف' : 'Delete'" severity="danger" @click="confirmDelete" />
      </template>
    </Dialog>
    </template>
  </div>
</template>


<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

.gallery-detail-view {
  min-height: 100vh;
  background-color: $slate-50;

  &.rtl {
    direction: rtl;
  }
}

.breadcrumb {
  margin-bottom: $spacing-4;
  padding: 0;
  background: transparent;
}

.breadcrumb-link {
  color: $intalio-teal-500;
  text-decoration: none;

  &:hover {
    text-decoration: underline;
  }
}

.gallery-header {
  margin: (-$spacing-6) (-$spacing-6) $spacing-6 (-$spacing-6);
}

.header-cover {
  height: 200px;
  background-size: cover;
  background-position: center;
  background-color: $slate-100;
}

.header-overlay {
  height: 100%;
  background: linear-gradient(135deg, rgba(0, 0, 0, 0.7) 0%, rgba(0, 0, 0, 0.4) 100%);
  padding: $spacing-8;
  display: flex;
  align-items: flex-end;
}

.header-content {
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  color: white;
  gap: $spacing-4;
  flex-wrap: wrap;
}

.header-badges {
  display: flex;
  gap: $spacing-2;
  margin-bottom: $spacing-2;
}

.header-info h1 {
  margin: 0 0 $spacing-1 0;
  font-size: $font-size-2xl;
  font-weight: $font-weight-bold;
}

.name-arabic {
  margin: 0 0 $spacing-2 0;
  opacity: 0.8;
  font-size: $font-size-lg;
}

.description {
  margin: 0 0 $spacing-3 0;
  opacity: 0.9;
  max-width: 600px;
}

.gallery-stats {
  display: flex;
  gap: $spacing-6;
  font-size: $font-size-sm;
  opacity: 0.9;

  span {
    display: flex;
    align-items: center;
    gap: $spacing-2;
  }
}

.header-actions {
  display: flex;
  gap: $spacing-2;
}

.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: $spacing-6;
  gap: $spacing-4;
  flex-wrap: wrap;
}

.filters {
  display: flex;
  gap: $spacing-4;
  flex-wrap: wrap;
}

.search-field {
  min-width: 250px;
}

.toolbar-actions {
  display: flex;
  gap: $spacing-2;
  align-items: center;
}

.media-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: $spacing-4;
}

.media-card {
  @include card-base;
  border: 2px solid $slate-200;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.2s ease;

  &:hover {
    border-color: $intalio-teal-500;
  }

  &.selected {
    border-color: $intalio-teal-500;
    box-shadow: 0 0 0 2px rgba($intalio-teal-500, 0.2);
  }
}

.media-thumb {
  position: relative;
  height: 150px;
  background: $slate-100;

  img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
}

.thumb-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2.5rem;

  &.image { color: $info-500; }
  &.video { color: $warning-500; }
  &.audio { color: $intalio-teal-500; }
}

.media-overlay {
  position: absolute;
  inset: 0;
  opacity: 0;
  transition: opacity 0.2s ease;
  background: rgba(0, 0, 0, 0.3);
}

.media-card:hover .media-overlay,
.media-card.selected .media-overlay {
  opacity: 1;
}

.selection-checkbox {
  position: absolute;
  top: $spacing-2;
  inset-inline-start: $spacing-2;
}

.card-action-btn {
  @include card-action-btn;
  position: absolute;
  top: $spacing-2;
  inset-inline-end: $spacing-2;
}

.action-menu {
  @include action-menu-popup;
}

.duration-badge {
  position: absolute;
  bottom: $spacing-2;
  inset-inline-end: $spacing-2;
  background: rgba(0, 0, 0, 0.7);
  color: white;
  padding: $spacing-1 $spacing-2;
  border-radius: $radius-sm;
  font-size: $font-size-xs;
  display: flex;
  align-items: center;
  gap: $spacing-1;
}

.featured-badge {
  position: absolute;
  top: $spacing-2;
  inset-inline-end: $spacing-2;
}

.media-info {
  padding: $spacing-3;
}

.file-name {
  display: block;
  font-size: $font-size-sm;
  font-weight: $font-weight-medium;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  margin-bottom: $spacing-1;
  color: $slate-900;
}

.media-meta {
  display: flex;
  gap: $spacing-3;
  font-size: $font-size-xs;
  color: $slate-500;
}

.media-list {
  @include card-base;
}

.media-name-cell {
  display: flex;
  align-items: center;
  gap: $spacing-3;
}

.media-thumb-small {
  width: 40px;
  height: 40px;
  border-radius: $radius-md;
  background: $slate-100;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;

  img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
}

.media-name-info {
  display: flex;
  flex-direction: column;

  .name {
    font-weight: $font-weight-medium;
    color: $slate-900;
  }

  .original-name {
    font-size: $font-size-xs;
    color: $slate-500;
  }
}

.action-buttons {
  display: flex;
  gap: $spacing-1;
}

.empty-state {
  text-align: center;
  padding: $spacing-16 $spacing-8;
  @include card-base;

  i {
    font-size: 4rem;
    color: $slate-400;
    margin-bottom: $spacing-4;
  }

  h3 {
    margin: 0 0 $spacing-2 0;
    color: $slate-900;
  }

  p {
    margin: 0 0 $spacing-4 0;
    color: $slate-500;
  }
}

.media-viewer {
  display: grid;
  grid-template-columns: 1fr 300px;
  gap: $spacing-6;
  min-height: 400px;

  @media (max-width: 768px) {
    grid-template-columns: 1fr;
  }
}

.viewer-main {
  display: flex;
  align-items: center;
  justify-content: center;
  background: $slate-100;
  border-radius: $radius-lg;
  overflow: hidden;

  img {
    max-width: 100%;
    max-height: 60vh;
    object-fit: contain;
  }
}

.video-player,
.audio-player {
  width: 100%;
  max-height: 60vh;
}

.viewer-sidebar {
  display: flex;
  flex-direction: column;
  gap: $spacing-6;
}

.info-section {
  h4 {
    margin: 0 0 $spacing-3 0;
    font-size: $font-size-sm;
    text-transform: uppercase;
    color: $slate-500;
    font-weight: $font-weight-semibold;
  }
}

.info-grid {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;
}

.info-item {
  display: flex;
  justify-content: space-between;

  .label {
    color: $slate-500;
  }

  .value {
    font-weight: $font-weight-medium;
    color: $slate-900;
  }
}

.tags-list {
  display: flex;
  flex-wrap: wrap;
  gap: $spacing-2;
}

.viewer-actions {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;
  margin-top: auto;
}

.w-full {
  width: 100%;
}

.upload-zone {
  width: 100%;
}

.upload-empty {
  text-align: center;
  padding: $spacing-12;
  color: $slate-500;

  i {
    font-size: 3rem;
    margin-bottom: $spacing-4;
  }

  p {
    margin: 0 0 $spacing-2 0;
  }

  span {
    font-size: $font-size-sm;
  }
}

// Responsive adjustments
@media (max-width: 768px) {
  .gallery-header {
    margin: (-$spacing-4) (-$spacing-4) $spacing-4 (-$spacing-4);
  }

  .header-overlay {
    padding: $spacing-4;
  }

  .header-content {
    flex-direction: column;
    align-items: flex-start;
  }

  .gallery-stats {
    flex-wrap: wrap;
    gap: $spacing-3;
  }

  .toolbar {
    flex-direction: column;
    align-items: stretch;
  }

  .filters {
    flex-direction: column;
  }

  .search-field {
    min-width: 100%;
  }
}
</style>
