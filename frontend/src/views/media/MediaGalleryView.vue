<template>
  <div class="media-gallery-view" :class="{ 'rtl': isRTL }">
    <!-- Error State -->
    <ErrorState
      v-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل معرض الوسائط' : 'Failed to load media gallery'"
      show-retry
      size="lg"
      @retry="handleRetry"
    />

    <template v-else>
    <!-- Page Header -->
    <PageHeader
      :title="t('media.gallery')"
      :description="t('media.galleryDescription')"
      :breadcrumbs="breadcrumbs"
      padding-bottom="xl"
      background-variant="gradient"
      :animated="shouldAnimate"
    >
      <template #actions>
        <Button
          :label="t('media.createGallery')"
          icon="pi pi-plus"
          severity="secondary"
          @click="showCreateGalleryDialog = true"
        />
        <Button
          :label="t('media.upload')"
          icon="pi pi-upload"
          @click="showUploadDialog = true"
        />
      </template>
    </PageHeader>

    <!-- Stats Bar -->
    <StatsBar
      :stats="statsBarItems"
      :loading="false"
      overlap="lg"
      :animated="shouldAnimate"
      :animate-numbers="shouldAnimate"
      :counter-duration="1200"
    />

    <!-- Main Content -->
    <div class="main-content">
      <!-- Filters and View Toggle -->
      <div class="toolbar">
        <div class="filters">
          <IconField class="search-field">
            <InputIcon class="pi pi-search" />
            <InputText
              v-model="filters.search"
              :placeholder="$t('media.searchGalleries')"
            />
          </IconField>
          <Select
            v-model="filters.type"
            :options="galleryTypes"
            optionLabel="label"
            optionValue="value"
            :placeholder="$t('media.galleryType')"
            showClear
            class="type-filter"
          />
        </div>
        <div class="view-toggle">
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
        </div>
      </div>

      <!-- Gallery Grid View -->
      <div
        v-if="viewMode === 'grid'"
        class="galleries-grid"
        :class="{
          'galleries-grid--animated': shouldAnimate,
          'galleries-grid--visible': isContentVisible
        }"
      >
        <div
          v-for="(gallery, index) in galleries"
          :key="gallery.id"
          class="gallery-card"
          :style="shouldAnimate ? { '--card-index': index } : undefined"
          @click="goToGallery(gallery.id)"
        >
          <div class="gallery-cover">
            <img
              v-if="gallery.coverImageUrl"
              :src="gallery.coverImageUrl"
              :alt="gallery.name"
            />
            <div v-else class="cover-placeholder">
              <i class="pi pi-images"></i>
            </div>
            <div class="gallery-preview">
              <img
                v-for="(thumb, index) in gallery.previewThumbnails.slice(0, 4)"
                :key="index"
                :src="thumb"
                :alt="`Preview ${index + 1}`"
              />
            </div>
            <Tag
              :value="gallery.type"
              :severity="getTypeSeverity(gallery.type)"
              class="type-badge"
            />
          </div>
          <div class="gallery-info">
            <h3>{{ gallery.name }}</h3>
            <div class="gallery-meta">
              <span><i class="pi pi-images"></i> {{ gallery.itemCount }} {{ $t('media.items') }}</span>
              <span><i class="pi pi-database"></i> {{ gallery.totalSizeFormatted }}</span>
            </div>
            <div class="gallery-footer">
              <span class="owner">{{ gallery.ownerName }}</span>
              <span class="date">{{ formatDate(gallery.createdAt) }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Gallery List View -->
      <div v-else class="galleries-list">
        <DataTable
          :value="galleries"
          stripedRows
          @row-click="(e) => goToGallery(e.data.id)"
          class="gallery-table"
        >
          <Column field="name" :header="$t('common.name')">
            <template #body="{ data }">
              <div class="gallery-name-cell">
                <div class="gallery-thumb">
                  <img v-if="data.coverImageUrl" :src="data.coverImageUrl" :alt="data.name" />
                  <i v-else class="pi pi-images"></i>
                </div>
                <div class="gallery-name-info">
                  <span class="name">{{ data.name }}</span>
                  <span class="name-arabic" v-if="data.nameArabic">{{ data.nameArabic }}</span>
                </div>
              </div>
            </template>
          </Column>
          <Column field="type" :header="$t('common.type')">
            <template #body="{ data }">
              <Tag :value="data.type" :severity="getTypeSeverity(data.type)" />
            </template>
          </Column>
          <Column field="itemCount" :header="$t('media.items')" />
          <Column field="totalSizeFormatted" :header="$t('media.size')" />
          <Column field="ownerName" :header="$t('common.owner')" />
          <Column field="createdAt" :header="$t('common.created')">
            <template #body="{ data }">
              {{ formatDate(data.createdAt) }}
            </template>
          </Column>
        </DataTable>
      </div>

      <!-- Empty State -->
      <div v-if="galleries.length === 0" class="empty-state">
        <i class="pi pi-images"></i>
        <h3>{{ $t('media.noGalleries') }}</h3>
        <p>{{ $t('media.createFirstGallery') }}</p>
        <Button
          :label="$t('media.createGallery')"
          icon="pi pi-plus"
          @click="showCreateGalleryDialog = true"
        />
      </div>
    </div>

    <!-- Create Gallery Dialog -->
    <Dialog
      v-model:visible="showCreateGalleryDialog"
      :header="$t('media.createGallery')"
      :modal="true"
      :style="{ width: '500px' }"
    >
      <div class="gallery-form">
        <div class="form-field">
          <label>{{ $t('media.galleryName') }} *</label>
          <InputText v-model="newGallery.name" class="w-full" />
        </div>
        <div class="form-field">
          <label>{{ $t('media.galleryNameArabic') }}</label>
          <InputText v-model="newGallery.nameArabic" class="w-full" dir="rtl" />
        </div>
        <div class="form-field">
          <label>{{ $t('common.description') }}</label>
          <Textarea v-model="newGallery.description" rows="3" class="w-full" />
        </div>
        <div class="form-row">
          <div class="form-field">
            <label>{{ $t('media.galleryType') }} *</label>
            <Select
              v-model="newGallery.type"
              :options="galleryTypes"
              optionLabel="label"
              optionValue="value"
              class="w-full"
            />
          </div>
          <div class="form-field">
            <label>{{ $t('collaboration.visibility') }} *</label>
            <Select
              v-model="newGallery.visibility"
              :options="visibilityOptions"
              optionLabel="label"
              optionValue="value"
              class="w-full"
            />
          </div>
        </div>
        <div class="form-field">
          <label>{{ $t('content.tags') }}</label>
          <Chips v-model="newGallery.tags" class="w-full" />
        </div>
      </div>
      <template #footer>
        <Button
          :label="$t('common.cancel')"
          severity="secondary"
          @click="showCreateGalleryDialog = false"
        />
        <Button
          :label="$t('common.create')"
          icon="pi pi-check"
          @click="createGallery"
        />
      </template>
    </Dialog>

    <!-- Upload Dialog -->
    <Dialog
      v-model:visible="showUploadDialog"
      :header="$t('media.uploadMedia')"
      :modal="true"
      :style="{ width: '600px' }"
    >
      <div class="upload-form">
        <div class="form-field">
          <label>{{ $t('media.selectGallery') }} *</label>
          <Select
            v-model="uploadConfig.galleryId"
            :options="galleries"
            optionLabel="name"
            optionValue="id"
            :placeholder="$t('media.selectGallery')"
            class="w-full"
          />
        </div>

        <FileUpload
          mode="advanced"
          :multiple="true"
          accept="image/*,video/*,audio/*"
          :maxFileSize="104857600"
          :auto="false"
          @select="onFilesSelected"
          class="upload-zone"
        >
          <template #header="{ chooseCallback, uploadCallback, clearCallback, files }">
            <div class="upload-header">
              <Button
                :label="$t('media.chooseFiles')"
                icon="pi pi-plus"
                @click="chooseCallback"
              />
              <Button
                :label="$t('media.uploadAll')"
                icon="pi pi-upload"
                severity="success"
                :disabled="!files || files.length === 0"
                @click="uploadCallback"
              />
              <Button
                :label="$t('common.cancel')"
                icon="pi pi-times"
                severity="secondary"
                :disabled="!files || files.length === 0"
                @click="clearCallback"
              />
            </div>
          </template>
          <template #empty>
            <div class="upload-empty">
              <i class="pi pi-cloud-upload"></i>
              <p>{{ $t('media.dragDropFiles') }}</p>
              <span>{{ $t('media.maxFileSize') }}: 100MB</span>
            </div>
          </template>
        </FileUpload>
      </div>
    </Dialog>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import { PageHeader, StatsBar } from '@/components/base'
import ErrorState from '@/components/base/ErrorState.vue'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import type { StatItem } from '@/components/base/StatsBar.vue'

const router = useRouter()
const { t, locale } = useI18n()
const prefersReducedMotion = useReducedMotion()

// RTL support
const isRTL = computed(() => locale.value === 'ar')
const isContentVisible = ref(false)
const isLoading = ref(true)
const error = ref<Error | null>(null)

// Animation control
const shouldAnimate = computed(() => !prefersReducedMotion.value)

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: t('media.gallery') }
])

const showCreateGalleryDialog = ref(false)
const showUploadDialog = ref(false)
const viewMode = ref('grid')

const viewModes = [
  { value: 'grid', icon: 'pi pi-th-large' },
  { value: 'list', icon: 'pi pi-list' }
]

const filters = reactive({
  search: '',
  type: null as string | null
})

const newGallery = ref({
  name: '',
  nameArabic: '',
  description: '',
  type: 'General',
  visibility: 'Private',
  tags: [] as string[]
})

const uploadConfig = ref({
  galleryId: null as string | null
})

const galleryTypes = [
  { label: 'General', value: 'General' },
  { label: 'Photos', value: 'Photos' },
  { label: 'Videos', value: 'Videos' },
  { label: 'Event Media', value: 'EventMedia' },
  { label: 'Project Media', value: 'ProjectMedia' },
  { label: 'Team Media', value: 'TeamMedia' }
]

const visibilityOptions = [
  { label: 'Private', value: 'Private' },
  { label: 'Team', value: 'Team' },
  { label: 'Department', value: 'Department' },
  { label: 'Organization', value: 'Organization' },
  { label: 'Public', value: 'Public' }
]

const stats = ref({
  totalImages: 2100,
  totalVideos: 890,
  totalAudio: 234,
  totalStorageFormatted: '50 GB'
})

// StatsBar items
const statsBarItems = computed<StatItem[]>(() => [
  {
    icon: 'pi pi-images',
    value: stats.value.totalImages,
    label: t('media.images'),
    labelArabic: 'الصور',
    colorClass: 'primary'
  },
  {
    icon: 'pi pi-video',
    value: stats.value.totalVideos,
    label: t('media.videos'),
    labelArabic: 'الفيديوهات',
    colorClass: 'warning'
  },
  {
    icon: 'pi pi-volume-up',
    value: stats.value.totalAudio,
    label: t('media.audio'),
    labelArabic: 'الصوتيات',
    colorClass: 'info'
  },
  {
    icon: 'pi pi-database',
    value: stats.value.totalStorageFormatted,
    label: t('media.storage'),
    labelArabic: 'التخزين',
    colorClass: 'success'
  }
])

const galleries = ref([
  {
    id: '1',
    name: 'Opening Ceremony Photos',
    nameArabic: 'صور حفل الافتتاح',
    type: 'Photos',
    visibility: 'Organization',
    coverImageUrl: '/media/galleries/opening/cover.jpg',
    itemCount: 156,
    totalSizeFormatted: '500 MB',
    ownerName: 'Mohammed Al-Rashid',
    previewThumbnails: ['/media/thumb1.jpg', '/media/thumb2.jpg', '/media/thumb3.jpg', '/media/thumb4.jpg'],
    createdAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '2',
    name: 'Stadium Construction Progress',
    nameArabic: 'تقدم بناء الملعب',
    type: 'Videos',
    visibility: 'Team',
    coverImageUrl: '/media/galleries/construction/cover.jpg',
    itemCount: 42,
    totalSizeFormatted: '2 GB',
    ownerName: 'Sara Ali',
    previewThumbnails: ['/media/thumb5.jpg', '/media/thumb6.jpg', '/media/thumb7.jpg'],
    createdAt: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '3',
    name: 'Team Photos 2024',
    nameArabic: 'صور الفريق 2024',
    type: 'TeamMedia',
    visibility: 'Organization',
    coverImageUrl: '/media/galleries/team/cover.jpg',
    itemCount: 89,
    totalSizeFormatted: '300 MB',
    ownerName: 'Ahmed Hassan',
    previewThumbnails: ['/media/thumb8.jpg', '/media/thumb9.jpg'],
    createdAt: new Date(Date.now() - 15 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '4',
    name: 'Press Conference Media',
    nameArabic: 'وسائط المؤتمر الصحفي',
    type: 'EventMedia',
    visibility: 'Public',
    coverImageUrl: '/media/galleries/press/cover.jpg',
    itemCount: 234,
    totalSizeFormatted: '1 GB',
    ownerName: 'Fatima Khan',
    previewThumbnails: ['/media/thumb10.jpg', '/media/thumb11.jpg', '/media/thumb12.jpg', '/media/thumb13.jpg'],
    createdAt: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString()
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

function goToGallery(id: string) {
  router.push({ name: 'media-gallery-detail', params: { id } })
}

function createGallery() {
  console.log('Creating gallery:', newGallery.value)
  showCreateGalleryDialog.value = false
  newGallery.value = {
    name: '',
    nameArabic: '',
    description: '',
    type: 'General',
    visibility: 'Private',
    tags: []
  }
}

function onFilesSelected(event: any) {
  console.log('Files selected:', event.files)
}

// Data loading with error handling
async function loadGalleries() {
  try {
    error.value = null
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 600))
    // In real implementation: fetch galleries from API
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
  loadGalleries()
}

onMounted(() => {
  loadGalleries()
})
</script>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

// Page Container
.media-gallery-view {
  @include page-view;

  &.rtl {
    direction: rtl;
  }
}

// Main Content
.main-content {
  @include page-content;
}

// Toolbar
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
  flex: 1;
}

.search-field {
  min-width: 300px;

  @include mobile {
    min-width: 100%;
  }
}

.type-filter {
  min-width: 150px;
}

// Gallery Grid
.galleries-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: $spacing-6;
}

.gallery-card {
  @include card-base;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.2s;

  &:hover {
    border-color: $intalio-teal-500;
    box-shadow: $shadow-elevated-md;
    transform: translateY(-2px);
  }
}

.gallery-cover {
  position: relative;
  height: 180px;
  background: $slate-100;
  overflow: hidden;

  img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
}

.cover-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 3rem;
  color: $slate-400;
}

.gallery-preview {
  position: absolute;
  bottom: $spacing-2;
  left: $spacing-2;
  right: $spacing-2;
  display: flex;
  gap: $spacing-1;

  img {
    width: 40px;
    height: 40px;
    border-radius: $radius-sm;
    object-fit: cover;
    border: 2px solid white;
    box-shadow: $shadow-sm;
  }
}

.type-badge {
  position: absolute;
  top: $spacing-2;
  right: $spacing-2;
}

.gallery-info {
  padding: $spacing-4;

  h3 {
    margin: 0 0 $spacing-2 0;
    font-size: $font-size-base;
    font-weight: $font-weight-semibold;
    color: $slate-900;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
}

.gallery-meta {
  display: flex;
  gap: $spacing-4;
  margin-bottom: $spacing-3;
  font-size: $font-size-xs;
  color: $slate-500;

  span {
    display: flex;
    align-items: center;
    gap: $spacing-1;
  }
}

.gallery-footer {
  display: flex;
  justify-content: space-between;
  font-size: $font-size-xs;
  color: $slate-500;
  padding-top: $spacing-3;
  border-top: 1px solid $slate-200;
}

// Gallery List
.galleries-list {
  @include card-base;
  overflow: hidden;
}

.gallery-table :deep(.p-datatable-tbody > tr) {
  cursor: pointer;
}

.gallery-name-cell {
  display: flex;
  align-items: center;
  gap: $spacing-3;
}

.gallery-thumb {
  width: 48px;
  height: 48px;
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

  i {
    font-size: 1.25rem;
    color: $slate-400;
  }
}

.gallery-name-info {
  display: flex;
  flex-direction: column;

  .name {
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }

  .name-arabic {
    font-size: $font-size-sm;
    color: $slate-500;
  }
}

// Empty State
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
    margin: 0 0 $spacing-6 0;
    color: $slate-500;
  }
}

// Forms
.gallery-form,
.upload-form {
  display: flex;
  flex-direction: column;
  gap: $spacing-4;
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;

  label {
    font-weight: $font-weight-medium;
    font-size: $font-size-sm;
    color: $slate-700;
  }
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: $spacing-4;

  @include mobile {
    grid-template-columns: 1fr;
  }
}

.w-full {
  width: 100%;
}

// Upload
.upload-zone {
  width: 100%;
}

.upload-header {
  display: flex;
  gap: $spacing-2;
  margin-bottom: $spacing-4;
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
    font-size: $font-size-lg;
  }

  span {
    font-size: $font-size-sm;
  }
}

// ============================================
// MASONRY LOAD ANIMATION
// ============================================
@keyframes galleryCardFadeIn {
  from {
    opacity: 0;
    transform: translateY(30px) scale(0.94);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.galleries-grid {
  // Animation state - cards hidden initially
  &--animated {
    .gallery-card {
      opacity: 0;
      transform: translateY(30px) scale(0.94);
    }

    // When visible, animate cards in with stagger
    &.galleries-grid--visible {
      .gallery-card {
        animation: galleryCardFadeIn 0.5s ease-out forwards;
        animation-delay: calc(var(--card-index, 0) * 100ms);
      }
    }
  }

  // Ensure cards without animation are visible
  &:not(.galleries-grid--animated) {
    .gallery-card {
      opacity: 1;
      transform: none;
      animation: none;
    }
  }
}

// ============================================
// ENHANCED CARD HOVER EFFECTS
// ============================================
.gallery-card {
  &:hover {
    box-shadow:
      0 20px 40px -10px rgba(0, 0, 0, 0.15),
      0 10px 20px -5px rgba(0, 0, 0, 0.1);
    transform: translateY(-6px) scale(1.02);
    border-color: $intalio-teal-400;

    .gallery-cover img {
      transform: scale(1.08);
    }

    .gallery-preview img {
      transform: scale(1.05);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
    }

    .gallery-info h3 {
      color: $intalio-teal-600;
    }
  }

  &:active {
    transform: translateY(-4px) scale(1.01);
    box-shadow:
      0 10px 20px -5px rgba(0, 0, 0, 0.12),
      0 5px 10px -3px rgba(0, 0, 0, 0.08);
  }

  .gallery-cover img {
    transition: transform 0.4s ease-out;
  }

  .gallery-preview img {
    transition: all 0.3s ease-out;
  }

  .gallery-info h3 {
    transition: color 0.2s ease;
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================
@media (prefers-reduced-motion: reduce) {
  .galleries-grid {
    &--animated {
      .gallery-card {
        opacity: 1;
        transform: none;
        animation: none !important;
      }
    }
  }

  .gallery-card {
    animation: none !important;
    transition: background-color 0.15s, box-shadow 0.15s, border-color 0.15s;

    &:hover {
      transform: none;

      .gallery-cover img {
        transform: none;
      }

      .gallery-preview img {
        transform: none;
      }
    }

    &:active {
      transform: none;
    }

    .gallery-cover img,
    .gallery-preview img {
      transition: none;
    }
  }
}
</style>
